import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../models/city.dart';
import '../../services/mock_data_service.dart';
import '../../services/ai_trip_service.dart';

class AiFormScreen extends StatefulWidget {
  const AiFormScreen({super.key});

  @override
  State<AiFormScreen> createState() => _AiFormScreenState();
}

class _AiFormScreenState extends State<AiFormScreen> {
  // Form State
  String? _selectedCityId;
  String _selectedBudget = 'Standard';
  double _duration = 3.0; // Days
  final List<String> _selectedInterests = [];

  final List<String> _budgets = ['Budget', 'Standard', 'Luxury'];
  final List<String> _interests = [
    'History',
    'Food',
    'Nature',
    'Shopping',
    'Relaxation',
    'Nightlife',
  ];

  late List<City> _availableCities;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    // For demo, just picking Egypt's cities
    final country = MockDataService.getCountries().firstWhere(
      (c) => c.name == 'Egypt',
    );
    _availableCities = country.cities;
    if (_availableCities.isNotEmpty) {
      _selectedCityId = _availableCities.first.id;
    }
  }

  Future<void> _generatePlan() async {
    if (_selectedCityId == null) return;

    setState(() {
      _isLoading = true;
    });

    final cityName = _availableCities
        .firstWhere((c) => c.id == _selectedCityId)
        .name;

    try {
      final tripPlan = await AiTripService.generateTrip(
        destination: cityName,
        days: _duration.round(),
        budget: _selectedBudget,
        interests: _selectedInterests,
      );

      if (mounted) {
        context.push('/map/ai-planner/result', extra: tripPlan);
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text('Failed to generate plan: $e')));
      }
    } finally {
      if (mounted) {
        setState(() {
          _isLoading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        title: const Text('AI Trip Planner'),
        centerTitle: true,
        backgroundColor: Colors.white,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Colors.black),
          onPressed: () => context.pop(),
        ),
        titleTextStyle: const TextStyle(
          color: Colors.black,
          fontWeight: FontWeight.bold,
          fontSize: 18,
        ),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header
            const Center(
              child: Icon(
                Icons.auto_awesome,
                size: 60,
                color: Colors.deepPurple,
              ),
            ),
            const SizedBox(height: 16),
            const Text(
              "Tell us your preferences, and we'll craft the perfect trip for you.",
              textAlign: TextAlign.center,
              style: TextStyle(color: Colors.grey, fontSize: 16),
            ),
            const SizedBox(height: 32),

            // Destination Dropdown
            _buildSectionLabel("Where do you want to go?"),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              decoration: BoxDecoration(
                color: Colors.grey[100],
                borderRadius: BorderRadius.circular(12),
              ),
              child: DropdownButtonHideUnderline(
                child: DropdownButton<String>(
                  value: _selectedCityId,
                  isExpanded: true,
                  items: _availableCities.map((city) {
                    return DropdownMenuItem(
                      value: city.id,
                      child: Text(city.name),
                    );
                  }).toList(),
                  onChanged: (val) {
                    setState(() {
                      _selectedCityId = val;
                    });
                  },
                ),
              ),
            ),
            const SizedBox(height: 24),

            // Duration Slider
            _buildSectionLabel("How long is your trip? (Days)"),
            Row(
              children: [
                Text("1 Day", style: TextStyle(color: Colors.grey[600])),
                Expanded(
                  child: Slider(
                    value: _duration,
                    min: 1,
                    max: 14,
                    divisions: 13,
                    label: "${_duration.round()} Days",
                    activeColor: Colors.deepPurple,
                    onChanged: (val) {
                      setState(() {
                        _duration = val;
                      });
                    },
                  ),
                ),
                Text("14 Days", style: TextStyle(color: Colors.grey[600])),
              ],
            ),
            Center(
              child: Text(
                "${_duration.round()} Days",
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  fontSize: 16,
                  color: Colors.deepPurple,
                ),
              ),
            ),
            const SizedBox(height: 24),

            // Budget Segmented
            _buildSectionLabel("What is your budget?"),
            Wrap(
              spacing: 12,
              children: _budgets.map((budget) {
                final isSelected = _selectedBudget == budget;
                return ChoiceChip(
                  label: Text(budget),
                  selected: isSelected,
                  selectedColor: Colors.deepPurple.withOpacity(0.2),
                  labelStyle: TextStyle(
                    color: isSelected ? Colors.deepPurple : Colors.black87,
                    fontWeight: isSelected
                        ? FontWeight.bold
                        : FontWeight.normal,
                  ),
                  onSelected: (selected) {
                    if (selected) {
                      setState(() {
                        _selectedBudget = budget;
                      });
                    }
                  },
                );
              }).toList(),
            ),
            const SizedBox(height: 24),

            // Interests Multi-select
            _buildSectionLabel("What are you interested in?"),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: _interests.map((interest) {
                final isSelected = _selectedInterests.contains(interest);
                return FilterChip(
                  label: Text(interest),
                  selected: isSelected,
                  selectedColor: Colors.deepPurple.withOpacity(0.2),
                  checkmarkColor: Colors.deepPurple,
                  labelStyle: TextStyle(
                    color: isSelected ? Colors.deepPurple : Colors.black87,
                  ),
                  onSelected: (selected) {
                    setState(() {
                      if (selected) {
                        _selectedInterests.add(interest);
                      } else {
                        _selectedInterests.remove(interest);
                      }
                    });
                  },
                );
              }).toList(),
            ),
            const SizedBox(height: 40),

            // Submit Button
            SizedBox(
              width: double.infinity,
              height: 56,
              child: ElevatedButton(
                onPressed: _isLoading ? null : _generatePlan,
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.deepPurple,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(16),
                  ),
                  elevation: 5,
                ),
                child: _isLoading
                    ? const CircularProgressIndicator(color: Colors.white)
                    : const Text(
                        "Generate My Trip",
                        style: TextStyle(
                          fontSize: 18,
                          color: Colors.white,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
              ),
            ),
            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Widget _buildSectionLabel(String label) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Text(
        label,
        style: const TextStyle(
          fontSize: 16,
          fontWeight: FontWeight.bold,
          color: Colors.black87,
        ),
      ),
    );
  }
}
