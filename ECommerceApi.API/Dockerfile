# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ECommerceApi.API/ECommerceApi.API.csproj", "ECommerceApi.API/"]
COPY ["ECommerceApi.Application/ECommerceApi.Application.csproj", "ECommerceApi.Application/"]
COPY ["ECommerceApi.Domain/ECommerceApi.Domain.csproj", "ECommerceApi.Domain/"]
COPY ["ECommerceApi.Infrastructure/ECommerceApi.Infrastructure.csproj", "ECommerceApi.Infrastructure/"]
RUN dotnet restore "./ECommerceApi.API/ECommerceApi.API.csproj"
COPY . .
WORKDIR "/src/ECommerceApi.API"
RUN dotnet build "./ECommerceApi.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ECommerceApi.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerceApi.API.dll"]