using Domain.Entities;

namespace Application.DTOs
{
    public class DeviceSettingsResponseDTO : BaseDTO
    {
        public float Value { get; set; }
        public int SettingId { get; set; }
        public string? UpdateStatus { get; set; }
        public int DeviceId { get; set; }
        public int? UserId { get; set; }
    }
}
