FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Atlantic.Notification/Atlantic.Notification.csproj", "Atlantic.Notification/"]
RUN dotnet restore "Atlantic.Notification/Atlantic.Notification.csproj"
COPY . .
WORKDIR "/src/Atlantic.Notification"
RUN dotnet publish "Atlantic.Notification.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Atlantic.Notification.dll"]