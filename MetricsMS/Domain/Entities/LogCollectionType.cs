namespace Domain.Entities;

public class LogCollectionType : BaseEntity
{
    public ICollection<LogCollection>? Collections { get; set; } = new List<LogCollection>();
}