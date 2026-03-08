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
                 .Where(types => typeof(ISpExecuterRegistration).IsAssignableFrom(types) && !types.IsInterface);
            if (registrations.Any())
            {
                ISpExecuterRegistration instance = (ISpExecuterRegistration)Activator.CreateInstance(registrations.First())!;
                instance.RegisterForDependencyInjection(services);
            }
            
            
            return services;
        }
    }
}
