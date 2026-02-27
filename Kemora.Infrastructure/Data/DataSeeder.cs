using Kemora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await SeedGovernoratesAsync(context);
            await SeedCategoriesAsync(context);
            await SeedPlaceTypesAsync(context);
        }

        private static async Task SeedGovernoratesAsync(ApplicationDbContext context)
        {
            if (await context.Governorates.AnyAsync()) return;

            var governorates = new List<Governorate>
            {
                new() { Name = "Cairo", Region = "Greater Cairo" },
                new() { Name = "Giza", Region = "Greater Cairo" },
                new() { Name = "Alexandria", Region = "Northern Coast" },
                new() { Name = "Luxor", Region = "Upper Egypt" },
                new() { Name = "Aswan", Region = "Upper Egypt" },
                new() { Name = "Red Sea", Region = "Eastern Coast" },
                new() { Name = "South Sinai", Region = "Sinai Peninsula" },
                new() { Name = "North Sinai", Region = "Sinai Peninsula" },
                new() { Name = "Matrouh", Region = "Northern Coast" },
                new() { Name = "Fayoum", Region = "Central Egypt" },
                new() { Name = "Suez", Region = "Canal Zone" },
                new() { Name = "Ismailia", Region = "Canal Zone" },
                new() { Name = "Port Said", Region = "Canal Zone" },
                new() { Name = "Dakahlia", Region = "Nile Delta" },
                new() { Name = "Sharqia", Region = "Nile Delta" },
                new() { Name = "Qalyubia", Region = "Greater Cairo" },
                new() { Name = "Kafr El Sheikh", Region = "Nile Delta" },
                new() { Name = "Gharbia", Region = "Nile Delta" },
                new() { Name = "Monufia", Region = "Nile Delta" },
                new() { Name = "Beheira", Region = "Nile Delta" },
                new() { Name = "Beni Suef", Region = "Central Egypt" },
                new() { Name = "Minya", Region = "Central Egypt" },
                new() { Name = "Asyut", Region = "Upper Egypt" },
                new() { Name = "Sohag", Region = "Upper Egypt" },
                new() { Name = "Qena", Region = "Upper Egypt" },
                new() { Name = "New Valley", Region = "Western Desert" },
                new() { Name = "Damietta", Region = "Nile Delta" }
            };

            context.Governorates.AddRange(governorates);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categories = new List<Category>
            {
                new() { Name = "Historical" },
                new() { Name = "Beach" },
                new() { Name = "Cultural" },
                new() { Name = "Adventure" },
                new() { Name = "Religious" },
                new() { Name = "Nature" },
                new() { Name = "Shopping" },
                new() { Name = "Food & Dining" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedPlaceTypesAsync(ApplicationDbContext context)
        {
            if (await context.PlaceTypes.AnyAsync()) return;

            // Get the category IDs
            var historical = await context.Categories.FirstAsync(c => c.Name == "Historical");
            var beach = await context.Categories.FirstAsync(c => c.Name == "Beach");
            var cultural = await context.Categories.FirstAsync(c => c.Name == "Cultural");
            var adventure = await context.Categories.FirstAsync(c => c.Name == "Adventure");
            var religious = await context.Categories.FirstAsync(c => c.Name == "Religious");
            var nature = await context.Categories.FirstAsync(c => c.Name == "Nature");
            var shopping = await context.Categories.FirstAsync(c => c.Name == "Shopping");
            var food = await context.Categories.FirstAsync(c => c.Name == "Food & Dining");

            var types = new List<PlaceType>
            {
                new() { GoogleType = "temple", DisplayName = "Temple", CategoryID = historical.CategoryID },
                new() { GoogleType = "pyramid", DisplayName = "Pyramid", CategoryID = historical.CategoryID },
                new() { GoogleType = "museum", DisplayName = "Museum", CategoryID = cultural.CategoryID },
                new() { GoogleType = "beach_resort", DisplayName = "Beach Resort", CategoryID = beach.CategoryID },
                new() { GoogleType = "national_park", DisplayName = "National Park", CategoryID = nature.CategoryID },
                new() { GoogleType = "mosque", DisplayName = "Mosque", CategoryID = religious.CategoryID },
                new() { GoogleType = "church", DisplayName = "Church", CategoryID = religious.CategoryID },
                new() { GoogleType = "restaurant", DisplayName = "Restaurant", CategoryID = food.CategoryID },
                new() { GoogleType = "market", DisplayName = "Market/Bazaar", CategoryID = shopping.CategoryID },
                new() { GoogleType = "hotel", DisplayName = "Hotel", CategoryID = beach.CategoryID },
                new() { GoogleType = "oasis", DisplayName = "Oasis", CategoryID = nature.CategoryID },
                new() { GoogleType = "diving_spot", DisplayName = "Diving Spot", CategoryID = adventure.CategoryID },
                new() { GoogleType = "safari", DisplayName = "Desert Safari", CategoryID = adventure.CategoryID }
            };

            context.PlaceTypes.AddRange(types);
            await context.SaveChangesAsync();
        }
    }
}
