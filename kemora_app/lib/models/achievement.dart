class Achievement {
  final String id;
  final String title;
  final String description;
  final String
  iconAsset; // Or use IconData if prefered, but asset is more flexible
  final bool isUnlocked;

  Achievement({
    required this.id,
    required this.title,
    required this.description,
    required this.iconAsset,
    this.isUnlocked = true,
  });
}
