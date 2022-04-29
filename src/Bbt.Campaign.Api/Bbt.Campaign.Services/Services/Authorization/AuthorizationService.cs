using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Models.Authorization;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;

namespace Bbt.Campaign.Services.Services.Authorization
{
    public class AuthorizationService : IAuthorizationservice, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;

        public AuthorizationService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
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
            //List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync()).Data;
            //List<ParameterDto> roleTypeList = (await _parameterService.GetRoleTypeListAsync()).Data;
            //List<int> userRoleList = new List<int>();


            //if(!userRoleList.Any())
            //    throw new Exception("Kullanıcı rollerini bulunamadı.");

            //



            //foreach(int roleTypeId in userRoleList) 
            //{
            //    List<int> moduleTypeList = roleAuthorizationList.Where(x=> x.RoleTypeId == roleTypeId)
            //        .Select(x => x.ModuleTypeId).Distinct().ToList();
            //    foreach (int moduleTypeId in moduleTypeList) 
            //    {
            //        UserAuthorizationDto userAuthorizationDto = new UserAuthorizationDto();
            //        userAuthorizationDto.ModuleId = moduleTypeId;
            //        foreach (var roleAuthorization in roleAuthorizationList.Where(x => x.RoleTypeId == roleTypeId && x.ModuleTypeId == moduleTypeId))
            //            userAuthorizationDto.ProcessList.Add(roleAuthorization.ProcessTypeId);

            //        userAuthorizationList.Add(userAuthorizationDto);
            //    }
            //}

            return await BaseResponse<List<UserAuthorizationDto>>.SuccessAsync(userAuthorizationList);
        }

        public async Task<BaseResponse<List<UserRoleDto>>> UpdateUserRolesDevelopmentAsync(string userId, string userRoles) 
        {
            if (!StaticValues.IsDevelopment)
                throw new Exception("Bu işlem sadece development ortamı için kullanılabilir.");

            await UpdateUserRoles(userId, userRoles);

            List<UserRoleDto> userRoleList = new List<UserRoleDto>();

            return await BaseResponse<List<UserRoleDto>>.SuccessAsync(userRoleList);
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

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
