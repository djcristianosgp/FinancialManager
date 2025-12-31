# 📝 Guia de Desenvolvimento

Este documento contém informações técnicas para desenvolvedores que desejam contribuir com o projeto **Financial Manager**.

---

## 🏗️ Estrutura do Projeto

### Camadas da Arquitetura

O projeto segue **Clean Architecture** com as seguintes camadas:

#### 1. Domain (Domínio)
- **Localização:** `FinancialManager.Domain`
- **Responsabilidade:** Contém as regras de negócio puras e entidades
- **Dependências:** Nenhuma
- **Conteúdo:**
  - `Entities/` - Entidades do domínio
  - `Enums/` - Enumerações do sistema

#### 2. Application (Aplicação)
- **Localização:** `FinancialManager.Application`
- **Responsabilidade:** Define interfaces e DTOs
- **Dependências:** Domain
- **Conteúdo:**
  - `DTOs/` - Data Transfer Objects
  - `Services/` - Interfaces de serviços

#### 3. Infrastructure (Infraestrutura)
- **Localização:** `FinancialManager.Infrastructure`
- **Responsabilidade:** Implementação técnica (banco de dados, serviços)
- **Dependências:** Domain, Application
- **Conteúdo:**
  - `Persistence/` - Contexto do EF Core e Migrations
  - `Services/` - Implementação dos serviços
  - `Identity/` - Configuração do ASP.NET Identity

#### 4. Web (Apresentação)
- **Localização:** `FinancialManager.Web`
- **Responsabilidade:** Interface do usuário
- **Dependências:** Application, Infrastructure
- **Conteúdo:**
  - `Pages/` - Páginas Blazor e Razor Pages
  - `Shared/` - Componentes compartilhados
  - `wwwroot/` - Arquivos estáticos (CSS, JS, imagens)

---

## 🛠️ Ferramentas Necessárias

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (recomendado) ou **VS Code**
- **SQL Server Management Studio** ou **DB Browser for SQLite** (opcional)
- **Docker Desktop** (para containerização)
- **Git** para controle de versão

### Extensões Recomendadas para VS Code
- C# Dev Kit
- Blazor WASM Debugging
- SQLite Viewer
- Docker

---

## 🚀 Configuração do Ambiente

### 1. Clone o Repositório
```bash
git clone https://github.com/seu-usuario/FinancialManager.git
cd FinancialManager
```

### 2. Restaure as Dependências
```bash
dotnet restore
```

### 3. Configure o Banco de Dados

O projeto usa **SQLite** por padrão. O banco é criado automaticamente na primeira execução.

Para recriar o banco:
```bash
cd FinancialManager.Infrastructure
dotnet ef database drop -f --startup-project ../FinancialManager.Web
dotnet ef database update --startup-project ../FinancialManager.Web
```

### 4. Execute a Aplicação
```bash
cd FinancialManager.Web
dotnet run
```

Acesse: https://localhost:5001

---

## 📊 Banco de Dados

### Migrations

#### Criar uma nova Migration
```bash
cd FinancialManager.Infrastructure
dotnet ef migrations add NomeDaMigration --startup-project ../FinancialManager.Web
```

#### Aplicar Migrations
```bash
dotnet ef database update --startup-project ../FinancialManager.Web
```

#### Reverter Migration
```bash
dotnet ef database update NomeDaMigrationAnterior --startup-project ../FinancialManager.Web
```

#### Remover última Migration
```bash
dotnet ef migrations remove --startup-project ../FinancialManager.Web
```

---

## 🧪 Testes

### Estrutura de Testes (Futuro)
```
FinancialManager.Tests/
├── Domain.Tests/          # Testes de entidades e regras de negócio
├── Application.Tests/     # Testes de serviços
├── Infrastructure.Tests/  # Testes de integração com BD
└── Web.Tests/            # Testes de UI
```

### Executar Testes
```bash
dotnet test
```

---

## 📦 Docker

### Build da Imagem
```bash
docker build -t financial-manager:latest .
```

### Executar Container
```bash
docker run -d -p 8080:8080 -v $(pwd)/Data:/app/Data financial-manager:latest
```

### Docker Compose
```bash
# Iniciar
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar
docker-compose down
```

---

## 🎨 Padrões de Código

### Nomenclatura

- **Classes e Métodos:** PascalCase
  ```csharp
  public class BankAccountService { }
  public async Task<List<Income>> GetAllAsync() { }
  ```

- **Variáveis e Parâmetros:** camelCase
  ```csharp
  var bankAccount = new BankAccount();
  public void ProcessTransaction(decimal amount) { }
  ```

- **Constantes:** UPPER_CASE
  ```csharp
  private const int MAX_INSTALLMENTS = 12;
  ```

### Organização de Código

- Sempre use `async`/`await` para operações assíncronas
- Prefira LINQ sobre loops quando apropriado
- Use DTOs para comunicação entre camadas
- Evite lógica de negócio na camada Web
- Use Dependency Injection

### Exemplo de Service
```csharp
public class ExampleService : IExampleService
{
    private readonly ApplicationDbContext _context;
    
    public ExampleService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<ItemDto>> GetAllAsync()
    {
        return await _context.Items
            .OrderBy(i => i.Name)
            .Select(i => new ItemDto(i.Id, i.Name))
            .ToListAsync();
    }
}
```

---

## 🐛 Debugging

### Visual Studio
1. Pressione **F5** para iniciar em modo debug
2. Use breakpoints clicando na margem esquerda
3. Use **F10** (Step Over) e **F11** (Step Into)

### VS Code
1. Configure `.vscode/launch.json`:
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/FinancialManager.Web/bin/Debug/net8.0/FinancialManager.Web.dll",
            "args": [],
            "cwd": "${workspaceFolder}/FinancialManager.Web",
            "stopAtEntry": false,
            "serverReadyAction": {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            }
        }
    ]
}
```

---

## 📝 Commit Messages

Siga o padrão **Conventional Commits**:

```
tipo(escopo): descrição curta

Descrição mais detalhada se necessário

Resolves #123
```

### Tipos:
- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Documentação
- `style`: Formatação (sem mudança de código)
- `refactor`: Refatoração
- `test`: Adição de testes
- `chore`: Tarefas de manutenção

### Exemplos:
```bash
git commit -m "feat(expenses): adiciona filtro por categoria"
git commit -m "fix(dashboard): corrige cálculo de saldo total"
git commit -m "docs(readme): atualiza instruções de instalação"
```

---

## 🔒 Segurança

### Boas Práticas
- Nunca commite senhas ou secrets
- Use User Secrets para desenvolvimento:
  ```bash
  dotnet user-secrets init --project FinancialManager.Web
  dotnet user-secrets set "ConnectionStrings:Default" "sua-string"
  ```
- Valide todas as entradas do usuário
- Use `[Authorize]` em todas as páginas que requerem autenticação

---

## 📚 Recursos Úteis

### Documentação Oficial
- [.NET 8](https://docs.microsoft.com/dotnet/)
- [Blazor](https://docs.microsoft.com/aspnet/core/blazor/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)

### Tutoriais
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://www.digitalocean.com/community/conceptual_articles/s-o-l-i-d-the-first-five-principles-of-object-oriented-design)

---

## 🤝 Como Contribuir

1. **Fork** o projeto
2. Crie uma **branch** para sua feature: `git checkout -b feature/MinhaFeature`
3. **Commit** suas mudanças: `git commit -m 'feat: adiciona MinhaFeature'`
4. **Push** para a branch: `git push origin feature/MinhaFeature`
5. Abra um **Pull Request**

### Checklist antes do PR
- [ ] Código compila sem erros
- [ ] Código segue os padrões do projeto
- [ ] Testes foram adicionados/atualizados
- [ ] Documentação foi atualizada
- [ ] Commit messages seguem o padrão

---

## 📞 Suporte

- 🐛 **Bugs:** Abra uma [issue](https://github.com/seu-usuario/FinancialManager/issues)
- 💡 **Ideias:** Abra uma [discussion](https://github.com/seu-usuario/FinancialManager/discussions)
- 📧 **Email:** seu-email@example.com

---

**Happy Coding! 🚀**
