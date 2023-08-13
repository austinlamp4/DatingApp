using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt => {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection")); //Uses builder.Configuration.GetConnectionString to get the connection string from the Config file
            }); //Adding our DB service, opt is specifying our options
            services.AddScoped<ITokenService, TokenService>();
            services.AddCors();

            return services;
        }
    }
}