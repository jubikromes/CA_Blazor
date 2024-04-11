using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.ViewModels.Auth;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = default!;
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; } = default!;
    [JsonPropertyName("id_token")] 
    public string IdToken { get; set; } = default!;
    [JsonPropertyName("refresh_token")] 
    public string RefreshToken { get; set; } = default!;
}
