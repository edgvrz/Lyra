mitienda@tienda.com
superadmin@admin.com
## Clonar repositorio  1
git clone URL 
## Descargar
.NET 9 SDK verificar dotnet --version
## en vs code 
C#
C# Dev Kit
ASP.NET Core Razor
## DESCARGAR(ACTUALIZAR .NET) 2
dotnet restore 
dotnet ef database update (SI NO COMPARTO DB)
dotnet-ef VERIFICAR SI ESTA INSTALADO
## si se quiere compartir base de datos
descargar app.db preferible no

## Características
- Registro e inicio de sesión
- Análisis corporal inteligente
- Detección automática del tipo de cuerpo
- Recomendación de prendas personalizadas
- Sistema de favoritos
- Gestión de tiendas afiliadas
- Panel administrativo
- Clasificación por colorimetría
- Recomendaciones según tono de piel



## Roles del sistema

### Cliente
- Completa perfil corporal
- Recibe recomendaciones
- Guarda favoritos

### Administrador
- Gestiona prendas
- Gestiona tiendas
- Aprueba tiendas afiliadas

### Tienda
- Publica prendas
- Administra catálogo

## Cómo ejecutar el proyecto

### 1. Clonar repositorio

```bash
git clone URL_DEL_REPOSITORIO

### 2. Entrar al proyecto

```bash
cd Lyra
```

### 3. Restaurar dependencias

```bash
dotnet restore
```

### 4. Ejecutar aplicación

```bash
dotnet run
```

---

## 🗄️ Base de datos

El proyecto utiliza SQLite local.

Si no existe la base de datos:

```bash
dotnet ef database update
```

---

## 💡 Propuesta de valor

Lyra busca mejorar la autoestima, reducir compras innecesarias y promover la moda sostenible mediante recomendaciones inteligentes y personalizadas.

---

## 📌 Estado del proyecto

Proyecto académico / prototipo funcional MVP.
