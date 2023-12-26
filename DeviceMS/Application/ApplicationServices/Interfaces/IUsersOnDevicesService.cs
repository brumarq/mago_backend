using Application.DTOs.UsersOnDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices.Interfaces
{
    public interface IUsersOnDevicesService
    {
        Task<UserOnDeviceResponseDTO> CreateUserOnDeviceAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO);
        Task<bool> DeleteUserOnDeviceAsync(int userOnDeviceId);
    }
}
