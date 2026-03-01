import 'package:equatable/equatable.dart';

class Comment extends Equatable {
  final String id;
  final String postId;
  final String authorId;
  final String authorName;
  final String authorProfilePicture;
  final String content;
  final DateTime createdAt;

  const Comment({
    required this.id,
    required this.postId,
    required this.authorId,
    required this.authorName,
    required this.authorProfilePicture,
    required this.content,
    required this.createdAt,
  });

  @override
  List<Object?> get props => [id, postId, authorId, authorName, authorProfilePicture, content, createdAt];
}

class Post extends Equatable {
  final String id;
  final String authorId;
  final String authorName;
  final String authorProfilePicture;
  final String content;
  final String? imageUrl;
  final String? locationId;
  final String? locationName;
  final DateTime createdAt;
  final int likesCount;
  final int commentsCount;
  final bool isLikedByMe;

  const Post({
    required this.id,
    required this.authorId,
    required this.authorName,
    required this.authorProfilePicture,
    required this.content,
    this.imageUrl,
    this.locationId,
    this.locationName,
    required this.createdAt,
    this.likesCount = 0,
    this.commentsCount = 0,
    this.isLikedByMe = false,
  });

  @override
  List<Object?> get props => [
        id,
        authorId,
        authorName,
        authorProfilePicture,
        content,
        imageUrl,
        locationId,
        locationName,
        createdAt,
        likesCount,
        commentsCount,
        isLikedByMe,
      ];
}
