using Model.Entities;

namespace Model;

public class StatusType : BaseEntity
{
	public ICollection<Status>? Statusses { get; set; } = new List<Status>();
	public ICollection<UserOnStatusType>? UserOnStatusTypes { get; set; } = new List<UserOnStatusType>();
}
