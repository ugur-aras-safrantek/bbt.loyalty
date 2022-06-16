namespace Bbt.Campaign.Api.Base
{
    public static class General
    {
        public static string GetUserIdFromHeader(HttpRequest request) 
        {

            if(request.Headers.TryGetValue("userid", out var value)) 
            {
                return value; 
            }
            else 
            {
                throw new Exception("userid giriniz");
            }
        }

        public static void CheckAuthorization(HttpRequest request) 
        { 
        
        }

    }

    
}
