import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../repositories/i_auth_repository.dart';

class UploadProfilePictureUseCase {
  final IAuthRepository repository;

  UploadProfilePictureUseCase(this.repository);

  Future<Either<Failure, String>> call(String filePath) async {
    return await repository.uploadProfilePicture(filePath);
  }
}
