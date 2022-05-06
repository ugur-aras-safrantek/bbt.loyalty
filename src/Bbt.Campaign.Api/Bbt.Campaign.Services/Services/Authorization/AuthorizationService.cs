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
    public class AuthorizationService : IAuthorizationservice, IScopedService
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

            List<UserAuthorizationDto> userAuthorizationList = new List<UserAuthorizationDto>();
            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync())?.Data;
            if(roleAuthorizationList == null || !roleAuthorizationList.Any()) 
                throw new Exception("Rol tanımları bulunamadı.");
            List<ParameterDto> userRoleList = (await _parameterService.GetSingleUserRoleListAsync(request.UserId))?.Data; 
            if(userRoleList == null)
                return await BaseResponse<List<UserAuthorizationDto>>.SuccessAsync(userAuthorizationList);

            roleAuthorizationList = roleAuthorizationList.Where(x => userRoleList.Any(p2 => Int32.Parse(p2.Name) == x.RoleTypeId)).ToList();
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
        public async Task<BaseResponse<List<ParameterDto>>> UpdateUserRolesAsync(string userId, string userRoles) 
        {
            await UpdateUserRoles(userId, userRoles);

            List<ParameterDto> usersRoleList = (await _parameterService.GetSingleUserRoleListAsync(userId))?.Data;
            if (usersRoleList == null)
                usersRoleList = new List<ParameterDto>();

            return await BaseResponse<List<ParameterDto>>.SuccessAsync(usersRoleList);
        }
        private async Task UpdateUserRoles(string userId, string userRoles) 
        {
            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync()).Data;
            List<ParameterDto> roleTypeList = (await _parameterService.GetRoleTypeListAsync()).Data;
            List<ParameterDto> allUsersRoleList = (await _parameterService.GetAllUsersRoleListAsync())?.Data;
            List<int> userRoleList = new List<int>();

            if (allUsersRoleList == null)
                allUsersRoleList = new List<ParameterDto>();

            allUsersRoleList = allUsersRoleList.Where(x => x.Code != userId).ToList();

            //remove user roles in database
            foreach (var itemRemove in _unitOfWork.GetRepository<UserRoleEntity>().GetAll(x => x.UserId == userId).ToList())
            {
                await _unitOfWork.GetRepository<UserRoleEntity>().DeleteAsync(itemRemove);
            }

            List<string> userRoleStrList = userRoles.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string roleName in userRoleStrList)
            {
                var roleType = roleTypeList.Where(x => x.Name == roleName.Trim()).FirstOrDefault();
                if (roleType == null)
                    throw new Exception("Rol bilgisi hatalı.");

                await _unitOfWork.GetRepository<UserRoleEntity>().AddAsync(new UserRoleEntity() 
                { 
                    UserId = userId,
                    RoleTypeId = roleType.Id
                });

                allUsersRoleList.Add(new ParameterDto() { Id=1, Code=userId, Name= roleType.Id.ToString() });
            }

            var cacheValue = JsonConvert.SerializeObject(allUsersRoleList);
            await _redisDatabaseProvider.SetAsync(CacheKeys.AllUsersRoleList, cacheValue);

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<BaseResponse<CheckAuthorizationResponse>> CheckAuthorizationAsync(CheckAuthorizationRequest request) 
        {
            CheckAuthorizationResponse response = new CheckAuthorizationResponse();
            response.HasAuthorization = await HasAuthorization(request.UserId, request.ModuleTypeId, request.AuthorizationTypeId);
            return await BaseResponse<CheckAuthorizationResponse>.SuccessAsync(response);
        }
        public async Task<bool> HasAuthorization(string userId, int moduleTypeId, int authorizationTypeId)
        {
            bool retVal = false;
            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync())?.Data;
            if (roleAuthorizationList == null || !roleAuthorizationList.Any())
                throw new Exception("Rol tanımları bulunamadı.");

            List<ParameterDto> userRoleList = (await _parameterService.GetSingleUserRoleListAsync(userId))?.Data;
            if (userRoleList == null)
                return retVal;

            roleAuthorizationList = roleAuthorizationList
                .Where(x => userRoleList.Any(p2 => Int32.Parse(p2.Name) == x.RoleTypeId) 
                                                && x.ModuleTypeId == moduleTypeId && x.AuthorizationTypeId == authorizationTypeId)
                .ToList();
            retVal = roleAuthorizationList.Any();

            return retVal;
        }
    }
}
