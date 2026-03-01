import '../../domain/entities/user.dart';

class UserModel extends User {
  const UserModel({
    required super.id,
    required super.email,
    required super.username,
    required super.profilePictureUrl,
    required super.bio,
    super.earnedBadgesCount,
  });

  factory UserModel.fromJson(Map<String, dynamic> json) {
    return UserModel(
      id: json['id'] as String? ?? '',
      email: json['email'] as String? ?? '',
      username: json['username'] as String? ?? '',
      profilePictureUrl: json['profilePictureUrl'] as String? ?? '',
      bio: json['bio'] as String? ?? '',
      earnedBadgesCount: json['earnedBadgesCount'] as int? ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'email': email,
      'username': username,
      'profilePictureUrl': profilePictureUrl,
      'bio': bio,
      'earnedBadgesCount': earnedBadgesCount,
    };
  }
}
