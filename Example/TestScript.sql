IF TYPE_ID('dbo.InBaseTestClass2') IS NULL
BEGIN
    CREATE TYPE dbo.InBaseTestClass2 AS TABLE
    (
        Count6 INT, IsActive6 BIT, LargeNumber6 BIGINT, Ratio6 DECIMAL(18,6),
        Name6 SMALLINT, TinyNumber6 TINYINT, LessPreciseFloat6 REAL, DoublePrecision6 FLOAT,
        SingleChar6 NCHAR(1), SByteValue6 SMALLINT, UShortValue6 INT, UIntValue6 BIGINT,
        ULongValue6 DECIMAL(20,0), NIntValue6 BIGINT, NUIntValue6 DECIMAL(20,0),
        OccurredOn6 DATETIME, OccurredAt6 DATETIMEOFFSET(7), Duration6 TIME(7),
        UniqueId6 UNIQUEIDENTIFIER, StatusType6 NVARCHAR(255)
    );
END;
GO



CREATE OR ALTER PROCEDURE dbo.TransientTest1
    @Count5 INT,
    @IsActive5 BIT,
    @LargeNumber5 BIGINT,
    @Ratio5 DECIMAL(18,6),
    @Name5 SMALLINT,
    @TinyNumber5 TINYINT,
    @LessPreciseFloat5 REAL,
    @DoublePrecision5 FLOAT,
    @SingleChar5 NCHAR(1),
    @SByteValue5 SMALLINT,
    @UShortValue5 INT,
    @UIntValue5 BIGINT,
    @ULongValue5 DECIMAL(20,0),
    @NIntValue5 BIGINT,
    @NUIntValue5 DECIMAL(20,0),
    @OccurredOn5 DATETIME,
    @OccurredAt5 DATETIMEOFFSET(7),
    @Duration5 TIME(7),
    @UniqueId5 UNIQUEIDENTIFIER,
    @StatusType5 NVARCHAR(255),

    -- ?? InBaseTestClass non-nullable ??
    @Count INT,
    @IsActive BIT,
    @LargeNumber BIGINT,
    @Ratio DECIMAL(18,6),
    @Name SMALLINT,
    @TinyNumber TINYINT,
    @LessPreciseFloat REAL,
    @DoublePrecision FLOAT,
    @SingleChar NCHAR(1),
    @SByteValue SMALLINT,
    @UShortValue INT,
    @UIntValue BIGINT,
    @ULongValue DECIMAL(20,0),
    @NIntValue BIGINT,
    @NUIntValue DECIMAL(20,0),
    @OccurredOn DATETIME,
    @OccurredAt DATETIMEOFFSET(7),
    @Duration TIME(7),
    @UniqueId UNIQUEIDENTIFIER,
    @BinaryData VARBINARY(MAX),
    @CharArray NVARCHAR(MAX),
    @StatusType NVARCHAR(255),

    -- ?? TVP: NameList ??
    @NameList dbo.InBaseTestClass2 READONLY,

    -- ?? Output: Count1 ??
    @Count1DBParam INT OUTPUT,

    -- ?? Nullable (suffix 1) ??
    @IsActive1 BIT               = NULL,
    @LargeNumber1 BIGINT         = NULL,
    @Ratio1 DECIMAL(18,6)        = NULL,
    @Name1 SMALLINT              = NULL,
    @TinyNumber1 TINYINT         = NULL,
    @LessPreciseFloat1 REAL      = NULL,
    @DoublePrecision1 FLOAT      = NULL,
    @SingleChar1 NCHAR(1)        = NULL,
    @SByteValue1 SMALLINT        = NULL,
    @UShortValue1 INT            = NULL,
    @UIntValue1 BIGINT           = NULL,
    @ULongValue1 DECIMAL(20,0)   = NULL,
    @NIntValue1 BIGINT           = NULL,
    @NUIntValue1 DECIMAL(20,0)   = NULL,
    @OccurredOn1 DATETIME        = NULL,
    @OccurredAt1 DATETIMEOFFSET(7) = NULL,
    @Duration1 TIME(7)           = NULL,
    @UniqueId1 UNIQUEIDENTIFIER  = NULL,
    @BinaryData1 VARBINARY(MAX)  = NULL,
    @CharArray1 NVARCHAR(MAX)    = NULL,
    @StatusType1 NVARCHAR(255)   = NULL,

    -- ?? TVP: NameList1 ??
    @NameList1 dbo.InBaseTestClass2 READONLY,

    -- ?? Output params (suffix 2, DBParam names) ??
    @Count2DBParam INT OUTPUT,
    @IsActive2DBParam BIT OUTPUT,
    @LargeNumber2DBParam BIGINT OUTPUT,
    @Ratio2DBParam DECIMAL(18,6) OUTPUT,
    @Name2DBParam SMALLINT OUTPUT,
    @TinyNumber2DBParam TINYINT OUTPUT,
    @LessPreciseFloat2DBParam REAL OUTPUT,
    @DoublePrecision2DBParam FLOAT OUTPUT,
    @SingleChar2DBParam NCHAR(1) OUTPUT,
    @SByteValue2DBParam SMALLINT OUTPUT,
    @UShortValue2DBParam INT OUTPUT,
    @UIntValue2DBParam BIGINT OUTPUT,
    @ULongValue2DBParam DECIMAL(20,0) OUTPUT,
    @NIntValue2DBParam BIGINT OUTPUT,
    @NUIntValue2DBParam DECIMAL(20,0) OUTPUT,
    @OccurredOn2DBParam DATETIME OUTPUT,
    @OccurredAt2DBParam DATETIMEOFFSET(7) OUTPUT,
    @Duration2DBParam TIME(7) OUTPUT,
    @UniqueId2DBParam UNIQUEIDENTIFIER OUTPUT,
    @BinaryData2DBParam VARBINARY(MAX) OUTPUT,
    @CharArray2DBParam NVARCHAR(MAX) OUTPUT,
    @StatusType2DBParam NVARCHAR(255) OUTPUT,

    -- ?? Override params (suffix 4, mapped via OverrideXxx DBParam) ??
    @OverrideCount4 INT,
    @OverrideIsActive4 BIT,
    @OverrideLargeNumber4 BIGINT,
    @OverrideRatio4 DECIMAL(18,6),
    @OverrideName14 SMALLINT,
    @OverrideTinyNumber4 TINYINT,
    @OverrideLessPreciseFloat4 REAL,
    @OverrideDoublePrecision4 FLOAT,
    @OverrideSingleChar4 NCHAR(1),
    @OverrideSByteValue4 SMALLINT,
    @OverrideUShortValue4 INT,
    @OverrideUIntValue4 BIGINT,
    @OverrideULongValue4 DECIMAL(20,0),
    @OverrideNIntValue4 BIGINT,
    @OverrideNUIntValue4 DECIMAL(20,0),
    @OverrideOccurredOn4 DATETIME,
    @OverrideOccurredAt4 DATETIMEOFFSET(7),
    @OverrideDuration4 TIME(7),
    @OverrideUniqueId4 UNIQUEIDENTIFIER,
    @OverrideStatusType4 NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- ?? Set output parameters ??
    SET @Count1DBParam          = 2147483647;
    SET @Count2DBParam          = 2147483647;
    SET @IsActive2DBParam       = 1;
    SET @LargeNumber2DBParam    = CAST(9223372036854775807 AS BIGINT);
    SET @Ratio2DBParam          = CAST(999999999999.999999 AS DECIMAL(18,6));
    SET @Name2DBParam           = CAST(32767 AS SMALLINT);
    SET @TinyNumber2DBParam     = CAST(255 AS TINYINT);
    SET @LessPreciseFloat2DBParam = CAST(3.4028235E+38 AS REAL);
    SET @DoublePrecision2DBParam  = CAST(1.7976931348623157E+308 AS FLOAT);
    SET @SingleChar2DBParam     = N'Z';
    SET @SByteValue2DBParam     = CAST(127 AS SMALLINT);
    SET @UShortValue2DBParam    = 65535;
    SET @UIntValue2DBParam      = CAST(4294967295 AS BIGINT);
    SET @ULongValue2DBParam     = CAST(18446744073709551615 AS DECIMAL(20,0));
    SET @NIntValue2DBParam      = CAST(9223372036854775807 AS BIGINT);
    SET @NUIntValue2DBParam     = CAST(18446744073709551615 AS DECIMAL(20,0));
    SET @OccurredOn2DBParam     = CAST('9999-12-31T23:59:59.997' AS DATETIME);
    SET @OccurredAt2DBParam     = CAST('9999-12-31 23:59:59.9999999 +00:00' AS DATETIMEOFFSET);
    SET @Duration2DBParam       = CAST('23:59:59.9999999' AS TIME);
    SET @UniqueId2DBParam       = CAST('ffffffff-ffff-ffff-ffff-ffffffffffff' AS UNIQUEIDENTIFIER);
    SET @BinaryData2DBParam     = 0xFFFFFF;
    SET @CharArray2DBParam      = N'ZZZ';
    SET @StatusType2DBParam     = N'Active';

    -- ?? Single result set: OutBaseTestClass hierarchy ??
    -- Column order: OutBaseTestClass2 ? OutBaseTestClass1 ? OutBaseTestClass
    -- (ResultExclusion=true columns are omitted)
    SELECT
        -- OutBaseTestClass2 (suffix 6)
        CAST(2147483647 AS INT) AS Count6,
        CAST(1 AS BIT) AS IsActive6,
        CAST(9223372036854775807 AS BIGINT) AS LargeNumber6,
        CAST(999999999999.999999 AS DECIMAL(18,6)) AS Ratio6,
        CAST(32767 AS SMALLINT) AS Name6,
        CAST(255 AS TINYINT) AS TinyNumber6,
        CAST(3.4028235E+38 AS REAL) AS LessPreciseFloat6,
        CAST(1.7976931348623157E+308 AS FLOAT) AS DoublePrecision6,
        N'Z' AS SingleChar6,
        CAST(127 AS SMALLINT) AS SByteValue6,
        CAST(65535 AS INT) AS UShortValue6,
        CAST(4294967295 AS BIGINT) AS UIntValue6,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS ULongValue6,
        CAST(9223372036854775807 AS BIGINT) AS NIntValue6,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS NUIntValue6,
        CAST('9999-12-31T23:59:59.997' AS DATETIME) AS OccurredOn6,
        CAST('9999-12-31 23:59:59.9999999 +00:00' AS DATETIMEOFFSET) AS OccurredAt6,
        CAST('23:59:59.9999999' AS TIME) AS Duration6,
        CAST('ffffffff-ffff-ffff-ffff-ffffffffffff' AS UNIQUEIDENTIFIER) AS UniqueId6,
        N'Active' AS StatusType6,

        -- OutBaseTestClass1 (suffix 5)
        CAST(2147483647 AS INT) AS Count5,
        CAST(1 AS BIT) AS IsActive5,
        CAST(9223372036854775807 AS BIGINT) AS LargeNumber5,
        CAST(999999999999.999999 AS DECIMAL(18,6)) AS Ratio5,
        CAST(32767 AS SMALLINT) AS Name5,
        CAST(255 AS TINYINT) AS TinyNumber5,
        CAST(3.4028235E+38 AS REAL) AS LessPreciseFloat5,
        CAST(1.7976931348623157E+308 AS FLOAT) AS DoublePrecision5,
        N'Z' AS SingleChar5,
        CAST(127 AS SMALLINT) AS SByteValue5,
        CAST(65535 AS INT) AS UShortValue5,
        CAST(4294967295 AS BIGINT) AS UIntValue5,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS ULongValue5,
        CAST(9223372036854775807 AS BIGINT) AS NIntValue5,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS NUIntValue5,
        CAST('9999-12-31T23:59:59.997' AS DATETIME) AS OccurredOn5,
        CAST('9999-12-31 23:59:59.9999999 +00:00' AS DATETIMEOFFSET) AS OccurredAt5,
        CAST('23:59:59.9999999' AS TIME) AS Duration5,
        CAST('ffffffff-ffff-ffff-ffff-ffffffffffff' AS UNIQUEIDENTIFIER) AS UniqueId5,
        N'Active' AS StatusType5,

        -- OutBaseTestClass non-nullable
        CAST(2147483647 AS INT) AS [Count],
        CAST(1 AS BIT) AS IsActive,
        CAST(9223372036854775807 AS BIGINT) AS LargeNumber,
        CAST(999999999999.999999 AS DECIMAL(18,6)) AS Ratio,
        CAST(32767 AS SMALLINT) AS Name,
        CAST(255 AS TINYINT) AS TinyNumber,
        CAST(3.4028235E+38 AS REAL) AS LessPreciseFloat,
        CAST(1.7976931348623157E+308 AS FLOAT) AS DoublePrecision,
        N'Z' AS SingleChar,
        CAST(127 AS SMALLINT) AS SByteValue,
        CAST(65535 AS INT) AS UShortValue,
        CAST(4294967295 AS BIGINT) AS UIntValue,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS ULongValue,
        CAST(9223372036854775807 AS BIGINT) AS NIntValue,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS NUIntValue,
        CAST('9999-12-31T23:59:59.997' AS DATETIME) AS OccurredOn,
        CAST('9999-12-31 23:59:59.9999999 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('23:59:59.9999999' AS TIME) AS Duration,
        CAST('ffffffff-ffff-ffff-ffff-ffffffffffff' AS UNIQUEIDENTIFIER) AS UniqueId,
        0xFFFFFF AS BinaryData,
        N'ZZZ' AS CharArray,
        N'Active' AS StatusType,

        -- Nullable (suffix 1)
        CAST(2147483647 AS INT) AS Count1,
        CAST(1 AS BIT) AS IsActive1,
        CAST(9223372036854775807 AS BIGINT) AS LargeNumber1,
        CAST(999999999999.999999 AS DECIMAL(18,6)) AS Ratio1,
        CAST(32767 AS SMALLINT) AS Name1,
        CAST(255 AS TINYINT) AS TinyNumber1,
        CAST(3.4028235E+38 AS REAL) AS LessPreciseFloat1,
        CAST(1.7976931348623157E+308 AS FLOAT) AS DoublePrecision1,
        N'Z' AS SingleChar1,
        CAST(127 AS SMALLINT) AS SByteValue1,
        CAST(65535 AS INT) AS UShortValue1,
        CAST(4294967295 AS BIGINT) AS UIntValue1,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS ULongValue1,
        CAST(9223372036854775807 AS BIGINT) AS NIntValue1,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS NUIntValue1,
        CAST('9999-12-31T23:59:59.997' AS DATETIME) AS OccurredOn1,
        CAST('9999-12-31 23:59:59.9999999 +00:00' AS DATETIMEOFFSET) AS OccurredAt1,
        CAST('23:59:59.9999999' AS TIME) AS Duration1,
        CAST('ffffffff-ffff-ffff-ffff-ffffffffffff' AS UNIQUEIDENTIFIER) AS UniqueId1,
        0xFFFF AS BinaryData1,
        N'ZZ' AS CharArray1,
        N'Active' AS StatusType1,

        -- DBParam-mapped (suffix 2)
        CAST(2147483647 AS INT) AS Count2DBParam,
        CAST(1 AS BIT) AS IsActive2DBParam,
        CAST(9223372036854775807 AS BIGINT) AS LargeNumber2DBParam,
        CAST(999999999999.999999 AS DECIMAL(18,6)) AS Ratio2DBParam,
        CAST(32767 AS SMALLINT) AS Name2DBParam,
        CAST(255 AS TINYINT) AS TinyNumber2DBParam,
        CAST(3.4028235E+38 AS REAL) AS LessPreciseFloat2DBParam,
        CAST(1.7976931348623157E+308 AS FLOAT) AS DoublePrecision2DBParam,
        N'Z' AS SingleChar2DBParam,
        CAST(127 AS SMALLINT) AS SByteValue2DBParam,
        CAST(65535 AS INT) AS UShortValue2DBParam,
        CAST(4294967295 AS BIGINT) AS UIntValue2DBParam,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS ULongValue2DBParam,
        CAST(9223372036854775807 AS BIGINT) AS NIntValue2DBParam,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS NUIntValue2DBParam,
        CAST('9999-12-31T23:59:59.997' AS DATETIME) AS OccurredOn2DBParam,
        CAST('9999-12-31 23:59:59.9999999 +00:00' AS DATETIMEOFFSET) AS OccurredAt2DBParam,
        CAST('23:59:59.9999999' AS TIME) AS Duration2DBParam,
        CAST('ffffffff-ffff-ffff-ffff-ffffffffffff' AS UNIQUEIDENTIFIER) AS UniqueId2DBParam,
        0xFFFFFF AS BinaryData2DBParam,
        N'ZZZ' AS CharArray2DBParam,
        N'Active' AS StatusType2DBParam,

        -- Override (suffix 4)
        CAST(2147483647 AS INT) AS OverrideCount4,
        CAST(1 AS BIT) AS OverrideIsActive4,
        CAST(9223372036854775807 AS BIGINT) AS OverrideLargeNumber4,
        CAST(999999999999.999999 AS DECIMAL(18,6)) AS OverrideRatio4,
        CAST(32767 AS SMALLINT) AS OverrideName14,
        CAST(255 AS TINYINT) AS OverrideTinyNumber4,
        CAST(3.4028235E+38 AS REAL) AS OverrideLessPreciseFloat4,
        CAST(1.7976931348623157E+308 AS FLOAT) AS OverrideDoublePrecision4,
        N'Z' AS OverrideSingleChar4,
        CAST(127 AS SMALLINT) AS OverrideSByteValue4,
        CAST(65535 AS INT) AS OverrideUShortValue4,
        CAST(4294967295 AS BIGINT) AS OverrideUIntValue4,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS OverrideULongValue4,
        CAST(9223372036854775807 AS BIGINT) AS OverrideNIntValue4,
        CAST(18446744073709551615 AS DECIMAL(20,0)) AS OverrideNUIntValue4,
        CAST('9999-12-31T23:59:59.997' AS DATETIME) AS OverrideOccurredOn4,
        CAST('9999-12-31 23:59:59.9999999 +00:00' AS DATETIMEOFFSET) AS OverrideOccurredAt4,
        CAST('23:59:59.9999999' AS TIME) AS OverrideDuration4,
        CAST('ffffffff-ffff-ffff-ffff-ffffffffffff' AS UNIQUEIDENTIFIER) AS OverrideUniqueId4,
        N'Active' AS OverrideStatusType4;
END;
GO


CREATE OR ALTER PROCEDURE dbo.TransientTest2
    @HeaderIdDBParam INT,
    @Name NVARCHAR(MAX)         = NULL,
    @MyName NVARCHAR(MAX)       = NULL,
    @Count INT,
    @IsActive NVARCHAR(MAX)     = NULL,
    @LargeNumber BIGINT,
    @Ratio DECIMAL(18,6),
    @BinaryData VARBINARY(MAX)  = NULL,
    @OccurredOn DATETIME,
    @OccurredAt DATETIMEOFFSET(7),
    @Duration TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    -- Return a list of TestClass rows (inherits HeaderResult1)
    -- Columns: CustomeName (maps to HeaderId), Name, MyName, Count, IsActive, LargeNumber, Ratio, BinaryData, OccurredOn, OccurredAt, Duration
    SELECT
        CAST(2147483647 AS INT) AS CustomeName,
        N'MaxTestName' AS Name,
        N'MaxMyName' AS MyName,
        CAST(2147483647 AS INT) AS [Count],
        N'true' AS IsActive,
        CAST(9223372036854775807 AS BIGINT) AS LargeNumber,
        CAST(999999999999.999999 AS DECIMAL(18,6)) AS Ratio,
        0xFFFFFF AS BinaryData,
        CAST('9999-12-31T23:59:59.997' AS DATETIME) AS OccurredOn,
        CAST('9999-12-31 23:59:59.9999999 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('23:59:59.9999999' AS TIME) AS Duration
    UNION ALL
    SELECT
        CAST(2147483647 AS INT) AS CustomeName,
        N'MaxTestName2' AS Name,
        N'MaxMyName2' AS MyName,
        CAST(2147483647 AS INT) AS [Count],
        N'true' AS IsActive,
        CAST(9223372036854775807 AS BIGINT) AS LargeNumber,
        CAST(999999999999.999999 AS DECIMAL(18,6)) AS Ratio,
        0xFFFFFF AS BinaryData,
        CAST('9999-12-31T23:59:59.997' AS DATETIME) AS OccurredOn,
        CAST('9999-12-31 23:59:59.9999999 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('23:59:59.9999999' AS TIME) AS Duration;
END;
GO

-- SP for Test3_With_TupleContainingSingle
-- Returns 2 result sets: TestClass (single row), HeaderResult1 (single row)
-- No input parameters
CREATE OR ALTER PROCEDURE dbo.TransientTest3
AS
BEGIN
    SET NOCOUNT ON;

    -- Result set 1: TestClass (inherits HeaderResult1, adds Name from CustomeName, MyName)
    SELECT
        CAST(100 AS INT) AS CustomeName,
        N'TestName1' AS Name,
        N'TestMyName1' AS MyName,
        CAST(50 AS INT) AS [Count],
        N'true' AS IsActive,
        CAST(123456789 AS BIGINT) AS LargeNumber,
        CAST(99.99 AS DECIMAL(18,6)) AS Ratio,
        0xAABBCC AS BinaryData,
        CAST('2025-01-15' AS DATETIME) AS OccurredOn,
        CAST('2025-01-15 10:30:00.0000000 +05:30' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('02:30:00' AS TIME) AS Duration;

    -- Result set 2: HeaderResult1
    SELECT
        CAST(200 AS INT) AS CustomeName,
        N'HeaderName1' AS Name,
        N'HeaderMyName1' AS MyName,
        CAST(75 AS INT) AS [Count],
        N'false' AS IsActive,
        CAST(987654321 AS BIGINT) AS LargeNumber,
        CAST(55.55 AS DECIMAL(18,6)) AS Ratio,
        0xDDEEFF AS BinaryData,
        CAST('2025-06-20' AS DATETIME) AS OccurredOn,
        CAST('2025-06-20 14:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('05:45:00' AS TIME) AS Duration;
END;
GO

-- SP for Test4_With_TupleContaing2List
-- Input: TestClass111 parameters
-- Returns 2 result sets: List<TestClass> (multiple rows), List<HeaderResult1> (multiple rows)
CREATE OR ALTER PROCEDURE dbo.TransientTest4
    @HeaderIdDBParam INT,
    @Name NVARCHAR(MAX)         = NULL,
    @MyName NVARCHAR(MAX)       = NULL,
    @Count INT,
    @IsActive NVARCHAR(MAX)     = NULL,
    @LargeNumber BIGINT,
    @Ratio DECIMAL(18,6),
    @BinaryData VARBINARY(MAX)  = NULL,
    @OccurredOn DATETIME,
    @OccurredAt DATETIMEOFFSET(7),
    @Duration TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    -- Result set 1: List<TestClass> (2 rows)
    SELECT
        CAST(1 AS INT) AS CustomeName,
        N'Name_Row1' AS Name,
        N'MyName_Row1' AS MyName,
        CAST(10 AS INT) AS [Count],
        N'true' AS IsActive,
        CAST(1000 AS BIGINT) AS LargeNumber,
        CAST(10.50 AS DECIMAL(18,6)) AS Ratio,
        0xAA AS BinaryData,
        CAST('2025-01-01' AS DATETIME) AS OccurredOn,
        CAST('2025-01-01 08:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('01:00:00' AS TIME) AS Duration
    UNION ALL
    SELECT
        CAST(2 AS INT) AS CustomeName,
        N'Name_Row2' AS Name,
        N'MyName_Row2' AS MyName,
        CAST(20 AS INT) AS [Count],
        N'false' AS IsActive,
        CAST(2000 AS BIGINT) AS LargeNumber,
        CAST(20.75 AS DECIMAL(18,6)) AS Ratio,
        0xBB AS BinaryData,
        CAST('2025-02-01' AS DATETIME) AS OccurredOn,
        CAST('2025-02-01 09:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('02:00:00' AS TIME) AS Duration;

    -- Result set 2: List<HeaderResult1> (2 rows)
    SELECT
        CAST(101 AS INT) AS CustomeName,
        N'Header_Row1' AS Name,
        N'HeaderMy_Row1' AS MyName,
        CAST(30 AS INT) AS [Count],
        N'true' AS IsActive,
        CAST(3000 AS BIGINT) AS LargeNumber,
        CAST(30.25 AS DECIMAL(18,6)) AS Ratio,
        0xCC AS BinaryData,
        CAST('2025-03-01' AS DATETIME) AS OccurredOn,
        CAST('2025-03-01 10:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('03:00:00' AS TIME) AS Duration
    UNION ALL
    SELECT
        CAST(102 AS INT) AS CustomeName,
        N'Header_Row2' AS Name,
        N'HeaderMy_Row2' AS MyName,
        CAST(40 AS INT) AS [Count],
        N'false' AS IsActive,
        CAST(4000 AS BIGINT) AS LargeNumber,
        CAST(40.50 AS DECIMAL(18,6)) AS Ratio,
        0xDD AS BinaryData,
        CAST('2025-04-01' AS DATETIME) AS OccurredOn,
        CAST('2025-04-01 11:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('04:00:00' AS TIME) AS Duration;
END;
GO

-- SP for Test5_With_TupleContaingSingleNList
-- Input: TestClass111 parameters
-- Returns 2 result sets: List<HeaderResult1> (multiple rows), TestClass (single row)
CREATE OR ALTER PROCEDURE dbo.TransientTest5
    @HeaderIdDBParam INT,
    @Name NVARCHAR(MAX)         = NULL,
    @MyName NVARCHAR(MAX)       = NULL,
    @Count INT,
    @IsActive NVARCHAR(MAX)     = NULL,
    @LargeNumber BIGINT,
    @Ratio DECIMAL(18,6),
    @BinaryData VARBINARY(MAX)  = NULL,
    @OccurredOn DATETIME,
    @OccurredAt DATETIMEOFFSET(7),
    @Duration TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    -- Result set 1: List<HeaderResult1> (2 rows)
    SELECT
        CAST(301 AS INT) AS CustomeName,
        N'HdrName_R1' AS Name,
        N'HdrMyName_R1' AS MyName,
        CAST(11 AS INT) AS [Count],
        N'true' AS IsActive,
        CAST(1111 AS BIGINT) AS LargeNumber,
        CAST(11.11 AS DECIMAL(18,6)) AS Ratio,
        0xA1 AS BinaryData,
        CAST('2025-07-01' AS DATETIME) AS OccurredOn,
        CAST('2025-07-01 06:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('01:11:00' AS TIME) AS Duration
    UNION ALL
    SELECT
        CAST(302 AS INT) AS CustomeName,
        N'HdrName_R2' AS Name,
        N'HdrMyName_R2' AS MyName,
        CAST(22 AS INT) AS [Count],
        N'false' AS IsActive,
        CAST(2222 AS BIGINT) AS LargeNumber,
        CAST(22.22 AS DECIMAL(18,6)) AS Ratio,
        0xB2 AS BinaryData,
        CAST('2025-08-01' AS DATETIME) AS OccurredOn,
        CAST('2025-08-01 07:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('02:22:00' AS TIME) AS Duration;

    -- Result set 2: TestClass (single row)
    SELECT
        CAST(500 AS INT) AS CustomeName,
        N'SingleName' AS Name,
        N'SingleMyName' AS MyName,
        CAST(99 AS INT) AS [Count],
        N'true' AS IsActive,
        CAST(9999 AS BIGINT) AS LargeNumber,
        CAST(88.88 AS DECIMAL(18,6)) AS Ratio,
        0xEEFF AS BinaryData,
        CAST('2025-12-25' AS DATETIME) AS OccurredOn,
        CAST('2025-12-25 18:30:00.0000000 +05:30' AS DATETIMEOFFSET) AS OccurredAt,
        CAST('06:30:00' AS TIME) AS Duration;
END;
GO

-- SP for Test6_With_Tuple_N_Single
-- Input: TestClass111 parameters (use @HeaderIdDBParam as condition: 1 = nested tuple, 2 = single)
-- @HeaderIdDBParam = 1 => 2 result sets: List<TestClass1> + HeaderResult1 => returns ((List<TestClass1>, HeaderResult1), null)
-- @HeaderIdDBParam = 2 => 1 result set:  List<TestClass1> (read as single) => returns (null, TestClass1)
CREATE OR ALTER PROCEDURE dbo.TransientTest6
    @HeaderIdDBParam INT,
    @Name NVARCHAR(MAX)         = NULL,
    @MyName NVARCHAR(MAX)       = NULL,
    @Count INT,
    @IsActive NVARCHAR(MAX)     = NULL,
    @LargeNumber BIGINT,
    @Ratio DECIMAL(18,6),
    @BinaryData VARBINARY(MAX)  = NULL,
    @OccurredOn DATETIME,
    @OccurredAt DATETIMEOFFSET(7),
    @Duration TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    IF @HeaderIdDBParam = 1
    BEGIN
        -- Result set 1: List<TestClass1> (2 rows)
        -- Columns: Name (string), Count (int), OccuredOn (datetime), IsActive (bit)
        SELECT
            N'Alice' AS Name,
            CAST(10 AS INT) AS [Count],
            CAST('2025-03-01' AS DATETIME) AS OccuredOn,
            CAST(1 AS BIT) AS IsActive
        UNION ALL
        SELECT
            N'Bob' AS Name,
            CAST(20 AS INT) AS [Count],
            CAST('2025-04-01' AS DATETIME) AS OccuredOn,
            CAST(0 AS BIT) AS IsActive;

        -- Result set 2: HeaderResult1 (single row)
        -- Columns: CustomeName (int), Name, MyName, Count, IsActive (string), LargeNumber, Ratio, BinaryData, OccurredOn, OccurredAt, Duration
        SELECT
            CAST(600 AS INT) AS CustomeName,
            N'HdrTuple6' AS Name,
            N'HdrMyTuple6' AS MyName,
            CAST(55 AS INT) AS [Count],
            N'true' AS IsActive,
            CAST(6666 AS BIGINT) AS LargeNumber,
            CAST(66.66 AS DECIMAL(18,6)) AS Ratio,
            0xABCD AS BinaryData,
            CAST('2025-06-15' AS DATETIME) AS OccurredOn,
            CAST('2025-06-15 12:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
            CAST('03:33:00' AS TIME) AS Duration;
    END
    ELSE
    BEGIN
        -- Result set 1 only: List<TestClass1> (1 row, read as single TestClass1)
        SELECT
            N'Charlie' AS Name,
            CAST(77 AS INT) AS [Count],
            CAST('2025-09-01' AS DATETIME) AS OccuredOn,
            CAST(1 AS BIT) AS IsActive;
    END
END;
GO

-- SP for Test7_With_Tuple_N_List
-- Input: TestClass111 parameters (use @HeaderIdDBParam as condition: 1 = nested tuple, 2 = list)
-- @HeaderIdDBParam = 1 => 2 result sets: HeaderResult1 + List<Record4Result> => returns ((HeaderResult1, List<Record4Result>), null)
-- @HeaderIdDBParam = 2 => 1 result set:  List<HeaderResult1>               => returns (null, List<HeaderResult1>)
CREATE OR ALTER PROCEDURE dbo.TransientTest7
    @HeaderIdDBParam INT,
    @Name NVARCHAR(MAX)         = NULL,
    @MyName NVARCHAR(MAX)       = NULL,
    @Count INT,
    @IsActive NVARCHAR(MAX)     = NULL,
    @LargeNumber BIGINT,
    @Ratio DECIMAL(18,6),
    @BinaryData VARBINARY(MAX)  = NULL,
    @OccurredOn DATETIME,
    @OccurredAt DATETIMEOFFSET(7),
    @Duration TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    IF @HeaderIdDBParam = 1
    BEGIN
        -- Result set 1: HeaderResult1 (1 row, used as single via FirstOrDefault)
        SELECT
            CAST(701 AS INT) AS CustomeName,
            N'Hdr7Name' AS Name,
            N'Hdr7MyName' AS MyName,
            CAST(70 AS INT) AS [Count],
            N'true' AS IsActive,
            CAST(7000 AS BIGINT) AS LargeNumber,
            CAST(70.70 AS DECIMAL(18,6)) AS Ratio,
            0xAA77 AS BinaryData,
            CAST('2025-07-07' AS DATETIME) AS OccurredOn,
            CAST('2025-07-07 07:07:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
            CAST('07:07:00' AS TIME) AS Duration;

        -- Result set 2: List<Record4Result> (2 rows)
        -- Columns: Record4Id (int), HeaderId (int), Name1 (string), Count (int), IsActive (bit), LargeNumber (bigint)
        SELECT
            CAST(1 AS INT) AS Record4Id,
            CAST(701 AS INT) AS HeaderId,
            N'Rec4_Row1' AS Name1,
            CAST(11 AS INT) AS [Count],
            CAST(1 AS BIT) AS IsActive,
            CAST(1100 AS BIGINT) AS LargeNumber
        UNION ALL
        SELECT
            CAST(2 AS INT) AS Record4Id,
            CAST(701 AS INT) AS HeaderId,
            N'Rec4_Row2' AS Name1,
            CAST(22 AS INT) AS [Count],
            CAST(0 AS BIT) AS IsActive,
            CAST(2200 AS BIGINT) AS LargeNumber;
    END
    ELSE
    BEGIN
        -- Result set 1 only: List<HeaderResult1> (2 rows)
        SELECT
            CAST(801 AS INT) AS CustomeName,
            N'ListHdr_R1' AS Name,
            N'ListHdrMy_R1' AS MyName,
            CAST(80 AS INT) AS [Count],
            N'true' AS IsActive,
            CAST(8000 AS BIGINT) AS LargeNumber,
            CAST(80.80 AS DECIMAL(18,6)) AS Ratio,
            0xBB88 AS BinaryData,
            CAST('2025-08-08' AS DATETIME) AS OccurredOn,
            CAST('2025-08-08 08:08:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
            CAST('08:08:00' AS TIME) AS Duration
        UNION ALL
        SELECT
            CAST(802 AS INT) AS CustomeName,
            N'ListHdr_R2' AS Name,
            N'ListHdrMy_R2' AS MyName,
            CAST(90 AS INT) AS [Count],
            N'false' AS IsActive,
            CAST(9000 AS BIGINT) AS LargeNumber,
            CAST(90.90 AS DECIMAL(18,6)) AS Ratio,
            0xCC99 AS BinaryData,
            CAST('2025-09-09' AS DATETIME) AS OccurredOn,
            CAST('2025-09-09 09:09:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
            CAST('09:09:00' AS TIME) AS Duration;
    END
END;
GO

-- SP for Test8_With_Tuple_Single_List
-- Input: TestClass111 parameters (use @HeaderIdDBParam as condition: 1 = tuple1, 2 = tuple2, 3 = tuple3)
-- @HeaderIdDBParam = 1 => 2 result sets: List<TestClass1> + TestClass2       => returns ((List<TestClass1>, TestClass2), null, null)
-- @HeaderIdDBParam = 2 => 4 result sets: List<TestClass1> + TestClass2 + List<TestClass3> + TestClass8 => returns (null, (List<TestClass1>, TestClass2, List<TestClass3>, TestClass8), null)
-- @HeaderIdDBParam = 3 => 4 result sets: List<TestClass1> + TestClass10 + List<TestClass11> + TestClass12 => returns (null, null, (List<TestClass1>, TestClass10, List<TestClass11>, TestClass12))
CREATE OR ALTER PROCEDURE dbo.TransientTest8
    @HeaderIdDBParam INT,
    @Name NVARCHAR(MAX)         = NULL,
    @MyName NVARCHAR(MAX)       = NULL,
    @Count INT,
    @IsActive NVARCHAR(MAX)     = NULL,
    @LargeNumber BIGINT,
    @Ratio DECIMAL(18,6),
    @BinaryData VARBINARY(MAX)  = NULL,
    @OccurredOn DATETIME,
    @OccurredAt DATETIMEOFFSET(7),
    @Duration TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    IF @HeaderIdDBParam = 1
    BEGIN
        -- Tuple Item1: (List<TestClass1>, TestClass2)

        -- Result set 1: List<TestClass1> (2 rows)
        -- Columns: Name (string), Count (int), OccuredOn (datetime, Unique), IsActive (bit)
        SELECT
            N'T1_Alice' AS Name,
            CAST(10 AS INT) AS [Count],
            CAST('2025-01-10' AS DATETIME) AS OccuredOn,
            CAST(1 AS BIT) AS IsActive
        UNION ALL
        SELECT
            N'T1_Bob' AS Name,
            CAST(20 AS INT) AS [Count],
            CAST('2025-01-20' AS DATETIME) AS OccuredOn,
            CAST(0 AS BIT) AS IsActive;

        -- Result set 2: TestClass2 (single row)
        -- Columns: Count (int), OccuredOn (datetime), IsActive (bit), LargeNumber (bigint, Unique)
        SELECT
            CAST(100 AS INT) AS [Count],
            CAST('2025-02-15' AS DATETIME) AS OccuredOn,
            CAST(1 AS BIT) AS IsActive,
            CAST(100000 AS BIGINT) AS LargeNumber;
    END
    ELSE IF @HeaderIdDBParam = 2
    BEGIN
        -- Tuple Item2: (List<TestClass1>, TestClass2, List<TestClass3>, TestClass8)

        -- Result set 1: List<TestClass1> (2 rows)
        SELECT
            N'T2_Charlie' AS Name,
            CAST(30 AS INT) AS [Count],
            CAST('2025-03-01' AS DATETIME) AS OccuredOn,
            CAST(1 AS BIT) AS IsActive
        UNION ALL
        SELECT
            N'T2_Diana' AS Name,
            CAST(40 AS INT) AS [Count],
            CAST('2025-03-15' AS DATETIME) AS OccuredOn,
            CAST(0 AS BIT) AS IsActive;

        -- Result set 2: TestClass2 (single row)
        SELECT
            CAST(200 AS INT) AS [Count],
            CAST('2025-04-01' AS DATETIME) AS OccuredOn,
            CAST(0 AS BIT) AS IsActive,
            CAST(200000 AS BIGINT) AS LargeNumber;

        -- Result set 3: List<TestClass3> (2 rows)
        -- Columns: Title (string, Unique), Quantity (int), Price (decimal), IsEnabled (bit)
        SELECT
            N'Widget_A' AS Title,
            CAST(50 AS INT) AS Quantity,
            CAST(19.99 AS DECIMAL(18,6)) AS Price,
            CAST(1 AS BIT) AS IsEnabled
        UNION ALL
        SELECT
            N'Widget_B' AS Title,
            CAST(75 AS INT) AS Quantity,
            CAST(29.99 AS DECIMAL(18,6)) AS Price,
            CAST(0 AS BIT) AS IsEnabled;

        -- Result set 4: TestClass8 (single row)
        -- Columns: CodeDBParam (string, Unique), Rank (int), Balance (decimal), UpdatedOn (datetime)
        SELECT
            N'CODE_T8_01' AS CodeDBParam,
            CAST(1 AS INT) AS [Rank],
            CAST(5000.50 AS DECIMAL(18,6)) AS Balance,
            CAST('2025-04-20' AS DATETIME) AS UpdatedOn;
    END
    ELSE
    BEGIN
        -- Tuple Item3: (List<TestClass1>, TestClass10, List<TestClass11>, TestClass12)

        -- Result set 1: List<TestClass1> (2 rows)
        SELECT
            N'T3_Eve' AS Name,
            CAST(55 AS INT) AS [Count],
            CAST('2025-05-01' AS DATETIME) AS OccuredOn,
            CAST(1 AS BIT) AS IsActive
        UNION ALL
        SELECT
            N'T3_Frank' AS Name,
            CAST(65 AS INT) AS [Count],
            CAST('2025-05-15' AS DATETIME) AS OccuredOn,
            CAST(0 AS BIT) AS IsActive;

        -- Result set 2: TestClass10 (single row)
        -- Columns: Tag (string, Unique), Frequency (bigint), Priority (smallint), ExpiresOn (datetime)
        SELECT
            N'TAG_10_01' AS Tag,
            CAST(999999 AS BIGINT) AS Frequency,
            CAST(5 AS SMALLINT) AS [Priority],
            CAST('2025-12-31' AS DATETIME) AS ExpiresOn;

        -- Result set 3: List<TestClass11> (2 rows)
        -- Columns: Vendor (string), Units (int), Discount (decimal), IsApproved (bit)
        SELECT
            N'Vendor_X' AS Vendor,
            CAST(100 AS INT) AS Units,
            CAST(15.50 AS DECIMAL(18,6)) AS Discount,
            CAST(1 AS BIT) AS IsApproved
        UNION ALL
        SELECT
            N'Vendor_Y' AS Vendor,
            CAST(200 AS INT) AS Units,
            CAST(25.75 AS DECIMAL(18,6)) AS Discount,
            CAST(0 AS BIT) AS IsApproved;

        -- Result set 4: TestClass12 (single row)
        -- Columns: Channel (string), Bandwidth (float), RegisteredOn (datetime), Throughput (bigint)
        SELECT
            N'Channel_Alpha' AS Channel,
            CAST(500.75 AS FLOAT) AS Bandwidth,
            CAST('2025-06-01' AS DATETIME) AS RegisteredOn,
            CAST(1000000 AS BIGINT) AS Throughput;
    END
END;
GO

-- SP for Test9_With_2Tuple_Single
-- Input: TestClass111 parameters (use @HeaderIdDBParam as condition: 1 = tuple1, 2 = tuple2, 3 = list)
-- @HeaderIdDBParam = 1 => 2 result sets: List<HeaderResult1> + TestClass1           => returns ((List<HeaderResult1>, TestClass1), null, null-list)
-- @HeaderIdDBParam = 2 => 3 result sets: List<HeaderResult1> + List<Record4Result> + TestClass1 => returns (null, (List<HeaderResult1>, List<Record4Result>, TestClass1), null-list)
-- @HeaderIdDBParam = 3 => 1 result set:  List<HeaderResult1>                        => returns (null, null, List<HeaderResult1>)
CREATE OR ALTER PROCEDURE dbo.TransientTest9
    @HeaderIdDBParam INT,
    @Name NVARCHAR(MAX)         = NULL,
    @MyName NVARCHAR(MAX)       = NULL,
    @Count INT,
    @IsActive NVARCHAR(MAX)     = NULL,
    @LargeNumber BIGINT,
    @Ratio DECIMAL(18,6),
    @BinaryData VARBINARY(MAX)  = NULL,
    @OccurredOn DATETIME,
    @OccurredAt DATETIMEOFFSET(7),
    @Duration TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    IF @HeaderIdDBParam = 1
    BEGIN
        -- Result set 1: List<HeaderResult1> (single row)
        -- Mapper reads: CustomeName, Name, MyName, Count, IsActive,
        --               LargeNumber, Ratio, BinaryData, OccurredOn, OccurredAt, Duration
        SELECT
            CAST(901 AS INT)                                             AS CustomeName,
            N'Hdr9_R1'                                                   AS Name,
            N'Hdr9My_R1'                                                 AS MyName,
            CAST(91 AS INT)                                              AS [Count],
            N'true'                                                      AS IsActive,
            CAST(9100 AS BIGINT)                                         AS LargeNumber,
            CAST(91.10 AS DECIMAL(18,6))                                 AS Ratio,
            0xA901                                                       AS BinaryData,
            CAST('2025-09-01' AS DATETIME)                               AS OccurredOn,
            CAST('2025-09-01 09:01:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
            CAST('09:01:00' AS TIME)                                     AS Duration;

        -- Result set 2: TestClass1 (single row)
        -- Mapper detects OccuredOn → reads: Name, Count, OccuredOn, IsActive
        SELECT
            N'T9_Single'                   AS Name,
            CAST(91 AS INT)                AS [Count],
            CAST('2025-09-01' AS DATETIME) AS OccuredOn,    -- note: typo is intentional, matches C# mapper
            CAST(1 AS BIT)                 AS IsActive;
    END
    ELSE
    BEGIN
        -- Result set 1: List<HeaderResult1> (single row)
        SELECT
            CAST(1002 AS INT)                                            AS CustomeName,
            N'Hdr10_Name2'                                               AS Name,
            N'Hdr10My_Name2'                                             AS MyName,
            CAST(102 AS INT)                                             AS [Count],
            N'false'                                                     AS IsActive,
            CAST(10200 AS BIGINT)                                        AS LargeNumber,
            CAST(102.20 AS DECIMAL(18,6))                                AS Ratio,
            0xAA20                                                       AS BinaryData,
            CAST('2025-10-02' AS DATETIME)                               AS OccurredOn,
            CAST('2025-10-02 10:20:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
            CAST('10:20:00' AS TIME)                                     AS Duration;

        -- Result set 2: List<Record4Result>
        -- Mapper detects Name1 → reads: Record4Id, HeaderId, Name1, Count, IsActive, LargeNumber
        SELECT
            CAST(1 AS INT)         AS Record4Id,
            CAST(1002 AS INT)      AS HeaderId,
            N'Rec4_Name1'          AS Name1,
            CAST(102 AS INT)       AS [Count],
            CAST(0 AS BIT)         AS IsActive,
            CAST(10200 AS BIGINT)  AS LargeNumber;

        -- Result set 3: TestClass1 (single row)
        -- Mapper reads: Name, Count, OccuredOn, IsActive
        SELECT
            N'T10_Single'                  AS Name,
            CAST(110 AS INT)               AS [Count],
            CAST('2025-10-20' AS DATETIME) AS OccuredOn,    -- note: typo is intentional, matches C# mapper
            CAST(1 AS BIT)                 AS IsActive;
    END
END;
GO
-- SP for Test10_With_OR
-- Input: TestClass111 parameters
-- Behavior: HeaderId=1 => returns List<HeaderResult11> (multiple rows)
--           HeaderId=2 => returns TestClass1 (single row)
CREATE OR ALTER PROCEDURE dbo.TransientTest10
    @HeaderIdDBParam INT,
    @Name NVARCHAR(MAX)         = NULL,
    @MyName NVARCHAR(MAX)       = NULL,
    @Count INT,
    @IsActive NVARCHAR(MAX)     = NULL,
    @LargeNumber BIGINT,
    @Ratio DECIMAL(18,6),
    @BinaryData VARBINARY(MAX)  = NULL,
    @OccurredOn DATETIME,
    @OccurredAt DATETIMEOFFSET(7),
    @Duration TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    IF @HeaderIdDBParam = 1
    BEGIN
        -- Result set: List<HeaderResult11> (2 rows)
        -- Mapper reads: HeaderId, CustomeName, Count, IsActive, LargeNumber, Ratio,
        --               BinaryData, OccurredOn, OccurredAt, Duration,
        --               HeaderId (HeaderIdOut), CountOut, IsActiveOut, LargeNumberOut,
        --               RatioOut, BinaryDataOut, OccurredOnOut, OccurredAtOut, DurationOut
        SELECT
            CAST(1001 AS INT)                                            AS HeaderId,
            CAST(1001 AS INT)                                            AS CustomeName,
            N'Hdr10_R1'                                                  AS Name,
            N'Hdr10My_R1'                                                AS MyName,
            CAST(101 AS INT)                                             AS [Count],
            N'true'                                                      AS IsActive,
            CAST(10000 AS BIGINT)                                        AS LargeNumber,
            CAST(100.10 AS DECIMAL(18,6))                                AS Ratio,
            0xAA0011                                                     AS BinaryData,
            CAST('2025-10-01' AS DATETIME)                               AS OccurredOn,
            CAST('2025-10-01 10:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
            CAST('01:00:00' AS TIME)                                     AS Duration,
            CAST(201 AS INT)                                             AS CountOut,
            CAST(1 AS BIT)                                               AS IsActiveOut,
            CAST(20100 AS BIGINT)                                        AS LargeNumberOut,
            CAST(201.10 AS DECIMAL(18,6))                                AS RatioOut,
            0xBB0011                                                     AS BinaryDataOut,
            CAST('2025-11-01' AS DATETIME)                               AS OccurredOnOut,
            CAST('2025-11-01 11:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAtOut,
            CAST('11:00:00' AS TIME)                                     AS DurationOut
        UNION ALL
        SELECT
            CAST(1002 AS INT)                                            AS HeaderId,
            CAST(1002 AS INT)                                            AS CustomeName,
            N'Hdr10_R2'                                                  AS Name,
            N'Hdr10My_R2'                                                AS MyName,
            CAST(202 AS INT)                                             AS [Count],
            N'false'                                                     AS IsActive,
            CAST(20000 AS BIGINT)                                        AS LargeNumber,
            CAST(200.20 AS DECIMAL(18,6))                                AS Ratio,
            0xBB0022                                                     AS BinaryData,
            CAST('2025-10-02' AS DATETIME)                               AS OccurredOn,
            CAST('2025-10-02 11:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAt,
            CAST('02:00:00' AS TIME)                                     AS Duration,
            CAST(202 AS INT)                                             AS CountOut,
            CAST(0 AS BIT)                                               AS IsActiveOut,
            CAST(20200 AS BIGINT)                                        AS LargeNumberOut,
            CAST(202.20 AS DECIMAL(18,6))                                AS RatioOut,
            0xBB0022                                                     AS BinaryDataOut,
            CAST('2025-11-02' AS DATETIME)                               AS OccurredOnOut,
            CAST('2025-11-02 12:00:00.0000000 +00:00' AS DATETIMEOFFSET) AS OccurredAtOut,
            CAST('12:00:00' AS TIME)                                     AS DurationOut;
    END
    ELSE
    BEGIN
        -- Result set: TestClass1 (single row)
        -- Mapper detects OccuredOn → reads: Name, Count, OccuredOn, IsActive
        SELECT
            N'T10_Single'                  AS Name,
            CAST(110 AS INT)               AS [Count],
            CAST('2025-10-20' AS DATETIME) AS OccuredOn,   -- intentional typo, matches mapper
            CAST(1 AS BIT)                 AS IsActive;
    END
END;
GO