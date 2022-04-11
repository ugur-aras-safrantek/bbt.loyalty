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
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ContractId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }

        public string? SectorName { get; set; }
        public string? ViewOptionName { get; set; }
        public string? ProgramTypeName { get; set; }
        public string? AchievementTypeName { get; set; }//kazanım tipi
        public string? JoinTypeName { get; set; } //rule - Dahil Olma Şekli


    }
}
