using dotnet_test_project.Api;
using dotnet_test_project.BLL;
using dotnet_test_project.View;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnet_test_project
{
    class Program
    {
        static async Task Main()
        {
            using HttpClient client = ApiHelper.CreateClient();
            await new PeopleView(new PeopleBLL(client), Console.In, Console.Out).Run();
        }
    }
}
