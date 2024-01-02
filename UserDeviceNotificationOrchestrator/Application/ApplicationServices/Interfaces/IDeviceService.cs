﻿using Application.DTOs.UsersOnDevices;
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
        Task<HttpResponseMessage> GetDeviceExistenceStatus(int deviceId);
        Task<UserOnDeviceResponseDTO> CreateNotificationAsync(CreateUserOnDeviceDTO createUserOnDeviceDTO);
        Task<IActionResult> DeleteUserOnDeviceEntryAsync(string userId, int deviceId);
    }
}
