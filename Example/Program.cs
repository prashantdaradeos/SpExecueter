// See https://aka.ms/new-console-template for more information
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

string conString = "Server=localhost\\SQLEXPRESS;Database=Test;Trusted_Connection=True; TrustServerCertificate=True; Integrated Security=True;";




var class3Obj = new Class3
{
    Class3String = "demo-C3",
    Class3Int = 123,
    Class3Bool = true,
    Class3long = 999999,
    Class3double = 12.34,
    Class3Binary = Encoding.UTF8.GetBytes("hello"),

   /* Class4List = new()
    {
        new Class4
        {
            Class4String = "row-1",
            Class4Int    = 1,
            Class4Bool   = false,
            Class4long   = 100
        }
    },*/

    // each string[] must have exactly the columns your TVP will expect
   /* Class4Array = new()
    {
        new[] { "col1", "col2", "col3", "col4" }
    }*/
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

Console.WriteLine("");