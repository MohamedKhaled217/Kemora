import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import '../models/country.dart';
import '../models/post.dart';
import '../services/mock_data_service.dart';

class AppProvider with ChangeNotifier {
  List<Country> _countries = [];
  bool _isLoading = false;

  List<Country> get countries => _countries;
  bool get isLoading => _isLoading;

  List<Post> _posts = [];
  List<Post> get posts => _posts;

  void loadPosts() {
    if (_posts.isNotEmpty) return;
    _posts = MockDataService.getPosts();
    notifyListeners();
  }

  void addPost(Post post) {
    _posts.insert(0, post);
    notifyListeners();
  }

  Locale _currentLocale = const Locale('en');
  Locale get currentLocale => _currentLocale;

  void changeLanguage(Locale locale) {
    _currentLocale = locale;
    notifyListeners();
  }

  Future<void> loadCountries() async {
    _isLoading = true;
    notifyListeners();

    // Simulate network delay
    await Future.delayed(const Duration(seconds: 1));

    _countries = MockDataService.getCountries();
    _isLoading = false;
    notifyListeners();
  }
}
