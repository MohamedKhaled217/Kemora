import 'package:equatable/equatable.dart';
import 'place.dart';

class Trip extends Equatable {
  final String id;
  final String title;
  final DateTime startDate;
  final DateTime endDate;
  final List<Place> plannedPlaces;

  const Trip({
    required this.id,
    required this.title,
    required this.startDate,
    required this.endDate,
    this.plannedPlaces = const [],
  });

  @override
  List<Object?> get props => [id, title, startDate, endDate, plannedPlaces];
}
