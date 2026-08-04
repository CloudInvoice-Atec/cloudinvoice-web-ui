using cloudinvoice_web_ui.Auth;
using cloudinvoice_web_ui.Components;
using cloudinvoice_web_ui.Services.Customers;
using cloudinvoice_web_ui.Services.Identity;
using cloudinvoice_web_ui.Services.Invoices;
using cloudinvoice_web_ui.Services.Settings;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
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


builder.Services.AddAuthentication();
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

app.UseAuthentication();
app.UseAuthorization();

// TEM DE ESTAR AQUI!
app.UseSession();

app.UseAntiforgery();

app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
