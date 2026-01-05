#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Output;
using Reko.Core.Scripts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Reko.Core.Services
{
    /// <summary>
    /// This interface is used by low-level code to communicate with the 
    /// driver, be it a command line or a GUI.
    /// </summary>
    public interface IEventListener
    {
        /// <summary>
        /// A reference to an instance of <see cref="IProgressIndicator"/>.
        /// </summary>
        IProgressIndicator Progress { get; }

        /// <summary>
        /// Creates an address navigator.
        /// </summary>
        /// <param name="program">Program containing the address.</param>
        /// <param name="address">Address to navigate to.</param>
        /// <returns>An address navigator.
        /// </returns>
        ICodeLocation CreateAddressNavigator(IReadOnlyProgram program, Address address);

        /// <summary>
        /// Creates an procedure navigator.
        /// </summary>
        /// <param name="program">Program containing the address.</param>
        /// <param name="proc">Procedure to navigate to.</param>
        /// <returns>An procedure navigator.
        /// </returns>
        ICodeLocation CreateProcedureNavigator(IReadOnlyProgram program, Procedure proc);

        /// <summary>
        /// Creates an block navigator.
        /// </summary>
        /// <param name="program">Program containing the address.</param>
        /// <param name="block">Basic block to navigate to.</param>
        /// <returns>An procedure navigator.
        /// </returns>

        ICodeLocation CreateBlockNavigator(IReadOnlyProgram program, Block block);

        /// <summary>
        /// Creates an block navigator.
        /// </summary>
        /// <param name="program">Program containing the address.</param>
        /// <param name="stm"><see cref="Statement"/> to navigate to.</param>
        /// <returns>An procedure navigator.
        /// </returns>
        ICodeLocation CreateStatementNavigator(IReadOnlyProgram program, Statement stm);

        /// <summary>
        /// Creates an jump table navigator.
        /// </summary>
        /// <param name="program">Program containing the address.</param>
        /// <param name="arch">Architecture.</param>
        /// <param name="addrIndirectJump">Address of the indirect jump.</param>
        /// <param name="addrVector">Address of the start of the table.</param>
        /// <param name="stride">The stride of the entries in the table.</param>
        /// <returns>An procedure navigator.
        /// </returns>
        ICodeLocation CreateJumpTableNavigator(
            IReadOnlyProgram program, 
            IProcessorArchitecture arch, 
            Address addrIndirectJump, 
            Address? addrVector,
            int stride);

        /// <summary>
        /// Writes an informational diagnostic message.
        /// </summary>
        /// <param name="message">Message to write.</param>
        void Info(string message);

        /// <summary>
        /// Writes an informational diagnostic format string and arguments.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="args">Arguments to interpolate.</param>
        void Info(string message, params object [] args);

        /// <summary>
        /// Writes an informational diagnostic message scoped
        /// to a <see cref="ICodeLocation"/>.
        /// </summary>
        /// <param name="location"><see cref="ICodeLocation"/> context.</param>
        /// <param name="message">Message to write.</param>
        void Info(ICodeLocation location, string message);

        /// <summary>
        /// Writes an informational diagnostic message with interploated
        /// arguments, scoped
        /// to a <see cref="ICodeLocation"/>.
        /// </summary>
        /// <param name="location"><see cref="ICodeLocation"/> context.</param>
        /// <param name="message">Message to write.</param>
        /// <param name="args">Arguments to interpolate.</param>
        void Info(ICodeLocation location, string message, params object[] args);

        /// <summary>
        /// Writes a warning message.
        /// </summary>
        /// <param name="message">Message to write.</param>
        void Warn(string message);

        /// <summary>
        /// Writes a warning format string and arguments.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="args">Arguments to interpolate.</param>
        void Warn(string message, params object[] args);

        /// <summary>
        /// Writes a warning message associated with a <see cref="ProgramAddress"/>.
        /// </summary>
        /// <param name="paddr">Program address at which the warning occurred.</param>
        /// <param name="message">Message to write.</param>
        void Warn(ProgramAddress paddr, string message);

        /// <summary>
        /// Writes a warning message scoped to a <see cref="ICodeLocation"/>.
        /// </summary>
        /// <param name="location">Location where the warning occurred.</param>
        /// <param name="message">Message to write.</param>
        void Warn(ICodeLocation location, string message);

        /// <summary>
        /// Writes an informational diagnostic message with interploated
        /// arguments, scoped
        /// to a <see cref="ICodeLocation"/>.
        /// </summary>
        /// <param name="location"><see cref="ICodeLocation"/> context.</param>
        /// <param name="message">Message to write.</param>
        /// <param name="args">Arguments to interpolate.</param>
        void Warn(ICodeLocation location, string message, params object[] args);


        /// <summary>
        /// Writes an error message.
        /// </summary>
        /// <param name="message">Message to write.</param>
        void Error(string message);

        /// <summary>
        /// Writes an error format string and arguments.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="args">Arguments to interpolate.</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Writes an error message.
        /// </summary>
        /// <param name="ex">Exception that was raised.</param>
        /// <param name="message">Message to write.</param>
        void Error(Exception ex, string message);

        /// <summary>
        /// Writes an error message.
        /// </summary>
        /// <param name="ex">Exception that was raised.</param>
        /// <param name="message">Message to write.</param>
        /// <param name="args">Arguments to interpolate.</param>
        void Error(Exception ex, string message, params object[] args);

        /// <summary>
        /// Writes an error message.
        /// </summary>
        /// <param name="paddr">Program address at which the error occurred.</param>
        /// <param name="message">Message to write.</param>
        void Error(ProgramAddress paddr, string message);

        /// <summary>
        /// Writes an error message.
        /// </summary>
        /// <param name="location">Location where the warning occurred.</param>
        /// <param name="message">Message to write.</param>
        void Error(ICodeLocation location, string message);

        /// <summary>
        /// Writes an error message scoped to a <see cref="ICodeLocation"/>.
        /// </summary>
        /// <param name="location"><see cref="ICodeLocation"/> context.</param>
        /// <param name="message">Message to write.</param>
        /// <param name="args">Arguments to interpolate.</param>
        void Error(ICodeLocation location, string message, params object[] args);

        /// <summary>
        /// Writes an error message scoped to a <see cref="ICodeLocation"/>.
        /// </summary>
        /// <param name="location"><see cref="ICodeLocation"/> context.</param>
        /// <param name="ex">Exception that was raised.</param>
        /// <param name="message">Message to write.</param>
        void Error(ICodeLocation location, Exception ex, string message);

        /// <summary>
        /// Writes an error message scoped to a <see cref="ICodeLocation"/>.
        /// </summary>
        /// <param name="location"><see cref="ICodeLocation"/> context.</param>
        /// <param name="ex">Exception that was raised.</param>
        /// <param name="message">Message to write.</param>
        /// <param name="args">Arguments to interpolate.</param>
        void Error(ICodeLocation location, Exception ex, string message, params object[] args);

        /// <summary>
        /// Writes a script error message.
        /// </summary>
        /// <param name="scriptError">Script error to display.</param>
        void Error(ScriptError scriptError);

        /// <summary>
        /// This method is called to determine whether the caller should quit
        /// gracefully.
        /// </summary>
        /// <returns>True if cancellation has been requested, false if not.
        /// </returns>
        /// <remarks>
        /// Implementations of potentially long-running loops are expected to 
        /// poll this method occasionally in order to respond to cancellation
        /// requests from the user.
        /// </remarks>
        bool IsCanceled();
    }

    /// <summary>
    /// This is a null implementation of <see cref="IEventListener"/>. It discards
    /// any calls.
    /// </summary>
    public class NullEventListener : IEventListener
    {
        /// <summary>
        /// Shared instance of <see cref="NullEventListener"/>.
        /// </summary>
        public static IEventListener Instance { get; } = new NullEventListener();

        #region IEventListener Members

        /// <inheritdoc/>
        public IProgressIndicator Progress => NullProgressIndicator.Instance;

        /// <inheritdoc/>
        public void Info(string message)
        {
            Debug.Print("Info: {0}", message);
        }

        /// <inheritdoc/>
        public void Info(string message, params object [] args)
        {
            Debug.Print("Info: {0}", string.Format(message, args));
        }

        /// <inheritdoc/>
        public void Info(ICodeLocation location, string message)
        {
            Debug.Print("Info: {0}: {1}", location, message);
        }

        /// <inheritdoc/>
        public void Info(ICodeLocation location, string message, params object[] args)
        {
            Debug.Print("Info: {0}: {1}", location,
                string.Format(message, args));
        }

        /// <inheritdoc/>
        public void Warn(string message)
        {
            Debug.Print("Warning: {0}", message);
        }

        /// <inheritdoc/>
        public void Warn(string message, params object[] args)
        {
            Debug.Print("Warning: {0}", string.Format(message, args));
        }

        /// <inheritdoc/>
        public void Warn(ProgramAddress paddr, string message)
        {
            Debug.Print("Warning: {0}: {1}", paddr.Address, message);
        }

        /// <inheritdoc/>
        public void Warn(ICodeLocation location, string message)
        {
            Debug.Print("Warning: {0}: {1}", location, message);
        }

        /// <inheritdoc/>
        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Debug.Print("Warning: {0}: {1}", location,
                string.Format(message, args));
        }

        /// <inheritdoc/>
        public void Error(string message)
        {
            Debug.Print("Error: {0}", message);
        }

        /// <inheritdoc/>
        public void Error(string message, params object[] args)
        {
            Debug.Print("Error: {0}", string.Format(message, args));
        }

        /// <inheritdoc/>
        public void Error(Exception ex, string message)
        {
            Debug.Print("Error: {0} {1}", message, ex.Message);
        }

        /// <inheritdoc/>
        public void Error(Exception ex, string message, params object[] args)
        {
            Debug.Print("Error: {0} {1}",
                string.Format(message, args),
                ex.Message);
        }

        /// <inheritdoc/>
        public void Error(ProgramAddress paddr, string message)
        {
            Debug.Print("Error: {0}: {1}", paddr.Address, message);
        }

        /// <inheritdoc/>
        public void Error(ICodeLocation location, string message)
        {
            Debug.Print("Error: {0}: {1}", location, message);
        }

        /// <inheritdoc/>
        public void Error(ICodeLocation location, string message, params object [] args)
        {
            Debug.Print("Error: {0}: {1}", location,
                string.Format(message, args));
        }

        /// <inheritdoc/>
        public void Error(ICodeLocation location, Exception ex, string message)
        {
            Debug.Print("Error: {0}: {1} {2}", location, message, ex.Message);
        }

        /// <inheritdoc/>
        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        { 
            Debug.Print("Error: {0}: {1} {2}", 
                location, 
                string.Format(message, args),
                ex.Message);
        }

        /// <inheritdoc/>
        public void Error(ScriptError scriptError)
        {
            Debug.Print(
                "Error: {0}: {1} {2}",
                scriptError.FileName,
                scriptError.Message,
                scriptError.Exception.Message);
        }

        /// <inheritdoc/>
        public void ShowStatus(string caption)
        {
            Debug.Print("Status: {0}", caption);
        }

        /// <inheritdoc/>
        public void Progress_ShowProgress(string caption, int numerator, int denominator)
        {
            //$TODO: show progress
        }

        /// <inheritdoc/>
        public void Progress_Advance(int advance)
        {
        }

        /// <inheritdoc/>
        public ICodeLocation CreateAddressNavigator(IReadOnlyProgram program, Address address)
        {
            return new NullCodeLocation(address.ToString());
        }

        /// <inheritdoc/>
        public ICodeLocation CreateProcedureNavigator(IReadOnlyProgram program, Procedure proc)
        {
            return new NullCodeLocation(proc.Name);
        }

        /// <inheritdoc/>
        public ICodeLocation CreateBlockNavigator(IReadOnlyProgram program, Block block)
        {
            return new NullCodeLocation(block.Id);
        }

        /// <inheritdoc/>
        public ICodeLocation CreateStatementNavigator(IReadOnlyProgram program, Statement stm)
        {
            return new NullCodeLocation(stm.Address.ToString());
        }

        /// <inheritdoc/>
        public ICodeLocation CreateJumpTableNavigator(IReadOnlyProgram _, IProcessorArchitecture arch, Address addrIndirectJump, Address? addrVector, int stride)
        {
            return new NullCodeLocation(addrIndirectJump.ToString());
        }

        /// <inheritdoc/>
        public bool IsCanceled()
        {
            return false;
        }

        #endregion
    }


    /// <summary>
    /// Null implementation of <see cref="ICodeLocation"/>.
    /// </summary>
    public class NullCodeLocation : ICodeLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="NullCodeLocation"/>.
        /// </summary>
        /// <param name="text">Text of this item.</param>
        public NullCodeLocation(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Display text for this item.
        /// </summary>
        public string Text { get; private set; }

        /// <inheritdoc/>
        public ValueTask NavigateTo()
        {
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Text;
        }
    }
}
