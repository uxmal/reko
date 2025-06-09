namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class SymbolSourceDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.listSources = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colExtension = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCustomSource = new System.Windows.Forms.CheckBox();
            this.txtAssembly = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPickAssembly = new System.Windows.Forms.Button();
            this.btnSymbolFile = new System.Windows.Forms.Button();
            this.txtSymbolFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.listClasses = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Symbol sour&ces:";
            // 
            // listSources
            // 
            this.listSources.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listSources.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colDescription,
            this.colExtension});
            this.listSources.Location = new System.Drawing.Point(12, 88);
            this.listSources.MultiSelect = false;
            this.listSources.Name = "listSources";
            this.listSources.Size = new System.Drawing.Size(602, 168);
            this.listSources.TabIndex = 3;
            this.listSources.UseCompatibleStateImageBehavior = false;
            this.listSources.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.DisplayIndex = 2;
            this.colName.Text = "Name";
            // 
            // colDescription
            // 
            this.colDescription.DisplayIndex = 0;
            this.colDescription.Text = "Description";
            this.colDescription.Width = 315;
            // 
            // colExtension
            // 
            this.colExtension.DisplayIndex = 1;
            this.colExtension.Text = "Extension";
            this.colExtension.Width = 75;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(459, 400);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(540, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkCustomSource
            // 
            this.chkCustomSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkCustomSource.AutoSize = true;
            this.chkCustomSource.Location = new System.Drawing.Point(12, 262);
            this.chkCustomSource.Name = "chkCustomSource";
            this.chkCustomSource.Size = new System.Drawing.Size(153, 17);
            this.chkCustomSource.TabIndex = 4;
            this.chkCustomSource.Text = "Use &Custom symbol source";
            this.chkCustomSource.UseVisualStyleBackColor = true;
            // 
            // txtAssembly
            // 
            this.txtAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAssembly.Location = new System.Drawing.Point(28, 302);
            this.txtAssembly.Name = "txtAssembly";
            this.txtAssembly.Size = new System.Drawing.Size(556, 20);
            this.txtAssembly.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 286);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = ".NET &Assembly:";
            // 
            // btnPickAssembly
            // 
            this.btnPickAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPickAssembly.Location = new System.Drawing.Point(590, 300);
            this.btnPickAssembly.Name = "btnPickAssembly";
            this.btnPickAssembly.Size = new System.Drawing.Size(24, 23);
            this.btnPickAssembly.TabIndex = 6;
            this.btnPickAssembly.Text = "...";
            this.btnPickAssembly.UseVisualStyleBackColor = true;
            // 
            // btnSymbolFile
            // 
            this.btnSymbolFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSymbolFile.Location = new System.Drawing.Point(591, 26);
            this.btnSymbolFile.Name = "btnSymbolFile";
            this.btnSymbolFile.Size = new System.Drawing.Size(24, 23);
            this.btnSymbolFile.TabIndex = 2;
            this.btnSymbolFile.Text = "...";
            this.btnSymbolFile.UseVisualStyleBackColor = true;
            // 
            // txtSymbolFile
            // 
            this.txtSymbolFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSymbolFile.Location = new System.Drawing.Point(12, 28);
            this.txtSymbolFile.Name = "txtSymbolFile";
            this.txtSymbolFile.Size = new System.Drawing.Size(573, 20);
            this.txtSymbolFile.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "&Symbol file or URL:";
            // 
            // listClasses
            // 
            this.listClasses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listClasses.FormattingEnabled = true;
            this.listClasses.Location = new System.Drawing.Point(28, 333);
            this.listClasses.Name = "listClasses";
            this.listClasses.Size = new System.Drawing.Size(586, 56);
            this.listClasses.TabIndex = 12;
            // 
            // SymbolSourceDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(627, 435);
            this.Controls.Add(this.listClasses);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSymbolFile);
            this.Controls.Add(this.btnSymbolFile);
            this.Controls.Add(this.btnPickAssembly);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAssembly);
            this.Controls.Add(this.chkCustomSource);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.listSources);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SymbolSourceDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Symbol source";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listSources;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colExtension;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCustomSource;
        private System.Windows.Forms.TextBox txtAssembly;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPickAssembly;
        private System.Windows.Forms.Button btnSymbolFile;
        private System.Windows.Forms.TextBox txtSymbolFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listClasses;
    }
}