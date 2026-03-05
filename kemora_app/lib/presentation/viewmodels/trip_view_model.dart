import 'package:flutter/material.dart';

import '../../domain/entities/trip.dart';
import '../../domain/usecases/trip_usecases.dart';
import '../../domain/usecases/generate_ai_itinerary_usecase.dart';
import '../../domain/usecases/swap_place_usecase.dart';
import '../../domain/usecases/save_ai_plan_usecase.dart';
import '../../domain/entities/ai_itinerary.dart';
import '../../domain/entities/trip_plan_request.dart';

enum TripState { initial, loading, loaded, error }

class TripViewModel extends ChangeNotifier {
  final GetUserTripsUseCase getUserTripsUseCase;
  final CreateTripPlanUseCase createTripPlanUseCase;
  final GenerateAiItineraryUseCase generateAiItineraryUseCase;
  final SwapPlaceUseCase swapPlaceUseCase;
  final SaveAiPlanUseCase saveAiPlanUseCase;

  TripViewModel({
    required this.getUserTripsUseCase,
    required this.createTripPlanUseCase,
    required this.generateAiItineraryUseCase,
    required this.swapPlaceUseCase,
    required this.saveAiPlanUseCase,
  });

  TripState _state = TripState.initial;
  TripState get state => _state;

  List<Trip> _trips = [];
  List<Trip> get trips => _trips;

  AIItinerary? _currentPlan;
  AIItinerary? get currentPlan => _currentPlan;

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

  Future<void> generateAiItinerary(TripPlanRequest request) async {
    _state = TripState.loading;
    _errorMessage = null;
    _currentPlan = null;
    notifyListeners();

    final result = await generateAiItineraryUseCase(request);

    result.fold(
      (failure) {
        _state = TripState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (itinerary) {
        _currentPlan = itinerary;
        _state = TripState.loaded;
        notifyListeners();
      },
    );
  }

  Future<void> swapPlace(String currentPlaceName, String preferences) async {
    if (_currentPlan == null) return;
    
    final result = await swapPlaceUseCase(currentPlaceName, preferences);

    result.fold(
      (failure) {
        _errorMessage = failure.message;
        notifyListeners();
      },
      (newItem) {
        final updatedDays = _currentPlan!.days.map((day) {
          final updatedActivities = day.activities.map((activity) {
            if (activity.name == currentPlaceName) {
              return newItem;
            }
            return activity;
          }).toList();
          return TripDay(dayNumber: day.dayNumber, activities: updatedActivities);
        }).toList();
        
        _currentPlan = AIItinerary(
          title: _currentPlan!.title,
          duration: _currentPlan!.duration,
          days: updatedDays,
        );
        notifyListeners();
      },
    );
  }

  Future<bool> savePlan(DateTime startDate, DateTime endDate) async {
    if (_currentPlan == null) return false;

    _state = TripState.loading;
    _errorMessage = null;
    notifyListeners();

    final result = await saveAiPlanUseCase(_currentPlan!, startDate, endDate);

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
