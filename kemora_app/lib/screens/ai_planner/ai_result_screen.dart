import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import '../../domain/entities/ai_itinerary.dart';
import '../../presentation/viewmodels/trip_view_model.dart';
import '../../presentation/widgets/itinerary_widgets.dart';
import '../../core/theme/app_theme.dart';

class AiResultScreen extends StatelessWidget {
  final AIItinerary itinerary;

  const AiResultScreen({super.key, required this.itinerary});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      body: CustomScrollView(
        slivers: [
          // Premium Hero Header
          SliverAppBar(
            expandedHeight: 200,
            pinned: true,
            stretch: true,
            backgroundColor: AppTheme.primaryBlue,
            flexibleSpace: FlexibleSpaceBar(
              title: Text(
                itinerary.title,
                style: const TextStyle(
                  color: AppTheme.primarySand,
                  fontWeight: FontWeight.bold,
                  fontSize: 16,
                  shadows: [Shadow(color: Colors.black45, blurRadius: 10)],
                ),
              ),
              background: Stack(
                fit: StackFit.expand,
                children: [
                   // Generic Egypt backdrop if destination image not available at top level
                  Image.network(
                    "https://images.unsplash.com/photo-1572252009286-268acec5ca0a?auto=format&fit=crop&w=1200",
                    fit: BoxFit.cover,
                  ),
                  Container(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                        colors: [
                          Colors.transparent,
                          AppTheme.primaryBlue.withValues(alpha: 0.8),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
            leading: IconButton(
              icon: const Icon(Icons.arrow_back),
              onPressed: () => context.pop(),
            ),
            actions: [
              IconButton(
                icon: const Icon(Icons.save_outlined),
                onPressed: () => _showSaveDialog(context),
              ),
            ],
          ),

          // Content List
          SliverPadding(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 24),
            sliver: SliverList(
              delegate: SliverChildBuilderDelegate(
                (context, index) {
                  final day = itinerary.days[index];
                  return Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      DayHeader(day: day),
                      const SizedBox(height: 24),
                      ...day.activities.asMap().entries.map((entry) {
                        return ActivityTimelineTile(
                          activity: entry.value,
                          isLast: entry.key == day.activities.length - 1,
                        );
                      }),
                      const SizedBox(height: 32),
                    ],
                  );
                },
                childCount: itinerary.days.length,
              ),
            ),
          ),
          
          // Bottom CTA
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.all(24),
              child: ElevatedButton(
                onPressed: () => _showSaveDialog(context),
                style: ElevatedButton.styleFrom(
                  minimumSize: const Size(double.infinity, 56),
                ),
                child: const Text("CUSTOMIZE & SAVE PLAN"),
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _showSaveDialog(BuildContext context) {
    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      builder: (ctx) => Container(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              "Save to My Travels",
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 12),
            const Text("This will add this premium itinerary to your account for offline access and sharing."),
            const SizedBox(height: 24),
            ElevatedButton(
              onPressed: () async {
                final success = await context.read<TripViewModel>().savePlan(
                  DateTime.now(),
                  DateTime.now().add(Duration(days: itinerary.days.length)),
                );
                if (success && context.mounted) {
                  context.pop();
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(content: Text("Plan saved successfully!")),
                  );
                }
              },
              style: ElevatedButton.styleFrom(minimumSize: const Size(double.infinity, 52)),
              child: const Text("CONFIRM SAVE"),
            ),
          ],
        ),
      ),
    );
  }
}
