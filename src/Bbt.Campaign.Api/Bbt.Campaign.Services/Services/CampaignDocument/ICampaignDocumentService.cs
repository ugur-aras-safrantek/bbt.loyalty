using Bbt.Campaign.Core.Enums;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Models.CampaignDocument;

namespace Bbt.Campaign.Services.Services.CampaignDocument
{
    public interface ICampaignDocumentService
    {
        public Task<BaseResponse<AddCampaignDocumentResponse>> AddAsync(AddCampaignDocumentRequest campaignDocument);
        Task<BaseResponse<GetCampaignDocumentResponse>> GetDocumentsByCampaign(int campaignId);
        Task<BaseResponse<AddCampaignHtmlContentResponse>> AddHtmContents(AddCampaignHtmlContentRequest campaignDocument);
        Task<BaseResponse<GetCampaignHtmlContentsResponse>> GetHtmlContentsByCampaign(int campaignId);
        Task<BaseResponse<GetCampaignDocumentResponse>> GetDocumentsByType(int campaignId, int documentTypeId);
    }
}
