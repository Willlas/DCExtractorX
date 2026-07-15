#nullable enable

namespace DCExtractorX.Gui.Forms;

partial class SettingsForm
{
    System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    Label LB_modelConversion = null!;
    ComboBox CB_modelConversion = null!;
    CheckBox CH_exportAnimations = null!;
    CheckBox CH_showMDSWarnings = null!;
    CheckBox CH_showFileExtensionWarnings = null!;
    Button BT_ok = null!;
    Button BT_cancel = null!;

    void InitializeComponent()
    {
        LB_modelConversion = new Label();
        CB_modelConversion = new ComboBox();
        CH_exportAnimations = new CheckBox();
        CH_showMDSWarnings = new CheckBox();
        CH_showFileExtensionWarnings = new CheckBox();
        BT_ok = new Button();
        BT_cancel = new Button();
        SuspendLayout();
        //
        // LB_modelConversion
        //
        LB_modelConversion.AutoSize = true;
        LB_modelConversion.Location = new Point(12, 15);
        LB_modelConversion.Name = "LB_modelConversion";
        LB_modelConversion.Size = new Size(102, 15);
        LB_modelConversion.TabIndex = 0;
        LB_modelConversion.Text = "Model Conversion:";
        //
        // CB_modelConversion
        //
        CB_modelConversion.DropDownStyle = ComboBoxStyle.DropDownList;
        CB_modelConversion.Location = new Point(150, 12);
        CB_modelConversion.Name = "CB_modelConversion";
        CB_modelConversion.Size = new Size(180, 23);
        CB_modelConversion.TabIndex = 1;
        //
        // CH_exportAnimations
        //
        CH_exportAnimations.AutoSize = true;
        CH_exportAnimations.Location = new Point(12, 48);
        CH_exportAnimations.Name = "CH_exportAnimations";
        CH_exportAnimations.Size = new Size(300, 19);
        CH_exportAnimations.TabIndex = 2;
        CH_exportAnimations.Text = "Export animations (KNOWN BROKEN bind poses)";
        CH_exportAnimations.UseVisualStyleBackColor = true;
        //
        // CH_showMDSWarnings
        //
        CH_showMDSWarnings.AutoSize = true;
        CH_showMDSWarnings.Location = new Point(12, 75);
        CH_showMDSWarnings.Name = "CH_showMDSWarnings";
        CH_showMDSWarnings.Size = new Size(180, 19);
        CH_showMDSWarnings.TabIndex = 3;
        CH_showMDSWarnings.Text = "Show MDS warnings";
        CH_showMDSWarnings.UseVisualStyleBackColor = true;
        //
        // CH_showFileExtensionWarnings
        //
        CH_showFileExtensionWarnings.AutoSize = true;
        CH_showFileExtensionWarnings.Location = new Point(12, 102);
        CH_showFileExtensionWarnings.Name = "CH_showFileExtensionWarnings";
        CH_showFileExtensionWarnings.Size = new Size(220, 19);
        CH_showFileExtensionWarnings.TabIndex = 4;
        CH_showFileExtensionWarnings.Text = "Show file extension warnings";
        CH_showFileExtensionWarnings.UseVisualStyleBackColor = true;
        //
        // BT_ok
        //
        BT_ok.Location = new Point(150, 140);
        BT_ok.Name = "BT_ok";
        BT_ok.Size = new Size(85, 30);
        BT_ok.TabIndex = 5;
        BT_ok.Text = "OK";
        BT_ok.UseVisualStyleBackColor = true;
        BT_ok.Click += BT_ok_Click;
        //
        // BT_cancel
        //
        BT_cancel.Location = new Point(245, 140);
        BT_cancel.Name = "BT_cancel";
        BT_cancel.Size = new Size(85, 30);
        BT_cancel.TabIndex = 6;
        BT_cancel.Text = "Cancel";
        BT_cancel.UseVisualStyleBackColor = true;
        BT_cancel.Click += BT_cancel_Click;
        //
        // SettingsForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(344, 185);
        Controls.Add(LB_modelConversion);
        Controls.Add(CB_modelConversion);
        Controls.Add(CH_exportAnimations);
        Controls.Add(CH_showMDSWarnings);
        Controls.Add(CH_showFileExtensionWarnings);
        Controls.Add(BT_ok);
        Controls.Add(BT_cancel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Settings";
        ResumeLayout(false);
        PerformLayout();
    }
}
