using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Services.Auth;
using cloudinvoice_web_ui.Services.Customers;
using cloudinvoice_web_ui.Services.Invoices;
using cloudinvoice_web_ui.Services.Settings;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<JwtAuthorizationHandler>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Define "Cookies" como o esquema padrão para lidar com os redirecionamentos do [Authorize]
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        // Define para onde o utilizador é atirado caso tente aceder a uma página protegida sem login
        options.LoginPath = "/"; // Se o teu login for noutra rota (ex: "/public/login"), altera aqui
    });
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// HttpClient para a Identity.API
builder.Services.AddHttpClient("IdentityAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:IdentityApi"] ?? "https://localhost:5001");
});

// HttpClient para a Catalog.API
builder.Services.AddHttpClient("CatalogAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:CatalogApi"] ?? "https://localhost:5003");
})
    .AddHttpMessageHandler<JwtAuthorizationHandler>();

// HttpClient para a Billing.API
builder.Services.AddHttpClient("BillingAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:BillingApi"] ?? "https://localhost:5005");
})
    .AddHttpMessageHandler<JwtAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
