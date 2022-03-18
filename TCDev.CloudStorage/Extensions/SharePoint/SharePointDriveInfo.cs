// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using Microsoft.Graph;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.SharePoint
{
   public class SharePointDriveInfo : ICloudStorageProviderDriveInfo
   {
      public string WebUrlPath { get; set; }

      public SharePointDriveInfo()
      {
      }

      public SharePointDriveInfo(Drive drive)
      {
         var isSharePoint = drive.DriveType == "documentLibrary";
         dynamic group = null;
         var groupName = "";
         if (isSharePoint && drive.Owner?.AdditionalData != null && drive.Owner.AdditionalData.TryGetValue("group", out group))
            if (@group != null)
               groupName = @group.displayName;

         this.Name = drive.Name;
         this.Id = drive.Id;
         this.Type = isSharePoint ? "Group Drive" : "User Drive";
         this.WebUrl = drive.WebUrl.Replace("%20", "+") ?? "";
         var webUrlNoHttp = this.WebUrl.Substring(this.WebUrl.IndexOf("://") + 3);
         this.WebUrlPath = webUrlNoHttp.Substring(webUrlNoHttp.IndexOf("/")) ?? "";
         this.Owner = isSharePoint ? groupName : drive.Owner.User?.DisplayName ?? "";
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