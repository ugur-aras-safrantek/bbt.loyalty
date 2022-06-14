using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Models.Authorization;
using Bbt.Campaign.Services.Services.Parameter;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Services.Services.Authorization
{
    public class TokenHandler
    {
        public IConfiguration Configuration { get; set; }
        public TokenHandler(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        
    }
}
