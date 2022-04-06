using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Target.Group;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Target.Group;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Target.Services.Services.Target;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.Target
{
    public class TargetGroupService : ITargetGroupService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITargetService _targetService;

        public TargetGroupService(IUnitOfWork unitOfWork, IMapper mapper, ITargetService targetService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _targetService = targetService;
        }

        public async Task<BaseResponse<TargetGroupDto>> GetTargetGroupAsync(int id)
        {
            var targetGroupEntity = await _unitOfWork.GetRepository<TargetGroupEntity>().GetByIdAsync(id);
            if (targetGroupEntity != null)
            {
                TargetGroupDto targetGroupDto = _mapper.Map<TargetGroupDto>(targetGroupEntity);
                return await BaseResponse<TargetGroupDto>.SuccessAsync(targetGroupDto);
            }
            return null;
        }

        public async Task<BaseResponse<List<TargetGroupDto>>> GetListAsync()
        {
            List<TargetGroupDto> targetGroups = _unitOfWork.GetRepository<TargetGroupEntity>().GetAll(x => x.IsDeleted != true).Select(x => _mapper.Map<TargetGroupDto>(x)).ToList();
            return await BaseResponse<List<TargetGroupDto>>.SuccessAsync(targetGroups);
        }

        public async Task<BaseResponse<TargetGroupDto>> AddAsync(TargetGroupInsertRequest request) 
        {
            var entity = new TargetGroupEntity()
            {
                Name = request.Name,
            };

            if (request.TargetList is { Count: > 0 })
            {
                entity.TargetGroupLines = new List<TargetGroupLineEntity>();
                request.TargetList.ForEach(x =>
                {
                    entity.TargetGroupLines.Add(new TargetGroupLineEntity()
                    {
                        TargetId = x
                    });
                });
            }

            entity = await _unitOfWork.GetRepository<TargetGroupEntity>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            var mappedTargetGroupDto = _mapper.Map<TargetGroupDto>(entity);
            return await BaseResponse<TargetGroupDto>.SuccessAsync(mappedTargetGroupDto);
        }

        public async Task<BaseResponse<TargetGroupDto>> UpdateAsync(TargetGroupUpdateRequest request) 
        {
            var entity = await _unitOfWork.GetRepository<TargetGroupEntity>()
                .GetAll(x => x.Id == request.Id && x.IsDeleted != true)
                .Include(x => x.TargetGroupLines.Where(t => t.IsDeleted != true))
                .FirstOrDefaultAsync();

            if (entity == null) throw new Exception("Hedef Grubu bulunamadı.");

            //Target Group
            entity.Name = request.Name;

            //Target GroupLines
            foreach (var targetGroupLineEntity in entity.TargetGroupLines)
            {
                await _unitOfWork.GetRepository<TargetGroupLineEntity>().DeleteAsync(targetGroupLineEntity);
            }
            if (request.TargetList is { Count: > 0 })
            {
                request.TargetList.ForEach(x =>
                {
                    var targetGroupLineEntity = new TargetGroupLineEntity()
                    {
                        TargetGroupId = request.Id,
                        TargetId = x,
                    };
                    _unitOfWork.GetRepository<TargetGroupLineEntity>().AddAsync(targetGroupLineEntity);
                });
            }

            await _unitOfWork.GetRepository<TargetGroupEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return await GetTargetGroupAsync(request.Id);

        }

        public async Task<BaseResponse<TargetGroupDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<TargetGroupEntity>().GetByIdAsync(id);
            if (entity == null)
                return null;

            entity.IsDeleted = true;
            await _unitOfWork.GetRepository<TargetGroupEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return await GetTargetGroupAsync(entity.Id);
        }

        public async Task<BaseResponse<TargetGroupDto>> DeleteLineAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<TargetGroupLineEntity>().GetByIdAsync(id);
            if (entity == null)
                return null;

            entity.IsDeleted = true;
            await _unitOfWork.GetRepository<TargetGroupLineEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return await GetTargetGroupAsync(entity.Id);
        }

        public async Task<BaseResponse<TargetGroupInsertFormDto>> GetInsertForm()
        {
            TargetGroupInsertFormDto response = new TargetGroupInsertFormDto();
            await FillForm(response);

            return await BaseResponse<TargetGroupInsertFormDto>.SuccessAsync(response);
        }

        public async Task<BaseResponse<TargetGroupUpdateFormDto>> GetUpdateForm(int id)
        {
            TargetGroupUpdateFormDto response = new TargetGroupUpdateFormDto();
            await FillForm(response);
            response.TargetGroup = (await GetTargetGroupAsync(id))?.Data;

            return await BaseResponse<TargetGroupUpdateFormDto>.SuccessAsync(response);
        }

        private async Task FillForm(TargetGroupInsertFormDto response)
        {
            response.TargetList = (await _targetService.GetListAsync())?.Data.Select(x => _mapper.Map<ParameterDto>(x)).ToList();
        }
    }
}
