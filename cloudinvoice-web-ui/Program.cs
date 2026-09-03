using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.Components;
using cloudinvoice_web_ui.Services.Catalog;
using cloudinvoice_web_ui.Services.Customers;
using cloudinvoice_web_ui.Services.Identity;
using cloudinvoice_web_ui.Services.Invoices;
using cloudinvoice_web_ui.Services.Settings;
using cloudinvoice_web_ui.Services.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
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

builder.Services.AddScoped<TokenProvider>();
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

// Adiciona suporte para acesso ao HttpContext (muito comum precisar disto junto com sessões)
builder.Services.AddHttpContextAccessor();

// Configura o armazenamento em memória para a sessão
builder.Services.AddDistributedMemoryCache();

// Regista o serviço de Sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Tempo para a sessão expirar
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Necessário para funcionar mesmo sem consentimento de cookies GDPR
});


builder.Services.AddAuthorization();

// HttpClient para a Identity.API
builder.Services.AddHttpClient("IdentityAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:IdentityApi"] ?? "https://localhost:5001");
});

// HttpClient para a Catalog.API
builder.Services.AddHttpClient("CatalogAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:CatalogApi"] ?? "https://localhost:5003");
});

// HttpClient para a Billing.API
builder.Services.AddHttpClient("BillingAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:BillingApi"] ?? "https://localhost:5005");
});


builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRouting();

// TEM DE ESTAR AQUI!
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
