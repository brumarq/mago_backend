namespace Model.Entities
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public bool SysAdmin { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
