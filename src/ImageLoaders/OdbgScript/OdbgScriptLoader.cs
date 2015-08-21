﻿#region License
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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.OdbgScript
{
    using Reko.Arch.X86;
    using Reko.Environments.Win32;
    using Reko.ImageLoaders.MzExe;
    using System.IO;
    using rulong = System.UInt64;

    /// <summary>
    /// ImageLoader that uses OdbgScript to assist in the unpacking of 
    /// compressed or obfuscated binaries.
    /// </summary>
    /// <remarks>Uses the optional Argument property from the app.config
    /// file to specify the script file to use.</remarks>
    public class OdbgScriptLoader : ImageLoader
    {
        private Debugger debugger;
        private OllyLang scriptInterpreter;
        private rulong OldIP;

        public OdbgScriptLoader(IServiceProvider services, string filename, byte[] imgRaw)
            : base(services, filename, imgRaw)
        {
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr32(0x00400000); }
            set { throw new NotImplementedException(); }
        }

        public LoadedImage Image { get; private set; }
        public ImageMap ImageMap { get; set; }
        public IntelArchitecture Architecture { get; set; }

        /// <summary>
        /// Original entry point to the executable, before it was packed.
        /// </summary>
        public Address OriginalEntryPoint { get; set; }

        public override Program Load(Address addrLoad)
        {
            // First load the file as a PE Executable. This gives us a (writeable) image and 
            // the packed entry point.
            var pe = CreatePeImageLoader();
            var program = pe.Load(pe.PreferredBaseAddress);
            var rr = pe.Relocate(program, pe.PreferredBaseAddress);
            this.Image = program.Image;
            this.ImageMap = program.ImageMap;
            this.Architecture = (IntelArchitecture)program.Architecture;

            var win32 = new Win32Emulator(program.Image, program.Platform, program.ImportReferences);
            var state = (X86State)program.Architecture.CreateProcessorState();
            var emu = new X86Emulator((IntelArchitecture) program.Architecture, program.Image, win32);
            this.debugger = new Debugger(emu);
            this.scriptInterpreter = new OllyLang();
            this.scriptInterpreter.Host = new Host(this);
            this.scriptInterpreter.Debugger = this.debugger;
            emu.InstructionPointer = rr.EntryPoints[0].Address;
            emu.WriteRegister(Registers.esp, (uint)Image.BaseAddress.ToLinear() + 0x1000 - 4u);
            emu.BeforeStart += emu_BeforeStart;
            emu.ExceptionRaised += emu_ExceptionRaised;

            // Load the script.
            LoadScript(Argument, scriptInterpreter.script);

            emu.Start();

            foreach (var ic in win32.InterceptedCalls)
            {
                program.InterceptedCalls.Add(Address.Ptr32(ic.Key), ic.Value);
            }
            return program;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            var eps = new List<EntryPoint>();
            if (OriginalEntryPoint != null)
                eps.Add(new EntryPoint(OriginalEntryPoint, Architecture.CreateProcessorState()));
            return new RelocationResults(eps, new RelocationDictionary());
        }

        public virtual PeImageLoader CreatePeImageLoader()
        {
            ExeImageLoader mz = new ExeImageLoader(Services, Filename, RawImage);
            PeImageLoader pe = new PeImageLoader(Services, Filename, RawImage, mz.e_lfanew);
            return pe;
        }

        public virtual void LoadScript(string scriptFilename, OllyScript script)
        {
            script.LoadFile(scriptFilename, null);
        }

        private void emu_ExceptionRaised(object sender, EventArgs e)
        {
        }

        private void emu_BeforeStart(object sender, EventArgs e)
        {
            scriptInterpreter.Reset();
            scriptInterpreter.debuggee_running = false;
            OldIP = 0;
            scriptInterpreter.InitGlobalVariables();
            ScripterResume();
        }

        /*
        Design:

        ScripterResume MUST be called from within the debug loop
         - BP callback
         - or via plugin interface:
           + Call to ScripterAutoDebug which loads exe and calls DebugLoop and calls ScripterResume on EP
        it will immediately return, this is needed for returning to the debug loop
        and executing until a breakpoint/exception occurs:

        / + DebugLoop()
        ^   + OnBP/OnException callback
        |     + OllyLang::Step()
        ^	  [do commands until return to loop is required (RUN, STI, etc.)]
        |     -
        ^   -
        \ -

        When done, call FinishedCallback
        (if script loaded inside debug loop and not via ScripterExecuteScript)
        or return
        */

        // TitanEngine plugin callbacks
        void TitanDebuggingCallBack(DEBUG_EVENT debugEvent, int CallReason)
        {
            switch (CallReason)
            {
            case Ue.UE_PLUGIN_CALL_REASON_POSTDEBUG:
                break;
            case Ue.UE_PLUGIN_CALL_REASON_EXCEPTION:
                switch (debugEvent.dwDebugEventCode)
                {
                case DEBUG_EVENT.CREATE_PROCESS_DEBUG_EVENT:
                    scriptInterpreter.InitGlobalVariables();
                    break;
                case DEBUG_EVENT.EXCEPTION_DEBUG_EVENT:
                    if (scriptInterpreter.script_running)
                    {
                        rulong NewIP = debugger.GetContextData(eContextData.UE_CIP);
                        //if(debugEvent.u.Exception.ExceptionRecord.ExceptionCode == 1) // EXCEPTION_BREAKPOINT)
                        NewIP--;

                        //DBG_LOG("Exception debug event @ " + Helper.rul2hexstr(NewIP));   //$LATER

                        if (NewIP != OldIP)
                            scriptInterpreter.debuggee_running = false;

                        //$LATER
                        //if(!debugEvent.u.Exception.dwFirstChance)
                        //    ollylang.OnException();

                        OldIP = NewIP;
                    }
                    break;
                }
                break;
            }
        }

        public bool ScripterLoadFile(string szFileName)
        {
            scriptInterpreter.Reset();
            return scriptInterpreter.script.LoadFile(szFileName);
        }

        public bool ScripterLoadBuffer(string szScript)
        {
            scriptInterpreter.Reset();
            return scriptInterpreter.script.LoadScriptFromString(szScript);
        }

        public bool ScripterResume()
        {
            scriptInterpreter.Run();
            return true;
        }

        bool ScripterPause()
        {
            return scriptInterpreter.Pause();
        }

        private void AutoDebugEntry()
        {
            ScripterResume();
        }

        bool ScripterAutoDebug(string szDebuggee)
        {
            if (scriptInterpreter.script.IsLoaded && scriptInterpreter.Debugger.InitDebugEx(szDebuggee, null, null, AutoDebugEntry) != null)
            {
                scriptInterpreter.Debugger.DebugLoop();
                return true;
            }
            return false;
        }
    }
}
