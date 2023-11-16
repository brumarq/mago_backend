using Model.Entities;

namespace Model;

public class LogCollectionType : BaseEntity
{
	public ICollection<LogCollection>? Collections { get; set; } = new List<LogCollection>();
}
