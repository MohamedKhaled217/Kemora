import 'package:flutter/material.dart';
import '../../domain/entities/user.dart';
import '../../domain/usecases/login_usecase.dart';
import '../../domain/usecases/register_usecase.dart';

enum AuthState { initial, loading, authenticated, unauthenticated, error }

class AuthViewModel extends ChangeNotifier {
  final LoginUseCase loginUseCase;
  final RegisterUseCase registerUseCase;

  AuthViewModel({required this.loginUseCase, required this.registerUseCase});

  AuthState _state = AuthState.initial;
  AuthState get state => _state;

  User? _user;
  User? get user => _user;

  String? _errorMessage;
  String? get errorMessage => _errorMessage;

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
      (user) {
        _user = user;
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
      (user) {
        _user = user;
        _state = AuthState.authenticated;
        notifyListeners();
      },
    );
  }

  void logout() {
    _user = null;
    _state = AuthState.unauthenticated;
    notifyListeners();
  }
}
