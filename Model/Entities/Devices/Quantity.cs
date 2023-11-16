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

        [ForeignKey("UnitId")]
        public Unit? Unit { get; set; }
    }
}
