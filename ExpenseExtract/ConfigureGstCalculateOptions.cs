using ExpenseExtract.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseExtract
{
    public static class ConfigureGstCalculateOptionsExtension
    {
        public static void ConfigureGstCalculateOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GstCalculateOptions>(options =>
            {
                if (decimal.TryParse(configuration["GstRate"], out var rate))
                {
                    if (rate < 0 || rate > 1)
                    {
                        throw new InvalidConfigurationException("GstRate should be between 0 and 1");
                    }
                    options.Rate = rate;
                }
                else
                {
                    throw new InvalidConfigurationException("GstRate should be decimal");
                }
            });
        }
    }
}
