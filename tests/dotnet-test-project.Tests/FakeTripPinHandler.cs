using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dotnet_test_project.Tests
{
    // Stands in for the TripPin service: records each request and replies with a canned body.
    sealed class FakeTripPinHandler : HttpMessageHandler
    {
        readonly HttpStatusCode status;
        readonly string body;

        public FakeTripPinHandler(HttpStatusCode status, string body = "{}")
        {
            this.status = status;
            this.body = body;
        }

        public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            });
        }
    }

    static class TripPinSamples
    {
        public const string PeopleCollection = """
            {
              "@odata.context": "https://services.odata.org/TripPinRESTierService/$metadata#People",
              "value": [
                {
                  "UserName": "russellwhyte", "FirstName": "Russell", "LastName": "Whyte", "MiddleName": null,
                  "Gender": "Male", "Age": null, "Emails": ["Russell@example.com", "Russell@contoso.com"],
                  "FavoriteFeature": "Feature1", "Features": [],
                  "AddressInfo": [{ "Address": "187 Suffolk Ln.", "City": { "Name": "Boise", "CountryRegion": "United States", "Region": "ID" } }],
                  "HomeAddress": null
                },
                {
                  "UserName": "scottketchum", "FirstName": "Scott", "LastName": "Ketchum", "MiddleName": null,
                  "Gender": "Male", "Age": null, "Emails": ["Scott@example.com"],
                  "FavoriteFeature": "Feature1", "Features": [], "AddressInfo": [], "HomeAddress": null
                }
              ]
            }
            """;

        public const string EmptyCollection = """{ "value": [] }""";

        public const string Russell = """
            {
              "UserName": "russellwhyte", "FirstName": "Russell", "LastName": "Whyte",
              "Gender": "Male", "Age": 42, "Emails": ["Russell@example.com"],
              "FavoriteFeature": "Feature1", "Features": ["Feature1", "Feature2"],
              "AddressInfo": [{ "Address": "187 Suffolk Ln.", "City": { "Name": "Boise", "CountryRegion": "United States", "Region": "ID" } }],
              "HomeAddress": { "Address": "1 Home Rd.", "City": null }
            }
            """;

        public const string SparsePerson = """{ "UserName": "sparse", "FirstName": "Sam", "LastName": "Parse" }""";
    }
}
