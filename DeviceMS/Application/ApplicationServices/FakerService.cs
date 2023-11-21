using Application.ApplicationServices.Interfaces;
using Bogus;
using Domain.Entities;

namespace Application.ApplicationServices
{
    public class FakerService : IFakerService
    {
        private static readonly Lazy<FakerService> LazyInstance = new Lazy<FakerService>(() => new FakerService());
        public static FakerService Instance => LazyInstance.Value;

        private IEnumerable<Device> _fakeDevices;
        private IEnumerable<DeviceType> _fakeDeviceTypes;

        public FakerService()
        {
            _fakeDevices = GenerateFakeDevices(20);
            _fakeDeviceTypes = GenerateFakeDeviceTypes(20);
        }

        public async Task<IEnumerable<Device>> GetFakeDevicesAsync()
        {
            return await Task.FromResult(Instance._fakeDevices);
        }

        public async Task<IEnumerable<DeviceType>> GetFakeDeviceTypesAsync()
        {
            return await Task.FromResult(Instance._fakeDeviceTypes);
        }

        public async Task CreateFakeDeviceTypeAsync(DeviceType deviceType)
        {
            Instance._fakeDeviceTypes = Instance._fakeDeviceTypes.Concat(new[] { deviceType });

            await Task.CompletedTask;
        }

        public async Task UpdateFakeDeviceTypeAsync(int id, DeviceType deviceType)
        {
            var deviceTypes = Instance._fakeDeviceTypes.ToList();

            var singleDeviceType = Instance._fakeDeviceTypes.FirstOrDefault(x => x.Id == id);

            deviceTypes.Add(singleDeviceType);

            await Task.CompletedTask;
        }

        private IEnumerable<Device> GenerateFakeDevices(int count)
        {
            var faker = new Faker<Device>()
                .RuleFor(d => d.Name, f => f.Commerce.ProductName())
                .RuleFor(d => d.DeviceType, f => GenerateFakeDeviceType())
                .RuleFor(d => d.SendSettingsAtConn, f => f.Random.Bool())
                .RuleFor(d => d.SendSettingsNow, f => f.Random.Bool())
                .RuleFor(d => d.AuthId, f => f.Random.Guid().ToString())
                .RuleFor(d => d.PwHash, f => f.Internet.Password())
                .RuleFor(d => d.Salt, f => f.Random.AlphaNumeric(8))
                .RuleFor(d => d.Id, f => f.IndexFaker + 1);

            return faker.Generate(count);
        }

        private DeviceType GenerateFakeDeviceType()
        {
            var faker = new Faker<DeviceType>()
                .RuleFor(dt => dt.Name, f => f.Commerce.Department())
                .RuleFor(dt => dt.Id, f => f.IndexFaker + 1);

            return faker.Generate();
        }

        private IEnumerable<DeviceType> GenerateFakeDeviceTypes(int count)
        {
            var faker = new Faker<DeviceType>()
                .RuleFor(dt => dt.Id, f => f.IndexFaker + 1)
                .RuleFor(dt => dt.Name, f => f.Commerce.ProductName());

            return faker.Generate(count);
        }
    }
}
