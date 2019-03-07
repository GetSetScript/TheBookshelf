using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookShelf.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookShelf
{
    /// <summary>
    /// Represents the custom configuration of the Dependency Injection Services and the HTTP Pipeline
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Creates a new instance of <see cref="Startup"/>
        /// </summary>
        /// <param name="configuration">An Object for retrieving the application configuration values</param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the Dependancey Injection Services
        /// </summary>
        /// <param name="services">The Service Collection to add the DI Services to</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the middleware used by the HTTP Request/Response pipeline
        /// </summary>
        /// <param name="app">The Application Builder to add the middleware to</param>
        /// <param name="env">The Hosting Environment information</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.useDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
