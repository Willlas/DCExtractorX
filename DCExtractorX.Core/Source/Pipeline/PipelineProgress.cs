using System;

namespace DC.Pipeline
{
    /// <summary>
    /// Class   :   "PipelineProgress"
    /// 
    /// Purpose :   ambient, static progress reporting for the "Analyze + Extract All" pipeline, mirroring the
    /// existing DC.IO.DCProgress pattern (static events, no UI framework dependency) but with the richer,
    /// pipeline-specific fields the GUI/CLI want to display (stage, current file/asset, processed/warning/error
    /// counts) alongside the existing value/maximum/name progress bar driven by DCProgress.
    /// </summary>
    public static class PipelineProgress
    {
        static string m_szStage = string.Empty;
        static string m_szCurrentFile = string.Empty;
        static string m_szCurrentAsset = string.Empty;
        static int m_nProcessed;
        static int m_nWarnings;
        static int m_nErrors;

        public static event Action<string> StageChanged;
        public static event Action<string> CurrentFileChanged;
        public static event Action<string> CurrentAssetChanged;
        public static event Action<int> ProcessedChanged;
        public static event Action<int> WarningsChanged;
        public static event Action<int> ErrorsChanged;

        public static string stage
        {
            get { return m_szStage; }
            set { m_szStage = value ?? string.Empty; StageChanged?.Invoke(m_szStage); }
        }

        public static string currentFile
        {
            get { return m_szCurrentFile; }
            set { m_szCurrentFile = value ?? string.Empty; CurrentFileChanged?.Invoke(m_szCurrentFile); }
        }

        public static string currentAsset
        {
            get { return m_szCurrentAsset; }
            set { m_szCurrentAsset = value ?? string.Empty; CurrentAssetChanged?.Invoke(m_szCurrentAsset); }
        }

        public static int processed
        {
            get { return m_nProcessed; }
            set { m_nProcessed = value; ProcessedChanged?.Invoke(m_nProcessed); }
        }

        public static int warnings
        {
            get { return m_nWarnings; }
            set { m_nWarnings = value; WarningsChanged?.Invoke(m_nWarnings); }
        }

        public static int errors
        {
            get { return m_nErrors; }
            set { m_nErrors = value; ErrorsChanged?.Invoke(m_nErrors); }
        }

        /// <summary>
        /// Resets all fields to their initial state, ready for a new run.
        /// </summary>
        public static void Reset()
        {
            stage = string.Empty;
            currentFile = string.Empty;
            currentAsset = string.Empty;
            processed = 0;
            warnings = 0;
            errors = 0;
        }
    }
}
