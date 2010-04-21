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

using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    public class FakePhasePageInteractor : IPhasePageInteractor
    {
        private ISite site;
        private bool canAdvance;

        public FakePhasePageInteractor()
        {
        }

        #region IPhasePageInteractor Members

        public virtual bool CanAdvance
        {
            get { return canAdvance; }
            set { canAdvance = value; }
        }

        public virtual void EnterPage()
        {
        }

        public virtual bool LeavePage()
        {
            return true;
        }

        #endregion

        #region IComponent Members

        public event EventHandler Disposed;

        public ISite Site
        {
            get { return site; }
            set { site = value; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        #endregion

        #region ICommandTarget Members

        public bool QueryStatus(ref Guid cmdSet, int cmdId, Decompiler.Gui.CommandStatus status, Decompiler.Gui.CommandText text)
        {
            return false;
        }

        public bool Execute(ref Guid cmdSet, int cmdId)
        {
            return false;
        }

        #endregion
    }

    public class FakeInitialPageInteractor : FakePhasePageInteractor, InitialPageInteractor
    {
        public bool OpenBinaryCalled;

        public FakeInitialPageInteractor()
        {
        }

        #region InitialPageInteractor Members

        public void OpenBinary(string file, DecompilerHost host)
        {
            OpenBinaryCalled = true;

        }

        #endregion
    }

    public class FakeLoadedPageInteractor : FakePhasePageInteractor, ILoadedPageInteractor
    {
        public FakeLoadedPageInteractor() 
        {
        }
    }
}
