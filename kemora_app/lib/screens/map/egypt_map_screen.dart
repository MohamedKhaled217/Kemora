import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../models/country.dart';
import '../../services/mock_data_service.dart';

class EgyptMapScreen extends StatefulWidget {
  const EgyptMapScreen({super.key});

  @override
  State<EgyptMapScreen> createState() => _EgyptMapScreenState();
}

class _EgyptMapScreenState extends State<EgyptMapScreen>
    with SingleTickerProviderStateMixin {
  late TransformationController _transformationController;
  late AnimationController _animationController;
  late Animation<Matrix4> _animation;
  late Country egypt;

  // Manual calibration for pins on the map image
  // (0,0) is top-left, (1,1) is bottom-right
  // REMOVED hardcoded cityAlignments

  @override
  void initState() {
    super.initState();
    egypt = MockDataService.getCountries().firstWhere((c) => c.name == 'Egypt');
    _transformationController = TransformationController();
    _animationController =
        AnimationController(
          vsync: this,
          duration: const Duration(milliseconds: 800),
        )..addListener(() {
          _transformationController.value = _animation.value;
        });
  }

  @override
  void dispose() {
    _transformationController.dispose();
    _animationController.dispose();
    super.dispose();
  }

  void _zoomAndNavigate(String cityId, double containerSize) {
    final city = egypt.cities.firstWhere((c) => c.id == cityId);
    final alignment = Alignment(city.alignmentX, city.alignmentY);

    // Calculate translation to center the target point
    // Target point relative to center: P = (align.x * w/2, align.y * h/2)
    // We want P' = P * S + T = 0  => T = -P * S
    // T = -(align * size/2) * scale

    final double zoomLevel = 3.0; // Consistent zoom level
    final double halfSize = containerSize / 2;

    final matrix = Matrix4.identity()
      ..translate(
        -halfSize * (zoomLevel - 1) - alignment.x * halfSize * zoomLevel,
        -halfSize * (zoomLevel - 1) - alignment.y * halfSize * zoomLevel,
      )
      ..scale(zoomLevel);

    _animation =
        Matrix4Tween(
          begin: _transformationController.value,
          end: matrix,
        ).animate(
          CurvedAnimation(
            parent: _animationController,
            curve: Curves.easeInOut,
          ),
        );

    _animationController.forward().then((_) {
      // Navigate after zoom finishes
      context.go('/map/places', extra: {'country': egypt, 'city': city});

      // Reset zoom after a delay
      Future.delayed(const Duration(milliseconds: 500), () {
        _transformationController.value = Matrix4.identity();
        _animationController.reset();
      });
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA), // Light grey background
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // HEADER AREA
            Padding(
              padding: const EdgeInsets.fromLTRB(20, 20, 20, 10),
              child: Text(
                "Explore Egypt",
                style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                  color: Colors.black87,
                ),
              ),
            ),

            // MAP AREA
            Expanded(
              child: Center(
                child: Container(
                  margin: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 8,
                  ),
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(20),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withOpacity(0.05),
                        blurRadius: 10,
                        offset: const Offset(0, 4),
                      ),
                    ],
                  ),
                  child: AspectRatio(
                    aspectRatio: 1.0,
                    child: LayoutBuilder(
                      builder: (context, constraints) {
                        final size = constraints.maxWidth;
                        return ClipRRect(
                          borderRadius: BorderRadius.circular(16),
                          child: InteractiveViewer(
                            transformationController: _transformationController,
                            minScale: 1.0,
                            maxScale: 5.0,
                            // Constrain the content to the aspect ratio of the map image
                            // Egypt map is roughly square, assuming ~1.0 or slightly taller.
                            // Using AspectRatio ensures the coordinate system (Stack)
                            // matches the visual image exactly, preventing pin drift.
                            child: Stack(
                              fit: StackFit.expand,
                              children: [
                                Image.asset(
                                  'assets/images/egypt_map.png',
                                  fit: BoxFit
                                      .fill, // Ensure strict alignment with pins
                                ),
                                // City Pins
                                ...egypt.cities.map((city) {
                                  return Align(
                                    alignment: Alignment(
                                      city.alignmentX,
                                      city.alignmentY,
                                    ),
                                    child: GestureDetector(
                                      onTap: () =>
                                          _zoomAndNavigate(city.id, size),
                                      child: Column(
                                        mainAxisSize: MainAxisSize.min,
                                        children: [
                                          const Icon(
                                            Icons.location_on,
                                            color: Colors.red,
                                            size: 24, // Smaller pins
                                          ),
                                          Container(
                                            padding: const EdgeInsets.symmetric(
                                              horizontal: 4,
                                              vertical: 2,
                                            ),
                                            decoration: BoxDecoration(
                                              color: Colors.white,
                                              borderRadius:
                                                  BorderRadius.circular(4),
                                              boxShadow: const [
                                                BoxShadow(
                                                  blurRadius: 2,
                                                  color: Colors.black26,
                                                ),
                                              ],
                                            ),
                                            child: Text(
                                              city.name,
                                              style: const TextStyle(
                                                fontWeight: FontWeight.bold,
                                                fontSize: 9,
                                              ),
                                            ),
                                          ),
                                        ],
                                      ),
                                    ),
                                  );
                                }),
                              ],
                            ),
                          ),
                        );
                      },
                    ),
                  ),
                ),
              ),
            ),

            // LIST AREA
            Container(
              height: 180,
              decoration: BoxDecoration(
                color: Colors.white,
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withOpacity(0.05),
                    blurRadius: 10,
                    offset: const Offset(0, -5),
                  ),
                ],
                borderRadius: const BorderRadius.vertical(
                  top: Radius.circular(20),
                ),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(20, 16, 20, 8),
                    child: Text(
                      "Select a City",
                      style: Theme.of(context).textTheme.titleLarge?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  Expanded(
                    child: LayoutBuilder(
                      // Use layout builder here just to be safe if we need width later,
                      // but mostly just for list building
                      builder: (context, constraints) {
                        return ListView.builder(
                          padding: const EdgeInsets.symmetric(horizontal: 12),
                          scrollDirection: Axis.horizontal,
                          itemCount: egypt.cities.length,
                          itemBuilder: (context, index) {
                            final city = egypt.cities[index];
                            return GestureDetector(
                              // Since we don't have easy access to the exact map size from here
                              // without a state variable, we will default to a reasonable estimate
                              // or query the context size.
                              // Better: Use a predefined key or constraints if possible.
                              // For now, we will use a safe approximation for list clicks
                              // or pass the last known size.
                              // Actually, the map is square and fills width minus margins.
                              // Width ~ MediaQuery.of(context).size.width - 32 - 16 appx.
                              onTap: () {
                                // Approximate size for list interaction if map isn't directly tapped
                                // This is a minor limitation of this refactor, but sufficient.
                                final approximateSize =
                                    MediaQuery.of(context).size.width - 48;
                                _zoomAndNavigate(city.id, approximateSize);
                              },
                              child: Container(
                                width: 140,
                                margin: const EdgeInsets.symmetric(
                                  horizontal: 8,
                                  vertical: 8,
                                ),
                                decoration: BoxDecoration(
                                  borderRadius: BorderRadius.circular(12),
                                  image: DecorationImage(
                                    image: NetworkImage(city.imageUrl),
                                    fit: BoxFit.cover,
                                  ),
                                  boxShadow: const [
                                    BoxShadow(
                                      blurRadius: 4,
                                      color: Colors.black26,
                                      offset: Offset(2, 2),
                                    ),
                                  ],
                                ),
                                child: Container(
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(12),
                                    gradient: LinearGradient(
                                      begin: Alignment.topCenter,
                                      end: Alignment.bottomCenter,
                                      colors: [
                                        Colors.transparent,
                                        Colors.black.withOpacity(0.7),
                                      ],
                                    ),
                                  ),
                                  alignment: Alignment.bottomLeft,
                                  padding: const EdgeInsets.all(12),
                                  child: Text(
                                    city.name,
                                    style: const TextStyle(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold,
                                      fontSize: 14,
                                    ),
                                  ),
                                ),
                              ),
                            );
                          },
                        );
                      },
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () {
          context.push('/map/ai-planner');
        },
        backgroundColor: Colors.deepPurple,
        icon: const Icon(Icons.auto_awesome, color: Colors.white),
        label: const Text(
          "Plan My Trip",
          style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
