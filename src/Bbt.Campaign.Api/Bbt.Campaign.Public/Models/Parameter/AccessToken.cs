using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Parameter
{
    public class AccessToken
    {
        public string Token_type { get; set; }
        public string Access_token { get; set; }
        public int Expires_in { get; set; }
    }
}
