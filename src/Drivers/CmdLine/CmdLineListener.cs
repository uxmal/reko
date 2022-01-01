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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Reko.CmdLine
{
    public class CmdLineListener : DecompilerEventListener
    {
        private bool isCanceled;
        private string currentCaption;
        private int position;
        private int total;
        private bool needsNewLine;

        public bool Quiet { get; set; }

        public ICodeLocation CreateAddressNavigator(Program program, Address address)
        {
            return new NullCodeLocation(address.ToString());
        }

        public ICodeLocation CreateProcedureNavigator(Program program, Procedure proc)
        {
            return new NullCodeLocation(proc.Name);
        }

        public ICodeLocation CreateBlockNavigator(Program program, Block block)
        {
            return new NullCodeLocation(block.Id);
        }

        public ICodeLocation CreateStatementNavigator(Program program, Statement stm)
        {
            return new NullCodeLocation(program.SegmentMap.MapLinearAddressToAddress(stm.LinearAddress).ToString());
        }

        public ICodeLocation CreateJumpTableNavigator(Program program, IProcessorArchitecture _, Address addrIndirectJump, Address addrVector, int stride)
        {
            return new NullCodeLocation(addrIndirectJump.ToString());
        }

        public void AddDiagnostic(ICodeLocation location, Diagnostic d)
        {
            EnsureNewLine();
            Console.Out.WriteLine("{0}: {1}: {2}",
                location.Text,
                d.ImageKey,
                d.Message);
        }

        public void Info(string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("{0}", message);
        }

        public void Info(string message, params object [] args)
        {
            Info(string.Format(message, args));
        }

        public void Info(ICodeLocation location, string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("{0}: {1}", location.Text, message);
        }

        public void Info(ICodeLocation location, string message, params object[] args)
        {
            Info(location, string.Format(message, args));
        }

        public void Warn(string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("Warning: {0}", message);
        }

        public void Warn(string message, params object[] args)
        {
            Warn(string.Format(message, args));
        }

        public void Warn(ICodeLocation location, string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("{0}: warning: {1}", location.Text, message);
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Warn(location, string.Format(message, args));
        }

        public void Error(string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("Error: {0}", message);
        }

        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(ICodeLocation location, string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("{0}: error: {1}", location.Text, message);
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            Error(location, string.Format(message, args));
        }

        public void Error(Exception ex, string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("Error: {0}", message);
            Console.Out.WriteLine("    {0}", ex.Message);
            while (ex != null)
            {
                Console.Out.WriteLine("    {0}", ex.StackTrace);
                ex = ex.InnerException;
            }
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Error(ex, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("{0}: error: {1}", location.Text, message);
            Console.Out.WriteLine("    {0}", ex.Message);
            while (ex != null)
            {
                Console.Out.WriteLine("    {0}", ex.StackTrace);
                ex = ex.InnerException;
            }
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

        public void ShowStatus(string caption)
        {
            if (Quiet)
                return;
            EnsureNewLine();
            Console.WriteLine(caption);
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            if (caption != currentCaption)
            {
                if (!Quiet)
                    Console.Out.WriteLine();
                currentCaption = caption;
            }

            if (denominator == 0)
            {
                if (numerator == 0)
                    denominator = 1;
                else
                    denominator = numerator;
            }

            this.position = numerator;
            this.total = denominator;
            ReportProgress();
        }

        private void ReportProgress()
        {
            if (Quiet)
                return;
            if (total > 0)
            {
                var percentDone = Math.Min(
                    100,
                    (int) Math.Round(((position * 100.0) / (double) total)));

                Console.Out.Write("\r{0,60}\r", "");
                Console.Out.Write("{0} [{1}%]", currentCaption, percentDone);
                this.needsNewLine = true;
            }
        }

        public void Advance(int count)
        {
            this.position += count;
            ReportProgress();
        }

        public bool IsCanceled()
        {
            return isCanceled;
        }

        public void CancelDecompilation(string reason)
        {
            EnsureNewLine();
            Console.Out.Write("Decompilation canceled. {0}", reason);
            this.isCanceled = true;
        }

        private void EnsureNewLine()
        {
            if (needsNewLine)
            {
                Console.Out.WriteLine();
                needsNewLine = false;
            }
        }
    }
}
