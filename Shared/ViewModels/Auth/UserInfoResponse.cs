using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.ViewModels.Auth;

public class UserInfoResponse
{
    [JsonPropertyName("sub")]
    public string UniqueIdentifier { get; set; }

    public string Name { get; set; }

    [JsonPropertyName("given_name")]
    public string GivenName { get; set; }

    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; }

    public string Email { get; set; }

    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    public string Picture { get; set; }

    public string Locale { get; set; }



    public const string UserIdClaimType = "sub";
    public const string NameClaimType = "name";


    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal) =>
       new()
       {
           UserId = GetRequiredClaim(principal, UserIdClaimType),
           Name = GetRequiredClaim(principal, NameClaimType),
       };

    public ClaimsPrincipal ToClaimsPrincipal() =>
        new(new ClaimsIdentity(
            [new(UserIdClaimType, Email), new(NameClaimType, Name)],
            authenticationType: nameof(UserInfo),
            nameType: NameClaimType,
            roleType: null));

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value ?? throw new InvalidOperationException($"Could not find required '{claimType}' claim.");

}
