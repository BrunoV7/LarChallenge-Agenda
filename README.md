# Agenda de Contatos — Desafio Técnico

API REST para gerenciamento de uma agenda de contatos, permitindo o cadastro de pessoas e seus respectivos telefones, com relacionamento um-para-muitos entre eles. Inclui autenticação via JWT, validação de CPF e telefone, e testes automatizados.

## Tecnologias

- **.NET 10** — WebAPI REST
- **Entity Framework Core** — ORM e migrations
- **SQLite** — persistência de dados
- **JWT (Bearer)** — autenticação e autorização
- **BCrypt** — hash de senhas
- **xUnit** — testes automatizados
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

2. Configure as variáveis de ambiente. Copie o arquivo de exemplo e defina a chave JWT:
   ```bash
   cp .env.example .env
   ```
   Abra o `.env` e defina `Jwt__Key` com uma chave secreta de no mínimo 32 caracteres.

3. Aplique as migrations para criar o banco de dados:
   ```bash
   dotnet ef database update
   ```

4. Execute a aplicação:
   ```bash
   dotnet run
   ```

5. Acesse a documentação interativa da API (Scalar):
   ```
   http://localhost:5132/scalar/
   ```

> As datas usam o formato `dd/MM/yyyy` (ex.: `"21/03/1995"`), tanto na entrada quanto na saída.

## Autenticação

A API usa autenticação JWT. Os endpoints de pessoas e telefones exigem um token válido; apenas o registro e o login são públicos.

Para obter um token:
1. Registre um usuário em `POST /api/auth/register` (ou faça login em `POST /api/auth/login`)
2. A resposta traz um token JWT
3. Envie o token no header `Authorization: Bearer {token}` nas demais requisições

No Scalar, use o botão de autenticação para inserir o token e testar os endpoints protegidos.

Um usuário administrador é criado automaticamente na primeira execução:
- **Email:** `admin@agenda.com`
- **Senha:** `Admin@123`

Use essas credenciais no login para obter um token com acesso de administrador.

## Endpoints

### Autenticação

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/register` | Registra um novo usuário e retorna um token |
| POST | `/api/auth/login` | Autentica um usuário e retorna um token |

### Pessoas

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/pessoas` | Lista pessoas ativas (paginado) |
| GET | `/api/pessoas/{cpf}` | Busca uma pessoa pelo CPF |
| POST | `/api/pessoas` | Cadastra uma nova pessoa |
| PUT | `/api/pessoas/{cpf}` | Atualiza uma pessoa existente |
| PUT | `/api/pessoas/reativar/{cpf}` | Reativa uma pessoa desativada |
| DELETE | `/api/pessoas/{cpf}` | Remove uma pessoa (soft delete) |

### Telefones

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/telefones` | Lista telefones ativos (paginado); aceita filtro `?cpf=` |
| GET | `/api/telefones/{id}` | Busca um telefone pelo ID |
| GET | `/api/telefones/buscar?telefone=` | Busca telefones por número |
| POST | `/api/telefones` | Cadastra um novo telefone vinculado a uma pessoa |
| PUT | `/api/telefones/{id}` | Atualiza um telefone existente |
| DELETE | `/api/telefones/{id}` | Remove um telefone (soft delete) |

### Usuários (somente Admin)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/users` | Lista usuários |
| DELETE | `/api/users/{id}` | Desativa um usuário |

As listagens aceitam os parâmetros de query `page` (padrão: 1) e `size` (padrão: 10, máximo: 100).

## Decisões de Design

### Autenticação com JWT
A API usa JWT para autenticação. As senhas são armazenadas com BCrypt e a chave do JWT fica em uma variável de ambiente (`.env`), fora do código. Na validação do token são verificados assinatura, emissor, audiência e expiração.

### Soft Delete
Pessoas e telefones não são excluídos fisicamente. Ao deletar, o campo `IsActive` é alterado para `false` e o registro deixa de aparecer nas consultas. Escolhi manter os registros no banco para preservar o histórico. Pessoas podem ser reativadas através de um endpoint específico.

### CPF como identificador de Pessoa
Nas rotas de pessoa, uso o CPF como identificador (`/api/pessoas/{cpf}`) em vez do `Id` do banco. O CPF já identifica a pessoa no contexto da aplicação e evita expor um ID interno. A unicidade é garantida por um índice único no banco.

### Validação e normalização
CPF e telefone são validados antes de salvar — o CPF pelos dígitos verificadores, o telefone pelas regras de DDD, quantidade de dígitos e prefixo. Depois são normalizados para armazenar somente os números, evitando que formatos diferentes gerem registros distintos.

### Telefone único por pessoa
A duplicidade de telefone é verificada por pessoa: duas pessoas podem ter o mesmo número, mas a mesma pessoa não pode cadastrá-lo duas vezes.

### Controle de acesso por role
Usuários têm papel `Admin` ou `User`. As rotas de gerenciamento de usuários (`/api/users`) exigem papel `Admin`, verificado via `[Authorize(Roles = "Admin")]`. Um usuário comum autenticado recebe `403 Forbidden` nessas rotas.

### Outras práticas
Erros são tratados por um `IExceptionHandler` global, convertendo exceções em respostas `ProblemDetails` (`400`, `404`, `500`) sem `try/catch` repetido nos controllers. As entidades não são expostas diretamente — DTOs definem o que entra e sai de cada endpoint, evitando referência circular entre `Pessoa` e `Telefone` e impedindo o retorno de dados internos como o hash da senha.

## Limitações conhecidas

- O CPF não pode ser alterado via update — o campo é ignorado no `PUT` de pessoa.
- Ao reativar uma pessoa, todos os seus telefones inativos são reativados, incluindo os que haviam sido removidos individualmente antes.
- Um CPF ou número de telefone removido permanece reservado. Não há endpoint de reativação de telefone.
- Um administrador pode desativar a própria conta. Caso o sistema fique sem administradores ativos, o admin padrão é recriado na próxima inicialização.

## Testes

O projeto inclui testes automatizados (xUnit) para as validações de CPF e telefone:

```bash
dotnet test
```

## Estrutura do Projeto

```
Agenda/
├── Controllers/     # Endpoints da API (Pessoa, Telefone, Auth, Users)
├── Services/        # Regras de negócio e acesso a dados
├── Validators/      # Validação de CPF e telefone
├── Extensions/      # Configuração de JWT e OpenAPI
├── Converters/      # Conversor de data (dd/MM/yyyy)
├── Data/            # DbContext e configuração do banco
├── Models/          # Entidades de domínio (Pessoa, Telefone, User)
├── DTO/             # Objetos de transferência (request/response)
├── Middleware/      # Tratamento global de exceções
├── Migrations/      # Histórico de migrations do EF Core
└── Agenda.Tests/    # Testes automatizados (xUnit)
```

O projeto segue uma separação em camadas: os **Controllers** lidam apenas com HTTP, os **Services** concentram a lógica de negócio, e o acesso a dados é feito via **Entity Framework Core** através do `DbContext
