using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Bbt.Campaign.Services.Services.Remote
{
    public class RemoteService : IRemoteService, IScopedService
    {
        private readonly IParameterService _parameterService;

        public RemoteService(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }
        public async Task<List<string>> GetCampaignChannelList() 
        {
            List<string> channelCodeList = new List<string>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.GetAsync(StaticValues.ChannelCodeServiceUrl);
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content != null)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        channelCodeList = JsonConvert.DeserializeObject<List<string>>((JObject.Parse(apiResponse)["data"]).ToString());
                        if (channelCodeList != null && channelCodeList.Any()) 
                        { 
                            return channelCodeList;
                        }
                        else 
                        { 
                            throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); 
                        }
                    }
                    else 
                    { 
                        throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); 
                    }
                }
                else 
                { 
                    throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); 
                }
            }
        }
        private async Task<string> GetAccessToken() 
        {
            string retVal = string.Empty;
            var serviceConstantList = (await _parameterService.GetServiceConstantListAsync())?.Data;
            string baseAddress = serviceConstantList?.Where(x => x.Code == "BaseAddress").FirstOrDefault().Name;
            string tokenAddress = serviceConstantList?.Where(x => x.Code == "Token").FirstOrDefault().Name;

            string client_id_value = serviceConstantList?.Where(x => x.Code == "client_id").FirstOrDefault().Name;
            string grant_type_value = serviceConstantList?.Where(x => x.Code == "grant_type").FirstOrDefault().Name;
            string client_secret_value = serviceConstantList?.Where(x => x.Code == "client_secret").FirstOrDefault().Name;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress); 
                var content = new FormUrlEncodedContent(new[]
                {

                        new KeyValuePair<string, string>("client_id", client_id_value),
                        new KeyValuePair<string, string>("grant_type", grant_type_value),
                        new KeyValuePair<string, string>("client_secret", client_secret_value)
                });
                var result = await client.PostAsync(tokenAddress, content);
                var responseContent = result.Content.ReadAsStringAsync().Result;
                //AccessToken token = JsonConvert.DeserializeObject<AccessToken>(result.Content.ReadAsStringAsync().Result);
                //accessToken = token.access_token;
                return "";
            }

        }
        private async Task<string> GetDocument(int id) 
        {
            string accessToken = await GetAccessToken();
            string baseAddress = await GetServiceConstantValue("BaseAddress");
            string apiAddress = await GetServiceConstantValue("Document");
            apiAddress = apiAddress.Replace("{key}", id.ToString());
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress); 

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var result = await client.GetAsync(apiAddress);
                var responseContent = result.Content.ReadAsStringAsync().Result;
            }
            return string.Empty;
        }
        private async Task<string> GetServiceConstantValue(string code) 
        {
            string retVal = string.Empty;
            var serviceConstantList = (await _parameterService.GetServiceConstantListAsync())?.Data;
            var serviceConstant = serviceConstantList?.Where(x => x.Code == code).FirstOrDefault();
            if (serviceConstant != null)
                retVal = serviceConstant.Name;
            return retVal;
        }
    }
}
