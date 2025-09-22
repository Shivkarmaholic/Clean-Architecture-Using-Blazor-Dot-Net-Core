using BlazorApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Infrastructure.Persistence
{
    public static class ApplicationDbContextInitializer
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var env = services.GetRequiredService<IHostEnvironment>();
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            try
            {
                //logger.LogInformation("Ensuring database exists and applying migrations...");
                //await context.Database.MigrateAsync();
                //await SeedDatabaseAsync(context, services);
                //logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database");

                // In Development, rethrow to surface issues; otherwise, keep the app running.
                if (env.IsDevelopment())
                {
                    throw;
                }
            }
        }

        private static async Task SeedDatabaseAsync(ApplicationDbContext context, IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                if (!await context.Customers.AnyAsync())
                {
                    context.Customers.AddRange(
                        new Customer
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "john.doe@example.com",
                            PhoneNumber = "555-123-4567",
                            Address = "123 Main St, Anytown, USA",
                            CreatedDate = DateTime.UtcNow
                        },
                        new Customer
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "Jane",
                            LastName = "Smith",
                            Email = "jane.smith@example.com",
                            PhoneNumber = "555-987-6543",
                            Address = "456 Oak Ave, Somewhere, USA",
                            CreatedDate = DateTime.UtcNow
                        }
                    );

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Don’t crash startup because of seed issues; log and continue.
                logger.LogError(ex, "An error occurred while seeding the database");
            }
        }
    }
}