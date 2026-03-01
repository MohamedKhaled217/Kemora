import '../../domain/entities/trip.dart';

class TripModel extends Trip {
  const TripModel({
    required super.id,
    required super.title,
    required super.startDate,
    required super.endDate,
    super.plannedPlaces,
  });

  factory TripModel.fromJson(Map<String, dynamic> json) {
    // You'd typically parse places conditionally, here we keep it simple
    return TripModel(
      id: json['id']?.toString() ?? '',
      title: json['title'] as String? ?? 'Unnamed Trip',
      startDate: json['startDate'] != null ? DateTime.parse(json['startDate']) : DateTime.now(),
      endDate: json['endDate'] != null ? DateTime.parse(json['endDate']) : DateTime.now().add(const Duration(days: 1)),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'title': title,
      'startDate': startDate.toIso8601String(),
      'endDate': endDate.toIso8601String(),
      'placeIds': plannedPlaces.map((p) => p.id).toList(),
    };
  }
}
