using Bbt.Campaign.Public.Models.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Customer
{
    public class CustomerCampaignListFilterRequest : PagingRequest
    {
        public string CustomerCode { get; set; }
        public int PageTypeId { get; set; }
    }
}
