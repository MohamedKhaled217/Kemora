import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../models/user_model.dart';

abstract class AuthRemoteDataSource {
  Future<UserModel> login(String email, String password);
  Future<UserModel> register(String fullName, String email, String country, String password);
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final Dio dio;

  AuthRemoteDataSourceImpl({required this.dio});

  @override
  Future<UserModel> login(String email, String password) async {
    try {
      final response = await dio.post(
        '/api/v1/auth/login',
        data: {'email': email, 'password': password},
      );
      
      if (response.statusCode == 200) {
        // Handle both `{ user: {...} }` and flat `{ id, email, token }` responses
        return UserModel.fromJson(response.data['user'] ?? response.data);
      } else {
        throw const ServerFailure('Invalid credentials');
      }
    } on DioException catch (e) {
      final data = e.response?.data;
      final errorMessage = data is String ? data : (data?['message'] ?? 'Server Error');
      throw ServerFailure(errorMessage);
    }
  }

  @override
  Future<UserModel> register(String fullName, String email, String country, String password) async {
    try {
      final response = await dio.post(
        '/api/v1/auth/register',
        data: {
          'fullName': fullName,
          'email': email,
          'country': country,
          'password': password
        },
      );
      
      if (response.statusCode == 200 || response.statusCode == 201) {
        return UserModel.fromJson(response.data['user'] ?? response.data);
      } else {
        throw const ServerFailure('Registration failed');
      }
    } on DioException catch (e) {
      final data = e.response?.data;
      final errorMessage = data is String ? data : (data?['message'] ?? 'Server Error');
      throw ServerFailure(errorMessage);
    }
  }
}
