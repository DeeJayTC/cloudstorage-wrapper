// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TCDev.CloudStorage.Extensions.Dropbox;
using TCDev.CloudStorage.Extensions.OneDrive;
using TCDev.CloudStorage.Extensions.SharePoint;

namespace TCDev.CloudStorage.Extensions
{
   /// <summary>
   ///    Extension methods for <see cref="IApplicationBuilder" /> to add a Bot to the ASP.NET Core request execution
   ///    pipeline.
   /// </summary>
   /// <seealso cref="ServiceCollectionExtensions" />
   public static class ApplicationBuilderExtensions
   {
      /// <summary>
      ///    UseDropbox -> Enable Dropbox Service to be used by the Framework
      /// </summary>
      /// <param name="applicationBuilder"></param>
      /// <returns></returns>
      public static IApplicationBuilder UseDropbox(this IApplicationBuilder applicationBuilder)
      {
         if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

         var options = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<DropboxOptions>>().Value;

         return applicationBuilder;
      }

      /// <summary>
      ///    UseSharePoint -> Enable SharePoint Service to be used by the Framework
      /// </summary>
      /// <param name="applicationBuilder"></param>
      /// <returns></returns>
      public static IApplicationBuilder UseSharePoint(this IApplicationBuilder applicationBuilder)
      {
         if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

         var options = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<SharePointOptions>>().Value;

         return applicationBuilder;
      }


      /// <summary>
      ///    UseOneDrive -> Enable OneDrive Service to be used by the Framework
      /// </summary>
      /// <param name="applicationBuilder"></param>
      /// <returns></returns>
      public static IApplicationBuilder UseOneDrive(this IApplicationBuilder applicationBuilder)
      {
         if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

         var options = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<OneDriveOptions>>().Value;

         return applicationBuilder;
      }
   }
}