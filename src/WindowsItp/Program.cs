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

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ItpForm());
        }

        public class Instr
        {
        }

        public class Instr1 : Instr
        {
        }

        public class Instr2 : Instr
        {
        }

        public abstract class BaseDasm<T> : IEnumerator<T>
        {
            public abstract T Current { get; } 
            
            object System.Collections.IEnumerator.Current { get { return Current; } }

            public void Dispose() { }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        public class Dasm1 : BaseDasm<Instr1>
        {
            public override Instr1 Current
            {
                get { throw new NotImplementedException(); }
            }
        }
        
        public class Dasm2 : BaseDasm<Instr2>
        {
            public override Instr2 Current
            {
                get { throw new NotImplementedException(); }
            }
        }

        public interface Arch
        {
            IEnumerator<Instr> GetInstrs();
        }

        public class Arch1 : Arch
        {
            public IEnumerator<Instr> GetInstrs()
            {
                return new Dasm1();
            }
        }

        public class Arch2 : Arch
        {
            public IEnumerator<Instr> GetInstrs()
            {
                return new Dasm2();
            }
        }
    }
}
