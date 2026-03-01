import 'package:json_annotation/json_annotation.dart';
import 'place.dart';

part 'city.g.dart';

@JsonSerializable(explicitToJson: true)
class City {
  final String id;
  final String name;
  final String description;
  final String imageUrl;
  final double alignmentX;
  final double alignmentY;
  final List<Place> places;

  City({
    required this.id,
    required this.name,
    required this.description,
    required this.imageUrl,
    this.alignmentX = 0.0,
    this.alignmentY = 0.0,
    required this.places,
  });

  factory City.fromJson(Map<String, dynamic> json) => _$CityFromJson(json);
  Map<String, dynamic> toJson() => _$CityToJson(this);
}
