using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Dtos.Target.Group;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignTarget;
using Bbt.Campaign.Services.ListData;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.CampaignTarget
{
    public class CampaignTargetService : ICampaignTargetService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignService _campaignService;

        public CampaignTargetService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, ICampaignService campaignService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
        }

        public async Task<BaseResponse<CampaignTargetDto>> UpdateAsync(CampaignTargetInsertRequest request)
        {
            await CheckValidationsAsync(request);

            foreach (var campaignTargetEntityDelete in _unitOfWork.GetRepository<CampaignTargetEntity>()
                .GetAll(x => x.CampaignId == request.CampaignId && !x.IsDeleted).ToList())
            {
                await _unitOfWork.GetRepository<CampaignTargetEntity>().DeleteAsync(campaignTargetEntityDelete);
            }

            string targetListStr = ",";
            foreach(int targetId in request.TargetList) 
            {
                targetListStr += targetId == 0 ? "#" : ("," + targetId.ToString()); 
            }

            string[] targetListArray = targetListStr.Split('#');
            foreach(string targetIds in targetListArray) 
            {
                string[] targetIdsArray = targetIds.Split(',');
                if(targetIdsArray.Length > 0) 
                {
                    TargetGroupEntity targetGroupEntity = new TargetGroupEntity();
                    targetGroupEntity.Name = "";
                    targetGroupEntity = await _unitOfWork.GetRepository<TargetGroupEntity>().AddAsync(targetGroupEntity);

                    foreach(string targetId in targetIdsArray) 
                    {
                        if (!string.IsNullOrEmpty(targetId)) 
                        {
                            var entity = new CampaignTargetEntity()
                            {
                                CampaignId = request.CampaignId,
                                TargetGroup = targetGroupEntity,
                                TargetOperationId = (int)TargetOperationsEnum.And,
                                TargetId = Convert.ToInt32(targetId),
                            };
                            await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(entity);
                        }
                        
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return await this.GetListByCampaignAsync(request.CampaignId);
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

        public async Task<BaseResponse<CampaignTargetInsertFormDto>> GetInsertForm()
        {
            CampaignTargetInsertFormDto response = new CampaignTargetInsertFormDto();

            await FillForm(response);

            return await BaseResponse<CampaignTargetInsertFormDto>.SuccessAsync(response);
        }

        private async Task FillForm(CampaignTargetInsertFormDto response)
        {
            response.TargetList = _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => !x.IsDeleted)
                .Select(x => _mapper.Map<ParameterDto>(x)).ToList();

            
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
            var campaignTargetDto = await GetCampaignTargetDto(campaignId, false, 0, 0);

            if (campaignTargetDto != null)
            {
                return await BaseResponse<CampaignTargetDto>.SuccessAsync(campaignTargetDto);
            }

            return await BaseResponse<CampaignTargetDto>.FailAsync("Kampanya hedefi bulunamadı.");
        }

        public async Task<CampaignTargetDto> GetCampaignTargetDto(int campaignId, bool removeInvisible, decimal usedAmount, int usedNumberOfTransaction)
        {
            var campaignTargetQuery = _unitOfWork.GetRepository<CampaignTargetListEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);
            if (removeInvisible)
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

                decimal remainAmount = 0;
                decimal targetAmount = campaignTargetList
                    .Where(x => x.TargetGroupId == targetGroupId && x.TotalAmount != null)
                    .Sum(t=>t.TotalAmount) ?? 0;
                if(targetAmount > 0) 
                {
                    remainAmount = (usedAmount >  targetAmount) ? 0 : (targetAmount - usedAmount);

                    targetGroupDto.TargetAmount = targetAmount;
                    targetGroupDto.RemainAmount = remainAmount;
                }

                int remainNumberOfTransaction = 0;
                int targetNumberOfTransaction = campaignTargetList
                    .Where(x => x.TargetGroupId == targetGroupId && x.NumberOfTransaction != null)
                    .Sum(t => t.NumberOfTransaction) ?? 0;
                if(targetNumberOfTransaction > 0) 
                {
                    remainNumberOfTransaction = (usedNumberOfTransaction > targetNumberOfTransaction) ? 0 : 
                        (targetNumberOfTransaction - usedNumberOfTransaction);

                    targetGroupDto.TargetNumberOfTransaction = targetNumberOfTransaction;
                    targetGroupDto.RemainNumberOfTransaction = remainNumberOfTransaction;
                }
                
                foreach (var campaignTarget in campaignTargetList.Where(x => x.TargetGroupId == targetGroupId))
                {
                    targetGroupDto.TargetList.Add(
                        new TargetParameterDto
                        {
                            Id = campaignTarget.TargetId,
                            Name = campaignTarget.Name,
                            Code = "",
                            UsedAmount = usedAmount == 0 ? null : usedAmount,
                            UsedNumberOfTransaction = usedNumberOfTransaction == 0 ? null : usedNumberOfTransaction,
                        });
                }
                campaignTargetDto.TargetGroupList.Add(targetGroupDto);
            }

            return campaignTargetDto;
        }

        public async Task<BaseResponse<CampaignTargetUpdateFormDto>> GetUpdateForm(int campaignId)
        {
            CampaignTargetUpdateFormDto response = new CampaignTargetUpdateFormDto();

            await FillForm(response);

            response.IsInvisibleCampaign = await _campaignService.IsInvisibleCampaign(campaignId);

            response.CampaignTargetList = (await GetListByCampaignAsync(campaignId))?.Data;

            return await BaseResponse<CampaignTargetUpdateFormDto>.SuccessAsync(response);
        }
    }
}
