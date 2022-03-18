// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Collections.Generic;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.SharePoint
{
   public class SharePointAccountInfo : ICloudStorageProviderAccountInfo
   {
      public string Name { get; set; }
      public string Id { get; set; }
      public string Email { get; set; }
      public string Country { get; set; }
      public double QuotaShared { get; set; }
      public double QuotaMax { get; set; }
      public double QuotaUsed { get; set; }
      public string QuotaState { get; set; }
      public string DriveType { get; set; }
      public List<ICloudStorageProviderDriveInfo> Drives { get; set; }
   }
}