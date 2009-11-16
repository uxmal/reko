using Decompiler.Configuration;
using Decompiler.Core;
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class WindowsDecompilerEventListener : IWorkerDialogService, DecompilerEventListener
    {
        private WorkerDialog dlg;
        private ThreadStart worker;
        private IDecompilerUIService uiSvc;
        private IDiagnosticsService diagnosticService;

        private string status;

        public WindowsDecompilerEventListener(IServiceProvider sp)
        {
            uiSvc = (IDecompilerUIService)sp.GetService(typeof(IDecompilerUIService));
            diagnosticService = (IDiagnosticsService)sp.GetService(typeof(IDiagnosticsService));
        }

        private WorkerDialog CreateDialog(string caption)
        {
            if (dlg != null)
                throw new InvalidOperationException("Dialog is already running.");
            this.dlg = new WorkerDialog();

            dlg.Load += new EventHandler(dlg_Load);
            dlg.Closed += new EventHandler(dlg_Closed);
            dlg.Worker.WorkerSupportsCancellation = true;
            dlg.Worker.WorkerReportsProgress = true;
            dlg.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            dlg.Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            dlg.Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            dlg.DialogResult = DialogResult.Cancel;
            dlg.Caption.Text = caption;
            return dlg;
        }

        public bool StartBackgroundWork(string caption, ThreadStart backgroundworker)
        {
            try
            {
                this.worker = backgroundworker;
                using (dlg = CreateDialog(caption))
                {
                    return uiSvc.ShowModalDialog(dlg) == DialogResult.OK;
                }
            }
            finally
            {
                dlg = null;
            }
        }

        void dlg_Load(object sender, EventArgs e)
        {
            dlg.Worker.RunWorkerAsync(worker);
        }

        private void dlg_Closed(object sender, EventArgs e)
        {
            dlg.Worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            dlg.Worker.ProgressChanged -= new ProgressChangedEventHandler(Worker_ProgressChanged);
            dlg = null;
            worker = null;
        }


        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ThreadStart worker = (ThreadStart)e.Argument;
            worker();
        }



        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (dlg == null)
                return;
            dlg.ProgressBar.Value = e.ProgressPercentage;
            dlg.Detail.Text = status;
        }


        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                dlg.DialogResult = DialogResult.Cancel;
            }
            else if (e.Error != null)
            {
                //$REVIEW: drops it on the floor.;
            }
            else
            {
                dlg.DialogResult = DialogResult.OK;
            }
            dlg.Close();
        }


        #region DecompilerEventListener Members

        void DecompilerEventListener.ProgramLoaded()
        {
            Interlocked.Exchange<string>(ref status, "Source program loaded.");
            dlg.Worker.ReportProgress(100);
        }

        void DecompilerEventListener.ProgramScanned()
        {
            Interlocked.Exchange<string>(ref status, "Program scanned.");
            dlg.Worker.ReportProgress(100);
        }

        void DecompilerEventListener.MachineCodeRewritten()
        {
            Interlocked.Exchange<string>(ref status, "Intermediate code generated.");
            dlg.Worker.ReportProgress(100);
        }

        void DecompilerEventListener.InterproceduralAnalysisComplete()
        {
            Interlocked.Exchange<string>(ref status, "Interprocedural analysis complete.");
            dlg.Worker.ReportProgress(100);
        }

        void DecompilerEventListener.ProceduresTransformed()
        {
            Interlocked.Exchange<string>(ref status, "Interprocedural analysis complete.");
            dlg.Worker.ReportProgress(100);
        }

        void DecompilerEventListener.TypeReconstructionComplete()
        {
            throw new NotImplementedException();
        }

        void DecompilerEventListener.CodeStructuringComplete()
        {
            throw new NotImplementedException();
        }

        void DecompilerEventListener.DecompilationFinished()
        {
            throw new NotImplementedException();
        }

        void DecompilerEventListener.WriteDiagnostic(Decompiler.Core.DiagnosticOld d, Decompiler.Core.Address addr, string format, params object[] args)
        {
            Diagnostic di = null;
            switch (d)
            {
                case DiagnosticOld.Error:
                case DiagnosticOld.FatalError:
                    di = new ErrorDiagnostic(addr, format, args);
                    break;
                case DiagnosticOld.Warning:
                    di = new WarningDiagnostic(addr, format, args);
                    break;
                case DiagnosticOld.Info:
                    status = string.Format(format, args);
                    dlg.Worker.ReportProgress(0);
                    return;
            }
            dlg.Invoke(new Action<Diagnostic>(diagnosticService.AddDiagnostic), di);
        }

        void DecompilerEventListener.ShowProgress(string caption, int numerator, int denominator)
        {
            status = caption;
            dlg.Worker.ReportProgress((int)((numerator * 100L) / denominator));
        }

        #endregion
    }
}
