import 'package:flutter/material.dart';
import '../../../models/post.dart';

class PostCard extends StatelessWidget {
  final Post post;

  const PostCard({super.key, required this.post});

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header
            Row(
              children: [
                CircleAvatar(
                  backgroundImage: NetworkImage(post.userImage),
                  radius: 20,
                  onBackgroundImageError: (_, __) => const Icon(Icons.person),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        post.userName,
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                          fontSize: 16,
                        ),
                      ),
                      Text(
                        post.timeAgo,
                        style: TextStyle(color: Colors.grey[600], fontSize: 12),
                      ),
                    ],
                  ),
                ),
                const Icon(Icons.more_horiz),
              ],
            ),
            const SizedBox(height: 12),
            // Content
            Text(post.content),
            if (post.imageUrl != null) ...[
              const SizedBox(height: 12),
              ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: Image.network(
                  post.imageUrl!,
                  height: 200,
                  width: double.infinity,
                  fit: BoxFit.cover,
                  errorBuilder: (context, error, stackTrace) => const SizedBox(
                    height: 200,
                    child: Center(child: Icon(Icons.broken_image)),
                  ),
                ),
              ),
            ],
            const SizedBox(height: 12),
            // Action Buttons
            Row(
              children: [
                Icon(Icons.favorite_border, color: Colors.grey[600]),
                const SizedBox(width: 4),
                Text('${post.likes}'),
                const SizedBox(width: 16),
                Icon(Icons.chat_bubble_outline, color: Colors.grey[600]),
                const SizedBox(width: 4),
                Text('${post.comments.length}'),
                const Spacer(),
                Icon(Icons.share, color: Colors.grey[600]),
              ],
            ),
            // Comments Section (Preview)
            if (post.comments.isNotEmpty) ...[
              const Divider(height: 24),
              ...post.comments.map(
                (comment) => Padding(
                  padding: const EdgeInsets.only(bottom: 8.0),
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      CircleAvatar(
                        backgroundImage: NetworkImage(comment.userImage),
                        radius: 12,
                        onBackgroundImageError: (_, __) =>
                            const Icon(Icons.person, size: 12),
                      ),
                      const SizedBox(width: 8),
                      Expanded(
                        child: RichText(
                          text: TextSpan(
                            style: DefaultTextStyle.of(context).style,
                            children: [
                              TextSpan(
                                text: '${comment.userName} ',
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              TextSpan(text: comment.text),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
