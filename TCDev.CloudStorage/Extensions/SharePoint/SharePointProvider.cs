// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.SharePoint
{
   public class SharePointOptions
   {
   }

   public class SharePointProvider : ICloudStorageProvider, ICloudStorageProviderWithSites
   {
      public async Task<ICloudStorageProviderAccountInfo> GetAccountInfoAsync(string accessToken, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");
         var includeAllDrives = extraOptions?.GetValue("includeAllDrives", false);

         Drive drive;
         if (!string.IsNullOrWhiteSpace(driveId))
            drive = await client.Drives[driveId].Request().GetAsync();
         else
            drive = await client.Me.Drive.Request().GetAsync();

         var driveName = drive.Name;
         var driveType = "";
         var accountId = "";
         var accountName = "";
         var accountEmail = "";
         var accountCountry = "";

         if (drive.DriveType == "documentLibrary") // SharePoint Group Drive
         {
            dynamic group = null;
            if (drive.Owner?.AdditionalData != null && drive.Owner.AdditionalData.TryGetValue("group", out group))
            {
               accountId = group?.id ?? "";
               accountName = group?.displayName ?? "";
               accountEmail = group?.email ?? "";
               driveType = "Group Drive";
            }
         }
         else // Personal OneDrive for Business
         {
            var user = await client.Users[drive.Owner.User.Id].Request().GetAsync();
            accountId = user.Id;
            accountName = user.DisplayName;
            accountEmail = user.UserPrincipalName;
            accountCountry = user.Country;
            driveType = "User Drive";
         }

         var info = new SharePointAccountInfo
         {
            Id = accountId, Name = accountName, Email = accountEmail, Country = accountCountry, QuotaMax = drive.Quota.Total != null ? Convert.ToDouble(drive.Quota.Total) : -1, QuotaUsed = drive.Quota.Used != null ? Convert.ToDouble(drive.Quota.Used) : 0, QuotaState = drive.Quota.State, DriveType = driveType, Drives = new List<ICloudStorageProviderDriveInfo>()
         };
         if (includeAllDrives.HasValue && includeAllDrives.Value)
            info.Drives = await GetAvailableDrivesAsync(accessToken);
         else
            info.Drives.Add(new SharePointDriveInfo(drive));

         return info;
      }

      public async Task<ICloudStorageProviderItem> GetFileInfoAsync(string accessToken, string fileId, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         DriveItem item;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[fileId].Request().GetAsync();
         else
            item = await client.Me.Drive.Items[fileId].Request().GetAsync();

         if (item == null) return null;

         return new SharePointItemInfo(item);
      }

      public async Task<ICloudStorageProviderThumbnail> GetFileThumbnailAsync(string accessToken, string fileId, string size, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         IDriveItemThumbnailsCollectionPage item;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[fileId].Thumbnails.Request().GetAsync();
         else
            item = await client.Me.Drive.Items[fileId].Thumbnails.Request().GetAsync();

         if (item == null) return null;

         return new SharePointThumbnail(item, size);
      }

      public async Task<ICloudStorageProviderListResult> GetFolderContentsAsync(string accessToken, string folderId, bool showFiles, bool includeMounted, bool includeMedia, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         var options = new Option[] {new QueryOption("$expand", "children")};

         DriveItem folderItem;
         if (!string.IsNullOrWhiteSpace(driveId))
         {
            folderItem = await client.Drives[driveId].Items[folderId].Request(options).GetAsync();
         }
         else
         {
            if (folderId == "root")
               folderItem = await client.Me.Drive.Root.Request(options).GetAsync();
            else
               folderItem = await client.Me.Drive.Items[folderId].Request(options).GetAsync();
         }

         var result = new SharePointListResult {FolderInfo = new SharePointItemInfo(folderItem)};

         Parallel.ForEach(folderItem.Children, item =>
         {
            if (item.File == null || item.File != null && showFiles) result.Contents.Add(new SharePointItemInfo(item));
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

         var result = new SharePointListResult {FolderInfo = new SharePointItemInfo(folderItem)};

         Parallel.ForEach(folderItem.Children, item => { result.Contents.Add(new SharePointItemInfo(item)); });

         if (result.Contents?.Count > 0) result.Contents = result.Contents.OrderByDescending(p => p.IsDirectory).ThenBy(p => p.Name).ThenBy(p => p.Modified).ToList();

         return result;
      }

      public async Task<ICloudStorageProviderListResult> GetFolderContentsSearchAsync(string accessToken, string folderId, bool showFiles, bool includeMounted, bool includeMedia, string searchFor, bool recursive, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         DriveItem folderItem;
         IDriveItemSearchCollectionPage searchResult;
         if (!string.IsNullOrWhiteSpace(driveId))
         {
            folderItem = await client.Drives[driveId].Items[folderId].Request().GetAsync();
            searchResult = await client.Drives[driveId].Items[folderId].Search(searchFor).Request().GetAsync(); // Always recursive.
         }
         else
         {
            folderItem = await client.Me.Drive.Items[folderId].Request().GetAsync();
            searchResult = await client.Me.Drive.Items[folderId].Search(searchFor).Request().GetAsync(); // Always recursive.
         }

         var result = new SharePointListResult {FolderInfo = new SharePointItemInfo(folderItem)};

         Parallel.ForEach(searchResult, item =>
         {
            if (item.File != null) result.Contents.Add(new SharePointItemInfo(item));
         });

         if (result.Contents?.Count > 0) result.Contents = result.Contents.OrderByDescending(p => p.IsDirectory).ThenBy(p => p.Name).ThenBy(p => p.Modified).ToList();

         return result;
      }

      public async Task<ICloudStorageProviderItem> GetFolderInfoAsync(string accessToken, string folderId, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         DriveItem folderItem;
         if (!string.IsNullOrWhiteSpace(driveId))
            folderItem = await client.Drives[driveId].Items[folderId].Request().GetAsync();
         else
            folderItem = await client.Me.Drive.Items[folderId].Request().GetAsync();

         if (folderItem == null) return null;

         return new SharePointItemInfo(folderItem);
      }

      public async Task<ICloudStorageProviderItem> PostFile(string accessToken, string folderId, IFormFile file, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         var fileStream = file.OpenReadStream();

         DriveItem item;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[folderId].ItemWithPath(file.FileName).Content.Request().PutAsync<DriveItem>(fileStream);
         else
            item = await client.Me.Drive.Items[folderId].ItemWithPath(file.FileName).Content.Request().PutAsync<DriveItem>(fileStream);

         if (item == null) return null;

         return new SharePointItemInfo(item);
      }

      public async Task<ICloudStorageProviderItem> PutFile(string accessToken, string fileId, IFormFile file, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         var fileStream = file.OpenReadStream();

         DriveItem item;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[fileId].Content.Request().PutAsync<DriveItem>(fileStream);
         else
            item = await client.Me.Drive.Items[fileId].Content.Request().PutAsync<DriveItem>(fileStream);

         if (item == null) return null;

         return new SharePointItemInfo(item);
      }

      public async Task<ICloudStorageProviderItem> DeleteFile(string accessToken, string fileId, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         if (!string.IsNullOrWhiteSpace(driveId))
            await client.Drives[driveId].Items[fileId].Request().DeleteAsync();
         else
            await client.Me.Drive.Items[fileId].Request().DeleteAsync();

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

         DriveItem item;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[folderId].Children.Request().AddAsync(newFolder);
         else
            item = await client.Me.Drive.Items[folderId].Children.Request().AddAsync(newFolder);

         if (item == null) return null;

         return new SharePointItemInfo(item);
      }

      public async Task<ICloudStorageProviderItem> CopyItem(string accessToken, string itemId, string name, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         DriveItem item;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[itemId].Copy(name).Request().PostAsync();
         else
            item = await client.Me.Drive.Items[itemId].Copy(name).Request().PostAsync();

         if (item == null) return null;

         return new SharePointItemInfo(item);
      }

      public async Task<string> CreateLink(string accessToken, string itemId, string linkType, string linkScope, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveId = extraOptions?.GetValue("driveId", "");

         Permission item;
         if (!string.IsNullOrWhiteSpace(driveId))
            item = await client.Drives[driveId].Items[itemId].CreateLink(linkType, linkScope).Request().PostAsync();
         else
            item = await client.Me.Drive.Items[itemId].CreateLink(linkType, linkScope).Request().PostAsync();

         return item.Link.WebUrl;
      }

      public async Task<ICloudStorageProviderDriveInfo> GetDriveInfoAsync(string accessToken, string driveId)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var drive = await client.Drives[driveId].Request().GetAsync();

         if (drive == null) return null;

         return new SharePointDriveInfo(drive);
      }

      public async Task<List<ICloudStorageProviderDriveInfo>> GetAvailableDrivesAsync(string accessToken)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveList = new List<ICloudStorageProviderDriveInfo>();

         IUserMemberOfCollectionWithReferencesPage directoryCollection = null;
         try
         {
            directoryCollection = await client.Me.MemberOf.Request().GetAsync();
         }
         catch (ServiceException ex)
         {
            // Return exception unless it's an Access Denied/Restricted response
            if (ex.IsMatch(GraphErrorCode.AccessDenied.ToString()) || ex.IsMatch(GraphErrorCode.AccessRestricted.ToString())) throw new Exception("Permission denied. Could not retrieve groups list.");

            throw;
         }

         if (directoryCollection?.Count > 0)
            Parallel.ForEach(directoryCollection, directoryObject =>
            {
               if (!(directoryObject is Group group)) return;
               if (@group.Visibility == null) return;

               Task<IGroupDrivesCollectionPage> drives = null;
               try
               {
                  drives = client.Groups[@group.Id].Drives.Request().GetAsync();
                  if (drives?.Result?.Count == 0) return;

                  // Lock driveList to avoid threading issues 
                  lock (driveList)
                  {
                     // here you can add or remove items from the fileInfo list
                     driveList.AddRange(drives?.Result?.Where(p => !string.IsNullOrEmpty(p.Id)).Select(drive => new SharePointDriveInfo(drive)).ToList());
                  }
               }
               catch (ServiceException ex)
               {
                  // Return exception unless it's an Access Denied/Restricted response
                  if (ex.IsMatch(GraphErrorCode.AccessDenied.ToString()) || ex.IsMatch(GraphErrorCode.AccessRestricted.ToString())) throw new Exception("Permission denied. Could not load group drive data.");

                  throw;
               }
            });

         // Get the personal drives
         IUserDrivesCollectionPage meDrives = null;
         try
         {
            meDrives = await client.Me.Drives.Request().GetAsync();
         }
         catch (ServiceException ex)
         {
            // Return exception unless it's an Access Denied/Restricted response
            if (ex.IsMatch(GraphErrorCode.AccessDenied.ToString()) || ex.IsMatch(GraphErrorCode.AccessRestricted.ToString())) throw new Exception("Permission denied. Could not load drive data.");

            throw;
         }

         if (meDrives?.Count > 0)
            Parallel.ForEach(meDrives, drive =>
            {
               // Lock driveList to avoid threading issues 
               lock (driveList)
               {
                  driveList.Add(new SharePointDriveInfo(drive));
               }
            });

         // Get the site drives
         // NOTE: Currently, the MS Graph SDK does not support this call we use to
         // get all sites, so we have to bypass the SDK for now.
         var requestUrl = "https://graph.microsoft.com/v1.0/sites?search=*&select=id,webUrl,drives,siteCollection";
         var message = new HttpRequestMessage(HttpMethod.Get, requestUrl);
         var content = "";
         try
         {
            await client.AuthenticationProvider.AuthenticateRequestAsync(message);
            var response = await client.HttpProvider.SendAsync(message);
            content = await response.Content.ReadAsStringAsync();
         }
         catch (Exception ex) when (ex is ServiceException || ex is HttpRequestException || ex is NullReferenceException)
         {
            throw new Exception("Request for sites list failed.");
         }

         // If content was returned, let's try to parse it as json.
         List<Site> sites = null;
         if (!string.IsNullOrEmpty(content))
            try
            {
               var contentObject = JObject.Parse(content);
               JToken contentValue = null;
               if (contentObject.TryGetValue("value", out contentValue)) sites = JsonConvert.DeserializeObject<List<Site>>(contentValue.ToString());
            }
            catch (Exception ex) when (ex is JsonSerializationException || ex is JsonReaderException || ex is NullReferenceException)
            {
               throw new Exception("Unable to retrieve sites list.");
            }

         // If sites exist, let's fetch the drives for each site.
         if (sites?.Count > 0)
         {
            var siteList = sites.Where(s => !string.IsNullOrWhiteSpace(s.Id)).ToList();

            if (siteList?.Count > 0)
               Parallel.ForEach(siteList, siteObject =>
               {
                  if (siteObject is Site site)
                  {
                     Task<ISiteDrivesCollectionPage> drives = null;
                     try
                     {
                        drives = client.Sites[site.Id].Drives.Request().GetAsync();
                     }
                     catch (ServiceException ex)
                     {
                        // Return exception unless it's an Access Denied/Restricted response
                        if (!ex.IsMatch(GraphErrorCode.AccessDenied.ToString()) && !ex.IsMatch(GraphErrorCode.AccessRestricted.ToString())) throw;
                     }

                     if (drives?.Result?.Count > 0)
                        Parallel.ForEach(drives.Result, drive =>
                        {
                           if (string.IsNullOrEmpty(drive?.Id) || drive?.Owner == null) return;
                           if (!driveList.Exists(p => p != null && p.Id == drive.Id))
                              lock (driveList)
                              {
                                 driveList.Add(new SharePointDriveInfo(drive));
                              }
                        });
                  }
               });
         }

         return driveList.OrderBy(p => p.Name).ToList();
      }

      public async Task<List<ICloudStorageProviderSiteInfo>> GetSitesAsync(string accessToken, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         // Get the site drives
         // NOTE: Currently, the MS Graph SDK does not support this call we use to
         // get all sites, so we have to bypass the SDK for now.
         var requestUrl = "https://graph.microsoft.com/v1.0/sites?search=*";
         var message = new HttpRequestMessage(HttpMethod.Get, requestUrl);
         var content = "";
         try
         {
            await client.AuthenticationProvider.AuthenticateRequestAsync(message);
            var response = await client.HttpProvider.SendAsync(message);
            content = await response.Content.ReadAsStringAsync();
         }
         catch (Exception ex) when (ex is ServiceException || ex is HttpRequestException || ex is NullReferenceException)
         {
            throw new Exception("Request for sites list failed.");
         }

         // If content was returned, let's try to parse it as json.
         List<Site> sites = null;
         if (!string.IsNullOrEmpty(content))
            try
            {
               var contentObject = JObject.Parse(content);
               JToken contentValue = null;
               if (contentObject.TryGetValue("value", out contentValue)) sites = JsonConvert.DeserializeObject<List<Site>>(contentValue.ToString());
            }
            catch (Exception ex) when (ex is JsonSerializationException || ex is JsonReaderException || ex is NullReferenceException)
            {
               throw new Exception("Unable to retrieve sites list.");
            }

         var result = new List<ICloudStorageProviderSiteInfo>();
         if (sites?.Count > 0)
         {
            var siteList = sites.Where(s => !string.IsNullOrWhiteSpace(s.Id)).OrderBy(s => s.Name).ToList();

            if (siteList?.Count > 0)
               Parallel.ForEach(siteList, site => { result.Add(new SharePointSiteInfo(site)); });
         }

         return result.OrderBy(s => s.DisplayName).ToList();
      }

      public async Task<List<ICloudStorageProviderSiteInfo>> GetSitesSearchAsync(string accessToken, string searchFor, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var options = new List<QueryOption>
         {
            new QueryOption("$search", searchFor)
         };

         IGraphServiceSitesCollectionPage sites;
         try
         {
            sites = await client.Sites.Request(options).GetAsync();
         }
         catch (ServiceException ex)
         {
            // Return exception unless it's an Access Denied/Restricted response
            if (ex.IsMatch(GraphErrorCode.AccessDenied.ToString()) || ex.IsMatch(GraphErrorCode.AccessRestricted.ToString())) throw new Exception("Permission denied. Could not search sites.");

            throw;
         }

         if (sites == null) throw new Exception("Unable to retrieve sites search results.");

         var result = new List<ICloudStorageProviderSiteInfo>();
         if (sites?.Count > 0)
         {
            var siteList = sites.Where(s => !string.IsNullOrWhiteSpace(s.Id)).OrderBy(s => s.Name).ToList();

            if (siteList?.Count > 0)
               Parallel.ForEach(siteList, site => { result.Add(new SharePointSiteInfo(site)); });
         }

         return result.OrderBy(s => s.DisplayName).ToList();
      }

      public async Task<ICloudStorageProviderSiteInfo> GetSiteInfoAsync(string accessToken, string siteId, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         Site site;
         try
         {
            site = await client.Sites[siteId].Request().GetAsync();
         }
         catch (ServiceException ex)
         {
            // Return exception unless it's an Access Denied/Restricted response
            if (ex.IsMatch(GraphErrorCode.AccessDenied.ToString()) || ex.IsMatch(GraphErrorCode.AccessRestricted.ToString())) throw new Exception("Permission denied. Could not retrieve site info.");

            throw;
         }

         if (site == null) throw new Exception("Unable to retrieve site info.");

         return new SharePointSiteInfo(site);
      }

      public async Task<List<ICloudStorageProviderDriveInfo>> GetDrivesAsync(string accessToken, string siteId, ExtraOptions extraOptions = null)
      {
         if (string.IsNullOrWhiteSpace(siteId)) siteId = extraOptions?.GetValue("siteId", "");
         if (string.IsNullOrWhiteSpace(siteId)) throw new Exception("No site ID provided. Unable to retrieve drive list.");

         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         var driveList = new List<ICloudStorageProviderDriveInfo>();

         ISiteDrivesCollectionPage drives = null;
         try
         {
            drives = await client.Sites[siteId].Drives.Request().GetAsync();
         }
         catch (ServiceException ex)
         {
            // Return exception unless it's an Access Denied/Restricted response
            if (ex.IsMatch(GraphErrorCode.AccessDenied.ToString()) || ex.IsMatch(GraphErrorCode.AccessRestricted.ToString())) throw new Exception("Permission denied. Could not load drives for this site.");

            throw;
         }

         if (drives?.Count > 0)
            Parallel.ForEach(drives, drive =>
            {
               if (string.IsNullOrEmpty(drive?.Id) || drive?.Owner == null) return;
               if (!driveList.Exists(p => p != null && p.Id == drive.Id))
                  lock (driveList)
                  {
                     driveList.Add(new SharePointDriveInfo(drive));
                  }
            });

         return driveList.OrderBy(p => p.Name).ToList();
      }

      public async Task<ICloudStorageProviderDriveInfo> GetDriveInfoAsync(string accessToken, string driveId, ExtraOptions extraOptions = null)
      {
         var client = GraphApiHelper.GetAuthenticatedClient(accessToken);

         Drive drive;
         try
         {
            drive = await client.Drives[driveId].Request().GetAsync();
         }
         catch (ServiceException ex)
         {
            // Return exception unless it's an Access Denied/Restricted response
            if (ex.IsMatch(GraphErrorCode.AccessDenied.ToString()) || ex.IsMatch(GraphErrorCode.AccessRestricted.ToString())) throw new Exception("Permission denied. Could not retrieve drive info.");

            throw;
         }

         if (drive == null) throw new Exception("Unable to retrieve drive info.");

         return new SharePointDriveInfo(drive);
      }
   }
}