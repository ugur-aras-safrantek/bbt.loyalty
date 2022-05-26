using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Services.Services.Remote
{
    public interface IRemoteService
    {
        public Task<List<string>> GetChannelList();
    }
}
