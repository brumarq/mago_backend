using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities.Devices
{
    public class Quantity : BaseEntity
    {
        public string? Name { get; set; }
        public Unit? BaseUnit { get; set; }
        public ICollection<Unit>? Units { get; set; } = new HashSet<Unit>();
    }
}
