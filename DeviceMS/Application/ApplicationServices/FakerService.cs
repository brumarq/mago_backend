using Application.ApplicationServices.Interfaces;
using Bogus;
using Domain.Entities;

namespace Application.ApplicationServices
{
    public class FakerService : IFakerService
    {
        public async Task<IEnumerable<Device>> GetFakeDevicesAsync()
        {
            return await Task.FromResult(GenerateFakeDevices(20));
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
    }
}
