namespace NotificationsService.Core.Domain.Models;

public class UserOnStatusType : BaseEntity
{
	public int UserId { get; set; }
	public int DeviceId { get; set; }
	public StatusType? StatusType { get; set; }
}