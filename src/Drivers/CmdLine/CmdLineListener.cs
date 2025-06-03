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

using Reko.Core;
using Reko.Core.Output;
using Reko.Core.Scripts;
using Reko.Core.Services;
using Reko.Services;
using System;

namespace Reko.CmdLine
{
    /// <summary>
    /// Implements <see cref="IDecompilerEventListener"/> for the command line client.
    /// </summary>
    public class CmdLineListener : IDecompilerEventListener
    {
        private bool isCanceled;
        private bool needsNewLine;

        public CmdLineListener()
        {
            this.Progress = new ProgressIndicator(this);
        }

        public IProgressIndicator Progress { get; }

        public bool Quiet { get; set; }

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

        public ICodeLocation CreateJumpTableNavigator(IReadOnlyProgram program, IProcessorArchitecture _, Address addrIndirectJump, Address? addrVector, int stride)
        {
            return new NullCodeLocation(addrIndirectJump.ToString());
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

        public void Warn(ProgramAddress paddr, string message)
        {
            Warn(CreateAddressNavigator(paddr.Program, paddr.Address), message);
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

        public void Error(ProgramAddress paddr, string message)
        {
            Error(CreateAddressNavigator(paddr.Program, paddr.Address), message);
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

        public void Error(Exception exception, string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("Error: {0}", message);
            Console.Out.WriteLine("    {0}", exception.Message);
            var ex = exception;
            while (ex is not null)
            {
                Console.Out.WriteLine("    {0}", ex.StackTrace);
                ex = ex.InnerException;
            }
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Error(ex, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception exception, string message)
        {
            EnsureNewLine();
            Console.Out.WriteLine("{0}: error: {1}", location.Text, message);
            Console.Out.WriteLine("    {0}", exception.Message);
            var ex = exception;
            while (ex is not null)
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


        public void OnProcedureFound(Program program, Address address)
        {
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

        private class ProgressIndicator : IProgressIndicator
        {
            private readonly CmdLineListener outer;
            private string currentCaption;
            private int position;
            private int total;

            public ProgressIndicator(CmdLineListener outer)
            {
                this.outer = outer;
                this.currentCaption = "";
            }

            public void Advance(int count)
            {
                this.position += count;
                this.ReportProgress();
            }

            public void SetCaption(string caption)
            {
                if (caption != this.currentCaption)
                {
                    if (!outer.Quiet)
                        Console.Out.WriteLine();
                    this.currentCaption = caption;
                }
            }

            public void ShowProgress(string caption, int numerator, int denominator)
            {
                if (caption != this.currentCaption)
                {
                    if (!outer.Quiet)
                        Console.Out.WriteLine();
                    this.currentCaption = caption;
                }
                ShowProgress(numerator, denominator);
            }

            public void ShowProgress(int numerator, int denominator)
            { 
                if (denominator == 0)
                {
                    if (numerator == 0)
                        denominator = 1;
                    else
                        denominator = numerator;
                }

                this.position = numerator;
                this.total = denominator;
                this.ReportProgress();
            }

            public void ShowStatus(string caption)
            {
                if (outer.Quiet)
                    return;
                outer.EnsureNewLine();
                Console.WriteLine(caption);
            }

            private void ReportProgress()
            {
                if (outer.Quiet)
                    return;
                if (total > 0)
                {
                    var percentDone = Math.Min(
                        100,
                        (int) Math.Round(((position * 100.0) / (double) total)));

                    Console.Out.Write("\r{0,60}\r", "");
                    Console.Out.Write("{0} [{1}%]", currentCaption, percentDone);
                    outer.needsNewLine = true;
                }
            }

            public void Finish()
            {
                Console.Out.Write("{0} [Done]", currentCaption);
                Console.Out.WriteLine();
                outer.needsNewLine = false;
            }
        }

    }
}
