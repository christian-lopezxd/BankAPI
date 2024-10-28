# API para Banco

Este proyecto es una API RESTful para gestionar tarjetas de crédito y transacciones. Utiliza ASP.NET Core y Entity Framework Core.

## Requisitos Previos

Antes de comenzar, asegúrate de tener instalado lo siguiente en tu máquina:

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup) 

## Configuración del Proyecto

1. **Clonar el Repositorio**

   Clonar el repositorio desde GitHub a tu máquina local usando el siguiente comando:

   ```bash
   git clone https://github.com/christian-lopezxd/BankAPI.git
   cd BankAPI


   
## Configuracion de la cadena de conexion
Para este paso es recomendado utilizar [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/es/vs/)
importar el proyecto a Visual Studio mediante el archivo BankAPI.sln 

Una vez abierto el proyecto deberemos de configurar el archivo appsettings.json para que nos permita hacer la conexión entre la base de datos y nuestra API

Tenemos que cambiar **YourServerName** por el nombre del servidor que tenemos en SQL Server Management Studio.
Tambien **userName** por un nombre de usuario con permisos para crear y administrar contraseñas en SQL Server.
Finalmente cambiar **userPassword** por la respectiva contraseña del usuario que añadimos



  ```code
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YourServerName;Database=BancoAtlantidaDb;user=userName;password=userPassword;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

```

## Restaurar Paquetes NuGet
*En la terminal segurarse de estar en la carpeta \Backend\BankAPI>*
Dentro del proyecto abrir una terminal y ejecutar el siguiente comando:
```bash
dotnet restore
```
*Este comando restaurará toda la paqueteria NuGet necesaria para que el proyecto funcione*



## Crear la base de datos y migrar los modelos

Primero deberemos de crear una migración, la cual permite cambiar el modelo de datos e implementar los cambios en producción mediante la actualización del esquema de la base de datos sin tener que quitar y volver a crear la base de datos

**NombreDeLaMigracion** puede ser cualquier nombre

```bash
dotnet ef migrations add NombreDeLaMigracion
```
y hacemos el respectivo update para que lleguen los datos de la migracion a nuestra base de datos

```bash
dotnet ef database update
```
*Este proceso se encarga de crear automaticamente la base de datos siempre y cuando la conexión sea correcta*

## Levantar el servicio
Luego solo faltará levantar el proyecto para que este funcione

```bash
dotnet run
```

Luego de esto el servicio estara corriendo en el [localhost:5080](http://localhost:5080)

## Consultar la información desde Swagger

Para consultar la documentación de la API puedes acceder a http://localhost:5080/swagger/index.html o probar desde Postman con el archivo adjunto.




