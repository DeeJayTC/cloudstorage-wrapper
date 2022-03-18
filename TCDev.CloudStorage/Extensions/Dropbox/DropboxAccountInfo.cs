// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Collections.Generic;
using Dropbox.Api.Users;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.Dropbox
{
   public class DropboxAccountInfo : ICloudStorageProviderAccountInfo
   {
      public DropboxAccountInfo(FullAccount accountInfo)
      {
         this.Name = accountInfo.Name.DisplayName;
         this.Id = accountInfo.AccountId;
         this.Name = accountInfo.Email;
      }

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