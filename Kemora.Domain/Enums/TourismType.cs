using System.Text.Json.Serialization;

namespace Kemora.Domain.Enums
{
    /// <summary>
    /// Types of tourism that a traveler might be interested in.
    /// Multiple can be selected for a single trip plan request.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TourismType
    {
        /// <summary>Relaxation, beaches, resorts, spa retreats</summary>
        Leisure,

        /// <summary>Museums, archaeological sites, historical landmarks, heritage</summary>
        CulturalHeritage,

        /// <summary>Desert safaris, diving, hiking, extreme sports</summary>
        Adventure,

        /// <summary>Nature reserves, wildlife, sustainable tourism</summary>
        EcoTourism,

        /// <summary>Conferences, exhibitions, corporate events</summary>
        Business,

        /// <summary>Medical treatments, wellness retreats, thermal springs</summary>
        MedicalWellness,

        /// <summary>Mosques, churches, monasteries, spiritual journeys</summary>
        ReligiousPilgrimage,

        /// <summary>Sporting events, stadiums, golf, water sports</summary>
        Sports,

        /// <summary>Street food tours, cooking classes, local cuisine experiences</summary>
        Culinary
    }
}
