namespace Domain.Entities;

public class StatusType: BaseEntity
{
    
    public string Name { get; set; }
    public ICollection<Status>? Statuses { get; set; } = new List<Status>();
}