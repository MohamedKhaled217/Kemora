import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../entities/trip.dart';
import '../repositories/i_trip_repository.dart';

class CreateTripPlanUseCase {
  final ITripRepository repository;

  CreateTripPlanUseCase(this.repository);

  Future<Either<Failure, Trip>> call(String title, DateTime startDate, DateTime endDate, List<String> placeIds) async {
    return await repository.createTripPlan(title, startDate, endDate, placeIds);
  }
}

class GetUserTripsUseCase {
  final ITripRepository repository;

  GetUserTripsUseCase(this.repository);

  Future<Either<Failure, List<Trip>>> call() async {
    return await repository.getUserTrips();
  }
}
