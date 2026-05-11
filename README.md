# WizCo — API de Pedidos

API REST para gerenciamento de pedidos, desenvolvida com .NET 8 e ASP.NET Core Web API.

## Tecnologias

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8 + SQLite
- FluentValidation 11
- AutoMapper 14
- Swashbuckle / Swagger UI
- xUnit + Moq + FluentAssertions

## Como executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Executar a API

```bash
dotnet run --project src/WizCo.Api
```

A API ficará disponível em `https://localhost:{porta}`.
O Swagger UI estará acessível diretamente na raiz: `https://localhost:{porta}/`.

O banco de dados SQLite (`wizco_pedidos.db`) é criado e migrado automaticamente na primeira execução — nenhum passo adicional é necessário.

### Executar os testes

```bash
dotnet test
```

## Endpoints

| Método | Rota                                                          | Descrição                              |
|--------|---------------------------------------------------------------|----------------------------------------|
| POST   | `/pedidos`                                                    | Cria um novo pedido                    |
| GET    | `/pedidos/{id}`                                               | Retorna pedido completo com itens      |
| GET    | `/pedidos?status=Pago&pagina=1&tamanhoPagina=10`              | Lista pedidos paginados (filtro opcional) |
| PUT    | `/pedidos/{id}/cancelar`                                      | Cancela um pedido                      |

### Paginação

A listagem (`GET /pedidos`) retorna um envelope com metadados:

```json
{
  "itens": [ /* lista de pedidos */ ],
  "pagina": 1,
  "tamanhoPagina": 10,
  "totalItens": 25,
  "totalPaginas": 3,
  "temPaginaAnterior": false,
  "temProximaPagina": true
}
```

- `pagina` (default `1`) — número da página, 1-based. Valores `< 1` são clamped para `1`.
- `tamanhoPagina` (default `10`, máximo `100`) — itens por página. Valores fora do intervalo são clamped automaticamente.

## Decisões técnicas

### Entidades com setters privados
Protege os invariantes de domínio, forçando mutações a passarem por métodos controlados (`Pedido.Cancelar()`). O EF Core acessa o campo de backing `_itens` via configuração explícita de `PropertyAccessMode.Field`.

### DTOs separados por request/response
Nunca expõe entidades diretamente. Requests usam `sealed class` com `init` properties (compatível com model binding). Responses usam `sealed record` (imutáveis, igualdade estrutural).

### Dois métodos de leitura no repositório
- `ObterPorIdAsync` → usa `AsNoTracking` para leituras (melhor performance, sem change tracking desnecessário).
- `ObterPorIdParaEdicaoAsync` → carrega com tracking para que o EF detecte mudanças automaticamente ao salvar.

### Middleware global de exceções
Centraliza o tratamento de erros sem poluir controllers ou services com código HTTP:
- `KeyNotFoundException` → `404 Not Found`
- `InvalidOperationException` → `409 Conflict`
- Qualquer outra exceção → `500 Internal Server Error`

O `400 Bad Request` de validação é tratado pelo pipeline do FluentValidation, que retorna `ValidationProblemDetails` automaticamente antes do controller executar.

### AutoMapper com Status como string
Retorna `"Novo"`, `"Pago"`, `"Cancelado"` no JSON em vez de inteiros, conforme esperado pelos consumidores da API.

### Migrations automáticas no startup
`db.Database.MigrateAsync()` no `Program.cs` garante que o banco esteja funcional imediatamente, sem passos manuais.
