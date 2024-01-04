using Application.DTOs.UsersOnDevices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices.Interfaces
{
    public interface IDeviceService
    {
        void CheckDeviceExistence(int deviceID);
        Task<UserOnDeviceResponseDTO> CreateUserOnDeviceEntryAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO);
        Task<IEnumerable<UserOnDeviceResponseDTO>> GetUserOnDeviceEntryByUserId(string userId);
        Task<IActionResult> DeleteUserOnDeviceEntryAsync(string userId, int deviceId);
    }
}
