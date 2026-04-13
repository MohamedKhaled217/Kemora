import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../screens/auth/login_screen.dart';
import '../../screens/auth/signup_screen.dart';
import '../../screens/places/places_screen.dart';
import '../../screens/place_details/place_details_screen.dart';
import '../../models/country.dart';
import '../../models/city.dart';
import '../../models/place.dart';

// Shell for bottom navigation (to be implemented later)
// Shell for bottom navigation (to be implemented later)
import '../../screens/common/scaffold_with_navbar.dart';
import '../../screens/map/egypt_map_screen.dart';
import '../../screens/community/community_screen.dart';
import '../../screens/profile/profile_screen.dart';
import '../../domain/entities/ai_itinerary.dart';

final GlobalKey<NavigatorState> _rootNavigatorKey = GlobalKey<NavigatorState>();
final GlobalKey<NavigatorState> _shellNavigatorKey =
    GlobalKey<NavigatorState>();

final GoRouter appRouter = GoRouter(
  navigatorKey: _rootNavigatorKey,
  initialLocation: '/login',
  routes: [
    GoRoute(path: '/login', builder: (context, state) => const LoginScreen()),
    GoRoute(path: '/signup', builder: (context, state) => const SignupScreen()),
    ShellRoute(
      navigatorKey: _shellNavigatorKey,
      builder: (context, state, child) {
        return ScaffoldWithNavBar(child: child);
      },
      routes: [
        GoRoute(
          path: '/map',
          builder: (context, state) => const EgyptMapScreen(),
          routes: [
            GoRoute(
              path: 'ai-planner',
              builder: (context, state) => const AiFormScreen(),
              routes: [
                GoRoute(
                  path: 'result',
                  builder: (context, state) {
                    final plan = state.extra as AIItinerary;
                    return AiResultScreen(itinerary: plan);
                  },
                ),
              ],
            ),
            GoRoute(
              path: 'places',
              builder: (context, state) {
                final country = (state.extra as Map)['country'] as Country;
                final city = (state.extra as Map)['city'] as City;
                return PlacesScreen(country: country, city: city);
              },
              routes: [
                GoRoute(
                  path: 'details',
                  builder: (context, state) {
                    final place = (state.extra as Map)['place'] as Place;
                    return PlaceDetailsScreen(place: place);
                  },
                ),
              ],
            ),
          ],
        ),
        GoRoute(
          path: '/community',
          builder: (context, state) => const CommunityScreen(),
        ),
        GoRoute(
          path: '/profile',
          builder: (context, state) => const ProfileScreen(),
        ),
      ],
    ),
  ],
);
