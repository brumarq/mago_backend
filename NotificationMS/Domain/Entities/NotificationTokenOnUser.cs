using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class NotificationTokenOnUser : BaseEntity
    {
        public string? NotificationToken { get; set; }
        public string? UserId { get; set; }
    }
}
