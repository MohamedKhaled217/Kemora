import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../l10n/app_localizations.dart';
import '../../providers/app_provider.dart';
import '../../services/mock_data_service.dart';
import 'widgets/achievement_card.dart';
import '../../presentation/viewmodels/auth_view_model.dart';
import '../../presentation/screens/profile/settings_screen.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final allAchievements = MockDataService.getAchievements();
    // Show only first 2 achievements
    final displayedAchievements = allAchievements.take(2).toList();
    final l10n = AppLocalizations.of(context)!;
    final user = context.watch<AuthViewModel>().user;

    return Scaffold(
      body: SingleChildScrollView(
        child: Column(
          children: [
            const SizedBox(height: 40),
            // User Header
            CircleAvatar(
              radius: 50,
              backgroundImage: user?.profilePictureUrl != null 
                ? NetworkImage(user!.profilePictureUrl!) 
                : const NetworkImage('https://images.unsplash.com/photo-1535713875002-d1d0cf377fde'),
              child: InkWell(
                onTap: () {
                  // TODO: Image Picker
                },
              ),
            ),
            const SizedBox(height: 16),
            Text(
              user?.fullName ?? 'Alex Johnson',
              style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
            Text(
              user?.bio ?? 'Passionate Traveler',
              style: const TextStyle(color: Colors.grey, fontSize: 16),
            ),
            const SizedBox(height: 32),

            // Achievements Section
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20.0),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    l10n.achievements,
                    style: const TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  TextButton(
                    onPressed: () {
                      _showAllAchievements(context, allAchievements);
                    },
                    child: Text(l10n.seeAll),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            ListView.builder(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              padding: const EdgeInsets.symmetric(horizontal: 16),
              itemCount: displayedAchievements.length,
              itemBuilder: (context, index) {
                return AchievementCard(
                  achievement: displayedAchievements[index],
                );
              },
            ),
            const SizedBox(height: 24),

            // Settings Section
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20.0),
              child: Align(
                alignment: AlignmentDirectional.centerStart,
                child: Text(
                  l10n.settings,
                  style: const TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
            const SizedBox(height: 12),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16.0),
              child: Card(
                elevation: 2,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                child: ListTile(
                  leading: const Icon(Icons.language, color: Colors.orange),
                  title: Text(l10n.language),
                  trailing: Consumer<AppProvider>(
                    builder: (context, provider, child) {
                      return DropdownButton<Locale>(
                        value: provider.currentLocale,
                        underline: Container(),
                        onChanged: (Locale? newLocale) {
                          if (newLocale != null) {
                            provider.changeLanguage(newLocale);
                          }
                        },
                        items: const [
                          DropdownMenuItem(
                            value: Locale('en'),
                            child: Text('English'),
                          ),
                          DropdownMenuItem(
                            value: Locale('ar'),
                            child: Text('العربية'),
                          ),
                        ],
                      );
                    },
                  ),
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16.0),
              child: Card(
                elevation: 2,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                child: ListTile(
                  leading: const Icon(Icons.settings, color: Colors.blue),
                  title: const Text('Account Settings'),
                  trailing: const Icon(Icons.arrow_forward_ios, size: 16),
                  onTap: () {
                    Navigator.push(context, MaterialPageRoute(builder: (_) => const SettingsScreen()));
                  },
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16.0),
              child: Card(
                elevation: 2,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                child: ListTile(
                  leading: const Icon(Icons.logout, color: Colors.red),
                  title: const Text('Logout', style: TextStyle(color: Colors.red)),
                  onTap: () {
                    context.read<AuthViewModel>().logout();
                  },
                ),
              ),
            ),
            const SizedBox(height: 24),
          ],
        ),
      ),
    );
  }

  void _showAllAchievements(BuildContext context, List<dynamic> achievements) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (context) => DraggableScrollableSheet(
        initialChildSize: 0.6,
        minChildSize: 0.4,
        maxChildSize: 0.9,
        expand: false,
        builder: (context, scrollController) {
          return Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              children: [
                Container(
                  width: 40,
                  height: 4,
                  margin: const EdgeInsets.only(bottom: 16),
                  decoration: BoxDecoration(
                    color: Colors.grey[300],
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
                Text(
                  AppLocalizations.of(context)!.achievements, // Replaced title
                  style: const TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 16),
                Expanded(
                  child: ListView.builder(
                    controller: scrollController,
                    itemCount: achievements.length,
                    itemBuilder: (context, index) {
                      return AchievementCard(achievement: achievements[index]);
                    },
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}
