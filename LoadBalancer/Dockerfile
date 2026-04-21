# Multi-stage build for LoadBalancer
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the LoadBalancer project file and the Shared project reference so restore can find them
COPY LoadBalancer/LoadBalancer.csproj ./LoadBalancer/
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore "./LoadBalancer/LoadBalancer.csproj"

# Copy the rest of the repository and publish the LoadBalancer project
COPY . .
RUN dotnet publish "./LoadBalancer/LoadBalancer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS="http://+:80"
EXPOSE 80
ENTRYPOINT ["dotnet", "LoadBalancer.dll"]
