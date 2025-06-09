
namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class StackTraceDialog
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
            this.stackTraceListView = new System.Windows.Forms.ListView();
            this.locationColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.descriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // stackTraceListView
            // 
            this.stackTraceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.locationColumnHeader,
            this.descriptionColumnHeader});
            this.stackTraceListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackTraceListView.FullRowSelect = true;
            this.stackTraceListView.HideSelection = false;
            this.stackTraceListView.Location = new System.Drawing.Point(0, 0);
            this.stackTraceListView.Name = "stackTraceListView";
            this.stackTraceListView.Size = new System.Drawing.Size(882, 450);
            this.stackTraceListView.TabIndex = 0;
            this.stackTraceListView.UseCompatibleStateImageBehavior = false;
            this.stackTraceListView.View = System.Windows.Forms.View.Details;
            // 
            // locationColumnHeader
            // 
            this.locationColumnHeader.Text = "";
            this.locationColumnHeader.Width = 30;
            // 
            // descriptionColumnHeader
            // 
            this.descriptionColumnHeader.Text = "Traceback (most recent call last):";
            this.descriptionColumnHeader.Width = 720;
            // 
            // StackTraceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 450);
            this.Controls.Add(this.stackTraceListView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StackTraceDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Stack Trace";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView stackTraceListView;
        private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
        private System.Windows.Forms.ColumnHeader locationColumnHeader;
    }
}