using Sigil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;



namespace SpExecuter.Utility
{

    public class DBConstants
    {
        public static int SpRequestClassesCount { get; set; }
        public static int SpResponseClassesCount { get; set; }
        public static Type[] SpRequestModelTypeArray { get; set; } 
        public static Type[] SpResponseModelTypeArray { get; set; }
        public static Dictionary<string, Delegate> tVPsdelegates { get; set; } 

    }
    internal class AppConstants
    {
        #region Constants
        public const string AtTheRate = "@";
        public const string UnderScore = "_";
        public static readonly Type StringType = typeof(string);
        public static readonly Type IntType = typeof(int);
        public static readonly Type IntNullableType = typeof(int?);

        public static readonly Type BoolType = typeof(bool);
        public static readonly Type BoolNullableType = typeof(bool?);

        public static readonly Type LongType = typeof(long);
        public static readonly Type LongNullableType = typeof(long?);

        public static readonly Type DateTimeType = typeof(DateTime);
        public static readonly Type DateTimeNullableType = typeof(DateTime?);

        public static readonly Type DoubleType = typeof(double);
        public static readonly Type DoubleNullableType = typeof(double?);

        public static readonly Type FloatType = typeof(float);
        public static readonly Type FloatNullableType = typeof(float?);

        public static readonly Type ByteArrayType = typeof(byte[]);

        public static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);
        public static readonly Type NullableDateTimeOffsetType = typeof(DateTimeOffset?);

        public static readonly Type TimeSpanType = typeof(TimeSpan);
        public static readonly Type NullableTimeSpanType = typeof(TimeSpan?);

        public static readonly Type DecimalType = typeof(decimal);
        public static readonly Type NullableDecimalType = typeof(decimal?);

        public static Type ListType { get; } = typeof(List<>);
        public static Type IListType { get; } = typeof(IList<>);
        #endregion

        public static PropertyInfo[][] SpRequestPropertyInfoCache { get; set; }
        public static PropertyInfo[][] SpResponsePropertyInfoCache { get; set; }
      
        public static Dictionary<string, Func<object, object>>[] CachedPropertyAccessorDelegates { get; set; }
        public static Dictionary<string, Action<object, object>>[] CachedPropertySetterDelegates { get; set; }

        #region Getter & Setter

        public static Func<object, object> CreateGetter(Type targetType,
            string propertyName, PropertyInfo propertyInfo)
        {
            var emitter = Emit<Func<object, object>>.NewDynamicMethod($"{targetType.Name}{UnderScore}{propertyName}")
                .LoadArgument(0)
                .CastClass(targetType)
                .Call(propertyInfo.GetGetMethod(true));
            if (propertyInfo.PropertyType.IsValueType)
            {
                emitter.Box(propertyInfo.PropertyType);
            }
            emitter.Return();
            return emitter.CreateDelegate();

        }
      /*  public static Action<object, object> CreateSetter(Type targetType,
        string propertyName, PropertyInfo propertyInfo)
        {
            // Prepare a DynamicMethod-backed emitter for Action<object,object>
            var emitter = Emit<Action<object, object>>.NewDynamicMethod(
                    $"{targetType.Name}{UnderScore}{propertyName}")
                // 1st arg: the target instance (as object)
                .LoadArgument(0)
                // cast it to the actual declaring type
                .CastClass(targetType)
                // 2nd arg: the value to set (as object)
                .LoadArgument(1);

            // if the property is a value-type, unbox; otherwise cast
            if (propertyInfo.PropertyType.IsValueType)
                emitter.UnboxAny(propertyInfo.PropertyType);
            else
                emitter.CastClass(propertyInfo.PropertyType);

            // call the property's set method
            emitter.Call(propertyInfo.GetSetMethod(true));

            // return (void)
            emitter.Return();

            // compile and return the delegate
            return emitter.CreateDelegate();
        }*/
        #endregion
    }

    public interface ISpResponse { }
    public class SkipResponse { }
    public class NoRequest { }
    public class GenericSpResponse : ISpResponse
    {
        public int NumberOfRowsAffected { get; set; } = 0;
        public bool IsSuccess { get; set; } = true;
    }
    public class SpExecuterException : Exception
    {
        public string Information { get; set; }

        public SpExecuterException(StringBuilder info, Exception ex) : base(ex.Message, ex)
        {
            Information = info.ToString();
        }
    }
}
