import '../../domain/entities/badge.dart';

class BadgeModel extends Badge {
  const BadgeModel({
    required super.id,
    required super.name,
    required super.description,
    required super.iconUrl,
    required super.criteria,
  });

  factory BadgeModel.fromJson(Map<String, dynamic> json) {
    return BadgeModel(
      id: json['id']?.toString() ?? '',
      name: json['name'] as String? ?? 'Unknown Badge',
      description: json['description'] as String? ?? '',
      iconUrl: json['iconUrl'] as String? ?? 'https://via.placeholder.com/150',
      criteria: json['criteria'] as String? ?? '',
    );
  }
}

class UserBadgeModel extends UserBadge {
  const UserBadgeModel({
    required super.id,
    required super.userId,
    required super.badge,
    required super.earnedAt,
  });

  factory UserBadgeModel.fromJson(Map<String, dynamic> json) {
    return UserBadgeModel(
      id: json['id']?.toString() ?? '',
      userId: json['userId']?.toString() ?? '',
      badge: BadgeModel.fromJson(json['badge'] ?? {}),
      earnedAt: json['earnedAt'] != null ? DateTime.parse(json['earnedAt']) : DateTime.now(),
    );
  }
}
