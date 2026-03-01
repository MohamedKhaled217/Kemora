import '../models/ai_plan.dart';

class AiTripService {
  static Future<TripPlan> generateOneDayPlan({
    required String city,
    required String budget, // 'Budget', 'Standard', 'Luxury'
    required List<String> interests,
  }) async {
    // Simulate API delay
    await Future.delayed(const Duration(seconds: 2));

    // Simple mock logic logic based on inputs
    String hotel;
    double price;
    double rating;
    String hotelImg;

    if (budget == 'Luxury') {
      hotel = "The Nile Ritz-Carlton, $city";
      price = 500.0;
      rating = 4.9;
      hotelImg =
          "https://cf.bstatic.com/xdata/images/hotel/max1024x768/73336496.jpg?k=2f939055814510006323490729307307223072307308230723072307"; // Placeholder
    } else if (budget == 'Standard') {
      hotel = "Steigenberger Hotel, $city";
      price = 200.0;
      rating = 4.5;
      hotelImg =
          "https://cf.bstatic.com/xdata/images/hotel/max1024x768/49117866.jpg?k=293849102938402938409238409283409283409283";
    } else {
      hotel = "$city City Hostel";
      price = 50.0;
      rating = 4.0;
      hotelImg =
          "https://cf.bstatic.com/xdata/images/hotel/max1024x768/112341234.jpg?k=123123123123123";
    }

    // Generate activities
    final List<DayPlan> itinerary = [];
    // We'll just generate the number of days requested, but let's default to a 3-day max for now in this function signature or just produce 1 day logic expanded.
    // Let's assume the UI calls this with days. For now, let's just make a 3-day itinerary for demo.

    for (int i = 1; i <= 3; i++) {
      itinerary.add(
        DayPlan(
          dayNumber: i,
          activities: [
            Activity(
              time: "09:00 AM",
              title: "Breakfast at Local Cafe",
              description: "Start your day with traditional $city breakfast.",
              type: ActivityType.restaurant,
            ),
            Activity(
              time: "10:30 AM",
              title: "Visit $city Museum",
              description: "Explore the history of $city.",
              type: ActivityType.sightseeing,
            ),
            Activity(
              time: "01:00 PM",
              title: "Lunch at The View",
              description: "Enjoy meal with a view.",
              type: ActivityType.restaurant,
            ),
            Activity(
              time: "03:00 PM",
              title: "Walk through Old Town",
              description: "Discover hidden gems.",
              type: ActivityType.sightseeing,
            ),
            Activity(
              time: "08:00 PM",
              title: "Dinner & Nightlife",
              description: "Relax after a long day.",
              type: ActivityType.restaurant,
            ),
          ],
        ),
      );
    }

    return TripPlan(
      destination: city,
      hotelName: hotel,
      hotelRating: rating,
      hotelImageUrl: hotelImg,
      estimatedCost: price * 3, // simplified total
      dailyItinerary: itinerary,
    );
  }

  // More flexible generator
  static Future<TripPlan> generateTrip({
    required String destination,
    required int days,
    required String budget,
    required List<String> interests,
  }) async {
    await Future.delayed(const Duration(seconds: 3)); // Thinking...

    // Determine Vibe
    bool isHistory = interests.contains("History");
    bool isFood = interests.contains("Food");

    List<DayPlan> daysPlans = [];
    for (int d = 1; d <= days; d++) {
      List<Activity> acts = [];

      // Morning
      acts.add(
        Activity(
          time: "08:30 AM",
          title: "Breakfast at ${destination} Delights",
          description: "Famous for their falafel and coffee.",
          type: ActivityType.restaurant,
        ),
      );

      // Late Morning
      if (isHistory) {
        acts.add(
          Activity(
            time: "10:00 AM",
            title: "Historical Tour of ${destination}",
            description: "Guided tour of ancient monuments.",
            type: ActivityType.sightseeing,
          ),
        );
      } else {
        acts.add(
          Activity(
            time: "10:00 AM",
            title: "${destination} City Park",
            description: "Refresh morning walk.",
            type: ActivityType.sightseeing,
          ),
        );
      }

      // Lunch
      acts.add(
        Activity(
          time: "01:00 PM",
          title: isFood ? "Gourmet Lunch Experience" : "Quick Local Lunch",
          description: "Time to refuel.",
          type: ActivityType.restaurant,
        ),
      );

      // Afternoon
      acts.add(
        Activity(
          time: "03:00 PM",
          title: "Afternoon Adventure",
          description: "Visit the main bazaar and shop for souvenirs.",
          type: ActivityType.shopping,
        ),
      );

      // Evening
      acts.add(
        Activity(
          time: "08:00 PM",
          title: "Dinner at The Rooftop",
          description: "Fine dining with a view of the city lights.",
          type: ActivityType.restaurant,
        ),
      );

      daysPlans.add(DayPlan(dayNumber: d, activities: acts));
    }

    String hotelName = budget == "Luxury"
        ? "Grand ${destination} Hotel"
        : "Cozy ${destination} Inn";

    return TripPlan(
      destination: destination,
      estimatedCost: (budget == "Luxury" ? 500 : 150) * days.toDouble(),
      hotelName: hotelName,
      hotelRating: budget == "Luxury" ? 4.9 : 4.2,
      hotelImageUrl:
          "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&q=80&w=2070",
      dailyItinerary: daysPlans,
    );
  }
}
