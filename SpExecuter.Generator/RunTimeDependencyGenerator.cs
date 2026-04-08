using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpExecuter.Generator
{
    internal class RunTimeDependencyGenerator
    {
        internal static string GetClassToRegister(Dictionary<string, (string, Lifetime)> pairs,
           INamedTypeSymbol interfaceSymbol, AttributeData attr)
        {
            string interfaceName = interfaceSymbol.Name;
            string namespaceString = interfaceSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            string className = string.Empty;
            if (interfaceName[0] == 'I' || interfaceName[0] == 'i')
            {
                className = namespaceString + "." + interfaceName.Substring(1);

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
        internal static void GenerateRuntimeDependencies(Dictionary<string, (string, Lifetime)> registerClasses,
         StringBuilder buildServices, Information info)
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

            if (info!=null && !info.BuildFailed)
            {
                foreach (var classInfo in registerClasses)
                {
                    buildServices.AppendLine($@"             services.Add{registerClasses[classInfo.Key].Item2.ToString()}<{classInfo.Key},{classInfo.Value.Item1}>();");
                }
            }





            buildServices.AppendLine(@$" 
           
                        }}
                    }}
                }}");

        }

    }
}
