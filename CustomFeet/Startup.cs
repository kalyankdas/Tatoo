using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Tatoo.Azure.BlobStorage;

namespace CustomFeet
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

           
        }
       public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options =>
            {
                options.SslPort = 44302;
                options.Filters.Add(new RequireHttpsAttribute());
            });
            services.AddSingleton(_ => Configuration);
            services.AddScoped<IImageManager, ImageManager>();
            
           
        }
       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
           
            loggerFactory.AddDebug();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                Authority = String.Format(Configuration["AzureAd:AadInstance"], Configuration["AzureAD:Tenant"]),
                Audience = Configuration["AzureAd:Audience"],
            });



          /*  var keyAsBase64 = Configuration["jwt:clientSecret"].Replace('_', '/').Replace('-', '+');
            var keyAsBytes = Convert.FromBase64String(keyAsBase64);

            var options = new JwtBearerOptions
            {
                TokenValidationParameters =
                {
                    ValidIssuer = $"https://{Configuration["jwt:domain"]}/",
                    ValidAudience = Configuration["jwt:clientId"],
                    IssuerSigningKey = new SymmetricSecurityKey(keyAsBytes)
                }
            };
            app.UseJwtBearerAuthentication(options);*/

            //app.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //    // You also need to update /wwwroot/app/scripts/app.js
            //    Authority = Configuration["jwt:authority"],
            //    Audience = Configuration["jwt:audience"],
            //    Events = new JwtBearerEvents()
            //    {
            //        OnAuthenticationFailed = c =>
            //        {
            //            c.HandleResponse();

            //            c.Response.StatusCode = 500;
            //            c.Response.ContentType = "text/plain";
            //            if (env.IsDevelopment())
            //            {
            //                // Debug only, in production do not share exceptions with the remote host.
            //                return c.Response.WriteAsync(c.Exception.ToString());
            //            }
            //            return c.Response.WriteAsync("An error occurred processing your authentication.");
            //        }
            //    }
            //});
            app.UseMvc();
        }
    }
}
