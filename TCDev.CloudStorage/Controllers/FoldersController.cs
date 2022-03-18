// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
   [ProducesResponseType(typeof(List<ICloudStorageProviderItem>), 200)]
   [Route("/folders")]
   public class FoldersController : Controller
   {
      /// <summary>
      ///    Returns a list of the folder contents
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the folderId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("{folderId}/list")]
      public async Task<object> GetFolderContents([FromRoute] string folderId, [FromQuery] bool showFiles, [FromQuery] bool includeMounted, [FromQuery] bool includeMedia, [FromQuery] string extraOptions)
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
                  if (provider != null)
                  {
                     var task = await provider.GetFolderContentsAsync(accessToken, folderId, showFiles, includeMounted, includeMedia, this.Request.GetExtraOptions());
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
      ///    Returns a list of the folder contents
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the folderId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("list")]
      public async Task<object> GetFolderContentsByPath([FromQuery] string path, [FromQuery] bool showFiles, [FromQuery] bool includeMounted, [FromQuery] bool includeMedia, [FromQuery] string extraOptions)
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
                  if (provider != null)
                  {
                     var task = await provider.GetFolderContentsByPathAsync(accessToken, path, showFiles, includeMounted, includeMedia, this.Request.GetExtraOptions());
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
      ///    Returns full details for the folder
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderFolderInfo" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderFolderInfo">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the folderId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("{folderId}")]
      public async Task<object> GetFolderDetails([FromRoute] string folderId, [FromQuery] string driveId)
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
                  if (provider != null)
                  {
                     var task = await provider.GetFolderInfoAsync(accessToken, folderId, this.Request.GetExtraOptions());
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
      ///    Searches the given folder and its subfolders
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderFolderInfo" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderFolderInfo">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the folderId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("{folderId}/search")]
      public async Task<object> GetFolderSearch([FromRoute] string folderId, [FromQuery] bool showFiles, [FromQuery] string searchFor, [FromQuery] bool recursive, [FromQuery] string driveId)
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
                  if (provider != null)
                  {
                     var task = await provider.GetFolderContentsSearchAsync(accessToken, folderId, showFiles, true, true, searchFor, recursive, this.Request.GetExtraOptions());
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
      ///    Creates a new folder
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the folderId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpPost("{folderId}")]
      public async Task<object> CreateFolder([FromRoute] string folderId, [FromQuery] string name, [FromQuery] string driveId)
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
                  if (provider != null)
                  {
                     var task = await provider.CreateFolder(accessToken, folderId, name, this.Request.GetExtraOptions());
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
      ///    Deletes the folder
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the folderId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpDelete("{folderId}")]
      public async Task<object> DeleteFolder([FromRoute] string folderId, [FromQuery] string driveId)
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
                  if (provider != null)
                  {
                     var task = await provider.DeleteFile(accessToken, folderId, this.Request.GetExtraOptions());
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
      ///    Uploads one or multiple files to the folder
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the folderId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpPost("{folderId}/files")]
      public async Task<object> UploadFile([FromRoute] string folderId, [FromForm] IFormFile file, [FromQuery] string driveId)
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
                  if (provider != null)
                  {
                     var task = await provider.PostFile(accessToken, folderId, file, this.Request.GetExtraOptions());
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