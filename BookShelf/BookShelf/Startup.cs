using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookShelf.Core;
using BookShelf.Data;
using BookShelf.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.BrowserLink;

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
            
            services.AddDbContext<BooksDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("BookshelfDatabase")), ServiceLifetime.Scoped);

            services.AddTransient<BookDataSeeder>();
            services.AddTransient<IBookRepositoryService, BookRepositoryService>();
            services.AddTransient<IImageAdjuster, ImageAdjuster>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the middleware used by the HTTP Request/Response pipeline
        /// </summary>
        /// <param name="app">The Application Builder to add the middleware to</param>
        /// <param name="env">The Hosting Environment information</param>
        /// <param name="bookDataSeeder">Used for adding default book data to the database</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BookDataSeeder bookDataSeeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Books}/{action=Index}/{id?}");
            });

            bookDataSeeder.Seed();
        }
    }
}
