using Application.ApplicationServices.Interfaces;
using Bogus;
using Domain.Entities;

namespace Application.ApplicationServices
{
    public class FakerService : IFakerService
    {
        private static readonly Lazy<FakerService> LazyInstance = new Lazy<FakerService>(() => new FakerService());
        public static FakerService Instance => LazyInstance.Value;

        private IEnumerable<LogCollection> _fakeDeviceMetrics;
        private IEnumerable<AggregatedLog> _fakeAggregatedLogs;

        public FakerService()
        {
            _fakeDeviceMetrics = GenerateFakeDeviceMetrics(20);
            _fakeAggregatedLogs = GenerateFakeAggregatedLogs(20);
        }

        public async Task<IEnumerable<LogCollection>> GetFakeDeviceMetricsAsync()
        {
            return await Task.FromResult(Instance._fakeDeviceMetrics);
        }

        public async Task<IEnumerable<AggregatedLog>> GetFakeAggregatedLogsAsync()
        {
            return await Task.FromResult(Instance._fakeAggregatedLogs);
        }

        private IEnumerable<LogCollection> GenerateFakeDeviceMetrics(int count)
        {
            var faker = new Faker<LogCollection>()
                .RuleFor(u => u.Id, f => f.IndexFaker + 1)
                .RuleFor(u => u.DeviceId, f => f.IndexFaker + 1)
                .RuleFor(u => u.Timestamp, f => f.Date.Past())
                .RuleFor(u => u.Values, f => GenerateFakeLogValues(f.Random.Number(1, 10)));

            return faker.Generate(count);
        }

        private IEnumerable<AggregatedLog> GenerateFakeAggregatedLogs(int count)
        {
            var faker = new Faker<AggregatedLog>()
                .RuleFor(al => al.Id, f => f.IndexFaker + 1)
                .RuleFor(al => al.Type, f => f.Lorem.Word())
                .RuleFor(al => al.AverageValue, f => f.Random.Double())
                .RuleFor(al => al.MinValue, f => f.Random.Double())
                .RuleFor(al => al.MaxValue, f => f.Random.Double())
                .RuleFor(al => al.Date, f => GenerateRandomDate(f));

            return faker.Generate(count);
        }

        private DateOnly GenerateRandomDate(Faker faker)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var weekBack = currentDate.AddDays(-faker.Random.Number(1, 7));
            var monthBack = currentDate.AddMonths(-faker.Random.Number(1, 4));
            var yearBack = currentDate.AddYears(-faker.Random.Number(1, 1));

            var selectedRange = faker.PickRandom(weekBack, monthBack, yearBack);

            return selectedRange;
        }

        private List<LogValue> GenerateFakeLogValues(int numberOfValues)
        {
            var logValueFaker = new Faker<LogValue>()
                .RuleFor(lv => lv.Id, faker => faker.IndexFaker + 1)
                .RuleFor(lv => lv.Value, faker => faker.Random.Float())
                .RuleFor(lv => lv.Field, faker => new Field
                {
                    Id = faker.IndexFaker + 1,
                    Name = faker.Lorem.Word(),
                    UnitId = faker.Random.Number(1, 10),
                    DeviceTypeId = faker.Random.Number(1, 5),
                    Loggable = faker.Random.Bool()
                });

            return logValueFaker.Generate(numberOfValues);
        }
    }
}
