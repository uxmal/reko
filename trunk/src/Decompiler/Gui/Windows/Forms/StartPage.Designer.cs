namespace Decompiler.Gui.Windows.Forms
{
    partial class StartPage
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
            this.btnBrowseInputFile = new System.Windows.Forms.Button();
            this.txtInputFile = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLoadAddress = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTypes = new System.Windows.Forms.TextBox();
            this.btnBrowseTypes = new System.Windows.Forms.Button();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.txtIL = new System.Windows.Forms.TextBox();
            this.btnBrowseIL = new System.Windows.Forms.Button();
            this.txtAssembler = new System.Windows.Forms.TextBox();
            this.btnBrowseAssembler = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrowseInputFile
            // 
            this.btnBrowseInputFile.Location = new System.Drawing.Point(332, 30);
            this.btnBrowseInputFile.Name = "btnBrowseInputFile";
            this.btnBrowseInputFile.Size = new System.Drawing.Size(25, 23);
            this.btnBrowseInputFile.TabIndex = 0;
            this.btnBrowseInputFile.Text = "...";
            this.btnBrowseInputFile.UseVisualStyleBackColor = true;
            // 
            // txtInputFile
            // 
            this.txtInputFile.Location = new System.Drawing.Point(6, 32);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.Size = new System.Drawing.Size(320, 20);
            this.txtInputFile.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtInputFile);
            this.groupBox1.Controls.Add(this.btnBrowseInputFile);
            this.groupBox1.Controls.Add(this.txtLoadAddress);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 116);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input Files";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Load A&ddress:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Binary E&xecutable File:";
            // 
            // txtLoadAddress
            // 
            this.txtLoadAddress.Location = new System.Drawing.Point(88, 58);
            this.txtLoadAddress.Name = "txtLoadAddress";
            this.txtLoadAddress.Size = new System.Drawing.Size(100, 20);
            this.txtLoadAddress.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtTypes);
            this.groupBox2.Controls.Add(this.btnBrowseTypes);
            this.groupBox2.Controls.Add(this.txtSource);
            this.groupBox2.Controls.Add(this.btnBrowseSource);
            this.groupBox2.Controls.Add(this.txtIL);
            this.groupBox2.Controls.Add(this.btnBrowseIL);
            this.groupBox2.Controls.Add(this.txtAssembler);
            this.groupBox2.Controls.Add(this.btnBrowseAssembler);
            this.groupBox2.Location = new System.Drawing.Point(3, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(363, 203);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output Files";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "C/C++ &Type Definitions:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "C/C++ &Source Code:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "&Intermediate Language:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "&Assembler :";
            // 
            // txtTypes
            // 
            this.txtTypes.Location = new System.Drawing.Point(6, 162);
            this.txtTypes.Name = "txtTypes";
            this.txtTypes.Size = new System.Drawing.Size(320, 20);
            this.txtTypes.TabIndex = 7;
            // 
            // btnBrowseTypes
            // 
            this.btnBrowseTypes.Location = new System.Drawing.Point(332, 160);
            this.btnBrowseTypes.Name = "btnBrowseTypes";
            this.btnBrowseTypes.Size = new System.Drawing.Size(25, 23);
            this.btnBrowseTypes.TabIndex = 6;
            this.btnBrowseTypes.Text = "...";
            this.btnBrowseTypes.UseVisualStyleBackColor = true;
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(6, 120);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(320, 20);
            this.txtSource.TabIndex = 5;
            // 
            // btnBrowseSource
            // 
            this.btnBrowseSource.Location = new System.Drawing.Point(332, 118);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(25, 23);
            this.btnBrowseSource.TabIndex = 4;
            this.btnBrowseSource.Text = "...";
            this.btnBrowseSource.UseVisualStyleBackColor = true;
            // 
            // txtIL
            // 
            this.txtIL.Location = new System.Drawing.Point(6, 78);
            this.txtIL.Name = "txtIL";
            this.txtIL.Size = new System.Drawing.Size(320, 20);
            this.txtIL.TabIndex = 3;
            // 
            // btnBrowseIL
            // 
            this.btnBrowseIL.Location = new System.Drawing.Point(332, 76);
            this.btnBrowseIL.Name = "btnBrowseIL";
            this.btnBrowseIL.Size = new System.Drawing.Size(25, 23);
            this.btnBrowseIL.TabIndex = 2;
            this.btnBrowseIL.Text = "...";
            this.btnBrowseIL.UseVisualStyleBackColor = true;
            // 
            // txtAssembler
            // 
            this.txtAssembler.Location = new System.Drawing.Point(6, 36);
            this.txtAssembler.Name = "txtAssembler";
            this.txtAssembler.Size = new System.Drawing.Size(320, 20);
            this.txtAssembler.TabIndex = 1;
            // 
            // btnBrowseAssembler
            // 
            this.btnBrowseAssembler.Location = new System.Drawing.Point(332, 34);
            this.btnBrowseAssembler.Name = "btnBrowseAssembler";
            this.btnBrowseAssembler.Size = new System.Drawing.Size(25, 23);
            this.btnBrowseAssembler.TabIndex = 0;
            this.btnBrowseAssembler.Text = "...";
            this.btnBrowseAssembler.UseVisualStyleBackColor = true;
            // 
            // StartPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "StartPage";
            this.Size = new System.Drawing.Size(372, 331);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBrowseInputFile;
        private System.Windows.Forms.TextBox txtInputFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTypes;
        private System.Windows.Forms.Button btnBrowseTypes;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.TextBox txtIL;
        private System.Windows.Forms.Button btnBrowseIL;
        private System.Windows.Forms.TextBox txtAssembler;
        private System.Windows.Forms.Button btnBrowseAssembler;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLoadAddress;
        private System.Windows.Forms.Label label6;
    }
}
