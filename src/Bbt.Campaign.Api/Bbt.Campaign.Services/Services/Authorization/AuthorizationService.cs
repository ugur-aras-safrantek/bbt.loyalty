using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Models.Authorization;
using Bbt.Campaign.Public.Models.Parameter;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.CacheKey;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        public async Task<BaseResponse<UserAuthorizationResponseDto>> LoginAsync(string code, string state) 
        {
            UserAuthorizationResponseDto response = new UserAuthorizationResponseDto();

            response.Code = code;   
            response.State = state;

            //UserModelDto userModel = await GetUserRoles(code, state);
            //var result = new GetSearchPersonSummaryDto
            //{
            //    CitizenshipNumber = response.Tckn
            //};

            UserModelDto userModel = new UserModelDto();
            userModel.Authority = new AuthorityModel();
            userModel.UserId = "11";
            userModel.Authority.IsLoyaltyCreator = false;
            userModel.Authority.IsLoyaltyApprover = false;
            userModel.Authority.IsLoyaltyReader = true;
            userModel.Authority.IsLoyaltyRuleCreator = true;
            userModel.Authority.IsLoyaltyRuleApprover = true;

            //set user roles
            List<UserRoleDto> userRoleList = new List<UserRoleDto>();
            if (userModel.Authority.IsLoyaltyCreator)
                userRoleList.Add(new UserRoleDto() { Id = 1, UserId = userModel.UserId, RoleTypeId = (int)RoleTypeEnum.IsLoyaltyCreator });
            if (userModel.Authority.IsLoyaltyApprover)
                userRoleList.Add(new UserRoleDto() { Id = 2, UserId = userModel.UserId, RoleTypeId = (int)RoleTypeEnum.IsLoyaltyApprover });
            if (userModel.Authority.IsLoyaltyReader)
                userRoleList.Add(new UserRoleDto() { Id = 3, UserId = userModel.UserId, RoleTypeId = (int)RoleTypeEnum.IsLoyaltyReader });
            if (userModel.Authority.IsLoyaltyRuleCreator)
                userRoleList.Add(new UserRoleDto() { Id = 4, UserId = userModel.UserId, RoleTypeId = (int)RoleTypeEnum.IsLoyaltyRuleCreator });
            if (userModel.Authority.IsLoyaltyRuleApprover)
                userRoleList.Add(new UserRoleDto() { Id = 5, UserId = userModel.UserId, RoleTypeId = (int)RoleTypeEnum.IsLoyaltyRuleApprover });

            if(!userRoleList.Any())
                throw new Exception("Bu kullanıcı için tanımlı bir yetki bulunamadı.");

            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync())?.Data;
            if (roleAuthorizationList == null || !roleAuthorizationList.Any())
                throw new Exception("Rol tanımları bulunamadı.");

            roleAuthorizationList = roleAuthorizationList.Where(x => userRoleList.Any(p2 => p2.RoleTypeId == x.RoleTypeId)).ToList();
            var moduleTypeList = roleAuthorizationList.Select(x => x.ModuleTypeId).Distinct().ToList();
            List<UserAuthorizationDto> userAuthorizationList = new List<UserAuthorizationDto>();
            foreach (int moduleTypeId in moduleTypeList)
            {
                UserAuthorizationDto userAuthorizationDto = new UserAuthorizationDto();
                List<int> authorizationList = new List<int>();
                foreach (var roleAuthorization in roleAuthorizationList.Where(x => x.ModuleTypeId == moduleTypeId))
                    authorizationList.Add(roleAuthorization.AuthorizationTypeId);

                userAuthorizationDto.ModuleId = moduleTypeId;
                userAuthorizationDto.AuthorizationList = authorizationList;
                userAuthorizationList.Add(userAuthorizationDto);
            }
            response.AuthorizationList = userAuthorizationList;

            Token token = await CreateAccessToken(userModel);
            response.AccessToken = token.AccessToken;

            //await UpdateUserProcessDate(userModel.UserId);

            return await BaseResponse<UserAuthorizationResponseDto>.SuccessAsync(response);
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
            //DateTime lastProcessDate = userRoleList[0].LastProcessDate;
            //if(lastProcessDate.AddMinutes(StaticValues.SessionTimeout) < DateTime.Now) 
            //    throw new Exception(StaticFormValues.SessionTimeoutAlert);

            //modul ve işlem bazlı sorgulama
            List<RoleAuthorizationDto> userRoleAuthorizationList = roleAuthorizationList
                .Where(x => userRoleList.Any(p2 => p2.RoleTypeId == x.RoleTypeId) 
                                                && x.ModuleTypeId == moduleTypeId && x.AuthorizationTypeId == authorizationTypeId)
                .ToList();
            if (!userRoleAuthorizationList.Any())
                throw new Exception(StaticFormValues.UnAuthorizedUserAlert);

            await UpdateUserProcessDate(userId);
        }


        public async Task CheckAuthorizationAsync2(UserRoleDto2 userRoleDto, int moduleTypeId, int authorizationTypeId)
        {
            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync())?.Data;
            if (roleAuthorizationList == null || !roleAuthorizationList.Any())
                throw new Exception("Rol tanımları bulunamadı.");

            // kullanıcı yetkileri
            //var userRoleList = (await _parameterService.GetUserRoleListAsync(userRoleDto.UserId))?.Data;
            //if (userRoleList == null || !userRoleList.Any())
            //    throw new Exception(StaticFormValues.UnAuthorizedUserAlert);

            //session timeout kontrolu
            //DateTime lastProcessDate = userRoleList[0].LastProcessDate;
            //if(lastProcessDate.AddMinutes(StaticValues.SessionTimeout) < DateTime.Now) 
            //    throw new Exception(StaticFormValues.SessionTimeoutAlert);

            //modul ve işlem bazlı sorgulama
            List<RoleAuthorizationDto> userRoleAuthorizationList = roleAuthorizationList
                .Where(x => userRoleDto.RoleTypeIdList.Any(p2 => p2 == x.RoleTypeId)
                                                && x.ModuleTypeId == moduleTypeId && x.AuthorizationTypeId == authorizationTypeId)
                .ToList();
            if (!userRoleAuthorizationList.Any())
                throw new Exception(StaticFormValues.UnAuthorizedUserAlert);

            //await UpdateUserProcessDate(userId);
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

        private async Task<UserModelDto> GetUserRoles(string code, string state)
        {
            UserModelDto userModel;
            string accessToken = string.Empty;

            if (state == "LoyaltyGondor")
            {
                using (var client = new HttpClient())
                {
                    string baseAddress = await _parameterService.GetServiceConstantValue("AccessTokenBaseAddress");
                    string apiAddress = await _parameterService.GetServiceConstantValue("AccessTokenApiAddress");
                    client.BaseAddress = new Uri(baseAddress);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("client_id", await _parameterService.GetServiceConstantValue("client_id")),
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("client_secret", await _parameterService.GetServiceConstantValue("client_secret")),
                        new KeyValuePair<string, string>("redirect_uri", await _parameterService.GetServiceConstantValue("redirect_uri")),
                    });

                    var result = await client.PostAsync(apiAddress, content);
                    var responseContent = result.Content.ReadAsStringAsync().Result;
                    AccessToken token = JsonConvert.DeserializeObject<AccessToken>(result.Content.ReadAsStringAsync().Result);
                    if (token.Access_token == null)
                        throw new Exception("Token servisinden veri çekilirken hata alındı.");
                    accessToken = token.Access_token;
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://gondor-apigateway.burgan.com.tr");
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("access_token", accessToken),
                    });
                    var result = await client.PostAsync(await _parameterService.GetServiceConstantValue("ResourceApiAddress"), content);

                    userModel = JsonConvert.DeserializeObject<UserModelDto>(result.Content.ReadAsStringAsync().Result);
                }
            }
            else { throw new Exception("Invalid state."); }
            return userModel;
        }

        public async Task<Token> CreateAccessToken(UserModelDto user)
        {
            var tokenInstance = new Token();
            var claims = new Claim[]{
                new Claim(JwtRegisteredClaimNames.NameId,Guid.NewGuid().ToString()),
                new Claim("UserId",user.UserId.ToString()),
                new Claim("IsLoyaltyCreator",user.Authority.IsLoyaltyCreator.ToString()),
                new Claim("IsLoyaltyApprover",user.Authority.IsLoyaltyApprover.ToString()),
                new Claim("IsLoyaltyReader",user.Authority.IsLoyaltyReader.ToString()),
                new Claim("IsLoyaltyRuleCreator",user.Authority.IsLoyaltyRuleCreator.ToString()),
                new Claim("IsLoyaltyRuleApprover",user.Authority.IsLoyaltyRuleApprover.ToString())
            };

            //Security  Key'in simetriğini alıyoruz.
            SymmetricSecurityKey securityKey = 
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(StaticValues.SecurityKey));

            //Şifrelenmiş kimliği oluşturuyoruz.
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Oluşturulacak token ayarlarını veriyoruz.
            tokenInstance.Expiration = DateTime.Now.AddMinutes(60);
            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: StaticValues.Issuer,
                audience: StaticValues.Audience,
                expires: tokenInstance.Expiration,//Token süresini 5 dk olarak belirliyorum
                notBefore: DateTime.Now,//Token üretildikten ne kadar süre sonra devreye girsin ayarlıyouz.
                signingCredentials: signingCredentials,
                claims: claims
                );

            //Token oluşturucu sınıfında bir örnek alıyoruz.
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            //Token üretiyoruz.
            tokenInstance.AccessToken = tokenHandler.WriteToken(securityToken);

            //Refresh Token üretiyoruz.
            tokenInstance.RefreshToken = CreateRefreshToken();
            return tokenInstance;
        }

        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(number);
                return Convert.ToBase64String(number);
            }
        }

        public async Task CheckAuthorizationAsync2(string userId, int moduleTypeId, int authorizationTypeId) 
        {
            //var x =  System.Security.Claims.ClaimsPrincipal.Current.Claims.Where(x=>x.) 


            //if (User.Claims.Count() == 0)
            //    throw new Exception("");
        }

        private enum RoleTypeEnum
        {
            [Description("isLoyaltyCreator")]
            IsLoyaltyCreator = 1,
            [Description("isLoyaltyApprover")]
            IsLoyaltyApprover = 2,
            [Description("isLoyaltyReader")]
            IsLoyaltyReader = 3,
            [Description("isLoyaltyRuleCreator")]
            IsLoyaltyRuleCreator = 4,
            [Description("isLoyaltyRuleApprover")]
            IsLoyaltyRuleApprover = 5
        }


    }
}
