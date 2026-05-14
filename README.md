# Ticketing Platform - Proyecto de Software 🎫

Sistema robusto de reserva y venta de entradas con gestión de alta concurrencia, transacciones ACID y procesos asíncronos.

## 🚀 Características Principales

- **Arquitectura Limpia (Clean Architecture):** Separación estricta de responsabilidades en capas (Domain, Application, Infrastructure, Web API).
- **Control de Concurrencia:** Implementación de *Optimistic Locking* mediante campos de versión para evitar la doble reserva de butacas.
- **Transaccionalidad ACID:** Operaciones críticas (reserva y pago) envueltas en transacciones para asegurar la integridad de los datos.
- **Procesos en Segundo Plano:** Background Service encargado de la liberación automática de reservas vencidas (5 minutos).
- **Auditoría Completa:** Registro inmutable de cada acción (intentos exitosos, fallidos, pagos y liberaciones).
- **API RESTful:** Cumplimiento de estándares de industria (sustantivos plurales, códigos HTTP correctos, versionado v1).
- **Frontend Dinámico:** Interfaz moderna con catálogo de eventos, mapa de asientos interactivo, temporizadores y notificaciones en tiempo real.

## 🛠️ Tecnologías Utilizadas

- **Backend:** .NET 8, Entity Framework Core 8, SQL Server.
- **Frontend:** HTML5, CSS3 (Vanilla), JavaScript (Fetch API).
- **Documentación:** Swagger / OpenAPI.

## 📦 Instalación y Configuración

### Pre-requisitos
- .NET 8 SDK
- SQL Server (LocalDB o instancia superior)

### 1. Configuración de la Base de Datos
Desde la raíz del proyecto, ejecuta los siguientes comandos para aplicar las migraciones y crear la base de datos:

```bash
# Navegar a la carpeta del proyecto API
cd Trabajo_ps

# Ejecutar migraciones (se crearán automáticamente los datos de prueba / Seed)
dotnet ef database update --project ../Infrastructure --startup-project .
```

### 2. Ejecutar el Backend
```bash
dotnet run
```
La API estará disponible en `http://localhost:5000` y la documentación interactiva en `http://localhost:5000/swagger`.

### 3. Ejecutar el Frontend
El frontend es independiente del backend. Puedes abrir el archivo `Frontend/index.html` directamente en tu navegador o utilizar un servidor estático (ej: Live Server en VS Code).

Asegúrate de que la API esté corriendo para que el frontend pueda consumir los datos.

## 📝 Auditoría y Trazabilidad
El sistema registra cada milisegundo de actividad en la tabla `AuditLogs`. Puedes consultar esta tabla para verificar la resolución de conflictos de concurrencia y la liberación automática de asientos.

---
**Cátedra:** Proyecto de Software  
**Docente:** Ing. Olivera Lucas  
**Grupo:** 10 (Mendes Ana, Condori Edson)
