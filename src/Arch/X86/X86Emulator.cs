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
using System.Linq;
using System.Text;

namespace Decompiler.Arch.X86
{
    public class X86Emulator
    {
       public event EventHandler BeforeStart    ;
       public event EventHandler BreakpointHit  ;
       public event EventHandler ExceptionRaised;

        private Core.IProcessorArchitecture processorArchitecture;
        private Core.LoadedImage loadedImage;
        private X86State state;

        public X86Emulator(Core.IProcessorArchitecture processorArchitecture, Core.LoadedImage loadedImage, X86State state)
        {
            this.processorArchitecture = processorArchitecture;
            this.loadedImage = loadedImage;
            this.state = state;
        }
        public Core.Address InstructionPointer { get; set; }

        IEnumerator<IntelInstruction> dasm;

        public void Run()
        {
            CreateStack();
            BeforeStart.Fire(this);
            try
            {
                while (dasm.MoveNext())
                {
                    CheckBreakPoints();
                    Execute(dasm.Current);
                }
            }
            catch (Exception)
            {
                ExceptionRaised.Fire(this);
            }
        }

        public void CreateStack()
        {

        }

        public void CheckBreakPoints()
        {

        }

        public IntelInstruction Fetch()
        {
            throw new NotImplementedException();
        }

        public void Execute(IntelInstruction instr)
        {

        }
    }
}
