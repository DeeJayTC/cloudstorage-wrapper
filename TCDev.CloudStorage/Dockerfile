FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY TCDev.CloudStorage.csproj TCDev.CloudStorage/TCDev.CloudStorage.csproj
RUN dotnet restore /src/TCDev.CloudStorage/TCDev.CloudStorage.csproj
WORKDIR /src/TCDev.CloudStorage
COPY . .
RUN dotnet build TCDev.CloudStorage.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TCDev.CloudStorage.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TCDev.CloudStorage.dll"]