// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Extensions
{
   /// <summary>
   ///    Extension class for cloud storage providers.
   /// </summary>
   /// <seealso cref="ApplicationBuilderExtensions" />
   /// <seealso cref="ICloudStorageProvider" />
   public static class ServiceCollectionExtensions
   {
      private static readonly JsonSerializer ActivitySerializer = JsonSerializer.Create();

      /// <summary>
      ///    Adds and configures services for a <typeparamref name="TProviderType">specified provider type</typeparamref> to the
      ///    <see cref="IServiceCollection" />.
      /// </summary>
      /// <typeparam name="TProviderType">
      ///    A concrete type of
      ///    <see cref="ICloudStorageProvider"/> that is to be registered and exposed to the Bot Framework.
      /// </typeparam>
      /// <param name="services">The <see cref="IServiceCollection" />.</param>
      /// <param name="configureAction">A callback that can further be used to configure the bot.</param>
      /// <returns>A reference to this instance after the operation has completed.</returns>
      public static IServiceCollection AddProvider<TProviderType>(this IServiceCollection services, Action<ICloudStorageProviderOptions> configureAction = null) where TProviderType : class, ICloudStorageProvider
      {
         services.AddTransient<ICloudStorageProvider, TProviderType>();

         services.Configure(configureAction);

         return services;
      }

      /// <summary>
      ///    Adds and configures services for a <typeparamref name="TProviderType">specified provider type</typeparamref> to the
      ///    <see cref="IServiceCollection" />.
      /// </summary>
      /// <typeparam name="TProviderType">
      ///    A concrete type of
      ///    <see cref="ICloudStorageProviderWithSites"/> that is to be registered and exposed to the Bot Framework.
      /// </typeparam>
      /// <param name="services">The <see cref="IServiceCollection" />.</param>
      /// <param name="configureAction">A callback that can further be used to configure the bot.</param>
      /// <returns>A reference to this instance after the operation has completed.</returns>
      public static IServiceCollection AddProviderWithSites<TProviderType>(this IServiceCollection services, Action<ICloudStorageProviderOptions> configureAction = null) where TProviderType : class, ICloudStorageProviderWithSites
      {
         services.AddTransient<ICloudStorageProviderWithSites, TProviderType>();

         services.Configure(configureAction);

         return services;
      }
   }
}