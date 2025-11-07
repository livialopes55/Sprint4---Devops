using System.Net;
using FluentAssertions;
using Xunit;

namespace MottuApi.Tests;

public class VersioningTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;
    public VersioningTests(CustomWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task GET_filiais_deve_retornar_header_de_versao()
    {
        var resp = await _client.GetAsync("/api/v1/filiais");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.TryGetValues("api-supported-versions", out var values).Should().BeTrue();
    }
}
