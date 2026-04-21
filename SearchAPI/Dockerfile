# Multi-stage build for SearchAPI
# Uses .NET SDK to build then runtime image to run
# Adjust SDK/runtime tags if you target a different .NET version

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project file first (relative to the build context) and restore dependencies.
# The build context is the repository root, so the project file is at SearchAPI/SearchAPI.csproj.
COPY SearchAPI/SearchAPI.csproj ./SearchAPI/
RUN dotnet restore "./SearchAPI/SearchAPI.csproj"

# Copy the rest of the repo and publish the SearchAPI project.
COPY . .
RUN dotnet publish "./SearchAPI/SearchAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS="http://+:8080"
EXPOSE 8080
ENTRYPOINT ["dotnet", "SearchAPI.dll"]
