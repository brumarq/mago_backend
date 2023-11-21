using Domain.Entities;

namespace Application.ApplicationServices.Interfaces
{
    public interface IFakerService
    {
        Task<IEnumerable<Device>> GetFakeDevicesAsync();
    }
}
