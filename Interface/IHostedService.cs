using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DefaultUsers : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultUsers ( IServiceProvider serviceProvider )
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync ( CancellationToken cancellationToken )
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await EnsureRoles(roleManager);

        foreach (var userEmail in new [] { "librarian1@example.com", "librarian2@example.com" })
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                user = new IdentityUser { UserName = userEmail, Email = userEmail };
                var result = await userManager.CreateAsync(user, "Librarian@123");
                if (!result.Succeeded)
                {
                    continue;
                }
            }

            await userManager.AddToRoleAsync(user, "Librarian");
        }
    }

    public Task StopAsync ( CancellationToken cancellationToken ) => Task.CompletedTask;

    public async Task CreateUser ( UserManager<IdentityUser> userManager, string email, string password )
    {
        var user = new IdentityUser { UserName = email, Email = email };
        await userManager.CreateAsync(user, password);
    }

    private async Task EnsureRoles ( RoleManager<IdentityRole> roleManager )
    {
        string [] roleNames = { "Librarian", "Student", "Member" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
