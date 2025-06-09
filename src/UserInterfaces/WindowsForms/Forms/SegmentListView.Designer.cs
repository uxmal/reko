namespace Reko.UserInterfaces.WindowsForms.Forms
{
    partial class SegmentListView
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
            this.listViewSegments = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colAddress = new System.Windows.Forms.ColumnHeader();
            this.colEnd = new System.Windows.Forms.ColumnHeader();
            this.colLength = new System.Windows.Forms.ColumnHeader();
            this.colRead = new System.Windows.Forms.ColumnHeader();
            this.colWrite = new System.Windows.Forms.ColumnHeader();
            this.colExecute = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listViewSegments
            // 
            this.listViewSegments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewSegments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colAddress,
            this.colEnd,
            this.colLength,
            this.colRead,
            this.colWrite,
            this.colExecute});
            this.listViewSegments.HideSelection = false;
            this.listViewSegments.Location = new System.Drawing.Point(3, 3);
            this.listViewSegments.Name = "listViewSegments";
            this.listViewSegments.Size = new System.Drawing.Size(848, 438);
            this.listViewSegments.TabIndex = 0;
            this.listViewSegments.UseCompatibleStateImageBehavior = false;
            this.listViewSegments.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 140;
            // 
            // colAddress
            // 
            this.colAddress.Text = "Address";
            this.colAddress.Width = 120;
            // 
            // colEnd
            // 
            this.colEnd.Text = "End";
            this.colEnd.Width = 120;
            // 
            // colLength
            // 
            this.colLength.Text = "Length";
            this.colLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colLength.Width = 80;
            // 
            // colRead
            // 
            this.colRead.Text = "R";
            this.colRead.Width = 25;
            // 
            // colWrite
            // 
            this.colWrite.Text = "W";
            this.colWrite.Width = 25;
            // 
            // colExecute
            // 
            this.colExecute.Text = "X";
            this.colExecute.Width = 25;
            // 
            // SegmentListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewSegments);
            this.Name = "SegmentListView";
            this.Size = new System.Drawing.Size(854, 444);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewSegments;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colAddress;
        private System.Windows.Forms.ColumnHeader colEnd;
        private System.Windows.Forms.ColumnHeader colLength;
        private System.Windows.Forms.ColumnHeader colRead;
        private System.Windows.Forms.ColumnHeader colWrite;
        private System.Windows.Forms.ColumnHeader colExecute;
    }
}
