import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../../domain/entities/user.dart';
import '../../domain/repositories/i_auth_repository.dart';
import '../datasources/auth_remote_data_source.dart';

class AuthRepositoryImpl implements IAuthRepository {
  final AuthRemoteDataSource remoteDataSource;

  AuthRepositoryImpl({required this.remoteDataSource});

  @override
  Future<Either<Failure, User>> login(String email, String password) async {
    try {
      final user = await remoteDataSource.login(email, password);
      return Right(user);
    } on Failure catch (e) {
      return Left(e);
    } catch (e) {
      return Left(ServerFailure('Connection Error: ${e.toString()}'));
    }
  }

  @override
  Future<Either<Failure, User>> register(String fullName, String email, String country, String password) async {
    try {
      final user = await remoteDataSource.register(fullName, email, country, password);
      return Right(user);
    } on Failure catch (e) {
      return Left(e);
    } catch (e) {
      return Left(ServerFailure('Connection Error: ${e.toString()}'));
    }
  }

  @override
  Future<Either<Failure, void>> logout() async {
    // Implement token clearing logic here
    return const Right(null);
  }

  @override
  Future<Either<Failure, User>> getCurrentUser() async {
    // Implement local cache fetch or /api/auth/me logic
    return const Left(CacheFailure('No cached user found'));
  }
}
