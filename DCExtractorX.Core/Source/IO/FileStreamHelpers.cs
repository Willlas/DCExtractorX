using Custom.Diagnostics;
using System;
using System.IO;

namespace Custom.IO
{
    /// <summary>
    /// Class   :   "FileStreamHelpers"
    /// 
    /// Purpose :   a helpers class for various readers and writer filestream classes.
    /// 
    /// There's not much to say, they just wrap the usual file stream functionality, making the opening of file streams appear less bloated in the functions that use them.
    /// 
    /// Failures are reported through Custom.Diagnostics.Log instead of a direct WinForms MessageBox, so this class
    /// has no UI framework dependency. Hosts (CLI, GUI) assign Log.Current to route these messages appropriately.
    /// </summary>
    public static class FileStreamHelpers
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Opens a binary reader.
        /// </summary>
        /// <param name="szFilePath">The file path for the reader to open.</param>
        /// <returns>The binary reader, otherwise null.</returns>
        public static BinaryReader OpenBinaryReader(string szFilePath)
        {
            BinaryReader tReader;
            try
            {
                tReader = new BinaryReader(new FileStream(szFilePath, FileMode.Open, FileAccess.Read));
            }
            catch (Exception ex)
            {
                Log.Current.Error("OpenBinaryReader: Unable to load input file.\n" + ex.Message, "Exception:OpenBinaryReader");
                return null;
            }
            return tReader;
        }

        /// <summary>
        /// Opens a binary writer.
        /// </summary>
        /// <param name="szFilePath">The file path for the writer to open.</param>
        /// <returns>The binary writer, otherwise null.</returns>
        public static BinaryWriter OpenBinaryWriter(string szFilePath)
        {
            BinaryWriter tWriter;
            try
            {
                tWriter = new BinaryWriter(new FileStream(szFilePath, FileMode.Create, FileAccess.Write));
            }
            catch (Exception ex)
            {
                Log.Current.Error("OpenBinaryWriter: Unable to open output file.\n" + ex.Message, "Exception:OpenBinaryWriter");
                return null;
            }
            return tWriter;
        }

        /// <summary>
        /// Opens a stream reader.
        /// </summary>
        /// <param name="szFilePath">The file path for the reader to open.</param>
        /// <returns>The stream reader, otherwise null.</returns>
        public static StreamReader OpenStreamReader(string szFilePath)
        {
            StreamReader tReader;
            try
            {
                tReader = new StreamReader(new FileStream(szFilePath, FileMode.Open, FileAccess.Read));
            }
            catch (Exception ex)
            {
                Log.Current.Error("OpenStreamReader: Unable to load input file.\n" + ex.Message, "Exception:OpenStreamReader");
                return null;
            }
            return tReader;
        }

        /// <summary>
        /// Opens a stream writer.
        /// </summary>
        /// <param name="szFilePath">The file path for the writer to open.</param>
        /// <returns>The stream writer, otherwise null.</returns>
        public static StreamWriter OpenStreamWriter(string szFilePath)
        {
            StreamWriter tWriter;
            try
            {
                tWriter = new StreamWriter(new FileStream(szFilePath, FileMode.Create, FileAccess.Write));
            }
            catch (Exception ex)
            {
                Log.Current.Error("OpenStreamWriter: Unable to open output file.\n" + ex.Message, "Exception:OpenStreamWriter");
                return null;
            }
            return tWriter;
        }
    }
}
