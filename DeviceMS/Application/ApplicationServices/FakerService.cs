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
        private IEnumerable<SettingValue> _fakeSettingValues;

        public FakerService()
        {
            _fakeDevices = GenerateFakeDevices(20);
            _fakeDeviceTypes = GenerateFakeDeviceTypes(20);
            _fakeSettingValues = GenerateFakeSettingValues(20);
        }

        public async Task<IEnumerable<Device>> GetFakeDevicesAsync()
        {
            return await Task.FromResult(Instance._fakeDevices);
        }

        public async Task CreateFakeDeviceAsync(Device device)
        {
            Instance._fakeDevices = Instance._fakeDevices.Concat(new[] { device });

            await Task.CompletedTask;
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

        public async Task<IEnumerable<SettingValue>> GetFakeSettingValues()
        {
            return await Task.FromResult(Instance._fakeSettingValues);
        }

        #region Generation of fake data for Devices
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
        #endregion

        #region Generation of fake data for DeviceTypes
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
        #endregion

        #region Generation of fake data for Device Settings
        private IEnumerable<SettingValue> GenerateFakeSettingValues(int count)
        {
            var faker = new Faker<SettingValue>()
                .RuleFor(s => s.Id, f => f.IndexFaker + 1)
                .RuleFor(s => s.Value, f => f.Random.Float())
                .RuleFor(s => s.Setting, f => GenerateFakeSetting())
                .RuleFor(s => s.UpdateStatus, f => f.Random.Words())
                .RuleFor(s => s.Device, f => f.PickRandom(_fakeDevices))
                .RuleFor(s => s.UserId, f => f.Random.Int());

            return faker.Generate(count);
        }

        private Setting GenerateFakeSetting()
        {
            var faker = new Faker<Setting>()
                .RuleFor(s => s.Id, f => f.IndexFaker+1)
                .RuleFor(s => s.Name, f => f.Lorem.Word())
                .RuleFor(s => s.DefaultValue, f => f.Random.Float())
                .RuleFor(s => s.Unit, f => GenerateFakeUnit())
                .RuleFor(s => s.DeviceType, f => f.PickRandom(_fakeDeviceTypes))
                .RuleFor(s => s.ViewedBy, f => f.Name.FullName())
                .RuleFor(s => s.EditedBy, f => f.Name.FullName());

            return faker.Generate();
        }

        private Unit GenerateFakeUnit()
        {
            var faker = new Faker<Unit>()
                .RuleFor(u => u.Id, f => f.IndexFaker + 1)
                .RuleFor(u => u.Name, f => f.Lorem.Word())
                .RuleFor(u => u.Symbol, f => f.Random.String(3))
                .RuleFor(u => u.Factor, f => f.Random.Float())
                .RuleFor(u => u.Offset, f => f.Random.Float())
                .RuleFor(u => u.Quantity, f => GenerateFakeQuantity());

            return faker.Generate();
        }

        private Quantity GenerateFakeQuantity()
        {
            var faker = new Faker<Quantity>()
                .RuleFor(q => q.Id, f => f.IndexFaker + 1)
                .RuleFor(q => q.Name, f => f.Lorem.Word());

            return faker.Generate();
        }
        #endregion
    }
}
