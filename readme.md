# 🍔 Burger House Tools API

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)]()
[![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)]()
[![Keycloak](https://img.shields.io/badge/Keycloak-003545?style=for-the-badge&logo=keycloak&logoColor=white)]()
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)]()
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)]()

Este projeto foi desenvolvido para botar em prática aprendizados de backend. O objetivo principal é construir uma API robusta em ASP.NET Core, orquestrando banco de dados, mensageria e autenticação/autorização com Keycloak via Docker, aplicando boas práticas de arquitetura e código limpo.

---

## 🎯 O que foi implementado

* **Orquestração com Docker Compose:** Execução simultânea do **SQL Server**, **RabbitMQ** e **Keycloak** com importação automática do Realm.
* **Autenticação e Autorização (Keycloak & JWT):**
  * Integração com Keycloak via OpenID Connect (JWT Bearer).
  * Controle de acesso baseado em Roles (`Admin`, `Caixa`, `Garcom`).
  * Endpoint customizado `POST /api/auth/login` que realiza o login e retorna o JWT Bearer Token.
  * Criação de usuários integrada: ao cadastrar um usuário (apenas Admin), ele é registrado no Keycloak (login, senha e role) e os dados de negócio (`DailyRate`, `Pix`) são persistidos no SQL Server.
* **Entity Framework Core Avançado:** 
  * Mapeamento Code-First e execução de Migrations via CLI (`dotnet ef`).
  * Suporte a campos de negócio como `DailyRate` (decimal com precisão 10,2), `Pix` e `KeycloakId`.
* **Mensageria com RabbitMQ:** 
  * `BackgroundService` consumidor de mensagens para cadastro de novos usuários.
* **Documentação com Swagger:**
  * Documentação interativa com botão **Authorize** para envio do Bearer Token.

---

## 🏗️ Estrutura do Projeto

```text
burger-house-tools-api/
├── Controllers/       # Gerenciamento de rotas HTTP (UsersController, AuthController)
├── Models/            # Entidades de domínio (User)
│   └── Enums/         # Enumerações (UserRole)
├── DTOs/              # Objetos de transferência de dados (UserInputDto, LoginInputDto, etc.)
├── Data/              # Configuração do Entity Framework (AppDbContext, Migrations)
├── Services/          # Regras de negócio, KeycloakService e Workers RabbitMQ
├── keycloak/          # Arquivo de importação automática do Realm (realm-export.json)
├── Program.cs         # Ponto de entrada, middlewares de autenticação/autorização e Swagger
└── docker-compose.yml # Infraestrutura (SQL Server + RabbitMQ + Keycloak)
```

---

## 🚀 Como Executar e Testar a Partir de Agora

### 1. Pré-requisitos
* **.NET SDK** instalado (versão 10.0 ou superior)
* **Docker Desktop** instalado e em execução

### 2. Subir os Serviços (Infraestrutura)
Na raiz do repositório, inicie os containers via Docker Compose:

```bash
docker compose up -d
```

> Isso inicializará automaticamente:
> * **SQL Server** na porta `1433`
> * **RabbitMQ** nas portas `5672` (AMQP) e `15672` (Painel Web)
> * **Keycloak** na porta `8080` com o Realm `burger-house`, clients e usuários de teste já importados!

### 3. Aplicar as Migrações do Banco de Dados
Ainda na raiz ou dentro da pasta do projeto da API, execute:

```bash
dotnet ef database update --project burger-house-tools-api
```

---

### 4. Usuários de Teste Pré-configurados

O Keycloak já nasce com os seguintes usuários prontos para teste:

| Usuário | Senha | Role | Permissões na API |
| :--- | :--- | :--- | :--- |
| **`admin`** | `admin123` | `Admin` | Acesso total: listar, criar, editar e excluir usuários |
| **`caixa`** | `caixa123` | `Caixa` | Apenas consulta (`GET /api/users`) |
| **`garcom`** | `garcom123` | `Garcom` | Apenas consulta (`GET /api/users`) |

---

### 5. Testando pelo Swagger

1. **Inicie a API:**
   * Pelo Visual Studio (pressione `F5`), ou pelo terminal:
     ```bash
     dotnet run --project burger-house-tools-api
     ```
2. **Abra o Swagger no navegador:**
   * Acesse `https://localhost:7176/swagger` (ou a porta informada no terminal).

3. **Obter o Token de Acesso:**
   * Abra o endpoint `POST /api/auth/login`.
   * Clique em **Try it out** e faça o login com um dos usuários de teste:
     ```json
     {
       "login": "admin",
       "password": "admin123"
     }
     ```
   * Clique em **Execute**.
   * Copie o valor de **`access_token`** retornado na resposta (apenas o código que inicia com `eyJ...`, sem as aspas).

4. **Autenticar o Swagger:**
   * Clique no botão verde **`Authorize`** 🔒 no topo direito da página do Swagger.
   * Cole o token no campo **`Value:`** (apenas o token puro, sem escrever `Bearer`).
   * Clique em **Authorize** e depois em **Close**.

5. **Testar os Endpoints Protegidos:**
   * `GET /api/users` → Lista os usuários do banco (permitido para `Admin`, `Caixa` e `Garcom`).
   * `POST /api/users` → Cria um novo usuário no Keycloak e no banco local (permitido **apenas** para `Admin`):
     ```json
     {
       "name": "Novo Garcom",
       "login": "novogarcom",
       "password": "senhaForte123!",
       "userRole": "Garcom",
       "dailyRate": 150.00,
       "pix": "garcom@email.com"
     }
     ```
   * `PUT /api/users/{id}` → Atualiza dados de um usuário (apenas `Admin`).
   * `DELETE /api/users/{id}` → Remove usuário (apenas `Admin`).

---

## 🐰 Como acessar o Painel do RabbitMQ

1. Acesse: [http://localhost:15672](http://localhost:15672)
2. Credenciais:
   * **Usuário:** `guest`
   * **Senha:** `guest`
3. Aba **Queues** → selecione a fila `new_user_queue`.
4. Em **Publish message**, envie mensagens para cadastro assíncrono.

---

## 🔐 Como acessar o Console Administrativo do Keycloak

1. Acesse: [http://localhost:8080](http://localhost:8080)
2. Credenciais do Administrador do Keycloak:
   * **Usuário:** `admin`
   * **Senha:** `admin`
3. Selecione o realm **`burger-house`** no canto superior esquerdo para visualizar roles, clients e usuários cadastrados.
