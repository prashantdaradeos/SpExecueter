 using Example;
using Example.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SpExecuter.Utility;
using System.Text;

var services = new ServiceCollection();
services.ConfigureSpExecuter();             // Mandatory Call this method from Your APP

var provider = services.BuildServiceProvider();
var transientExecutor = provider.GetRequiredService<ITransientSpExecutor>();


string connectionString = "Server=localhost\\SQLEXPRESS;Database=Test;Trusted_Connection=True; TrustServerCertificate=True; Integrated Security=True;";




try
{
    // 1. Test1_With_Single
    var res1 = await transientExecutor.Test1_With_Single(connectionString, AllObjects.CreateInBaseTestClass());
    Console.WriteLine("Test1_With_Single: " + JsonConvert.SerializeObject(res1));

    // 2. Test2_With_List
    var res2 = await transientExecutor.Test2_With_List(connectionString, AllObjects.CreateTestClass111());
    Console.WriteLine("Test2_With_List: " + JsonConvert.SerializeObject(res2));

    // 3. Test3_With_TupleContainingSingle
    var res3 = await transientExecutor.Test3_With_TupleContainingSingle(connectionString);
    Console.WriteLine("Test3_With_TupleContainingSingle: " + JsonConvert.SerializeObject(res3));

    // 4. Test4_With_TupleContaing2List
    var res4 = await transientExecutor.Test4_With_TupleContaing2List(connectionString, AllObjects.CreateTestClass111ForTest4());
    Console.WriteLine("Test4_With_TupleContaing2List: " + JsonConvert.SerializeObject(res4));

    // 5. Test5_With_TupleContaingSingleNList
    var res5 = await transientExecutor.Test5_With_TupleContaingSingleNList(connectionString, AllObjects.CreateTestClass111ForTest4());
    Console.WriteLine("Test5_With_TupleContaingSingleNList: " + JsonConvert.SerializeObject(res5));

    // 6. Test6_With_Tuple_N_Single (HeaderId=1: tuple, HeaderId=2: single)
    var res6_1 = await transientExecutor.Test6_With_Tuple_N_Single(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test6_With_Tuple_N_Single (HeaderId=1): " + JsonConvert.SerializeObject(res6_1));
    var res6_2 = await transientExecutor.Test6_With_Tuple_N_Single(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test6_With_Tuple_N_Single (HeaderId=2): " + JsonConvert.SerializeObject(res6_2));

    // 7. Test7_With_Tuple_N_List (HeaderId=1: tuple, HeaderId=2: list)
    var res7_1 = await transientExecutor.Test7_With_Tuple_N_List(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test7_With_Tuple_N_List (HeaderId=1): " + JsonConvert.SerializeObject(res7_1));
    var res7_2 = await transientExecutor.Test7_With_Tuple_N_List(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test7_With_Tuple_N_List (HeaderId=2): " + JsonConvert.SerializeObject(res7_2));

    // 8. Test8_With_Tuple_Single_List (HeaderId=1: tuple1, 2: tuple2, 3: tuple3)
    var res8_1 = await transientExecutor.Test8_With_Tuple_Single_List(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test8_With_Tuple_Single_List (HeaderId=1): " + JsonConvert.SerializeObject(res8_1));
    var res8_2 = await transientExecutor.Test8_With_Tuple_Single_List(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test8_With_Tuple_Single_List (HeaderId=2): " + JsonConvert.SerializeObject(res8_2));
    var res8_3 = await transientExecutor.Test8_With_Tuple_Single_List(connectionString, AllObjects.CreateTestClass111ForTest6(3));
    Console.WriteLine("Test8_With_Tuple_Single_List (HeaderId=3): " + JsonConvert.SerializeObject(res8_3));

    // 9. Test9_With_2Tuple_Single (HeaderId=1: tuple1, 2: tuple2, 3: list)
    var res9_1 = await transientExecutor.Test9_With_2Tuple_Single(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test9_With_2Tuple_Single (HeaderId=1): " + JsonConvert.SerializeObject(res9_1));
    var res9_2 = await transientExecutor.Test9_With_2Tuple_Single(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test9_With_2Tuple_Single (HeaderId=2): " + JsonConvert.SerializeObject(res9_2));
    var res9_3 = await transientExecutor.Test9_With_2Tuple_Single(connectionString, AllObjects.CreateTestClass111ForTest6(3));
    Console.WriteLine("Test9_With_2Tuple_Single (HeaderId=3): " + JsonConvert.SerializeObject(res9_3));

    // 10. Test10_With_OR (HeaderId=1: List<HeaderResult11>, HeaderId=2: TestClass1)
    var res10_1 = await transientExecutor.Test10_With_OR(connectionString, AllObjects.CreateTestClass111ForTest6(1));
    Console.WriteLine("Test10_With_OR (HeaderId=1): " + JsonConvert.SerializeObject(res10_1));
    var res10_2 = await transientExecutor.Test10_With_OR(connectionString, AllObjects.CreateTestClass111ForTest6(2));
    Console.WriteLine("Test10_With_OR (HeaderId=2): " + JsonConvert.SerializeObject(res10_2));
}
catch (SpExecuterException ex)
{
    //Log, Debugg with information
   string extraInfo= ex.Information;
}
Console.WriteLine("Test execution completed.");

