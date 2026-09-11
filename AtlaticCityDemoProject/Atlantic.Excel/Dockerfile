FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Atlantic.Excel/Atlantic.Excel.csproj", "Atlantic.Excel/"]
RUN dotnet restore "Atlantic.Excel/Atlantic.Excel.csproj"
COPY . .
WORKDIR "/src/Atlantic.Excel"
RUN dotnet publish "Atlantic.Excel.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Atlantic.Excel.dll"]