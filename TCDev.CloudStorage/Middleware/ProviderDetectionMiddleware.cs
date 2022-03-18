// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TCDev.CloudStorage.Extensions;

namespace TCDev.CloudStorage.Middleware
{
   /// <summary>
   ///    Extension for ProviderDetection Middleware
   /// </summary>
   public static class ProviderDetectionServiceExtension
   {
      /// <summary>
      ///    Enable Teamwork Provider Selector for CSP
      /// </summary>
      /// <param name="builder"></param>
      /// <returns></returns>
      public static IApplicationBuilder UseTeamworkProviderDetection(this IApplicationBuilder builder)
      {
         return builder.UseMiddleware<ProviderDetectionService>();
      }
   }


   /// <summary>
   ///    Handles authentication to csp
   /// </summary>
   public class ProviderDetectionService
   {
      // The middleware delegate to call after this one finishes processing
      private readonly RequestDelegate _next;

      /// <summary>
      ///    Base Constructor
      /// </summary>
      public ProviderDetectionService(RequestDelegate next)
      {
         this._next = next;
      }

      /// <summary>
      ///    Main implementation for middleware
      /// </summary>
      /// <param name="httpContext"></param>
      /// <returns></returns>
      public async Task Invoke(HttpContext httpContext)
      {
         try
         {
            var param = httpContext.Request.Query["provider"];
            if (!string.IsNullOrEmpty(param))
            {
               var providerType = ProviderRegistry.GetProviderByName(param);
               if (providerType != null)
               {
                  httpContext.Items["csp"] = providerType;
                  await this._next.Invoke(httpContext);
               }
            }
         }
         catch (Exception ex)
         {
            await httpContext.Response.WriteAsync("An error occured while analyzing your call, make sure to pass the correct params, message: " + ex.Message);
            httpContext.Response.StatusCode = 500;
         }
      }
   }
}