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
using Reko.Core.Scripts;
using Reko.Core.Services;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaEventListener : DecompilerEventListener, IWorkerDialogService
    {
        private IServiceProvider services;

        public AvaloniaEventListener(IServiceProvider services)
        {
            this.services = services;
        }

        public void Advance(int count)
        {
            throw new NotImplementedException();
        }

        public ICodeLocation CreateAddressNavigator(Program program, Address address)
        {
            throw new NotImplementedException();
        }

        public ICodeLocation CreateBlockNavigator(Program program, Block block)
        {
            throw new NotImplementedException();
        }

        public ICodeLocation CreateJumpTableNavigator(Program program, IProcessorArchitecture arch, Address addrIndirectJump, Address? addrVector, int stride)
        {
            throw new NotImplementedException();
        }

        public ICodeLocation CreateProcedureNavigator(Program program, Procedure proc)
        {
            throw new NotImplementedException();
        }

        public ICodeLocation CreateStatementNavigator(Program program, Statement stm)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception ex, string message)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(ICodeLocation location, string message)
        {
            throw new NotImplementedException();
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            throw new NotImplementedException();
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(ScriptError scriptError)
        {
            throw new NotImplementedException();
        }

        public void FinishBackgroundWork()
        {
            throw new NotImplementedException();
        }

        public void Info(string message)
        {
            throw new NotImplementedException();
        }

        public void Info(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Info(ICodeLocation location, string message)
        {
            throw new NotImplementedException();
        }

        public void Info(ICodeLocation location, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public bool IsCanceled()
        {
            throw new NotImplementedException();
        }

        public void SetCaption(string newCaption)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string p, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            throw new NotImplementedException();
        }

        public void ShowStatus(string caption)
        {
            throw new NotImplementedException();
        }

        public bool StartBackgroundWork(string caption, Action backgroundWork)
        {
            return false;   //$TODO
        }

        public void Warn(string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(ICodeLocation location, string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
