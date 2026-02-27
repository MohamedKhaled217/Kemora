using Kemora.Api.DTOs;
using Kemora.Domain.Entities;
using Kemora.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kemora.Api.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize]
    public class PlacesManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlacesManagementController(ApplicationDbContext context) => _context = context;

        // ═══════════════════════════════════════════════════════════════════════
        // GOVERNORATES
        // ═══════════════════════════════════════════════════════════════════════

        [HttpPost("governorates")]
        public async Task<ActionResult<GovernorateResponseDto>> CreateGovernorate([FromBody] CreateGovernorateDto dto)
        {
            var gov = new Governorate { Name = dto.Name, Region = dto.Region };
            _context.Governorates.Add(gov);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGovernorate), new { id = gov.GovernorateID },
                new GovernorateResponseDto { GovernorateID = gov.GovernorateID, Name = gov.Name, Region = gov.Region });
        }

        [HttpGet("governorates")]
        [AllowAnonymous]
        public async Task<ActionResult<List<GovernorateResponseDto>>> GetGovernorates()
        {
            return Ok(await _context.Governorates
                .Select(g => new GovernorateResponseDto
                {
                    GovernorateID = g.GovernorateID,
                    Name = g.Name,
                    Region = g.Region,
                    PlaceCount = g.Places.Count
                }).ToListAsync());
        }

        [HttpGet("governorates/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<GovernorateResponseDto>> GetGovernorate(int id)
        {
            var g = await _context.Governorates.Include(x => x.Places)
                .FirstOrDefaultAsync(x => x.GovernorateID == id);
            if (g == null) return NotFound();
            return Ok(new GovernorateResponseDto
            {
                GovernorateID = g.GovernorateID, Name = g.Name,
                Region = g.Region, PlaceCount = g.Places.Count
            });
        }

        [HttpPut("governorates/{id}")]
        public async Task<IActionResult> UpdateGovernorate(int id, [FromBody] CreateGovernorateDto dto)
        {
            var g = await _context.Governorates.FindAsync(id);
            if (g == null) return NotFound();
            g.Name = dto.Name; g.Region = dto.Region;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("governorates/{id}")]
        public async Task<IActionResult> DeleteGovernorate(int id)
        {
            var g = await _context.Governorates.Include(x => x.Places).FirstOrDefaultAsync(x => x.GovernorateID == id);
            if (g == null) return NotFound();
            if (g.Places.Any()) return BadRequest("Cannot delete governorate that still has places.");
            _context.Governorates.Remove(g);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ═══════════════════════════════════════════════════════════════════════
        // CATEGORIES
        // ═══════════════════════════════════════════════════════════════════════

        [HttpPost("categories")]
        public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var cat = new Category { Name = dto.Name };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategories), null,
                new CategoryResponseDto { CategoryID = cat.CategoryID, Name = cat.Name });
        }

        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryResponseDto>>> GetCategories()
        {
            return Ok(await _context.Categories
                .Include(c => c.PlaceTypes)
                .Select(c => new CategoryResponseDto
                {
                    CategoryID = c.CategoryID, Name = c.Name,
                    PlaceTypes = c.PlaceTypes.Select(pt => new PlaceTypeResponseDto
                    {
                        TypeID = pt.TypeID, GoogleType = pt.GoogleType,
                        DisplayName = pt.DisplayName, CategoryName = c.Name
                    }).ToList()
                }).ToListAsync());
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto dto)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return NotFound();
            c.Name = dto.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var c = await _context.Categories.Include(x => x.PlaceTypes).FirstOrDefaultAsync(x => x.CategoryID == id);
            if (c == null) return NotFound();
            if (c.PlaceTypes.Any()) return BadRequest("Cannot delete a category that still has place types.");
            _context.Categories.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ═══════════════════════════════════════════════════════════════════════
        // PLACE TYPES
        // ═══════════════════════════════════════════════════════════════════════

        [HttpPost("placetypes")]
        public async Task<ActionResult<PlaceTypeResponseDto>> CreatePlaceType([FromBody] CreatePlaceTypeDto dto)
        {
            var cat = await _context.Categories.FindAsync(dto.CategoryID);
            if (cat == null) return BadRequest("Category not found.");

            var pt = new PlaceType { GoogleType = dto.GoogleType, DisplayName = dto.DisplayName, CategoryID = dto.CategoryID };
            _context.PlaceTypes.Add(pt);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaceTypes), null,
                new PlaceTypeResponseDto { TypeID = pt.TypeID, GoogleType = pt.GoogleType, DisplayName = pt.DisplayName, CategoryName = cat.Name });
        }

        [HttpGet("placetypes")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PlaceTypeResponseDto>>> GetPlaceTypes()
        {
            return Ok(await _context.PlaceTypes
                .Include(pt => pt.Category)
                .Select(pt => new PlaceTypeResponseDto
                {
                    TypeID = pt.TypeID, GoogleType = pt.GoogleType,
                    DisplayName = pt.DisplayName, CategoryName = pt.Category.Name
                }).ToListAsync());
        }

        [HttpPut("placetypes/{id}")]
        public async Task<IActionResult> UpdatePlaceType(int id, [FromBody] CreatePlaceTypeDto dto)
        {
            var pt = await _context.PlaceTypes.FindAsync(id);
            if (pt == null) return NotFound();
            pt.GoogleType = dto.GoogleType; pt.DisplayName = dto.DisplayName; pt.CategoryID = dto.CategoryID;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("placetypes/{id}")]
        public async Task<IActionResult> DeletePlaceType(int id)
        {
            var pt = await _context.PlaceTypes.Include(x => x.Places).FirstOrDefaultAsync(x => x.TypeID == id);
            if (pt == null) return NotFound();
            if (pt.Places.Any()) return BadRequest("Cannot delete a place type that still has places.");
            _context.PlaceTypes.Remove(pt);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ═══════════════════════════════════════════════════════════════════════
        // PLACES (admin create/update/delete, public list/detail)
        // ═══════════════════════════════════════════════════════════════════════

        [HttpPost("places")]
        public async Task<ActionResult<PlaceDetailDto>> CreatePlace([FromBody] CreatePlaceDto dto)
        {
            if (!await _context.Governorates.AnyAsync(g => g.GovernorateID == dto.GovernorateID))
                return BadRequest("Governorate not found.");
            if (!await _context.PlaceTypes.AnyAsync(pt => pt.TypeID == dto.PlaceTypeID))
                return BadRequest("PlaceType not found.");

            var place = new Place
            {
                Name = dto.Name, Description = dto.Description, Address = dto.Address,
                Latitude = dto.Latitude, Longitude = dto.Longitude,
                Phone = dto.Phone, Website = dto.Website,
                Rating = dto.Rating, PriceLevel = dto.PriceLevel,
                OpeningHoursJSON = dto.OpeningHoursJSON, MainImageURL = dto.MainImageURL,
                GovernorateID = dto.GovernorateID, PlaceTypeID = dto.PlaceTypeID
            };
            _context.Places.Add(place);
            await _context.SaveChangesAsync();
            return Created($"/api/places/db/{place.PlaceID}", new { placeID = place.PlaceID });
        }

        [HttpPut("places/{id}")]
        public async Task<IActionResult> UpdatePlace(int id, [FromBody] UpdatePlaceDto dto)
        {
            var place = await _context.Places.FindAsync(id);
            if (place == null) return NotFound();

            if (dto.Name != null) place.Name = dto.Name;
            if (dto.Description != null) place.Description = dto.Description;
            if (dto.Address != null) place.Address = dto.Address;
            if (dto.Latitude.HasValue) place.Latitude = dto.Latitude.Value;
            if (dto.Longitude.HasValue) place.Longitude = dto.Longitude.Value;
            if (dto.Phone != null) place.Phone = dto.Phone;
            if (dto.Website != null) place.Website = dto.Website;
            if (dto.Rating.HasValue) place.Rating = dto.Rating.Value;
            if (dto.PriceLevel.HasValue) place.PriceLevel = dto.PriceLevel.Value;
            if (dto.OpeningHoursJSON != null) place.OpeningHoursJSON = dto.OpeningHoursJSON;
            if (dto.MainImageURL != null) place.MainImageURL = dto.MainImageURL;
            if (dto.GovernorateID.HasValue) place.GovernorateID = dto.GovernorateID.Value;
            if (dto.PlaceTypeID.HasValue) place.PlaceTypeID = dto.PlaceTypeID.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("places/{id}")]
        public async Task<IActionResult> DeletePlace(int id)
        {
            var place = await _context.Places.FindAsync(id);
            if (place == null) return NotFound();
            _context.Places.Remove(place);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // PUBLIC place endpoints (no admin prefix)
    // ═══════════════════════════════════════════════════════════════════════════

    [Route("api/places")]
    [ApiController]
    public class PlacesPublicController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PlacesPublicController(ApplicationDbContext context) => _context = context;

        /// <summary>List / search DB-stored places (paginated, filterable).</summary>
        [HttpGet("db")]
        public async Task<ActionResult<List<PlaceListDto>>> ListPlaces(
            [FromQuery] int? governorateId,
            [FromQuery] int? placeTypeId,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.Places
                .Include(p => p.Governorate)
                .Include(p => p.PlaceType)
                .AsQueryable();

            if (governorateId.HasValue) query = query.Where(p => p.GovernorateID == governorateId.Value);
            if (placeTypeId.HasValue) query = query.Where(p => p.PlaceTypeID == placeTypeId.Value);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) || p.Address.Contains(search));

            var list = await query
                .OrderByDescending(p => p.Rating)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(p => new PlaceListDto
                {
                    PlaceID = p.PlaceID, Name = p.Name, Address = p.Address,
                    Rating = p.Rating, PriceLevel = p.PriceLevel,
                    GovernorateName = p.Governorate.Name,
                    PlaceTypeName = p.PlaceType.DisplayName,
                    MainImageURL = p.MainImageURL
                }).ToListAsync();

            return Ok(list);
        }

        /// <summary>Get full place detail.</summary>
        [HttpGet("db/{id}")]
        public async Task<ActionResult<PlaceDetailDto>> GetPlace(int id)
        {
            var p = await _context.Places
                .Include(x => x.Governorate).Include(x => x.PlaceType)
                .Include(x => x.Photos).Include(x => x.Reviews).Include(x => x.Events)
                .FirstOrDefaultAsync(x => x.PlaceID == id);
            if (p == null) return NotFound();

            return Ok(new PlaceDetailDto
            {
                PlaceID = p.PlaceID, GooglePlaceID = p.GooglePlaceID,
                Name = p.Name, Description = p.Description, Address = p.Address,
                Latitude = p.Latitude, Longitude = p.Longitude,
                Phone = p.Phone, Website = p.Website,
                Rating = p.Rating, PriceLevel = p.PriceLevel,
                OpeningHoursJSON = p.OpeningHoursJSON, MainImageURL = p.MainImageURL,
                GovernorateName = p.Governorate.Name,
                PlaceTypeName = p.PlaceType.DisplayName,
                Photos = p.Photos.Select(ph => new PhotoResponseDto
                    { PhotoID = ph.PhotoID, ImageURL = ph.ImageURL, IsMain = ph.IsMain, PlaceID = ph.PlaceID }).ToList(),
                Reviews = p.Reviews.Select(r => new ReviewResponseDto
                    { ReviewID = r.ReviewID, AuthorName = r.AuthorName, Rating = r.Rating, Text = r.Text, PlaceID = r.PlaceID }).ToList(),
                Events = p.Events.Select(e => new EventResponseDto
                    { EventID = e.EventID, Name = e.Name, StartDate = e.StartDate, EndDate = e.EndDate, PlaceID = e.PlaceID, PlaceName = p.Name }).ToList()
            });
        }
    }
}
