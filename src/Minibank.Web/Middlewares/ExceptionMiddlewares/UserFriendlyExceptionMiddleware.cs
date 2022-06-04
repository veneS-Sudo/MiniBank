using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
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
            catch (ParametersValidationException validationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(validationException.Message);
            }
            catch (ObjectNotFoundException objectNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(objectNotFoundException.Message);
            }
            catch (NegativeAmountConvertException negativeAmountConvertException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(negativeAmountConvertException.Message);
            }
            catch (LackOfFundsException lackOfFundsException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(lackOfFundsException.Message);
            }
            catch (ValidationException exception)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = exception.Errors
                    .Select(error => $"{error.PropertyName}: {error.ErrorMessage}");

                await context.Response.WriteAsJsonAsync(errors);
            }
        }
    }
}