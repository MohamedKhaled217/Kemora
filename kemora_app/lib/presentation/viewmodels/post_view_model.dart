import 'package:flutter/material.dart';
import '../../domain/entities/post.dart';
import '../../domain/usecases/post_usecases.dart';

enum PostState { initial, loading, loaded, error }

class PostViewModel extends ChangeNotifier {
  final GetFeedUseCase getFeedUseCase;
  final CreatePostUseCase createPostUseCase;

  PostViewModel({
    required this.getFeedUseCase,
    required this.createPostUseCase,
  });

  PostState _state = PostState.initial;
  PostState get state => _state;

  List<Post> _posts = [];
  List<Post> get posts => _posts;

  String? _errorMessage;
  String? get errorMessage => _errorMessage;

  Future<void> loadFeed() async {
    _state = PostState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await getFeedUseCase();
    result.fold(
      (failure) {
        _state = PostState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (postsList) {
        _posts = postsList;
        _state = PostState.loaded;
        notifyListeners();
      },
    );
  }

  Future<void> createPost(String content, {String? imagePath}) async {
    _state = PostState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await createPostUseCase(content, imagePath: imagePath);
    result.fold(
      (failure) {
        _state = PostState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (post) {
        _posts.insert(0, post); // Add to top
        _state = PostState.loaded;
        notifyListeners();
      },
    );
  }
}
