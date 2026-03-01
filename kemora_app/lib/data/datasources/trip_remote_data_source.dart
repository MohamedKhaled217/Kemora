import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../models/trip_model.dart';

abstract class TripRemoteDataSource {
  Future<TripModel> createTripPlan(String title, DateTime startDate, DateTime endDate, List<String> placeIds);
  Future<List<TripModel>> getUserTrips();
}

class TripRemoteDataSourceImpl implements TripRemoteDataSource {
  final Dio dio;

  TripRemoteDataSourceImpl({required this.dio});

  @override
  Future<TripModel> createTripPlan(String title, DateTime startDate, DateTime endDate, List<String> placeIds) async {
    try {
      final response = await dio.post(
        '/api/v1/trips',
        data: {
          'title': title,
          'startDate': startDate.toIso8601String(),
          'endDate': endDate.toIso8601String(),
          'placeIds': placeIds,
        },
      );
      if (response.statusCode == 200 || response.statusCode == 201) {
        return TripModel.fromJson(response.data);
      } else {
        throw const ServerFailure('Failed to create trip');
      }
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }

  @override
  Future<List<TripModel>> getUserTrips() async {
    try {
      final response = await dio.get('/api/v1/trips');
      if (response.statusCode == 200) {
        final List<dynamic> data = response.data;
        return data.map((json) => TripModel.fromJson(json)).toList();
      } else {
        throw const ServerFailure('Failed to fetch trips');
      }
    } on DioException catch (e) {
      throw ServerFailure(e.response?.data['message'] ?? 'Server Error');
    }
  }
}
