using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Minibank.Web.Infrastructure.Authentication;

namespace Minibank.Web.Middlewares.AuthenticationMiddlewares
{
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthenticationValidator _authenticationValidator;

        public CustomAuthenticationMiddleware(RequestDelegate next, AuthenticationValidator authenticationValidator)
        {
            _next = next;
            _authenticationValidator = authenticationValidator;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out StringValues authToken))
            {
                (bool isNotExpired, DateTimeOffset ExpirationTime) validationTokenExpirationTime =
                    await _authenticationValidator.CheckTokenExpirationTime(authToken.ToString());

                if (!validationTokenExpirationTime.isNotExpired)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsJsonAsync(
                        $"Токен был просрочен в: '{validationTokenExpirationTime.ExpirationTime.ToString()}'");
                    return;    
                }
            }

            await _next(context);
        }
    }
}