# ETAPA 1: Construcción (Build)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Definimos el argumento para recibir el token
ARG NUGET_KEY

# 1. Agregamos la fuente de GitHub con credenciales
# Usamos --store-password-in-clear-text porque Linux en Docker no soporta encriptación de credenciales por defecto
RUN dotnet nuget add source "https://nuget.pkg.github.com/SolucionesGlobalesSAS/index.json" \
    --name "GitHub" \
    --username "adrian-castro-sgsas" \
    --password "$NUGET_KEY" \
    --store-password-in-clear-text

# 2. Copiamos los archivos de proyecto (Capas)
COPY ["FrameAppWS/FrameAppWS.csproj", "FrameAppWS/"]
COPY ["Tyc.Interface/Tyc.Interface.csproj", "Tyc.Interface/"]
COPY ["Tyc.Modelo/Tyc.Modelo.csproj", "Tyc.Modelo/"]
COPY ["Tyc.Implementacion/Tyc.Implementacion.csproj", "Tyc.Implementacion/"]

# Copiamos archivos de configuración si existen
COPY Directory.Build.props .
COPY Directory.Packages.props .

# 3. Restauramos dependencias
# Nota: No necesitamos repetir las url con -s porque ya agregamos la de GitHub arriba.
# La de DevExpress la dejamos explícita porque tiene la clave en la URL.
RUN dotnet restore "FrameAppWS/FrameAppWS.csproj" \
    -s "https://api.nuget.org/v3/index.json" \
    -s "https://nuget.devexpress.com/BJhx7YFZYJxRgRyWzgVAnAgxOlEy8rqZxOJuegPYyAiPLjRcGp/api" \
    -s "https://nuget.pkg.github.com/SolucionesGlobalesSAS/index.json"

# 4. Copiamos el resto del código
COPY . .

# 5. Publicamos
WORKDIR "/src/FrameAppWS"
RUN dotnet publish -c Release -o /app/out

# ETAPA 2: Ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Configuración de Zona Horaria (Optimizada para la imagen base de Microsoft)
ENV TZ=America/Bogota
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "FrameAppWS.dll"]