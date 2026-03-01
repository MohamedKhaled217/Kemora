import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../models/post_model.dart';

abstract class PostRemoteDataSource {
  Future<List<PostModel>> getFeed();
  Future<PostModel> createPost(String content, {String? imagePath, String? locationId});
  Future<void> likePost(String postId);
  Future<void> unlikePost(String postId);
  Future<List<CommentModel>> getPostComments(String postId);
  Future<CommentModel> addComment(String postId, String content);
}

class PostRemoteDataSourceImpl implements PostRemoteDataSource {
  final Dio dio;

  PostRemoteDataSourceImpl({required this.dio});

  @override
  Future<List<PostModel>> getFeed() async {
    try {
      final response = await dio.get('/api/v1/posts');
      if (response.statusCode == 200) {
        final List<dynamic> data = response.data;
        return data.map((json) => PostModel.fromJson(json)).toList();
      } else {
        throw const ServerFailure('Failed to fetch feed');
      }
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }

  @override
  Future<PostModel> createPost(String content, {String? imagePath, String? locationId}) async {
    try {
      final formData = FormData.fromMap({
        'content': content,
        if (locationId != null) 'locationId': locationId,
        if (imagePath != null) 'image': await MultipartFile.fromFile(imagePath),
      });

      final response = await dio.post('/api/v1/posts', data: formData);
      if (response.statusCode == 200 || response.statusCode == 201) {
        return PostModel.fromJson(response.data);
      } else {
        throw const ServerFailure('Failed to create post');
      }
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }

  @override
  Future<void> likePost(String postId) async {
    try {
      final response = await dio.post('/api/v1/reactions/like/$postId');
      if (response.statusCode != 200) throw const ServerFailure('Failed to like post');
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }

  @override
  Future<void> unlikePost(String postId) async {
    try {
      final response = await dio.post('/api/v1/reactions/unlike/$postId');
      if (response.statusCode != 200) throw const ServerFailure('Failed to unlike post');
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }

  @override
  Future<List<CommentModel>> getPostComments(String postId) async {
    try {
      final response = await dio.get('/api/v1/comments/post/$postId');
      if (response.statusCode == 200) {
        final List<dynamic> data = response.data;
        return data.map((json) => CommentModel.fromJson(json)).toList();
      } else {
        throw const ServerFailure('Failed to fetch comments');
      }
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }

  @override
  Future<CommentModel> addComment(String postId, String content) async {
    try {
      final response = await dio.post('/api/v1/comments/post/$postId', data: {'content': content});
      if (response.statusCode == 200 || response.statusCode == 201) {
        return CommentModel.fromJson(response.data);
      } else {
        throw const ServerFailure('Failed to add comment');
      }
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }
}
