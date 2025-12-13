using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kemora.Domain.Entities
{
    public class Trip
    {
        [Key] public int TripID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<TripPlace> TripPlaces { get; set; }
    }

    public class TripPlace
    {
        [Key] public int TripPlaceID { get; set; }
        public int TripID { get; set; }
        public Trip Trip { get; set; }
        public int PlaceID { get; set; }
        public Place Place { get; set; }
        public DateTime VisitDate { get; set; }
        public string? Notes { get; set; }
    }

    // --- AI CACHING LAYERS ---
    public class TripTemplate
    {
        public int Id { get; set; }
        public string Signature { get; set; } // "Cairo_3Days_Budget"
        public ICollection<TripTemplateItem> Items { get; set; }
    }

    public class TripTemplateItem
    {
        public int Id { get; set; }
        public int TripTemplateId { get; set; }
        public int PlaceId { get; set; }
        public int DayNumber { get; set; }
    }
}