# 📋 Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/lang/pt-BR/).

---

## [1.1.0] - 2025-12-31

### 🔄 Modificado

#### Migração para PostgreSQL
- **BREAKING CHANGE**: Substituído SQLite por PostgreSQL
- Atualizado `Npgsql.EntityFrameworkCore.PostgreSQL` v8.0.0
- Removido `Microsoft.EntityFrameworkCore.Sqlite`
- Nova connection string: `Host=postgres;Database=financialmanager;Username=postgres;Password=postgres`

#### Docker Compose
- Adicionado container PostgreSQL 16 Alpine
- Configurado healthcheck para PostgreSQL
- Dependency entre aplicação e banco de dados
- Volume persistente `postgres_data` para dados do PostgreSQL
- Removido volume `./Data` (não mais necessário)

#### Migrations
- Removidas migrations antigas do SQLite
- Criada nova migration `InitialPostgreSQL` para PostgreSQL
- Estrutura de tabelas otimizada para PostgreSQL

#### Configuração
- Removida lógica específica do SQLite no `DependencyInjection.cs`
- Atualizado `appsettings.json` com connection string PostgreSQL
- Simplificado `Program.cs` (removido gerenciamento de diretório Data)

### ✨ Adicionado

#### PostgreSQL
- Container PostgreSQL 16 Alpine no docker-compose
- Porta 5432 exposta para acesso externo
- Credenciais padrão: postgres/postgres
- Database: financialmanager
- Volume persistente para dados

#### Documentação
- Instruções de acesso ao PostgreSQL no README
- Comandos para verificar banco de dados
- Credenciais e connection strings atualizadas

### 🗑️ Removido
- Dependência do SQLite
- Pasta `Data/` para banco SQLite
- Migrations antigas do SQLite
- Código específico para gerenciamento de caminho SQLite

### 🎯 Benefícios da Migração
- ✅ Banco de dados mais robusto e escalável
- ✅ Melhor performance em operações complexas
- ✅ Suporte a transações avançadas
- ✅ Pronto para produção
- ✅ Melhor suporte a tipos de dados
- ✅ Backup e recovery mais confiáveis

---

## [1.0.0] - 2025-12-31

### ✨ Adicionado

#### Core Features
- Sistema completo de gestão financeira pessoal
- Autenticação e autorização com ASP.NET Identity
- Arquitetura limpa (Clean Architecture) com 4 camadas bem definidas

#### Módulos Implementados
- **Dashboard**
  - Visão consolidada de receitas e despesas
  - Gráficos de categoria
  - Métricas principais (saldo total, receitas/despesas do mês)
  - Lista de movimentações recentes

- **Contas Bancárias**
  - CRUD completo de contas bancárias
  - Tipos: Corrente, Poupança, Digital
  - Controle de saldo inicial e atual
  - Histórico de transações
  - Página de detalhes com movimentações

- **Receitas**
  - CRUD de receitas
  - Categorização
  - Suporte a recorrência (Única, Mensal, Anual)
  - Vinculação a contas bancárias
  - Data de recebimento

- **Despesas**
  - CRUD de despesas
  - Múltiplas formas de pagamento (Dinheiro, Débito, Crédito)
  - Status (Pago/Pendente)
  - Categorização
  - Vinculação a conta bancária ou cartão de crédito

- **Cartões de Crédito**
  - CRUD de cartões
  - Configuração de limite
  - Dias de fechamento e vencimento
  - Controle de limite disponível
  - Transações com parcelamento
  - Visualização de faturas
  - Página de detalhes com transações

#### Infraestrutura
- **Banco de Dados**
  - SQLite para portabilidade
  - Entity Framework Core
  - Migrations automáticas
  - Data Seeding com usuário padrão

- **Docker**
  - Dockerfile otimizado (multi-stage build)
  - Docker Compose configurado
  - Persistência de dados via volumes
  - Porta 8080 exposta

#### Interface
- Design moderno com tema dark
- Layout responsivo com Bootstrap 5
- Navegação intuitiva
- Feedback visual (alertas de sucesso/erro)
- Ícones e badges para melhor UX

### 🏗️ Arquitetura

- **Domain Layer**
  - 7 entidades principais
  - 5 enumerações
  - Regras de negócio encapsuladas

- **Application Layer**
  - 5 interfaces de serviço
  - DTOs para todas as operações
  - Separação clara de responsabilidades

- **Infrastructure Layer**
  - 5 serviços implementados
  - Configuração do DbContext
  - Identity com ApplicationUser
  - Seed de dados inicial

- **Web Layer**
  - Blazor Server
  - Razor Pages para autenticação
  - Componentes compartilhados
  - CSS customizado

### 📚 Documentação

- README.md completo com:
  - Visão geral do projeto
  - Stack tecnológica
  - Instruções de instalação
  - Como rodar local e com Docker
  - Arquitetura detalhada
  - Roadmap de funcionalidades

- DEVELOPMENT.md com:
  - Guia para desenvolvedores
  - Estrutura do projeto
  - Padrões de código
  - Como contribuir
  - Comandos úteis

- LICENSE (MIT)
- CHANGELOG.md (este arquivo)

### 🔒 Segurança

- ASP.NET Identity configurado
- Data Protection com chaves persistidas
- Rotas protegidas com `[Authorize]`
- Senhas criptografadas
- HTTPS habilitado

### 🎨 Design

- Tema dark profissional
- Paleta de cores moderna
- Interface limpa e minimalista
- Cards e painéis estilizados
- Responsividade garantida

---

## 🔮 Próximas Versões

### [1.1.0] - Planejado

#### Melhorias de Qualidade
- [ ] Testes unitários para Domain e Application
- [ ] Testes de integração para Infrastructure
- [ ] Testes de UI para Web
- [ ] Validações avançadas nos formulários
- [ ] Tratamento global de erros
- [ ] Logging estruturado (Serilog)

### [1.2.0] - Planejado

#### Features Avançadas
- [ ] Relatórios em PDF
- [ ] Exportação para Excel
- [ ] Importação de extratos bancários (CSV/OFX)
- [ ] Gráficos interativos (Chart.js ou ApexCharts)
- [ ] Categorias personalizadas pelo usuário
- [ ] Tags para transações

### [1.3.0] - Planejado

#### Funcionalidades Financeiras
- [ ] Metas financeiras
- [ ] Orçamento mensal
- [ ] Projeções futuras
- [ ] Comparativo mensal/anual
- [ ] Alertas de vencimento
- [ ] Notificações por email

### [2.0.0] - Futuro

#### Arquitetura e Deploy
- [ ] Migração para Blazor WebAssembly
- [ ] API REST separada
- [ ] Autenticação JWT
- [ ] Suporte a PostgreSQL/SQL Server
- [ ] Pipeline CI/CD (GitHub Actions)
- [ ] Deploy em Azure/AWS
- [ ] Monitoramento (Application Insights)
- [ ] Cache distribuído (Redis)

---

## 🎯 Convenções de Versionamento

Este projeto usa [Semantic Versioning](https://semver.org/):

- **MAJOR** (X.0.0): Mudanças incompatíveis na API
- **MINOR** (0.X.0): Novas funcionalidades compatíveis
- **PATCH** (0.0.X): Correções de bugs

---

## 📝 Tipos de Mudanças

- **Adicionado**: Novas funcionalidades
- **Modificado**: Alterações em funcionalidades existentes
- **Descontinuado**: Funcionalidades que serão removidas
- **Removido**: Funcionalidades removidas
- **Corrigido**: Correções de bugs
- **Segurança**: Correções de vulnerabilidades

---

**Última atualização:** 31/12/2025
