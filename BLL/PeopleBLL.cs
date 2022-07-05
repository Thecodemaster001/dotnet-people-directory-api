using dotnet_test_project.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace dotnet_test_project.BLL
{
    public class PeopleBLL
    {
        const string PeoplePath = "People";
        readonly HttpClient client;

        public PeopleBLL(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<People> GetPeople() => GetCollection(PeoplePath);

        public Task<People> FilterPeople(string firstName)
        {
            string filter = Uri.EscapeDataString($"FirstName eq {ODataString(firstName)}");
            return GetCollection($"{PeoplePath}?$filter={filter}");
        }

        public async Task<Person> GetPerson(string userName)
        {
            string path = $"{PeoplePath}({Uri.EscapeDataString(ODataString(userName))})";
            using HttpResponseMessage response = await client.GetAsync(path);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            EnsureSuccess(response);
            return await response.Content.ReadFromJsonAsync<Person>();
        }

        public static bool IsValidOption(int option) => option >= 1 && option <= 3;

        async Task<People> GetCollection(string path)
        {
            using HttpResponseMessage response = await client.GetAsync(path);
            EnsureSuccess(response);
            return await response.Content.ReadFromJsonAsync<People>();
        }

        static void EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"TripPin returned {(int)response.StatusCode} {response.ReasonPhrase}", null, response.StatusCode);
            }
        }

        // OData string literals escape a single quote by doubling it.
        static string ODataString(string value) => "'" + (value ?? string.Empty).Replace("'", "''") + "'";
    }
}
