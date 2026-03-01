// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'city.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

City _$CityFromJson(Map<String, dynamic> json) => City(
  id: json['id'] as String,
  name: json['name'] as String,
  description: json['description'] as String,
  imageUrl: json['imageUrl'] as String,
  alignmentX: (json['alignmentX'] as num?)?.toDouble() ?? 0.0,
  alignmentY: (json['alignmentY'] as num?)?.toDouble() ?? 0.0,
  places: (json['places'] as List<dynamic>)
      .map((e) => Place.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$CityToJson(City instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'description': instance.description,
  'imageUrl': instance.imageUrl,
  'alignmentX': instance.alignmentX,
  'alignmentY': instance.alignmentY,
  'places': instance.places.map((e) => e.toJson()).toList(),
};
