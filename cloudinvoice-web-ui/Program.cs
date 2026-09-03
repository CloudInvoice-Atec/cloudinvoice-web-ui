using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.Components;
using cloudinvoice_web_ui.Services.Auth;
using cloudinvoice_web_ui.Services.Catalog;
using cloudinvoice_web_ui.Services.Customers;
using cloudinvoice_web_ui.Services.Invoices;
using cloudinvoice_web_ui.Services.Settings;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;

var supportedCultures = new[] { new CultureInfo("pt-PT") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-PT"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true; // Ligar os erros detalhados!
    });

// 1. Core Auth Services (A nossa implementação JWT)
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/"; // A tua página de login
    });

builder.Services.AddAuthorization(); // Ativa a verificação de Roles e Policies

// 2. Named HttpClients para os Microserviços
builder.Services.AddHttpClient("IdentityAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:IdentityApi"] ?? "https://localhost:5001");
});

builder.Services.AddHttpClient("CatalogAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:CatalogApi"] ?? "https://localhost:5003");
});

builder.Services.AddHttpClient("BillingAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:BillingApi"] ?? "https://localhost:5005");
});

// 3. Domain Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();