namespace Custom.Diagnostics
{
    /// <summary>
    /// Interface   :   "ILog"
    /// 
    /// Purpose     :   an abstraction for reporting warnings/errors from Core parsing/exporting code without
    /// taking a hard dependency on any particular UI framework (WinForms MessageBox, console, file, etc).
    /// 
    /// This replaces the direct System.Windows.Forms.MessageBox.Show calls that used to live inside the
    /// parser/exporter code. Hosts (CLI, GUI) assign Log.Current to a sink appropriate for their environment.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Reports a non-fatal warning (e.g. a suspicious but recoverable condition during parsing).
        /// </summary>
        /// <param name="message">The message to report.</param>
        /// <param name="title">A short title/category for the message.</param>
        void Warn(string message, string title);

        /// <summary>
        /// Reports an error (e.g. a failure that aborts the current operation).
        /// </summary>
        /// <param name="message">The message to report.</param>
        /// <param name="title">A short title/category for the message.</param>
        void Error(string message, string title);
    }

    /// <summary>
    /// Class   :   "NullLog"
    /// 
    /// Purpose :   a silent ILog implementation. This is the default sink so that Core has no visible
    /// side effects unless a host explicitly wires up logging.
    /// </summary>
    public sealed class NullLog : ILog
    {
        public void Warn(string message, string title) { }
        public void Error(string message, string title) { }
    }

    /// <summary>
    /// Class   :   "Log"
    /// 
    /// Purpose :   a global access point for the current ILog sink, mirroring the way DCProgress previously
    /// gave DC.IO code a static, ambient way to report progress without threading a reference through every call.
    /// </summary>
    public static class Log
    {
        static ILog s_tCurrent = new NullLog();

        /// <summary>
        /// Gets/Sets the current log sink. Defaults to a silent NullLog.
        /// </summary>
        public static ILog Current
        {
            get { return s_tCurrent; }
            set { s_tCurrent = value ?? new NullLog(); }
        }
    }
}
