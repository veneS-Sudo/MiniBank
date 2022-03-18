using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Data.CurrencyProviders;
using Minibank.Data.Users.Accounts.Repositories;
using Minibank.Data.Users.Repositories;

namespace Minibank.Data
{
    public static class Bootstraps
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ICurrencyRateProvider, CurrencyRateHttpProvider>(option =>
                option.BaseAddress = new Uri(configuration["CbrDailyUri"]));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();

            return services;
        }
    }
}