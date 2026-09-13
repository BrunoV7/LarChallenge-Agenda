# Agenda de Contatos — Desafio Técnico

API REST para gerenciamento de uma agenda de contatos, permitindo o cadastro de pessoas e seus respectivos telefones, com relacionamento um-para-muitos entre eles.

## Tecnologias

- **.NET 10** — WebAPI REST
- **Entity Framework Core** — ORM e migrations
- **SQLite** — persistência de dados
- **Scalar** — documentação interativa da API

## Como executar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Ferramenta de migrations do EF Core:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Passos

1. Clone o repositório:
   ```bash
   git clone https://github.com/BrunoV7/LarChallenge-Agenda.git
   cd LarChallenge-Agenda
   ```

2. Aplique as migrations para criar o banco de dados:
   ```bash
   dotnet ef database update
   ```

3. Execute a aplicação:
   ```bash
   dotnet run
   ```

4. Acesse a documentação interativa da API (Scalar):
   ```
   http://localhost:5132/scalar/
   ```

## Endpoints

### Pessoas

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/pessoas` | Lista pessoas ativas (paginado) |
| GET | `/api/pessoas/{cpf}` | Busca uma pessoa pelo CPF |
| POST | `/api/pessoas` | Cadastra uma nova pessoa |
| PUT | `/api/pessoas/{cpf}` | Atualiza uma pessoa existente |
| DELETE | `/api/pessoas/{cpf}` | Remove uma pessoa (soft delete) |

### Telefones

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/telefones` | Lista telefones ativos (paginado) |
| GET | `/api/telefones/{id}` | Busca um telefone pelo ID |
| POST | `/api/telefones` | Cadastra um novo telefone vinculado a uma pessoa |
| PUT | `/api/telefones/{id}` | Atualiza um telefone existente |
| DELETE | `/api/telefones/{id}` | Remove um telefone (soft delete) |

As listagens aceitam os parâmetros de query `page` (padrão: 1) e `size` (padrão: 10, máximo: 100).

## Decisões de Design

### Soft Delete
Tanto pessoas quanto telefones não são removidos fisicamente do banco. Ao "deletar", o registro é marcado como inativo (`IsActive = false`) e deixa de aparecer nas consultas. Isso preserva o histórico dos dados para fins de auditoria, permitindo rastrear registros que foram removidos.

### CPF como identificador de Pessoa
As rotas de pessoa utilizam o CPF como identificador público (`/api/pessoas/{cpf}`) em vez do Id interno. O CPF é o identificador natural de negócio de uma pessoa, sendo único e significativo para o usuário. Um índice único no banco garante que não existam CPFs duplicados.

### Telefone único por pessoa
A validação de número duplicado é feita por pessoa, não globalmente. Duas pessoas diferentes podem ter o mesmo número (ex.: um telefone residencial compartilhado), mas a mesma pessoa não pode cadastrar o mesmo número duas vezes.

### Tratamento de erros centralizado
Os erros são tratados por um handler global (`IExceptionHandler`) que traduz exceções de domínio em respostas HTTP apropriadas (404, 400, 500) no formato padrão ProblemDetails. Isso mantém os controllers limpos, sem blocos try/catch repetidos.

### DTOs de request e response
As entidades de domínio nunca são expostas diretamente pela API. DTOs específicos controlam o que entra e o que sai, evitando problemas de serialização (referência circular na relação Pessoa↔Telefone) e escondendo detalhes internos.

## Estrutura do Projeto

```
Agenda/
├── Controllers/     # Endpoints da API (Pessoa e Telefone)
├── Services/        # Regras de negócio e acesso a dados
├── Data/            # DbContext e configuração do banco
├── Models/          # Entidades de domínio (Pessoa, Telefone)
│   └── utils/       # Utilitários (PagedResult)
├── DTO/
│   ├── requests/    # Objetos de entrada da API
│   └── response/    # Objetos de saída da API
├── Middleware/      # Tratamento global de exceções
└── Migrations/      # Histórico de migrations do EF Core
```

O projeto segue uma separação em camadas: os **Controllers** lidam apenas com HTTP, os **Services** concentram a lógica de negócio, e o acesso a dados é feito via **Entity Framework Core** através do `DbContext`.

