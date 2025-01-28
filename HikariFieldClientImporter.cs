using HikariFieldClientImporter.Entity;
using HikariFieldClientImporter.Helper;
using HtmlAgilityPack;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HikariFieldClientImporter
{
    [LoadPlugin]
    public class HikariFieldClientImporter : LibraryPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private HikariFieldClientImporterSettingsViewModel settings { get; set; }

        private const string HIKARI_FIELD_CLIENT_FOLDER = "hikari-field-client";

        private const string HIKARI_FIELD_INSTALL_FILE = "installs.json";

        private const string HIKARI_FIELD_SHOP_URL = "https://store.hikarifield.co.jp/shop/";

        public override Guid Id { get; } = Guid.Parse("a8050aa6-0fb3-4b28-8bac-491d644e81c7");

        // Change to something more appropriate
        public override string Name => "Hikari Field";

        // Implementing Client adds ability to open it via special menu in playnite.
        public override LibraryClient Client { get; } = new HikariFieldClientImporterClient();

        public HikariFieldClientImporter(IPlayniteAPI api) : base(api)
        {
            settings = new HikariFieldClientImporterSettingsViewModel(this);
            Properties = new LibraryPluginProperties
            {
                HasSettings = false
            };
        }

        public override IEnumerable<GameMetadata> GetGames(LibraryGetGamesArgs args)
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var installFile = Path.Combine(appdata, HIKARI_FIELD_CLIENT_FOLDER, HIKARI_FIELD_INSTALL_FILE);
            var result = new List<GameMetadata>();
            if (!File.Exists(installFile))
            {
                logger.Warn($"Hikari Field install file {installFile} not found.");
                return result;
            }

            var jsonString = File.ReadAllText(installFile);
            var installs = JsonHelper.DeserializeObject<HFInstalls>(jsonString);
            var allGames = PlayniteApi.Database.Games.Where(g => g.PluginId == Id).ToList();
            var installedGames = new HashSet<string>();

            foreach (var install in installs.Installs)
            {
                var gameId = install.Key;
                var gameInfo = install.Value;
                if (gameInfo is null
                    || string.IsNullOrWhiteSpace(gameInfo.ExecFile)
                    // 非exe的暂不处理
                    || !gameInfo.ExecFile.EndsWith(".exe"))
                {
                    continue;
                }

                var gameShopUri = gameId;
                var isTrial = false;
                if (gameShopUri.EndsWith("_trial"))
                {
                    gameShopUri = gameShopUri.Substring(0, gameShopUri.Length - 6);
                    isTrial = true;
                }
                else if (gameShopUri.EndsWith("trial"))
                {
                    gameShopUri = gameShopUri.Substring(0, gameShopUri.Length - 5);
                    isTrial = true;
                }
                var url = $"{HIKARI_FIELD_SHOP_URL}{gameShopUri}";
                var web = new HtmlWeb();
                var doc = web.Load(url);

                // 游戏简介
                // <section class="introduction mt-4 mb-3">
                var node = doc.DocumentNode.SelectSingleNode("//section[contains(@class, 'introduction')]");
                var htmlStr = "";
                if (node != null)
                {
                    htmlStr = node.OuterHtml;
                }

                // 标题
                var titleNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'shop-top')]/div/h1");
                var title = "";
                if (titleNode != null)
                {
                    var innerText = titleNode.InnerText;
                    var lines = innerText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    title = lines.First().Trim();
                }
                if (isTrial)
                {
                    title += "(体验版)";
                }

                var imgNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'banner')]/img[contains(@class, 'img')]");
                var imgSrc = "";
                if (imgNode != null)
                {
                    imgSrc = imgNode.GetAttributeValue("src", "");
                }

                var game = new GameMetadata
                {
                    GameId = gameId,
                    Name = title,
                    Links = new List<Link>
                    {
                        new Link("Store", url)
                    },
                    Description = htmlStr,
                    Icon = new MetadataFile(Path.Combine(gameInfo.InstalledPath, gameInfo.ExecFile)),
                    BackgroundImage = new MetadataFile(imgSrc),
                    InstallDirectory = gameInfo.InstalledPath,
                    GameActions = new List<GameAction>
                    {
                        new GameAction
                        {
                            Type = GameActionType.File,
                            Path = Path.Combine(gameInfo.InstalledPath, gameInfo.ExecFile),
                            IsPlayAction = true,
                            WorkingDir = gameInfo.InstalledPath
                        }
                    },
                    IsInstalled = true,
                };

                // 检查游戏是否已存在
                var existingGame = allGames.Find(g => g.GameId == game.GameId);
                if (existingGame != null)
                {
                    // 更新已有游戏的元数据
                    existingGame.Name = game.Name;
                    existingGame.Description = game.Description;
                    existingGame.InstallDirectory = game.InstallDirectory;
                    existingGame.IsInstalled = game.IsInstalled;
                    // 更新其他元数据
                    existingGame.Links.Clear();
                    foreach (var link in game.Links)
                    {
                        existingGame.Links.Add(link);
                    }
                    existingGame.GameActions.Clear();
                    foreach (var action in game.GameActions)
                    {
                        existingGame.GameActions.Add(action);
                    }

                    // 保存更改
                    PlayniteApi.Database.Games.Update(existingGame);
                    installedGames.Add(existingGame.GameId);
                }
                else
                {
                    // 添加新游戏
                    result.Add(game);
                }

            }

            // 删除已卸载的游戏
            foreach (var game in allGames)
            {
                if (!installedGames.Contains(game.GameId))
                {
                    game.IsInstalled = false;
                    PlayniteApi.Database.Games.Update(game);
                }
            }

            return result;
            // Return list of user's games.
            //return new List<GameMetadata>()
            //{
            //    new GameMetadata()
            //    {
            //        Name = "Notepad",
            //        GameId = "notepad",
            //        GameActions = new List<GameAction>
            //        {
            //            new GameAction()
            //            {
            //                Type = GameActionType.File,
            //                Path = "notepad.exe",
            //                IsPlayAction = true
            //            }
            //        },
            //        IsInstalled = true,
            //        Icon = new MetadataFile(@"c:\Windows\notepad.exe")
            //    },
            //    new GameMetadata()
            //    {
            //        Name = "Calculator",
            //        GameId = "calc",
            //        GameActions = new List<GameAction>
            //        {
            //            new GameAction()
            //            {
            //                Type = GameActionType.File,
            //                Path = "calc.exe",
            //                IsPlayAction = true
            //            }
            //        },
            //        IsInstalled = true,
            //        Icon = new MetadataFile(@"https://playnite.link/applogo.png"),
            //        BackgroundImage = new MetadataFile(@"https://playnite.link/applogo.png")
            //    }
            //};
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new HikariFieldClientImporterSettingsView(settings);
        }
    }
}