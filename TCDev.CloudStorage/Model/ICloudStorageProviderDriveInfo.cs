// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

namespace TCDev.CloudStorage.Model
{
   public interface ICloudStorageProviderDriveInfo
   {
      string Name { get; set; }
      string Id { get; set; }
      string Type { get; set; }
      string WebUrl { get; set; }
      string Owner { get; set; }
      double QuotaShared { get; set; }
      double QuotaMax { get; set; }
      double QuotaUsed { get; set; }
      string QuotaState { get; set; }
   }
}