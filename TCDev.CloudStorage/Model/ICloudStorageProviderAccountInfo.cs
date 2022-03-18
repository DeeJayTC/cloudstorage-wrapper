// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Collections.Generic;

namespace TCDev.CloudStorage.Model
{
   public interface ICloudStorageProviderAccountInfo
   {
      string Name { get; set; }
      string Id { get; set; }
      string Email { get; set; }
      string Country { get; set; }
      double QuotaShared { get; set; }
      double QuotaMax { get; set; }
      double QuotaUsed { get; set; }
      string QuotaState { get; set; }
      string DriveType { get; set; }
      List<ICloudStorageProviderDriveInfo> Drives { get; set; }
   }
}