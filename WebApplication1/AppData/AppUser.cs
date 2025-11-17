using Microsoft.AspNetCore.Identity;

namespace WebApplication1.AppData;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
    
    public string? Address { get; set; }
}