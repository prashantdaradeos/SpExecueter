using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpExecuter.Utility

{
    public interface ISpExecuterRegistration
    {
        void RegisterForDependencyInjection(IServiceCollection services);
    }

    public static class StartUp
    {
       public static IServiceCollection ConfigureSpExecuter(this IServiceCollection services)
       {
            IEnumerable<Type> registrations = AppDomain.CurrentDomain.GetAssemblies()
                 .SelectMany(a => a.GetTypes())
                 .Where(types => typeof(ISpExecuterRegistration).IsAssignableFrom(types) && 
                 !types.IsInterface);

            // Register services from ALL projects that have SpExecuterRegistration
            foreach (Type registrationType in registrations)
            {
                ISpExecuterRegistration instance = 
                    (ISpExecuterRegistration)Activator.CreateInstance(registrationType)!;
                instance.RegisterForDependencyInjection(services);
            }

            return services;
        }
    }
}
