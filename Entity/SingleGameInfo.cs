using Playnite.SDK.Data;
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
        [SerializationPropertyName("build_id")]
        public int BuildId { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [SerializationPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// 分支？
        /// </summary>
        [SerializationPropertyName("depot")]
        public string Depot { get; set; }

        /// <summary>
        /// 安装路径
        /// </summary>
        [SerializationPropertyName("installed_path")]
        public string InstalledPath { get; set; }

        /// <summary>
        /// 执行文件
        /// </summary>
        [SerializationPropertyName("exec_file")]
        public string ExecFile { get; set; }
    }
}
