using dotnet_test_project.BLL;
using dotnet_test_project.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace dotnet_test_project.View
{
    public class PeopleView
    {
        readonly PeopleBLL people;
        readonly TextReader input;
        readonly TextWriter output;

        public PeopleView(PeopleBLL people, TextReader input, TextWriter output)
        {
            this.people = people;
            this.input = input;
            this.output = output;
        }

        public async Task Run()
        {
            output.WriteLine("Please select from below");
            output.WriteLine("1-Show People");
            output.WriteLine("2-Search People");
            output.WriteLine("3-Show Details of Specific Person");
            output.WriteLine("Press 1,2 or 3 from Above...");

            int? option = ReadOption();
            if (option == null)
            {
                return;
            }
            if (!PeopleBLL.IsValidOption(option.Value))
            {
                output.WriteLine("Invalid Option");
                return;
            }

            try
            {
                switch (option.Value)
                {
                    case 1:
                        ShowList((await people.GetPeople())?.Value);
                        break;
                    case 2:
                        output.Write("Enter First Name : ");
                        ShowList((await people.FilterPeople(input.ReadLine()))?.Value);
                        break;
                    case 3:
                        output.Write("Enter User Name : ");
                        ShowDetails(await people.GetPerson(input.ReadLine()));
                        break;
                }
            }
            catch (HttpRequestException ex)
            {
                output.WriteLine($"Request failed: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                output.WriteLine("Request failed: the TripPin service did not answer in time");
            }
            catch (JsonException ex)
            {
                output.WriteLine($"Unexpected response from TripPin: {ex.Message}");
            }
        }

        int? ReadOption()
        {
            string line = input.ReadLine();
            int option;
            while (!int.TryParse(line, out option))
            {
                if (line == null)
                {
                    return null;
                }
                output.WriteLine("Not a valid number, try again.");
                line = input.ReadLine();
            }
            return option;
        }

        void ShowList(List<Person> list)
        {
            if (list == null || list.Count == 0)
            {
                output.WriteLine("No Record Found");
                return;
            }
            foreach (Person person in list)
            {
                ShowSummary(person);
            }
        }

        void ShowDetails(Person person)
        {
            if (person == null)
            {
                output.WriteLine("No Record Found");
                return;
            }
            ShowSummary(person);
            output.WriteLine($"\tGender : {person.Gender}");
            output.WriteLine($"\tFavourite Feature : {person.FavoriteFeature}");
            output.WriteLine($"\tFeatures : {JoinOrNone(person.Features)}");
            output.WriteLine($"\tAddress : {JoinOrNone(person.AddressInfo?.Select(location => location.Address))}");
            if (person.HomeAddress != null)
            {
                output.WriteLine($"\tHome Address : {person.HomeAddress.Address}");
            }
        }

        void ShowSummary(Person person)
        {
            output.WriteLine($"User Name : {person.UserName}");
            output.WriteLine($"\tName : {person.FirstName} {person.LastName}");
            output.WriteLine($"\tAge : {person.Age}");
            output.WriteLine($"\tEmail : {JoinOrNone(person.Emails)}");
        }

        static string JoinOrNone(IEnumerable<string> values)
        {
            string joined = string.Join(", ", values ?? Enumerable.Empty<string>());
            return joined.Length > 0 ? joined : "-";
        }
    }
}
