// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Collections.Generic;

namespace TCDev.CloudStorage.Model
{
   public interface ICloudStorageProviderSiteInfo
   {
      string Id { get; set; }
      string Name { get; set; }
      string DisplayName { get; set; }
      string WebUrl { get; set; }
      string WebUrlHost { get; set; }
      string WebUrlPath { get; set; }
      DateTime CreatedDateTime { get; set; }
      List<ICloudStorageProviderDriveInfo> Drives { get; set; }
   }
}