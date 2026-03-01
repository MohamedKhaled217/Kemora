import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../viewmodels/places_view_model.dart';
import 'place_detail_screen.dart';

class PlacesScreen extends StatefulWidget {
  const PlacesScreen({super.key});

  @override
  State<PlacesScreen> createState() => _PlacesScreenState();
}

class _PlacesScreenState extends State<PlacesScreen> {
  final List<String> categories = ['All', 'Pyramids', 'Temples', 'Museums', 'Parks'];

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<PlacesViewModel>().loadPlaces();
    });
  }

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<PlacesViewModel>();

    return Scaffold(
      appBar: AppBar(
        title: const Text('Explore Egypt', style: TextStyle(fontWeight: FontWeight.bold)),
      ),
      body: Column(
        children: [
          // Category Selector
          SizedBox(
            height: 60,
            child: ListView.builder(
              scrollDirection: Axis.horizontal,
              itemCount: categories.length,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              itemBuilder: (context, index) {
                final category = categories[index];
                final isSelected = category == viewModel.currentCategory;
                return Padding(
                  padding: const EdgeInsets.only(right: 8.0),
                  child: ChoiceChip(
                    label: Text(category),
                    selected: isSelected,
                    onSelected: (selected) {
                      if (selected) {
                        viewModel.loadPlaces(category);
                      }
                    },
                    selectedColor: Theme.of(context).primaryColor,
                    labelStyle: TextStyle(
                      color: isSelected ? Colors.white : Colors.black87,
                      fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                    ),
                  ),
                );
              },
            ),
          ),
          
          // Content Area
          Expanded(
            child: _buildContent(viewModel),
          ),
        ],
      ),
    );
  }

  Widget _buildContent(PlacesViewModel viewModel) {
    if (viewModel.state == PlacesState.loading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (viewModel.state == PlacesState.error) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.error_outline, size: 64, color: Colors.redAccent),
            const SizedBox(height: 16),
            Text(viewModel.errorMessage ?? 'An error occurred', style: const TextStyle(fontSize: 16)),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: () => viewModel.loadPlaces(viewModel.currentCategory),
              child: const Text('Retry'),
            )
          ],
        ),
      );
    }

    if (viewModel.places.isEmpty) {
      return const Center(child: Text('No places found in this category.'));
    }

    return ListView.builder(
      padding: const EdgeInsets.all(16),
      itemCount: viewModel.places.length,
      itemBuilder: (context, index) {
        final place = viewModel.places[index];
        return Card(
          elevation: 4,
          margin: const EdgeInsets.only(bottom: 16),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          child: InkWell(
            onTap: () {
              Navigator.of(context).push(
                MaterialPageRoute(
                  builder: (context) => PlaceDetailScreen(place: place),
                ),
              );
            },
            borderRadius: BorderRadius.circular(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                ClipRRect(
                  borderRadius: const BorderRadius.vertical(top: Radius.circular(16)),
                  child: Hero(
                    tag: 'place_image_${place.id}',
                    child: Image.network(
                      place.imageUrl,
                      height: 200,
                      fit: BoxFit.cover,
                      errorBuilder: (context, error, stackTrace) => Container(
                        height: 200,
                        color: Colors.grey[300],
                        child: const Icon(Icons.image_not_supported, size: 50, color: Colors.grey),
                      ),
                    ),
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Expanded(
                            child: Text(
                              place.name,
                              style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                          Row(
                            children: [
                              const Icon(Icons.star, color: Colors.amber, size: 20),
                              const SizedBox(width: 4),
                              Text(place.rating.toString(), style: const TextStyle(fontWeight: FontWeight.bold)),
                            ],
                          )
                        ],
                      ),
                      const SizedBox(height: 8),
                      Text(
                        place.category,
                        style: TextStyle(color: Theme.of(context).primaryColor, fontWeight: FontWeight.w600),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        place.description,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: const TextStyle(color: Colors.black54),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
