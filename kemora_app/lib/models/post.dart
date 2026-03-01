class Post {
  final String id;
  final String userName;
  final String userImage;
  final String content;
  final String? imageUrl;
  final List<Comment> comments;
  final int likes;
  final String timeAgo;

  Post({
    required this.id,
    required this.userName,
    required this.userImage,
    required this.content,
    this.imageUrl,
    required this.comments,
    required this.likes,
    required this.timeAgo,
  });
}

class Comment {
  final String id;
  final String userName;
  final String userImage;
  final String text;

  Comment({
    required this.id,
    required this.userName,
    required this.userImage,
    required this.text,
  });
}
