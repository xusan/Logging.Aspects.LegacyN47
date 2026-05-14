using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Linq;

namespace Logging.Aspects.Legacy
{
    public class NLogFileLogger : IFileLoger
    {
        private Logger logger;
        private string logDir, logFileName, currentLogPath;        

        public void Init()
        {
            //Logger creates folder for every day with yyyy-MM-dd as name 
            //Each folder(yyyy-MM-dd) contains log files for each session/run 
            logDir = GetLogsFolder();
            //create day folder
            var dayStamp = DateTime.Now.ToString("yyyy-MM-dd");
            var sessionFolder = Path.Combine(logDir, dayStamp);
            // Create unique log file name for each session using timestamp
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logFileName = $"session_{timestamp}.log";
            currentLogPath = Path.Combine(sessionFolder, logFileName);

            //set configuration
            var config = new LoggingConfiguration();
            //save log to file
            var fileTarget = new FileTarget("logfile")
            {
                FileName = currentLogPath,
                // Log just message as it is, because we have own message format
                Layout = "${message}",
                // Performance: keep file open
                KeepFileOpen = true,
                //ConcurrentWrites = true,
                // Disable automatic archiving, we will archive manually
                ArchiveEvery = FileArchivePeriod.None
            };
            //add this file target to config
            config.AddTarget(fileTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);
            LogManager.Configuration = config;
            logger = LogManager.GetCurrentClassLogger();

            CleanupOldLogs();
        }

        public void WriteLine(string message)
        {
            logger.Info(message);
        }

        /// <summary>
        /// Keep only logs from the last 7 days(7 folders with logs)
        /// </summary>
        private void CleanupOldLogs()
        {
            try
            {
                // Get all directories matching the yyyy-MM-dd pattern
                var folders = Directory.GetDirectories(logDir)
                    .Select(dir => new DirectoryInfo(dir))
                    .Where(di => DateTime.TryParseExact(
                        di.Name,
                        "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out _))
                    .OrderByDescending(di => DateTime.ParseExact(
                        di.Name,
                        "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture))
                    .ToList();

                if (folders.Count > 7)
                {
                    // Keep the newest 7, remove the rest
                    var oldFolders = folders.Skip(7);

                    foreach (var folder in oldFolders)
                    {
                        try
                        {
                            Console.WriteLine($"Deleting: {folder.FullName}");
                            Directory.Delete(folder.FullName, true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to delete {folder.FullName}: {ex}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Skip log clean up because there is less than 7 logs");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string GetLogsFolder()
        {
            var path = "Xusan_NLog";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
