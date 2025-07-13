using Example.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using SpExecuter.Utility;
using System.Text;

var services = new ServiceCollection();
services.ConfigureSpExecuter();             // Mandatory Call this method from Your APP

var provider = services.BuildServiceProvider();
var scopedExecutor = provider.GetRequiredService<IScopedSpExecutor>();
var singletonExecutor = provider.GetRequiredService<ISingletonSpExecutor>();


string connectionString = "Server=localhost\\SQLEXPRESS;Database=Test;Trusted_Connection=True; TrustServerCertificate=True; Integrated Security=True;";
var headerOnly = new HeaderInfo
{
    MyName = "OnlyHeader",
    Count = 1,
    IsActive = false,
    LargeNumber = 98765,
    Ratio = 99.99,
    BinaryData = Encoding.UTF8.GetBytes("one-header"),
    OccurredOn = DateTime.UtcNow,
    OccurredAt = DateTimeOffset.UtcNow,
    Duration = TimeSpan.FromMinutes(10)
};
var header = new HeaderParameters
{
    MyName = "Sample Header",
    Count = 10,
    IsActive = true,
    LargeNumber = 1234567890,
    Ratio = 45.67,
    BinaryData = Encoding.UTF8.GetBytes("HelloWorld"),
    OccurredOn = new DateTime(2025, 7, 12, 10, 30, 0),
    OccurredAt = new DateTimeOffset(2025, 7, 12, 10, 30, 0, TimeSpan.FromHours(5.5)),
    Duration = new TimeSpan(2, 15, 30), // 2h 15m 30s

    Record4Items = new List<Record4TableType>
    {
        new Record4TableType
        {
            Name = "Record A",
            Count = 1,
            IsActive = true,
            LargeNumber = 1000
        },
        new Record4TableType
        {
            Name = "Record B",
            Count = 2,
            IsActive = false,
            LargeNumber = 2000
        }
    },

    AuditItems = new List<AuditInfoTableType>
    {
        new AuditInfoTableType
        {
            Description = "Audit Entry 1",
            Count = 5,
            IsActive = true,
            LargeNumber = 5000,
            Ratio = 99.99,
            BinaryData = new byte[] { 0x01, 0x02 },
            OccurredOn = DateTime.Now.Date,
            OccurredAt = DateTimeOffset.Now,
            Duration = TimeSpan.FromMinutes(90)
        },
        new AuditInfoTableType
        {
            Description = "Audit Entry 2",
            Count = 8,
            IsActive = false,
            LargeNumber = 8888,
            Ratio = 75.25,
            BinaryData = new byte[] { 0x03, 0x04 },
            OccurredOn = DateTime.Today.AddDays(-1),
            OccurredAt = DateTimeOffset.Now.AddDays(-1),
            Duration = TimeSpan.FromHours(1)
        }
    }
};




HeaderResult single = await scopedExecutor.GetSingleRecordAsync(connectionString, header);

(HeaderResult h, Record4Result r4, AuditInfoResult a) =
    await scopedExecutor.GetTupleOfRecordsAsync(connectionString, header);

(List<HeaderResult> headers, Record4Result singleRecord4) =
    await scopedExecutor.GetListAndObjectAsync(connectionString, header);

List<HeaderResult> headerList = await singletonExecutor.GetListAsync(connectionString, header);

(List<HeaderResult> headers2, List<Record4Result> record4s2) =
    await singletonExecutor.GetTwoListsAsync(connectionString,header);

GenericSpResponse staticInsertResult = await singletonExecutor.GetGenericResponseAsync(connectionString);


GenericSpResponse insertHeaderResult =
    await singletonExecutor.GetGenericResponseAsync(connectionString, headerOnly);

(List<HeaderResult> hList, List<Record4Result> rList, List<AuditInfoResult> aList) =
    await singletonExecutor.GetThreeListsAsync(connectionString,header);

var resultInStringArray = await SpExecutor.ExecuteSpToStringArray("SaveFullHeaderDetails", connectionString, 1, header);

Console.WriteLine("Test execution completed.");

