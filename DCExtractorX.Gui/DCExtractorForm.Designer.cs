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

    Button BT_extractAndConvertAll = null!;
    ProgressBar PB_progress = null!;
    Label LB_progressText = null!;
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
        BT_extractAndConvertAll = new Button();
        PB_progress = new ProgressBar();
        LB_progressText = new Label();
        BT_cancelWork = new Button();
        menuStrip1.SuspendLayout();
        SuspendLayout();
        //
        // menuStrip1
        //
        menuStrip1.Items.AddRange([fileMenuItem, settingsMenuItem]);
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(480, 24);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        //
        // fileMenuItem
        //
        fileMenuItem.DropDownItems.AddRange([extractMenuItem, convertMenuItem, extractAndConvertMenuItem, closeMenuItem]);
        fileMenuItem.Name = "fileMenuItem";
        fileMenuItem.Size = new Size(37, 20);
        fileMenuItem.Text = "File";
        //
        // extractMenuItem
        //
        extractMenuItem.DropDownItems.AddRange([extractDatItem, pakMenuItem]);
        extractMenuItem.Name = "extractMenuItem";
        extractMenuItem.Size = new Size(180, 22);
        extractMenuItem.Text = "Extract";
        extractMenuItem.ToolTipText = "Functions which extract data from an archive";
        //
        // extractDatItem
        //
        extractDatItem.Name = "extractDatItem";
        extractDatItem.Size = new Size(180, 22);
        extractDatItem.Text = "DAT";
        extractDatItem.ToolTipText = "Extract the contents of a Level-5 .dat file and its accompanying .hd2 or .hd3 file";
        extractDatItem.Click += BT_ExtractDAT_Click;
        //
        // pakMenuItem
        //
        pakMenuItem.DropDownItems.AddRange([extractPakFileItem, extractPakDirectoryItem]);
        pakMenuItem.Name = "pakMenuItem";
        pakMenuItem.Size = new Size(300, 22);
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
        convertMenuItem.DropDownItems.AddRange([mdsMenuItem, imgMenuItem, tm2MenuItem]);
        convertMenuItem.Name = "convertMenuItem";
        convertMenuItem.Size = new Size(180, 22);
        convertMenuItem.Text = "Convert";
        convertMenuItem.ToolTipText = "Functions which convert files from one format to another";
        //
        // mdsMenuItem
        //
        mdsMenuItem.DropDownItems.AddRange([objMenuItem, smdMenuItem]);
        mdsMenuItem.Name = "mdsMenuItem";
        mdsMenuItem.Size = new Size(99, 22);
        mdsMenuItem.Text = "MDS";
        mdsMenuItem.ToolTipText = "Functions for converting Level-5 .mds model files into other formats";
        //
        // objMenuItem
        //
        objMenuItem.DropDownItems.AddRange([convertMdsObjFileItem, convertMdsObjDirectoryItem]);
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
        smdMenuItem.DropDownItems.AddRange([convertMdsSmdFileItem, convertMdsSmdDirectoryItem]);
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
        imgMenuItem.DropDownItems.AddRange([convertImgFileItem, convertImgDirectoryItem]);
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
        tm2MenuItem.DropDownItems.AddRange([convertTm2FileItem, convertTm2DirectoryItem]);
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
        extractAndConvertMenuItem.Size = new Size(180, 22);
        extractAndConvertMenuItem.Text = "Extract && Convert Everything";
        extractAndConvertMenuItem.ToolTipText = "Extracts and converts an entire game data directory in one pass";
        extractAndConvertMenuItem.Click += BT_extractAndConvert_Click;
        //
        // closeMenuItem
        //
        closeMenuItem.Name = "closeMenuItem";
        closeMenuItem.Size = new Size(180, 22);
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
        // BT_extractAndConvertAll
        //
        BT_extractAndConvertAll.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        BT_extractAndConvertAll.Location = new Point(12, 40);
        BT_extractAndConvertAll.Name = "BT_extractAndConvertAll";
        BT_extractAndConvertAll.Size = new Size(456, 60);
        BT_extractAndConvertAll.TabIndex = 1;
        BT_extractAndConvertAll.Text = "Extract && Convert Everything";
        BT_extractAndConvertAll.UseVisualStyleBackColor = true;
        BT_extractAndConvertAll.Click += BT_extractAndConvert_Click;
        //
        // PB_progress
        //
        PB_progress.Location = new Point(12, 150);
        PB_progress.Name = "PB_progress";
        PB_progress.Size = new Size(456, 23);
        PB_progress.TabIndex = 2;
        //
        // LB_progressText
        //
        LB_progressText.AutoSize = true;
        LB_progressText.Location = new Point(12, 176);
        LB_progressText.Name = "LB_progressText";
        LB_progressText.Size = new Size(0, 15);
        LB_progressText.TabIndex = 3;
        //
        // BT_cancelWork
        //
        BT_cancelWork.Enabled = false;
        BT_cancelWork.Location = new Point(12, 200);
        BT_cancelWork.Name = "BT_cancelWork";
        BT_cancelWork.Size = new Size(456, 30);
        BT_cancelWork.TabIndex = 4;
        BT_cancelWork.Text = "Cancel";
        BT_cancelWork.UseVisualStyleBackColor = true;
        BT_cancelWork.Click += BT_cancelWork_Click;
        //
        // DCExtractorForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(480, 242);
        Controls.Add(BT_extractAndConvertAll);
        Controls.Add(PB_progress);
        Controls.Add(LB_progressText);
        Controls.Add(BT_cancelWork);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        MaximizeBox = false;
        Name = "DCExtractorForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "DCExtractorX";
        ResumeLayout(false);
        PerformLayout();
    }
}
