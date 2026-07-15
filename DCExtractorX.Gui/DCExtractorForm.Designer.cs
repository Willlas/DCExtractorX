#nullable enable

namespace DCExtractorX.Gui;

partial class DCExtractorForm
{
    System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    MenuStrip menuStrip1 = null!;
    ToolStripMenuItem fileMenuItem = null!;
    ToolStripMenuItem extractMenuItem = null!;
    ToolStripMenuItem extractDatItem = null!;
    ToolStripMenuItem pakMenuItem = null!;
    ToolStripMenuItem extractPakFileItem = null!;
    ToolStripMenuItem extractPakDirectoryItem = null!;
    ToolStripMenuItem convertMenuItem = null!;
    ToolStripMenuItem mdsMenuItem = null!;
    ToolStripMenuItem objMenuItem = null!;
    ToolStripMenuItem convertMdsObjFileItem = null!;
    ToolStripMenuItem convertMdsObjDirectoryItem = null!;
    ToolStripMenuItem smdMenuItem = null!;
    ToolStripMenuItem convertMdsSmdFileItem = null!;
    ToolStripMenuItem convertMdsSmdDirectoryItem = null!;
    ToolStripMenuItem imgMenuItem = null!;
    ToolStripMenuItem convertImgFileItem = null!;
    ToolStripMenuItem convertImgDirectoryItem = null!;
    ToolStripMenuItem tm2MenuItem = null!;
    ToolStripMenuItem convertTm2FileItem = null!;
    ToolStripMenuItem convertTm2DirectoryItem = null!;
    ToolStripMenuItem extractAndConvertMenuItem = null!;
    ToolStripMenuItem closeMenuItem = null!;
    ToolStripMenuItem settingsMenuItem = null!;

    Label LB_gameFolder = null!;
    TextBox TB_gameFolder = null!;
    Button BT_browseGameFolder = null!;
    Label LB_outputFolder = null!;
    TextBox TB_outputFolder = null!;
    Button BT_browseOutputFolder = null!;

    Button BT_extractAndConvertAll = null!;
    ProgressBar PB_progress = null!;
    Label LB_progressText = null!;
    Label LB_stageText = null!;
    Label LB_currentFileText = null!;
    Label LB_currentAssetText = null!;
    ListBox LB_log = null!;
    Label LB_summaryText = null!;
    Button BT_openOutputFolder = null!;
    Button BT_cancelWork = null!;

    void InitializeComponent()
    {
        menuStrip1 = new MenuStrip();
        fileMenuItem = new ToolStripMenuItem();
        extractMenuItem = new ToolStripMenuItem();
        extractDatItem = new ToolStripMenuItem();
        pakMenuItem = new ToolStripMenuItem();
        extractPakFileItem = new ToolStripMenuItem();
        extractPakDirectoryItem = new ToolStripMenuItem();
        convertMenuItem = new ToolStripMenuItem();
        mdsMenuItem = new ToolStripMenuItem();
        objMenuItem = new ToolStripMenuItem();
        convertMdsObjFileItem = new ToolStripMenuItem();
        convertMdsObjDirectoryItem = new ToolStripMenuItem();
        smdMenuItem = new ToolStripMenuItem();
        convertMdsSmdFileItem = new ToolStripMenuItem();
        convertMdsSmdDirectoryItem = new ToolStripMenuItem();
        imgMenuItem = new ToolStripMenuItem();
        convertImgFileItem = new ToolStripMenuItem();
        convertImgDirectoryItem = new ToolStripMenuItem();
        tm2MenuItem = new ToolStripMenuItem();
        convertTm2FileItem = new ToolStripMenuItem();
        convertTm2DirectoryItem = new ToolStripMenuItem();
        extractAndConvertMenuItem = new ToolStripMenuItem();
        closeMenuItem = new ToolStripMenuItem();
        settingsMenuItem = new ToolStripMenuItem();
        LB_gameFolder = new Label();
        TB_gameFolder = new TextBox();
        BT_browseGameFolder = new Button();
        LB_outputFolder = new Label();
        TB_outputFolder = new TextBox();
        BT_browseOutputFolder = new Button();
        BT_extractAndConvertAll = new Button();
        PB_progress = new ProgressBar();
        LB_progressText = new Label();
        LB_stageText = new Label();
        LB_currentFileText = new Label();
        LB_currentAssetText = new Label();
        LB_log = new ListBox();
        LB_summaryText = new Label();
        BT_openOutputFolder = new Button();
        BT_cancelWork = new Button();
        menuStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.Items.AddRange(new ToolStripItem[] { fileMenuItem, settingsMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(621, 24);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        menuStrip1.ItemClicked += menuStrip1_ItemClicked;
        // 
        // fileMenuItem
        // 
        fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { extractMenuItem, convertMenuItem, extractAndConvertMenuItem, closeMenuItem });
        fileMenuItem.Name = "fileMenuItem";
        fileMenuItem.Size = new Size(37, 20);
        fileMenuItem.Text = "File";
        // 
        // extractMenuItem
        // 
        extractMenuItem.DropDownItems.AddRange(new ToolStripItem[] { extractDatItem, pakMenuItem });
        extractMenuItem.Name = "extractMenuItem";
        extractMenuItem.Size = new Size(183, 22);
        extractMenuItem.Text = "Extract";
        extractMenuItem.ToolTipText = "Functions which extract data from an archive";
        // 
        // extractDatItem
        // 
        extractDatItem.Name = "extractDatItem";
        extractDatItem.Size = new Size(287, 22);
        extractDatItem.Text = "DAT";
        extractDatItem.ToolTipText = "Extract the contents of a Level-5 .dat file and its accompanying .hd2 or .hd3 file";
        extractDatItem.Click += BT_ExtractDAT_Click;
        // 
        // pakMenuItem
        // 
        pakMenuItem.DropDownItems.AddRange(new ToolStripItem[] { extractPakFileItem, extractPakDirectoryItem });
        pakMenuItem.Name = "pakMenuItem";
        pakMenuItem.Size = new Size(287, 22);
        pakMenuItem.Text = "PAK, CHR, IPK, MPK, PCP, EFP, SND, SKY";
        pakMenuItem.ToolTipText = "Extract the contents of a Level-5 pack-family file. There are many different types of pak file";
        // 
        // extractPakFileItem
        // 
        extractPakFileItem.Name = "extractPakFileItem";
        extractPakFileItem.Size = new Size(122, 22);
        extractPakFileItem.Text = "File";
        extractPakFileItem.ToolTipText = "Extracts a pak file's contents to the same directory";
        extractPakFileItem.Click += BT_extractPAKFile_Click;
        // 
        // extractPakDirectoryItem
        // 
        extractPakDirectoryItem.Name = "extractPakDirectoryItem";
        extractPakDirectoryItem.Size = new Size(122, 22);
        extractPakDirectoryItem.Text = "Directory";
        extractPakDirectoryItem.ToolTipText = "Extracts all pak-related files in the directory and subdirectories to their respective directories";
        extractPakDirectoryItem.Click += BT_extractPAKDirectory_Click;
        // 
        // convertMenuItem
        // 
        convertMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mdsMenuItem, imgMenuItem, tm2MenuItem });
        convertMenuItem.Name = "convertMenuItem";
        convertMenuItem.Size = new Size(183, 22);
        convertMenuItem.Text = "Convert";
        convertMenuItem.ToolTipText = "Functions which convert files from one format to another";
        // 
        // mdsMenuItem
        // 
        mdsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { objMenuItem, smdMenuItem });
        mdsMenuItem.Name = "mdsMenuItem";
        mdsMenuItem.Size = new Size(99, 22);
        mdsMenuItem.Text = "MDS";
        mdsMenuItem.ToolTipText = "Functions for converting Level-5 .mds model files into other formats";
        // 
        // objMenuItem
        // 
        objMenuItem.DropDownItems.AddRange(new ToolStripItem[] { convertMdsObjFileItem, convertMdsObjDirectoryItem });
        objMenuItem.Name = "objMenuItem";
        objMenuItem.Size = new Size(99, 22);
        objMenuItem.Text = "OBJ";
        objMenuItem.ToolTipText = "Functions for converting Level-5 .mds models to wavefront .obj models";
        // 
        // convertMdsObjFileItem
        // 
        convertMdsObjFileItem.Name = "convertMdsObjFileItem";
        convertMdsObjFileItem.Size = new Size(122, 22);
        convertMdsObjFileItem.Text = "File";
        convertMdsObjFileItem.ToolTipText = "Converts the selected Level-5 .mds model to a wavefront .obj file";
        convertMdsObjFileItem.Click += BT_convertMDStoOBJFile_Click;
        // 
        // convertMdsObjDirectoryItem
        // 
        convertMdsObjDirectoryItem.Name = "convertMdsObjDirectoryItem";
        convertMdsObjDirectoryItem.Size = new Size(122, 22);
        convertMdsObjDirectoryItem.Text = "Directory";
        convertMdsObjDirectoryItem.ToolTipText = "Converts a directory of Level-5 .mds models to wavefront .obj models";
        convertMdsObjDirectoryItem.Click += BT_convertMDStoOBJDirectory_Click;
        // 
        // smdMenuItem
        // 
        smdMenuItem.DropDownItems.AddRange(new ToolStripItem[] { convertMdsSmdFileItem, convertMdsSmdDirectoryItem });
        smdMenuItem.Name = "smdMenuItem";
        smdMenuItem.Size = new Size(99, 22);
        smdMenuItem.Text = "SMD";
        smdMenuItem.ToolTipText = "Functions for converting Level-5 .mds models to studiomdl .smd models";
        // 
        // convertMdsSmdFileItem
        // 
        convertMdsSmdFileItem.Name = "convertMdsSmdFileItem";
        convertMdsSmdFileItem.Size = new Size(122, 22);
        convertMdsSmdFileItem.Text = "File";
        convertMdsSmdFileItem.ToolTipText = "Converts the selected Level-5 .mds model to a studiomdl .smd file";
        convertMdsSmdFileItem.Click += BT_convertMDStoSMDFile_Click;
        // 
        // convertMdsSmdDirectoryItem
        // 
        convertMdsSmdDirectoryItem.Name = "convertMdsSmdDirectoryItem";
        convertMdsSmdDirectoryItem.Size = new Size(122, 22);
        convertMdsSmdDirectoryItem.Text = "Directory";
        convertMdsSmdDirectoryItem.ToolTipText = "Converts a directory of Level-5 .mds models to studiomdl .smd models";
        convertMdsSmdDirectoryItem.Click += BT_convertMDStoSMDDirectory_Click;
        // 
        // imgMenuItem
        // 
        imgMenuItem.DropDownItems.AddRange(new ToolStripItem[] { convertImgFileItem, convertImgDirectoryItem });
        imgMenuItem.Name = "imgMenuItem";
        imgMenuItem.Size = new Size(99, 22);
        imgMenuItem.Text = "IMG";
        imgMenuItem.ToolTipText = "Functions for converting Level-5 .img image packages to .png images";
        // 
        // convertImgFileItem
        // 
        convertImgFileItem.Name = "convertImgFileItem";
        convertImgFileItem.Size = new Size(122, 22);
        convertImgFileItem.Text = "File";
        convertImgFileItem.Click += BT_convertIMGFile_Click;
        // 
        // convertImgDirectoryItem
        // 
        convertImgDirectoryItem.Name = "convertImgDirectoryItem";
        convertImgDirectoryItem.Size = new Size(122, 22);
        convertImgDirectoryItem.Text = "Directory";
        convertImgDirectoryItem.Click += BT_convertIMGDirectory_Click;
        // 
        // tm2MenuItem
        // 
        tm2MenuItem.DropDownItems.AddRange(new ToolStripItem[] { convertTm2FileItem, convertTm2DirectoryItem });
        tm2MenuItem.Name = "tm2MenuItem";
        tm2MenuItem.Size = new Size(99, 22);
        tm2MenuItem.Text = "TM2";
        tm2MenuItem.ToolTipText = "Functions for converting PS2 TIM2 .tm2 images to .png images";
        // 
        // convertTm2FileItem
        // 
        convertTm2FileItem.Name = "convertTm2FileItem";
        convertTm2FileItem.Size = new Size(122, 22);
        convertTm2FileItem.Text = "File";
        convertTm2FileItem.Click += BT_convertTM2File_Click;
        // 
        // convertTm2DirectoryItem
        // 
        convertTm2DirectoryItem.Name = "convertTm2DirectoryItem";
        convertTm2DirectoryItem.Size = new Size(122, 22);
        convertTm2DirectoryItem.Text = "Directory";
        convertTm2DirectoryItem.Click += BT_convertTM2Directory_Click;
        // 
        // extractAndConvertMenuItem
        // 
        extractAndConvertMenuItem.Name = "extractAndConvertMenuItem";
        extractAndConvertMenuItem.Size = new Size(183, 22);
        extractAndConvertMenuItem.Text = "Analyze && Extract All";
        extractAndConvertMenuItem.ToolTipText = "Scans, extracts, converts, analyzes, and reports on an entire game data directory in one pass";
        extractAndConvertMenuItem.Click += BT_analyzeExtractAll_Click;
        // 
        // closeMenuItem
        // 
        closeMenuItem.Name = "closeMenuItem";
        closeMenuItem.Size = new Size(183, 22);
        closeMenuItem.Text = "Close";
        closeMenuItem.Click += BT_close_Click;
        // 
        // settingsMenuItem
        // 
        settingsMenuItem.Name = "settingsMenuItem";
        settingsMenuItem.Size = new Size(63, 20);
        settingsMenuItem.Text = "Settings";
        settingsMenuItem.Click += BT_settings_Click;
        // 
        // LB_gameFolder
        // 
        LB_gameFolder.AutoSize = true;
        LB_gameFolder.Location = new Point(12, 32);
        LB_gameFolder.Name = "LB_gameFolder";
        LB_gameFolder.Size = new Size(77, 15);
        LB_gameFolder.TabIndex = 0;
        LB_gameFolder.Text = "Game Folder:";
        // 
        // TB_gameFolder
        // 
        TB_gameFolder.Location = new Point(98, 29);
        TB_gameFolder.Name = "TB_gameFolder";
        TB_gameFolder.ReadOnly = true;
        TB_gameFolder.Size = new Size(430, 23);
        TB_gameFolder.TabIndex = 0;
        // 
        // BT_browseGameFolder
        // 
        BT_browseGameFolder.Location = new Point(534, 28);
        BT_browseGameFolder.Name = "BT_browseGameFolder";
        BT_browseGameFolder.Size = new Size(75, 25);
        BT_browseGameFolder.TabIndex = 1;
        BT_browseGameFolder.Text = "Browse...";
        BT_browseGameFolder.UseVisualStyleBackColor = true;
        BT_browseGameFolder.Click += BT_browseGameFolder_Click;
        // 
        // LB_outputFolder
        // 
        LB_outputFolder.AutoSize = true;
        LB_outputFolder.Location = new Point(12, 62);
        LB_outputFolder.Name = "LB_outputFolder";
        LB_outputFolder.Size = new Size(84, 15);
        LB_outputFolder.TabIndex = 2;
        LB_outputFolder.Text = "Output Folder:";
        // 
        // TB_outputFolder
        // 
        TB_outputFolder.Location = new Point(98, 59);
        TB_outputFolder.Name = "TB_outputFolder";
        TB_outputFolder.ReadOnly = true;
        TB_outputFolder.Size = new Size(430, 23);
        TB_outputFolder.TabIndex = 2;
        // 
        // BT_browseOutputFolder
        // 
        BT_browseOutputFolder.Location = new Point(534, 58);
        BT_browseOutputFolder.Name = "BT_browseOutputFolder";
        BT_browseOutputFolder.Size = new Size(75, 25);
        BT_browseOutputFolder.TabIndex = 3;
        BT_browseOutputFolder.Text = "Browse...";
        BT_browseOutputFolder.UseVisualStyleBackColor = true;
        BT_browseOutputFolder.Click += BT_browseOutputFolder_Click;
        // 
        // BT_extractAndConvertAll
        // 
        BT_extractAndConvertAll.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        BT_extractAndConvertAll.Location = new Point(12, 94);
        BT_extractAndConvertAll.Name = "BT_extractAndConvertAll";
        BT_extractAndConvertAll.Size = new Size(597, 50);
        BT_extractAndConvertAll.TabIndex = 4;
        BT_extractAndConvertAll.Text = "Analyze + Extract All";
        BT_extractAndConvertAll.UseVisualStyleBackColor = true;
        BT_extractAndConvertAll.Click += BT_analyzeExtractAll_Click;
        // 
        // PB_progress
        // 
        PB_progress.Location = new Point(12, 154);
        PB_progress.Name = "PB_progress";
        PB_progress.Size = new Size(597, 23);
        PB_progress.TabIndex = 5;
        // 
        // LB_progressText
        // 
        LB_progressText.AutoSize = true;
        LB_progressText.Location = new Point(12, 180);
        LB_progressText.Name = "LB_progressText";
        LB_progressText.Size = new Size(0, 15);
        LB_progressText.TabIndex = 6;
        // 
        // LB_stageText
        // 
        LB_stageText.AutoSize = true;
        LB_stageText.Location = new Point(12, 200);
        LB_stageText.Name = "LB_stageText";
        LB_stageText.Size = new Size(42, 15);
        LB_stageText.TabIndex = 7;
        LB_stageText.Text = "Stage: ";
        // 
        // LB_currentFileText
        // 
        LB_currentFileText.AutoSize = true;
        LB_currentFileText.Location = new Point(12, 218);
        LB_currentFileText.Name = "LB_currentFileText";
        LB_currentFileText.Size = new Size(31, 15);
        LB_currentFileText.TabIndex = 8;
        LB_currentFileText.Text = "File: ";
        // 
        // LB_currentAssetText
        // 
        LB_currentAssetText.AutoSize = true;
        LB_currentAssetText.Location = new Point(12, 236);
        LB_currentAssetText.Name = "LB_currentAssetText";
        LB_currentAssetText.Size = new Size(41, 15);
        LB_currentAssetText.TabIndex = 9;
        LB_currentAssetText.Text = "Asset: ";
        // 
        // LB_log
        // 
        LB_log.FormattingEnabled = true;
        LB_log.HorizontalScrollbar = true;
        LB_log.IntegralHeight = false;
        LB_log.Location = new Point(12, 258);
        LB_log.Name = "LB_log";
        LB_log.Size = new Size(597, 220);
        LB_log.TabIndex = 7;
        // 
        // LB_summaryText
        // 
        LB_summaryText.AutoSize = true;
        LB_summaryText.Location = new Point(12, 484);
        LB_summaryText.Name = "LB_summaryText";
        LB_summaryText.Size = new Size(0, 15);
        LB_summaryText.TabIndex = 10;
        // 
        // BT_openOutputFolder
        // 
        BT_openOutputFolder.Enabled = false;
        BT_openOutputFolder.Location = new Point(12, 506);
        BT_openOutputFolder.Name = "BT_openOutputFolder";
        BT_openOutputFolder.Size = new Size(597, 30);
        BT_openOutputFolder.TabIndex = 8;
        BT_openOutputFolder.Text = "Open Output Folder";
        BT_openOutputFolder.UseVisualStyleBackColor = true;
        BT_openOutputFolder.Click += BT_openOutputFolder_Click;
        // 
        // BT_cancelWork
        // 
        BT_cancelWork.Enabled = false;
        BT_cancelWork.Location = new Point(12, 542);
        BT_cancelWork.Name = "BT_cancelWork";
        BT_cancelWork.Size = new Size(597, 30);
        BT_cancelWork.TabIndex = 9;
        BT_cancelWork.Text = "Cancel";
        BT_cancelWork.UseVisualStyleBackColor = true;
        BT_cancelWork.Click += BT_cancelWork_Click;
        // 
        // DCExtractorForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(621, 584);
        Controls.Add(LB_gameFolder);
        Controls.Add(TB_gameFolder);
        Controls.Add(BT_browseGameFolder);
        Controls.Add(LB_outputFolder);
        Controls.Add(TB_outputFolder);
        Controls.Add(BT_browseOutputFolder);
        Controls.Add(BT_extractAndConvertAll);
        Controls.Add(PB_progress);
        Controls.Add(LB_progressText);
        Controls.Add(LB_stageText);
        Controls.Add(LB_currentFileText);
        Controls.Add(LB_currentAssetText);
        Controls.Add(LB_log);
        Controls.Add(LB_summaryText);
        Controls.Add(BT_openOutputFolder);
        Controls.Add(BT_cancelWork);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        MaximizeBox = false;
        Name = "DCExtractorForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "DCExtractorX";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
