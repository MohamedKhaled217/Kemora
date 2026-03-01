import 'package:equatable/equatable.dart';

class User extends Equatable {
  final String id;
  final String email;
  final String username;
  final String profilePictureUrl;
  final String bio;
  final int earnedBadgesCount;

  const User({
    required this.id,
    required this.email,
    required this.username,
    required this.profilePictureUrl,
    required this.bio,
    this.earnedBadgesCount = 0,
  });

  @override
  List<Object?> get props => [
        id,
        email,
        username,
        profilePictureUrl,
        bio,
        earnedBadgesCount,
      ];
}
