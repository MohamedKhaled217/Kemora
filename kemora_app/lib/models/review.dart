import 'package:json_annotation/json_annotation.dart';

part 'review.g.dart';

@JsonSerializable()
class Review {
  final String id;
  final String userName;
  final String userImage; // Placeholder or URL
  final double rating;
  final String date;
  final String comment;

  Review({
    required this.id,
    required this.userName,
    required this.userImage,
    required this.rating,
    required this.date,
    required this.comment,
  });

  factory Review.fromJson(Map<String, dynamic> json) => _$ReviewFromJson(json);
  Map<String, dynamic> toJson() => _$ReviewToJson(this);
}
