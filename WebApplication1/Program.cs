using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.AppData;

// Setup connection string
var builder = WebApplication.CreateBuilder(args);

// Get connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

// Register DbContext
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(connectionString));

// Register Identity
// builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AppDBContext>();

builder.Services.AddIdentity<AppUser, IdentityRole>(
    options => {
        // Keep old configuration
        options.SignIn.RequireConfirmedAccount = true; 
        
        // Configure Password
        // options.Password.RequireDigit = false;
        // options.Password.RequiredLength = 6;
        // options.Password.RequireNonAlphanumeric = false;
        // options.Password.RequireUppercase = false;
        // options.Password.RequireLowercase = false;
    }
)
    .AddEntityFrameworkStores<AppDBContext>()
    .AddDefaultTokenProviders();

// Register controllers
builder.Services.AddControllersWithViews();

// Register Razor Pages
builder.Services.AddRazorPages();

// Register session support
builder.Services.AddDistributedMemoryCache();

// Configure session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Identity cookies
builder.Services.ConfigureApplicationCookie(options => {
    // Configure default login/logout paths
    options.LoginPath = "/User/Login"; 
    options.LogoutPath = "/User/Logout";
    options.AccessDeniedPath = "/User/AccessDenied";
});

// Build the application
var app = builder.Build();

// Seed Roles and Admin User
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DataSeeder.SeedRolesAsync(roleManager);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    await DataSeeder.SeedAdminUserAsync(userManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Configure error handling
    app.UseExceptionHandler("/Home/Error");
    // Configure HSTS
    app.UseHsts();
}

// Configure middleware
app.UseHttpsRedirection();

// Configure static files
app.UseStaticFiles(); 

// Configure routing
app.UseRouting();

// Configure session
app.UseSession();

// Configure authorization
app.UseAuthorization();

// Configure areas
app.MapControllerRoute(
    name : "areas",
    pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// Configure default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Shop}/{action=Index}/{id?}");

// Configure Razor Pages
app.MapRazorPages();

// Run the application
app.Run();