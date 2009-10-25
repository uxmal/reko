/* 
 * Copyright (C) 1999-2009 John Källén.
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
    /// <summary>
    /// Base class for all interactors in charge of phase pages. Provides common functionality
    /// such as command routing.
    /// </summary>
    public abstract class PhasePageInteractor : 
        IComponent,
        ICommandTarget
    {
        public event EventHandler Disposed;

        private ISite site;
        private IDecompilerService decompilerSvc;
        private IDecompilerUIService decompilerUiSvc;

        public PhasePageInteractor()
        {
        }

        public virtual bool CanAdvance
        {
            get { return decompilerSvc.Decompiler != null; }
        }

        public virtual DecompilerDriver Decompiler
        {
            get { return decompilerSvc.Decompiler; }
        }

        protected object EnsureService(Type svcType)
        {
            object svc = site.GetService(svcType);
            if (svc == null)
                throw new InvalidOperationException(string.Format(
                    "Attempt to obtain {0} service interface failed.", svcType.FullName));
            return svc;
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

        public abstract object Page { get; }

        protected IDecompilerUIService UIService
        {
            get { return decompilerUiSvc; }
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
                    decompilerSvc = (IDecompilerService) EnsureService(typeof(IDecompilerService));
                    decompilerUiSvc = (IDecompilerUIService) EnsureService(typeof(IDecompilerUIService));
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