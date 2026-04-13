import 'package:flutter/material.dart';
import '../social/chat_detail_screen.dart';
import '../../../domain/entities/user.dart';
import '../../../domain/entities/chat.dart';

class PublicProfileScreen extends StatelessWidget {
  final String userId;
  final String userName;
  final String? profilePictureUrl;
  final String? bio;

  const PublicProfileScreen({
    super.key,
    required this.userId,
    required this.userName,
    this.profilePictureUrl,
    this.bio,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(userName),
        elevation: 0,
        backgroundColor: Colors.white,
        foregroundColor: Colors.black,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          children: [
            CircleAvatar(
              radius: 60,
              backgroundImage: profilePictureUrl != null && profilePictureUrl!.isNotEmpty
                  ? NetworkImage(profilePictureUrl!)
                  : null,
              child: profilePictureUrl == null || profilePictureUrl!.isEmpty
                  ? const Icon(Icons.person, size: 60)
                  : null,
            ),
            const SizedBox(height: 24),
            Text(
              userName,
              style: const TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
            if (bio != null && bio!.isNotEmpty) ...[
               const SizedBox(height: 12),
               Text(
                 bio!,
                 style: const TextStyle(fontSize: 16, color: Colors.grey),
                 textAlign: TextAlign.center,
               ),
            ],
            const SizedBox(height: 32),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => ChatDetailScreen(
                        conversation: Conversation(
                          contactId: userId,
                          contactName: userName,
                          contactProfilePicture: profilePictureUrl,
                          lastMessage: '',
                          lastMessageAt: DateTime.now(),
                          unreadCount: 0,
                        ),
                      ),
                    ),
                  );
                },
                icon: const Icon(Icons.message),
                label: const Text('Send Message'),
                style: ElevatedButton.styleFrom(
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  backgroundColor: const Color(0xFFC5A358),
                  foregroundColor: Colors.white,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
              ),
            ),
            const SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              child: OutlinedButton.icon(
                onPressed: () {
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(content: Text('Friend request sent!')),
                  );
                },
                icon: const Icon(Icons.person_add),
                label: const Text('Add Friend'),
                style: OutlinedButton.styleFrom(
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  foregroundColor: const Color(0xFF1A1A1A),
                  side: const BorderSide(color: Color(0xFF1A1A1A)),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
