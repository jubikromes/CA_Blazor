using ConfamPassTemp.Models;
using Shared.ViewModels.Auth;

namespace Shared.Interfaces;

public interface IAuthService
{
    Task<UserInfoResponse> LoginAsync(SigninRequest request);
    Task<FormResult> RegisterAsync(RegisterRequest request);
    public Task<bool> CheckAuthenticatedAsync();
    public Task LogoutAsync();

}
