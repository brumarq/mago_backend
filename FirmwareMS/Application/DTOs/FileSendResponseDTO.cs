using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class FileSendResponseDTO : BaseDTO
    {
        public string? UpdateStatus { get; set; }
        public int DeviceId { get; set; }
        public int UserId { get; set; }
        public string? File { get; set; }
        public int CurrPart { get; set; }
        public int TotParts { get; set; }
    }
}
