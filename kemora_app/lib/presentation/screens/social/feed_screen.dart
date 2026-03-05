import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:timeago/timeago.dart' as timeago;
import 'package:image_picker/image_picker.dart';
import 'dart:io';
import 'package:flutter/foundation.dart' show kIsWeb;
import '../../../domain/entities/post.dart';
import '../../viewmodels/post_view_model.dart';
import '../../viewmodels/trip_view_model.dart';
import 'chat_list_screen.dart';
import 'post_detail_screen.dart';

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
      context.read<TripViewModel>().loadTrips();
    });
  }

  void _showCreatePostBottomSheet() {
    final contentController = TextEditingController();
    String? selectedTripId;
    String? selectedTripTitle;
    XFile? selectedImage;
    final ImagePicker picker = ImagePicker();

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
      builder: (context) {
        return StatefulBuilder(
          builder: (context, setModalState) {
            final tripVM = context.watch<TripViewModel>();
            
            Future<void> pickImage() async {
              final XFile? image = await picker.pickImage(source: ImageSource.gallery);
              if (image != null) {
                setModalState(() {
                  selectedImage = image;
                });
              }
            }

            return Padding(
              padding: EdgeInsets.only(
                bottom: MediaQuery.of(context).viewInsets.bottom,
                left: 20, right: 20, top: 24,
              ),
              child: SingleChildScrollView(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    const Text('Create Post', style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold)),
                    const SizedBox(height: 16),
                    TextField(
                      controller: contentController,
                      maxLines: 4,
                      decoration: InputDecoration(
                        hintText: "Share your experience...",
                        filled: true,
                        fillColor: Colors.grey[50],
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(16), borderSide: BorderSide(color: Colors.grey[200]!)),
                      ),
                    ),
                    const SizedBox(height: 16),
                    if (selectedImage != null)
                      Stack(
                        children: [
                          ClipRRect(
                            borderRadius: BorderRadius.circular(12),
                            child: kIsWeb 
                                ? Image.network(selectedImage!.path, height: 150, width: double.infinity, fit: BoxFit.cover)
                                : Image.file(File(selectedImage!.path), height: 150, width: double.infinity, fit: BoxFit.cover),
                          ),
                          Positioned(
                            top: 8, right: 8,
                            child: CircleAvatar(
                              backgroundColor: Colors.black54,
                              child: IconButton(
                                icon: const Icon(Icons.close, color: Colors.white, size: 20),
                                onPressed: () => setModalState(() => selectedImage = null),
                              ),
                            ),
                          ),
                        ],
                      )
                    else
                      OutlinedButton.icon(
                        onPressed: pickImage,
                        icon: const Icon(Icons.image_outlined),
                        label: const Text('Add Photo'),
                        style: OutlinedButton.styleFrom(
                          padding: const EdgeInsets.symmetric(vertical: 12),
                          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                        ),
                      ),
                    const SizedBox(height: 20),
                    const Text('Recommend a Trip Plan', style: TextStyle(fontWeight: FontWeight.bold)),
                    const SizedBox(height: 8),
                    if (tripVM.trips.isEmpty)
                      const Text('No trips found to recommend.', style: TextStyle(color: Colors.grey, fontSize: 12))
                    else
                      SizedBox(
                        height: 50,
                        child: ListView.builder(
                          scrollDirection: Axis.horizontal,
                          itemCount: tripVM.trips.length,
                          itemBuilder: (context, index) {
                            final trip = tripVM.trips[index];
                            final isSelected = selectedTripId == trip.id;
                            return Padding(
                              padding: const EdgeInsets.only(right: 8),
                              child: FilterChip(
                                label: Text(trip.title),
                                selected: isSelected,
                                onSelected: (val) {
                                  setModalState(() {
                                    selectedTripId = val ? trip.id : null;
                                    selectedTripTitle = val ? trip.title : null;
                                  });
                                },
                                selectedColor: const Color(0xFFC5A358),
                                labelStyle: TextStyle(color: isSelected ? Colors.white : Colors.black87),
                                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                              ),
                            );
                          },
                        ),
                      ),
                    const SizedBox(height: 24),
                    ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFF1A1A1A),
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
                      ),
                      onPressed: () {
                        if (contentController.text.isNotEmpty) {
                          context.read<PostViewModel>().createPost(
                            contentController.text,
                            imageFile: selectedImage,
                            recommendedTripId: selectedTripId,
                            recommendedTripTitle: selectedTripTitle,
                          );
                          Navigator.pop(context);
                        }
                      },
                      child: const Text('Post', style: TextStyle(fontWeight: FontWeight.bold)),
                    ),
                    const SizedBox(height: 24),
                  ],
                ),
              ),
            );
          }
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<PostViewModel>();

    return Scaffold(
      backgroundColor: const Color(0xFFF8F9FA),
      appBar: AppBar(
        title: const Text('Kemora Social', style: TextStyle(color: Colors.black, fontWeight: FontWeight.bold)),
        backgroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(
            icon: const Icon(Icons.message_outlined, color: Colors.black),
            onPressed: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const ChatListScreen())),
          ),
          IconButton(
            icon: const Icon(Icons.add_box_outlined, color: Colors.black),
            onPressed: _showCreatePostBottomSheet,
          ),
        ],
      ),
      body: _buildContent(viewModel),
    );
  }

  Widget _buildContent(PostViewModel viewModel) {
    if (viewModel.state == PostState.loading && viewModel.posts.isEmpty) {
      return const Center(child: CircularProgressIndicator(color: Color(0xFFC5A358)));
    }

    if (viewModel.state == PostState.error && viewModel.posts.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.wifi_off, size: 64, color: Colors.grey),
            const SizedBox(height: 16),
            Text(viewModel.errorMessage ?? 'Could not load feed', style: const TextStyle(color: Colors.grey, fontSize: 16)),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: () => viewModel.loadFeed(),
              style: ElevatedButton.styleFrom(backgroundColor: const Color(0xFFC5A358), foregroundColor: Colors.white),
              child: const Text('Retry'),
            ),
          ],
        ),
      );
    }

    if (viewModel.posts.isEmpty) {
      return const Center(child: Text('Start the conversation!'));
    }

    return RefreshIndicator(
      color: const Color(0xFFC5A358),
      onRefresh: () => viewModel.loadFeed(),
      child: ListView.builder(
        itemCount: viewModel.posts.length,
        padding: const EdgeInsets.symmetric(vertical: 12),
        itemBuilder: (context, index) {
          final post = viewModel.posts[index];
          return _buildPostCard(post, viewModel);
        },
      ),
    );
  }

  Widget _buildPostCard(Post post, PostViewModel viewModel) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [BoxShadow(color: Colors.black.withOpacity(0.04), blurRadius: 10, offset: const Offset(0, 4))],
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(20),
          onTap: () => Navigator.push(
            context,
            MaterialPageRoute(builder: (_) => PostDetailScreen(post: post)),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
            ListTile(
              leading: CircleAvatar(
                backgroundImage: NetworkImage(post.authorProfilePicture),
                onBackgroundImageError: (_, __) {},
                child: post.authorProfilePicture.isEmpty
                    ? Text(post.authorName.isNotEmpty ? post.authorName[0].toUpperCase() : '?')
                    : null,
              ),
              title: Text(post.authorName, style: const TextStyle(fontWeight: FontWeight.bold)),
              subtitle: Text(timeago.format(post.createdAt)),
              trailing: const Icon(Icons.arrow_forward_ios, size: 14, color: Colors.grey),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Text(post.content, style: const TextStyle(fontSize: 15, height: 1.4)),
            ),
            // Media image display
            if (post.imageUrl != null && post.imageUrl!.isNotEmpty) ...[
              const SizedBox(height: 12),
              ClipRRect(
                borderRadius: const BorderRadius.only(
                  bottomLeft: Radius.circular(20),
                  bottomRight: Radius.circular(20),
                ),
                child: Image.network(
                  post.imageUrl!,
                  width: double.infinity,
                  height: 220,
                  fit: BoxFit.cover,
                  errorBuilder: (_, __, ___) => Container(
                    height: 100,
                    color: Colors.grey[100],
                    child: const Center(child: Icon(Icons.image_not_supported, color: Colors.grey)),
                  ),
                ),
              ),
            ],
            if (post.recommendedTripId != null)
              _buildRecommendedPlanCard(post.recommendedTripTitle ?? 'Trip Plan'),
            const SizedBox(height: 4),
            Row(
              children: [
                IconButton(
                  icon: Icon(
                    post.isLikedByMe ? Icons.favorite : Icons.favorite_border,
                    color: post.isLikedByMe ? Colors.red : Colors.grey,
                  ),
                  onPressed: () => viewModel.toggleLike(post.id),
                ),
                Text('${post.likesCount}', style: const TextStyle(color: Colors.grey)),
                const SizedBox(width: 8),
                // Comment button now navigates to PostDetailScreen
                IconButton(
                  icon: const Icon(Icons.comment_outlined, color: Colors.grey),
                  onPressed: () => Navigator.push(
                    context,
                    MaterialPageRoute(builder: (_) => PostDetailScreen(post: post)),
                  ),
                ),
                Text('${post.commentsCount}', style: const TextStyle(color: Colors.grey)),
              ],
            ),
          ],
        ),
      ),
      ),
    );
  }

  Widget _buildRecommendedPlanCard(String title) {
    return Container(
      margin: const EdgeInsets.fromLTRB(16, 12, 16, 0),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: const Color(0xFFC5A358).withOpacity(0.05),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: const Color(0xFFC5A358).withOpacity(0.25)),
      ),
      child: Row(
        children: [
          const Icon(Icons.map_outlined, color: Color(0xFFC5A358), size: 18),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text('Recommended Plan', style: TextStyle(fontSize: 10, fontWeight: FontWeight.bold, color: Color(0xFFC5A358))),
                Text(title, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14)),
              ],
            ),
          ),
          const Icon(Icons.chevron_right, color: Color(0xFFC5A358)),
        ],
      ),
    );
  }
}
