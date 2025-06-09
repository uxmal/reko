namespace Reko.WindowsItp
{
    partial class SuffixArrayPerformanceDialog
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
            this.button1 = new System.Windows.Forms.Button();
            this.lblSuffixArrayComputation = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSASequence = new System.Windows.Forms.Label();
            this.txtText = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.lblLcp = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 122);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Compute suffix array";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblSuffixArrayComputation
            // 
            this.lblSuffixArrayComputation.AutoSize = true;
            this.lblSuffixArrayComputation.Location = new System.Drawing.Point(12, 148);
            this.lblSuffixArrayComputation.Name = "lblSuffixArrayComputation";
            this.lblSuffixArrayComputation.Size = new System.Drawing.Size(35, 13);
            this.lblSuffixArrayComputation.TabIndex = 1;
            this.lblSuffixArrayComputation.Text = "label1";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 96);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(132, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "1000000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Array size:";
            // 
            // lblSASequence
            // 
            this.lblSASequence.Location = new System.Drawing.Point(12, 171);
            this.lblSASequence.Name = "lblSASequence";
            this.lblSASequence.Size = new System.Drawing.Size(237, 104);
            this.lblSASequence.TabIndex = 4;
            this.lblSASequence.Text = "label2";
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(12, 13);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(100, 20);
            this.txtText.TabIndex = 5;
            this.txtText.Text = "banana";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Type 1",
            "Type 2"});
            this.comboBox1.Location = new System.Drawing.Point(12, 56);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(132, 21);
            this.comboBox1.TabIndex = 6;
            // 
            // lblLcp
            // 
            this.lblLcp.Location = new System.Drawing.Point(255, 171);
            this.lblLcp.Name = "lblLcp";
            this.lblLcp.Size = new System.Drawing.Size(237, 104);
            this.lblLcp.TabIndex = 7;
            this.lblLcp.Text = "label2";
            // 
            // SuffixArrayPerformanceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 284);
            this.Controls.Add(this.lblLcp);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.lblSASequence);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lblSuffixArrayComputation);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SuffixArrayPerformanceDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SuffixArrayPerformanceDialog";
            this.Load += new System.EventHandler(this.SuffixArrayPerformanceDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblSuffixArrayComputation;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSASequence;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lblLcp;
    }
}