import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/app_provider.dart';
import '../../models/post.dart';
import 'widgets/post_card.dart';

class CommunityScreen extends StatefulWidget {
  const CommunityScreen({super.key});

  @override
  State<CommunityScreen> createState() => _CommunityScreenState();
}

class _CommunityScreenState extends State<CommunityScreen> {
  @override
  void initState() {
    super.initState();
    // Load posts once
    WidgetsBinding.instance.addPostFrameCallback((_) {
      Provider.of<AppProvider>(context, listen: false).loadPosts();
    });
  }

  void _showAddPostDialog(BuildContext context) {
    final TextEditingController contentController = TextEditingController();
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('New Post'),
        content: TextField(
          controller: contentController,
          decoration: const InputDecoration(
            hintText: 'What\'s on your mind?',
            border: OutlineInputBorder(),
          ),
          maxLines: 3,
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              if (contentController.text.isNotEmpty) {
                final newPost = Post(
                  id: DateTime.now().toString(),
                  userName: 'You',
                  userImage:
                      'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde', // Same as profile mock
                  content: contentController.text,
                  comments: [],
                  likes: 0,
                  timeAgo: 'Just now',
                );
                Provider.of<AppProvider>(
                  context,
                  listen: false,
                ).addPost(newPost);
                Navigator.of(ctx).pop();
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text('Post published!')),
                );
              }
            },
            child: const Text('Post'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Community'),
        centerTitle: false,
        actions: [
          IconButton(
            icon: const Icon(
              Icons.add_a_photo,
            ), // Keeping original icon or similar
            onPressed: () => _showAddPostDialog(context),
          ),
        ],
      ),
      body: Consumer<AppProvider>(
        builder: (context, appProvider, child) {
          final posts = appProvider.posts;
          return ListView.builder(
            itemCount: posts.length,
            itemBuilder: (context, index) {
              return PostCard(post: posts[index]);
            },
          );
        },
      ),
    );
  }
}
