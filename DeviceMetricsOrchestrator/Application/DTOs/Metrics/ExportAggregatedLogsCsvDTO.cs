using Domain.Enums;
using System.Text.Json.Serialization;

namespace Application.DTOs.Metrics
{
    public class ExportAggregatedLogsCsvDTO
    {
        public string? FileName { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AggregatedLogDateType AggregatedLogDateType { get; set; } 
        public int DeviceId { get; set; }
        public int FieldId { get; set; }
    }
}
