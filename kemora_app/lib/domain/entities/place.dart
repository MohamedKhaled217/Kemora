import 'package:equatable/equatable.dart';

class Place extends Equatable {
  final String id;
  final String name;
  final String description;
  final String category;
  final String imageUrl;
  final double latitude;
  final double longitude;
  final double rating;

  const Place({
    required this.id,
    required this.name,
    required this.description,
    required this.category,
    required this.imageUrl,
    required this.latitude,
    required this.longitude,
    required this.rating,
  });

  @override
  List<Object?> get props => [
        id,
        name,
        description,
        category,
        imageUrl,
        latitude,
        longitude,
        rating,
      ];
}

class Governorate extends Equatable {
  final String id;
  final String name;
  final String? imageUrl;
  final String? region;

  const Governorate({
    required this.id,
    required this.name,
    this.imageUrl,
    this.region,
  });

  @override
  List<Object?> get props => [id, name, imageUrl, region];
}
