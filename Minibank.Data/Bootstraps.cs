using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Transfers.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Data.DatabaseLayer.Context;
using Minibank.Data.DatabaseLayer.DbModels.Accounts.Repositories;
using Minibank.Data.DatabaseLayer.DbModels.Transfers.Repositories;
using Minibank.Data.DatabaseLayer.DbModels.Users.Repositories;
using Minibank.Data.Providers.CurrencyProviders;

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
            services.AddScoped<IMoneyTransferRepository, MoneyTransferRepository>();

            services.AddDbContext<MinibankContext>(
                option =>
                    option.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                        npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(MinibankContext).Assembly.FullName)));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            return services;
        }
    }
}