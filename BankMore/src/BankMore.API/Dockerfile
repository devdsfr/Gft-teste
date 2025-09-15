FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/BankMore.API/BankMore.API.csproj", "src/BankMore.API/"]
COPY ["src/BankMore.Domain/BankMore.Domain.csproj", "src/BankMore.Domain/"]
COPY ["src/BankMore.Infrastructure/BankMore.Infrastructure.csproj", "src/BankMore.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "src/BankMore.API/BankMore.API.csproj"

# Copiar código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/src/BankMore.API"
RUN dotnet build "BankMore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankMore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Criar diretório para dados
RUN mkdir -p /app/data

# Copiar scripts SQL
COPY scripts/ /app/scripts/

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.API.dll"]

