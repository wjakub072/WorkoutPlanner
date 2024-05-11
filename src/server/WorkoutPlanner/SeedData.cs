using Microsoft.AspNetCore.Identity;
using Serilog;
using WorkoutPlanner.Api.Repositories;
using WorkoutPlanner.Models.Auth;

namespace WorkoutPlanner.Api;

public class SeedData
{
    public static bool HasCalled(string[] args)
    {
        bool seed = args.Contains("--seed");
        return seed && Environment.UserInteractive;
    }

    public static string[] ClearArguments(string[] args)
    {
        return args.Except(new[] { "--seed" }).ToArray();
    }

    public static async Task EnsureSeedDataAsync(IServiceProvider services){
        try{
            await ensureSeedDataAsync(services);
        } catch (Exception e)
        {
            System.Console.WriteLine(e.ToString());
        }
    }
    private static async Task ensureSeedDataAsync(IServiceProvider services)
    {
        Console.Write("Set admin user name: ");
        string adminName = Console.ReadLine() ?? "";

        Console.Write("Set admin password: ");
        string adminPassword = GetPassword();

        if (adminName.Length == 0)
        {
            throw new NotSupportedException("Admin name must not be empty");
        }

        if (adminPassword.Length == 0)
        {
            throw new NotSupportedException("Admin password must not be empty");
        }

        Serilog.ILogger logger = Log.ForContext<SeedData>();
        logger.Information("Seeding database...");

        using IServiceScope scope =
            services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        UserManager<ApplicationUser> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        RoleManager<ApplicationRole> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        IProfileRepository accountRepository = 
            scope.ServiceProvider.GetRequiredService<IProfileRepository>();

        // roles
        foreach (string roleName in ApplicationRoles.All)
        {
            ApplicationRole? role = await roleManager.FindByNameAsync(roleName);

            if (role != null) continue;

            IdentityResult result = await roleManager.CreateAsync(
                new ApplicationRole() { Name = roleName });

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.First().Description);
            }

            logger.Information("{Role} created", roleName);
        }

        List<IPasswordValidator<ApplicationUser>> backup = userManager.PasswordValidators.ToList();
        userManager.PasswordValidators.Clear();

        // users
        ApplicationUser? user = await userManager.FindByNameAsync(adminName);

        if (user == null)
        {
            var account = await accountRepository.CreateBlankProfileAsync(adminName, adminName);

            user = new ApplicationUser()
            {
                UserName = adminName,
                AccountId = account.Id
            };

            IdentityResult result = await userManager.CreateAsync(user, adminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.First().Description);
            }

            await userManager.SetEmailAsync(user, adminName);

            result = await userManager.AddToRoleAsync(user, ApplicationRoles.Admin);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.First().Description);
            }

            logger.Information("{User} user created", adminName);
        }
        else
        {
            logger.Information("{User} user already exists", adminName);

            string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult result =
                await userManager.ResetPasswordAsync(user, resetToken, adminPassword);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.First().Description);
            }

            logger.Information("Reset password to default for {User}", adminName);
        }

        backup.ForEach(userManager.PasswordValidators.Add);
        logger.Information("Done seeding database.");
    }

    private static string GetPassword()
    {
        Stack<char> pwd = new();
        while (true)
        {
            ConsoleKeyInfo i = Console.ReadKey(true);
            if (i.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (i.Key == ConsoleKey.Backspace)
            {
                if (pwd.Count > 0)
                {
                    pwd.Pop();
                    Console.Write("\b \b");
                }
            }
            else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
            {
                pwd.Push(i.KeyChar);
                Console.Write("*");
            }
        }

        return new string(pwd.Reverse().ToArray());
    }
}
