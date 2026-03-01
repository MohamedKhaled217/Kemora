import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../viewmodels/post_view_model.dart';

class FeedScreen extends StatefulWidget {
  const FeedScreen({super.key});

  @override
  State<FeedScreen> createState() => _FeedScreenState();
}

class _FeedScreenState extends State<FeedScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<PostViewModel>().loadFeed();
    });
  }

  void _showCreatePostBottomSheet() {
    final contentController = TextEditingController();
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(20))),
      builder: (context) {
        return Padding(
          padding: EdgeInsets.only(
            bottom: MediaQuery.of(context).viewInsets.bottom,
            left: 16, right: 16, top: 24,
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const Text('Create Post', style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
              const SizedBox(height: 16),
              TextField(
                controller: contentController,
                maxLines: 4,
                decoration: InputDecoration(
                  hintText: "What's on your mind?",
                  filled: true,
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(12), borderSide: BorderSide.none),
                ),
              ),
              const SizedBox(height: 16),
              ElevatedButton(
                style: ElevatedButton.styleFrom(padding: const EdgeInsets.symmetric(vertical: 16)),
                onPressed: () {
                  if (contentController.text.isNotEmpty) {
                    context.read<PostViewModel>().createPost(contentController.text);
                    Navigator.pop(context);
                  }
                },
                child: const Text('Post'),
              ),
              const SizedBox(height: 24),
            ],
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<PostViewModel>();

    return Scaffold(
      appBar: AppBar(
        title: const Text('Kemora Feed', style: TextStyle(fontWeight: FontWeight.bold)),
        actions: [
          IconButton(
            icon: const Icon(Icons.add_box_outlined),
            onPressed: _showCreatePostBottomSheet,
          ),
        ],
      ),
      body: _buildContent(viewModel),
    );
  }

  Widget _buildContent(PostViewModel viewModel) {
    if (viewModel.state == PostState.loading && viewModel.posts.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (viewModel.state == PostState.error && viewModel.posts.isEmpty) {
      return Center(
        child: Text(viewModel.errorMessage ?? 'An error occurred', style: const TextStyle(color: Colors.red)),
      );
    }

    if (viewModel.posts.isEmpty) {
      return const Center(
        child: Text('No posts yet, be the first!', style: TextStyle(fontSize: 16, color: Colors.grey)),
      );
    }

    return ListView.builder(
      itemCount: viewModel.posts.length,
      itemBuilder: (context, index) {
        final post = viewModel.posts[index];
        return Card(
          margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
          elevation: 2,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    CircleAvatar(
                      backgroundImage: NetworkImage(post.authorProfilePicture),
                      onBackgroundImageError: (_, __) => const Icon(Icons.person),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(post.authorName, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                          if (post.locationName != null)
                            Text(post.locationName!, style: TextStyle(color: Theme.of(context).primaryColor, fontSize: 12)),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                Text(post.content, style: const TextStyle(fontSize: 15)),
                if (post.imageUrl != null) ...[
                  const SizedBox(height: 12),
                  ClipRRect(
                    borderRadius: BorderRadius.circular(12),
                    child: Image.network(post.imageUrl!, fit: BoxFit.cover),
                  ),
                ],
                const SizedBox(height: 12),
                const Divider(),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  children: [
                    TextButton.icon(
                      onPressed: () {}, // Like logic
                      icon: Icon(post.isLikedByMe ? Icons.favorite : Icons.favorite_border,
                          color: post.isLikedByMe ? Colors.red : Colors.grey),
                      label: Text('${post.likesCount}'),
                    ),
                    TextButton.icon(
                      onPressed: () {}, // Comment logic
                      icon: const Icon(Icons.comment_outlined, color: Colors.grey),
                      label: Text('${post.commentsCount}'),
                    ),
                  ],
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
