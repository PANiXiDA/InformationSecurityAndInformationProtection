using BL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BL.Standard
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            services.AddScoped<IUsersBL, UsersBL>();
            services.AddScoped<ITokensBL, TokensBL>();

            return services;
        }
    }
}
