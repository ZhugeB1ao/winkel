using Microsoft.AspNetCore.Identity;

namespace winkel.AppData;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
    
    public string? Address { get; set; }
}