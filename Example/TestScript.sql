-- Main table to store Header data
CREATE TABLE dbo.Headers
(
    HeaderId     INT IDENTITY(1,1) PRIMARY KEY,
    Name         NVARCHAR(100)     NOT NULL,
    Count        INT               NOT NULL,
    IsActive     BIT               NOT NULL,
    LargeNumber  BIGINT            NOT NULL,
    Ratio        DECIMAL(18, 6)    NOT NULL,
    BinaryData   VARBINARY(MAX)    NULL,
    OccurredOn   DATETIME2(7)      NOT NULL,
    OccurredAt   DATETIMEOFFSET(7) NOT NULL,
    Duration     TIME(7)           NOT NULL
);
GO

-- Detail table for Record4 items
CREATE TABLE dbo.Record4
(
    Record4Id    INT IDENTITY(1,1) PRIMARY KEY,
    HeaderId     INT               NOT NULL
        FOREIGN KEY REFERENCES dbo.Headers(HeaderId),
    Name         NVARCHAR(100)     NOT NULL,
    Count        INT               NOT NULL,
    IsActive     BIT               NOT NULL,
    LargeNumber  BIGINT            NOT NULL
);
GO

-- Detail table for AuditInfo items
CREATE TABLE dbo.AuditInfo
(
    AuditInfoId  INT IDENTITY(1,1) PRIMARY KEY,
    HeaderId     INT               NOT NULL
        FOREIGN KEY REFERENCES dbo.Headers(HeaderId),
    Description  NVARCHAR(500)     NOT NULL,
    Count        INT               NOT NULL,
    IsActive     BIT               NOT NULL,
    LargeNumber  BIGINT            NOT NULL,
    Ratio        DECIMAL(18, 6)    NOT NULL,
    BinaryData   VARBINARY(MAX)    NULL,
    OccurredOn   DATETIME2(7)      NOT NULL,
    OccurredAt   DATETIMEOFFSET(7) NOT NULL,
    Duration     TIME(7)           NOT NULL
);
GO
-- Table type for Record4 list
CREATE TYPE dbo.Record4TableType AS TABLE
(
    Name         NVARCHAR(100)     NOT NULL,
    Count        INT               NOT NULL,
    IsActive     BIT               NOT NULL,
    LargeNumber  BIGINT            NOT NULL
);
GO

-- Table type for AuditInfo list
CREATE TYPE dbo.AuditInfoTableType AS TABLE
(
    Description  NVARCHAR(500)     NOT NULL,
    Count        INT               NOT NULL,
    IsActive     BIT               NOT NULL,
    LargeNumber  BIGINT            NOT NULL,
    Ratio        DECIMAL(18, 6)    NOT NULL,
    BinaryData   VARBINARY(MAX)    NULL,
    OccurredOn   DATETIME2(7)      NOT NULL,
    OccurredAt   DATETIMEOFFSET(7) NOT NULL,
    Duration     TIME(7)           NOT NULL
);
GO




CREATE OR Alter PROCEDURE dbo.SaveFullHeaderDetails 
    @Name         NVARCHAR(100),
    @Count        INT,
    @IsActive     BIT,
    @LargeNumber  BIGINT,
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredOn   DATETIME2(7),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT, XACT_ABORT ON;

    BEGIN TRANSACTION;

    -- Insert into Header table and get the generated HeaderId
    DECLARE @NewHeader TABLE (HeaderId INT);
    INSERT INTO dbo.Headers
    (
        Name, Count, IsActive, LargeNumber, Ratio, BinaryData,
        OccurredOn, OccurredAt, Duration
    )
    OUTPUT INSERTED.HeaderId INTO @NewHeader(HeaderId)
    VALUES
    (
        @Name, @Count, @IsActive, @LargeNumber, @Ratio, @BinaryData,
        @OccurredOn, @OccurredAt, @Duration
    );

    DECLARE @HeaderId INT = (SELECT HeaderId FROM @NewHeader);

    -- Insert Record4 items
    INSERT INTO dbo.Record4 (HeaderId, Name, Count, IsActive, LargeNumber)
    SELECT @HeaderId, Name, Count, IsActive, LargeNumber
    FROM @Record4Items;

    -- Insert AuditInfo items
    INSERT INTO dbo.AuditInfo
    (
        HeaderId, Description, Count, IsActive, LargeNumber,
        Ratio, BinaryData, OccurredOn, OccurredAt, Duration
    )
    SELECT
        @HeaderId, Description, Count, IsActive, LargeNumber,
        Ratio, BinaryData, OccurredOn, OccurredAt, Duration
    FROM @AuditItems;

    COMMIT TRANSACTION;

    -- Return inserted values (from actual tables)
    SELECT * FROM dbo.Headers;
    SELECT  * FROM dbo.Record4 ORDER BY Record4Id;
    SELECT  * FROM dbo.AuditInfo ORDER BY AuditInfoId;
END;
GO



CREATE PROCEDURE dbo.InsertHeaderOnly
    @Name         NVARCHAR(100),
    @Count        INT,
    @IsActive     BIT,
    @LargeNumber  BIGINT,
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredOn   DATETIME2(7),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7)
AS
BEGIN

    INSERT INTO dbo.Headers
    (
        Name, Count, IsActive, LargeNumber, Ratio, BinaryData,
        OccurredOn, OccurredAt, Duration
    )
    VALUES
    (
        @Name, @Count, @IsActive, @LargeNumber, @Ratio, @BinaryData,
        @OccurredOn, @OccurredAt, @Duration
    );

END;
GO




CREATE PROCEDURE dbo.InsertHeaderStatic
AS
BEGIN

    INSERT INTO dbo.Headers
    (
        Name,
        Count,
        IsActive,
        LargeNumber,
        Ratio,
        BinaryData,
        OccurredOn,
        OccurredAt,
        Duration
    )
    VALUES
    (
        N'Static Header',         -- Name
        100,                      -- Count
        1,                        -- IsActive (true)
        9999999999,               -- LargeNumber
        12.3456,                  -- Ratio
        CONVERT(VARBINARY(MAX), 'StaticData'), -- BinaryData
        SYSDATETIME(),            -- OccurredOn (DateTime)
        SYSDATETIMEOFFSET(),      -- OccurredAt (DateTimeOffset)
        '01:30:00'                -- Duration (1 hour 30 minutes)
    );

END;
GO
