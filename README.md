
 ejecutar los siguientes scrips 

 CREATE TABLE Usuarios
(
    Id                     INT IDENTITY (1,1) PRIMARY KEY,
    Correo                 VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash           VARCHAR(MAX) NOT NULL,
    Rol                    VARCHAR(50),
    RefreshToken           VARCHAR(255),
    RefreshTokenExpiryTime DATETIME
);

CREATE TABLE CargaArchivo
(
    Id            INT IDENTITY (1,1) PRIMARY KEY,
    NombreArchivo VARCHAR(255) NOT NULL,
    Url           VARCHAR(500) NOT NULL,
    Estado        VARCHAR(10),
    FechaRegistro DATETIME
);

create table DetalleCarga
(
    Id             INT IDENTITY (1,1) PRIMARY KEY,
    CargaArchivoId INT FOREIGN KEY REFERENCES CargaArchivo (Id),
    Periodo        VARCHAR(6),
    CantidadFilas  INT
);

CREATE TABLE DataProcesada
(
    Id             INT IDENTITY (1,1) PRIMARY KEY,
    CargaArchivoId INT FOREIGN KEY REFERENCES CargaArchivo (Id),
    CodigoProducto VARCHAR(6),
    MensajeLog     varchar(250)
);


https://localhost:3000/api/auth/login
https://localhost:3000/api/excel/upload

[excel_data.xlsx](https://github.com/user-attachments/files/32131113/excel_data.xlsx)

