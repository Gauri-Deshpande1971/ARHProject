using System.Linq;
using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IFormGridService<>), typeof(FormGridService<>));
            services.AddScoped<IMastersService, MastersService>();
           // services.AddScoped<IUserService, UserService>();
            services.AddHttpClient();
            // services.AddScoped<IAppDesignService, AppDesignService>();
            // services.AddScoped<IDataSyncService, DataSyncService>();
            // services.AddHttpClient<IDataSyncHttpClient, DataSyncHttpClient>();
            services.AddScoped<IPaValidator, PaValidator>();

            services.Configure<ApiBehaviorOptions>(options => 
            {
                options.InvalidModelStateResponseFactory = actionContext => 
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToArray();
                    
                    var errorResponse = new ApiValidationErrorResponse
                        {
                            Errors = errors
                        };
                    
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}