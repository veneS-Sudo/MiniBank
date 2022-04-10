using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Core.Domains.Users.Services;

namespace Minibank.Core
{
    public static class Bootstraps
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();

            services.AddFluentValidation().AddValidatorsFromAssembly(typeof(IUserService).Assembly);
            return services;
        }
    }
}