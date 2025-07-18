# SpExecuter


SpExecuter auto‑generates strongly‑typed, DI‑ready wrappers around your SQL Server stored procedures—no more ADO.NET boilerplate. Define your parameter and response classes, tag them with a few attributes, and let the source generator do the rest.


---

## Key Features

- **Automatic Dynamic IL** of `SqlParameter` mappings based on your C# classes  
- **Attribute‑based Overrides** for parameter names (`[DbParam]`) and TVP type names (`[TVP]`)  
- **Table‑Valued Parameter** support via `List<T>`  
- **Strongly‑typed Response** mapping to your output classes (implementing `ISpResponse`)  
- **Dependency Injection** registration with configurable lifetimes  
- **Rich Exception Details** via `SpExecuterException`  


---
## Getting Started


Add these PackageReference entries to your `.csproj` (use your actual version):

```xml
<PackageReference Include="CodePiece.SpExecuter.Generator" Version="1.0.3"
                  ReferenceOutputAssembly="false" />
<PackageReference Include="CodePiece.SpExecuter.Utility" Version="1.0.3" />
```


> **Note:** The generator package must be referenced with \`ReferenceOutputAssembly="false"\` in your \`.csproj\`.
---

## Configuration

In your `Program.cs`, register the executor:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSpExecuter();

var app = builder.Build();
// …
```

---

## Defining Parameter Classes

Create a class mapping your SP inputs:
```sql
CREATE OR ALTER PROCEDURE dbo.SaveFullHeaderDetails
    @Name            NVARCHAR(100),
    @Count           INT,
    @IsActive        BIT,
    @LargeNumber     BIGINT,
    @Ratio           DECIMAL(18,6),
    @BinaryData      VARBINARY(MAX),
    @Occurredon      DATETIME2(7),
    @OccurredAt      DATETIMEOFFSET(7),
    @Duration        TIME(7),
    @Record4Items    dbo.Record4TableType READONLY,
    @AuditItems      dbo.AuditInfoTableType READONLY

```


```csharp
public class HeaderParameters
{
    [DbParam("Name")]
    public string MyName { get; set; }

    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public double Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTime Occurredon { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
    public List<Record4TableType> Record4Items { get; set; } = new();
    public List<AuditInfoTableType> AuditItems   { get; set; } = new();
}
```
- If you need a different parameter name, use the \`[DbParam]\` attribute

---

## Table‑Valued Parameters (TVPs)

- Define your TVP row types and use `List<>` in params:
- By default, SpExecuter adds the schema \`dbo.\` to your class name.  
- Override the SQL type name with the \`[TVP("schema.MyCustomType")]\` attribute on the class.

```sql
CREATE TYPE dbo.Record4TableType AS TABLE
(
    Name         NVARCHAR(100)     NOT NULL,
    Count        INT               NOT NULL,
    IsActive     BIT               NOT NULL,
    LargeNumber  BIGINT            NOT NULL
);
CREATE TYPE dbo.AuditInfoTableType AS TABLE
(
    Description  NVARCHAR(500)     NOT NULL,
    Count        INT               NOT NULL
);
```


```csharp
[TVP("dbo.Record4TableType")]
public class Record4TableType
{
    public string Name        { get; set; }
    public int    Count       { get; set; }
    public bool   IsActive    { get; set; }
    public long   LargeNumber { get; set; }
}

public class AuditInfoTableType
{
    public string Description { get; set; }
    public int    Count       { get; set; }
}
```

---

## Defining Response Classes

- Datatypes &  Order of properties coming from Datatable and your C# class should exactly match.
```sql

    SELECT HeaderId, Name, Count, IsActive, LargeNumber, Ratio,
           BinaryData, Occurredon, OccurredAt, Duration
    FROM dbo.Headers;

    SELECT Record4Id, HeaderId, Name, Count, IsActive, LargeNumber 
        FROM dbo.Record4Table;


```
Result DTOs must implement `ISpResponse`:

```csharp
public class HeaderResult : ISpResponse
{
    public int    HeaderId   { get; set; }
    public string Name       { get; set; }
    public int    Count      { get; set; }
    public bool   IsActive   { get; set; }
    public long   LargeNumber{ get; set; }
    public decimal Ratio      { get; set; }
    public byte[] BinaryData { get; set; }
}
public class Record4Result : ISpResponse
{
    public int Record4Id { get; set; }
    public int HeaderId { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
}
```




---

## Creating Your SP Handler Interface


- Create an interface to represent your stored‑procedure client, and decorate it with \`[SpHandler]\`. You can specify the DI lifetime (\`Singleton\`, \`Scoped\`, or \`Transient\`).
- Decorate with `[StoredProcedure]` and specify SP name

```csharp
using SpExecuter.Utility;

[SpHandler(Lifetime.Singleton)]
public interface ISingletonSpExecutor
{
    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<(List<HeaderResult> hList,
               List<Record4Result> rList)>
    GetTwoListsAsync(string connectionString, HeaderParameters parameters);
}
```

---

## Using the Executor in a Service

Example service injecting and calling your SP:

```csharp
public class HeaderService
{
    private readonly ISingletonSpExecutor _singletonSpExecutor;

    public HeaderService(ISingletonSpExecutor singletonSpExecutor)
        => _singletonSpExecutor = singletonSpExecutor;

    public async ValueTask GetList()
    {
        try
        {
            var (hList, rList) = await _singletonSpExecutor
                .GetTwoListsAsync("Your_Connection_String", new HeaderParameters());
            // …
        }
        catch (SpExecuterException ex)
        {
            // Log/debug with ex.Information
            string extraInfo = ex.Information;
        }
    }
}
```



---

## Example Project

A complete working example is available in the **Example** project of this repository. See:

```
/Example/Program.cs
/Example/Example.cs
```

---

## Contributing
Thank you for considering contributing! Whether you’re fixing a typo, improving our documentation, suggesting new features, or simply sharing how you’re using SpExecuter in your own projects, your input is invaluable. Feel free to open an issue to discuss ideas, submit a pull request with your changes, or join our community discussions to help shape the future of this library. Every contribution—big or small—helps make SpExecuter better for everyone!
 

---

## License

MIT License — see [LICENSE](LICENSE) for details.
