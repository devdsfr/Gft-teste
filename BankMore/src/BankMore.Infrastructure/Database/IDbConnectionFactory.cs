using System.Data;

namespace BankMore.Infrastructure.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

