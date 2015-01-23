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
using System.Threading.Tasks;

namespace Decompiler.ImageLoaders.OdbgScript
{
    using Decompiler.Arch.X86;
    using Decompiler.ImageLoaders.MzExe;
    using System.IO;
    using rulong = System.UInt64;


    /// <summary>
    /// /
    /// </summary>
    /// <remarks>Uses the optional Argument property from the app.config file to specify the
    /// script file to use.</remarks>
    public class OdbgScriptLoader : ImageLoader
    {
        private Debugger Debugger;

        public OdbgScriptLoader(IServiceProvider services, string filename, byte[] imgRaw)
            : base(services, filename, imgRaw)
        {
            ollylang = new OllyLang();
            Debugger = new Debugger();
        }

        public override Address PreferredBaseAddress
        {
            get { throw new NotImplementedException(); }
        }

        public override LoaderResults Load(Address addrLoad)
        {
            // First load the file as a PE Executable. This gives us a (writeable) image and 
            // the OEP.
            PeImageLoader pe = CreatePeImageLoader();
            LoaderResults lr = pe.Load(pe.PreferredBaseAddress);
            RelocationResults rr = pe.Relocate(pe.PreferredBaseAddress);

            // Load the script.
            LoadScript(Argument, ollylang.script);

            // Initialize the emulator instruction pointer.
            X86State state = (X86State)lr.Architecture.CreateProcessorState();
            X86Emulator emu = new X86Emulator((IntelArchitecture) lr.Architecture, lr.Image);    //$Create emulator?
            emu.InstructionPointer = rr.EntryPoints[0].Address;
            emu.BeforeStart += emu_BeforeStart;
            emu.BreakpointHit += emu_BreakPointHit;
            emu.ExceptionRaised += emu_ExceptionRaised;

            emu.Run();

            //$TODO: somehow collect the results of the script.
            throw new NotImplementedException();
        }

        public override RelocationResults Relocate(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public virtual PeImageLoader CreatePeImageLoader()
        {
            ExeImageLoader mz = new ExeImageLoader(Services, Filename, RawImage);
            PeImageLoader pe = new PeImageLoader(Services, Filename, RawImage, mz.e_lfanew);
            return pe;
        }

        public virtual void LoadScript(string scriptFilename, OllyLang.OllyScript script)
        {
            script.load_file(scriptFilename, null);
        }

        private void emu_BreakPointHit(object sender, EventArgs e)
        {
            ollylang.Step();
        }

        private void emu_ExceptionRaised(object sender, EventArgs e)
        {

        }

        private void emu_BeforeStart(object sender, EventArgs e)
        {
            ollylang.Reset();
            ollylang.debuggee_running = false;
            OldIP = 0;
            //if(FileNameFromHandle(debugEvent->u.CreateProcessInfo.hFile, TargetPath))
            //{
            //    strcpy(TargetDirectory, folderfrompath(TargetPath).c_str());
            ollylang.InitGlobalVariables();
            //}
        }

        // 
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


        //bool FileNameFromHandle(HANDLE hFile, char Name[MAX_PATH]);


        // TitanEngine plugin callbacks

        rulong OldIP;

        void TitanDebuggingCallBack(DEBUG_EVENT debugEvent, int CallReason)
        {
            switch (CallReason)
            {
            case Ue.UE_PLUGIN_CALL_REASON_PREDEBUG:
                ollylang.Reset();
                ollylang.debuggee_running = false;
                OldIP = 0;
                break;
            case Ue.UE_PLUGIN_CALL_REASON_POSTDEBUG:
                break;
            case Ue.UE_PLUGIN_CALL_REASON_EXCEPTION:
                switch (debugEvent.dwDebugEventCode)
                {
                case DEBUG_EVENT.CREATE_PROCESS_DEBUG_EVENT:
                    //if(FileNameFromHandle(debugEvent->u.CreateProcessInfo.hFile, TargetPath))
                    //{
                    //    strcpy(TargetDirectory, folderfrompath(TargetPath).c_str());
                    ollylang.InitGlobalVariables();
                    //}
                    break;
                case DEBUG_EVENT.EXCEPTION_DEBUG_EVENT:
                    if (ollylang.script_running)
                    {
                        rulong NewIP = Debugger.GetContextData(eContextData.UE_CIP);
                        //if(debugEvent.u.Exception.ExceptionRecord.ExceptionCode == 1) // EXCEPTION_BREAKPOINT)
                        NewIP--;

                        //DBG_LOG("Exception debug event @ " + Helper.rul2hexstr(NewIP));   //$LATER

                        if (NewIP != OldIP)
                            ollylang.debuggee_running = false;

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

        private OllyLang ollylang;

        //
        public bool ScripterLoadFile(string szFileName)
        {
            ollylang.Reset();
            return ollylang.script.load_file(szFileName);
        }


        public bool ScripterLoadBuffer(string szScript)
        {
            ollylang.Reset();
            return ollylang.script.load_buff(szScript);
        }

        bool ScripterResume()
        {
            return ollylang.Run();
        }

        bool ScripterPause()
        {
            return ollylang.Pause();
        }

        private void AutoDebugEntry()
        {
            ScripterResume();
        }
        bool ScripterAutoDebug(string szDebuggee)
        {
            if (ollylang.script.isloaded() && ollylang.Debugger.InitDebugEx(szDebuggee, null, null, AutoDebugEntry) != null)
            {
                ollylang.Debugger.DebugLoop();
                return true;
            }
            return false;
        }

    }
}
