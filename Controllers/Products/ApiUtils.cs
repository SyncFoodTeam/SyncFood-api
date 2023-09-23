using System.Net.Http.Headers;
using SyncFoodApi.Controllers.Products.DTO;
using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Products
{
    public static class ApiUtils
    {
        static string FREndPoint = "https://fr.openfoodfacts.org/";
        static string ENEndpoint = "https://en.openfoodfacts.org/";
        static HttpClient client = new HttpClient();

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri(FREndPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }


        /*public static async ProductPrivateDTO getProduct(string barCode)
        {
            string route = "product/";
            HttpResponseMessage response = await client.GetAsync(route += barCode);
            if (response.IsSuccessStatusCode)
            {
                response.Content
            }
        }*/
    }
}
