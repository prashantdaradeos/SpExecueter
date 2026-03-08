using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpExecuter.Generator
{
    internal class NestedTupleResultBinding
    {
        internal static StringBuilder GeneratedNestedResultSets(Information info)
        {
            StringBuilder builder= new StringBuilder();
            ConditionInfo conditionInfo = new ConditionInfo(info);
            conditionInfo.FlattenedReturnTypes = new Dictionary<int, List<INamedTypeSymbol>>();

            if (info.ConditionType == ConditionType.OR)
            {
                int nestedTupleSequence = 0;
                foreach (var nestedTuple in conditionInfo.ReturnType.TupleElements)
                { 
                    var tupleType = nestedTuple.Type as INamedTypeSymbol;
                    tupleType = Common.GetNonNullableType(tupleType);
                    List<INamedTypeSymbol> flattenedTypes = new List<INamedTypeSymbol>();
                    flattenedTypes.Add(tupleType);
                    conditionInfo.FlattenedReturnTypes[nestedTupleSequence++] = flattenedTypes;
                }
            }
            else
            {
                int nestedTupleSequence = 0;
                foreach (var nestedTuple in conditionInfo.ReturnType.TupleElements)
                {
                    var tupleType = nestedTuple.Type as INamedTypeSymbol;
                    tupleType = Common.GetNonNullableType(tupleType);
                    List<INamedTypeSymbol> flattenedTypes = new List<INamedTypeSymbol>();
                    Common.Flatten(tupleType, flattenedTypes);
                    conditionInfo.FlattenedReturnTypes[nestedTupleSequence++] = flattenedTypes;
                }
            }
           

            GenerateForSameElements(conditionInfo, builder);
            builder.Append(GenerateResultSetsForUnique(conditionInfo));

            return builder;
        }
       

        internal static string GenerateReturnStatementForConditionalResultSets(Information info,
           INamedTypeSymbol returnType)
        {
            StringBuilder builder= new StringBuilder();
            ConditionInfo conditionInfo = new ConditionInfo(info);
            List<StringBuilder> builders = new List<StringBuilder>();
            int i = 1;

            foreach (var current in returnType.TupleElements)
            {
                StringBuilder currentBuilder = new StringBuilder();
                currentBuilder.AppendLine($"if(returnItem{i}){{");
                string varName= GetNameOfVariable(current.Type as INamedTypeSymbol, i,0);
                currentBuilder.Append($"return (");
                List<string> tempList =returnType.TupleElements.ToList().Select((value,index) => {
                    if (i ==index+1)
                    {
                        return varName;
                    }
                    else
                    {
                        return "null";
                    }
                }).ToList();
                currentBuilder.Append(string.Join(",", tempList));
                currentBuilder.AppendLine(");");
                currentBuilder.AppendLine("}");
                builders.Add(currentBuilder);
                i++;
            }
            return string.Join("", builders);
        }
        private static StringBuilder GenerateResultSetsForUnique(ConditionInfo conditionInfo)
        {
            StringBuilder builder= new StringBuilder();

            int tempSameElements = conditionInfo.SameElementsInNestedTuple +
                     conditionInfo.SameElementsListAgnostic;

          var  varNames = GetAllVarNames(conditionInfo);

            GenerateRecurssively(conditionInfo, null,null, builder, tempSameElements, varNames);

      
            return builder;
        }

        private static void GenerateRecurssively(ConditionInfo conditionInfo,
                   List<List<INamedTypeSymbol>> flattenedTypes, string[] uniqueKeys,
                   StringBuilder builder, int forPosition, List<List<string>> varNames)
        {
            if (flattenedTypes == null)
            {
                flattenedTypes = conditionInfo.FlattenedReturnTypes.Values.ToList();
            }

            if (flattenedTypes.Count == 1)
            {
                var tuple = flattenedTypes[0];

                for (int i = forPosition; i < tuple.Count; i++)
                {
                    builder.Append(Common.BindListElements(conditionInfo, tuple[i], i, true, varNames, flattenedTypes));

                }
                return;
            }

            string[] uniqueProps = flattenedTypes.
                                Select(kvp => Common.GetUniqueProperty(conditionInfo, kvp.Count > forPosition ?
                                kvp[forPosition] : null)).ToArray();

            uniqueKeys = flattenedTypes == null ? uniqueProps : uniqueKeys;

            var duplicateIndexes = GetDuplicateIndexes(uniqueProps);

            // If all uniqueProps are the same (only one group), just recurse without if
            if (duplicateIndexes.Count == 1)
            {
                var group = duplicateIndexes[0].Select(index => flattenedTypes[index]).ToList();
                // Generate code for the element at the current position for this group
                builder.Append(Common.BindListElements(conditionInfo, group[0][forPosition], forPosition, true, varNames, group));
                GenerateRecurssively(conditionInfo, group, uniqueKeys, builder, forPosition + 1, varNames);
                return;
            }

            bool isFirst = true;
            if (forPosition > 0)
            {
                builder.AppendLine("     if(await reader.NextResultAsync())");
                builder.AppendLine("     {");
            }
            foreach (List<int> duplicateIndex in duplicateIndexes)
            {
                string uniqueValue = uniqueProps[duplicateIndex[0]];
                int currentIndex = Array.IndexOf(uniqueProps, uniqueValue);

                if (isFirst)
                {
                    builder.AppendLine($"     if(ColumnExists(reader,\"{uniqueValue}\"))");
                    isFirst = false;
                }
                else
                {
                    builder.AppendLine($"     else if(ColumnExists(reader,\"{uniqueValue}\"))");
                }
                builder.AppendLine($"          {{");
                var group = duplicateIndex.Select(index => flattenedTypes[index]).ToList();
                // Generate code for the element at the current position for this group
                builder.Append(Common.BindListElements(conditionInfo, group[0][forPosition], forPosition, false, varNames, group));
                GenerateRecurssively(conditionInfo, group, uniqueKeys, builder, forPosition + 1, varNames);
                builder.AppendLine($"          }}");
            }
            if (forPosition > 0)
            {
                builder.AppendLine("     }");
            }
        }

        private static List<List<string>> GetAllVarNames(ConditionInfo info)
        {
            List<List<string>> varNames= new List<List<string>>(info.FlattenedReturnTypes.Count);
            for(int i = 0; i < info.FlattenedReturnTypes.Count; i++)
            {
                List<string> subVarNames =new List<string>(info.FlattenedReturnTypes[i].Count);
                for(int j = 0; j < info.FlattenedReturnTypes[i].Count; j++)
                {
                    subVarNames.Add(GetNameOfVariable(info.FlattenedReturnTypes[i][j], i+1,
                        info.FlattenedReturnTypes[i].Count > 1 ? j + 1 : 0));
                }
                varNames.Add(subVarNames);
            }
            return varNames;
        }
        internal static List<List<int>> GetDuplicateIndexes(string[] uniqueProps)
        {
            var valueToIndexes = new Dictionary<string, List<int>>();
            for (int i = 0; i < uniqueProps.Length; i++)
            {
                var value = uniqueProps[i];
                if (string.IsNullOrWhiteSpace(value))
                    continue;
                if (!valueToIndexes.TryGetValue(value, out var indexes))
                {
                    indexes = new List<int>();
                    valueToIndexes[value] = indexes;
                }
                indexes.Add(i);
            }
            return valueToIndexes.Values.ToList();
        }
        private static void GenerateForSameElements(ConditionInfo conditionInfo, StringBuilder builder)
       {
            if (conditionInfo.SameElementsInNestedTuple > 0)
            {
                int maxReturnedTuple = conditionInfo.FlattenedReturnTypes
                         .OrderByDescending(kvp => kvp.Value.Count)
                        .First().Key;
                for (int i = 0; i < conditionInfo.SameElementsInNestedTuple; i++)
                {
                    builder.Append(Common.BindElements(conditionInfo,
                        conditionInfo.FlattenedReturnTypes[maxReturnedTuple][i], i));
                }
                var names = conditionInfo.FlattenedReturnTypes[maxReturnedTuple];


                for (int i = 0; i < conditionInfo.SameElementsInNestedTuple; i++)
                {
                    List<string> nameBuilder = new List<string>();

                    for (int j = 0; j < conditionInfo.FlattenedReturnTypes.Count; j++)
                    {
                        nameBuilder.Add(GetNameOfVariable(names[i], j + 1,

                            conditionInfo.FlattenedReturnTypes[j].Count > 1 ? i + 1 : 0));
                    }

                    string leftSide = string.Join(" = ", nameBuilder);
                    string rightSide = nameBuilder[0].StartsWith("item")
                        ? $"itemCommon{i + 1}"
                        : $"listCommon{i + 1}";

                    builder.AppendLine($"        {leftSide} = {rightSide};");
                }
            }
            builder.Append(GenerateForListAgnostic(conditionInfo));
            builder.AppendLine();
            
            var changeReturnItem = conditionInfo.FlattenedReturnTypes.
            Select((kvp, index) =>  kvp.Value.Count == (conditionInfo.SameElementsInNestedTuple+
            conditionInfo.SameElementsListAgnostic) ?
                "returnItem" + (index + 1) : "");
            bool allEmpty = changeReturnItem.All(returnType=> string.IsNullOrEmpty(returnType));
            if (!allEmpty)
            {
            builder.AppendLine($"        {string.Join(" = true; ", changeReturnItem.Where(s => !string.IsNullOrEmpty(s)))} = true;");

            }
           

        }
        private static  StringBuilder GenerateForListAgnostic(ConditionInfo conditionInfo)
        {
            StringBuilder builder= new StringBuilder();
            Common.CheckElementsSameOrNot(conditionInfo);

            if (conditionInfo.SameElementsListAgnostic <= 0)
            {
                return builder;
            }
            
            for (int i=conditionInfo.SameElementsInNestedTuple;
                i<(conditionInfo.SameElementsInNestedTuple+conditionInfo.SameElementsListAgnostic);
                i++)
            {
                var itemIsListAt = conditionInfo.FlattenedReturnTypes
                    .Where(kvp =>
                        kvp.Value[i].IsGenericType &&
                        kvp.Value[i].OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
                    .Select(kvp => kvp.Key)
                    .ToList().First();
                 builder.Append(Common.BindElements(conditionInfo,
                        conditionInfo.FlattenedReturnTypes[itemIsListAt][i], i));

                for(int j = 0; j < conditionInfo.FlattenedReturnTypes.Count; j++)
                {
                    
                    INamedTypeSymbol atEachList = conditionInfo.FlattenedReturnTypes[j][i];
                    atEachList = Common.GetNonNullableType(atEachList);
                    string currentVarName=GetNameOfVariable(
                            conditionInfo.FlattenedReturnTypes[j][i], j + 1,
                            conditionInfo.FlattenedReturnTypes[j].Count > 1 ? i + 1 : 0);
                    string rightPart=currentVarName.StartsWith("item")
                        ? $"listCommon{i + 1}.FirstOrDefault()"
                        : $"listCommon{i + 1}";
                    builder.AppendLine($"                   {currentVarName}={rightPart};");
                }

            }

            return builder;
        }

        private static string GetNameOfVariable(
            INamedTypeSymbol namedType, int index, int? subIndex=null)
        {
            var elementType = Common.GetNonNullableType(namedType);
            if (elementType.TypeKind == TypeKind.Class &&
                    elementType.DeclaringSyntaxReferences.Length > 0)
            {
                return
                     subIndex == 0 ?
                    $"item{index}" 
                    :$"item{index}_{subIndex}";
            }
            else if(elementType.IsGenericType &&
                elementType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
            {
                return 
                    subIndex ==0 ?
                    $"list{index}"
                    :$"list{index}_{subIndex}";
            }
            return string.Empty;
        }
        private static string GetUniquePropertyFromTuple(Information info, StringBuilder builder)
        {
            foreach (var element in info.ReturnType.TupleElements)
            {
                var elementType = element.Type as INamedTypeSymbol;
                elementType = Common.GetNonNullableType(elementType);
                
            }

            return "";
        }

    }
}
