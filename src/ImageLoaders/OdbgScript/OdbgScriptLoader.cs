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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Services;
using Reko.Environments.Windows;
using Reko.ImageLoaders.MzExe;
using System;
using System.Collections.Generic;
using System.IO;
using rulong = System.UInt64;

namespace Reko.ImageLoaders.OdbgScript
{
    /// <summary>
    /// ImageLoader that uses OdbgScript to assist in the unpacking of 
    /// compressed or obfuscated binaries.
    /// </summary>
    /// <remarks>Uses the optional Argument property from the app.config
    /// file to specify the script file to use.</remarks>
    public class OdbgScriptLoader : ImageLoader
    {
        private readonly ImageLoader originalImageLoader;
        private Debugger debugger;
        private OllyLang scriptInterpreter;
        private rulong OldIP;

        public OdbgScriptLoader(ImageLoader imageLoader) 
            : base(imageLoader.Services, imageLoader.Filename, imageLoader.RawImage)
        {
            this.originalImageLoader = imageLoader;
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr32(0x00400000); }
            set { throw new NotImplementedException(); }
        }

        public SegmentMap ImageMap { get; set; }
        public IProcessorArchitecture Architecture { get; set; }

        /// <summary>
        /// Original entry point to the executable, before it was packed.
        /// </summary>
        public Address OriginalEntryPoint { get; set; }

        public override Program Load(Address addrLoad)
        {
            // First load the file. This gives us a (writeable) image and 
            // the packed entry point.
            var origLdr = this.originalImageLoader;
            var program = origLdr.Load(origLdr.PreferredBaseAddress);
            var rr = origLdr.Relocate(program, origLdr.PreferredBaseAddress);
            this.ImageMap = program.SegmentMap;
            this.Architecture = program.Architecture;

            var envEmu = program.Platform.CreateEmulator(program.SegmentMap, program.ImportReferences);
            var emu = program.Architecture.CreateEmulator(program.SegmentMap, envEmu);
            this.debugger = new Debugger(emu);
            this.scriptInterpreter = new OllyLang(Services, program.Architecture);
            this.scriptInterpreter.Host = new Host(this, program.SegmentMap);
            this.scriptInterpreter.Debugger = this.debugger;
            emu.InstructionPointer = rr.EntryPoints[0].Address;
            emu.BeforeStart += emu_BeforeStart;
            emu.ExceptionRaised += emu_ExceptionRaised;

            var stackSeg = envEmu.InitializeStack(emu, rr.EntryPoints[0].ProcessorState);
            LoadScript(Argument, scriptInterpreter.Script);
            emu.Start();
            envEmu.TearDownStack(stackSeg);

            foreach (var ic in envEmu.InterceptedCalls)
            {
                program.InterceptedCalls.Add(ic.Key, ic.Value);
            }
            return program;
        }

        public override ImageSegment AddSegmentReference(Address addr, ushort seg)
        {
            return originalImageLoader.AddSegmentReference(addr, seg);
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            var eps = new List<ImageSymbol>();
            var syms = new SortedList<Address, ImageSymbol>();
            if (OriginalEntryPoint != null)
            {
                var sym = ImageSymbol.Procedure(program.Architecture, OriginalEntryPoint, state:Architecture.CreateProcessorState());
                syms.Add(sym.Address, sym);
                eps.Add(sym);
            }
            return new RelocationResults(eps, syms);
        }

        public virtual PeImageLoader CreatePeImageLoader()
        {
            ExeImageLoader mz = new ExeImageLoader(Services, Filename, RawImage);
            var e_lfanew = mz.LoadLfaToNewHeader();
            if (!e_lfanew.HasValue)
                throw new BadImageFormatException();
            PeImageLoader pe = new PeImageLoader(Services, Filename, RawImage, e_lfanew.Value);
            return pe;
        }

        public virtual void LoadScript(string scriptFilename, OllyScript script)
        {
            // If the script file is not a rooted path, first try looking at 
            // the current directory. If there is no file there, try finding 
            // it in the installation directory. 
            var fsSvc = Services.RequireService<IFileSystemService>();
            if (!fsSvc.IsPathRooted(scriptFilename))
            {
                string curDir = fsSvc.GetCurrentDirectory();
                var absPath = Path.Combine(curDir, scriptFilename);
                if (fsSvc.FileExists(absPath))
                {
                    scriptFilename = absPath;
                }
                else
                {
                    var dir = Path.GetDirectoryName(GetType().Assembly.Location);
                    absPath = Path.Combine(dir, scriptFilename);
                    scriptFilename = absPath;
                }
            }
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
            return scriptInterpreter.Script.LoadFile(szFileName);
        }

        public bool ScripterLoadBuffer(string szScript)
        {
            scriptInterpreter.Reset();
            return scriptInterpreter.Script.LoadScriptFromString(szScript);
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
            if (scriptInterpreter.Script.IsLoaded && scriptInterpreter.Debugger.InitDebugEx(szDebuggee, null, null, AutoDebugEntry) != null)
            {
                scriptInterpreter.Debugger.DebugLoop();
                return true;
            }
            return false;
        }
    }
}
