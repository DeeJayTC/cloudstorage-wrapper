// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Collections.Generic;
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
   [ProducesResponseType(typeof(List<ICloudStorageProviderDriveInfo>), 200)]
   [Route("/drives")]
   public class DrivesController : Controller
   {
      /// <summary>
      ///    Returns available drives for the specified site
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderDriveInfo" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderDriveInfo">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the siteId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet]
      public async Task<object> Get([FromQuery] string extraOptions)
      {
         try
         {
            if (this.Request.IsValid())
            {
               var requestData = this.Request.GetData();
               var provider = requestData.GetProviderWithSites();
               if (provider != null)
               {
                  // Lets get the access token from auth info
                  var accessToken = this.Request.HttpContext.Items["accessToken"].ToString();

                  // Forward the call to the provider implementation we need
                  if (provider != null)
                  {
                     var task = await provider.GetDrivesAsync(accessToken, "", this.Request.GetExtraOptions());
                     return Json(task);
                  }
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

      /// <summary>
      ///    Returns drive info for the specified drive ID
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderDriveInfo" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderDriveInfo">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("{driveId}")]
      public async Task<object> GetDriveInfo([FromRoute] string driveId, [FromQuery] string extraOptions)
      {
         try
         {
            if (this.Request.IsValid())
            {
               var requestData = this.Request.GetData();
               var provider = requestData.GetProviderWithSites();
               if (provider != null)
               {
                  // Lets get the access token from auth info
                  var accessToken = this.Request.HttpContext.Items["accessToken"].ToString();

                  // Forward the call to the provider implementation we need
                  if (provider != null)
                  {
                     var task = await provider.GetDriveInfoAsync(accessToken, driveId, this.Request.GetExtraOptions());
                     return Json(task);
                  }
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