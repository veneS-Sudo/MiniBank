using System;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Minibank.Core.Exceptions.FriendlyExceptions;

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
            catch (ValidationException validationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(validationException.Message);
            }
            catch (ObjectNotFoundException objectNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(objectNotFoundException.Message);
            }
            catch (FluentValidation.ValidationException exception)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = exception.Errors
                    .Select(error => $"{error.PropertyName}: {error.ErrorMessage}");

                await context.Response.WriteAsJsonAsync(errors);
            }
        }
    }
}