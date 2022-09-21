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
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaEventListener : DecompilerEventListener, IWorkerDialogService
    {
        private IServiceProvider services;
        private IDiagnosticsService diagnosticSvc;
        private IStatusBarService sbSvc;
        private SynchronizationContext uiSyncCtx;

        private CancellationTokenSource cancellationSvc;
        private bool isCanceled;
        private int position;
        private int total;

        public AvaloniaEventListener(IServiceProvider services)
        {
            var uiSyncCtx = SynchronizationContext.Current;
            if (uiSyncCtx is null)
                throw new InvalidOperationException("No SynchronizationContext.");
            this.uiSyncCtx = uiSyncCtx;
            this.services = services;
            diagnosticSvc = services.RequireService<IDiagnosticsService>();
        }

        public bool IsBackgroundWorkerRunning { get; private set; }


        public void Advance(int count)
        {
            uiSyncCtx?.Post(delegate
            {
                this.position += count;
                if (total > 0)
                {
                    var percentDone = Math.Min(
                        100,
                        (int) ((position * 100L) / total));
                }
            },
            null);
        }
        public ICodeLocation CreateAddressNavigator(IReadOnlyProgram program, Address addr)
        {
            return new AddressNavigator(program, addr, services);
        }

        public ICodeLocation CreateBlockNavigator(IReadOnlyProgram program, Block block)
        {
            return new BlockNavigator(program, block, services);
        }

        public ICodeLocation CreateJumpTableNavigator(IReadOnlyProgram program, IProcessorArchitecture arch, Address addrIndirectJump, Address? addrVector, int stride)
        {
            return new JumpVectorNavigator(program, arch, addrIndirectJump, addrVector, stride, services);
        }

        public ICodeLocation CreateProcedureNavigator(IReadOnlyProgram program, Procedure proc)
        {
            return new ProcedureNavigator(program, proc, services);
        }

        public ICodeLocation CreateStatementNavigator(IReadOnlyProgram program, Statement stm)
        {
            return new StatementNavigator(program, stm, services);
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
                new ScriptErrorNavigator(scriptError, services),
                scriptError.Exception,
                scriptError.Message);
        }

        public void FinishBackgroundWork()
        {
        }

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

        public bool IsCanceled()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Intended to be call by a worker thread; uses Invoke to make sure the delegate is invoked
        /// on the UI thread.
        /// </summary>
        /// <param name="newCaption"></param>
        public void SetCaption(string newCaption)
        {
            uiSyncCtx?.Post(delegate
            {
                sbSvc.SetText(newCaption);
                sbSvc.HideProgress();
            }, null);
        }

        public void ShowError(string failedOperation, Exception ex)
        {
            //$TODO: should log this in DecompilerEventListener.
            uiSyncCtx?.Post(delegate
            {
                var loc = new NullCodeLocation("");
                Error(loc, ex, failedOperation);
            }, null);
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            uiSyncCtx?.Post(delegate
            {
                sbSvc.SetSubtext(caption);
                this.position = numerator;
                this.total = denominator;
                if (denominator > 0)
                {
                    var percentDone = Math.Min(
                        100,
                        (int) ((numerator * 100L) / denominator));
                    sbSvc.ShowProgress(percentDone);
                }
            },
            null);
        }

        public void ShowStatus(string newStatus)
        {
            uiSyncCtx?.Post(delegate
            {
                sbSvc.SetSubtext(newStatus);
            }, null);
        }

        /// <summary>
        /// The UI thread requests a background operation by calling this method.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="backgroundTask"></param>
        /// <returns></returns>
        public async ValueTask<bool> StartBackgroundWork(string caption, Action backgroundTask)
        {
            if (this.IsBackgroundWorkerRunning)
                throw new InvalidOperationException("A background task is already running.");
            var sc = new ServiceContainer(services);
            this.sbSvc = sc.RequireService<IStatusBarService>();

            this.cancellationSvc = new CancellationTokenSource();
            sc.AddService<CancellationTokenSource>(cancellationSvc);

            this.IsBackgroundWorkerRunning = true;
            try
            {
                await Task.Run(backgroundTask);
                return true;
            }
            catch (Exception ex)
            {
                // The background task should be catching exceptions,
                // so if one leaks out here, we have a programming
                // error.
                var a = new NullCodeLocation("");
                Error(a, ex, "An internal error occurred.");
                return false;
            }
            finally
            {
                this.IsBackgroundWorkerRunning = false;
                sbSvc.HideProgress();
                sbSvc.SetSubtext("");
            }
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


        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@   


        private void dlg_Cancelled(object sender, EventArgs e)
        {
            cancellationSvc.Cancel();
            this.isCanceled = true;
            Warn(new NullCodeLocation(""), "User canceled the decompiler.");
        }

        //void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    if (dlg == null)
        //        return;
        //    if (e.ProgressPercentage != STATUS_UPDATE_ONLY)
        //    {
        //        dlg.ProgressBar.Value = e.ProgressPercentage;
        //        sbSvc.ShowProgress(e.ProgressPercentage);
        //    }
        //    dlg.Detail.Text = status;
        //    sbSvc.SetSubtext(status);
        //}

        #region DecompilerEventListener Members

        // Is usually called on a worker thread.
        bool DecompilerEventListener.IsCanceled()
        {
            return this.isCanceled;
        }

        #endregion
    }
}
