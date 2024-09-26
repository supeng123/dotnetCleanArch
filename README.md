## Dotnet command line

list all available commands

```
dotnet new list

dotnet new sln
```

generate class library

```
dotnet new classlib -o Domain
dotnet new classlib -o Core
dotnet new classlib -o Infrastructure
```

generate api project with controller

```
dotnet new webapi -o API -controllers
```

put API project into solution

```
dotnet sln add API
dotnet sln add Domain
dotnet sln add Core
dotnet sln add Infrastructure
```

add project references

```
cd API
dotnet add reference ../Infrastucture

cd Core
dotnet add reference ../Domain

cd Infrastucture
dotnet add reference ../Domain
dotnet add reference ../Core
```

verify the application

```
dotnet restore
dotnet build
```

run application

```
cd API
dotnet run
dotnet watch
```

migration

```
dotnet ef database drop -p Infrastructure -s API
dotnet ef migrations remove -p Infrastructure -s API

dotnet ef migrations add InitialCreate -p Infrastructure -s API -o Data/Migrations
dotnet ef database update -p Infrastructure -s API -o Data/Migrations
```

things need to be done

1. introduce fluent validation
2. add automapper
3. add mediatr
4. add identity
