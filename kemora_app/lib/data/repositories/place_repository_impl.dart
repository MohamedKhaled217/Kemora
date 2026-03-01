import 'package:dartz/dartz.dart';
import '../../core/error/failures.dart';
import '../../domain/entities/place.dart';
import '../../domain/repositories/i_place_repository.dart';
import '../datasources/places_remote_data_source.dart';

class PlaceRepositoryImpl implements IPlaceRepository {
  final PlacesRemoteDataSource remoteDataSource;

  PlaceRepositoryImpl({required this.remoteDataSource});

  @override
  Future<Either<Failure, List<Place>>> getPlaces() async {
    try {
      final places = await remoteDataSource.getPlaces();
      return Right(places);
    } on Failure catch (e) {
      return Left(e);
    } catch (e) {
      return const Left(ServerFailure('Unexpected Error'));
    }
  }

  @override
  Future<Either<Failure, List<Place>>> getPlacesByCategory(String category) async {
    try {
      final places = await remoteDataSource.getPlacesByCategory(category);
      return Right(places);
    } on Failure catch (e) {
      return Left(e);
    } catch (e) {
      return const Left(ServerFailure('Unexpected Error'));
    }
  }

  @override
  Future<Either<Failure, Place>> getPlaceDetails(String id) async {
    try {
      final place = await remoteDataSource.getPlaceDetails(id);
      return Right(place);
    } on Failure catch (e) {
      return Left(e);
    } catch (e) {
      return const Left(ServerFailure('Unexpected Error'));
    }
  }
}
