using BoDi;
using Microsoft.Extensions.Configuration;

namespace DeviceMetricsOrchestratorsTests.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeTestRun(IObjectContainer container)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configurationBuilder.AddEnvironmentVariables();

            var configuration = configurationBuilder.Build();

            container.RegisterInstanceAs<IConfiguration>(configuration);
        }

        [BeforeScenario]
        public void CreateHttpClient()
        {
            var httpClient = new HttpClient();
            _objectContainer.RegisterInstanceAs<HttpClient>(httpClient);
        }
    }
}