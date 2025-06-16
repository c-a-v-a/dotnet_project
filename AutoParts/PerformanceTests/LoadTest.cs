namespace PerformanceTests;

using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using System.Net.Http;
using System.Threading.Tasks;

public class LoadTest
{
    public static void Run(string jwtToken)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");

        var scenario = Scenario.Create("GetActiveOrders", async context =>
        {
            var response = await httpClient.GetAsync("http://localhost:7252/api/ServiceOrders");

            var statusCode = (int)response.StatusCode;
            var body = await response.Content.ReadAsStringAsync();

            return Response.Ok(statusCode == 200 ? body : null, 0);
        })
        .WithWarmUpDuration(TimeSpan.FromSeconds(1))
        .WithLoadSimulations(Simulation.KeepConstant(50, TimeSpan.FromSeconds(20))); // 50 równoległych użytkowników przez 20 sekund

        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFolder("./reports")
            .WithReportFormats(ReportFormat.Txt, ReportFormat.Html)
            .Run();

        // Raport PDF będzie dostępny w folderze ./reports
    }
}

