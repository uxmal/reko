namespace Reko.WindowsItp
{
    partial class DecoderPerformanceDialog
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
            this.btnDoit = new System.Windows.Forms.Button();
            this.lblTest = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.rdbLoadFile = new System.Windows.Forms.RadioButton();
            this.rdbRandom = new System.Windows.Forms.RadioButton();
            this.txtRandomSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rdbThreadedDasm = new System.Windows.Forms.RadioButton();
            this.rdbInterpretedDasm = new System.Windows.Forms.RadioButton();
            this.rdbRealDasm = new System.Windows.Forms.RadioButton();
            this.chkRewriter = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDoit
            // 
            this.btnDoit.Location = new System.Drawing.Point(7, 241);
            this.btnDoit.Name = "btnDoit";
            this.btnDoit.Size = new System.Drawing.Size(75, 23);
            this.btnDoit.TabIndex = 0;
            this.btnDoit.Text = "&Do it";
            this.btnDoit.UseVisualStyleBackColor = true;
            this.btnDoit.Click += new System.EventHandler(this.btnDoit_Click);
            // 
            // lblTest
            // 
            this.lblTest.AutoSize = true;
            this.lblTest.Location = new System.Drawing.Point(4, 267);
            this.lblTest.Name = "lblTest";
            this.lblTest.Size = new System.Drawing.Size(35, 13);
            this.lblTest.TabIndex = 1;
            this.lblTest.Text = "label1";
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(24, 30);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(296, 20);
            this.txtFilename.TabIndex = 2;
            // 
            // rdbLoadFile
            // 
            this.rdbLoadFile.AutoSize = true;
            this.rdbLoadFile.Checked = true;
            this.rdbLoadFile.Location = new System.Drawing.Point(6, 7);
            this.rdbLoadFile.Name = "rdbLoadFile";
            this.rdbLoadFile.Size = new System.Drawing.Size(65, 17);
            this.rdbLoadFile.TabIndex = 3;
            this.rdbLoadFile.TabStop = true;
            this.rdbLoadFile.Text = "&Load file";
            this.rdbLoadFile.UseVisualStyleBackColor = true;
            // 
            // rdbRandom
            // 
            this.rdbRandom.AutoSize = true;
            this.rdbRandom.Location = new System.Drawing.Point(6, 50);
            this.rdbRandom.Name = "rdbRandom";
            this.rdbRandom.Size = new System.Drawing.Size(131, 17);
            this.rdbRandom.TabIndex = 4;
            this.rdbRandom.Text = "&Generate random data";
            this.rdbRandom.UseVisualStyleBackColor = true;
            // 
            // txtRandomSize
            // 
            this.txtRandomSize.Location = new System.Drawing.Point(57, 70);
            this.txtRandomSize.Name = "txtRandomSize";
            this.txtRandomSize.Size = new System.Drawing.Size(100, 20);
            this.txtRandomSize.TabIndex = 5;
            this.txtRandomSize.Text = "1000000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Si&ze:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtFilename);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.rdbLoadFile);
            this.panel1.Controls.Add(this.txtRandomSize);
            this.panel1.Controls.Add(this.rdbRandom);
            this.panel1.Location = new System.Drawing.Point(1, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(409, 100);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rdbThreadedDasm);
            this.panel2.Controls.Add(this.rdbInterpretedDasm);
            this.panel2.Controls.Add(this.rdbRealDasm);
            this.panel2.Location = new System.Drawing.Point(1, 106);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(404, 90);
            this.panel2.TabIndex = 8;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // rdbThreadedDasm
            // 
            this.rdbThreadedDasm.AutoSize = true;
            this.rdbThreadedDasm.Location = new System.Drawing.Point(6, 49);
            this.rdbThreadedDasm.Name = "rdbThreadedDasm";
            this.rdbThreadedDasm.Size = new System.Drawing.Size(201, 17);
            this.rdbThreadedDasm.TabIndex = 2;
            this.rdbThreadedDasm.Text = "Use &simulated &threading disassembler";
            this.rdbThreadedDasm.UseVisualStyleBackColor = true;
            // 
            // rdbInterpretedDasm
            // 
            this.rdbInterpretedDasm.AutoSize = true;
            this.rdbInterpretedDasm.Location = new System.Drawing.Point(6, 26);
            this.rdbInterpretedDasm.Name = "rdbInterpretedDasm";
            this.rdbInterpretedDasm.Size = new System.Drawing.Size(209, 17);
            this.rdbInterpretedDasm.TabIndex = 1;
            this.rdbInterpretedDasm.Text = "Use simulated &interpreting disassembler";
            this.rdbInterpretedDasm.UseVisualStyleBackColor = true;
            // 
            // rdbRealDasm
            // 
            this.rdbRealDasm.AutoSize = true;
            this.rdbRealDasm.Checked = true;
            this.rdbRealDasm.Location = new System.Drawing.Point(6, 3);
            this.rdbRealDasm.Name = "rdbRealDasm";
            this.rdbRealDasm.Size = new System.Drawing.Size(149, 17);
            this.rdbRealDasm.TabIndex = 0;
            this.rdbRealDasm.TabStop = true;
            this.rdbRealDasm.Text = "Use &real A32 disassembler";
            this.rdbRealDasm.UseVisualStyleBackColor = true;
            // 
            // chkRewriter
            // 
            this.chkRewriter.AutoSize = true;
            this.chkRewriter.Location = new System.Drawing.Point(1, 202);
            this.chkRewriter.Name = "chkRewriter";
            this.chkRewriter.Size = new System.Drawing.Size(83, 17);
            this.chkRewriter.TabIndex = 9;
            this.chkRewriter.Text = "Run re&writer";
            this.chkRewriter.UseVisualStyleBackColor = true;
            // 
            // DecoderPerformanceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 334);
            this.Controls.Add(this.chkRewriter);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTest);
            this.Controls.Add(this.btnDoit);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DecoderPerformanceDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DecoderPerformanceDialog";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDoit;
        private System.Windows.Forms.Label lblTest;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.RadioButton rdbLoadFile;
        private System.Windows.Forms.RadioButton rdbRandom;
        private System.Windows.Forms.TextBox txtRandomSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdbInterpretedDasm;
        private System.Windows.Forms.RadioButton rdbRealDasm;
        private System.Windows.Forms.RadioButton rdbThreadedDasm;
        private System.Windows.Forms.CheckBox chkRewriter;
    }
}