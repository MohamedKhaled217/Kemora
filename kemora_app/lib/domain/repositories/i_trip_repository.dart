import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../entities/trip.dart';

abstract class ITripRepository {
  Future<Either<Failure, Trip>> createTripPlan(String title, DateTime startDate, DateTime endDate, List<String> placeIds);
  Future<Either<Failure, List<Trip>>> getUserTrips();
}
