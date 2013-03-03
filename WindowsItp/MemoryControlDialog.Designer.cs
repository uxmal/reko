namespace Decompiler.WindowsItp
{
    partial class MemoryControlDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkShowData = new System.Windows.Forms.CheckBox();
            this.memoryControl1 = new Decompiler.Gui.Windows.Controls.MemoryControl();
            this.SuspendLayout();
            // 
            // chkShowData
            // 
            this.chkShowData.AutoSize = true;
            this.chkShowData.Location = new System.Drawing.Point(13, 9);
            this.chkShowData.Name = "chkShowData";
            this.chkShowData.Size = new System.Drawing.Size(79, 17);
            this.chkShowData.TabIndex = 1;
            this.chkShowData.Text = "Show &Data";
            this.chkShowData.UseVisualStyleBackColor = true;
            this.chkShowData.CheckedChanged += new System.EventHandler(this.chkShowData_CheckedChanged);
            // 
            // memoryControl1
            // 
            this.memoryControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.memoryControl1.BytesPerRow = 16;
            this.memoryControl1.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryControl1.Location = new System.Drawing.Point(0, 32);
            this.memoryControl1.Name = "memoryControl1";
            this.memoryControl1.ProgramImage = null;
            this.memoryControl1.SelectedAddress = null;
            this.memoryControl1.Size = new System.Drawing.Size(404, 163);
            this.memoryControl1.TabIndex = 0;
            this.memoryControl1.Text = "memoryControl1";
            this.memoryControl1.TopAddress = null;
            this.memoryControl1.WordSize = 1;
            // 
            // MemoryControlDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 194);
            this.Controls.Add(this.chkShowData);
            this.Controls.Add(this.memoryControl1);
            this.Name = "MemoryControlDialog";
            this.Text = "MemoryControlDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Decompiler.Gui.Windows.Controls.MemoryControl memoryControl1;
        private System.Windows.Forms.CheckBox chkShowData;
    }
}