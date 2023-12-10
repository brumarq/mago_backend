namespace Application.DTOs.Firmware;

public class CreateFileSendDTO
{
    public int DeviceId { get; set; }
    public int UserId { get; set; }
    public string File { get; set; }
}