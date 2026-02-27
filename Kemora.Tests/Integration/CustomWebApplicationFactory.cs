using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Kemora.Tests.Integration
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove ALL EF Core registrations
                var efDescriptors = services.Where(d =>
                    d.ServiceType.FullName != null &&
                    (d.ServiceType.FullName.Contains("DbContextOptions") ||
                     d.ServiceType == typeof(ApplicationDbContext) ||
                     d.ServiceType.FullName.Contains("EntityFrameworkCore"))).ToList();

                foreach (var descriptor in efDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Re-register with InMemory
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + System.Guid.NewGuid().ToString());
                });
            });
        }
    }
}
