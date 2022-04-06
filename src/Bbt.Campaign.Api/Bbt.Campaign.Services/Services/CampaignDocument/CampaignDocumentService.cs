using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Enums;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignDocument;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using System.Text;

namespace Bbt.Campaign.Services.Services.CampaignDocument
{
    public class CampaignDocumentService : ICampaignDocumentService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CampaignDocumentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<AddCampaignDocumentResponse>> AddAsync(AddCampaignDocumentRequest campaignDocument)
        {
            if (campaignDocument == null || campaignDocument.CampaignListImage == null)
                return await BaseResponse<AddCampaignDocumentResponse>.FailAsync("Kampanya'ya ait listeleme görseli boş geçilemez");

            if (campaignDocument == null || campaignDocument.CampaignDetailImage == null)
                return await BaseResponse<AddCampaignDocumentResponse>.FailAsync("Kampanya'ya ait detay görseli boş geçilemez");

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignDocument.CampaignId);
            if (campaignEntity == null)
                return await BaseResponse<AddCampaignDocumentResponse>.FailAsync("Kampanya bilgisi boş geçilemez");

            long length = campaignDocument.CampaignListImage.Length;
            if (length < 0)
                return await BaseResponse<AddCampaignDocumentResponse>.FailAsync("Kampanya'ya ait listeleme görseli hatası");

            using var fileStreamList = campaignDocument.CampaignListImage.OpenReadStream();
            byte[] bytesList = new byte[length];
            fileStreamList.Read(bytesList, 0, (int)campaignDocument.CampaignListImage.Length);


            using var fileStreamDetail = campaignDocument.CampaignDetailImage.OpenReadStream();
            byte[] bytesDetail = new byte[length];
            fileStreamDetail.Read(bytesDetail, 0, (int)campaignDocument.CampaignDetailImage.Length);

            await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
            {
                CampaignId = campaignDocument.CampaignId,
                DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignListImage,
                MimeType = MimeTypeExtensions.ToMimeType(Path.GetExtension(campaignDocument.CampaignListImageName)),
                Content = bytesList,
                DocumentName = campaignDocument.CampaignListImageName
            });

            await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
            {
                CampaignId = campaignDocument.CampaignId,
                DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignDetailImage,
                MimeType = MimeTypeExtensions.ToMimeType(Path.GetExtension(campaignDocument.CampaignDetailImageName)),
                Content = bytesDetail,
                DocumentName = campaignDocument.CampaignDetailImageName
            });

            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<AddCampaignDocumentResponse>()
            {
            };
        }
        public async Task<BaseResponse<GetCampaignDocumentResponse>> GetDocumentsByCampaign(int campaignId)
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
            if (campaignEntity == null)
                return await BaseResponse<GetCampaignDocumentResponse>.FailAsync("Kampanya bilgisi boş geçilemez");

            var documents = _unitOfWork.GetRepository<CampaignDocumentEntity>().GetAll(x => x.CampaignId == campaignId);
            GetCampaignDocumentResponse result = new GetCampaignDocumentResponse()
            {
                Documents = documents.Select(x => new DocumentModel()
                {
                    Data = Convert.ToBase64String(x.Content, 0, x.Content.Length),
                    DocumentName = x.DocumentName,
                    DocumentType = (DocumentTypePublicEnum)x.DocumentType,
                    MimeType = x.MimeType
                }).ToList()
            };
            return new BaseResponse<GetCampaignDocumentResponse>()
            {
                Data = result
            };
        }
        public async Task<BaseResponse<GetCampaignDocumentResponse>> GetDocumentsByType(int campaignId, int documentTypeId)
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
            if (campaignEntity == null)
                return await BaseResponse<GetCampaignDocumentResponse>.FailAsync("Kampanya bilgisi boş geçilemez");

            DocumentTypeDbEnum documentTypeDbEnum = Helpers.ToEnum<DocumentTypeDbEnum>(documentTypeId);
            var documents = _unitOfWork.GetRepository<CampaignDocumentEntity>()
                .GetAll(x => x.CampaignId == campaignId && x.DocumentType == documentTypeDbEnum && !x.IsDeleted);
            GetCampaignDocumentResponse result = new GetCampaignDocumentResponse()
            {
                Documents = documents.Select(x => new DocumentModel()
                {
                    Data = Convert.ToBase64String(x.Content, 0, x.Content.Length),
                    DocumentName = x.DocumentName,
                    DocumentType = (DocumentTypePublicEnum)x.DocumentType,
                    MimeType = x.MimeType
                }).ToList()
            };
            return new BaseResponse<GetCampaignDocumentResponse>()
            {
                Data = result
            };
        }

        public async Task<BaseResponse<AddCampaignHtmlContentResponse>> AddHtmContents(AddCampaignHtmlContentRequest campaignDocument)
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignDocument.CampaignId);
            if (campaignEntity == null)
                return await BaseResponse<AddCampaignHtmlContentResponse>.FailAsync("Kampanya bilgisi boş geçilemez");

            if (!string.IsNullOrEmpty(campaignDocument.ContentTr))
            {
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                {
                    CampaignId = campaignDocument.CampaignId,
                    DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignContentTurkish,
                    MimeType = MimeTypeExtensions.HtmlMimeType,
                    Content = Encoding.UTF8.GetBytes(campaignDocument.ContentTr),
                    DocumentName = nameof(campaignDocument.ContentTr)
                });
            }

            if (!string.IsNullOrEmpty(campaignDocument.ContentEn))
            {
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                {
                    CampaignId = campaignDocument.CampaignId,
                    DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignContentEnglish,
                    MimeType = MimeTypeExtensions.HtmlMimeType,
                    Content = Encoding.UTF8.GetBytes(campaignDocument.ContentEn),
                    DocumentName = nameof(campaignDocument.ContentEn)
                });
            }

            if (!string.IsNullOrEmpty(campaignDocument.DetailEn))
            {
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                {
                    CampaignId = campaignDocument.CampaignId,
                    DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignDetailEnglish,
                    MimeType = MimeTypeExtensions.HtmlMimeType,
                    Content = Encoding.UTF8.GetBytes(campaignDocument.DetailEn),
                    DocumentName = nameof(campaignDocument.DetailEn)
                });
            }

            if (!string.IsNullOrEmpty(campaignDocument.DetailTr))
            {
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                {
                    CampaignId = campaignDocument.CampaignId,
                    DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignDetailTurkish,
                    MimeType = MimeTypeExtensions.HtmlMimeType,
                    Content = Encoding.UTF8.GetBytes(campaignDocument.DetailTr),
                    DocumentName = nameof(campaignDocument.DetailTr)
                });
            }

            if (!string.IsNullOrEmpty(campaignDocument.SummaryEn))
            {
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                {
                    CampaignId = campaignDocument.CampaignId,
                    DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignSummaryEnglish,
                    MimeType = MimeTypeExtensions.HtmlMimeType,
                    Content = Encoding.UTF8.GetBytes(campaignDocument.SummaryEn),
                    DocumentName = nameof(campaignDocument.SummaryEn)
                });
            }

            if (!string.IsNullOrEmpty(campaignDocument.SummaryTr))
            {
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                {
                    CampaignId = campaignDocument.CampaignId,
                    DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignSummaryTurkish,
                    MimeType = MimeTypeExtensions.HtmlMimeType,
                    Content = Encoding.UTF8.GetBytes(campaignDocument.SummaryTr),
                    DocumentName = nameof(campaignDocument.SummaryTr)
                });
            }

            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<AddCampaignHtmlContentResponse>()
            {
            };
        }

        public async Task<BaseResponse<GetCampaignHtmlContentsResponse>> GetHtmlContentsByCampaign(int campaignId)
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
            if (campaignEntity == null)
                return await BaseResponse<GetCampaignHtmlContentsResponse>.FailAsync("Kampanya bilgisi boş geçilemez");

            var documents = _unitOfWork.GetRepository<CampaignDocumentEntity>().GetAll(x => x.CampaignId == campaignId);
            GetCampaignHtmlContentsResponse result = new GetCampaignHtmlContentsResponse() { CampaignId = campaignEntity.Id };
            foreach (var document in documents)
            {
                if (document.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignContentEnglish)
                {
                    result.ContentEn = Encoding.Default.GetString(document.Content);
                }
                else if (document.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignContentTurkish)
                {
                    result.ContentTr = Encoding.Default.GetString(document.Content);
                }

                else if (document.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignSummaryTurkish)
                {
                    result.SummaryTr = Encoding.Default.GetString(document.Content);
                }
                else if (document.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignSummaryEnglish)
                {
                    result.SummaryEn = Encoding.Default.GetString(document.Content);
                }

                else if (document.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignDetailTurkish)
                {
                    result.DetailTr = Encoding.Default.GetString(document.Content);
                }
                else if (document.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignDetailEnglish)
                {
                    result.DetailEn = Encoding.Default.GetString(document.Content);
                }
            }
            return new BaseResponse<GetCampaignHtmlContentsResponse>()
            {
                Data = result
            };
        }
    }
}
