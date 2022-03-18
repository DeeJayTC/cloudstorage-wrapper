// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TCDev.CloudStorage.Extensions;

namespace TCDev.CloudStorage.Model
{
   /// <summary>
   ///    Basic Interface listing all API Calls a CSP should provide
   /// </summary>
   public interface ICloudStorageProvider
   {
      /// <summary>
      ///    Returns account info
      /// </summary>
      /// <param name="accessToken"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderAccountInfo> GetAccountInfoAsync(string accessToken, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns account info
      /// </summary>
      /// <param name="accessToken"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderDriveInfo> GetDriveInfoAsync(string accessToken, string driveId);

      /// <summary>
      ///    Returns account info
      /// </summary>
      /// <param name="accessToken"></param>
      /// <returns></returns>
      Task<List<ICloudStorageProviderDriveInfo>> GetAvailableDrivesAsync(string accessToken);

      /// <summary>
      ///    Returns Folder Info
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="folderId"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderItem> GetFolderInfoAsync(string accessToken, string folderId, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns Folder contents
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="folderId"></param>
      /// <param name="showFiles"></param>
      /// <param name="includeMounted"></param>
      /// <param name="includeMedia"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderListResult> GetFolderContentsAsync(string accessToken, string folderId, bool showFiles, bool includeMounted, bool includeMedia, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns Folder contents
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="path"></param>
      /// <param name="showFiles"></param>
      /// <param name="includeMounted"></param>
      /// <param name="includeMedia"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderListResult> GetFolderContentsByPathAsync(string accessToken, string path, bool showFiles, bool includeMounted, bool includeMedia, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns Folder contents by searching
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="folderId"></param>
      /// <param name="showFiles"></param>
      /// <param name="searchFor"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderListResult> GetFolderContentsSearchAsync(string accessToken, string folderId, bool showFiles, bool includeMounted, bool includeMedia, string searchFor, bool recursive, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns file details
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="fileId"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderItem> GetFileInfoAsync(string accessToken, string fileId, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns file details
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="fileId"></param>
      /// <param name="size"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderThumbnail> GetFileThumbnailAsync(string accessToken, string fileId, string size, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Upload new file to a folder
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="folderId"></param>
      /// <param name="file"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderItem> PostFile(string accessToken, string folderId, IFormFile file, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Update file content
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="fileId"></param>
      /// <param name="file"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderItem> PutFile(string accessToken, string fileId, IFormFile file, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Delete a file
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="fileId"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderItem> DeleteFile(string accessToken, string fileId, ExtraOptions extraOptions = null);

      /// <summary>
      ///    CreateFolder
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="folderId"></param>
      /// <param name="name"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderItem> CreateFolder(string accessToken, string folderId, string name, ExtraOptions extraOptions = null);

      /// <summary>
      ///    CopyItem
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="itemId"></param>
      /// <param name="name"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderItem> CopyItem(string accessToken, string itemId, string name, ExtraOptions extraOptions = null);

      /// <summary>
      ///    CopyItem
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="itemId"></param>
      /// <param name="linkType"></param>
      /// <param name="linkScope"></param>
      /// <returns></returns>
      Task<string> CreateLink(string accessToken, string itemId, string linkType, string linkScope, ExtraOptions extraOptions = null);
   }
}