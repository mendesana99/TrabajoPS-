# TicketPlatform - Premium Entertainment System

Sistema de reserva de tickets profesional desarrollado con **.NET 8** (Clean Architecture) y **Vanilla Frontend**.

## 🚀 Cómo empezar

### 1. Requisitos previos
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [SQL Server Express / LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb)

### 2. Configuración del Backend
1. Navega a la carpeta del proyecto API:
   ```bash
   cd Trabajo_ps
   ```
2. Restaura las dependencias:
   ```bash
   dotnet restore
   ```
3. Aplica las migraciones para crear la base de datos local:
   ```bash
   dotnet ef database update --project ../Infrastructure/Infrastructure.csproj --startup-project ./Trabajo_ps.csproj
   ```
4. Ejecuta el servidor:
   ```bash
   dotnet run
   ```
   El servidor estará disponible en `http://localhost:5000` y el **Swagger** en `http://localhost:5000/swagger`.

### 3. Configuración del Frontend
El frontend es totalmente independiente. No requiere compilación.
1. Abre el archivo `Frontend/index.html` directamente en tu navegador o usa una extensión como *Live Server* en VS Code.
2. Asegúrate de que el backend esté corriendo para que las llamadas a la API funcionen.

---

## 🏗️ Arquitectura del Proyecto
El sistema sigue los principios de **Clean Architecture** y **CQRS**:

*   **Domain**: Entidades puras y excepciones de dominio.
*   **Application**: Casos de uso, DTOs, interfaces de repositorio y lógica de negocio.
*   **Infrastructure**: Implementación de persistencia (EF Core), Unit of Work y servicios externos.
*   **API (Trabajo_ps)**: Controladores, middlewares y configuración de la app.

## 🛠️ Tecnologías utilizadas
*   **Backend**: C# 12, ASP.NET Core 8, Entity Framework Core 8.
*   **Frontend**: HTML5, CSS3 (Vanilla), JavaScript (ES6+).
*   **Base de Datos**: SQL Server.
*   **Documentación**: Swagger / OpenAPI.

---

## 📋 Observaciones Corregidas (Entrega 2)
*   [x] Eliminación de dependencias de BD en Application.
*   [x] Implementación de Middleware Global de Excepciones.
*   [x] Rutas RESTful (sin verbos en la URL).
*   [x] Paginación en catálogo de eventos.
*   [x] Fluent API con precisión y longitudes explícitas.
*   [x] Auditoría de intentos fallidos.
*   [x] Actualización parcial del mapa de asientos (sin recargar).
*   [x] Sistema de notificaciones moderno (Toasts).
