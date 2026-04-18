# TicketPlatform - Sistema de Venta de Entradas (Trabajo Práctico)

Este proyecto es una plataforma robusta para la gestión de eventos y reserva de butacas numeradas, diseñado para soportar alta demanda y garantizar la integridad de los datos mediante el uso de **Arquitectura Limpia (Clean Architecture)** y patrones de diseño modernos.

## 🚀 Primera Entrega: Fundamentos y Catálogo

Esta entrega cumple con los siguientes requerimientos:
- **Base de Datos**: Modelo relacional completo con migración automática y precarga de datos (SeedData).
- **Backend API**: Endpoints RESTful para eventos, sectores y butacas con jerarquía estricta.
- **Frontend**: Interfaz UI/UX premium con catálogo de eventos y mapa dinámico de asientos.
- **Auditoría**: Registro inmutable de cada acción crítica.

---

## 🏗️ Arquitectura del Sistema

Se implementó una **Arquitectura Limpia** dividida en 4 capas para asegurar el desacoplamiento y la testeabilidad:

1. **Domain (Núcleo)**: Entidades puras y reglas de negocio. Contiene las entidades `Event`, `Seat`, `Sector`, `Reservation`, `User` y `AuditLog`.
2. **Application (Orquestación)**: Contiene la lógica de los Casos de Uso. Se utilizó el patrón **Command/Query/Handler** manual para procesar cada requerimiento (ej: `ReserveSeatHandler`).
3. **Infrastructure (Persistencia)**: Implementación de Entity Framework Core, repositorios genéricos y el patrón **Unit Of Work** para garantizar la atomicidad en las transacciones.
4. **Presentation (Interfaz)**: API REST construida en ASP.NET Core y un Frontend SPA en Vanilla JS/CSS servido estáticamente.

---

## 🛠️ Tecnologías Utilizadas

- **Backend**: .NET 8, ASP.NET Core API.
- **Persistencia**: Entity Framework Core con SQL Server (Optimistic Locking mediante campo `Version`).
- **Frontend**: HTML5, CSS3 (Modern Glassmorphism Design), Vanilla JavaScript.
- **Documentación**: Swagger / OpenAPI.

---

## 🏁 Pasos para Ejecutar el Proyecto

### Requisitos Previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB o Express).

### Configuración
1. Clonar el repositorio.
2. Abrir la solución en Visual Studio o VS Code.
3. Asegurarse de que la cadena de conexión en `Presentation/appsettings.json` apunte a su servidor de base de datos local:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TicketPlatformDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

### Ejecución
1. Abrir una terminal en el directorio raíz.
2. Ejecutar el proyecto de presentación:
   ```bash
   dotnet run --project Presentation/Presentation.csproj
   ```
3. Al iniciar, el sistema ejecutará automáticamente el **SeedData**, cargando:
   - 1 Evento (Concierto de Rock 2026).
   - 2 Sectores (Platea Baja y Alta).
   - 100 Butacas (50 por sector).
   - 1 Usuario de prueba (ID: 1).

4. Acceder vía navegador a: `http://localhost:5000` (o el puerto que indique la consola).
5. Swagger disponible en: `http://localhost:5000/swagger`.

---

## ⚖️ Decisiones de Diseño (Justificación)

- **Patrón Command/Handler**: Se eligió para cumplir con la separación de responsabilidades solicitada, permitiendo que cada acción del sistema sea una unidad aislada y auditable.
- **Optimistic Locking**: El modelo `Seat` cuenta con una propiedad `[Timestamp] byte[] Version` para prevenir que dos usuarios reserven el mismo asiento simultáneamente (concurrencia de datos).
- **Auditoría Automática**: El `AppDbContext` sobrescribe `SaveChangesAsync` para inyectar automáticamente valores en `CreatedAt` y `UpdatedAt` en todas las entidades.

---
**Cátedra**: Proyecto de Software  
**Docente**: Cabral, Leonardo.
**Alumnos**: Condori, Edson. Mendes, Ana.
