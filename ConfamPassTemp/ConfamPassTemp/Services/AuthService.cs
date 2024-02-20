using ConfamPassTemp.Components.ViewModels.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Newtonsoft.Json;

namespace ConfamPassTemp.Services;

public class AuthService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<SigninResponse?> Login(SigninRequest request)
    {
        var data = new[]
            {
                new KeyValuePair<string, string>("password", request.Password),
                new KeyValuePair<string, string>("username", request.Username),
                new KeyValuePair<string, string>("client_id" ,"test_webapp"),
                new KeyValuePair<string, string>("scope" ,"openid profile email roles offline_access"),
                new KeyValuePair<string, string>("grant_type" ,"password")
            };
        var result = await _httpClient.PostAsync("connect/token", new FormUrlEncodedContent(data));
        if (result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SigninResponse>(content);
        }
        else
        {
            return null;
        }
    }
    public async Task<bool> Register(Components.ViewModels.Auth.RegisterRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("auth/register", request);
        return result.IsSuccessStatusCode;
    }
}
