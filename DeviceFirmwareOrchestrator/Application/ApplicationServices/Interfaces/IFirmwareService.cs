using Application.DTOs.Firmware;

namespace Application.ApplicationServices.Interfaces;

public interface IFirmwareService
{
    Task<FileSendResponseDTO> CreateFileSendAsync(CreateFileSendDTO newFileSendDto);
    Task<IEnumerable<FileSendResponseDTO>> GetFirmwareHistoryForDeviceAsync(int deviceId);
}