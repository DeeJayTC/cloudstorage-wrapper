// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace TCDev.CloudStorage.Middleware
{
   public class CorsMiddleware
   {
      private readonly RequestDelegate _next;

      public CorsMiddleware(RequestDelegate next)
      {
         this._next = next;
      }

      public Task Invoke(HttpContext httpContext)
      {
         httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
         httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
         httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Authorization, X-CSRF-Token, X-Requested-For, X-Requested-With, Accept, Accept-Version, Content-Length, Content-Type, Content-MD5, Date, X-Api-Version, X-File-Name");
         httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");

         if (httpContext.Request.Method == "OPTIONS")
         {
            httpContext.Response.StatusCode = 200;
            return httpContext.Response.WriteAsync("OK");
         }


         return this._next(httpContext);
      }
   }

   // Extension method used to add the middleware to the HTTP request pipeline.
   public static class CorsMiddlewareExtensions
   {
      public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder builder, string v)
      {
         return builder.UseMiddleware<CorsMiddleware>();
      }
   }
}