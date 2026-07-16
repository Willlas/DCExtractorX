using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Custom.Diagnostics
{
    /// <summary>
    /// Enumeration :   "LogLevel"
    /// 
    /// Purpose     :   the severity of a single log entry, ordered from least to most severe.
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Class   :   "LogEntry"
    /// 
    /// Purpose :   a single structured log record. Every field beyond Timestamp/Level/Message is optional context
    /// that callers can attach depending on what they're doing (parsing an asset, extracting a file, etc).
    /// </summary>
    public sealed class LogEntry
    {
        public DateTime Timestamp;
        public LogLevel Level;
        public string Message;
        public string Asset;
        public string File;
        public string Path;
        public long? Offset;
        public string Model;
        public string Texture;
        public Exception Exception;
        public string StackTrace;

        /// <summary>
        /// Formats the entry as a single human-readable block suitable for console or file output.
        /// </summary>
        public string Format()
        {
            StringBuilder tBuilder = new StringBuilder();
            tBuilder.Append(Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
            tBuilder.Append(" [").Append(Level.ToString().ToUpperInvariant()).Append("] ");
            tBuilder.Append(Message);

            List<string> tContext = new List<string>();
            if (string.IsNullOrEmpty(Asset) == false) tContext.Add("Asset=" + Asset);
            if (string.IsNullOrEmpty(Model) == false) tContext.Add("Model=" + Model);
            if (string.IsNullOrEmpty(Texture) == false) tContext.Add("Texture=" + Texture);
            if (string.IsNullOrEmpty(File) == false) tContext.Add("File=" + File);
            if (string.IsNullOrEmpty(Path) == false) tContext.Add("Path=" + Path);
            if (Offset.HasValue) tContext.Add("Offset=" + Offset.Value);

            if (tContext.Count > 0)
                tBuilder.Append(" | ").Append(string.Join(" ", tContext));

            if (Exception != null)
            {
                tBuilder.Append(Environment.NewLine).Append("    Exception: ").Append(Exception.GetType().Name).Append(": ").Append(Exception.Message);
                string szStack = StackTrace ?? Exception.StackTrace;
                if (string.IsNullOrEmpty(szStack) == false)
                    tBuilder.Append(Environment.NewLine).Append(szStack);
            }

            return tBuilder.ToString();
        }
    }

    /// <summary>
    /// Interface   :   "ILogSink"
    /// 
    /// Purpose     :   a destination that log entries can be written to (console, file, GUI list, etc).
    /// </summary>
    public interface ILogSink
    {
        void Write(LogEntry tEntry);
    }

    /// <summary>
    /// Class   :   "ConsoleLogSink"
    /// 
    /// Purpose :   writes log entries to the console, colored by severity.
    /// </summary>
    public sealed class ConsoleLogSink : ILogSink
    {
        public void Write(LogEntry tEntry)
        {
            ConsoleColor eColor = tEntry.Level switch
            {
                LogLevel.Debug => ConsoleColor.DarkGray,
                LogLevel.Info => ConsoleColor.Gray,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.Gray,
            };

            Console.ForegroundColor = eColor;
            Console.WriteLine(tEntry.Format());
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Class   :   "FileLogSink"
    /// 
    /// Purpose :   appends log entries to a single log file for the duration of a run.
    /// </summary>
    public sealed class FileLogSink : ILogSink
    {
        readonly object m_tLock = new object();
        readonly string m_szFilePath;

        public FileLogSink(string szFilePath)
        {
            m_szFilePath = szFilePath;
        }

        public string filePath => m_szFilePath;

        public void Write(LogEntry tEntry)
        {
            lock (m_tLock)
            {
                File.AppendAllText(m_szFilePath, tEntry.Format() + Environment.NewLine);
            }
        }
    }

    /// <summary>
    /// Class   :   "Logger"
    /// 
    /// Purpose :   a central, ambient logging facility used throughout Core, the CLI, and the GUI. Writes to
    /// console and a single per-run file under a "logs" directory (one file per run, named by start timestamp),
    /// and raises an event so hosts (e.g. the GUI) can additionally display entries in a live log view.
    /// 
    /// This never throws: a failure to write to a sink (e.g. a locked log file) is swallowed so that logging
    /// itself can never crash the run it's trying to describe.
    /// </summary>
    public static class Logger
    {
        static readonly List<ILogSink> s_tFileSinks = new List<ILogSink>();
        static readonly List<ILogSink> s_tConsoleSinks = new List<ILogSink>();
        static bool s_bInitialized;

        /// <summary>
        /// Raised for every log entry, regardless of console echo settings. Hosts (e.g. the GUI log view) can
        /// subscribe to this to display the full log live.
        /// </summary>
        public static event Action<LogEntry> EntryLogged;

        /// <summary>
        /// Gets the path of the current run's log file, once initialized.
        /// </summary>
        public static string LogFilePath { get; private set; }

        /// <summary>
        /// Initializes the logger for this run: creates the logs directory (if needed) and a new, timestamped
        /// log file for this run. Safe to call more than once; subsequent calls are ignored.
        /// </summary>
        /// <param name="szLogsDirectory">The directory to write log files to. Defaults to a "logs" folder next to the executable.</param>
        /// <param name="bEchoToConsole">Whether entries should also be written to the console by default.</param>
        public static void Init(string szLogsDirectory = null, bool bEchoToConsole = true)
        {
            if (s_bInitialized)
                return;
            s_bInitialized = true;

            try
            {
                string szDirectory = szLogsDirectory ?? Path.Combine(AppContext.BaseDirectory, "logs");
                Directory.CreateDirectory(szDirectory);

                string szFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture) + ".log";
                LogFilePath = Path.Combine(szDirectory, szFileName);

                s_tFileSinks.Add(new FileLogSink(LogFilePath));
            }
            catch (Exception ex)
            {
                //If we can't even create the log file, fall back to console-only logging rather than crashing startup.
                Console.Error.WriteLine("Logger: Unable to initialize file logging: " + ex.Message);
            }

            if (bEchoToConsole)
                s_tConsoleSinks.Add(new ConsoleLogSink());
        }

        /// <summary>
        /// Adds an additional sink (e.g. a GUI list-backed sink) that receives every entry.
        /// </summary>
        public static void AddSink(ILogSink tSink)
        {
            if (tSink != null)
                s_tFileSinks.Add(tSink);
        }

        public static void Debug(string szMessage, string szAsset = null, string szFile = null, string szPath = null, long? nOffset = null, string szModel = null, string szTexture = null, Exception tException = null, bool bEchoToConsole = true)
            => Write(LogLevel.Debug, szMessage, szAsset, szFile, szPath, nOffset, szModel, szTexture, tException, bEchoToConsole);

        public static void Info(string szMessage, string szAsset = null, string szFile = null, string szPath = null, long? nOffset = null, string szModel = null, string szTexture = null, Exception tException = null, bool bEchoToConsole = true)
            => Write(LogLevel.Info, szMessage, szAsset, szFile, szPath, nOffset, szModel, szTexture, tException, bEchoToConsole);

        public static void Warn(string szMessage, string szAsset = null, string szFile = null, string szPath = null, long? nOffset = null, string szModel = null, string szTexture = null, Exception tException = null, bool bEchoToConsole = true)
            => Write(LogLevel.Warning, szMessage, szAsset, szFile, szPath, nOffset, szModel, szTexture, tException, bEchoToConsole);

        public static void Error(string szMessage, string szAsset = null, string szFile = null, string szPath = null, long? nOffset = null, string szModel = null, string szTexture = null, Exception tException = null, bool bEchoToConsole = true)
            => Write(LogLevel.Error, szMessage, szAsset, szFile, szPath, nOffset, szModel, szTexture, tException, bEchoToConsole);

        static void Write(LogLevel eLevel, string szMessage, string szAsset, string szFile, string szPath, long? nOffset, string szModel, string szTexture, Exception tException, bool bEchoToConsole)
        {
            LogEntry tEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = eLevel,
                Message = szMessage ?? string.Empty,
                Asset = szAsset,
                File = szFile,
                Path = szPath,
                Offset = nOffset,
                Model = szModel,
                Texture = szTexture,
                Exception = tException,
                StackTrace = tException?.StackTrace,
            };

            for (int i = 0; i < s_tFileSinks.Count; i++)
                SafeWrite(s_tFileSinks[i], tEntry);

            if (bEchoToConsole)
            {
                for (int i = 0; i < s_tConsoleSinks.Count; i++)
                    SafeWrite(s_tConsoleSinks[i], tEntry);
            }

            try
            {
                EntryLogged?.Invoke(tEntry);
            }
            catch
            {
                //A misbehaving subscriber (e.g. a GUI event handler) must never crash logging itself.
            }
        }

        static void SafeWrite(ILogSink tSink, LogEntry tEntry)
        {
            try
            {
                tSink.Write(tEntry);
            }
            catch
            {
                //Logging must never throw; a broken sink just silently stops working rather than crashing the run.
            }
        }
    }

    /// <summary>
    /// Class   :   "LoggingLog"
    /// 
    /// Purpose :   decorates an existing ILog (ConsoleLog, GuiLog) so that every Warn/Error also gets recorded by
    /// the central Logger (file + optional console echo), without changing the existing host-specific behavior
    /// (colored console lines, MessageBox popups) that callers of Log.Current already rely on.
    /// 
    /// The Logger echo is intentionally suppressed here (bEchoToConsole: false) since the wrapped ILog already
    /// prints/displays the message itself; this only adds a persistent record to the run's log file.
    /// </summary>
    public sealed class LoggingLog : ILog
    {
        readonly ILog m_tInner;

        public LoggingLog(ILog tInner)
        {
            m_tInner = tInner ?? new NullLog();
        }

        public void Warn(string message, string title)
        {
            Logger.Warn(message, szAsset: title, bEchoToConsole: false);
            m_tInner.Warn(message, title);
        }

        public void Error(string message, string title)
        {
            Logger.Error(message, szAsset: title, bEchoToConsole: false);
            m_tInner.Error(message, title);
        }
    }
}
