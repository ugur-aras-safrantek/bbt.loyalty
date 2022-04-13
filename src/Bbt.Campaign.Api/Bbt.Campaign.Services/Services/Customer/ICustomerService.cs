using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Services.Services.Customer
{
    public interface ICustomerService
    {
        public Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerViewMinFormAsync(int campaignId, string contentRootPath);
    }
}
