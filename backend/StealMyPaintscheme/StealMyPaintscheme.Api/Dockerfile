FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StealMyPaintscheme.Api/StealMyPaintscheme.Api.csproj", "StealMyPaintscheme.Api/"]
RUN dotnet restore "StealMyPaintscheme.Api/StealMyPaintscheme.Api.csproj"
COPY . .
WORKDIR "/src/StealMyPaintscheme.Api"
RUN dotnet build "./StealMyPaintscheme.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StealMyPaintscheme.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StealMyPaintscheme.Api.dll"]
