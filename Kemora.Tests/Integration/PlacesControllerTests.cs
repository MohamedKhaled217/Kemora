using FluentAssertions;
using Kemora.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Kemora.Tests.Integration
{
    public class PlacesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public PlacesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetPlaces_ReturnsSuccessAndOkStatus()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/places");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var result = await response.Content.ReadFromJsonAsync<PagedResult<PlacePublicDto>>();
            result.Should().NotBeNull();
        }
    }
}
