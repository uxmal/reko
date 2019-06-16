namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class CallHierarchyView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CallHierarchyView));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeCallHierarchy = new System.Windows.Forms.TreeView();
            this.listDetails = new System.Windows.Forms.ListView();
            this.colCallSites = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProcedure = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 25);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeCallHierarchy);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.listDetails);
            this.splitContainer.Size = new System.Drawing.Size(825, 125);
            this.splitContainer.SplitterDistance = 443;
            this.splitContainer.TabIndex = 0;
            // 
            // treeCallHierarchy
            // 
            this.treeCallHierarchy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeCallHierarchy.Location = new System.Drawing.Point(0, 0);
            this.treeCallHierarchy.Name = "treeCallHierarchy";
            this.treeCallHierarchy.Size = new System.Drawing.Size(443, 125);
            this.treeCallHierarchy.TabIndex = 0;
            // 
            // listDetails
            // 
            this.listDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCallSites,
            this.colProcedure});
            this.listDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDetails.Location = new System.Drawing.Point(0, 0);
            this.listDetails.Name = "listDetails";
            this.listDetails.Size = new System.Drawing.Size(378, 125);
            this.listDetails.TabIndex = 0;
            this.listDetails.UseCompatibleStateImageBehavior = false;
            this.listDetails.View = System.Windows.Forms.View.Details;
            // 
            // colCallSites
            // 
            this.colCallSites.Text = "Call Sites";
            this.colCallSites.Width = 93;
            // 
            // colProcedure
            // 
            this.colProcedure.Text = "Procedure";
            this.colProcedure.Width = 255;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDelete});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(825, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.White;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 22);
            this.btnDelete.Text = "Remove root";
            // 
            // CallHierarchyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.toolStrip);
            this.Name = "CallHierarchyView";
            this.Size = new System.Drawing.Size(825, 150);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeCallHierarchy;
        private System.Windows.Forms.ListView listDetails;
        private System.Windows.Forms.ColumnHeader colCallSites;
        private System.Windows.Forms.ColumnHeader colProcedure;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnDelete;
    }
}
