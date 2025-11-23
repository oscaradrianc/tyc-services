#FROM mcr.microsoft.com/dotnet/sdk:8.0.100-alpine3.18 AS build
FROM mcr.microsoft.com/dotnet/sdk:8.0.203-jammy AS build
WORKDIR /app

ARG NUGET_KEY

RUN dotnet nuget add source --username david-gomez-sgsas --password $NUGET_KEY --store-password-in-clear-text --name GitHub "https://nuget.pkg.github.com/SolucionesGlobalesSAS/index.json"

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY SIGO.Modelo/*.csproj ./SIGO.Modelo/
COPY SIGO.Implementacion/*.csproj ./SIGO.Implementacion/
COPY SIGO.Interface/*.csproj ./SIGO.Interface/
COPY FrameAppWS/*.csproj ./FrameAppWS/
RUN dotnet restore -s https://nuget.pkg.github.com/SolucionesGlobalesSAS/index.json -s https://api.nuget.org/v3/index.json -s https://nuget.devexpress.com/euIojTxlxKNjWsBOwPNI3GaRWpCF3uPBYyHZkq6B6VKbjcKHcY/api

# copy everything else and build app
COPY SIGO.Modelo/. ./SIGO.Modelo/
COPY SIGO.Implementacion/. ./SIGO.Implementacion/
COPY SIGO.Interface/. ./SIGO.Interface/

COPY FrameAppWS/. ./FrameAppWS/

WORKDIR /app/FrameAppWS

RUN dotnet publish -c Release -o out

#FROM mcr.microsoft.com/dotnet/aspnet:8.0.0-alpine3.18 AS runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-jammy AS runtime

#RUN apk add -U tzdata
ENV TZ=America/Bogota
#RUN cp /usr/share/zoneinfo/America/Bogota /etc/localtime
RUN ln -sf /usr/share/zoneinfo/America/Bogota /etc/localtime
 

COPY --from=build /app/FrameAppWS/out /app

WORKDIR /app

HEALTHCHECK CMD curl --fail http://localhost || exit 1

ENTRYPOINT ["dotnet", "FrameAppWS.dll"]