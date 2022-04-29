using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Customer
{
    public class SetFavoriteRequest
    {
        public string CustomerCode { get; set; }
        public int CampaignId { get; set; }
        public bool IsFavorite { get; set; }
    }
}
