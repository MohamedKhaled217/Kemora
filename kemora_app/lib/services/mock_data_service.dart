import '../models/country.dart';
import '../models/city.dart';
import '../models/place.dart';
import '../models/review.dart';
import '../models/post.dart';
import '../models/achievement.dart';

class MockDataService {
  static List<Country> getCountries() {
    return [
      Country(
        id: '1',
        name: 'Egypt',
        description: 'Explore the land of Pharaohs and ancient history.',
        imageUrl:
            'https://images.unsplash.com/photo-1553913861-c0fddf2619ee', // Cairo Tower/City
        cities: [
          City(
            id: 'c1',
            name: 'Cairo',
            description:
                'The capital of Egypt and the largest city in the Arab world.',
            imageUrl:
                'https://images.unsplash.com/photo-1572252009286-268acec5ca0a', // Pyramids view
            alignmentX: 0.12,
            alignmentY: -0.65,
            places: [
              Place(
                id: 'p1',
                name: 'Egyptian Museum',
                description:
                    'Home to an extensive collection of ancient Egyptian antiquities.',
                imageUrl:
                    'https://images.unsplash.com/photo-1548602088-9d12a4f9c10f',
                category: 'Museum',
                rating: 4.8,
                openingTime: '09:00 AM - 05:00 PM',
                latitude: 30.0478,
                longitude: 31.2336,
              ),
              Place(
                id: 'p2',
                name: 'Giza Pyramids',
                description:
                    'The last of the ancient Seven Wonders of the World.',
                imageUrl:
                    'https://images.unsplash.com/photo-1503177119275-0aa32b3a9368',
                category: 'Historic',
                rating: 4.9,
                openingTime: '08:00 AM - 04:00 PM',
                latitude: 29.9792,
                longitude: 31.1342,
                address: "Al Haram, Giza Governorate, Egypt",
                priceAdult: 200,
                priceChild: 100,
                transportInfo: [
                  "Bus Lines: 33, 44",
                  "Metro: Line 2 (Giza Station)",
                  "Taxi: Available",
                ],
                reviews: [
                  Review(
                    id: "r1",
                    userName: "Alice",
                    userImage: "",
                    rating: 5,
                    date: "2023-10-15",
                    comment: "Breathtaking views!",
                  ),
                  Review(
                    id: "r2",
                    userName: "Bob",
                    userImage: "",
                    rating: 4,
                    date: "2023-09-20",
                    comment: "Very crowded but worth it.",
                  ),
                ],
              ),
              Place(
                id: 'p3',
                name: 'Khan El Khalili',
                description:
                    'A famous bazaar and souq in the historic center of Cairo.',
                imageUrl:
                    'https://images.unsplash.com/photo-1590520621343-bd21d7b38d30',
                category: 'Market',
                rating: 4.7,
                openingTime: '10:00 AM - 11:00 PM',
                latitude: 30.0475,
                longitude: 31.2623,
              ),
            ],
          ),
          City(
            id: 'c2',
            name: 'Alexandria',
            description:
                'The Mediterranean port city known for its Greco-Roman landmarks.',
            imageUrl:
                'https://images.unsplash.com/photo-1588698947348-18eeb6343588',
            alignmentX: -0.2,
            alignmentY: -0.85,
            places: [],
          ),
          City(
            id: 'c3',
            name: 'Port Said',
            description:
                'A city that lies north of the Suez Canal on the Mediterranean Coast.',
            imageUrl:
                'https://images.unsplash.com/photo-1565535318621-0814421b5250',
            alignmentX: 0.3,
            alignmentY: -0.8,
            places: [],
          ),
          City(
            id: 'c4',
            name: 'Sharm El-Sheikh',
            description:
                'A resort town between the desert of the Sinai Peninsula and the Red Sea.',
            imageUrl:
                'https://images.unsplash.com/photo-1628087570404-b631185444b0',
            alignmentX: 0.6,
            alignmentY: -0.4,
            places: [
              Place(
                id: 'p4',
                name: 'Naama Bay',
                description:
                    'A natural bay and resort area known for its cafes and nightlife.',
                imageUrl:
                    'https://images.unsplash.com/photo-1606822393309-87a32d169223',
                category: 'Beach',
                rating: 4.7,
                openingTime: 'Open 24 Hours',
                latitude: 27.9158,
                longitude: 34.3299,
              ),
              Place(
                id: 'p5',
                name: 'Ras Mohammed Park',
                description:
                    'A national park at the southern extreme of the Sinai Peninsula.',
                imageUrl:
                    'https://images.unsplash.com/photo-1544551763-46a013bb70d5',
                category: 'Nature',
                rating: 4.9,
                openingTime: '07:00 AM - 04:00 PM',
                latitude: 27.7314,
                longitude: 34.2505,
              ),
            ],
          ),
          City(
            id: 'c5',
            name: 'Hurghada',
            description:
                'A beach resort town stretching some 40km along Egypt’s Red Sea coast.',
            imageUrl:
                'https://images.unsplash.com/photo-1598506143924-f7615951d38a',
            alignmentX: 0.5,
            alignmentY: -0.1,
            places: [],
          ),
          City(
            id: 'c6',
            name: 'Luxor',
            description:
                'A city on the east bank of the Nile River in southern Egypt.',
            imageUrl:
                'https://images.unsplash.com/photo-1568222687556-9d33b3b4d455',
            alignmentX: 0.25,
            alignmentY: 0.3,
            places: [],
          ),
          City(
            id: 'c7',
            name: 'Aswan',
            description:
                'A city on the Nile River, has distinctively significant archaeological sites.',
            imageUrl:
                'https://images.unsplash.com/photo-1539768942893-daf53e448371',
            alignmentX: 0.3,
            alignmentY: 0.6,
            places: [],
          ),
          City(
            id: 'c8',
            name: 'Siwa Oasis',
            description:
                'An urban oasis in Egypt between the Qattara Depression and the Great Sand Sea.',
            imageUrl:
                'https://images.unsplash.com/photo-1582234032644-32219b6736dd',
            alignmentX: -0.65,
            alignmentY: -0.3,
            places: [],
          ),
          City(
            id: 'c9',
            name: 'Dahab',
            description:
                'A small town on the southeast coast of the Sinai Peninsula in Egypt.',
            imageUrl:
                'https://images.unsplash.com/photo-1605364850020-038202974918',
            alignmentX: 0.62,
            alignmentY: -0.55,
            places: [],
          ),
          City(
            id: 'c10',
            name: 'Marsa Alam',
            description:
                'A town in south-eastern Egypt, known for its sandy beaches and coal reefs.',
            imageUrl:
                'https://images.unsplash.com/photo-1602435773295-201556942426',
            alignmentX: 0.55,
            alignmentY: 0.1,
            places: [],
          ),
        ],
      ),
    ];
  }

  static List<Post> getPosts() {
    return [
      Post(
        id: 'post1',
        userName: 'Sarah Jenkins',
        userImage:
            'https://images.unsplash.com/photo-1494790108377-be9c29b29330', // Woman portrait
        content:
            'Finally visited the Great Pyramids of Giza! Usually seen in books, but seeing them in person is magical. #Egypt #Travel',
        imageUrl:
            'https://images.unsplash.com/photo-1503177119275-0aa32b3a9368', // Pyramids
        likes: 1240,
        timeAgo: '2h ago',
        comments: [
          Comment(
            id: 'c1',
            userName: 'Ahmed Ali',
            userImage:
                'https://images.unsplash.com/photo-1500648767791-00dcc994a43e', // Man portrait
            text: 'Welcome to Egypt! Hope you enjoy your stay.',
          ),
          Comment(
            id: 'c2',
            userName: 'Jessica Lee',
            userImage:
                'https://images.unsplash.com/photo-1534528741775-53994a69daeb', // Woman portrait
            text: 'Amazing photos! added to my bucket list.',
          ),
        ],
      ),
      Post(
        id: 'post2',
        userName: 'Mike Ross',
        userImage:
            'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d', // Man portrait
        content:
            'Diving in the Red Sea was an unforgettable experience. The corals are so vibrant!',
        imageUrl:
            'https://images.unsplash.com/photo-1544551763-46a013bb70d5', // Underwater/Red Sea
        likes: 856,
        timeAgo: '5h ago',
        comments: [],
      ),
      Post(
        id: 'post3',
        userName: 'Elena Rodriguez',
        userImage:
            'https://images.unsplash.com/photo-1544005313-94ddf0286df2', // Woman portrait
        content:
            'Khan El Khalili market at night is a vibe. The lanterns are beautiful.',
        imageUrl:
            'https://images.unsplash.com/photo-1590520621343-bd21d7b38d30', // Khan El Khalili
        likes: 2103,
        timeAgo: '1d ago',
        comments: [
          Comment(
            id: 'c3',
            userName: 'Omar Hassan',
            userImage:
                'https://images.unsplash.com/photo-1506794778202-cad84cf45f1d', // Man portrait
            text: 'Did you try the tea at El Fishawy?',
          ),
        ],
      ),
    ];
  }

  static List<Achievement> getAchievements() {
    return [
      Achievement(
        id: 'a1',
        title: 'Pyramid Explorer',
        description: 'Visited the Great Pyramids of Giza.',
        iconAsset:
            'assets/icons/pyramid.png', // Placeholder, we will use IconData or just Emoji for now if assets missing
      ),
      Achievement(
        id: 'a2',
        title: 'Red Sea Diver',
        description: 'Completed a diving session in the Red Sea.',
        iconAsset: 'assets/icons/diving.png',
      ),
      Achievement(
        id: 'a3',
        title: 'History Buff',
        description: 'Visited 5 different museums in Egypt.',
        iconAsset: 'assets/icons/museum.png',
        isUnlocked: false,
      ),
      Achievement(
        id: 'a4',
        title: 'Market Master',
        description: 'Bought a souvenir from Khan El Khalili.',
        iconAsset: 'assets/icons/market.png',
        isUnlocked: false,
      ),
    ];
  }
}
