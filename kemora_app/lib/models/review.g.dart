// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Review _$ReviewFromJson(Map<String, dynamic> json) => Review(
  id: json['id'] as String,
  userName: json['userName'] as String,
  userImage: json['userImage'] as String,
  rating: (json['rating'] as num).toDouble(),
  date: json['date'] as String,
  comment: json['comment'] as String,
);

Map<String, dynamic> _$ReviewToJson(Review instance) => <String, dynamic>{
  'id': instance.id,
  'userName': instance.userName,
  'userImage': instance.userImage,
  'rating': instance.rating,
  'date': instance.date,
  'comment': instance.comment,
};
