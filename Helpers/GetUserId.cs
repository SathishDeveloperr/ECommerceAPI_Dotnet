using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ECommerceAPI.Helpers
{
    public static class GetUserId
    {
        public static int? GetUserIdFromJwtToken(string jwtToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    return null;
                }

                var userIdClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "sub");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
