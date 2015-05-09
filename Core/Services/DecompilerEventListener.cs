#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Core.Services
{
    public interface DecompilerEventListener
    {
        ICodeLocation CreateAddressNavigator(Program program, Address address);
        ICodeLocation CreateProcedureNavigator(Procedure proc);
        ICodeLocation CreateBlockNavigator(Block block);
        void Warn(ICodeLocation location, string message);
        void Error(ICodeLocation location, string message);
        void Error(ICodeLocation location, Exception ex, string message);

        void ShowStatus(string caption);
        void ShowProgress(string caption, int numerator, int denominator);
    }

    public class NullDecompilerEventListener : DecompilerEventListener
    {
        private static NullDecompilerEventListener e = new NullDecompilerEventListener();

        public static DecompilerEventListener Instance { get { return e; } }

        #region DecompilerEventListener Members

        public void Warn(ICodeLocation location, string message)
        {
            Debug.Print("Warning: {0}: {1}", location, message);
        }

        public void Error(ICodeLocation location, string message)
        {
            Debug.Print("Error: {0}: {1}", location, message);
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            Debug.Print("Error: {0}: {1} {2}", location, message, ex.Message);
        }

        public void AddDiagnostic(Diagnostic d)
        {
            throw new NotImplementedException();
        }

        public void ShowStatus(string caption)
        {
            throw new NotImplementedException();
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            throw new NotImplementedException();
        }

        public ICodeLocation CreateAddressNavigator(Program program, Address address)
        {
            return new NullCodeLocation(address.ToString());
        }

        public ICodeLocation CreateProcedureNavigator(Procedure proc)
        {
            return new NullCodeLocation(proc.Name);
        }

        public ICodeLocation CreateBlockNavigator(Block block)
        {
            return new NullCodeLocation(block.Name);
        }

        public void AddDiagnostic(ICodeLocation location, Diagnostic d)
        {
            throw new NotImplementedException();
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

        public void NavigateTo()
        {
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
