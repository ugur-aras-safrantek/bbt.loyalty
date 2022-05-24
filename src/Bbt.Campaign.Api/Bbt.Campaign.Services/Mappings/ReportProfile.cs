using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos.Report;

namespace Bbt.Campaign.Services.Mappings
{
    public class ReportProfile : Profile
    {
        public ReportProfile() 
        { 
            CreateMap<CustomerReportDetailDto, CustomerReportDetailEntity>().ReverseMap();
        } 
    }
}
