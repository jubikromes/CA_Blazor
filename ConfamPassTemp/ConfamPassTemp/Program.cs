using Blazored.LocalStorage;
using ConfamPassTemp.Client.Pages;
using ConfamPassTemp.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Shared.Interfaces;
using Shared.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("AuthOptions"));


builder.Services.AddHttpClient("Auth");

builder.Services.AddHttpClient<PersistingAuthenticationStateProvider>(options =>
{
    options.BaseAddress = new Uri("https://localhost:7191/");
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<PersistingAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<PersistingAuthenticationStateProvider>());

builder.Services.AddScoped(
    sp => (IAuthService)sp.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddBlazoredLocalStorage();
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();


builder.Services.AddMudServices();

var app = builder.Build();
app.UsePathBase("/confampass");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.MapGroup("/authentication").MapLoginAndLogout();

app.Run();
