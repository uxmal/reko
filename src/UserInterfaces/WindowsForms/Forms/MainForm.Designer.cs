namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            statusStrip = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            statusLblText = new System.Windows.Forms.ToolStripStatusLabel();
            statusLblSubText = new System.Windows.Forms.ToolStripStatusLabel();
            statusProgress = new System.Windows.Forms.ToolStripProgressBar();
            statusLblSelection = new System.Windows.Forms.ToolStripStatusLabel();
            treeBrowser = new System.Windows.Forms.TreeView();
            imlBrowser = new System.Windows.Forms.ImageList(components);
            tabControl1 = new System.Windows.Forms.TabControl();
            tabDiagnostics = new System.Windows.Forms.TabPage();
            listDiagnostics = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            imageList = new System.Windows.Forms.ImageList(components);
            diagnosticsToolstrip = new System.Windows.Forms.ToolStrip();
            btnDiagnosticFilter = new System.Windows.Forms.ToolStripButton();
            tabFindResults = new System.Windows.Forms.TabPage();
            listFindResults = new System.Windows.Forms.ListView();
            tabCallGraphNavigator = new System.Windows.Forms.TabPage();
            callGraphNavigatorView = new CallGraphNavigatorView();
            tabConsole = new System.Windows.Forms.TabPage();
            tabOutput = new System.Windows.Forms.TabPage();
            outputWindowPanel = new System.Windows.Forms.Panel();
            outputWindowToolstrip = new System.Windows.Forms.ToolStrip();
            ddlOutputWindowSources = new System.Windows.Forms.ToolStripComboBox();
            ofd = new System.Windows.Forms.OpenFileDialog();
            sfd = new System.Windows.Forms.SaveFileDialog();
            splitContainerMain = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            tabControl2 = new System.Windows.Forms.TabControl();
            tabProject = new System.Windows.Forms.TabPage();
            tabProcedures = new System.Windows.Forms.TabPage();
            procedureListPanel = new ProcedureListPanel();
            tabDocuments = new System.Windows.Forms.TabControl();
            statusStrip.SuspendLayout();
            tabControl1.SuspendLayout();
            tabDiagnostics.SuspendLayout();
            diagnosticsToolstrip.SuspendLayout();
            tabFindResults.SuspendLayout();
            tabCallGraphNavigator.SuspendLayout();
            tabOutput.SuspendLayout();
            outputWindowToolstrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            tabControl2.SuspendLayout();
            tabProject.SuspendLayout();
            tabProcedures.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel, statusLblText, statusLblSubText, statusProgress, statusLblSelection });
            statusStrip.Location = new System.Drawing.Point(0, 593);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 17, 0);
            statusStrip.Size = new System.Drawing.Size(914, 22);
            statusStrip.TabIndex = 2;
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // statusLblText
            // 
            statusLblText.Name = "statusLblText";
            statusLblText.Size = new System.Drawing.Size(0, 17);
            // 
            // statusLblSubText
            // 
            statusLblSubText.Name = "statusLblSubText";
            statusLblSubText.Size = new System.Drawing.Size(0, 17);
            // 
            // statusProgress
            // 
            statusProgress.Name = "statusProgress";
            statusProgress.Size = new System.Drawing.Size(100, 16);
            statusProgress.Visible = false;
            // 
            // statusLblSelection
            // 
            statusLblSelection.Name = "statusLblSelection";
            statusLblSelection.Size = new System.Drawing.Size(0, 17);
            // 
            // treeBrowser
            // 
            treeBrowser.AllowDrop = true;
            treeBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            treeBrowser.ImageIndex = 0;
            treeBrowser.ImageList = imlBrowser;
            treeBrowser.Location = new System.Drawing.Point(4, 4);
            treeBrowser.Margin = new System.Windows.Forms.Padding(4);
            treeBrowser.Name = "treeBrowser";
            treeBrowser.SelectedImageIndex = 0;
            treeBrowser.Size = new System.Drawing.Size(191, 344);
            treeBrowser.TabIndex = 0;
            treeBrowser.GotFocus += treeBrowser_GotFocus;
            // 
            // imlBrowser
            // 
            imlBrowser.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            imlBrowser.ImageStream = (System.Windows.Forms.ImageListStreamer) resources.GetObject("imlBrowser.ImageStream");
            imlBrowser.TransparentColor = System.Drawing.Color.Transparent;
            imlBrowser.Images.SetKeyName(0, "Binary.ico");
            imlBrowser.Images.SetKeyName(1, "Userproc.ico");
            imlBrowser.Images.SetKeyName(2, "Data.ico");
            imlBrowser.Images.SetKeyName(3, "Procedure.ico");
            imlBrowser.Images.SetKeyName(4, "typelib.ico");
            imlBrowser.Images.SetKeyName(5, "RoSection.ico");
            imlBrowser.Images.SetKeyName(6, "RwSection.ico");
            imlBrowser.Images.SetKeyName(7, "RxSection.ico");
            imlBrowser.Images.SetKeyName(8, "WxSection.ico");
            imlBrowser.Images.SetKeyName(9, "DiscardableSection.ico");
            imlBrowser.Images.SetKeyName(10, "Platform.ico");
            imlBrowser.Images.SetKeyName(11, "Cpu.ico");
            imlBrowser.Images.SetKeyName(12, "EntryProcedure.ico");
            imlBrowser.Images.SetKeyName(13, "UserEntryProcedure.ico");
            imlBrowser.Images.SetKeyName(14, "Script.py.ico");
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabDiagnostics);
            tabControl1.Controls.Add(tabFindResults);
            tabControl1.Controls.Add(tabCallGraphNavigator);
            tabControl1.Controls.Add(tabConsole);
            tabControl1.Controls.Add(tabOutput);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Margin = new System.Windows.Forms.Padding(4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(914, 209);
            tabControl1.TabIndex = 5;
            // 
            // tabDiagnostics
            // 
            tabDiagnostics.Controls.Add(listDiagnostics);
            tabDiagnostics.Controls.Add(diagnosticsToolstrip);
            tabDiagnostics.Location = new System.Drawing.Point(4, 24);
            tabDiagnostics.Margin = new System.Windows.Forms.Padding(4);
            tabDiagnostics.Name = "tabDiagnostics";
            tabDiagnostics.Padding = new System.Windows.Forms.Padding(4);
            tabDiagnostics.Size = new System.Drawing.Size(906, 181);
            tabDiagnostics.TabIndex = 1;
            tabDiagnostics.Text = "Diagnostics";
            tabDiagnostics.UseVisualStyleBackColor = true;
            // 
            // listDiagnostics
            // 
            listDiagnostics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2 });
            listDiagnostics.Dock = System.Windows.Forms.DockStyle.Fill;
            listDiagnostics.FullRowSelect = true;
            listDiagnostics.Location = new System.Drawing.Point(4, 29);
            listDiagnostics.Margin = new System.Windows.Forms.Padding(4);
            listDiagnostics.Name = "listDiagnostics";
            listDiagnostics.Size = new System.Drawing.Size(898, 148);
            listDiagnostics.SmallImageList = imageList;
            listDiagnostics.TabIndex = 2;
            listDiagnostics.UseCompatibleStateImageBehavior = false;
            listDiagnostics.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Address";
            columnHeader1.Width = 95;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Description";
            columnHeader2.Width = 624;
            // 
            // imageList
            // 
            imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            imageList.ImageStream = (System.Windows.Forms.ImageListStreamer) resources.GetObject("imageList.ImageStream");
            imageList.TransparentColor = System.Drawing.Color.Transparent;
            imageList.Images.SetKeyName(0, "Open.ico");
            imageList.Images.SetKeyName(1, "Save.ico");
            imageList.Images.SetKeyName(2, "RestartDecompilation.ico");
            imageList.Images.SetKeyName(3, "NextPhase.ico");
            imageList.Images.SetKeyName(4, "FinishDecompilation.ico");
            imageList.Images.SetKeyName(5, "Error");
            imageList.Images.SetKeyName(6, "Warning");
            imageList.Images.SetKeyName(7, "Info");
            imageList.Images.SetKeyName(8, "CloseTab");
            imageList.Images.SetKeyName(9, "Collapse.ico");
            imageList.Images.SetKeyName(10, "CreateSegment.ico");
            // 
            // diagnosticsToolstrip
            // 
            diagnosticsToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnDiagnosticFilter });
            diagnosticsToolstrip.Location = new System.Drawing.Point(4, 4);
            diagnosticsToolstrip.Name = "diagnosticsToolstrip";
            diagnosticsToolstrip.Size = new System.Drawing.Size(898, 25);
            diagnosticsToolstrip.TabIndex = 3;
            diagnosticsToolstrip.Text = "toolStrip1";
            // 
            // btnDiagnosticFilter
            // 
            btnDiagnosticFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnDiagnosticFilter.Image = (System.Drawing.Image) resources.GetObject("btnDiagnosticFilter.Image");
            btnDiagnosticFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnDiagnosticFilter.Name = "btnDiagnosticFilter";
            btnDiagnosticFilter.Size = new System.Drawing.Size(23, 22);
            btnDiagnosticFilter.Text = "Filter messages";
            // 
            // tabFindResults
            // 
            tabFindResults.Controls.Add(listFindResults);
            tabFindResults.Location = new System.Drawing.Point(4, 24);
            tabFindResults.Margin = new System.Windows.Forms.Padding(4);
            tabFindResults.Name = "tabFindResults";
            tabFindResults.Padding = new System.Windows.Forms.Padding(4);
            tabFindResults.Size = new System.Drawing.Size(906, 181);
            tabFindResults.TabIndex = 0;
            tabFindResults.Text = "Find results";
            tabFindResults.UseVisualStyleBackColor = true;
            // 
            // listFindResults
            // 
            listFindResults.Dock = System.Windows.Forms.DockStyle.Fill;
            listFindResults.FullRowSelect = true;
            listFindResults.Location = new System.Drawing.Point(4, 4);
            listFindResults.Margin = new System.Windows.Forms.Padding(4);
            listFindResults.Name = "listFindResults";
            listFindResults.Size = new System.Drawing.Size(898, 173);
            listFindResults.TabIndex = 0;
            listFindResults.UseCompatibleStateImageBehavior = false;
            // 
            // tabCallGraphNavigator
            // 
            tabCallGraphNavigator.Controls.Add(callGraphNavigatorView);
            tabCallGraphNavigator.Location = new System.Drawing.Point(4, 24);
            tabCallGraphNavigator.Name = "tabCallGraphNavigator";
            tabCallGraphNavigator.Padding = new System.Windows.Forms.Padding(3);
            tabCallGraphNavigator.Size = new System.Drawing.Size(906, 181);
            tabCallGraphNavigator.TabIndex = 5;
            tabCallGraphNavigator.Text = "Call Graph Navigator";
            tabCallGraphNavigator.UseVisualStyleBackColor = true;
            // 
            // callGraphNavigatorView
            // 
            callGraphNavigatorView.Dock = System.Windows.Forms.DockStyle.Fill;
            callGraphNavigatorView.Location = new System.Drawing.Point(3, 3);
            callGraphNavigatorView.Name = "callGraphNavigatorView";
            callGraphNavigatorView.Size = new System.Drawing.Size(900, 175);
            callGraphNavigatorView.TabIndex = 0;
            callGraphNavigatorView.ViewModel = null;
            // 
            // tabConsole
            // 
            tabConsole.Location = new System.Drawing.Point(4, 24);
            tabConsole.Margin = new System.Windows.Forms.Padding(4);
            tabConsole.Name = "tabConsole";
            tabConsole.Size = new System.Drawing.Size(906, 181);
            tabConsole.TabIndex = 2;
            tabConsole.Text = "Console";
            tabConsole.UseVisualStyleBackColor = true;
            // 
            // tabOutput
            // 
            tabOutput.Controls.Add(outputWindowPanel);
            tabOutput.Controls.Add(outputWindowToolstrip);
            tabOutput.Location = new System.Drawing.Point(4, 24);
            tabOutput.Margin = new System.Windows.Forms.Padding(4);
            tabOutput.Name = "tabOutput";
            tabOutput.Padding = new System.Windows.Forms.Padding(4);
            tabOutput.Size = new System.Drawing.Size(906, 181);
            tabOutput.TabIndex = 4;
            tabOutput.Text = "Output";
            tabOutput.UseVisualStyleBackColor = true;
            // 
            // outputWindowPanel
            // 
            outputWindowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            outputWindowPanel.Location = new System.Drawing.Point(4, 29);
            outputWindowPanel.Name = "outputWindowPanel";
            outputWindowPanel.Size = new System.Drawing.Size(898, 148);
            outputWindowPanel.TabIndex = 2;
            // 
            // outputWindowToolstrip
            // 
            outputWindowToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { ddlOutputWindowSources });
            outputWindowToolstrip.Location = new System.Drawing.Point(4, 4);
            outputWindowToolstrip.Name = "outputWindowToolstrip";
            outputWindowToolstrip.Size = new System.Drawing.Size(898, 25);
            outputWindowToolstrip.TabIndex = 1;
            outputWindowToolstrip.Text = "toolStrip1";
            // 
            // ddlOutputWindowSources
            // 
            ddlOutputWindowSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            ddlOutputWindowSources.Name = "ddlOutputWindowSources";
            ddlOutputWindowSources.Size = new System.Drawing.Size(121, 25);
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerMain.Location = new System.Drawing.Point(0, 0);
            splitContainerMain.Margin = new System.Windows.Forms.Padding(4);
            splitContainerMain.Name = "splitContainerMain";
            splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(tabControl1);
            splitContainerMain.Size = new System.Drawing.Size(914, 593);
            splitContainerMain.SplitterDistance = 380;
            splitContainerMain.TabIndex = 9;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Margin = new System.Windows.Forms.Padding(4);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(tabControl2);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(tabDocuments);
            splitContainer2.Size = new System.Drawing.Size(914, 380);
            splitContainer2.SplitterDistance = 207;
            splitContainer2.TabIndex = 0;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(tabProject);
            tabControl2.Controls.Add(tabProcedures);
            tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl2.Location = new System.Drawing.Point(0, 0);
            tabControl2.Margin = new System.Windows.Forms.Padding(4);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new System.Drawing.Size(207, 380);
            tabControl2.TabIndex = 1;
            // 
            // tabProject
            // 
            tabProject.Controls.Add(treeBrowser);
            tabProject.Location = new System.Drawing.Point(4, 24);
            tabProject.Margin = new System.Windows.Forms.Padding(4);
            tabProject.Name = "tabProject";
            tabProject.Padding = new System.Windows.Forms.Padding(4);
            tabProject.Size = new System.Drawing.Size(199, 352);
            tabProject.TabIndex = 0;
            tabProject.Text = "Project";
            tabProject.UseVisualStyleBackColor = true;
            // 
            // tabProcedures
            // 
            tabProcedures.Controls.Add(procedureListPanel);
            tabProcedures.Location = new System.Drawing.Point(4, 24);
            tabProcedures.Margin = new System.Windows.Forms.Padding(4);
            tabProcedures.Name = "tabProcedures";
            tabProcedures.Padding = new System.Windows.Forms.Padding(4);
            tabProcedures.Size = new System.Drawing.Size(199, 352);
            tabProcedures.TabIndex = 1;
            tabProcedures.Text = "Procedures";
            tabProcedures.UseVisualStyleBackColor = true;
            // 
            // procedureListPanel
            // 
            procedureListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            procedureListPanel.Location = new System.Drawing.Point(4, 4);
            procedureListPanel.Name = "procedureListPanel";
            procedureListPanel.Size = new System.Drawing.Size(191, 344);
            procedureListPanel.TabIndex = 0;
            // 
            // tabDocuments
            // 
            tabDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            tabDocuments.ImageList = imageList;
            tabDocuments.Location = new System.Drawing.Point(0, 0);
            tabDocuments.Margin = new System.Windows.Forms.Padding(4);
            tabDocuments.Name = "tabDocuments";
            tabDocuments.SelectedIndex = 0;
            tabDocuments.Size = new System.Drawing.Size(703, 380);
            tabDocuments.TabIndex = 0;
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(914, 615);
            Controls.Add(splitContainerMain);
            Controls.Add(statusStrip);
            Icon = (System.Drawing.Icon) resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "MainForm";
            Text = "Reko Decompiler";
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabDiagnostics.ResumeLayout(false);
            tabDiagnostics.PerformLayout();
            diagnosticsToolstrip.ResumeLayout(false);
            diagnosticsToolstrip.PerformLayout();
            tabFindResults.ResumeLayout(false);
            tabCallGraphNavigator.ResumeLayout(false);
            tabOutput.ResumeLayout(false);
            tabOutput.PerformLayout();
            outputWindowToolstrip.ResumeLayout(false);
            outputWindowToolstrip.PerformLayout();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            tabProject.ResumeLayout(false);
            tabProcedures.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TreeView treeBrowser;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabFindResults;
        private System.Windows.Forms.TabPage tabDiagnostics;
        private System.Windows.Forms.ListView listFindResults;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ListView listDiagnostics;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ImageList imlBrowser;
        private System.Windows.Forms.TabPage tabConsole;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabControl tabDocuments;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabProject;
        private System.Windows.Forms.TabPage tabProcedures;
        private System.Windows.Forms.TabPage tabOutput;
        private System.Windows.Forms.ToolStrip outputWindowToolstrip;
        private System.Windows.Forms.ToolStripComboBox ddlOutputWindowSources;
        private System.Windows.Forms.Panel outputWindowPanel;
        private System.Windows.Forms.ToolStrip diagnosticsToolstrip;
        private System.Windows.Forms.ToolStripButton btnDiagnosticFilter;
        private System.Windows.Forms.ToolStripStatusLabel statusLblText;
        private System.Windows.Forms.ToolStripStatusLabel statusLblSubText;
        private System.Windows.Forms.ToolStripProgressBar statusProgress;
        private System.Windows.Forms.ToolStripStatusLabel statusLblSelection;
        private System.Windows.Forms.TabPage tabCallGraphNavigator;
        private CallGraphNavigatorView callGraphNavigatorView;
        private ProcedureListPanel procedureListPanel;
    }
}



