FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Atlantic.Worker/Atlantic.Worker.csproj", "Atlantic.Worker/"]
RUN dotnet restore "Atlantic.Worker/Atlantic.Worker.csproj"
COPY . .
WORKDIR "/src/Atlantic.Worker"
RUN dotnet publish "Atlantic.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Atlantic.Worker.dll"]