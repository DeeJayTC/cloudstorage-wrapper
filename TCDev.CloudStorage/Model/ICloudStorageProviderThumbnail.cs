// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using Newtonsoft.Json;

namespace TCDev.CloudStorage.Model
{
   public interface ICloudStorageProviderThumbnail
   {
      [JsonProperty("url")]
      string Url { get; set; }

      [JsonProperty("height")]
      int? Height { get; set; }

      [JsonProperty("width")]
      int? Width { get; set; }
   }
}