using ExpenseExtract.Exceptions;
using ExpenseExtract.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseExtract
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GstCalculateOptions>(options =>
            {
                if (decimal.TryParse(Configuration["GstRate"], out var rate))
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
            services.AddSingleton<IExpenseExtractService, ExpenseExtractService>();
            services.AddSingleton<IGstCalculateService, GstCalculateService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
