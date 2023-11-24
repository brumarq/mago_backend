namespace Application.DTOs.Setting
{
    public class SettingValueResponseDTO : BaseDTO
    {
        public float Value { get; set; }
        public SettingDTO? Setting { get; set; }
        public string? UpdateStatus { get; set; }
        public int DeviceId { get; set; }
        public int? UserId { get; set; }
    }
}