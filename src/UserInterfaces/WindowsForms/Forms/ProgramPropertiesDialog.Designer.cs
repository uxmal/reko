namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class ProgramPropertiesDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkRunScript = new System.Windows.Forms.CheckBox();
            this.txtScript = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.ddlOutputPolicies = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabLoading = new System.Windows.Forms.TabPage();
            this.tabScanning = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.scanHeuristics = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabAnalysis = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.analysisHeuristics = new System.Windows.Forms.CheckedListBox();
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabLoading.SuspendLayout();
            this.tabScanning.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabAnalysis.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkRunScript
            // 
            this.chkRunScript.AutoSize = true;
            this.chkRunScript.Location = new System.Drawing.Point(9, 7);
            this.chkRunScript.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkRunScript.Name = "chkRunScript";
            this.chkRunScript.Size = new System.Drawing.Size(174, 19);
            this.chkRunScript.TabIndex = 0;
            this.chkRunScript.Text = "Run OllyScript after &loading:";
            this.chkRunScript.UseVisualStyleBackColor = true;
            // 
            // txtScript
            // 
            this.txtScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScript.Enabled = false;
            this.txtScript.Location = new System.Drawing.Point(9, 33);
            this.txtScript.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtScript.Multiline = true;
            this.txtScript.Name = "txtScript";
            this.txtScript.Size = new System.Drawing.Size(422, 310);
            this.txtScript.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(254, 388);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 27);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(349, 388);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 27);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabLoading);
            this.tabControl1.Controls.Add(this.tabScanning);
            this.tabControl1.Controls.Add(this.tabAnalysis);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(450, 381);
            this.tabControl1.TabIndex = 4;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.ddlOutputPolicies);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Location = new System.Drawing.Point(4, 24);
            this.tabGeneral.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabGeneral.Size = new System.Drawing.Size(442, 353);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // ddlOutputPolicies
            // 
            this.ddlOutputPolicies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlOutputPolicies.FormattingEnabled = true;
            this.ddlOutputPolicies.Location = new System.Drawing.Point(12, 28);
            this.ddlOutputPolicies.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ddlOutputPolicies.Name = "ddlOutputPolicies";
            this.ddlOutputPolicies.Size = new System.Drawing.Size(419, 23);
            this.ddlOutputPolicies.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 8);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Output files:";
            // 
            // tabLoading
            // 
            this.tabLoading.Controls.Add(this.txtScript);
            this.tabLoading.Controls.Add(this.chkRunScript);
            this.tabLoading.Location = new System.Drawing.Point(4, 24);
            this.tabLoading.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLoading.Name = "tabLoading";
            this.tabLoading.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLoading.Size = new System.Drawing.Size(442, 353);
            this.tabLoading.TabIndex = 1;
            this.tabLoading.Text = "Loading";
            this.tabLoading.UseVisualStyleBackColor = true;
            // 
            // tabScanning
            // 
            this.tabScanning.Controls.Add(this.groupBox1);
            this.tabScanning.Location = new System.Drawing.Point(4, 24);
            this.tabScanning.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabScanning.Name = "tabScanning";
            this.tabScanning.Size = new System.Drawing.Size(442, 353);
            this.tabScanning.TabIndex = 2;
            this.tabScanning.Text = "Scanning";
            this.tabScanning.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.scanHeuristics);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(428, 218);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Heuristic scanning";
            // 
            // scanHeuristics
            // 
            this.scanHeuristics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scanHeuristics.FormattingEnabled = true;
            this.scanHeuristics.Location = new System.Drawing.Point(8, 90);
            this.scanHeuristics.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.scanHeuristics.Name = "scanHeuristics";
            this.scanHeuristics.Size = new System.Drawing.Size(412, 94);
            this.scanHeuristics.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(4, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(418, 50);
            this.label1.TabIndex = 1;
            this.label1.Text = "Heuristic scanning guesses where executable statements are by disassembling the b" +
    "inary and resolving possible conflicts.  Different scanning heuristics give diff" +
    "erent results.";
            // 
            // tabAnalysis
            // 
            this.tabAnalysis.Controls.Add(this.label3);
            this.tabAnalysis.Controls.Add(this.analysisHeuristics);
            this.tabAnalysis.Location = new System.Drawing.Point(4, 24);
            this.tabAnalysis.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabAnalysis.Name = "tabAnalysis";
            this.tabAnalysis.Size = new System.Drawing.Size(442, 353);
            this.tabAnalysis.TabIndex = 3;
            this.tabAnalysis.Text = "Analysis";
            this.tabAnalysis.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(14, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(413, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Select the heuristics to apply during the data flow analysis:";
            // 
            // analysisHeuristics
            // 
            this.analysisHeuristics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.analysisHeuristics.FormattingEnabled = true;
            this.analysisHeuristics.Location = new System.Drawing.Point(14, 44);
            this.analysisHeuristics.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.analysisHeuristics.Name = "analysisHeuristics";
            this.analysisHeuristics.Size = new System.Drawing.Size(412, 94);
            this.analysisHeuristics.TabIndex = 3;
            // 
            // ProgramPropertiesDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(450, 428);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgramPropertiesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Program Properties";
            this.tabControl1.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabLoading.ResumeLayout(false);
            this.tabLoading.PerformLayout();
            this.tabScanning.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabAnalysis.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkRunScript;
        private System.Windows.Forms.TextBox txtScript;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabLoading;
        private System.Windows.Forms.TabPage tabScanning;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox scanHeuristics;
        private System.Windows.Forms.ComboBox ddlOutputPolicies;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabAnalysis;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox analysisHeuristics;
    }
}