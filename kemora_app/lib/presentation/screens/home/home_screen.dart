import 'package:flutter/material.dart';
import '../explore/places_screen.dart';
import '../trip/trip_planner_screen.dart';
import '../social/feed_screen.dart';
import '../badges/badges_screen.dart';
import 'package:provider/provider.dart';
import '../../viewmodels/auth_view_model.dart';
import '../auth/login_screen.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  int _currentIndex = 0;

  @override
  Widget build(BuildContext context) {
    final authViewModel = context.watch<AuthViewModel>();
    
    // Fallback if user is null somehow, although it shouldn't happen if properly logged in
    final userId = authViewModel.user?.id ?? '1';

    final screens = [
      const PlacesScreen(),
      const TripPlannerScreen(),
      const FeedScreen(),
      BadgesScreen(userId: userId),
    ];

    return Scaffold(
      body: screens[_currentIndex],
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _currentIndex,
        onTap: (index) {
          setState(() {
            _currentIndex = index;
          });
        },
        items: const [
          BottomNavigationBarItem(
            icon: Icon(Icons.explore_outlined),
            activeIcon: Icon(Icons.explore),
            label: 'Explore',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.flight_takeoff_outlined),
            activeIcon: Icon(Icons.flight_takeoff),
            label: 'Trips',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.feed_outlined),
            activeIcon: Icon(Icons.feed),
            label: 'Social',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.military_tech_outlined),
            activeIcon: Icon(Icons.military_tech),
            label: 'Badges',
          ),
        ],
      ),
    );
  }
}
