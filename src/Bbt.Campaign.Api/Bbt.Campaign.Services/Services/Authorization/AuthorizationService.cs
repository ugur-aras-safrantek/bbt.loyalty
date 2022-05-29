using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Models.Authorization;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.CacheKey;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Newtonsoft.Json;

namespace Bbt.Campaign.Services.Services.Authorization
{
    public class AuthorizationService : IAuthorizationService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly IRedisDatabaseProvider _redisDatabaseProvider;

        public AuthorizationService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService,
            IRedisDatabaseProvider redisDatabaseProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _redisDatabaseProvider = redisDatabaseProvider;
        }
        public async Task<BaseResponse<List<UserAuthorizationDto>>> LoginAsync(LoginRequest request) 
        {
            if (!StaticValues.IsDevelopment) 
            {
                //servisten user roller çekilecek
                string userRoles = "";
                await UpdateUserRoles(request.UserId, userRoles);
            }

            await UpdateUserProcessDate(request.UserId);

            List<UserAuthorizationDto> userAuthorizationList = new List<UserAuthorizationDto>();
            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync())?.Data;
            if(roleAuthorizationList == null || !roleAuthorizationList.Any()) 
                throw new Exception("Rol tanımları bulunamadı.");
            List<UserRoleDto> userRoleList = (await _parameterService.GetUserRoleListAsync(request.UserId))?.Data; 
            if(userRoleList == null)
                return await BaseResponse<List<UserAuthorizationDto>>.SuccessAsync(userAuthorizationList);

            roleAuthorizationList = roleAuthorizationList.Where(x => userRoleList.Any(p2 => p2.RoleTypeId == x.RoleTypeId)).ToList();
            var moduleTypeList = roleAuthorizationList.Select(x=>x.ModuleTypeId).Distinct().ToList();
            foreach(int moduleTypeId in moduleTypeList) 
            {
                UserAuthorizationDto userAuthorizationDto = new UserAuthorizationDto();
                List<int> authorizationList = new List<int>();
                foreach(var roleAuthorization in roleAuthorizationList.Where(x=>x.ModuleTypeId == moduleTypeId)) 
                {
                    authorizationList.Add(roleAuthorization.AuthorizationTypeId);
                }
                userAuthorizationDto.ModuleId = moduleTypeId;
                userAuthorizationDto.AuthorizationList = authorizationList;
                userAuthorizationList.Add(userAuthorizationDto);
            }

            return await BaseResponse<List<UserAuthorizationDto>>.SuccessAsync(userAuthorizationList);
        }
        public async Task<BaseResponse<LogoutResponse>> LogoutAsync(LogoutRequest request) 
        {
            LogoutResponse response = new LogoutResponse();

            await UpdateUserRoles(request.UserId, string.Empty);

            return await BaseResponse<LogoutResponse>.SuccessAsync(response);
        }
        public async Task<BaseResponse<List<UserRoleDto>>> UpdateUserRolesAsync(string userId, string userRoles) 
        {
            await UpdateUserRoles(userId, userRoles);

            List<UserRoleDto> usersRoleList = (await _parameterService.GetUserRoleListAsync(userId))?.Data;
            if (usersRoleList == null)
                usersRoleList = new List<UserRoleDto>();

            return await BaseResponse<List<UserRoleDto>>.SuccessAsync(usersRoleList);
        }
        private async Task UpdateUserRoles(string userId, string userRoles) 
        {
            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync()).Data;
            List<ParameterDto> roleTypeList = (await _parameterService.GetRoleTypeListAsync()).Data;

            foreach (var itemRemove in _unitOfWork.GetRepository<UserRoleEntity>().GetAll(x => x.UserId == userId).ToList())
            {
                await _unitOfWork.GetRepository<UserRoleEntity>().DeleteAsync(itemRemove);
            }

            DateTime now = DateTime.Now;
            List<string> userRoleStrList = userRoles.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            foreach (string roleName in userRoleStrList)
            {
                var roleType = roleTypeList.Where(x => x.Name == roleName.Trim()).FirstOrDefault();
                if (roleType == null)
                    throw new Exception("Rol bilgisi hatalı.");

                await _unitOfWork.GetRepository<UserRoleEntity>().AddAsync(
                    new UserRoleEntity() { UserId = userId, RoleTypeId = roleType.Id, LastProcessDate = now, });
            }
            
            await _unitOfWork.SaveChangesAsync();

            await _parameterService.SetUserRoleListAsync(userId, new List<UserRoleDto>()); 
        }
        public async Task<BaseResponse<CheckAuthorizationResponse>> CheckAuthorizationAsync(CheckAuthorizationRequest request) 
        {
            CheckAuthorizationResponse response = new CheckAuthorizationResponse();
            await CheckAuthorizationAsync(request.UserId, request.ModuleTypeId, request.AuthorizationTypeId);
            response.HasAuthorization = true; 
            return await BaseResponse<CheckAuthorizationResponse>.SuccessAsync(response);
        }
        public async Task CheckAuthorizationAsync(string userId, int moduleTypeId, int authorizationTypeId)
        {
            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync())?.Data;
            if (roleAuthorizationList == null || !roleAuthorizationList.Any())
                throw new Exception("Rol tanımları bulunamadı.");

            // kullanıcı yetkileri
            var userRoleList = (await _parameterService.GetUserRoleListAsync(userId))?.Data;
            if (userRoleList == null || !userRoleList.Any())
                throw new Exception(StaticFormValues.UnAuthorizedUserAlert);

            //session timeout kontrolu
            DateTime lastProcessDate = userRoleList[0].LastProcessDate;
            if(lastProcessDate.AddMinutes(StaticValues.SessionTimeout) < DateTime.Now) 
                throw new Exception(StaticFormValues.SessionTimeoutAlert);

            //modul ve işlem bazlı sorgulama
            List<RoleAuthorizationDto> userRoleAuthorizationList = roleAuthorizationList
                .Where(x => userRoleList.Any(p2 => p2.RoleTypeId == x.RoleTypeId) 
                                                && x.ModuleTypeId == moduleTypeId && x.AuthorizationTypeId == authorizationTypeId)
                .ToList();
            if (!userRoleAuthorizationList.Any())
                throw new Exception(StaticFormValues.UnAuthorizedUserAlert);

            await UpdateUserProcessDate(userId);
        }
        private async Task UpdateUserProcessDate(string userId) 
        {
            DateTime now = DateTime.Now;
            List<UserRoleDto> userRoleList = new List<UserRoleDto>();
            foreach (var item in _unitOfWork.GetRepository<UserRoleEntity>().GetAll(x => x.UserId == userId).ToList())
            {
                item.LastProcessDate = now;
                await _unitOfWork.GetRepository<UserRoleEntity>().UpdateAsync(item);
                userRoleList.Add(new UserRoleDto() { Id = item.Id, RoleTypeId = item.RoleTypeId, LastProcessDate=item.LastProcessDate });
            }
            await _unitOfWork.SaveChangesAsync();
            await _parameterService.SetUserRoleListAsync(userId, userRoleList);
        }
    }
}
