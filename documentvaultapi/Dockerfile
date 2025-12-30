# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["documentvaultapi.csproj", "./"]
RUN dotnet restore "documentvaultapi.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "documentvaultapi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "documentvaultapi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "documentvaultapi.dll"]
