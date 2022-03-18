// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Http;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.Dropbox
{
   public class DropboxProvider : ICloudStorageProvider
   {
      public async Task<ICloudStorageProviderAccountInfo> GetAccountInfoAsync(string accessToken, ExtraOptions extraOptions = null)
      {
         using (var client = new DropboxClient(accessToken))
         {
            var info = await client.Users.GetCurrentAccountAsync();
            return new DropboxAccountInfo(info);
         }
      }

      public async Task<ICloudStorageProviderListResult> GetFolderContentsAsync(string accessToken, string folderId, bool showFiles, bool includeMounted, bool includeMedia, ExtraOptions extraOptions = null)
      {
         using (var client = new DropboxClient(accessToken))
         {
            var result = new DropboxListResult {Contents = new List<ICloudStorageProviderItem>()};
            ListFolderResult folderContent;
            if (folderId == "root")
            {
               result.FolderInfo = new DropboxItemInfo
               {
                  Hash = "/", IsDirectory = true, Id = "root", IsRoot = true, Name = "root", Modified = DateTime.MinValue, HasThumbs = false, Icon = "folder", IsShared = false
               };
               folderContent = await client.Files.ListFolderAsync(new ListFolderArg(string.Empty, includeMountedFolders: includeMounted, includeMediaInfo: includeMedia));
            }
            else
            {
               if (folderId.IndexOf("id:", StringComparison.Ordinal) == -1 && folderId.IndexOf(@"/") == -1) folderId = "id:" + folderId;
               var folderinfo = await client.Files.GetMetadataAsync(folderId, true, false, true);
               result.FolderInfo = new DropboxItemInfo(folderinfo);
               folderContent = await client.Files.ListFolderAsync(new ListFolderArg(folderId, includeMountedFolders: includeMounted, includeMediaInfo: includeMedia));
            }

            Parallel.ForEach(folderContent.Entries, item =>
            {
               if (item.IsFile && showFiles) result.Contents.Add(new DropboxItemInfo(item));
               if (!item.IsFile) result.Contents.Add(new DropboxItemInfo(item));
            });

            while (folderContent.HasMore)
            {
               folderContent = await client.Files.ListFolderContinueAsync(folderContent.Cursor);
               Parallel.ForEach(folderContent.Entries, item =>
               {
                  if (item.IsFile && showFiles) result.Contents.Add(new DropboxItemInfo(item));
                  if (!item.IsFile) result.Contents.Add(new DropboxItemInfo(item));
               });
            }

            result.Contents = result.Contents.OrderByDescending(p => p.IsDirectory).ThenBy(p => p.Name).ThenBy(p => p.Modified).ToList();

            return result;
         }
      }

      public async Task<ICloudStorageProviderListResult> GetFolderContentsSearchAsync(
         string accessToken, 
         string folderId, 
         bool showFiles, 
         bool includeMounted, 
         bool includeMedia, 
         string searchFor,
         bool recursive, 
         ExtraOptions extraOptions = null)
      {
         using (var client = new DropboxClient(accessToken))
         {
            var result = new DropboxListResult {Contents = new List<ICloudStorageProviderItem>()};
            SearchV2Result folderContent;

            var options = new SearchOptions();
            
            if (folderId == "root")
            {
               result.FolderInfo = new DropboxItemInfo
               {
                  Hash = "/", 
                  IsDirectory = true, 
                  Id = "root", 
                  IsRoot = true, 
                  Name = "root", 
                  Modified = DateTime.MinValue, 
                  HasThumbs = false, 
                  Icon = "folder", 
                  IsShared = false
               };
               folderContent = await client.Files.SearchV2Async(new SearchV2Arg(searchFor, options));
            }
            else
            {
               if (folderId.IndexOf("id:", StringComparison.Ordinal) == -1) folderId = "id:" + folderId;
               //var folderinfo = await client.Files.GetMetadataAsync(folderId, true, false, true);
               //result.FolderInfo = new DropboxItemInfo(folderinfo);
               folderContent = await client.Files.SearchV2Async(new SearchV2Arg(searchFor, new SearchOptions(folderId)));
            }


            Parallel.ForEach(folderContent.Matches, item =>
            {
               if (item.Metadata.IsMetadata && item.Metadata.AsMetadata.Value.IsFile && showFiles) result.Contents.Add(new DropboxItemInfo(item.Metadata));
               if (!item.Metadata.IsMetadata && item.Metadata.AsMetadata.Value.IsFile) result.Contents.Add(new DropboxItemInfo(item.Metadata));
            });

            //todo: Paging
            //if (folderContent.More)
            //{
            //    while (folderContent.More)
            //    {
            //        folderContent = await client.Files.SearchAsync(new SearchArg(pa));
            //        Parallel.ForEach(folderContent.Matches, item =>
            //        {
            //            if (item.Metadata.IsFile && showFiles)
            //            {
            //                result.content.Add(new DropboxItemInfo(item));
            //            }
            //            if (!item.Metadata.IsFile)
            //            {
            //                result.content.Add(new DropboxItemInfo(item));
            //            }
            //        });
            //    }
            //}
            result.Contents =
               result.Contents.OrderByDescending(p => p.IsDirectory).ThenBy(p => p.Name).ThenBy(p => p.Modified).ToList();
            return result;
         }
      }

      public async Task<ICloudStorageProviderItem> GetFileInfoAsync(string accessToken, string fileId, ExtraOptions extraOptions = null)
      {
         using (var client = new DropboxClient(accessToken))
         {
            if (fileId.IndexOf("id:", StringComparison.Ordinal) == -1) fileId = "id:" + fileId;
            var fileInfo = await client.Files.GetMetadataAsync(fileId, true, false, true);

            return new DropboxItemInfo(fileInfo);
         }
      }

      public async Task<ICloudStorageProviderItem> GetFolderInfoAsync(string accessToken, string folderId, ExtraOptions extraOptions = null)
      {
         using (var client = new DropboxClient(accessToken))
         {
            if (folderId.IndexOf("id:", StringComparison.Ordinal) == -1) folderId = "id:" + folderId;
            var fileInfo = await client.Files.GetMetadataAsync(folderId, true, false, true);
            return new DropboxItemInfo(fileInfo);
         }
      }

      public Task<ICloudStorageProviderItem> PostFile(string accessToken, string folderId, IFormFile file, ExtraOptions extraOptions = null)
      {
         throw new NotImplementedException();
      }

      public Task<ICloudStorageProviderItem> PutFile(string accessToken, string fileId, IFormFile file, ExtraOptions extraOptions = null)
      {
         throw new NotImplementedException();
      }

      public async Task<ICloudStorageProviderItem> DeleteFile(string accessToken, string fileId, ExtraOptions extraOptions = null)
      {
         try
         {
            using (var client = new DropboxClient(accessToken))
            {
               var info = await client.Files.DeleteV2Async("id:" + fileId);
               return new DropboxItemInfo(info.Metadata);
            }
         }
         catch (Exception ex)
         {
            return null;
         }
      }

      public Task<ICloudStorageProviderItem> CreateFolder(string accessToken, string folderId, string name, ExtraOptions extraOptions = null)
      {
         throw new NotImplementedException();
      }

      public Task<ICloudStorageProviderItem> CopyItem(string accessToken, string itemId, string name, ExtraOptions extraOptions = null)
      {
         throw new NotImplementedException();
      }

      public Task<ICloudStorageProviderListResult> GetFolderContentsByPathAsync(string accessToken, string path, bool showFiles, bool includeMounted, bool includeMedia, ExtraOptions extraOptions = null)
      {
         throw new NotImplementedException();
      }

      public Task<string> CreateLink(string accessToken, string itemId, string linkType, string linkScope, ExtraOptions extraOptions = null)
      {
         throw new NotImplementedException();
      }

      public Task<ICloudStorageProviderDriveInfo> GetDriveInfoAsync(string accessToken, string driveId)
      {
         throw new NotImplementedException();
      }

      public Task<List<ICloudStorageProviderDriveInfo>> GetAvailableDrivesAsync(string accessToken)
      {
         throw new NotImplementedException();
      }

      public Task<ICloudStorageProviderThumbnail> GetFileThumbnailAsync(string accessToken, string fileId, string size, ExtraOptions extraOptions = null)
      {
         throw new NotImplementedException();
      }
   }

   public class DropboxOptions
   {
   }
}