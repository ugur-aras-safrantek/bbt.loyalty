using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class CampaignReportListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ContractId { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }
        public bool IsContract { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public int? Order { get; set; }
        public int? SectorId { get; set; }
        public string? SectorName { get; set; }
        public int? ViewOptionId { get; set; }
        public string? ViewOptionName { get; set; }
        public int? ProgramTypeId { get; set; }
        public string? ProgramTypeName { get; set; }
        public string AchievementTypeId { get; set; }//kazanım tipi
        public string? AchievementTypeName { get; set; }
        public string? AchievementAmount { get; set; }
        public string? AchievementRate { get; set; }
        public string? TopLimitName { get; set; }
        public int? CustomerCount { get; set; }
        public int JoinTypeId { get; set; } //rule - Dahil Olma Şekli
        public string? JoinTypeName { get; set; }
    }
}
