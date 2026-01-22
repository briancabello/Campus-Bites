using EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;

namespace mvc2025TermProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Retrieve Connection String from appsettings.json
            var connectionString = 
                builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Retrieve Email Configuration from appsettings.json
            var emailConfig = builder.Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();

            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            builder.Services.AddDbContext<ApplicationDbContext>(options => 
            
                options
                    .UseLazyLoadingProxies(useLazyLoadingProxies: true)
                    .UseSqlServer(connectionString)
            
            );

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services
                .AddDefaultIdentity<CampusBitesUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;

                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                })
                .AddRoles<IdentityRole>()   // <-- This is where we enabled Role-based Authorization
                .AddEntityFrameworkStores<ApplicationDbContext>()
                ;

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(); // ADDED

            var app = builder.Build();  // Enabling/Disabling specific services that were configured above (for USAGE)

            // Seed Roles (User, Administrator) into the Database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                    // 1. Get the DB Context and User/Role Managers
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<CampusBitesUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // 2. Seed initial Roles and User
                    await ContextSeed.SeedRolesAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
