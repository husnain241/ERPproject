using ERPsystem1.Models;
using Microsoft.AspNetCore.Identity;

namespace ERPsystem1.Services;

public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> RegisterAsync(string email, string password, string fullName)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName  // ✅ THIS LINE IS CRITICAL
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Optional: Sign in automatically
            // await _signInManager.SignInAsync(user, isPersistent: false);
            return (true, null);
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return (false, errors);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, "User not found");

        var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);

        return result.Succeeded
            ? (true, null)
            : (false, "Invalid credentials");
    }





}
