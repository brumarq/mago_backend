   namespace Model.Entities.Users
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public bool SysAdmin { get; set; }
        public string? Password { get; set; }
    }
}
