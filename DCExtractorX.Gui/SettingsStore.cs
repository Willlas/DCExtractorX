using DC;

namespace DCExtractorX.Gui;

/// <summary>
/// Class   :   "SettingsStore"
///
/// Purpose :   loads/saves the headless DC.Settings values to a "settings.txt" file next to the executable.
/// This replaces the legacy DCExtractor.Settings.LoadSettings/SaveSettings, which Core intentionally dropped
/// since persistence is a host (GUI/CLI) concern.
/// </summary>
static class SettingsStore
{
    static string FilePath => Path.Combine(AppContext.BaseDirectory, "settings.txt");

    /// <summary>
    /// Loads settings from disk into DC.Settings, if the settings file exists. Missing/invalid entries are left at their defaults.
    /// </summary>
    public static void Load()
    {
        string szPath = FilePath;
        if (File.Exists(szPath) == false)
            return;

        foreach (string szLine in File.ReadAllLines(szPath))
        {
            int nSeparator = szLine.IndexOf('=');
            if (nSeparator < 0)
                continue;

            string szKey = szLine[..nSeparator].Trim();
            string szValue = szLine[(nSeparator + 1)..].Trim();

            switch (szKey)
            {
                case nameof(Settings.modelConversion):
                    if (Enum.TryParse(szValue, out Settings.ModelConversionSetting eConversion))
                        Settings.modelConversion = eConversion;
                    break;
                case nameof(Settings.exportAnimations):
                    if (bool.TryParse(szValue, out bool bExportAnimations))
                        Settings.exportAnimations = bExportAnimations;
                    break;
                case nameof(Settings.showMDSWarnings):
                    if (bool.TryParse(szValue, out bool bShowMDSWarnings))
                        Settings.showMDSWarnings = bShowMDSWarnings;
                    break;
                case nameof(Settings.showFileExtensionWarnings):
                    if (bool.TryParse(szValue, out bool bShowFileExtensionWarnings))
                        Settings.showFileExtensionWarnings = bShowFileExtensionWarnings;
                    break;
            }
        }
    }

    /// <summary>
    /// Saves the current DC.Settings values to disk.
    /// </summary>
    public static void Save()
    {
        string[] szLines =
        [
            $"{nameof(Settings.modelConversion)}={Settings.modelConversion}",
            $"{nameof(Settings.exportAnimations)}={Settings.exportAnimations}",
            $"{nameof(Settings.showMDSWarnings)}={Settings.showMDSWarnings}",
            $"{nameof(Settings.showFileExtensionWarnings)}={Settings.showFileExtensionWarnings}",
        ];

        File.WriteAllLines(FilePath, szLines);
    }
}
