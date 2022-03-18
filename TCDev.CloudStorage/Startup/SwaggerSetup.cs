// TCDev.de 2022/03/18
// TCDev.CloudStorage.SwaggerSetup.cs
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using TCDev.CloudStorage.Utility;

namespace TCDev.CloudStorage.StartupExtension
{
   public static class SwaggerSetup
   {
      /// <summary>
      ///    Add and configure swagger services
      /// </summary>
      /// <param name="services"></param>
      /// <returns></returns>
      public static IServiceCollection SetupSwagger(this IServiceCollection services, IConfiguration config)
      {
         var swaggerConfig = new SwaggerConfig();
         config.Bind("Swagger", swaggerConfig);

         services.AddSwaggerGen(c =>
         {
            c.SwaggerDoc(swaggerConfig.Version,
               new OpenApiInfo
               {
                  Title = swaggerConfig.Name,
                  Version = swaggerConfig.Version,
                  Description = swaggerConfig.Description,
                  Contact = new OpenApiContact
                  {
                     Email = swaggerConfig.ContactEmail, Name = swaggerConfig.ContactName, Url = new Uri(swaggerConfig.WebsiteUrl)
                  }
               });

            c.OperationFilter<SwaggerAddRequiredParameters>();


            // Set the comments path for the Swagger JSON and UI.
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var xmlPath = Path.Combine(basePath, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            c.IncludeXmlComments(xmlPath);
         });
         return services;
      }


      public static IApplicationBuilder UseSwaggerConfigured(this IApplicationBuilder app)
      {
         var swaggerConfig = AppSettings.Instance.Get<SwaggerConfig>("Swagger");

         app.UseSwagger();
         app.UseSwaggerUI(
            c =>
               c.SwaggerEndpoint(
                  swaggerConfig.Endpoint,
                  swaggerConfig.Name
               ));


         return app;
      }
   }
}
