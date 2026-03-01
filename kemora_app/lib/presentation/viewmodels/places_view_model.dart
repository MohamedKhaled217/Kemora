import 'package:flutter/material.dart';
import '../../domain/entities/place.dart';
import '../../domain/usecases/get_places_usecase.dart';
import '../../domain/usecases/get_places_by_category_usecase.dart';

enum PlacesState { initial, loading, loaded, error }

class PlacesViewModel extends ChangeNotifier {
  final GetPlacesUseCase getPlacesUseCase;
  final GetPlacesByCategoryUseCase getPlacesByCategoryUseCase;

  PlacesViewModel({
    required this.getPlacesUseCase,
    required this.getPlacesByCategoryUseCase,
  });

  PlacesState _state = PlacesState.initial;
  PlacesState get state => _state;

  List<Place> _places = [];
  List<Place> get places => _places;

  String? _errorMessage;
  String? get errorMessage => _errorMessage;

  String _currentCategory = 'All';
  String get currentCategory => _currentCategory;

  Future<void> loadPlaces([String category = 'All']) async {
    _state = PlacesState.loading;
    _currentCategory = category;
    _errorMessage = null;
    notifyListeners();

    final result = category == 'All'
        ? await getPlacesUseCase()
        : await getPlacesByCategoryUseCase(category);

    result.fold(
      (failure) {
        _state = PlacesState.error;
        _errorMessage = failure.message;
        notifyListeners();
      },
      (placesList) {
        _places = placesList;
        _state = PlacesState.loaded;
        notifyListeners();
      },
    );
  }
}
