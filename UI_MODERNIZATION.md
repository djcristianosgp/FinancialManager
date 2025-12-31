# Modernização da UI com MudBlazor

## 📋 Resumo

Modernização completa da interface do usuário utilizando **MudBlazor 8.15.0**, framework de componentes moderno e elegante para Blazor. A aplicação agora possui design profissional estilo SaaS, modo escuro por padrão, e biblioteca de componentes reutilizáveis.

---

## 🎨 Design System

### Paleta de Cores

#### Tema Escuro (Padrão)
- **Primary**: `#6366F1` (Indigo claro)
- **Secondary**: `#22C55E` (Verde)
- **Tertiary**: `#F97316` (Laranja)
- **Success**: `#22C55E`
- **Info**: `#3B82F6`
- **Warning**: `#F59E0B`
- **Error**: `#EF4444`
- **Background**: `#0F172A` (Dark slate)
- **Surface**: `#1E293B` (Lighter dark slate)

### Tipografia
- **Font Family**: Inter, Roboto, Helvetica, Arial, sans-serif
- **Border Radius**: 8px (cantos arredondados)
- **Elevação**: Sombras suaves e modernas

---

## 🏗️ Estrutura Criada

### 1. Theme System
**Arquivo**: `Theme/AppTheme.cs`
- Tema customizado com paleta dark
- Propriedades de layout configuradas
- Facilmente extensível para tema claro

### 2. MainLayout Modernizado
**Arquivo**: `Shared/MainLayout.razor`

**Recursos**:
- ✅ **MudAppBar**: Cabeçalho fixo com ações
- ✅ **MudDrawer**: Sidebar minimizável com hover expand
- ✅ **MudNavMenu**: Navegação com ícones Material Design
- ✅ **Toggle Dark Mode**: Alternar entre modo claro/escuro
- ✅ **Menu de Usuário**: Dropdown com perfil, configurações e logout
- ✅ **Layout Responsivo**: Adaptável para mobile e desktop

### 3. Biblioteca de Componentes Reutilizáveis
**Diretório**: `Components/Common/`

#### AppCard
- Card padrão com título, ícone e ações
- Suporta header actions e card actions
- Elevação configurável
- Classes CSS customizáveis

**Uso**:
```razor
<AppCard Title="Meu Card" 
         Icon="@Icons.Material.Filled.Dashboard"
         IconColor="Color.Primary"
         Elevation="2">
    <p>Conteúdo do card</p>
</AppCard>
```

#### AppStatCard
- Card estatístico com gradientes
- Exibe valor, label, ícone
- Indicador de tendência (positivo/negativo)
- 5 variantes de cores (Primary, Success, Error, Warning, Info)
- Animação hover (eleva 4px)

**Uso**:
```razor
<AppStatCard Label="Saldo Total"
            Value="R$ 10.000,00"
            Icon="@Icons.Material.Filled.AccountBalance"
            Variant="Color.Primary"
            Trend="+5.2% este mês"
            TrendPositive="true" />
```

#### AppButton
- Botão padronizado com loading state
- Suporta ícones (start/end)
- Variantes: Filled, Outlined, Text
- FullWidth opcional
- Desabilitar/loading automaticamente

**Uso**:
```razor
<AppButton Color="Color.Primary" 
          StartIcon="@Icons.Material.Filled.Save"
          Loading="@_isSaving"
          OnClick="@SaveData">
    Salvar
</AppButton>
```

#### AppDialog
- Dialog padrão com botões de ação
- Ícone e título configuráveis
- Botões Cancelar/Confirmar personalizáveis
- Loading state no botão de confirmação

**Uso**:
```razor
<AppDialog Title="Editar Item"
          Icon="@Icons.Material.Filled.Edit"
          IconColor="Color.Primary"
          OnConfirm="@HandleConfirm"
          OnCancel="@HandleCancel">
    <p>Conteúdo do diálogo</p>
</AppDialog>
```

#### AppConfirmDialog
- Dialog de confirmação simplificado
- Ideal para ações destrutivas (deletar, remover)
- Alerta visual automático
- Botão de confirmação em vermelho por padrão

**Uso**:
```razor
<AppConfirmDialog Title="Excluir Item?"
                 Message="Esta ação não pode ser desfeita."
                 Details="Todos os dados relacionados serão removidos."
                 ConfirmText="Sim, excluir"
                 ConfirmColor="Color.Error"
                 OnConfirm="@DeleteItem" />
```

---

## 📄 Dashboard Refatorado

### Arquivo: `Pages/Dashboard.razor`

**Mudanças Principais**:
1. ✅ **4 Stat Cards** com gradientes e ícones
   - Saldo Total (Primary)
   - Receitas do Mês (Success)
   - Despesas do Mês (Error)
   - Uso do Crédito (Info)

2. ✅ **Card de Despesas por Categoria**
   - Progress bars com cores do tema
   - Chips com valores monetários
   - Mensagem quando não há dados

3. ✅ **Tabela de Movimentações Recentes**
   - MudTable responsiva
   - Chips para categorias
   - Valores coloridos (verde/vermelho)
   - Hover effect

4. ✅ **Card de Próximas Faturas**
   - Lista densa com detalhes
   - Data de vencimento
   - Valores em destaque
   - Alerta quando não há faturas

5. ✅ **Card de Uso do Crédito**
   - Progress bar com cores dinâmicas
   - Verde (<70%), Amarelo (70-90%), Vermelho (>90%)
   - Alerta quando limite alto
   - Exibição de valores atual/limite

---

## 🚀 Como Usar

### Iniciar Aplicação
```bash
docker-compose up -d --build
```

### Acessar
- **URL**: http://localhost:8080
- **Usuário**: admin@admin.com
- **Senha**: 123456

### Build Local
```bash
cd c:\AtualDev\Fontes\Web\FinancialManager
dotnet build
dotnet run --project FinancialManager.Web
```

---

## 📦 Pacotes Instalados

```xml
<PackageReference Include="MudBlazor" Version="8.15.0" />
```

---

## 🔧 Configuração MudBlazor

### Program.cs
```csharp
using MudBlazor.Services;

builder.Services.AddMudServices();
```

### _Host.cshtml
```html
<!-- Font Inter -->
<link rel="preconnect" href="https://fonts.googleapis.com">
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap" rel="stylesheet">

<!-- MudBlazor CSS -->
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />

<!-- MudBlazor JS -->
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

### _Imports.razor
```razor
@using MudBlazor
@using FinancialManager.Web.Components.Common
@using FinancialManager.Application.Dtos
@using FinancialManager.Domain.Enums
```

---

## ✅ Status de Implementação

### Completo ✅
- [x] Instalação e configuração MudBlazor
- [x] Tema customizado (AppTheme.cs)
- [x] MainLayout com sidebar e topbar
- [x] Toggle dark mode funcional
- [x] Menu de usuário com dropdown
- [x] AppCard
- [x] AppStatCard com gradientes
- [x] AppButton com loading
- [x] AppDialog
- [x] AppConfirmDialog
- [x] Dashboard completamente refatorado
- [x] Compilação sem erros

### Próximas Etapas 🔄
- [ ] Refatorar página Incomes (Receitas)
- [ ] Refatorar página Expenses (Despesas)
- [ ] Refatorar página Bank Accounts
- [ ] Refatorar página Credit Cards
- [ ] Implementar tema claro
- [ ] Adicionar gráficos (ApexCharts ou MudBlazor Charts)
- [ ] Animações de transição entre páginas

---

## 🎯 Boas Práticas Aplicadas

1. **Componentes Reutilizáveis**: Todos os componentes base podem ser usados em qualquer página
2. **Type Safety**: Tipos genéricos configurados corretamente (MudChip<T>, MudList<T>)
3. **Responsividade**: MudGrid com breakpoints (xs, sm, md, lg)
4. **Acessibilidade**: Ícones Material Design semânticos
5. **Performance**: Componentes lazy loading onde aplicável
6. **Manutenibilidade**: Código limpo, comentado, bem estruturado
7. **Design Consistente**: Paleta de cores unificada em todos componentes

---

## 📖 Documentação MudBlazor

- **Site Oficial**: https://mudblazor.com
- **API Reference**: https://mudblazor.com/api
- **Componentes**: https://mudblazor.com/components/
- **Exemplos**: https://mudblazor.com/getting-started/examples

---

## 🐛 Troubleshooting

### Erro: "The type of component 'MudChip' cannot be inferred"
**Solução**: Adicionar parâmetro de tipo genérico
```razor
<!-- ❌ Errado -->
<MudChip Size="Size.Small">Texto</MudChip>

<!-- ✅ Correto -->
<MudChip T="string" Size="Size.Small">Texto</MudChip>
```

### Erro: "IDialogReference não contém definição para Cancel"
**Solução**: Usar `MudDialog?.Close(DialogResult.Cancel())` ao invés de `MudDialog?.Cancel()`

### Container não inicia
**Solução**: Verificar logs
```bash
docker-compose logs -f financialmanager-app
```

---

## 👨‍💻 Autor

**Financial Manager Team**  
Portfolio Project - Clean Architecture + MudBlazor
