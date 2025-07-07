using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpExecuter.Utility;

namespace Example.DataAccess;


#region Stored‑procedure handler interfaces

[SpHandler(Lifetime.Scoped)]
public interface IScopedSpExecutor
{
    [StoredProcedure("Test_GetSingleRecord")]
    ValueTask<Record4> GetSingleRecordAsync(string connectionString, HeaderParameters parameters);

    [StoredProcedure("Test_GetTupleOfRecords")]
    ValueTask<(Record4, Record5)> GetTupleOfRecordsAsync(string connectionString, HeaderParameters parameters);

    [StoredProcedure("Test_GetListAndObject")]
    ValueTask<(List<Record4>, Record5)> GetListAndObjectAsync(string connectionString, CompositeRecord composite);
}

[SpHandler(Lifetime.Singleton)]
public interface ISingletonSpExecutor
{
    [StoredProcedure("Test_GetListByInput")]
    ValueTask<List<Record5>> GetListByInputAsync(string connectionString, Record7 parameters);

    [StoredProcedure("Test_GetTwoLists")]
    ValueTask<(List<CompositeRecord>, List<Record5>)> GetTwoListsAsync(string connectionString);

    [StoredProcedure("Test_GetGenericResponse")]
    ValueTask<GenericSpResponse> GetGenericResponseAsync(string connectionString);

    [StoredProcedure("Test_GetThreeLists")]
    ValueTask<(List<CompositeRecord>, List<Record5>, List<HeaderParameters>)> GetThreeListsAsync(string connectionString);
}

#endregion

#region DTOs

/// <summary>
/// Parameters passed to the primary stored procedures.
/// </summary>
public class HeaderParameters : ISpResponse
{
    public string Name { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
    public double Ratio { get; set; }

    public List<Record4> Record4Items { get; set; } = new();
    public List<AuditInfo> AuditItems { get; set; } = new();

    public byte[] BinaryData { get; set; }
}

public class Record4 : ISpResponse
{
    public string Name { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
    public long LargeNumber { get; set; }
}

public class Record5 : ISpResponse
{
    public string Name { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
}

public class CompositeRecord : ISpResponse
{
    public string Name { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }

    public List<Record4> Record4Items { get; set; } = new();
    public List<Record5> Record5Items { get; set; } = new();
    public List<Record7> Record7Items { get; set; } = new();
}

public class Record7 : ISpResponse
{
    public string Name { get; set; }
    public int Count { get; set; }
    public bool IsActive { get; set; }
}

public class AuditInfo
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

#endregion


/*

using SpExecuter.Utility;

namespace Example;


 
[SpHandler(Lifetime.Scoped)]
public interface IClass1
{
    [StoredProcedure("1stSp")]
    ValueTask<Class4> ExecuteFrom3(string conString, Class3 class3Obj);
    [StoredProcedure("Sp_ExecuteForTupleWithObjects")]
    ValueTask<(Class4, Class5)> ExecuteForTupleWithObjects(string conString, Class3 class3Obj);
    [StoredProcedure("Sp_ExecuteForTupleWithListAndObject")]
    ValueTask<(List<Class4>, Class5)> ExecuteForTuplewitListNObject(string conString, Class6 class6Obj);

}

[SpHandler(Lifetime.Singleton)]
public interface IClass2
{
    [StoredProcedure("Sp_ExecuteFrom7")]
    ValueTask<List<Class5>> ExecuteFrom7(string conString, Class7 class7Obj);
    [StoredProcedure("Sp_ExecuteToLists")]
    ValueTask<(List<Class6>, List<Class5>)> ExecuteToLists(string conString);
    [StoredProcedure("Sp_ExecuteToListsAnother")]
    ValueTask<GenericSpResponse> ExecuteToListsAnother(string conString);
    [StoredProcedure("Sp_ExecuteToLists3")]
    ValueTask<(List<Class6>, List<Class5>,List<Class3>)> ExecuteToLists3(string conString);

}
public class Class3:ISpResponse
{
    public string Class3String { get; set; }
    public int Class3Int { get; set; }
    public bool Class3Bool { get; set; }
    public long Class3long { get; set; }
    public double Class3double { get; set; }

    public List<Class4> Class4List { get; set; } = new List<Class4>();
    public List<Class8> Class8List { get; set; } = new List<Class8>();
    public byte[] Class3Binary { get; set; }
}


public class Class4 : ISpResponse
{
    public string Class4String { get; set; }
    public int Class4Int { get; set; }
    public bool Class4Bool { get; set; }
    public long Class4long { get; set; }

}
public class Class5 : ISpResponse
{
    public string Class5String { get; set; }
    public int Class5Int { get; set; }
    public bool Class5Bool { get; set; }
}
public class Class6 : ISpResponse
{
    public string Class6String { get; set; }
    public int Class6Int { get; set; }
    public bool Class6Bool { get; set; }
    public List<Class4> Class4List { get; set; } = new List<Class4>();
    public List<Class5> Class5List { get; set; } = new List<Class5>();
    public List<Class7> Class7List { get; set; } = new List<Class7>();
 

}
public class Class7 : ISpResponse
{
    public string Class7String { get; set; }
    public int Class7Int { get; set; }
    public bool Class7Bool { get; set; }
}
public class Class8
{
    public string Class8String { get; set; }
    public int Class8Int { get; set; }
    public bool Class8Bool { get; set; }
    public long Class8long { get; set; }
    public double Class8double { get; set; }
    public byte[] Class8Binary { get; set; }
    public DateTime Class8DateTime { get; set; }
    public DateTimeOffset Class8Date { get; set; }
    public TimeSpan Class8TimeSpan { get; set; }


}
*/