using Client.Providers.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<AuthenticationStateProvider, ComfamPassAuthProvider>();

await builder.Build().RunAsync();
