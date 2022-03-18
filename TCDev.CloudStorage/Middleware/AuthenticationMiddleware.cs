// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TCDev.CloudStorage.Middleware
{
   public static class AuthenticationException
   {
      public static IApplicationBuilder UseTeamworkAuthentication(this IApplicationBuilder builder)
      {
         return builder.UseMiddleware<AuthenticationMiddleware>();
      }
   }

   /// <summary>
   ///    Handles authentication to csp
   /// </summary>
   public class AuthenticationMiddleware
   {
      // The middleware delegate to call after this one finishes processing
      private readonly RequestDelegate _next;

      /// <summary>
      ///    Base Constructor
      /// </summary>
      public AuthenticationMiddleware(RequestDelegate next)
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
            // Lets check if this might be a swagger request
            if (httpContext.Request.Path.Value.IndexOf("swagger", StringComparison.Ordinal) > 0)
            {
               await this._next.Invoke(httpContext);
               return;
            }

            // Lets check if this request is for our proxy system
            if (httpContext.Request.Path.Value.IndexOf("proxy", StringComparison.Ordinal) > 0)
               if (httpContext.Request.Headers["x-requested-for"] != string.Empty)
               {
                  await this._next.Invoke(httpContext);
                  return;
               }

            // Lets see if someone passed an access_token directly
            var token = httpContext.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
               httpContext.Items["accessToken"] = token;
               await this._next.Invoke(httpContext);
               return;
            }

            // No access - check header
            token = httpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(token))
            {
               httpContext.Items["accessToken"] = token.ToString().Replace("Bearer", "").Trim();
               await this._next.Invoke(httpContext);
               return;
            }

            // No access - check redis
            token = httpContext.Request.Query["token"];

            // No token -> return 401
            if (string.IsNullOrEmpty(token))
            {
               httpContext.Response.Clear();
               httpContext.Response.StatusCode = 401;
               await httpContext.Response.WriteAsync("You need to either pass access_token or an auth token for CSW");
            }
         }
         catch (Exception ex)
         {
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(ex));
         }
      }
   }
}