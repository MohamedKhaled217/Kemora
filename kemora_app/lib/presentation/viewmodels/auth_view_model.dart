import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart' show kIsWeb;
import 'package:google_sign_in/google_sign_in.dart';
import '../../domain/entities/user.dart';
import '../../domain/usecases/login_usecase.dart';
import '../../domain/usecases/register_usecase.dart';
import '../../domain/usecases/google_login_usecase.dart';
import '../../domain/usecases/update_preferences_usecase.dart';
import '../../domain/usecases/change_password_usecase.dart';
import '../../domain/usecases/change_email_usecase.dart';
import '../../domain/usecases/update_profile_usecase.dart';
import '../../domain/usecases/upload_profile_picture_usecase.dart';
import '../../domain/entities/user_preferences.dart';
import '../../core/auth/token_storage.dart';
import 'package:image_picker/image_picker.dart';

enum AuthState { initial, loading, authenticated, unauthenticated, error }

class AuthViewModel extends ChangeNotifier {
  final LoginUseCase loginUseCase;
  final RegisterUseCase registerUseCase;
  final GoogleLoginUseCase googleLoginUseCase;
  final UpdatePreferencesUseCase updatePreferencesUseCase;
  final ChangePasswordUseCase changePasswordUseCase;
  final ChangeEmailUseCase changeEmailUseCase;
  final UpdateProfileUseCase updateProfileUseCase;
  final UploadProfilePictureUseCase uploadProfilePictureUseCase;

  final GoogleSignIn _googleSignIn = GoogleSignIn(
    scopes: ['email', 'profile'],
  );

  AuthViewModel({
    required this.loginUseCase,
    required this.registerUseCase,
    required this.googleLoginUseCase,
    required this.updatePreferencesUseCase,
    required this.changePasswordUseCase,
    required this.changeEmailUseCase,
    required this.updateProfileUseCase,
    required this.uploadProfilePictureUseCase,
  });

  AuthState _state = AuthState.initial;
  AuthState get state => _state;

  User? _user;
  User? get user => _user;

  String? _errorMessage;
  String? get errorMessage => _errorMessage;

  void _onAuthSuccess(User user) {
    _user = user;
    // Persist JWT token so Dio interceptor includes it in all subsequent requests
    if (user.token != null && user.token!.isNotEmpty) {
      TokenStorage.instance.saveTokens(
        token: user.token!,
        refreshToken: user.refreshToken,
      );
    }
    _state = AuthState.authenticated;
    notifyListeners();
  }

  Future<void> login(String email, String password) async {
    _state = AuthState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await loginUseCase(email, password);

    result.fold(
      (failure) {
        _state = AuthState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (user) => _onAuthSuccess(user),
    );
  }

  Future<void> signInWithGoogle() async {
    // Google Sign-In on Web requires a Google OAuth2 Client ID configured
    // in web/index.html. Until it's set up, show a clear message.
    if (kIsWeb) {
      _state = AuthState.error;
      _errorMessage = 'Google Sign-In is not yet configured for Web. Please use email/password login.';
      notifyListeners();
      return;
    }

    _state = AuthState.loading;
    _errorMessage = null;
    notifyListeners();

    try {
      final GoogleSignInAccount? googleUser = await _googleSignIn.signIn();
      if (googleUser == null) {
        _state = AuthState.initial;
        notifyListeners();
        return;
      }

      final GoogleSignInAuthentication googleAuth = await googleUser.authentication;
      final String? idToken = googleAuth.idToken;

      if (idToken == null) {
        _state = AuthState.error;
        _errorMessage = 'Could not retrieve ID token from Google';
        notifyListeners();
        return;
      }

      final result = await googleLoginUseCase(idToken);

      result.fold(
        (failure) {
          _state = AuthState.error;
          _errorMessage = failure.message;
          notifyListeners();
        },
        (user) => _onAuthSuccess(user),
      );
    } catch (e) {
      _state = AuthState.error;
      _errorMessage = 'Google Sign-In Error: ${e.toString()}';
      notifyListeners();
    }
  }

  Future<void> updatePreferences(UserPreferences prefs) async {
    _state = AuthState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await updatePreferencesUseCase(prefs);

    result.fold(
      (failure) {
        _state = AuthState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (_) {
        // Preferences updated — keep existing user but refresh state
        _state = AuthState.authenticated;
        notifyListeners();
      },
    );
  }

  Future<void> register(String fullName, String email, String country, String password) async {
    _state = AuthState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await registerUseCase(fullName, email, country, password);

    result.fold(
      (failure) {
        _state = AuthState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (user) => _onAuthSuccess(user),
    );
  }

  Future<void> changePassword(String currentPassword, String newPassword) async {
    _state = AuthState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await changePasswordUseCase(currentPassword, newPassword);

    result.fold(
      (failure) {
        _state = AuthState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (_) {
        _state = AuthState.authenticated;
        notifyListeners();
      },
    );
  }

  Future<void> changeEmail(String newEmail, String password) async {
    _state = AuthState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await changeEmailUseCase(newEmail, password);

    result.fold(
      (failure) {
        _state = AuthState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (_) {
        // Assume user object still kept in state, but email changes.
        // We could log them out or just rely on backend to update.
        _state = AuthState.authenticated;
        notifyListeners();
      },
    );
  }

  void logout() {
    _user = null;
    TokenStorage.instance.clearTokens();
    _googleSignIn.signOut();
    _state = AuthState.unauthenticated;
    notifyListeners();
  }

  Future<void> updateProfile(String fullName, String? bio) async {
    _state = AuthState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await updateProfileUseCase(fullName, bio);

    result.fold(
      (failure) {
        _state = AuthState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (updatedUser) {
        // Merge updated fields into existing user
        if (_user != null) {
          _user = _user!.copyWith(
            fullName: updatedUser.fullName,
            bio: updatedUser.bio,
          );
        }
        _state = AuthState.authenticated;
        notifyListeners();
      },
    );
  }

  Future<void> uploadProfilePicture() async {
    final picker = ImagePicker();
    final XFile? image = await picker.pickImage(source: ImageSource.gallery);
    
    if (image == null) return;

    _state = AuthState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await uploadProfilePictureUseCase(image.path);

    result.fold(
      (failure) {
        _state = AuthState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (imageUrl) {
        if (_user != null) {
          _user = _user!.copyWith(profilePictureUrl: imageUrl);
        }
        _state = AuthState.authenticated;
        notifyListeners();
      },
    );
  }
}
