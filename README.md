SpExecuter is a .NET source generator that eliminates ADO.NET boilerplate by auto-generating strongly-typed, dependency-injected wrappers for your SQL Server stored procedures. Simply define your C# parameter and response classes, apply a few attributes, and the Roslyn generator produces the complete execution code — supporting every common data type, table-valued parameters, output parameters, class inheritance, and complex return shapes including tuples, nested conditional branches, and OR-based result detection. Both packages (CodePiece.SpExecuter.Generator and CodePiece.SpExecuter.Utility) target .NET Standard 2.0, making them compatible with modern .NET versions which has Roslyn source generator.


---

## Key Features

- **Automatic Dynamic code generation** of `Stored Procedure` execution based on your C# classes
- **Attribute-based Configuration** via `[ParamConfig]` for parameter name overrides, output params, exclusions, and uniqueness
- **Table-Valued Parameter** support via `List<T>` with optional `[TVP]` for custom type names
- **Strongly-typed Responses**  plain C# classes (no marker interface required)
- **Rich Return Types**  single object, `List<T>`, flat tuples, nested nullable tuples, and OR condition branching
- **Inheritance Support** parameter and response classes can use class inheritance
- **Dependency Injection** registration with configurable lifetimes (`Transient`, `Scoped`, `Singleton`)
- **Rich Exception Details** via `SpExecuterException`

---

## Getting Started

Add these PackageReference entries to your `.csproj` (use your actual version):

```xml
<PackageReference Include="CodePiece.SpExecuter.Generator" Version="2.0.0"
                  OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
<PackageReference Include="CodePiece.SpExecuter.Utility" Version="2.0.0" />
```

> **Note:** The generator package must be referenced with `OutputItemType="Analyzer"` and `ReferenceOutputAssembly="false"`.

---

## Configuration

In your `Program.cs` (or startup), register the executor:

```csharp
using SpExecuter.Utility;

var services = new ServiceCollection();
services.ConfigureSpExecuter();             // Register all [SpHandler] implementations

var provider = services.BuildServiceProvider();
var transientExecutor = provider.GetRequiredService<ITransientSpExecutor>();
```

---

## Attributes Reference

| Attribute | Target | Purpose |
|---|---|---|
| `[SpHandler(Lifetime)]` | Interface | Marks an interface for source generation. Lifetime: `Transient`, `Scoped`, `Singleton` |
| `[StoredProcedure("name")]` | Method | Maps a method to a stored procedure |
| `[ParamConfig]` | Property | Configures parameter/result mapping (see below) |
| `[TVP("schema.TypeName")]` | Class | Overrides the SQL TVP type name (default: `dbo.<ClassName>`) |

### `[ParamConfig]` Options

| Property | Applied On | Description |
|---|---|---|
| `DBParam = "name"` | Input & Output | Maps the C# property to a different SQL parameter/column name |
| `OutParam = true` | Input | Marks the parameter as a SQL OUTPUT parameter |
| `ParamExclusion = true` | Input | Excludes the property from being sent as an input parameter |
| `ResultExclusion = true` | Output | Excludes the property from result set mapping |
| `Unique = true` | Output | Marks the column as the unique identifier for OR condition result set detection |

---

## Defining Parameter Classes  All Data Types

This first method demonstrates every supported data type, including non-nullable, nullable, TVP, output, excluded, and overridden parameters via class inheritance.

### Stored Procedure

```sql
CREATE TYPE dbo.InBaseTestClass2 AS TABLE
(
    Count6 INT, IsActive6 BIT, LargeNumber6 BIGINT, Ratio6 DECIMAL(18,6),
    Name6 SMALLINT, TinyNumber6 TINYINT, LessPreciseFloat6 REAL, DoublePrecision6 FLOAT,
    SingleChar6 NCHAR(1), SByteValue6 SMALLINT, UShortValue6 INT, UIntValue6 BIGINT,
    ULongValue6 DECIMAL(20,0), NIntValue6 BIGINT, NUIntValue6 DECIMAL(20,0),
    OccurredOn6 DATETIME, OccurredAt6 DATETIMEOFFSET(7), Duration6 TIME(7),
    UniqueId6 UNIQUEIDENTIFIER, StatusType6 NVARCHAR(255)
);
GO

CREATE OR ALTER PROCEDURE dbo.TransientTest1
    -- Non-nullable (inherited from InBaseTestClass1)
    @Count5 INT, @IsActive5 BIT, @LargeNumber5 BIGINT, @Ratio5 DECIMAL(18,6),
    @Name5 SMALLINT, @TinyNumber5 TINYINT, @LessPreciseFloat5 REAL, @DoublePrecision5 FLOAT,
    @SingleChar5 NCHAR(1), @SByteValue5 SMALLINT, @UShortValue5 INT, @UIntValue5 BIGINT,
    @ULongValue5 DECIMAL(20,0), @NIntValue5 BIGINT, @NUIntValue5 DECIMAL(20,0),
    @OccurredOn5 DATETIME, @OccurredAt5 DATETIMEOFFSET(7), @Duration5 TIME(7),
    @UniqueId5 UNIQUEIDENTIFIER, @StatusType5 NVARCHAR(255),

    -- Non-nullable (own properties)
    @Count INT, @IsActive BIT, @LargeNumber BIGINT, @Ratio DECIMAL(18,6),
    @Name SMALLINT, @TinyNumber TINYINT, @LessPreciseFloat REAL, @DoublePrecision FLOAT,
    @SingleChar NCHAR(1), @SByteValue SMALLINT, @UShortValue INT, @UIntValue BIGINT,
    @ULongValue DECIMAL(20,0), @NIntValue BIGINT, @NUIntValue DECIMAL(20,0),
    @OccurredOn DATETIME, @OccurredAt DATETIMEOFFSET(7), @Duration TIME(7),
    @UniqueId UNIQUEIDENTIFIER, @BinaryData VARBINARY(MAX), @CharArray NVARCHAR(MAX),
    @StatusType NVARCHAR(255),

    -- TVP
    @NameList dbo.InBaseTestClass2 READONLY,

    -- Output parameter
    @Count1DBParam INT OUTPUT,

    -- Nullable parameters (suffix 1)
    @IsActive1 BIT = NULL, @LargeNumber1 BIGINT = NULL, ...
    -- (all nullable types supported)

    -- Nullable TVP
    @NameList1 dbo.InBaseTestClass2 READONLY,

    -- Output parameters (suffix 2, with DBParam name override)
    @Count2DBParam INT OUTPUT, @IsActive2DBParam BIT OUTPUT, ...

    -- Overridden parameters (suffix 4, with DBParam name override from base class)
    @OverrideCount4 INT, @OverrideIsActive4 BIT, ...
AS
BEGIN
    SET NOCOUNT ON;
    -- Sets output params and returns a single result set
    ...
END;
```

### C#  Base Class (inherited properties)

```csharp
public class InBaseTestClass1
{
    public virtual int Count4 { get; set; }
    public virtual bool IsActive4 { get; set; }
    public virtual long LargeNumber4 { get; set; }
    public virtual decimal Ratio4 { get; set; }
    public virtual short Name4 { get; set; }
    public virtual byte TinyNumber4 { get; set; }
    public virtual float LessPreciseFloat4 { get; set; }
    public virtual double DoublePrecision4 { get; set; }
    public virtual char SingleChar4 { get; set; }
    public virtual sbyte SByteValue4 { get; set; }
    public virtual ushort UShortValue4 { get; set; }
    public virtual uint UIntValue4 { get; set; }
    public virtual ulong ULongValue4 { get; set; }
    public virtual nint NIntValue4 { get; set; }
    public virtual nuint NUIntValue4 { get; set; }
    public virtual DateTime OccurredOn4 { get; set; }
    public virtual DateTimeOffset OccurredAt4 { get; set; }
    public virtual TimeSpan Duration4 { get; set; }
    public virtual Guid UniqueId4 { get; set; }
    public virtual StatusType StatusType4 { get; set; }

    public int Count5 { get; set; }
    public bool IsActive5 { get; set; }
    public long LargeNumber5 { get; set; }
    public decimal Ratio5 { get; set; }
    public short Name5 { get; set; }
    public byte TinyNumber5 { get; set; }
    public float LessPreciseFloat5 { get; set; }
    public double DoublePrecision5 { get; set; }
    public char SingleChar5 { get; set; }
    public sbyte SByteValue5 { get; set; }
    public ushort UShortValue5 { get; set; }
    public uint UIntValue5 { get; set; }
    public ulong ULongValue5 { get; set; }
    public nint NIntValue5 { get; set; }
    public nuint NUIntValue5 { get; set; }
    public DateTime OccurredOn5 { get; set; }
    public DateTimeOffset OccurredAt5 { get; set; }
    public TimeSpan Duration5 { get; set; }
    public Guid UniqueId5 { get; set; }
    public StatusType StatusType5 { get; set; }
}
```

### C# TVP Row Class

```csharp
public class InBaseTestClass2
{
    public int Count6 { get; set; }
    public bool IsActive6 { get; set; }
    public long LargeNumber6 { get; set; }
    public decimal Ratio6 { get; set; }
    public short Name6 { get; set; }
    public byte TinyNumber6 { get; set; }
    public float LessPreciseFloat6 { get; set; }
    public double DoublePrecision6 { get; set; }
    public char SingleChar6 { get; set; }
    public sbyte SByteValue6 { get; set; }
    public ushort UShortValue6 { get; set; }
    public uint UIntValue6 { get; set; }
    public ulong ULongValue6 { get; set; }
    public nint NIntValue6 { get; set; }
    public nuint NUIntValue6 { get; set; }
    public DateTime OccurredOn6 { get; set; }
    public DateTimeOffset OccurredAt6 { get; set; }
    public TimeSpan Duration6 { get; set; }
    public Guid UniqueId6 { get; set; }
    public StatusType StatusType6 { get; set; }
}
```

### C#  Main Input Parameter Class

```csharp
public class InBaseTestClass : InBaseTestClass1
{
    // -- Non-nullable parameters (mapped by property name) --
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public decimal Ratio { get; set; }
    public short Name { get; set; }
    public byte TinyNumber { get; set; }
    public float LessPreciseFloat { get; set; }
    public double DoublePrecision { get; set; }
    public char SingleChar { get; set; }
    public sbyte SByteValue { get; set; }
    public ushort UShortValue { get; set; }
    public uint UIntValue { get; set; }
    public ulong ULongValue { get; set; }
    public nint NIntValue { get; set; }
    public nuint NUIntValue { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
    public Guid UniqueId { get; set; }
    public byte[] BinaryData { get; set; }
    public char[] CharArray { get; set; }
    public StatusType StatusType { get; set; }
    public List<InBaseTestClass2> NameList { get; set; }       // TVP

    // -- Output parameter (DBParam overrides the SQL param name) --
    [ParamConfig(DBParam = "Count1DBParam", OutParam = true)]
    public int? Count1 { get; set; }

    // -- Nullable parameters --
    public bool? IsActive1 { get; set; }
    public long? LargeNumber1 { get; set; }
    public decimal? Ratio1 { get; set; }
    public short? Name1 { get; set; }
    public byte? TinyNumber1 { get; set; }
    public float? LessPreciseFloat1 { get; set; }
    public double? DoublePrecision1 { get; set; }
    public char? SingleChar1 { get; set; }
    public sbyte? SByteValue1 { get; set; }
    public ushort? UShortValue1 { get; set; }
    public uint? UIntValue1 { get; set; }
    public ulong? ULongValue1 { get; set; }
    public nint? NIntValue1 { get; set; }
    public nuint? NUIntValue1 { get; set; }
    public DateTime? OccurredOn1 { get; set; }
    public DateTimeOffset? OccurredAt1 { get; set; }
    public TimeSpan? Duration1 { get; set; }
    public Guid? UniqueId1 { get; set; }
    public byte[]? BinaryData1 { get; set; }
    public char[]? CharArray1 { get; set; }
    public StatusType? StatusType1 { get; set; }
    public List<InBaseTestClass2>? NameList1 { get; set; }     // Nullable TVP

    // -- Output parameters (all with DBParam override) --
    [ParamConfig(DBParam = "Count2DBParam", OutParam = true)]
    public int Count2 { get; set; }
    [ParamConfig(DBParam = "IsActive2DBParam", OutParam = true)]
    public bool IsActive2 { get; set; }
    [ParamConfig(DBParam = "LargeNumber2DBParam", OutParam = true)]
    public long LargeNumber2 { get; set; }
    // ... (all types follow the same pattern)

    // -- Excluded parameters (not sent to SQL) --
    [ParamConfig(ParamExclusion = true)]
    public int Count3 { get; set; }
    [ParamConfig(ParamExclusion = true)]
    public bool IsActive3 { get; set; }
    // ... (excluded from SQL command)

    // -- Overridden parameters (from base class, with DBParam override) --
    [ParamConfig(DBParam = "OverrideCount4")]
    public override int Count4 { get; set; }
    [ParamConfig(DBParam = "OverrideIsActive4")]
    public override bool IsActive4 { get; set; }
    // ... (overrides virtual properties from InBaseTestClass1)
}
```

### C#  Response Class (single result set)

```csharp
public class OutBaseTestClass : OutBaseTestClass1
{
    // Non-nullable response properties
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public decimal Ratio { get; set; }
    public short Name { get; set; }
    public byte TinyNumber { get; set; }
    public float LessPreciseFloat { get; set; }
    public double DoublePrecision { get; set; }
    public char SingleChar { get; set; }
    public sbyte SByteValue { get; set; }
    public ushort UShortValue { get; set; }
    public uint UIntValue { get; set; }
    public ulong ULongValue { get; set; }
    public nint NIntValue { get; set; }
    public nuint NUIntValue { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
    public Guid UniqueId { get; set; }
    public byte[] BinaryData { get; set; }
    public char[] CharArray { get; set; }
    public StatusType StatusType { get; set; }

    // Nullable response properties
    public int? Count1 { get; set; }
    public bool? IsActive1 { get; set; }
    // ...

    // DBParam-mapped columns (Unique marks identifier for OR detection)
    [ParamConfig(DBParam = "Count2DBParam", Unique = true)]
    public int Count2 { get; set; }
    [ParamConfig(DBParam = "IsActive2DBParam")]
    public bool IsActive2 { get; set; }
    // ...

    // ResultExclusion � property exists in class but not mapped from result set
    [ParamConfig(ResultExclusion = true)]
    public int Count3 { get; set; }
    // ...

    // Override columns (from base class, with DBParam override)
    [ParamConfig(DBParam = "OverrideCount4")]
    public override int Count4 { get; set; }
    // ...
}
```

### C#  Handler Interface & Call

```csharp
[SpHandler(Lifetime.Transient)]
public interface ITransientSpExecutor
{
    [StoredProcedure("TransientTest1")]
    Task<OutBaseTestClass> Test1_With_Single(string connectionString, InBaseTestClass parameters);
}
```

```csharp
var res1 = await transientExecutor.Test1_With_Single(connectionString, new InBaseTestClass { /* ... */ });
```

---

## Supported C# to SQL Data Type Mappings

| C# Type | SQL Server Type |
|---|---|
| `int` / `int?` | `INT` |
| `bool` / `bool?` | `BIT` |
| `long` / `long?` | `BIGINT` |
| `decimal` / `decimal?` | `DECIMAL` |
| `short` / `short?` | `SMALLINT` |
| `byte` / `byte?` | `TINYINT` |
| `float` / `float?` | `REAL` |
| `double` / `double?` | `FLOAT` |
| `char` / `char?` | `NCHAR(1)` |
| `sbyte` / `sbyte?` | `SMALLINT` |
| `ushort` / `ushort?` | `INT` |
| `uint` / `uint?` | `BIGINT` |
| `ulong` / `ulong?` | `DECIMAL(20,0)` |
| `nint` / `nint?` | `BIGINT` |
| `nuint` / `nuint?` | `DECIMAL(20,0)` |
| `DateTime` / `DateTime?` | `DATETIME` |
| `DateTimeOffset` / `DateTimeOffset?` | `DATETIMEOFFSET` |
| `TimeSpan` / `TimeSpan?` | `TIME` |
| `Guid` / `Guid?` | `UNIQUEIDENTIFIER` |
| `byte[]` / `byte[]?` | `VARBINARY(MAX)` |
| `char[]` / `char[]?` | `NVARCHAR(MAX)` |
| `enum` / `enum?` | `NVARCHAR(255)` |
| `string` | `NVARCHAR(MAX)` |
| `List<T>` | Table-Valued Parameter |

---

## Table-Valued Parameters (TVPs)

- Use `List<T>` in your parameter class for TVP parameters.
- By default, SpExecuter uses `dbo.<ClassName>` as the SQL type name.
- Override with `[TVP("schema.CustomTypeName")]` on the class.

```csharp
[TVP("dbo.Record4TableType")]
public class Record4TableType
{
    public string Name        { get; set; }
    public int    Count       { get; set; }
    public bool   IsActive    { get; set; }
    public long   LargeNumber { get; set; }
}

// Uses default name: dbo.AuditInfoTableType
public class AuditInfoTableType
{
    public string Description { get; set; }
    public int    Count       { get; set; }
    public bool   IsActive    { get; set; }
    public long   LargeNumber { get; set; }
    public decimal Ratio      { get; set; }
    public byte[] BinaryData  { get; set; }
    public DateTime OccurredOn       { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration         { get; set; }
}
```

---

## Defining Response Classes

Response classes are plain C# classes no marker interface needed. Only the string type exhibits loose typing behavior—values of other types can be implicitly converted to a string when assigned. However, all other data types are strictly typed, and any mismatch will result in a runtime error..

Use `[ParamConfig]` to:
- Map to a different column name: `[ParamConfig(DBParam = "CustomeName")]`
- Mark a unique identifier column for OR detection: `[ParamConfig(Unique = true)]`
- Exclude from result mapping: `[ParamConfig(ResultExclusion = true)]`

```csharp
public class HeaderResult1
{
    [ParamConfig(DBParam = "CustomeName", Unique = true)]
    public int HeaderId { get; set; }
    public string Name { get; set; }
    public string MyName { get; set; }
    public int Count { get; set; }
    public string IsActive { get; set; }
    public long LargeNumber { get; set; }
    public decimal Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
}

public class Record4Result
{
    public int Record4Id { get; set; }
    public int HeaderId { get; set; }
    [ParamConfig(Unique = true)]
    public string Name1 { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
}
```

### Response class with inheritance

```csharp
public class TestClass : HeaderResult1
{
    [ParamConfig(DBParam = "CustomeName")]
    public string Name { get; set; }
    public string MyName { get; set; }
}
```

---

## Creating Your SP Handler Interface

Decorate an interface with `[SpHandler(Lifetime)]` and its methods with `[StoredProcedure("name")]`.

The first parameter is always the connection string. The optional second parameter is the input class.

### Return a single object

```csharp
[StoredProcedure("TransientTest1")]
Task<OutBaseTestClass> Test1_With_Single(string connectionString, InBaseTestClass parameters);
```

### Return a list

```csharp
[StoredProcedure("TransientTest2")]
Task<List<TestClass>> Test2_With_List(string connectionString, TestClass111 parameters);
```

### Return a flat tuple (multiple result sets)

```csharp
// Two singles
[StoredProcedure("TransientTest3")]
Task<(TestClass, HeaderResult1)> Test3_With_TupleContainingSingle(string connectionString);

// Two lists
[StoredProcedure("TransientTest4")]
Task<(List<TestClass>, List<HeaderResult1>)> Test4_With_TupleContaing2List(string connectionString, TestClass111 parameters);

// Mixed: list + single
[StoredProcedure("TransientTest5")]
Task<(List<HeaderResult1>, TestClass)> Test5_With_TupleContaingSingleNList(string connectionString, TestClass111 parameters);
```

### Nested nullable tuples (conditional result sets)

When a stored procedure returns **different result sets** based on a condition (e.g., an `IF/ELSE` on an input parameter), use nested nullable tuples. Each nullable tuple `(...)?` represents a branch — only the matching branch is populated, others are `null`.

At runtime, the generated code checks whether a **unique** column (marked with `[ParamConfig(Unique = true)]`) exists in the current result set using `ColumnExists(reader, "uniqueColumnName")`. Based on which unique column is found, it routes to the correct tuple branch.

```csharp
// SP returns EITHER (List<TestClass1> + HeaderResult1) OR just TestClass1
// based on @HeaderIdDBParam value
[StoredProcedure("TransientTest6")]
Task<((List<TestClass1>, HeaderResult1)?, TestClass1)> Test6_With_Tuple_N_Single(
    string connectionString, TestClass111 parameters);
```

**Usage:**
```csharp
// HeaderId = 1 -> nested tuple is populated
var res6_1 = await transientExecutor.Test6_With_Tuple_N_Single(connectionString,
    new TestClass111 { HeaderId = 1, Count = 10, LargeNumber = 6000L, Ratio = 60.60m,
        OccurredOn = new DateTime(2025, 6, 1),
        OccurredAt = new DateTimeOffset(2025, 6, 1, 10, 0, 0, TimeSpan.Zero),
        Duration = TimeSpan.FromMinutes(30) });
// res6_1.Item1 has value: (List<TestClass1>, HeaderResult1)
// res6_1.Item2 is default

// HeaderId = 2 -> single is populated
var res6_2 = await transientExecutor.Test6_With_Tuple_N_Single(connectionString,
    new TestClass111 { HeaderId = 2, /* ... */ });
// res6_2.Item1 is null
// res6_2.Item2 has the TestClass1 value
```

### Multiple nested tuples (3+ branches)

```csharp
// SP returns different result sets based on @HeaderIdDBParam = 1, 2, or 3
[StoredProcedure("TransientTest8")]
Task<((List<TestClass1>, TestClass2)?,
      (List<TestClass1>, TestClass2, List<TestClass3>, TestClass8)?,
      (List<TestClass1>, TestClass10, List<TestClass11>, TestClass12)?)>
    Test8_With_Tuple_Single_List(string connectionString, TestClass111 parameters);
```

### Nested tuples + flat tail element

```csharp
// 2 nullable tuple branches + a flat List<HeaderResult1> for a 3rd branch
[StoredProcedure("TransientTest9")]
Task<((List<HeaderResult1>, TestClass1)?,
      (List<HeaderResult1>, List<Record4Result>, TestClass1)?,
      List<HeaderResult1>)>
    Test9_With_2Tuple_Single(string connectionString, TestClass111 parameters);
```

---

## OR Condition (ConditionType.OR)

When a stored procedure can return **one of several completely different shapes** from a single result set (same number of result sets, different column schemas), use `ConditionType = ConditionType.OR`.

Each response class must have a `[ParamConfig(Unique = true)]` property. At runtime, the generated code uses `ColumnExists(reader, "uniqueColumnName")` to check the result set's column schema and determine which type matches, then maps accordingly. Only the matching item in the tuple is populated; the other is `default`.

```csharp
[StoredProcedure("TransientTest10", ConditionType = ConditionType.OR)]
Task<(List<HeaderResult11>, TestClass1)> Test10_With_OR(
    string connectionString, TestClass111 parameters);
```

**How it works:**
- `HeaderResult11` has `[ParamConfig(DBParam = "CustomeName", Unique = true)]` on its `Name` property
- `TestClass1` has `[ParamConfig(Unique = true)]` on its `OccuredOn` property
- The generator checks the result set columns for the unique column name and populates the matching type

```csharp
// HeaderId = 1 -> result set columns match HeaderResult11 (has CustomeName column)
var res10_1 = await transientExecutor.Test10_With_OR(connectionString,
    new TestClass111 { HeaderId = 1, Count = 10, LargeNumber = 6000L, Ratio = 60.60m,
        OccurredOn = new DateTime(2025, 6, 1),
        OccurredAt = new DateTimeOffset(2025, 6, 1, 10, 0, 0, TimeSpan.Zero),
        Duration = TimeSpan.FromMinutes(30) });
// res10_1.Item1 -> List<HeaderResult11> (populated)
// res10_1.Item2 -> TestClass1 (default)

// HeaderId = 2 -> result set columns match TestClass1 (has OccuredOn column)
var res10_2 = await transientExecutor.Test10_With_OR(connectionString,
    new TestClass111 { HeaderId = 2, /* ... */ });
// res10_2.Item1 -> List<HeaderResult11> (default/empty)
// res10_2.Item2 -> TestClass1 (populated)
```

---

## Complete Program.cs Example

```csharp
using Example;
using Example.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SpExecuter.Utility;
using System.Text;

var services = new ServiceCollection();
services.ConfigureSpExecuter();

var provider = services.BuildServiceProvider();
var transientExecutor = provider.GetRequiredService<ITransientSpExecutor>();

string connectionString = "Server=localhost\\SQLEXPRESS;Database=Test;Trusted_Connection=True;TrustServerCertificate=True;Integrated Security=True;";

try
{
    // 1. Single result
    var res1 = await transientExecutor.Test1_With_Single(connectionString, AllObjects.CreateInBaseTestClass());
    Console.WriteLine("Test1_With_Single: " + JsonConvert.SerializeObject(res1));

    // 2. List result
    var res2 = await transientExecutor.Test2_With_List(connectionString, AllObjects.CreateTestClass111());
    Console.WriteLine("Test2_With_List: " + JsonConvert.SerializeObject(res2));

    // 3. Tuple with two singles (no params)
    var res3 = await transientExecutor.Test3_With_TupleContainingSingle(connectionString);
    Console.WriteLine("Test3_With_TupleContainingSingle: " + JsonConvert.SerializeObject(res3));

    // 4. Tuple with two lists
    var res4 = await transientExecutor.Test4_With_TupleContaing2List(connectionString, AllObjects.CreateTestClass111ForTest4());
    Console.WriteLine("Test4_With_TupleContaing2List: " + JsonConvert.SerializeObject(res4));

    // 5. Tuple: list + single
    var res5 = await transientExecutor.Test5_With_TupleContaingSingleNList(connectionString, AllObjects.CreateTestClass111ForTest4());
    Console.WriteLine("Test5_With_TupleContaingSingleNList: " + JsonConvert.SerializeObject(res5));

    // 6. Nested nullable tuple OR single
    var res6_1 = await transientExecutor.Test6_With_Tuple_N_Single(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test6 (HeaderId=1): " + JsonConvert.SerializeObject(res6_1));
    var res6_2 = await transientExecutor.Test6_With_Tuple_N_Single(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test6 (HeaderId=2): " + JsonConvert.SerializeObject(res6_2));

    // 7. Nested nullable tuple OR list
    var res7_1 = await transientExecutor.Test7_With_Tuple_N_List(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test7 (HeaderId=1): " + JsonConvert.SerializeObject(res7_1));
    var res7_2 = await transientExecutor.Test7_With_Tuple_N_List(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test7 (HeaderId=2): " + JsonConvert.SerializeObject(res7_2));

    // 8. Three nested tuple branches
    var res8_1 = await transientExecutor.Test8_With_Tuple_Single_List(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test8 (HeaderId=1): " + JsonConvert.SerializeObject(res8_1));
    var res8_2 = await transientExecutor.Test8_With_Tuple_Single_List(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test8 (HeaderId=2): " + JsonConvert.SerializeObject(res8_2));
    var res8_3 = await transientExecutor.Test8_With_Tuple_Single_List(connectionString, AllObjects.CreateTestClass111ForTest6(3));
    Console.WriteLine("Test8 (HeaderId=3): " + JsonConvert.SerializeObject(res8_3));

    // 9. Two nested tuples + flat list
    var res9_1 = await transientExecutor.Test9_With_2Tuple_Single(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test9 (HeaderId=1): " + JsonConvert.SerializeObject(res9_1));
    var res9_2 = await transientExecutor.Test9_With_2Tuple_Single(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test9 (HeaderId=2): " + JsonConvert.SerializeObject(res9_2));
    var res9_3 = await transientExecutor.Test9_With_2Tuple_Single(connectionString, AllObjects.CreateTestClass111ForTest6(3));
    Console.WriteLine("Test9 (HeaderId=3): " + JsonConvert.SerializeObject(res9_3));

    // 10. OR condition
    var res10_1 = await transientExecutor.Test10_With_OR(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test10_OR (HeaderId=1): " + JsonConvert.SerializeObject(res10_1));
    var res10_2 = await transientExecutor.Test10_With_OR(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test10_OR (HeaderId=2): " + JsonConvert.SerializeObject(res10_2));
}
catch (SpExecuterException ex)
{
    // Log/debug with extra information
    string extraInfo = ex.Information;
}
Console.WriteLine("Test execution completed.");
```

---

## Example Project

A complete working example is available in the **Example** project of this repository. See:

```
/Example/Program.cs        entry point with all test calls
/Example/Example.cs        interface definitions, DTOs, response classes
/Example/AllObjects.cs     helper methods to create test input objects
/Example/TestScript.sql    SQL scripts to create the test stored procedures
```

---

## Contributing

Thank you for considering contributing! Whether you're fixing a typo, improving our documentation, suggesting new features, or simply sharing how you're using SpExecuter in your own projects, your input is invaluable. Feel free to open an issue to discuss ideas, submit a pull request with your changes, or join our community discussions to help shape the future of this library. Every contribution  big or small helps make SpExecuter better for everyone!

---

## License

MIT License  see [LICENSE](LICENSE) for details.

👉 For full usage examples, and source code, please visit the GitHub repository:

🔗 [https://github.com/prashantdaradeos/SpExecueter](https://github.com/prashantdaradeos/SpExecueter)
