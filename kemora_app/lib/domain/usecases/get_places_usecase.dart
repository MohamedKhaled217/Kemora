import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../entities/place.dart';
import '../repositories/i_place_repository.dart';

class GetPlacesUseCase {
  final IPlaceRepository repository;

  GetPlacesUseCase(this.repository);

  Future<Either<Failure, List<Place>>> call() async {
    return await repository.getPlaces();
  }
}
