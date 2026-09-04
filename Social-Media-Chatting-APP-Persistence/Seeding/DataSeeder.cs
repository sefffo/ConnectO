using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social_Media_Chatting_APP_Domain.Entities;

namespace Social_Media_Chatting_APP_Persistence.Seeding
{
    public static class DataSeeder
    {
        private static readonly List<(AppUser User, string Password)> SeedUsers =
        [
            (
                new AppUser
                {
                    Id = "11111111-1111-1111-1111-111111111111",
                    UserName = "ProfileA",
                    NormalizedUserName = "PROFILEA",
                    Email = "profilea@connecto.test",
                    NormalizedEmail = "PROFILEA@CONNECTO.TEST",
                    EmailConfirmed = true,
                    DisplayName = "Profile A",
                    Bio = "Hey! I am Profile A — a test account for ConnectO.",
                    Gender = "Other",
                    DateOfBirth = new DateOnly(2000, 1, 1),
                    IsOnline = false,
                    IsGoogleAccount = false,
                    IsTwoFactorSetup = false,
                    CreatedAt = DateTime.UtcNow,
                    AllowMessageFromStrangers = true,
                    ShowOnlineStatus = true,
                    ShowLastSeen = true,
                    IsDeactivated = false,
                    LockoutEnabled = false
                },
                "Test@1234!"
            ),
            (
                new AppUser
                {
                    Id = "22222222-2222-2222-2222-222222222222",
                    UserName = "ProfileB",
                    NormalizedUserName = "PROFILEB",
                    Email = "profileb@connecto.test",
                    NormalizedEmail = "PROFILEB@CONNECTO.TEST",
                    EmailConfirmed = true,
                    DisplayName = "Profile B",
                    Bio = "Hey! I am Profile B — a test account for ConnectO.",
                    Gender = "Other",
                    DateOfBirth = new DateOnly(2000, 1, 1),
                    IsOnline = false,
                    IsGoogleAccount = false,
                    IsTwoFactorSetup = false,
                    CreatedAt = DateTime.UtcNow,
                    AllowMessageFromStrangers = true,
                    ShowOnlineStatus = true,
                    ShowLastSeen = true,
                    IsDeactivated = false,
                    LockoutEnabled = false
                },
                "Test@1234!"
            )
        ];

        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(DataSeeder));

            foreach (var (user, password) in SeedUsers)
            {
                var exists = await userManager.FindByNameAsync(user.UserName!);
                if (exists is not null)
                {
                    logger.LogInformation("Seed user '{UserName}' already exists — skipping.", user.UserName);
                    continue;
                }

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    logger.LogInformation("Seed user '{UserName}' created successfully.", user.UserName);
                else
                    logger.LogError("Failed to create seed user '{UserName}': {Errors}",
                        user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
