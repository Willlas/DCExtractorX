using System;
using System.Collections.Generic;

namespace DC.Reporting
{
    /// <summary>
    /// Class   :   "AssetRecord"
    /// 
    /// Purpose :   a single tracked event for one asset during a pipeline run (a success, failure, warning, or
    /// unsupported-file notice), used to populate the various report CSVs.
    /// </summary>
    public sealed class AssetRecord
    {
        public DateTime Timestamp = DateTime.Now;
        public string Asset;
        public string Stage;
        public string Message;
        public string Path;
    }

    /// <summary>
    /// Class   :   "RunReport"
    /// 
    /// Purpose :   accumulates counters and per-asset records for a single "Analyze + Extract All" pipeline run.
    /// Not thread-safe by itself; the pipeline serializes asset processing per stage, so this is only ever
    /// mutated from one thread at a time.
    /// </summary>
    public sealed class RunReport
    {
        public DateTime StartedAt = DateTime.Now;
        public DateTime? FinishedAt;

        public int Processed;
        public int Success;
        public int Failed;
        public int Unsupported;
        public int Warnings;
        public int Errors;

        public readonly List<AssetRecord> SuccessRecords = new List<AssetRecord>();
        public readonly List<AssetRecord> ErrorRecords = new List<AssetRecord>();
        public readonly List<AssetRecord> WarningRecords = new List<AssetRecord>();
        public readonly List<AssetRecord> UnsupportedRecords = new List<AssetRecord>();

        public readonly List<DC.Analysis.ModelAnalysis> ModelAnalyses = new List<DC.Analysis.ModelAnalysis>();

        /// <summary>
        /// Records that an asset was looked at during a stage (regardless of outcome).
        /// </summary>
        public void RecordProcessed()
        {
            Processed++;
        }

        /// <summary>
        /// Records a successfully processed asset.
        /// </summary>
        public void RecordSuccess(string szAsset, string szStage, string szMessage = null, string szPath = null)
        {
            Success++;
            SuccessRecords.Add(new AssetRecord { Asset = szAsset, Stage = szStage, Message = szMessage, Path = szPath });
        }

        /// <summary>
        /// Records that an asset failed during a stage. This never throws or stops the run - the caller is
        /// expected to continue processing the next asset.
        /// </summary>
        public void RecordFailure(string szAsset, string szStage, string szMessage, string szPath = null)
        {
            Failed++;
            Errors++;
            ErrorRecords.Add(new AssetRecord { Asset = szAsset, Stage = szStage, Message = szMessage, Path = szPath });
        }

        /// <summary>
        /// Records a non-fatal warning for an asset.
        /// </summary>
        public void RecordWarning(string szAsset, string szStage, string szMessage, string szPath = null)
        {
            Warnings++;
            WarningRecords.Add(new AssetRecord { Asset = szAsset, Stage = szStage, Message = szMessage, Path = szPath });
        }

        /// <summary>
        /// Records an asset/file that was recognized but isn't supported (e.g. an unknown format).
        /// </summary>
        public void RecordUnsupported(string szAsset, string szStage, string szMessage, string szPath = null)
        {
            Unsupported++;
            UnsupportedRecords.Add(new AssetRecord { Asset = szAsset, Stage = szStage, Message = szMessage, Path = szPath });
        }

        /// <summary>
        /// Marks the run as finished.
        /// </summary>
        public void Finish()
        {
            FinishedAt = DateTime.Now;
        }
    }
}
