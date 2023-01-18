using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WebApi.IntegrationTests;

public class GetPetsByStatusTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly WireMockServer _server;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GetPetsByStatusTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _server = WireMockServer.Start(5175);
    }

    [Fact]
    public async Task GetPetsByStatus_Should_ReturnAtleastOnePet()
    {
        // Arrange
        var expected = new List<PetDto> { new(Id: 1, Name: "Dog") };

        _server
            .Given(
                Request.Create()
                .WithPath("/pet/findByStatus")
                .WithParam("status", PetStatus.Available.ToString().ToLower())
                .UsingGet())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithBody(JsonSerializer.Serialize(new List<PetDto> { new(1, "Dog") })));

        // Act
        var actual = await _httpClient.GetFromJsonAsync<List<PetDto>>($"/pet/findByStatus?status={PetStatus.Available}", JsonOptions);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _server.Dispose();
    }
}