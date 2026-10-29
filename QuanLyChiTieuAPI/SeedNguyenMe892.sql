-- ============================================
-- Seed data for account nguyenme892@gmail.com
-- Database: QuanLyChiTieuDb (SQL Server Express)
-- ============================================

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Step 1: Find or create user
DECLARE @UserId INT;
DECLARE @UserExists INT;

SELECT @UserExists = COUNT(*) FROM Users WHERE Email = N'nguyenme892@gmail.com';

IF @UserExists = 0
BEGIN
    INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (
        N'Nguyen Van A',
        N'nguyenme892@gmail.com',
        N'$2b$12$fxx4r.62pfNplNzVzyWiWuLuSgcxdU.dtwPkqubusGzMFdm8zQDMe',
        N'0123456789',
        1,  -- IsActive
        GETDATE(),
        GETDATE()
    );
    SET @UserId = SCOPE_IDENTITY();
    PRINT 'User created with Id = ' + CAST(@UserId AS NVARCHAR(10));
END
ELSE
BEGIN
    SELECT @UserId = Id FROM Users WHERE Email = N'nguyenme892@gmail.com';
    PRINT 'User found with Id = ' + CAST(@UserId AS NVARCHAR(10));
END

-- Step 2: Create wallets if not exist
IF NOT EXISTS (SELECT 1 FROM Wallets WHERE UserId = @UserId AND Name = N'Tiền mặt')
BEGIN
    INSERT INTO Wallets (UserId, Name, Description, Balance, Currency, IsDefault, IsActive, CreatedAt, UpdatedAt)
    VALUES (@UserId, N'Tiền mặt', N'Ví tiền mặt', 15000000, N'VND', 1, 1, GETDATE(), GETDATE());
    PRINT 'Wallet [Tiền mặt] created';
END
ELSE
BEGIN
    PRINT 'Wallet [Tiền mặt] already exists';
END

IF NOT EXISTS (SELECT 1 FROM Wallets WHERE UserId = @UserId AND Name = N'Tài khoản Ngân hàng')
BEGIN
    INSERT INTO Wallets (UserId, Name, Description, Balance, Currency, IsDefault, IsActive, CreatedAt, UpdatedAt)
    VALUES (@UserId, N'Tài khoản Ngân hàng', N'Tài khoản ngân hàng', 50000000, N'VND', 0, 1, GETDATE(), GETDATE());
    PRINT 'Wallet [Tài khoản Ngân hàng] created';
END
ELSE
BEGIN
    PRINT 'Wallet [Tài khoản Ngân hàng] already exists';
END

IF NOT EXISTS (SELECT 1 FROM Wallets WHERE UserId = @UserId AND Name = N'Thẻ tín dụng')
BEGIN
    INSERT INTO Wallets (UserId, Name, Description, Balance, Currency, IsDefault, IsActive, CreatedAt, UpdatedAt)
    VALUES (@UserId, N'Thẻ tín dụng', N'Thẻ tín dụng', -2000000, N'VND', 0, 1, GETDATE(), GETDATE());
    PRINT 'Wallet [Thẻ tín dụng] created';
END
ELSE
BEGIN
    PRINT 'Wallet [Thẻ tín dụng] already exists';
END

-- Get wallet IDs (ASCII variable names)
DECLARE @Wallet1 INT, @Wallet2 INT, @Wallet3 INT;

SELECT @Wallet1 = Id FROM Wallets WHERE UserId = @UserId AND Name = N'Tiền mặt';
SELECT @Wallet2 = Id FROM Wallets WHERE UserId = @UserId AND Name = N'Tài khoản Ngân hàng';
SELECT @Wallet3 = Id FROM Wallets WHERE UserId = @UserId AND Name = N'Thẻ tín dụng';

PRINT 'Wallet IDs: Wallet1=' + CAST(@Wallet1 AS NVARCHAR(10)) + 
      ', Wallet2=' + CAST(@Wallet2 AS NVARCHAR(10)) + 
      ', Wallet3=' + CAST(@Wallet3 AS NVARCHAR(10));

-- Step 3: Use WHILE loop to insert 600 random transactions (2024-2026)
DECLARE @i INT = 1;
DECLARE @TransactionDate DATETIME2;
DECLARE @CategoryId INT;
DECLARE @WalletId INT;
DECLARE @IsIncome BIT;
DECLARE @Amount DECIMAL(18, 2);
DECLARE @Note NVARCHAR(200);
DECLARE @YearOffset INT;
DECLARE @Month INT;
DECLARE @Day INT;
DECLARE @NoteSuffixes TABLE (Suffix NVARCHAR(50));

-- Pre-populate note suffixes
INSERT INTO @NoteSuffixes VALUES (N'tháng 1'), (N'tháng 2'), (N'tháng 3'), (N'tháng 4'),
                                  (N'tháng 5'), (N'tháng 6'), (N'tháng 7'), (N'tháng 8'),
                                  (N'tháng 9'), (N'tháng 10'), (N'tháng 11'), (N'tháng 12');

WHILE @i <= 600
BEGIN
    -- Random date between 2024-01-01 and 2026-10-28 (using ABS(CHECKSUM(NEWID())) for random integers)
    SET @YearOffset = ABS(CHECKSUM(NEWID())) % 3;
    SET @Month = (ABS(CHECKSUM(NEWID())) % 12) + 1;
    SET @Day = (ABS(CHECKSUM(NEWID())) % 28) + 1;
    SET @TransactionDate = DATEADD(DAY, @Day - 1, DATEADD(MONTH, @Month - 1, DATEADD(YEAR, @YearOffset, '2024-01-01')));

    -- Random category (1-12)
    SET @CategoryId = (ABS(CHECKSUM(NEWID())) % 12) + 1;

    -- Random wallet (balanced distribution)
    SET @WalletId = CASE (ABS(CHECKSUM(NEWID())) % 3)
                     WHEN 0 THEN @Wallet1
                     WHEN 1 THEN @Wallet2
                     ELSE @Wallet3
                   END;

    -- Random amount (Expense: 20,000 - 5,000,000 VND; Income: 2,000,000 - 30,000,000 VND)
    SET @IsIncome = (ABS(CHECKSUM(NEWID())) % 2);

    IF @IsIncome = 1
        SET @Amount = ROUND((ABS(CHECKSUM(NEWID())) % 28000001 + 2000000), 2);
    ELSE
        SET @Amount = ROUND((ABS(CHECKSUM(NEWID())) % 4980001 + 20000), 2);

    -- Random note
    SET @Note = CASE @IsIncome
                  WHEN 1 THEN N'Thu nhập ' + (SELECT TOP 1 Suffix FROM @NoteSuffixes ORDER BY NEWID())
                  ELSE N'Chi tiêu ' + (SELECT TOP 1 Suffix FROM @NoteSuffixes ORDER BY NEWID())
                END;

    -- Insert transaction
    IF @UserId IS NOT NULL AND @WalletId IS NOT NULL
    BEGIN
        SET IDENTITY_INSERT Transactions ON;
        INSERT INTO Transactions (Id, Amount, Note, IsIncome, WalletId, CategoryId, UserId, TransactionDate, CreatedAt, UpdatedAt)
        VALUES (
            3000 + @i,
            @Amount,
            @Note,
            @IsIncome,
            @WalletId,
            @CategoryId,
            @UserId,
            @TransactionDate,
            GETDATE(),
            GETDATE()
        );
        SET IDENTITY_INSERT Transactions OFF;
    END

    SET @i = @i + 1;
END

PRINT '600 transactions inserted successfully.';

-- Step 4: Update wallet balances based on transaction totals
BEGIN
    DECLARE @TotalIncome DECIMAL(18, 2), @TotalExpense DECIMAL(18, 2);

    -- Calculate totals for each wallet
    UPDATE Wallets
    SET Balance = (
        SELECT COALESCE(SUM(CASE WHEN T.IsIncome = 1 THEN T.Amount ELSE -T.Amount END), 0)
        FROM Transactions T
        WHERE T.WalletId = Wallets.Id
    )
    WHERE UserId = @UserId;

    PRINT 'Wallet balances recalculated.';
END

-- Step 5: Verify data
PRINT 'Verification:';
DECLARE @UserCount INT, @WalletCount INT, @TransactionCount INT;
SELECT @UserCount = COUNT(*) FROM Users WHERE Email = N'nguyenme892@gmail.com';
SELECT @WalletCount = COUNT(*) FROM Wallets WHERE UserId = @UserId;
SELECT @TransactionCount = COUNT(*) FROM Transactions WHERE UserId = @UserId;
PRINT 'Users: ' + CAST(@UserCount AS NVARCHAR(10));
PRINT 'Wallets: ' + CAST(@WalletCount AS NVARCHAR(10));
SELECT Id, Name, Balance FROM Wallets WHERE UserId = @UserId;
PRINT 'Transactions: ' + CAST(@TransactionCount AS NVARCHAR(10));
SELECT COUNT(*) FROM Transactions WHERE UserId = @UserId;
