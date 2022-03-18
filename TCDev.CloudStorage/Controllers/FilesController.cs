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
   [Route("/files")]
   public class FilesController : Controller
   {
      /// <summary>
      ///    Returns full details for the file
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the fileId or driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("{fileId}")]
      public async Task<object> GetFileDetails([FromRoute] string fileId)
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
                  var task = await provider.GetFileInfoAsync(accessToken, fileId, this.Request.GetExtraOptions());
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
      ///    Uploads one or multiple files
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the folderId or driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpPost]
      public async Task<object> UploadFile([FromForm] IFormFile file, [FromQuery] string folderId = "root")
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
                  var task = await provider.PostFile(accessToken, folderId, file, this.Request.GetExtraOptions());
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
      ///    Uploads a new revision of the file (form encoded data)
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the fileId or driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpPut("{fileId}")]
      public async Task<object> UpdateFile([FromRoute] string fileId, [FromForm] IFormFile file)
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
                  var task = await provider.PutFile(accessToken, fileId, file, this.Request.GetExtraOptions());
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
      ///    Uploads the contents of a file (form encoded data)
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the fileId or driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpPatch("{fileId}")]
      public async Task<object> UpdateFileContent([FromRoute] string fileId, IFormFile file)
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
                  var task = await provider.PutFile(accessToken, fileId, file, this.Request.GetExtraOptions());
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
      ///    Returns thumbnail images for the file
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the fileId or driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("{fileId}/thumbnails")]
      public async Task<object> GetFileThumbnails([FromRoute] string fileId, [FromQuery] string size)
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
                  var task = await provider.GetFileThumbnailAsync(accessToken, fileId, size, this.Request.GetExtraOptions());
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
      ///    Returns download links for the file
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the fileId or driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("{fileId}/download")]
      public async Task<object> GetFileDownloadLinks([FromRoute] string fileId)
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
                  var task = await provider.GetFileInfoAsync(accessToken, fileId, this.Request.GetExtraOptions());
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
      ///    Deletes the file
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the fileId or driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpDelete("{fileId}")]
      public async Task<object> DeleteFile([FromRoute] string fileId)
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
                  var task = await provider.DeleteFile(accessToken, fileId, this.Request.GetExtraOptions());
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
      ///    Searches all contents of the drive
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderFolderInfo" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderFolderInfo">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the folderId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpGet("search")]
      public async Task<object> GetFolderSearch([FromQuery] bool showFiles, [FromQuery] string searchTerm, [FromQuery] bool recursive)
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
                  var task = await provider.GetFolderContentsSearchAsync(accessToken, "", showFiles, true, true, searchTerm, recursive, this.Request.GetExtraOptions());
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
      ///    Creates a copy of the file and returns the newly created file details
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderItem" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderItem">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the fileId or driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpPost("{fileId}/copy")]
      public async Task<object> CopyFile([FromRoute] string fileId, [FromQuery] string name)
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
                  var task = await provider.CopyItem(accessToken, fileId, name, this.Request.GetExtraOptions());
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
      ///    Creates a copy of the file and returns the newly created file details
      /// </summary>
      /// <returns>
      ///    <see cref="string" />
      /// </returns>
      /// <response code="200" cref="string">OK</response>
      /// <response code="401">If the given token is invalid, authentication failed</response>
      /// <response code="404">If nothing could be found for the fileId or driveId</response>
      /// <response code="501">If the integration is disabled for the provider</response>
      /// <response code="503">If there is no implementation for the requested provider</response>
      [HttpPost("{fileId}/link")]
      public async Task<object> CreateLink([FromRoute] string fileId, [FromQuery] string linkType, [FromQuery] string linkScope)
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
                  var task = await provider.CreateLink(accessToken, fileId, linkType, linkScope, this.Request.GetExtraOptions());
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