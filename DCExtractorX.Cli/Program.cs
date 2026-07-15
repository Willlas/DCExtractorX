using Custom.Diagnostics;
using DC;
using DC.IO;
using DC.Pipeline;
using DC.Reporting;
using DC.Types;
using System;
using System.IO;

namespace DCExtractorX.Cli
{
    /// <summary>
    /// Class   :   "ConsoleLog"
    /// 
    /// Purpose :   routes Core diagnostics (Custom.Diagnostics.ILog) to the console instead of a WinForms MessageBox.
    /// This is the CLI's log sink; the GUI host would instead wire Log.Current to a MessageBox-based sink to
    /// preserve the legacy popup behavior.
    /// </summary>
    sealed class ConsoleLog : ILog
    {
        public void Warn(string message, string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] {title}: {message}");
            Console.ResetColor();
        }

        public void Error(string message, string title)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {title}: {message}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Class   :   "Program"
    /// 
    /// Purpose :   a thin, headless command-line front-end over DCExtractorX.Core. Each subcommand maps 1:1 to one
    /// of the 12 operations that used to be wired to menu items/BackgroundWorkers in the legacy WinForms app
    /// (see DCExtractor_Operations.cs in the original repo). No behavior beyond that mapping is added here.
    /// </summary>
    static class Program
    {
        static int Main(string[] args)
        {
            Logger.Init();
            Log.Current = new LoggingLog(new ConsoleLog());
            DCProgress.NameChanged += szName => Console.WriteLine(szName);

            if (args.Length == 0)
            {
                PrintUsage();
                return 1;
            }

            string szCommand = args[0].ToLowerInvariant();
            string[] szRest = new string[args.Length - 1];
            Array.Copy(args, 1, szRest, 0, szRest.Length);

            try
            {
                switch (szCommand)
                {
                    case "extract-dat": return ExtractDat(szRest);
                    case "extract-pak": return ExtractPak(szRest);
                    case "extract-pak-dir": return ExtractPakDirectory(szRest);
                    case "analyze-extract-all": return AnalyzeExtractAll(szRest);
                    case "mds2obj": return ConvertMdsToObj(szRest);
                    case "mds2obj-dir": return ConvertMdsToObjDirectory(szRest);
                    case "mds2smd": return ConvertMdsToSmd(szRest);
                    case "mds2smd-dir": return ConvertMdsToSmdDirectory(szRest);
                    case "img2png": return ConvertImgToPng(szRest);
                    case "img2png-dir": return ConvertImgToPngDirectory(szRest);
                    case "tm22png": return ConvertTm2ToPng(szRest);
                    case "tm22png-dir": return ConvertTm2ToPngDirectory(szRest);
                    case "-h":
                    case "--help":
                    case "help":
                        PrintUsage();
                        return 0;
                    default:
                        Console.Error.WriteLine($"Unknown command: {szCommand}");
                        PrintUsage();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Unhandled error: " + ex.Message);
                return 1;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("DCExtractorX CLI - headless unpacker/converter for PS2-era Level-5 game assets.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dcx extract-dat <path.dat> <path.hd2|path.hd3> <outDir>");
            Console.WriteLine("  dcx extract-pak <path.pak|.chr|.efp|.ipk|.mpk|.pcp|.sky|.snd>");
            Console.WriteLine("  dcx extract-pak-dir <dir>");
            Console.WriteLine("  dcx analyze-extract-all <gameDir> <outDir>   Scan -> Extract -> Convert -> Analyze -> Report, in one pass.");
            Console.WriteLine("                                                <gameDir> must contain DATA.DAT + DATA.HD2/DATA.HD3 at its root.");
            Console.WriteLine("  dcx mds2obj <path.mds>");
            Console.WriteLine("  dcx mds2obj-dir <dir>");
            Console.WriteLine("  dcx mds2smd <path.mds> [--split] [--anim]");
            Console.WriteLine("  dcx mds2smd-dir <dir> [--split] [--anim]");
            Console.WriteLine("  dcx img2png <path.img>");
            Console.WriteLine("  dcx img2png-dir <dir>");
            Console.WriteLine("  dcx tm22png <path.tm2>");
            Console.WriteLine("  dcx tm22png-dir <dir>");
            Console.WriteLine();
            Console.WriteLine("  --split   Export each mesh in a model as a separate .smd file (default: one combined model).");
            Console.WriteLine("  --anim    Also export .mot animations to an 'anims' subfolder. KNOWN BROKEN: bind-pose rotations");
            Console.WriteLine("            are incorrect for animated output (see docs/known-issues.md). Off by default.");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Extraction.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static int ExtractDat(string[] args)
        {
            if (args.Length < 3)
            {
                Console.Error.WriteLine("Usage: extract-dat <path.dat> <path.hd2|path.hd3> <outDir>");
                return 1;
            }

            string szDatPath = args[0];
            string szHdPath = args[1];
            string szOutDir = args[2];

            DAT.HDType eType;
            if (szHdPath.ToLowerInvariant().EndsWith(".hd2"))
                eType = DAT.HDType.Two;
            else if (szHdPath.ToLowerInvariant().EndsWith(".hd3"))
                eType = DAT.HDType.Three;
            else
            {
                Console.Error.WriteLine("The HD file must have a .hd2 or .hd3 extension.");
                return 1;
            }

            DAT.ExtractHD(szDatPath, szHdPath, szOutDir, eType);
            return 0;
        }

        static int ExtractPak(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: extract-pak <path>");
                return 1;
            }

            PAK.ExtractFile(args[0]);
            return 0;
        }

        // All 8 pack-family extensions use the same generic PAK parser (CONFIRMED BY CODE in PAK.cs).
        static readonly string[] PakExtensions = { ".pak", ".chr", ".ipk", ".mpk", ".pcp", ".efp", ".snd", ".sky" };

        static int ExtractPakDirectory(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: extract-pak-dir <dir>");
                return 1;
            }

            PAK.ExtractDirectory(args[0], PakExtensions);
            return 0;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Analyze + Extract All.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Runs the full "Analyze + Extract All" pipeline (Scan -&gt; Extract -&gt; Convert -&gt; Analyze -&gt; Report), printing
        /// live progress as each asset is processed. Individual asset failures are logged/recorded but never stop
        /// the run; only invalid arguments or a missing/unreadable root archive cause a non-zero exit here.
        /// </summary>
        static int AnalyzeExtractAll(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: analyze-extract-all <gameDir> <outDir>");
                return 1;
            }

            int nMaximum = 1;
            DCProgress.MaximumChanged += nValue => nMaximum = Math.Max(1, nValue);

            void PrintProgress(string szAsset)
            {
                if (string.IsNullOrEmpty(szAsset))
                    return;

                Console.WriteLine();
                Console.WriteLine($"[{PipelineProgress.processed}/{nMaximum}]");
                Console.WriteLine($"Asset={szAsset}");
                Console.WriteLine($"Stage={PipelineProgress.stage}");
                Console.WriteLine($"Warnings={PipelineProgress.warnings}");
                Console.WriteLine($"Errors={PipelineProgress.errors}");
            }

            PipelineProgress.CurrentAssetChanged += PrintProgress;

            try
            {
                RunReport tReport = AnalyzeExtractPipeline.Run(args[0], args[1]);

                Console.WriteLine();
                Console.WriteLine("=== Analyze + Extract All - FINISHED ===");
                Console.WriteLine($"Processed={tReport.Processed} Success={tReport.Success} Failed={tReport.Failed} Unsupported={tReport.Unsupported} Warnings={tReport.Warnings} Errors={tReport.Errors}");
                Console.WriteLine("Reports: " + Path.Combine(args[1], "Reports"));
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Fatal: " + ex.Message);
                return 1;
            }
            finally
            {
                PipelineProgress.CurrentAssetChanged -= PrintProgress;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Model conversion.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static int ConvertMdsToObj(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: mds2obj <path.mds>");
                return 1;
            }

            if (MDS.Load(args[0], out Model tModel, false))
                WavefrontOBJ.Save(tModel);
            return 0;
        }

        static int ConvertMdsToObjDirectory(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: mds2obj-dir <dir>");
                return 1;
            }

            Model[] tModels = MDS.LoadDirectory(args[0]);
            for (int i = 0; i < tModels.Length; i++)
                WavefrontOBJ.Save(tModels[i]);
            return 0;
        }

        static int ConvertMdsToSmd(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: mds2smd <path.mds> [--split] [--anim]");
                return 1;
            }

            ApplyModelConversionFlags(args);

            if (MDS.Load(args[0], out Model tModel, false))
                PerformSaveSmd(tModel);
            return 0;
        }

        static int ConvertMdsToSmdDirectory(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: mds2smd-dir <dir> [--split] [--anim]");
                return 1;
            }

            ApplyModelConversionFlags(args);

            Model[] tModels = MDS.LoadDirectory(args[0]);
            for (int i = 0; i < tModels.Length; i++)
                PerformSaveSmd(tModels[i]);
            return 0;
        }

        static void ApplyModelConversionFlags(string[] args)
        {
            Settings.modelConversion = Settings.ModelConversionSetting.OneModel;
            Settings.exportAnimations = false;
            foreach (string szArg in args)
            {
                if (string.Equals(szArg, "--split", StringComparison.OrdinalIgnoreCase))
                    Settings.modelConversion = Settings.ModelConversionSetting.ManyModels;
                else if (string.Equals(szArg, "--anim", StringComparison.OrdinalIgnoreCase))
                    Settings.exportAnimations = true;
            }
        }

        /// <summary>
        /// Mirrors the legacy DCExtractorForm.PerformSaveSMD behavior exactly (see original DCExtractor.cs).
        /// </summary>
        static void PerformSaveSmd(Model tModel)
        {
            if (Settings.modelConversion == Settings.ModelConversionSetting.OneModel)
            {
                SMD.Save(tModel);
                if (Settings.exportAnimations)
                    SMD.SaveAnimations(tModel);
            }
            else
            {
                Model[] tModels = tModel.Split();
                for (int i = 0; i < tModels.Length; i++)
                    SMD.Save(tModels[i]);
                if (Settings.exportAnimations)
                    SMD.SaveAnimations(tModel);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Texture conversion.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static int ConvertImgToPng(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: img2png <path.img>");
                return 1;
            }

            if (IMG.Load(args[0], out TIM2Image[] tImages))
            {
                for (int i = 0; i < tImages.Length; i++)
                    ImageHelpers.SavePNG(tImages[i]);
            }
            return 0;
        }

        static int ConvertImgToPngDirectory(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: img2png-dir <dir>");
                return 1;
            }

            TIM2Image[] tImages = IMG.LoadDirectory(args[0]);
            SavePngs(tImages);
            return 0;
        }

        static int ConvertTm2ToPng(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: tm22png <path.tm2>");
                return 1;
            }

            TIM2Image tImage = TM2.Load(args[0]);
            if (tImage != null)
                ImageHelpers.SavePNG(tImage);
            return 0;
        }

        static int ConvertTm2ToPngDirectory(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: tm22png-dir <dir>");
                return 1;
            }

            TIM2Image[] tImages = TM2.LoadDirectory(args[0]);
            SavePngs(tImages);
            return 0;
        }

        static void SavePngs(TIM2Image[] tImages)
        {
            for (int i = 0; i < tImages.Length; i++)
                ImageHelpers.SavePNG(tImages[i]);
        }
    }
}
