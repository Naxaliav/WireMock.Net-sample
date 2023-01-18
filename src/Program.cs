using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<PetStoreClient>(client =>
{
    client.BaseAddress = new Uri("https://petstore.swagger.io/v2/");
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

app.MapGet("/pet/findByStatus", async (PetStoreClient petStoreClient, PetStatus status)
    => Results.Ok(await petStoreClient.FindPetsByStatusAsync(status)));

app.Run();


internal sealed class PetStoreClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public PetStoreClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PetDto>> FindPetsByStatusAsync(
        PetStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<PetDto>>(
            $"pet/findByStatus?status={status.ToString().ToLower()}", SerializerOptions, cancellationToken)
               ?? throw new PetStoreException("Failed to deserialize bets by status.");
    }

    public sealed class PetStoreException : Exception
    {
        public PetStoreException(string message) : base(message) { }
    }
}

internal sealed record PetDto(
    long Id,
    string Name);

public enum PetStatus
{
    Available = 1,
    Pending,
    Sold
}

public partial class Program { }