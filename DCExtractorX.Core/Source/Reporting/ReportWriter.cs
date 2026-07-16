using Custom.Diagnostics;
using Custom.IO;
using DC.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DC.Reporting
{
    /// <summary>
    /// Class   :   "ReportWriter"
    /// 
    /// Purpose :   writes the CSV/JSON report files for a completed (or in-progress) pipeline run: summary.csv,
    /// summary.json, analysis.csv, errors.csv, warnings.csv, unsupported.csv, and a copy of the run's log as
    /// execution.log. Also writes the detailed per-model models.csv under Assets/Analysis.
    /// </summary>
    public static class ReportWriter
    {
        static readonly string[] s_szModelColumns =
        {
            "ModelName", "MeshCount", "BoneCount", "Vertices", "Faces", "Normals", "UVs", "Materials",
            "HasWGT", "HasMOT", "HasBBP", "HierarchyDepth", "Classification", "ClassificationScore",
        };

        /// <summary>
        /// Writes Assets/Analysis/models.csv with the full per-model statistics.
        /// </summary>
        public static void WriteModelsCsv(string szFilePath, IEnumerable<ModelAnalysis> tAnalyses)
        {
            try
            {
                IEnumerable<IEnumerable<object>> tRows = tAnalyses.Select(a => (IEnumerable<object>)new object[]
                {
                    a.ModelName, a.MeshCount, a.BoneCount, a.VertexCount, a.FaceCount, a.NormalCount, a.UVCount,
                    a.MaterialCount, a.HasWGT, a.HasMOT, a.HasBBP, a.HierarchyDepth, a.Classification, a.ClassificationScore,
                });

                CsvExporter.Write(szFilePath, s_szModelColumns, tRows);
                Logger.Info("Wrote model analysis CSV.", szPath: szFilePath, bEchoToConsole: false);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to write models.csv.", szPath: szFilePath, tException: ex);
            }
        }

        /// <summary>
        /// Writes all run-level report files (summary.csv, summary.json, analysis.csv, errors.csv, warnings.csv,
        /// unsupported.csv, execution.log) into the given Reports directory. Never throws; a failure writing one
        /// report is logged and the rest are still attempted.
        /// </summary>
        public static void WriteAll(string szReportsDirectory, RunReport tReport)
        {
            Directory.CreateDirectory(szReportsDirectory);

            WriteSummaryCsv(Path.Combine(szReportsDirectory, "summary.csv"), tReport);
            WriteSummaryJson(Path.Combine(szReportsDirectory, "summary.json"), tReport);
            WriteModelsCsv(Path.Combine(szReportsDirectory, "analysis.csv"), tReport.ModelAnalyses);
            WriteRecordsCsv(Path.Combine(szReportsDirectory, "errors.csv"), tReport.ErrorRecords);
            WriteRecordsCsv(Path.Combine(szReportsDirectory, "warnings.csv"), tReport.WarningRecords);
            WriteRecordsCsv(Path.Combine(szReportsDirectory, "unsupported.csv"), tReport.UnsupportedRecords);
            CopyExecutionLog(Path.Combine(szReportsDirectory, "execution.log"));
        }

        static void WriteSummaryCsv(string szFilePath, RunReport tReport)
        {
            try
            {
                List<IEnumerable<object>> tRows = new List<IEnumerable<object>>
                {
                    new object[] { "Processed", tReport.Processed },
                    new object[] { "Success", tReport.Success },
                    new object[] { "Failed", tReport.Failed },
                    new object[] { "Unsupported", tReport.Unsupported },
                    new object[] { "Warnings", tReport.Warnings },
                    new object[] { "Errors", tReport.Errors },
                    new object[] { "ModelsAnalyzed", tReport.ModelAnalyses.Count },
                    new object[] { "StartedAt", tReport.StartedAt.ToString("yyyy-MM-dd HH:mm:ss") },
                    new object[] { "FinishedAt", tReport.FinishedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty },
                    new object[] { "DurationSeconds", tReport.FinishedAt.HasValue ? (tReport.FinishedAt.Value - tReport.StartedAt).TotalSeconds : 0.0 },
                };

                CsvExporter.Write(szFilePath, new[] { "Metric", "Value" }, tRows);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to write summary.csv.", szPath: szFilePath, tException: ex);
            }
        }

        static void WriteSummaryJson(string szFilePath, RunReport tReport)
        {
            try
            {
                var tSummary = new
                {
                    tReport.Processed,
                    tReport.Success,
                    tReport.Failed,
                    tReport.Unsupported,
                    tReport.Warnings,
                    tReport.Errors,
                    ModelsAnalyzed = tReport.ModelAnalyses.Count,
                    StartedAt = tReport.StartedAt,
                    FinishedAt = tReport.FinishedAt,
                    DurationSeconds = tReport.FinishedAt.HasValue ? (tReport.FinishedAt.Value - tReport.StartedAt).TotalSeconds : 0.0,
                };

                string szDirectory = Path.GetDirectoryName(szFilePath);
                if (string.IsNullOrEmpty(szDirectory) == false)
                    Directory.CreateDirectory(szDirectory);

                File.WriteAllText(szFilePath, JsonSerializer.Serialize(tSummary, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to write summary.json.", szPath: szFilePath, tException: ex);
            }
        }

        static void WriteRecordsCsv(string szFilePath, IReadOnlyList<AssetRecord> tRecords)
        {
            try
            {
                IEnumerable<IEnumerable<object>> tRows = tRecords.Select(r => (IEnumerable<object>)new object[]
                {
                    r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"), r.Asset, r.Stage, r.Message, r.Path,
                });

                CsvExporter.Write(szFilePath, new[] { "Timestamp", "Asset", "Stage", "Message", "Path" }, tRows);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to write report CSV.", szPath: szFilePath, tException: ex);
            }
        }

        static void CopyExecutionLog(string szDestinationPath)
        {
            try
            {
                if (string.IsNullOrEmpty(Logger.LogFilePath) || File.Exists(Logger.LogFilePath) == false)
                    return;

                File.Copy(Logger.LogFilePath, szDestinationPath, true);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to copy execution.log.", szPath: szDestinationPath, tException: ex);
            }
        }
    }
}
