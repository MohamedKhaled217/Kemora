// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'country.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Country _$CountryFromJson(Map<String, dynamic> json) => Country(
  id: json['id'] as String,
  name: json['name'] as String,
  description: json['description'] as String,
  imageUrl: json['imageUrl'] as String,
  cities: (json['cities'] as List<dynamic>)
      .map((e) => City.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$CountryToJson(Country instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'description': instance.description,
  'imageUrl': instance.imageUrl,
  'cities': instance.cities.map((e) => e.toJson()).toList(),
};
