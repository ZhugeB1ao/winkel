using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.AppData;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly AppDBContext _context;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    
    public UserController(
        ILogger<UserController> logger, 
        AppDBContext context, 
        UserManager<AppUser> userManager,
        IUserStore<AppUser> userStore,
        SignInManager<AppUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl="")
    {
        returnUrl ??= Url.Content("~/");
        // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        if (ModelState.IsValid)
        {
            var user = CreateUser();
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.PhoneNumber = model.Phone;
            user.Address = model.Address;
            user.EmailConfirmed = true;
            
            // Set username into database
            await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                        
            // Encrypt password and set username into database
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Shop");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View();
    }

    public IActionResult Login(string returnUrl = "")
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "")
    {
        if (ModelState.IsValid)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "Products", new { area = "Admin" });
                }
                else
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Shop");
                }
             
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
        }
        
        return View(model);
    }
    
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        
        // Clear all session data (cart, etc.)
        HttpContext.Session.Clear();
        
        return RedirectToAction("Index", "Home");
    }

    private AppUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<AppUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                                                $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }
}