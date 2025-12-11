using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.AppData;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string from appsettings.json.
// If it's missing, throw an exception.
var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") 
    ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

// Register Entity Framework DbContext using SQL Server.
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(connectionString));

// Add ASP.NET Core Identity with default configuration.
// RequireConfirmedAccount = true means user must confirm email before login.
builder.Services.AddDefaultIdentity<AppUser>(options => 
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDBContext>();

// Add MVC controllers and views to the service container.
builder.Services.AddControllersWithViews();

// Register in-memory cache for storing session data.
builder.Services.AddDistributedMemoryCache();

// Register Session service with custom session cookie settings.
builder.Services.AddSession(cfg =>
{
    cfg.Cookie.Name = ".ShoppingCart";      // Name of the session cookie stored in the browser
    cfg.IdleTimeout = TimeSpan.FromMinutes(30); // Session expiration time
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Use custom error page when not in development mode.
    app.UseExceptionHandler("/Home/Error");

    // Enable HTTP Strict Transport Security (forces HTTPS).
    app.UseHsts();
}

// Redirect all HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Enable serving static files (images, CSS, JS, etc.)
app.UseStaticFiles();

// Enable Session middleware — required before Routing.
app.UseSession();

// Enable request routing.
app.UseRouting();

// Enable authorization (checks user permissions).
app.UseAuthorization();

// Route for Areas (Admin, User, etc.)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// Default route for the website.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Shop}/{action=Index}/{id?}"
);

// Enable Razor Pages (required for Identity UI)
app.MapRazorPages();

// Start the application.
app.Run();
