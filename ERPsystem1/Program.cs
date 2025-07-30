using ERPsystem1.Components;
using ERPsystem1.Data;
using ERPsystem1.Models;
using ERPsystem1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myConnection")));

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


builder.Services.AddScoped<UserService>();


builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7176/"); // Replace with your actual URL
});

var app = builder.Build();


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


app.MapPost("/api/login", async (
    UserService userService,
    [FromBody] ERPsystem1.Components.Pages.Login.LoginModel login) =>
{
    var (success, error) = await userService.LoginAsync(login.Email, login.Password);
    return success ? Results.Ok() : Results.BadRequest(error);

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
