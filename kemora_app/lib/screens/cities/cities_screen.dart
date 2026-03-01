import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../models/country.dart';
// import '../../models/city.dart'; // Not strictly needed if we access via country.cities

class CitiesScreen extends StatelessWidget {
  final Country country;
  const CitiesScreen({super.key, required this.country});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text("${country.name} Cities")),
      body: country.cities.isEmpty
          ? const Center(child: Text("No cities found for this country."))
          : ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: country.cities.length,
              itemBuilder: (context, index) {
                final city = country.cities[index];
                return Card(
                  margin: const EdgeInsets.only(bottom: 16),
                  elevation: 2,
                  child: ListTile(
                    contentPadding: const EdgeInsets.all(16),
                    leading: CircleAvatar(
                      radius: 30,
                      backgroundImage: NetworkImage(city.imageUrl),
                      onBackgroundImageError: (_, __) {},
                      child: const Icon(Icons.location_city),
                    ),
                    title: Text(
                      city.name,
                      style: const TextStyle(fontWeight: FontWeight.bold),
                    ),
                    subtitle: Text(
                      city.description,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                    trailing: const Icon(Icons.arrow_forward_ios, size: 16),
                    onTap: () {
                      context.go(
                        '/countries/details/cities/places',
                        extra: {'country': country, 'city': city},
                      );
                    },
                  ),
                );
              },
            ),
    );
  }
}
