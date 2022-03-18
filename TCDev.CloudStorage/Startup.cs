// TCDev.de 2020/02/04
// TCDev.CloudStorage.Startup.cs
// https://www.github.com/deejaytc/dotnet-utils

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TCDev.CloudStorage.Controllers;
using TCDev.CloudStorage.Extensions;
using TCDev.CloudStorage.Extensions.Dropbox;
using TCDev.CloudStorage.Extensions.OneDrive;
using TCDev.CloudStorage.Extensions.SharePoint;
using TCDev.CloudStorage.Middleware;
using TCDev.CloudStorage.StartupExtension;

namespace TCDev.CloudStorage
{
   /// <summary>
   ///    Main Startup implementation
   /// </summary>
   public class Startup
   {
      /// <summary>
      ///    Holds the configuration implementation
      /// </summary>
      public IConfiguration Configuration { get; }

      /// <summary>
      ///    Startup class to handle app startup
      /// </summary>
      /// <param name="configuration"></param>
      public Startup(IHostingEnvironment env)
      {
         var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
            .AddEnvironmentVariables();
         this.Configuration = builder.Build();
      }

      /// This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddOptions();

         services.AddControllers();

         services.Configure<FormOptions>(options =>
         {
            options.MultipartBodyLengthLimit = 1073741824; // 1.0 GB
         });

         services.AddDistributedRedisCache(option =>
         {
            option.Configuration = this.Configuration["Redis:ConnectionString"];
            option.InstanceName = this.Configuration["Redis:InstanceName"];
         });

         services.SetupCors(this.Configuration);
         services.SetupSwagger(this.Configuration);


         services.AddProvider<DropboxProvider>(options =>
         {
            // Add Options
         });
         services.AddProvider<SharePointProvider>(options =>
         {
            // Add Options
         });
         services.AddProviderWithSites<SharePointProvider>(options =>
         {
            // Add Options
         });
         services.AddProvider<OneDriveProvider>(options =>
         {
            // Add Options
         });
      }

      /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.EnvironmentName == "development") app.UseDeveloperExceptionPage();
         app.UseStaticFiles();

         // We want to use our own stuff
         app.UseTeamworkAuthentication();

         // Enable Swagger
         app.UseSwagger();
         app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "TCDev CSW v1"); });

         app.UseRouting();
         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllers();

            endpoints.MapControllerRoute(
               "account",
               "account/{action=Get}",
               new
               {
                  controller = typeof(AccountController)
               }
            );

            endpoints.MapControllerRoute(
               "folders",
               "folders/{folderId?}/{action=GetFolderContents}",
               new
               {
                  controller = typeof(FoldersController)
               }
            );

            endpoints.MapControllerRoute(
               "files",
               "files/{fileId?}/{action=GetFolderContent}",
               new
               {
                  controller = typeof(FilesController)
               }
            );

            endpoints.MapControllerRoute(
               "area_proxy",
               "proxy/{*url}",
               new
               {
                  controller = "Proxy", action = "Get"
               }
            );
         });

         // Enable providers
         app.UseTeamworkProviderSelector();
         app.UseDropbox();
         app.UseSharePoint();
         app.UseOneDrive();
      }
   }
}
