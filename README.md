
# 💰 FinanceiroApp — Desafio Full Stack

Sistema de controle financeiro pessoal desenvolvido com **Vue 3 + Nuxt 3 + Vuetify** no frontend e **C# .NET 8 + EF Core** no backend.

---

## 📐 Modelagem de Dados

### Entidades principais

| Entidade     | Descrição                                               |
|-------------|--------------------------------------------------------|
| `Usuarios`  | Cadastro de usuários do sistema                         |
| `Contas`    | Contas bancárias, cartões e carteiras do usuário        |
| `Categorias`| Classificações de receitas e despesas                  |
| `Transacoes`| Movimentações financeiras vinculadas a conta/categoria |
| `Orcamentos`| Limites mensais de gasto por categoria                 |

### Relacionamentos
- Um usuário possui N contas e N categorias
- Uma conta possui N transações e N orçamentos
- Uma categoria classifica N transações e N orçamentos

---

## 🖥️ Frontend — Nuxt 3 + Vuetify

### Stack
- **Nuxt 3** (SSR/SPA)
- **Vue 3** (Composition API + `<script setup>`)
- **Vuetify 3** (Material Design)
- **Pinia** (gerenciamento de estado)
- **TypeScript**

### Estrutura
```
frontend/
├── layouts/
│   └── default.vue          # Navigation drawer + app bar
├── pages/
│   ├── index.vue            # Dashboard (métricas + últimas transações + orçamentos)
│   ├── transacoes.vue       # CRUD completo com filtros
│   ├── contas.vue           # Gerenciamento de contas
│   ├── categorias.vue       # Gerenciamento de categorias
│   └── orcamentos.vue       # Controle de orçamentos mensais
├── components/
│   ├── MetricCard.vue       # Card de métrica reutilizável
│   └── TransacaoDialog.vue  # Dialog criar/editar transação
├── composables/
│   ├── useApi.ts            # Cliente HTTP genérico
│   ├── useContas.ts         # Lógica de contas
│   └── useCategorias.ts     # Lógica de categorias
├── stores/
│   └── transacoes.ts        # Store Pinia de transações
├── types/
│   └── index.ts             # Interfaces TypeScript
└── plugins/
    └── vuetify.ts           # Configuração do Vuetify
```

### Instalação e execução

```bash
cd frontend
npm install
npm run dev       # http://localhost:3000
```

---

## ⚙️ Backend — C# .NET 8 + EF Core

### Stack
- **.NET 8** Web API
- **Entity Framework Core 8** (SQLite em dev, qualquer banco em prod)
- **Swagger/OpenAPI** para documentação
- **CORS** configurado para o frontend

### Endpoints da API

#### Contas
| Método | Rota                | Descrição              |
|--------|---------------------|------------------------|
| GET    | `/api/contas`       | Listar contas          |
| GET    | `/api/contas/{id}`  | Obter conta por ID     |
| POST   | `/api/contas`       | Criar conta            |
| PUT    | `/api/contas/{id}`  | Atualizar conta        |
| DELETE | `/api/contas/{id}`  | Desativar conta        |

#### Categorias
| Método | Rota                    | Descrição                           |
|--------|-------------------------|-------------------------------------|
| GET    | `/api/categorias`       | Listar (filtro `?tipo=Despesa`)     |
| GET    | `/api/categorias/{id}`  | Obter por ID                        |
| POST   | `/api/categorias`       | Criar                               |
| PUT    | `/api/categorias/{id}`  | Atualizar                           |
| DELETE | `/api/categorias/{id}`  | Excluir                             |

#### Transações
| Método | Rota                     | Descrição                                         |
|--------|--------------------------|---------------------------------------------------|
| GET    | `/api/transacoes`        | Listar (filtros: tipo, categoriaId, contaId, data, busca) |
| GET    | `/api/transacoes/{id}`   | Obter por ID                                      |
| POST   | `/api/transacoes`        | Criar (atualiza saldo da conta automaticamente)   |
| PUT    | `/api/transacoes/{id}`   | Atualizar (reverte e recalcula saldo)             |
| DELETE | `/api/transacoes/{id}`   | Excluir (reverte saldo)                           |

#### Orçamentos
| Método | Rota                   | Descrição                       |
|--------|------------------------|---------------------------------|
| GET    | `/api/orcamentos`      | Listar (filtro `?mes=2025-06`)  |
| GET    | `/api/orcamentos/{id}` | Obter por ID                    |
| POST   | `/api/orcamentos`      | Criar                           |
| PUT    | `/api/orcamentos/{id}` | Atualizar limite                |
| DELETE | `/api/orcamentos/{id}` | Excluir                         |

#### Relatórios
| Método | Rota                             | Descrição                          |
|--------|----------------------------------|------------------------------------|
| GET    | `/api/relatorios/mensal`         | Resumo dos últimos N meses         |
| GET    | `/api/relatorios/por-categoria`  | Gastos por categoria no mês        |

### Instalação e execução

```bash
cd backend
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run         # http://localhost:5000
# Swagger UI:      http://localhost:5000/swagger
```

### Variáveis de ambiente (produção)

```json
// appsettings.Production.json
{
  "ConnectionStrings": {
    "Default": "Server=...;Database=financeiro;..."
  }
}
```

---

## 🚀 Rodando tudo junto

1. Inicie o backend: `cd backend && dotnet run`
2. Inicie o frontend: `cd frontend && npm run dev`
3. Acesse `http://localhost:3000`
4. Documentação da API: `http://localhost:5000/swagger`

---
