using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpExecuter.Generator
{
    internal sealed class OutParameterGenerator
    {

        internal static StringBuilder ResultBindingImplementation(Information info)
        {

            StringBuilder resultBindingImpl = new StringBuilder();
            if (info.IsNonQuery)
            {
                resultBindingImpl.AppendLine($"         int rows= await command.ExecuteNonQueryAsync();");
                resultBindingImpl.AppendLine(info.OutParams.ToString());
                resultBindingImpl.AppendLine($"         return rows;");
                return resultBindingImpl;
            }
            resultBindingImpl.AppendLine($"         await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);");
            INamedTypeSymbol retunType = info.MethodSymbol.ReturnType as INamedTypeSymbol;
            retunType = retunType.TypeArguments[0] as INamedTypeSymbol;

            if (retunType.TypeKind == TypeKind.Class &&
                    retunType.DeclaringSyntaxReferences.Length > 0)
            {
                resultBindingImpl.AppendLine("         if (await reader.ReadAsync()){");
                //If flag for fetching indices is true then get ordinals code
                if (!info.ExcludeIndices)
                {
                    GenerateGetOrdinalsConditionaly(info, resultBindingImpl);
                }
                resultBindingImpl.AppendLine($"           {retunType.Name} item = new {retunType.Name}();");
                resultBindingImpl.Append(GenerateSingleClassBinding(info,retunType,"", 1));
                resultBindingImpl.AppendLine($"          await reader.DisposeAsync();");
                resultBindingImpl.AppendLine(info.OutParams.ToString());
                resultBindingImpl.AppendLine($"           return item;");
                resultBindingImpl.AppendLine("         }");
                resultBindingImpl.AppendLine("         return null;");
                resultBindingImpl.Append(AddCatchBlock(info));
            }
            else if (retunType.IsGenericType)
            {
                if (retunType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
                {
                    var genericType = retunType.TypeArguments[0] as INamedTypeSymbol;
                    GetList(info, genericType, resultBindingImpl,1);
                    resultBindingImpl.AppendLine($"          await reader.DisposeAsync();");
                    resultBindingImpl.AppendLine(info.OutParams.ToString());
                    resultBindingImpl.AppendLine("         return list1;");
                    resultBindingImpl.Append(AddCatchBlock(info));
                }
                else if (retunType.IsTupleType)
                {
                    if (!Common.IsNestedTuple(retunType))
                    {
                       
                        if(info.ConditionType == ConditionType.OR)
                        {
                            GenerateNestedTuplesResultBinding(info,resultBindingImpl);
                        }
                        else
                        {
                            GenerateTupleBinding(info, retunType, resultBindingImpl);
                        }
                    }
                    else
                    {
                        GenerateNestedTuplesResultBinding(info,resultBindingImpl);
                    }
                    resultBindingImpl.Append(AddCatchBlock(info));
                    
                }

            }
            return resultBindingImpl;
        }
        private static void GenerateNestedTuplesResultBinding(Information info,StringBuilder builder)
        {
            var list = new List<string>();
            var returnType = info.MethodSymbol.ReturnType as INamedTypeSymbol;
            Initiate(returnType.TypeArguments[0] as INamedTypeSymbol, list);
            list.ForEach(item => builder.AppendLine(item));
            list.Clear();
            var nullList = new List<string>();
            GetBooleanForVerification(returnType.TypeArguments[0]
                as INamedTypeSymbol, list, nullList);
            list.ForEach(item => builder.AppendLine(item));

            builder.Append(NestedTupleResultBinding.GeneratedNestedResultSets(info));
            builder.AppendLine($"          await reader.DisposeAsync();");
            builder.AppendLine(info.OutParams.ToString());

            if (info.ConditionType == ConditionType.OR)
            {
                builder.Append(NestedTupleResultBinding.GenerateReturnStatementForConditionalResultSets(info,
                     returnType.TypeArguments[0]
                   as INamedTypeSymbol));
            }
            else
            {
                builder.Append(GenerateIfConditionsForTupleReturn(info, returnType.TypeArguments[0]
                   as INamedTypeSymbol));
            }
               
            builder.AppendLine("         return (" + string.Join(",", nullList) + ");");

        }
        private static void Initiate(INamedTypeSymbol namedSym,
                List<string> init, params int[] uniqueNumbers)
        {

            if (namedSym == null)
                return;
            if(namedSym.NullableAnnotation== NullableAnnotation.Annotated)
            {
                namedSym = namedSym.TypeArguments[0] as INamedTypeSymbol;
            }

            if (namedSym.IsTupleType)
            {
                int i = 1;
                foreach (var element in namedSym.TupleElements)
                {
                    List<int> list = uniqueNumbers.ToList<int>();
                    list.Add(i);
                    Initiate( element.Type as INamedTypeSymbol,
                        init, list.ToArray());
                    i++;
                }
                return;
            }

            if (namedSym.IsGenericType)
            {
              
                    init.Add($"         List<{namedSym.TypeArguments[0].Name}> list{string.Join("_", uniqueNumbers)} = null;");
                return;
            }


            if (namedSym.TypeKind == TypeKind.Class &&
                namedSym.DeclaringSyntaxReferences.Length > 0)
            {
                init.Add($"         {namedSym.Name} item{string.Join("_",uniqueNumbers)} = null;");
                return;
            }
        }

        private static void GetBooleanForVerification(INamedTypeSymbol namedSym,
               List<string> bools,List<string> returnNulls)
        {
            int i= 1;
            foreach (var element in namedSym.TupleElements)
            {
                bools.Add($"            bool returnItem{i} = false;");
                returnNulls.Add($"null");
                i++;
            }
        }
        private static void GetReturnForNested(INamedTypeSymbol wholeRetunType,
               INamedTypeSymbol currentType, StringBuilder builder, int itemNo)
        {
            List<string> list = new List<string>();
           
            for (int j = 1;j<= wholeRetunType.TupleElements.Count(); j++)
            {
                if(j==itemNo)
                {
                   break;
                }
                list.Add("null");
            }


                if (currentType.TypeKind == TypeKind.Class &&
                   currentType.DeclaringSyntaxReferences.Length > 0)
                {
                list.Add("item" + itemNo);
                }
                else if (currentType.IsGenericType &&
                    currentType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
                {

                list.Add("list" + itemNo);
                }
                else
                {
                list.Add( NestedTupleElementsReturnGeneration(currentType, itemNo));
                }
            for (int j = itemNo + 1; j <= wholeRetunType.TupleElements.Count(); j++)
            {

                list.Add("null");
            }


        builder.Append(string.Join(",", list));
        
        }
       private static string NestedTupleElementsReturnGeneration(INamedTypeSymbol nestedReturnType,
           int itemNo)
        {
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            int i = 1;
            foreach (var element in nestedReturnType.TupleElements)
            {
                var elementType = element.Type as INamedTypeSymbol;
                if (elementType.TypeKind == TypeKind.Class &&
                                elementType.DeclaringSyntaxReferences.Length > 0)
                {
                    list.Add($"item{itemNo}_{i}");
                }
                else if (elementType.IsGenericType &&
                    elementType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
                {
                    list.Add($"list{itemNo}_{i}");
                }
                i++;
            }
            builder.Append(string.Join(",", list));
            builder.Append(")");
            return builder.ToString();
        }
        private static string GenerateIfConditionsForTupleReturn(Information info,INamedTypeSymbol wholeRetunType)
        {
            List<StringBuilder> builders = new List<StringBuilder>();
            
            // Build a dictionary of tuple index (1-based) to its element count
            var tupleLengths = new Dictionary<int, int>();
            int idx = 1;
            foreach (var current in wholeRetunType.TupleElements)
            {
                var returnType = current.Type as INamedTypeSymbol;
                if (returnType.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    returnType = returnType.TypeArguments[0] as INamedTypeSymbol;
                }
                int length = returnType.IsTupleType ? returnType.TupleElements.Length : 1;
                tupleLengths[idx] = length;
                idx++;
            }

            // Sort by tuple length descending so longer tuples are checked first
            var sorted = tupleLengths.OrderByDescending(kvp => kvp.Value).ToList();

            foreach (var kvp in sorted)
            {
                int i = kvp.Key;
                var current = wholeRetunType.TupleElements[i - 1];
                StringBuilder currentBuilder = new StringBuilder();
                currentBuilder.AppendLine($"           if(returnItem{i}){{");
                currentBuilder.Append($"              return (");
                var returnType = current.Type as INamedTypeSymbol;
                if (returnType.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    returnType = returnType.TypeArguments[0] as INamedTypeSymbol;
                }
                GetReturnForNested(wholeRetunType, returnType, currentBuilder, i);
                currentBuilder.AppendLine(");");
                currentBuilder.AppendLine("              }");
                builders.Add(currentBuilder);
            }
            return string.Join("", builders);
        }
        private static void GenerateTupleBinding(Information info,
            INamedTypeSymbol retunType,
            StringBuilder resultBindingImpl)
        {
            int itemNumber = 1;
            foreach (var element in retunType.TupleElements)
            {
                var elementType = element.Type as INamedTypeSymbol;
                if (elementType.TypeKind == TypeKind.Class &&
                    elementType.DeclaringSyntaxReferences.Length > 0)
                {
                    if (itemNumber != 1)
                    {
                        resultBindingImpl.AppendLine($"         {elementType.Name} item{itemNumber} =null;");
                        resultBindingImpl.AppendLine("         if(await reader.NextResultAsync()){");
                    }
                    else
                    {
                        resultBindingImpl.AppendLine($"         {elementType.Name} item{itemNumber} =null;");

                    }
                    resultBindingImpl.AppendLine("              if (await reader.ReadAsync()){");
                    if (!info.ExcludeIndices)
                    {
                        GenerateGetOrdinalsConditionaly(info, resultBindingImpl);
                    }
                    resultBindingImpl.AppendLine($"           item{itemNumber} = new {elementType.Name}();");
                    resultBindingImpl.Append(GenerateSingleClassBinding(info, elementType, itemNumber.ToString(), 1, itemNumber));
                    resultBindingImpl.AppendLine("              }");
                    if (itemNumber != 1)
                    {
                        resultBindingImpl.AppendLine("         }");
                    }
                }
                else if (elementType.IsGenericType &&
                    elementType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
                {
                    var innerType = elementType.TypeArguments[0] as INamedTypeSymbol;
                    if (itemNumber != 1)
                    {
                        resultBindingImpl.AppendLine($"         List<{innerType.Name}> list{itemNumber} =new List<{innerType.Name}>();");
                        resultBindingImpl.AppendLine("         if(await reader.NextResultAsync()){");
                    }
                    else
                    {
                        resultBindingImpl.AppendLine($"         List<{innerType.Name}> list{itemNumber} =new List<{innerType.Name}>();");
                    }
                    resultBindingImpl.AppendLine("         while(await reader.ReadAsync()){");
                    if (!info.ExcludeIndices)
                    {
                        GenerateGetOrdinalsConditionaly(info, resultBindingImpl);
                    }
                    resultBindingImpl.AppendLine($"            {innerType.Name} item = new {innerType.Name}();");
                    resultBindingImpl.Append(GenerateSingleClassBinding(info, innerType, "", 1, itemNumber));
                    resultBindingImpl.AppendLine($"            list{itemNumber}.Add(item);");
                    resultBindingImpl.AppendLine("             }");
                    if (itemNumber != 1)
                    {
                        resultBindingImpl.AppendLine("         }");
                    }
                }

                itemNumber++;
            }
            var returnType = info.MethodSymbol.ReturnType as INamedTypeSymbol;
            resultBindingImpl.AppendLine($"          await reader.DisposeAsync();");
            resultBindingImpl.AppendLine(info.OutParams.ToString());
            resultBindingImpl.AppendLine("              return (" + GetReturnStatementForTuple(retunType) + " );");

        }
        private static string GetReturnStatementForTuple(INamedTypeSymbol symbol)
        {
            int i = 1;
            List<string> elements = new List<string>();
            
            foreach (var element in symbol.TupleElements)
            {
                if (element.Type.TypeKind == TypeKind.Class &&
                                element.Type.DeclaringSyntaxReferences.Length > 0)
                {
                    elements.Add($"item{i}");
                }
                else if (element.Type.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
                {
                    elements.Add($"list{i}");
                }
                i++;
            }
            return string.Join(",", elements);
        }
       private static StringBuilder AddCatchBlock(Information info)
        {
            var sb = new StringBuilder();
            sb.AppendLine("         }")
              .AppendLine("         catch (Exception ex)")
              .AppendLine("         {");
            sb.AppendLine($"                     StringBuilder info = new StringBuilder().AppendLine(\"Stored Procedure --> \" + \"{info.StoredProcedureName}\")")
              .AppendLine("                    .Append(\"  :::  \")")
              .AppendLine($"                    .AppendLine(\"DataBase --> \" + {info.MethodSymbol.Parameters[0].Name});");
          
            if (info.MethodSymbol.Parameters.Length > 1)
             { 
              sb.AppendLine()
              .AppendLine("                     info.AppendLine();")
              .AppendLine($"                     info.AppendLine(\"  :::  Parameter Object --> \" + global::Utf8Json.JsonSerializer.ToJsonString({info.MethodSymbol.Parameters[1].Name}));")
              .AppendLine("                         if (param != default && param.Count() > 0)")
              .AppendLine("                         {")
              .AppendLine("                             info.AppendLine(\"  :::  \" + \"SQL Parameters --> \");")
              .AppendLine("                             foreach (var oneparam in param)")
              .AppendLine("                             {")
              .AppendLine("                              info.Append(\"       \" + oneparam.ParameterName + \" : \").AppendLine(oneparam.Value + \",  \");")
              .AppendLine("                             }")
              .AppendLine("                         }");
              }
              sb.AppendLine()
              .AppendLine("                    throw new SpExecuterException(info, ex);")
              .AppendLine("         }");

            return sb;
        }
        private static void GetList(Information info,
            INamedTypeSymbol namedType,
            StringBuilder builder,int listNumber)
        {

            builder.AppendLine($"         List<{namedType.Name}> list{listNumber}=new List<{namedType.Name}>();");
            builder.AppendLine("         while (await reader.ReadAsync()){");
            if (!info.ExcludeIndices)
            {
                GenerateGetOrdinalsConditionaly(info, builder);
                
            }
            builder.AppendLine($"           {namedType.Name} item = new {namedType.Name}();");
            builder.Append(GenerateSingleClassBinding(info, namedType,"", 1));
            builder.AppendLine($"           list{listNumber}.Add(item);");
            builder.AppendLine("         }");
        }
        private static void GenerateGetOrdinalsConditionaly(Information info,
            StringBuilder builder)
        {

            if (info.ExcludeIndicesClassLevel || info.ExcludeIndicesMethodLevel)
            {
                return;
            }
            builder.AppendLine();
            builder.AppendLine($"         if(Flags.{info.MethodName}FetchFlag){{");
            builder.AppendLine($"           lock(_{info.MethodName}FetchLock){{");
            INamedTypeSymbol returnType = info.MethodSymbol.ReturnType as INamedTypeSymbol;
            returnType = returnType.TypeArguments[0] as INamedTypeSymbol;
            List<string> ordinals = new List<string>();
            
            CreateAllIndicesForFetching(info.Context, returnType, ordinals,
               info.UseShort, info.MethodSequenceNo, 1);
            ordinals.ForEach(ordinal =>
            {
                builder.AppendLine($"           {ordinal}");
            });
            builder.AppendLine($"           Flags.{info.MethodName}FetchFlag=false;");
            builder.AppendLine("            }");
            builder.AppendLine("         }");
        }
        private static void CreateAllIndicesForFetching(SourceProductionContext context,
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
                    CreateAllIndicesForFetching(context, element.Type as INamedTypeSymbol,
                        ordinals, saveIndicesInShort, methodNo, list.ToArray());
                    i++;
                }
                return;
            }

            if (namedSym.IsGenericType)
            {

                CreateAllIndicesForFetching(context,
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
                string dbParamName = string.Empty;
                string datatype = saveIndicesInShort ? "short" : "byte";

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
                        dbParamName = propertySymbol.Name;
                        if (paramConfigAttr != null)
                        {

                            var excludeArg = paramConfigAttr.NamedArguments
                                    .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.ResultExclusion));
                            bool excludeArgValue = Convert.ToBoolean(excludeArg.Value.Value);
                            if (excludeArgValue)
                            {
                                exclude = excludeArgValue;
                            }
                            var dbParamArg = paramConfigAttr.NamedArguments
                                    .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.DBParam));
                            string tempName = dbParamArg.Key != null? Convert.ToString(dbParamArg.Value.Value):"";
                            if (!string.IsNullOrWhiteSpace(tempName))
                            {
                                dbParamName = tempName;
                            }
                         
                        }
                        if (!exclude)
                        {
                            ordinals.Add($"  SP{methodNo}_Base{i}_{string.Join("", uniqueNumbers)}_{propertySymbol.Name} =({datatype}) reader.GetOrdinal(\"{dbParamName}\");");
                        }
                    }
                   
                }
                return;
            }    
        }

        private static StringBuilder GenerateSingleClassBinding(Information info,
            INamedTypeSymbol classSymbol,string suffix,params int[] uniqueNumbers)
        {
            Dictionary<int, INamedTypeSymbol> inheritanceInfo = new Dictionary<int, INamedTypeSymbol>();
            Common.GetInheritanceInfo(classSymbol,0, inheritanceInfo);
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
                    builder.AppendLine($"                   item{suffix}.{propertySymbol.Name} = {Common.GetPropertyType(info,propertySymbol, dbParamName, ordinalVariableName)}");


                }

            }
            
           
            return builder;
        }
        
    }
    }

