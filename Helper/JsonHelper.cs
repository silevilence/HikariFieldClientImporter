using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariFieldClientImporter.Helper
{
    internal static class JsonHelper
    {
        public static string SerializeObject(object value)
        {
            return Playnite.SDK.Data.Serialization.ToJson(value, true);
        }

        public static T DeserializeObject<T>(string value)
            where T : class
        {
            return Playnite.SDK.Data.Serialization.FromJson<T>(value);
        }
    }
}
