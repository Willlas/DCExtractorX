using Custom.Diagnostics;
using DC.Analysis;
using DC.IO;
using DC.Reporting;
using DC.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DC.Pipeline
{
    /// <summary>
    /// Class   :   "AnalyzeExtractPipeline"
    /// 
    /// Purpose :   the "Analyze + Extract All" one-click workflow: Scan -&gt; Extract -&gt; Convert -&gt; Analyze -&gt; Report.
    /// 
    /// This is purely an orchestrator: every extraction/conversion call it makes (DAT.ExtractHD, PAK.ExtractDirectory,
    /// MDS.LoadDirectory, TM2/IMG.LoadDirectory, SMD.Save, ImageHelpers.SavePNG) is the exact same Core call the
    /// existing menu items already use - nothing about how archives are read or files are converted has changed.
    /// What's new here is: resolving/validating input, organizing the results into the Output/ folder structure,
    /// analyzing the original parsed models, and writing the report files. Individual asset failures are logged
    /// and recorded (via RunReport) but never stop the run; only a missing/unreadable root archive is fatal.
    /// </summary>
    public static class AnalyzeExtractPipeline
    {
        static readonly string[] s_szPakExtensions = { ".pak", ".chr", ".ipk", ".mpk", ".pcp", ".efp", ".snd", ".sky" };

        //Extensions that are either source archives (already accounted for) or already routed to a specific
        //Output/ subfolder elsewhere in OrganizeOutputs. Anything extracted with an extension NOT in this set is
        //treated as "OtherThings"/unsupported for organization purposes.
        static readonly HashSet<string> s_szKnownExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".dat", ".hd2", ".hd3", ".pak", ".chr", ".ipk", ".mpk", ".pcp", ".efp", ".snd", ".sky",
            ".mds", ".wgt", ".mot", ".bbp", ".cfg", ".tm2", ".img", ".smd", ".obj", ".png",
        };

        /// <summary>
        /// Runs the full pipeline against a game directory (expected to contain DATA.DAT + DATA.HD2/HD3 at its
        /// root, matching the legacy "Extract &amp; Convert Everything" input), writing all outputs under
        /// szOutputDirectory. Extraction/conversion happens in-place in szGameDirectory first (exactly like the
        /// legacy flow), then results are organized (copied) into the Output/ tree.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown only for a fatal startup condition: invalid input or an unreadable root archive.</exception>
        public static RunReport Run(string szGameDirectory, string szOutputDirectory)
        {
            if (string.IsNullOrWhiteSpace(szGameDirectory) || Directory.Exists(szGameDirectory) == false)
                throw new InvalidOperationException("Invalid input: game directory does not exist: " + szGameDirectory);
            if (string.IsNullOrWhiteSpace(szOutputDirectory))
                throw new InvalidOperationException("Invalid input: no output directory specified.");

            RunReport tReport = new RunReport();
            PipelineProgress.Reset();

            //Captures every Warning/Error the whole run logs (including deep inside MDS/WGT/MOT/BBP/IMG/TM2/PAK/DAT
            //and the exporters) into the report, tagged with whatever stage was active when it was logged.
            void OnEntryLogged(LogEntry tEntry)
            {
                string szAsset = tEntry.Asset ?? tEntry.File ?? "(unknown)";
                if (tEntry.Level == LogLevel.Error)
                {
                    tReport.RecordFailure(szAsset, PipelineProgress.stage, tEntry.Message, tEntry.Path);
                    PipelineProgress.errors = tReport.Errors;
                }
                else if (tEntry.Level == LogLevel.Warning)
                {
                    tReport.RecordWarning(szAsset, PipelineProgress.stage, tEntry.Message, tEntry.Path);
                    PipelineProgress.warnings = tReport.Warnings;
                }
            }

            Logger.EntryLogged += OnEntryLogged;
            try
            {
                RunCore(szGameDirectory, szOutputDirectory, tReport);
            }
            finally
            {
                Logger.EntryLogged -= OnEntryLogged;
            }

            return tReport;
        }

        static void RunCore(string szGameDirectory, string szOutputDirectory, RunReport tReport)
        {
            Logger.Info("Starting Analyze + Extract All pipeline.", szPath: szGameDirectory);

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Scan - locate the root archive. This is the one case where a failure aborts the whole run.
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            PipelineProgress.stage = "Scan";
            DCProgress.name = "Scanning";

            string szDatPath = Path.Combine(szGameDirectory, "DATA.DAT");
            string szHd2Path = Path.Combine(szGameDirectory, "DATA.HD2");
            string szHd3Path = Path.Combine(szGameDirectory, "DATA.HD3");
            bool bHasHd2 = File.Exists(szHd2Path);
            bool bHasHd3 = File.Exists(szHd3Path);

            if (File.Exists(szDatPath) == false || (bHasHd2 == false && bHasHd3 == false))
            {
                string szMessage = "Cannot open root archive: DATA.DAT and DATA.HD2/DATA.HD3 must both exist in " + szGameDirectory;
                Logger.Error(szMessage, szPath: szGameDirectory);
                throw new InvalidOperationException(szMessage);
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Prepare the Output/ folder structure up-front (section 4 of the spec).
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            string szAssetsDir = Path.Combine(szOutputDirectory, "Assets");
            string szModelsDir = Path.Combine(szAssetsDir, "Models");
            string szTexturesDir = Path.Combine(szAssetsDir, "Textures");
            string szPngDir = Path.Combine(szAssetsDir, "PNG");
            string szAnimationsDir = Path.Combine(szAssetsDir, "Animations");
            string szAnalysisDir = Path.Combine(szAssetsDir, "Analysis");
            string szTexturesWithoutModelsDir = Path.Combine(szOutputDirectory, "TexturesWithoutModels");
            string szOtherThingsDir = Path.Combine(szOutputDirectory, "OtherThings");
            string szReportsDir = Path.Combine(szOutputDirectory, "Reports");

            foreach (string szDir in new[] { szModelsDir, szTexturesDir, szPngDir, szAnimationsDir, szAnalysisDir, szTexturesWithoutModelsDir, szOtherThingsDir, szReportsDir })
                Directory.CreateDirectory(szDir);

            try
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////
                // Extract - identical calls to the legacy "Extract & Convert Everything" flow.
                //////////////////////////////////////////////////////////////////////////////////////////////////
                PipelineProgress.stage = "Extract";
                DCProgress.name = "Extracting DAT archive";

                DAT.HDType eType = bHasHd2 ? DAT.HDType.Two : DAT.HDType.Three;
                string szHdPath = bHasHd2 ? szHd2Path : szHd3Path;
                tReport.RecordProcessed();
                DAT.ExtractHD(szDatPath, szHdPath, szGameDirectory, eType);

                DCProgress.name = "Extracting PAK-family archives";
                PAK.ExtractDirectory(szGameDirectory, s_szPakExtensions);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // Convert - identical calls to the legacy flow (MDS -> SMD, TM2/IMG -> PNG).
                //////////////////////////////////////////////////////////////////////////////////////////////////
                PipelineProgress.stage = "Convert";

                //MDS/TM2/IMG's *.LoadDirectory already reports per-file progress via DCProgress (including files
                //that fail to parse); tapping into it here gives an approximate "attempted" count for every asset,
                //not just the ones that happened to load successfully. (nValue <= 0 is a stage reset, not a file.)
                void OnFileAttempted(int nValue) { if (nValue > 0) { tReport.RecordProcessed(); PipelineProgress.processed = tReport.Processed; } }
                DCProgress.ValueChanged += OnFileAttempted;

                Model[] tModels;
                TIM2Image[] tTm2Images;
                TIM2Image[] tImgImages;
                try
                {
                    tModels = MDS.LoadDirectory(szGameDirectory);
                    foreach (Model tModel in tModels)
                    {
                        PipelineProgress.currentAsset = tModel.Name;
                        PipelineProgress.currentFile = Path.GetFileName(tModel.filePath);

                        PerformSaveSmd(tModel);
                        PerformSaveSObj(tModel);
                    }

                    tTm2Images = TM2.LoadDirectory(szGameDirectory);
                    SavePngs(tTm2Images, tReport);

                    tImgImages = IMG.LoadDirectory(szGameDirectory);
                    SavePngs(tImgImages, tReport);
                }
                finally
                {
                    DCProgress.ValueChanged -= OnFileAttempted;
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // Analyze - the ORIGINAL parsed MDS models only, never exported OBJ/SMD.
                //////////////////////////////////////////////////////////////////////////////////////////////////
                PipelineProgress.stage = "Analyze";
                List<ModelAnalysis> tAnalyses = new List<ModelAnalysis>();
                foreach (Model tModel in tModels)
                {
                    PipelineProgress.currentAsset = tModel.Name;
                    PipelineProgress.currentFile = Path.GetFileName(tModel.filePath);

                    try
                    {
                        ModelAnalysis tAnalysis = ModelAnalyzer.Analyze(tModel);
                        tAnalyses.Add(tAnalysis);
                        tReport.RecordSuccess(tAnalysis.ModelName, "Analyze", szPath: tModel.filePath);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Failed to analyze model.", szAsset: Path.GetFileName(tModel.filePath), szModel: tModel.Name, szPath: tModel.filePath, tException: ex);
                    }
                }
                tReport.ModelAnalyses.AddRange(tAnalyses);
                ReportWriter.WriteModelsCsv(Path.Combine(szAnalysisDir, "models.csv"), tAnalyses);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                // Organize - copy the extracted/converted results into the Output/ tree (source dir is untouched
                // beyond what Extract/Convert already wrote there, exactly as the legacy flow leaves it).
                //////////////////////////////////////////////////////////////////////////////////////////////////
                PipelineProgress.stage = "Organize";
                DCProgress.name = "Organizing output";
                OrganizeOutputs(szGameDirectory, tAnalyses, szModelsDir, szTexturesDir, szPngDir, szAnimationsDir, szTexturesWithoutModelsDir, szOtherThingsDir, tReport);

                tReport.Success = Math.Max(0, tReport.Processed - tReport.Failed);
            }
            catch (Exception ex)
            {
                //A safety net: none of Extract/Convert/Analyze/Organize are expected to throw (their own internals
                //already catch and log per-asset failures), but if something unexpected still escapes, the run
                //stops early rather than crashing the host - whatever was accomplished is kept and still reported.
                Logger.Error("Unexpected pipeline failure; run stopped early. Partial results and reports are still written.", tException: ex);
            }
            finally
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////
                // Report - always written, even if something above threw, so a failed run is still diagnosable.
                //////////////////////////////////////////////////////////////////////////////////////////////////
                PipelineProgress.stage = "Report";
                tReport.Finish();
                ReportWriter.WriteAll(szReportsDir, tReport);
            }

            PipelineProgress.stage = "Finished";
            Logger.Info($"Analyze + Extract All finished. Processed={tReport.Processed} Success={tReport.Success} Failed={tReport.Failed} Warnings={tReport.Warnings} Errors={tReport.Errors}", szPath: szOutputDirectory);
        }

        /// <summary>
        /// Mirrors the legacy PerformSaveSMD behavior (see DCExtractorForm.Operations.cs / CLI Program.cs), wrapped
        /// so a single model's export failure is logged and recorded but doesn't stop the run.
        /// </summary>
        static void PerformSaveSmd(Model tModel)
        {
            try
            {
                if (Settings.modelConversion == Settings.ModelConversionSetting.OneModel)
                {
                    SMD.Save(tModel);
                    if (Settings.exportAnimations)
                        SMD.SaveAnimations(tModel);
                }
                else
                {
                    Model[] tSplitModels = tModel.Split();
                    foreach (Model tSplitModel in tSplitModels)
                        SMD.Save(tSplitModel);
                    if (Settings.exportAnimations)
                        SMD.SaveAnimations(tModel);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to convert model to SMD.", szAsset: Path.GetFileName(tModel.filePath), szModel: tModel.Name, szPath: tModel.filePath, tException: ex);
            }
        }

        /// <summary>
        /// Mirrors the legacy PerformSaveSOBJ behavior (see DCExtractorForm.Operations.cs / CLI Program.cs), wrapped
        /// so a single model's export failure is logged and recorded but doesn't stop the run.
        /// </summary>
        static void PerformSaveSObj(Model tModel)
        {
            try
            {
                if (Settings.modelConversion == Settings.ModelConversionSetting.OneModel)
                {
                    WavefrontOBJ.Save(tModel);
                }
                else
                {
                    Model[] tSplitModels = tModel.Split();
                    foreach (Model tSplitModel in tSplitModels)
                        WavefrontOBJ.Save(tSplitModel);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to convert model to SMD.", szAsset: Path.GetFileName(tModel.filePath), szModel: tModel.Name, szPath: tModel.filePath, tException: ex);
            }
        }

        static void SavePngs(TIM2Image[] tImages, RunReport tReport)
        {
            foreach (TIM2Image tImage in tImages)
            {
                PipelineProgress.currentAsset = Path.GetFileNameWithoutExtension(tImage.filePath);
                PipelineProgress.currentFile = Path.GetFileName(tImage.filePath);
                PipelineProgress.processed++;
                tReport.RecordProcessed();

                try
                {
                    ImageHelpers.SavePNG(tImage);
                }
                catch (Exception ex)
                {
                    //ImageHelpers.SavePNG already logs/catches internally; this is a last-resort net in case a
                    //future change reintroduces a throw, so one bad texture still can't abort the whole batch.
                    Logger.Error("Failed to convert texture to PNG.", szAsset: Path.GetFileName(tImage.filePath), szTexture: Path.GetFileNameWithoutExtension(tImage.filePath), szPath: tImage.filePath, tException: ex);
                }
            }
        }

        /// <summary>
        /// Copies extracted/converted files from the game directory into the Output/ tree, sorting textures into
        /// PNG (matches a known model material) vs TexturesWithoutModels (doesn't), and anything left over with an
        /// unrecognized extension into OtherThings (recorded as unsupported).
        /// </summary>
        static void OrganizeOutputs(string szGameDirectory, List<ModelAnalysis> tAnalyses, string szModelsDir, string szTexturesDir, string szPngDir, string szAnimationsDir, string szTexturesWithoutModelsDir, string szOtherThingsDir, RunReport tReport)
        {
            HashSet<string> tMaterialNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            //Material names aren't part of ModelAnalysis (which is a stats summary), so re-derive them cheaply
            //from the model file names themselves isn't possible here; instead we accept any PNG whose base name
            //(minus a trailing _0/_1/etc mip/subimage suffix) matches a model's own name as a practical proxy.
            foreach (ModelAnalysis tAnalysis in tAnalyses)
                tMaterialNames.Add(tAnalysis.ModelName);

            foreach (string szFile in Directory.GetFiles(szGameDirectory, "*", SearchOption.AllDirectories))
            {
                string szExtension = Path.GetExtension(szFile);
                string szRelative = Path.GetRelativePath(szGameDirectory, szFile);

                try
                {
                    if (string.Equals(szExtension, ".smd", StringComparison.OrdinalIgnoreCase))
                    {
                        bool bIsAnimation = szRelative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Any(p => string.Equals(p, "anims", StringComparison.OrdinalIgnoreCase));
                        CopyInto(szFile, bIsAnimation ? szAnimationsDir : szModelsDir, szRelative);
                    }
                    else if (string.Equals(szExtension, ".obj", StringComparison.OrdinalIgnoreCase))
                    {
                        CopyInto(szFile, szModelsDir, szRelative);
                    }
                    else if (string.Equals(szExtension, ".tm2", StringComparison.OrdinalIgnoreCase))
                    {
                        CopyInto(szFile, szTexturesDir, szRelative);
                    }
                    else if (string.Equals(szExtension, ".png", StringComparison.OrdinalIgnoreCase))
                    {
                        string szBaseName = StripPngSuffix(Path.GetFileNameWithoutExtension(szFile));
                        bool bMatchesModel = tMaterialNames.Contains(szBaseName);
                        CopyInto(szFile, bMatchesModel ? szPngDir : szTexturesWithoutModelsDir, szRelative);
                    }
                    else if (s_szKnownExtensions.Contains(szExtension) == false)
                    {
                        CopyInto(szFile, szOtherThingsDir, szRelative);
                        tReport.RecordUnsupported(Path.GetFileName(szFile), "Organize", "No known asset category for extension '" + szExtension + "'.", szFile);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to organize extracted file into Output.", szAsset: Path.GetFileName(szFile), szFile: Path.GetFileName(szFile), szPath: szFile, tException: ex);
                }
            }
        }

        static string StripPngSuffix(string szBaseName)
        {
            int nUnderscore = szBaseName.LastIndexOf('_');
            if (nUnderscore > 0 && int.TryParse(szBaseName.Substring(nUnderscore + 1), out _))
                return szBaseName.Substring(0, nUnderscore);
            return szBaseName;
        }

        static void CopyInto(string szSourceFile, string szDestinationRoot, string szRelativePath)
        {
            string szDestinationPath = Path.Combine(szDestinationRoot, szRelativePath);
            string szDestinationDir = Path.GetDirectoryName(szDestinationPath);
            if (string.IsNullOrEmpty(szDestinationDir) == false)
                Directory.CreateDirectory(szDestinationDir);

            File.Copy(szSourceFile, szDestinationPath, true);
        }
    }
}
