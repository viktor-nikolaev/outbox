using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace ZionApps.Outbox.Helpers
{
    internal static class IdentityPropertyExtractor
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo?> Cache = new();

        /// <summary>
        /// Tries to extract the value of Id or {TypeName}Id property.
        /// Similar thing to what EF does 
        /// </summary>
        /// <param name="src"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object? GetIdentityValue<T>(this T src) where T : notnull
        {
            var property = Cache.GetOrAdd(typeof(T), FindIdentityProperty);
            return property?.GetValue(src, null);
        }

        private static PropertyInfo? FindIdentityProperty(Type type)
        {
            const string idName = "id";
            var fullIdName = string.Concat(type.Name, idName);
            
            PropertyInfo? fullIdProp = null;

            foreach (var property in type.GetProperties())
            {
                if (property.Name.Equals(idName, StringComparison.OrdinalIgnoreCase))
                {
                    // idName takes priority
                    return property;
                }

                if (property.Name.Equals(fullIdName, StringComparison.OrdinalIgnoreCase))
                {
                    fullIdProp = property;
                }
            }

            return fullIdProp;
        }
    }
}