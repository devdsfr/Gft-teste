using Microsoft.Data.Sqlite;
using System.Reflection;

namespace BankMore.Infrastructure.Database;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        // Criar o diretório se não existir
        var connectionStringBuilder = new SqliteConnectionStringBuilder(_connectionString);
        var databasePath = connectionStringBuilder.DataSource;
        var directory = Path.GetDirectoryName(databasePath);
        
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Executar o script de inicialização
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var scriptPath = Path.Combine(GetScriptsDirectory(), "init_database.sql");
        if (File.Exists(scriptPath))
        {
            var script = await File.ReadAllTextAsync(scriptPath);
            var commands = script.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var commandText in commands)
            {
                if (!string.IsNullOrWhiteSpace(commandText))
                {
                    using var command = new SqliteCommand(commandText.Trim(), connection);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }

    private static string GetScriptsDirectory()
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        
        // Navegar até a raiz do projeto para encontrar a pasta scripts
        var currentDirectory = assemblyDirectory;
        while (currentDirectory != null && !Directory.Exists(Path.Combine(currentDirectory, "scripts")))
        {
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        if (currentDirectory != null)
        {
            return Path.Combine(currentDirectory, "scripts");
        }

        // Fallback: usar o diretório atual
        return Path.Combine(Directory.GetCurrentDirectory(), "scripts");
    }
}

