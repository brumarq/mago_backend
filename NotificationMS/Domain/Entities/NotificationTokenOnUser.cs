namespace Domain.Entities
{
    public class NotificationTokenOnUser : BaseEntity
    {
        public string? NotificationToken { get; set; }
        public string? UserId { get; set; }
    }
}
    