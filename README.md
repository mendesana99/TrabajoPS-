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
  
**Nota:** Podria ocurrir al ejecutar proyecto, si se tienen tablas previamente generadas, que no termine de compilarse. Borrando las tablas preexistentes se soluciona esto.

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

## Detalle de Funcionalidades Implementadas (Entrega 1 y Entrega 2)
### Entrega 1: Fundamentos y Reserva Inicial
- **Modelo de Base de Datos:** Entidades configuradas (Events, Sectors, Seats, Users, Reservations, AuditLogs).
- **Code-First & Migrations:** Generado con EF Core. Precarga inicial con `SeedData.cs`.
- **API RESTful:** Jerarquías plurales correctas (`GET /api/v1/Events/{id}/seats`). Uso de códigos HTTP correctos (`200 OK`, `400 Bad Request`).
- **Frontend y UX:** La interfaz asíncrona (Vanilla JS) permite visualizar el catálogo y el mapa de asientos, distinguiendo visualmente las butacas.

### Entrega 2: Alta Concurrencia, Transacciones y Procesos Asíncronos
- **Optimistic Locking (Control de Concurrencia):** Las butacas incluyen un campo `Version` (`[ConcurrencyCheck]`) para evitar doble asignación. Devuelve un `409 Conflict` adecuadamente.
- **Transaccionalidad Estricta (ACID):** Se utiliza `IUnitOfWork` (BeginTransaction, Commit, Rollback) durante la confirmación del pago para garantizar atomicidad entre el estado de butaca, reserva y auditoría.
- **Liberación Automática (Background Job):** Se implementó un `ReservationCleanupService` (Worker/Cron) que corre en segundo plano y libera automáticamente las reservas con más de 5 minutos de antigüedad sin pagar.
- **Temporizador y UX:** Frontend implementa cuenta regresiva visual y notifica instantáneamente cuando hay conflictos de concurrencia refrescando el mapa.
- **Auditoría y Trazabilidad:** Se genera un registro inmutable en `AuditLogs` en cada intento exitoso o fallido de reserva, pago y expiración automática con detalles precisos.
