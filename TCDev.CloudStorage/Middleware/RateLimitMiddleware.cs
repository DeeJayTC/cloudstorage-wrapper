//// TCDev 2022/03/17
//// Apache 2.0 License
//// https://www.github.com/deejaytc/dotnet-utils

//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Newtonsoft.Json;
//using TCDev.CloudStorage.Services;

//namespace TCDev.CloudStorage.Middleware
//{
//   /// <summary>
//   ///    Rate limit middleware to make sure we're properly checking against rate limits of third parties when trying to
//   ///    forward calls.
//   ///    Current setting is 1000 calls in 1 minute for the same accessToken
//   /// </summary>
//   public static class RateLimitMiddlewareException
//   {
//      public static IApplicationBuilder UseRateLimitMiddleware(this IApplicationBuilder builder)
//      {
//         return builder.UseMiddleware<RateLimitMiddleware>();
//      }
//   }


//   public class RateLimitMiddleware
//   {
//      // The middleware delegate to call after this one finishes processing
//      private readonly RequestDelegate _next;
//      private readonly RateLimitService _service;

//      /// <summary>
//      ///    Base Constructor
//      /// </summary>
//      public RateLimitMiddleware(RequestDelegate next, RateLimitService rateLimitService)
//      {
//         this._next = next;
//         this._service = rateLimitService;
//      }

//      /// <summary>
//      ///    Main implementation for middleware
//      /// </summary>
//      /// <param name="httpContext"></param>
//      /// <returns></returns>
//      public async Task Invoke(HttpContext httpContext)
//      {
//         try
//         {
//            // Do we already have an access token?
//            string token = null;
//            if (httpContext != null && httpContext.Items.ContainsKey("accessToken") && httpContext.Items["accessToken"] != null) token = httpContext.Items["accessToken"].ToString();

//            if (string.IsNullOrEmpty(token)) token = httpContext.Request.Query["access_token"];
//            if (!string.IsNullOrEmpty(token)) token.Replace("Bearer", "").Trim();

//            // If we have a token at this stage, we check the rate limit
//            if (!string.IsNullOrEmpty(token))
//            {
//               var isChecked = this._service.CheckRateGate(token);
//               if (isChecked) await this._next.Invoke(httpContext);
//               return;
//            }

//            //if we do not have a token, just pass the call through...won't work anyway
//            await this._next.Invoke(httpContext);
//         }
//         catch (Exception ex)
//         {
//            httpContext.Response.StatusCode = 500;
//            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(ex));
//         }
//      }
//   }
//}