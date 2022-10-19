using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Dtos.Target.Group;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.CampaignTarget;
using Bbt.Campaign.Services.ListData;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.Draft;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Services.Services.Remote;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.CampaignTarget
{
    public class CampaignTargetService : ICampaignTargetService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDraftService _draftService;
        private readonly IRemoteService _remoteService;

        public CampaignTargetService(IUnitOfWork unitOfWork, IMapper mapper, IDraftService draftService, IRemoteService remoteService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _draftService = draftService;
            _remoteService = remoteService;
        }
        public async Task<BaseResponse<CampaignTargetDto>> AddAsync(CampaignTargetInsertRequest request, string userId) 
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            await CheckValidationsAsync(request);

            await Update(request, userId);

            return await this.GetListByCampaignAsync(request.CampaignId);
        }

        public async Task<BaseResponse<CampaignTargetDto>> UpdateAsync(CampaignTargetInsertRequest request, string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.Update;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            await CheckValidationsAsync(request);

            await Update(request, userId);

            return await this.GetListByCampaignAsync(request.CampaignId);
        }

        private async Task Update(CampaignTargetInsertRequest request, string userid) 
        {
            int processTypeId = await _draftService.GetCampaignProcessType(request.CampaignId);
            if (processTypeId == (int)ProcessTypesEnum.CreateDraft)
            {
                request.CampaignId = await _draftService.CreateCampaignDraftAsync(request.CampaignId, userid, (int)PageTypeEnum.CampaignTarget);
            }
            else
            {
                var campaignUpdatePageEntity = _unitOfWork.GetRepository<CampaignUpdatePageEntity>().GetAll().Where(x => x.CampaignId == request.CampaignId).FirstOrDefault();
                if (campaignUpdatePageEntity != null)
                {
                    campaignUpdatePageEntity.IsCampaignTargetUpdated = true;
                    await _unitOfWork.GetRepository<CampaignUpdatePageEntity>().UpdateAsync(campaignUpdatePageEntity);
                }
            }

            foreach (var campaignTargetEntityDelete in _unitOfWork.GetRepository<CampaignTargetEntity>()
                .GetAll(x => x.CampaignId == request.CampaignId && !x.IsDeleted).ToList())
            {
                await _unitOfWork.GetRepository<CampaignTargetEntity>().DeleteAsync(campaignTargetEntityDelete);
            }

            string targetListStr = ",";
            foreach (int targetId in request.TargetList)
            {
                targetListStr += targetId == 0 ? "#" : ("," + targetId.ToString());
            }

            string[] targetListArray = targetListStr.Split('#');
            foreach (string targetIds in targetListArray)
            {
                string[] targetIdsArray = targetIds.Split(',');
                if (targetIdsArray.Length > 0)
                {
                    TargetGroupEntity targetGroupEntity = new TargetGroupEntity();
                    targetGroupEntity.Name = "";
                    targetGroupEntity.CreatedBy = userid;
                    targetGroupEntity = await _unitOfWork.GetRepository<TargetGroupEntity>().AddAsync(targetGroupEntity);

                    foreach (string targetId in targetIdsArray)
                    {
                        if (!string.IsNullOrEmpty(targetId))
                        {
                            var entity = new CampaignTargetEntity()
                            {
                                CampaignId = request.CampaignId,
                                TargetGroup = targetGroupEntity,
                                TargetOperationId = (int)TargetOperationsEnum.And,
                                TargetId = Convert.ToInt32(targetId),
                                CreatedBy = userid,
                            };
                            await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(entity);
                        }

                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        async Task CheckValidationsAsync(CampaignTargetInsertRequest input) 
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                        .GetAll(x => x.Id == input.CampaignId && !x.IsDeleted)
                        .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya hatalı.");
            }

            int targetCount = input.TargetList.Where(x => x > 0).Count();
            if(targetCount == 0)
                throw new Exception("Hedef giriniz.");

            foreach (int targetId in input.TargetList.Where(x => x > 0))
            {
                var entity = await _unitOfWork.GetRepository<TargetEntity>()
                    .GetAll(x => x.Id == targetId && !x.IsDeleted)
                    .FirstOrDefaultAsync();
                if (entity == null)
                {
                    throw new Exception("Hedef hatalı.");
                }
            }


            //tekillik kontrolu
            string targetListStr = ",";
            foreach (int targetId in input.TargetList)
            {
                targetListStr += targetId == 0 ? "#" : ("," + targetId.ToString());
            }
            List<CampaignTargetEntity> campaignTargetList = new List<CampaignTargetEntity>();
            int targetGroupId = 1;
            string[] targetListArray = targetListStr.Split('#');
            foreach (string targetIds in targetListArray)
            {
                string[] targetIdsArray = targetIds.Split(',');
                if (targetIdsArray.Length > 0)
                {
                    foreach (string targetId in targetIdsArray)
                    {
                        if (!string.IsNullOrEmpty(targetId))
                        {
                            var entity = new CampaignTargetEntity()
                            {
                                CampaignId = input.CampaignId,
                                TargetGroupId = targetGroupId,
                                TargetOperationId = (int)TargetOperationsEnum.And,
                                TargetId = Convert.ToInt32(targetId),
                            };
                            campaignTargetList.Add(entity);
                        }  
                    }
                }
                targetGroupId++;
            }

            var groupList = campaignTargetList.Select(x=>x.TargetGroupId).Distinct().ToList();
            foreach (var groupId in groupList) 
            { 
                int currentTargetCount = campaignTargetList.Where(x => x.TargetGroupId == groupId).Count();
                foreach(var entity in campaignTargetList.Where(x=>x.TargetGroupId != groupId)) 
                {
                    int nextTargetGroupId = entity.TargetGroupId;
                    int nextTargetCount = campaignTargetList.Where(x => x.TargetGroupId == nextTargetGroupId).Count();
                    if (currentTargetCount != nextTargetCount)
                        continue;

                    bool isExists = true;
                    foreach (var currentEntity in campaignTargetList.Where(x => x.TargetGroupId == groupId))
                    {
                        int currenctTargetId = currentEntity.TargetId;
                        var nextEntity = campaignTargetList.Where(t => t.TargetGroupId == nextTargetGroupId && t.TargetId == currenctTargetId).FirstOrDefault();
                        if (nextEntity == null)
                        {
                            isExists = false;
                            continue;
                        }
                    }

                    if (isExists)
                    {
                        throw new Exception("Aynı hedef grubu girilemez.");
                    }
                }
            }
        }

        public async Task<BaseResponse<CampaignTargetDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<CampaignTargetEntity>().GetByIdAsync(id);
            if (entity == null)
                return null;

            await _unitOfWork.GetRepository<CampaignTargetEntity>().DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return await GetCampaignTargetAsync(entity.Id);
        }

        public async Task<BaseResponse<CampaignTargetDto>> GetCampaignTargetAsync(int id)
        {
            var campaignTargetEntity = await _unitOfWork.GetRepository<CampaignTargetEntity>().GetByIdAsync(id);
            if (campaignTargetEntity != null)
            {
                CampaignTargetDto campaignTargetDto = _mapper.Map<CampaignTargetDto>(campaignTargetEntity);
                return await BaseResponse<CampaignTargetDto>.SuccessAsync(campaignTargetDto);
            }
            return null;
        }

        public async Task<BaseResponse<CampaignTargetInsertFormDto>> GetInsertForm(string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CampaignTargetInsertFormDto response = new CampaignTargetInsertFormDto();

            await FillForm(response);

            return await BaseResponse<CampaignTargetInsertFormDto>.SuccessAsync(response);
        }

        private async Task FillForm(CampaignTargetInsertFormDto response)
        {
            response.TargetList = _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => x.IsActive && !x.IsDeleted && x.StatusId == (int)StatusEnum.Approved)
                .Select(x => _mapper.Map<ParameterDto>(x))
                .ToList(); 
        }

        public async Task<BaseResponse<List<CampaignTargetDto>>> GetListAsync()
        {
            List<CampaignTargetDto> campaignTargetDtos =
                _unitOfWork.GetRepository<CampaignTargetEntity>()
                .GetAll(x => x.IsDeleted != true)
                .Include(x => x.Target)
                .Select(x => _mapper.Map<CampaignTargetDto>(x)).ToList();
            return await BaseResponse<List<CampaignTargetDto>>.SuccessAsync(campaignTargetDtos);
        }

        public async Task<BaseResponse<CampaignTargetDto>> GetListByCampaignAsync(int campaignId)
        {
            var campaignTargetDto = await GetCampaignTargetDto(campaignId, false);

            if (campaignTargetDto != null)
            {
                return await BaseResponse<CampaignTargetDto>.SuccessAsync(campaignTargetDto);
            }

            return await BaseResponse<CampaignTargetDto>.FailAsync("Kampanya hedefi bulunamadı.");
        }

        public async Task<CampaignTargetDto> GetCampaignTargetDto(int campaignId, bool isRemoveInvisible)
        {
            var campaignTargetQuery = _unitOfWork.GetRepository<CampaignTargetListEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);
            if(isRemoveInvisible) 
                campaignTargetQuery = campaignTargetQuery.Where(x => x.TargetViewTypeId != (int)TargetViewTypeEnum.Invisible);

            if (!campaignTargetQuery.Any()) return null;

            var campaignTargetList = CampaignTargetListData.GetCampaignTargetList(campaignTargetQuery);

            var grouplist = campaignTargetList.Select(x => x.TargetGroupId).Distinct().ToList();

            var campaignTargetDto = new CampaignTargetDto();
            campaignTargetDto.CampaignId = campaignId;
            campaignTargetDto.GroupCount = grouplist.Count();

            foreach (var targetGroupId in grouplist)
            {
                var targetGroupDto = new TargetGroupDto();
                targetGroupDto.Id = targetGroupId;

                foreach (var campaignTarget in campaignTargetList.Where(x => x.TargetGroupId == targetGroupId))
                {
                    targetGroupDto.TargetList.Add(
                        new TargetParameterDto
                        {
                            Id = campaignTarget.TargetId,
                            Name = campaignTarget.Name, 
                            //Code = "",
                            Title = campaignTarget.Title,
                            DescriptionTr = campaignTarget.DescriptionTr,
                            DescriptionEn = campaignTarget.DescriptionEn,
                            TargetDetailTr = campaignTarget.TargetDetailTr,
                            TargetDetailEn = campaignTarget.TargetDetailEn,
                        });
                }
                campaignTargetDto.TargetGroupList.Add(targetGroupDto);
            }
            return campaignTargetDto;
        }

        public async Task<CampaignTargetDto> GetCampaignTargetDtoCustomer(int campaignId, List<TargetParameterDto> targetSourceList)
        {
            var campaignTargetQuery = _unitOfWork.GetRepository<CampaignTargetListEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);
            campaignTargetQuery = campaignTargetQuery.Where(x => x.TargetViewTypeId != (int)TargetViewTypeEnum.Invisible);//removeInvisible

            if (!campaignTargetQuery.Any()) return null;

            var campaignTargetList = CampaignTargetListData.GetCampaignTargetList(campaignTargetQuery);

            var grouplist = campaignTargetList.Select(x => x.TargetGroupId).Distinct().ToList();

            var campaignTargetDto = new CampaignTargetDto();
            campaignTargetDto.CampaignId = campaignId;
            campaignTargetDto.GroupCount = grouplist.Count();

            foreach (var targetGroupId in grouplist)
            {
                var targetGroupDto = new TargetGroupDto();
                targetGroupDto.Id = targetGroupId;

                decimal targetAmount = campaignTargetList
                        .Where(x => x.TargetGroupId == targetGroupId && x.TotalAmount != null)
                        .Sum(t => t.TotalAmount) ?? 0;
                int targetNumberOfTransaction = campaignTargetList
                        .Where(x => x.TargetGroupId == targetGroupId && x.NumberOfTransaction != null)
                        .Sum(t => t.NumberOfTransaction) ?? 0;

                if (targetAmount > 0)
                {
                    decimal remainAmount = 0;
                    if (targetAmount > 0)
                    {
                       // var targetSource = targetSourceList.Where(x=>x.Id == )


                        //remainAmount = (usedAmount > targetAmount) ? 0 : (targetAmount - usedAmount);
                    }
                    //targetGroupDto.TargetAmount = targetAmount;
                    //targetGroupDto.RemainAmount = remainAmount;
                    targetGroupDto.TargetAmountStr = Helpers.ConvertNullablePriceString(targetAmount);
                    targetGroupDto.RemainAmountStr = Helpers.ConvertNullablePriceString(remainAmount);
                }
                else if (targetNumberOfTransaction > 0)
                {

                }

                //if (usedNumberOfTransaction > 0)
                //{
                //    int remainNumberOfTransaction = 0;

                //    if (targetNumberOfTransaction > 0)
                //    {
                //        remainNumberOfTransaction = (usedNumberOfTransaction > targetNumberOfTransaction) ? 0 :
                //            (targetNumberOfTransaction - usedNumberOfTransaction);

                //        targetGroupDto.TargetNumberOfTransaction = targetNumberOfTransaction;
                //        targetGroupDto.RemainNumberOfTransaction = remainNumberOfTransaction;
                //    }
                //}

                foreach (var campaignTarget in campaignTargetList.Where(x => x.TargetGroupId == targetGroupId))
                {
                    int percent = 0;
                    decimal totalAmount = campaignTarget.TotalAmount ?? 0;
                    int numberOfTransaction = campaignTarget.NumberOfTransaction ?? 0;

                    //if (usedAmount > 0 || usedNumberOfTransaction > 0)
                    //{
                    //    if (usedAmount > 0)
                    //    {
                    //        if (totalAmount > 0)
                    //            percent = (usedAmount > targetAmount) ? 100 : ((int)(usedAmount / targetAmount) * 100);
                    //    }
                    //    else
                    //    {
                    //        if (numberOfTransaction > 0)
                    //            percent = (usedNumberOfTransaction > numberOfTransaction) ? 100 : ((int)(usedNumberOfTransaction / numberOfTransaction) * 100);
                    //    }
                    //}

                    //targetGroupDto.TargetList.Add(
                    //    new TargetParameterDto
                    //    {
                    //        Id = campaignTarget.TargetId,
                    //        Name = campaignTarget.Name,
                    //        Code = "",
                    //        UsedAmountStr = Helpers.ConvertNullablePriceString(usedAmount),
                    //        UsedNumberOfTransaction = usedNumberOfTransaction == 0 ? null : usedNumberOfTransaction,
                    //        Percent = percent
                    //    });
                }
                
                campaignTargetDto.TargetGroupList.Add(targetGroupDto);
            }

            return campaignTargetDto;
        }

        public async Task<CampaignTargetDto> GetCampaignTargetDtoCustomer(int campaignId, decimal usedAmount, int usedNumberOfTransaction)
        {
            var campaignTargetQuery = _unitOfWork.GetRepository<CampaignTargetListEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);
            campaignTargetQuery = campaignTargetQuery.Where(x => x.TargetViewTypeId != (int)TargetViewTypeEnum.Invisible);

            if (!campaignTargetQuery.Any()) return null;

            

            var campaignTargetList = CampaignTargetListData.GetCampaignTargetList(campaignTargetQuery);

            var grouplist = campaignTargetList.Select(x => x.TargetGroupId).Distinct().ToList();

            var campaignTargetDto = new CampaignTargetDto();
            campaignTargetDto.CampaignId = campaignId;
            campaignTargetDto.GroupCount = grouplist.Count();

            foreach (var targetGroupId in grouplist)
            {
                var targetGroupDto = new TargetGroupDto();
                
                targetGroupDto.Id = targetGroupId;

                decimal targetAmount = campaignTargetList
                        .Where(x => x.TargetGroupId == targetGroupId && x.TotalAmount != null)
                        .Sum(t => t.TotalAmount) ?? 0;
                int targetNumberOfTransaction = campaignTargetList
                        .Where(x => x.TargetGroupId == targetGroupId && x.NumberOfTransaction != null)
                        .Sum(t => t.NumberOfTransaction) ?? 0;

                if (targetAmount > 0)
                {
                    decimal remainAmount = (usedAmount > targetAmount) ? 0 : (targetAmount - usedAmount);
                    //targetGroupDto.TargetAmount = targetAmount;
                    //targetGroupDto.RemainAmount = remainAmount;
                    targetGroupDto.TargetAmountStr = Helpers.ConvertNullablePriceString(targetAmount);
                    targetGroupDto.TargetAmountCurrencyCode = "TRY";
                    targetGroupDto.RemainAmountStr = Helpers.ConvertNullablePriceString(remainAmount);
                }

                //if (usedNumberOfTransaction > 0)
                //{
                //    int remainNumberOfTransaction = 0;

                //    if (targetNumberOfTransaction > 0)
                //    {
                //        remainNumberOfTransaction = (usedNumberOfTransaction > targetNumberOfTransaction) ? 0 :
                //            (targetNumberOfTransaction - usedNumberOfTransaction);

                //        //targetGroupDto.TargetNumberOfTransaction = targetNumberOfTransaction;
                //        //targetGroupDto.RemainNumberOfTransaction = remainNumberOfTransaction;
                //    }
                //}

                foreach (var campaignTarget in campaignTargetList.Where(x => x.TargetGroupId == targetGroupId))
                {
                    int percent = 0;
                    decimal totalAmount = campaignTarget.TotalAmount ?? 0;
                    int numberOfTransaction = campaignTarget.NumberOfTransaction ?? 0;
                    string usedAmountStr = string.Empty;
                    string usedAmountCurrencyCode = null;
                    string remainAmountStr = string.Empty;

                    if (totalAmount > 0)
                    {
                        usedAmountStr = usedAmount == 0 ? "0" : Helpers.ConvertNullablePriceString(usedAmount);
                        usedAmountCurrencyCode = "TRY";
                        remainAmountStr = (usedAmount >= totalAmount) ? "0" : Helpers.ConvertNullablePriceString(totalAmount - usedAmount);
                        percent = (usedAmount >= totalAmount) ? 100 : (int)((usedAmount / totalAmount) * 100);

                    }
                    else if (numberOfTransaction > 0)
                    {
                        usedAmountStr = usedNumberOfTransaction.ToString();
                        remainAmountStr = (numberOfTransaction - usedNumberOfTransaction).ToString();
                        percent = (usedNumberOfTransaction >= numberOfTransaction) ? 100 : (int)(((decimal)usedNumberOfTransaction / (decimal)numberOfTransaction) * 100);
                    }

                    targetGroupDto.TargetList.Add(
                        new TargetParameterDto
                        {
                            Id = campaignTarget.TargetId,
                            Name = campaignTarget.Name,
                            Title = campaignTarget.Title,
                            TargetViewTypeId = campaignTarget.TargetViewTypeId,
                            UsedAmountStr = usedAmountStr,
                            UsedAmountCurrencyCode = usedAmountCurrencyCode,
                            RemainAmountStr = remainAmountStr,
                            Percent = percent,
                            DescriptionTr = campaignTarget.DescriptionTr,
                            DescriptionEn = campaignTarget.DescriptionEn,
                            TargetDetailTr = campaignTarget.TargetDetailTr,
                            TargetDetailEn = campaignTarget.TargetDetailEn,
                        });
                }
                campaignTargetDto.TargetGroupList.Add(targetGroupDto);
            }

            return campaignTargetDto;
        }

        public async Task<CampaignTargetDto2> GetCampaignTargetDtoCustomer2(int campaignId, string customerCode, string lang, bool isTest) 
        {
            CampaignTargetDto2 campaignTargetDto2 = new CampaignTargetDto2();
            List<TargetParameterDto2> progressBarlist = new List<TargetParameterDto2>();
            List<TargetParameterDto2> informationlist = new List<TargetParameterDto2>();
            List<TargetParameterDto2> targetParameterList = new List<TargetParameterDto2>();
            campaignTargetDto2.IsAchieved = false;
            decimal usedAmount = 0;
            decimal targetAmount = 0;
            decimal remainAmount = 0;
            int index = 0;
            int usedNumberOfTransaction = 0;
            bool isAchieved = false;
            if (isTest) 
            {
                campaignTargetDto2.IsAchieved = isAchieved;
                var campaignTargetQuery = _unitOfWork.GetRepository<CampaignTargetListEntity>().GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);
                campaignTargetQuery = campaignTargetQuery.Where(x => x.TargetViewTypeId != (int)TargetViewTypeEnum.Invisible);
                var campaignTargetList = CampaignTargetListData.GetCampaignTargetList(campaignTargetQuery);

                foreach (var campaignTarget in campaignTargetList)
                {
                    TargetParameterDto2 targetParameterDto2 = new TargetParameterDto2();
                    targetParameterDto2.Id = campaignTarget.Id;
                    targetParameterDto2.Name = campaignTarget.Name;
                    targetParameterDto2.Title = campaignTarget.Title;
                    targetParameterDto2.TargetViewTypeId = campaignTarget.TargetViewTypeId;
                    targetParameterDto2.TargetGroupId = campaignTarget.TargetGroupId;
                    targetParameterDto2.TotalAmount = campaignTarget.TotalAmount ?? 0;
                    targetParameterDto2.NumberOfTransaction = campaignTarget.NumberOfTransaction ?? 0;

                    if (campaignTarget.TotalAmount > 0) 
                    {
                        targetAmount = campaignTarget.TotalAmount ?? 0;

                        targetParameterDto2.TargetAmount = targetAmount;
                        targetParameterDto2.TargetAmountStr = Helpers.ConvertNullablePriceString(targetAmount);
                        targetParameterDto2.TargetAmountCurrencyCode = "TRY";

                        usedAmount = 0;
                        targetParameterDto2.UsedAmount = usedAmount;
                        targetParameterDto2.UsedAmountStr = Helpers.ConvertNullablePriceString(usedAmount);
                        targetParameterDto2.UsedAmountCurrencyCode = "TRY";

                        remainAmount = (targetAmount > usedAmount) ? (targetAmount - usedAmount) : 0;
                        targetParameterDto2.RemainAmount = remainAmount;
                        targetParameterDto2.RemainAmountStr = remainAmount == 0 ? "0" : Helpers.ConvertNullablePriceString(remainAmount);
                        targetParameterDto2.RemainAmountCurrencyCode = "TRY";

                        if (index == 0) 
                        {
                            campaignTargetDto2.TargetAmountStr = targetParameterDto2.TargetAmountStr;
                            campaignTargetDto2.TargetAmountCurrencyCode = "TRY";
                            campaignTargetDto2.RemainAmountStr = targetParameterDto2.RemainAmountStr;
                            campaignTargetDto2.RemainAmountCurrencyCode = "TRY";
                        }
                        index++;
                    }
                    else if(campaignTarget.NumberOfTransaction > 0)
                    {
                        targetAmount = campaignTarget.NumberOfTransaction ?? 0;

                        targetParameterDto2.TargetAmount = targetAmount;
                        targetParameterDto2.TargetAmountStr = campaignTarget.NumberOfTransaction.ToString();
                        targetParameterDto2.TargetAmountCurrencyCode = null;

                        usedNumberOfTransaction = 0;
                        targetParameterDto2.UsedAmount = usedNumberOfTransaction;
                        targetParameterDto2.UsedAmountStr = usedNumberOfTransaction.ToString(); 
                        targetParameterDto2.UsedAmountCurrencyCode = null;

                        remainAmount = targetAmount > usedAmount ? (targetAmount - usedAmount) : 0;
                        targetParameterDto2.RemainAmount = remainAmount;
                        targetParameterDto2.RemainAmountStr = remainAmount.ToString();
                        targetParameterDto2.RemainAmountCurrencyCode = null;
                    }

                    targetParameterDto2.Percent = remainAmount == 0 ? 100 : (int)((usedAmount / targetAmount) * 100);
                    targetParameterDto2.IsAchieved = remainAmount == 0;
                    targetParameterDto2.Description = lang == "tr" ? campaignTarget.DescriptionTr : campaignTarget.DescriptionEn;
                    targetParameterList.Add(targetParameterDto2);

                    if (targetParameterDto2.TargetViewTypeId == (int)TargetViewTypeEnum.ProgressBar)
                        progressBarlist.Add(targetParameterDto2);
                    else if (targetParameterDto2.TargetViewTypeId == (int)TargetViewTypeEnum.Information)
                        informationlist.Add(targetParameterDto2);
                }

                var targetGroupList = targetParameterList.Select(x => x.TargetGroupId).Distinct().ToList();
                foreach (int targetGroupId in targetGroupList)
                {
                    var unAchievedItem = targetParameterList
                        .Where(x => x.TargetGroupId == targetGroupId && x.IsAchieved == false)
                        .FirstOrDefault();
                    if (unAchievedItem == null)
                    {
                        campaignTargetDto2.IsAchieved = true;
                        break;
                    }
                }
            }
            else 
            {
                var goalResultByCustomerAndCampaing = await _remoteService.GetGoalResultByCustomerAndCampaingData(customerCode, campaignId, lang);
                if (goalResultByCustomerAndCampaing != null)
                {
                    if (goalResultByCustomerAndCampaing.TargetList != null && goalResultByCustomerAndCampaing.TargetList.Any())
                    {
                        isAchieved = goalResultByCustomerAndCampaing.isAchieved;
                        campaignTargetDto2.IsAchieved = isAchieved;
                        campaignTargetDto2.LastMonthIsAchieved = goalResultByCustomerAndCampaing.lastMonthIsAchieved;
                        campaignTargetDto2.TotalUsed = goalResultByCustomerAndCampaing.TotalUsed;
                        foreach (var goalResult in goalResultByCustomerAndCampaing.TargetList
                            .Where(x => x.CampaignId > 0 && x.Detail.IsDeleted == false && x.Detail.TargetViewTypeId != (int)TargetViewTypeEnum.Invisible))
                        {

                            bool isAmount = goalResult.Detail.TotalAmount > 0;
                            bool isNumberOfTransaction = goalResult.Detail.NumberOfTransaction  > 0;

                            TargetParameterDto2 targetParameterDto2 = new TargetParameterDto2();
                            targetParameterDto2.Id = 0;
                            targetParameterDto2.Name = goalResult.Name;
                            targetParameterDto2.Title = goalResult.Title;
                            targetParameterDto2.TargetViewTypeId = goalResult.Detail.TargetViewTypeId;
                            targetParameterDto2.TargetGroupId = goalResult.TargetGroupId;
                            targetParameterDto2.TotalAmount = (decimal)goalResult.Detail.TotalAmount;
                            targetParameterDto2.NumberOfTransaction = goalResult.Detail.NumberOfTransaction;

                            if (isAmount)
                            {
                                targetAmount = targetParameterDto2.TotalAmount ?? 0;
                                targetParameterDto2.TargetAmount = targetAmount;
                                targetParameterDto2.TargetAmountStr = Helpers.ConvertNullablePriceString(targetAmount);
                                targetParameterDto2.TargetAmountCurrencyCode = goalResult.Detail.StreamResult.Currency == null ? null :
                                    goalResult.Detail.StreamResult.Currency == "TRY" ? "TL" :
                                    goalResult.Detail.StreamResult.Currency;

                                usedAmount = (decimal)goalResult.Detail.StreamResult.Amount;
                                targetParameterDto2.UsedAmount = usedAmount;
                                targetParameterDto2.UsedAmountStr = Helpers.ConvertNullablePriceString(usedAmount);
                                targetParameterDto2.UsedAmountCurrencyCode = goalResult.Detail.StreamResult.Currency == null ? null :
                                    goalResult.Detail.StreamResult.Currency == "TRY" ? "TL" :
                                    goalResult.Detail.StreamResult.Currency;

                                remainAmount = (decimal)goalResult.Detail.StreamResult.RemainingAmount;
                                targetParameterDto2.RemainAmount = remainAmount;
                                targetParameterDto2.RemainAmountStr = Helpers.ConvertNullablePriceString(remainAmount);
                                targetParameterDto2.RemainAmountCurrencyCode = goalResult.Detail.StreamResult.Currency == null ? null :
                                    goalResult.Detail.StreamResult.Currency == "TRY" ? "TL" :
                                    goalResult.Detail.StreamResult.Currency;

                                targetParameterDto2.IsAchieved = isAchieved;
                                targetParameterDto2.Percent = (int)goalResult.Detail.StreamResult.AmountPercent;

                                if (index == 0)
                                {
                                    campaignTargetDto2.TargetAmountStr = targetParameterDto2.TargetAmountStr;
                                    campaignTargetDto2.TargetAmountCurrencyCode = targetParameterDto2.TargetAmountCurrencyCode;

                                    campaignTargetDto2.RemainAmountStr = targetParameterDto2.RemainAmountStr;
                                    campaignTargetDto2.RemainAmountCurrencyCode = targetParameterDto2.RemainAmountCurrencyCode;

                                    campaignTargetDto2.UsedAmountStr = targetParameterDto2.UsedAmountStr;
                                    campaignTargetDto2.UsedAmountCurrencyCode = targetParameterDto2.UsedAmountCurrencyCode;
                                }
                                index++;
                            }
                            else if (isNumberOfTransaction)
                            {
                                targetAmount = targetParameterDto2.NumberOfTransaction ?? 0;

                                targetParameterDto2.TargetAmount = targetAmount;
                                targetParameterDto2.TargetAmountStr = targetAmount.ToString();
                                targetParameterDto2.TargetAmountCurrencyCode = null;

                                usedAmount = goalResult.Detail.StreamResult.Times;
                                targetParameterDto2.UsedAmount = (int)usedAmount;
                                targetParameterDto2.UsedAmountStr = usedAmount.ToString();
                                targetParameterDto2.UsedAmountCurrencyCode = null;
                                targetParameterDto2.UsedNumberOfTransaction = (int)usedAmount;

                                remainAmount = goalResult.Detail.StreamResult.RemainingTimes;
                                targetParameterDto2.RemainAmount = remainAmount;
                                targetParameterDto2.RemainAmountStr = ((int)remainAmount).ToString();
                                targetParameterDto2.RemainAmountCurrencyCode = null;

                                targetParameterDto2.IsAchieved = isAchieved;
                                targetParameterDto2.Percent = (int)goalResult.Detail.StreamResult.TimesPercent;
                            }

                            targetParameterDto2.Description = goalResult.Detail.Description;
                            targetParameterList.Add(targetParameterDto2);

                            if (targetParameterDto2.TargetViewTypeId == (int)TargetViewTypeEnum.ProgressBar)
                                progressBarlist.Add(targetParameterDto2);
                            else if (targetParameterDto2.TargetViewTypeId == (int)TargetViewTypeEnum.Information)
                                informationlist.Add(targetParameterDto2);
                        }
                    }
                }
            }

            campaignTargetDto2.ProgressBarlist = progressBarlist;
            campaignTargetDto2.Informationlist = informationlist;

            return campaignTargetDto2;
        }

        public async Task<BaseResponse<CampaignTargetUpdateFormDto>> GetUpdateForm(int campaignId, string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CampaignTargetUpdateFormDto response = new CampaignTargetUpdateFormDto();

            await FillForm(response);

            CampaignProperty campaignProperty = await _draftService.GetCampaignProperties(campaignId);
            response.IsUpdatableCampaign = campaignProperty.IsUpdatableCampaign;
            response.IsInvisibleCampaign = campaignProperty.IsInvisibleCampaign;

            response.CampaignTargetList = (await GetListByCampaignAsync(campaignId))?.Data;

            return await BaseResponse<CampaignTargetUpdateFormDto>.SuccessAsync(response);
        }
    }
}
