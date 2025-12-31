# 🔄 Guia de Migração: SQLite → PostgreSQL

Este documento descreve a migração realizada do SQLite para PostgreSQL no projeto Financial Manager.

---

## 📋 Resumo das Alterações

### ✅ Concluído

1. **Pacotes NuGet**
   - ❌ Removido: `Microsoft.EntityFrameworkCore.Sqlite`
   - ✅ Adicionado: `Npgsql.EntityFrameworkCore.PostgreSQL v8.0.0`

2. **Configuração do DbContext**
   - Atualizado para usar `.UseNpgsql()` ao invés de `.UseSqlite()`
   - Removida lógica de resolução de caminho do SQLite

3. **Connection String**
   - **Antes:** `Data Source=Data/financial.db`
   - **Depois:** `Host=localhost;Database=financialmanager;Username=postgres;Password=postgres`

4. **Docker Compose**
   - Adicionado container PostgreSQL 16 Alpine
   - Configurado volume persistente: `postgres_data`
   - Healthcheck configurado
   - Dependência entre app e database

5. **Migrations**
   - ✅ Removidas migrations antigas do SQLite
   - ✅ Criada nova migration: `InitialPostgreSQL`

6. **Estrutura de Arquivos**
   - ❌ Removida pasta `Data/` (SQLite)
   - ✅ Criada pasta `DataProtectionKeys/` (chaves de criptografia)

---

## 🐳 Estrutura Docker Atualizada

```yaml
services:
  postgres:
    image: postgres:16-alpine
    ports:
      - "5432:5432"
    environment:
      POSTGRES_DB: financialmanager
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      
  financialmanager:
    depends_on:
      postgres:
        condition: service_healthy
    environment:
      ConnectionStrings__Default: Host=postgres;Database=financialmanager;Username=postgres;Password=postgres
```

---

## 🚀 Como Usar Após Migração

### Execução Local (com PostgreSQL instalado)

1. **Instalar PostgreSQL 16**
   - Windows: https://www.postgresql.org/download/windows/
   - Mac: `brew install postgresql@16`
   - Linux: `sudo apt install postgresql-16`

2. **Criar Database**
```sql
psql -U postgres
CREATE DATABASE financialmanager;
\q
```

3. **Atualizar Connection String** (se necessário)
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=financialmanager;Username=postgres;Password=SUA_SENHA"
  }
}
```

4. **Aplicar Migrations**
```bash
cd FinancialManager.Infrastructure
dotnet ef database update --startup-project ../FinancialManager.Web
```

5. **Executar Aplicação**
```bash
cd ../FinancialManager.Web
dotnet run
```

### Execução com Docker (Recomendado)

1. **Iniciar tudo com um comando**
```bash
docker-compose up -d --build
```

2. **Verificar status**
```bash
docker-compose ps
```

3. **Acessar aplicação**
```
http://localhost:8080
```

---

## 🗄️ Comandos Úteis PostgreSQL

### Acessar PostgreSQL via Docker
```bash
docker exec -it financialmanager-postgres psql -U postgres -d financialmanager
```

### Comandos SQL Úteis
```sql
-- Listar tabelas
\dt

-- Descrever tabela
\d BankAccounts

-- Ver dados
SELECT * FROM "AspNetUsers";

-- Contar registros
SELECT COUNT(*) FROM "Expenses";

-- Sair
\q
```

### Backup e Restore
```bash
# Backup
docker exec financialmanager-postgres pg_dump -U postgres financialmanager > backup.sql

# Restore
docker exec -i financialmanager-postgres psql -U postgres financialmanager < backup.sql
```

---

## 🔍 Verificação Pós-Migração

### ✅ Checklist

- [ ] Container PostgreSQL rodando: `docker ps | grep postgres`
- [ ] Database criado: `docker exec financialmanager-postgres psql -U postgres -l`
- [ ] Tabelas criadas: `docker exec financialmanager-postgres psql -U postgres -d financialmanager -c "\dt"`
- [ ] Migrations aplicadas: Verificar tabela `__EFMigrationsHistory`
- [ ] Usuário admin criado: Login com `admin@admin.com` / `123456`
- [ ] Aplicação acessível em http://localhost:8080

### Verificar Tabelas Criadas
```bash
docker exec financialmanager-postgres psql -U postgres -d financialmanager -c "
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema='public' 
ORDER BY table_name;
"
```

**Resultado esperado:**
- AspNetRoleClaims
- AspNetRoles
- AspNetUserClaims
- AspNetUserLogins
- AspNetUserRoles
- AspNetUserTokens
- AspNetUsers
- BankAccounts
- BankTransactions
- CreditCardTransactions
- CreditCards
- Expenses
- Incomes
- __EFMigrationsHistory

---

## 📊 Comparação: SQLite vs PostgreSQL

| Aspecto | SQLite | PostgreSQL |
|---------|--------|------------|
| **Tipo** | Arquivo local | Servidor de BD |
| **Concorrência** | Limitada | Excelente |
| **Transações** | Básicas | Avançadas (ACID) |
| **Escalabilidade** | Pequenos projetos | Produção enterprise |
| **Performance** | Boa para leitura | Otimizada para escrita/leitura |
| **Tipos de dados** | Limitados | Extensos (JSON, Array, etc) |
| **Backup** | Copiar arquivo | Ferramentas nativas (pg_dump) |
| **Replicação** | Não suportada | Suportada nativamente |
| **Conexões simultâneas** | Uma escrita por vez | Múltiplas conexões |

---

## 🐛 Troubleshooting

### Problema: Container PostgreSQL não inicia

**Solução:**
```bash
# Ver logs
docker logs financialmanager-postgres

# Reiniciar
docker-compose restart postgres
```

### Problema: Aplicação não conecta no PostgreSQL

**Verificar:**
1. PostgreSQL está rodando: `docker ps`
2. Connection string está correta
3. PostgreSQL passou no healthcheck: `docker inspect financialmanager-postgres`

**Testar conexão:**
```bash
docker exec financialmanager-postgres pg_isready -U postgres
```

### Problema: Migrations não aplicam

**Solução:**
```bash
# Remover migration atual
cd FinancialManager.Infrastructure
dotnet ef migrations remove --startup-project ../FinancialManager.Web

# Recriar
dotnet ef migrations add InitialPostgreSQL --startup-project ../FinancialManager.Web

# Aplicar
dotnet ef database update --startup-project ../FinancialManager.Web
```

### Problema: Dados antigos do SQLite

Os dados do SQLite **não são migrados automaticamente**. Se precisar migrar dados:

1. Exportar do SQLite
2. Converter para SQL PostgreSQL
3. Importar no PostgreSQL

**Nota:** Como este é um projeto novo, não há necessidade de migração de dados.

---

## 🎯 Próximos Passos

### Melhorias Recomendadas

1. **Segurança**
   - [ ] Alterar senha padrão do PostgreSQL
   - [ ] Usar secrets para credenciais
   - [ ] Configurar SSL para PostgreSQL

2. **Performance**
   - [ ] Adicionar índices nas tabelas
   - [ ] Configurar connection pooling
   - [ ] Otimizar queries N+1

3. **Backup**
   - [ ] Configurar backup automático
   - [ ] Testar restore
   - [ ] Documentar estratégia de backup

4. **Monitoramento**
   - [ ] Adicionar pgAdmin ou outro cliente visual
   - [ ] Configurar logs de queries lentas
   - [ ] Monitorar uso de recursos

---

## 📚 Referências

- [Npgsql Documentation](https://www.npgsql.org/efcore/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [EF Core PostgreSQL Provider](https://www.npgsql.org/efcore/index.html)
- [Docker PostgreSQL Image](https://hub.docker.com/_/postgres)

---

**Migração realizada em:** 31/12/2025  
**Versão:** 1.1.0  
**Status:** ✅ Concluída com sucesso
