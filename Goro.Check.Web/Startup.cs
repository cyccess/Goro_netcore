using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Goro.Check.Cache;
using Goro.Check.Data;
using Goro.Check.Service;
using Newtonsoft.Json.Serialization;

namespace Goro.Check.Web
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
            WebConfig.ConnectionString = Configuration.GetConnectionString("SqlServerConnection");

            WebConfig.WebHost = Configuration.GetConnectionString("WebHost");

            WebConfig.APPID = Configuration["WechatConfig:APPID"];
            WebConfig.APPSECRET = Configuration["WechatConfig:APPSECRET"];
            WebConfig.MCHID = Configuration["WechatConfig:MCHID"];
            WebConfig.KEY = Configuration["WechatConfig:KEY"];

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "高罗微信审核系统API",
                    TermsOfService = "crp",
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddMemoryCache();

            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                CacheService.Init(cache);
                return cache;
            });
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IApiService, ApiService>();

            services.AddMvc().AddJsonOptions(op => op.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Goro API V1");
            });
        }
    }
}
