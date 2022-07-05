using dotnet_test_project.Api;
using dotnet_test_project.BLL;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace dotnet_test_project.Tests
{
    public class PeopleBLLTests
    {
        static (PeopleBLL, FakeTripPinHandler) Create(HttpStatusCode status, string body = "{}")
        {
            var handler = new FakeTripPinHandler(status, body);
            return (new PeopleBLL(ApiHelper.CreateClient(handler)), handler);
        }

        [Fact]
        public async Task GetPeople_maps_the_odata_value_array()
        {
            var (bll, handler) = Create(HttpStatusCode.OK, TripPinSamples.PeopleCollection);

            var people = await bll.GetPeople();

            Assert.Equal(new[] { "russellwhyte", "scottketchum" }, people.Value.ConvertAll(p => p.UserName));
            Assert.Null(people.Value[0].Age);
            Assert.Equal("Boise", people.Value[0].AddressInfo[0].City.Name);
            var request = Assert.Single(handler.Requests);
            Assert.Equal("https://services.odata.org/TripPinRESTierService/People", request.RequestUri.AbsoluteUri);
            Assert.Contains(request.Headers.Accept, h => h.MediaType == "application/json");
        }

        [Fact]
        public async Task FilterPeople_builds_an_eq_filter_on_first_name()
        {
            var (bll, handler) = Create(HttpStatusCode.OK, TripPinSamples.EmptyCollection);

            await bll.FilterPeople("Russell");

            Assert.Equal("?$filter=FirstName%20eq%20%27Russell%27", handler.Requests[0].RequestUri.Query);
        }

        [Fact]
        public async Task FilterPeople_escapes_quotes_and_reserved_characters()
        {
            var (bll, handler) = Create(HttpStatusCode.OK, TripPinSamples.EmptyCollection);

            await bll.FilterPeople("O'Neil&$top=1");

            Assert.Equal("?$filter=FirstName%20eq%20%27O%27%27Neil%26%24top%3D1%27", handler.Requests[0].RequestUri.Query);
        }

        [Fact]
        public async Task GetPerson_requests_the_quoted_key()
        {
            var (bll, handler) = Create(HttpStatusCode.OK, TripPinSamples.Russell);

            var person = await bll.GetPerson("russellwhyte");

            Assert.Equal("Whyte", person.LastName);
            Assert.Equal(42, person.Age);
            Assert.EndsWith("/People(%27russellwhyte%27)", handler.Requests[0].RequestUri.AbsoluteUri);
        }

        [Fact]
        public async Task GetPerson_returns_null_when_the_user_does_not_exist()
        {
            var (bll, _) = Create(HttpStatusCode.NotFound);

            Assert.Null(await bll.GetPerson("nobody"));
        }

        [Fact]
        public async Task Server_errors_surface_as_HttpRequestException_with_the_status()
        {
            var (bll, _) = Create(HttpStatusCode.InternalServerError);

            var ex = await Assert.ThrowsAsync<HttpRequestException>(() => bll.GetPeople());

            Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
            Assert.Contains("500", ex.Message);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        public void IsValidOption_accepts_only_menu_entries(int option, bool expected)
        {
            Assert.Equal(expected, PeopleBLL.IsValidOption(option));
        }
    }
}
