# Utilizar la imagen oficial del SDK de .NET 10 como base para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copiar el archivo csproj y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código y construir
COPY . ./
RUN dotnet publish -c Release -o out

# Utilizar la imagen de runtime de ASP.NET para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# Configuración para Entorno de Producción
ENV ASPNETCORE_ENVIRONMENT=Production
ENV TZ=America/Lima

# Exponer el puerto
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# (Opcional) Ejecutar contenedor como un usuario no root para mayor seguridad
# RUN chown -R $APP_UID:$APP_UID /app
# USER $APP_UID

ENTRYPOINT ["dotnet", "FinancieraBackend.dll"]
