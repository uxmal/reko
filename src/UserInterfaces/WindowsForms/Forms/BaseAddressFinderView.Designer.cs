namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class BaseAddressFinderView
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
            this.chkGuessStrings = new System.Windows.Forms.CheckBox();
            this.chkGuessPrologs = new System.Windows.Forms.CheckBox();
            this.progressStringGuess = new System.Windows.Forms.ProgressBar();
            this.progressPrologGuess = new System.Windows.Forms.ProgressBar();
            this.listCandidates = new System.Windows.Forms.ListView();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.colAddress = new System.Windows.Forms.ColumnHeader();
            this.columnStrings = new System.Windows.Forms.ColumnHeader();
            this.columnPrologs = new System.Windows.Forms.ColumnHeader();
            this.btnChangeBaseAddress = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkGuessStrings
            // 
            this.chkGuessStrings.AutoSize = true;
            this.chkGuessStrings.Location = new System.Drawing.Point(3, 68);
            this.chkGuessStrings.Name = "chkGuessStrings";
            this.chkGuessStrings.Size = new System.Drawing.Size(216, 19);
            this.chkGuessStrings.TabIndex = 0;
            this.chkGuessStrings.Text = "Guess base address based on &strings";
            this.chkGuessStrings.UseVisualStyleBackColor = true;
            // 
            // chkGuessPrologs
            // 
            this.chkGuessPrologs.AutoSize = true;
            this.chkGuessPrologs.Location = new System.Drawing.Point(3, 132);
            this.chkGuessPrologs.Name = "chkGuessPrologs";
            this.chkGuessPrologs.Size = new System.Drawing.Size(278, 19);
            this.chkGuessPrologs.TabIndex = 1;
            this.chkGuessPrologs.Text = "Guess base address based on procedure &prologs";
            this.chkGuessPrologs.UseVisualStyleBackColor = true;
            // 
            // progressStringGuess
            // 
            this.progressStringGuess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressStringGuess.Location = new System.Drawing.Point(3, 93);
            this.progressStringGuess.Name = "progressStringGuess";
            this.progressStringGuess.Size = new System.Drawing.Size(795, 23);
            this.progressStringGuess.TabIndex = 2;
            // 
            // progressPrologGuess
            // 
            this.progressPrologGuess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressPrologGuess.Location = new System.Drawing.Point(3, 157);
            this.progressPrologGuess.Name = "progressPrologGuess";
            this.progressPrologGuess.Size = new System.Drawing.Size(795, 23);
            this.progressPrologGuess.TabIndex = 3;
            // 
            // listCandidates
            // 
            this.listCandidates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listCandidates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAddress,
            this.columnStrings,
            this.columnPrologs});
            this.listCandidates.Location = new System.Drawing.Point(0, 215);
            this.listCandidates.Name = "listCandidates";
            this.listCandidates.Size = new System.Drawing.Size(798, 254);
            this.listCandidates.TabIndex = 4;
            this.listCandidates.UseCompatibleStateImageBehavior = false;
            this.listCandidates.View = System.Windows.Forms.View.Details;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(3, 186);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(75, 23);
            this.btnStartStop.TabIndex = 5;
            this.btnStartStop.Text = "&Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            // 
            // colAddress
            // 
            this.colAddress.Text = "Address";
            this.colAddress.Width = 120;
            // 
            // columnStrings
            // 
            this.columnStrings.Text = "String results";
            this.columnStrings.Width = 120;
            // 
            // columnPrologs
            // 
            this.columnPrologs.Text = "Prolog results";
            this.columnPrologs.Width = 120;
            // 
            // btnChangeBaseAddress
            // 
            this.btnChangeBaseAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnChangeBaseAddress.Location = new System.Drawing.Point(198, 489);
            this.btnChangeBaseAddress.Name = "btnChangeBaseAddress";
            this.btnChangeBaseAddress.Size = new System.Drawing.Size(129, 23);
            this.btnChangeBaseAddress.TabIndex = 6;
            this.btnChangeBaseAddress.Text = "&Change base address";
            this.btnChangeBaseAddress.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(3, 490);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(189, 23);
            this.textBox1.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 472);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "&Address";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(0, 16);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(804, 33);
            this.label2.TabIndex = 9;
            this.label2.Text = "Base Address Finder";
            // 
            // BaseAddressFinderView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnChangeBaseAddress);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.listCandidates);
            this.Controls.Add(this.progressPrologGuess);
            this.Controls.Add(this.progressStringGuess);
            this.Controls.Add(this.chkGuessPrologs);
            this.Controls.Add(this.chkGuessStrings);
            this.Name = "BaseAddressFinderView";
            this.Size = new System.Drawing.Size(804, 517);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkGuessStrings;
        private System.Windows.Forms.CheckBox chkGuessPrologs;
        private System.Windows.Forms.ProgressBar progressStringGuess;
        private System.Windows.Forms.ProgressBar progressPrologGuess;
        private System.Windows.Forms.ListView listCandidates;
        private System.Windows.Forms.ColumnHeader colAddress;
        private System.Windows.Forms.ColumnHeader columnStrings;
        private System.Windows.Forms.ColumnHeader columnPrologs;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnChangeBaseAddress;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
