# TicketPlatform - Proyecto de Software

Plataforma de venta de entradas robusta que garantiza la integridad de los datos bajo alta demanda. Desarrollado aplicando principios de Clean Architecture y REST.

## Tecnologías Utilizadas
- **Backend:** C# (.NET Core), Entity Framework Core (Code-First)
- **Base de Datos:** LocalDB / SQL Server (configurado en `appsettings.json`)
- **Frontend:** HTML5, CSS3, JavaScript (Vanilla)
- **Arquitectura:** Clean Architecture, CQRS, Patrón Unit of Work, Repository Pattern.

## Requisitos Previos
- [.NET SDK](https://dotnet.microsoft.com/download) instalado en tu equipo.
- Visual Studio o Visual Studio Code.
- SQL Server Express / LocalDB (usualmente incluido con Visual Studio).

## Pasos para ejecutar el proyecto

### 1. Compilar el proyecto
Abre una terminal en la carpeta raíz del proyecto (`Trabajo_ps`) y restaura los paquetes/compila:
```bash
dotnet build
```

### 2. Configurar y Levantar la Base de Datos
El proyecto utiliza **Entity Framework Core (Code-First)**. La creación de la base de datos y la carga de datos iniciales (1 Evento, 2 Sectores, 100 Butacas, 1 Usuario) se ejecutan automáticamente al iniciar la aplicación por primera vez gracias al archivo `SeedData.cs`.

Si de todas formas deseas aplicar las migraciones manualmente, puedes correr:
```bash
cd Infrastructure
dotnet ef database update --startup-project ../Trabajo_ps
```
*(Asegúrate de tener instaladas las herramientas de ef core: `dotnet tool install --global dotnet-ef`)*

### 3. Lanzar la Aplicación
Navega a la carpeta principal del proyecto Web API (`Trabajo_ps` que contiene el archivo `Program.cs`) y ejecuta:
```bash
cd Trabajo_ps
dotnet run
```

### 4. Acceder a la Interfaz y a la API
Una vez que la consola indique que la aplicación está escuchando peticiones (usualmente en el puerto 5000 o 5001):
- **Frontend (UI de Venta):** Abre tu navegador e ingresa a `http://localhost:5000/index.html` (o en su defecto `https://localhost:5001/index.html`).
- **Documentación de la API (Swagger):** Abre tu navegador e ingresa a `http://localhost:5000/swagger` para ver todos los endpoints disponibles.

---

## Detalle de Funcionalidades Implementadas (Entrega 1)
- **Modelo de Base de Datos:** Entidades configuradas (Events, Sectors, Seats, Users, Reservations, AuditLogs).
- **Code-First & Migrations:** Generado con EF Core.
- **Optimistic Locking:** Las butacas incluyen un campo `Version` (`[ConcurrencyCheck]`) para garantizar la integridad frente a la alta concurrencia.
- **Auditoría:** Se genera un registro inmutable en `AuditLogs` cada vez que se bloquea un asiento.
- **Transaccionalidad (ACID):** Se utiliza `IUnitOfWork` (BeginTransaction, Commit, Rollback).
- **API RESTful:** Jerarquías plurales correctas (`GET /api/v1/Events/{id}/seats` y `GET /api/v1/Events/{id}/sectors`). Uso de códigos HTTP correctos (`200 OK`, `400 Bad Request`, `409 Conflict`).
- **Frontend Asíncrono:** La interfaz consulta la API mediante `Fetch API` y actualiza la vista de forma reactiva (sin recargar la página), mostrando el mapa y sus estados (Disponible, Reservado, Vendido).
- **Manejo de UI y UX:** Al hacer clic en un asiento disponible, se efectúa la reserva, la UI muestra un modal con un temporizador de 5 minutos, y refleja alertas al usuario.
