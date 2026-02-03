# Use the official .NET SDK image for building and testing
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY poc/RuneEngraver.PoC/RuneEngraver.PoC.csproj poc/RuneEngraver.PoC/
COPY poc/Tests/Tests.csproj poc/Tests/
RUN dotnet restore poc/RuneEngraver.PoC/RuneEngraver.PoC.csproj
RUN dotnet restore poc/Tests/Tests.csproj

# Copy the remaining source code
COPY poc/ poc/

# Build the application
RUN dotnet build poc/RuneEngraver.PoC/RuneEngraver.PoC.csproj -c Release -o /app/build

# Run tests
RUN dotnet test poc/Tests/Tests.csproj -c Release --logger:trx

# Publish the application
FROM build AS publish
RUN dotnet publish poc/RuneEngraver.PoC/RuneEngraver.PoC.csproj -c Release -o /app/publish

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RuneEngraver.PoC.dll"]
