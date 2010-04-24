/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Gui;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public interface IPhasePageInteractor : IComponent, ICommandTarget
    {
        bool CanAdvance { get; }

        void EnterPage();

        bool LeavePage();

    }

    /// <summary>
    /// Base class for all interactors in charge of phase pages. Provides common functionality
    /// such as command routing.
    /// </summary>
    public abstract class PhasePageInteractorImpl : 
        IPhasePageInteractor
    {
        public event EventHandler Disposed;

        private ISite site;
        private IDecompilerService decompilerSvc;
        private IDecompilerShellUiService decompilerUiSvc;
        private IWorkerDialogService workerDlgSvc;

        public PhasePageInteractorImpl()
        {
        }

        public virtual bool CanAdvance
        {
            get { return decompilerSvc.Decompiler != null; }
        }

        public virtual IDecompiler Decompiler
        {
            get { return decompilerSvc.Decompiler; }
            set { decompilerSvc.Decompiler = value; }
        }

        protected T EnsureService<T>()
        {
            var svc = GetService<T>();
            if (svc == null)
                throw new InvalidOperationException(string.Format(
                    "Attempt to obtain {0} service interface failed.", typeof(T).FullName));
            return svc;
        }

        protected T GetService<T>()
        {
            return (T)site.GetService(typeof(T));
        }

        /// <summary>
        /// Derived classes should copy populate editable controls with initial values.
        /// </summary>
        public abstract void EnterPage();

        /// <summary>
        /// Derived classes should copy any values out of controls.
        /// </summary>
        /// <returns>False if derived class wants to cancel leaving the page.</returns>
        public abstract bool LeavePage();

        protected IDecompilerShellUiService UIService
        {
            get { return decompilerUiSvc; }
        }

        protected IWorkerDialogService WorkerDialogService
        {
            get { return workerDlgSvc; }
        }


        #region ICommandTarget Members

        public virtual bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
        {
            return false;
        }

        public virtual bool Execute(ref Guid cmdSet, int cmdId)
        {
            return false;
        }

        #endregion

        #region IComponent interface 
        public virtual ISite Site
        {
            get { return site; }
            set
            {
                site = value;
                if (site != null)
                {
                    decompilerSvc = EnsureService<IDecompilerService>();
                    decompilerUiSvc = EnsureService<IDecompilerShellUiService>();
                    workerDlgSvc = EnsureService<IWorkerDialogService>();
                }
                else
                {
                    decompilerSvc = null;
                    decompilerUiSvc = null;
                }
            }

        }

        public virtual void Dispose()
        {
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }
        #endregion


    }
}