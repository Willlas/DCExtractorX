using DCExtractorX.Gui.Forms;

namespace DCExtractorX.Gui;

/// <summary>
/// Class   :   "DCExtractorForm"
///
/// Purpose :   a form that displays various extraction/conversion operations for Level-5 file formats, backed by
/// the headless DCExtractorX.Core library. GUI-side port of the legacy DCExtractor.DCExtractorForm.
///
/// Most of the heavy lifting is done by DCExtractorX.Core; this form only handles menu clicks, file dialogs, and
/// marshaling DC.IO.DCProgress updates onto the UI thread. Operation bodies live in DCExtractorForm.Operations.cs.
/// </summary>
public partial class DCExtractorForm : Form
{
    //data.
    readonly FileDialogFilter DATFormat = new("Level-5 Data", ".dat");
    readonly FileDialogFilter HD2Format = new("Level-5 HD2 Data", ".hd2");
    readonly FileDialogFilter HD3Format = new("Level-5 HD3 Data", ".hd3");
    string HD_AllTypesFilter = string.Empty;

    //packs.
    readonly FileDialogFilter PAKFormat = new("Level-5 Pack File", ".pak");
    readonly FileDialogFilter CHRFormat = new("Level-5 Character", ".chr");
    readonly FileDialogFilter IPKFormat = new("Level-5 Image-Pack", ".ipk");
    readonly FileDialogFilter MPKFormat = new("Level-5 Model-Pack", ".mpk");
    readonly FileDialogFilter PCPFormat = new("Level-5 PCP", ".pcp");
    readonly FileDialogFilter EFPFormat = new("Level-5 Effect Pack", ".efp");
    readonly FileDialogFilter SNDFormat = new("Level-5 Sound", ".snd");
    readonly FileDialogFilter SKYFormat = new("Level-5 Skybox", ".sky");
    FileDialogFilter[] PAK_TypeList = [];
    string PAK_AllTypesFilter = string.Empty;

    //assets.
    readonly FileDialogFilter MDSFormat = new("Level-5 Model", ".mds");
    readonly FileDialogFilter TM2Format = new("PS2 TIM2 Image", ".tm2");
    readonly FileDialogFilter IMGFormat = new("Level-5 Image", ".img");

    //exports (used by the Extract & Convert Everything move step).
    readonly FileDialogFilter SMDFormat = new("Studiomdl Data", ".smd");
    readonly FileDialogFilter PNGFormat = new("PNG Image", ".png");
    readonly FileDialogFilter CFGFormat = new("Configuration", ".cfg");

    bool m_bIsClosing;

    public DCExtractorForm()
    {
        InitializeComponent();

        ToggleButtons(true);

        FileDialogHelpers.CurrentWorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty;

        BuildFilters();

        FormClosed += (_, _) =>
        {
            m_bIsClosing = true;
            SettingsStore.Save();
        };
    }

    /// <summary>
    /// Builds the filter strings for the application.
    /// </summary>
    void BuildFilters()
    {
        HD_AllTypesFilter = FileDialogFilter.GenerateFilterString(
        [
            FileDialogFilter.CombineFilters("HD2/HD3 Files", [HD2Format, HD3Format]),
            HD2Format,
            HD3Format,
            FileDialogFilter.AllFiles,
        ]);

        PAK_TypeList = [PAKFormat, CHRFormat, IPKFormat, MPKFormat, PCPFormat, EFPFormat, SNDFormat, SKYFormat];

        PAK_AllTypesFilter = FileDialogFilter.GenerateFilterString(
        [
            FileDialogFilter.CombineFilters("PAK-Type Files", PAK_TypeList),
            .. PAK_TypeList,
            FileDialogFilter.AllFiles,
        ]);
    }

    /// <summary>
    /// Toggles the enabled state of the operation-triggering controls.
    /// </summary>
    void ToggleButtons(bool bState)
    {
        menuStrip1.Enabled = bState;
        BT_extractAndConvertAll.Enabled = bState;
        BT_cancelWork.Enabled = !bState;
    }

    void BT_close_Click(object? sender, EventArgs e)
    {
        Close();
    }

    void BT_settings_Click(object? sender, EventArgs e)
    {
        using SettingsForm tForm = new();
        tForm.ShowDialog(this);
    }
}
