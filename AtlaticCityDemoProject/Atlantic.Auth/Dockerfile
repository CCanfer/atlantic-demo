FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Atlantic.Auth/Atlantic.Auth.csproj", "Atlantic.Auth/"]
RUN dotnet restore "Atlantic.Auth/Atlantic.Auth.csproj"
COPY . .
WORKDIR "/src/Atlantic.Auth"
RUN dotnet publish "Atlantic.Auth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Atlantic.Auth.dll"]