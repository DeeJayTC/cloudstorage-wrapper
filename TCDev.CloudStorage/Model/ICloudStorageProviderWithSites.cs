// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Collections.Generic;
using System.Threading.Tasks;
using TCDev.CloudStorage.Extensions;

namespace TCDev.CloudStorage.Model
{
   /// <summary>
   ///    Basic Interface listing all API Calls a CSP with sites should provide
   /// </summary>
   public interface ICloudStorageProviderWithSites
   {
      /// <summary>
      ///    Returns all sites
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="extraOptions"></param>
      /// <returns></returns>
      Task<List<ICloudStorageProviderSiteInfo>> GetSitesAsync(string accessToken, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns sites matching search criteria
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="searchFor"></param>
      /// <param name="extraOptions"></param>
      /// <returns></returns>
      Task<List<ICloudStorageProviderSiteInfo>> GetSitesSearchAsync(string accessToken, string searchFor, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns site info
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="siteId"></param>
      /// <param name="extraOptions"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderSiteInfo> GetSiteInfoAsync(string accessToken, string siteId, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns drives for specified site
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="siteId"></param>
      /// <param name="extraOptions"></param>
      /// <returns></returns>
      Task<List<ICloudStorageProviderDriveInfo>> GetDrivesAsync(string accessToken, string siteId, ExtraOptions extraOptions = null);

      /// <summary>
      ///    Returns drive info
      /// </summary>
      /// <param name="accessToken"></param>
      /// <param name="driveId"></param>
      /// <param name="extraOptions"></param>
      /// <returns></returns>
      Task<ICloudStorageProviderDriveInfo> GetDriveInfoAsync(string accessToken, string driveId, ExtraOptions extraOptions = null);
   }
}