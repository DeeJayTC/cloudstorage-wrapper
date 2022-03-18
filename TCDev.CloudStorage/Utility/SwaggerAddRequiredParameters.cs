// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TCDev.CloudStorage.Utility
{
   public class SwaggerAddRequiredParameters : IOperationFilter
   {
      public void Apply(OpenApiOperation operation, OperationFilterContext context)
      {
         if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

         operation.Parameters.Add(new OpenApiParameter
         {
            Name = "provider",
            In = ParameterLocation.Query,
            Required = true
         });

         operation.Parameters.Add(new OpenApiParameter
         {
            Name = "token",
            In = ParameterLocation.Header,
            Required = true
         });
      }
   }
}