FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Atlantic.Gateway/Atlantic.Gateway.csproj", "Atlantic.Gateway/"]
RUN dotnet restore "Atlantic.Gateway/Atlantic.Gateway.csproj"
COPY . .
WORKDIR "/src/Atlantic.Gateway"
RUN dotnet publish "Atlantic.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Atlantic.Gateway.dll"]