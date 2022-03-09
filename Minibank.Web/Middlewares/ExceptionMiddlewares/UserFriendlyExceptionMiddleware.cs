using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Minibank.Core.Exceptions.FriendlyException;

namespace Minibank.Web.Middlewares.ExceptionMiddlewares
{
    public class UserFriendlyExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public UserFriendlyExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UserFriendlyException friendlyException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(friendlyException.Message);
            }
        }
    }
}