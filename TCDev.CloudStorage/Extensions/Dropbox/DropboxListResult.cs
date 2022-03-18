// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Collections.Generic;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.Dropbox
{
   public class DropboxListResult : ICloudStorageProviderListResult
   {
      public ICloudStorageProviderItem FolderInfo { get; set; }
      public List<ICloudStorageProviderItem> Contents { get; set; }
   }
}