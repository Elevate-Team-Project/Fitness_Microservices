# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ✅ FIX: Include folder name in path
COPY ["ProgressTrackingService/ProgressTrackingService.csproj", "ProgressTrackingService/"]

# ✅ FIX: Restore full path
RUN dotnet restore "ProgressTrackingService/ProgressTrackingService.csproj"

COPY . .

# ✅ FIX: Change directory to project folder
WORKDIR "/src/ProgressTrackingService"

RUN dotnet build "ProgressTrackingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProgressTrackingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProgressTrackingService.dll"]