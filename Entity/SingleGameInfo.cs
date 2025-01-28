using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariFieldClientImporter.Entity
{
    internal class SingleGameInfo
    {
        /// <summary>
        /// 不知道是啥
        /// </summary>
        [JsonProperty("build_id")]
        public int BuildId { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// 分支？
        /// </summary>
        [JsonProperty("depot")]
        public string Depot { get; set; }

        /// <summary>
        /// 安装路径
        /// </summary>
        [JsonProperty("installed_path")]
        public string InstalledPath { get; set; }

        /// <summary>
        /// 执行文件
        /// </summary>
        [JsonProperty("exec_file")]
        public string ExecFile { get; set; }
    }
}
