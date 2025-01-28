using HikariFieldClientImporter.Helper;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariFieldClientImporter
{
    public class HikariFieldClientImporterClient : LibraryClient
    {

        public override bool IsInstalled => true;

        public override string Icon => GlobalVars.IconPath;

        public override void Open()
        {
            if (string.IsNullOrWhiteSpace(GlobalVars.HFClientInstallPath))
            {
                throw new Exception("未设定HikariField客户端安装路径");
            }

            var path = GlobalVars.HFClientInstallPath;
            // 根据设定的是目录或文件分别处理
            if (Directory.Exists(path))
            {
                // 设定目录时，打开目录下的HIKARI FIELD CLIENT.exe
                var exePath = Path.Combine(path, "HIKARI FIELD CLIENT.exe");
                if (File.Exists(exePath))
                {
                    System.Diagnostics.Process.Start(exePath);
                }
                else
                {
                    throw new Exception("设定的目录下未找到 HikariField 客户端");
                }
            }
            else if (File.Exists(path))
            {
                // 如果是exe文件，直接打开
                if (Path.GetExtension(path).ToLower() == ".exe")
                {
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    throw new Exception("设定的文件不是一个可执行文件");
                }
            }
            else
            {
                throw new Exception("设定的路径不存在");
            }
        }
    }
}