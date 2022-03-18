using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCDev.CloudStorage.StartupExtension
{
	public class CorsConfig
	{
		public string Name { get; set; }
		public bool AllowAllOrigins { get; set; } = true;
		public bool AllowAllMethods { get; set; } = true;
		public string[] AllowedOrigins { get; set; } = new string[] { "localhost" };
		public string[] AllowedMethods { get; set; } = new string[] { "GET" };

		public bool AllowCredentials { get; set; } = true;
		public string[] ExposedHeaders { get; set; } = new string[] {};
	}
}
