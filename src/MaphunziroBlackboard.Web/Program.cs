using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Serilog;
using MaphunziroBlackboard.Infrastructure.Data;
using MaphunziroBlackboard.Domain.Entities;
using MaphunziroBlackboard.Application.Services;
using MaphunziroBlackboard.Application.Interfaces;
using MaphunziroBlackboard.Infrastructure.Repositories;
using MaphunziroBlackboard.Core.Interfaces;
using MaphunziroBlackboard.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    if (context.HostingEnvironment.IsDevelopment())
    {
        configuration
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine("logs", "log-.txt"), rollingInterval: Serilog.RollingInterval.Day);
    }
    else
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Authentication configuration (OAuth schemes are registered only when ClientId + secret exist — empty appsettings would otherwise fail OAuthOptions.Validate())
var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authenticationBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.CallbackPath = "/signin-google";
    });
}

var msClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
var msClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
if (!string.IsNullOrWhiteSpace(msClientId) && !string.IsNullOrWhiteSpace(msClientSecret))
{
    authenticationBuilder.AddMicrosoftAccount(options =>
    {
        options.ClientId = msClientId;
        options.ClientSecret = msClientSecret;
        options.CallbackPath = "/signin-microsoft";
    });
}

// Add SignalR
builder.Services.AddSignalR();

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register services
builder.Services.AddScoped<IUserService, UserService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add distributed memory cache for session
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// In development, keep HTTP working even if the dev cert isn't trusted yet.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed data
await SeedData(app);

app.Run();

async Task SeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Apply migrations
        await context.Database.MigrateAsync();

        // Seed roles
        await SeedRoles(roleManager);

        // Seed default admin user
        await SeedDefaultAdmin(userManager);

        // Seed default school
        await SeedDefaultSchool(context);

        logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    var roles = new[]
    {
        "SuperAdmin",
        "SchoolAdmin",
        "Teacher",
        "Student",
        "Parent",
        "Accountant",
        "Librarian",
        "Registrar"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

async Task SeedDefaultAdmin(UserManager<ApplicationUser> userManager)
{
    const string adminEmail = "admin@maphunziro.com";
    const string adminPassword = "Admin@123";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System",
            LastName = "Administrator",
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
        }
    }
}

async Task SeedDefaultSchool(ApplicationDbContext context)
{
    if (!await context.Schools.AnyAsync())
    {
        var school = new School
        {
            Name = "Maphunziro Secondary School",
            Code = "MSS001",
            Description = "A premier secondary school in Malawi providing quality education",
            Address = "P.O. Box 1234, Lilongwe",
            City = "Lilongwe",
            Country = "Malawi",
            PostalCode = "1234",
            Phone = "+265 1 234 567",
            Email = "info@maphunziro.com",
            Website = "www.maphunziro.com",
            PrincipalName = "Dr. John Banda",
            IsActive = true,
            SchoolType = SchoolType.Private,
            EducationLevel = EducationLevel.Secondary,
            EstablishedDate = new DateTime(1990, 1, 15)
        };

        await context.Schools.AddAsync(school);
        await context.SaveChangesAsync();
    }
}
