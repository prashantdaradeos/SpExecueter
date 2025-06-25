

using SpExecuter.Utility;

namespace Example;



[SpHandler(Lifetime.Scoped)]
public interface IClass1
{
    [StoredProcedure("1stSp")]
    ValueTask<Class4> ExecuteFrom3(string conString, Class3 class3Obj);
    [StoredProcedure("2ndSp")]
    ValueTask<(Class4, Class5)> ExecuteForTupleWithObjects(string conString, Class3 class3Obj);
    [StoredProcedure("3rdSp")]
    ValueTask<(List<Class4>, Class5)> ExecuteForTuplewitListNObject(string conString, Class6 class6Obj);

}

[SpHandler(Lifetime.Singleton)]
public interface IClass2
{
    [StoredProcedure("3rdSp")]
    ValueTask<List<Class5>> ExecuteFrom7(string conString, Class7 class7Obj);
    [StoredProcedure("3rdSp")]
    ValueTask<(List<Class6>, List<Class5>)> ExecuteToLists(string conString);
    [StoredProcedure("4thSp")]
    ValueTask<GenericSpResponse> ExecuteToListsAnother(string conString);
}
public class Class3:ISpResponse
{
    public string Class3String { get; set; }
    public int Class3Int { get; set; }
    public bool Class3Bool { get; set; }
    public long Class3long { get; set; }
    public double Class3double { get; set; }
    public List<Class4> Class4List { get; set; }
    = new List<Class4>();

    [TVPType(typeof(Class4))]
    public List<string[]> Class4Array { get; set; } = new List<string[]>();
    public byte[] Class3Binary { get; set; }
}

[TVP("Class4TVP")]
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
}
public class Class7 : ISpResponse
{
    public string Class7String { get; set; }
    public int Class7Int { get; set; }
    public bool Class7Bool { get; set; }
}
