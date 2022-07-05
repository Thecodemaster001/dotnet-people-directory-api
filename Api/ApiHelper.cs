using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace dotnet_test_project.Api
{
    public static class ApiHelper
    {
        public static readonly Uri TripPinBaseAddress = new Uri("https://services.odata.org/TripPinRESTierService/");

        public static HttpClient CreateClient(HttpMessageHandler handler = null)
        {
            HttpClient client = handler == null ? new HttpClient() : new HttpClient(handler);
            client.BaseAddress = TripPinBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
