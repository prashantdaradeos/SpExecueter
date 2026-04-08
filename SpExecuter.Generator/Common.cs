using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace SpExecuter.Generator
{
    internal class Common
    {
        internal static void GetInheritanceInfo(
            INamedTypeSymbol mainObject, int inhertanceNo,
            Dictionary<int, INamedTypeSymbol> list)
        {
            if (mainObject == null ||
                mainObject.SpecialType == SpecialType.System_Object)
            {
                return;
            }
            list.Add(inhertanceNo, mainObject);
            mainObject = mainObject.BaseType;
            GetInheritanceInfo(mainObject, ++inhertanceNo, list);
        }
        internal static bool NeedToSkipProperty(SourceProductionContext context,
            Dictionary<int, INamedTypeSymbol> inheritanceInfo,
            IPropertySymbol propertySymbol, int currentIndex)
        {
            currentIndex--;
            for (; currentIndex >= 0; currentIndex--)
            {
                INamedTypeSymbol parameterType = inheritanceInfo[currentIndex];
                foreach (var currentProp in parameterType.GetMembers().OfType<IPropertySymbol>())
                {
                    if (SymbolEqualityComparer.Default.Equals(currentProp.OverriddenProperty, propertySymbol))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        internal static bool IsNestedTuple(INamedTypeSymbol namedType)
        {
            if (namedType.NullableAnnotation == NullableAnnotation.Annotated)
            {
                namedType = namedType.TypeArguments[0] as INamedTypeSymbol;
            }
            if (!namedType.IsTupleType)
            {
                return false;
            }
            foreach (var element in namedType.TupleElements)
            {
                var elementType = element.Type;
                if (elementType.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    elementType = ((INamedTypeSymbol)elementType).TypeArguments[0];
                }
                if (elementType.IsTupleType)
                {
                    return true;
                }
            }
            return false;
        }
        internal static INamedTypeSymbol GetNonNullableType(INamedTypeSymbol namedType)
        {
            if (namedType.NullableAnnotation == NullableAnnotation.Annotated)
            {
                namedType = namedType.TypeArguments[0] as INamedTypeSymbol;
            }
            return namedType;
        }
        internal static INamedTypeSymbol GetInnerType(INamedTypeSymbol namedType)
        {
            if (namedType.NullableAnnotation == NullableAnnotation.Annotated)
            {
                namedType = namedType.TypeArguments[0] as INamedTypeSymbol;
            }
            if (namedType.IsGenericType &&
                namedType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
            {
                var innerType = namedType.TypeArguments[0] as INamedTypeSymbol;
                if (innerType.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    innerType = innerType.TypeArguments[0] as INamedTypeSymbol;
                }
                return innerType;
            }
            return namedType;
        }
        internal static int GetReturnCountFromTuple(Information info, INamedTypeSymbol returnType)
        {
            if (returnType.IsGenericType && returnType.IsTupleType)
            {
                return returnType.TupleElements.Length;
            }
            return 1;
        }
        internal static void Flatten(INamedTypeSymbol symbol,
            List<INamedTypeSymbol> result)
        {
            if (symbol.IsTupleType)
            {
                foreach (var element in symbol.TupleElements)
                {
                    Flatten((INamedTypeSymbol)element.Type, result);
                }
                return;
            }
            result.Add(symbol);
        }
        internal static StringBuilder BindElements(Information info, INamedTypeSymbol returnType, int commonItemNo)
        {
            commonItemNo = commonItemNo + 1;
            StringBuilder builder = new StringBuilder();

            var elementType = returnType;
            if (elementType.TypeKind == TypeKind.Class &&
                elementType.DeclaringSyntaxReferences.Length > 0)
            {
                if (commonItemNo != 1)
                {
                    builder.AppendLine($"         {elementType.Name} itemCommon{commonItemNo} =null;");
                    builder.AppendLine("         if(await reader.NextResultAsync()){");
                }
                else
                {
                    builder.AppendLine($"         {elementType.Name} itemCommon{commonItemNo} =null;");
                }

                builder.AppendLine("              if (await reader.ReadAsync()){");

                builder.AppendLine($"           itemCommon{commonItemNo} = new {elementType.Name}();");
                builder.Append(GenerateSingleClassBinding(info, elementType, "Common" + commonItemNo.ToString()));
                builder.AppendLine("              }");
                if (commonItemNo != 1)
                {
                    builder.AppendLine("         }");
                }

            }
            else if (elementType.IsGenericType &&
                elementType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
            {
                var innerType = elementType.TypeArguments[0] as INamedTypeSymbol;
                if (commonItemNo != 1)
                {
                    builder.AppendLine($"         List<{innerType.Name}> listCommon{commonItemNo} =new List<{innerType.Name}>();");
                    builder.AppendLine("         if(await reader.NextResultAsync()){");
                }
                else
                {
                    builder.AppendLine($"         List<{innerType.Name}> listCommon{commonItemNo} =new List<{innerType.Name}>();");
                }
                builder.AppendLine("         while(await reader.ReadAsync()){");
                builder.AppendLine($"            {innerType.Name} item = new {innerType.Name}();");
                builder.Append(GenerateSingleClassBinding(info, innerType, ""));
                builder.AppendLine($"            listCommon{commonItemNo}.Add(item);");
                builder.AppendLine("             }");
                if (commonItemNo != 1)
                {
                    builder.AppendLine("         }");
                }
            }

            return builder;
        }
        internal static StringBuilder BindListElements(ConditionInfo info, 
            INamedTypeSymbol returnType, int varName,
            bool readNextResult,List<List<string>> varNames,
            List<List<INamedTypeSymbol>> symbol)
        {
            
            StringBuilder builder = new StringBuilder();

            var innerType = GetNonNullableType( returnType);
             innerType = GetInnerType( returnType);
           // List<int> horIndexes= GetHorizontalIndexes(info, innerType,varName,builder);
            if (readNextResult)
            {
                builder.AppendLine("if(await reader.NextResultAsync()){");
            }
                builder.AppendLine($"List<{innerType.Name}> el{varName} =new List<{innerType.Name}>();");
                builder.AppendLine("while(await reader.ReadAsync()){");
                builder.AppendLine($"{innerType.Name} item = new {innerType.Name}();");
                builder.Append(GenerateSingleClassBinding(info, innerType, ""));
                builder.AppendLine($"el{varName}.Add(item);");
                builder.AppendLine("}");
          
         
            foreach(var list in symbol)
            { 
                int key = info.FlattenedReturnTypes
                              .FirstOrDefault(x =>
                                  x.Value.SequenceEqual(list, SymbolEqualityComparer.Default))
                              .Key;
                string lhs = varNames[key][varName];
                string rhs = lhs.StartsWith("item") ? $"el{varName}.FirstOrDefault()" : $"el{varName}";
                builder.AppendLine($"{lhs} = {rhs};");
                if (info.FlattenedReturnTypes[key].Count == varName + 1)
                {
                    builder.AppendLine($"returnItem{key + 1} = true;");
                }
            }
          

            if (readNextResult)
            {
                builder.AppendLine("}");
            }
            return builder;
        }
        internal static List<int> GetHorizontalIndexes(ConditionInfo info,INamedTypeSymbol symbol,
            int position,StringBuilder builder)
        {
            int i = 0;
            List<int> indexes = new List<int>();
            foreach(var current in info.FlattenedReturnTypes.Values)
            {
                if (current == null || position >= current.Count)
                {
                    i++;
                    continue;
                }
                var currentSymbol = current[position];
                currentSymbol = GetNonNullableType(currentSymbol);
                currentSymbol = GetInnerType(currentSymbol);
                if( SymbolEqualityComparer.Default.Equals(symbol, currentSymbol))
                {
                    indexes.Add(i);
                }
                i++;
            }
            return indexes;
        }
        internal static StringBuilder GenerateSingleClassBinding(Information info,
          INamedTypeSymbol classSymbol, string suffix, params int[] uniqueNumbers)
        {
            Dictionary<int, INamedTypeSymbol> inheritanceInfo = new Dictionary<int, INamedTypeSymbol>();
            Common.GetInheritanceInfo(classSymbol, 0, inheritanceInfo);
            string dbParamName = string.Empty;
            StringBuilder builder = new StringBuilder();
            var parameterType = inheritanceInfo[0];


            for (int i = inheritanceInfo.Count - 1; i >= 0; i--)
            {
                parameterType = inheritanceInfo[i];
                foreach (var propertySymbol in parameterType.GetMembers().OfType<IPropertySymbol>())
                {

                    if (Common.NeedToSkipProperty(info.Context, inheritanceInfo, propertySymbol, i))
                    {
                        continue;
                    }
                    //string parameterName = propertySymbol.Name;
                    var paramConfigAttr = propertySymbol
                        .GetAttributes()
                        .FirstOrDefault(attr => attr.AttributeClass.Name.Contains("ParamConfig") ||
                            attr.AttributeClass.ToDisplayString().Contains("SpExecuter.Utility.ParamConfig"));
                    bool exclude = false;
                    dbParamName = propertySymbol.Name;
                    if (paramConfigAttr != null)
                    {

                        var excludeArg = paramConfigAttr.NamedArguments
                                .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.ResultExclusion));
                        bool excludeArgValue = Convert.ToBoolean(excludeArg.Value.Value);
                        if (excludeArgValue)
                        {
                            continue;
                        }

                        var outParamArg = paramConfigAttr.NamedArguments
                                .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.OutParam));
                        bool outParamArgValue = Convert.ToBoolean(outParamArg.Value.Value);
                        if (outParamArgValue)
                        {
                            continue;
                        }

                        var dbParamArg = paramConfigAttr.NamedArguments
                                .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.DBParam));
                        string tempName = dbParamArg.Key != null ? Convert.ToString(dbParamArg.Value.Value) : "";
                        if (!string.IsNullOrWhiteSpace(tempName))
                        {
                            dbParamName = tempName;
                        }

                    }

                    string ordinalVariableName = $"SP{info.MethodSequenceNo}_Base{i}_{string.Join("", uniqueNumbers)}_{propertySymbol.Name}";
                    builder.AppendLine($"                   item{suffix}.{propertySymbol.Name} = {GetPropertyType(info, propertySymbol, dbParamName, ordinalVariableName)}");


                }

            }


            return builder;
        }
        internal static string GetPropertyType(Information info,
            IPropertySymbol prop, string parameterName, string ordinalVariableName)
        {

            var propType = prop.Type;
            var annotation = prop.NullableAnnotation;
            bool isNullable = annotation == NullableAnnotation.Annotated;

            if (isNullable && propType is not IArrayTypeSymbol)
            {
                propType = ((INamedTypeSymbol)propType).TypeArguments[0];
            }
            switch (propType.SpecialType)

            {
                case SpecialType.System_String:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\"))   ? null : Convert.ToString(reader.GetValue(reader.GetOrdinal(\"{parameterName}\")));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName})? null:Convert.ToString(reader.GetValue({ordinalVariableName}));";

                case SpecialType.System_Int32:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\"))   ? {(isNullable ? "(int?)null" : "0")} : reader.GetInt32(reader.GetOrdinal(\"{parameterName}\"));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName})? {(isNullable ? "(int?)null" : "0")}:reader.GetInt32({ordinalVariableName});";

                case SpecialType.System_DateTime:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\"))   ? {(isNullable ? "(DateTime?)null" : "DateTime.MinValue")} : reader.GetDateTime(reader.GetOrdinal(\"{parameterName}\"));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(DateTime?)null" : "DateTime.MinValue")} : reader.GetDateTime({ordinalVariableName});";

                case SpecialType.System_Boolean:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\"))   ? {(isNullable ? "(bool?)null" : "false")} : reader.GetBoolean(reader.GetOrdinal(\"{parameterName}\"));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(bool?)null" : "false")} : reader.GetBoolean({ordinalVariableName});";

                case SpecialType.System_Int64:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\"))   ? {(isNullable ? "(long?)null" : "0L")}: reader.GetInt64(reader.GetOrdinal(\"{parameterName}\"));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(long?)null" : "0L")} : reader.GetInt64({ordinalVariableName});";

                case SpecialType.System_Decimal:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\"))   ? {(isNullable ? "(decimal?)null" : "0m")}: reader.GetDecimal(reader.GetOrdinal(\"{parameterName}\"));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(decimal?)null" : "0m")} : reader.GetDecimal({ordinalVariableName});";

                case SpecialType.System_Int16:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\"))   ?  {(isNullable ? "(short?)null" : "default")} : reader.GetInt16(reader.GetOrdinal(\"{parameterName}\"));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(short?)null" : "default")} : reader.GetInt16({ordinalVariableName});";


                case SpecialType.System_Single:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\"))   ?   {(isNullable ? "(float?)null" : "0f")}  : reader.GetFloat(reader.GetOrdinal(\"{parameterName}\"));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(float?)null" : "0f")} : reader.GetFloat({ordinalVariableName});";

                case SpecialType.System_Double:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\"))   ?   {(isNullable ? "(double?)null" : "0d")} : reader.GetDouble(reader.GetOrdinal(\"{parameterName}\"));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(double?)null" : "0d")} : reader.GetDouble({ordinalVariableName});";
                case SpecialType.System_Byte:
                    if (info.ExcludeIndices)
                    {
                        return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(byte?)null" : "default")} : reader.GetByte(reader.GetOrdinal(\"{parameterName}\"));";
                    }
                    return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(byte?)null" : "default")} : reader.GetByte({ordinalVariableName});";
                case SpecialType.System_Char:
                    if (info.ExcludeIndices)
                    {
                        return
                            $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) " +
                            $"? {(isNullable ? "(char?)null" : "'\\0'")} " +
                            $": reader.GetString(reader.GetOrdinal(\"{parameterName}\"))[0];";
                    }
                    return
                        $"reader.IsDBNull({ordinalVariableName}) " +
                        $"? {(isNullable ? "(char?)null" : "'\\0'")} " +
                        $": reader.GetString({ordinalVariableName})[0];";
                case SpecialType.System_SByte:
                    {
                        if (info.ExcludeIndices)
                        {
                            return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(sbyte?)null" : "default")} : Convert.ToSByte(reader.GetInt16(reader.GetOrdinal(\"{parameterName}\")));";
                        }
                        return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(sbyte?)null" : "default")} : Convert.ToSByte(reader.GetInt16({ordinalVariableName}));";
                    }

                case SpecialType.System_UInt16:
                    {
                        if (info.ExcludeIndices)
                        {
                            return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(ushort?)null" : "default")} : Convert.ToUInt16(reader.GetInt32(reader.GetOrdinal(\"{parameterName}\")));";
                        }
                        return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(ushort?)null" : "default")} : Convert.ToUInt16(reader.GetInt32({ordinalVariableName}));";
                    }
                case SpecialType.System_UInt32:
                    {
                        if (info.ExcludeIndices)
                        {
                            return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(uint?)null" : "0")} : Convert.ToUInt32(reader.GetInt64(reader.GetOrdinal(\"{parameterName}\")));";
                        }
                        return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(uint?)null" : "0")} : Convert.ToUInt32(reader.GetInt64({ordinalVariableName}));";
                    }
                case SpecialType.System_UInt64:
                    {
                        if (info.ExcludeIndices)
                        {
                            return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(ulong?)null" : "0")} : Convert.ToUInt64(reader.GetDecimal(reader.GetOrdinal(\"{parameterName}\")));";
                        }
                        return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(ulong?)null" : "0")} : Convert.ToUInt64(reader.GetDecimal({ordinalVariableName}));";
                    }
                case SpecialType.System_IntPtr:
                    {
                        if (info.ExcludeIndices)
                        {
                            return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(IntPtr?)null" : "IntPtr.Zero")} : new IntPtr(reader.GetInt64(reader.GetOrdinal(\"{parameterName}\")));";
                        }
                        return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(IntPtr?)null" : "IntPtr.Zero")} : new IntPtr(reader.GetInt64({ordinalVariableName}));";
                    }
                case SpecialType.System_UIntPtr:
                    {
                        if (info.ExcludeIndices)
                        {
                            return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(UIntPtr?)null" : "UIntPtr.Zero")} : new UIntPtr((ulong)reader.GetDecimal(reader.GetOrdinal(\"{parameterName}\")));";
                        }
                        return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(UIntPtr?)null" : "UIntPtr.Zero")} : new UIntPtr((ulong)reader.GetDecimal({ordinalVariableName}));";
                    }
                case SpecialType.None:
                    {
                        if (prop.Type is IArrayTypeSymbol arrayType)
                        {
                            if (arrayType.ElementType.SpecialType == SpecialType.System_Byte)
                            {

                                if (info.ExcludeIndices)
                                {
                                    return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? null : (byte[])reader.GetValue(reader.GetOrdinal(\"{parameterName}\"));";
                                }
                                return $"reader.IsDBNull({ordinalVariableName}) ? null : (byte[])reader.GetValue({ordinalVariableName});";
                            }
                            else if (arrayType.ElementType.SpecialType == SpecialType.System_Char)
                            {

                                if (info.ExcludeIndices)
                                {
                                    return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? null : reader.GetString(reader.GetOrdinal(\"{parameterName}\")).ToCharArray();";
                                }
                                return $"reader.IsDBNull({ordinalVariableName}) ? null : reader.GetString({ordinalVariableName}).ToCharArray();";
                            }
                        }
                        else if (propType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals("global::System.DateTimeOffset"))
                        {
                            if (info.ExcludeIndices)
                            {
                                return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(DateTimeOffset?)null" : "default")} : reader.GetDateTimeOffset(reader.GetOrdinal(\"{parameterName}\"));";
                            }
                            return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(DateTimeOffset?)null" : "default")} : reader.GetDateTimeOffset({ordinalVariableName});";

                        }
                        else if (propType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals("global::System.TimeSpan"))
                        {
                            if (info.ExcludeIndices)
                            {
                                return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(TimeSpan?)null" : "TimeSpan.Zero")} : (TimeSpan)reader.GetValue(reader.GetOrdinal(\"{parameterName}\"));";
                            }
                            return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(TimeSpan?)null" : "TimeSpan.Zero")} : (TimeSpan)reader.GetValue({ordinalVariableName});";

                        }
                        else if (propType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals("global::System.Guid"))
                        {
                            if (info.ExcludeIndices)
                            {
                                return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? "(Guid?)null" : "Guid.Empty")} : reader.GetGuid(reader.GetOrdinal(\"{parameterName}\"));";
                            }
                            return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? "(Guid?)null" : "Guid.Empty")} : reader.GetGuid({ordinalVariableName});";
                        }
                        else if (propType.TypeKind == TypeKind.Enum)
                        {
                            var enumType = prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            string nonNullableType= enumType;
                            if (isNullable)
                            {
                                nonNullableType = nonNullableType.Replace("?", "");
                            }
                            if (info.ExcludeIndices)
                            {
                                return $"reader.IsDBNull(reader.GetOrdinal(\"{parameterName}\")) ? {(isNullable ? $"({enumType})null" : $"default({enumType})")} : ({enumType})Enum.Parse(typeof({nonNullableType}), reader.GetString(reader.GetOrdinal(\"{parameterName}\")));";
                            }
                            return $"reader.IsDBNull({ordinalVariableName}) ? {(isNullable ? $"({enumType})null" : $"default({enumType})")} : ({enumType})Enum.Parse(typeof({nonNullableType}), reader.GetString({ordinalVariableName}));";
                        }
                        return "//" + propType.TypeKind + propType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + " Not supported";
                    }




            }


            return "//Not supported";
        }


    
     internal static string GetUniqueProperty(Information info,INamedTypeSymbol namedType)
        {
            if (namedType == null)
            {
                return null;
            }
            var elementType = Common.GetNonNullableType(namedType);
            //isClass = false;
            if (elementType.TypeKind == TypeKind.Class &&
                    elementType.DeclaringSyntaxReferences.Length > 0)
            {
               // isClass = true;
                return GetUniquePropertyName(info,elementType);
            }
            else if (elementType.IsGenericType &&
                elementType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
            {
                return GetUniquePropertyName(info,elementType.TypeArguments[0] as INamedTypeSymbol);
            }
            return string.Empty;
        }
        private static string GetUniquePropertyName(Information info,INamedTypeSymbol namedType)
        {
            string uniquePropertyName = string.Empty;
            var elementType = Common.GetNonNullableType(namedType);
            Dictionary<int, INamedTypeSymbol> inheritanceInfo = new Dictionary<int, INamedTypeSymbol>();
            Common.GetInheritanceInfo(elementType, 0, inheritanceInfo);
            for (int i = inheritanceInfo.Count - 1; i >= 0; i--)
            {
                var parameterType = inheritanceInfo[i];

                foreach (IPropertySymbol propertySymbol in parameterType.GetMembers().OfType<IPropertySymbol>())
                {
                    if (Common.NeedToSkipProperty(info.Context, inheritanceInfo, propertySymbol, i))
                    {
                        continue;
                    }
                    var paramConfigAttr = propertySymbol.GetAttributes()
                        .FirstOrDefault(attr => attr.AttributeClass.Name.Contains("ParamConfig") ||
                            attr.AttributeClass.ToDisplayString().Contains("SpExecuter.Utility.ParamConfig"));
                    if (paramConfigAttr == null)
                    {
                        continue;
                    }
                    var uniqueArg = paramConfigAttr.NamedArguments
                                    .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.Unique));
                    bool uniqueArgValue = Convert.ToBoolean(uniqueArg.Value.Value);
                    if (uniqueArgValue)
                    {
                        uniquePropertyName= propertySymbol.Name;
                    }
                    else
                    {
                                               continue;
                    }
                    var dbParamArg = paramConfigAttr.NamedArguments
                            .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.DBParam));
                    string dbParamArgValue = Convert.ToString(dbParamArg.Value.Value);
                    if (!string.IsNullOrWhiteSpace(dbParamArgValue))
                    {
                        uniquePropertyName = dbParamArgValue;
                    }
                    return uniquePropertyName;
                }
            }
           
            return string.Empty;
        }
        internal static void CheckElementsSameOrNot(ConditionInfo conditionInfo)
        {
            conditionInfo.SameElementsListAgnostic = 0;
            bool allHaveMoreThanSame =conditionInfo.FlattenedReturnTypes.Values.All(list => list.Count > conditionInfo.SameElementsInNestedTuple);
            if(!allHaveMoreThanSame)
            {
                return;
            }
            int keyWithSmallestList = conditionInfo.FlattenedReturnTypes
                                    .OrderBy(kvp => kvp.Value.Count)
                                    .First()
                                    .Key; 
            for (int i=conditionInfo.SameElementsInNestedTuple;i < conditionInfo.FlattenedReturnTypes[keyWithSmallestList].Count; i++)
            {
                INamedTypeSymbol namedType= conditionInfo.FlattenedReturnTypes[keyWithSmallestList][i];
                namedType= GetNonNullableType(namedType);
                namedType= GetInnerType(namedType);
                bool allMatch = true;
                for(int j=0;j< conditionInfo.FlattenedReturnTypes.Count;j++)
                {
                    if(j== keyWithSmallestList)
                    {
                        continue;
                    }

                    var otherNamedType = conditionInfo.FlattenedReturnTypes[j][i];
                    otherNamedType = GetNonNullableType(otherNamedType);
                    otherNamedType = GetInnerType(otherNamedType);
                    if (!SymbolEqualityComparer.Default.Equals(namedType, otherNamedType))
                    {
                        allMatch = false;
                        break;
                    }
                }
                if (!allMatch) {
                    break;
                }
                else
                {
                    conditionInfo.SameElementsListAgnostic++;
                }
            }
        }
    } 
}
