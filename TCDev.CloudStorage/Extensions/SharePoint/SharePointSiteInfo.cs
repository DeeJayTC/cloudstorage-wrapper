// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using System.Collections.Generic;
using Microsoft.Graph;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.SharePoint
{
   public class SharePointSiteInfo : ICloudStorageProviderSiteInfo
   {
      public SharePointSiteInfo()
      {
      }

      public SharePointSiteInfo(Site site)
      {
         this.Id = site.Id;
         this.Name = site.Name;
         this.DisplayName = site.DisplayName;
         this.WebUrl = site.WebUrl.Replace("%20", "+");
         this.WebUrlHost = "";
         this.WebUrlPath = "";
         if (this.WebUrl.Contains("://"))
         {
            var webUrlNoHttp = this.WebUrl.Substring(this.WebUrl.IndexOf("://") + 3);
            this.WebUrlHost = webUrlNoHttp;
            if (webUrlNoHttp.Contains("/"))
            {
               this.WebUrlHost = webUrlNoHttp.Substring(0, webUrlNoHttp.IndexOf("/"));
               this.WebUrlPath = webUrlNoHttp.Substring(webUrlNoHttp.IndexOf("/"));
            }
         }

         if (site.CreatedDateTime != null && site.CreatedDateTime.Value != null && site.CreatedDateTime.Value.UtcDateTime != null) this.CreatedDateTime = site.CreatedDateTime.Value.UtcDateTime;
      }

      public string Id { get; set; }
      public string Name { get; set; }
      public string DisplayName { get; set; }
      public string WebUrl { get; set; }
      public string WebUrlHost { get; set; }
      public string WebUrlPath { get; set; }
      public DateTime CreatedDateTime { get; set; }
      public List<ICloudStorageProviderDriveInfo> Drives { get; set; }
   }
}