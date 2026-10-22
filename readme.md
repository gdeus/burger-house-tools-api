# 🍔 Burger House Tools API

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)]()
[![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)]()
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)]()
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)]()

Este projeto foi desenvolvido como para botar em prática meus aprendizados backend dos últimos anos. O objetivo principal foi construir uma API robusta em ASP.NET Core, orquestrando banco de dados e mensageria via Docker, e aplicando boas práticas de arquitetura e código limpo.

---

## 🎯 O que eu estudei e aprendi neste ciclo

Durante o desenvolvimento desta aplicação, consolidei diversos conceitos essenciais para o desenvolvimento de software moderno:

* **Orquestração com Docker Compose:** Configuração e execução simultânea do SQL Server e RabbitMQ, isolando a infraestrutura da aplicação e facilitando o setup do ambiente de desenvolvimento.
* **Arquitetura e Separação de Responsabilidades:** Transição de Minimal APIs para uma estrutura organizada com **Controllers** e **DTOs** (Data Transfer Objects), garantindo que as entidades de domínio não sejam expostas diretamente nos endpoints.
* **Entity Framework Core Avançado:** 
  * Mapeamento Code-First e execução de Migrations via CLI (`dotnet-ef`).
  * Configuração de `HasConversion<string>()` no `OnModelCreating` para salvar **Enums** (`UserRole`) como texto legível no banco de dados, em vez de números inteiros.
* **Serialização JSON:** Implementação do `JsonStringEnumConverter` tanto nas opções da API quanto no desserializador do RabbitMQ, garantindo consistência na entrada e saída de dados.
* **Mensageria com RabbitMQ:** 
  * Criação de um `BackgroundService` (Worker) para consumir mensagens de forma assíncrona.
  * Resolução do desafio arquitetural de injeção de dependência: uso do `IServiceScopeFactory` para instanciar serviços com ciclo de vida *Scoped* (como o `AppDbContext`) dentro de um serviço *Singleton* (o Consumer do RabbitMQ).
* **Automação de Setup:** Criação de scripts em shell (`start.sh`) para automatizar o build, execução de migrations e inicialização dos serviços para novos desenvolvedores.

---

## 🏗️ Estrutura do Projeto

O projeto segue uma arquitetura baseada em separação de responsabilidades (Separation of Concerns):

```text
burger-house-tools-api/
├── Controllers/       # Gerenciamento de rotas HTTP e injeção do contexto
├── Models/            # Entidades de domínio (Usuario)
│   └── Enums/         # Enumerações (UserRole)
├── DTOs/              # Objetos de transferência de dados (UsuarioInputDto)
├── Data/              # Configuração do Entity Framework (AppDbContext, Migrations)
├── Services/          # Regras de negócio e Workers (UsuarioConsumer)
├── Program.cs         # Ponto de entrada, middlewares e injeção de dependência
└── docker-compose.yml # Infraestrutura (SQL Server + RabbitMQ)

## 🐰 Como acessar e testar o RabbitMQ

Com a aplicação rodando via Docker, o painel de gerenciamento do RabbitMQ já estará disponível localmente.

1. Abra o navegador e acesse: [http://localhost:15672](http://localhost:15672)
2. Faça o login com as credenciais padrão:
   * **Usuário:** `guest`
   * **Senha:** `guest`
3. No menu superior, clique na aba **Queues** e selecione a fila `new_user_queue`.
4. Role a página até a seção **Publish message**.
5. No campo **Payload**, cole o JSON de exemplo abaixo:

```json
```json
{
  "name": "Fabio",
  "login": "fabito",
  "password": "123123",
  "userRole": "Caixa"
}

## 📖 Documentação da API (Swagger)

A API utiliza o Swagger para documentar e disponibilizar uma interface interativa de todos os endpoints. Isto permite testar o CRUD diretamente pelo navegador.

1. Com a aplicação a correr no terminal, abra o seu navegador.
2. Aceda ao seguinte endereço: `http://localhost:<porta_da_aplicacao>/swagger`
   *(Nota: A porta exata será exibida no terminal após executar o `start.sh` ou o `dotnet run`. Exemplo: `http://localhost:5000/swagger`).*
3. Através do Swagger, pode realizar pedidos HTTP (GET, POST, PUT, DELETE) e validar rapidamente se os dados inseridos via RabbitMQ já se encontram persistidos na base de dados utilizando o endpoint `GET`.