namespace Application.DTOs;

public class ErrorResponseDto
{
    public int StatusCode { get; set; } = 500; 
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = "Unknown error";
    public string ErrorCode { get; set; } = string.Empty;
}

