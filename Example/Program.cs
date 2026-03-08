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




try
{
  
    //Before calling this method build at least Once to get IntelliSense for request object number
    //var resultInStringArray = await SpExecutor.ExecuteSpToStringArray("SaveFullHeaderDetails", connectionString, SpRequest.HeaderParameters, header);
}
catch (SpExecuterException ex)
{
    //Log, Debugg with information
   string extraInfo= ex.Information;
}
Console.WriteLine("Test execution completed.");

