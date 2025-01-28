using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HikariFieldClientImporter.Helper
{
    internal static class GlobalVars
    {
        public static string HFClientInstallPath { get; set; } = "";

        public static string IconPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"icon.png");
    }
}
