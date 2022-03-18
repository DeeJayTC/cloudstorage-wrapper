// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using TCDev.CloudStorage.Extensions.Dropbox;
using TCDev.CloudStorage.Extensions.OneDrive;
using TCDev.CloudStorage.Extensions.SharePoint;

namespace TCDev.CloudStorage.Extensions
{
   /// <summary>
   ///    Tools for provider handling
   /// </summary>
   public static class ProviderRegistry
   {
      /// <summary>
      ///    Returns the type for the specific extension by using its name
      /// </summary>
      /// <param name="name"></param>
      /// <returns></returns>
      public static Type GetProviderByName(string name)
      {
         switch (name)
         {
            case "dropbox":
               return typeof(DropboxProvider);
            case "sharepoint":
            case "onedrivebusiness":
               return typeof(SharePointProvider);
            case "onedrive":
               return typeof(OneDriveProvider);
            //case "gdrive":
            //	return typeof(GoogleDrive.GoogleDrive);
         }

         return null;
      }
   }
}