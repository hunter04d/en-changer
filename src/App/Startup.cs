using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnChanger.Database;
using EnChanger.Database.Abstractions;
using EnChanger.Infrastructure;
using EnChanger.Services;
using EnChanger.Services.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Serilog;

namespace EnChanger
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<IApplicationDbContext, ApplicationDbContext>(builder =>
                    builder.UseNpgsql(connString, options => options.UseNodaTime())
                );

            services.AddDataProtection();

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(_env.ContentRootPath, "vue/dist")));

            services.AddTransient<IPasswordService, PasswordService>();
            services.AddSingleton<IClock>(SystemClock.Instance);

            services.AddControllers(options =>
                {
                    options.ModelBinderProviders.Insert(0, new Base64GuidModelBinderProvider());
                })
                .AddNewtonsoftJson(settings =>
                    settings.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(_env.ContentRootPath, "vue/dist")),
                RequestPath = "/static",
            });
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
