using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Minibank.Core;
using Minibank.Data;
using Minibank.Web.HostedServices;
using Minibank.Web.Infrastructure.Authentication;
using Minibank.Web.Infrastructure.Swagger;
using Minibank.Web.Middlewares.AuthenticationMiddlewares;
using Minibank.Web.Middlewares.ExceptionMiddlewares;

namespace Minibank.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerAuthentication(Configuration);

            services.AddHostedService<MigrationHostedService>();
            
           services.AddCore();
           services.AddData(Configuration);

            services.AddSingleton<AuthenticationValidator>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Audience = "api";
                options.Authority = Environment.GetEnvironmentVariable("AUTHENTICATION_AUTHORITY") ??
                                    Configuration.GetConnectionString("AUTHENTICATION_AUTHORITY");
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Minibank API v1.0"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<CustomAuthenticationMiddleware>();
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseMiddleware<UserFriendlyExceptionMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
