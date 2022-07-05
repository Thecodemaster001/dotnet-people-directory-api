using dotnet_test_project.Api;
using dotnet_test_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_test_project.BLL
{
    public class PeopleBLL
    {
        static readonly string url = "https://services.odata.org/TripPinRESTierService/People";
        public static async Task<People> GetPeoples()
        {
            HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                People people = await response.Content.ReadFromJsonAsync<People>();

                return people;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public static async Task<People> FilterPeople(string firstName)
        {
            string filter = Uri.EscapeDataString($"FirstName eq {ODataString(firstName)}");
            HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url + "?$filter=" + filter);
            if (response.IsSuccessStatusCode)
            {
                People people = await response.Content.ReadFromJsonAsync<People>();

                return people;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        public static async Task<Person> GetSpecificPeople(string userName)
        {
            string uri = url + "(" + Uri.EscapeDataString(ODataString(userName)) + ")";
            HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(uri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            if (response.IsSuccessStatusCode)
            {
                Person people = await response.Content.ReadFromJsonAsync<Person>();

                return people;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
        // OData string literals escape a single quote by doubling it.
        static string ODataString(string value) => "'" + (value ?? string.Empty).Replace("'", "''") + "'";

        public static bool ValidInput(int option)
        {
            if (option == 1 || option == 2 || option == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
