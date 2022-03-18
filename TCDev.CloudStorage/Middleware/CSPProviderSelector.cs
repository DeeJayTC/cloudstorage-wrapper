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
   public static class CSPProviderSelectorException
   {
      public static IApplicationBuilder UseTeamworkProviderSelector(this IApplicationBuilder builder)
      {
         return builder.UseMiddleware<CSPProviderSelector>();
      }
   }


   /// <summary>
   ///    Handles authentication to csp
   /// </summary>
   public class CSPProviderSelector
   {
      // The middleware delegate to call after this one finishes processing
      private readonly RequestDelegate _next;

      /// <summary>
      ///    Base Constructor
      /// </summary>
      public CSPProviderSelector(RequestDelegate next)
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
                  httpContext.Items["csp"] = providerType;
               // TEMPORARY: This line is causing CORS preflight check to fail. It's changing the response from 200 to 404, for some reason.
               //await _next.Invoke(httpContext);
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