using dotnet_test_project.Api;
using dotnet_test_project.BLL;
using dotnet_test_project.View;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace dotnet_test_project.Tests
{
    public class PeopleViewTests
    {
        const string Menu =
            "Please select from below\n1-Show People\n2-Search People\n3-Show Details of Specific Person\nPress 1,2 or 3 from Above...\n";

        static async Task<string> Run(string typed, HttpStatusCode status, string body = "{}")
        {
            var client = ApiHelper.CreateClient(new FakeTripPinHandler(status, body));
            var output = new StringWriter { NewLine = "\n" };
            await new PeopleView(new PeopleBLL(client), new StringReader(typed), output).Run();
            return output.ToString();
        }

        [Fact]
        public async Task Option_1_lists_every_person()
        {
            string text = await Run("1\n", HttpStatusCode.OK, TripPinSamples.PeopleCollection);

            Assert.Equal(Menu +
                "User Name : russellwhyte\n\tName : Russell Whyte\n\tAge : \n\tEmail : Russell@example.com, Russell@contoso.com\n" +
                "User Name : scottketchum\n\tName : Scott Ketchum\n\tAge : \n\tEmail : Scott@example.com\n", text);
        }

        [Fact]
        public async Task Option_2_reports_no_match()
        {
            string text = await Run("2\nNobody\n", HttpStatusCode.OK, TripPinSamples.EmptyCollection);

            Assert.EndsWith("Enter First Name : No Record Found\n", text);
        }

        [Fact]
        public async Task Option_3_prints_details_including_gender_and_home_address()
        {
            string text = await Run("3\nrussellwhyte\n", HttpStatusCode.OK, TripPinSamples.Russell);

            Assert.EndsWith(
                "Enter User Name : User Name : russellwhyte\n\tName : Russell Whyte\n\tAge : 42\n\tEmail : Russell@example.com\n" +
                "\tGender : Male\n\tFavourite Feature : Feature1\n\tFeatures : Feature1, Feature2\n" +
                "\tAddress : 187 Suffolk Ln.\n\tHome Address : 1 Home Rd.\n", text);
        }

        [Fact]
        public async Task Option_3_tolerates_a_person_without_lists()
        {
            string text = await Run("3\nsparse\n", HttpStatusCode.OK, TripPinSamples.SparsePerson);

            Assert.Contains("\tEmail : -\n", text);
            Assert.Contains("\tFeatures : -\n\tAddress : -\n", text);
        }

        [Fact]
        public async Task Option_3_reports_an_unknown_user()
        {
            string text = await Run("3\nnobody\n", HttpStatusCode.NotFound);

            Assert.EndsWith("No Record Found\n", text);
        }

        [Fact]
        public async Task Server_error_is_reported_without_crashing()
        {
            string text = await Run("1\n", HttpStatusCode.ServiceUnavailable);

            Assert.EndsWith("Request failed: TripPin returned 503 Service Unavailable\n", text);
        }

        [Fact]
        public async Task Retries_non_numbers_and_stops_at_end_of_input()
        {
            string text = await Run("abc\n", HttpStatusCode.OK);

            Assert.Equal(Menu + "Not a valid number, try again.\n", text);
        }

        [Fact]
        public async Task Rejects_an_out_of_range_option()
        {
            string text = await Run("7\n", HttpStatusCode.OK);

            Assert.Equal(Menu + "Invalid Option\n", text);
        }
    }
}
