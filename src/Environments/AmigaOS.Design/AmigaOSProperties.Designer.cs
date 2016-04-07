namespace Reko.Environments.AmigaOS.Design
{
    partial class AmigaOSProperties
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblKickstart = new System.Windows.Forms.Label();
            this.cmbKickstartVersions = new System.Windows.Forms.ComboBox();
            this.lblUsedLibraries = new System.Windows.Forms.Label();
            this.lstLoadedLibs = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Image = global::Reko.Environments.AmigaOS.Design.Properties.Resources.AmigaOS;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 125);
            this.label1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(148, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(588, 29);
            this.label2.TabIndex = 1;
            this.label2.Text = "AmigaOS properties";
            // 
            // lblKickstart
            // 
            this.lblKickstart.AutoSize = true;
            this.lblKickstart.Location = new System.Drawing.Point(145, 58);
            this.lblKickstart.Name = "lblKickstart";
            this.lblKickstart.Size = new System.Drawing.Size(118, 13);
            this.lblKickstart.TabIndex = 2;
            this.lblKickstart.Text = "Select &Kickstart version";
            // 
            // cmbKickstartVersions
            // 
            this.cmbKickstartVersions.FormattingEnabled = true;
            this.cmbKickstartVersions.Location = new System.Drawing.Point(148, 75);
            this.cmbKickstartVersions.Name = "cmbKickstartVersions";
            this.cmbKickstartVersions.Size = new System.Drawing.Size(182, 21);
            this.cmbKickstartVersions.TabIndex = 3;
            // 
            // lblUsedLibraries
            // 
            this.lblUsedLibraries.AutoSize = true;
            this.lblUsedLibraries.Location = new System.Drawing.Point(337, 58);
            this.lblUsedLibraries.Name = "lblUsedLibraries";
            this.lblUsedLibraries.Size = new System.Drawing.Size(47, 13);
            this.lblUsedLibraries.TabIndex = 4;
            this.lblUsedLibraries.Text = "Will use:";
            // 
            // lstLoadedLibs
            // 
            this.lstLoadedLibs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstLoadedLibs.Location = new System.Drawing.Point(337, 75);
            this.lstLoadedLibs.Name = "lstLoadedLibs";
            this.lstLoadedLibs.Size = new System.Drawing.Size(187, 161);
            this.lstLoadedLibs.TabIndex = 5;
            this.lstLoadedLibs.UseCompatibleStateImageBehavior = false;
            this.lstLoadedLibs.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Library";
            this.columnHeader1.Width = 144;
            // 
            // AmigaOSProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lstLoadedLibs);
            this.Controls.Add(this.lblUsedLibraries);
            this.Controls.Add(this.cmbKickstartVersions);
            this.Controls.Add(this.lblKickstart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "AmigaOSProperties";
            this.Size = new System.Drawing.Size(731, 271);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblKickstart;
        private System.Windows.Forms.ComboBox cmbKickstartVersions;
        private System.Windows.Forms.Label lblUsedLibraries;
        private System.Windows.Forms.ListView lstLoadedLibs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}
