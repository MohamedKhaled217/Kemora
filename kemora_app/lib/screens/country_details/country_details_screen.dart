import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../models/country.dart';

class CountryDetailsScreen extends StatelessWidget {
  final Country country;
  const CountryDetailsScreen({super.key, required this.country});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: CustomScrollView(
        slivers: [
          SliverAppBar(
            expandedHeight: 250.0,
            floating: false,
            pinned: true,
            flexibleSpace: FlexibleSpaceBar(
              title: Text(country.name),
              background: Image.network(
                country.imageUrl,
                fit: BoxFit.cover,
                errorBuilder: (context, error, stackTrace) => Container(color: Colors.grey),
              ),
            ),
          ),
          SliverPadding(
            padding: const EdgeInsets.all(16.0),
            sliver: SliverList(
              delegate: SliverChildListDelegate([
                 Text(
                  "About ${country.name}",
                  style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 16),
                Text(
                  country.description,
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
                const SizedBox(height: 32),
                ElevatedButton.icon(
                  onPressed: () {
                     context.go('/countries/details/cities', extra: country);
                  },
                  icon: const Icon(Icons.location_city),
                  label: const Text("Explore Cities"),
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(vertical: 16),
                    textStyle: const TextStyle(fontSize: 18),
                  ),
                ),
              ]),
            ),
          ),
        ],
      ),
    );
  }
}
