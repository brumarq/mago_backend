using Application.DTOs;
using System;

public interface IFirmwareService
{
    Task<CreateFileSendDTO> CreateFileSendAsync(CreateFileSendDTO createFileSendDTO);
    Task<IEnumerable<FileSendResponseDTO>> GetFileSendHistoryByDeviceId(int deviceId);
}
