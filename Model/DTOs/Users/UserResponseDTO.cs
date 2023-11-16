namespace Model.DTOs.Users
{
    public class UserResponseDTO : BaseDTO
    {
        public string? Name { get; set; }
        public bool SysAdmin { get; set; } // maybe this too because only an admin can set this
        public string? Password { get; set; }

        // FIXME: Response contains User Id (from baseDTO) and password?
    }
}
