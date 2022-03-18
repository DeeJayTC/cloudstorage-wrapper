// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using Microsoft.Graph;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions.SharePoint
{
   public class SharePointThumbnail : ICloudStorageProviderThumbnail
   {
      public SharePointThumbnail(IDriveItemThumbnailsCollectionPage thumbnailSets, string size)
      {
         foreach (var set in thumbnailSets)
         {
            switch (size.ToLower().Trim())
            {
               case "small":
                  this.Url = set.Small.Url;
                  this.Height = set.Small.Height;
                  this.Width = set.Small.Width;
                  break;
               case "medium":
                  this.Url = set.Medium.Url;
                  this.Height = set.Medium.Height;
                  this.Width = set.Medium.Width;
                  break;
               case "large":
                  this.Url = set.Large.Url;
                  this.Height = set.Large.Height;
                  this.Width = set.Large.Width;
                  break;
               default:
                  this.Url = set.Small.Url;
                  this.Height = set.Small.Height;
                  this.Width = set.Small.Width;
                  break;
            }
         }
      }

      public string Url { get; set; }
      public int? Height { get; set; }
      public int? Width { get; set; }
   }
}