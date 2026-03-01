import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../../domain/entities/trip.dart';
import '../../domain/repositories/i_trip_repository.dart';
import '../datasources/trip_remote_data_source.dart';

class TripRepositoryImpl implements ITripRepository {
  final TripRemoteDataSource remoteDataSource;

  TripRepositoryImpl({required this.remoteDataSource});

  @override
  Future<Either<Failure, Trip>> createTripPlan(String title, DateTime startDate, DateTime endDate, List<String> placeIds) async {
    try {
      final trip = await remoteDataSource.createTripPlan(title, startDate, endDate, placeIds);
      return Right(trip);
    } on Failure catch (e) {
      return Left(e);
    } catch (e) {
      return const Left(ServerFailure('Unexpected Error'));
    }
  }

  @override
  Future<Either<Failure, List<Trip>>> getUserTrips() async {
    try {
      final trips = await remoteDataSource.getUserTrips();
      return Right(trips);
    } on Failure catch (e) {
      return Left(e);
    } catch (e) {
      return const Left(ServerFailure('Unexpected Error'));
    }
  }
}
