// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using TCDev.CloudStorage.Extensions;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Controllers
{
   /// <summary>
   ///    Account Controller for CSP requests
   /// </summary>
   [Produces("application/json")]
   [Route("/account")]
   public class AccountController : Controller
   {
      /// <summary>
      ///    Returns full info for the cloud storage provider accessible by the access_token
      /// </summary>
      /// <response code="200">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [ProducesResponseType(typeof(ICloudStorageProviderAccountInfo), 200)]
      [HttpGet]
      public async Task<object> Get([FromQuery] string extraOptions = "")
      {
         try
         {
            if (this.Request.IsValid())
            {
               var requestData = this.Request.GetData();
               var provider = requestData.GetProvider();
               if (provider != null)
               {
                  // Lets get the access token from auth info
                  var accessToken = this.Request.HttpContext.Items["accessToken"].ToString();

                  // Forward the call to the provider implementation we need
                  var task = await provider.GetAccountInfoAsync(accessToken, this.Request.GetExtraOptions());
                  return Json(task);
               }

               this.Response.StatusCode = (int) HttpStatusCode.NoContent;
               return Content("No valid provider was able to take the call");
            }

            this.Response.StatusCode = (int) HttpStatusCode.NotImplemented;
            return Content("The requested provider is not available or not implemented");
         }
         catch (ServiceException ex)
         {
            this.Response.StatusCode = (int) ex.StatusCode;
            return Content(ex.Message);
         }
         catch (Exception ex)
         {
            this.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return Content(ex.Message);
         }
      }
   }
}