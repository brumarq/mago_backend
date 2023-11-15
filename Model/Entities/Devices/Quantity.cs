using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities.Devices
{
    public class Quantity : BaseEntity
    {
        public string? Name { get; set; }
        public Unit? Unit { get; set; }
    }
}
