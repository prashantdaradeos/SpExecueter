using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace SpExecuter.Generator
{
    internal sealed class InputParameterGenerator
    {
        internal static void GenerateOrdinals(Information info, StringBuilder methodImpl)
        {
            if (info.ExcludeIndicesClassLevel || info.ExcludeIndicesMethodLevel)
            {
                return;
            }
            INamedTypeSymbol returnType = info.MethodSymbol.ReturnType as INamedTypeSymbol;
            returnType = returnType.TypeArguments[0] as INamedTypeSymbol;
            List<string> ordinals = new List<string>();
            int methodSequenceNo = info.MethodSequenceNo;
            CreateAllIndices(info.Context, returnType, ordinals,
               info.UseShort, methodSequenceNo, 1);
            ordinals.ForEach(ordinal =>
            {
                methodImpl.AppendLine($" {ordinal}");
            });
        }
        private static void CreateAllIndices(SourceProductionContext context,
                 INamedTypeSymbol namedSym, List<string> ordinals,
                 bool saveIndicesInShort,
                 int methodNo, params int[] uniqueNumbers)
        {

            if (namedSym == null)
                return;


            if (namedSym.IsTupleType)
            {
                int i = 1;
                foreach (var element in namedSym.TupleElements)
                {
                    List<int> list = uniqueNumbers.ToList<int>();
                    list.Add(i);
                    CreateAllIndices(context, element.Type as INamedTypeSymbol,
                        ordinals, saveIndicesInShort, methodNo, list.ToArray());
                    i++;
                }
                return;
            }

            if (namedSym.IsGenericType)
            {

                CreateAllIndices(context,
                    namedSym.TypeArguments[0] as INamedTypeSymbol,
                    ordinals, saveIndicesInShort, methodNo, uniqueNumbers);

                return;
            }


            if (namedSym.TypeKind == TypeKind.Class &&
                namedSym.DeclaringSyntaxReferences.Length > 0)
            {
                Dictionary<int, INamedTypeSymbol> inheritanceInfo = new Dictionary<int, INamedTypeSymbol>();
                Common.GetInheritanceInfo(namedSym,
                    0, inheritanceInfo);
                for (int i = inheritanceInfo.Count - 1; i >= 0; i--)
                {
                    var parameterType = inheritanceInfo[i];

                    foreach (var propertySymbol in parameterType.GetMembers().OfType<IPropertySymbol>())
                    {
                        if (Common.NeedToSkipProperty(context, inheritanceInfo, propertySymbol, i))
                        {
                            continue;
                        }
                        string parameterName = propertySymbol.Name;
                        var paramConfigAttr = propertySymbol
                            .GetAttributes()
                            .FirstOrDefault(attr => attr.AttributeClass.Name.Contains("ParamConfig") ||
                                attr.AttributeClass.ToDisplayString().Contains("SpExecuter.Utility.ParamConfig"));
                        bool exclude = false;
                        if (paramConfigAttr != null)
                        {

                            var excludeArg = paramConfigAttr.NamedArguments
                                    .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.ResultExclusion));
                            bool excludeArgValue = Convert.ToBoolean(excludeArg.Value.Value);
                            if (excludeArgValue)
                            {
                                exclude = excludeArgValue;
                            }
                        }
                        if (!exclude)
                        {
                            string datatype = saveIndicesInShort ? "short" : "byte";
                            ordinals.Add($"  private static {datatype}  SP{methodNo}_Base{i}_{string.Join("", uniqueNumbers)}_{propertySymbol.Name} {{get;set;}}");
                        }


                    }
                }
                return;
            }
        }
        internal static StringBuilder GenerateSQLParameters(Information info, RequestProp req)
        {
            StringBuilder sqlParams = new StringBuilder(string.Empty);
            Dictionary<int, INamedTypeSymbol> inheritanceInfo = new Dictionary<int, INamedTypeSymbol>();
            info.SqlInParamName = string.Empty;
            if (info.MethodSymbol.Parameters.Length > 1)
            {
                info.SqlInParamName = info.MethodSymbol.Parameters[1].Name;
                Common.GetInheritanceInfo(info.MethodSymbol.Parameters[1].Type as INamedTypeSymbol,
                    0, inheritanceInfo);
            }
            for (int i = inheritanceInfo.Count - 1; i >= 0; i--)
            {
                var parameterType = inheritanceInfo[i];

                foreach (var propertySymbol in parameterType.GetMembers().OfType<IPropertySymbol>())
                {
                    if (Common.NeedToSkipProperty(info.Context, inheritanceInfo, propertySymbol, i))
                    {
                        continue;
                    }
                    req.PropertySymbol = propertySymbol;
                    req.DBParam = req.ParameterName = propertySymbol.Name;
                    req.OutParam = false;
                    req.OutParamLength = 0;


                    var paramConfigAttr = req.PropertySymbol
                    .GetAttributes()
                    .FirstOrDefault(attr => attr.AttributeClass.Name.Contains("ParamConfig") ||
                        attr.AttributeClass.ToDisplayString().Contains("SpExecuter.Utility.ParamConfig"));


                    if (paramConfigAttr != null)
                    {

                        var excludeArg = paramConfigAttr.NamedArguments
                                .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.ParamExclusion));
                        bool excludeArgValue = Convert.ToBoolean(excludeArg.Value.Value);
                        if (excludeArgValue)
                        {
                            continue;
                        }
                        var dbParamArg = paramConfigAttr.NamedArguments
                                .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.DBParam));
                        string tempName = Convert.ToString(dbParamArg.Value.Value);
                        if (!string.IsNullOrWhiteSpace(tempName))
                        {
                            req.DBParam = tempName;
                        }
                        var isOutArg = paramConfigAttr.NamedArguments
                                .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.OutParam));
                        req.OutParam = Convert.ToBoolean(isOutArg.Value.Value);
                        if (req.OutParam)
                        {

                            var outParamLengthArg = paramConfigAttr.NamedArguments
                                .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.OutParamLength));
                            req.OutParamLength = Convert.ToInt32(outParamLengthArg.Value.Value);
                        }
                        
                        //Check Unique & ResultExclusion is set
                        var isUniqueArg = paramConfigAttr.NamedArguments
                                  .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.Unique));
                        var resultExclusionArgs = paramConfigAttr.NamedArguments
                                  .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.ResultExclusion));
                        if (isUniqueArg.Key != null || resultExclusionArgs.Key != null)
                        {
                            Validations.InputPropertiesValidation(info, req);
                        }
                    }
                        sqlParams.Append(GetPropertyImplementation(info, req));
                }
            }
            return sqlParams;

        }

        private static StringBuilder GetPropertyImplementation(Information info,
            RequestProp req)
        {
            StringBuilder propertyImplementation = new StringBuilder();
            ITypeSymbol type = req.PropertySymbol.Type;
            switch (type)
            {
                case IArrayTypeSymbol:
                    {
                        var namedType = type as IArrayTypeSymbol;
                        StringBuilder propertySyntax = new StringBuilder();

                        if (namedType.ElementType.SpecialType == SpecialType.System_Byte)
                        {
                            switch (req.OutParam)
                            {
                                case false:
                                    propertySyntax.AppendLine($"         param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.VarBinary, -1)" +
                                            $"{{Value = {info.SqlInParamName}.{req.ParameterName} ==null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                    break;
                                case true:
                                    propertySyntax.AppendLine($"         SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.VarBinary, -1);");
                                    propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                                    propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                                    info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? null : (byte[]){req.ParameterName}.Value;");

                                    break;
                            }
                        }
                        else if (namedType.ElementType.SpecialType == SpecialType.System_Char)
                        {
                            switch (req.OutParam)
                            {
                                case false:
                                    propertySyntax.AppendLine($"         param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.NVarChar, -1)" +
                                                    $"{{Value = string.IsNullOrWhiteSpace(new string({info.SqlInParamName}.{req.ParameterName})) ? DBNull.Value : new string({info.SqlInParamName}.{req.ParameterName})}});");
                                    break;
                                case true:
                                    propertySyntax.AppendLine($"        SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.NVarChar, -1);");
                                    propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                                    propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                                    info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? null : ((string){req.ParameterName}.Value).ToCharArray();");

                                    break;
                            }
                        }

                        propertyImplementation.Append(propertySyntax);
                        break;
                    }
                case INamedTypeSymbol:
                    {
                        INamedTypeSymbol namedTypeSymbol = type as INamedTypeSymbol;
                        if (namedTypeSymbol.ConstructedFrom.OriginalDefinition.SpecialType
                            == SpecialType.System_Nullable_T)
                        {
                            GetPrimitivePropertyImplementation(namedTypeSymbol.TypeArguments[0] as INamedTypeSymbol,
                                propertyImplementation, true,
                              info, req);
                        }
                        else if (
                               namedTypeSymbol.SpecialType == SpecialType.System_String
                            || namedTypeSymbol.SpecialType == SpecialType.System_Int32
                            || namedTypeSymbol.SpecialType == SpecialType.System_Boolean
                            || namedTypeSymbol.SpecialType == SpecialType.System_Int64
                            || namedTypeSymbol.SpecialType == SpecialType.System_Decimal
                            || namedTypeSymbol.SpecialType == SpecialType.System_Int16
                            || namedTypeSymbol.SpecialType == SpecialType.System_Byte
                            || namedTypeSymbol.SpecialType == SpecialType.System_Single
                            || namedTypeSymbol.SpecialType == SpecialType.System_Double
                            || namedTypeSymbol.SpecialType == SpecialType.System_Char
                            || namedTypeSymbol.SpecialType == SpecialType.System_SByte
                            || namedTypeSymbol.SpecialType == SpecialType.System_UInt16
                            || namedTypeSymbol.SpecialType == SpecialType.System_UInt32
                            || namedTypeSymbol.SpecialType == SpecialType.System_UInt64
                            || namedTypeSymbol.SpecialType == SpecialType.System_IntPtr
                            || namedTypeSymbol.SpecialType == SpecialType.System_UIntPtr
                            || namedTypeSymbol.SpecialType == SpecialType.System_DateTime
                            || namedTypeSymbol.ToDisplayString().Contains("System.DateTimeOffset")
                            || namedTypeSymbol.ToDisplayString().Contains("System.TimeSpan")
                            || namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Guid"
                            || namedTypeSymbol.TypeKind == TypeKind.Enum)
                        {
                            GetPrimitivePropertyImplementation(namedTypeSymbol,
                                propertyImplementation, false,
                              info, req);

                        }
                        else if (ImplementsIEnumerable(namedTypeSymbol))
                        {
                            propertyImplementation.Append(GenerateTVP(info,req,namedTypeSymbol));
                        }

                    }
                    break;
            }

            return propertyImplementation;
        }
        private static bool ImplementsIEnumerable(INamedTypeSymbol type)
        {
            if (type.TypeKind == TypeKind.Array)
                return true;

            var ienumerableType = type.AllInterfaces
                .FirstOrDefault(i => i.OriginalDefinition.SpecialType ==
                                     SpecialType.System_Collections_Generic_IEnumerable_T ||
                                     i.OriginalDefinition.SpecialType == 
                                     SpecialType.System_Collections_IEnumerable);

            if (ienumerableType != null)
                return true;

            return type.OriginalDefinition.SpecialType ==
                   SpecialType.System_Collections_Generic_IEnumerable_T;
        }
        private static StringBuilder GetPrimitivePropertyImplementation(
          INamedTypeSymbol namedTypeSymbol, StringBuilder propertyImplementation,
          bool isNullable, Information info, RequestProp req)
        {

            StringBuilder propertySyntax = new StringBuilder();
            propertyImplementation.Append("         ");
            if (namedTypeSymbol.SpecialType == SpecialType.System_String)
            {
                switch (req.OutParam)
                {
                    case true:
                        {
                            int limit = req.OutParamLength == 0 ? -1 : req.OutParamLength;
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}= new SqlParameter(\"@{req.DBParam}\",SqlDbType.NVarChar, {limit});");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? null : Convert.ToString({req.ParameterName}.Value);");

                            break;
                        }
                    case false:
                        {
                            propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\",string.IsNullOrWhiteSpace({info.SqlInParamName}.{req.ParameterName})?  DBNull.Value : {info.SqlInParamName}.{req.ParameterName} ));");
                            break;
                        }
                }
                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_DateTime)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.DateTime){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.DateTime){{Value = {info.SqlInParamName}.{req.ParameterName}== null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.DateTime);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToDateTime({req.ParameterName}.Value);");
                            break;
                        }

                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_Int32)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Int){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Int){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.Int);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (int?)null : Convert.ToInt32({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToInt32({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_Boolean)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Bit){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Bit){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.Bit);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (bool?)null : Convert.ToBoolean({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToBoolean({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_Int64)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.BigInt){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.BigInt){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.BigInt);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (long?)null : Convert.ToInt64({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToInt64({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_Decimal)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Decimal){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Decimal){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.Decimal);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (decimal?)null : Convert.ToDecimal({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToDecimal({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_Int16)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.SmallInt){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.SmallInt){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.SmallInt);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (short?)null : Convert.ToInt16({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToInt16({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_Byte)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.TinyInt){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.TinyInt){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.TinyInt);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (byte?)null : Convert.ToByte({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToByte({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_Single)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Real){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Real){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.Real);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (float?)null : Convert.ToSingle({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToSingle({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_Double)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Float){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Float){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.Float);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (double?)null : Convert.ToDouble({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToDouble({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_Char)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.NChar, 1){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.NChar, 1){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.NChar, 1);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (char?)null : Convert.ToChar({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToChar({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_SByte)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.SmallInt){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.SmallInt){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.SmallInt);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (sbyte?)null : Convert.ToSByte({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToSByte({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_UInt16)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Int){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Int){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.Int);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (ushort?)null : Convert.ToUInt16({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToUInt16({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_UInt32)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.BigInt){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.BigInt){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.BigInt);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (uint?)null : Convert.ToUInt32({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToUInt32({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_UInt64)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Decimal){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Decimal){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.Decimal);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (ulong?)null : Convert.ToUInt64({req.ParameterName}.Value);");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = Convert.ToUInt64({req.ParameterName}.Value);");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_IntPtr)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.BigInt){{Value = (long){info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.BigInt){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : (long){info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.BigInt);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (IntPtr?)null : new IntPtr(Convert.ToInt64({req.ParameterName}.Value));");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = new IntPtr(Convert.ToInt64({req.ParameterName}.Value));");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.SpecialType == SpecialType.System_UIntPtr)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Decimal){{Value = (decimal){info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Decimal){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value :(decimal){info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.Decimal);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (UIntPtr?)null : new UIntPtr(Convert.ToUInt64({req.ParameterName}.Value));");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = new UIntPtr(Convert.ToUInt64({req.ParameterName}.Value));");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals("global::System.DateTimeOffset"))
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.DateTimeOffset){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.DateTimeOffset){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.DateTimeOffset);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (DateTimeOffset?)null : (DateTimeOffset){req.ParameterName}.Value;");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = (DateTimeOffset){req.ParameterName}.Value;");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals("global::System.TimeSpan"))
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Time){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.Time){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.Time);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (TimeSpan?)null : (TimeSpan){req.ParameterName}.Value;");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = (TimeSpan){req.ParameterName}.Value;");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals("global::System.Guid"))
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.UniqueIdentifier){{Value = {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.UniqueIdentifier){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.UniqueIdentifier);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? (Guid?)null : (Guid){req.ParameterName}.Value;");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = (Guid){req.ParameterName}.Value;");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }
            else if (namedTypeSymbol.TypeKind == TypeKind.Enum)
            {
                switch (req.OutParam)
                {
                    case false:
                        {
                            switch (isNullable)
                            {
                                case false:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.NVarChar,255){{Value = {info.SqlInParamName}.{req.ParameterName}.ToString()}});");
                                        break;
                                    }
                                case true:
                                    {
                                        propertySyntax.AppendLine($"param.Add(new SqlParameter(\"@{req.DBParam}\", SqlDbType.NVarChar,255){{Value = {info.SqlInParamName}.{req.ParameterName} == null ? DBNull.Value : {info.SqlInParamName}.{req.ParameterName}.ToString()}});");
                                        break;
                                    }
                            }
                            break;
                        }
                    case true:
                        {
                            string enumFullName = namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            propertySyntax.AppendLine($"SqlParameter {req.ParameterName}=new SqlParameter(\"@{req.DBParam}\", SqlDbType.NVarChar, 255);");
                            propertySyntax.AppendLine($"         {req.ParameterName}.Direction = ParameterDirection.Output;");
                            propertySyntax.AppendLine($"         param.Add({req.ParameterName});");
                            if (isNullable)
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = {req.ParameterName}.Value == DBNull.Value ? ({enumFullName}?)null : ({enumFullName})Enum.Parse(typeof({enumFullName}), Convert.ToString({req.ParameterName}.Value));");
                            else
                                info.OutParams.AppendLine($"         {info.SqlInParamName}.{req.ParameterName} = ({enumFullName})Enum.Parse(typeof({enumFullName}), Convert.ToString({req.ParameterName}.Value));");
                            break;
                        }
                }

                propertyImplementation.Append(propertySyntax);
            }

            return propertyImplementation;
        }
        private static StringBuilder GenerateTVP(Information info
            ,RequestProp req,INamedTypeSymbol namedTypeSymbol)
        {
            StringBuilder tvpFunc = new StringBuilder();
            var elementType = namedTypeSymbol.TypeArguments[0];
            var attrData = elementType
                            .GetAttributes()
                            .FirstOrDefault(a => 
                            a.AttributeClass.
                            ToDisplayString(SymbolDisplayFormat.
                            FullyQualifiedFormat).Equals
                            ("global::SpExecuter.Utility.TVP"));
            Dictionary<int, INamedTypeSymbol> inheritanceInfo =
                new Dictionary<int, INamedTypeSymbol>();
            Common.GetInheritanceInfo(elementType as INamedTypeSymbol, 0, inheritanceInfo);
            string dataTypeName = attrData ==null ? "dbo." +elementType.Name: (string)attrData.ConstructorArguments[0].Value;
            bool includeInherited = attrData != null &&
                attrData.NamedArguments.Any(na => na.Key == nameof(TVP.IncludeInherited)) &&
                (bool)attrData.NamedArguments.First(na => na.Key == nameof(TVP.IncludeInherited)).Value.Value;

            tvpFunc.AppendLine($"         var {req.ParameterName}DataTable = new System.Data.DataTable(\"{req.ParameterName}\");");

            GenerateInheritedPropertyTVP(info,req, inheritanceInfo, includeInherited,tvpFunc);


            tvpFunc.AppendLine($"          SqlParameter {req.ParameterName} = new SqlParameter(\"@{req.DBParam}\", SqlDbType.Structured)");
            tvpFunc.AppendLine($"           {{");
            tvpFunc.AppendLine($"            TypeName = \"{dataTypeName}\",");
            tvpFunc.AppendLine($"            Value = (object){req.ParameterName}DataTable ?? DBNull.Value");
            tvpFunc.AppendLine($"           }};");
            tvpFunc.AppendLine($"         param.Add({req.ParameterName});");
            return tvpFunc;
        }
        private static bool IsCollectionType(ITypeSymbol type) =>
                   type switch
                   {
                       IArrayTypeSymbol arr when arr.ElementType.SpecialType == SpecialType.System_Byte => false,
                       IArrayTypeSymbol => true,
                       INamedTypeSymbol nt when nt.IsGenericType &&
                                               nt.AllInterfaces.Any(i =>
                                                    i.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ==
                                                    "System.Collections.Generic.IEnumerable<T>") => true,

                       _ => false
                   };
        private static void GenerateInheritedPropertyTVP(Information info,
            RequestProp req,Dictionary<int,INamedTypeSymbol> inheritanceInfo,
            bool includeInherited,StringBuilder tvpFunc)
        {
            if (includeInherited)
            {
                for (int i =0; i < inheritanceInfo.Count; i++)
                {
                    var elementType = inheritanceInfo[i];
                    foreach (var prop in elementType.GetMembers().OfType<IPropertySymbol>())
                    {
                        if (prop.IsIndexer || prop.SetMethod is null || IsCollectionType(prop.Type)) continue;

                        string columnType =
                            prop.Type is IArrayTypeSymbol arr && arr.ElementType.SpecialType == SpecialType.System_Byte
                            ? "byte[]"
                            : prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                        tvpFunc.AppendLine($"         {req.ParameterName}DataTable.Columns.Add(\"{prop.Name}\", typeof({columnType}));");
                    }

                }
                tvpFunc.AppendLine($"         foreach (var item in {info.SqlInParamName}.{req.ParameterName})");
                tvpFunc.AppendLine($"         {{ ");
                tvpFunc.AppendLine($"          var row = {req.ParameterName}DataTable.NewRow();");
                for (int i = 0; i < inheritanceInfo.Count; i++)
                {
                    var elementType = inheritanceInfo[i];
                    foreach (var prop in elementType.GetMembers().OfType<IPropertySymbol>())
                    {
                        if (prop.IsIndexer || prop.SetMethod is null || IsCollectionType(prop.Type)) continue;
                        tvpFunc.AppendLine($"           row[\"{prop.Name}\"] = item.{prop.Name};");
                    }
                }
                tvpFunc.AppendLine($"          {req.ParameterName}DataTable.Rows.Add(row);");
                tvpFunc.AppendLine($"         }}");
            }
            else
            {
               

                var elementType = inheritanceInfo[0];
                foreach (var prop in elementType.GetMembers().OfType<IPropertySymbol>())
                {
                    if (prop.IsIndexer || prop.SetMethod is null || IsCollectionType(prop.Type)) continue;

                    string columnType =
                        prop.Type is IArrayTypeSymbol arr && arr.ElementType.SpecialType == SpecialType.System_Byte
                        ? "byte[]"
                        : prop.Type.TypeKind == TypeKind.Enum?
                        "string":
                        prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    columnType = columnType.Equals("sbyte")?"short": columnType;
                    columnType = columnType.Equals("ushort") ?"int": columnType;
                    columnType = columnType.Equals("uint") ?"long": columnType; 
                    columnType = columnType.Equals("ulong") ?"decimal": columnType;
                    columnType = columnType.Equals("nint") ?"long": columnType;
                    columnType = columnType.Equals("nuint") ?"decimal": columnType;

                    tvpFunc.AppendLine($"         {req.ParameterName}DataTable.Columns.Add(\"{prop.Name}\", typeof({columnType}));");
                }
                tvpFunc.AppendLine($"         foreach (var item in {info.SqlInParamName}.{req.ParameterName})");
                tvpFunc.AppendLine($"         {{ ");
                tvpFunc.AppendLine($"          var row = {req.ParameterName}DataTable.NewRow();");
                foreach (var prop in elementType.GetMembers().OfType<IPropertySymbol>())
                {
                    if (prop.IsIndexer || prop.SetMethod is null || IsCollectionType(prop.Type)) continue;
                    string columnType =
                        prop.Type is IArrayTypeSymbol arr && arr.ElementType.SpecialType == SpecialType.System_Byte
                        ? "byte[]"
                        : prop.Type.TypeKind == TypeKind.Enum ?
                        "string" :
                        prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    columnType = columnType.Equals("nint") ? $"Convert.ToInt64(item.{prop.Name})" :
                                 columnType.Equals("nuint") ? $"Convert.ToDecimal(item.{prop.Name})" :
                                 "";
                    columnType= string.IsNullOrWhiteSpace(columnType)?$"item.{prop.Name}": columnType;

                    tvpFunc.AppendLine($"row[\"{prop.Name}\"] = item.{prop.Name} == null ? (object)DBNull.Value :{columnType} ;");
                }
                tvpFunc.AppendLine($"          {req.ParameterName}DataTable.Rows.Add(row);");
                tvpFunc.AppendLine($"         }}");

            }
        }
    }
}
