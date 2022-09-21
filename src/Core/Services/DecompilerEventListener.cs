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
using Reko.Core.Machine;
using Reko.Core.Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Services
{
    /// <summary>
    /// This interface is used by the worker thread to communicate with the 
    /// driver, be it a command line or a GUI.
    /// </summary>
    public interface DecompilerEventListener
    {
        ICodeLocation CreateAddressNavigator(IReadOnlyProgram program, Address address);
        ICodeLocation CreateProcedureNavigator(IReadOnlyProgram program, Procedure proc);
        ICodeLocation CreateBlockNavigator(IReadOnlyProgram program, Block block);
        ICodeLocation CreateStatementNavigator(IReadOnlyProgram program, Statement stm);
        ICodeLocation CreateJumpTableNavigator(IReadOnlyProgram program, IProcessorArchitecture arch, Address addrIndirectJump, Address? addrVector, int stride);

        void Info(string message);
        void Info(string message, params object [] args);
        void Info(ICodeLocation location, string message);
        void Info(ICodeLocation location, string message, params object[] args);
        void Warn(string message);
        void Warn(string message, params object[] args);
        void Warn(ICodeLocation location, string message);
        void Warn(ICodeLocation location, string message, params object[] args);
        void Error(string message);
        void Error(string message, params object[] args);
        void Error(Exception ex, string message);
        void Error(Exception ex, string message, params object[] args);

        void Error(ICodeLocation location, string message);
        void Error(ICodeLocation location, string message, params object[] args);
        void Error(ICodeLocation location, Exception ex, string message);
        void Error(ICodeLocation location, Exception ex, string message, params object[] args);
        void Error(ScriptError scriptError);

        void ShowStatus(string caption);
        void ShowProgress(string caption, int numerator, int denominator);
        void Advance(int count);
        bool IsCanceled();
    }

    public class NullDecompilerEventListener : DecompilerEventListener
    {
        public static DecompilerEventListener Instance { get; } = new NullDecompilerEventListener();

        #region DecompilerEventListener Members

        public void Info(string message)
        {
            Debug.Print("Info: {0}", message);
        }

        public void Info(string message, params object [] args)
        {
            Debug.Print("Info: {0}", string.Format(message, args));
        }

        public void Info(ICodeLocation location, string message)
        {
            Debug.Print("Info: {0}: {1}", location, message);
        }

        public void Info(ICodeLocation location, string message, params object[] args)
        {
            Debug.Print("Info: {0}: {1}", location,
                string.Format(message, args));
        }

        public void Warn(string message)
        {
            Debug.Print("Warning: {0}", message);
        }

        public void Warn(string message, params object[] args)
        {
            Debug.Print("Warning: {0}", string.Format(message, args));
        }

        public void Warn(ICodeLocation location, string message)
        {
            Debug.Print("Warning: {0}: {1}", location, message);
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Debug.Print("Warning: {0}: {1}", location,
                string.Format(message, args));
        }

        public void Error(string message)
        {
            Debug.Print("Error: {0}", message);
        }

        public void Error(string message, params object[] args)
        {
            Debug.Print("Error: {0}", string.Format(message, args));
        }

        public void Error(Exception ex, string message)
        {
            Debug.Print("Error: {0} {1}", message, ex.Message);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Debug.Print("Error: {0} {1}",
                string.Format(message, args),
                ex.Message);
        }

        public void Error(ICodeLocation location, string message)
        {
            Debug.Print("Error: {0}: {1}", location, message);
        }

        public void Error(ICodeLocation location, string message, params object [] args)
        {
            Debug.Print("Error: {0}: {1}", location,
                string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            Debug.Print("Error: {0}: {1} {2}", location, message, ex.Message);
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        { 
            Debug.Print("Error: {0}: {1} {2}", 
                location, 
                string.Format(message, args),
                ex.Message);
        }

        public void Error(ScriptError scriptError)
        {
            Debug.Print(
                "Error: {0}: {1} {2}",
                scriptError.FileName,
                scriptError.Message,
                scriptError.Exception.Message);
        }

        public void AddDiagnostic(Diagnostic d)
        {
            throw new NotImplementedException();
        }

        public void ShowStatus(string caption)
        {
            Debug.Print("Status: {0}", caption);
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            //$TODO: show progress
        }

        public void Advance(int advance)
        {
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
            return new NullCodeLocation(block.Id);
        }

        public ICodeLocation CreateStatementNavigator(IReadOnlyProgram program, Statement stm)
        {
            return new NullCodeLocation(stm.Address.ToString());
        }

        public ICodeLocation CreateJumpTableNavigator(IReadOnlyProgram _, IProcessorArchitecture arch, Address addrIndirectJump, Address? addrVector, int stride)
        {
            return new NullCodeLocation(addrIndirectJump.ToString());
        }

        public void AddDiagnostic(ICodeLocation location, Diagnostic d)
        {
            throw new NotImplementedException();
        }

        public bool IsCanceled()
        {
            return false;
        }

        #endregion
    }

    public class NullCodeLocation : ICodeLocation
    {
        public NullCodeLocation(string text)
        {
            this.Text = text;
        }

        public string Text { get; private set; }

        public ValueTask NavigateTo()
        {
            return ValueTask.CompletedTask;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
