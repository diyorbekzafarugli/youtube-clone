using System.Text.RegularExpressions;

namespace YouTubeClone.Application.Common.Validators;

public static class PasswordValidator
{
    public static bool HasUppercase(string password) 
        => Regex.IsMatch(password, "[A-Z]");
    public static bool HasLowercase(string password) 
        => Regex.IsMatch(password, "[a-z]");
    public static bool HasDigit(string password) 
        => Regex.IsMatch(password, "[0-9]");
    public static bool HasSpecialChar(string password) 
        => Regex.IsMatch(password, @"[\W_]");
}
