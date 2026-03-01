import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../entities/post.dart';

abstract class IPostRepository {
  Future<Either<Failure, List<Post>>> getFeed();
  Future<Either<Failure, Post>> createPost(String content, {String? imagePath, String? locationId});
  Future<Either<Failure, void>> likePost(String postId);
  Future<Either<Failure, void>> unlikePost(String postId);
  Future<Either<Failure, List<Comment>>> getPostComments(String postId);
  Future<Either<Failure, Comment>> addComment(String postId, String content);
}
