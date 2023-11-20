namespace NotificationsService.Core.Domain.Models;

public class StatusType : BaseEntity
{
	public ICollection<Status>? Statuses { get; set; } = new List<Status>();
	public ICollection<UserOnStatusType>? UserOnStatusTypes { get; set; } = new List<UserOnStatusType>();
}
