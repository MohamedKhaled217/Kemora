import 'package:json_annotation/json_annotation.dart';
import 'city.dart';

part 'country.g.dart';

@JsonSerializable(explicitToJson: true)
class Country {
  final String id;
  final String name;
  final String description;
  final String imageUrl;
  final List<City> cities;

  Country({
    required this.id,
    required this.name,
    required this.description,
    required this.imageUrl,
    required this.cities,
  });

  factory Country.fromJson(Map<String, dynamic> json) => _$CountryFromJson(json);
  Map<String, dynamic> toJson() => _$CountryToJson(this);
}
