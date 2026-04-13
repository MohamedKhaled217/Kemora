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
      id: json['placeID']?.toString() ?? '',
      name: json['name'] as String? ?? 'Unknown Place',
      description: json['description'] as String? ?? json['address'] as String? ?? 'No description available.',
      category: json['placeTypeName'] as String? ?? 'Uncategorized',
      imageUrl: json['mainImageURL'] as String? ?? 'https://picsum.photos/400/300',
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

class GovernorateModel extends Governorate {
  const GovernorateModel({
    required super.id,
    required super.name,
    super.imageUrl,
    super.region,
  });

  factory GovernorateModel.fromJson(Map<String, dynamic> json) {
    return GovernorateModel(
      id: json['governorateID']?.toString() ?? '',
      name: json['name'] as String? ?? 'Unknown Governorate',
      imageUrl: json['imageURL'] as String?,
      region: json['region'] as String?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'imageUrl': imageUrl,
      'region': region,
    };
  }
}
