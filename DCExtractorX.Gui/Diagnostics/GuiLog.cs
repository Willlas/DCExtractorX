using Custom.Diagnostics;

namespace DCExtractorX.Gui.Diagnostics;

/// <summary>
/// Class   :   "GuiLog"
///
/// Purpose :   routes Core diagnostics (Custom.Diagnostics.ILog) to a MessageBox, preserving the legacy
/// WinForms popup behavior that used to live directly inside the parser/exporter code.
/// </summary>
sealed class GuiLog : ILog
{
    public void Warn(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public void Error(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
