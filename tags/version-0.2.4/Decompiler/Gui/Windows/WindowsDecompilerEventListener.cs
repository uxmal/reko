#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Configuration;
using Decompiler.Core;
using Decompiler.Core.Services;
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Implements the IWorkerDialogService and DecompilerEventListener services for the Windows Forms GUI.
    /// </summary>
    public class WindowsDecompilerEventListener : IWorkerDialogService, DecompilerEventListener
    {
        private WorkerDialog dlg;
        private Action task;
        private IServiceProvider sp;
        private IDecompilerUIService uiSvc;
        private IDiagnosticsService diagnosticSvc;
        private Exception lastException;

        private string status;
        private const int STATUS_UPDATE_ONLY = -4711;

        public WindowsDecompilerEventListener(IServiceProvider sp)
        {
            this.sp = sp;
            uiSvc = sp.GetService<IDecompilerUIService>();
            diagnosticSvc = sp.GetService<IDiagnosticsService>();
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

        /// <summary>
        /// The UI thread requests a background operation by calling this method.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="backgroundTask"></param>
        /// <returns></returns>
        public bool StartBackgroundWork(string caption, Action backgroundTask)
        {
            lastException = null;
            try
            {
                this.task = backgroundTask;
                using (dlg = CreateDialog(caption))
                {
                    if (uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                        return true;
                    if (lastException != null)
                        uiSvc.ShowError(lastException, "{0}", caption);
                    return false;
                }
            }
            finally
            {
                dlg = null;
            }
        }

        public void SetCaption(string newCaption)
        {
            dlg.Invoke(new Action<string>(delegate(string c)
                {
                    dlg.Caption.Text = c;
                }),
                newCaption);
        }

        public void ShowError(string failedOperation, Exception ex)
        {
            dlg.Invoke(new Action<Exception>(delegate(Exception exc)
                {
                    uiSvc.ShowError(ex, failedOperation);
                }),
                ex);
        }

        public void FinishBackgroundWork()
        {
        }

        void dlg_Load(object sender, EventArgs e)
        {
            dlg.Worker.RunWorkerAsync(task);
        }

        private void dlg_Closed(object sender, EventArgs e)
        {
            dlg.Worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            dlg.Worker.ProgressChanged -= new ProgressChangedEventHandler(Worker_ProgressChanged);
            task = null;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Action worker = (Action)e.Argument;
                worker();
            }
            catch (AddressCorrelatedException acex)
            {
                AddDiagnostic(CreateAddressNavigator(acex.Address), new ErrorDiagnostic(acex.Message));
            }
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (dlg == null)
                return;
            if (e.ProgressPercentage != STATUS_UPDATE_ONLY)
            {
                dlg.ProgressBar.Value = e.ProgressPercentage;
            }
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
                lastException = e.Error;
                dlg.DialogResult = DialogResult.Cancel;
            }
            else
            {
                dlg.DialogResult = DialogResult.OK;
            }
            dlg.Close();
        }


        #region DecompilerEventListener Members

        public void AddDiagnostic(ICodeLocation location, Diagnostic d)
        {
            if (dlg != null)
                dlg.Invoke(new Action<ICodeLocation, Diagnostic>(diagnosticSvc.AddDiagnostic), location, d);
            else
                diagnosticSvc.AddDiagnostic(location, d);
                    
        }

        void DecompilerEventListener.ShowStatus(string caption)
        {
            ShowStatus(caption);
        }

        public ICodeLocation CreateAddressNavigator(Address addr)
        {
            return new AddressNavigator(addr, sp);
        }

        ICodeLocation DecompilerEventListener.CreateBlockNavigator(Block block)
        {
            return new BlockNavigator(block, sp);
        }

        ICodeLocation DecompilerEventListener.CreateProcedureNavigator(Procedure proc)
        {
            return new ProcedureNavigator(proc, sp);
        }


        private void ShowStatus(string newStatus)
        {
            if (dlg == null)
                return;
            System.Threading.Interlocked.Exchange<string>(ref status, newStatus);
            dlg.Worker.ReportProgress(STATUS_UPDATE_ONLY);
        }

        void DecompilerEventListener.ShowProgress(string caption, int numerator, int denominator)
        {
            if (dlg == null)
                return;
            System.Threading.Interlocked.Exchange<string>(ref status, caption);
            dlg.Worker.ReportProgress((int)((numerator * 100L) / denominator));
        }

        #endregion
    }
}
