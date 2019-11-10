using Microsoft.Extensions.DependencyInjection;
using System;

namespace GroceryStoreAPI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract || !type.IsPublic)
                        continue;

                    if (!type.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var serviceType = assembly.GetType(type.Namespace + ".I" + type.Name);

                    if (serviceType == null)
                        services.AddSingleton(type);
                    else
                        services.AddSingleton(serviceType, type);
                }
            }
        }
    }
}
