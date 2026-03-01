import 'package:json_annotation/json_annotation.dart';
import 'review.dart';

part 'place.g.dart';

@JsonSerializable()
class Place {
  final String id;
  final String name;
  final String description;
  final String imageUrl;
  final String category;
  final double rating;
  final String openingTime;
  final double latitude;
  final double longitude;
  final String address;
  final double priceAdult;
  final double priceChild;
  final List<String> transportInfo;
  final List<String> galleryImages;
  final List<Review> reviews;

  Place({
    required this.id,
    required this.name,
    required this.description,
    required this.imageUrl,
    required this.category,
    required this.rating,
    required this.openingTime,
    required this.latitude,
    required this.longitude,
    this.address = '',
    this.priceAdult = 0.0,
    this.priceChild = 0.0,
    this.transportInfo = const [],
    this.galleryImages = const [],
    this.reviews = const [],
  });

  factory Place.fromJson(Map<String, dynamic> json) => _$PlaceFromJson(json);
  Map<String, dynamic> toJson() => _$PlaceToJson(this);
}
