namespace TestDomain.Firmware;

public class FileSendResponseDTO : BaseDTO
{
    public string? UpdateStatus { get; set; }
    public int DeviceId { get; set; }
    public string? File { get; set; }
    public int CurrParts { get; set; }
    public int TotParts { get; set; }
}