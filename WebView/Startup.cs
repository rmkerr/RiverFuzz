using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaptureParse.Parsers;
using Database;
using Database.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebView.Models;
using WebView.Models.Services;

namespace WebView
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
            services.AddTransient<IFuzzerRepository, FuzzerRepository>();
            services.AddSingleton<IDatabaseHelper, DatabaseHelper>();

            //TODO: add logging to classes to make use of DI
            //tODO: may want to use the fancy "register all of type" extension later
            services.AddTransient<ICaptureParse, TextCaptureParse>();
            services.AddTransient<ICaptureParse, BurpCaptureParse>();
            services.AddSingleton<IParserFactory, ParserFactory>();
            services.AddTransient<ParserService>();

            // TODO: Evaluate if this is the right way to do this:
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddControllersWithViews();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IDatabaseHelper dbHelper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=FuzzerRun}/{action=Summary}");
            });

            dbHelper.CreateIfNotExists();
        }
    }
}
