

namespace Bbt.Campaign.Public.Models.Authorization
{
    public class LoginRequest
    {
        public string UserId { get; set; }
        public string? Roles { get; set; }
    }
}
