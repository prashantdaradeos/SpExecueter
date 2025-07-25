using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpExecuter.Utility;

namespace Example.DataAccess;


#region Stored‑procedure handler interfaces

[SpHandler(Lifetime.Scoped)]
public interface IScopedSpExecutor
{
    //Even though the first parameter name 'connectionString' is currently optional,
    //it's recommended to maintain this convention to ensure compatibility with future versions.


    // For getting single record, use ValueTask<T> as return type.
    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<HeaderResult> GetSingleRecordAsync(string connectionString, HeaderParameters parameters);

    // For getting multiple records, use ValueTask<(  )> as return type.
    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<(HeaderResult, Record4Result, AuditInfoResult)> GetTupleOfRecordsAsync(string connectionString, HeaderParameters parameters);

    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<(List<HeaderResult>, Record4Result)> GetListAndObjectAsync(string connectionString, HeaderParameters parameters);
}

[SpHandler(Lifetime.Singleton)]
public interface ISingletonSpExecutor
{
    //For getting list of records, use ValueTask<List<T>> as return type.
    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<List<HeaderResult>> GetListAsync(string connectionString, HeaderParameters parameters);

    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<(List<HeaderResult>, List<Record4Result>)> GetTwoListsAsync(string connectionString, HeaderParameters parameters);

    //If SP does not return any result, use GenericSpResponse as Response.
    //You can skip passing parameters if SP does not require any.

    [StoredProcedure("InsertHeaderStatic")]
    ValueTask<GenericSpResponse> GetGenericResponseAsync(string connectionString);
    [StoredProcedure("InsertHeaderOnly")]
    ValueTask<GenericSpResponse> GetGenericResponseAsync(string connectionString, HeaderInfo parameters);

    //If you want to skip datatablethen pass 'SkipResponse' as parameter.
    [StoredProcedure("SaveFullHeaderDetails")]
    ValueTask<(List<HeaderResult>,SkipResponse sr, List<AuditInfoResult>)> GetThreeListsAsync(string connectionString, HeaderParameters parameters);
}

#endregion

#region DTOs
//Property Names must match the stored procedure parameter names or provide name with [DbParam].
//Order of properties does not matter.
//Only below types are supported as parameters

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
//You can pass TVPs as parameters to stored procedures as given below with list.
// '.dbo' will be added automatically to the type name.
// If [TVP] is specified, the names given will be passed to stored procedure as table-valued type.
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

//Order of properties and datatype matter. they must match order of table coming from SP.
//All response classes must implement ISpResponse interface.
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

