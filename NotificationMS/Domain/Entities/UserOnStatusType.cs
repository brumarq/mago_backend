namespace Domain.Entities;

public class UserOnStatusType : BaseEntity
{
    public int UserId { get; set; }
    public int DeviceId { get; set; }
    public StatusType? StatusType { get; set; }
}