# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia só o csproj primeiro (melhora cache do restore)
COPY MottuApi.csproj ./
RUN dotnet restore MottuApi.csproj

# Copia o restante do código e publica
COPY . .
RUN dotnet publish MottuApi.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MottuApi.dll"]
