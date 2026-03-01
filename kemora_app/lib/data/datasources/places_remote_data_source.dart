import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../models/place_model.dart';

abstract class PlacesRemoteDataSource {
  Future<List<PlaceModel>> getPlaces();
  Future<List<PlaceModel>> getPlacesByCategory(String category);
  Future<PlaceModel> getPlaceDetails(String id);
}

class PlacesRemoteDataSourceImpl implements PlacesRemoteDataSource {
  final Dio dio;

  PlacesRemoteDataSourceImpl({required this.dio});

  @override
  Future<List<PlaceModel>> getPlaces() async {
    try {
      final response = await dio.get('/api/v1/places');
      if (response.statusCode == 200) {
        // The API returns a PagedResult containing an 'items' array
        final data = response.data['items'] ?? response.data;
        if (data is List) {
          return data.map((json) => PlaceModel.fromJson(json)).toList();
        }
        return [];
      } else {
        throw const ServerFailure('Failed to load places');
      }
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }

  @override
  Future<List<PlaceModel>> getPlacesByCategory(String category) async {
    try {
      final response = await dio.get('/api/v1/places', queryParameters: {'category': category});
      if (response.statusCode == 200) {
        // The API returns a PagedResult containing an 'items' array
        final data = response.data['items'] ?? response.data;
        if (data is List) {
          return data.map((json) => PlaceModel.fromJson(json)).toList();
        }
        return [];
      } else {
        throw const ServerFailure('Failed to load places by category');
      }
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }

  @override
  Future<PlaceModel> getPlaceDetails(String id) async {
    try {
      final response = await dio.get('/api/v1/places/$id');
      if (response.statusCode == 200) {
        return PlaceModel.fromJson(response.data);
      } else {
        throw const ServerFailure('Failed to load place details');
      }
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }
}
