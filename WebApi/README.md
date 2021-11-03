# Create a appsettings.Local.json file in the WebApi folder and copy the following content inside of it:

```json
{
    "ConnectionStrings": {
        "SqlServerConnection": "Data Source=JSGHJ72;User ID=sa;Password=KwJNolfvex8m;Database=WebApiDB;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
    }
}
```


Then change the connection setting based on your environment DB settings. 

# Generate DB initializer from models:

1. Open Power Shell 
2.1. Install the entity framework tool:
    dotnet tool install --global dotnet-ef
2.2. Or if it is needed to update the entity framework tool then:
    dotnet tool update --global dotnet-ef
3.1. CD C:\Users\Username\Documents\GitHub\BoilerplateAuthenticationAPI\WebApi
3.2. Maybe you need to remove the last migrations, go to Migration folder and delete the files
4. dotnet ef migrations add InitialCreate --context AccountDbContext --output-dir Migrations/Auth


# Migrate DB:

1. dotnet ef database update --context AccountDbContext

# Revert the last migration:

1. dotnet ef database update 0 --context AccountDbContext

# Remove the migration file:

1. dotnet ef migrations remove --context AccountDbContext

# Make model from DB
dotnet ef dbcontext scaffold "server=localhost;port=3306;userid=root;password=;database=WebApidb;TreatTinyAsBoolean=true;" Pomelo.EntityFrameworkCore.MySql -o Models/Auth -c "AccountDbContext"
dotnet ef dbcontext scaffold "Data Source=JSGHJ72;User ID=sa;Password=KwJNolfvex8m;Database=WebApiDB;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" Microsoft.EntityFrameworkCore.SqlServer -o Entities/Exported -c "DbContext"
dotnet ef dbcontext scaffold "Data Source=DESKTOP-I2G04UF;User ID=sa;Password=KwJNolfvex8m;Database=WebApiDB;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" Microsoft.EntityFrameworkCore.SqlServer -o Entities/Exported -c "DbContext"
