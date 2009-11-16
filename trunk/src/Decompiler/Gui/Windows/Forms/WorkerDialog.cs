using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public partial class WorkerDialog : Form
    {
        public WorkerDialog()
        {
            InitializeComponent();
        }

        public BackgroundWorker Worker
        {
            get { return backgroundWorker; }
        }

        public ProgressBar ProgressBar
        {
            get { return progressBar; }
        }

        public Label Caption
        {
            get { return lblCaption; }
        }

        public Label Detail
        {
            get { return lblDetailText; }
        }

    }
}
