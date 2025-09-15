using BankMore.Tarifas;

var builder = Host.CreateApplicationBuilder(args);

// Configuração simplificada para demonstração
// Em produção, configurar adequadamente o KafkaFlow
var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
