namespace Application.DTOs;

public class ManagementTokenRequestContent
{
    public string client_id { get; set; } = string.Empty;
    public string client_secret { get; set; } = string.Empty;
    public string audience { get; set; } = string.Empty;
    public string grant_type { get; set; } = string.Empty;
}