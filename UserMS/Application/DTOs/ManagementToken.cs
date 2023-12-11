namespace Application.DTOs;

public class ManagementToken
{
    public string Token { get; set; }
    public DateTime ExpirationTime { get; set; }
}