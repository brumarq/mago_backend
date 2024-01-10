using System;
using BoDi;
using TechTalk.SpecFlow;

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

        [BeforeScenario]
        public void CreateHttpClient()
        {
            var httpClient = new HttpClient();
            // Configure your HttpClient here (e.g., headers, base address, etc.)
            _objectContainer.RegisterInstanceAs<HttpClient>(httpClient);
        }
    }

}