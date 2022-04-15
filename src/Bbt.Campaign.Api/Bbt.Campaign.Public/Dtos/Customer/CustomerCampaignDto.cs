using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Customer
{
    public class CustomerCampaignDto
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public int CampaignId { get; set; }
        public bool IsJoin { get; set; }
        public bool IsFavorite { get; set; }
    }
}
