// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.OneDrive
{
   public class OneDriveOptions
   {
   }

   public class OneDriveProvider : ICloudStorageProvider
   {
      public async Task<ICloudStorageProviderAccountInfo> GetAccountInfoAsync(string accessToken, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);
         var driveId = extraOptions?.GetValue("driveId", "");

         Drive drive;
         if (!string.IsNullOrWhiteSpace(driveId))
            drive = await client.Drives[driveId].Request().GetAsync();
         else
            drive = await client.Me.Drive.Request().GetAsync();

         var driveName = drive.Name;
         var driveType = "";

         string accountId, accountName, accountEmail, accountCountry = "";
         if (drive.DriveType == "personal") // SharePoint Group Drive
         {
            //var user = await client.Users[drive.Owner.User.Id].Request().GetAsync();
            accountId = drive.Owner.User.Id;
            accountName = drive.Owner.User.DisplayName;
            accountEmail = drive.Owner.User.DisplayName;
            accountCountry = "unknown"; //drive.Owner.User.Country;
            driveType = "User Drive";

            var info = new OneDriveAccountInfo
            {
               Id = accountId, Name = accountName, Email = accountEmail, Country = accountCountry, QuotaMax = drive.Quota.Total != null ? Convert.ToDouble(drive.Quota.Total) : -1, QuotaUsed = drive.Quota.Used != null ? Convert.ToDouble(drive.Quota.Used) : 0, QuotaState = drive.Quota.State, DriveType = driveType, Drives = new List<ICloudStorageProviderDriveInfo>() // await GetAvailableDrivesAsync(accessToken)
            };
            return info;
         }

         return null;
      }

      public async Task<ICloudStorageProviderItem> GetFileInfoAsync(string accessToken, string fileId, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         DriveItem item = null;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[fileId].Request().GetAsync();
         else
            try
            {
               item = await client.Me.Drive.Items[fileId].Request().GetAsync();
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedFileId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(fileId, out fixedFileId))
                  item = await client.Me.Drive.Items[fixedFileId].Request().GetAsync();
               else
                  throw;
            }

         if (item == null) return null;

         return new OneDriveItemInfo(item);
      }

      public async Task<ICloudStorageProviderThumbnail> GetFileThumbnailAsync(string accessToken, string fileId, string size, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         IDriveItemThumbnailsCollectionPage item = null;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[fileId].Thumbnails.Request().GetAsync();
         else
            try
            {
               item = await client.Me.Drive.Items[fileId].Thumbnails.Request().GetAsync();
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedFileId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(fileId, out fixedFileId))
                  item = await client.Me.Drive.Items[fixedFileId].Thumbnails.Request().GetAsync();
               else
                  throw;
            }

         if (item == null) return null;

         return new OneDriveThumbnail(item, size);
      }

      public async Task<ICloudStorageProviderListResult> GetFolderContentsAsync(string accessToken, string folderId, bool showFiles, bool includeMounted, bool includeMedia, ExtraOptions extraOptions = null)
      {
         // Hack to fix a projects issue
         if (folderId == "0") folderId = "root";

         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         var options = new Option[] {new QueryOption("$expand", "children")};

         DriveItem folderItem = null;
         if (!string.IsNullOrWhiteSpace(driveId))
            folderItem = await client.Drives[driveId].Items[folderId].Request(options).GetAsync();
         else
            try
            {
               folderItem = await client.Me.Drive.Items[folderId].Request(options).GetAsync();
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedFolderId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(folderId, out fixedFolderId))
                  folderItem = await client.Me.Drive.Items[fixedFolderId].Request(options).GetAsync();
               else
                  throw;
            }

         var result = new OneDriveListResult {FolderInfo = new OneDriveItemInfo(folderItem)};

         Parallel.ForEach(folderItem.Children, item =>
         {
            if (item.File == null || item.File != null && showFiles) result.Contents.Add(new OneDriveItemInfo(item));
         });

         if (result.Contents?.Count > 0) result.Contents = result.Contents.OrderByDescending(p => p.IsDirectory).ThenBy(p => p.Name).ThenBy(p => p.Modified).ToList();

         return result;
      }

      public async Task<ICloudStorageProviderListResult> GetFolderContentsByPathAsync(string accessToken, string path, bool showFiles, bool includeMounted, bool includeMedia, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         var options = new Option[] {new QueryOption("$expand", "children")};

         DriveItem folderItem;
         if (!string.IsNullOrWhiteSpace(driveId))
            folderItem = await client.Drives[driveId].Root.ItemWithPath(path).Request(options).GetAsync();
         else
            folderItem = await client.Me.Drive.Root.ItemWithPath(path).Request(options).GetAsync();

         var result = new OneDriveListResult {FolderInfo = new OneDriveItemInfo(folderItem)};

         Parallel.ForEach(folderItem.Children, item => { result.Contents.Add(new OneDriveItemInfo(item)); });

         if (result.Contents?.Count > 0) result.Contents = result.Contents.OrderByDescending(p => p.IsDirectory).ThenBy(p => p.Name).ThenBy(p => p.Modified).ToList();

         return result;
      }

      public async Task<ICloudStorageProviderListResult> GetFolderContentsSearchAsync(string accessToken, string folderId, bool showFiles, bool includeMounted, bool includeMedia, string searchFor, bool recursive, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         DriveItem folderItem = null;
         var fixedFolderId = "";
         IDriveItemSearchCollectionPage searchResult;
         if (!string.IsNullOrWhiteSpace(driveId))
         {
            folderItem = await client.Drives[driveId].Items[folderId].Request().GetAsync();
            searchResult = await client.Drives[driveId].Items[folderId].Search(searchFor).Request().GetAsync(); // Always recursive.
         }
         else
         {
            try
            {
               folderItem = await client.Me.Drive.Items[folderId].Request().GetAsync();
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(folderId, out fixedFolderId))
               {
                  folderItem = await client.Me.Drive.Items[fixedFolderId].Request().GetAsync();
                  folderId = fixedFolderId;
               }
               else
               {
                  throw;
               }
            }

            searchResult = await client.Me.Drive.Items[folderId].Search(searchFor).Request().GetAsync(); // Always recursive.
         }

         var result = new OneDriveListResult {FolderInfo = new OneDriveItemInfo(folderItem)};

         Parallel.ForEach(searchResult, item =>
         {
            if (item.File != null) result.Contents.Add(new OneDriveItemInfo(item));
         });

         if (result.Contents?.Count > 0) result.Contents = result.Contents.OrderByDescending(p => p.IsDirectory).ThenBy(p => p.Name).ThenBy(p => p.Modified).ToList();

         return result;
      }

      public async Task<ICloudStorageProviderItem> GetFolderInfoAsync(string accessToken, string folderId, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         DriveItem folderItem = null;
         if (!string.IsNullOrWhiteSpace(driveId))
            folderItem = await client.Drives[driveId].Items[folderId].Request().GetAsync();
         else
            try
            {
               folderItem = await client.Me.Drive.Items[folderId].Request().GetAsync();
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedFolderId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(folderId, out fixedFolderId))
                  folderItem = await client.Me.Drive.Items[fixedFolderId].Request().GetAsync();
               else
                  throw;
            }

         if (folderItem == null) return null;

         return new OneDriveItemInfo(folderItem);
      }

      public async Task<ICloudStorageProviderItem> PostFile(string accessToken, string folderId, IFormFile file, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");
         var fileStream = file.OpenReadStream();

         DriveItem item = null;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[folderId].ItemWithPath(file.FileName).Content.Request().PutAsync<DriveItem>(fileStream);
         else
            try
            {
               item = await client.Me.Drive.Items[folderId].ItemWithPath(file.FileName).Content.Request().PutAsync<DriveItem>(fileStream);
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedFolderId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(folderId, out fixedFolderId))
                  item = await client.Me.Drive.Items[fixedFolderId].ItemWithPath(file.FileName).Content.Request().PutAsync<DriveItem>(fileStream);
               else
                  throw;
            }

         if (item == null) return null;

         return new OneDriveItemInfo(item);
      }

      public async Task<ICloudStorageProviderItem> PutFile(string accessToken, string fileId, IFormFile file, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         var fileStream = file.OpenReadStream();

         DriveItem item = null;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[fileId].Content.Request().PutAsync<DriveItem>(fileStream);
         else
            try
            {
               item = await client.Me.Drive.Items[fileId].Content.Request().PutAsync<DriveItem>(fileStream);
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedFileId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(fileId, out fixedFileId))
                  item = await client.Me.Drive.Items[fixedFileId].Content.Request().PutAsync<DriveItem>(fileStream);
               else
                  throw;
            }

         if (item == null) return null;

         return new OneDriveItemInfo(item);
      }

      public async Task<ICloudStorageProviderItem> DeleteFile(string accessToken, string fileId, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         if (!string.IsNullOrWhiteSpace(driveId))
            await client.Drives[driveId].Items[fileId].Request().DeleteAsync();
         else
            try
            {
               await client.Me.Drive.Items[fileId].Request().DeleteAsync();
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedFileId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(fileId, out fixedFileId))
                  await client.Me.Drive.Items[fixedFileId].Request().DeleteAsync();
               else
                  throw;
            }

         return null;
      }

      public async Task<ICloudStorageProviderItem> CreateFolder(string accessToken, string folderId, string name, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         var newFolder = new DriveItem
         {
            Name = name, Folder = new Folder()
         };

         DriveItem item = null;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[folderId].Children.Request().AddAsync(newFolder);
         else
            try
            {
               item = await client.Me.Drive.Items[folderId].Children.Request().AddAsync(newFolder);
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedFolderId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(folderId, out fixedFolderId))
                  item = await client.Me.Drive.Items[folderId].Children.Request().AddAsync(newFolder);
               else
                  throw;
            }

         if (item == null) return null;

         return new OneDriveItemInfo(item);
      }

      public async Task<ICloudStorageProviderItem> CopyItem(string accessToken, string itemId, string name, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         DriveItem item = null;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[itemId].Copy(name).Request().PostAsync();
         else
            try
            {
               item = await client.Me.Drive.Items[itemId].Copy(name).Request().PostAsync();
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedItemId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(itemId, out fixedItemId))
                  item = await client.Me.Drive.Items[fixedItemId].Copy(name).Request().PostAsync();
               else
                  throw;
            }

         if (item == null) return null;

         return new OneDriveItemInfo(item);
      }

      public async Task<string> CreateLink(string accessToken, string itemId, string linkType, string linkScope, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         Permission item = null;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[itemId].CreateLink(linkType, linkScope).Request().PostAsync();
         else
            try
            {
               item = await client.Me.Drive.Items[itemId].CreateLink(linkType, linkScope).Request().PostAsync();
            }
            catch (ServiceException ex)
            {
               // If invalid request or item not found, attempt a fix on the ID and try again
               var fixedItemId = "";
               if ((ex.IsMatch(GraphErrorCode.InvalidRequest.ToString()) || ex.IsMatch(GraphErrorCode.ItemNotFound.ToString())) && TryIdFix(itemId, out fixedItemId))
                  item = await client.Me.Drive.Items[fixedItemId].CreateLink(linkType, linkScope).Request().PostAsync();
               else
                  throw;
            }

         return item.Link.WebUrl;
      }

      public async Task<ICloudStorageProviderDriveInfo> GetDriveInfoAsync(string accessToken, string driveId)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var drive = await client.Drives[driveId].Request().GetAsync();

         if (drive == null) return null;

         return new OneDriveDriveInfo(drive);
      }

      public async Task<List<ICloudStorageProviderDriveInfo>> GetAvailableDrivesAsync(string accessToken)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveList = new List<ICloudStorageProviderDriveInfo>();

         var directoryCollection = await client.Me.MemberOf.Request().GetAsync();

         Parallel.ForEach(directoryCollection, directoryObject =>
         {
            if (directoryObject is Group)
            {
               var group = directoryObject as Group;

               if (group.Visibility != null)
               {
                  var drives = client.Groups[group.Id].Drives.Request().GetAsync();
                  if (drives != null && drives.Result.Count > 0)
                     Parallel.ForEach(drives.Result, drive => { driveList.Add(new OneDriveDriveInfo(drive)); });
               }
            }
         });

         var meDrives = await client.Me.Drives.Request().GetAsync();

         Parallel.ForEach(meDrives, drive => { driveList.Add(new OneDriveDriveInfo(drive)); });

         return driveList;
      }

      //	This is a fix for IDs that were saved in Projects before switching
      //	to Sythesis CSW. It extracts the ID from the following formats:
      //	
      //	ID: "file.02cc622e56811d09.2CC622E56811D09!503"
      //	Returns: "2CC622E56811D09!503"
      //
      //	ID: "folder.902ad432a8305c80.902AD432A8305C80!118"
      //	Returns: "902AD432A8305C80!118"
      private bool TryIdFix(string oldItemId, out string newItemId)
      {
         newItemId = oldItemId;
         if (oldItemId.StartsWith("file.") || oldItemId.StartsWith("folder."))
         {
            var parts = oldItemId.Split(".");
            if (parts.Length == 3)
            {
               newItemId = parts[2];
               return true;
            }
         }

         return false;
      }
   }
}