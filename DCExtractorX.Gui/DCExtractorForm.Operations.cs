using DC;
using DC.IO;
using DC.Types;
using DCExtractorX.Gui.Forms;

namespace DCExtractorX.Gui;

/// <summary>
/// Class   :   "DCExtractorForm_Operations"
///
/// Purpose :   the 12 operations that used to be wired to menu items/BackgroundWorkers in the legacy WinForms app
/// (see DCExtractor_Operations.cs in the original repo), modernized to async/await + CancellationToken instead of
/// DCBackgroundWorker. Each button click validates it isn't already working, prompts for input via file/folder
/// dialogs, then hands off to DC.IO Core calls through RunOperationAsync.
/// </summary>
public partial class DCExtractorForm : Form
{
    CancellationTokenSource? m_tCancellationSource;

    bool isWorking => m_tCancellationSource != null;

    /// <summary>
    /// Runs an operation on a background thread (unless it opts to marshal itself back), wiring DCProgress
    /// events to the progress bar/label and BT_cancelWork to a CancellationToken.
    /// </summary>
    async Task RunOperationAsync(Func<CancellationToken, Task> tOperation)
    {
        if (isWorking)
            return;

        m_tCancellationSource = new CancellationTokenSource();
        CancellationToken tToken = m_tCancellationSource.Token;
        DCProgress.CancelRequested = () => tToken.IsCancellationRequested;

        DCProgress.ValueChanged += OnProgressValueChanged;
        DCProgress.MaximumChanged += OnProgressMaximumChanged;
        DCProgress.NameChanged += OnProgressNameChanged;

        ToggleButtons(false);
        try
        {
            await tOperation(tToken);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            DCProgress.ValueChanged -= OnProgressValueChanged;
            DCProgress.MaximumChanged -= OnProgressMaximumChanged;
            DCProgress.NameChanged -= OnProgressNameChanged;

            m_tCancellationSource.Dispose();
            m_tCancellationSource = null;

            if (m_bIsClosing == false && IsDisposed == false)
                ToggleButtons(true);
        }
    }

    void OnProgressValueChanged(int nValue)
    {
        if (m_bIsClosing || IsDisposed)
            return;
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => OnProgressValueChanged(nValue)));
            return;
        }
        PB_progress.Value = System.Math.Clamp(nValue, PB_progress.Minimum, PB_progress.Maximum);
    }

    void OnProgressMaximumChanged(int nValue)
    {
        if (m_bIsClosing || IsDisposed)
            return;
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => OnProgressMaximumChanged(nValue)));
            return;
        }
        PB_progress.Maximum = System.Math.Max(1, nValue);
    }

    void OnProgressNameChanged(string szName)
    {
        if (m_bIsClosing || IsDisposed)
            return;
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => OnProgressNameChanged(szName)));
            return;
        }
        LB_progressText.Text = szName ?? string.Empty;
    }

    void BT_cancelWork_Click(object? sender, EventArgs e)
    {
        m_tCancellationSource?.Cancel();
    }

    /// <summary>
    /// Gathers the extensions (with leading dot) for all pack-family formats.
    /// </summary>
    string[] PakExtensions()
    {
        string[] szExtensions = new string[PAK_TypeList.Length];
        for (int i = 0; i < PAK_TypeList.Length; i++)
            szExtensions[i] = "." + PAK_TypeList[i].Extensions[0];
        return szExtensions;
    }

    #region Extraction
    async void BT_ExtractDAT_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFileDialog(DATFormat.ToString(), "Choose a valid .DAT to extract", out string szDatPath) &&
            FileDialogHelpers.ShowOpenFileDialog(HD_AllTypesFilter, "Choose a valid .HD2 or .HD3", out string szHdPath) &&
            FileDialogHelpers.ShowOpenFolderDialog("Choose an output directory to write the .DAT to", out string szOutPath))
        {
            await RunOperationAsync(token => Task.Run(() =>
            {
                DAT.HDType eType = szHdPath.EndsWith(".hd2", StringComparison.OrdinalIgnoreCase) ? DAT.HDType.Two : DAT.HDType.Three;
                DAT.ExtractHD(szDatPath, szHdPath, szOutPath, eType);
            }, token));
        }
    }

    async void BT_extractPAKFile_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFileDialog(PAK_AllTypesFilter, "Choose a PAK-type file to extract", out string szPath))
            await RunOperationAsync(token => Task.Run(() => PAK.ExtractFile(szPath), token));
    }

    async void BT_extractPAKDirectory_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFolderDialog("Choose a directory to extract PAK-type files", out string szPath))
            await RunOperationAsync(token => Task.Run(() => PAK.ExtractDirectory(szPath, PakExtensions()), token));
    }
    #endregion

    #region Conversion
    async void BT_convertMDStoOBJFile_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFileDialog(MDSFormat.ToString(), "Choose a .mds file to convert to a .obj.", out string szPath))
            await RunOperationAsync(token => Task.Run(() =>
            {
                if (MDS.Load(szPath, out Model tModel, false))
                    WavefrontOBJ.Save(tModel);
            }, token));
    }

    async void BT_convertMDStoOBJDirectory_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFolderDialog("Choose an input directory to convert mds files.", out string szDir))
            await RunOperationAsync(token => Task.Run(() =>
            {
                Model[] tModels = MDS.LoadDirectory(szDir);
                foreach (Model tModel in tModels)
                    WavefrontOBJ.Save(tModel);
            }, token));
    }

    async void BT_convertMDStoSMDFile_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFileDialog(MDSFormat.ToString(), "Choose a .mds file to convert to a .smd.", out string szPath))
            await RunOperationAsync(token => Task.Run(() =>
            {
                if (MDS.Load(szPath, out Model tModel, false))
                    PerformSaveSMD(tModel);
            }, token));
    }

    async void BT_convertMDStoSMDDirectory_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFolderDialog("Choose an input directory to convert mds files.", out string szDir))
            await RunOperationAsync(token => Task.Run(() =>
            {
                Model[] tModels = MDS.LoadDirectory(szDir);
                foreach (Model tModel in tModels)
                    PerformSaveSMD(tModel);
            }, token));
    }

    async void BT_convertIMGFile_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFileDialog(IMGFormat.ToString(), "Choose a .img file to convert.", out string szPath))
            await RunOperationAsync(token => Task.Run(() =>
            {
                if (IMG.Load(szPath, out TIM2Image[] tImages))
                {
                    foreach (TIM2Image tImage in tImages)
                        ImageHelpers.SavePNG(tImage);
                }
            }, token));
    }

    async void BT_convertIMGDirectory_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFolderDialog("Choose a directory to convert img files.", out string szDir))
            await RunOperationAsync(token => Task.Run(() => SavePNGs(IMG.LoadDirectory(szDir)), token));
    }

    async void BT_convertTM2File_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFileDialog(TM2Format.ToString(), "Choose a .tm2 file to convert.", out string szPath))
            await RunOperationAsync(token => Task.Run(() =>
            {
                TIM2Image tImage = TM2.Load(szPath);
                if (tImage != null)
                    ImageHelpers.SavePNG(tImage);
            }, token));
    }

    async void BT_convertTM2Directory_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFolderDialog("Choose a directory to convert tm2 files.", out string szDir))
            await RunOperationAsync(token => Task.Run(() => SavePNGs(TM2.LoadDirectory(szDir)), token));
    }
    #endregion

    #region ExtractAndConvert
    async void BT_extractAndConvert_Click(object? sender, EventArgs e)
    {
        if (isWorking)
            return;

        if (FileDialogHelpers.ShowOpenFolderDialog("Choose a directory to extract ALL files from (containing DATA.DAT/DATA.HD2).", out string szExtractPath) &&
            FileDialogHelpers.ShowOpenFolderDialog("Choose a destination directory for the converted output.", out string szDestinationPath))
        {
            //The heavy lifting runs in the background, but the final move-to-destination step shows WinForms
            //dialogs (conflict prompts), so it deliberately runs back on the UI thread after the await completes.
            await RunOperationAsync(async token =>
            {
                await Task.Run(() => ExtractAndConvertAll(szExtractPath), token);

                if (DCProgress.canceled == false)
                    GuiFileHelpers.MoveFiles(szExtractPath, szDestinationPath, [SMDFormat, PNGFormat, CFGFormat], true, [SMDFormat, PNGFormat]);
            });
        }
    }

    /// <summary>
    /// Mirrors the legacy DCExtractorForm_Operations.ExtractAndConvert behavior (minus the final move, which the
    /// caller performs back on the UI thread).
    /// </summary>
    void ExtractAndConvertAll(string szExtractPath)
    {
        DCProgress.name = "Unpacking All";
        DCProgress.value = 0;

        if (DCProgress.canceled == false)
            DAT.ExtractHD(Path.Combine(szExtractPath, "DATA.DAT"), Path.Combine(szExtractPath, "DATA.HD2"), szExtractPath, DAT.HDType.Two);

        if (DCProgress.canceled == false)
            PAK.ExtractDirectory(szExtractPath, PakExtensions());

        if (DCProgress.canceled == false)
        {
            Model[] tModels = MDS.LoadDirectory(szExtractPath);
            foreach (Model tModel in tModels)
                PerformSaveSMD(tModel);
        }

        if (DCProgress.canceled == false)
            SavePNGs(TM2.LoadDirectory(szExtractPath));

        if (DCProgress.canceled == false)
            SavePNGs(IMG.LoadDirectory(szExtractPath));

        if (DCProgress.canceled)
        {
            DCProgress.name = "Unpack All - CANCELED";
            return;
        }

        DCProgress.name = "Unpack All - FINISHED";
        DCProgress.value = DCProgress.maximum;
    }
    #endregion

    /// <summary>
    /// Mirrors the legacy DCExtractorForm.PerformSaveSMD behavior exactly.
    /// </summary>
    static void PerformSaveSMD(Model tModel)
    {
        if (Settings.modelConversion == Settings.ModelConversionSetting.OneModel)
        {
            SMD.Save(tModel);
            if (Settings.exportAnimations)
                SMD.SaveAnimations(tModel);
        }
        else
        {
            Model[] tModels = tModel.Split();
            foreach (Model tSplitModel in tModels)
                SMD.Save(tSplitModel);
            if (Settings.exportAnimations)
                SMD.SaveAnimations(tModel);
        }
    }

    /// <summary>
    /// Saves a list of TIM2 images to PNGs in the same directory they were loaded from, reporting progress.
    /// </summary>
    static void SavePNGs(TIM2Image[] tImages)
    {
        if (DCProgress.canceled)
            return;

        DCProgress.value = 0;
        DCProgress.maximum = System.Math.Max(1, tImages.Length);
        DCProgress.name = "Saving PNGs";

        for (int i = 0; i < tImages.Length; i++)
        {
            DCProgress.name = "Saving PNG " + Path.ChangeExtension(Path.GetFileName(tImages[i].filePath), ".png");

            ImageHelpers.SavePNG(tImages[i]);

            DCProgress.value = i;

            if (DCProgress.canceled)
            {
                DCProgress.name = "Saving PNGs - CANCELED";
                return;
            }
        }

        DCProgress.value = DCProgress.maximum;
        DCProgress.name = "Saving PNGs - FINISHED";
    }
}
