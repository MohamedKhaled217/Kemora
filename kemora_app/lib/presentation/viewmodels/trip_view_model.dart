import 'package:flutter/material.dart';

import '../../domain/entities/trip.dart';
import '../../domain/usecases/trip_usecases.dart';

enum TripState { initial, loading, loaded, error }

class TripViewModel extends ChangeNotifier {
  final GetUserTripsUseCase getUserTripsUseCase;
  final CreateTripPlanUseCase createTripPlanUseCase;

  TripViewModel({
    required this.getUserTripsUseCase,
    required this.createTripPlanUseCase,
  });

  TripState _state = TripState.initial;
  TripState get state => _state;

  List<Trip> _trips = [];
  List<Trip> get trips => _trips;

  String? _errorMessage;
  String? get errorMessage => _errorMessage;

  Future<void> loadTrips() async {
    _state = TripState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await getUserTripsUseCase();

    result.fold(
      (failure) {
        _state = TripState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (tripsList) {
        _trips = tripsList;
        _state = TripState.loaded;
        notifyListeners();
      },
    );
  }

  Future<bool> createTrip(
      String title, DateTime start, DateTime end, List<String> places) async {
    _state = TripState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await createTripPlanUseCase(title, start, end, places);

    return result.fold(
      (failure) {
        _state = TripState.error;
        _errorMessage = failure.message;
        notifyListeners();
        return false;
      },
      (trip) {
        _trips.add(trip);
        _state = TripState.loaded;
        notifyListeners();
        return true;
      },
    );
  }
}
