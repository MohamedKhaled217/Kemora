import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';
import '../../domain/entities/trip_plan_request.dart';
import '../../domain/enums/tourism_type.dart';
import '../../presentation/viewmodels/trip_view_model.dart';
import '../../presentation/viewmodels/places_view_model.dart';
import '../../core/theme/app_theme.dart';

class AiFormScreen extends StatefulWidget {
  const AiFormScreen({super.key});

  @override
  State<AiFormScreen> createState() => _AiFormScreenState();
}

class _AiFormScreenState extends State<AiFormScreen> {
  String? _selectedGovernorate;
  String _selectedBudget = 'Mid-Range';
  double _duration = 3.0;
  final List<TourismType> _selectedInterests = [];

  final List<String> _budgets = ['Budget', 'Mid-Range', 'Luxury'];
  
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<PlacesViewModel>().getGovernorates();
    });
  }

  Future<void> _generatePlan() async {
    if (_selectedGovernorate == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please select a destination')),
      );
      return;
    }

    final request = TripPlanRequest(
      location: _selectedGovernorate!,
      durationDays: _duration.round(),
      budget: _selectedBudget,
      interests: _selectedInterests,
      preferences: "A premium cultural and comfortable experience",
    );

    final tripVM = context.read<TripViewModel>();
    await tripVM.generateAiItinerary(request);

    if (mounted) {
      if (tripVM.state == TripState.loaded && tripVM.currentPlan != null) {
        context.push('/map/ai-planner/result', extra: tripVM.currentPlan);
      } else if (tripVM.state == TripState.error) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(tripVM.errorMessage ?? 'Failed to generate plan')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('AI Trip Planner'),
        backgroundColor: AppTheme.primaryBlue,
        foregroundColor: AppTheme.primarySand,
      ),
      body: Consumer<PlacesViewModel>(
        builder: (context, placesVM, child) {
          return SingleChildScrollView(
            padding: const EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Center(
                  child: Icon(Icons.auto_awesome, size: 64, color: AppTheme.primaryGold),
                ),
                const SizedBox(height: 16),
                const Text(
                  "Design Your Elite Egyptian Experience",
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.primaryBlue,
                  ),
                ),
                const SizedBox(height: 32),

                // Destination
                _buildLabel("Destination Governorate"),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(color: AppTheme.primarySand),
                  ),
                  child: DropdownButtonHideUnderline(
                    child: DropdownButton<String>(
                      value: _selectedGovernorate,
                      isExpanded: true,
                      hint: const Text("Select a governorate"),
                      items: placesVM.governorates.map((gov) {
                        return DropdownMenuItem(
                          value: gov.name,
                          child: Text(gov.name),
                        );
                      }).toList(),
                      onChanged: (val) => setState(() => _selectedGovernorate = val),
                    ),
                  ),
                ),
                const SizedBox(height: 24),

                // Duration
                _buildLabel("Duration: ${_duration.round()} Days"),
                Slider(
                  value: _duration,
                  min: 1,
                  max: 7,
                  divisions: 6,
                  activeColor: AppTheme.primaryGold,
                  inactiveColor: AppTheme.primarySand,
                  onChanged: (val) => setState(() => _duration = val),
                ),
                const SizedBox(height: 24),

                // Budget
                _buildLabel("Budget Tier"),
                Wrap(
                  spacing: 12,
                  children: _budgets.map((b) {
                    final sel = _selectedBudget == b;
                    return ChoiceChip(
                      label: Text(b),
                      selected: sel,
                      selectedColor: AppTheme.primaryGold.withValues(alpha: 0.2),
                      onSelected: (s) => setState(() => _selectedBudget = b),
                    );
                  }).toList(),
                ),
                const SizedBox(height: 24),

                // Interests
                _buildLabel("Interests"),
                Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: TourismType.values.map((type) {
                    final sel = _selectedInterests.contains(type);
                    return FilterChip(
                      label: Text(type.toString().split('.').last),
                      selected: sel,
                      selectedColor: AppTheme.accentOasis.withValues(alpha: 0.2),
                      onSelected: (s) {
                        setState(() {
                          s ? _selectedInterests.add(type) : _selectedInterests.remove(type);
                        });
                      },
                    );
                  }).toList(),
                ),
                
                const SizedBox(height: 48),

                Consumer<TripViewModel>(
                  builder: (context, tripVM, _) {
                    final isLoading = tripVM.state == TripState.loading;
                    return SizedBox(
                      width: double.infinity,
                      child: ElevatedButton(
                        onPressed: isLoading ? null : _generatePlan,
                        child: isLoading 
                          ? const CircularProgressIndicator(color: AppTheme.primaryBlue)
                          : const Text("GENERATE PREMIUM ITINERARY"),
                      ),
                    );
                  },
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildLabel(String text) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Text(
        text,
        style: const TextStyle(
          fontSize: 16,
          fontWeight: FontWeight.bold,
          color: AppTheme.primaryBlue,
        ),
      ),
    );
  }
}
