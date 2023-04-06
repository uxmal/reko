namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class CallGraphNavigatorView
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
            listCallers = new System.Windows.Forms.ListView();
            colCallerName = new System.Windows.Forms.ColumnHeader();
            colCallerAddress = new System.Windows.Forms.ColumnHeader();
            listCallees = new System.Windows.Forms.ListView();
            colCalleeName = new System.Windows.Forms.ColumnHeader();
            colCalleeAddress = new System.Windows.Forms.ColumnHeader();
            lblAddress = new System.Windows.Forms.Label();
            linkProcedure = new System.Windows.Forms.LinkLabel();
            lblSignature = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            lblCallers = new System.Windows.Forms.Label();
            lblProcedure = new System.Windows.Forms.Label();
            lblCallees = new System.Windows.Forms.Label();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            btnBack = new System.Windows.Forms.ToolStripButton();
            btnForward = new System.Windows.Forms.ToolStripButton();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listCallers
            // 
            listCallers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listCallers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colCallerName, colCallerAddress });
            listCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            listCallers.Location = new System.Drawing.Point(3, 29);
            listCallers.Name = "listCallers";
            listCallers.Size = new System.Drawing.Size(327, 381);
            listCallers.TabIndex = 0;
            listCallers.UseCompatibleStateImageBehavior = false;
            listCallers.View = System.Windows.Forms.View.Details;
            listCallers.DoubleClick += listCallers_DoubleClick;
            // 
            // colCallerName
            // 
            colCallerName.Text = "Name";
            colCallerName.Width = 180;
            // 
            // colCallerAddress
            // 
            colCallerAddress.Text = "Address";
            colCallerAddress.Width = 180;
            // 
            // listCallees
            // 
            listCallees.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listCallees.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colCalleeName, colCalleeAddress });
            listCallees.Dock = System.Windows.Forms.DockStyle.Fill;
            listCallees.Location = new System.Drawing.Point(669, 29);
            listCallees.Name = "listCallees";
            listCallees.Size = new System.Drawing.Size(329, 381);
            listCallees.TabIndex = 1;
            listCallees.UseCompatibleStateImageBehavior = false;
            listCallees.View = System.Windows.Forms.View.Details;
            listCallees.DoubleClick += listCallees_DoubleClick;
            // 
            // colCalleeName
            // 
            colCalleeName.Text = "Name";
            colCalleeName.Width = 180;
            // 
            // colCalleeAddress
            // 
            colCalleeAddress.Text = "Address";
            colCalleeAddress.Width = 180;
            // 
            // lblAddress
            // 
            lblAddress.AutoSize = true;
            lblAddress.Location = new System.Drawing.Point(3, 19);
            lblAddress.Name = "lblAddress";
            lblAddress.Size = new System.Drawing.Size(58, 15);
            lblAddress.TabIndex = 2;
            lblAddress.Text = "0000:0000";
            // 
            // linkProcedure
            // 
            linkProcedure.AutoSize = true;
            linkProcedure.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            linkProcedure.Location = new System.Drawing.Point(3, 0);
            linkProcedure.Name = "linkProcedure";
            linkProcedure.Size = new System.Drawing.Size(166, 19);
            linkProcedure.TabIndex = 3;
            linkProcedure.TabStop = true;
            linkProcedure.Text = "<No procedure selected>";
            // 
            // lblSignature
            // 
            lblSignature.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblSignature.Location = new System.Drawing.Point(3, 34);
            lblSignature.Name = "lblSignature";
            lblSignature.Size = new System.Drawing.Size(319, 334);
            lblSignature.TabIndex = 4;
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(lblSignature);
            panel1.Controls.Add(lblAddress);
            panel1.Controls.Add(linkProcedure);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(336, 29);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(327, 381);
            panel1.TabIndex = 5;
            // 
            // lblCallers
            // 
            lblCallers.AutoSize = true;
            lblCallers.Location = new System.Drawing.Point(3, 0);
            lblCallers.Name = "lblCallers";
            lblCallers.Size = new System.Drawing.Size(106, 15);
            lblCallers.TabIndex = 6;
            lblCallers.Text = "Calling procedures";
            // 
            // lblProcedure
            // 
            lblProcedure.AutoSize = true;
            lblProcedure.Location = new System.Drawing.Point(336, 0);
            lblProcedure.Name = "lblProcedure";
            lblProcedure.Size = new System.Drawing.Size(61, 15);
            lblProcedure.TabIndex = 7;
            lblProcedure.Text = "Procedure";
            // 
            // lblCallees
            // 
            lblCallees.AutoSize = true;
            lblCallees.Location = new System.Drawing.Point(669, 0);
            lblCallees.Name = "lblCallees";
            lblCallees.Size = new System.Drawing.Size(102, 15);
            lblCallees.TabIndex = 8;
            lblCallees.Text = "Called procedures";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3333359F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3333359F));
            tableLayoutPanel1.Controls.Add(listCallers, 0, 1);
            tableLayoutPanel1.Controls.Add(listCallees, 2, 1);
            tableLayoutPanel1.Controls.Add(panel1, 1, 1);
            tableLayoutPanel1.Controls.Add(lblCallers, 0, 0);
            tableLayoutPanel1.Controls.Add(lblProcedure, 1, 0);
            tableLayoutPanel1.Controls.Add(lblCallees, 2, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(1001, 413);
            tableLayoutPanel1.TabIndex = 9;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnBack, btnForward });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(1001, 25);
            toolStrip1.TabIndex = 10;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnBack
            // 
            btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnBack.Image = Resources.Back;
            btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnBack.Name = "btnBack";
            btnBack.Size = new System.Drawing.Size(23, 22);
            btnBack.Text = "Back";
            // 
            // btnForward
            // 
            btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnForward.Image = Resources.Forward;
            btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnForward.Name = "btnForward";
            btnForward.Size = new System.Drawing.Size(23, 22);
            btnForward.Text = "Forward";
            // 
            // CallGraphNavigatorView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Controls.Add(toolStrip1);
            Name = "CallGraphNavigatorView";
            Size = new System.Drawing.Size(1001, 438);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView listCallers;
        private System.Windows.Forms.ListView listCallees;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.LinkLabel linkProcedure;
        private System.Windows.Forms.Label lblSignature;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblCallers;
        private System.Windows.Forms.Label lblProcedure;
        private System.Windows.Forms.Label lblCallees;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ColumnHeader colCallerName;
        private System.Windows.Forms.ColumnHeader colCallerAddress;
        private System.Windows.Forms.ColumnHeader colCalleeName;
        private System.Windows.Forms.ColumnHeader colCalleeAddress;
        private System.Windows.Forms.ToolStripButton btnBack;
        private System.Windows.Forms.ToolStripButton btnForward;
    }
}
