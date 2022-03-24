using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Minibank.Web.Middlewares.ExceptionMiddlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync("Внутренняя ошибка сервера");
            }
        }
    }
}