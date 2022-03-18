// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TCDev.CloudStorage.Model;

namespace TCDev.CloudStorage.Controllers
{
   /// <summary>
   ///    Account Controller for CSP requests
   /// </summary>
   [Produces("application/json")]
   [ProducesResponseType(typeof(List<ICloudStorageProviderSystemState>), 200)]
   [Route("/system")]
   public class SystemController : Controller
   {
      /// <summary>
      ///    Returns a list of implemented providers, working functions for those and ad
      /// </summary>
      /// <returns>
      ///    <see cref="ICloudStorageProviderSystemState" />
      /// </returns>
      /// <response code="200" cref="ICloudStorageProviderSystemState">OK</response>
      [HttpGet("")]
      public object Get()
      {
         return new List<ICloudStorageProviderSystemState>();
      }
   }


   /// <summary>
   ///    Retrieve System Health Data
   /// </summary>
   [Produces("application/json")]
   [ProducesResponseType(typeof(CSWSystemState), 200)]
   [Route("/health")]
   public class HealthController : Controller
   {
      /// <summary>
      ///    Get information about system health, uptime and provider state
      /// </summary>
      /// <returns>
      ///    <see cref="CSWSystemState" />
      /// </returns>
      /// <response code="200" cref="CSWSystemState">OK</response>
      [HttpGet("")]
      public object Get()
      {
         return Json("OK");
      }
   }
}