# Customer Management API

API REST para gerenciamento de dados de clientes, construída com **.NET 8**, seguindo os princípios de **Arquitetura Hexagonal (Ports & Adapters)** e **Clean Architecture**.

---

## Sumário

- [Arquitetura C4](#arquitetura-c4)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Endpoints da API](#endpoints-da-api)
- [Domínio](#domínio)
- [Configuração e Execução](#configuração-e-execução)
- [Testes](#testes)

---

## Arquitetura C4

### Nível 1 — Contexto do Sistema

```
┌─────────────────┐         ┌──────────────────────────┐         ┌─────────────────┐
│                 │  HTTP   │                          │  TCP    │                 │
│   Cliente /     │────────▶│  Customer Management     │────────▶│    MongoDB       │
│   Frontend      │◀────────│         API              │◀────────│                 │
│                 │         │                          │         └─────────────────┘
└─────────────────┘         │                          │
                            │                          │         ┌─────────────────┐
                            │                          │  HTTP   │  Receita        │
                            │                          │────────▶│  Federal API    │
                            │                          │         └─────────────────┘
                            │                          │
                            │                          │         ┌─────────────────┐
                            │                          │  Kafka  │  Event Broker   │
                            │                          │────────▶│  (Confluent)    │
                            └──────────────────────────┘         └─────────────────┘
```

### Nível 2 — Containers

```
┌──────────────────────────────────────────────────────────────────────────┐
│                        Customer Management API                          │
│                                                                          │
│  ┌────────────┐   ┌───────────────┐   ┌──────────┐   ┌──────────────┐  │
│  │    API      │──▶│  Application  │──▶│  Domain  │   │Infrastructure│  │
│  │ Controllers │   │  UseCases /   │   │ Entities │◀──│  Mongo /     │  │
│  │ Middlewares │   │  Queries      │   │ ValueObj │   │  Kafka /     │  │
│  │ HealthCheck │   │  Ports        │   │ Enums    │   │  ExtServices │  │
│  └────────────┘   └───────────────┘   └──────────┘   └──────────────┘  │
│                                                                          │
│  ┌──────────────┐                                                        │
│  │ CrossCutting  │  Resolução de dependências (DI)                       │
│  └──────────────┘                                                        │
└──────────────────────────────────────────────────────────────────────────┘
```

### Nível 3 — Componentes (Application)

```
                         ┌──────────────────────────┐
                         │      Ports / Inbound      │
                         │  ISaveCustomerUseCase     │
                         │  ICustomerQueryService    │
                         └────────────┬─────────────┘
                                      │
              ┌───────────────────────┼───────────────────────┐
              ▼                       ▼                       ▼
   ┌──────────────────┐   ┌──────────────────┐   ┌──────────────────┐
   │ SaveCustomerUse  │   │ CustomerQuery    │   │    Mapper        │
   │ Case             │   │ Service          │   │ CustomerMapper   │
   └────────┬─────────┘   └────────┬─────────┘   └──────────────────┘
            │                      │
            ▼                      ▼
   ┌──────────────────────────────────────────┐
   │          Ports / Outbound                 │
   │  ICustomerRepository                      │
   │  ICustomerQueryRepository                 │
   │  ICustomerProducer                        │
   │  IReceitaFederalService                   │
   └──────────────────────────────────────────┘
```

---

## Estrutura do Projeto

```
src/
├── CustomerManagementApi.Api/
├── CustomerManagementApi.Application/
├── CustomerManagementApi.Domain/
├── CustomerManagementApi.Infrastructure/
├── CustomerManagementApi.CrossCutting/
└── CustomerManagementApi.Tests/
```

### CustomerManagementApi.Api — Camada de Apresentação

Ponto de entrada da aplicação. Responsável por receber requisições HTTP, delegar para a camada de aplicação e retornar respostas formatadas. Não contém lógica de negócio.

| Pasta / Arquivo | Responsabilidade |
|---|---|
| **Controllers/V1/** | Controllers REST versionados. Cada controller expõe endpoints de um recurso da API. Depende apenas de portas **Inbound** da Application. |
| `CustomerController.cs` | Endpoints CRUD de clientes (GET, POST, PUT) com paginação, filtro por nome e contagem. |
| **Configurations/** | Classes de configuração de serviços registrados no pipeline do ASP.NET. |
| `LoggingServiceConfiguration.cs` | Registra o serviço de logging (Serilog) no container de DI. |
| `SwaggerConfiguration.cs` | Configura o Swagger/OpenAPI incluindo o filtro do header `appOrigin` na documentação. |
| **HealthChecks/** | Implementações de health checks para monitoramento da infraestrutura. |
| `MongoDbHealthCheck.cs` | Verifica conectividade com o MongoDB executando um ping. |
| `KafkaHealthCheck.cs` | Verifica conectividade com o broker Kafka. |
| **Middlewares/** | Middlewares customizados que interceptam todas as requisições no pipeline. |
| `ExceptionMiddleware.cs` | Middleware centralizado de tratamento de exceções. Captura `DomainException` (422), `ValidationException` (422), `KeyNotFoundException` (404), `MongoWriteException` (409), `MongoConnectionException` (503) e exceções genéricas (500). |
| `Program.cs` | Entry point — configura o pipeline de middlewares, DI, routing, Swagger, health checks e inicia a aplicação. |
| `appsettings.json` | Configurações gerais da aplicação (logging, etc). |
| `docker-compose.yaml` | Orquestração de containers locais: MongoDB, Mongo Express, Kafka, Zookeeper e Schema Registry. |

---

### CustomerManagementApi.Application — Camada de Aplicação

Orquestra os casos de uso da aplicação. Contém a lógica de aplicação (não de domínio), mapeamentos, validações de entrada e definição das portas (interfaces) hexagonais. Não depende de infraestrutura.

| Pasta / Arquivo | Responsabilidade |
|---|---|
| **UseCases/** | Implementação dos casos de uso (comandos). Cada classe representa uma ação de negócio. |
| `SaveCustomerUseCase.cs` | Caso de uso para criar e atualizar clientes. Valida dados de entrada com Flunt, delega validações de formato para Value Objects do domínio, mapeia para entidade e persiste via porta outbound. |
| **Queries/** | Serviços de consulta (leitura). Separados dos use cases para respeitar CQRS. |
| `CustomerQueryService.cs` | Consulta clientes paginados (com filtro por nome), por ID e contagem total. Delega para `ICustomerQueryRepository`. |
| **Ports/Inbound/** | Interfaces que a camada de apresentação consome. Definem o contrato dos casos de uso e serviços de consulta. |
| `ISaveCustomerUseCase.cs` | Contrato para criar/atualizar cliente: `Executar(id, request, cancellationToken)`. |
| `ICustomerQueryService.cs` | Contrato para consultas: `GetCustomers()`, `GetCustomersById()`, `Count()`. |
| **Ports/Outbound/** | Interfaces que a camada de aplicação depende (implementadas pela infraestrutura). Seguem o princípio de Inversão de Dependência. |
| `ICustomerRepository.cs` | Porta de escrita: `GetById()`, `Insert()`, `Update()`. |
| `ICustomerQueryRepository.cs` | Porta de leitura: `GetCustomers()`, `GetCustomersById()`, `Count()`. |
| **Ports/Outbound/ExternalServices/** | Interfaces de integrações externas. |
| `IReceitaFederalService.cs` | Contrato para consulta de CPF/CNPJ na Receita Federal. |
| `ICustomerProducer.cs` | Contrato para publicação de eventos de cliente no Kafka. |
| **RequestModel/** | DTOs de entrada (request bodies). Desacoplados das entidades de domínio. |
| `CustomerRequestModel.cs` | Modelo de requisição com campos: Name, DocumentType, DocumentNumber, Email, Phone. |
| **ResponseModel/** | DTOs de saída (response bodies). |
| `CustomerResponseModel.cs` | Dados retornados de um cliente (Id, Name, DocumentType, DocumentNumber, Email, Phone). |
| `GenericResponseModel.cs` | Wrapper genérico para respostas paginadas com `Page`, `PageSize` e `Data`. |
| **Mapper/** | Classes de mapeamento entre camadas. Evitam acoplamento direto entre DTOs e entidades. |
| `CustomerMapper.cs` | Converte `CustomerRequestModel` → `Customer` (entidade) → `CustomerResponseModel`. |
| **ValueObjects/** | Objetos de valor e exceções da camada de aplicação. |
| `ValidationException.cs` | Exceção que carrega uma lista de notificações Flunt para erros de validação de entrada. |
| `ErrorModel.cs` | Modelo padronizado de erro retornado pela API com lista de `Notification` (key/message). |
| `Result.cs` | Pattern Result para retorno de operações com sucesso ou erro. |
| `NotifiableObject.cs` | Classe base para objetos com validação via notificações Flunt. |
| **Logs/** | Abstração de logging desacoplada do framework. |
| `ILoggingService.cs` | Interface de logging com métodos Information, Warning, Error. |
| `SerilogLoggingService.cs` | Implementação usando Serilog com contexto de correlação. |
| **Commons/** | Constantes e utilitários compartilhados. |
| `CommonsConstants.cs` | Constantes centralizadas: configurações do MongoDB (host, user, password, connection string, database, collection), Kafka (bootstrap servers, tópicos), paginação (defaults e limites), headers e URLs de serviços externos. |
| `Setup.cs` | Registra os serviços da camada de aplicação no container de DI (`ICustomerQueryService`, `ISaveCustomerUseCase`). |

---

### CustomerManagementApi.Domain — Camada de Domínio

Núcleo da aplicação. Contém entidades, value objects, enums e exceções de negócio. Não depende de nenhuma outra camada — é o centro da arquitetura hexagonal.

| Pasta / Arquivo | Responsabilidade |
|---|---|
| **Entities/** | Entidades de domínio com identidade e ciclo de vida. |
| `Customer.cs` | Entidade principal com: `Id`, `Name`, `DocumentType`, `DocumentNumber` (VO), `Email` (VO), `Phone` (VO), `CreatedDate`. |
| **ValueObjects/** | Objetos imutáveis que encapsulam validação de negócio. Lançam `DomainException` quando inválidos. |
| `Email.cs` | Valida formato de email via regex, normaliza para lowercase. Rejeita domínios com pontos consecutivos (ex: `dominio..com`). |
| `Phone.cs` | Valida que o telefone contém exatamente 11 dígitos (DDD + número). Normaliza removendo formatação. |
| `Document.cs` | Factory que cria o Value Object correto (Cpf, Cnpj ou Passport) conforme o `DocumentType`. |
| `Cpf.cs` | Valida CPF com 11 dígitos e cálculo de dígito verificador. |
| `Cnpj.cs` | Valida CNPJ com 14 dígitos e cálculo de dígito verificador. |
| `Passport.cs` | Valida passaporte com 6 a 20 caracteres alfanuméricos. |
| **Enums/** | Enumerações que representam conceitos de domínio. |
| `DocumentType.cs` | Tipos de documento: `UNDEFINED(0)`, `CPF(1)`, `CNPJ(3)`, `PASSAPORT(4)`. |
| `TypeAddress.cs` | Tipos de endereço: `Location(1)`, `Billing(2)`. |
| **Exceptions/** | Exceções de domínio que sinalizam violação de regras de negócio. Capturadas pelo `ExceptionMiddleware` como HTTP 422. |
| `DomainException.cs` | Classe base para todas as exceções de domínio. |
| `InvalidDocumentException.cs` | Lançada quando CPF, CNPJ ou passaporte é inválido. |
| `InvalidEmailException.cs` | Lançada quando o formato de email é inválido. |
| `InvalidPhoneException.cs` | Lançada quando o formato de telefone é inválido. |

---

### CustomerManagementApi.Infrastructure — Camada de Infraestrutura

Implementa as portas outbound definidas na Application. Contém integrações com banco de dados, mensageria e serviços externos. Depende de Application e Domain.

| Pasta / Arquivo | Responsabilidade |
|---|---|
| **Mongo/Config/** | Configuração e setup da conexão com MongoDB. |
| `MongoSetup.cs` | Cria o `MongoClient` com connection string, SSL/TLS (somente ambientes remotos), timeout e criação automática de índices (Id, Name, DocumentNumber). |
| `ContextMongo.cs` | Implementa `IContextMongo`. Gerencia o acesso ao database e coleções do MongoDB. |
| **Mongo/Document/** | Documentos de persistência (representação no MongoDB). Desacoplados das entidades de domínio. |
| `CustomerMongoDocument.cs` | Documento com campos primitivos: Id, Name, DocumentType (int), DocumentNumber (string), Email (string), Phone (string), CreatedDate. |
| **Mongo/Mapper/** | Mapeamento entre entidades de domínio e documentos MongoDB. |
| `CustomerMongoMapper.cs` | Converte `Customer` (entidade) ↔ `CustomerMongoDocument` e `CustomerMongoDocument` → `CustomerResponseModel`. |
| **Mongo/Repositories/** | Implementações dos repositórios que acessam o MongoDB. |
| `BaseMongoRepository.cs` | Repositório genérico com operações CRUD: `List()`, `Get()`, `Insert()`, `Update()`. Usa `FilterDefinition` e `UpdateDefinition` do driver MongoDB. |
| `CustomerRepository.cs` | Implementa `ICustomerRepository` e `ICustomerQueryRepository`. Operações: listagem paginada com filtro por nome (regex case-insensitive), busca por ID, inserção, atualização parcial e contagem. |
| **Kafka/** | Integração com Apache Kafka via Confluent. (Ainda não ativa.) |
| `KafkaSetup.cs` | Configura o produtor Kafka com suporte a SASL/SSL e Schema Registry. |
| `KafkaProducer.cs` | Produtor genérico que serializa mensagens com Avro e publica em tópicos Kafka. |
| **ExternalServices/** | Clientes HTTP para serviços externos. |
| `ReceitaFederalService.cs` | Consulta dados de CPF/CNPJ na API da Receita Federal com retry policy (3 tentativas com backoff exponencial). |
| **Http/** | Helpers e extensões para chamadas HTTP. |
| `HttpClientExtension.cs` | Extensões para configuração de `HttpClient`. |
| `HttpResponseExtensions.cs` | Extensões para deserialização de `HttpResponseMessage`. |
| `Setup.cs` | Registra as dependências de infraestrutura no container de DI: MongoDB, Kafka, HttpClient, repositórios e serviços externos. |

---

### CustomerManagementApi.CrossCutting — Resolução de Dependências

Camada transversal que orquestra o registro de todas as dependências. É referenciada apenas pelo projeto Api.

| Pasta / Arquivo | Responsabilidade |
|---|---|
| `DependencyResolver.cs` | Método `AddDependencies()` que chama `AddInfrastructure()` e `AddApplications()`, centralizando todo o registro de DI em um único ponto. |

---

### CustomerManagementApi.Tests — Testes

Projeto de testes unitários usando xUnit e Moq.

| Pasta / Arquivo | Responsabilidade |
|---|---|
| **Api/** | Testes da camada de apresentação (Controllers). |
| `CustomerControllerTests.cs` | 8 testes unitários que verificam todos os endpoints do `CustomerController` usando mocks das interfaces inbound. Cobre cenários de sucesso, not found e filtros. |
| **Application/** | Pasta reservada para testes dos use cases e query services. |
| **Domain/** | Testes da camada de domínio. |
| `StringExtensionTests.cs` | Testes de validação de email, telefone e documento via extensões de string. |

---

### Arquivos de Configuração (raiz)

| Arquivo | Responsabilidade |
|---|---|
| `azure-pipelines.yml` | Pipeline de CI/CD no Azure DevOps (build, test, deploy). |
| `Dockerfile` | Imagem Docker da aplicação para deploy em containers. |
| `Config/values-dev.yml` | Valores de configuração para ambiente de desenvolvimento. |
| `Config/values-tst.yml` | Valores de configuração para ambiente de testes/homologação. |
| `Config/values-prd.yml` | Valores de configuração para ambiente de produção. |

---

## Endpoints da API

Base URL: `/api/v1/customer`

| Método | Rota       | Descrição                          | Query Params                         | Status Codes          |
|--------|------------|------------------------------------|--------------------------------------|-----------------------|
| GET    | `/`        | Lista clientes paginados           | `page`, `pageSize`, `name` (filtro)  | 200, 500              |
| GET    | `/{id}`    | Obtém cliente por ID               | —                                    | 200, 404, 500         |
| GET    | `/count`   | Contagem total de clientes         | —                                    | 200, 500              |
| POST   | `/`        | Cria novo cliente                  | —                                    | 201, 422, 500         |
| PUT    | `/{id}`    | Atualiza cliente existente         | —                                    | 200, 404, 422, 500    |

### Request Body (POST / PUT)

```json
{
  "name": "João da Silva",
  "documentType": 1,
  "documentNumber": "12345678901",
  "email": "joao@email.com",
  "phone": "11999999999"
}
```

### Response Body

```json
{
  "id": "guid-gerado",
  "name": "João da Silva",
  "documentType": 1,
  "documentNumber": "12345678901",
  "email": "joao@email.com",
  "phone": "11999999999"
}
```

### Paginação (GET /)

```json
{
  "page": 1,
  "pageSize": 100,
  "data": [ ... ]
}
```

### Erro (422 / 404 / 500)

```json
{
  "errors": [
    { "key": "Name", "message": "O nome do cliente é obrigatório." }
  ]
}
```

### Headers obrigatórios

| Header       | Descrição                              |
|--------------|----------------------------------------|
| `appOrigin`  | Identifica a origem da solicitação     |

---

## Domínio

### Entidade Customer

| Campo          | Tipo           | Descrição                    |
|----------------|----------------|------------------------------|
| Id             | `string`       | Identificador único (GUID)   |
| Name           | `string`       | Nome completo                |
| DocumentType   | `DocumentType` | Tipo do documento            |
| DocumentNumber | `Document` (VO)| Número do documento validado |
| Email          | `Email` (VO)   | Email validado               |
| Phone          | `Phone?` (VO)  | Telefone validado (opcional) |
| CreatedDate    | `DateTime`     | Data de criação (UTC)        |

### Value Objects

- **Document**: Valida CPF (11 dígitos com verificador), CNPJ (14 dígitos com verificador) ou Passaporte (6-20 caracteres) conforme `DocumentType`.
- **Email**: Valida formato com regex, normaliza para lowercase. Rejeita domínios com pontos consecutivos.
- **Phone**: Aceita apenas 11 dígitos (DDD + número), normaliza removendo formatação.

### Enums

- **DocumentType**: `UNDEFINED(0)`, `CPF(1)`, `CNPJ(3)`, `PASSAPORT(4)`
- **TypeAddress**: `Location(1)`, `Billing(2)`

### Exceções de Domínio

Todas herdam de `DomainException` e são capturadas pelo `ExceptionMiddleware` retornando **HTTP 422**:

- `InvalidDocumentException` — Documento inválido
- `InvalidEmailException` — Email inválido
- `InvalidPhoneException` — Telefone inválido

---

## Configuração e Execução

### Pré-requisitos

- .NET 8 SDK
- Docker com WSL (para MongoDB local)

### Variáveis de ambiente

As variáveis são configuradas em `src/CustomerManagementApi.Api/properties/launchSettings.json` e carregadas automaticamente ao executar via Visual Studio ou `dotnet run`.

#### MongoDB (obrigatório)

| Variável           | Descrição              | Valor padrão (launchSettings) |
|--------------------|------------------------|-------------------------------|
| `MONGODB_HOST`     | Host e porta do MongoDB | `localhost:27017`            |
| `MONGODB_USER`     | Usuário do MongoDB     | `admin`                       |
| `MONGODB_PASSWORD` | Senha do MongoDB       | `Passw0rd`                    |

#### Receita Federal (integração externa)

| Variável                      | Descrição                    | Valor padrão (launchSettings) |
|-------------------------------|------------------------------|-------------------------------|
| `BASE_URL_API_RECEITAFEDERAL` | URL base da API              | `https://ExemploTeste`        |
| `API_KEY_API_RECEITAFEDERAL`  | API Key para autenticação    | `teste`                       |

#### Kafka (ainda não configurado — valores placeholder)

| Variável                       | Descrição                     | Valor padrão (launchSettings) |
|--------------------------------|-------------------------------|-------------------------------|
| `BOOTSTRAP_SERVERS`            | Servidores Kafka              | `teste`                       |
| `KAFKA_USE_SASL_SSL`          | Usar SASL/SSL                 | `true`                        |
| `SASL_USERNAME`               | Usuário SASL                  | `teste`                       |
| `SASL_PASSWORD`               | Senha SASL                    | `teste`                       |
| `KAFKA_CONFLUENT_SCHEMA_URL`  | URL do Schema Registry        | `teste`                       |
| `KAFKA_CONFLUENT_SCHEMA_KEY`  | Chave do Schema Registry      | `teste`                       |
| `KAFKA_CONFLUENT_SCHEMA_SECRET`| Secret do Schema Registry    | `teste`                       |

#### Aplicação

| Variável                 | Descrição              | Valor padrão (launchSettings) |
|--------------------------|------------------------|-------------------------------|
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicação  | `Development`                 |
| `LOG_LEVEL`              | Nível de log           | `Debug`                       |

### Passo a passo para executar

#### 1. Subir o MongoDB via Docker

> **Nota:** O Kafka ainda não está ativo neste projeto. Apenas o MongoDB é necessário.

O `docker-compose.yaml` usa variáveis de ambiente para as credenciais do MongoDB. Antes de subir, defina-as:

**PowerShell (Windows):**
```powershell
cd src\CustomerManagementApi.Api
$env:MONGO_INITDB_ROOT_USERNAME="admin"
$env:MONGO_INITDB_ROOT_PASSWORD="Passw0rd"
docker compose up -d mongo
```

**Bash (WSL/Linux):**
```bash
cd src/CustomerManagementApi.Api
MONGO_INITDB_ROOT_USERNAME=admin MONGO_INITDB_ROOT_PASSWORD=Passw0rd docker compose up -d mongo
```

> As credenciais acima devem coincidir com `MONGODB_USER` e `MONGODB_PASSWORD` do `launchSettings.json`.

#### 2. Build do projeto

```bash
cd src
dotnet build CustomerManagementApi.sln
```

#### 3. Executar a aplicação

**Via CLI:**
```bash
cd src
dotnet run --project CustomerManagementApi.Api --launch-profile "CustomerManagementApi.Api"
```

**Via Visual Studio:**
Abrir `src/CustomerManagementApi.sln` e pressionar F5. O perfil `CustomerManagementApi.Api` será usado automaticamente.

#### 4. Acessar a API

| Recurso       | URL                                |
|---------------|------------------------------------|
| Swagger UI    | https://localhost:5001/swagger     |
| API (HTTP)    | http://localhost:5000              |
| API (HTTPS)   | https://localhost:5001             |

### Health Checks

| Rota              | Descrição                          |
|-------------------|------------------------------------|
| `/health`         | Health check básico                |
| `/health/ready`   | Readiness (MongoDB + Kafka)        |

---

## Testes

```bash
cd src
dotnet test CustomerManagementApi.Tests/CustomerManagementApi.Tests.csproj
```

### Cobertura de testes

| Camada       | Classe testada            | Testes |
|--------------|---------------------------|--------|
| API          | CustomerControllerTests   | 8      |
| Domain       | StringExtensionTests      | vários |

### Executar com cobertura

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```