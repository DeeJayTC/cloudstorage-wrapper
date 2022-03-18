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
   ///    Sites Controller
   /// </summary>
   [Produces("application/json")]
   [ProducesResponseType(typeof(List<ICloudStorageProviderSiteInfo>), 200)]
   [Route("/sites")]
   public class SitesController : Controller
   {
      /// <summary>
      ///    Returns a list of sites (Sharepoint only)
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderSiteInfo" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderSiteInfo">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet]
      public async Task<object> Get([FromQuery] string extraOptions)
      {
         try
         {
            if (this.Request.IsValid())
            {
               if (this.Request.GetProviderName() != "sharepoint") return Content("This can only be used for Sharepoint");

               var requestData = this.Request.GetData();
               var provider = requestData.GetProviderWithSites();
               if (provider != null)
               {
                  // Lets get the access token from auth info
                  var accessToken = this.Request.HttpContext.Items["accessToken"].ToString();
                  var task = await provider.GetSitesAsync(accessToken, this.Request.GetExtraOptions());
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


      /// <summary>
      ///    Returns full details for the site  (Sharepoint only)
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderSiteInfo" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderSiteInfo">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the siteId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("{siteId}")]
      public async Task<object> GetSiteDetails([FromRoute] string siteId, [FromQuery] string extraOptions)
      {
         try
         {
            if (this.Request.IsValid())
            {
               if (this.Request.GetProviderName() != "sharepoint") return Content("This can only be used for Sharepoint");

               var requestData = this.Request.GetData();
               var provider = requestData.GetProviderWithSites();
               if (provider != null)
               {
                  // Lets get the access token from auth info
                  var accessToken = this.Request.HttpContext.Items["accessToken"].ToString();
                  var task = await provider.GetSiteInfoAsync(accessToken, siteId, this.Request.GetExtraOptions());
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


      /// <summary>
      ///    Searches the available sites (Sharepoint only)
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderSiteInfo" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderSiteInfo">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("search")]
      public async Task<object> GetSitesSearch([FromQuery] string searchFor, [FromQuery] string extraOptions)
      {
         try
         {
            if (this.Request.IsValid())
            {
               if (this.Request.GetProviderName() != "sharepoint") return Content("This can only be used for Sharepoint");

               var requestData = this.Request.GetData();
               var provider = requestData.GetProviderWithSites();
               if (provider != null)
               {
                  // Lets get the access token from auth info
                  var accessToken = this.Request.HttpContext.Items["accessToken"].ToString();
                  var task = await provider.GetSitesSearchAsync(accessToken, searchFor, this.Request.GetExtraOptions());
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


      /// <summary>
      ///    Returns available drives for the specified site (Sharepoint only)
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderSiteInfo" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderSiteInfo">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the siteId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("{siteId}/drives")]
      public async Task<object> GetSiteDrives([FromRoute] string siteId, [FromQuery] string extraOptions)
      {
         try
         {
            if (this.Request.IsValid())
            {
               if (this.Request.GetProviderName() != "sharepoint") return Content("This can only be used for Sharepoint");

               var requestData = this.Request.GetData();
               var provider = requestData.GetProviderWithSites();
               if (provider != null)
               {
                  // Lets get the access token from auth info
                  var accessToken = this.Request.HttpContext.Items["accessToken"].ToString();
                  var task = await provider.GetDrivesAsync(accessToken, siteId, this.Request.GetExtraOptions());
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