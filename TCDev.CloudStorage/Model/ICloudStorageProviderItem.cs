// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using Newtonsoft.Json;

namespace TCDev.CloudStorage.Model
{
   public interface ICloudStorageProviderItem
   {
      [JsonProperty("id")]
      string Id { get; set; }

      [JsonProperty("hash")]
      string Hash { get; set; }

      [JsonProperty("name")]
      string Name { get; set; }

      [JsonProperty("path")]
      string Path { get; set; }

      [JsonProperty("displayPath")]
      string DisplayPath { get; set; }

      [JsonProperty("isDirectory")]
      bool IsDirectory { get; set; }

      string Root { get; set; }

      [JsonProperty("isRoot")]
      bool IsRoot { get; set; }

      [JsonProperty("size")]
      double Size { get; set; }

      [JsonProperty("revision")]
      string Revision { get; set; }

      [JsonProperty("modified")]
      DateTime Modified { get; set; }

      [JsonProperty("mime")]
      string MimeType { get; set; }

      [JsonProperty("hasThumbs")]
      bool HasThumbs { get; set; }

      string Icon { get; set; }

      [JsonProperty("parentId")]
      string ParentId { get; set; }

      [JsonProperty("isShared")]
      bool IsShared { get; set; }

      [JsonProperty("shareId")]
      string ShareId { get; set; }

      [JsonProperty("viewUrl")]
      string ViewUrl { get; set; }

      [JsonProperty("downloadUrl")]
      string DownloadUrl { get; set; }
   }
}