using CustodialWallet.Infostructure.DbContext;
using CustodialWallet.Infostructure.Interface;
using Dapper;

namespace CustodialWallet.Infostructure.Repository
{
    public class InitRepository(DapperContext dapperContext) : IInitRepository
    {
        private readonly DapperContext _dapperContext = dapperContext;

        public async Task InitDatabaseAsync()
        {
            using var connection = _dapperContext.CreateConnection();

            var sqlUsers = @"CREATE TABLE IF NOT EXISTS Users (
                    Id UUID PRIMARY KEY,
                    Email VARCHAR(255) NOT NULL UNIQUE
                    );";

            var sqlCurrencies = @"CREATE TABLE IF NOT EXISTS Currencies (
                    Id UUID PRIMARY KEY,
                    ShortName VARCHAR(10) NOT NULL UNIQUE,
                    FullName VARCHAR(255) NOT NULL
                    );";

            var sqlBalances = @"CREATE TABLE IF NOT EXISTS Balances (
                    Id UUID PRIMARY KEY,
                    Amount MONEY NOT NULL,
                    CurrencyId UUID NOT NULL,
                    UserId UUID NOT NULL,
                    CONSTRAINT fk_currency
                        FOREIGN KEY (CurrencyId) 
                        REFERENCES Currencies(Id),
                    CONSTRAINT fk_user
                        FOREIGN KEY (UserId) 
                        REFERENCES Users(Id),
                    CONSTRAINT unique_user_currency
                        UNIQUE (UserId, CurrencyId)
                    );";

            var sqlLogs = @"CREATE TABLE IF NOT EXISTS ErrorLogs (
                    Id UUID PRIMARY KEY,
                    Message TEXT NOT NULL,
                    Source TEXT,
                    StackTrace TEXT,
                    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL
                    );";

             var triggerUserCreateGuid = @"CREATE OR REPLACE FUNCTION generate_user_id()
                    RETURNS TRIGGER AS $$
                    BEGIN
                        NEW.Id := gen_random_uuid();
                        RETURN NEW;
                    END;
                    $$ LANGUAGE plpgsql;

                    DROP TRIGGER IF EXISTS trg_generate_user_id ON Users;

                    CREATE TRIGGER trg_generate_user_id
                    BEFORE INSERT ON Users
                    FOR EACH ROW
                    EXECUTE FUNCTION generate_user_id();";

            var triggerUserCreateBalances = @"CREATE OR REPLACE FUNCTION create_default_balances()
                    RETURNS TRIGGER AS $$
                    BEGIN
                        INSERT INTO Balances (Id, Amount, CurrencyId, UserId)
                        SELECT
                            gen_random_uuid(),
                            0.00,
                            Currencies.Id,
                            NEW.Id
                        FROM Currencies;

                        RETURN NEW;
                    END;
                    $$ LANGUAGE plpgsql;

                    DROP TRIGGER IF EXISTS trg_create_default_balances ON Users;

                    CREATE TRIGGER trg_create_default_balances
                    AFTER INSERT ON Users
                    FOR EACH ROW
                    EXECUTE FUNCTION create_default_balances();";

            var triggerLogCreate = @"CREATE OR REPLACE FUNCTION generate_error_log_id_and_timestamp()
                    RETURNS TRIGGER AS $$
                    BEGIN
                        NEW.Id := gen_random_uuid();
    
                        NEW.CreatedAt := NOW();
    
                        RETURN NEW;
                    END;
                    $$ LANGUAGE plpgsql;

                    DROP TRIGGER IF EXISTS trg_generate_error_log_id_and_timestamp ON ErrorLogs;

                    CREATE TRIGGER trg_generate_error_log_id_and_timestamp
                    BEFORE INSERT ON ErrorLogs
                    FOR EACH ROW
                    EXECUTE FUNCTION generate_error_log_id_and_timestamp();";

            var testData = @"
                    INSERT INTO Currencies (Id, ShortName, FullName)
                    SELECT gen_random_uuid(), 'BTC', 'Bitcoin'
                    WHERE NOT EXISTS (SELECT 1 FROM Currencies WHERE ShortName = 'BTC');

                    INSERT INTO Currencies (Id, ShortName, FullName)
                    SELECT gen_random_uuid(), 'SOL', 'Solana'
                    WHERE NOT EXISTS (SELECT 1 FROM Currencies WHERE ShortName = 'SOL');

                    INSERT INTO Currencies (Id, ShortName, FullName)
                    SELECT gen_random_uuid(), 'USDT', 'United States Dollar Tether'
                    WHERE NOT EXISTS (SELECT 1 FROM Currencies WHERE ShortName = 'USDT');

                    INSERT INTO Users (Id, Email)
                    SELECT gen_random_uuid(), 'testuser@example.com'
                    WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'testuser@example.com');

                    INSERT INTO Balances (Id, Amount, CurrencyId, UserId)
                    SELECT 
                        gen_random_uuid(), 
                        CASE 
                            WHEN c.ShortName = 'BTC' THEN 1000.00 
                            WHEN c.ShortName = 'SOL' THEN 500.00  
                            WHEN c.ShortName = 'USDT' THEN 10000.00 
                        END,
                        c.Id, 
                        u.Id
                    FROM Users u
                    CROSS JOIN Currencies c
                    WHERE u.Email = 'testuser@example.com'
                    AND NOT EXISTS (
                        SELECT 1 FROM Balances b WHERE b.UserId = u.Id AND b.CurrencyId = c.Id
                    );";

            await connection.ExecuteAsync(sqlUsers);
            await connection.ExecuteAsync(sqlCurrencies);
            await connection.ExecuteAsync(sqlBalances);
            await connection.ExecuteAsync(sqlLogs);
            await connection.ExecuteAsync(triggerUserCreateGuid);
            await connection.ExecuteAsync(triggerUserCreateBalances);
            await connection.ExecuteAsync(triggerLogCreate);
            await connection.ExecuteAsync(testData);
        }
    }
}
