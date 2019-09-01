using AddonScraper.FileIO;
using AddonToolkit.Model;
using AddonToolkit.Parse;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AddonScraper
{
    public class Program
    {
        //private static readonly NLog.ILogger logger = LogManager.GetLogger("AddonScraper");
        private static Microsoft.Extensions.Logging.ILogger logger;// = LogManager.GetLogger("AddonScraper");

        public static readonly Dictionary<string, string> PROJECT_URLS = new Dictionary<string, string>()
        {
            {"bigwigs", "big-wigs"},
            {"weakauras", "weakauras-2"},
            {"aap-core","azeroth-auto-pilot"},
            {"dbm-core","deadly-boss-mods"},
            {"omnicc", "omni-cc"},
            {"omen","omen-threat-meter"},
            {"littlewigs","little-wigs"},
            {"elvui_sle", "elvui-shadow-light"},
            {"atlasloot", "atlasloot-enhanced"},
            {"healbot", "heal-bot-continued"},
            {"tradeskillmaster", "tradeskill-master"},
            {"auc-advanced", "auctioneer"},
            {"titan", "titan-panel"},
            {"tidyplates_threatplates", "tidy-plates-threat-plates"},
            {"maxdps", "maxdps-rotation-helper"},
            {"easydelete", "easy-delete"},
            {"raidframeicons", "enhanced-raid-frame-icons"},
            {"allthethings", "all-the-things"},
            {"dbm-dragonsoul", "deadly-boss-mods-cataclysm-mods"},
            {"dbm-icecrown", "deadly-boss-mods-wotlk"},
            {"dbm-pandaria", "deadly-boss-mods-mop"},
            {"dbm-outlands", "dbm-bc" },
            {"kui_nameplates", "kuinameplates" },
            {"dbm-draenor", "deadly-boss-mods-wod" },
            {"dbm-brokenisles", "deadly-boss-mods-dbm-legion" },
            {"shadowedunitframes", "shadowed-unit-frames" },
            {"capping", "capping-bg-timers" },
            {"prat-3.0", "prat-3-0" },
            {"dbm-party-bc", "deadly-boss-mods-dbm-dungeons" },
            {"nugcombobar", "nugie-combo-bar" },
            {"gse", "gse-gnome-sequencer-enhanced-advanced-macros" },
            {"quafe", "quafe-ui" },
            {"stuf", "stuf-unit-frames" },
            {"simpleinterruptannounce", "sia" },
            {"svui_!core", "supervillain-ui" },
            {"dhud", "dhud-3_0" },
            {"sctd", "sct-damage" },
            {"details_raidinfo-throneofthunder", "details-legacy-raids-info" },
            {"phoenixstylemod_panda_tier1", "phoenixstyle-pandaria-mod" },
            {"hunterpets", "hunterpets-bugfix-fork" },
            {"tidyplates_slim_horizontal", "tidy-plates-slim" },
            {"totalrp3_extended","total-rp-3-extended"},
            {"inflight","inflight-taxi-timer"},
            {"raidachievement_pandaraids","raidachievement_pandaria"},
            {"powerauras","power-auras-classic-v4"},
            {"raidachievement_wodraids","raidachievement_wod"},
            {"lorti ui","lorti_ui"},
            {"adibags_byexpansion","adibags_by_expansion"},
            {"handynotes_dmf_basic","handynotes_dmf-darkmoon-faire"},
            {"questguru","questguru-2-1"},
            {"auditor3","auditor"},
            {"jojomonk","jojos-bizarre-adventure-sfx"},
            {"elvui_chattweaks","elvui-ctc"},
            {"phoenixstyle_loader_cata","phoenixstyle-cataclysm-mod"},
            {"goh","get-over-here-dk"},
            {"razergamingkeypad","razer-orbweaver-add"},
            {"executive_assistant","exec_assist"},
            {"simpletauntannounce","sta"},
            {"lunaeclipse_rotation","lunaeclipse-rotation-helper"},
            {"critsound","crit-sound-isler"},
            {"vilinkasinsanity","vilins"},
            {"derpystuffing","derpy-stuffing-bags"},
            {"broker_statsnow_spells","broker_statsnow"},
            {"controlpanel","controlpanel2"},
            {"tankui","brewmaster"},
            {"clcinfo","clctracker"},
            {"perl_config","perl-classic-unit-frames"},
            {"sct","scrolling-combat-text"},
            {"ct_core","ctmod"},
            {"qdkp_v2","quick-dkp-v2"},
            {"dxe","deus-vox-encounters"},
            {"lookingforgroup_av","lookingforgroup-alterac-valley"},
            {"mapnotes","map-notes-fans-update"},
            {"npa2","nameplateadvance2"},
            {"arenalive3","arenalive-unitframes"},
            {"blink","blinks-health-text-hud"},
            {"kong","kingkongframefader"},
            {"acb_auras","azcastbar-plugins"},
            {"pandariasunnart","sunn-viewport-art-pack-pandaria"},
            {"grimtools","grims-tools"},
            {"biamprepared","i-am-prepared"},
            {"bettericonselector","bettericonselect"},
            {"arenalivespectator3","vadraks-arenalive"},
            {"raidwatch","rw2"},
            {"guildmemberinfo","guild-trade-skills"},
            {"win","photoshop-blp-format-plugin"},
            {"broker_raidfinder","broker-raid-finder"},
            {"lunarsphere","lunarsphere-fan-update"},
            {"autotarget","project-1358"},
            {"zerkinui modular string elements","zerkinui-competitive-configs-strings"},
            {"broker_stats","broker-statistics"},
            {"shotglass","shotglass-raid-frames"},
            {"cashcontrol","auditor-fan-update"},
            {"pqc alchemy", "pqc-profession-quests-complete"},
            {"ouf_brethrenrf","ouf_brethrenraidframes"},
            {"corruptedashbringer","dark-whispers"},
            {"metahud","metahud-revived"},
            {"foxsmoothui-v3.1","foxsmoothui"},
            {"semi_core","semi-frames"},
            {"levelbuddy","wowlevelbuddy"},
            {"[mec's]-ancient_mana","mecs-broker-plugin-pack"},
            {"raidwatch_ulduar","raidwatch_wotlk"},
            {"chatmondialserveur","project-1713"},
            {"dbcs_data_0000","dbcs_data"},
            {"mysellall","mysellall"},
            {"xpmultibar","xp-multibar"},
        };

        public static HashSet<string> IgnoredProjectNames = new HashSet<string>()
        {
            "musician",
            "pvpsound",
            "azeroth-adventure-album",
            "brewmastertools",
            "beancounter-export",
            "itemanalysis",
            "wowquote",
            "new-warcraft-custom-sounds",
            "improved-chaos-bolt-channel-demonfire-sound",
            "crystal-login",
            "syianaui",
            "lowered-combat-sounds",
            "baddysounds",
            "cokedrivers-ui",
            "guildbanktimer",
            "puffin",
            "cp42-0",
            "malsumis-wow-font-changer",
            "tanari-tongues-addon",
            "damage-watch",
            "quickchecklinksplayer",
            "bwonsamdimute",
            "heavier-twohanded-sword-sounds",
            "warcraft-custom-music",
            "inobaigold",
            "rhoninmute",
            "oom",
        };

        public static async Task Main(string[] args)
        {
            int startPage = int.Parse(args[0]);
            int endPage = int.Parse(args[1]);

            Directory.CreateDirectory(@".\temp");
            var outDir = Directory.CreateDirectory(@".\out");

            List<AddonData> loadedData = Storage.LoadAddonData();

            string date = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            var directory = outDir.CreateSubdirectory(date);
            LoggingConfiguration config = new LoggingConfiguration();
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget("target1")
            {
                Layout = @"${date:format=HH\:mm\:ss} ${level} ${message} ${exception}"
            };
            config.AddTarget(consoleTarget);

            FileTarget fileTarget = new FileTarget("target2")
            {
                FileName = directory.FullName + "/log.txt",
                Layout = "${longdate} ${level} ${message}  ${exception}"
            };
            config.AddTarget(fileTarget);
            config.AddRuleForAllLevels(fileTarget); // only errors to file
            config.AddRuleForAllLevels(consoleTarget); // all to console
            LogManager.Configuration = config;
            //logger = LogManager.GetLogger("AddonScraper");

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new NLogLoggerProvider()); //TODO: use a nlog provider for Microsoft.Extenstions.Logger, and add the provider here.

            logger = loggerFactory.CreateLogger("AddonScraper");
            logger.LogInformation("Start");
            logger.LogInformation("Loaded " + loadedData.Count);
            //log2.LogInformation("LOG2 start");
            //HashSet<string> query = loadedData.GroupBy(x => x.FolderName)
            //                        .Where(g => g.Count() > 1)
            //                        .Select(g => g.Key)
            //                        .ToHashSet();

            //List<AddonData> allDuplicates = loadedData.Where(ad => query.Contains(ad.FolderName)).ToList();
            //allDuplicates.Sort((x, y) => x.FolderName.CompareTo(y.FolderName));
            //Storage.SaveToFile(allDuplicates, "duplicates", Formatting.Indented, 1, 340);
            //logger.Info("done");
            //await Task.Delay(20000);
            using (HttpClient httpClient = new HttpClient())
            {
                //int start = 1;
                //int tries = 5;
                //int files = 68;
                //int span = 5;

                int start = startPage;
                int tries = 5;
                int files = endPage - startPage + 1;
                int span = 1;

                List<Task> tasks = new List<Task>();
                List<CurseAddon> allCurse = new List<CurseAddon>();
                List<AddonData> allValid = new List<AddonData>();
                List<AddonData> allFailed = new List<AddonData>();

                for (int i = 0; i < files; i++)
                {
                    int from = start + (i * span);
                    int to = from + span - 1;

                    List<CurseAddon> scrapedAddons = await Scrape(httpClient, tries, from, to);
                    List<CurseAddon> addons = scrapedAddons.Where(curseAddon => !IgnoredProjectNames.Contains(curseAddon.AddonURL)).ToList();
                    Storage.SaveToFile(directory, addons, from, to);
                    allCurse.AddRange(addons);

                    (List<AddonData> valid, List<AddonData> failed) = await FullProccess(httpClient, addons, tries, loadedData);
                    Storage.SaveToFile(directory, valid, "valid", Formatting.None, from, to);
                    Storage.SaveToFile(directory, failed, "failed", Formatting.Indented, from, to);
                    allValid.AddRange(valid);
                    allFailed.AddRange(failed);
                }

                int end = start + (files * span) - 1;

                Storage.SaveToFile(directory, allCurse, start, end);
                Storage.SaveToFile(directory, allValid, "allvalid", Formatting.None, start, end);
                Storage.SaveToFile(directory, allFailed, "allfailed", Formatting.Indented, start, end);

                HashSet<string> query = allValid.GroupBy(x => x.FolderName)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key)
                                    .ToHashSet();
                List<AddonData> allDuplicates = allValid.Where(ad => query.Contains(ad.FolderName)).ToList();
                allDuplicates.Sort((x, y) => x.FolderName.CompareTo(y.FolderName));
                Storage.SaveToFile(directory, allDuplicates, "duplicates", Formatting.Indented, start, end);
            }

            logger.LogInformation("End");
        }

        public static async Task<(List<AddonData>, List<AddonData>)> FullProccess(HttpClient httpClient, List<CurseAddon> addons, int tries, List<AddonData> loadedData)
        {
            ConcurrentBag<AddonData> validAddonData = new ConcurrentBag<AddonData>();
            ConcurrentBag<AddonData> NonValidAddonData = new ConcurrentBag<AddonData>();

            List<Task<AddonData>> addonDataTasks = addons.Select(curseAddon => FromCurseToAddonData(httpClient, curseAddon, tries, loadedData)).ToList();
            AddonData[] addonDatas = await Task.WhenAll(addonDataTasks);

            List<Task> proccessTasks = addonDatas.Select(ad => ProccessAddonData(httpClient, ad).ContinueWith((t) =>
            {
                bool result = t.Result.Item1;
                AddonData addonData = t.Result.Item2;

                addonData.HasRetail = addonData.Downloads.Any(d => d.GameVersion.First().ToString() == "8");
                addonData.HasClassic = addonData.Downloads.Any(d => d.GameVersion.First().ToString() == "1");

                if (result)
                {
                    validAddonData.Add(addonData);
                    logger.LogInformation("Match found for: " + addonData.ProjectName + " = " + addonData.FolderName);
                }
                else
                {
                    NonValidAddonData.Add(addonData);
                    logger.LogInformation("No Match found for: " + addonData.ProjectName + " = " + addonData.FolderName);
                }
                addonData.Downloads = new List<Download>();
            })).ToList();
            await Task.WhenAll(proccessTasks);

            return (validAddonData.ToList(), NonValidAddonData.ToList());
        }

        public static async Task<(bool, AddonData)> ProccessAddonData(HttpClient httpClient, AddonData addonData)
        {
            if (!string.IsNullOrEmpty(addonData.FolderName))
            {
                return (true, addonData);
            }

            bool found = false;
            string zipFile = string.Empty;
            try
            {
                zipFile = await Update.DLWithHttpProgress(httpClient, addonData.Downloads[0]);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Something went wrong in " + nameof(ProccessAddonData) + " for " + addonData.ProjectName);
                return (found, addonData);
            }

            if (string.IsNullOrEmpty(zipFile) || !File.Exists(zipFile))
            {
                logger.LogError("No zipFile found for " + addonData.ProjectName);
                return (found, addonData);
            }

            FileInfo fileInfo = new FileInfo(zipFile);

            addonData.Size = fileInfo.Length;

            try
            {
                (int entries, List<string> folders) = Update.UpdateAddon2(zipFile);
                addonData.Files = entries;

                if (folders.Count == 1)
                {
                    addonData.FolderName = folders[0];
                    found = true;
                }
                else
                {
                    foreach (string folder in folders)
                    {
                        string lowered = folder.ToLower();
                        if (PROJECT_URLS.TryGetValue(lowered, out string mapped))
                        {
                            if (mapped.Equals(addonData.ProjectName))
                            {
                                found = true;
                                addonData.FolderName = folder;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        HashSet<string> urlNames = new HashSet<string>() {
                    addonData.ProjectName,
                    addonData.ProjectName.Replace("-",""),
                    addonData.ProjectName.Replace("_",""),
                    addonData.ProjectName.Replace("_", "-"),
                    addonData.ProjectName.Replace("-", "_")};
                        foreach (string folder in folders)
                        {
                            string lowered = folder.ToLower();

                            HashSet<string> folderNames = new HashSet<string>() {
                    lowered,
                    lowered.Replace(" ", "-"),
                    lowered.Replace("_", "-")};

                            if (folderNames.Contains(addonData.ProjectName))
                            {
                                found = true;
                                addonData.FolderName = folder;

                                break;
                            }

                            if (urlNames.Contains(lowered))
                            {
                                found = true;
                                addonData.FolderName = folder;

                                break;
                            }
                        }
                    }
                }

                addonData.SubFolders = folders.Where(f => !f.Equals(addonData.FolderName)).ToHashSet();

                if (found)
                {
                    File.Delete(zipFile);
                    Directory.Delete(zipFile.Replace(".zip", ""), true);
                }

                return (found, addonData);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Someting went wrong for " + addonData.ProjectName);
                return (found, addonData);
            }
        }

        private static async Task<AddonData> FromCurseToAddonData(HttpClient httpClient, CurseAddon curseAddon, int tries, List<AddonData> loadedData)
        {
            List<AddonData> foundDataList = loadedData.Where(ad => ad.ProjectName.Equals(curseAddon.AddonURL)).ToList();
            if (foundDataList.Count == 1)
            {
                AddonData foundData = foundDataList[0];
                foundData.NrOfDownloads = curseAddon.Downloads;
                foundData.UpdatedEpoch = curseAddon.UpdatedEpoch;
                foundData.CreatedEpoch = curseAddon.CreatedEpoch;
                foundData.Description = curseAddon.Description;
                foundData.Title = curseAddon.Title;

                try
                {
                    foundData.Downloads = await Pure.Version.DownloadVersionsFor(httpClient, foundData.ProjectName);
                }
                catch (Exception e)
                {
                    logger.LogError(e, nameof(FromCurseToAddonData) + " error getting versions");
                }

                AddonData copy = new AddonData()
                {
                    CreatedEpoch = foundData.CreatedEpoch,
                    Title = foundData.Title,
                    Description = foundData.Description,
                    Downloads = foundData.Downloads,
                    Files = foundData.Files,
                    FolderName = foundData.FolderName,
                    NrOfDownloads = foundData.NrOfDownloads,
                    ProjectName = foundData.ProjectName,
                    Size = foundData.Size,
                    SubFolders = foundData.SubFolders,
                    UpdatedEpoch = foundData.UpdatedEpoch
                };

                return copy;
            }
            if (foundDataList.Count > 1)
            {
                logger.LogInformation("foundDataList.Count=" + foundDataList.Count + " for " + curseAddon.AddonURL);
            }

            AddonData addonData = curseAddon.toAddonData();

            for (int i = 0; i < tries; i++)
            {
                try
                {
                    addonData.Downloads = await Pure.Version.DownloadVersionsFor(httpClient, addonData.ProjectName);
                    if (addonData.Downloads.Count > 1)
                    {
                        return addonData;
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, nameof(FromCurseToAddonData) + " try: " + i + "/" + tries);
                }
            }

            return addonData;
        }

        public static async Task<List<CurseAddon>> Scrape(HttpClient httpClient, int tries, int from, int to)
        {
            List<CurseAddon> addons = new List<CurseAddon>();
            for (int i = from; i <= to; i++)
            {
                await Task.Delay(10);
                string page = await DownloadPage(httpClient, tries, i);
                if (string.IsNullOrEmpty(page))
                {
                    logger.LogWarning("MISSED page: " + i);
                }
                else
                {
                    try
                    {
                        List<CurseAddon> addonsFromPage = HtmlAgilityParser.FromCursePageToCurseAddons(page);
                        addons.AddRange(addonsFromPage);
                        logger.LogInformation("Page: " + i + ", Added: " + addonsFromPage.Count);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, nameof(Program.Scrape));
                    }
                }
            }

            return addons;
        }

        public static async Task<string> DownloadPage(HttpClient httpClient, int tries, int page)
        {
            for (int i = 0; i < tries; i++)
            {
                try
                {
                    string body = await httpClient.GetStringAsync("https://www.curseforge.com/wow/addons" + "?page=" + page);

                    if (!string.IsNullOrEmpty(body))
                    {
                        return body;
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, nameof(DownloadPage) + ", try: " + i);
                }
            }
            return string.Empty;
        }
    }
}