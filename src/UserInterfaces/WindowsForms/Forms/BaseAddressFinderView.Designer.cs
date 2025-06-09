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
            if (disposing && (components is not null))
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
            this.progressStringGuess = new System.Windows.Forms.ProgressBar();
            this.listCandidates = new System.Windows.Forms.ListView();
            this.colAddress = new System.Windows.Forms.ColumnHeader();
            this.columnStrings = new System.Windows.Forms.ColumnHeader();
            this.columnPrologs = new System.Windows.Forms.ColumnHeader();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnChangeBaseAddress = new System.Windows.Forms.Button();
            this.txtBaseAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressStringGuess
            // 
            this.progressStringGuess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressStringGuess.Location = new System.Drawing.Point(3, 93);
            this.progressStringGuess.Name = "progressStringGuess";
            this.progressStringGuess.Size = new System.Drawing.Size(795, 23);
            this.progressStringGuess.TabIndex = 0;
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
            this.listCandidates.Location = new System.Drawing.Point(0, 151);
            this.listCandidates.Name = "listCandidates";
            this.listCandidates.Size = new System.Drawing.Size(798, 318);
            this.listCandidates.TabIndex = 2;
            this.listCandidates.UseCompatibleStateImageBehavior = false;
            this.listCandidates.View = System.Windows.Forms.View.Details;
            this.listCandidates.SelectedIndexChanged += new System.EventHandler(this.listCandidates_SelectedIndexChanged);
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
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(3, 122);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(75, 23);
            this.btnStartStop.TabIndex = 1;
            this.btnStartStop.Text = "&Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnChangeBaseAddress
            // 
            this.btnChangeBaseAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnChangeBaseAddress.Location = new System.Drawing.Point(198, 489);
            this.btnChangeBaseAddress.Name = "btnChangeBaseAddress";
            this.btnChangeBaseAddress.Size = new System.Drawing.Size(129, 23);
            this.btnChangeBaseAddress.TabIndex = 4;
            this.btnChangeBaseAddress.Text = "&Change base address";
            this.btnChangeBaseAddress.UseVisualStyleBackColor = true;
            this.btnChangeBaseAddress.Click += new System.EventHandler(this.btnChangeBaseAddress_Click);
            // 
            // txtBaseAddress
            // 
            this.txtBaseAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtBaseAddress.Location = new System.Drawing.Point(3, 490);
            this.txtBaseAddress.Name = "txtBaseAddress";
            this.txtBaseAddress.Size = new System.Drawing.Size(189, 23);
            this.txtBaseAddress.TabIndex = 3;
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(197, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "Guess base address based on &strings";
            // 
            // BaseAddressFinderView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressStringGuess);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBaseAddress);
            this.Controls.Add(this.btnChangeBaseAddress);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.listCandidates);
            this.Controls.Add(this.label1);
            this.Name = "BaseAddressFinderView";
            this.Size = new System.Drawing.Size(804, 517);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ProgressBar progressStringGuess;
        private System.Windows.Forms.ListView listCandidates;
        private System.Windows.Forms.ColumnHeader colAddress;
        private System.Windows.Forms.ColumnHeader columnStrings;
        private System.Windows.Forms.ColumnHeader columnPrologs;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnChangeBaseAddress;
        private System.Windows.Forms.TextBox txtBaseAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
