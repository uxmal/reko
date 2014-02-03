namespace Decompiler.WindowsItp
{
    partial class TreeViewDialog
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
            this.treeView = new System.Windows.Forms.TreeView();
            this.btnOneByOne = new System.Windows.Forms.Button();
            this.btnAllAtOnce = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtItems = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Location = new System.Drawing.Point(12, 12);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(179, 222);
            this.treeView.TabIndex = 0;
            // 
            // btnOneByOne
            // 
            this.btnOneByOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOneByOne.Location = new System.Drawing.Point(197, 38);
            this.btnOneByOne.Name = "btnOneByOne";
            this.btnOneByOne.Size = new System.Drawing.Size(75, 23);
            this.btnOneByOne.TabIndex = 1;
            this.btnOneByOne.Text = "One by One";
            this.btnOneByOne.UseVisualStyleBackColor = true;
            this.btnOneByOne.Click += new System.EventHandler(this.btnOneByOne_Click);
            // 
            // btnAllAtOnce
            // 
            this.btnAllAtOnce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAllAtOnce.Location = new System.Drawing.Point(197, 67);
            this.btnAllAtOnce.Name = "btnAllAtOnce";
            this.btnAllAtOnce.Size = new System.Drawing.Size(75, 23);
            this.btnAllAtOnce.TabIndex = 2;
            this.btnAllAtOnce.Text = "All at once";
            this.btnAllAtOnce.UseVisualStyleBackColor = true;
            this.btnAllAtOnce.Click += new System.EventHandler(this.btnAllAtOnce_Click);
            // 
            // lblResult
            // 
            this.lblResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(12, 237);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(0, 13);
            this.lblResult.TabIndex = 3;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(197, 109);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtItems
            // 
            this.txtItems.Location = new System.Drawing.Point(197, 12);
            this.txtItems.Name = "txtItems";
            this.txtItems.Size = new System.Drawing.Size(75, 20);
            this.txtItems.TabIndex = 5;
            this.txtItems.TextChanged += new System.EventHandler(this.txtItems_TextChanged);
            // 
            // TreeViewDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.txtItems);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnAllAtOnce);
            this.Controls.Add(this.btnOneByOne);
            this.Controls.Add(this.treeView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TreeViewDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TreeViewDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button btnOneByOne;
        private System.Windows.Forms.Button btnAllAtOnce;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtItems;
    }
}