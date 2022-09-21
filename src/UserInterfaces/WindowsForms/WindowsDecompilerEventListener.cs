#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Core;
using Reko.Core.Scripts;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Implements the IWorkerDialogService and DecompilerEventListener services for the Windows Forms GUI.
    /// </summary>
    public class WindowsDecompilerEventListener : IWorkerDialogService, DecompilerEventListener
    {
        private IWorkerDialog dlg;
        private Action task;
        private IServiceProvider sp;
        private IDecompilerShellUiService uiSvc;
        private IDiagnosticsService diagnosticSvc;
        private Exception lastException;

        private string status;
        private const int STATUS_UPDATE_ONLY = -4711;
        private CancellationTokenSource cancellationSvc;
        private bool isCanceled;
        private int position;
        private int total;

        public WindowsDecompilerEventListener(IServiceProvider sp)
        {
            this.sp = sp;
            uiSvc = sp.GetService<IDecompilerShellUiService>();
            diagnosticSvc = sp.GetService<IDiagnosticsService>();
        }

        private IWorkerDialog CreateDialog(string caption)
        {
            if (dlg != null)
                throw new InvalidOperationException("Dialog is already running.");
            this.dlg = sp.RequireService<IDialogFactory>().CreateWorkerDialog();
            this.cancellationSvc = new CancellationTokenSource();
            this.sp.RequireService<IServiceContainer>().AddService<CancellationTokenSource>(cancellationSvc);
            dlg.Load += new EventHandler(dlg_Load);
            dlg.Closed += new EventHandler(dlg_Closed);
            dlg.CancellationButton.Click += dlg_Cancelled;
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
        public async ValueTask<bool> StartBackgroundWork(string caption, Action backgroundTask)
        {
            lastException = null;
            try
            {
                this.task = backgroundTask;
                using (dlg = CreateDialog(caption))
                {
                    if (await uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                        return true;
                    if (lastException != null)
                        await uiSvc.ShowError(lastException, "{0}", caption);
                    return false;
                }
            }
            finally
            {
                dlg = null;
            }
        }

        /// <summary>
        /// Intended to be call by a worker thread; uses Invoke to make sure the delegate is invoked
        /// on the UI thread.
        /// </summary>
        /// <param name="newCaption"></param>
        public void SetCaption(string newCaption)
        {
            dlg.Invoke(() => { dlg.Caption.Text = newCaption; });
        }

        public void ShowError(string failedOperation, Exception ex)
        {
            dlg.Invoke(() => { uiSvc.ShowError(ex, failedOperation); });
        }

        public void FinishBackgroundWork()
        {
        }

        void dlg_Load(object sender, EventArgs e)
        {
            this.isCanceled = false;
            dlg.CancellationButton.Enabled = true;
            dlg.Worker.RunWorkerAsync(task);
        }

        private void dlg_Closed(object sender, EventArgs e)
        {
            dlg.Worker.RunWorkerCompleted -= Worker_RunWorkerCompleted;
            dlg.Worker.ProgressChanged -= Worker_ProgressChanged;
            task = null;
            sp.RequireService<IServiceContainer>().RemoveService(typeof(CancellationTokenSource));
        }

        private void dlg_Cancelled(object sender, EventArgs e)
        {
            cancellationSvc.Cancel();
            this.isCanceled = true;
            Warn(new NullCodeLocation(""), "User canceled the decompiler.");
            dlg.CancellationButton.Enabled = false;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Action worker = (Action)e.Argument;
                worker();
            }
            catch (Exception ex)
            {
                Error(new NullCodeLocation(""), ex, "An internal error occurred.");
                throw;
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

        public void Info(string message)
        {
            diagnosticSvc.Inform(new NullCodeLocation(""), message);
        }

        public void Info(string message, params object[] args)
        {
            diagnosticSvc.Inform(new NullCodeLocation(""), message, args);
        }

        public void Info(ICodeLocation location, string message)
        {
            diagnosticSvc.Inform(location, message);
        }

        public void Info(ICodeLocation location, string message, params object[] args)
        {
            diagnosticSvc.Inform(location, message, args);
        }

        public void Warn(string message)
        {
            diagnosticSvc.Warn(new NullCodeLocation(""), message);
        }

        public void Warn(string message, params object[] args)
        {
            diagnosticSvc.Warn(new NullCodeLocation(""), message, args);
        }

        public void Warn(ICodeLocation location, string message)
        {
            diagnosticSvc.Warn(location, message);
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            diagnosticSvc.Warn(location, message, args);
        }

        public void Error(string message)
        {
            diagnosticSvc.Error(new NullCodeLocation(""), message);
        }

        public void Error(string message, params object[] args)
        {
            diagnosticSvc.Error(new NullCodeLocation(""), message, args);
        }

        public void Error(Exception ex, string message)
        {
            diagnosticSvc.Error(ex, message);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            diagnosticSvc.Error(new NullCodeLocation(""), ex, message, args);
        }

        public void Error(ICodeLocation location, string message)
        {
            diagnosticSvc.Error(location, message);
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            diagnosticSvc.Error(location, message, args);
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            diagnosticSvc.Error(location, ex, message);
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            diagnosticSvc.Error(location, ex, message, args);
        }

        public void Error(ScriptError scriptError)
        {
            diagnosticSvc.Error(
                new ScriptErrorNavigator(scriptError, sp),
                scriptError.Exception,
                scriptError.Message);
        }

        void DecompilerEventListener.ShowStatus(string caption)
        {
            ShowStatus(caption);
        }

        public ICodeLocation CreateAddressNavigator(IReadOnlyProgram program, Address addr)
        {
            return new AddressNavigator(program, addr, sp);
        }

        ICodeLocation DecompilerEventListener.CreateBlockNavigator(IReadOnlyProgram program, Block block)
        {
            return new BlockNavigator(program, block, sp);
        }

        ICodeLocation DecompilerEventListener.CreateProcedureNavigator(IReadOnlyProgram program, Procedure proc)
        {
            return new ProcedureNavigator(program, proc, sp);
        }

        ICodeLocation DecompilerEventListener.CreateStatementNavigator(IReadOnlyProgram program, Statement stm)
        {
            return new StatementNavigator(program, stm, sp);
        }

        ICodeLocation DecompilerEventListener.CreateJumpTableNavigator(IReadOnlyProgram program, IProcessorArchitecture arch, Address addrIndirectJump, Address addrVector, int stride)
        {
            return new JumpVectorNavigator(program, arch, addrIndirectJump, addrVector, stride, sp);
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
            this.position = numerator;
            this.total = denominator;
            System.Threading.Interlocked.Exchange<string>(ref status, caption);
            if (denominator > 0)
            {
                var percentDone = Math.Min(
                    100,
                    (int) ((numerator * 100L) / denominator));
                dlg.Worker.ReportProgress(percentDone);
            }
        }

        void DecompilerEventListener.Advance(int count)
        {
            if (dlg == null)
                return;
            this.position += count;
            var percentDone = Math.Min(
                100,
                (int) ((position * 100L) / total));
            dlg.Worker.ReportProgress(percentDone);
        }

        // Is usually called on a worker thread.
        bool DecompilerEventListener.IsCanceled()
        {
            return this.isCanceled;
        }

        #endregion
    }
}
