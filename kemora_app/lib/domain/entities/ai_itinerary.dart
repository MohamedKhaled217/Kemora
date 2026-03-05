import 'package:equatable/equatable.dart';

class ItineraryItem extends Equatable {
  final String name;
  final String description;
  final String timeOfDay;
  final String? imageUrl;
  final double? rating;
  final String? itineraryReview;
  final double? latitude;
  final double? longitude;
  final String? category;

  const ItineraryItem({
    required this.name,
    required this.description,
    required this.timeOfDay,
    this.imageUrl,
    this.rating,
    this.itineraryReview,
    this.latitude,
    this.longitude,
    this.category,
  });

  @override
  List<Object?> get props => [
        name,
        description,
        timeOfDay,
        imageUrl,
        rating,
        itineraryReview,
        latitude,
        longitude,
        category,
      ];
}

class TripDay extends Equatable {
  final int dayNumber;
  final List<ItineraryItem> activities;

  const TripDay({
    required this.dayNumber,
    required this.activities,
  });

  @override
  List<Object?> get props => [dayNumber, activities];
}

class AIItinerary extends Equatable {
  final String title;
  final String duration;
  final List<TripDay> days;

  const AIItinerary({
    required this.title,
    required this.duration,
    required this.days,
  });

  @override
  List<Object?> get props => [title, duration, days];
}
