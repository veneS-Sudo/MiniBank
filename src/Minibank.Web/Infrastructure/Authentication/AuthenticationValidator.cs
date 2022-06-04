using System;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Minibank.Web.Infrastructure.Authentication
{
    public class AuthenticationValidator
    {
        public AuthenticationValidator()
        { }
        public async Task<(bool, DateTimeOffset)> CheckTokenExpirationTime(string authToken)
        {
            var base64Payload = authToken.Split('.').ElementAt(1);
            var bytePayload = Convert.FromBase64String(base64Payload);
            var jsonPayload = Encoding.UTF8.GetString(bytePayload);
            var obj = JsonSerializer.Deserialize<AuthPayload>(jsonPayload);
            if (obj == null)
            {
                throw new AuthenticationException("не удалось преобразовать токен аунтификации");
            }
            
            var expirationDateTime = DateTimeOffset.FromUnixTimeSeconds(obj.exp).ToLocalTime();
            if (DateTimeOffset.UtcNow > expirationDateTime)
            {
                return new ValueTuple<bool, DateTimeOffset>(false, expirationDateTime);
            }
            return new ValueTuple<bool, DateTimeOffset>(true, expirationDateTime);
        }
    }
}