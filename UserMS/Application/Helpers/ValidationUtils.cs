using Application.Exceptions;

namespace Application.Helpers;

public static class ValidationUtils
{
    public static void ValidatePasswordStrength(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new BadRequestException("Password cannot be empty.");
        }

        var minLength = 8;
        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        if (password.Length < minLength)
        {
            throw new BadRequestException($"Password must be at least {minLength} characters long.");
        }
        if (!hasUpper)
        {
            throw new BadRequestException("Password must contain at least one uppercase letter.");
        }
        if (!hasLower)
        {
            throw new BadRequestException("Password must contain at least one lowercase letter.");
        }
        if (!hasDigit)
        {
            throw new BadRequestException("Password must contain at least one digit.");
        }
        if (!hasSpecial)
        {
            throw new BadRequestException("Password must contain at least one special character.");
        }
    }
    
    public static void ValidateEmail(string? email)
    {
        try
        {
            if (email == null) throw new BadRequestException("Email cannot be empty.");;
            
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email)
            {
                throw new BadRequestException("Invalid email format.");
            }
        }
        catch
        {
            throw new BadRequestException("Invalid email format.");
        }
    }
}
