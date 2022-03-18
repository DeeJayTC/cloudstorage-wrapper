// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Collections.Generic;

namespace TCDev.CloudStorage.Model
{
   public interface ICloudStorageProviderListResult
   {
      ICloudStorageProviderItem FolderInfo { get; set; }
      List<ICloudStorageProviderItem> Contents { get; set; }
   }
}