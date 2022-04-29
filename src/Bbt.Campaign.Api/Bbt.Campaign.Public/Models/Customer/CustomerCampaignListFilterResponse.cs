using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.Customer;
using Bbt.Campaign.Public.Models.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Customer
{
    public class CustomerCampaignListFilterResponse : PagingResponse
    {
        public CustomerCampaignListFilterResponse()
        {
            CustomerCampaignList = new List<CustomerCampaignMinListDto>();
        }

        public List<int> FavoriteList { get; set; }
        public List<CustomerCampaignMinListDto> CustomerCampaignList { get; set; }
    }
}
