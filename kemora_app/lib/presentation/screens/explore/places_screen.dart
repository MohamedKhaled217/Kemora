import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../viewmodels/places_view_model.dart';
import '../../../domain/entities/place.dart';
import '../trip/trip_planner_screen.dart';
import 'place_detail_screen.dart';
import 'governorate_places_screen.dart';

class PlacesScreen extends StatefulWidget {
  const PlacesScreen({super.key});

  @override
  State<PlacesScreen> createState() => _PlacesScreenState();
}

class _PlacesScreenState extends State<PlacesScreen> {
  final List<String> categories = ['All', 'Historical', 'Beach', 'Nature', 'Museums'];

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final vm = context.read<PlacesViewModel>();
      vm.loadPlaces();
      vm.loadTopPlaces();
      vm.loadGovernorates();
    });
  }

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<PlacesViewModel>();

    return Scaffold(
      backgroundColor: const Color(0xFFF8F9FA),
      body: CustomScrollView(
        slivers: [
          _buildAppBar(),
          SliverToBoxAdapter(child: _buildHeroCarousel(viewModel)),
          SliverToBoxAdapter(child: _buildSectionHeader('Explore by Governorate', () => _showAllGovernorates(context, viewModel))),
          SliverToBoxAdapter(child: _buildGovernorateList(viewModel)),
          SliverToBoxAdapter(child: _buildSectionHeader('Popular Categories', () => _showAllCategories(context, viewModel))),
          SliverToBoxAdapter(child: _buildCategorySelector(viewModel)),
          SliverPadding(
            padding: const EdgeInsets.symmetric(horizontal: 20),
            sliver: _buildPlacesList(viewModel),
          ),
          const SliverToBoxAdapter(child: SizedBox(height: 30)),
        ],
      ),
    );
  }

  Widget _buildAppBar() {
    return SliverAppBar(
      expandedHeight: 0,
      floating: true,
      backgroundColor: Colors.white,
      elevation: 0,
      title: const Text(
        'Explore Egypt',
        style: TextStyle(color: Colors.black, fontWeight: FontWeight.w900, fontSize: 24, letterSpacing: -0.5),
      ),
      actions: [
        IconButton(
          onPressed: () {},
          icon: const Icon(Icons.search, color: Colors.black),
        ),
        const SizedBox(width: 8),
      ],
    );
  }

  Widget _buildSectionHeader(String title, VoidCallback? onSeeAll) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 24, 20, 12),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(title, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Color(0xFF1A1A1A))),
          if (onSeeAll != null)
            TextButton(onPressed: onSeeAll, child: const Text('See All', style: TextStyle(color: Color(0xFFC5A358)))),
        ],
      ),
    );
  }

  void _showAllGovernorates(BuildContext context, PlacesViewModel viewModel) {
    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(20))),
      isScrollControlled: true,
      builder: (context) => DraggableScrollableSheet(
        initialChildSize: 0.6,
        expand: false,
        builder: (context, controller) => ListView.builder(
          controller: controller,
          padding: const EdgeInsets.all(16),
          itemCount: viewModel.governorates.length,
          itemBuilder: (context, index) {
            final gov = viewModel.governorates[index];
            return ListTile(
              leading: CircleAvatar(backgroundImage: NetworkImage(gov.imageUrl ?? 'https://via.placeholder.com/150')),
              title: Text(gov.name, style: const TextStyle(fontWeight: FontWeight.bold)),
              trailing: const Icon(Icons.arrow_forward_ios, size: 16),
              onTap: () {
                Navigator.pop(context);
                Navigator.push(context, MaterialPageRoute(builder: (_) => GovernoratePlacesScreen(governorate: gov)));
              },
            );
          },
        ),
      ),
    );
  }

  void _showAllCategories(BuildContext context, PlacesViewModel viewModel) {
    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(20))),
      builder: (context) => Padding(
        padding: const EdgeInsets.all(16),
        child: Wrap(
          spacing: 8,
          runSpacing: 8,
          children: categories.map((cat) {
            final isSelected = cat == viewModel.currentCategory;
            return ChoiceChip(
              label: Text(cat),
              selected: isSelected,
              onSelected: (val) {
                viewModel.loadPlaces(cat);
                Navigator.pop(context);
              },
              selectedColor: const Color(0xFF1A1A1A),
              labelStyle: TextStyle(
                color: isSelected ? Colors.white : Colors.black87,
                fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
              ),
            );
          }).toList(),
        ),
      ),
    );
  }

  Widget _buildHeroCarousel(PlacesViewModel viewModel) {
    if (viewModel.topPlaces.isEmpty) return const SizedBox.shrink();

    return SizedBox(
      height: 280,
      child: ListView.builder(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 16),
        itemCount: viewModel.topPlaces.length,
        itemBuilder: (context, index) {
          final place = viewModel.topPlaces[index];
          return GestureDetector(
            onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => PlaceDetailScreen(place: place))),
            child: Container(
              width: 320,
              margin: const EdgeInsets.only(right: 16),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(24),
                image: DecorationImage(
                image: NetworkImage(place.imageUrl),
                  fit: BoxFit.cover,
                  onError: (error, stackTrace) {}, // Avoid crashing if missing
                ),
              ),
              child: Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(24),
                  gradient: LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    colors: [Colors.transparent, Colors.black.withOpacity(0.8)],
                  ),
                ),
                padding: const EdgeInsets.all(20),
                child: Stack(
                  children: [
                    Positioned(
                      bottom: 0,
                      left: 0,
                      right: 0,
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.end,
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                            decoration: BoxDecoration(color: const Color(0xFFC5A358), borderRadius: BorderRadius.circular(20)),
                            child: const Text('FEATURED', style: TextStyle(color: Colors.white, fontSize: 10, fontWeight: FontWeight.bold)),
                          ),
                          const SizedBox(height: 8),
                          Text(place.name, style: const TextStyle(color: Colors.white, fontSize: 22, fontWeight: FontWeight.bold)),
                          const SizedBox(height: 4),
                          Row(
                            children: [
                              const Icon(Icons.location_on, color: Colors.white70, size: 14),
                              const SizedBox(width: 4),
                              Text(place.category, style: const TextStyle(color: Colors.white70, fontSize: 14)),
                              const Spacer(),
                              const Icon(Icons.star, color: Color(0xFFC5A358), size: 16),
                              const SizedBox(width: 4),
                              Text(place.rating.toString(), style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
                            ],
                          ),
                        ],
                      ),
                    ),
                    Positioned(
                      top: 12,
                      right: 12,
                      child: _buildPlanButton(context, place),
                    ),
                  ],
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildGovernorateList(PlacesViewModel viewModel) {
    if (viewModel.governorates.isEmpty) return const SizedBox.shrink();

    return SizedBox(
      height: 120,
      child: ListView.builder(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 16),
        itemCount: viewModel.governorates.length,
        itemBuilder: (context, index) {
          final gov = viewModel.governorates[index];
          return GestureDetector(
            onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => GovernoratePlacesScreen(governorate: gov))),
            child: Container(
              width: 100,
              margin: const EdgeInsets.only(right: 12),
              child: Column(
                children: [
                  CircleAvatar(
                    radius: 35,
                    backgroundImage: NetworkImage(gov.imageUrl ?? 'https://via.placeholder.com/150'),
                    backgroundColor: Colors.grey[200],
                  ),
                  const SizedBox(height: 8),
                  Text(gov.name, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600), textAlign: TextAlign.center, maxLines: 1),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildCategorySelector(PlacesViewModel viewModel) {
    return SizedBox(
      height: 50,
      child: ListView.builder(
        scrollDirection: Axis.horizontal,
        itemCount: categories.length,
        padding: const EdgeInsets.symmetric(horizontal: 16),
        itemBuilder: (context, index) {
          final category = categories[index];
          final isSelected = category == viewModel.currentCategory;
          return Padding(
            padding: const EdgeInsets.only(right: 8),
            child: ChoiceChip(
              label: Text(category),
              selected: isSelected,
              onSelected: (selected) => viewModel.loadPlaces(category),
              backgroundColor: Colors.white,
              selectedColor: const Color(0xFF1A1A1A),
              labelStyle: TextStyle(
                color: isSelected ? Colors.white : Colors.black87,
                fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
              ),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              side: BorderSide(color: isSelected ? Colors.transparent : Colors.grey[300]!),
            ),
          );
        },
      ),
    );
  }

  Widget _buildPlacesList(PlacesViewModel viewModel) {
    if (viewModel.state == PlacesState.loading && viewModel.places.isEmpty) {
      return const SliverFillRemaining(child: Center(child: CircularProgressIndicator(color: Color(0xFFC5A358))));
    }

    if (viewModel.state == PlacesState.error && viewModel.places.isEmpty) {
      return SliverFillRemaining(
        child: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.error_outline, size: 60, color: Colors.grey),
              const SizedBox(height: 16),
              Text(viewModel.errorMessage ?? 'Failed to load places', style: const TextStyle(color: Colors.grey)),
            ],
          ),
        ),
      );
    }

    return SliverList(
      delegate: SliverChildBuilderDelegate(
        (context, index) {
          final place = viewModel.places[index];
          return Container(
            margin: const EdgeInsets.only(bottom: 20),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(20),
              boxShadow: [BoxShadow(color: Colors.black.withOpacity(0.03), blurRadius: 10, offset: const Offset(0, 4))],
            ),
            child: Column(
              children: [
                ClipRRect(
                  borderRadius: const BorderRadius.vertical(top: Radius.circular(20)),
                  child: Image.network(
                    place.imageUrl,
                    height: 180,
                    width: double.infinity,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) => Container(
                      height: 180,
                      width: double.infinity,
                      color: Colors.grey[200],
                      child: const Center(child: Icon(Icons.image_not_supported, color: Colors.grey, size: 40)),
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
                          Text(place.name, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                          Row(
                            children: [
                              const Icon(Icons.star, color: Color(0xFFC5A358), size: 16),
                              const SizedBox(width: 4),
                              Text(place.rating.toString(), style: const TextStyle(fontWeight: FontWeight.bold)),
                            ],
                          ),
                        ],
                      ),
                      const SizedBox(height: 4),
                      Text(place.category, style: TextStyle(color: Colors.grey[600], fontSize: 13)),
                      const SizedBox(height: 12),
                      Row(
                        children: [
                          Expanded(
                            child: ElevatedButton(
                              onPressed: () => Navigator.push(context, MaterialPageRoute(builder: (_) => PlaceDetailScreen(place: place))),
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFFF8F9FA),
                                foregroundColor: Colors.black87,
                                elevation: 0,
                                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                              ),
                              child: const Text('Details'),
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: ElevatedButton(
                              onPressed: () {
                                Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                    builder: (_) => TripPlannerScreen(
                                      preSelectedPlaceId: place.id.toString(),
                                      preSelectedPlaceName: place.name,
                                      preSelectedLat: place.latitude,
                                      preSelectedLng: place.longitude,
                                    ),
                                  ),
                                );
                              },
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFF1A1A1A),
                                foregroundColor: Colors.white,
                                elevation: 0,
                                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                              ),
                              child: const Text('Plan Trip'),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
          );
        },
        childCount: viewModel.places.length,
      ),
    );
  }

  Widget _buildPlanButton(BuildContext context, Place place) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white.withAlpha(230), // Was 0.9 opacity
        shape: BoxShape.circle,
        boxShadow: [BoxShadow(color: Colors.black.withAlpha(25), blurRadius: 8)], // Was 0.1 opacity
      ),
      child: IconButton(
        icon: const Icon(Icons.auto_awesome, color: Color(0xFFC5A358), size: 20),
        onPressed: () {
          Navigator.push(
            context,
            MaterialPageRoute(
              builder: (_) => TripPlannerScreen(
                preSelectedPlaceId: place.id.toString(),
                preSelectedPlaceName: place.name,
                preSelectedLat: place.latitude,
                preSelectedLng: place.longitude,
              ),
            ),
          );
        },
        tooltip: 'Plan a trip here',
      ),
    );
  }
}
