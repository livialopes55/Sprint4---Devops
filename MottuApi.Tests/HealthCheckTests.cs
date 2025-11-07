using System.Net;
using FluentAssertions;
using Xunit;

namespace MottuApi.Tests;

public class HealthCheckTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;
    public HealthCheckTests(CustomWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task GET_health_deve_retornar_200()
    {
        var resp = await _client.GetAsync("/health");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
