import 'package:get_it/get_it.dart';
import 'package:dio/dio.dart';

import '../../data/datasources/auth_remote_data_source.dart';
import '../../data/datasources/places_remote_data_source.dart';
import '../../data/datasources/trip_remote_data_source.dart';
import '../../data/repositories/auth_repository_impl.dart';
import '../../data/repositories/place_repository_impl.dart';
import '../../data/repositories/trip_repository_impl.dart';
import '../../data/repositories/post_repository_impl.dart';
import '../../data/repositories/badge_repository_impl.dart';
import '../../data/datasources/post_remote_data_source.dart';
import '../../data/datasources/badge_remote_data_source.dart';
import '../../domain/repositories/i_auth_repository.dart';
import '../../domain/repositories/i_place_repository.dart';
import '../../domain/repositories/i_trip_repository.dart';
import '../../domain/repositories/i_post_repository.dart';
import '../../domain/repositories/i_badge_repository.dart';
import '../../domain/usecases/login_usecase.dart';
import '../../domain/usecases/register_usecase.dart';
import '../../domain/usecases/get_places_usecase.dart';
import '../../domain/usecases/get_places_by_category_usecase.dart';
import '../../domain/usecases/trip_usecases.dart';
import '../../domain/usecases/post_usecases.dart';
import '../../domain/usecases/badge_usecases.dart';
import '../../presentation/viewmodels/auth_view_model.dart';
import '../../presentation/viewmodels/places_view_model.dart';
import '../../presentation/viewmodels/trip_view_model.dart';
import '../../presentation/viewmodels/post_view_model.dart';
import '../../presentation/viewmodels/badge_view_model.dart';

final sl = GetIt.instance;

Future<void> init() async {
  // Features - Auth
  // ViewModels
  sl.registerFactory(() => AuthViewModel(loginUseCase: sl(), registerUseCase: sl()));

  // Use Cases
  sl.registerLazySingleton(() => LoginUseCase(sl()));
  sl.registerLazySingleton(() => RegisterUseCase(sl()));

  // Repository
  sl.registerLazySingleton<IAuthRepository>(
    () => AuthRepositoryImpl(remoteDataSource: sl()),
  );

  // Data sources
  sl.registerLazySingleton<AuthRemoteDataSource>(
    () => AuthRemoteDataSourceImpl(dio: sl()),
  );

  // Features - Places
  // ViewModels
  sl.registerFactory(() => PlacesViewModel(
        getPlacesUseCase: sl(),
        getPlacesByCategoryUseCase: sl(),
      ));

  // Use Cases
  sl.registerLazySingleton(() => GetPlacesUseCase(sl()));
  sl.registerLazySingleton(() => GetPlacesByCategoryUseCase(sl()));

  // Repository
  sl.registerLazySingleton<IPlaceRepository>(
    () => PlaceRepositoryImpl(remoteDataSource: sl()),
  );

  // Data sources
  sl.registerLazySingleton<PlacesRemoteDataSource>(
    () => PlacesRemoteDataSourceImpl(dio: sl()),
  );

  // Features - Trips
  // ViewModels
  sl.registerFactory(() => TripViewModel(
        getUserTripsUseCase: sl(),
        createTripPlanUseCase: sl(),
      ));

  // Use Cases
  sl.registerLazySingleton(() => GetUserTripsUseCase(sl()));
  sl.registerLazySingleton(() => CreateTripPlanUseCase(sl()));

  // Repository
  sl.registerLazySingleton<ITripRepository>(
    () => TripRepositoryImpl(remoteDataSource: sl()),
  );

  // Data sources
  sl.registerLazySingleton<TripRemoteDataSource>(
    () => TripRemoteDataSourceImpl(dio: sl()),
  );



  // Features - Social (Posts)
  // ViewModels
  sl.registerFactory(() => PostViewModel(
        getFeedUseCase: sl(),
        createPostUseCase: sl(),
      ));

  // Use Cases
  sl.registerLazySingleton(() => GetFeedUseCase(sl()));
  sl.registerLazySingleton(() => CreatePostUseCase(sl()));

  // Repository
  sl.registerLazySingleton<IPostRepository>(
    () => PostRepositoryImpl(remoteDataSource: sl()),
  );

  // Data sources
  sl.registerLazySingleton<PostRemoteDataSource>(
    () => PostRemoteDataSourceImpl(dio: sl()),
  );

  // Features - Badges (Gamification)
  // ViewModels
  sl.registerFactory(() => BadgeViewModel(
        getUserBadgesUseCase: sl(),
        getAllBadgesUseCase: sl(),
      ));

  // Use Cases
  sl.registerLazySingleton(() => GetUserBadgesUseCase(sl()));
  sl.registerLazySingleton(() => GetAllBadgesUseCase(sl()));

  // Repository
  sl.registerLazySingleton<IBadgeRepository>(
    () => BadgeRepositoryImpl(remoteDataSource: sl()),
  );

  // Data sources
  sl.registerLazySingleton<BadgeRemoteDataSource>(
    () => BadgeRemoteDataSourceImpl(dio: sl()),
  );

  // Core
  // TODO: Add Token Interceptor
  sl.registerLazySingleton(() {
    final dio = Dio(
      BaseOptions(
        baseUrl: 'http://localhost:5299', // Updated for Windows Desktop testing
        connectTimeout: const Duration(seconds: 10),
        receiveTimeout: const Duration(seconds: 10),
        headers: {'Content-Type': 'application/json'},
      ),
    );
    
    // Add logging interceptor for easier debugging
    dio.interceptors.add(LogInterceptor(
      request: true,
      requestHeader: true,
      requestBody: true,
      responseHeader: true,
      responseBody: true,
      error: true,
    ));
    
    return dio;
  });
}
