# Especificação de Implementação — API de Pedidos (.NET 8)

## Objetivo
Desenvolver uma API REST utilizando ASP.NET Core Web API para gerenciamento de pedidos e itens de pedido, aplicando boas práticas de arquitetura, validações, organização de código e regras de negócio.

O foco da implementação deve ser:
- clareza do código
- separação de responsabilidades
- consistência das regras de negócio
- estrutura escalável
- legibilidade

---

# Stack Tecnológica

## Backend
- .NET 8
- ASP.NET Core Web API

## Persistência
- Entity Framework Core


## Banco de Dados
- SQLite (preferencial)

---

# Estrutura Esperada do Projeto

Sugestão de organização:

```text
/src
 ├── Controllers
 ├── Services
 ├── Repositories
 ├── Entities
 ├── DTOs
 ├── Validators
 ├── Data
 ├── Mappings
 ├── Middlewares
 └── Configurations
```

---

# Entidades

## Pedido

Campos:
- `Id` → Guid
- `ClienteNome` → string
- `DataCriacao` → DateTime
- `Status` → enum
- `ValorTotal` → decimal
- `Itens` → coleção de ItemPedido

---

## ItemPedido

Campos:
- `Id` → Guid/int
- `PedidoId` → Guid
- `ProdutoNome` → string
- `Quantidade` → int
- `PrecoUnitario` → decimal

---

# Enum de Status

Criar enum:

```csharp
public enum PedidoStatus
{
    Novo = 1,
    Pago = 2,
    Cancelado = 3
}
```

---

# Regras de Negócio

## Regras obrigatórias

### Pedido deve possuir itens
Não permitir criação de pedido vazio.

---

### Quantidade deve ser maior que zero
Todos os itens devem possuir:

```text
Quantidade > 0
```

---

### ValorTotal deve ser calculado automaticamente
O backend deve calcular:

```text
ValorTotal = soma(Quantidade * PrecoUnitario)
```

O valor não deve ser enviado pelo cliente.

---

### Pedido Pago não pode ser cancelado
Ao cancelar:
- validar status atual
- impedir cancelamento caso status seja `Pago`

Retornar erro apropriado.

---

# Endpoints

---

## POST /pedidos

### Objetivo
Criar pedido com itens.

### Request esperado

```json
{
  "clienteNome": "João",
  "itens": [
    {
      "produtoNome": "Notebook",
      "quantidade": 1,
      "precoUnitario": 3500
    }
  ]
}
```

---

### Regras
- validar itens
- calcular valor total
- status inicial = `Novo`
- data de criação automática

---

### Response esperado

```json
{
  "id": "guid",
  "clienteNome": "João",
  "status": "Novo",
  "valorTotal": 3500
}
```

---

## GET /pedidos/{id}

### Objetivo
Retornar pedido completo com itens.

### Response esperado

```json
{
  "id": "guid",
  "clienteNome": "João",
  "dataCriacao": "2026-05-11T10:00:00",
  "status": "Novo",
  "valorTotal": 3500,
  "itens": [
    {
      "produtoNome": "Notebook",
      "quantidade": 1,
      "precoUnitario": 3500
    }
  ]
}
```

---

## GET /pedidos?status=Pago

### Objetivo
Listar pedidos filtrando por status.

### Regras
- filtro opcional
- retornar lista

---

## PUT /pedidos/{id}/cancelar

### Objetivo
Cancelar pedido.

### Regras
- pedido deve existir
- não permitir cancelamento de pedido pago

### Resultado esperado
- atualizar status para `Cancelado`

---

# Arquitetura Recomendada

## Controller
Responsável apenas por:
- receber request
- retornar response
- delegar processamento

Evitar regra de negócio no controller.

---

## Service
Responsável por:
- regras de negócio
- validações de domínio
- cálculos
- orquestração

---

## Repository
Responsável por:
- persistência
- consultas
- acesso ao banco

---

# DTOs

Utilizar DTOs para:
- request
- response

Evitar expor entidades diretamente.

Exemplo:

```csharp
public class CriarPedidoRequest
{
    public string ClienteNome { get; set; }
    public List<CriarItemPedidoRequest> Itens { get; set; }
}
```

---

# Persistência

## Entity Framework Core
Caso utilize EF:
- configurar DbContext
- relacionamento 1:N entre Pedido e ItemPedido
- migrations opcionais

---

## Dapper
Caso utilize Dapper:
- queries parametrizadas
- evitar SQL Injection
- separar queries da camada de negócio

---

# Validações

## Obrigatórias
- cliente nome obrigatório
- pedido com pelo menos 1 item
- quantidade > 0
- preço unitário > 0

---

## Diferencial
Utilizar:
- FluentValidation

---

# Tratamento de Erros

Implementar respostas apropriadas:
- `400 BadRequest`
- `404 NotFound`
- `409 Conflict`
- `500 InternalServerError`

Sugestão:
- middleware global de exceções

---

# Logs

Diferencial recomendado:
- logs com ILogger

Exemplos:
- criação de pedido
- cancelamento
- erros

---

# Testes Unitários

Diferencial recomendado:
- validar regras de negócio principais

Exemplos:
- impedir cancelamento de pedido pago
- cálculo de valor total
- impedir pedido sem itens

Framework sugerido:
- xUnit

---

# Documentação

Adicionar:
- Swagger/OpenAPI

Endpoints devem estar documentados automaticamente.

---

# Critérios de Avaliação

O teste não será avaliado apenas pelo funcionamento.

Os principais critérios são:
- organização do projeto
- clareza do código
- separação de responsabilidades
- legibilidade
- consistência arquitetural
- qualidade das validações
- boas práticas do .NET
- tratamento de erros
- nomenclatura

---

# Diferenciais Técnicos Relevantes

Itens que agregam valor:
- Clean Architecture simplificada
- FluentValidation
- AutoMapper
- testes unitários
- middleware global
- paginação
- logging estruturado
- uso correto de async/await
- repository pattern bem aplicado

---

# Entrega

O projeto deve conter:
- código-fonte
- instruções de execução
- dependências configuradas
- banco funcional ao iniciar

Idealmente incluir:
- README.md com instruções de execução e decisões técnicas.