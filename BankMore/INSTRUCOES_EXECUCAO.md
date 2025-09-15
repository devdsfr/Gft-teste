# Instruções de Execução - BankMore

## 🚀 Passo a Passo para Executar no JetBrains Rider

### 1. Pré-requisitos
- JetBrains Rider 2023.3 ou superior
- .NET 8 SDK instalado
- Docker Desktop (opcional, para execução completa)

### 2. Abrindo o Projeto

1. Abra o JetBrains Rider
2. Clique em "Open" ou "File > Open"
3. Navegue até a pasta `BankMore` e selecione o arquivo `BankMore.sln`
4. Aguarde o Rider carregar a solução e restaurar os pacotes NuGet

### 3. Configuração Inicial

#### 3.1 Verificar Configurações de Build
1. No Solution Explorer, clique com o botão direito na solução
2. Selecione "Build Solution" ou pressione `Ctrl+Shift+B`
3. Verifique se não há erros de compilação

#### 3.2 Configurar Múltiplos Projetos de Startup
1. Clique com o botão direito na solução no Solution Explorer
2. Selecione "Properties" ou "Set Startup Projects"
3. Escolha "Multiple startup projects"
4. Configure:
   - **BankMore.API**: Action = Start
   - **BankMore.Tarifas**: Action = Start
   - Outros projetos: Action = None

### 4. Executando a Aplicação

#### 4.1 Execução via Rider (Recomendado)
1. Pressione `F5` ou clique no botão "Run" (▶️)
2. O Rider iniciará ambos os projetos simultaneamente
3. A API estará disponível em: `https://localhost:7000` ou `http://localhost:5000`
4. O Swagger estará em: `https://localhost:7000/swagger`

#### 4.2 Execução Manual (Terminal)
```bash
# Terminal 1 - API
cd src/BankMore.API
dotnet run

# Terminal 2 - Serviço de Tarifas
cd src/BankMore.Tarifas
dotnet run
```

### 5. Testando a API

#### 5.1 Via Swagger UI
1. Acesse `https://localhost:7000/swagger`
2. Use a interface gráfica para testar os endpoints
3. Para endpoints autenticados:
   - Primeiro faça login em `/contas/login`
   - Copie o token retornado
   - Clique em "Authorize" no Swagger
   - Cole o token no formato: `Bearer <seu-token>`

#### 5.2 Via Postman
1. Importe a coleção `BankMore_Postman_Collection.json`
2. Configure a variável `base_url` para `https://localhost:7000`
3. Execute os requests na ordem:
   - Cadastrar Conta
   - Login (o token será salvo automaticamente)
   - Demais operações

### 6. Fluxo de Teste Completo

#### 6.1 Cadastrar Primeira Conta
```http
POST /contas/cadastrar
{
  "cpf": "12345678901",
  "nome": "João Silva",
  "senha": "senha123"
}
```

#### 6.2 Cadastrar Segunda Conta
```http
POST /contas/cadastrar
{
  "cpf": "98765432100",
  "nome": "Maria Santos",
  "senha": "senha456"
}
```

#### 6.3 Fazer Login (João)
```http
POST /contas/login
{
  "cpfOuNumeroConta": "12345678901",
  "senha": "senha123"
}
```

#### 6.4 Adicionar Saldo (Crédito)
```http
POST /movimentacoes
Authorization: Bearer <token-joao>
{
  "chaveIdempotencia": "credito-inicial-001",
  "valor": 1000.00,
  "tipoMovimento": "C"
}
```

#### 6.5 Consultar Saldo
```http
GET /contas/saldo
Authorization: Bearer <token-joao>
```

#### 6.6 Fazer Transferência
```http
POST /transferencias
Authorization: Bearer <token-joao>
{
  "chaveIdempotencia": "transfer-001",
  "numeroContaDestino": 2,
  "valor": 100.00
}
```

### 7. Estrutura de Pastas no Rider

```
BankMore/
├── src/
│   ├── BankMore.API/          # API Principal
│   ├── BankMore.Domain/       # Domínio
│   ├── BankMore.Infrastructure/ # Infraestrutura
│   └── BankMore.Tarifas/      # Serviço de Tarifas
├── scripts/                   # Scripts SQL
├── docker/                    # Configurações Docker
└── BankMore.sln              # Arquivo de Solução
```

### 8. Debugging

#### 8.1 Configurar Breakpoints
1. Clique na margem esquerda do editor para adicionar breakpoints
2. Execute em modo debug (`F5`)
3. Use as ferramentas de debug do Rider para inspecionar variáveis

#### 8.2 Logs da Aplicação
- Os logs aparecem na janela "Run" do Rider
- Para logs mais detalhados, configure o nível em `appsettings.json`

### 9. Banco de Dados

#### 9.1 Localização
- O arquivo SQLite será criado automaticamente em:
  - Desenvolvimento: `src/BankMore.API/bankmore.db`
  - Docker: `/app/data/bankmore.db`

#### 9.2 Visualizar Dados
1. No Rider, vá em "View > Tool Windows > Database"
2. Adicione uma nova conexão SQLite
3. Aponte para o arquivo `bankmore.db`
4. Explore as tabelas e dados

### 10. Troubleshooting

#### 10.1 Erro de Porta em Uso
```bash
# Verificar processos usando a porta
netstat -ano | findstr :5000
# Matar processo se necessário
taskkill /PID <process-id> /F
```

#### 10.2 Erro de Certificado HTTPS
- No desenvolvimento, aceite o certificado auto-assinado
- Ou configure para usar apenas HTTP alterando `launchSettings.json`

#### 10.3 Problemas de Dependências
```bash
# Limpar e restaurar
dotnet clean
dotnet restore
dotnet build
```

### 11. Configurações Avançadas

#### 11.1 Variáveis de Ambiente
Crie um arquivo `.env` na raiz do projeto:
```
ASPNETCORE_ENVIRONMENT=Development
JWT_SECRET_KEY=BankMore_SuperSecretKey_2024_MinimumLength32Characters!
CONNECTION_STRING=Data Source=bankmore.db
```

#### 11.2 Configuração de Logging
No `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "BankMore": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 12. Próximos Passos

Após executar com sucesso:
1. Explore a documentação Swagger
2. Teste todos os cenários de uso
3. Verifique os logs de ambos os serviços
4. Experimente cenários de erro (saldo insuficiente, conta inativa, etc.)
5. Teste a idempotência enviando a mesma requisição duas vezes

---

**Dica**: Mantenha ambos os projetos (API e Tarifas) executando simultaneamente para testar o fluxo completo de transferências e tarifas.

