using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace SpExecuter.Generator
{
    internal enum Lifetime
    {
        Scoped, Singleton, Transient
    }
    [Generator]
    public class SpGenerator : IIncrementalGenerator
    {

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var interfaceDeclarations = context.SyntaxProvider
                 .CreateSyntaxProvider(IsCandidateInterface, GetInterfaceInfo).
                 Where(i => i is not null).
                 Collect();


            context.RegisterSourceOutput(interfaceDeclarations, GenerateCode);

        }

        private void GenerateCode(SourceProductionContext context, System.Collections.Immutable.ImmutableArray<ITypeSymbol> array)
        {

            //For Generating Implementation classes
            Dictionary<string, StringBuilder> allClassSyntax = new Dictionary<string, StringBuilder>();

            //For generating extension method for registring interfaces and classes for DI
            Dictionary<string, (string, Lifetime)> registerClasses = new Dictionary<string, (string, Lifetime)>();
            StringBuilder buildServices = new StringBuilder();

            //For Generating Response class conatining SP Response class info
            StringBuilder responseClassConfiguration = new StringBuilder();
            HashSet<string> uniqueResponseClasses = new HashSet<string>();

            //For Generating Response class conatining SP Request class info
            StringBuilder requestClassConfiguration = new StringBuilder();
            HashSet<string> uniqueRequestClasses = new HashSet<string>();
            if (!array.IsDefaultOrEmpty)
            {

                GeneratedExecuterClasses(allClassSyntax,
                 array, registerClasses, uniqueResponseClasses, uniqueRequestClasses);
                HashSet<string> addedClasses = new HashSet<string>();

                foreach (KeyValuePair<string, StringBuilder> generateClass in allClassSyntax)
                {
                    bool isClassAdded = addedClasses.Add(generateClass.Key);
                    if (isClassAdded)
                    {
                        context.AddSource($"{generateClass.Key}.g.cs", SourceText.From(generateClass.Value.ToString(), Encoding.UTF8));
                    }
                }


                GenerateRequestClassesConfiguration(uniqueRequestClasses, requestClassConfiguration);
                context.AddSource($"{"RequestClasses"}Generated.g.cs", SourceText.From(requestClassConfiguration.ToString(), Encoding.UTF8));

                GenerateResponseClassesConfiguration(uniqueResponseClasses, responseClassConfiguration);
                context.AddSource($"{"ResponseClasses"}Generated.g.cs", SourceText.From(responseClassConfiguration.ToString(), Encoding.UTF8));

            }

        }
        private static void GeneratedExecuterClasses(Dictionary<string, StringBuilder> allClassSyntax,
          ImmutableArray<ITypeSymbol> interfaces,
         Dictionary<string, (string, Lifetime)> registerClasses, HashSet<string> uniqueReturnClasses,
           HashSet<string> uniqueRequestClasses)
        {

            foreach (INamedTypeSymbol interfaceSymbol in interfaces)
            {

                AttributeData attr = interfaceSymbol.GetAttributes().FirstOrDefault()!;
                //allClassSyntax.Add("My"+i,new StringBuilder( interfaceSymbol.Name));
                // Get interface name and class name for DI
                string className = GetClassToRegister(registerClasses, interfaceSymbol.Name, attr);
                string namespaceName = interfaceSymbol.ContainingNamespace?.ToDisplayString()!;
                if (!allClassSyntax.ContainsKey(className))
                {
                    //Get class declaration syntax
                    StringBuilder classSyntax = new StringBuilder();
                    GetInitialSyntax(classSyntax,
                        namespaceName ?? "SpExecuter", className, interfaceSymbol.Name);

                    //Loop and Add Methods in class declared in interface
                    foreach (IMethodSymbol member in interfaceSymbol.GetMembers().OfType<IMethodSymbol>())
                    {
                        //Get Method Metadata for generating Declaration
                        string returnType = member.ReturnType.ToDisplayString();
                        string methodName = member.Name;

                        string parameters = string.Join(", ",
                            member.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));
                        //Get StoredProcedure Name
                        string spName = string.Empty;
                        foreach (AttributeData methodAttribute in member.GetAttributes())
                        {
                            INamedTypeSymbol methodAttributeSymbol = methodAttribute.AttributeClass;
                            if (methodAttributeSymbol?.Name == "StoredProcedure")
                            {
                                TypedConstant spNameArg = methodAttribute.ConstructorArguments[0];
                                spName = spNameArg.Value as string ?? "";
                            }
                        }

                        string connctionStringParamName = member.Parameters[0].Name;
                        string objectParamName = "null";
                        string requestTypeName = "NoRequest";
                        string paramNeeded = "false";

                        if (member.Parameters.Length > 1)
                        {
                            objectParamName = member.Parameters[1].Name;
                            requestTypeName = member.Parameters[1].Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                            uniqueRequestClasses.Add(requestTypeName);

                            paramNeeded = "true";
                        }
                        string returnTypes = GetReturnTypeStringForExecuter(member, uniqueReturnClasses);
                        string returnStatement = GenerateReturnStatement(member.ReturnType);

                        // Generate Method Implementation
                        classSyntax.AppendLine($"    public async {returnType} {methodName}({parameters})");
                        classSyntax.AppendLine("    {");
                        classSyntax.AppendLine("        List<ISpResponse>[] response =await SpExecutor.ExecuteSpToObjects(");
                        classSyntax.AppendLine($"           spName: \"{spName}\", dbName: {connctionStringParamName}, spNeedParameters: {paramNeeded},spEntity: {objectParamName}, ");
                        classSyntax.AppendLine($"           requestObjectNumber: SpRequest.{requestTypeName} ");
                        classSyntax.AppendLine($"           {returnTypes} );");
                        classSyntax.AppendLine($"           {returnStatement}");
                        classSyntax.AppendLine("    }");
                        classSyntax.AppendLine();
                    }

                    //Class implementation complete
                    classSyntax.AppendLine("}");

                    allClassSyntax.Add(className, classSyntax);
                }


            }
        }
        private ITypeSymbol GetInterfaceInfo(GeneratorSyntaxContext context, CancellationToken token)
        {
            if (context.Node is InterfaceDeclarationSyntax interfaceDecl &&
                    interfaceDecl.AttributeLists
                    .Where(a => a.Attributes.Any(b =>
                     b.Name.NormalizeWhitespace().ToFullString().Contains("SpHandler")))
                     .Count() > 0)
            {
                return context.SemanticModel.GetDeclaredSymbol(interfaceDecl) as ITypeSymbol;
            }
            else
            {
                return null;
            }
        }
        private bool IsCandidateInterface(SyntaxNode node, CancellationToken token)
        {
            bool result = false;
            if (node is InterfaceDeclarationSyntax interfaceDeclaration)
            {
                result = interfaceDeclaration.AttributeLists.Count > 0;
            }

            return result;
        }
        private static void GenerateResponseClassesConfiguration(HashSet<string> uniqueResponseCasses, StringBuilder builder)
        {
            builder.AppendLine($"namespace SpExecuter.Utility"); builder.AppendLine("{");
            builder.AppendLine($"   public class SpResponse");
            builder.AppendLine("    {");
            builder.AppendLine();
            builder.AppendLine($"       public const int SkipResponse = 0 ;");
            builder.AppendLine();

            int srNo = 1;
            foreach (string responseClass in uniqueResponseCasses)
            {
                builder.AppendLine($"public const int {responseClass} = {srNo} ;");
                builder.AppendLine();
                srNo++;
            }
            builder.AppendLine($@" }}
}}");
        }
        private static void GenerateRequestClassesConfiguration(HashSet<string> uniqueRequestClasses, StringBuilder builder)
        {
            builder.AppendLine($"namespace SpExecuter.Utility"); builder.AppendLine("{");
            builder.AppendLine($"   public class SpRequest");
            builder.AppendLine("    {");
            builder.AppendLine();
            builder.AppendLine($"   public const int NoRequest = 0 ;");
            builder.AppendLine();
            int srNo = 1;
            foreach (string requestClass in uniqueRequestClasses)
            {
                builder.AppendLine($"public const int {requestClass} = {srNo};");
                builder.AppendLine();
                srNo++;
            }
            builder.AppendLine($@"  }}
}}");

        }
        private static void CollectUserDefinedClasses(
                ITypeSymbol typeSymbol,
                List<string> outSet)
        {

            if (typeSymbol == null)
                return;

            // 1) If this is a named type (which includes tuples, generics, and non‐generic classes)
            if (typeSymbol is INamedTypeSymbol namedSym)
            {
                // 1a) TUPLE‐case: IsTupleType == true means this is e.g. (A, B) or (A, B, C, …)
                if (namedSym.IsTupleType)
                {
                    foreach (var element in namedSym.TupleElements)
                    {
                        CollectUserDefinedClasses(element.Type, outSet);
                    }
                    return;
                }

                // 1b) GENERIC‐case: unwrap all type arguments (e.g. ValueTask<T>, List<T>, Dictionary<A,B>, etc.)
                if (namedSym.IsGenericType)
                {
                    foreach (var typeArg in namedSym.TypeArguments)
                    {
                        CollectUserDefinedClasses(typeArg, outSet);
                    }
                    return;
                }

                // 1c) PLAIN CLASS (non‐generic, non‐tuple):
                //     If it’s a “user‐declared” class (i.e. TypeKind.Class + has DeclaringSyntaxReferences),
                //     add it to the set.
                if (namedSym.TypeKind == TypeKind.Class &&
                    namedSym.DeclaringSyntaxReferences.Length > 0)
                {
                    outSet.Add(namedSym.Name);
                }

                // 1d) ARRAY disguised as namedSym (rare):
                //     Sometimes arrays show up as IArrayTypeSymbol, but we handle that below.
                if (namedSym.TypeKind == TypeKind.Array &&
                    namedSym is IArrayTypeSymbol arraySym)
                {
                    CollectUserDefinedClasses(arraySym.ElementType, outSet);
                }

                return;
            }

            // 2) If it’s an array type (caught here if not already handled above)
            if (typeSymbol is IArrayTypeSymbol arrayType)
            {
                CollectUserDefinedClasses(arrayType.ElementType, outSet);
                return;
            }

            // 3) Otherwise (primitive, pointer, type parameter, etc.) → ignore
        }
        private static string GetReturnTypeStringForExecuter(IMethodSymbol member, HashSet<string> uniqueReturnClasses)
        {
            //GETMethods return type classes and add them in Hashset
            List<string> currentMethodsReturnTypes = new List<string>();
            CollectUserDefinedClasses(member.ReturnType, currentMethodsReturnTypes);
            // Transform each item to "SpResponse.ClassX"
            string[] responseArray = currentMethodsReturnTypes
                .Select(item => $"SpResponse.{item}")
                .ToArray();
            string returnTypes = string.Join(", ", responseArray);
            returnTypes = string.IsNullOrEmpty(returnTypes) ? ", " + returnTypes : string.Empty;
            foreach (string eachReturnType in currentMethodsReturnTypes)
            {
                uniqueReturnClasses.Add(eachReturnType);
            }
            return returnTypes;
        }

        private static string GenerateReturnStatement(ITypeSymbol returnType)
        {
           

            // 1) Unwrap Task<T> or ValueTask<T>
            if (returnType is INamedTypeSymbol named)
            {
                var def = named.OriginalDefinition?.ToDisplayString() ?? "";
                if (def.StartsWith("System.Threading.Tasks.Task<", StringComparison.Ordinal) ||
                    def.StartsWith("System.Threading.Tasks.ValueTask<", StringComparison.Ordinal))
                {
                    if (named.TypeArguments.Length > 0)
                        returnType = named.TypeArguments[0];
                    else
                        return "/* error: generic Task/ValueTask had no type-args */ return default;";
                }
            }

            string expr;

            // 2) Tuple via TupleElements
            if (returnType is INamedTypeSymbol tupleSym && tupleSym.IsTupleType)
            {
                var parts = new List<string>();
                for (int i = 0; i < tupleSym.TupleElements.Length; i++)
                {
                    var elemType = tupleSym.TupleElements[i].Type;

                    if (elemType is INamedTypeSymbol ne
                        && ne.OriginalDefinition?.ToDisplayString().StartsWith("System.Collections.Generic.List<", StringComparison.Ordinal) == true
                        && ne.TypeArguments.Length == 1)
                    {
                        // Use helper instead of direct cast
                        var itemT = ne.TypeArguments[0].ToDisplayString();
                        parts.Add($"SpExecutor.GetStronglyTypedList<{itemT}>(response[{i}])");
                    }
                    else
                    {
                        // Scalar or object
                        var tName = elemType?.ToDisplayString() ?? "object";
                        parts.Add($"({tName})response[{i}][0]");
                    }
                }
                expr = "(" + string.Join(", ", parts) + ")";
            }
            // 3) Plain List<T>
            else if (returnType is INamedTypeSymbol listSym
                     && listSym.OriginalDefinition?.ToDisplayString().StartsWith("System.Collections.Generic.List<", StringComparison.Ordinal) == true
                     && listSym.TypeArguments.Length == 1)
            {
                var itemT = listSym.TypeArguments[0].ToDisplayString();
                expr = $"SpExecutor.GetStronglyTypedList<{itemT}>(response[0])";
            }
            // 4) Single value
            else
            {
                var tName = returnType.ToDisplayString() ?? "object";
                expr = $"({tName})response[0][0]";
            }

            return "return " + expr + ";";
        }

        //private static string GenerateReturnStatement(ITypeSymbol returnType)
        //{
        //    if (returnType == null)
        //        return "/* error: returnType was null */ return default;";

        //    // 1) Unwrap Task<T> or ValueTask<T>
        //    if (returnType is INamedTypeSymbol named)
        //    {
        //        var def = named.OriginalDefinition?.ToDisplayString() ?? "";
        //        if (def.StartsWith("System.Threading.Tasks.Task<", StringComparison.Ordinal) ||
        //            def.StartsWith("System.Threading.Tasks.ValueTask<", StringComparison.Ordinal))
        //        {
        //            // only unwrap if there _is_ a type-arg
        //            if (named.TypeArguments.Length > 0)
        //                returnType = named.TypeArguments[0];
        //            else
        //                return "/* error: generic Task/ValueTask had no type-args */ return default;";
        //        }
        //    }

        //    string expr;

        //    // 2) Tuple via TupleElements (no TupleUnderlyingType)
        //    if (returnType is INamedTypeSymbol tupleSym && tupleSym.IsTupleType)
        //    {
        //        var parts = new List<string>();
        //        for (int i = 0; i < tupleSym.TupleElements.Length; i++)
        //        {
        //            var elemType = tupleSym.TupleElements[i].Type;
        //            if (elemType is INamedTypeSymbol ne
        //                && ne.OriginalDefinition?.ToDisplayString().StartsWith("System.Collections.Generic.List<", StringComparison.Ordinal) == true
        //                && ne.TypeArguments.Length == 1)
        //            {
        //                // List<T> element
        //                var itemT = ne.TypeArguments[0].ToDisplayString();
        //                parts.Add($"(List<{itemT}>)(response[{i}])");
        //            }
        //            else
        //            {
        //                // Scalar or object
        //                var tName = elemType?.ToDisplayString() ?? "object";
        //                parts.Add($"({tName})response[{i}][0]");
        //            }
        //        }
        //        expr = "(" + string.Join(", ", parts) + ")";
        //    }
        //    // 3) Plain List<T>
        //    else if (returnType is INamedTypeSymbol listSym
        //             && listSym.OriginalDefinition?.ToDisplayString().StartsWith("System.Collections.Generic.List<", StringComparison.Ordinal) == true
        //             && listSym.TypeArguments.Length == 1)
        //    {
        //        var itemT = listSym.TypeArguments[0].ToDisplayString();
        //        expr = $"(List<{itemT}>)response[0]";
        //    }
        //    // 4) Single value
        //    else
        //    {
        //        var tName = returnType.ToDisplayString() ?? "object";
        //        expr = $"({tName})response[0][0]";
        //    }

        //    return "return " + expr + ";";
        //}


        /* public static string GenerateReturnStatement(ITypeSymbol returnType)
         {
             string originalDef = returnType.OriginalDefinition.ToDisplayString();
             bool isGenericTask = originalDef.StartsWith("System.Threading.Tasks.Task<");
             bool isGenericValueTask = originalDef.StartsWith("System.Threading.Tasks.ValueTask<");

             // Unwrap Task<T> or ValueTask<T>
             ITypeSymbol innerReturnType = returnType;
             if ((isGenericTask || isGenericValueTask) &&
                 returnType is INamedTypeSymbol namedType &&
                 namedType.TypeArguments.Length == 1)
             {
                 innerReturnType = namedType.TypeArguments[0];
             }

             ExpressionSyntax returnExpr;

             // Handle tuple types (C# tuple or System.Tuple)
             if (innerReturnType is INamedTypeSymbol namedInner &&
                 (namedInner.IsTupleType || namedInner.OriginalDefinition.ToDisplayString().StartsWith("System.Tuple<")))
             {
                 var elementTypes = namedInner.IsTupleType
                     ? namedInner.TupleElements.Select(e => e.Type).ToList()
                     : namedInner.TypeArguments.ToList();

                 var elementExprs = elementTypes.Select((elementType, index) =>
                 {
                     var elementAccess = SyntaxFactory.ElementAccessExpression(
                         SyntaxFactory.ElementAccessExpression(
                             SyntaxFactory.IdentifierName("response"),
                             SyntaxFactory.BracketedArgumentList(
                                 SyntaxFactory.SingletonSeparatedList(
                                     SyntaxFactory.Argument(
                                         SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(index))
                                     )
                                 )
                             )
                         ),
                         SyntaxFactory.BracketedArgumentList(
                             SyntaxFactory.SingletonSeparatedList(
                                 SyntaxFactory.Argument(
                                     SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0))
                                 )
                             )
                         )
                     );

                     return SyntaxFactory.CastExpression(
                         SyntaxFactory.ParseTypeName(elementType.ToDisplayString()),
                         elementAccess
                     );
                 }).ToList();

                 if (namedInner.IsTupleType)
                 {
                     returnExpr = SyntaxFactory.TupleExpression(
                         SyntaxFactory.SeparatedList(elementExprs.Select(SyntaxFactory.Argument))
                     );
                 }
                 else
                 {
                     returnExpr = SyntaxFactory.ObjectCreationExpression(
                         SyntaxFactory.ParseTypeName(innerReturnType.ToDisplayString())
                     ).WithArgumentList(
                         SyntaxFactory.ArgumentList(
                             SyntaxFactory.SeparatedList(elementExprs.Select(SyntaxFactory.Argument))
                         )
                     );
                 }
             }
             else if (innerReturnType is INamedTypeSymbol namedList &&
                      namedList.OriginalDefinition.ToDisplayString().StartsWith("System.Collections.Generic.List<"))
             {
                 var elementAccess = SyntaxFactory.ElementAccessExpression(
                     SyntaxFactory.ElementAccessExpression(
                         SyntaxFactory.IdentifierName("response"),
                         SyntaxFactory.BracketedArgumentList(
                             SyntaxFactory.SingletonSeparatedList(
                                 SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))
                             )
                         )
                     ),
                     SyntaxFactory.BracketedArgumentList(
                         SyntaxFactory.SingletonSeparatedList(
                             SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))
                         )
                     )
                 );

                 returnExpr = SyntaxFactory.CastExpression(
                     SyntaxFactory.ParseTypeName(innerReturnType.ToDisplayString()),
                     elementAccess
                 );
             }
             else
             {
                 var elementAccess = SyntaxFactory.ElementAccessExpression(
                     SyntaxFactory.ElementAccessExpression(
                         SyntaxFactory.IdentifierName("response"),
                         SyntaxFactory.BracketedArgumentList(
                             SyntaxFactory.SingletonSeparatedList(
                                 SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))
                             )
                         )
                     ),
                     SyntaxFactory.BracketedArgumentList(
                         SyntaxFactory.SingletonSeparatedList(
                             SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))
                         )
                     )
                 );

                 returnExpr = SyntaxFactory.CastExpression(
                     SyntaxFactory.ParseTypeName(innerReturnType.ToDisplayString()),
                     elementAccess
                 );
             }

             if (isGenericTask)
             {
                 returnExpr = SyntaxFactory.InvocationExpression(
                     SyntaxFactory.MemberAccessExpression(
                         SyntaxKind.SimpleMemberAccessExpression,
                         SyntaxFactory.ParseName("System.Threading.Tasks.Task"),
                         SyntaxFactory.GenericName("FromResult")
                             .WithTypeArgumentList(
                                 SyntaxFactory.TypeArgumentList(
                                     SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                         SyntaxFactory.ParseTypeName(innerReturnType.ToDisplayString())
                                     )
                                 )
                             )
                     ),
                     SyntaxFactory.ArgumentList(
                         SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(returnExpr))
                     )
                 );
             }
             else if (isGenericValueTask)
             {
                 returnExpr = SyntaxFactory.ObjectCreationExpression(
                     SyntaxFactory.ParseTypeName($"System.Threading.Tasks.ValueTask<{innerReturnType.ToDisplayString()}>")
                 ).WithArgumentList(
                     SyntaxFactory.ArgumentList(
                         SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(returnExpr))
                     )
                 );
             }


             return SyntaxFactory.ReturnStatement(returnExpr).ToFullString().Substring(9);
         }
 */
        private static void GetInitialSyntax(StringBuilder builder,
            string namespaceName, string className, string interfaceName)
        {
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using SpExecuter.Utility;");
            builder.AppendLine($"namespace {namespaceName};");
            builder.AppendLine($"public class {className} : {interfaceName}");
            builder.AppendLine("{");

        }
        private static string GetClassToRegister(Dictionary<string, (string, Lifetime)> pairs,
            string interfaceName, AttributeData attr)
        {
            string className = string.Empty;
            if (interfaceName[0] == 'I' || interfaceName[0] == 'i')
            {
                className = interfaceName.Substring(1);

            }
            else
            {
                className = interfaceName + "Class";

            }
            TypedConstant lifeTime = attr.ConstructorArguments[0];
            var lifetime = lifeTime.Value is int intValue
                            ? (Lifetime)intValue
                            : Lifetime.Singleton;
            if (!pairs.ContainsKey(interfaceName))
            {
                pairs.Add(interfaceName, (className, lifetime));
            }
            return className;
        }
    }
}
