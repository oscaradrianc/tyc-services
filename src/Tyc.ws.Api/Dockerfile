FROM mcr.microsoft.com/dotnet/aspnet:9.0.6-alpine3.22 AS base

RUN apk add -U tzdata
ENV TZ=America/Bogota
RUN cp /usr/share/zoneinfo/America/Bogota /etc/localtime

USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0.301-alpine3.22 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Directory.Build.props ./
COPY ["src/Tyc.ws.Api/Tyc.ws.Api.csproj", "src/Tyc.ws.Api/"]
COPY ["src/Tyc.ws.Features/Tyc.ws.Features.csproj", "src/Tyc.ws.Features/"]

ARG NUGET_KEY

RUN dotnet nuget add source --username david-gomez-sgsas --password $NUGET_KEY --store-password-in-clear-text --name GitHub "https://nuget.pkg.github.com/SolucionesGlobalesSAS/index.json"

RUN dotnet restore "./src/Tyc.ws.Api/Tyc.ws.Api.csproj" -s https://nuget.pkg.github.com/SolucionesGlobalesSAS/index.json -s https://api.nuget.org/v3/index.json -s https://nuget.devexpress.com/euIojTxlxKNjWsBOwPNI3GaRWpCF3uPBYyHZkq6B6VKbjcKHcY/api
COPY . .
WORKDIR "/src/src/Tyc.ws.Api"
RUN dotnet build "./Tyc.ws.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tyc.ws.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tyc.ws.Api.dll"]