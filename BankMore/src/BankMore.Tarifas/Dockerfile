FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/BankMore.Tarifas/BankMore.Tarifas.csproj", "src/BankMore.Tarifas/"]
COPY ["src/BankMore.Domain/BankMore.Domain.csproj", "src/BankMore.Domain/"]
COPY ["src/BankMore.Infrastructure/BankMore.Infrastructure.csproj", "src/BankMore.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "src/BankMore.Tarifas/BankMore.Tarifas.csproj"

# Copiar código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/src/BankMore.Tarifas"
RUN dotnet build "BankMore.Tarifas.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankMore.Tarifas.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Criar diretório para dados
RUN mkdir -p /app/data

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Tarifas.dll"]

