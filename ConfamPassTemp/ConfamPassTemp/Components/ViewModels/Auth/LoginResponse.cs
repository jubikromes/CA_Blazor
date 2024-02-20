namespace ConfamPassTemp.Components.ViewModels.Auth;

public class SigninRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public record SigninResponse
{
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
    public string? Token { get; set; }
}
