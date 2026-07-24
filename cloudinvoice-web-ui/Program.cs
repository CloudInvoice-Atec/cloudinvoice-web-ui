using cloudinvoice_web_ui.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
