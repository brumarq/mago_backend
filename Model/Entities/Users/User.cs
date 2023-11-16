namespace Model.Entities.Users
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool SysAdmin { get; set; }
        public string? PwHash { get; set; }
        public string? Salt { get; set; }
    }
}
