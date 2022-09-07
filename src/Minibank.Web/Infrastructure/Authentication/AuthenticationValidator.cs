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
        public Task<(bool, DateTimeOffset)> CheckTokenExpirationTime(string authToken)
        {
            var base64Payload = authToken.Split('.').ElementAt(1);
            var bytePayload = ParseBase64WithoutPadding(base64Payload);
            var jsonPayload = Encoding.UTF8.GetString(bytePayload);
            var obj = JsonSerializer.Deserialize<AuthPayload>(jsonPayload);
            if (obj == null)
            {
                throw new AuthenticationException("не удалось преобразовать токен аунтификации");
            }
            
            var expirationDateTime = DateTimeOffset.FromUnixTimeSeconds(obj.exp).ToLocalTime();
            if (DateTimeOffset.UtcNow > expirationDateTime)
            {
                return Task.FromResult(new ValueTuple<bool, DateTimeOffset>(false, expirationDateTime));
            }
            return Task.FromResult(new ValueTuple<bool, DateTimeOffset>(true, expirationDateTime));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "==";
                    break;
                case 3: base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}