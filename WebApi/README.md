## Create a appsettings.Local.json file in the WebApi folder and copy the following content inside of it:

```json
{
    "ConnectionStrings": {
        "SqlServerConnection": "Data Source=JSGHJ72;User ID=sa;Password=KwJNolfvex8m;Database=WebApiDB;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
    }
}
```


Then change the connection setting based on your environment DB settings. 

## Generate DB initializer from models:

1. Open Power Shell 
2. Install the entity framework tool:
```
    dotnet tool install --global dotnet-ef
```
3. Or if it is needed to update the entity framework tool then:
```
    dotnet tool update --global dotnet-ef
```
4. Move to your project folder:
```
CD C:\Users\Username\Documents\GitHub\BoilerplateAuthenticationAPI\WebApi
```
5. Maybe you need to remove the last migrations, go to `Migration/Auth` folder and delete the files
6. Generate the migration files for MSSQL:
```
dotnet ef migrations add InitialCreate --context AccountDbContext --output-dir Migrations/Auth
```

## Migrate DB:

```
dotnet ef database update --context AccountDbContext
```

## Revert the last migration:

```
dotnet ef database update 0 --context AccountDbContext
```

## Remove the migration file:

```
dotnet ef migrations remove --context AccountDbContext
```

## Make model from DB

1. If you have MySQL:
```
dotnet ef dbcontext scaffold "server=localhost;port=3306;userid=root;password=;database=WebApiDB;TreatTinyAsBoolean=true;" Pomelo.EntityFrameworkCore.MySql -o Models/Auth -c "AccountDbContext"
```
2. If you have MSSQL:

``` 
dotnet ef dbcontext scaffold "Data Source=JSGHJ72;User ID=sa;Password=KwJNolfvex8m;Database=WebApiDB;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" Microsoft.EntityFrameworkCore.SqlServer -o Entities/Exported -c "DbContext"
```