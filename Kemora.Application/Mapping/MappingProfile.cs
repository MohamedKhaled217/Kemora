using AutoMapper;
using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using System.Linq;

namespace Kemora.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, AuthResponseDto>()
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id));
            CreateMap<ApplicationUser, ProfileDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id));
            CreateMap<ApplicationUser, PublicProfileDto>()
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id));

            // Gamification
            CreateMap<Badge, BadgeResponseDto>();
            CreateMap<UserBadge, UserBadgeResponseDto>()
                .ForMember(d => d.BadgeName, o => o.MapFrom(s => s.Badge.Name))
                .ForMember(d => d.BadgeDescription, o => o.MapFrom(s => s.Badge.Description));
            CreateMap<UserPoint, PointHistoryDto>()
                .ForMember(d => d.SourcePlaceName, o => o.MapFrom(s => s.SourcePlace.Name));
            CreateMap<ApplicationUser, LeaderboardEntryDto>()
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Rank, o => o.Ignore());
            
            // Favorites
            CreateMap<UserFavorite, FavoriteResponseDto>()
                .ForMember(d => d.PlaceName, o => o.MapFrom(s => s.Place.Name))
                .ForMember(d => d.PlaceAddress, o => o.MapFrom(s => s.Place.Address))
                .ForMember(d => d.MainImageURL, o => o.MapFrom(s => s.Place.MainImageURL));

            // Trips
            CreateMap<TripPlace, TripPlaceResponseDto>()
                .ForMember(d => d.PlaceName, o => o.MapFrom(s => s.Place.Name));
            CreateMap<Trip, TripListDto>()
                .ForMember(d => d.PlaceCount, o => o.MapFrom(s => s.TripPlaces.Count));
            CreateMap<Trip, TripDetailDto>()
                .ForMember(d => d.Places, o => o.MapFrom(s => s.TripPlaces.OrderBy(tp => tp.VisitDate)));

            // Social/Posts
            CreateMap<PostMedia, PostMediaResponseDto>();
            CreateMap<CommentMedia, CommentMediaResponseDto>();

            CreateMap<Post, PostListResponseDto>()
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.UserID))
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.ReactionCount, o => o.MapFrom(s => s.Reactions.Count))
                .ForMember(d => d.CommentCount, o => o.MapFrom(s => s.Comments.Count));
            
            CreateMap<Post, PostDetailResponseDto>()
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.UserID))
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.ReactionCount, o => o.MapFrom(s => s.Reactions.Count))
                .ForMember(d => d.CommentCount, o => o.MapFrom(s => s.Comments.Count))
                .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments.OrderByDescending(c => c.CreatedAt)));

            CreateMap<Comment, CommentResponseDto>()
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.UserID))
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.ReactionCount, o => o.MapFrom(s => s.Reactions.Count));

            // Reviews
            CreateMap<Review, ReviewResponseDto>();

            // Notifications
            CreateMap<Notification, NotificationDto>();

            // Photos
            CreateMap<Photo, PhotoResponseDto>();

            // Events
            CreateMap<Event, EventResponseDto>();

            // Places / Management
            CreateMap<Governorate, GovernorateDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<PlaceType, PlaceTypeDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
            
            CreateMap<Place, PlaceAdminDto>()
                .ForMember(d => d.GovernorateName, o => o.MapFrom(s => s.Governorate.Name))
                .ForMember(d => d.PlaceTypeName, o => o.MapFrom(s => s.PlaceType.DisplayName));
            CreateMap<Place, PlacePublicDto>()
                .ForMember(d => d.PlaceTypeName, o => o.MapFrom(s => s.PlaceType.DisplayName));
            CreateMap<Place, PlaceDetailPublicDto>()
                .ForMember(d => d.PlaceTypeName, o => o.MapFrom(s => s.PlaceType.DisplayName));
        }
    }
}
