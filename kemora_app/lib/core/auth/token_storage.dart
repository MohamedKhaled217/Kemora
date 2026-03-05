/// A simple singleton to hold the JWT token in memory during the session.
/// In production, this should be replaced with secure storage (flutter_secure_storage).
class TokenStorage {
  TokenStorage._();
  static final TokenStorage _instance = TokenStorage._();
  static TokenStorage get instance => _instance;

  String? _token;
  String? _refreshToken;

  String? get token => _token;
  String? get refreshToken => _refreshToken;

  void saveTokens({required String token, String? refreshToken}) {
    _token = token;
    _refreshToken = refreshToken;
  }

  void clearTokens() {
    _token = null;
    _refreshToken = null;
  }

  bool get isAuthenticated => _token != null && _token!.isNotEmpty;
}
