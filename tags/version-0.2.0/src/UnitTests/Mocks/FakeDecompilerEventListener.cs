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

using Decompiler.Core;
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeDecompilerEventListener : DecompilerEventListener, IWorkerDialogService
    {
        private string lastDiagnostic;
        private string lastProgress;
        private bool finishedCalled;
        private string lastStatus;

        public void Finished()
        {
            finishedCalled = true;
        }

        public void AddErrorDiagnostic(Address addr, string format, params object[] args)
        {
            AddDiagnostic(new ErrorDiagnostic(addr, format, args));
        }

        public void AddWarningDiagnostic(Address addr, string format, params object [] args)
        {
            AddDiagnostic(new WarningDiagnostic(addr, format, args));
        }
        public void AddDiagnostic(Diagnostic d)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} - {1}: {2}", d.GetType().Name, d.Address, d.Message);
            lastDiagnostic = sb.ToString();
            System.Diagnostics.Debug.WriteLine(lastDiagnostic);
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            lastProgress = string.Format("{0}: {1}%", caption, (numerator * 100) / denominator);
        }

        public void ShowStatus(string status)
        {
            lastStatus = status;
        }

        public void CodeStructuringComplete()
        {
        }

        public void DecompilationFinished()
        {
        }

        public void InterproceduralAnalysisComplete()
        {
        }

        public void MachineCodeRewritten()
        {
        }

        public void ProceduresTransformed()
        {
        }

        public void ProgramLoaded()
        {
        }

        public void ProgramScanned()
        {
        }

        public void TypeReconstructionComplete()
        {
        }

        // Diagnostic methods.

        public bool FinishedCalled
        {
            get { return finishedCalled; }
        }

        public string LastDiagnostic
        {
            get { return lastDiagnostic; }
        }

        public string LastProgress
        {
            get { return lastProgress; }
        }


        #region IWorkerDialogService Members

        public bool StartBackgroundWork(string caption, System.Threading.ThreadStart backgroundWork)
        {
            backgroundWork();
            return true;
        }

        #endregion
    }
}
