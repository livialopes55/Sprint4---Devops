# Sprint 4 – Upgrades aplicados ao projeto MottuApi

Implementado:
- Health Checks: `/health` e `/health/db` (rota db placeholder)
- Versionamento (mantido): `api/v{version}`
- Segurança: JWT (`POST /api/v1/auth/login`)
- HATEOAS (mantido e pronto para estender aos demais controllers)
- ML.NET: `POST /api/v1/ml/sentiment`
- Swagger com XML comments + JWT Authorize
- Testes: xUnit (unitário + integração com WebApplicationFactory)

## Como rodar
1. Abra a solução `MottuSprint3.sln` no Visual Studio 2022
2. **Defina MottuApi** como Startup (`Set as Startup Project`)
3. Restaurar pacotes NuGet (clicar na solução -> Restore)
4. `F5` para executar (Swagger abre na raiz)

### Login (Swagger)
```json
{ "username": "admin", "password": "123456" }
```
Clique em **Authorize** e cole `Bearer SEU_TOKEN`.

## Testes
- Test Explorer -> Run All
- Ou `dotnet test` na raiz da solução


## Melhorias aplicadas do feedback da Sprint 3
- HATEOAS aplicado em **todos os métodos principais** (GET lista, GET por id, POST).
- Swagger com **descrições** e **exemplos** (remarks).
- Arquitetura e separação: adição de camadas lógicas (ML, Utils já existentes) + endpoints versionados.
