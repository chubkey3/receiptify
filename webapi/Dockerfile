# Stage 1: Build using .NET 9 preview SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "webapi.dll"]
