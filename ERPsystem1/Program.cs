using ERPsystem1.Components;
using ERPsystem1.Data;
using ERPsystem1.Models;
using ERPsystem1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myConnection")));

// ✅ Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ✅ Register custom services
builder.Services.AddScoped<UserService>();

// ✅ Authentication & Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// ✅ Blazor Server setup
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);

// ✅ Register named HttpClient with BaseAddress
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7176/"); // Replace with your actual URL
});

var app = builder.Build();

// ✅ Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery(); // Must be after auth, and before endpoints

// ✅ Minimal API login endpoint
app.MapPost("/api/login", async (
    UserService userService,
    [FromBody] ERPsystem1.Components.Pages.Login.LoginModel login) =>
{
    var (success, error) = await userService.LoginAsync(login.Email, login.Password);
    return success ? Results.Ok() : Results.BadRequest(error);
});

// ✅ Razor Components
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
