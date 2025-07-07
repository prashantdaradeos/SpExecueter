// See https://aka.ms/new-console-template for more information
using Example.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using SpExecuter.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

var services = new ServiceCollection();
services.ConfigureSpExecuter();

// Build provider and resolve executors
var provider = services.BuildServiceProvider();
var scopedExecutor = provider.GetRequiredService<IScopedSpExecutor>();
var singletonExecutor = provider.GetRequiredService<ISingletonSpExecutor>();

string connectionString = "Server=localhost\\SQLEXPRESS;Database=Test;Trusted_Connection=True; TrustServerCertificate=True; Integrated Security=True;";

// Sample input object
var inputHeader = new HeaderParameters
{
    Name = "demo-header",
    Count = 123,
    IsActive = true,
    LargeNumber = 999999,
    Ratio = 12.34,
    BinaryData = Encoding.UTF8.GetBytes("hello"),
    Record4Items = new()
    {
        new Record4 { Name = "row-1", Count = 1, IsActive = false, LargeNumber = 100 },
        new Record4 { Name = "row-2", Count = 2, IsActive = true, LargeNumber = 200 },
    },
    AuditItems = new()
    {
        new AuditInfo
        {
            Description = "Sample Text",
            Count = 42,
            IsActive = true,
            LargeNumber = 1234567890L,
            Ratio = 98.76,
            BinaryData = new byte[] { 0x01, 0x02, 0x03 },
            OccurredOn = DateTime.Now,
            OccurredAt = DateTimeOffset.Now,
            Duration = new TimeSpan(2, 30, 0)
        }
    }
};

var compositeInput = new CompositeRecord
{
    Name = "composite-test",
    Count = 42,
    IsActive = false
};

var filterRecord = new Record7
{
    Name = "filter-test",
    Count = 77,
    IsActive = true
};

// Scoped executor calls
(Record4 rec4, Record5 rec5) =
    await scopedExecutor.GetTupleOfRecordsAsync(connectionString, inputHeader);

(List<Record4> listRec4, Record5 singleRec5) =
    await scopedExecutor.GetListAndObjectAsync(connectionString, compositeInput);

// Singleton executor calls
List<Record5> filteredList =
    await singletonExecutor.GetListByInputAsync(connectionString, filterRecord);

(List<CompositeRecord> listComposite, List<Record5> listRec5) =
    await singletonExecutor.GetTwoListsAsync(connectionString);

GenericSpResponse genericResponse =
    await singletonExecutor.GetGenericResponseAsync(connectionString);

(List<CompositeRecord> listComposite2, List<Record5> listRec5b, List<HeaderParameters> listHeaderParams) =
    await singletonExecutor.GetThreeListsAsync(connectionString);

Console.WriteLine("Test execution completed.");



/*// See https://aka.ms/new-console-template for more information
using Example;
using Microsoft.Extensions.DependencyInjection;
using SpExecuter.Utility;
using System.Data;
using System.Text;



var services = new ServiceCollection();
services.ConfigureSpExecuter();
// 2) Resolve services from the built IServiceProvider
var provider = services.BuildServiceProvider();
var class1 = provider.GetRequiredService<IClass1>();
var class2 = provider.GetRequiredService<IClass2>();
var a=DBConstants.tVPsdelegates;
string conString = "Server=localhost\\SQLEXPRESS;Database=Test;Trusted_Connection=True; TrustServerCertificate=True; Integrated Security=True;";




var class3Obj = new Class3
{
    Class3String = "demo-C3",
    Class3Int = 123,
    Class3Bool = true,
    Class3long = 999999,
    Class3double = 12.34,
    Class3Binary = Encoding.UTF8.GetBytes("hello"),

    Class4List = new()
    {
        new Class4
        {
            Class4String = "row-1",
            Class4Int    = 1,
            Class4Bool   = false,
            Class4long   = 100
        },
          new Class4
        {
            Class4String = "row-2",
            Class4Int    = 2,
            Class4Bool   = true,
            Class4long   = 200
        },
    },

    Class8List  = new ()
    {new (){
        Class8String = "Sample Text",
        Class8Int = 42,
        Class8Bool = true,
        Class8long = 1234567890L,
        Class8double = 98.76,
        Class8Binary = new byte[] { 0x01, 0x02, 0x03 },
        Class8DateTime = DateTime.Now,
        Class8Date = DateTimeOffset.Now,
        Class8TimeSpan = new TimeSpan(2, 30, 0) // 2 hours, 30 minutes
    } }

};

// ----- b) For ExecuteForTuplewitListNObject -----------------
var class6Obj = new Class6
{
    Class6String = "demo-C6",
    Class6Int = 42,
    Class6Bool = false
};

// ----- c) For ExecuteFrom7 ----------------------------------
var class7Obj = new Class7
{
    Class7String = "demo-C7",
    Class7Int = 77,
    Class7Bool = true
};

(Class4 c4, Class5 c5) =
       await class1.ExecuteForTupleWithObjects(conString, class3Obj);

(List<Class4> listC4, Class5 singleC5) =
    await class1.ExecuteForTuplewitListNObject(conString, class6Obj);

// IClass2 methods
List<Class5> listC5 =
    await class2.ExecuteFrom7(conString, class7Obj);

(List<Class6> listC6, List<Class5> listC5b) =
    await class2.ExecuteToLists(conString);

GenericSpResponse generic =
    await class2.ExecuteToListsAnother(conString);

(List<Class6> listC6b, List<Class5> listC5c, List<Class3> listC3) =
    await class2.ExecuteToLists3(conString);

Console.WriteLine("");*/