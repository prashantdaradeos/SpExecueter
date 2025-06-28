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
            //Response
            AppConstants.SpResponsePropertyInfoCache = new PropertyInfo[DBConstants.SpResponseClassesCount][];
            for (int i = 0; i < DBConstants.SpResponseModelTypeArray.Length; i++)
            {
                AppConstants.SpResponsePropertyInfoCache[i] = DBConstants.SpResponseModelTypeArray[i].GetProperties();
            }
            //Request
            int numberOfRequestModels = DBConstants.SpRequestClassesCount;
            AppConstants.SpRequestPropertyInfoCache = new PropertyInfo[numberOfRequestModels][];
            AppConstants.CachedPropertyAccessorDelegates = new Dictionary<string, Func<object, object>>[numberOfRequestModels];
            for (int i = 0; i < DBConstants.SpRequestModelTypeArray.Length; i++)
            {
                AppConstants.SpRequestPropertyInfoCache[i] = DBConstants.SpRequestModelTypeArray[i].GetProperties();
                AppConstants.CachedPropertyAccessorDelegates[i] = new Dictionary<string, Func<object, object>>();
                foreach (var property in AppConstants.SpRequestPropertyInfoCache[i])
                {
                    AppConstants.CachedPropertyAccessorDelegates[i].Add(property.Name, AppConstants.CreateGetter
                        (DBConstants.SpRequestModelTypeArray[i], property.Name, property));
                }
            }



            return services;
        }
    }
}
