import '../../domain/entities/place.dart';

class PlaceModel extends Place {
  const PlaceModel({
    required super.id,
    required super.name,
    required super.description,
    required super.category,
    required super.imageUrl,
    required super.latitude,
    required super.longitude,
    required super.rating,
  });

  factory PlaceModel.fromJson(Map<String, dynamic> json) {
    return PlaceModel(
      id: json['id']?.toString() ?? '',
      name: json['name'] as String? ?? 'Unknown Place',
      description: json['description'] as String? ?? '',
      category: json['category'] as String? ?? 'Uncategorized',
      imageUrl: json['imageUrl'] as String? ?? 'https://via.placeholder.com/150',
      latitude: (json['latitude'] as num?)?.toDouble() ?? 0.0,
      longitude: (json['longitude'] as num?)?.toDouble() ?? 0.0,
      rating: (json['rating'] as num?)?.toDouble() ?? 0.0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'category': category,
      'imageUrl': imageUrl,
      'latitude': latitude,
      'longitude': longitude,
      'rating': rating,
    };
  }
}
