using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZionApps.Outbox.PostgreSql.Queries
{
    internal static class SqlSource
    {
        public static readonly string GetNonSequentialEventsToDispatch =
            GetStringOf(nameof(GetNonSequentialEventsToDispatch));

        public static readonly string GetSequentialEventsToDispatch =
            GetStringOf(nameof(GetSequentialEventsToDispatch));

        public static readonly string InsertEvent = GetStringOf(nameof(InsertEvent));
        public static readonly string UpdateEventStatus = GetStringOf(nameof(UpdateEventStatus));
        public static readonly string CreateEventsTable = GetStringOf(nameof(CreateEventsTable));

        private static string GetStringOf(string path)
        {
            var assembly = typeof(SqlSource).Assembly;
            var resourceStream = assembly.GetManifestResourceStream($"ZionApps.Outbox.PostgreSql.Queries.{path}.sql");
            if (resourceStream is null)
            {
                throw new KeyNotFoundException(path);
            }

            using var reader = new StreamReader(resourceStream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}