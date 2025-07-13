using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpExecuter.Utility;

namespace Example.DataAccess;


#region Stored‑procedure handler interfaces

[SpHandler(Lifetime.Scoped)]
public interface IScopedSpExecutor
{
    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<HeaderResult> GetSingleRecordAsync(string connectionString, HeaderParameters parameters);

    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<(HeaderResult, Record4Result, AuditInfoResult)> GetTupleOfRecordsAsync(string connectionString, HeaderParameters parameters);

    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<(List<HeaderResult>, Record4Result)> GetListAndObjectAsync(string connectionString, HeaderParameters parameters);
}

[SpHandler(Lifetime.Singleton)]
public interface ISingletonSpExecutor
{
    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<List<HeaderResult>> GetListAsync(string connectionString, HeaderParameters parameters);

    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<(List<HeaderResult>, List<Record4Result>)> GetTwoListsAsync(string connectionString, HeaderParameters parameters);

    [StoredProcedure("InsertHeaderStatic")]
    ValueTask<GenericSpResponse> GetGenericResponseAsync(string connectionString);
    [StoredProcedure("InsertHeaderOnly")]
    ValueTask<GenericSpResponse> GetGenericResponseAsync(string connectionString, HeaderInfo parameters);

    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<(List<HeaderResult>, List<Record4Result>, List<AuditInfoResult>)> GetThreeListsAsync(string connectionString, HeaderParameters parameters);
}

#endregion

#region DTOs
public class HeaderInfo
{
    [DbParam("Name")]
    public string MyName { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public double Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
}
public class HeaderParameters : HeaderInfo
{
 
    public List<Record4TableType> Record4Items { get; set; } = new();
    public List<AuditInfoTableType> AuditItems { get; set; } = new();
}
[TVP("dbo.Record4TableType")]
public class Record4TableType 
{

    public string Name { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
}
public class AuditInfoTableType
{
    public string Description { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public double Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
}


public class HeaderResult :ISpResponse
{
    public int HeaderId { get; set; }

    public string Name { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public decimal Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
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
public class AuditInfoResult : ISpResponse
{
    public int AuditInfoId { get; set; }
    public int HeaderId { get; set; }
    public string Description { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public decimal Ratio { get; set; }
    public byte[] BinaryData { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public TimeSpan Duration { get; set; }
}

#endregion

