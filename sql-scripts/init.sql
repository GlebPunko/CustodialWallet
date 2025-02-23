-- Создание таблицы Users
CREATE TABLE IF NOT EXISTS Users (
    Id UUID PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE
);

-- Создание таблицы Currencies
CREATE TABLE IF NOT EXISTS Currencies (
    Id UUID PRIMARY KEY,
    ShortName VARCHAR(10) NOT NULL UNIQUE,
    FullName VARCHAR(255) NOT NULL
);

-- Создание таблицы Balances
CREATE TABLE IF NOT EXISTS Balances (
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
);

-- Триггер для генерации GUID при создании пользователя
CREATE OR REPLACE FUNCTION generate_user_id()
    RETURNS TRIGGER AS $$
    BEGIN
        NEW.Id := gen_random_uuid();
        RETURN NEW;
    END;
    $$ LANGUAGE plpgsql;

CREATE TRIGGER trg_generate_user_id
    BEFORE INSERT ON Users
    FOR EACH ROW
    EXECUTE FUNCTION generate_user_id();

-- Триггер для создания балансов по умолчанию
CREATE OR REPLACE FUNCTION create_default_balances()
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

CREATE TRIGGER trg_create_default_balances
    AFTER INSERT ON Users
    FOR EACH ROW
    EXECUTE FUNCTION create_default_balances();

-- Вставка тестовых данных
INSERT INTO Currencies (Id, ShortName, FullName)
VALUES 
    (gen_random_uuid(), 'BTC', 'Bitcoin'),
    (gen_random_uuid(), 'SOL', 'Solana'),
    (gen_random_uuid(), 'USDT', 'United States Dollar Tether');

INSERT INTO Users (Email)
VALUES ('testuser@example.com');

WITH user_id AS (
    SELECT Id FROM Users WHERE Email = 'testuser@example.com'
),
currency_ids AS (
    SELECT Id, ShortName FROM Currencies
)
INSERT INTO Balances (Id, Amount, CurrencyId, UserId)
SELECT
    gen_random_uuid(), 
    CASE 
        WHEN ShortName = 'BTC' THEN 1000.00 
        WHEN ShortName = 'SOL' THEN 500.00  
        WHEN ShortName = 'USDT' THEN 10000.00 
    END,
    currency_ids.Id, 
    user_id.Id 
FROM user_id, currency_ids;