using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Devices
{
    public class Unit : BaseEntity
    {
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        public float? Factor { get; set; }
        public float? Offset { get; set; }
        public Quantity? Quantity { get; set; }
        public ICollection<Quantity>? BaseOfQuantities { get; set; } = new HashSet<Quantity>();
        public ICollection<Field>? Fields { get; set; } = new HashSet<Field>();
        public ICollection<Setting>? Settings { get; set; } = new HashSet<Setting>();
    }
}
