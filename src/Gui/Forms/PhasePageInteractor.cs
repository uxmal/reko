#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Services;
using Reko.Gui.Services;
using Reko.Services;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Reko.Gui.Forms
{
    public interface IPhasePageInteractor : ICommandTarget
    {
        bool CanAdvance { get; }

        /// <summary>
        /// Performs the work of this phase, before showing the results (in EnterPage).
        /// </summary>
        /// <remarks>This method is always called from a background thread. Do not call the UI directly from here; instead
        /// use the Diagnostic service or the IWorkerDialogService.SetCaption() to indicate progress.</remarks>
        /// <param name="workerDialogSvc"></param>
        void PerformWork(IWorkerDialogService workerDialogSvc);

        void EnterPage();

        bool LeavePage();

        /// <summary>
        /// Lets this Phase page interactor select the next phase to go to, given the
        /// current state of the project.
        /// </summary>
        /// <param name="decompilerPhases"></param>
        /// <returns>The next page to advance to, or null if no further advance is possible.</returns>
        IPhasePageInteractor? NextPage(DecompilerPhases decompilerPhases);
    }

    /// <summary>
    /// Base class for all interactors in charge of phase pages. Provides common functionality
    /// such as command routing.
    /// </summary>
    public abstract class PhasePageInteractorImpl : 
        IPhasePageInteractor
    {
        public event EventHandler? Disposed;

        private IDecompilerService decompilerSvc;
        private IDecompilerShellUiService decompilerUiSvc;
        private IWorkerDialogService workerDlgSvc;

        public PhasePageInteractorImpl(IServiceProvider services)
        {
            this.Services = services;
            decompilerSvc = services.RequireService<IDecompilerService>();
            decompilerUiSvc = services.RequireService<IDecompilerShellUiService>();
            workerDlgSvc = services.RequireService<IWorkerDialogService>();
        }

        public virtual bool CanAdvance
        {
            get { return decompilerSvc.Decompiler is not null; }
        }


        public virtual IDecompiler? Decompiler
        {
            get { return decompilerSvc.Decompiler; }
            set { decompilerSvc.Decompiler = value; }
        }

        public IServiceProvider Services { get; }

        /// <summary>
        /// Derived classes should copy populate editable controls with initial values.
        /// </summary>
        public abstract void EnterPage();

        public abstract void PerformWork(IWorkerDialogService workerDlgSvc);

        /// <summary>
        /// Derived classes should copy any values out of controls.
        /// </summary>
        /// <returns>False if derived class wants to cancel leaving the page.</returns>
        public abstract bool LeavePage();

        protected IDecompilerShellUiService UIService
        {
            get { return decompilerUiSvc; }
        }


        #region ICommandTarget Members

        public virtual bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            return false;
        }

        public virtual ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            return ValueTask.FromResult(false);
        }

        #endregion

        public virtual void Dispose()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public abstract IPhasePageInteractor? NextPage(DecompilerPhases decompilerPhases);
    }
}