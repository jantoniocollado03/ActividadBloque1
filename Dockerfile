# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar y restaurar el proyecto principal
COPY ["Expendedora/Expendedora.csproj", "Expendedora/"]
RUN dotnet restore "Expendedora/Expendedora.csproj"

# Copiar el resto del código y publicar
COPY . .
WORKDIR /src/Expendedora
RUN dotnet publish "Expendedora.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Expendedora.dll"]
