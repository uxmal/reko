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

using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Reko.Core;
using Reko.Core.Output;
using Reko.Core.Scripts;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.Services;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Mocks
{
    public class FakeDecompilerEventListener : IDecompilerEventListener, IWorkerDialogService
    {
        private string lastDiagnostic;
        private bool finishedCalled;
        private FakeIndicator progress = new FakeIndicator();

        public IProgressIndicator Progress => progress;
        
        public void Finished()
        {
            finishedCalled = true;
        }

        public void ShowError(string context, Exception ex)
        {
        }

        public void AddDiagnostic(ICodeLocation location, Diagnostic d)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} - {1} - {2}", d.GetType().Name, location.Text, d.Message);
            lastDiagnostic = sb.ToString();
            Debug.WriteLine(lastDiagnostic);
        }

        public void Info(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new InformationalDiagnostic(message));
        }

        public void Info(string message, params object[] args)
        {
            Info(string.Format(message, args));
        }

        public void Info(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new WarningDiagnostic(message));
        }

        public void Info(ICodeLocation location, string message, params object[] args)
        {
            Warn(location, string.Format(message, args));
        }

        public void Warn(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new WarningDiagnostic(message));
        }

        public void Warn(string message, params object[] args)
        {
            Warn(string.Format(message, args));
        }

        public void Warn(ProgramAddress paddr, string message)
        {
            AddDiagnostic(
                CreateAddressNavigator(paddr.Program, paddr.Address),
                new WarningDiagnostic(message));
        }

        public void Warn(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new WarningDiagnostic(message));
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Warn(location, string.Format(message, args));
        }

        public void Error(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new ErrorDiagnostic(message));
        }

        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(Exception ex, string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new ErrorDiagnostic(message, ex));
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Error(new NullCodeLocation(""), ex, string.Format(message, args));
        }

        public void Error(ProgramAddress paddr, string message)
        {
            AddDiagnostic(
                CreateAddressNavigator(paddr.Program, paddr.Address),
                new ErrorDiagnostic(message));
        }

        public void Error(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new ErrorDiagnostic(message));
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            Error(location, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            AddDiagnostic(location, new ErrorDiagnostic(message, ex));
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            Error(location, ex, string.Format(message, args));
        }

        public void Error(ScriptError scriptError)
        {
            Error(
                new NullCodeLocation(scriptError.FileName),
                scriptError.Exception,
                scriptError.Message);
        }

        public bool IsCanceled()
        {
            return false;
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
            get { return this.progress.LastProgress; }
        }


        public ICodeLocation CreateAddressNavigator(IReadOnlyProgram program, Address address)
        {
            return new NullCodeLocation(address.ToString());
        }

        public ICodeLocation CreateProcedureNavigator(IReadOnlyProgram program, Procedure proc)
        {
            return new NullCodeLocation(proc.Name);
        }

        public ICodeLocation CreateBlockNavigator(IReadOnlyProgram program, Block block)
        {
            return new NullCodeLocation(block.DisplayName);
        }

        public ICodeLocation CreateStatementNavigator(IReadOnlyProgram program, Statement stm)
        {
            return new NullCodeLocation(stm.Address.ToString());
        }

        public ICodeLocation CreateJumpTableNavigator(IReadOnlyProgram program, IProcessorArchitecture _, Address addrIndirectJump, Address? addrVector, int stride)
        {
            return new NullCodeLocation(addrIndirectJump.ToString());
        }

        public void OnProcedureFound(Program program, Address addr)
        {
        }

        #region IWorkerDialogService Members

        public ValueTask<bool> StartBackgroundWork(string caption, Action backgroundWork)
        {
            backgroundWork();
            return ValueTask.FromResult(true);
        }

        public void FinishBackgroundWork()
        { 
        }

        public void SetCaption(string newCaption)
        {
        }

        #endregion

        private class FakeIndicator : IProgressIndicator
        {
            private string lastStatus;

            public void ShowProgress(string caption, int numerator, int denominator)
            {
                LastProgress = string.Format("{0}: {1}%", caption, (numerator * 100) / (denominator != 0 ? denominator : 1));
            }

            public void ShowProgress(int numerator, int denominator)
            {
                LastProgress = string.Format("{0}%", (numerator * 100) / (denominator != 0 ? denominator : 1));
            }


            public string LastProgress { get; private set; }


            public void Advance(int count)
            {
            }

            public void SetCaption(string caption)
            {
            }

            public void ShowStatus(string status)
            {
                lastStatus = status;
            }

            public void Finish()
            {
            }

        }

    }
}
