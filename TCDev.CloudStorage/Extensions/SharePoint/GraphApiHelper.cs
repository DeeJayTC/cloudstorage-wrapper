// TCDev 2022/03/17
// Apache 2.0 License
// https://www.github.com/deejaytc/dotnet-utils

using System.Net.Http.Headers;
using Microsoft.Graph;

namespace TCDev.CloudStorage.Extensions.SharePoint
{
   public static class GraphApiHelper
   {
      public static GraphServiceClient GetAuthenticatedClient(string accessToken)
      {
         var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
            async requestMessage => { requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); }));

         return graphClient;
      }
   }
}