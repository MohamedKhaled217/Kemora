using Kemora.Domain.Entities;
using Microsoft.AspNetCore.Identity;
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
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedGovernoratesAsync(context);
            await SeedCategoriesAsync(context);
            await SeedPlaceTypesAsync(context);
            
            await SeedUsersAsync(userManager, context);
            await SeedTop20PlacesAsync(context);
            await SeedSocialPostsAsync(context);
        }

        private static async Task SeedGovernoratesAsync(ApplicationDbContext context)
        {
            if (await context.Governorates.AnyAsync()) return;

            var governorates = new List<Governorate>
            {
                new() { Name = "Cairo", Region = "Greater Cairo", ImageURL = "https://images.unsplash.com/photo-1572252009286-268acec5ca0a" },
                new() { Name = "Giza", Region = "Greater Cairo", ImageURL = "https://images.unsplash.com/photo-1503177119275-0aa32b3a9368" },
                new() { Name = "Alexandria", Region = "Northern Coast", ImageURL = "https://images.unsplash.com/photo-1621251817478-f685c7bb74e6" },
                new() { Name = "Luxor", Region = "Upper Egypt", ImageURL = "https://images.unsplash.com/photo-1594916301297-a7eb443a9926" },
                new() { Name = "Aswan", Region = "Upper Egypt", ImageURL = "https://images.unsplash.com/photo-1610486828590-edc9372e617d" },
                new() { Name = "Red Sea", Region = "Eastern Coast", ImageURL = "https://images.unsplash.com/photo-1500249821865-c7e4ff69b2d6" },
                new() { Name = "South Sinai", Region = "Sinai Peninsula", ImageURL = "https://images.unsplash.com/photo-1622350720516-ec7fdf41a100" },
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
                new() { Name = "New Valley", Region = "Western Desert", ImageURL = "https://images.unsplash.com/photo-1549488344-c7151e289f64" },
                new() { Name = "Damietta", Region = "Nile Delta" },
                new() { Name = "North Sinai", Region = "Sinai Peninsula" }
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
                new() { GoogleType = "safari", DisplayName = "Desert Safari", CategoryID = adventure.CategoryID },
                new() { GoogleType = "citadel", DisplayName = "Citadel/Fort", CategoryID = historical.CategoryID }
            };

            context.PlaceTypes.AddRange(types);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            if (userManager.Users.Any()) return;

            var users = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "john_doe", Email = "john@example.com", FullName = "John Doe", Country = "USA", ProfilePictureUrl = "https://i.pravatar.cc/150?u=john", UserPreferencesJSON = "{\"Budget\": \"Mid-Range\", \"Vibe\": \"Historical\"}" },
                new ApplicationUser { UserName = "sarah_smith", Email = "sarah@example.com", FullName = "Sarah Smith", Country = "UK", ProfilePictureUrl = "https://i.pravatar.cc/150?u=sarah", UserPreferencesJSON = "{\"Budget\": \"Luxury\", \"Vibe\": \"Relaxed\"}" },
                new ApplicationUser { UserName = "ahmed_ali", Email = "ahmed@example.com", FullName = "Ahmed Ali", Country = "Egypt", ProfilePictureUrl = "https://i.pravatar.cc/150?u=ahmed", UserPreferencesJSON = "{\"Budget\": \"Budget\", \"Vibe\": \"Adventure\"}" }
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Password123!");
            }
        }

        private static async Task SeedTop20PlacesAsync(ApplicationDbContext context)
        {
            if (await context.Places.CountAsync() > 25) return;

            var gizaGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Giza");
            var luxorGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Luxor");
            var cairoGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Cairo");
            var alexGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Alexandria");
            var aswanGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Aswan");
            var sSinaiGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "South Sinai");
            var newValleyGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "New Valley");
            var matrouhGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Matrouh");
            var qenaGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Qena");
            var fayoumGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Fayoum");
            var redSeaGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Red Sea");
            var asyutGov = await context.Governorates.FirstOrDefaultAsync(g => g.Name == "Asyut");

            var pyramidType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "pyramid");
            var templeType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "temple");
            var museumType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "museum");
            var citadelType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "citadel");
            var marketType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "market");
            var parkType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "national_park");
            var safariType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "safari");
            var beachType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "beach_resort");
            var divingType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "diving_spot");
            var restaurantType = await context.PlaceTypes.FirstOrDefaultAsync(pt => pt.GoogleType == "restaurant");

            if (gizaGov == null || pyramidType == null || templeType == null)
                return; // Early return if base expected data isn't seeded

            var places = new List<Place>
            {
                // Historical
                new Place { Name = "Giza Pyramids", Description = "The Great Pyramid of Giza is the oldest and largest of the three pyramids.", Address = "Al Haram, Giza", Latitude = 29.9792m, Longitude = 31.1342m, Rating = 4.8m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1503177119275-0aa32b3a9368", GovernorateID = gizaGov.GovernorateID, PlaceTypeID = pyramidType.TypeID },
                new Place { Name = "Karnak Temple", Description = "A vast mix of decayed temples, chapels, pylons near Luxor.", Address = "Karnak, Luxor", Latitude = 25.7188m, Longitude = 32.6573m, Rating = 4.9m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1594916301297-a7eb443a9926", GovernorateID = luxorGov.GovernorateID, PlaceTypeID = templeType.TypeID },
                new Place { Name = "Valley of the Kings", Description = "Valley where tombs were excavated for pharaohs.", Address = "Luxor", Latitude = 25.7402m, Longitude = 32.6014m, Rating = 4.7m, PriceLevel = 3, MainImageURL = "https://images.unsplash.com/photo-1601058268499-e52658b8ebf8", GovernorateID = luxorGov.GovernorateID, PlaceTypeID = templeType.TypeID },
                new Place { Name = "Qaitbay Citadel", Description = "A 15th-century defensive fortress on the Mediterranean sea coast.", Address = "Alexandria", Latitude = 31.2140m, Longitude = 29.8856m, Rating = 4.6m, PriceLevel = 1, MainImageURL = "https://images.unsplash.com/photo-1621251817478-f685c7bb74e6", GovernorateID = alexGov.GovernorateID, PlaceTypeID = citadelType.TypeID },
                new Place { Name = "Egyptian Museum", Description = "Home to an extensive collection of ancient Egyptian antiquities.", Address = "Tahrir Square, Cairo", Latitude = 30.0478m, Longitude = 31.2336m, Rating = 4.5m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1572252009286-268acec5ca0a", GovernorateID = cairoGov.GovernorateID, PlaceTypeID = museumType.TypeID },
                new Place { Name = "Philae Temple", Description = "Beautiful island temple complex in Aswan.", Address = "Aswan", Latitude = 24.0255m, Longitude = 32.8844m, Rating = 4.8m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1610486828590-edc9372e617d", GovernorateID = aswanGov.GovernorateID, PlaceTypeID = templeType.TypeID },
                new Place { Name = "Abu Simbel Temples", Description = "Two massive rock-cut temples in Upper Egypt.", Address = "Aswan", Latitude = 22.3370m, Longitude = 31.6258m, Rating = 4.9m, PriceLevel = 3, MainImageURL = "https://images.unsplash.com/photo-1522079031269-8db2de34b071", GovernorateID = aswanGov.GovernorateID, PlaceTypeID = templeType.TypeID },
                new Place { Name = "Edfu Temple", Description = "Extremely well-preserved Ptolemaic temple.", Address = "Edfu", Latitude = 24.9782m, Longitude = 32.8735m, Rating = 4.8m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1594916301297-a7eb443a9926", GovernorateID = aswanGov.GovernorateID, PlaceTypeID = templeType.TypeID },
                new Place { Name = "Dendera Temple Complex", Description = "One of the best-preserved temple complexes in Egypt.", Address = "Qena", Latitude = 26.1394m, Longitude = 32.6705m, Rating = 4.9m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1601058268499-e52658b8ebf8", GovernorateID = qenaGov.GovernorateID, PlaceTypeID = templeType.TypeID },
                new Place { Name = "Saqqara Step Pyramid", Description = "The oldest complete stone building complex known in history.", Address = "Badrashin, Giza", Latitude = 29.8712m, Longitude = 31.2166m, Rating = 4.8m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1503177119275-0aa32b3a9368", GovernorateID = gizaGov.GovernorateID, PlaceTypeID = pyramidType.TypeID },

                // Cultural & Religious
                new Place { Name = "Khan el-Khalili", Description = "A famous historical bazaar and souq in Cairo.", Address = "Cairo", Latitude = 30.0478m, Longitude = 31.2622m, Rating = 4.7m, PriceLevel = 1, MainImageURL = "https://images.unsplash.com/photo-1554559388-7e3e91124aba", GovernorateID = cairoGov.GovernorateID, PlaceTypeID = marketType.TypeID },
                new Place { Name = "Baron Empain Palace", Description = "A distinctive Hindu-style palace in Heliopolis.", Address = "Cairo", Latitude = 30.0867m, Longitude = 31.3303m, Rating = 4.6m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1629814407936-3980df24be70", GovernorateID = cairoGov.GovernorateID, PlaceTypeID = museumType.TypeID },
                new Place { Name = "Al-Azhar Mosque", Description = "One of the oldest and most influential Islamic landmarks.", Address = "Cairo", Latitude = 30.0457m, Longitude = 31.2599m, Rating = 4.8m, PriceLevel = 0, MainImageURL = "https://images.unsplash.com/photo-1590059039021-9b376722d7a2", GovernorateID = cairoGov.GovernorateID, PlaceTypeID = religiousType_OrDefault(context, "mosque") },
                new Place { Name = "Saladin Citadel", Description = "Medieval Islamic fortification in Cairo.", Address = "Cairo", Latitude = 30.0298m, Longitude = 31.2611m, Rating = 4.7m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1596560410041-945781a798f5", GovernorateID = cairoGov.GovernorateID, PlaceTypeID = citadelType.TypeID },
                new Place { Name = "Bibliotheca Alexandrina", Description = "Major library and cultural center on the Mediterranean.", Address = "Alexandria", Latitude = 31.2089m, Longitude = 29.9092m, Rating = 4.9m, PriceLevel = 1, MainImageURL = "https://images.unsplash.com/photo-1621251817478-f685c7bb74e6", GovernorateID = alexGov.GovernorateID, PlaceTypeID = museumType.TypeID },
                new Place { Name = "St. Catherine's Monastery", Description = "Ancient monastery at the foot of Mount Sinai.", Address = "Sinai", Latitude = 28.5559m, Longitude = 33.9760m, Rating = 4.8m, PriceLevel = 1, MainImageURL = "https://images.unsplash.com/photo-1622350720516-ec7fdf41a100", GovernorateID = sSinaiGov.GovernorateID, PlaceTypeID = religiousType_OrDefault(context, "church") },
                new Place { Name = "Temple of the Oracle", Description = "Famous oracle temple visited by Alexander the Great.", Address = "Siwa Oasis", Latitude = 29.2032m, Longitude = 25.5484m, Rating = 4.7m, PriceLevel = 1, MainImageURL = "https://images.unsplash.com/photo-1549488344-c7151e289f64", GovernorateID = matrouhGov.GovernorateID, PlaceTypeID = templeType.TypeID },
                new Place { Name = "Al Muharraq Monastery", Description = "Historic Christian monastery where the Holy Family stayed.", Address = "Asyut", Latitude = 27.3218m, Longitude = 30.8202m, Rating = 4.9m, PriceLevel = 0, MainImageURL = "https://images.unsplash.com/photo-1620216654275-23c34a217c06", GovernorateID = asyutGov.GovernorateID, PlaceTypeID = religiousType_OrDefault(context, "church") },

                // Nature, Beach & Adventure
                new Place { Name = "Ras Mohammed National Park", Description = "Prominent national park renowned for diving.", Address = "Sharm El Sheikh", Latitude = 27.7329m, Longitude = 34.2494m, Rating = 4.8m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1622350720516-ec7fdf41a100", GovernorateID = sSinaiGov.GovernorateID, PlaceTypeID = parkType.TypeID },
                new Place { Name = "White Desert", Description = "A desert known for striking chalk rock formations.", Address = "Farafra", Latitude = 27.0543m, Longitude = 27.9718m, Rating = 4.9m, PriceLevel = 3, MainImageURL = "https://images.unsplash.com/photo-1549488344-c7151e289f64", GovernorateID = newValleyGov.GovernorateID, PlaceTypeID = safariType.TypeID },
                new Place { Name = "Wadi el-Hitan", Description = "UNESCO World Heritage site for whale fossils.", Address = "Fayoum", Latitude = 29.2713m, Longitude = 30.0441m, Rating = 4.7m, PriceLevel = 3, MainImageURL = "https://images.unsplash.com/photo-1533142262417-ad51619058bb", GovernorateID = fayoumGov.GovernorateID, PlaceTypeID = parkType.TypeID },
                new Place { Name = "Montazah Palace Gardens", Description = "Royal palace and lush gardens overlooking the Mediterranean.", Address = "Alexandria", Latitude = 31.2882m, Longitude = 30.0163m, Rating = 4.6m, PriceLevel = 1, MainImageURL = "https://images.unsplash.com/photo-1621251817478-f685c7bb74e6", GovernorateID = alexGov.GovernorateID, PlaceTypeID = parkType.TypeID },
                new Place { Name = "Blue Hole", Description = "World famous and deadly submarine sinkhole for advanced divers.", Address = "Dahab", Latitude = 28.5721m, Longitude = 34.5369m, Rating = 4.7m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1500249821865-c7e4ff69b2d6", GovernorateID = sSinaiGov.GovernorateID, PlaceTypeID = divingType.TypeID },
                new Place { Name = "Giftun Islands", Description = "Stunning white sand beaches and coral reefs.", Address = "Hurghada", Latitude = 27.2343m, Longitude = 33.9458m, Rating = 4.8m, PriceLevel = 3, MainImageURL = "https://images.unsplash.com/photo-1500249821865-c7e4ff69b2d6", GovernorateID = redSeaGov.GovernorateID, PlaceTypeID = beachType.TypeID },
                new Place { Name = "Siwa Oasis", Description = "An isolated urban oasis surrounded by salt lakes and palms.", Address = "Siwa, Matrouh", Latitude = 29.2032m, Longitude = 25.5195m, Rating = 4.9m, PriceLevel = 2, MainImageURL = "https://images.unsplash.com/photo-1549488344-c7151e289f64", GovernorateID = matrouhGov.GovernorateID, PlaceTypeID = safariType.TypeID },
                new Place { Name = "Wadi Rayan Waterfalls", Description = "Egypt's only waterfalls, connecting two artificial lakes.", Address = "Fayoum", Latitude = 29.1350m, Longitude = 30.3479m, Rating = 4.5m, PriceLevel = 1, MainImageURL = "https://images.unsplash.com/photo-1533142262417-ad51619058bb", GovernorateID = fayoumGov.GovernorateID, PlaceTypeID = parkType.TypeID },
                
                // Food & Dining / Markets
                new Place { Name = "Sequoia", Description = "Exceptional dining with panoramic views of the Nile River.", Address = "Zamalek, Cairo", Latitude = 30.0631m, Longitude = 31.2210m, Rating = 4.6m, PriceLevel = 4, MainImageURL = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4", GovernorateID = cairoGov.GovernorateID, PlaceTypeID = restaurantType.TypeID },
                new Place { Name = "Fish Market Alexandria", Description = "Premium Mediterranean seafood restaurant with ocean views.", Address = "Alexandria", Latitude = 31.2018m, Longitude = 29.9158m, Rating = 4.8m, PriceLevel = 3, MainImageURL = "https://images.unsplash.com/photo-1559339352-11d035aa65de", GovernorateID = alexGov.GovernorateID, PlaceTypeID = restaurantType.TypeID },
                new Place { Name = "Aswan Spice Market", Description = "Vibrant souq filled with colorful spices and Nubian artifacts.", Address = "Aswan", Latitude = 24.0889m, Longitude = 32.8998m, Rating = 4.7m, PriceLevel = 1, MainImageURL = "https://images.unsplash.com/photo-1554559388-7e3e91124aba", GovernorateID = aswanGov.GovernorateID, PlaceTypeID = marketType.TypeID },
                new Place { Name = "El Fishawy Cafe", Description = "One of Cairo's oldest cafes, famous for mint tea and shisha.", Address = "Khan el-Khalili, Cairo", Latitude = 30.0478m, Longitude = 31.2622m, Rating = 4.6m, PriceLevel = 1, MainImageURL = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4", GovernorateID = cairoGov.GovernorateID, PlaceTypeID = restaurantType.TypeID }
            };

            context.Places.AddRange(places);
            await context.SaveChangesAsync();

            var pyramids = await context.Places.FirstOrDefaultAsync(p => p.Name == "Giza Pyramids");
            if (pyramids == null) return;
            var reviews = new List<Review>
            {
                new Review { AuthorName = "Ahmed M.", Rating = 5, Text = "Breathtaking experience. A must visit!", PlaceID = pyramids.PlaceID },
                new Review { AuthorName = "Sarah S.", Rating = 4, Text = "Very crowded, but definitely worth it.", PlaceID = pyramids.PlaceID }
            };
            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSocialPostsAsync(ApplicationDbContext context)
        {
            if (await context.Posts.AnyAsync()) return;

            var user1 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john_doe");
            var user2 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "sarah_smith");
            if (user1 == null || user2 == null) return;

            var post1 = new Post
            {
                Content = "Just visited the Pyramids and they were absolutely magnificent! ☀️🐫 #Egypt #Travel",
                UserID = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Media = new List<PostMedia> {
                    new PostMedia { MediaURL = "https://images.unsplash.com/photo-1503177119275-0aa32b3a9368", MediaType = "Image" }
                }
            };

            var post2 = new Post
            {
                Content = "I've planned out my entire itinerary for Luxor next week using Kemora's AI Planner! Who has recommendations?",
                UserID = user2.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-12)
            };

            context.Posts.AddRange(new[] { post1, post2 });
            await context.SaveChangesAsync();
            
            var comment1 = new Comment
            {
                PostID = post1.PostID,
                UserID = user2.Id,
                Content = "Amazing photo! Did you take a camel ride?",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            context.Comments.Add(comment1);
            await context.SaveChangesAsync();
        }
        private static int religiousType_OrDefault(ApplicationDbContext context, string type)
        {
            return context.PlaceTypes.Where(pt => pt.GoogleType == type).Select(pt => pt.TypeID).FirstOrDefault();
        }
    }
}
