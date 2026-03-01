import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../entities/post.dart';
import '../repositories/i_post_repository.dart';

class GetFeedUseCase {
  final IPostRepository repository;

  GetFeedUseCase(this.repository);

  Future<Either<Failure, List<Post>>> call() async {
    return await repository.getFeed();
  }
}

class CreatePostUseCase {
  final IPostRepository repository;

  CreatePostUseCase(this.repository);

  Future<Either<Failure, Post>> call(String content, {String? imagePath, String? locationId}) async {
    return await repository.createPost(content, imagePath: imagePath, locationId: locationId);
  }
}
