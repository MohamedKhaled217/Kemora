import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:provider/provider.dart';

import 'core/di/injection_container.dart' as di;
import 'core/theme/app_theme.dart';
import 'presentation/screens/auth/login_screen.dart';
import 'presentation/viewmodels/auth_view_model.dart';
import 'presentation/viewmodels/badge_view_model.dart';
import 'presentation/viewmodels/places_view_model.dart';
import 'presentation/viewmodels/post_view_model.dart';
import 'presentation/viewmodels/trip_view_model.dart';
import 'presentation/viewmodels/chat_view_model.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await di.init();
  runApp(const KemoraApp());
}

class KemoraApp extends StatelessWidget {
  const KemoraApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (context) => di.sl<AuthViewModel>()),
        ChangeNotifierProvider(create: (context) => di.sl<PlacesViewModel>()),
        ChangeNotifierProvider(create: (context) => di.sl<TripViewModel>()),
        ChangeNotifierProvider(create: (context) => di.sl<PostViewModel>()),
        ChangeNotifierProvider(create: (context) => di.sl<BadgeViewModel>()),
        ChangeNotifierProvider(create: (context) => di.sl<ChatViewModel>()),
      ],
      child: MaterialApp(
        title: 'Kemora Travel Guide',
        theme: AppTheme.lightTheme,
        debugShowCheckedModeBanner: false,
        // Using LoginScreen directly for initial development.
        // GoRouter can be implemented later when adding bottom nav and other routes.
        home: const LoginScreen(),
        localizationsDelegates: const [
          GlobalMaterialLocalizations.delegate,
          GlobalWidgetsLocalizations.delegate,
          GlobalCupertinoLocalizations.delegate,
        ],
        supportedLocales: const [Locale('en'), Locale('ar')],
      ),
    );
  }
}
