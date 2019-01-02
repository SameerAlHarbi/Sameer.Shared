using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sameer.Shared.Data
{
    public static class DataManagersExtensions
    {
        public static void AddSameerDbDataManagers<T>(this IServiceCollection services
            , IConfiguration configuration
            , ServiceLifetime lifetime = ServiceLifetime.Scoped) where T:DbContext
        {
            services.AddDbContext<T>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            , ServiceLifetime.Scoped);
            services.AddScoped<IRepository, Repository<T>>();

            services.AddScoped(typeof(IDataManager<>), typeof(GeneralManager<>));
            services.AddScoped(typeof(GeneralManager<>));

            var assembliesCollection = new List<Assembly>() { Assembly.GetCallingAssembly()};

            foreach (var item in Assembly.GetCallingAssembly().GetReferencedAssemblies())
            {
                assembliesCollection.Add(Assembly.Load(item));
            }

            var typesFromAssemblies = assembliesCollection.SelectMany(a => a.DefinedTypes.Where(x => isSubclassOf(x,typeof(GeneralManager<>))));

            foreach (var type in typesFromAssemblies)
                services.Add(new ServiceDescriptor(type, type, lifetime));
        }

        private static bool isSubclassOf(Type type, Type baseType)
        {
            if (type == null || baseType == null || type == baseType)
                return false;

            if (baseType.IsGenericType == false)
            {
                if (type.IsGenericType == false)
                    return type.IsSubclassOf(baseType);
            }
            else
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            type = type.BaseType;
            Type objectType = typeof(object);

            while (type != objectType && type != null)
            {
                Type curentType = type.IsGenericType ?
                    type.GetGenericTypeDefinition() : type;
                if (curentType == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }

    }
}
