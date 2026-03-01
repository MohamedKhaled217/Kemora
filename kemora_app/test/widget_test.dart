import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:kemora/core/di/injection_container.dart' as di;
import 'package:kemora/presentation/screens/auth/login_screen.dart';
import 'package:kemora/presentation/viewmodels/auth_view_model.dart';
import 'package:provider/provider.dart';

void main() {
  setUpAll(() async {
    // Initialize dependency injection
    await di.init();
  });

  testWidgets('Login screen UI test', (WidgetTester tester) async {
    // Build the LoginScreen widget with necessary providers.
    await tester.pumpWidget(
      ChangeNotifierProvider<AuthViewModel>(
        create: (_) => di.sl<AuthViewModel>(),
        child: const MaterialApp(
          home: LoginScreen(),
        ),
      ),
    );

    // Let the widget tree build.
    await tester.pump();

    // Verify the AppBar title.
    expect(find.text('Login to Kemora'), findsOneWidget);

    // Verify the welcome text.
    expect(find.text('Welcome Back to Kemora'), findsOneWidget);

    // Find TextFields by their labelText.
    expect(find.byWidgetPredicate((widget) => widget is TextField && widget.decoration?.labelText == 'Email'), findsOneWidget);
    expect(find.byWidgetPredicate((widget) => widget is TextField && widget.decoration?.labelText == 'Password'), findsOneWidget);

    // Verify the presence of the login button.
    expect(find.widgetWithText(ElevatedButton, 'Login'), findsOneWidget);
  });
}
