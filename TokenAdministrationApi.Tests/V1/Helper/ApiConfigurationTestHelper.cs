using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.Tests.V1.Helper
{
    public static class ApiConfigurationTestHelper
    {
        public static async Task<HttpResponseMessage> PostJsonAsync(
            HttpClient client,
            string path,
            object request)
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(request),
                Encoding.UTF8,
                "application/json");
            var response = await client.PostAsync(
                new Uri(path, UriKind.Relative),
                content).ConfigureAwait(true);
            content.Dispose();

            return response;
        }

        public static ApiNameLookup AddApiLookup(TokenDatabaseContext databaseContext)
        {
            var api = new ApiNameLookup
            {
                ApiName = "housing-api",
                ApiGatewayId = "gw-housing-dev"
            };
            databaseContext.ApiNameLookups.Add(api);
            databaseContext.SaveChanges();

            return api;
        }
    }
}
