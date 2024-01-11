using BoDi;

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
        
        [BeforeScenario]
        public void CreateHttpClient()
        {
            var httpClient = new HttpClient();
            _objectContainer.RegisterInstanceAs<HttpClient>(httpClient);
        }
    }
}