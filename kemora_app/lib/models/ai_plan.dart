class TripPlan {
  final String destination;
  final String hotelName;
  final double hotelRating;
  final String hotelImageUrl;
  final double estimatedCost;
  final List<DayPlan> dailyItinerary;

  TripPlan({
    required this.destination,
    required this.hotelName,
    required this.hotelRating,
    required this.hotelImageUrl,
    required this.estimatedCost,
    required this.dailyItinerary,
  });
}

class DayPlan {
  final int dayNumber;
  final List<Activity> activities;

  DayPlan({required this.dayNumber, required this.activities});
}

enum ActivityType { hotel, restaurant, sightseeing, transport, shopping, other }

class Activity {
  final String time;
  final String title;
  final String description;
  final ActivityType type;

  Activity({
    required this.time,
    required this.title,
    required this.description,
    required this.type,
  });
}
