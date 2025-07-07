using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
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

            Dictionary<string,StringBuilder> tVPsClasses = new Dictionary<string, StringBuilder>();
            if (!array.IsDefaultOrEmpty)
            {

                GeneratedExecuterClasses(allClassSyntax,
                 array, registerClasses, uniqueResponseClasses, uniqueRequestClasses, tVPsClasses);

                if (allClassSyntax.Count < 1)
                {
                    return;
                }
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
                context.AddSource($"RequestClasses.g.cs", SourceText.From(requestClassConfiguration.ToString(), Encoding.UTF8));

                GenerateResponseClassesConfiguration(uniqueResponseClasses, responseClassConfiguration);
                context.AddSource($"ResponseClasses.g.cs", SourceText.From(responseClassConfiguration.ToString(), Encoding.UTF8));

                GenerateRuntimeDependencies(registerClasses, buildServices, 
                    uniqueRequestClasses, uniqueResponseClasses, tVPsClasses);
                context.AddSource($"StartupExtension.g.cs", SourceText.From(buildServices.ToString(), Encoding.UTF8));

            }

        }

        private static void GenerateRuntimeDependencies(Dictionary<string, (string, Lifetime)> registerClasses,
            StringBuilder buildServices, HashSet<string> uniqueRequestClasses,
            HashSet<string> uniqueResponseClasses, Dictionary<string, StringBuilder> tVPsClasses)
        {   
            buildServices.AppendLine(
                $@"
using Microsoft.Extensions.DependencyInjection;
using System.Data;
namespace SpExecuter.Utility

{{
    public class SpExecuterRegistration : ISpExecuterRegistration
    {{
                     
        public void RegisterForDependencyInjection(IServiceCollection services)
        {{  ");


            foreach (var classInfo in registerClasses)
            {
            buildServices.AppendLine($@"             services.Add{registerClasses[classInfo.Key].Item2.ToString()}<{classInfo.Key},{classInfo.Value.Item1}>();");
            }
            buildServices.AppendLine($"             DBConstants.SpRequestClassesCount = SpRequest.TotalCount ;");
            buildServices.AppendLine($"             DBConstants.SpResponseClassesCount = SpResponse.TotalCount ;");

            buildServices.AppendLine($"             DBConstants.SpRequestModelTypeArray = new Type[] {{");
            buildServices.AppendLine($"                 typeof(SpExecuter.Utility.NoRequest),");

            foreach (string className in uniqueRequestClasses)
            {
                buildServices.AppendLine($"                 typeof({className}),");
            }
            buildServices.AppendLine("              };");
            buildServices.AppendLine($"             DBConstants.SpResponseModelTypeArray = new Type[] {{");
            buildServices.AppendLine($"                 typeof(SpExecuter.Utility.SkipResponse),");

            foreach (string className in uniqueResponseClasses)
            {
                buildServices.AppendLine($"                 typeof({className}),");
            }
            buildServices.AppendLine("              };");

            buildServices.AppendLine(@$"            DBConstants.tVPsdelegates = new Dictionary<string, Delegate>(){{");

            foreach(var keyPair in tVPsClasses)
            {
                buildServices.AppendLine($"           [\"{keyPair.Key.Split('.').Last()}\"] = {keyPair.Value}");
            }
            buildServices.AppendLine(@$" 
            }};
        }}
    }}
}}");

        }
           

        private static void GeneratedExecuterClasses(Dictionary<string, StringBuilder> allClassSyntax,
              ImmutableArray<ITypeSymbol> interfaces,
             Dictionary<string, (string, Lifetime)> registerClasses, HashSet<string> uniqueReturnClasses,
               HashSet<string> uniqueRequestClasses, Dictionary<string, StringBuilder> tVPsClasses)
        {

            foreach (INamedTypeSymbol interfaceSymbol in interfaces)
            {

                AttributeData attr = interfaceSymbol.GetAttributes().FirstOrDefault()!;
                //allClassSyntax.Add("My"+i,new StringBuilder( interfaceSymbol.Name));
                // Get interface name and class name for DI
                string className = GetClassToRegister(registerClasses, interfaceSymbol, attr);
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
                            requestTypeName = member.Parameters[1].Type.ToDisplayString();
                            uniqueRequestClasses.Add(requestTypeName);
                            GenerateTVP(member.Parameters[1].Type, tVPsClasses);
                            paramNeeded = "true";
                        }
                        string returnTypes = GetReturnTypeStringForExecuter(member, uniqueReturnClasses);
                        string returnStatement = GenerateReturnStatement(member.ReturnType);

                        // Generate Method Implementation
                        classSyntax.AppendLine($"    public async {returnType} {methodName}({parameters})");
                        classSyntax.AppendLine("    {");
                        classSyntax.AppendLine("        List<ISpResponse>[] response =await SpExecutor.ExecuteSpToObjects(");
                        classSyntax.AppendLine($"           spName: \"{spName}\", dbName: {connctionStringParamName}, spNeedParameters: {paramNeeded},spEntity: {objectParamName}, ");
                        classSyntax.AppendLine($"           requestObjectNumber: SpRequest.{requestTypeName.Split('.').Last()} ");
                        classSyntax.AppendLine($"           {returnTypes} );");
                        classSyntax.AppendLine($"           {returnStatement}");
                        classSyntax.AppendLine("    }");
                        classSyntax.AppendLine();
                    }

                    //Class implementation complete
                    classSyntax.AppendLine("}}");

                    allClassSyntax.Add(className, classSyntax);
                }


            }
        }

        private static void GenerateTVP(ITypeSymbol type,
           Dictionary<string, StringBuilder> tVPsClasses)
        {
            

            foreach (var member in type.GetMembers())
            {
                if (member is IPropertySymbol property)
                {
                    if (property.Type is INamedTypeSymbol propertyType && propertyType.IsGenericType)
                    {
                        // Check if it's a generic List<T>
                        bool isList = propertyType.OriginalDefinition.ToDisplayString() == "System.Collections.Generic.List<T>";

                        if (isList)
                        {
                            string propertyName = property.Name;
                            var elementType = propertyType.TypeArguments[0]; // Get T from List<T>
                            string elementTypeName = elementType.ToDisplayString();
                            
                            if (tVPsClasses.ContainsKey(elementTypeName))
                            {
                                continue;
                            }
                            StringBuilder tvpFunc = new StringBuilder();
                            GenerateFunctionDefinition(elementType, tvpFunc);
                            tVPsClasses.Add(elementTypeName, tvpFunc);


                        }
                    }
                }
            }
        }

        private static void GenerateFunctionDefinition(
    ITypeSymbol elementType,
    StringBuilder tvpDelegatesString)
        {
            // Fully‑qualified and simple names of the POCO we’re generating for
            string fullTypeName = elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            string simpleTypeName = elementType.Name;

            // ────────────────────────────────────────────────────────────
            // Begin delegate source
            // ────────────────────────────────────────────────────────────
            tvpDelegatesString.AppendLine($@"
(System.Collections.IList listObj) =>                    
{{
    // 1. Cast IList to the actual generic list
    var typedList = (System.Collections.Generic.List<{fullTypeName}>)listObj;

    // 2. Create DataTable + columns
    var dt = new System.Data.DataTable(""{simpleTypeName}"");");

            // ────────────────────────────────────────────────────────────
            // Generate DataColumn declarations
            // ────────────────────────────────────────────────────────────
            foreach (var prop in elementType.GetMembers().OfType<IPropertySymbol>())
            {
                // Skip indexers, write‑only props, or collection types (except byte[])
                if (prop.IsIndexer || prop.SetMethod is null || IsCollectionType(prop.Type)) continue;

                string columnName;

                // Special‑case byte[] so typeof(byte[]) compiles
                string columnType =
                    prop.Type is IArrayTypeSymbol arr && arr.ElementType.SpecialType == SpecialType.System_Byte
                    ? "byte[]"
                    : prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                tvpDelegatesString.AppendLine(
                    $@"        dt.Columns.Add(""{prop.Name}"", typeof({columnType}));");
            }

            // ────────────────────────────────────────────────────────────
            // Generate row‑population loop
            // ────────────────────────────────────────────────────────────
            tvpDelegatesString.AppendLine($@"
    // 3. Populate rows
    foreach (var item in typedList)
    {{
        var row = dt.NewRow();");

            foreach (var prop in elementType.GetMembers().OfType<IPropertySymbol>())
            {
                if (prop.IsIndexer || prop.SetMethod is null || IsCollectionType(prop.Type)) continue;
                tvpDelegatesString.AppendLine(
                    $@"        row[""{prop.Name}""] = item.{prop.Name};");
            }

            tvpDelegatesString.AppendLine($@"
        dt.Rows.Add(row);
    }}

    // 4. Return the DataTable
    return dt;
}},");

            // ────────────────────────────────────────────────────────────
            // Helper: treat byte[] as scalar, everything else collection
            // ────────────────────────────────────────────────────────────
            static bool IsCollectionType(ITypeSymbol type) =>
                type switch
                {
                    // byte[] is a scalar for TVP purposes
                    IArrayTypeSymbol arr when arr.ElementType.SpecialType == SpecialType.System_Byte => false,

                    // Any other array counts as a collection
                    IArrayTypeSymbol => true,

                    // Any generic type that implements IEnumerable<T> counts as collection
                    INamedTypeSymbol nt when nt.IsGenericType &&
                                            nt.AllInterfaces.Any(i =>
                                                 i.OriginalDefinition.ToDisplayString() ==
                                                 "System.Collections.Generic.IEnumerable<T>") => true,

                    _ => false
                };
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
            builder.AppendLine($"    public class SpResponse");
            builder.AppendLine("    {");
            builder.AppendLine();
            builder.AppendLine($"       public const int SkipResponse = 0 ;");
            builder.AppendLine();

            int srNo = 1;
            foreach (string responseClass in uniqueResponseCasses)
            {
                builder.AppendLine($"       public const int {responseClass.Split('.').Last()} = {srNo} ;");
                builder.AppendLine();
                srNo++;
            }
            builder.AppendLine($"       public const int TotalCount = {srNo} ;");

            builder.AppendLine($@"    }}
}}");
        }
        private static void GenerateRequestClassesConfiguration(HashSet<string> uniqueRequestClasses, StringBuilder builder)
        {
            builder.AppendLine($"namespace SpExecuter.Utility");
            builder.AppendLine("{");
            builder.AppendLine($"   public class SpRequest");
            builder.AppendLine("    {");
            builder.AppendLine();
            builder.AppendLine($"       public const int NoRequest = 0 ;");
            builder.AppendLine();
            int srNo = 1;
            foreach (string requestClass in uniqueRequestClasses)
            {
            builder.AppendLine($"       public const int {requestClass.Split('.').Last()} = {srNo};");
                builder.AppendLine();
                srNo++;
            }
            builder.AppendLine($"       public const int TotalCount = {srNo} ;");

            builder.AppendLine($@"   }}
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
                    outSet.Add(namedSym.ToDisplayString());
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
                .Select(item => $"SpResponse.{item.Split('.').Last()}")
                .ToArray();
            string returnTypes = string.Join(", ", responseArray);
            returnTypes = !string.IsNullOrEmpty(returnTypes) ? ", returnObjects: [" + returnTypes+"]" : string.Empty;
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

 
        private static void GetInitialSyntax(StringBuilder builder,
            string namespaceName, string className, string interfaceName)
        {
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using SpExecuter.Utility;");
            builder.AppendLine($"namespace {namespaceName}{{");
            builder.AppendLine($"public class {className} : {interfaceName}");
            builder.AppendLine("{");

        }
        private static string GetClassToRegister(Dictionary<string, (string, Lifetime)> pairs,
            INamedTypeSymbol interfaceSymbol, AttributeData attr)
        {
            string interfaceName = interfaceSymbol.Name;
            string namespaceString= interfaceSymbol.ContainingNamespace.ToDisplayString();
            string className = string.Empty;
            if (interfaceName[0] == 'I' || interfaceName[0] == 'i')
            {
                className = namespaceString+"."+interfaceName.Substring(1);

            }
            else
            {
                className = namespaceString + "." + interfaceName + "Class";

            }
            TypedConstant lifeTime = attr.ConstructorArguments[0];
            var lifetime = lifeTime.Value is int intValue
                            ? (Lifetime)intValue
                            : Lifetime.Singleton;
            if (!pairs.ContainsKey(interfaceName))
            {
                pairs.Add(namespaceString + "." + interfaceName, (className, lifetime));
            }
            return className.Split('.').Last();
        }
    }
}
