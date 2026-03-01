import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../entities/place.dart';

abstract class IPlaceRepository {
  Future<Either<Failure, List<Place>>> getPlaces();
  Future<Either<Failure, List<Place>>> getPlacesByCategory(String category);
  Future<Either<Failure, Place>> getPlaceDetails(String id);
}
