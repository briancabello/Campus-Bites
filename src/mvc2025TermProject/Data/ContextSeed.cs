using Microsoft.AspNetCore.Identity;

namespace mvc2025TermProject.Data
{
    public enum Roles
    {
        User,
        Administrator
    }

    public static class ContextSeed
    {
        public async static Task SeedRolesAsync(UserManager<CampusBitesUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Administrator.ToString()));

            return;
        }
    }
}
