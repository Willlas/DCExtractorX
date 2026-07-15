#nullable enable

namespace DCExtractorX.Gui.Forms;

partial class FileConflictDialog
{
    System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    Label LB_title = null!;
    Label LB_sourceCaption = null!;
    Label LB_sourceFile = null!;
    Label LB_destinationCaption = null!;
    Label LB_destinationFile = null!;
    Button BT_yes = null!;
    Button BT_no = null!;
    Button BT_yesToAll = null!;
    Button BT_noToAll = null!;

    void InitializeComponent()
    {
        LB_title = new Label();
        LB_sourceCaption = new Label();
        LB_sourceFile = new Label();
        LB_destinationCaption = new Label();
        LB_destinationFile = new Label();
        BT_yes = new Button();
        BT_no = new Button();
        BT_yesToAll = new Button();
        BT_noToAll = new Button();
        SuspendLayout();
        //
        // LB_title
        //
        LB_title.AutoSize = true;
        LB_title.Location = new Point(12, 9);
        LB_title.Name = "LB_title";
        LB_title.Size = new Size(300, 15);
        LB_title.TabIndex = 0;
        LB_title.Text = "The destination file already exists. Overwrite it?";
        //
        // LB_sourceCaption
        //
        LB_sourceCaption.AutoSize = true;
        LB_sourceCaption.Location = new Point(12, 34);
        LB_sourceCaption.Name = "LB_sourceCaption";
        LB_sourceCaption.Size = new Size(45, 15);
        LB_sourceCaption.TabIndex = 1;
        LB_sourceCaption.Text = "Source:";
        //
        // LB_sourceFile
        //
        LB_sourceFile.AutoEllipsis = true;
        LB_sourceFile.Location = new Point(12, 52);
        LB_sourceFile.Name = "LB_sourceFile";
        LB_sourceFile.Size = new Size(460, 15);
        LB_sourceFile.TabIndex = 2;
        //
        // LB_destinationCaption
        //
        LB_destinationCaption.AutoSize = true;
        LB_destinationCaption.Location = new Point(12, 77);
        LB_destinationCaption.Name = "LB_destinationCaption";
        LB_destinationCaption.Size = new Size(72, 15);
        LB_destinationCaption.TabIndex = 3;
        LB_destinationCaption.Text = "Destination:";
        //
        // LB_destinationFile
        //
        LB_destinationFile.AutoEllipsis = true;
        LB_destinationFile.Location = new Point(12, 95);
        LB_destinationFile.Name = "LB_destinationFile";
        LB_destinationFile.Size = new Size(460, 15);
        LB_destinationFile.TabIndex = 4;
        //
        // BT_yes
        //
        BT_yes.Location = new Point(12, 130);
        BT_yes.Name = "BT_yes";
        BT_yes.Size = new Size(110, 30);
        BT_yes.TabIndex = 5;
        BT_yes.Text = "Yes";
        BT_yes.UseVisualStyleBackColor = true;
        BT_yes.Click += BT_yes_Click;
        //
        // BT_no
        //
        BT_no.Location = new Point(128, 130);
        BT_no.Name = "BT_no";
        BT_no.Size = new Size(110, 30);
        BT_no.TabIndex = 6;
        BT_no.Text = "No";
        BT_no.UseVisualStyleBackColor = true;
        BT_no.Click += BT_no_Click;
        //
        // BT_yesToAll
        //
        BT_yesToAll.Location = new Point(244, 130);
        BT_yesToAll.Name = "BT_yesToAll";
        BT_yesToAll.Size = new Size(110, 30);
        BT_yesToAll.TabIndex = 7;
        BT_yesToAll.Text = "Yes To All";
        BT_yesToAll.UseVisualStyleBackColor = true;
        BT_yesToAll.Click += BT_yesToAll_Click;
        //
        // BT_noToAll
        //
        BT_noToAll.Location = new Point(360, 130);
        BT_noToAll.Name = "BT_noToAll";
        BT_noToAll.Size = new Size(110, 30);
        BT_noToAll.TabIndex = 8;
        BT_noToAll.Text = "No To All";
        BT_noToAll.UseVisualStyleBackColor = true;
        BT_noToAll.Click += BT_noToAll_Click;
        //
        // FileConflictDialog
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(484, 172);
        Controls.Add(LB_title);
        Controls.Add(LB_sourceCaption);
        Controls.Add(LB_sourceFile);
        Controls.Add(LB_destinationCaption);
        Controls.Add(LB_destinationFile);
        Controls.Add(BT_yes);
        Controls.Add(BT_no);
        Controls.Add(BT_yesToAll);
        Controls.Add(BT_noToAll);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FileConflictDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "File Conflict";
        ResumeLayout(false);
        PerformLayout();
    }
}
