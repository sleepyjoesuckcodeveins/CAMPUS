using CAMPUS.DAL;
using Microsoft.EntityFrameworkCore;

namespace CAMPUS.PL.Registration
{
    public static class DataRegistration
    {
        public static IServiceCollection AddCampusData(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CampusDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'CampusDb' is missing. Set it with dotnet user-secrets (see A4).");
            }

            services.AddDbContext<CampusDbContext>(options =>
                options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

            return services;
        }
    }
}
