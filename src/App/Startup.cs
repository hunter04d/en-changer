using System.IO;
using EnChanger.Database;
using EnChanger.Database.Abstractions;
using EnChanger.Infrastructure;
using EnChanger.Services;
using EnChanger.Services.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
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
            var clientAppDir = _configuration.GetValue<string>("ClientAppDir");
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
                    options.UseNpgsql(connString, npgSql => npgSql.UseNodaTime())
                );

            services.AddDataProtection();

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(_env.ContentRootPath, clientAppDir)));

            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<ISessionService, SessionService>();
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
            var clientAppDir = _configuration.GetValue<string>("ClientAppDir");
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(_env.ContentRootPath, clientAppDir)),
                RequestPath = "/static",
            });
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
