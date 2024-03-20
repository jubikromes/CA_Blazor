using Blazored.LocalStorage;
using ConfamPassTemp.Client.Pages;
using ConfamPassTemp.Components;
using ConfamPassTemp.Providers.Auth;
using Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient<AuthService>(options =>
{
    options.BaseAddress = new Uri("https://localhost:7191/");
});

builder.Services.AddScoped<AuthenticationStateProvider, ComfamPassAuthProvider>();
builder.Services.AddAuthorizationCore();

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




app.Run();
