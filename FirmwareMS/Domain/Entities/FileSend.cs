namespace Domain.Entities;

public class FileSend : BaseEntity
{
    public string? UpdateStatus { get; set; }
    public int DeviceId { get; set; }
    public int UserId { get; set; }
    public string? File { get; set; }
    public int CurrPart { get; set; }
    public int TotParts { get; set; }
}