// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using Dropbox.Api.Files;
using Newtonsoft.Json;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.Dropbox
{
   public class DropboxItemInfo : ICloudStorageProviderItem
   {
      public DropboxItemInfo(MetadataV2 metadata)
      {
         if (!metadata.IsMetadata) return;
         var file = metadata.AsMetadata.Value;
         if (file.IsFile)
         {
            this.Id = file.AsFile.Id;
            this.Hash = file.AsFile.ContentHash;
            this.Name = file.AsFile.Name;
            this.Path = file.AsFile.PathLower;
            this.IsDirectory = false;
            this.IsRoot = false;
            this.Revision = file.AsFile.Rev;
            this.Icon = "file";
            this.Modified = file.AsFile.ClientModified;
            this.Size = file.AsFile.Size;
            this.IsShared = file.AsFile.SharingInfo != null;
            this.ShareId = file.AsFile.SharingInfo != null ? file.AsFile.SharingInfo.ParentSharedFolderId : "";
         }
         else
         {
            this.Id = file.AsFolder.Id;
            this.Hash = file.AsFolder.Id;
            this.Name = file.AsFolder.Name;
            this.Path = file.AsFolder.PathLower;
            this.IsDirectory = true;
            this.Icon = "folder";
            this.ParentId = file.AsFolder.ParentSharedFolderId;
            this.IsShared = file.AsFolder.SharingInfo != null;
            this.ShareId = file.AsFolder.SharingInfo != null ? file.AsFolder.SharingInfo.SharedFolderId : "";
         }
      }

      public DropboxItemInfo(Metadata file)
      {
         if (file.IsFile)
         {
            this.Id = file.AsFile.Id;
            this.Hash = file.AsFile.ContentHash;
            this.Name = file.AsFile.Name;
            this.Path = file.AsFile.PathLower;
            this.IsDirectory = false;
            this.IsRoot = false;
            this.Revision = file.AsFile.Rev;
            this.Icon = "file";
            this.Modified = file.AsFile.ClientModified;
            this.Size = file.AsFile.Size;
            this.IsShared = file.AsFile.SharingInfo != null;
            this.ShareId = file.AsFile.SharingInfo != null ? file.AsFile.SharingInfo.ParentSharedFolderId : "";
         }
         else
         {
            this.Id = file.AsFolder.Id;
            this.Hash = file.AsFolder.Id;
            this.Name = file.AsFolder.Name;
            this.Path = file.AsFolder.PathLower;
            this.IsDirectory = true;
            this.Icon = "folder";
            this.ParentId = file.AsFolder.ParentSharedFolderId;
            this.IsShared = file.AsFolder.SharingInfo != null;
            this.ShareId = file.AsFolder.SharingInfo != null ? file.AsFolder.SharingInfo.SharedFolderId : "";
         }
      }


      public DropboxItemInfo()
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