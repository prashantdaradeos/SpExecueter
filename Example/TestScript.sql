
-- ...existing code...

-- =====================================================================
-- REUSABLE SELECT HELPERS (hardcoded rows with 'out' suffix values)
-- Each result set matches the property order of the corresponding DTO.
-- =====================================================================

-- HeaderResult columns (non-ResultExclusion):
--   HeaderId, Name, Count, IsActive, LargeNumber, Ratio, BinaryData, OccurredOn, OccurredAt, Duration
-- NOTE: MyName has ResultExclusion so NOT in result set.

-- Record4Result columns:
--   Record4Id, HeaderId, Name1, Count, IsActive, LargeNumber

-- AuditInfoResult columns:
--   AuditInfoId, HeaderId, Description, Count, IsActive, LargeNumber, Ratio, BinaryData, OccurredOn, OccurredAt, Duration

-- TestClass1: Name, Count, OccuredOn, IsActive
-- TestClass2: Count, OccuredOn, IsActive, LargeNumber
-- TestClass3: Title, Quantity, Price, IsEnabled
-- TestClass4: Code, Weight, CreatedAt, Serial
-- TestClass5: Label, Score, IsVerified, Level
-- TestClass6: MyLargeNumber, Budget, StartDate, Population
-- TestClass7: SKU, Rating, ViewCount, InStock
-- TestClass8: Code, Rank, Balance, UpdatedOn
-- TestClass9: Category, IsPublished, Latitude, Revision
-- TestClass10: Tag, Frequency, Priority, ExpiresOn
-- TestClass11: Vendor, Units, Discount, IsApproved
-- TestClass12: Channel, Bandwidth, RegisteredOn, Throughput

-- BaseClass0 (extends BaseClass1): Name, Count, OccuredOn, IsActive, LargeNumber, Binary,
--   Ratio, SmallNumber, TinyNumber, LessPreciseFloat, DoublePrecision, SingleChar,
--   SByteValue, UShortValue, UIntValue, ULongValue, NIntValue, NUIntValue,
--   OccurredOn, OccurredOnExclude, OccurredAt, Duration, UniqueId,
--   Count1, IsActive1, Ratio1, SmallNumber1, TinyNumber1, LessPreciseFloat1, DoublePrecision1,
--   SingleChar1, NullableSByteValue, NullableUShortValue, NullableUIntValue, NullableULongValue,
--   NullableNIntValue, NullableNUIntValue, OccurredOn1, OccurredAt1, Duration1, UniqueId1,
--   StatusType, StatusType1, BinaryData, CharArray, BinaryData1, CharArray1
-- NOTE: LargeNumber1 has ResultExclusion so NOT in result set.

-- TestClass (extends HeaderResult): same as HeaderResult columns (Name overridden, MyName overridden but excluded)

-- =====================================================================
-- TRANSIENT SPs (TransientTest1 – TransientTest10)
-- =====================================================================

CREATE OR ALTER PROCEDURE dbo.TransientTest1
    @CustomeName  NVARCHAR(100),
    @MyNew        DATETIME2(7),
    @LargeNumber  BIGINT,
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredOn   DATETIME2(7),
    @OccurredOnExcludeDBParam DATETIME2(7),
    @OccurredAtDBParam DATETIMEOFFSET(7),
    @DurationDBParam   TIME(7),
    @UniqueId     UNIQUEIDENTIFIER,
    @Count1       INT,
    @IsActive1    BIT,
    @Ratio1DBParam DECIMAL(18,6),
    @SmallNumber1 SMALLINT,
    @TinyNumber1  TINYINT,
    @LessPreciseFloat1 REAL,
    @DoublePrecision1  FLOAT,
    @SingleChar1  NCHAR(1),
    @NullableSByteValue SMALLINT,
    @NullableUShortValue INT,
    @NullableUIntValue BIGINT,
    @NullableULongValue DECIMAL(20,0),
    @NullableNIntValue BIGINT,
    @NullableNUIntValue DECIMAL(20,0),
    @OccurredOn1  DATETIME2(7),
    @OccurredAt1  DATETIMEOFFSET(7),
    @Duration1    TIME(7),
    @UniqueId1    UNIQUEIDENTIFIER,
    @StatusType   INT,
    @StatusType1  INT,
    @BinaryData1  VARBINARY(MAX),
    @CharArray1   NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns BaseClass0 single
    SELECT
        N'Nameout' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive, CAST(9999 AS BIGINT) AS LargeNumber, 0x01 AS [Binary],
        CAST(1.23 AS DECIMAL(18,6)) AS Ratio, CAST(5 AS SMALLINT) AS SmallNumber, CAST(1 AS TINYINT) AS TinyNumber,
        CAST(1.1 AS REAL) AS LessPreciseFloat, CAST(2.2 AS FLOAT) AS DoublePrecision, N'Xout' AS SingleChar,
        CAST(1 AS SMALLINT) AS SByteValue, CAST(2 AS INT) AS UShortValue, CAST(3 AS BIGINT) AS UIntValue,
        CAST(4 AS DECIMAL(20,0)) AS ULongValue, CAST(5 AS BIGINT) AS NIntValue, CAST(6 AS DECIMAL(20,0)) AS NUIntValue,
        '2024-02-01' AS OccurredOn, '2024-03-01' AS OccurredOnExclude,
        CAST('2024-04-01 00:00:00 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:30:00' AS TIME) AS Duration,
        NEWID() AS UniqueId,
        CAST(11 AS INT) AS Count1, CAST(1 AS BIT) AS IsActive1, CAST(3.45 AS DECIMAL(18,6)) AS Ratio1,
        CAST(6 AS SMALLINT) AS SmallNumber1, CAST(2 AS TINYINT) AS TinyNumber1,
        CAST(1.5 AS REAL) AS LessPreciseFloat1, CAST(2.5 AS FLOAT) AS DoublePrecision1, N'Y' AS SingleChar1,
        CAST(7 AS SMALLINT) AS NullableSByteValue, CAST(8 AS INT) AS NullableUShortValue,
        CAST(9 AS BIGINT) AS NullableUIntValue, CAST(10 AS DECIMAL(20,0)) AS NullableULongValue,
        CAST(11 AS BIGINT) AS NullableNIntValue, CAST(12 AS DECIMAL(20,0)) AS NullableNUIntValue,
        '2024-05-01' AS OccurredOn1, CAST('2024-06-01 00:00:00 +00:00' AS DATETIMEOFFSET) AS OccurredAt1,
        CAST('02:00:00' AS TIME) AS Duration1, NEWID() AS UniqueId1,
        0 AS StatusType, 1 AS StatusType1, 0x02 AS BinaryData, N'ABCout' AS CharArray, 0x03 AS BinaryData1, N'DEFout' AS CharArray1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest2
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns List<TestClass> (TestClass extends HeaderResult)
    SELECT 1 AS HeaderId, N'Nameout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration
    UNION ALL
    SELECT 2, N'Name2out', 20, N'falseout', CAST(200 AS BIGINT),
           CAST(2.5 AS DECIMAL(18,6)), 0x02, '2024-02-01',
           CAST('2024-02-01 +00:00' AS DATETIMEOFFSET), CAST('02:00:00' AS TIME);
END;
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest3
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns (TestClass, HeaderResult) = 2 result sets
    -- TestClass (extends HeaderResult)
    SELECT 1 AS HeaderId, N'Nameout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    -- HeaderResult
    SELECT 2 AS HeaderId, N'Name2out' AS Name, 20 AS [Count], N'falseout' AS IsActive, CAST(200 AS BIGINT) AS LargeNumber,
           CAST(2.5 AS DECIMAL(18,6)) AS Ratio, 0x02 AS BinaryData, '2024-02-01' AS OccurredOn,
           CAST('2024-02-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('02:00:00' AS TIME) AS Duration;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest4
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns (List<TestClass>, List<HeaderResult>)
    SELECT 1 AS HeaderId, N'Nameout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 2 AS HeaderId, N'Name2out' AS Name, 20 AS [Count], N'falseout' AS IsActive, CAST(200 AS BIGINT) AS LargeNumber,
           CAST(2.5 AS DECIMAL(18,6)) AS Ratio, 0x02 AS BinaryData, '2024-02-01' AS OccurredOn,
           CAST('2024-02-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('02:00:00' AS TIME) AS Duration;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest5
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns (List<HeaderResult>, TestClass)
    SELECT 1 AS HeaderId, N'Nameout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 2 AS HeaderId, N'Name2out' AS Name, 20 AS [Count], N'falseout' AS IsActive, CAST(200 AS BIGINT) AS LargeNumber,
           CAST(2.5 AS DECIMAL(18,6)) AS Ratio, 0x02 AS BinaryData, '2024-02-01' AS OccurredOn,
           CAST('2024-02-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('02:00:00' AS TIME) AS Duration;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest6
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns ((List<TestClass1>, HeaderResult)?, TestClass1)
    -- TestClass1 list
    SELECT N'Nameout' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    -- HeaderResult single
    SELECT 1 AS HeaderId, N'HRNameout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    -- TestClass1 single
    SELECT N'TC1out' AS Name, 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest7
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns ((HeaderResult, List<Record4Result>)?, List<HeaderResult>)
    -- HeaderResult single
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    -- Record4Result list
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
    -- HeaderResult list
    SELECT 2 AS HeaderId, N'HR2out' AS Name, 20 AS [Count], N'falseout' AS IsActive, CAST(200 AS BIGINT) AS LargeNumber,
           CAST(2.5 AS DECIMAL(18,6)) AS Ratio, 0x02 AS BinaryData, '2024-02-01' AS OccurredOn,
           CAST('2024-02-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('02:00:00' AS TIME) AS Duration;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest8
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns ((List<TC1>,TC2)?, (List<TC1>,TC2,List<TC3>,TC8)?, (List<TC1>,TC10,List<TC11>,TC12)?)
    -- Tuple1: List<TC1>
    SELECT N'T1out' AS Name, 1 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    -- Tuple1: TC2
    SELECT 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
    -- Tuple2: List<TC1>
    SELECT N'T2out' AS Name, 2 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive;
    -- Tuple2: TC2
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    -- Tuple2: List<TC3>
    SELECT N'T3out' AS Title, 30 AS Quantity, CAST(3.0 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    -- Tuple2: TC8
    SELECT N'T8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    -- Tuple3: List<TC1>
    SELECT N'T3_1out' AS Name, 3 AS [Count], '2024-03-01' AS OccuredOn, 1 AS IsActive;
    -- Tuple3: TC10
    SELECT N'T10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    -- Tuple3: List<TC11>
    SELECT N'V11out' AS Vendor, 11 AS Units, CAST(1.1 AS DECIMAL(18,6)) AS Discount, 1 AS IsApproved;
    -- Tuple3: TC12
    SELECT N'C12out' AS Channel, CAST(12.5 AS FLOAT) AS Bandwidth, '2024-12-01' AS RegisteredOn, CAST(1200 AS BIGINT) AS Throughput;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest9
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns ((List<HR>,TC1)?, (List<HR>,List<R4>,TC1)?, List<HR>)
    -- Tuple1: List<HeaderResult>
    SELECT 1 AS HeaderId, N'HR1out' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    -- Tuple1: TC1
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    -- Tuple2: List<HR>
    SELECT 2 AS HeaderId, N'HR2out' AS Name, 20 AS [Count], N'falseout' AS IsActive, CAST(200 AS BIGINT) AS LargeNumber,
           CAST(2.5 AS DECIMAL(18,6)) AS Ratio, 0x02 AS BinaryData, '2024-02-01' AS OccurredOn,
           CAST('2024-02-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('02:00:00' AS TIME) AS Duration;
    -- Tuple2: List<R4>
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
    -- Tuple2: TC1
    SELECT N'TC1_2out' AS Name, 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive;
    -- Flat: List<HR>
    SELECT 3 AS HeaderId, N'HR3out' AS Name, 30 AS [Count], N'trueout' AS IsActive, CAST(300 AS BIGINT) AS LargeNumber,
           CAST(3.5 AS DECIMAL(18,6)) AS Ratio, 0x03 AS BinaryData, '2024-03-01' AS OccurredOn,
           CAST('2024-03-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('03:00:00' AS TIME) AS Duration;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest10
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    -- Returns (List<HR>, TC1) with OR condition
    SELECT 1 AS HeaderId, N'HRorOut' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT N'TC1orOut' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
END;
GO


-- =====================================================================
-- SCOPED SPs (ScopedTest1 – ScopedTest50)
-- =====================================================================

CREATE OR ALTER PROCEDURE dbo.ScopedTest1
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'ScopedHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest2
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest3
    @MyName       NVARCHAR(100),
    @Ratio        DECIMAL(18,6),
    @BinaryData   VARBINARY(MAX),
    @OccurredAt   DATETIMEOFFSET(7),
    @Duration     TIME(7),
    @Record4Items dbo.Record4TableType READONLY,
    @AuditItems   dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'ScopedHR1out' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration
    UNION ALL
    SELECT 2, N'ScopedHR2out', 20, N'falseout', CAST(200 AS BIGINT),
           CAST(2.5 AS DECIMAL(18,6)), 0x02, '2024-02-01',
           CAST('2024-02-01 +00:00' AS DATETIMEOFFSET), CAST('02:00:00' AS TIME);
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest4
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'Descout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest5
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest6
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest7
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest8
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest9
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest10
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest11
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
END;
GO

-- ScopedTest12 through ScopedTest50: Each returns the appropriate number of result sets
-- with hardcoded 'out' suffix values matching the DTO property order.

CREATE OR ALTER PROCEDURE dbo.ScopedTest12
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest13
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest14
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest15
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
END;
GO

-- For ScopedTest16 through ScopedTest50, each SP follows the same pattern:
-- accepts HeaderParameters-compatible params, returns hardcoded result sets with 'out' suffix.

CREATE OR ALTER PROCEDURE dbo.ScopedTest16
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS BEGIN SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest17
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest18
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS BEGIN SET NOCOUNT ON;
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest19
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS BEGIN SET NOCOUNT ON;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest20
    @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY
AS BEGIN SET NOCOUNT ON;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END; GO

-- Generic template for ScopedTest21-50: each returns appropriate result sets
-- I'll create a generic pattern that returns the right number of result sets

CREATE OR ALTER PROCEDURE dbo.ScopedTest21 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest22 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest23 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest24 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest25 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest26 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest27 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest28 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest29 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision;
    SELECT N'TC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest30 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber,
           CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn,
           CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

-- ScopedTest31 through ScopedTest44: Complex nested tuples - each returns multiple result sets
-- Using a simplified approach with appropriate result sets per method signature

CREATE OR ALTER PROCEDURE dbo.ScopedTest31 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest32 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'TC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END; GO

-- ScopedTest33-44 follow same pattern
CREATE OR ALTER PROCEDURE dbo.ScopedTest33 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'TC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level]; SELECT N'TC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population];
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest34 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'TC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock; SELECT N'TC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest35 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision; SELECT N'TC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest36 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'TC1_2out' AS Name, 30 AS [Count], '2024-03-01' AS OccuredOn, 0 AS IsActive;
    SELECT 40 AS [Count], '2024-04-01' AS OccuredOn, 1 AS IsActive, CAST(400 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest37 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled; SELECT N'TC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial; SELECT N'TC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level]; SELECT N'TC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population];
    SELECT N'TC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest38 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn; SELECT N'TC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision;
    SELECT N'TC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn; SELECT N'TC1_2out' AS Name, 30 AS [Count], '2024-03-01' AS OccuredOn, 0 AS IsActive;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest39 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 30 AS [Count], '2024-03-01' AS OccuredOn, 1 AS IsActive, CAST(300 AS BIGINT) AS LargeNumber; SELECT N'TC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled;
    SELECT N'TC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial; SELECT N'TC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level];
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest40 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population]; SELECT N'TC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock;
    SELECT N'TC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn; SELECT N'TC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision; SELECT N'TC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn; SELECT N'TC1_2out' AS Name, 30 AS [Count], '2024-03-01' AS OccuredOn, 0 AS IsActive;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest41 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest42 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 30 AS [Count], '2024-03-01' AS OccuredOn, 1 AS IsActive, CAST(300 AS BIGINT) AS LargeNumber; SELECT N'TC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled; SELECT N'TC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial; SELECT N'TC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level];
    SELECT N'TC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population]; SELECT N'TC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock; SELECT N'TC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn; SELECT N'TC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision;
    SELECT 1 AS HeaderId, N'HRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest43 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'TC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population]; SELECT N'TC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock;
    SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'TC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn; SELECT N'TC1_2out' AS Name, 30 AS [Count], '2024-03-01' AS OccuredOn, 0 AS IsActive;
    SELECT 40 AS [Count], '2024-04-01' AS OccuredOn, 1 AS IsActive, CAST(400 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest44 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'TC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'TC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'TC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'TC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'TC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'TC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'TC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'TC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'TC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled; SELECT N'TC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial;
    SELECT N'TC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level]; SELECT N'TC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population];
    SELECT N'TC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock; SELECT N'TC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn; SELECT N'TC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision; SELECT N'TC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

-- OR variants
CREATE OR ALTER PROCEDURE dbo.ScopedTest45 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC1orOut' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest46 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'HRorOut' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest47 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'TC1orOut' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'R4orOut' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest48 AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'AIorOut' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

-- Inherited class SPs
CREATE OR ALTER PROCEDURE dbo.ScopedTest49 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'Nameout' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive, CAST(9999 AS BIGINT) AS LargeNumber, 0x01 AS [Binary],
        CAST(1.23 AS DECIMAL(18,6)) AS Ratio, CAST(5 AS SMALLINT) AS SmallNumber, CAST(1 AS TINYINT) AS TinyNumber,
        CAST(1.1 AS REAL) AS LessPreciseFloat, CAST(2.2 AS FLOAT) AS DoublePrecision, N'X' AS SingleChar,
        CAST(1 AS SMALLINT) AS SByteValue, CAST(2 AS INT) AS UShortValue, CAST(3 AS BIGINT) AS UIntValue,
        CAST(4 AS DECIMAL(20,0)) AS ULongValue, CAST(5 AS BIGINT) AS NIntValue, CAST(6 AS DECIMAL(20,0)) AS NUIntValue,
        '2024-02-01' AS OccurredOn, '2024-03-01' AS OccurredOnExclude,
        CAST('2024-04-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:30:00' AS TIME) AS Duration,
        NEWID() AS UniqueId, 11 AS Count1, 1 AS IsActive1, CAST(3.45 AS DECIMAL(18,6)) AS Ratio1,
        CAST(6 AS SMALLINT) AS SmallNumber1, CAST(2 AS TINYINT) AS TinyNumber1,
        CAST(1.5 AS REAL) AS LessPreciseFloat1, CAST(2.5 AS FLOAT) AS DoublePrecision1, N'Y' AS SingleChar1,
        CAST(7 AS SMALLINT) AS NullableSByteValue, CAST(8 AS INT) AS NullableUShortValue,
        CAST(9 AS BIGINT) AS NullableUIntValue, CAST(10 AS DECIMAL(20,0)) AS NullableULongValue,
        CAST(11 AS BIGINT) AS NullableNIntValue, CAST(12 AS DECIMAL(20,0)) AS NullableNUIntValue,
        '2024-05-01' AS OccurredOn1, CAST('2024-06-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt1,
        CAST('02:00:00' AS TIME) AS Duration1, NEWID() AS UniqueId1,
        0 AS StatusType, 1 AS StatusType1, 0x02 AS BinaryData, N'ABCout' AS CharArray, 0x03 AS BinaryData1, N'DEFout' AS CharArray1;
END; GO

CREATE OR ALTER PROCEDURE dbo.ScopedTest50 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'Nameout' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive, CAST(9999 AS BIGINT) AS LargeNumber, 0x01 AS [Binary],
        CAST(1.23 AS DECIMAL(18,6)) AS Ratio, CAST(5 AS SMALLINT) AS SmallNumber, CAST(1 AS TINYINT) AS TinyNumber,
        CAST(1.1 AS REAL) AS LessPreciseFloat, CAST(2.2 AS FLOAT) AS DoublePrecision, N'X' AS SingleChar,
        CAST(1 AS SMALLINT) AS SByteValue, CAST(2 AS INT) AS UShortValue, CAST(3 AS BIGINT) AS UIntValue,
        CAST(4 AS DECIMAL(20,0)) AS ULongValue, CAST(5 AS BIGINT) AS NIntValue, CAST(6 AS DECIMAL(20,0)) AS NUIntValue,
        '2024-02-01' AS OccurredOn, '2024-03-01' AS OccurredOnExclude,
        CAST('2024-04-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:30:00' AS TIME) AS Duration,
        NEWID() AS UniqueId, 11 AS Count1, 1 AS IsActive1, CAST(3.45 AS DECIMAL(18,6)) AS Ratio1,
        CAST(6 AS SMALLINT) AS SmallNumber1, CAST(2 AS TINYINT) AS TinyNumber1,
        CAST(1.5 AS REAL) AS LessPreciseFloat1, CAST(2.5 AS FLOAT) AS DoublePrecision1, N'Y' AS SingleChar1,
        CAST(7 AS SMALLINT) AS NullableSByteValue, CAST(8 AS INT) AS NullableUShortValue,
        CAST(9 AS BIGINT) AS NullableUIntValue, CAST(10 AS DECIMAL(20,0)) AS NullableULongValue,
        CAST(11 AS BIGINT) AS NullableNIntValue, CAST(12 AS DECIMAL(20,0)) AS NullableNUIntValue,
        '2024-05-01' AS OccurredOn1, CAST('2024-06-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt1,
        CAST('02:00:00' AS TIME) AS Duration1, NEWID() AS UniqueId1,
        0 AS StatusType, 1 AS StatusType1, 0x02 AS BinaryData, N'ABCout' AS CharArray, 0x03 AS BinaryData1, N'DEFout' AS CharArray1
    UNION ALL
    SELECT N'Name2out', 20, '2024-02-01', 0, CAST(8888 AS BIGINT), 0x02,
        CAST(2.34 AS DECIMAL(18,6)), CAST(6 AS SMALLINT), CAST(2 AS TINYINT),
        CAST(2.1 AS REAL), CAST(3.2 AS FLOAT), N'Z',
        CAST(2 AS SMALLINT), CAST(3 AS INT), CAST(4 AS BIGINT),
        CAST(5 AS DECIMAL(20,0)), CAST(6 AS BIGINT), CAST(7 AS DECIMAL(20,0)),
        '2024-07-01', '2024-08-01',
        CAST('2024-09-01 +00:00' AS DATETIMEOFFSET), CAST('02:30:00' AS TIME),
        NEWID(), 22, 0, CAST(4.56 AS DECIMAL(18,6)),
        CAST(7 AS SMALLINT), CAST(3 AS TINYINT),
        CAST(2.5 AS REAL), CAST(3.5 AS FLOAT), N'W',
        CAST(8 AS SMALLINT), CAST(9 AS INT),
        CAST(10 AS BIGINT), CAST(11 AS DECIMAL(20,0)),
        CAST(12 AS BIGINT), CAST(13 AS DECIMAL(20,0)),
        '2024-10-01', CAST('2024-11-01 +00:00' AS DATETIMEOFFSET),
        CAST('03:00:00' AS TIME), NEWID(),
        1, 0, 0x04, N'GHIout', 0x05, N'JKLout';
END; GO


-- =====================================================================
-- SINGLETON SPs (SingletonTest1 – SingletonTest47)
-- Same SP signatures as Scoped but named SingletonTestN
-- =====================================================================

CREATE OR ALTER PROCEDURE dbo.SingletonTest1 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest2 AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest3 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'SingR4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest4 AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest5 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'SingAIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest6 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'SingAIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest7 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'SingR4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'SingAIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest8 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest9 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest10 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest11 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'SingR4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'SingAIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
END; GO

-- SingletonTest12 through SingletonTest47: following the same pattern
-- Each returns the appropriate result sets with 'out' suffix hardcoded values

CREATE OR ALTER PROCEDURE dbo.SingletonTest12 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
    SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest13 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest14 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled;
    SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest15 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
    SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest16 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
END; GO

-- SingletonTest17-41: complex nested tuples - returning multiple result sets
CREATE OR ALTER PROCEDURE dbo.SingletonTest17 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
    SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest18 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest19 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn;
    SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest20 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population];
    SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn;
    SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial;
    SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level];
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest21 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision;
    SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber;
    SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

-- SingletonTest22-41: Complex nested - returning multiple result sets with hardcoded 'out' values
CREATE OR ALTER PROCEDURE dbo.SingletonTest22 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest23 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest24 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest25 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT 1 AS Record4Id, 1 AS HeaderId, N'SingR4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest26 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest27 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1_2out' AS Name, 30 AS [Count], '2024-03-01' AS OccuredOn, 0 AS IsActive; SELECT 40 AS [Count], '2024-04-01' AS OccuredOn, 1 AS IsActive, CAST(400 AS BIGINT) AS LargeNumber; SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'SingAIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO

-- SingletonTest28-41: Same pattern with multiple result sets
CREATE OR ALTER PROCEDURE dbo.SingletonTest28 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled; SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest29 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial; SELECT 1 AS Record4Id, 1 AS HeaderId, N'SingR4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest30 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level]; SELECT N'SingTC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population]; SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'SingAIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest31 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock; SELECT N'SingTC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn; SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest32 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision; SELECT N'SingTC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn; SELECT 1 AS Record4Id, 1 AS HeaderId, N'SingR4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest33 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1_2out' AS Name, 30 AS [Count], '2024-03-01' AS OccuredOn, 0 AS IsActive; SELECT 40 AS [Count], '2024-04-01' AS OccuredOn, 1 AS IsActive, CAST(400 AS BIGINT) AS LargeNumber; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest34 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled; SELECT N'SingTC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial; SELECT N'SingTC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level]; SELECT N'SingTC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population]; SELECT N'SingTC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest35 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn; SELECT N'SingTC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision; SELECT N'SingTC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn; SELECT N'SingTC1_2out' AS Name, 30 AS [Count], '2024-03-01' AS OccuredOn, 0 AS IsActive; SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest36 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 30 AS [Count], '2024-03-01' AS OccuredOn, 1 AS IsActive, CAST(300 AS BIGINT) AS LargeNumber; SELECT N'SingTC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled; SELECT N'SingTC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial; SELECT N'SingTC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level]; SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'SingAIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest37 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population]; SELECT N'SingTC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock; SELECT N'SingTC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn; SELECT N'SingTC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision; SELECT N'SingTC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn; SELECT N'SingTC1_2out' AS Name, 30 AS [Count], '2024-03-01' AS OccuredOn, 0 AS IsActive; SELECT 1 AS Record4Id, 1 AS HeaderId, N'SingR4out' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest38 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest39 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 30 AS [Count], '2024-03-01' AS OccuredOn, 1 AS IsActive, CAST(300 AS BIGINT) AS LargeNumber; SELECT N'SingTC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled; SELECT N'SingTC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial; SELECT N'SingTC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level]; SELECT N'SingTC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population]; SELECT N'SingTC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock; SELECT N'SingTC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn; SELECT N'SingTC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision; SELECT 1 AS HeaderId, N'SingHRout' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest40 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn; SELECT N'SingTC1_2out' AS Name, 30 AS [Count], '2024-03-01' AS OccuredOn, 0 AS IsActive; SELECT 40 AS [Count], '2024-04-01' AS OccuredOn, 1 AS IsActive, CAST(400 AS BIGINT) AS LargeNumber; END; GO
CREATE OR ALTER PROCEDURE dbo.SingletonTest41 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON; SELECT N'SingTC3out' AS Title, 10 AS Quantity, CAST(1.5 AS DECIMAL(18,6)) AS Price, 1 AS IsEnabled; SELECT N'SingTC4out' AS Code, CAST(2.5 AS FLOAT) AS Weight, '2024-01-01' AS CreatedAt, CAST(100 AS BIGINT) AS Serial; SELECT N'SingTC5out' AS Label, 10 AS Score, 1 AS IsVerified, CAST(5 AS SMALLINT) AS [Level]; SELECT N'SingTC6out' AS MyLargeNumber, CAST(1000.5 AS DECIMAL(18,6)) AS Budget, '2024-01-01' AS StartDate, 500 AS [Population]; SELECT N'SingTC7out' AS SKU, CAST(4.5 AS REAL) AS Rating, CAST(1000 AS BIGINT) AS ViewCount, 1 AS InStock; SELECT N'SingTC8out' AS Code, 8 AS [Rank], CAST(8.8 AS DECIMAL(18,6)) AS Balance, '2024-08-01' AS UpdatedOn; SELECT N'SingTC9out' AS Category, 1 AS IsPublished, CAST(40.5 AS FLOAT) AS Latitude, 3 AS Revision; SELECT N'SingTC10out' AS Tag, CAST(1000 AS BIGINT) AS Frequency, CAST(5 AS SMALLINT) AS [Priority], '2024-10-01' AS ExpiresOn; SELECT N'SingTC1out' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive; SELECT 20 AS [Count], '2024-02-01' AS OccuredOn, 0 AS IsActive, CAST(200 AS BIGINT) AS LargeNumber; SELECT N'SingTC3_2out' AS Title, 20 AS Quantity, CAST(2.5 AS DECIMAL(18,6)) AS Price, 0 AS IsEnabled; SELECT N'SingTC4_2out' AS Code, CAST(3.5 AS FLOAT) AS Weight, '2024-03-01' AS CreatedAt, CAST(300 AS BIGINT) AS Serial; SELECT N'SingTC5_2out' AS Label, 20 AS Score, 0 AS IsVerified, CAST(6 AS SMALLINT) AS [Level]; SELECT N'SingTC6_2out' AS MyLargeNumber, CAST(2000.5 AS DECIMAL(18,6)) AS Budget, '2024-06-01' AS StartDate, 600 AS [Population]; SELECT N'SingTC7_2out' AS SKU, CAST(5.5 AS REAL) AS Rating, CAST(2000 AS BIGINT) AS ViewCount, 0 AS InStock; SELECT N'SingTC8_2out' AS Code, 9 AS [Rank], CAST(9.9 AS DECIMAL(18,6)) AS Balance, '2024-09-01' AS UpdatedOn; SELECT N'SingTC9_2out' AS Category, 0 AS IsPublished, CAST(50.5 AS FLOAT) AS Latitude, 4 AS Revision; SELECT N'SingTC10_2out' AS Tag, CAST(2000 AS BIGINT) AS Frequency, CAST(6 AS SMALLINT) AS [Priority], '2024-11-01' AS ExpiresOn; SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'SingAIout' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration; END; GO

-- OR variants
CREATE OR ALTER PROCEDURE dbo.SingletonTest42 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC1orOut' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest43 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS HeaderId, N'SingHRorOut' AS Name, 10 AS [Count], N'trueout' AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest44 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingTC1orOut' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive;
    SELECT 1 AS Record4Id, 1 AS HeaderId, N'SingR4orOut' AS Name1, 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest45 AS BEGIN SET NOCOUNT ON;
    SELECT 1 AS AuditInfoId, 1 AS HeaderId, N'SingAIorOut' AS [Description], 10 AS [Count], 1 AS IsActive, CAST(100 AS BIGINT) AS LargeNumber, CAST(1.5 AS DECIMAL(18,6)) AS Ratio, 0x01 AS BinaryData, '2024-01-01' AS OccurredOn, CAST('2024-01-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:00:00' AS TIME) AS Duration;
END; GO

-- Inherited class SPs
CREATE OR ALTER PROCEDURE dbo.SingletonTest46 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingNameout' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive, CAST(9999 AS BIGINT) AS LargeNumber, 0x01 AS [Binary],
        CAST(1.23 AS DECIMAL(18,6)) AS Ratio, CAST(5 AS SMALLINT) AS SmallNumber, CAST(1 AS TINYINT) AS TinyNumber,
        CAST(1.1 AS REAL) AS LessPreciseFloat, CAST(2.2 AS FLOAT) AS DoublePrecision, N'X' AS SingleChar,
        CAST(1 AS SMALLINT) AS SByteValue, CAST(2 AS INT) AS UShortValue, CAST(3 AS BIGINT) AS UIntValue,
        CAST(4 AS DECIMAL(20,0)) AS ULongValue, CAST(5 AS BIGINT) AS NIntValue, CAST(6 AS DECIMAL(20,0)) AS NUIntValue,
        '2024-02-01' AS OccurredOn, '2024-03-01' AS OccurredOnExclude,
        CAST('2024-04-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:30:00' AS TIME) AS Duration,
        NEWID() AS UniqueId, 11 AS Count1, 1 AS IsActive1, CAST(3.45 AS DECIMAL(18,6)) AS Ratio1,
        CAST(6 AS SMALLINT) AS SmallNumber1, CAST(2 AS TINYINT) AS TinyNumber1,
        CAST(1.5 AS REAL) AS LessPreciseFloat1, CAST(2.5 AS FLOAT) AS DoublePrecision1, N'Y' AS SingleChar1,
        CAST(7 AS SMALLINT) AS NullableSByteValue, CAST(8 AS INT) AS NullableUShortValue,
        CAST(9 AS BIGINT) AS NullableUIntValue, CAST(10 AS DECIMAL(20,0)) AS NullableULongValue,
        CAST(11 AS BIGINT) AS NullableNIntValue, CAST(12 AS DECIMAL(20,0)) AS NullableNUIntValue,
        '2024-05-01' AS OccurredOn1, CAST('2024-06-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt1,
        CAST('02:00:00' AS TIME) AS Duration1, NEWID() AS UniqueId1,
        0 AS StatusType, 1 AS StatusType1, 0x02 AS BinaryData, N'SingABCout' AS CharArray, 0x03 AS BinaryData1, N'SingDEFout' AS CharArray1;
END; GO

CREATE OR ALTER PROCEDURE dbo.SingletonTest47 @MyName NVARCHAR(100), @Ratio DECIMAL(18,6), @BinaryData VARBINARY(MAX), @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7), @Record4Items dbo.Record4TableType READONLY, @AuditItems dbo.AuditInfoTableType READONLY AS BEGIN SET NOCOUNT ON;
    SELECT N'SingNameout' AS Name, 10 AS [Count], '2024-01-01' AS OccuredOn, 1 AS IsActive, CAST(9999 AS BIGINT) AS LargeNumber, 0x01 AS [Binary],
        CAST(1.23 AS DECIMAL(18,6)) AS Ratio, CAST(5 AS SMALLINT) AS SmallNumber, CAST(1 AS TINYINT) AS TinyNumber,
        CAST(1.1 AS REAL) AS LessPreciseFloat, CAST(2.2 AS FLOAT) AS DoublePrecision, N'X' AS SingleChar,
        CAST(1 AS SMALLINT) AS SByteValue, CAST(2 AS INT) AS UShortValue, CAST(3 AS BIGINT) AS UIntValue,
        CAST(4 AS DECIMAL(20,0)) AS ULongValue, CAST(5 AS BIGINT) AS NIntValue, CAST(6 AS DECIMAL(20,0)) AS NUIntValue,
        '2024-02-01' AS OccurredOn, '2024-03-01' AS OccurredOnExclude,
        CAST('2024-04-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt, CAST('01:30:00' AS TIME) AS Duration,
        NEWID() AS UniqueId, 11 AS Count1, 1 AS IsActive1, CAST(3.45 AS DECIMAL(18,6)) AS Ratio1,
        CAST(6 AS SMALLINT) AS SmallNumber1, CAST(2 AS TINYINT) AS TinyNumber1,
        CAST(1.5 AS REAL) AS LessPreciseFloat1, CAST(2.5 AS FLOAT) AS DoublePrecision1, N'Y' AS SingleChar1,
        CAST(7 AS SMALLINT) AS NullableSByteValue, CAST(8 AS INT) AS NullableUShortValue,
        CAST(9 AS BIGINT) AS NullableUIntValue, CAST(10 AS DECIMAL(20,0)) AS NullableULongValue,
        CAST(11 AS BIGINT) AS NullableNIntValue, CAST(12 AS DECIMAL(20,0)) AS NullableNUIntValue,
        '2024-05-01' AS OccurredOn1, CAST('2024-06-01 +00:00' AS DATETIMEOFFSET) AS OccurredAt1,
        CAST('02:00:00' AS TIME) AS Duration1, NEWID() AS UniqueId1,
        0 AS StatusType, 1 AS StatusType1, 0x02 AS BinaryData, N'SingABCout' AS CharArray, 0x03 AS BinaryData1, N'SingDEFout' AS CharArray1
    UNION ALL
    SELECT N'SingName2out', 20, '2024-02-01', 0, CAST(8888 AS BIGINT), 0x02,
        CAST(2.34 AS DECIMAL(18,6)), CAST(6 AS SMALLINT), CAST(2 AS TINYINT),
        CAST(2.1 AS REAL), CAST(3.2 AS FLOAT), N'Z',
        CAST(2 AS SMALLINT), CAST(3 AS INT), CAST(4 AS BIGINT),
        CAST(5 AS DECIMAL(20,0)), CAST(6 AS BIGINT), CAST(7 AS DECIMAL(20,0)),
        '2024-07-01', '2024-08-01',
        CAST('2024-09-01 +00:00' AS DATETIMEOFFSET), CAST('02:30:00' AS TIME),
        NEWID(), 22, 0, CAST(4.56 AS DECIMAL(18,6)),
        CAST(7 AS SMALLINT), CAST(3 AS TINYINT),
        CAST(2.5 AS REAL), CAST(3.5 AS FLOAT), N'W',
        CAST(8 AS SMALLINT), CAST(9 AS INT),
        CAST(10 AS BIGINT), CAST(11 AS DECIMAL(20,0)),
        CAST(12 AS BIGINT), CAST(13 AS DECIMAL(20,0)),
        '2024-10-01', CAST('2024-11-01 +00:00' AS DATETIMEOFFSET),
        CAST('03:00:00' AS TIME), NEWID(),
        1, 0, 0x04, N'SingGHIout', 0x05, N'SingJKLout';
END; GO
