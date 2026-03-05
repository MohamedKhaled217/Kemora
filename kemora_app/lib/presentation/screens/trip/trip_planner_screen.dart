import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';
import '../../viewmodels/trip_view_model.dart';

import 'generate_ai_itinerary_screen.dart';

class TripPlannerScreen extends StatefulWidget {
  final String? preSelectedPlaceId;
  final String? preSelectedPlaceName;
  final double? preSelectedLat;
  final double? preSelectedLng;

  const TripPlannerScreen({
    super.key,
    this.preSelectedPlaceId,
    this.preSelectedPlaceName,
    this.preSelectedLat,
    this.preSelectedLng,
  });

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
      backgroundColor: const Color(0xFFF8F9FA),
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0,
        title: const Text('Plan Your Trip', style: TextStyle(color: Colors.black, fontWeight: FontWeight.bold)),
        iconTheme: const IconThemeData(color: Colors.black),
      ),
      body: Column(
        children: [
          _buildAiPromotionCard(),
          Expanded(child: _buildContent(viewModel)),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _showCreateTripDialog,
        icon: const Icon(Icons.add),
        label: const Text('New Manual Trip'),
        backgroundColor: const Color(0xFF1A1A1A),
        foregroundColor: Colors.white,
      ),
    );
  }

  Widget _buildAiPromotionCard() {
    return Container(
      margin: const EdgeInsets.all(20),
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        gradient: const LinearGradient(colors: [Color(0xFF1A1A1A), Color(0xFF373737)]),
        borderRadius: BorderRadius.circular(24),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Row(
            children: [
              Icon(Icons.auto_awesome, color: Color(0xFFC5A358)),
              SizedBox(width: 8),
              Text('AI Trip Planner', style: TextStyle(color: Colors.white, fontSize: 18, fontWeight: FontWeight.bold)),
            ],
          ),
          const SizedBox(height: 12),
          const Text(
            'Let our AI handle the logistics based on your interests and travel style.',
            style: TextStyle(color: Colors.white70, height: 1.4),
          ),
          const SizedBox(height: 20),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) => GenerateAIItineraryScreen(
                      preSelectedPlaceId: widget.preSelectedPlaceId,
                      preSelectedPlaceName: widget.preSelectedPlaceName,
                      preSelectedLat: widget.preSelectedLat,
                      preSelectedLng: widget.preSelectedLng,
                    ),
                  ),
                );
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFFC5A358),
                foregroundColor: Colors.white,
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                padding: const EdgeInsets.symmetric(vertical: 14),
              ),
              child: Text(widget.preSelectedPlaceName != null ? 'Plan around ${widget.preSelectedPlaceName}' : 'Generate AI Plan'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildContent(TripViewModel viewModel) {
    if (viewModel.state == TripState.loading && viewModel.trips.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (viewModel.trips.isEmpty) {
      return const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.map_outlined, size: 60, color: Colors.grey),
            SizedBox(height: 16),
            Text('No manual trips yet.', style: TextStyle(color: Colors.grey, fontSize: 16)),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.symmetric(horizontal: 20),
      itemCount: viewModel.trips.length,
      itemBuilder: (context, index) {
        final trip = viewModel.trips[index];
        return Container(
          margin: const EdgeInsets.only(bottom: 16),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(16),
            boxShadow: [BoxShadow(color: Colors.black.withOpacity(0.02), blurRadius: 10, offset: const Offset(0, 4))],
          ),
          child: ListTile(
            contentPadding: const EdgeInsets.all(16),
            title: Text(trip.title, style: const TextStyle(fontWeight: FontWeight.bold)),
            subtitle: Text('${DateFormat('MMM d').format(trip.startDate)} - ${DateFormat('MMM d').format(trip.endDate)}'),
            trailing: const Icon(Icons.chevron_right),
            onTap: () {},
          ),
        );
      },
    );
  }
}
