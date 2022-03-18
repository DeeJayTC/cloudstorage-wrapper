// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using Newtonsoft.Json;

namespace TCDev.CloudStorage.Model
{
   public interface ICloudStorageProviderFolderInfo
   {
      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      string Hash { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      string Name { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      string Path { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      bool IsDirectory { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      string Root { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      bool IsRoot { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      double Size { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      string Revision { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      DateTime Modified { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      string MimeType { get; set; }
   }
}