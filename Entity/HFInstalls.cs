using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariFieldClientImporter.Entity
{
    internal class HFInstalls
    {
        /// <summary>
        /// game id str -> game info
        /// </summary>
        [SerializationPropertyName("installs")]
        public Dictionary<string, SingleGameInfo> Installs { get; set; }
    }
}
