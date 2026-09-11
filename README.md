# Guía de instalación y configuración de Atlantic APIs

## 1. Levantar los servicios

Ubicarse en la ruta donde se encuentran los archivos clonados del proyecto.

![Archivos clonados](https://github.com/user-attachments/assets/38496f25-9ba3-4d7f-97d0-bfd4df65facf)

Desde el terminal, ejecutar:

```bash
docker compose up --build -d
```

Una vez finalizado el proceso, todos los servicios estarán levantados.

![Servicios levantados](https://github.com/user-attachments/assets/97940d27-d9bb-479c-98cc-5fb15184f048)

---

## 2. Configurar la base de datos

Ingresar a la base de datos utilizando las siguientes credenciales:

| Parámetro    | Valor                |
| ------------ | -------------------- |
| **Server**   | `localhost,1433`     |
| **Usuario**  | `sa`                 |
| **Password** | `PasswordSeguro123!` |

Una vez conectado a la base de datos, ejecutar los siguientes scripts.

### 2.1. Tabla `Usuarios`

```sql
CREATE TABLE Usuarios
(
    Id                     INT IDENTITY (1,1) PRIMARY KEY,
    Correo                 VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash           VARCHAR(MAX) NOT NULL,
    Rol                    VARCHAR(50),
    RefreshToken           VARCHAR(255),
    RefreshTokenExpiryTime DATETIME
);
```

### 2.2. Tabla `CargaArchivo`

```sql
CREATE TABLE CargaArchivo
(
    Id            INT IDENTITY (1,1) PRIMARY KEY,
    NombreArchivo VARCHAR(255) NOT NULL,
    Url           VARCHAR(500) NOT NULL,
    Estado        VARCHAR(10),
    FechaRegistro DATETIME
);
```

### 2.3. Tabla `DetalleCarga`

```sql
CREATE TABLE DetalleCarga
(
    Id             INT IDENTITY (1,1) PRIMARY KEY,
    CargaArchivoId INT FOREIGN KEY REFERENCES CargaArchivo (Id),
    Periodo        VARCHAR(6),
    CantidadFilas  INT
);
```

### 2.4. Tabla `DataProcesada`

```sql
CREATE TABLE DataProcesada
(
    Id             INT IDENTITY (1,1) PRIMARY KEY,
    CargaArchivoId INT FOREIGN KEY REFERENCES CargaArchivo (Id),
    CodigoProducto VARCHAR(6),
    MensajeLog     VARCHAR(250)
);
```

---

## 3. Crear usuario

Una vez configurada la base de datos, crear un usuario mediante la API de registro.

### Endpoint

```http
POST http://localhost:3000/api/auth/register
```

### Body

Enviar el siguiente JSON:

```json
{
    "correo": "atlantic@gmail.com",
    "password": "1234",
    "Rol": "admin"
}
```

![Registro de usuario](https://github.com/user-attachments/assets/ffb67e1e-79d3-43c8-9082-04169a25102a)

---

## 4. Iniciar sesión y generar el token

Una vez creado el usuario, realizar el **login** utilizando las mismas credenciales.

El proceso de login generará el token necesario para consumir los endpoints protegidos de la API.

![Login y generación de token](https://github.com/user-attachments/assets/3ce911e7-04cc-4592-aec1-ddc308ca516b)

> **Importante:** utilizar el token generado en las solicitudes que requieran autenticación.

---

## 5. Carga y procesamiento del archivo Excel

Para realizar la carga y procesamiento de información, utilizar el archivo Excel proporcionado:

**Archivo:** `excel_data.xlsx`

[Descargar excel_data.xlsx](https://github.com/user-attachments/files/32131113/excel_data.xlsx)

Una vez obtenido el archivo, utilizar el endpoint correspondiente de la API para realizar la carga.

![Carga y procesamiento del Excel](https://github.com/user-attachments/assets/b43d7fe9-acea-4936-b202-eb7cd9ac8117)

---

## 6. Postman Collection

Para facilitar las pruebas de los endpoints, se proporciona una colección de Postman con las APIs configuradas.

**Archivo:** `AtlanticAPIs.postman_collection.json`

[Descargar AtlanticAPIs.postman_collection.json](https://github.com/user-attachments/files/32131811/AtlanticAPIs.postman_collection.json)

La colección permite realizar las diferentes operaciones disponibles en la API, incluyendo:

* Registro de usuarios.
* Login y generación de token.
* Autenticación mediante token.
* Carga del archivo Excel.
* Procesamiento de la información.
* Consulta de los resultados correspondientes.

---


* Docker.
* Docker Compose.
* SQL Server Management Studio (SSMS), Azure Data Studio u otro cliente compatible con SQL Server.
* Postman para ejecutar y validar los endpoints.
* Acceso al repositorio del proyecto.
