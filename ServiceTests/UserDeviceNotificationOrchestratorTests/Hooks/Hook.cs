using System;
using BoDi;
using TechTalk.SpecFlow;
using Microsoft.Extensions.Configuration;

namespace ServiceTests.Hooks
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
            var configuration = configurationBuilder.Build();

            container.RegisterInstanceAs<IConfiguration>(configuration);
        }

        [BeforeScenario]
        public void CreateHttpClient()
        {
            var httpClient = new HttpClient();
            // Configure your HttpClient here (e.g., headers, base address, etc.)
            _objectContainer.RegisterInstanceAs<HttpClient>(httpClient);
            
        }
    }

}