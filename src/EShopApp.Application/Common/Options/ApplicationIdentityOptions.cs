namespace EShopApp.Application.Common.Options;

public class ApplicationIdentityOptions
{
    public int PasswordRequiredLength { get; set; } = 8;
    public bool PasswordRequireDigit { get; set; } = true;
    public bool PasswordRequireLowercase { get; set; } = true;
    public bool PasswordRequireUppercase { get; set; } = true;
    public bool PasswordRequireNonAlphanumeric { get; set; } = true;
}