#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Assemblers;
using Reko.Core.Configuration;
using Reko.Gui;
using Reko.Gui.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;

namespace Reko.UnitTests.Gui.Forms
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

        public virtual string NextPhaseCaption { get { return "Hehe"; } }

        public virtual void PerformWork(IWorkerDialogService svc)
        {
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

        public bool QueryStatus(CommandID cmdId, Reko.Gui.CommandStatus status, Reko.Gui.CommandText text)
        {
            return false;
        }

        public bool Execute(CommandID cmdId)
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

        public bool OpenBinary(string file)
        {
            OpenBinaryCalled = true;
            return false;
        }

        public bool OpenBinaryAs(string file, LoadDetails details)
        {
            throw new NotImplementedException();
        }


        public bool Assemble(string file, Assembler asm)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeScannedPageInteractor : FakePhasePageInteractor, IScannedPageInteractor
    {
        public FakeScannedPageInteractor()
        {
        }
    }

    public class FakeAnalyzedPageInteractor : FakePhasePageInteractor, IAnalyzedPageInteractor
    {
        public FakeAnalyzedPageInteractor()
        {
        }
    }

    public class FakeFinalPageInteractor : FakePhasePageInteractor, IFinalPageInteractor
    {
        public FakeFinalPageInteractor()
        {
        }
    }
}
