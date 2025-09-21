# 🛒 E‑Commerce Microservices

Este repositorio implementa una arquitectura de microservicios para un sistema de e‑commerce, desarrollada en **.NET 8** con **Docker** y **SQL Server**.  
Incluye dos servicios principales:

- **ProductService** → Gestión de productos.
- **OrderService** → Gestión de pedidos.

---

## 🚀 Instrucciones de ejecución

### 1. Requisitos previos
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

### 2. Clonar el repositorio

git clone https://github.com/arcesantiago/e-commerce.git
cd e-commerce
3. Levantar el entorno con Docker Compose

bash
docker-compose up --build
Esto construirá las imágenes de ProductService, OrderService y levantará un contenedor de SQL Server.

4. Acceso a los servicios

ProductService API → http://localhost:8082/swagger

OrderService API → http://localhost:8081/swagger

SQL Server → localhost,1433 (usuario: sa, contraseña: Your_strong!Passw0rd)

🏗 Arquitectura del sistema

                ┌──────────────────┐
                │   ProductService  │
                │  (.NET 8, API)    │
                └─────────┬────────┘
                          │
                ┌─────────▼────────┐
                │   OrderService    │
                │  (.NET 8, API)    │
                └─────────┬────────┘
                          │
                ┌─────────▼────────┐
                │   SQL Server DB   │
                └───────────────────┘
                
Microservicios independientes: cada uno con su propio Dockerfile y pruebas unitarias.

Comunicación: HTTP/REST.

Persistencia: SQL Server (contenedor db).

Orquestación: docker-compose.yml unificado en la raíz.

Testing: proyectos de test separados por capa (API.Test, Application.Test, Infrastructure.Test).

⚙️ Decisiones técnicas tomadas

.NET 8 + Clean Architecture Separación en capas (API, Application, Domain, Infrastructure) para favorecer mantenibilidad y escalabilidad.

CQRS + MediatR Para separar comandos y consultas, mejorando la claridad y testabilidad.

Docker multi-stage builds Reduce el tamaño de las imágenes y acelera despliegues.

Contexto de build en raíz (.) Permite que los Dockerfile accedan a todas las capas del microservicio.

SQL Server en contenedor Facilita el desarrollo local y la integración en CI/CD.

CI/CD con GitHub Actions

Restaurar, compilar y testear cada microservicio.

Construir imágenes Docker listas para despliegue.

Uso de Microsoft Copilot Copilot se utilizó para asistir en la generación de documentación, estructuración de instrucciones y optimización de configuraciones de CI/CD.

📬 Colección de Postman / Documentación de API

En la carpeta /docs encontrarás:

Colección de Postman: E-Commerce.postman_collection.json Contiene ejemplos de requests para:

Crear, obtener, actualizar y eliminar productos.

Crear y consultar pedidos.

Documentación Swagger: Cada microservicio expone su propia documentación interactiva:

ProductService → /swagger

OrderService → /swagger

🧪 Ejecución de tests

Para ejecutar todos los tests localmente:

bash
dotnet test ProductService/ProductService.API.Test/ProductService.API.Test.csproj
dotnet test ProductService/ProductService.Application.Test/ProductService.Application.Test.csproj
dotnet test ProductService/ProductService.Infrastructure.Test/ProductService.Infrastructure.Test.csproj

dotnet test OrderService/OrderService.API.Test/OrderService.API.Test.csproj
dotnet test OrderService/OrderService.Application.Test/OrderService.Application.Test.csproj
dotnet test OrderService/OrderService.Infrastructure.Test/OrderService.Infrastructure.Test.c
