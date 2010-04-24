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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Decompiler.Gui.Windows.Forms
{
    public abstract class PhaseProgressInteractor
    {
        private bool wasCancelled;
        private Exception lastException;
        private BackgroundWorker worker;

        public PhaseProgressInteractor()
        {
            worker = new BackgroundWorker();
        }
        
        public void Attach(PhaseProgressDialog dialog)
        {
            dialog.Disposed += new EventHandler(dialog_Disposed);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        void dialog_Disposed(object sender, EventArgs e)
        {
            worker.Dispose();
            worker = null;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            wasCancelled = e.Cancelled;
            lastException = e.Error;
        }

        public bool WasCancelled { get { return wasCancelled; } }

        public Exception ThrownException { get { return lastException; } }


        // The following methods/properties are invoked on the background thread. ////////////////

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DoWork();
        }

        protected abstract void DoWork();


    }
}
