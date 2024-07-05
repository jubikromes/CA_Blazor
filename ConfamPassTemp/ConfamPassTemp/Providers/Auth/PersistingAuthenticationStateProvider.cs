using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Shared.Extensions;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using ConfamPassTemp.Models;
using Shared.Interfaces;
using Shared.ViewModels.Auth;
using Shared.ViewModels;
using Blazored.LocalStorage;
using Newtonsoft.Json.Linq;
using Shared.Options;
using Microsoft.Extensions.Options;


/// <summary>
/// Handles state for cookie-based auth.
/// </summary>
public class PersistingAuthenticationStateProvider : AuthenticationStateProvider, IAuthService
{
    private readonly AuthOptions _authOptions;
    private static UserInfoResponse? UserInfo { get; set; }

    /// <summary>
    /// Map the JavaScript-formatted properties to C#-formatted classes.
    /// </summary>
    private readonly JsonSerializerOptions jsonSerializerOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    /// <summary>
    /// Special auth client.
    /// </summary>
    private readonly HttpClient _httpClient;


    /// <summary>
    /// Authentication state.
    /// </summary>
    private bool _authenticated = false;

    /// <summary>
    /// Default principal for anonymous (not authenticated) users.
    /// </summary>
    private readonly ClaimsPrincipal Unauthenticated =
        new(new ClaimsIdentity());

    /// <summary>
    /// Create a new instance of the auth provider.
    /// </summary>
    /// <param name="httpClientFactory">Factory to retrieve auth client.</param>
    public PersistingAuthenticationStateProvider(IHttpClientFactory httpClientFactory, IOptions<AuthOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient("Auth");
        _authOptions = options.Value;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>The result serialized to a <see cref="FormResult"/>.
    /// </returns>
    public async Task<FormResult> RegisterAsync(RegisterRequest registerRequest)
    {
        string[] defaultDetail = ["An unknown error prevented registration from succeeding."];

        try
        {
            // make the request
            var result = await _httpClient.PostAsJsonAsync(
                "register", new
                {
                    registerRequest.Email,
                    registerRequest.Password
                });

            // successful?
            if (result.IsSuccessStatusCode)
            {
                return new FormResult { Succeeded = true };
            }

            // body should contain details about why it failed
            var details = await result.Content.ReadAsStringAsync();
            var problemDetails = JsonDocument.Parse(details);
            var errors = new List<string>();
            var errorList = problemDetails.RootElement.GetProperty("errors");

            foreach (var errorEntry in errorList.EnumerateObject())
            {
                if (errorEntry.Value.ValueKind == JsonValueKind.String)
                {
                    errors.Add(errorEntry.Value.GetString()!);
                }
                else if (errorEntry.Value.ValueKind == JsonValueKind.Array)
                {
                    errors.AddRange(
                        errorEntry.Value.EnumerateArray().Select(
                            e => e.GetString() ?? string.Empty)
                        .Where(e => !string.IsNullOrEmpty(e)));
                }
            }

            // return the error list
            return new FormResult
            {
                Succeeded = false,
                ErrorList = problemDetails == null ? defaultDetail : [.. errors]
            };
        }
        catch { }

        // unknown error
        return new FormResult
        {
            Succeeded = false,
            ErrorList = defaultDetail
        };
    }

    /// <summary>
    /// User login.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>The result of the login request serialized to a <see cref="FormResult"/>.</returns>
    public async Task<UserInfoResponse> LoginAsync(SigninRequest signInRequest)
    {
        try
        {
            // login with cookies
            var data = new[]
            {
                new KeyValuePair<string, string>("password", signInRequest.Password),
                new KeyValuePair<string, string>("username", signInRequest.Username),
                new KeyValuePair<string, string>("client_id" ,_authOptions.ClientId),
                new KeyValuePair<string, string>("scope" , _authOptions.Scopes),
                new KeyValuePair<string, string>("grant_type" ,_authOptions.GrantType)
            };
            var result = await _httpClient.PostAsync("https://localhost:7191/connect/token", new FormUrlEncodedContent(data));

            if (result.IsSuccessStatusCode)
            {
                var jsonString = await result.Content.ReadFromJsonAsync<TokenResponse>();

                var claimsFromJWT = jsonString?.AccessToken.ParseClaimsFromJwt();

                UserInfo = new UserInfoResponse { Email = signInRequest.Username };

                // need to refresh auth state
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());


                // success!
                return UserInfo;
            }
        }
        catch (Exception ex) { }

        // unknown error
        return null;
    }

    /// <summary>
    /// Get authentication state.
    /// </summary>
    /// <remarks>
    /// Called by Blazor anytime and authentication-based decision needs to be made, then cached
    /// until the changed state notification is raised.
    /// </remarks>
    /// <returns>The authentication state asynchronous request.</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;

        // default to not authenticated
        var user = Unauthenticated;

        try
        {
            if (UserInfo != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, UserInfo.Email),
                    new(ClaimTypes.Email, UserInfo.Email)
                };

                var id = new ClaimsIdentity(claims, nameof(PersistingAuthenticationStateProvider));
                user = new ClaimsPrincipal(id);
                _authenticated = true;
            }
            // to be used later

            //// the user info endpoint is secured, so if the user isn't logged in this will fail
            //var userResponse = await _httpClient.GetAsync("manage/info");

            //// throw if user info wasn't retrieved
            //userResponse.EnsureSuccessStatusCode();

            //// user is authenticated,so let's build their authenticated identity
            //var userJson = await userResponse.Content.ReadAsStringAsync();
            //var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, jsonSerializerOptions);


            //if (userInfo != null)
            //{
            //    // in our system name and email are the same
            //    //var claims = new List<Claim>
            //    //    {
            //    //        new(ClaimTypes.Name, userInfo.Email),
            //    //        new(ClaimTypes.Email, userInfo.Email)
            //    //    };

            //    //// add any additional claims
            //    //claims.AddRange(
            //    //    userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
            //    //        .Select(c => new Claim(c.Key, c.Value)));

            //    // tap the roles endpoint for the user's roles
            //    var rolesResponse = await _httpClient.GetAsync("roles");

            //    // throw if request fails
            //    rolesResponse.EnsureSuccessStatusCode();

            //    // read the response into a string
            //    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();

            //    // deserialize the roles string into an array
            //    var roles = JsonSerializer.Deserialize<RoleClaim[]>(rolesJson, jsonSerializerOptions);

            //    // if there are roles, add them to the claims collection
            //    if (roles?.Length > 0)
            //    {
            //        foreach (var role in roles)
            //        {
            //            if (!string.IsNullOrEmpty(role.Type) && !string.IsNullOrEmpty(role.Value))
            //            {
            //                //claims.Add(new Claim(role.Type, role.Value, role.ValueType, role.Issuer, role.OriginalIssuer));
            //            }
            //        }
            //    }

            // set the principal
            //var id = new ClaimsIdentity(claims, nameof(PersistingAuthenticationStateProvider));
            //user = new ClaimsPrincipal(id);
            //_authenticated = true;
            // }
        }
        catch { }

        // return the state
        return new AuthenticationState(user);
    }

    public async Task LogoutAsync()
    {
        const string Empty = "{}";
        var emptyContent = new StringContent(Empty, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("logout", emptyContent);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return _authenticated;
    }
    private async Task OnPersistingAsync()
    {
        //var authenticationState = await GetAuthenticationStateAsync();
        //var principal = authenticationState.User;

        //if (principal.Identity?.IsAuthenticated == true)
        //{
        //    persistentComponentState.PersistAsJson(nameof(UserInfo), UserInfo.FromClaimsPrincipal(principal));
        //}
    }

    public class RoleClaim
    {
        public string? Issuer { get; set; }
        public string? OriginalIssuer { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }
        public string? ValueType { get; set; }
    }
}