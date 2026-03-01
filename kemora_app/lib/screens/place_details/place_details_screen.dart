import 'package:flutter/material.dart';
import '../../models/place.dart';
import 'widgets/location_card.dart';
import 'widgets/ticket_price_card.dart';
import 'widgets/transport_card.dart';
import 'widgets/working_hours_card.dart';
import 'widgets/reviews_card.dart';

class PlaceDetailsScreen extends StatelessWidget {
  final Place place;
  const PlaceDetailsScreen({super.key, required this.place});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey.shade50,
      body: CustomScrollView(
        slivers: [
          SliverAppBar(
            expandedHeight: 300.0,
            floating: false,
            pinned: true,
            flexibleSpace: FlexibleSpaceBar(
              title: Text(
                place.name,
                style: const TextStyle(
                  color: Colors.white,
                  shadows: [Shadow(color: Colors.black, blurRadius: 2)],
                ),
              ),
              background: Stack(
                fit: StackFit.expand,
                children: [
                  Image.network(
                    place.imageUrl,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) => Container(color: Colors.grey),
                  ),
                  const DecoratedBox(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                        colors: [Colors.transparent, Colors.black54],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
          SliverPadding(
            padding: const EdgeInsets.all(16.0),
            sliver: SliverList(
              delegate: SliverChildListDelegate([
                // Details Section
                Card(
                  elevation: 0,
                  color: Colors.white,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(16),
                    side: const BorderSide(color: Colors.black12),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(16.0),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          "Details",
                          style: TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 18,
                          ),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          place.description,
                          style: const TextStyle(
                            color: Colors.grey,
                            height: 1.5,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
                const SizedBox(height: 16),

                // Location
                LocationCard(
                  address: place.address.isEmpty
                      ? "123 Main Street, Sharm El Sheikh"
                      : place.address,
                ), // Fallback for now
                const SizedBox(height: 16),

                // Transport
                TransportCard(
                  transportInfo: place.transportInfo.isEmpty
                      ? [
                          "Bus Lines: 12, 24, 56",
                          "Metro: Blue Line (Central Station)",
                          "Taxi: ~15 EGP from city center",
                        ]
                      : place.transportInfo,
                ),
                const SizedBox(height: 16),

                // Ticket Price
                TicketPriceCard(
                  priceAdult: place.priceAdult == 0 ? 50 : place.priceAdult,
                  priceChild: place.priceChild == 0 ? 25 : place.priceChild,
                ),
                const SizedBox(height: 16),

                // Working Hours
                WorkingHoursCard(openingTime: place.openingTime),
                const SizedBox(height: 16),

                // Reviews
                ReviewsCard(reviews: place.reviews),
                const SizedBox(height: 32),
              ]),
            ),
          ),
        ],
      ),
    );
  }
}
