// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System;

namespace TCDev.CloudStorage.Model
{
   public class CSWSystemState
   {
      public bool IsActive { get; set; } = true;
      public DateTime Started { get; set; } = new DateTime(2018, 05, 01);
   }
}