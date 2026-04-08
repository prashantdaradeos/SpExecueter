using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpExecuter.Generator
{
    internal sealed class Validations
    {
        internal static void MethodRetunTypeValidation(Information info)
        {
            string returnType = info.MethodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            if (!returnType.StartsWith("global::System.Threading.Tasks.Task"))
            {
                var diagnostic = Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "SE001",
                            title: "Invalid return type",
                            messageFormat: "Method '{0}' must return Task<T>",
                            category: "InvalidReturnTypeRules",
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true),
                        info.MethodSymbol.Locations.FirstOrDefault(),
                        info.MethodSymbol.Name);
                info.BuildFailed = true;
                info.Context.ReportDiagnostic(diagnostic);
            }
        }
        internal static bool ConnectionStringValidation(Information info)
        {
            if (info.MethodSymbol.Parameters.Length == 0 ||
            (info.MethodSymbol.Parameters[0].Type.SpecialType != SpecialType.System_String))
            {
                var diagnostic = Diagnostic.Create(
                       descriptor: new DiagnosticDescriptor(
                           id: "SE004",
                           title: "Missing Connection string",
                           messageFormat: "Method '{0}' must have valid Connection string parameter.",
                           category: "ConnectionStringRules",
                           DiagnosticSeverity.Error,
                           isEnabledByDefault: true),
                       location: info.MethodSymbol.Locations.FirstOrDefault(),
                       messageArgs: info.MethodSymbol.Name);

                info.Context.ReportDiagnostic(diagnostic);
                info.BuildFailed = true;
                return false;
            }
            return NonQueryReturnTypeValidation(info);


        }
        internal static bool NonQueryReturnTypeValidation(Information info)
        {
            var returnType = info.MethodSymbol.ReturnType as INamedTypeSymbol;
            returnType = returnType.TypeArguments[0] as INamedTypeSymbol;


            if (info.IsNonQuery && returnType.SpecialType != SpecialType.System_Int32)
            {
                var diagnostic = Diagnostic.Create(
                       descriptor: new DiagnosticDescriptor(
                           id: "SE005",
                           title: "Invalid return type",
                           messageFormat: "Non Query Method '{0}' must have valid Task<int> as return type.",
                           category: "InvalidReturnTypeRules",
                           DiagnosticSeverity.Error,
                           isEnabledByDefault: true),
                       location: info.MethodSymbol.Locations.FirstOrDefault(),
                       messageArgs: info.MethodSymbol.Name);
                info.BuildFailed = true;

                info.Context.ReportDiagnostic(diagnostic);

                return false;
            }


            return true;
        }
        internal static void StoredProcedureAttributeValidation(Information info)
        {
            if (info.AttributeAdded)
            {
                return;
            }
            var diagnostic = Diagnostic.Create(
                  descriptor: new DiagnosticDescriptor(
                      id: "SE002",
                      title: "Missing StoredProcedure attribute",
                      messageFormat: "Method '{0}' must have [StoredProcedure] attribute applied.",
                      category: "AttributeRules",
                      DiagnosticSeverity.Error,  // ❌ Fail the build
                      isEnabledByDefault: true),
                  location: info.MethodSymbol.Locations.FirstOrDefault(),
                  messageArgs: info.MethodSymbol.Name);
            info.BuildFailed = true;

            info.Context.ReportDiagnostic(diagnostic);
        }
        internal static bool StoredProcedureValidation(Information info)
        {
            if (string.IsNullOrWhiteSpace(info.StoredProcedureName))
            {
                var diagnostic = Diagnostic.Create(
                       descriptor: new DiagnosticDescriptor(
                           id: "SE003",
                           title: "Missing Stored Procedure",
                           messageFormat: "Method '{0}' must have valid Stored Procedure name.",
                           category: "StoredProcedureRules",
                           DiagnosticSeverity.Error,
                           isEnabledByDefault: true),
                       location: info.MethodSymbol.Locations.FirstOrDefault(),
                       messageArgs: info.MethodSymbol.Name);
                info.BuildFailed = true;

                info.Context.ReportDiagnostic(diagnostic);
                return false;
            }
            return true;
        }
        internal static void InputPropertiesValidation(Information info, RequestProp req)
        {

            var diagnostic = Diagnostic.Create(
                   descriptor: new DiagnosticDescriptor(
                       id: "SE006",
                       title: "Property Restricted",
                       messageFormat: "Please remove Unique or ResultExclusion property from {0} input parameter",
                       category: "PropertyRules",
                       DiagnosticSeverity.Error,
                       isEnabledByDefault: true),
                   location: req.PropertySymbol.Locations.FirstOrDefault(),
                   messageArgs: req.PropertySymbol.Name);
            info.BuildFailed = true;

            info.Context.ReportDiagnostic(diagnostic);

        }

        internal static void ValidateUniqueConstraint(Information info, INamedTypeSymbol mainReturnType)
        {

            foreach (var eachReturnType in mainReturnType.TupleElements)
            {
                var returnType = eachReturnType.Type as INamedTypeSymbol;
                if (returnType.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    returnType = returnType.TypeArguments[0] as INamedTypeSymbol;
                }
                if (returnType.TypeKind == TypeKind.Class &&
                        returnType.DeclaringSyntaxReferences.Length > 0)
                {
                    var (hasUniqueProp, uniqueAttributeName) = CheckClassHasUniqueAttribute(info, returnType);
                    if (!hasUniqueProp)
                    {
                        UniqueValidationForORCondition(info, returnType);

                    }
                }
                else if (returnType.IsGenericType &&
                    returnType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List"))
                {

                    var listTypeArg = returnType.TypeArguments[0] as INamedTypeSymbol;
                    var (hasUniqueProp, uniqueAttributeName) = CheckClassHasUniqueAttribute(info, listTypeArg);
                    if (!hasUniqueProp)
                    {
                        UniqueValidationForORCondition(info, listTypeArg);

                    }

                }
            }

        }
        internal static (bool, string) CheckClassHasUniqueAttribute(Information info, INamedTypeSymbol returnType)
        {
            string uniqueAttributeName1 = "";
            Dictionary<int, INamedTypeSymbol> inheritanceInfo = new Dictionary<int, INamedTypeSymbol>();
            Common.GetInheritanceInfo(returnType, 0, inheritanceInfo);
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
                        var dbParamArg = paramConfigAttr.NamedArguments
                            .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.DBParam));
                        string dbParamArgValue = Convert.ToString(dbParamArg.Value.Value);
                        if (!string.IsNullOrWhiteSpace(dbParamArgValue))
                        {
                            uniqueAttributeName1 = dbParamArgValue;
                        }
                        return (true, uniqueAttributeName1);
                    }

                }
            }
            return (false, string.Empty);
        }
        internal static void UniqueValidationForORCondition(Information info, INamedTypeSymbol returnType)
        {

            var diagnostic = Diagnostic.Create(
                   descriptor: new DiagnosticDescriptor(
                       id: "SE008",
                       title: "Unique Requirement",
                       messageFormat: "Please add a Unique attribute to one property in '{0}' for the return type of method '{1}'.",
                       category: "OrConditionRules",
                       DiagnosticSeverity.Error,
                       isEnabledByDefault: true),
                   location: info.MethodSymbol.Locations.FirstOrDefault(),
                   messageArgs: new object[2] { returnType.ToDisplayString(), info.MethodSymbol.Name });
            info.BuildFailed = true;

            info.Context.ReportDiagnostic(diagnostic);
        }

        internal static void ValidateNestedTuple(Information info, INamedTypeSymbol mainReturnType)
        {
            Dictionary<int, INamedTypeSymbol> nestedTupleElements = new Dictionary<int, INamedTypeSymbol>();
            List<int> returnCounts = new List<int>();
            int position = 0;

            foreach (var eachReturnType in mainReturnType.TupleElements)
            {
                var returnType = eachReturnType.Type as INamedTypeSymbol;

                if (returnType.IsTupleType &&
                    returnType.NullableAnnotation != NullableAnnotation.Annotated)
                {
                    NullValidationForNestedTuple(info, returnType);
                }
                returnType = returnType.NullableAnnotation == NullableAnnotation.Annotated ?
                    returnType.TypeArguments[0] as INamedTypeSymbol : returnType;


                nestedTupleElements.Add(position, returnType);
                returnCounts.Add(Common.GetReturnCountFromTuple(info, returnType));

                position++;

            }

            int sameElements = FindSameElements(info, returnCounts, nestedTupleElements);
            info.SameElementsInNestedTuple = sameElements;
            ValidateUniqueConstraintInNestedTuple(info, mainReturnType, sameElements);
        }
        internal static void DifferentTypesValidation(Information info, INamedTypeSymbol returnType, int position)
        {

            var diagnostic = Diagnostic.Create(
                   descriptor: new DiagnosticDescriptor(
                       id: "SE011",
                       title: "Same Type Requirement",
                       messageFormat: "Found different types with same unique attribute in the return type of method '{1}'. The conflicting type is at index {0}.",
                       category: "TypeRules",
                       DiagnosticSeverity.Error,
                       isEnabledByDefault: true),
                   location: info.MethodSymbol.Locations.FirstOrDefault(),
                   messageArgs: new object[2] { position, info.MethodSymbol.Name });
            info.BuildFailed = true;

            info.Context.ReportDiagnostic(diagnostic);
        }
        internal static void UniqueValidation(Information info, INamedTypeSymbol returnType)
        {

            var diagnostic = Diagnostic.Create(
                   descriptor: new DiagnosticDescriptor(
                       id: "SE012",
                       title: "Unique Requirement",
                       messageFormat: "Unique attribute not found for type {0} in method {1}",
                       category: "UniqueRules",
                       DiagnosticSeverity.Error,
                       isEnabledByDefault: true),
                   location: info.MethodSymbol.Locations.FirstOrDefault(),
                   messageArgs: new object[2] { returnType.Name, info.MethodSymbol.Name });
            info.BuildFailed = true;

            info.Context.ReportDiagnostic(diagnostic);
        }
        private static void ValidateUniqueConstraintInNestedTuple(Information info,
            INamedTypeSymbol mainReturnType, int sameElements)
        {
            Dictionary<int, List<INamedTypeSymbol>> flattenedTypes = new Dictionary<int, List<INamedTypeSymbol>>();

            mainReturnType.TupleElements.Select((returnType, index) =>
            {
                var flattened = new List<INamedTypeSymbol>();
                var type = returnType.Type as INamedTypeSymbol;
                type = Common.GetNonNullableType(type);
                Common.Flatten(type, flattened);
                flattenedTypes.Add(index, flattened);
                return "";
            }).ToList();
            string[] uniqueProperties = flattenedTypes.
                                Select(kvp =>
                                {
                                    if (kvp.Value.Count <= sameElements)
                                    {
                                        return "";
                                    }
                                    return Common.GetUniqueProperty(info, kvp.Value[sameElements]);
                                }).ToArray();
            ValidateRecurrsively(info, flattenedTypes.Values.ToList(), uniqueProperties, sameElements);

        }
        private static bool ValidateRecurrsively(Information info,
                   List<List<INamedTypeSymbol>> flattenedTypes, string[] uniqueKeys,
                  int forPosition)
        {
            // Step 1: If only one entry, return true
            if (flattenedTypes.Count == 1)
            {
                return true;
            }

            // Step 2b: Separate lists that are shorter than forPosition (they pass automatically)
            List<List<INamedTypeSymbol>> activeLists = new List<List<INamedTypeSymbol>>();
            for (int i = 0; i < flattenedTypes.Count; i++)
            {
                if (flattenedTypes[i].Count > forPosition)
                {
                    activeLists.Add(flattenedTypes[i]);
                }
            }

            // If after filtering we have 0 or 1 active lists, validation passes
            if (activeLists.Count <= 1)
            {
                return true;
            }

            // Step 2a: Unwrap all generic types at forPosition to inner types for comparison
            INamedTypeSymbol[] unwrappedTypes = new INamedTypeSymbol[activeLists.Count];
            for (int i = 0; i < activeLists.Count; i++)
            {
                var typeAtPos = activeLists[i][forPosition];
                typeAtPos = Common.GetNonNullableType(typeAtPos);
                unwrappedTypes[i] = Common.GetInnerType(typeAtPos);
            }

            // Check if all unwrapped types at this position are the same
            bool allSameType = true;
            for (int i = 1; i < unwrappedTypes.Length; i++)
            {
                if (!SymbolEqualityComparer.Default.Equals(unwrappedTypes[0], unwrappedTypes[i]))
                {
                    allSameType = false;
                    break;
                }
            }

            // If all types are the same at this position, skip unique checks and recurse to next position
            if (allSameType)
            {
                return ValidateRecurrsively(info, activeLists, uniqueKeys, forPosition + 1);
            }

            // Different types found — now unique attribute validation is required
            string[] uniqueProps = new string[activeLists.Count];
            for (int i = 0; i < activeLists.Count; i++)
            {
                uniqueProps[i] = Common.GetUniqueProperty(info, activeLists[i][forPosition]);
            }

            // Check all entries at this position have a unique attribute
            bool allValid = true;
            for (int i = 0; i < uniqueProps.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(uniqueProps[i]))
                {
                    UniqueValidation(info, unwrappedTypes[i]);
                    allValid = false;
                }
            }
            if (!allValid)
            {
                return false;
            }

            // Divide activeLists into groups by same unwrapped inner type at forPosition
            var typeGroups = new List<List<int>>();
            var typeGroupRepresentatives = new List<INamedTypeSymbol>();
            for (int i = 0; i < activeLists.Count; i++)
            {
                bool foundGroup = false;
                for (int g = 0; g < typeGroups.Count; g++)
                {
                    if (SymbolEqualityComparer.Default.Equals(unwrappedTypes[i], typeGroupRepresentatives[g]))
                    {
                        typeGroups[g].Add(i);
                        foundGroup = true;
                        break;
                    }
                }
                if (!foundGroup)
                {
                    typeGroups.Add(new List<int> { i });
                    typeGroupRepresentatives.Add(unwrappedTypes[i]);
                }
            }

            // Within each type group, all unique attributes must be the same
            bool valid = true;
            for (int g = 0; g < typeGroups.Count; g++)
            {
                string groupUnique = uniqueProps[typeGroups[g][0]];
                for (int j = 1; j < typeGroups[g].Count; j++)
                {
                    if (uniqueProps[typeGroups[g][j]] != groupUnique)
                    {
                        DifferentTypesValidation(info, typeGroupRepresentatives[g], forPosition);
                        valid = false;
                        break;
                    }
                }
            }

            // All groups should have different unique attributes
            for (int g1 = 0; g1 < typeGroups.Count; g1++)
            {
                for (int g2 = g1 + 1; g2 < typeGroups.Count; g2++)
                {
                    if (uniqueProps[typeGroups[g1][0]] == uniqueProps[typeGroups[g2][0]])
                    {
                        DifferentTypesValidation(info, typeGroupRepresentatives[g2], forPosition);
                        valid = false;
                    }
                }
            }

            if (!valid)
            {
                return false;
            }

            // Pass each group to recursion for next position
            bool allGroupsValid = true;
            foreach (List<int> group in typeGroups)
            {
                var groupLists = group.Select(index => activeLists[index]).ToList();
                var groupUniqueProps = group.Select(index => uniqueProps[index]).ToArray();
                if (!ValidateRecurrsively(info, groupLists, groupUniqueProps, forPosition + 1))
                {
                    allGroupsValid = false;
                }
            }

            return allGroupsValid;
        }
        internal static int FindSameElements(Information info,
           List<int> returnCounts,
           Dictionary<int, INamedTypeSymbol> nestedTupleElements)
        {

            int sameElements = 0, i = 0;
            foreach (var returnType in nestedTupleElements)
            {
                i++;

                if (!returnCounts.All(k => i <= k))
                {
                    break;
                }
                bool isSame = HasSameDefinitionAtPosition(info, i, nestedTupleElements);
                if (!isSame)
                {
                    break;
                }
                sameElements++;
            }


            return sameElements;

        }

        public static bool HasSameDefinitionAtPosition(Information info,
         int checkNumber,
         Dictionary<int, INamedTypeSymbol> nestedTupleElements)
        {


            INamedTypeSymbol? expected = null;
            foreach (var symbol in nestedTupleElements.Values)
            {

                var extracted = GetNthElement(info, symbol, checkNumber);

                if (extracted == null)
                    return false;

                if (expected == null)
                {
                    expected = extracted;
                }

                if (!SymbolEqualityComparer.Default.Equals(expected, extracted))
                {
                    return false;
                }
            }

            return true;
        }

        private static INamedTypeSymbol? GetNthElement(Information info,
        INamedTypeSymbol symbol,
        int position)
        {
            if (symbol.NullableAnnotation == NullableAnnotation.Annotated)
            {
                symbol = symbol.TypeArguments[0] as INamedTypeSymbol;
            }
            var flattened = new List<INamedTypeSymbol>();
            Common.Flatten(symbol, flattened);

            if (position > flattened.Count)
                return null;

            return flattened[position - 1].NullableAnnotation == NullableAnnotation.Annotated ?
                    flattened[position - 1].TypeArguments[0] as INamedTypeSymbol : flattened[position - 1];
        }

        private static void NullValidationForNestedTuple(Information info, INamedTypeSymbol returnType)
        {

            var diagnostic = Diagnostic.Create(
                   descriptor: new DiagnosticDescriptor(
                       id: "SE007",
                       title: "Null Requirement",
                       messageFormat: "Nested tuple '{0}' must be nullable in the return type of method '{1}'.",
                       category: "NestedTupleRules",
                       DiagnosticSeverity.Error,
                       isEnabledByDefault: true),
                   location: info.MethodSymbol.Locations.FirstOrDefault(),
                   messageArgs: new object[2] { returnType.ToDisplayString(), info.MethodSymbol.Name });
            info.BuildFailed = true;

            info.Context.ReportDiagnostic(diagnostic);
        }
        internal static bool DuplicateInSingleTupleValidation(Information info,
            INamedTypeSymbol tupleType)
        {

            tupleType = Common.GetNonNullableType(tupleType);
            if (!tupleType.IsTupleType)
            {
                return true;
            }
            var flattenedTypes = new List<INamedTypeSymbol>();
            Common.Flatten(tupleType, flattenedTypes);
            for (var i = 0; i < tupleType.TupleElements.Length; i++)
            {
                var element = tupleType.TupleElements[i];
                var elementType = element.Type as INamedTypeSymbol;
                elementType = Common.GetNonNullableType(elementType);
                elementType = elementType.IsGenericType &&
                    elementType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List") ?
                    elementType.TypeArguments[0] as INamedTypeSymbol : elementType;

                for (int j = i + 1; j < flattenedTypes.Count; j++)
                {
                    var compareType = flattenedTypes[j];
                    compareType = Common.GetNonNullableType(compareType);
                    compareType = compareType.IsGenericType &&
                    compareType.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List") ?
                    compareType.TypeArguments[0] as INamedTypeSymbol : compareType;
                    if (SymbolEqualityComparer.Default.Equals(elementType, compareType))
                    {
                        ShowDuplicateError(info);
                        info.BuildFailed = true;
                        return false;
                    }

                }
            }

            return true;
        }
        internal static bool DuplicateTupleInNestedTupleValidation(Information info,
            INamedTypeSymbol mainReturnType)
        {
            List<List<INamedTypeSymbol>> list = new List<List<INamedTypeSymbol>>();
            mainReturnType = Common.GetNonNullableType(mainReturnType);
            foreach (var nestedTuple in mainReturnType.TupleElements)
            {
                var tupleType = nestedTuple.Type as INamedTypeSymbol;
                tupleType = Common.GetNonNullableType(tupleType);
                List<INamedTypeSymbol> flattenedTypes = new List<INamedTypeSymbol>();
                Common.Flatten(tupleType, flattenedTypes);
                list.Add(flattenedTypes);
            }
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].SequenceEqual(list[j], SymbolEqualityComparer.Default))
                    {
                        ShowDuplicateTupleError(info);
                        info.BuildFailed = true;
                        return false;
                    }
                }
            }

            /* 
             if (!mainReturnType.IsTupleType)
             {
                 return true;
             }
             for(int i=0;i< mainReturnType.TupleElements.Length; i++)
             {
                 var element = mainReturnType.TupleElements[i];
                 var elementType = element.Type as INamedTypeSymbol;
                 elementType = Common.GetNonNullableType(elementType);
                 if (!elementType.IsTupleType)
                 {
                     elementType=elementType.IsGenericType &&
                         elementType.OriginalDefinition.ToDisplayString(
                             SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List") ?
                         elementType.TypeArguments[0] as INamedTypeSymbol : elementType;
                     elementType = Common.GetNonNullableType(elementType);

                 }
                 for(int j=i+1; j < mainReturnType.TupleElements.Length; j++)
                 {
                     var compareElement = mainReturnType.TupleElements[j];
                     var compareElementType = compareElement.Type as INamedTypeSymbol;
                     compareElementType = Common.GetNonNullableType(compareElementType);
                     if (!compareElementType.IsTupleType)
                     {
                         compareElementType = compareElementType.IsGenericType && compareElementType.OriginalDefinition.ToDisplayString(
                             SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Collections.Generic.List") ?
                         compareElementType.TypeArguments[0] as INamedTypeSymbol : compareElementType;
                         compareElementType = Common.GetNonNullableType(compareElementType);

                         if(SymbolEqualityComparer.Default.Equals(elementType, compareElementType))
                         {
                             ShowDuplicateError(info);
                             info.BuildFailed = true;
                             return false;
                         }
                         continue;
                     }

                     bool areEqual = AreTupleTypesEqual(elementType, compareElementType);
                     if (areEqual)
                     {
                         ShowDuplicateTupleError(info);
                         info.BuildFailed = true;
                         return false;
                     }
                 }
             }*/
            return true;

        }
        private static bool AreTupleTypesEqual(INamedTypeSymbol type1, INamedTypeSymbol type2)
        {
            type1 = Common.GetNonNullableType(type1);
            type2 = Common.GetNonNullableType(type2);

            if (!type1.IsTupleType || !type2.IsTupleType)
            {
                return false;
            }
            if (type1.TupleElements.Length != type2.TupleElements.Length)
            {
                return false;
            }
            for (int i = 0; i < type1.TupleElements.Length; i++)
            {
                var element1 = type1.TupleElements[i];
                var elementType1 = element1.Type as INamedTypeSymbol;
                elementType1 = Common.GetNonNullableType(elementType1);
                var element2 = type2.TupleElements[i];
                var elementType2 = element2.Type as INamedTypeSymbol;
                elementType2 = Common.GetNonNullableType(elementType2);
                if (!SymbolEqualityComparer.Default.Equals(elementType1, elementType2))
                {
                    return false;
                }
            }
            return true;
        }
        private static void ShowDuplicateError(Information info)
        {
            var diagnostic = Diagnostic.Create(
                       descriptor: new DiagnosticDescriptor(
                           id: "SE009",
                           title: "Duplicate Types",
                           messageFormat: "Method '{0}' must have unique return types in tuple.",
                           category: "ReturnTypesRules",
                           DiagnosticSeverity.Error,
                           isEnabledByDefault: true),
                       location: info.MethodSymbol.Locations.FirstOrDefault(),
                       messageArgs: info.MethodSymbol.Name);
            info.BuildFailed = true;

            info.Context.ReportDiagnostic(diagnostic);
        }
        private static void ShowDuplicateTupleError(Information info)
        {
            var diagnostic = Diagnostic.Create(
                       descriptor: new DiagnosticDescriptor(
                           id: "SE010",
                           title: "Duplicate Types",
                           messageFormat: "Method '{0}' must not contain duplicate nested tuple types in its return type. Each nested tuple type must be unique.",
                           category: "ReturnTypesRules",
                           DiagnosticSeverity.Error,
                           isEnabledByDefault: true),
                       location: info.MethodSymbol.Locations.FirstOrDefault(),
                       messageArgs: info.MethodSymbol.Name);
            info.BuildFailed = true;

            info.Context.ReportDiagnostic(diagnostic);
        }

        internal static void ValidateOutTypes(Information info,StringBuilder result)
        {
            Dictionary<int, List<INamedTypeSymbol>> allFlattenedTypes = new Dictionary<int, List<INamedTypeSymbol>>();

            if (info.ReturnType.IsTupleType)
            {
                if (info.ConditionType == ConditionType.OR)
                {
                    int nestedTupleSequence = 0;
                    foreach (var nestedTuple in info.ReturnType.TupleElements)
                    {
                        var tupleType = nestedTuple.Type as INamedTypeSymbol;
                        tupleType = Common.GetNonNullableType(tupleType);
                        List<INamedTypeSymbol> flattenedType = new List<INamedTypeSymbol>();
                        flattenedType.Add(tupleType);
                        allFlattenedTypes[nestedTupleSequence++] = flattenedType;
                    }
                }
                else
                {
                    int nestedTupleSequence = 0;
                    foreach (var nestedTuple in info.ReturnType.TupleElements)
                    {
                        var tupleType = nestedTuple.Type as INamedTypeSymbol;
                        tupleType = Common.GetNonNullableType(tupleType);
                        List<INamedTypeSymbol> flattenedTypes = new List<INamedTypeSymbol>();
                        Common.Flatten(tupleType, flattenedTypes);
                        allFlattenedTypes[nestedTupleSequence++] = flattenedTypes;
                    }
                }
                foreach (var kvp in allFlattenedTypes)
                {
                    foreach (var type in kvp.Value)
                    {
                        var nonNullableType = Common.GetNonNullableType(type);
                        var innerType = Common.GetInnerType(nonNullableType);
                        innerType = Common.GetNonNullableType(innerType);
                        ValidateOutType(info, innerType);

                    }
                }
            }
            else {
                var nonNullableType = Common.GetNonNullableType(info.ReturnType);
                var innerType = Common.GetInnerType(nonNullableType);
                innerType = Common.GetNonNullableType(innerType);
                ValidateOutType(info, innerType);
            }


          
        }

        private static void ValidateOutType(Information info,INamedTypeSymbol classSymbol )
        {
            Dictionary<int, INamedTypeSymbol> inheritanceInfo = new Dictionary<int, INamedTypeSymbol>();
            Common.GetInheritanceInfo(classSymbol, 0, inheritanceInfo);
 
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
                    if (paramConfigAttr != null)
                    {

                        var paramExclusionArg = paramConfigAttr.NamedArguments
                                .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.ParamExclusion));
                        bool paramExclusionArgValue = Convert.ToBoolean(paramExclusionArg.Value.Value);
                        if (paramExclusionArg.Key !=null || paramExclusionArgValue)
                        {
                            OutPropertiesValidation(info, propertySymbol, "ParamExclusion");
                        }

                        var outParamArg = paramConfigAttr.NamedArguments
                                .FirstOrDefault(kv => kv.Key == nameof(ParamConfig.OutParam));
                        bool outParamArgValue = Convert.ToBoolean(outParamArg.Value.Value);
                        if (outParamArgValue)
                        {
                            OutPropertiesValidation(info, propertySymbol, "OutParam");
                        }
                    }

                  
                }

            }
        }
        private static void OutPropertiesValidation(Information info, IPropertySymbol req,string attributeName)
        {

            var diagnostic = Diagnostic.Create(
                   descriptor: new DiagnosticDescriptor(
                       id: "SE006",
                       title: "Property Restricted",
                       messageFormat: "Please remove {1} property from {0} output parameter",
                       category: "PropertyRules",
                       DiagnosticSeverity.Error,
                       isEnabledByDefault: true),
                   location: req.Locations.FirstOrDefault(),
                   messageArgs: new object[] { req.Name, attributeName });
            info.BuildFailed = true;

            info.Context.ReportDiagnostic(diagnostic);

        }

    }
    }
