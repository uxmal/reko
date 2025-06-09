namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class ProcedureListPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcedureListPanel));
            listProcedures = new System.Windows.Forms.ListView();
            colProcAddress = new System.Windows.Forms.ColumnHeader();
            colProcName = new System.Windows.Forms.ColumnHeader();
            colProcSegment = new System.Windows.Forms.ColumnHeader();
            txtProcedureFilter = new System.Windows.Forms.TextBox();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            btnFilter = new System.Windows.Forms.ToolStripDropDownButton();
            allProceduresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            rootProceduresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            leafProceduresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listProcedures
            // 
            listProcedures.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            listProcedures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colProcAddress, colProcName, colProcSegment });
            listProcedures.FullRowSelect = true;
            listProcedures.Location = new System.Drawing.Point(4, 60);
            listProcedures.Margin = new System.Windows.Forms.Padding(4);
            listProcedures.Name = "listProcedures";
            listProcedures.Size = new System.Drawing.Size(373, 336);
            listProcedures.TabIndex = 3;
            listProcedures.UseCompatibleStateImageBehavior = false;
            listProcedures.View = System.Windows.Forms.View.Details;
            // 
            // colProcAddress
            // 
            colProcAddress.Text = "Address";
            colProcAddress.Width = 62;
            // 
            // colProcName
            // 
            colProcName.Text = "Name";
            colProcName.Width = 200;
            // 
            // colProcSegment
            // 
            colProcSegment.Text = "Segment";
            colProcSegment.Width = 100;
            // 
            // txtProcedureFilter
            // 
            txtProcedureFilter.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtProcedureFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtProcedureFilter.Location = new System.Drawing.Point(4, 29);
            txtProcedureFilter.Margin = new System.Windows.Forms.Padding(4);
            txtProcedureFilter.Name = "txtProcedureFilter";
            txtProcedureFilter.Size = new System.Drawing.Size(373, 23);
            txtProcedureFilter.TabIndex = 2;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnFilter });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(381, 25);
            toolStrip1.TabIndex = 4;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnFilter
            // 
            btnFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            btnFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { allProceduresToolStripMenuItem, rootProceduresToolStripMenuItem, leafProceduresToolStripMenuItem });
            btnFilter.Image = (System.Drawing.Image) resources.GetObject("btnFilter.Image");
            btnFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new System.Drawing.Size(108, 22);
            btnFilter.Text = "Filter procedures";
            // 
            // allProceduresToolStripMenuItem
            // 
            allProceduresToolStripMenuItem.Name = "allProceduresToolStripMenuItem";
            allProceduresToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            allProceduresToolStripMenuItem.Text = "&All procedures";
            // 
            // rootProceduresToolStripMenuItem
            // 
            rootProceduresToolStripMenuItem.Name = "rootProceduresToolStripMenuItem";
            rootProceduresToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            rootProceduresToolStripMenuItem.Text = "&Root procedures";
            // 
            // leafProceduresToolStripMenuItem
            // 
            leafProceduresToolStripMenuItem.Name = "leafProceduresToolStripMenuItem";
            leafProceduresToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            leafProceduresToolStripMenuItem.Text = "&Leaf procedures";
            // 
            // ProcedureListPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(toolStrip1);
            Controls.Add(listProcedures);
            Controls.Add(txtProcedureFilter);
            Name = "ProcedureListPanel";
            Size = new System.Drawing.Size(381, 400);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView listProcedures;
        private System.Windows.Forms.ColumnHeader colProcAddress;
        private System.Windows.Forms.ColumnHeader colProcName;
        private System.Windows.Forms.ColumnHeader colProcSegment;
        private System.Windows.Forms.TextBox txtProcedureFilter;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton btnFilter;
        private System.Windows.Forms.ToolStripMenuItem allProceduresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rootProceduresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leafProceduresToolStripMenuItem;
    }
}
