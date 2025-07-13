using System;

namespace SpExecuter.Utility
{
    //On Interface For registring for Implementation
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class SpHandler : Attribute
    {
        private readonly Lifetime LifeTime;
        public SpHandler(Lifetime serviceLifetime)
        {
            LifeTime = serviceLifetime;
        }
    }
    //On Method for Actual DB call implementation
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StoredProcedure : Attribute
    {
        private readonly string StoredProcedureName;
        public StoredProcedure(string storedProcedureName)
        {
            StoredProcedureName = storedProcedureName;
        }
    }
     //On Class for declaring as Table Valued Parameter (TVP) Name with different schema
     [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
     public sealed class TVP : Attribute
     {
         internal string TVPName;
         public TVP(string tVPName)
         {
             TVPName = tVPName;
         }
     }
    //On property for different parameter name in Sp
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DbParam : Attribute
    {
        internal string DbParamName;
        public DbParam(string dbParamName)
        {
            DbParamName = dbParamName;
        }
    }

}
