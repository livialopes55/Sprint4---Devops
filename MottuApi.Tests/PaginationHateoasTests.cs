using System.Net;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace MottuApi.Tests;

public class PaginationHateoasTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;
    public PaginationHateoasTests(CustomWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task Lista_filiais_deve_trazer_paginacao_e_links()
    {
        var resp = await _client.GetAsync("/api/v1/filiais?pageNumber=1&pageSize=5");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.TryGetProperty("data", out var data).Should().BeTrue();
        root.TryGetProperty("_links", out var links).Should().BeTrue();
        links.ToString().Should().Contain("self");

        // header X-Pagination
        resp.Headers.Contains("X-Pagination").Should().BeTrue();
    }
}
