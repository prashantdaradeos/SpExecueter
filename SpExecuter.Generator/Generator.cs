using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Xml.Linq;

/* info.Context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                    "SG001",
                     "",
                     "---------------------" +" ",
                    "",
                    DiagnosticSeverity.Warning,
                    true), info.MethodSymbol.Locations.FirstOrDefault()));*/
namespace SpExecuter.Generator
{
   
    [Generator]
    public class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var interfaceDeclarations = context.SyntaxProvider
                 .CreateSyntaxProvider(IsCandidateInterface, GetQualifiedInterfaces).
                 Where(i => i is not null).
                 Collect();
            context.RegisterSourceOutput(interfaceDeclarations, GenerateCode);

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
        private INamedTypeSymbol GetQualifiedInterfaces(GeneratorSyntaxContext context, CancellationToken token)
        {
    
            var interfaceSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
         
            string interfaceFullName = interfaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
          
            bool hasSpHandler = interfaceSymbol
                .GetAttributes()
                .Any(attr =>
                {
                    var attrClassName = attr.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    return attrClassName.Equals("global::SpExecuter.Utility.SpHandler",StringComparison.OrdinalIgnoreCase);
                });

            if (hasSpHandler)
            {
                return interfaceSymbol as INamedTypeSymbol;
            }
            else
            {
                return default!;
            }
        }
       
        private void GenerateCode(SourceProductionContext context, System.Collections.Immutable.ImmutableArray<INamedTypeSymbol> interfaceArray)
        {

            //For Generating Implementation classes
            Dictionary<string, StringBuilder> allClassSyntax = new Dictionary<string, StringBuilder>();

            //For generating extension method for registring interfaces and classes for DI
            Dictionary<string, (string, Lifetime)> registerClasses = new Dictionary<string, (string, Lifetime)>();
            StringBuilder buildServices = new StringBuilder();
            Information info = null;
            if (!interfaceArray.IsDefaultOrEmpty)
            {
                
                info= GenerateInterfaces(context, allClassSyntax,
                 interfaceArray, registerClasses);
            }

            HashSet<string> addedClasses = new HashSet<string>();
            if(info != null && !info.BuildFailed)
            {
                foreach (KeyValuePair<string, StringBuilder> generateClass in allClassSyntax)
                {
                    bool isClassAdded = addedClasses.Add(generateClass.Key);
                    if (isClassAdded)
                    {

                        string classCode = generateClass.Value.ToString();
                        var syntaxTree = SyntaxFactory.ParseCompilationUnit(classCode);
                        string formattedCode = syntaxTree.NormalizeWhitespace().ToFullString();
                        context.AddSource($"{generateClass.Key}.g.cs", SourceText.From(formattedCode, Encoding.UTF8));
                    }
                }

            }

            RunTimeDependencyGenerator.GenerateRuntimeDependencies(registerClasses, buildServices,info);
            var runtimeSyntaxTree = SyntaxFactory.ParseCompilationUnit(buildServices.ToString());
            string runTimeFormattedCode = runtimeSyntaxTree.NormalizeWhitespace().ToFullString();
            context.AddSource($"StartupExtension.g.cs", SourceText.From(runTimeFormattedCode, Encoding.UTF8));

        }
      
        private static Information GenerateInterfaces(
            SourceProductionContext context, 
            Dictionary<string, StringBuilder> allClassSyntax,
            ImmutableArray<INamedTypeSymbol> interfaces,
            Dictionary<string, (string, Lifetime)> registerClasses)
        {
            BaseModel baseModel = new BaseModel();
            baseModel.Context = context;
                Information info = null;
            foreach (INamedTypeSymbol interfaceSymbol in interfaces)
            {
                baseModel.InterfaceSymbol = interfaceSymbol;
                 var attr = baseModel.InterfaceSymbol
                    .GetAttributes()
                    .FirstOrDefault(attribute =>
                    {
                        var attrClass = attribute.AttributeClass;
                        if (attrClass == null)
                            return false;

                        var fullName = attrClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        return   fullName.Equals("global::SpExecuter.Utility.SpHandler",StringComparison.OrdinalIgnoreCase);
                    });

                baseModel.ExcludeIndicesClassLevel = false;
                var excludeIndicesIfArg = attr.NamedArguments
                                       .FirstOrDefault(kv => kv.Key == nameof(SpHandler.ExcludeIndices));
                baseModel.ExcludeIndicesClassLevel = Convert.ToBoolean(excludeIndicesIfArg.Value.Value);

                // Get interface name and class name for DI
                string className = RunTimeDependencyGenerator.GetClassToRegister(registerClasses, 
                    interfaceSymbol, attr);
                string namespaceName = baseModel.InterfaceSymbol.ContainingNamespace?.ToDisplayString()!;
                
                 info = new Information(baseModel)
                {
                    ClassName = className,
                    NamespaceName = namespaceName,

                };
                
                GenerateClasses(info, allClassSyntax);
               


            }
            return info;
        }
        private static void GenerateClasses(
            Information info,
            Dictionary<string, StringBuilder> allClassSyntax)
        {
            Dictionary<string, List<string>> AtomicFlags = new Dictionary<string, List<string>>();
            if (!allClassSyntax.ContainsKey(info.ClassName))
            {
                //Add classname and method name for background service flag change
                AtomicFlags.Add(info.ClassName, new List<string>());
                //Get class declaration syntax
                StringBuilder classSyntax = new StringBuilder();
                GetInitialSyntax(classSyntax, info);
                info.MethodSequenceNo = 1;
                //Loop and Add Methods in class declared in interface
                foreach (IMethodSymbol member in info.InterfaceSymbol.GetMembers().OfType<IMethodSymbol>())
                {
                    info.MethodSymbol = member;
                    AtomicFlags[info.ClassName].Add($"{info.MethodSymbol.Name}FetchFlag");
                    //Validate method signature
                    Validations.MethodRetunTypeValidation(info);
                    //Get Method Metadata for generating Declaration
                    info.MethodReturnTypeAsString = member.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    info.MethodName = member.Name;
                    string parameters = string.Join(", ",
                        member.Parameters.Select(p => $"{p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} {p.Name}"));

                    info.ExcludeIndicesMethodLevel = false;
                    info.UseShort = false;
                    info.IsNonQuery = false;
                    info.AttributeAdded = false;
                    info.ConditionType = ConditionType.AND;
                    info.SameElementsInNestedTuple = 0;
                    info.SameElementsListAgnostic = 0;
                    info.ReturnType = member.ReturnType as INamedTypeSymbol;
                    info.OutParams = new StringBuilder();
                    //Unwrap Task<>
                    info.ReturnType = info.ReturnType.TypeArguments[0] as INamedTypeSymbol;

                    foreach (AttributeData methodAttribute in member.GetAttributes())
                    {
                        INamedTypeSymbol methodAttributeSymbol = methodAttribute.AttributeClass!;
                        string attributeFullName = methodAttributeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        if (attributeFullName.Equals("global::SpExecuter.Utility.StoredProcedure", StringComparison.OrdinalIgnoreCase))
                        {
                            info.AttributeAdded = true;
                            TypedConstant spNameArg = methodAttribute.ConstructorArguments[0];
                            info.StoredProcedureName = (string)spNameArg.Value ?? "";

                            var isUseShortArg = methodAttribute.NamedArguments
                                        .FirstOrDefault(kv => kv.Key == nameof(StoredProcedure.UseShort));
                            info.UseShort = Convert.ToBoolean(isUseShortArg.Value.Value);
                            var excludeIndicesArg = methodAttribute.NamedArguments
                                        .FirstOrDefault(kv => kv.Key == nameof(StoredProcedure.ExcludeIndices));
                            info.ExcludeIndicesMethodLevel = Convert.ToBoolean(excludeIndicesArg.Value.Value);
                            var isNonQueryArg = methodAttribute.NamedArguments
                                       .FirstOrDefault(kv => kv.Key == nameof(StoredProcedure.IsNonQuery));
                            info.IsNonQuery = Convert.ToBoolean(isNonQueryArg.Value.Value);

                            var conditionTypeArg = methodAttribute.NamedArguments
                                       .FirstOrDefault(kv => kv.Key == nameof(StoredProcedure.ConditionType));
                            info.ConditionType = (ConditionType)Convert.ToInt32(conditionTypeArg.Value.Value);
                        }
                    }

                    Validations.StoredProcedureAttributeValidation(info);
                    info.ExcludeIndicesClassLevel=info.ExcludeIndicesMethodLevel=info.ExcludeIndices = true;
                    
                    INamedTypeSymbol returnType= info.MethodSymbol.ReturnType as INamedTypeSymbol;
                    returnType = returnType.TypeArguments[0] as INamedTypeSymbol;

                   
                    if (returnType.IsTupleType &&
                        info.ConditionType == ConditionType.OR &&
                        !Common.IsNestedTuple(returnType))
                    {
                        Validations.ValidateUniqueConstraint(info, returnType);
                        Validations.DuplicateInSingleTupleValidation(info, returnType);

                    }
                    if(returnType.IsTupleType &&
                        Common.IsNestedTuple(returnType))
                    {
                       
                        Validations.DuplicateTupleInNestedTupleValidation(info, returnType);
                        Validations.ValidateNestedTuple(info, returnType);
                    }
                   
                    InputParameterGenerator.GenerateOrdinals(info, classSyntax);
                    /*if (info.ExcludeIndicesClassLevel || info.ExcludeIndicesMethodLevel)
                    {
                        info.ExcludeIndices = false;
                    }*/
                    info.ExcludeIndicesClassLevel = info.ExcludeIndicesMethodLevel = info.ExcludeIndices = true;

                    //classSyntax.AppendLine($"    internal readonly object _{info.MethodName}FetchLock = new object();");

                    // Generate Method Implementation
                    classSyntax.AppendLine($"    public async {info.MethodReturnTypeAsString} {info.MethodName}({parameters})");
                    classSyntax.AppendLine("    { ");
                    classSyntax.AppendLine($"       List<SqlParameter> param = new List<SqlParameter>();");
                    classSyntax.AppendLine("       try{");
                    classSyntax.AppendLine(GenerateMethodImplementation(info));
                    classSyntax.AppendLine("    }");
                    classSyntax.AppendLine();
                    info.MethodSequenceNo++;
                }
                classSyntax.Append(GenerateUniqueCheckMethodImplementation());
                //Class implementation complete
                classSyntax.AppendLine("}}");

                allClassSyntax.Add(info.ClassName, classSyntax);
            }

        }
         private static StringBuilder GenerateUniqueCheckMethodImplementation()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("        private bool ColumnExists(IDataReader reader,string columnName)");
            builder.AppendLine("        {");
            builder.AppendLine("            try");
            builder.AppendLine("              {");
            builder.AppendLine("                reader.GetOrdinal(columnName);");
            builder.AppendLine("                return true;");
            builder.AppendLine("              }");
            builder.AppendLine("            catch");
            builder.AppendLine("                {   return false; }");
            builder.AppendLine("        }");
            return builder;
        }
        private static void GetInitialSyntax(StringBuilder builder,
             Information info)
        {
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using System.Data;");
            builder.AppendLine("using SpExecuter.Utility;");
            builder.AppendLine("using Microsoft.Data.SqlClient;");
            builder.AppendLine($"namespace {info.NamespaceName}{{");
            builder.AppendLine($"public class {info.ClassName} : {info.InterfaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}");
            builder.AppendLine("{");

        }
        
        
       
        private static string GenerateMethodImplementation(Information info)
        {
            
            if (!Validations.ConnectionStringValidation(info) ||
                !Validations.StoredProcedureValidation(info))
            {
                return string.Empty;
            }
          
            StringBuilder method = new StringBuilder();
            RequestProp req = new RequestProp();
            
            method.Append(InputParameterGenerator.GenerateSQLParameters(info, req));

            method.Append(ExecuterImplementation( info));
            
            method.Append(OutParameterGenerator.ResultBindingImplementation(info));

            return method.ToString();
            }
       
        private static StringBuilder ExecuterImplementation(Information info)
        {
            StringBuilder executerImplementation = new StringBuilder();
            executerImplementation.AppendLine();
            executerImplementation.AppendLine($"         await using var connection = new SqlConnection({info.MethodSymbol.Parameters[0].Name});");
            executerImplementation.AppendLine($"         await using var command = new SqlCommand(\"{info.StoredProcedureName}\", connection)");
            executerImplementation.AppendLine($"         {{       CommandType = CommandType.StoredProcedure      }};");
            executerImplementation.AppendLine($"         command.Parameters.AddRange(param.ToArray());");
            executerImplementation.AppendLine($"         await connection.OpenAsync();");

            return executerImplementation;
        }
       
    }

}