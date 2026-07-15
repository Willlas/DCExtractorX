using Custom.IO;

namespace DCExtractorX.Gui.Forms;

/// <summary>
/// Class   :   "GuiFileHelpers"
///
/// Purpose :   batch file move/delete operations used by the "Extract &amp; Convert Everything" pipeline. These are
/// GUI-shell concerns (they show a conflict dialog / message box) which Core intentionally does not include - see
/// DCExtractorX.Core.Source.Data.FileHelpers's remarks. GUI-side port of the legacy DCExtractor.Data.FileHelpers.MoveFiles/DeleteFiles.
/// </summary>
static class GuiFileHelpers
{
    /// <summary>
    /// Moves all files matching the given filters from the source directory to the destination directory,
    /// preserving the folder hierarchy. Prompts on conflicts, and optionally offers to delete the moved-from
    /// files afterward.
    /// </summary>
    public static void MoveFiles(string szSourceDir, string szDestinationDir, FileDialogFilter[] tFilters, bool bDeleteSource, FileDialogFilter[] tDeleteFilters)
    {
        if (Directory.Exists(szSourceDir) == false || tFilters == null || tFilters.Length == 0)
            return;

        List<string> tFilesToMove = [];
        foreach (FileDialogFilter tFilter in tFilters)
            tFilesToMove.AddRange(Directory.GetFiles(szSourceDir, "*" + tFilter.Extensions[0], SearchOption.AllDirectories));

        FileConflictDialog.FileConflictDialogResult eAction = FileConflictDialog.FileConflictDialogResult.None;
        using FileConflictDialog tDialog = new();

        foreach (string szSourceFile in tFilesToMove)
        {
            if (eAction != FileConflictDialog.FileConflictDialogResult.YesToAll &&
                eAction != FileConflictDialog.FileConflictDialogResult.NoToAll)
                eAction = FileConflictDialog.FileConflictDialogResult.None;

            string szSubPath = PathHelpers.SubPathOf(szSourceDir, Path.GetDirectoryName(szSourceFile) ?? szSourceDir, false);
            string szNewPath = Path.Combine(szDestinationDir, szSubPath, Path.GetFileName(szSourceFile));

            if (File.Exists(szNewPath))
            {
                if (eAction != FileConflictDialog.FileConflictDialogResult.YesToAll &&
                    eAction != FileConflictDialog.FileConflictDialogResult.NoToAll)
                {
                    tDialog.SourceFile = szSourceFile;
                    tDialog.DestinationFile = szNewPath;
                    eAction = tDialog.ShowConflictDialog();
                }

                if (eAction == FileConflictDialog.FileConflictDialogResult.No ||
                    eAction == FileConflictDialog.FileConflictDialogResult.NoToAll)
                    continue;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(szNewPath)!);

            try
            {
                File.Copy(szSourceFile, szNewPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "File Copy Error");
            }
        }

        if (bDeleteSource &&
            MessageBox.Show(
                "Would you like to delete the extracted files (.smd, .png) in the source directory to save space? (Original pak/mds/img files will be preserved, destination smd/png files will be preserved)",
                "Should I Delete?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            DeleteFiles(szSourceDir, tDeleteFilters);
    }

    static void DeleteFiles(string szDirectory, FileDialogFilter[] tDeleteFilters)
    {
        List<string> tFilesToDelete = [];
        foreach (FileDialogFilter tFilter in tDeleteFilters)
            tFilesToDelete.AddRange(Directory.GetFiles(szDirectory, "*" + tFilter.Extensions[0], SearchOption.AllDirectories));

        foreach (string szFile in tFilesToDelete)
            File.Delete(szFile);
    }
}
