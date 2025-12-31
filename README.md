# 💰 Financial Manager

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![SQLite](https://img.shields.io/badge/SQLite-3-003B57?logo=sqlite)](https://www.sqlite.org/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Sistema completo de gestão financeira pessoal desenvolvido com **C# (.NET 8)** e **Blazor Server**, seguindo os princípios de **Clean Architecture**.

O projeto oferece controle de despesas, receitas, cartões de crédito e contas bancárias, com dashboard analítico para visualização de dados financeiros.

---

## 🎯 Visão Geral

**Financial Manager** é uma aplicação web moderna para gestão financeira pessoal que permite:

- ✅ Controle completo de **receitas** e **despesas**
- 💳 Gestão de **cartões de crédito** com controle de faturas e parcelas
- 🏦 Gerenciamento de **contas bancárias** e transferências
- 📊 **Dashboard** com visão consolidada das finanças
- 🔐 Autenticação segura com **ASP.NET Identity**
- 🐳 Containerização com **Docker**

---

## 🛠️ Tecnologias

### Backend
- **C# .NET 8** - Framework principal
- **Blazor Server** - Interface web interativa
- **Entity Framework Core** - ORM para persistência de dados
- **ASP.NET Identity** - Sistema de autenticação e autorização
- **PostgreSQL** - Banco de dados relacional robusto e escalável

### Frontend
- **Blazor Components** - Componentes reativos
- **Bootstrap 5** - Framework CSS responsivo
- **Custom CSS** - Estilização moderna com dark theme

### Infrastructure
- **Docker** - Containerização
- **Docker Compose** - Orquestração de containers

---

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture**, organizado em camadas bem definidas:

```
FinancialManager/
│
├── FinancialManager.Domain/              # Camada de Domínio
│   ├── Entities/                         # Entidades do negócio
│   │   ├── BaseEntity.cs
│   │   ├── BankAccount.cs
│   │   ├── BankTransaction.cs
│   │   ├── CreditCard.cs
│   │   ├── CreditCardTransaction.cs
│   │   ├── Expense.cs
│   │   └── Income.cs
│   └── Enums/                            # Enumerações
│       ├── AccountType.cs
│       ├── BankTransactionType.cs
│       ├── ExpenseStatus.cs
│       ├── PaymentMethod.cs
│       └── RecurrenceType.cs
│
├── FinancialManager.Application/         # Camada de Aplicação
│   ├── DTOs/                             # Data Transfer Objects
│   │   ├── BankAccountDtos.cs
│   │   ├── CreditCardDtos.cs
│   │   ├── DashboardDtos.cs
│   │   ├── ExpenseDtos.cs
│   │   └── IncomeDtos.cs
│   └── Services/                         # Interfaces de serviços
│       ├── IBankAccountService.cs
│       ├── ICreditCardService.cs
│       ├── IDashboardService.cs
│       ├── IExpenseService.cs
│       └── IIncomeService.cs
│
├── FinancialManager.Infrastructure/      # Camada de Infraestrutura
│   ├── Persistence/                      # Persistência de dados
│   │   ├── ApplicationDbContext.cs
│   │   ├── Migrations/
│   │   └── Seed/
│   ├── Services/                         # Implementação dos serviços
│   │   ├── BankAccountService.cs
│   │   ├── CreditCardService.cs
│   │   ├── DashboardService.cs
│   │   ├── ExpenseService.cs
│   │   └── IncomeService.cs
│   └── Identity/                         # Identidade
│       └── ApplicationUser.cs
│
└── FinancialManager.Web/                 # Camada de Apresentação
    ├── Pages/                            # Páginas Blazor
    │   ├── BankAccounts/
    │   ├── CreditCards/
    │   ├── Expenses/
    │   ├── Incomes/
    │   └── Dashboard.razor
    ├── Shared/                           # Componentes compartilhados
    └── wwwroot/                          # Arquivos estáticos
```

### Princípios Aplicados

- ✅ **Separation of Concerns** - Responsabilidades bem definidas
- ✅ **Dependency Inversion** - Dependências apontam para abstrações
- ✅ **Single Responsibility** - Cada classe tem uma única responsabilidade
- ✅ **Clean Code** - Código legível e manutenível

---

## 🚀 Como Rodar

### 📋 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [PostgreSQL 16](https://www.postgresql.org/download/) (para execução local)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (recomendado, para rodar com Docker)

### 🏃 Execução Local

1. **Clone o repositório**
```bash
git clone https://github.com/seu-usuario/FinancialManager.git
cd FinancialManager
```

2. **Configure o PostgreSQL**
   - Instale o PostgreSQL
   - Crie um banco chamado `financialmanager`
   - Atualize a connection string em `appsettings.json` se necessário

3. **Restaure as dependências**
```bash
dotnet restore
```

4. **Execute as migrations**
```bash
cd FinancialManager.Infrastructure
dotnet ef database update --startup-project ../FinancialManager.Web
```

5. **Execute a aplicação**
```bash
cd ../FinancialManager.Web
dotnet run
```

6. **Acesse no navegador**
```
https://localhost:5001
```

### 🐳 Execução com Docker (Recomendado)

A forma mais simples de rodar o projeto é usando Docker Compose, que já inclui PostgreSQL configurado:

1. **Build e execução**
```bash
docker-compose up -d --build
```

2. **Acesse no navegador**
```
http://localhost:8080
```

3. **Verificar logs**
```bash
# Logs da aplicação
docker logs financialmanager-app

# Logs do PostgreSQL
docker logs financialmanager-postgres
```

4. **Parar os containers**
```bash
docker-compose down
```

5. **Parar e remover volumes (limpar banco de dados)**
```bash
docker-compose down -v
```

### 🗄️ Acesso ao PostgreSQL

Para acessar o banco de dados PostgreSQL diretamente:

```bash
# Via Docker
docker exec -it financialmanager-postgres psql -U postgres -d financialmanager

# Via cliente local (se PostgreSQL instalado)
psql -h localhost -p 5432 -U postgres -d financialmanager
```

**Credenciais do PostgreSQL:**
- Host: `localhost` (ou `postgres` dentro do Docker)
- Port: `5432`
- Database: `financialmanager`
- Username: `postgres`
- Password: `postgres`

### 📝 Credenciais Padrão

Ao executar pela primeira vez, o sistema cria um usuário padrão:

- **Email:** `admin@financialmanager.com`
- **Senha:** `Admin@123`

> ⚠️ **Importante:** Altere as credenciais após o primeiro login em ambiente de produção!

---

## 💡 Funcionalidades

### 📊 Dashboard
- Visão consolidada do saldo total
- Receitas e despesas do mês
- Gráfico de despesas por categoria
- Indicador de uso do cartão de crédito
- Faturas próximas ao vencimento

### 💰 Receitas
- Cadastro com título, valor, categoria e data
- Vinculação a conta bancária
- Suporte a receitas recorrentes
- Listagem e filtros

### 💸 Despesas
- Cadastro com múltiplas formas de pagamento:
  - **Dinheiro (Cash)**
  - **Débito (Debit)**
  - **Crédito (Credit)**
- Status: Pago / Pendente
- Categorização
- Vinculação a conta bancária ou cartão

### 💳 Cartões de Crédito
- Cadastro com nome, banco, limite
- Dias de fechamento e vencimento
- Lançamentos com parcelamento automático
- Controle de limite disponível
- Gestão de faturas (atual e futuras)

### 🏦 Contas Bancárias
- Tipos: Corrente, Poupança, Digital
- Saldo inicial e atual
- Movimentações (receitas, despesas, transferências)
- Atualização automática do saldo
- Transferências entre contas

---

## 🎨 Interface

A interface foi desenvolvida com foco em:

- 🌙 **Dark Theme Moderno** - Visual elegante e profissional
- 📱 **Responsivo** - Adaptável a diferentes dispositivos
- 🎯 **UX Intuitiva** - Navegação simples e eficiente
- ⚡ **Performance** - Carregamento rápido com Blazor Server

---

## 🔐 Segurança

- ✅ Autenticação com **ASP.NET Identity**
- ✅ Proteção de rotas com `[Authorize]`
- ✅ Senhas criptografadas
- ✅ Data Protection configurado
- ✅ HTTPS habilitado (local)
- ✅ PostgreSQL com conexões seguras

---

## 📦 Estrutura de Dados (PostgreSQL)

### Principais Entidades

#### BankAccount (Conta Bancária)
- Nome, Banco, Tipo
- Saldo inicial e atual
- Relacionamentos: Receitas, Despesas, Transações

#### CreditCard (Cartão de Crédito)
- Nome, Banco, Limite
- Dias de fechamento e vencimento
- Relacionamentos: Transações, Despesas

#### Income (Receita)
- Título, Valor, Categoria
- Data e recorrência
- Conta bancária vinculada

#### Expense (Despesa)
- Título, Valor, Categoria
- Forma de pagamento e status
- Conta bancária ou cartão vinculado

---

## 🗺️ Roadmap

### ✅ Fase 1 - Core (Concluído)
- [x] Estrutura do projeto (Clean Architecture)
- [x] Entidades de domínio
- [x] Persistência com EF Core + SQLite
- [x] Autenticação com Identity
- [x] CRUD de contas bancárias
- [x] CRUD de cartões de crédito
- [x] CRUD de receitas e despesas
- [x] Dashboard básico

### 🚧 Fase 2 - Melhorias (Em progresso)
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Validações avançadas
- [ ] Tratamento de erros global
- [ ] Logs estruturados

### 📅 Fase 3 - Features Avançadas
- [ ] Relatórios em PDF
- [ ] Exportação para Excel
- [ ] Metas financeiras
- [ ] Notificações de vencimento
- [ ] Gráficos interativos (Chart.js)
- [ ] Categorias personalizadas

### 🌐 Fase 4 - Deploy e CI/CD
- [ ] Pipeline CI/CD
- [ ] Deploy em Azure/AWS
- [ ] Monitoramento (Application Insights)
- [ ] Backup automático

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 👨‍💻 Autor

Desenvolvido como projeto de portfólio demonstrando boas práticas de desenvolvimento .NET.

**Stack:** C# | .NET 8 | Blazor | EF Core | Clean Architecture | Docker

---

## 📞 Contato

Para dúvidas ou sugestões, abra uma issue no repositório.

---

<div align="center">

**⭐ Se este projeto foi útil, deixe uma estrela!**

Made with ❤️ and C#

</div>
