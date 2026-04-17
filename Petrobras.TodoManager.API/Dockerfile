FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY . .
RUN dotnet restore Petrobras.TodoManager.sln
RUN dotnet test Petrobras.TodoManager.Tests/Petrobras.TodoManager.Tests.csproj --configuration Release --no-restore
RUN dotnet publish Petrobras.TodoManager.API/Petrobras.TodoManager.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Petrobras.TodoManager.API.dll"]