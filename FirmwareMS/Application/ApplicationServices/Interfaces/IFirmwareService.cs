using Application.DTOs;

public interface IFirmwareService
{
    Task<CreateFileSendDTO> CreateFileSendAsync(CreateFileSendDTO createFileSendDTO);
    Task<IEnumerable<FileSendResponseDTO>> GetFileSendHistoryByDeviceIdAsync(int deviceId);
}
