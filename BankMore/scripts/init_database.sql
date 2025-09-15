-- Script de inicialização do banco de dados SQLite para o Banco Digital da Ana

-- Tabela de conta corrente
CREATE TABLE IF NOT EXISTS contacorrente (
    idcontacorrente TEXT(37) PRIMARY KEY, -- id da conta corrente
    numero INTEGER(10) NOT NULL UNIQUE, -- numero da conta corrente
    cpf TEXT(11) NOT NULL UNIQUE, -- CPF do titular da conta corrente
    nome TEXT(100) NOT NULL, -- nome do titular da conta corrente
    ativo INTEGER(1) NOT NULL DEFAULT 1, -- indicativo se a conta esta ativa. (0 = inativa, 1 = ativa).
    senha TEXT(100) NOT NULL,
    salt TEXT(100) NOT NULL,
    datacriacao TEXT(25) NOT NULL, -- data de criação da conta no formato DD/MM/YYYY HH:MM:SS
    CHECK (ativo IN (0,1))
);

-- Tabela de movimento
CREATE TABLE IF NOT EXISTS movimento (
    idmovimento TEXT(37) PRIMARY KEY, -- identificacao unica do movimento
    idcontacorrente TEXT(37) NOT NULL, -- identificacao unica da conta corrente
    datamovimento TEXT(25) NOT NULL, -- data do movimento no formato DD/MM/YYYY HH:MM:SS
    tipomovimento TEXT(1) NOT NULL, -- tipo do movimento. (C = Credito, D = Debito).
    valor REAL NOT NULL, -- valor do movimento. Usar duas casas decimais.
    CHECK (tipomovimento IN ('C','D')),
    FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

-- Tabela de idempotência
CREATE TABLE IF NOT EXISTS idempotencia (
    chave_idempotencia TEXT(37) PRIMARY KEY, -- identificacao chave de idempotencia
    requisicao TEXT(1000), -- dados de requisicao
    resultado TEXT(1000) -- dados de retorno
);

-- Tabela de transferência
CREATE TABLE IF NOT EXISTS transferencia (
    idtransferencia TEXT(37) PRIMARY KEY, -- identificacao unica da transferencia
    idcontacorrente_origem TEXT(37) NOT NULL, -- identificacao unica da conta corrente de origem
    idcontacorrente_destino TEXT(37) NOT NULL, -- identificacao unica da conta corrente de destino
    datamovimento TEXT(25) NOT NULL, -- data da transferencia no formato DD/MM/YYYY HH:MM:SS
    valor REAL NOT NULL, -- valor da transferencia. Usar duas casas decimais.
    FOREIGN KEY(idcontacorrente_origem) REFERENCES contacorrente(idcontacorrente),
    FOREIGN KEY(idcontacorrente_destino) REFERENCES contacorrente(idcontacorrente)
);

-- Tabela de tarifa
CREATE TABLE IF NOT EXISTS tarifa (
    idtarifa TEXT(37) PRIMARY KEY, -- identificacao unica da tarifa
    idcontacorrente TEXT(37) NOT NULL, -- identificacao unica da conta corrente
    datamovimento TEXT(25) NOT NULL, -- data da tarifa no formato DD/MM/YYYY HH:MM:SS
    valor REAL NOT NULL, -- valor da tarifa. Usar duas casas decimais.
    FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

-- Índices para melhorar performance
CREATE INDEX IF NOT EXISTS idx_contacorrente_numero ON contacorrente(numero);
CREATE INDEX IF NOT EXISTS idx_contacorrente_cpf ON contacorrente(cpf);
CREATE INDEX IF NOT EXISTS idx_movimento_conta ON movimento(idcontacorrente);
CREATE INDEX IF NOT EXISTS idx_transferencia_origem ON transferencia(idcontacorrente_origem);
CREATE INDEX IF NOT EXISTS idx_transferencia_destino ON transferencia(idcontacorrente_destino);
CREATE INDEX IF NOT EXISTS idx_tarifa_conta ON tarifa(idcontacorrente);

