# Banco Digital da Ana - BankMore

Uma solução completa de banco digital desenvolvida em C# .NET 8, implementando funcionalidades de cadastro, autenticação, movimentações, transferências e sistema de tarifas com mensageria Kafka.

## 🏗️ Arquitetura

A solução foi desenvolvida seguindo os princípios de **Clean Architecture** e **CQRS (Command Query Responsibility Segregation)**, com as seguintes camadas:

- **BankMore.API**: API principal com endpoints REST
- **BankMore.Tarifas**: Background service para processamento de tarifas
- **BankMore.Domain**: Camada de domínio com entidades, comandos e handlers
- **BankMore.Infrastructure**: Implementações de repositórios e serviços

## 🚀 Tecnologias Utilizadas

- **.NET 8**: Framework principal
- **Dapper**: ORM para acesso a dados
- **SQLite**: Banco de dados
- **JWT**: Autenticação e autorização
- **KafkaFlow**: Mensageria (configuração simplificada para demonstração)
- **MediatR**: Implementação do padrão Mediator
- **Swagger/OpenAPI**: Documentação da API
- **Docker & Docker Compose**: Containerização

## 📋 Funcionalidades

### API Principal (BankMore.API)
- **POST /contas/cadastrar**: Cadastro de nova conta corrente
- **POST /contas/login**: Autenticação e geração de token JWT
- **POST /contas/inativar**: Inativação de conta corrente
- **POST /movimentacoes**: Realização de movimentações (crédito/débito)
- **GET /contas/saldo**: Consulta de saldo
- **POST /transferencias**: Transferências entre contas

### Background Service (BankMore.Tarifas)
- Processamento de tarifas de transferências
- Consumo de mensagens Kafka
- Débito automático de tarifas

## 🔧 Configuração e Execução

### Pré-requisitos
- Docker e Docker Compose
- .NET 8 SDK (para desenvolvimento)

### Executando com Docker Compose

1. Clone o repositório:
```bash
git clone <repository-url>
cd BankMore
```

2. Execute com Docker Compose:
```bash
docker-compose -f docker/docker-compose.yml up --build
```

3. A API estará disponível em: `http://localhost:5000`
4. Documentação Swagger: `http://localhost:5000/swagger`

### Executando Localmente

1. Restaurar dependências:
```bash
dotnet restore
```

2. Compilar a solução:
```bash
dotnet build
```

3. Executar a API:
```bash
cd src/BankMore.API
dotnet run
```

4. Executar o serviço de tarifas (em outro terminal):
```bash
cd src/BankMore.Tarifas
dotnet run
```

## 📊 Banco de Dados

O sistema utiliza SQLite com as seguintes tabelas:

- **contacorrente**: Dados das contas correntes
- **movimento**: Movimentações financeiras
- **transferencia**: Registros de transferências
- **tarifa**: Tarifas aplicadas
- **idempotencia**: Controle de idempotência

O banco é inicializado automaticamente na primeira execução.

## 🔐 Autenticação

Todos os endpoints (exceto cadastro e login) requerem autenticação JWT. 

**Header de autorização:**
```
Authorization: Bearer <token>
```

## 📝 Exemplos de Uso

### 1. Cadastrar Conta
```http
POST /contas/cadastrar
Content-Type: application/json

{
  "cpf": "12345678901",
  "nome": "João Silva",
  "senha": "minhasenha123"
}
```

### 2. Fazer Login
```http
POST /contas/login
Content-Type: application/json

{
  "cpfOuNumeroConta": "12345678901",
  "senha": "minhasenha123"
}
```

### 3. Consultar Saldo
```http
GET /contas/saldo
Authorization: Bearer <token>
```

### 4. Realizar Movimentação
```http
POST /movimentacoes
Authorization: Bearer <token>
Content-Type: application/json

{
  "chaveIdempotencia": "unique-key-123",
  "valor": 100.00,
  "tipoMovimento": "C"
}
```

### 5. Fazer Transferência
```http
POST /transferencias
Authorization: Bearer <token>
Content-Type: application/json

{
  "chaveIdempotencia": "transfer-key-456",
  "numeroContaDestino": 2,
  "valor": 50.00
}
```

## 🛡️ Segurança

- Senhas são armazenadas com hash SHA256 + salt
- Validação de CPF implementada
- Tokens JWT com expiração configurável
- Controle de idempotência para operações críticas

## 📋 Regras de Negócio

- CPF deve ser válido (algoritmo de validação)
- Apenas contas ativas podem realizar operações
- Valores devem ser positivos
- Débitos só são permitidos com saldo suficiente
- Créditos em contas de terceiros são permitidos
- Transferências geram tarifas automáticas (R$ 2,00)

## 🔄 Mensageria

O sistema utiliza Kafka para:
- Notificação de transferências realizadas
- Processamento assíncrono de tarifas
- Débito automático de tarifas nas contas

**Nota**: A implementação atual usa uma versão simplificada para demonstração. Em produção, configurar adequadamente o KafkaFlow.

## 🧪 Testes

Para executar os testes:
```bash
dotnet test
```

## 📚 Documentação da API

A documentação completa da API está disponível via Swagger em:
`http://localhost:5000/swagger`

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 👥 Equipe

Desenvolvido para o Banco Digital da Ana - BankMore

---

**Nota**: Esta é uma implementação de demonstração. Para uso em produção, considere implementar funcionalidades adicionais como:
- Logs estruturados
- Monitoramento e métricas
- Testes de carga
- Configuração completa do Kafka
- Backup e recuperação de dados
- Auditoria de transações

