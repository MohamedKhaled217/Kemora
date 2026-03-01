// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'place.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Place _$PlaceFromJson(Map<String, dynamic> json) => Place(
  id: json['id'] as String,
  name: json['name'] as String,
  description: json['description'] as String,
  imageUrl: json['imageUrl'] as String,
  category: json['category'] as String,
  rating: (json['rating'] as num).toDouble(),
  openingTime: json['openingTime'] as String,
  latitude: (json['latitude'] as num).toDouble(),
  longitude: (json['longitude'] as num).toDouble(),
  address: json['address'] as String? ?? '',
  priceAdult: (json['priceAdult'] as num?)?.toDouble() ?? 0.0,
  priceChild: (json['priceChild'] as num?)?.toDouble() ?? 0.0,
  transportInfo:
      (json['transportInfo'] as List<dynamic>?)
          ?.map((e) => e as String)
          .toList() ??
      const [],
  galleryImages:
      (json['galleryImages'] as List<dynamic>?)
          ?.map((e) => e as String)
          .toList() ??
      const [],
  reviews:
      (json['reviews'] as List<dynamic>?)
          ?.map((e) => Review.fromJson(e as Map<String, dynamic>))
          .toList() ??
      const [],
);

Map<String, dynamic> _$PlaceToJson(Place instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'description': instance.description,
  'imageUrl': instance.imageUrl,
  'category': instance.category,
  'rating': instance.rating,
  'openingTime': instance.openingTime,
  'latitude': instance.latitude,
  'longitude': instance.longitude,
  'address': instance.address,
  'priceAdult': instance.priceAdult,
  'priceChild': instance.priceChild,
  'transportInfo': instance.transportInfo,
  'galleryImages': instance.galleryImages,
  'reviews': instance.reviews,
};
