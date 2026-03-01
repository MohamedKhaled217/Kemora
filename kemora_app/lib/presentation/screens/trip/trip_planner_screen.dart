import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';
import '../../viewmodels/trip_view_model.dart';

class TripPlannerScreen extends StatefulWidget {
  const TripPlannerScreen({super.key});

  @override
  State<TripPlannerScreen> createState() => _TripPlannerScreenState();
}

class _TripPlannerScreenState extends State<TripPlannerScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<TripViewModel>().loadTrips();
    });
  }

  void _showCreateTripDialog() {
    // A simplified dialog; in a real app, this would be a full screen with place selection
    final titleController = TextEditingController();
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Create New Trip'),
        content: TextField(
          controller: titleController,
          decoration: const InputDecoration(labelText: 'Trip Title (e.g. Cairo Weekend)'),
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context), child: const Text('Cancel')),
          ElevatedButton(
            onPressed: () async {
              if (titleController.text.isNotEmpty) {
                final vm = context.read<TripViewModel>();
                final success = await vm.createTrip(
                  titleController.text,
                  DateTime.now(),
                  DateTime.now().add(const Duration(days: 3)),
                  [],
                );
                if (success && context.mounted) {
                  Navigator.pop(context);
                }
              }
            },
            child: const Text('Create'),
          )
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<TripViewModel>();

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Trips', style: TextStyle(fontWeight: FontWeight.bold)),
      ),
      body: _buildContent(viewModel),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _showCreateTripDialog,
        icon: const Icon(Icons.add),
        label: const Text('New Trip'),
        backgroundColor: Theme.of(context).primaryColor,
      ),
    );
  }

  Widget _buildContent(TripViewModel viewModel) {
    if (viewModel.state == TripState.loading && viewModel.trips.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (viewModel.state == TripState.error && viewModel.trips.isEmpty) {
      return Center(
        child: Text(viewModel.errorMessage ?? 'An error occurred', style: const TextStyle(color: Colors.red)),
      );
    }

    if (viewModel.trips.isEmpty) {
      return const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.flight_takeoff, size: 80, color: Colors.grey),
            SizedBox(height: 16),
            Text('No trips planned yet.', style: TextStyle(fontSize: 18, color: Colors.grey)),
            SizedBox(height: 8),
            Text('Tap the + button to start planning!'),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.all(16),
      itemCount: viewModel.trips.length,
      itemBuilder: (context, index) {
        final trip = viewModel.trips[index];
        return Card(
          elevation: 3,
          margin: const EdgeInsets.only(bottom: 16),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          child: ListTile(
            contentPadding: const EdgeInsets.all(16),
            leading: CircleAvatar(
              backgroundColor: Theme.of(context).primaryColor.withOpacity(0.2),
              child: Icon(Icons.map, color: Theme.of(context).primaryColor),
            ),
            title: Text(trip.title, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18)),
            subtitle: Padding(
              padding: const EdgeInsets.only(top: 8.0),
              child: Text(
                '${DateFormat('MMM d').format(trip.startDate)} - ${DateFormat('MMM d, yyyy').format(trip.endDate)}\n${trip.plannedPlaces.length} places planned',
                style: const TextStyle(height: 1.5),
              ),
            ),
            trailing: const Icon(Icons.chevron_right),
            onTap: () {
              // Navigate to trip details
            },
          ),
        );
      },
    );
  }
}
