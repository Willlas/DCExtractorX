using DC;

namespace DCExtractorX.Gui.Forms;

/// <summary>
/// Class   :   "SettingsForm"
///
/// Purpose :   lets the user view/edit the DC.Settings values used by model conversion and warning behavior.
/// GUI-side port of the legacy DCExtractor.Forms.SettingsForm, simplified to edit DC.Settings directly and
/// persist via SettingsStore (rather than the legacy SettingField control array + settings.txt roundtrip).
/// </summary>
partial class SettingsForm : Form
{
    public SettingsForm()
    {
        InitializeComponent();

        CB_modelConversion.Items.Add(Settings.ModelConversionSetting.OneModel);
        CB_modelConversion.Items.Add(Settings.ModelConversionSetting.ManyModels);
        CB_modelConversion.SelectedItem = Settings.modelConversion;

        CH_exportAnimations.Checked = Settings.exportAnimations;
        CH_showMDSWarnings.Checked = Settings.showMDSWarnings;
        CH_showFileExtensionWarnings.Checked = Settings.showFileExtensionWarnings;
    }

    void BT_ok_Click(object? sender, EventArgs e)
    {
        Settings.modelConversion = (Settings.ModelConversionSetting)CB_modelConversion.SelectedItem!;
        Settings.exportAnimations = CH_exportAnimations.Checked;
        Settings.showMDSWarnings = CH_showMDSWarnings.Checked;
        Settings.showFileExtensionWarnings = CH_showFileExtensionWarnings.Checked;

        SettingsStore.Save();

        DialogResult = DialogResult.OK;
        Close();
    }

    void BT_cancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
