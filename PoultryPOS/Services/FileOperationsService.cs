using System.Text.Json;
using System.IO;
using PoultryPOS.Models;

namespace PoultryPOS.Services
{
    public class FileOperationsService
    {
        private readonly SyncConfigurationService _configService;

        public FileOperationsService()
        {
            _configService = new SyncConfigurationService();
        }

        public void EnsureFoldersExist()
        {
            var config = _configService.GetConfiguration();
            var basePath = config.CloudFolderPath;

            try
            {
                Directory.CreateDirectory(basePath);
                Directory.CreateDirectory(Path.Combine(basePath, "PC1_Changes"));
                Directory.CreateDirectory(Path.Combine(basePath, "PC2_Changes"));
                Directory.CreateDirectory(Path.Combine(basePath, "Archive"));
                Directory.CreateDirectory(Path.Combine(basePath, "Archive", "Processed"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddChangeToTodaysFile(SyncChange change)
        {
            try
            {
                EnsureFoldersExist();

                var myChangesFolder = _configService.GetMyChangesFolder();
                var todayFileName = $"{_configService.GetDeviceId()}_{DateTime.Today:yyyy-MM-dd}.json";
                var filePath = Path.Combine(myChangesFolder, todayFileName);

                var dailyFile = LoadOrCreateDailyFile(filePath);
                dailyFile.Changes.Add(change);
                dailyFile.LastUpdated = DateTime.Now;

                SaveDailyFile(filePath, dailyFile);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DailySyncFile LoadOrCreateDailyFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    var dailyFile = JsonSerializer.Deserialize<DailySyncFile>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    return dailyFile ?? CreateNewDailyFile();
                }
                catch
                {
                    return CreateNewDailyFile();
                }
            }

            return CreateNewDailyFile();
        }

        private DailySyncFile CreateNewDailyFile()
        {
            return new DailySyncFile
            {
                DeviceId = _configService.GetDeviceId(),
                Date = DateTime.Today.ToString("yyyy-MM-dd"),
                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now,
                Changes = new List<SyncChange>()
            };
        }

        private void SaveDailyFile(string filePath, DailySyncFile dailyFile)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(dailyFile, options);
            File.WriteAllText(filePath, json);
        }

        public List<DailySyncFile> GetNewDailyFilesFromOtherDevices()
        {
            var dailyFiles = new List<DailySyncFile>();
            var config = _configService.GetConfiguration();

            var basePath = config.CloudFolderPath;
            var allDevices = new[] { "PC1", "PC2" };
            var myDeviceId = _configService.GetDeviceId();

            foreach (var device in allDevices.Where(d => d != myDeviceId))
            {
                var deviceFolder = Path.Combine(basePath, $"{device}_Changes");

                if (!Directory.Exists(deviceFolder)) continue;

                var files = Directory.GetFiles(deviceFolder, $"{device}_*.json").ToList();

                foreach (var file in files)
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var dailyFile = JsonSerializer.Deserialize<DailySyncFile>(json, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                        if (dailyFile != null && dailyFile.Changes.Count > 0)
                        {
                            dailyFiles.Add(dailyFile);
                        }
                    }
                    catch (Exception)
                    {
                        // Skip corrupted files
                    }
                }
            }

            return dailyFiles;
        }

        public void ArchiveProcessedFile(string sourceFilePath)
        {
            // Archive files older than 30 days
        }

        private void LogError(string message)
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PoultryPOS", "sync_errors.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}\n");
        }
    }
}