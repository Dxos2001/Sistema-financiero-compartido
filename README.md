# 💳 Sistema Financiero Compartido — Backend API

[![.NET Version](https://img.shields.io/badge/.NET-10.0%20%2F%209.0-purple?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-blue?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Entity Framework Core](https://img.shields.io/badge/ORM-EF%20Core%209.0-512BD4?logo=nuget)](https://docs.microsoft.com/ef/core/)
[![MySQL](https://img.shields.io/badge/Database-MySQL%20%2F%20MariaDB-4479A1?logo=mysql)](https://www.mysql.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-success)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![Swagger](https://img.shields.io/badge/API%20Docs-Swagger%20%2F%20OpenAPI-85EA2D?logo=swagger)](https://swagger.io/)

Plataforma backend transaccional de alta concurrencia diseñada bajo **Clean Architecture** para la administración de **grupos financieros solidarios**, flujos de caja mancomunados, mecanismos de **consenso descentralizado** para desembolsos y trazabilidad estricta mediante **Audit Logs**.

---

## 🏛️ Arquitectura del Sistema (Clean Architecture)

El proyecto sigue estrictamente los principios de **Clean Architecture** y **Domain-Driven Design (DDD)** para garantizar la separación de responsabilidades, testabilidad y desacoplamiento de infraestructura:

```
┌─────────────────────────────────────────────────────────────┐
│                      Controllers (API)                      │
│       (AuthController, Consensus, FinancialGroups, etc.)    │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│                    Application Services                     │
│  (AuthService, ConsensusRequestService, AuditLogService)    │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│                    Domain Layer (Core)                      │
│        (Entities, DTOs, Value Objects, Interfaces)          │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│                     Infrastructure                          │
│     (EF Core 9, Pomelo MySQL, DbContext, Migrations)        │
└─────────────────────────────────────────────────────────────┘
```

---

## 🌟 Módulos y Lógica de Negocio

1. **Gestión de Grupos Financieros (`FinancialGroups`):**
   * Creación y membresía de grupos de ahorro y crédito mancomunado.
   * Reglas de balance y límites crediticios por miembro (`GroupMembers`).
2. **Mecanismo de Consenso para Desembolsos (`ConsensusRequests`):**
   * Solicitudes de retiro o préstamos que requieren aprobación y votación de los miembros del grupo antes de su ejecución.
3. **Pistas de Auditoría Transaccional (`AuditLogs`):**
   * Registro inmutable de cada operación, cambio de estado y transacción financiera con marcas temporales e identificación de usuario.
4. **Transacciones Seguras (`Transactions`):**
   * Registro de depósitos, retiros y transferencias entre balances grupales e individuales.
5. **Autenticación y Control de Identidad (`Auth` / `Users` / `Persons`):**
   * Autenticación basada en JWT y separación entre entidad de seguridad (`User`) y persona natural (`Person`).

---

## 📡 Catálogo de Endpoints REST

| Módulo | Endpoint Base | Descripción |
| :--- | :--- | :--- |
| **Auth** | `/api/Auth` | Registro, autenticación y generación de tokens JWT |
| **Grupos** | `/api/FinancialGroups` | CRUD de grupos de ahorro y crédito |
| **Miembros** | `/api/GroupMembers` | Administración de miembros y roles dentro del grupo |
| **Consenso** | `/api/ConsensusRequests` | Creación de solicitudes y votación de consenso |
| **Transacciones** | `/api/Transactions` | Ejecución de movimientos financieros |
| **Auditoría** | `/api/AuditLogs` | Consulta de pistas de auditoría y trazabilidad |
| **Personas** | `/api/Persons` | Perfil demográfico y datos del cliente |
| **Usuarios** | `/api/Users` | Gestión de cuentas de usuario del sistema |

---

## 🛠️ Requisitos e Instalación

### Requisitos Previos:
* **.NET SDK 9.0 o 10.0**
* **MySQL 8.0+** o **MariaDB 10.6+**

### Ejecución Local:

1. **Clonar repositorio:**
   ```bash
   git clone https://github.com/Dxos2001/Sistema-financiero-compartido.git
   cd Sistema-financiero-compartido/FinancieraBackend
   ```

2. **Configurar cadena de conexión:**
   Ajusta `appsettings.json` o usa User Secrets:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=financiera_db;User=root;Password=tu_password;"
     }
   }
   ```

3. **Aplicar Migraciones de Base de Datos:**
   ```bash
   dotnet ef database update
   ```

4. **Compilar y Ejecutar:**
   ```bash
   dotnet run
   ```

5. **Exploración de la API:**
   Navega a `http://localhost:5000/swagger` para acceder a la consola interactiva Swagger UI.

---

## 👤 Autor

* **Alex León** — *Semi-Senior Backend Engineer* | [GitHub: @Dxos2001](https://github.com/Dxos2001)
