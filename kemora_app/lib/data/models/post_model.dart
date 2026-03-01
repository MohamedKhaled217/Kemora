import '../../domain/entities/post.dart';

class CommentModel extends Comment {
  const CommentModel({
    required super.id,
    required super.postId,
    required super.authorId,
    required super.authorName,
    required super.authorProfilePicture,
    required super.content,
    required super.createdAt,
  });

  factory CommentModel.fromJson(Map<String, dynamic> json) {
    return CommentModel(
      id: json['id']?.toString() ?? '',
      postId: json['postId']?.toString() ?? '',
      authorId: json['authorId']?.toString() ?? '',
      authorName: json['authorName'] as String? ?? 'Unknown User',
      authorProfilePicture: json['authorProfilePicture'] as String? ?? 'https://via.placeholder.com/150',
      content: json['content'] as String? ?? '',
      createdAt: json['createdAt'] != null ? DateTime.parse(json['createdAt']) : DateTime.now(),
    );
  }
}

class PostModel extends Post {
  const PostModel({
    required super.id,
    required super.authorId,
    required super.authorName,
    required super.authorProfilePicture,
    required super.content,
    super.imageUrl,
    super.locationId,
    super.locationName,
    required super.createdAt,
    super.likesCount,
    super.commentsCount,
    super.isLikedByMe,
  });

  factory PostModel.fromJson(Map<String, dynamic> json) {
    return PostModel(
      id: json['id']?.toString() ?? '',
      authorId: json['authorId']?.toString() ?? '',
      authorName: json['authorName'] as String? ?? 'Unknown Author',
      authorProfilePicture: json['authorProfilePicture'] as String? ?? 'https://via.placeholder.com/150',
      content: json['content'] as String? ?? '',
      imageUrl: json['imageUrl'] as String?,
      locationId: json['locationId']?.toString(),
      locationName: json['locationName'] as String?,
      createdAt: json['createdAt'] != null ? DateTime.parse(json['createdAt']) : DateTime.now(),
      likesCount: json['likesCount'] as int? ?? 0,
      commentsCount: json['commentsCount'] as int? ?? 0,
      isLikedByMe: json['isLikedByMe'] as bool? ?? false,
    );
  }
}
