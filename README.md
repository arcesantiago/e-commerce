# 🛒
**Microservicios y Testing Unitario**  
**Modalidad:** Desarrollo práctico  
**Asistido con:** Microsoft Copilot

Este repositorio contiene la solución desarrollada para la prueba técnica, cumpliendo con los requisitos de la consigna:  
- **.NET 8 con ASP.NET Core Web API**  
- **Arquitectura de microservicios** (Product Service y Order Service)  
- **Clean Architecture**  
- **Entity Framework Core** con Code First y Migrations  
- **Testing unitario** con cobertura mínima del 80%  
- **Docker** para contenerización  
- **MediatR** para CQRS (implementado como bonus)  

---

## 🚀 Instrucciones de ejecución

### 1. Requisitos previos
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

### 2. Clonar el repositorio

git clone https://github.com/arcesantiago/e-commerce.git
cd e-commerce
### 3. Levantar el entorno con Docker Compose

docker-compose up --build

Esto construirá las imágenes de ProductService, OrderService y levantará un contenedor de SQL Server con datos iniciales (seed).

### 4. Acceso a los servicios
ProductService API → http://localhost:8082/swagger

OrderService API → http://localhost:8081/swagger

SQL Server → localhost,1433 (usuario: sa, contraseña: Your_strong!Passw0rd)

🏗 Arquitectura del sistema

                ┌──────────────────┐
                │   ProductService  │
                │  (.NET 8, API)    │
                └─────────┬────────┘
                          │ HTTP + Polly
                ┌─────────▼────────┐
                │   OrderService    │
                │  (.NET 8, API)    │
                └─────────┬────────┘
                          │ EF Core
                ┌─────────▼────────┐
                │   SQL Server DB   │
                └───────────────────┘

Clean Architecture: Capas API, Application, Domain, Infrastructure.

Repository Pattern y Dependency Injection nativo de .NET.

Domain Driven Design básico.

Manejo de errores centralizado con middleware personalizado.

Comunicación entre microservicios vía HttpClientFactory + Polly (retry policies).

Validaciones con FluentValidation.

Configuración por entorno en appsettings.{Environment}.json.

Logging con Serilog.

📌 Endpoints implementados
Product Service
GET /api/products → Listar productos con paginación.

GET /api/products/{id} → Obtener producto por ID.

POST /api/products → Crear producto.

PUT /api/products/{id} → Actualizar producto.

DELETE /api/products/{id} → Eliminar producto.

Modelo:


public class Product {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

Order Service
GET /api/orders → Listar pedidos.

GET /api/orders/{id} → Obtener pedido por ID.

POST /api/orders → Crear pedido (valida existencia de producto y stock).

PUT /api/orders/{id}/status → Actualizar estado del pedido.

Modelo:


public class Order {
    public int Id { get; set; }
    public string CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> Items { get; set; }
}

public class OrderItem {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public enum OrderStatus {
    Pending, Confirmed, Processing, Shipped, Delivered, Cancelled
}

🧪 Testing Unitario
Frameworks y librerías:

xUnit

Moq

FluentAssertions

Microsoft.EntityFrameworkCore.InMemory

Cobertura mínima: 80% en ambos microservicios.

Product Service:

ProductControllerTests: todos los endpoints, códigos HTTP, datos válidos/ inválidos.

ProductServiceTests: lógica de negocio, validaciones, mocks de repositorio.

ProductRepositoryTests: CRUD y queries complejas con InMemory DB.

Order Service:

OrderControllerTests: endpoints, validaciones.

OrderServiceTests: lógica de negocio, cálculo de totales, mock de HttpClient.

OrderRepositoryTests: CRUD y queries con InMemory DB.

🐳 Infraestructura
Dockerfile para cada microservicio (multi-stage build).

docker-compose.yml con:

ProductService

OrderService

SQL Server

Red compartida appnet

Volumen persistente mssql_data

Base de datos:

Code First con Migrations.

Seed data inicial.

🎯 Decisiones técnicas tomadas
Clean Architecture para separación clara de responsabilidades.

CQRS con MediatR para comandos y consultas desacopladas.

Repository Pattern para acceso a datos.

FluentValidation para reglas de negocio.

HttpClientFactory + Polly para resiliencia en comunicación entre microservicios.

Testing unitario exhaustivo con mocks y base de datos en memoria.

Contenerización con Docker y orquestación con Docker Compose.

CI/CD básico con GitHub Actions para build, test y build de imágenes.

📬 Documentación de API

Swagger disponible en:

ProductService → http://54.175.121.198:8082/swagger

OrderService → http://54.175.121.198:8081/swagger
