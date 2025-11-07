using System.Net.Http.Json;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace MottuApi.Tests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ApiIntegrationTests(WebApplicationFactory<Program> factory) { _factory = factory; }

    [Fact]
    public async Task Health_Should_Return_200()
    {
        var client = _factory.CreateClient();
        var r = await client.GetAsync("/health");
        r.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Protected_Endpoint_Should_Require_JWT()
    {
        var client = _factory.CreateClient();
        var r = await client.PostAsJsonAsync("/api/v1/ml/sentiment", new { text = "ok" });
        r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_And_Call_Protected_Endpoint()
    {
        var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new { username = "admin", password = "123456" });
        login.EnsureSuccessStatusCode();
        var obj = await login.Content.ReadFromJsonAsync<System.Text.Json.Nodes.JsonObject>();
        var token = obj!["token"]!.ToString();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var r = await client.PostAsJsonAsync("/api/v1/ml/sentiment", new { text = "excelente" });
        r.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
