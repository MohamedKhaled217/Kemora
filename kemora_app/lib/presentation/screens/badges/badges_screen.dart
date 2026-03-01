import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../viewmodels/badge_view_model.dart';

class BadgesScreen extends StatefulWidget {
  final String userId;

  const BadgesScreen({super.key, required this.userId});

  @override
  State<BadgesScreen> createState() => _BadgesScreenState();
}

class _BadgesScreenState extends State<BadgesScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<BadgeViewModel>().loadUserBadges(widget.userId);
    });
  }

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<BadgeViewModel>();

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Cultural Badges', style: TextStyle(fontWeight: FontWeight.bold)),
      ),
      body: _buildContent(viewModel),
    );
  }

  Widget _buildContent(BadgeViewModel viewModel) {
    if (viewModel.state == BadgeState.loading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (viewModel.state == BadgeState.error) {
      return Center(
        child: Text(viewModel.errorMessage ?? 'An error occurred', style: const TextStyle(color: Colors.red)),
      );
    }

    if (viewModel.userBadges.isEmpty) {
      return const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.military_tech_outlined, size: 80, color: Colors.grey),
            SizedBox(height: 16),
            Text('No badges earned yet.', style: TextStyle(fontSize: 18, color: Colors.grey)),
            SizedBox(height: 8),
            Text('Visit places and share photos to earn badges!'),
          ],
        ),
      );
    }

    return GridView.builder(
      padding: const EdgeInsets.all(16),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        childAspectRatio: 0.8,
        crossAxisSpacing: 16,
        mainAxisSpacing: 16,
      ),
      itemCount: viewModel.userBadges.length,
      itemBuilder: (context, index) {
        final userBadge = viewModel.userBadges[index];
        return Card(
          elevation: 4,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                CircleAvatar(
                  radius: 40,
                  backgroundColor: Theme.of(context).primaryColor.withOpacity(0.1),
                  backgroundImage: NetworkImage(userBadge.badge.iconUrl),
                  onBackgroundImageError: (_, __) => const Icon(Icons.star, size: 40, color: Colors.amber),
                ),
                const SizedBox(height: 12),
                Text(
                  userBadge.badge.name,
                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                  textAlign: TextAlign.center,
                ),
                const SizedBox(height: 8),
                Text(
                  'Earned: ${userBadge.earnedAt.toLocal().toString().split(' ')[0]}',
                  style: const TextStyle(fontSize: 12, color: Colors.grey),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
