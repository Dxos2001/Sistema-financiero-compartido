# Sistema Financiero Compartido — Pseudocódigo

> Backend API REST construido con ASP.NET Core + Entity Framework Core + MySQL
> Integra un modelo de lenguaje local (Ollama / Qwen) para generar proyecciones financieras.

---

## Tabla de Contenidos

1. [Arquitectura General](#1-arquitectura-general)
2. [Modelos de Dominio](#2-modelos-de-dominio)
3. [Infraestructura — Conexión a la Base de Datos](#3-infraestructura--conexión-a-la-base-de-datos)
4. [Servicios de Aplicación](#4-servicios-de-aplicación)
   - 4.1 [AuthService](#41-authservice)
   - 4.2 [PersonService](#42-personservice)
   - 4.3 [UserService](#43-userservice)
   - 4.4 [FinancialGroupService](#44-financialgroupservice)
   - 4.5 [GroupMemberService](#45-groupmemberservice)
   - 4.6 [TransactionsService](#46-transactionsservice)
   - 4.7 [AuditLogService](#47-auditlogservice)
   - 4.8 [DeepSeekProjectionService](#48-deepseekprojectionservice)
5. [Controladores (Endpoints HTTP)](#5-controladores-endpoints-http)
6. [Arranque de la Aplicación (Program.cs)](#6-arranque-de-la-aplicación-programcs)

---

## 1. Arquitectura General

```
Cliente HTTP (Frontend / Postman)
        │
        ▼
┌─────────────────────────────────────┐
│          Controllers (HTTP API)      │  ← Enruta peticiones, retorna respuestas JSON
└──────────────┬──────────────────────┘
               │ inyección de dependencias
               ▼
┌─────────────────────────────────────┐
│       Application / Services         │  ← Lógica de negocio
│  (AuthService, TransactionsService,  │
│   DeepSeekProjectionService, etc.)   │
└──────────────┬──────────────────────┘
               │ consultas / escrituras
               ▼
┌─────────────────────────────────────┐
│   Infrastructure / DBConnection      │  ← Entity Framework Core (DbContext)
│          (MySQL vía Pomelo)          │
└─────────────────────────────────────┘
               │ LLM externo
               ▼
┌─────────────────────────────────────┐
│   Ollama API (Qwen 2.5 1.5b)        │  ← Genera texto del reporte de proyección
└─────────────────────────────────────┘
```

---

## 2. Modelos de Dominio

### Entidad: Persons
```
ENTIDAD Persons:
  id              : entero, clave primaria, autoincremental
  firstName       : cadena (máx 50), obligatorio
  lastName        : cadena (máx 50), obligatorio
  middleName      : cadena (máx 50), opcional
  dateOfBirth     : fecha, opcional
  identificationNumber : cadena (máx 20), opcional
  phone           : cadena (máx 15), opcional
  address         : cadena (máx 200), opcional
  createdAt       : fechaHora, valor por defecto = ahora
  ── relaciones ──
  user            : → Users (uno a uno)
```

### Entidad: Users
```
ENTIDAD Users:
  id          : entero, clave primaria, autoincremental
  username    : cadena (máx 50), obligatorio
  email       : cadena (máx 100), obligatorio, formato email
  personId    : entero, clave foránea → Persons
  createdAt   : fechaHora, valor por defecto = ahora
  ── relaciones ──
  person           : → Persons (uno a uno)
  createdGroups    : → FinancialGroups[] (uno a muchos)
  groupMemberships : → GroupMembers[] (uno a muchos)
  transactions     : → Transactions[] (uno a muchos)
```

### Entidad: FinancialGroups
```
ENTIDAD FinancialGroups:
  id                   : entero, clave primaria, autoincremental
  name                 : cadena (máx 100), obligatorio
  createdBy            : entero, clave foránea → Users
  balance              : decimal(15,2), por defecto 0.00
  createdAt            : fechaHora, valor por defecto = ahora
  cachedProjection     : texto largo, opcional  ← caché del reporte IA
  lastProjectionBalance: decimal(15,2), opcional ← balance cuando se cacheó
  ── relaciones ──
  creator      : → Users
  members      : → GroupMembers[]
  transactions : → Transactions[]
```

### Entidad: GroupMembers
```
ENTIDAD GroupMembers:
  ── clave compuesta ──
  groupId : entero, clave foránea → FinancialGroups
  userId  : entero, clave foránea → Users
  role    : enum { Admin, Member }, por defecto Member
  ── relaciones ──
  group : → FinancialGroups
  user  : → Users
```

### Entidad: Transactions
```
ENTIDAD Transactions:
  id        : entero, clave primaria, autoincremental
  groupId   : entero, clave foránea → FinancialGroups
  userId    : entero, clave foránea → Users
  amount    : decimal(15,2)
  type      : enum { Income, Expense }
  status    : enum { Pending, Approved, Rejected }, por defecto Pending
  createdAt : fechaHora, valor por defecto = ahora
  ── relaciones ──
  group     : → FinancialGroups
  user      : → Users
  auditLogs : → AuditLogs[]
```

### Entidad: AuditLogs
```
ENTIDAD AuditLogs:
  id             : entero, clave primaria, autoincremental
  transactionId  : entero, clave foránea → Transactions
  action         : cadena (máx 50), obligatorio  ← "Create" | "Update" | "Delete"
  hashIntegrity  : cadena (máx 256), obligatorio ← SHA-256 de la transacción
  timestamp      : fechaHora, valor por defecto = ahora
  ── relaciones ──
  transaction : → Transactions
```

---

## 3. Infraestructura — Conexión a la Base de Datos

```
CLASE DBConnection (hereda de DbContext):

  CONJUNTOS:
    Users           ← tabla Users
    Persons         ← tabla Persons
    FinancialGroups ← tabla FinancialGroups
    GroupMembers    ← tabla GroupMembers
    Transactions    ← tabla Transactions
    AuditLogs       ← tabla AuditLogs

  AL CREAR EL MODELO:
    Users  ──(1)──> Persons          [restricción al eliminar]
    FinancialGroups ──(N)──> Users   [restricción al eliminar]
    GroupMembers    ──(N)──> Groups  [cascada al eliminar]
    GroupMembers    ──(N)──> Users   [cascada al eliminar]
    Transactions    ──(N)──> Groups  [cascada al eliminar]
    Transactions    ──(N)──> Users   [restricción al eliminar]
    AuditLogs       ──(N)──> Transactions [cascada al eliminar]
    GroupMembers usa clave compuesta: {GroupId, UserId}
```

---

## 4. Servicios de Aplicación

### 4.1 AuthService

```
FUNCIÓN Autenticar(loginDTO) → cadena | nulo:
  usuario ← buscarEnBD(Users donde username == loginDTO.username)
  SI usuario es nulo:
    RETORNAR nulo
  FIN SI
  // TODO: verificar hash de contraseña (ej. BCrypt)
  RETORNAR "simulated_jwt_token_for_" + usuario.username
```

---

### 4.2 PersonService

```
FUNCIÓN CrearPersona(dto) → Persons:
  persona ← nueva Persons con (firstName, lastName, middleName,
                                dateOfBirth, identificationNumber,
                                phone, address, createdAt=ahora)
  guardarEnBD(persona)
  RETORNAR persona

FUNCIÓN EliminarPersona(identificationNumber) → booleano:
  persona ← buscarEnBD(Persons donde identificationNumber == id)
  SI persona es nula → RETORNAR falso
  eliminarDeBD(persona)
  RETORNAR verdadero

FUNCIÓN ObtenerTodasLasPersonas() → Lista<Persons>:
  RETORNAR todasLasPersonas()

FUNCIÓN ObtenerPersona(identificationNumber) → Persons:
  RETORNAR buscarEnBD(Persons donde identificationNumber == id)

FUNCIÓN PersonaExiste(identificationNumber) → booleano:
  RETORNAR existeEnBD(Persons donde identificationNumber == id)

FUNCIÓN ActualizarPersona(identificationNumber, dto) → booleano:
  persona ← buscarEnBD(...)
  SI persona es nula → RETORNAR falso
  actualizar todos los campos de persona con dto
  guardarEnBD(persona)
  RETORNAR verdadero
```

---

### 4.3 UserService

```
FUNCIÓN CrearUsuario(dto) → Users:
  usuario ← nuevo Users con (username, email, personId, createdAt=ahora)
  guardarEnBD(usuario)
  RETORNAR usuario

FUNCIÓN EliminarUsuario(username) → booleano:
  usuario ← buscarEnBD(Users donde username == dto.username)
  SI nulo → RETORNAR falso
  eliminarDeBD(usuario)
  RETORNAR verdadero

FUNCIÓN ObtenerTodosLosUsuarios() → Lista<Users>:
  RETORNAR todosLosUsuarios()

FUNCIÓN ObtenerUsuario(username) → Users:
  RETORNAR buscarEnBD(Users donde username == username)

FUNCIÓN UsuarioExiste(username) → booleano:
  RETORNAR existeEnBD(Users donde username == username)

FUNCIÓN ActualizarUsuario(username, dto) → booleano:
  usuario ← buscarEnBD(...)
  SI nulo → RETORNAR falso
  usuario.email ← dto.email
  guardarEnBD(usuario)
  RETORNAR verdadero

FUNCIÓN ObtenerUsuariosPorGrupo(groupId) → Lista<Users>:
  RETORNAR GroupMembers
    DONDE groupId == groupId
    INCLUIR User → Person
    SELECCIONAR User
```

---

### 4.4 FinancialGroupService

```
FUNCIÓN CrearGrupo(dto) → FinancialGroups:
  grupo ← nuevo FinancialGroups con (name, createdBy, balance=0, createdAt=ahora)
  guardarEnBD(grupo)
  RETORNAR grupo

FUNCIÓN EliminarGrupo(id) → booleano:
  grupo ← buscarEnBD(FinancialGroups por id)
  SI nulo → RETORNAR falso
  eliminarDeBD(grupo)
  RETORNAR verdadero

FUNCIÓN ObtenerTodosLosGrupos() → Lista<FinancialGroups>:
  grupos ← todosLosGrupos()
  balances ← agrupar Transactions por groupId:
              calcular suma(Income) - suma(Expense) por cada grupo
  PARA cada grupo EN grupos:
    SI existe balance calculado:
      grupo.balance ← balanceCalculado
    SINO:
      grupo.balance ← 0
  RETORNAR grupos

FUNCIÓN ObtenerGrupo(id) → FinancialGroups:
  grupo ← buscarEnBD(FinancialGroups por id)
  SI grupo no es nulo:
    ingresos ← sumar Transactions donde groupId==id Y type==Income
    gastos   ← sumar Transactions donde groupId==id Y type==Expense
    grupo.balance ← ingresos - gastos
  RETORNAR grupo

FUNCIÓN GrupoExiste(id) → booleano:
  RETORNAR existeEnBD(FinancialGroups donde id == id)

FUNCIÓN ActualizarGrupo(id, dto) → booleano:
  grupo ← buscarEnBD(...)
  SI nulo → RETORNAR falso
  grupo.name ← dto.name
  guardarEnBD(grupo)
  RETORNAR verdadero
```

---

### 4.5 GroupMemberService

```
FUNCIÓN AgregarMiembro(dto) → GroupMembers:
  miembro ← nuevo GroupMembers con (groupId, userId, role)
  guardarEnBD(miembro)
  RETORNAR miembro

FUNCIÓN ObtenerGruposPorUsuario(userId) → Lista<GroupMembers>:
  RETORNAR GroupMembers donde userId == userId

FUNCIÓN ObtenerMiembrosPorGrupo(groupId) → Lista<GroupMembers>:
  RETORNAR GroupMembers
    DONDE groupId == groupId
    INCLUIR User → Person

FUNCIÓN EsMiembro(groupId, userId) → booleano:
  RETORNAR existeEnBD(GroupMembers donde groupId==groupId Y userId==userId)

FUNCIÓN EliminarMiembro(groupId, userId) → booleano:
  miembro ← buscarEnBD(GroupMembers donde groupId==groupId Y userId==userId)
  SI nulo → RETORNAR falso
  eliminarDeBD(miembro)
  RETORNAR verdadero

FUNCIÓN ActualizarRolDeMiembro(groupId, userId, dto) → booleano:
  miembro ← buscarEnBD(GroupMembers donde groupId==groupId Y userId==userId)
  SI nulo → RETORNAR falso
  miembro.role ← dto.role
  guardarEnBD(miembro)
  RETORNAR verdadero
```

---

### 4.6 TransactionsService

```
FUNCIÓN GenerarHash(transaccion) → cadena:
  datos ← "{id}-{amount}-{type}-{status}-{createdAt}"
  RETORNAR SHA256(datos) codificado en Base64

──────────────────────────────────────────────────────────

FUNCIÓN CrearTransaccion(dto) → Transactions:
  transaccion ← nueva Transactions con (groupId, userId, amount, type,
                                         status=Pending, createdAt=ahora)
  guardarEnBD(transaccion)

  // Auditoría automática
  auditLog ← nuevo AuditLogs con (transactionId, action="Create",
                                   hashIntegrity=GenerarHash(transaccion),
                                   timestamp=ahora)
  guardarEnBD(auditLog)

  // Actualizar balance del grupo
  grupo ← buscarEnBD(FinancialGroups por transaccion.groupId)
  SI grupo no es nulo:
    SI type == Income → grupo.balance += amount
    SINO              → grupo.balance -= amount
    guardarEnBD(grupo)

  RETORNAR transaccion

──────────────────────────────────────────────────────────

FUNCIÓN EliminarTransaccion(id) → booleano:
  transaccion ← buscarEnBD(Transactions por id)
  SI nula → RETORNAR falso

  // Auditoría automática
  auditLog ← nuevo AuditLogs con (action="Delete", hash=GenerarHash(transaccion))
  guardarEnBD(auditLog)

  // Revertir balance del grupo
  grupo ← buscarEnBD(FinancialGroups por transaccion.groupId)
  SI grupo no es nulo:
    SI type == Income → grupo.balance -= amount   ← revertir
    SINO              → grupo.balance += amount   ← revertir

  eliminarDeBD(transaccion)
  RETORNAR verdadero

──────────────────────────────────────────────────────────

FUNCIÓN ActualizarTransaccion(id, dto) → booleano:
  transaccion ← buscarEnBD(Transactions por id)
  SI nula → RETORNAR falso

  grupo ← buscarEnBD(FinancialGroups por transaccion.groupId)
  SI grupo no es nulo:
    // Revertir impacto anterior
    SI transaccion.type == Income → grupo.balance -= transaccion.amount
    SINO                          → grupo.balance += transaccion.amount
    // Aplicar nuevo impacto
    SI dto.type == Income → grupo.balance += dto.amount
    SINO                  → grupo.balance -= dto.amount
    guardarEnBD(grupo)

  transaccion.amount ← dto.amount
  transaccion.type   ← dto.type
  guardarEnBD(transaccion)

  // Auditoría automática
  auditLog ← nuevo AuditLogs con (action="Update", hash=GenerarHash(transaccion))
  guardarEnBD(auditLog)

  RETORNAR verdadero

──────────────────────────────────────────────────────────

FUNCIÓN ObtenerTodasLasTransacciones() → Lista<Transactions>:
  RETORNAR todasLasTransacciones()

FUNCIÓN ObtenerTransaccion(id) → Transactions:
  RETORNAR buscarEnBD(Transactions por id)

FUNCIÓN TransaccionExiste(id) → booleano:
  RETORNAR existeEnBD(Transactions donde id == id)
```

---

### 4.7 AuditLogService

```
FUNCIÓN CrearLog(dto) → AuditLogs:
  log ← nuevo AuditLogs con (transactionId, action, hashIntegrity, timestamp=ahora)
  guardarEnBD(log)
  RETORNAR log

FUNCIÓN ObtenerTodosLosLogs() → Lista<AuditLogs>:
  RETORNAR todosLosLogs()

FUNCIÓN ObtenerLog(id) → AuditLogs:
  RETORNAR buscarEnBD(AuditLogs por id)

FUNCIÓN ObtenerLogsPorTransaccion(transactionId) → Lista<AuditLogs>:
  RETORNAR AuditLogs donde transactionId == transactionId
```

---

### 4.8 DeepSeekProjectionService

> Este servicio genera un reporte financiero de 6 meses usando un LLM local (Ollama).

```
FUNCIÓN GenerarProyeccionSeisMeses(groupId) → ProjectionResponseDTO:

  ── PASO 1: Validar grupo ──
  grupo ← buscarEnBD(FinancialGroups por groupId)
  SI grupo es nulo:
    RETORNAR { groupId, markdownReport = "El grupo financiero no existe." }

  ── PASO 2: Obtener transacciones ──
  transacciones ← buscarEnBD(Transactions donde groupId == groupId)
                  ordenadas por createdAt ASC

  totalIngresos ← suma(transacciones donde type == Income)
  totalGastos   ← suma(transacciones donde type == Expense)
  balanceActual ← totalIngresos - totalGastos

  ── PASO 3: Verificar caché ──
  SI grupo.lastProjectionBalance == balanceActual
     Y grupo.cachedProjection no está vacío:
    RETORNAR { groupId, markdownReport = grupo.cachedProjection }

  ── PASO 4: Filtrar últimos 3 meses ──
  hace3Meses          ← fechaActual - 3 meses
  transaccionesRecientes ← transacciones donde createdAt >= hace3Meses

  SI transaccionesRecientes está vacío:
    RETORNAR { groupId, markdownReport = "No hay transacciones en los últimos 3 meses..." }

  ── PASO 5: Calcular promedios mensuales ──
  ingresosRecientes ← suma(transaccionesRecientes donde type == Income)
  gastosRecientes   ← suma(transaccionesRecientes donde type == Expense)

  primerFecha      ← fecha mínima en transaccionesRecientes
  ultimaFecha      ← fecha máxima en transaccionesRecientes
  mesesDiferencia  ← calcular diferencia en meses (mínimo = 1)

  promedioIngresosXMes ← ingresosRecientes / mesesDiferencia
  promedioGastosXMes   ← gastosRecientes   / mesesDiferencia
  flujoNetoMensual     ← promedioIngresosXMes - promedioGastosXMes

  ── PASO 6: Construir tabla de proyección mensual ──
  tabla ← encabezado markdown: | Mes | Ingresos | Gastos | Flujo Neto | Balance Acumulado |
  balanceAcumulado ← balanceActual
  PARA i DESDE 1 HASTA 6:
    balanceAcumulado += flujoNetoMensual
    agregar fila: | Mes {i} | promedioIngresos | promedioGastos | flujoNeto | balanceAcumulado |

  balanceFinalProyectado ← balanceActual + (flujoNetoMensual × 6)

  ── PASO 7: Construir prompts para el LLM ──
  systemPrompt ← instrucción estricta para generar reporte Markdown con 3 secciones:
                  1. Resumen Actual
                  2. Proyección Cuantitativa Mensual
                  3. Riesgos y Recomendaciones

  userPrompt ← datos del grupo: balance, promedios, flujo neto, balance proyectado,
                lista de transacciones recientes

  ── PASO 8: Llamar a la API de Ollama ──
  cuerpo ← {
    model: "qwen2.5:1.5b",
    messages: [{ role:"system", content:systemPrompt },
               { role:"user",   content:userPrompt   }],
    options: { temperature: 0.3 },
    stream: false
  }
  respuesta ← HTTP POST "http://192.168.18.2:11434/api/chat" con cuerpo JSON

  SI respuesta no es exitosa:
    LANZAR excepción con código de error HTTP

  markdownGenerado ← extraer respuesta.message.content del JSON

  ── PASO 9: Inyectar tabla en el markdown ──
  SI markdownGenerado contiene "## 2. Proyección Cuantitativa Mensual":
    insertar tabla justo después de esa sección
  SINO:
    agregar tabla al final con encabezado "## Proyección Cuantitativa Mensual (Insertada Automáticamente)"

  ── PASO 10: Guardar en caché ──
  grupo.cachedProjection     ← markdownGenerado (con tabla)
  grupo.lastProjectionBalance ← balanceActual
  guardarEnBD(grupo)

  RETORNAR { groupId, markdownReport = markdownGenerado }
```

---

## 5. Controladores (Endpoints HTTP)

### AuthController — `/api/auth`
```
POST /login
  CUERPO: { username, password }
  token ← AuthService.Autenticar(dto)
  SI token es nulo → 401 Unauthorized
  SINO → 200 OK { token }
```

### PersonsController — `/api/persons`
```
GET    /            → 200 OK con lista de personas
GET    /{id}        → 200 OK | 404 Not Found
POST   /            → 201 Created con persona nueva
PUT    /{id}        → 200 OK | 404 Not Found
DELETE /{id}        → 204 No Content | 404 Not Found
```

### UsersController — `/api/users`
```
GET    /                      → lista de usuarios
GET    /{username}            → usuario por username
GET    /by-group/{groupId}    → usuarios pertenecientes al grupo
POST   /                      → crear usuario
PUT    /{username}            → actualizar email
DELETE /{username}            → eliminar usuario
```

### FinancialGroupsController — `/api/financialgroups`
```
GET    /       → lista de grupos (con balances calculados)
GET    /{id}   → grupo por id
POST   /       → crear grupo
PUT    /{id}   → actualizar nombre del grupo
DELETE /{id}   → eliminar grupo
```

### GroupMembersController — `/api/groupmembers`
```
GET    /group/{groupId}              → miembros del grupo
GET    /user/{userId}                → grupos de un usuario
POST   /                             → agregar miembro
PUT    /group/{groupId}/user/{userId}→ actualizar rol
DELETE /group/{groupId}/user/{userId}→ eliminar miembro
```

### TransactionsController — `/api/transactions`
```
GET    /       → todas las transacciones
GET    /{id}   → transacción por id
POST   /       → crear transacción (+ auditoría + actualiza balance)
PUT    /{id}   → actualizar transacción (+ auditoría + ajusta balance)
DELETE /{id}   → eliminar transacción (+ auditoría + revierte balance)
```

### AuditLogsController — `/api/auditlogs`
```
GET    /                           → todos los logs
GET    /{id}                       → log por id
GET    /transaction/{transactionId}→ logs por transacción
POST   /                           → crear log manualmente
```

### ProjectionsController — `/api/projections`
```
GET /group/{groupId}
  → DeepSeekProjectionService.GenerarProyeccionSeisMeses(groupId)
  → 200 OK { groupId, markdownReport }
```

---

## 6. Arranque de la Aplicación (Program.cs)

```
INICIO de la aplicación:

  1. Configurar JSON para ignorar referencias circulares

  2. Registrar servicios en el contenedor DI:
       - DBConnection (EF Core con MySQL)
       - PersonService, UserService, FinancialGroupService
       - GroupMemberService, TransactionsService, AuditLogService
       - AuthService
       - HttpClient (para llamadas al LLM)
       - DeepSeekProjectionService

  3. Configurar CORS (política "AllowFrontend"):
       Orígenes permitidos:
         - http://localhost:5173  (Vite dev)
         - http://localhost:4173  (Vite preview)
         - http://localhost:3000  (dev alternativo)
         - https://financierafrontend.d-xos.com (producción)
       Cabeceras: cualquiera
       Métodos: cualquiera
       Credenciales: permitidas

  4. Configurar pipeline HTTP:
       - Middleware de excepciones globales (GlobalExceptionMiddleware)
       - CORS
       - Swagger + SwaggerUI
       - Mapeo de controladores

  5. Endpoint de prueba de conexión:
       GET /test-connection
         → abrir conexión MySQL
         → SI éxito → "realizado con exito"
         → SI error → "Error: {mensaje}"

  6. Iniciar servidor (app.Run())
```

---

## Diagrama de Flujo — Crear una Transacción

```
Cliente → POST /api/transactions
              │
              ▼
     TransactionsController
       llama TransactionsService.CrearTransaccion(dto)
              │
              ├─ 1. Crear registro Transactions (status=Pending)
              │        └─ guardar en BD
              │
              ├─ 2. Crear AuditLog (action="Create", hash=SHA256(...))
              │        └─ guardar en BD
              │
              └─ 3. Buscar FinancialGroup por groupId
                       SI type == Income → balance += amount
                       SINO             → balance -= amount
                       └─ actualizar grupo en BD
              │
              ▼
     201 Created ← retorna Transactions creada
```

## Diagrama de Flujo — Generar Proyección con IA

```
Cliente → GET /api/projections/group/{groupId}
              │
              ▼
     ProjectionsController
       llama DeepSeekProjectionService.GenerarProyeccionSeisMeses(groupId)
              │
              ├─ Validar que el grupo exista
              ├─ Calcular balance actual de transacciones
              ├─ ¿Caché válida? → SÍ → retornar cached markdown
              │                 ↓ NO
              ├─ Filtrar transacciones últimos 3 meses
              ├─ Calcular promedios mensuales de ingresos/gastos
              ├─ Construir tabla markdown de proyección mes a mes
              ├─ Construir system prompt + user prompt con los datos
              ├─ POST http://ollama:11434/api/chat (modelo qwen2.5:1.5b)
              ├─ Recibir markdown del LLM
              ├─ Inyectar tabla en la sección correcta
              ├─ Guardar en caché (grupo.cachedProjection)
              │
              ▼
     200 OK ← { groupId, markdownReport }
```
