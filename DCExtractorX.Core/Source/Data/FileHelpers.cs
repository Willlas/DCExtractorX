using System.IO;

namespace DCExtractor.Data
{
    /// <summary>
    /// Class   :   "FileHelpers"
    /// 
    /// Purpose :   contains pure, host-agnostic file/path helpers used by Core exporters.
    /// 
    /// The legacy DCExtractor.Data.FileHelpers class also contained MoveFiles/DeleteFiles batch operations that
    /// depended on WinForms (FileConflictDialog, MessageBox) and File dialog filter types. Those are GUI-shell
    /// concerns and are intentionally not part of Core; only the pure ChangeExtension helper (used by SMD.Save)
    /// is migrated here.
    /// </summary>
    public static class FileHelpers
    {
        /// <summary>
        /// A modified version of the Path.ChangeExtension method that also handles files with no extension.
        /// </summary>
        /// <param name="szFileName">The file name to modify.</param>
        /// <param name="szNewExtension">The new extension to add.</param>
        /// <returns>The file path with a new extension.</returns>
        public static string ChangeExtension(string szFileName, string szNewExtension)
        {
            //If this extension is invalid, we need to correct it.
            if (szNewExtension.Contains(".") == false)
                szNewExtension = "." + szNewExtension;

            //If the original file doesn't have an extension, we run the risk of overwriting it. Let's add on an extension.
            if (Path.HasExtension(szFileName) == false)
                szFileName += szNewExtension;

            //If it did have an extension, there is a chance it's not the one we want. Otherwise, we'll overwrite the original file.
            return Path.ChangeExtension(szFileName, szNewExtension);
        }
    }
}
