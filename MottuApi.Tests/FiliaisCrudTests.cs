using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace MottuApi.Tests;

public class FiliaisCrudTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;
    public FiliaisCrudTests(CustomWebAppFactory f) => _client = f.CreateClient();

    record FilialDto(long Id, string Nome, string? Endereco);

    [Fact]
    public async Task Deve_criar_listar_atualizar_e_excluir_filial()
    {
        // POST
        var createdResp = await _client.PostAsJsonAsync("/api/v1/filiais", new { nome = "Filial Leste", endereco = "Av. B, 456" });
        createdResp.StatusCode.Should().Be(HttpStatusCode.Created);

        // lê payload criado (campo "item")
        var createdJson = await createdResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        createdJson.Should().NotBeNull();
        var itemNode = createdJson!["item"];
        var id = (long)((System.Text.Json.JsonElement)itemNode).GetProperty("id").GetInt64();

        // GET by id
        var getResp = await _client.GetAsync($"/api/v1/filiais/{id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // PUT
        var putResp = await _client.PutAsJsonAsync($"/api/v1/filiais/{id}", new { id, nome = "Filial Leste (upd)", endereco = "Av. B, 456" });
        putResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // DELETE
        var delResp = await _client.DeleteAsync($"/api/v1/filiais/{id}");
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // GET 404
        var get404 = await _client.GetAsync($"/api/v1/filiais/{id}");
        get404.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
