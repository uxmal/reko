namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class KeyBindingsDialog
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
            this.txtCommandName = new System.Windows.Forms.TextBox();
            this.listCommands = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ddlCommandShortcuts = new System.Windows.Forms.ComboBox();
            this.ddlWindows = new System.Windows.Forms.ComboBox();
            this.txtShortCut = new System.Windows.Forms.TextBox();
            this.btnAssign = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.ddlShortcutCommands = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Find commands whose name contains:";
            // 
            // txtCommandName
            // 
            this.txtCommandName.Location = new System.Drawing.Point(12, 30);
            this.txtCommandName.Name = "txtCommandName";
            this.txtCommandName.Size = new System.Drawing.Size(482, 20);
            this.txtCommandName.TabIndex = 1;
            // 
            // listCommands
            // 
            this.listCommands.FormattingEnabled = true;
            this.listCommands.Location = new System.Drawing.Point(12, 57);
            this.listCommands.Name = "listCommands";
            this.listCommands.Size = new System.Drawing.Size(482, 69);
            this.listCommands.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Shortcut for the command:";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(420, 148);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "&Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "&Use the shortcut in:";
            // 
            // ddlCommandShortcuts
            // 
            this.ddlCommandShortcuts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlCommandShortcuts.FormattingEnabled = true;
            this.ddlCommandShortcuts.Location = new System.Drawing.Point(12, 150);
            this.ddlCommandShortcuts.Name = "ddlCommandShortcuts";
            this.ddlCommandShortcuts.Size = new System.Drawing.Size(402, 21);
            this.ddlCommandShortcuts.TabIndex = 3;
            // 
            // ddlWindows
            // 
            this.ddlWindows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlWindows.FormattingEnabled = true;
            this.ddlWindows.Location = new System.Drawing.Point(12, 193);
            this.ddlWindows.Name = "ddlWindows";
            this.ddlWindows.Size = new System.Drawing.Size(213, 21);
            this.ddlWindows.TabIndex = 5;
            // 
            // txtShortCut
            // 
            this.txtShortCut.Location = new System.Drawing.Point(236, 193);
            this.txtShortCut.Name = "txtShortCut";
            this.txtShortCut.Size = new System.Drawing.Size(178, 20);
            this.txtShortCut.TabIndex = 6;
            // 
            // btnAssign
            // 
            this.btnAssign.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAssign.Location = new System.Drawing.Point(420, 191);
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.Size = new System.Drawing.Size(75, 23);
            this.btnAssign.TabIndex = 7;
            this.btnAssign.Text = "&Assign";
            this.btnAssign.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 222);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Shortcut currenty used  by";
            // 
            // ddlShortcutCommands
            // 
            this.ddlShortcutCommands.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlShortcutCommands.FormattingEnabled = true;
            this.ddlShortcutCommands.Location = new System.Drawing.Point(12, 238);
            this.ddlShortcutCommands.Name = "ddlShortcutCommands";
            this.ddlShortcutCommands.Size = new System.Drawing.Size(483, 21);
            this.ddlShortcutCommands.TabIndex = 8;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(339, 273);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(420, 273);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(236, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "&Press shortcut keys";
            // 
            // KeyBindingsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(507, 308);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.ddlShortcutCommands);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnAssign);
            this.Controls.Add(this.txtShortCut);
            this.Controls.Add(this.ddlWindows);
            this.Controls.Add(this.ddlCommandShortcuts);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listCommands);
            this.Controls.Add(this.txtCommandName);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyBindingsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Key Bindings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCommandName;
        private System.Windows.Forms.ListBox listCommands;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ddlCommandShortcuts;
        private System.Windows.Forms.ComboBox ddlWindows;
        private System.Windows.Forms.TextBox txtShortCut;
        private System.Windows.Forms.Button btnAssign;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ddlShortcutCommands;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label5;
    }
}