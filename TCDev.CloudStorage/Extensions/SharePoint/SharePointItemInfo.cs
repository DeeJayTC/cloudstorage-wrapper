// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using Microsoft.Graph;
using Newtonsoft.Json;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.SharePoint
{
   public class SharePointItemInfo : ICloudStorageProviderItem
   {
      public SharePointItemInfo(DriveItem item)
      {
         if (item.File != null)
         {
            this.Id = item.Id;
            this.Hash = item.Id;
            this.Name = item.Name;
            this.Path = item.ParentReference.Path;
            this.DisplayPath = item.ParentReference.Path == null ? "root" : item.ParentReference.Path.Replace("/drives/" + item.ParentReference.DriveId + "/root:", "root");
            this.IsDirectory = false;
            this.IsRoot = false;
            this.Revision = item.Versions?.Count.ToString();
            this.Icon = item.Name.LastIndexOf('.') > 0 ? item.Name.Substring(item.Name.LastIndexOf('.') + 1) : "file";
            if (item.LastModifiedDateTime != null) this.Modified = item.LastModifiedDateTime.Value.UtcDateTime;
            this.Size = Convert.ToDouble(item.Size);
            this.IsShared = item.Shared != null;
            this.ShareId = item.Shared != null ? item.ParentReference.ShareId : "";
            this.ViewUrl = item.WebUrl;
            if (item.AdditionalData != null && item.AdditionalData["@microsoft.graph.downloadUrl"] != null) this.DownloadUrl = item.AdditionalData["@microsoft.graph.downloadUrl"].ToString();
         }
         else
         {
            this.Id = item.Id;
            this.Hash = item.Id;
            this.Name = item.Name;
            this.Path = item.ParentReference.Path;
            this.DisplayPath = item.ParentReference.Path == null ? "root" : item.ParentReference.Path.Replace("/drives/" + item.ParentReference.DriveId + "/root:", "root");
            this.IsDirectory = true;
            this.IsRoot = item.Root != null;
            this.Icon = "folder";
            this.ParentId = item.ParentReference.Id;
            this.IsShared = item.Shared != null;
            this.ShareId = item.Shared != null ? item.ParentReference.ShareId : "";
         }
      }

      public SharePointItemInfo()
      {
      }

      [JsonProperty("id")]
      public string Id { get; set; }

      [JsonProperty("hash")]
      public string Hash { get; set; }

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("path")]
      public string Path { get; set; }

      [JsonProperty("displayPath")]
      public string DisplayPath { get; set; }

      [JsonProperty("isDirectory")]
      public bool IsDirectory { get; set; }

      public string Root { get; set; }

      [JsonProperty("isRoot")]
      public bool IsRoot { get; set; }

      [JsonProperty("size")]
      public double Size { get; set; }

      [JsonProperty("revision")]
      public string Revision { get; set; }

      [JsonProperty("modified")]
      public DateTime Modified { get; set; }

      [JsonProperty("mime")]
      public string MimeType { get; set; }

      [JsonProperty("hasThumbs")]
      public bool HasThumbs { get; set; }

      public string Icon { get; set; }

      [JsonProperty("parentId")]
      public string ParentId { get; set; }

      [JsonProperty("isShared")]
      public bool IsShared { get; set; }

      [JsonProperty("shareId")]
      public string ShareId { get; set; }

      [JsonProperty("viewUrl")]
      public string ViewUrl { get; set; }

      [JsonProperty("downloadUrl")]
      public string DownloadUrl { get; set; }
   }
}