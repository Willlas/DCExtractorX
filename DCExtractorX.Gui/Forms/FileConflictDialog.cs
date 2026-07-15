using System.ComponentModel;

namespace DCExtractorX.Gui.Forms;

/// <summary>
/// Class   :   "FileConflictDialog"
///
/// Purpose :   a small prompt shown when a file move would overwrite an existing file at the destination.
/// GUI-side port of the legacy DCExtractor.Forms.FileConflictDialog.
/// </summary>
partial class FileConflictDialog : Form
{
    public enum FileConflictDialogResult { None, Yes, No, YesToAll, NoToAll }

    FileConflictDialogResult m_eDialogResult = FileConflictDialogResult.None;

    //Not designer-bound (this form is hand-written, not built with the visual designer).
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string SourceFile
    {
        get => LB_sourceFile.Text;
        set => LB_sourceFile.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DestinationFile
    {
        get => LB_destinationFile.Text;
        set => LB_destinationFile.Text = value;
    }

    public FileConflictDialog()
    {
        InitializeComponent();
    }

    public FileConflictDialogResult ShowConflictDialog()
    {
        m_eDialogResult = FileConflictDialogResult.None;
        ShowDialog();
        return m_eDialogResult;
    }

    void BT_yes_Click(object? sender, EventArgs e)
    {
        m_eDialogResult = FileConflictDialogResult.Yes;
        Close();
    }

    void BT_no_Click(object? sender, EventArgs e)
    {
        m_eDialogResult = FileConflictDialogResult.No;
        Close();
    }

    void BT_yesToAll_Click(object? sender, EventArgs e)
    {
        m_eDialogResult = FileConflictDialogResult.YesToAll;
        Close();
    }

    void BT_noToAll_Click(object? sender, EventArgs e)
    {
        m_eDialogResult = FileConflictDialogResult.NoToAll;
        Close();
    }
}
