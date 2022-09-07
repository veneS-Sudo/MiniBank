using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Core.Domains.Transfers.Services;
using Minibank.Core.Domains.Users.Services;
using Minibank.Core.UniversalValidators;

namespace Minibank.Core
{
    public static class Bootstraps
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IMoneyTransferService, MoneyTransferService>();
            services.AddScoped<ICommissionCalculator, CommissionCalculator>();
            services.AddScoped<IFractionalNumberEditor, FractionalNumberEditor>();

            //Add all validators from assembly
            services.AddValidatorsFromAssemblyContaining<IdEntityValidator>();
            //include only client-side validators
            services.AddFluentValidationClientsideAdapters();
            
            return services;
        }
    }
}