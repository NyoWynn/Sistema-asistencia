# 📊 Sistema de Asistencia

Un sistema web completo para el control de asistencia de empleados desarrollado en ASP.NET Core 8.0 con Entity Framework Core y SQL Server.

## 🚀 Características Principales

### Para Empleados
- **Registro de Asistencia**: Marcar entrada y salida con un solo clic
- **Estado en Tiempo Real**: Visualización del estado actual de asistencia
- **Interfaz Intuitiva**: Diseño simple y fácil de usar
- **Validaciones**: Prevención de registros duplicados o inválidos

### Para Administradores
- **Gestión de Usuarios**: Crear, editar, eliminar y buscar empleados
- **Registro Manual**: Agregar o modificar registros de asistencia manualmente
- **Reportes Detallados**:
  - 📈 Reporte mensual con vista de calendario
  - ⏰ Llegadas tardías (después de 9:30 AM)
  - 🏃 Salidas tempranas (antes de 5:30 PM)
  - ❌ Reporte de ausencias por fecha
- **Búsqueda y Filtrado**: Encontrar empleados rápidamente

## 🛠️ Tecnologías Utilizadas

- **Backend**: ASP.NET Core 8.0
- **Base de Datos**: SQL Server con Entity Framework Core 9.0.8
- **Frontend**: Razor Pages con Bootstrap 5
- **Autenticación**: Sistema de sesiones personalizado
- **ORM**: Entity Framework Core con Code First

## 📋 Requisitos del Sistema

- .NET 8.0 SDK
- SQL Server (LocalDB, Express o Full)
- Visual Studio 2022 o Visual Studio Code
- Windows, macOS o Linux

## 🔧 Instalación y Configuración

### 1. Clonar el Repositorio
```bash
git clone https://github.com/NyoWynn/sistema-asistencia.git
cd sistema-asistencia
```

### 2. Configurar la Base de Datos
1. Abre `appsettings.json` y modifica la cadena de conexión:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=SistemaAsistenciaDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```

2. Ejecuta las migraciones para crear la base de datos:
```bash
dotnet ef database update
```

### 3. Ejecutar la Aplicación
```bash
dotnet run
```

La aplicación estará disponible en `https://localhost:5001` o `http://localhost:5000`

## 👤 Configuración Inicial

### Crear Usuario Administrador
1. Ejecuta la aplicación
2. En la base de datos, inserta manualmente un usuario administrador:
```sql
INSERT INTO Users (Email, Password, Name, IsAdmin) 
VALUES ('admin@empresa.com', 'admin123', 'Administrador', 1);
```

### Credenciales por Defecto
- **Email**: admin@empresa.com
- **Contraseña**: admin123

## 📱 Uso del Sistema

### Para Empleados
1. **Iniciar Sesión**: Usa tu email y contraseña
2. **Marcar Asistencia**: 
   - Si es tu primera vez del día: botón "Marcar Entrada"
   - Si ya entraste: botón "Marcar Salida"
3. **Ver Estado**: El sistema te muestra tu estado actual

### Para Administradores
1. **Gestión de Usuarios**: 
   - Ve a la sección "Usuarios" para administrar empleados
   - Usa la búsqueda para encontrar empleados específicos
2. **Reportes**:
   - **Mensual**: Vista de calendario con estado de cada empleado
   - **Llegadas Tardías**: Lista de empleados que llegaron después de 9:30 AM
   - **Salidas Tempranas**: Lista de empleados que salieron antes de 5:30 PM
   - **Ausencias**: Empleados que no registraron asistencia en una fecha específica
3. **Registro Manual**: Agregar o modificar registros de asistencia para cualquier empleado

## 🗄️ Estructura de la Base de Datos

### Tabla Users
- `Id`: Identificador único
- `Email`: Correo electrónico (único)
- `Password`: Contraseña
- `Name`: Nombre completo
- `IsAdmin`: Indica si es administrador

### Tabla AttendanceRecords
- `Id`: Identificador único
- `UserId`: Referencia al usuario
- `Timestamp`: Fecha y hora del registro
- `RecordType`: "Entrada" o "Salida"

## 🔒 Seguridad

- **Autenticación**: Sistema de sesiones con timeout de 20 minutos
- **Validación**: Prevención de registros duplicados
- **Autorización**: Separación de roles (empleado/administrador)
- **Protección CSRF**: Tokens anti-falsificación en formularios

## 📊 Características de los Reportes

### Reporte Mensual
- Vista de calendario con todos los días del mes
- Códigos de color para diferentes estados:
  - 🟢 Verde: Asistencia normal
  - 🟡 Amarillo: Llegada tardía
  - 🔴 Rojo: Salida temprana
  - ⚫ Negro: Ausente
  - 🟠 Naranja: Registro incompleto

### Filtros de Tiempo
- **Llegadas Tardías**: Después de 9:30 AM
- **Salidas Tempranas**: Antes de 5:30 PM
- **Horario Estándar**: 9:00 AM - 6:00 PM

## 🚀 Despliegue

### Para Producción
1. Configura la cadena de conexión para tu servidor de producción
2. Ejecuta las migraciones en el servidor
3. Configura HTTPS y certificados SSL
4. Ajusta los logs según tus necesidades

### Docker (Opcional)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SistemaAsistencia.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaAsistencia.dll"]
```



## 📝 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.
