// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using Microsoft.Graph;
using Newtonsoft.Json;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.OneDrive
{
   public class OneDriveDriveInfo : ICloudStorageProviderDriveInfo
   {
      public OneDriveDriveInfo()
      {
      }

      public OneDriveDriveInfo(Drive drive)
      {
         var isGroupDrive = drive.DriveType == "documentLibrary";
         dynamic group = null;
         if (isGroupDrive) @group = JsonConvert.DeserializeObject(drive.Owner.AdditionalData["group"].ToString());

         this.Name = drive.Name;
         this.Id = drive.Id;
         this.Type = isGroupDrive ? "Group Drive" : "User Drive";
         this.WebUrl = drive.WebUrl;
         this.Owner = isGroupDrive ? @group?.displayName?.ToString() : drive.Owner.User.DisplayName;
         this.QuotaState = drive.Quota.State;
         this.QuotaShared = Convert.ToDouble(drive.Quota.Total);
         this.QuotaMax = Convert.ToDouble(drive.Quota.Total);
         this.QuotaUsed = Convert.ToDouble(drive.Quota.Used);
      }

      public string Name { get; set; }
      public string Id { get; set; }
      public string Type { get; set; }
      public string WebUrl { get; set; }
      public string Owner { get; set; }
      public double QuotaShared { get; set; }
      public double QuotaMax { get; set; }
      public double QuotaUsed { get; set; }
      public string QuotaState { get; set; }
   }
}