using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Approval;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;
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
using System.Globalization;
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
            UserModelDto2 userModel2 = new UserModelDto2();
            UserRoleDto userRoleDto = new UserRoleDto();
            List<int> roleTypeIdList = new List<int>();
            UserModelDto userModel;

            try 
            {
                response.Code = code;
                response.State = state;

                if (StaticValues.IsDevelopment)
                {
                    userModel2.Tckn = code;
                    userRoleDto.UserId = code;

                    foreach (var item in _unitOfWork.GetRepository<UserRoleEntity>().GetAll(x => x.UserId == code).ToList())
                    {
                        switch (item.RoleTypeId)
                        {
                            case (int)RoleTypeEnum.IsLoyaltyCreator:
                                userModel2.Credentials.IsLoyaltyCreator = true;
                                break;
                            case (int)RoleTypeEnum.IsLoyaltyRuleCreator:
                                userModel2.Credentials.IsLoyaltyRuleCreator = true;
                                break;
                            case (int)RoleTypeEnum.IsLoyaltyRuleApprover:
                                userModel2.Credentials.IsLoyaltyRuleApprover = true;
                                break;
                            case (int)RoleTypeEnum.IsLoyaltyApprover:
                                userModel2.Credentials.IsLoyaltyApprover = true;
                                break;
                            case (int)RoleTypeEnum.IsLoyaltyReader:
                                userModel2.Credentials.IsLoyaltyReader = true;
                                break;
                            default:
                                break;

                        }
                        roleTypeIdList.Add(item.RoleTypeId);
                    }
                }
                else
                {
                    userModel = await GetUserRoles(code, state);

                    //UserModelDto userModel = new UserModelDto();
                    //userModel.Tckn = "11701604572";
                    //userModel.Credentials = new List<string>() { "isLoyaltyCreator###0", "isLoyaltyRuleCreator###1","isLoyaltyRuleApprover###1", "isLoyaltyApprover###1", "isLoyaltyReader###1"};

                    userModel2.Tckn = userModel.Tckn;
                    userRoleDto.UserId = userModel.Tckn;

                    foreach (string credential in userModel.Credentials)
                    {
                        string[] credentialArray = credential.Split("###");
                        string credentialName = credentialArray[0];
                        string credentialValue = credentialArray[1];
                        if (!string.IsNullOrEmpty(credentialValue) && credentialValue == "1")
                        {
                            switch (credentialName)
                            {
                                case "isLoyaltyCreator":
                                    roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyCreator);
                                    userModel2.Credentials.IsLoyaltyCreator = true;
                                    break;
                                case "isLoyaltyRuleCreator":
                                    roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyRuleCreator);
                                    userModel2.Credentials.IsLoyaltyRuleCreator = true;
                                    break;
                                case "isLoyaltyRuleApprover":
                                    roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyRuleApprover);
                                    userModel2.Credentials.IsLoyaltyRuleApprover = true;
                                    break;
                                case "isLoyaltyApprover":
                                    roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyApprover);
                                    userModel2.Credentials.IsLoyaltyApprover = true;
                                    break;
                                case "isLoyaltyReader":
                                    roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyReader);
                                    userModel2.Credentials.IsLoyaltyReader = true;
                                    break;
                            }
                        }
                    }
                }

                if (!roleTypeIdList.Any())
                    throw new Exception("Bu kullanıcı için tanımlı bir yetki bulunamadı.");

                List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync())?.Data;
                if (roleAuthorizationList == null || !roleAuthorizationList.Any())
                    throw new Exception("Rol tanımları bulunamadı.");

                roleAuthorizationList = roleAuthorizationList.Where(x => roleTypeIdList.Any(p2 => p2 == x.RoleTypeId)).ToList();
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

                Token token = await CreateAccessToken(userModel2);
                response.AccessToken = token.AccessToken;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());

            }

            

            //await _parameterService.SetUserLastProcessDate(userModel2.Tckn);

            return await BaseResponse<UserAuthorizationResponseDto>.SuccessAsync(response);
        }
  
        public async Task CheckAuthorizationAsync(UserRoleDto userRoleDto, int moduleTypeId, int authorizationTypeId)
        {
            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync())?.Data;
            if (roleAuthorizationList == null || !roleAuthorizationList.Any())
                throw new Exception("Rol tanımları bulunamadı.");

            // kullanıcı yetkileri
            //var userRoleList = (await _parameterService.GetUserRoleListAsync(userRoleDto.UserId))?.Data;
            //if (userRoleList == null || !userRoleList.Any())
            //    throw new Exception(StaticFormValues.UnAuthorizedUserAlert);

            //session timeout kontrolu
            //string _lastProcessDate = await _parameterService.GetUserLastProcessDate(userRoleDto.UserId);
            //if(string.IsNullOrEmpty(_lastProcessDate))
            //    throw new Exception(StaticFormValues.SessionTimeoutAlert);
            //DateTime lastProcessDate = DateTime.ParseExact(_lastProcessDate, Helpers.dateTimeFormat, CultureInfo.InvariantCulture);
            //if (lastProcessDate.AddMinutes(StaticValues.SessionTimeout) < DateTime.Now)
            //    throw new Exception(StaticFormValues.SessionTimeoutAlert);
            //await _parameterService.SetUserLastProcessDate(userRoleDto.UserId);

            //modul ve işlem bazlı sorgulama
            List<RoleAuthorizationDto> userRoleAuthorizationList = roleAuthorizationList
                .Where(x => userRoleDto.RoleTypeIdList.Any(p2 => p2 == x.RoleTypeId)
                                                && x.ModuleTypeId == moduleTypeId && x.AuthorizationTypeId == authorizationTypeId)
                .ToList();
            if (!userRoleAuthorizationList.Any())
                throw new Exception(StaticFormValues.UnAuthorizedUserAlert);
        }

        public async Task<BaseResponse<SuccessDto>> UpdateUserRolesAsync(string userId, string userRoles)
        {
            var response = new SuccessDto();
            List<RoleAuthorizationDto> roleAuthorizationList = (await _parameterService.GetRoleAuthorizationListAsync()).Data;
            List<ParameterDto> roleTypeList = (await _parameterService.GetRoleTypeListAsync()).Data;

            foreach (var itemRemove in _unitOfWork.GetRepository<UserRoleEntity>().GetAll(x => x.UserId == userId).ToList())
                await _unitOfWork.GetRepository<UserRoleEntity>().DeleteAsync(itemRemove);

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

            response.IsSuccess = true;

            return await BaseResponse<SuccessDto>.SuccessAsync(response);
        }









        private async Task UpdateUserProcessDate(string userId)
        {
            //DateTime now = DateTime.Now;
            //List<UserRoleDto> userRoleList = new List<UserRoleDto>();
            //foreach (var item in _unitOfWork.GetRepository<UserRoleEntity>().GetAll(x => x.UserId == userId).ToList())
            //{
            //    item.LastProcessDate = now;
            //    await _unitOfWork.GetRepository<UserRoleEntity>().UpdateAsync(item);
            //    userRoleList.Add(new UserRoleDto() { Id = item.Id, RoleTypeId = item.RoleTypeId, LastProcessDate = item.LastProcessDate });
            //}
            //await _unitOfWork.SaveChangesAsync();
            //await _parameterService.SetUserRoleListAsync(userId, userRoleList);
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
                        throw new Exception(responseContent);
                    accessToken = token.Access_token;
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(await _parameterService.GetServiceConstantValue("BaseAddress"));
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("access_token", accessToken),
                    });
                    var result = await client.PostAsync(await _parameterService.GetServiceConstantValue("ResourceApiAddress"), content);

                    userModel = JsonConvert.DeserializeObject<UserModelDto>(result.Content.ReadAsStringAsync().Result);

                    if(userModel == null)
                        throw new Exception("Kullanıcı rolleri bulunamadı.");

                    if(userModel.Credentials == null || !userModel.Credentials.Any())
                        throw new Exception("Kullanıcı rolleri bulunamadı.");

                }
            }
            else { throw new Exception("Invalid state."); }
            return userModel;
        }

        public async Task<Token> CreateAccessToken(UserModelDto2 user)
        {
            var tokenInstance = new Token();
            var claims = new Claim[]{
                new Claim(JwtRegisteredClaimNames.NameId,Guid.NewGuid().ToString()),
                new Claim("UserId",user.Tckn.ToString()),
                new Claim("IsLoyaltyCreator",user.Credentials.IsLoyaltyCreator.ToString()),
                new Claim("IsLoyaltyApprover",user.Credentials.IsLoyaltyApprover.ToString()),
                new Claim("IsLoyaltyReader",user.Credentials.IsLoyaltyReader.ToString()),
                new Claim("IsLoyaltyRuleCreator",user.Credentials.IsLoyaltyRuleCreator.ToString()),
                new Claim("IsLoyaltyRuleApprover",user.Credentials.IsLoyaltyRuleApprover.ToString())
            };

            //Security  Key'in simetriğini alıyoruz.
            SymmetricSecurityKey securityKey = 
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VS7djKZZ79RvVXJgH7RsefzvqtbqsFqLxbCzjwLvtqj4Jq2fJzZ7UMrERz8XtEAE"));

            //Şifrelenmiş kimliği oluşturuyoruz.
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Oluşturulacak token ayarlarını veriyoruz.
            tokenInstance.Expiration = DateTime.Now.AddMinutes(300);
            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: "Issuer",
                audience: "Audience",
                expires: tokenInstance.Expiration,
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

    }
}
