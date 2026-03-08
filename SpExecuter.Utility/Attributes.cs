using System;

namespace SpExecuter.Utility
{
   
    //On Interface For registring for Implementation
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class SpHandler : Attribute
    {
        public bool ExcludeIndices { get; set; }
        public SpHandler(Lifetime serviceLifetime)
        {
           
        }
    }
    //On Method for Actual DB call implementation
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StoredProcedure : Attribute
    {
        public bool IsNonQuery { get; set; } 
        public bool UseShort { get; set; }
        public bool ExcludeIndices { get; set; }
        public ConditionType ConditionType { get; set; } = ConditionType.AND;
        public StoredProcedure(string storedProcedureName)
        {

        }
    }
     
    //On property for different parameter name in Sp
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ParamConfig : Attribute
    {
        public string DBParam { get;set; }//Can be applied on IN and OUT parameters

        public bool OutParam { get; set; } = false;//Can be applied on IN parameters
        public bool ParamExclusion { get; set; } = false;//Can be applied on IN parameters

        public bool Unique { get; set; } = false;//Can be applied only on OUT parameters
        public bool ResultExclusion { get; set; } = false;//Can be applied on OUT parameters


     
    }
    //On Class for declaring as Table Valued Parameter (TVP) Name with different schema
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TVP : Attribute
    {
        internal string TVPName { get; set; }
        public bool IncludeInherited { get; set; }

        public TVP(string tVPName)
        {

        }
    }
    [Obsolete("Use DBName from ParamConfig Attribute instead of DbParam Attribute")]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DbParam: Attribute
    {
       
    }
}
