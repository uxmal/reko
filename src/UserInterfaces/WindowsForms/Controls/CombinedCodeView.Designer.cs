namespace Reko.UserInterfaces.WindowsForms.Controls
{
    partial class CombinedCodeView
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CombinedCodeView));
            Reko.Gui.TextViewing.EmptyEditorModel emptyEditorModel1 = new Reko.Gui.TextViewing.EmptyEditorModel();
            Reko.Gui.TextViewing.EmptyEditorModel emptyEditorModel2 = new Reko.Gui.TextViewing.EmptyEditorModel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnBack = new System.Windows.Forms.ToolStripButton();
            this.btnForward = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtAddress = new System.Windows.Forms.ToolStripTextBox();
            this.btnGo = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mixedCodeDataControl = new Reko.UserInterfaces.WindowsForms.Controls.MixedCodeDataControl();
            this.codeTextView = new Reko.UserInterfaces.WindowsForms.Controls.TextView();
            this.previewTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBack,
            this.btnForward,
            this.toolStripLabel1,
            this.txtAddress,
            this.btnGo});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(583, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnBack
            // 
            this.btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBack.Image = Resources.Back;
            this.btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(23, 22);
            this.btnBack.Text = "toolStripButton1";
            this.btnBack.ToolTipText = "Back";
            // 
            // btnForward
            // 
            this.btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnForward.Image = Resources.Forward;
            this.btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(23, 22);
            this.btnForward.Text = "toolStripButton2";
            this.btnForward.ToolTipText = "Forward";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(49, 22);
            this.toolStripLabel1.Text = "Address";
            // 
            // txtAddress
            // 
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(100, 25);
            // 
            // btnGo
            // 
            this.btnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGo.Image = ((System.Drawing.Image)(resources.GetObject("btnGo.Image")));
            this.btnGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(26, 22);
            this.btnGo.Text = "Go";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.mixedCodeDataControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.codeTextView);
            this.splitContainer1.Size = new System.Drawing.Size(583, 357);
            this.splitContainer1.SplitterDistance = 194;
            this.splitContainer1.TabIndex = 2;
            // 
            // mixedCodeDataControl
            // 
            this.mixedCodeDataControl.BackColor = System.Drawing.SystemColors.Window;
            this.mixedCodeDataControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mixedCodeDataControl.Location = new System.Drawing.Point(0, 0);
            this.mixedCodeDataControl.Model = emptyEditorModel1;
            this.mixedCodeDataControl.Name = "mixedCodeDataControl";
            this.mixedCodeDataControl.Program = null;
            this.mixedCodeDataControl.Services = null;
            this.mixedCodeDataControl.Size = new System.Drawing.Size(192, 355);
            this.mixedCodeDataControl.StyleClass = null;
            this.mixedCodeDataControl.TabIndex = 0;
            this.mixedCodeDataControl.Text = "mixedCodeDataControl1";
            this.mixedCodeDataControl.TopAddress = null;
            // 
            // codeTextView
            // 
            this.codeTextView.BackColor = System.Drawing.SystemColors.Window;
            this.codeTextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeTextView.Location = new System.Drawing.Point(0, 0);
            this.codeTextView.Model = emptyEditorModel2;
            this.codeTextView.Name = "codeTextView";
            this.codeTextView.Services = null;
            this.codeTextView.Size = new System.Drawing.Size(383, 355);
            this.codeTextView.StyleClass = null;
            this.codeTextView.TabIndex = 0;
            this.codeTextView.Text = "codeTextView";
            // 
            // previewTimer
            // 
            this.previewTimer.Interval = 500;
            // 
            // CombinedCodeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip);
            this.Name = "CombinedCodeView";
            this.Size = new System.Drawing.Size(583, 382);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnBack;
        private System.Windows.Forms.ToolStripButton btnForward;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txtAddress;
        private System.Windows.Forms.ToolStripButton btnGo;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private TextView codeTextView;
        private MixedCodeDataControl mixedCodeDataControl;
        private System.Windows.Forms.Timer previewTimer;
    }
}
