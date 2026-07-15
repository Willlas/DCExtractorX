namespace DCExtractorX.Gui.Forms;

/// <summary>
/// Class   :   "FileDialogFilter"
///
/// Purpose :   a small helper for generating Windows file dialog filter strings. GUI-side port of the legacy
/// Custom.Forms.FileDialogHelpers.FileDialogFilter, trimmed to what the GUI needs.
/// </summary>
sealed class FileDialogFilter
{
    string m_szName = "All";
    string[] m_szExtensions = ["*"];

    /// <summary>
    /// Gets a dialog filter that corresponds to all files.
    /// </summary>
    public static FileDialogFilter AllFiles => new();

    /// <summary>
    /// Gets the extensions associated with this file type (without the leading dot).
    /// </summary>
    public string[] Extensions => m_szExtensions;

    public FileDialogFilter()
    {
    }

    public FileDialogFilter(string szName, string szExtension)
    {
        m_szName = szName;
        m_szExtensions = [szExtension];
        Verify();
    }

    public FileDialogFilter(string szName, string[] szExtensions)
    {
        m_szName = szName;
        m_szExtensions = szExtensions;
        Verify();
    }

    void Verify()
    {
        for (int i = 0; i < m_szExtensions.Length; i++)
        {
            if (m_szExtensions[i].StartsWith('.'))
                m_szExtensions[i] = m_szExtensions[i][1..];
        }
    }

    public override string ToString()
    {
        //All Graphics Types (*.bmp,*.jpg)|*.bmp;*.jpg
        string szResult = m_szName + " (";
        for (int i = 0; i < m_szExtensions.Length; i++)
        {
            szResult += "*." + m_szExtensions[i];
            if (i < m_szExtensions.Length - 1)
                szResult += ",";
        }
        szResult += ")|";
        for (int i = 0; i < m_szExtensions.Length; i++)
        {
            szResult += "*." + m_szExtensions[i];
            if (i < m_szExtensions.Length - 1)
                szResult += ";";
        }
        return szResult;
    }

    /// <summary>
    /// Generates a filter string from a list of filters, joined for use with OpenFileDialog.Filter.
    /// </summary>
    public static string GenerateFilterString(FileDialogFilter[] tFilters)
    {
        return string.Join('|', (object[])tFilters);
    }

    /// <summary>
    /// Combines a list of filters into one overarching filter with a new display name.
    /// </summary>
    public static FileDialogFilter CombineFilters(string szNewName, FileDialogFilter[] tFilters)
    {
        List<string> tExtensions = [];
        foreach (FileDialogFilter tFilter in tFilters)
        {
            foreach (string szExtension in tFilter.Extensions)
            {
                if (tExtensions.Contains(szExtension) == false)
                    tExtensions.Add(szExtension);
            }
        }

        return new FileDialogFilter(szNewName, tExtensions.ToArray());
    }
}

/// <summary>
/// Class   :   "FileDialogHelpers"
///
/// Purpose :   thin wrappers around OpenFileDialog/OpenFolderDialog that remember the last-used directory.
/// GUI-side port of the legacy Custom.Forms.FileDialogHelpers static functions.
/// </summary>
static class FileDialogHelpers
{
    public static string CurrentWorkingDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Opens a file dialog and collects a selection from the user.
    /// </summary>
    public static bool ShowOpenFileDialog(string szFilter, string szTitle, out string szPath)
    {
        using OpenFileDialog tOpen = new()
        {
            Title = szTitle,
            Filter = szFilter,
            InitialDirectory = CurrentWorkingDirectory,
        };

        if (tOpen.ShowDialog() == DialogResult.OK)
        {
            szPath = tOpen.FileName;
            CurrentWorkingDirectory = Path.GetDirectoryName(szPath) ?? string.Empty;
            return true;
        }

        szPath = string.Empty;
        return false;
    }

    /// <summary>
    /// Opens a folder dialog and collects a selection from the user. Uses the classic FolderBrowserDialog,
    /// which already renders as the modern Vista-style picker on Windows via .NET Core 3.1+ (no extra NuGet
    /// dependency needed; System.Windows.Forms.OpenFolderDialog is not available on this SDK).
    /// </summary>
    public static bool ShowOpenFolderDialog(string szTitle, out string szPath)
    {
        using FolderBrowserDialog tOpen = new()
        {
            Description = szTitle,
            UseDescriptionForTitle = true,
            SelectedPath = CurrentWorkingDirectory,
        };

        if (tOpen.ShowDialog() == DialogResult.OK)
        {
            szPath = tOpen.SelectedPath;
            CurrentWorkingDirectory = szPath;
            return true;
        }

        szPath = string.Empty;
        return false;
    }
}
