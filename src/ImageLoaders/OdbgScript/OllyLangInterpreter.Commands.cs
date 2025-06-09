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
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    // https://github.com/x64dbg/ODbgScript/tree/master/ODbgScript
    public partial class OllyLangInterpreter
    {
        const int MAX_INSTR_SIZE = 16;      // x86
        const int PAGE_SIZE = 4096;         // x86 sometimes.

        bool DoADD(Expression[] args)
        {
            if (args.Length == 2)
            {
                if (GetAddress(args[0], out Address addr) && GetUlong(args[1], out ulong dw))
                {
                    return SetAddress(args[0], addr + dw);
                }
                if (GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
                {
                    return SetULong(args[0], dw1 + dw2);
                }
                else if (GetFloat(args[0], out double flt1) && GetFloat(args[1], out double flt2))
                {
                    return SetFloat(args[0], flt1 + flt2);
                }
                else if (GetFloat(args[0], out flt1) && GetUlong(args[1], out dw2))
                {
                    return SetFloat(args[0], flt1 + dw2);
                }
                else if (GetString(args[0], out string? str1) && GetUlong(args[1], out dw2))
                {
                    Var v1 = Var.Create(str1), v2 = Var.Create(dw2);
                    Var v3 = v1 + v2;
                    return SetString(args[0], v3.str!);
                }
                else if ((GetString(args[0], out str1) && GetAnyValue(args[1], out string? str2)) || (GetAnyValue(args[0], out str1) && GetAnyValue(args[1], out str2)))
                {
                    Var v1 = Var.Create(str1!), v2 = Var.Create(str2!);
                    Var v3 = v1 + v2;
                    return SetString(args[0], v3.str!);
                }
            }
            return false;
        }

        private bool DoAI(Expression[] args)
        {
            if (args.Length == 0)
            {
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoALLOC(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong size))
            {
                Address addr = Host.AllocateMemory(size);
                variables["$RESULT"] = Var.Create(addr);
                if (!addr.IsNull)
                    regBlockToFree(addr, size, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Analyze the module containing the address <paramref name='addr' />
        /// </summary>
        /// <remarks>
        /// Only valid for ODBG, we ignore the command here.
        /// </remarks>
        private bool DoAN(Expression[] args)
        {
            if (args.Length == 1 && GetAddress(args[0], out Address addr))
            {
                return true;
            }
            return false;
        }

        private bool DoAND(Expression[] args)
        {
            if (args.Length == 2 && GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
            {
                return SetULong(args[0], dw1 & dw2);
            }
            return false;
        }

        private bool DoAO(Expression[] args)
        {
            if (args.Length == 0)
            {
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoASK(Expression[] args)
        {
            if (args.Length == 1 && GetString(args[0], out string? title))
            {
                variables["$RESULT"] = variables["$RESULT_1"] = Var.Create(0);

                if (Host.DialogASK(title, out string returned))
                {
                    if (Helper.is_hex(returned))
                    {
                        variables["$RESULT"] = Var.Create(Helper.hexstr2rul(returned));
                        variables["$RESULT_1"] = Var.Create((returned.Length + 1) / 2); // size in bytes rounded to 2
                    }
                    else
                    {
                        returned = Helper.UnquoteString(returned, '"'); // To Accept input like "FFF" (forces string)
                        variables["$RESULT"] = Var.Create(returned);
                        variables["$RESULT_1"] = Var.Create(returned.Length);
                    }
                }
                else
                    Pause();
                return true;
            }
            return false;
        }

        private bool DoASM(Expression[] args)
        {
            if (args.Length >= 2 && args.Length <= 3 && GetAddress(args[0], out Address addr) && GetString(args[1], out string? cmd))
            {
                if (args.Length == 3 && !GetUlong(args[2], out ulong attempt))
                    return false;

                int len = Host.Assemble(FormatAsmDwords(cmd), addr);
                if (len == 0)
                {
                    errorstr = "Invalid command: " + cmd;
                    return false;
                }
                variables["$RESULT"] = Var.Create((ulong) len);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Assemble a text asm file at some address.
        /// </summary>
        /// <remarks>
        /// Example:
        /// <code>
        /// asmtxt EIP, "myasm.txt"
        /// </code>
        /// </remarks>
        private bool DoASMTXT(Expression[] args)
        {
            if (args.Length == 2 && GetAddress(args[0], out Address addr) && GetString(args[1], out string? asmfile))
            {
                int totallen = 0;
                List<string> lines = Host.getlines_file(asmfile);
                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (line.Length != 0)
                    {
                        int len = Host.Assemble(FormatAsmDwords(line), addr + totallen);
                        if (len == 0)
                        {
                            errorstr = "Invalid command: " + line;
                            return false;
                        }
                        totallen += len;
                    }
                }
                variables["$RESULT"] = Var.Create(totallen);
                variables["$RESULT_1"] = Var.Create(lines.Count);
                return true;
            }
            return false;
        }

        private bool DoATOI(Expression[] args)
        {
            ulong @base = 16;

            if (args.Length >= 1 && args.Length <= 2 && GetString(args[0], out string? str))
            {
                if (args.Length == 2 && !GetUlong(args[1], out @base))
                    return false;

                variables["$RESULT"] = Var.Create(Helper.str2rul(str, (uint) @base));
                return true;
            }
            return false;
        }

        private bool DoBC(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0)
                {
                    return DoBCA();
                }
                else if (GetAddress(args[0], out Address addr))
                {
                    Debugger.DeleteBPX(addr);
                    return true;
                }
            }
            return false;
        }

        private bool DoBCA(params Expression[] args)
        {
            if (args.Length == 0)
            {
                Debugger.RemoveAllBreakPoints(Ue.UE_OPTION_REMOVEALLDISABLED);
                Debugger.RemoveAllBreakPoints(Ue.UE_OPTION_REMOVEALLENABLED);
                return true;
            }
            return false;
        }

        private bool DoBD(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0)
                {
                    return DoBDA();
                }
                else if (GetAddress(args[0], out Address addr))
                {
                    Debugger.DisableBPX(addr);
                    return true;
                }
            }
            return false;
        }

        private bool DoBDA(params Expression[] args)
        {
            if (args.Length == 0)
            {
                Debugger.RemoveAllBreakPoints(Ue.UE_OPTION_DISABLEALL); // int3 only?
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoBEGINSEARCH(Expression[] args)
        {
            ulong start = 0;

            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 1 && !GetUlong(args[0], out start))
                    return false;

                return true;
            }
            return false;
        }

        // Olly only
        private bool DoENDSEARCH(params Expression[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set a BreakPoint.
        /// </summary>
        private bool DoBP(Expression[] args)
        {
            if (args.Length == 1 && GetAddress(args[0], out Address addr))
            {
                Debugger.SetBPX(addr, Ue.UE_BREAKPOINT, SoftwareCallback);
                return true;
            }
            return false;
        }

        // TE?
        private bool DoBPCND(Expression[] args)
        {
            if (args.Length == 2 && GetUlong(args[0], out ulong addr) && GetString(args[1], out string? condition))
            {
                errorstr = "Unsupported command!";
                return false;

                /*
                Setbreakpoint(addr, TY_ACTIVE, 0);
                Insertname(addr, NM_BREAK, (char *)condition);
                Deletenamerange(addr, addr + 1, NM_BREAKEXPL);
                Deletenamerange(addr, addr + 1, NM_BREAKEXPR);
                return true;
                */
            }
            return false;
        }

        private bool DoBPD(Expression[] args)
        {
            if (args.Length == 1)
            {
                return DoBPX(args[0], Constant.UInt64(1));
            }
            return false;
        }

        private bool DoBPGOTO(Expression[] args)
        {
            if (args.Length == 2 && GetAddress(args[0], out Address addr) && Script.TryGetLabel(args[1], out var bpLabel))
            {
                bpjumps[addr] = bpLabel;
                return true;
            }
            return false;
        }

        private bool DoBPHWCA(params Expression[] args)
        {
            if (args.Length == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Debugger.DeleteHardwareBreakPoint(i);
                }
                return true;
            }
            return false;
        }

        private bool DoBPHWC(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0)
                {
                    return DoBPHWCA();
                }
                else if (GetUlong(args[0], out ulong addr))
                {
                    ulong[] DRX = new ulong[4];

                    DRX[0] = Debugger.GetContextData(eContextData.UE_DR0);
                    DRX[1] = Debugger.GetContextData(eContextData.UE_DR1);
                    DRX[2] = Debugger.GetContextData(eContextData.UE_DR2);
                    DRX[3] = Debugger.GetContextData(eContextData.UE_DR3);
                    for (int i = 0; i < DRX.Length; i++)
                    {
                        if (DRX[i] == addr)
                        {
                            Debugger.DeleteHardwareBreakPoint(i);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set a hardware breakpoint. An optional type can specify the type of breakpoint
        /// r: break when memory address is read.
        /// w: break when memory address is written.
        /// x: break when program execution reaches the given address.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool DoBPHWS(Expression[] args)
        {
            string? typestr = "x";

            if (args.Length >= 1 && args.Length <= 2 && GetAddress(args[0], out Address addr))
            {
                if (args.Length == 2 && (!GetString(args[1], out typestr) || typestr.Length != 1))
                    return false;

                eHWBPType type;

                switch (typestr[0])
                {
                case 'r': type = Ue.UE_HARDWARE_READWRITE; break;
                case 'w': type = Ue.UE_HARDWARE_WRITE; break;
                case 'x': type = Ue.UE_HARDWARE_EXECUTE; break;
                default: return false;
                }

                Debugger.SetHardwareBreakPoint(addr, null, type, Ue.UE_HARDWARE_SIZE_1, HardwareCallback);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets logging breakpoint at address addr that logs expression expr
        /// </summary>
        private bool DoBPL(Expression[] args)
        {
            if (args.Length == 2 && GetAddress(args[0], out Address addr) &&
                GetString(args[1], out string? expression))
            {
                errorstr = "Unsupported command!";
                return false;

                /*
                expression = 'E' + expression; // 0x45//COND_NOBREAK | COND_LOGALWAYS | COND_ARGALWAYS | COND_FILLING
		
                Setbreakpoint(addr, TY_ACTIVE, 0);
                Deletenamerange(addr, addr + 1, NM_BREAK);
                Deletenamerange(addr, addr + 1, NM_BREAKEXPL);
                Insertname(addr, NM_BREAKEXPR, expression);
                return true;
                */
            }
            return false;
        }

        /// <summary>
        ///    Sets logging breakpoint at address addr that logs expression expr if condition cond is true
        /// Example:
        ///    bplcnd 401000, "eax", "eax > 1" // logs the value of eax everytime this line is passed and eax > 1
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>> 1

        private bool DoBPLCND(Expression[] args)
        {
            if (args.Length == 3 && GetAddress(args[0], out Address addr) &&
                GetString(args[1], out string? expression) &&
                GetString(args[2], out string? condition))
            {
                errorstr = "Unsupported command!";
                return false;

                /*
                Setbreakpoint(addr, TY_ACTIVE, 0);
                Deletenamerange(addr, addr + 1, NM_BREAKEXPL);
                Insertname(addr, NM_BREAK, condition);
                expression = 'C' + expression; // 0x43
                Insertname(addr, NM_BREAKEXPR, expression);
                return true;
                */
            }
            return false;
        }

        /// <summary>
        /// Clear memory breakpoint.
        /// </summary>
        private bool DoBPMC(params Expression[] args)
        {
            if (args.Length == 0)
            {
                if (membpaddr is not null && membpsize != 0)
                {
                    Debugger.RemoveMemoryBPX(membpaddr.Value, membpsize);
                    membpaddr = null;
                    membpsize = 0;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set memory breakpoint on read.
        /// </summary>
        private bool DoBPRM(Expression[] args)
        {
            if (args.Length == 2 && GetAddress(args[0], out Address addr) && GetUlong(args[1], out ulong size))
            {
                if (membpaddr is not null && membpsize != 0)
                    DoBPMC();

                if (Debugger.SetMemoryBPXEx(addr, size, Ue.UE_MEMORY_READ, true, MemoryCallback))
                {
                    membpaddr = addr;
                    membpsize = size;
                }
                return true;
            }
            return false;
        }

        private bool DoBPWM(Expression[] args)
        {
            if (args.Length == 2 && GetAddress(args[0], out Address addr) && GetUlong(args[1], out ulong size))
            {
                if (membpaddr is not null && membpsize != 0)
                    DoBPMC();

                if (Debugger.SetMemoryBPXEx(addr, size, Ue.UE_MEMORY_WRITE, true, MemoryCallback))
                {
                    membpaddr = addr;
                    membpsize = size;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set a breakpoint on call to an imported API call.
        /// </summary>
        private bool DoBPX(params Expression[] args)
        {
            if (args.Length >= 1 && args.Length <= 2 && GetString(args[0], out string? callname))
            {
                if (args.Length == 2 && !GetUlong(args[1], out ulong del))
                    return false;

                errorstr = "Unsupported command!";
                return false;

                /*
                int bpnmb = 0;
                //int args.Length;
                //int i;

                //t_table *reftable;
                //t_ref *pref;

                char findname[256] = {0};

                if(callname == "") 
                {
                    errorstr = "Function name missed";
                    return false;
                }

		
                char name[256];
                strcpy(name, callname);

                Findalldllcalls((t_dump *)Plugingetvalue(VAL_CPUDASM),0,"Intermodular calls");
                reftable=(t_table *)Plugingetvalue(VAL_REFERENCES);

                if(reftable is null || reftable.data.n == 0)
                {
                    errorstr = "No references";
                    return false;
                }

                 if(reftable.data.itemsize<sizeof(t_ref))
                {
                    errorstr = "Old version of OllyDbg";
                    return false;
                }

                for(i = 0; i < reftable.data.n; i++) 
                {
                    // The safest way: size of t_ref may change in the future!
                    pref=(t_ref *)((char *)reftable.data.data+reftable.data.itemsize*i);

                    if(Findlabel(pref.dest, findname) == 0) 
                    {// Unnamed destination
                        continue;
                    }
   
                    if(!_stricmp(name, findname)) 
                    {
                        if(!del) 
                        {   // Set breakpoint
                            SetBPX(pref.addr, UE_BREAKPOINT, &SoftwareCallback);
                            bpnmb++;
                        }
                        else 
                        {
                            DeleteBPX(pref.addr);
                            bpnmb++;
                        }
                    }
                }
                variables["$RESULT"] = bpnmb;
                return true;
                */
            }
            return false;
        }

        /// <summary>
        /// Converts string/dword variable to a Buffer
        /// </summary>
        /// <remarks>
        /// Example:
	    /// <code>
        /// mov s, "123"
        /// buf s
        /// log s // output #313233#
        /// </code>
        /// </remarks>
        private bool DoBUF(Expression[] args)
        {
            if (args.Length == 1 && args[0] is Identifier id && IsVariable(id.Name))
            {
                Var v = variables[id.Name];

                switch (v.type)
                {
                case Var.EType.STR: // empty buf + str . buf
                    if (!v.IsBuf)
                    {
                        v = Var.Create("##") + v.str!;
                        variables[id.Name] = v;
                    }
                    break;
                case Var.EType.DW: // empty buf + dw . buf
                    v = Var.Create("##") + v.ToUInt64();
                    variables[id.Name] = v;
                    break;
                }
                return true;
            }
            return false;
        }

        private bool DoCALL(params Expression[] args)
        {
            if (args.Length == 1 && Script.TryGetLabel(args[0], out int lineNumber))
            {
                calls.Add(script_pos + 1);
                return DoJMP(args);
            }
            return false;
        }

        private static readonly string[] valid_commands =
        {
            "SCRIPT", "SCRIPTLOG", "MODULES", "MEMORY",
            "THREADS", "BREAKPOINTS", "REFERENCES", "SOURCELIST",
            "WATCHES", "PATCHES", "CPU", "RUNTRACE",
            "WINDOWS", "CALLSTACK", "LOG", "TEXT",
            "FILE", "HANDLES", "SEH", "SOURCE"
        };

        private bool DoCLOSE(Expression[] args)
        {
            throw new NotImplementedException();
#if LATER
ulong hwnd;

	if(args.Length == 1)
	{
		if(valid_commands+_countof(valid_commands) != find(valid_commands, valid_commands+_countof(valid_commands), Helper.toupper(args[0])))
		{
			return true;
		}
		else if(GetUlong(args[0],  out hwnd) && hwnd)
		{
			DestroyWindow((HWND)hwnd);
			return true;
		}
		errorstr = "Bad operand";
	}
	return false;
#endif
        }

        private bool DoCMP(Expression[] args)
        {
            ulong size = 0;

            if (args.Length >= 2 && args.Length <= 3)
            {
                if (args.Length == 3 && !GetUlong(args[2], out size))
                    return false;

                Var v1, v2;
                if (GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
                {
                    v1 = Var.Create(dw1);
                    v2 = Var.Create(dw2);
                }
                else if (GetAddress(args[0], out Address addr1) && GetUlong(args[1], out ulong dw))
                {
                    v1 = Var.Create(addr1.ToLinear());
                    v2 = Var.Create(dw);
                }
                else if (GetFloat(args[0], out double flt1) && GetFloat(args[1], out double flt2))
                {
                    v1 = Var.Create(flt1);
                    v2 = Var.Create(flt2);
                }
                else if (GetAnyValue(args[0], out string? s1, true) && GetAnyValue(args[1], out string? s2, true))
                {
                    // see also SCMP command, code is not finished here...
                    v1 = Var.Create(s1!);
                    v2 = Var.Create(s2!);
                }
                else
                    return false;

                if (size > 0)
                {
                    v1.resize(8 * (int) size);
                    v2.resize(8 * (int) size);
                }

                int res = v1.Compare(v2); //Error if -2 (type mismatch)
                if (res != -2)
                {
                    SetCMPFlags(res);
                    return true;
                }
            }
            return false;
        }

        // Olly only
        private bool DoCMT(Expression[] args)
        {
            if (args.Length == 2 && GetUlong(args[0], out ulong addr) && GetString(args[1], out string? cmt))
            {
                return true;
            }
            return false;
        }

        private bool DoCOB(Expression[] args)
        {
            if (args.Length == 0)
            {
                EOB_row = -1;
                return true;
            }
            return false;
        }

        private bool DoCOE(Expression[] args)
        {
            if (args.Length == 0)
            {
                EOE_row = -1;
                return true;
            }
            return false;
        }

        private bool DoDBH(Expression[] args)
        {
            throw new NotImplementedException();
#if NEVER
	if(args.Length == 0)
	{
		Hider.HideDebugger(Host.TE_GetProcessHandle(), Ue.UE_HIDE_PEBONLY);
		return true;
	}
	return false;
#endif
        }

        private bool DoDBS(Expression[] args)
        {
            throw new NotImplementedException();
#if NEVER
    if (args.Length == 0)
	{
		Hider.UnHideDebugger(Host.TE_GetProcessHandle(), Ue.UE_HIDE_PEBONLY);
		return true;
	}
	return false;
#endif
        }

        private bool DoDEC(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (GetUlong(args[0], out ulong dw))
                {
                    dw--;
                    return SetULong(args[0], dw);
                }
                else if (GetFloat(args[0], out double flt))
                {
                    flt--;
                    return SetFloat(args[0], flt);
                }
            }
            return false;
        }

        private bool DoDIV(Expression[] args)
        {
            if (args.Length == 2)
            {
                if (GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2) && dw2 != 0)
                {
                    return SetULong(args[0], dw1 / dw2);
                }
                else if (GetFloat(args[0], out double flt1) && GetFloat(args[1], out double flt2) && flt2 != 0)
                {
                    return SetFloat(args[0], flt1 / flt2);
                }
                else if (GetFloat(args[0], out flt1) && GetUlong(args[1], out dw2) && dw2 != 0)
                {
                    return SetFloat(args[0], flt1 / dw2);
                }
                else if (GetUlong(args[0], out dw1) && GetFloat(args[1], out flt2) && flt2 != 0)
                {
                    return SetFloat(args[0], dw1 / flt2);
                }
                else
                {
                    errorstr = "Division by 0";
                    return false;
                }
            }
            return false;
        }

        private bool DoDM(Expression[] args)
        {
            if (args.Length == 3 && GetUlong(args[0], out ulong addr) && GetUlong(args[1], out ulong size) && GetString(args[2], out string? filename))
            {
                if (!Path.IsPathRooted(filename))
                    filename = Host.TE_GetTargetDirectory() + filename;

                // Truncate existing file
                var fsSvc = services.RequireService<IFileSystemService>();
                using (Stream hfile = fsSvc.CreateFileStream(filename, FileMode.Create, FileAccess.Write))
                {
                }
                return DoDMA(args);
            }
            return false;
        }

        private bool DoDMA(Expression[] args)
        {
            if (args.Length == 3 &&
                GetAddress(args[0], out Address addr) &&
                GetUlong(args[1], out ulong uSize) &&
                GetString(args[2], out string? filename))
            {
                int size = (int) uSize;
                if (!Path.IsPathRooted(filename))
                    filename = Path.Combine(Host.TE_GetTargetDirectory()!, filename);

                variables["$RESULT"] = Var.Create(size);

                Stream? hFile = null;
                try
                {
                    hFile = new FileStream(filename, FileMode.Append, FileAccess.Write);
                    byte[] membuf = new byte[PAGE_SIZE];
                    hFile.Seek(0, SeekOrigin.End);
                    while (size >= membuf.Length)
                    {
                        if (!Host.TryReadBytes(addr, membuf.Length, membuf))
                        {
                            Array.Clear(membuf, 0, membuf.Length);
                        }
                        hFile.Write(membuf, 0, membuf.Length);
                        addr += (uint) membuf.Length;
                        size -= membuf.Length;
                    }

                    if (size > 0)
                    {
                        if (!Host.TryReadBytes(addr, size, membuf))
                        {
                            Array.Clear(membuf, 0, size);
                        }
                        hFile.Write(membuf, 0, size);
                    }
                    return true;
                }
                catch
                {
                    errorstr = "Couldn't create file";
                }
                finally
                {
                    hFile?.Close();
                }
            }
            return false;
        }

        private bool DoDPE(Expression[] args)
        {
            IProcessorArchitecture? arch = null;
            if (args.Length >= 2 &&
                GetString(args[0], out string? filename) &&
                GetAddress(args[1], out Address ep) &&
                (args.Length == 2 || GetArchitecture(args[2], out arch)))
            {
                // We're cheating here and not actually dumping to disk. We don't need to 
                // because the image is unpacked in memory, and we are now ready to start
                // partying on it. All we have to do now is tell the caller how the value
                // of the original entry point, which is conveniently passed in the other
                // argument.
                arch ??= this.arch;
                var sym = ImageSymbol.Procedure(arch, ep, state: arch.CreateProcessorState());
                Host.OriginalEntryPoint = sym;
                return true;
            }
            return false;
        }

        private bool DoENDE(Expression[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoERUN(Expression[] args)
        {
            if (args.Length == 0)
            {
                ignore_exceptions = true;
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoESTI(Expression[] args)
        {
            if (args.Length == 0)
            {
                ignore_exceptions = true;
                stepcount = 1;
                StepIntoCallback();
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoESTEP(Expression[] args)
        {
            if (args.Length == 0)
            {
                ignore_exceptions = true;
                stepcount = 1;
                StepOverCallback();
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Transfer execution to a label when next breakpoint is hit.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool DoEOB(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0) // Go interactive
                {
                    EOB_row = -1;
                    return true;
                }
                else if (Script.TryGetLabel(args[0], out EOB_row)) // Set label to go to
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Transfer script execution to a label when next exception is hit.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool DoEOE(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0) // Go interactive
                {
                    EOE_row = -1;
                    return true;
                }
                else if (Script.TryGetLabel(args[0], out int lineNumber)) // Set label to go to
                {
                    EOE_row = lineNumber;
                    return true;
                }
            }
            return false;
        }

        private bool DoEVAL(Expression[] args)
        {
            if (args.Length == 1 && GetString(args[0], out string? to_eval))
            {
                variables["$RESULT"] = Var.Create(InterpolateVariables(to_eval, false));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Executes instructions between EXEC and ENDE in the context of the target process.
        /// Values in curly braces { }
        /// are replaced by their values.
        /// PUSHA / POPA commands could be useful when you use this.
        /// </summary>
        /// <remarks>
        /// Examples:
        /// This does some mov's
        /// <code>
        /// mov x, "eax"
        /// mov y, DEADBEEF
        /// 
        /// exec
        ///     mov {x},{y}  // mov eax, 0DEADBEEF will be executed
        ///     mov ecx, {x} // mov ecx, eax will be executed
        /// ende
        /// </code>
        /// This calls ExitProcess in the debugged application<code>
        /// exec
        ///	push 0
        ///	call ExitProcess
        /// ende
        /// ret
        /// </code>
        /// </remarks>

        private bool DoEXEC(Expression[] args)
        {
            if (args.Length == 0)
            {
                int first = script_pos + 1;
                int ende = Script.NextCommandIndex(first);

                if (ende > Script.Lines.Count)
                {
                    errorstr = "EXEC needs ENDE command!";
                    return false;
                }

                // max size for all commands + jmp eip
                int memsize = (int)(ende - first + 1) * MAX_INSTR_SIZE;

                pmemforexec = Host.AllocateMemory((uint)memsize);
                if (pmemforexec is not null)
                {
                    int len, totallen = 0;

                    for (int i = first; i < ende; i++)
                    {
                        string line = InterpolateVariables(Script.Lines[(int)i].RawLine!, true);
                        len = Host.Assemble(line, pmemforexec.Value + totallen);
                        if (len == 0)
                        {
                            Host.FreeMemory(pmemforexec.Value);
                            errorstr = "Invalid command: " + line;
                            return false;
                        }
                        totallen += len;
                    }

                    //return at ENDE
                    script_pos_next = ende;

                    Address eip = Debugger.InstructionPointer;

                    // Add jump to original EIP
                    string jmpstr = "jmp " + eip.ToString();
                    Address jmpaddr = pmemforexec.Value + (uint)totallen;

                    len = Host.Assemble(jmpstr, pmemforexec.Value + totallen);
                    totallen += len;

                    // Set new eip and run to the original one
                    Debugger.SetContextData(eContextData.UE_CIP, pmemforexec.Value.ToLinear());

                    // ignore next breakpoint
                    bInternalBP = true;

                    debuggee_running = false; // :s

                    //Debugger.SetBPX(jmpaddr, UE_BREAKPOINT, &EXECJMPCallback);
                    Debugger.SetBPX(eip, Ue.UE_BREAKPOINT, SoftwareCallback);

                    var block = new t_dbgmemblock
                    {
                        address = pmemforexec,
                        size = (uint) memsize,
                        script_pos = script_pos_next,
                        free_at_ip = eip,
                        autoclean = true
                    };
                    regBlockToFree(block);
                    resumeDebuggee = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// FILL [addr],[length],[value] fills a chunk of memory with a byte value.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool DoFILL(Expression[] args)
        {
            byte val = 0;
            if (args.Length == 3 && GetAddress(args[0], out Address addr) && GetUlong(args[1], out ulong len) && GetByte(args[2], out val))
            {
                byte[] membuf = new byte[PAGE_SIZE];
                int cb = Math.Min((int)len, membuf.Length);
                for (int i = 0; i < cb; ++i)
                {
                    membuf[i] = val;
                }
                
                // First write full pages, then partial page.
                while (len >= (ulong)membuf.Length)
                {
                    if (!Host.WriteMemory(addr, membuf.Length, membuf))
                        return false;

                    addr += (ulong)membuf.Length;
                    len -= (ulong)membuf.Length;
                }
                if (len > 0 && !Host.WriteMemory(addr, (int)len, membuf))
                    return false;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Searches 
        /// Searches memory starting at addr for the specified value.
        ///  found sets the reserved $RESULT variable. $RESULT == 0 if nothing found.
        /// The search string can also use the wildcard "??" (see below).
        /// </summary>
        /// <remarks>
        /// Example:
        /// <code>
        /// find eip, #6A00E8# // find a PUSH 0 followed by some kind of call
	    /// find eip, #6A??E8# // find a PUSH &lt;something&gt; followed by some kind of call
        /// </code>
        /// </remarks>
        private bool DoFIND(params Expression[] args)
        {
            string? finddata;
            ulong maxsize = 0;

            if (args.Length < 2 || args.Length > 3 || !GetAddress(args[0], out Address addr))
                return false;

            if (args.Length == 3 && !GetUlong(args[2], out maxsize))
                return false;

            if (GetUlong(args[1], out ulong dw))
            {
                finddata = Helper.rul2hexstr(Helper.ToHostEndianness(dw, arch.Endianness));
                // Remove trailing zeroes, keep even character args.Length
                int end;
                for (end = finddata.Length - 1; end != 0; end--)
                {
                    if (finddata[end] != '0')
                        break;
                }
                end++;
                finddata = finddata.PadRight(end + (end % 2), '0');
            }
            else if (GetString(args[1], out finddata))
            {
                var v = Var.Create(finddata);
                finddata = v.ToHexString();
                if (!v.IsBuf)
                    finddata = finddata.Replace("3f", "??"); // 0x3F = '?' . wildcard like "mov ?ax, ?bx"
            }
            else
                return false;

            if (!Helper.IsHexWild(finddata))
                return false;

            variables["$RESULT"] = Var.Create(0);

            // search in current mem block
            //$REVIEW: extremely inefficient O(n*m) algorithm, do we care?
            if (Host.TE_GetMemoryInfo(addr, out MEMORY_BASIC_INFORMATION? MemInfo))
            {
                int memlen = (int) ((MemInfo.BaseAddress! - addr) + (long) MemInfo.RegionSize);
                if (maxsize != 0 && (int)maxsize < memlen)
                    memlen = (int)maxsize;

                var ea = addr;
                if (!Host.SegmentMap.TryFindSegment(ea, out ImageSegment? segment))
                    throw new AccessViolationException();
                byte[] membuf = new byte[memlen];
                if (segment.MemoryArea is ByteMemoryArea bmem && bmem.TryReadBytes(ea, memlen, membuf))
                {
                    int bytecount = finddata.Length / 2;

                    byte[] mask = new byte[bytecount];
                    byte[] bytes= new byte[bytecount];

                    Helper.hexstr2bytemask(finddata, mask, bytecount);
                    Helper.hexstr2bytes(finddata, bytes, bytecount);

                    for (int i = 0; (i + bytecount) <= memlen; i++)
                    {
                        if (Helper.MaskedCompare(membuf, i, bytes, mask, bytecount))
                        {
                            variables["$RESULT"] = Var.Create(addr + i);
                            break;
                        }
                    }
                }
            }
            return true;
        }

        // TE?
        private bool DoFINDCALLS(Expression[] args)
        {
            //bool bRefVisible = false, bResetDisam = false;
            //, @base, size, disamsel = 0;

            if (args.Length >= 1 && args.Length <= 2 && GetUlong(args[0], out ulong addr))
            {
                errorstr = "Unsupported command!";
                return true;
            }
            return false;

            /*
            Getdisassemblerrange(&base, &size);

            //Get initial Ref Window State
            t_table* tt;
            tt=(t_table*) Plugingetvalue(VAL_REFERENCES);
            if (tt is not null)
                bRefVisible=(tt.hw!=0);

            t_dump* td;
            td=(t_dump*) Plugingetvalue(VAL_CPUDASM);
            if (td is null)
                return false;

            if(GetUlong(ops[0], addr))
            {
                if(addr < base || addr >= (base+size))
                {
                    //outside debugger window range
                    disamsel = td.sel0;
                    Setdisasm(addr, 0, 0);
                    bResetDisam = true;
                }

                variables["$RESULT"] = Var.Create(0);
                if(Findalldllcalls(td, addr, null) > 0)
                {
			
                    if (tt is null)
                        tt = (t_table*) Plugingetvalue(VAL_REFERENCES);

                    if (tt is not null) 
                    {
                        t_ref* tr;

                        if (tt.data.n > 1) 
                        {
                            //Filter results
                            string filter;
                            if(GetString(ops[1], filter) && filter != "")
                            {
                                //filter=ToLower(filter);
                                (char*) buffer[TEXTLEN+1];
                                for(int nref = tt.data.n-1; nref > 0; nref--)
                                {
                                    tr=(t_ref*) Getsortedbyselection(&tt.data, nref);
                                    if(tr is not null && tr.dest != 0)
                                    {
                                        //ZeroMemory(buffer,TEXTLEN+1);
                                        //Decodename(tr.dest,NM_LABEL,buffer);
                                        Findlabel(tr.dest, buffer);
                                        if(_stricmp(buffer, filter))
                                            Deletesorteddata(&tt.data, tr.addr);
                                    }
                                }
                            }

                            tr=(t_ref*) Getsortedbyselection(&tt.data, 1); //0 is CPU initial
                            if (tr is not null)
                                variables["$RESULT"] = tr.addr;
                        }

                        if(tt.hw && !bRefVisible)
                        {
                            DestroyWindow(tt.hw);
                            tt.hw = 0;
                        }
                    }
                }
                if(bResetDisam)
                    Setdisasm(disamsel, 0, 0);
                return true;
            }
            return false;
            */
        }

        /// <summary>
        /// Assembles the command and then searches for it.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool DoFINDCMD(Expression[] args)
        {
            //bool bRefVisible = false, bResetDisam = false;
            //string cmd, cmds;
            //int len;
            //int pos;
            //ulong addr, @base,size, attempt, opsize = 3, disamsel = 0;
            //int startadr = 0, endadr = 0, lps = 0, length, ncmd = 0, cmdpos = 0;
            //char error[256] = {0};

            if (args.Length == 2)
            {
                errorstr = "Unsupported command!";
                return false;
            }
            return false;

            /*
            Getdisassemblerrange(&base,&size);

            //Get initial Ref Window State
            t_table* tt;
            tt=(t_table*) Plugingetvalue(VAL_REFERENCES);
            if (tt is not null)
                bRefVisible=(tt.hw!=0);

            t_dump* td;
            td=(t_dump*) Plugingetvalue(VAL_CPUDASM);
            if (td is null)
                return false;

            ulong tmpaddr = TE_AllocMemory(0x100);

            if (GetUlong(ops[0], addr) 
                && GetString(ops[1], cmds))
            {
                if (addr<base || addr>=(base+size)) {
                    //outside debugger window range
                    disamsel=td.sel0;
                    Setdisasm(addr,0,0);
                    bResetDisam=true;
                }

                t_asmmodel model={0};
                t_extmodel models[NSEQ][NMODELS]={0};

                length = cmds.Length;
                while (cmdpos<length && ncmd<NSEQ)
                {

                    endadr= cmds.find(";",startadr);
                    if (endadr==-1)
                    {
                        endadr=length;
                    }
                    lps=endadr-startadr;

                    cmd=cmds.substr(startadr,lps);
			
                    attempt=0;
                    strcpy(buffer, cmd);

                    do {

                        if((len = Assemble(buffer, tmpaddr, &model, attempt, opsize, error)) <= 0)
                        {
                            if (attempt!=0) {
                                break;
                            }

                            pos=(cmd.Length+len);
                            if (pos>=0 && pos<cmd.Length)
                                errorstr = "\nFINDCMD error at \""+cmd.substr(pos,cmd.Length-pos)+"\"!\n\n";
                            else
                                errorstr = "\nFINDCMD error !\n\n";
                            errorstr.append(error);
                            goto return_false;
                        }
                        memcpy(&models[ncmd][attempt],&model,sizeof(model));
                        attempt++;

                    } while (len>0 && attempt<NMODELS);

                    startadr=endadr+1;
                    cmdpos+=lps+1;

                    ncmd++;
                }

                variables["$RESULT"]=0;
                if (Findallsequences(td,models,addr,null)>0) {
			
                    if (tt is null)
                        tt = (t_table*) Plugingetvalue(VAL_REFERENCES);

                    if (tt is not null) 
                    {
                        t_ref* tr;
                        if (tt.data.n > 1)
                        {
                            tr=(t_ref*) Getsortedbyselection(&tt.data, 1); //0 is CPU initial
                            if (tr is not null)
                                variables["$RESULT"]=tr.addr;
                        }

                        if (tt.hw && !bRefVisible) {
                            DestroyWindow(tt.hw);
                            tt.hw=0;
                        }
                    }
                }
                TE_FreeMemory(tmpaddr);
                if(bResetDisam)
                    Setdisasm(disamsel,0,0);
                return true;

            }
        return_false:
            if(bResetDisam)
                Setdisasm(disamsel,0,0);
            TE_FreeMemory(tmpaddr);
            return false;
            */
            //return true;
        }
        /*
        //Buggy, could assemble different command code bytes, (from chinese code)
        private bool DoFINDCMDS(string [] args)
        {

            string ops[2];
            t_asmmodel model;
            ulong addr;
            string cmds,args1,cmd;
            char opcode[256]={0},buff[32]={0},tmp[64]={0},error[64]={0};
            int i,pos,len=0,length=0,startadr=0,endadr=0,lps=0,codelen=0;
            int attempt=0,opsize=3;

            if(!CreateOp(args, ops, 2))
                return false;

            if (GetUlong(ops[0], addr) 
                && GetString(ops[1], cmds))
            {

              if (cmds.find(";")==-1)
              {
                nIgnoreNextValuesHist=1;
                return DoFINDoneCMD(args);
              }

              length = cmds.Length;
 
              ulong tmpaddr = TE_AllocMemory(0x100);

              while (len<length)
              {
                endadr= cmds.find(";",startadr);
                if (endadr==-1)
                {
                    endadr=length;
                }
                lps=endadr-startadr;
                cmd=cmds.substr(startadr,lps);
       
                strcpy(buffer, cmd);
                if((codelen = Assemble(buffer, tmpaddr, &model, attempt, opsize, error)) <= 0)
                {
                    pos=(cmd.Length+codelen);
                    if (pos>=0 && pos<cmd.Length)
                        errorstr = "\nFINDCMDS error on \""+cmd.substr(pos,cmd.Length-pos)+"\"!\n\n";
                    else
                        errorstr = "\nFINDCMDS error !\n\n";
                    errorstr.append(error);
                    TE_FreeMemory(tmpaddr);
                    return false;
                }
                else
                {
                    strcpy(buff, model.code);
                }

                i=0;
                while(i<codelen)
                {
                    _itoa(buff[i],tmp,16);
                    i++;
                    strcat(opcode,tmp);
                }

                startadr=endadr+1;
                len=len+lps+1;
              }
              TE_FreeMemory(tmpaddr);

              return DoFIND(ops[0] + ", " + '#' + opcode + '#');
            }
            return false;
        }
        */

        // TE?
        private bool DoFINDOP(Expression[] args)
        {
            string? finddata;
            ulong maxsize = 0;

            if (args.Length >= 2 && args.Length <= 3 && GetAddress(args[0], out Address addr))
            {
                if (args.Length == 3 && !GetUlong(args[2], out maxsize))
                    return false;

                if (GetUlong(args[1], out ulong dw))
                {
                    finddata = Helper.rul2hexstr(Helper.ToHostEndianness(dw, arch.Endianness));
                    // Remove trailing zeroes, keep even character args.Length
                    int end;
                    for (end = finddata.Length - 1; end != 0; end--)
                    {
                        if (finddata[end] != '0')
                            break;
                    }
                    end++;
                    finddata = finddata.Remove(end + (end % 2), '0');
                }
                else if (GetString(args[1], out finddata))
                {
                    Var v = Var.Create(finddata);
                    finddata = v.ToHexString();
                    if (!v.IsBuf)
                        Helper.ReplaceString(ref finddata, "3f", "??"); // 0x3F = '?' . wildcard like "mov ?ax, ?bx"
                }
                else
                    return false;

                if (Helper.IsHexWild(finddata))
                {
                    variables["$RESULT"] = Var.Create(0);

                    // search in current mem block
                    if (Host.TE_GetMemoryInfo(addr, out MEMORY_BASIC_INFORMATION? MemInfo))
                    {
                        ulong memlen = (ulong) (MemInfo.BaseAddress! - addr) + MemInfo.RegionSize;
                        if (maxsize != 0 && maxsize < memlen)
                            memlen = maxsize;

                        byte[] membuf, mask, bytes;

                        membuf = new byte[memlen];
                        if (Host.TryReadBytes(addr, (int)memlen, membuf))
                        {
                            int bytecount = finddata.Length / 2;

                            mask = new byte[bytecount];
                            bytes = new byte[bytecount + MAX_INSTR_SIZE];

                            Helper.hexstr2bytemask(finddata, mask, bytecount);
                            Helper.hexstr2bytes(finddata, bytes, bytecount);

                            for (int i = 0; (i + bytecount) <= (uint) memlen;)
                            {
                                int len = Host.LengthDisassemble(membuf, i);
                                if (len == 0)
                                    break;

                                if (len >= bytecount && Helper.MaskedCompare(membuf, i, bytes, mask, bytecount))
                                {
                                    variables["$RESULT"] = Var.Create(addr + (uint) i);
                                    variables["$RESULT_1"] = Var.Create(len);
                                    break;
                                }
                                i += len;
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private bool DoFINDMEM(Expression[] args)
        {
            if (args.Length >= 1 && args.Length <= 2)
            {
                Address addr = default;
                if (args.Length == 2 && !GetAddress(args[1], out addr))
                    return false;

                variables["$RESULT"] = Var.Create(0);
                while (Host.TE_GetMemoryInfo(addr!, out MEMORY_BASIC_INFORMATION? MemInfo) && variables["$RESULT"].ToUInt64() == 0)
                {
                    if (!DoFIND(addr, args[0]))
                        return false;
                    addr = MemInfo.BaseAddress!.Value + MemInfo.RegionSize;
                }
                return true;
            }
            return false;
        }

        private bool DoFREE(Expression[] args)
        {
            ulong size = 0;

            if (args.Length >= 1 && args.Length <= 2 && GetAddress(args[0], out Address addr))
            {
                if (args.Length == 2 && !GetUlong(args[1], out size))
                    return false;

                variables["$RESULT"] = Var.Create(0);

                if ((size == 0 && Host.FreeMemory(addr)) || (size != 0 && Host.FreeMemory(addr, size)))
                {
                    variables["$RESULT"] = Var.Create(1);
                    UnregMemBlock(addr);
                }
                return true;
            }
            return false;
        }

        private bool DoGAPI(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong addr))
            {
                errorstr = "Unsupported command!";
                return false;

                //variables["$RESULT_4"] = Debugger.GetJumpDestination(Host.TE_GetProcessHandle(), Debugger.GetContextData(UE_CIP)); 

                /*
                rulogn size, test, addr2
                BYTE buffer[MAXCMDSIZE];
                //size=Readmemory(buffer, addr, MAXCMDSIZE, MM_SILENT);
                size = Readcommand(addr, (char *)buffer);
		
                if(size > 0)
                {
                    t_disasm disasm;
                    size = Disasm(buffer, size, addr, null, &disasm, DISASM_CODE, null);
                    test = disasm.jmpaddr;

                    if(size > 0)
                    {
        //				variables["$RESULT"] = disasm.result; //asm command text
        //				variables["$RESULT_1"] = disasm.dump;     //command bytes
                        variables["$RESULT_3"] = disasm.addrdata;
                        variables["$RESULT_4"] = disasm.jmpaddr; 

                    }
                }
                if(test)
                {
                    t_disasm disasm;
                    size = Disasm(buffer, size, addr, null, &disasm, DISASM_CODE, null);
                    char sym[4096] = {0};
                    char buf[TEXTLEN] = {0};
                    addr2 = disasm.addrdata;
                    int res = Decodeaddress(addr2, 0, ADC_JUMP | ADC_STRING | ADC_ENTRY | ADC_OFFSET | ADC_SYMBOL, sym, 4096, buf);
                    if(res)
                    {
                        variables["$RESULT"] = sym;
                        char *tmp = strstr(sym, ".");
                        if(tmp)
                        {
                            strtok(sym, ">");                          //buxfix
                            *tmp = '\0';
                            variables["$RESULT_1"] = sym + 2;          //bugfix
                            variables["$RESULT_2"] = tmp + 1;
                        }
                    }
                    return true;
                }
                variables["$RESULT"] = Var.Create(0);
                return true;
                */
            }
            return false;
        }

        private bool DoGBPM(Expression[] args)
        {
            if (args.Length == 0)
            {
                variables["$RESULT"] = Var.Create(break_memaddr);
                return true;
            }
            return false;
        }

        private bool DoGBPR(Expression[] args)
        {
            if (args.Length == 0)
            {
                variables["$RESULT"] = Var.Create(break_reason);
                return true;
            }
            return false;
        }

        private bool DoGCI(Expression[] args)
        {
            if (args.Length == 2 && GetAddress(args[0], out Address addr) && args[1] is Identifier cmd)
            {
                var param = cmd.Name.ToUpperInvariant();
                if (param == "COMMAND")
                {
                    var instr = Host.Disassemble(addr);
                    if (instr is not null)
                        variables["$RESULT"] = Var.Create(instr.ToString());
                    else
                        variables["$RESULT"] = Var.Create(0);
                    return true;
                }
                else if (param == "CONDITION") // Olly only
                {
                    variables["$RESULT"] = Var.Create(0);
                    return true;
                }
                else if (param == "DESTINATION")
                {
                    /*
                    if(is_RETNX)
                        variables["$RESULT"] = [ESP+X];
                    */
                    variables["$RESULT"] = Debugger.GetJumpDestination(Host.TE_GetProcessHandle(), addr);
                    return true;
                }
                else if (param == "SIZE")
                {
                    variables["$RESULT"] = Var.Create(Host.LengthDisassembleEx(addr));
                    return true;
                }
                else if (param == "TYPE") // Olly only
                {
                    variables["$RESULT"] = Var.Create(0);
                    return true;
                }
            }
            return false;
        }

        // Olly only
        private bool DoGCMT(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong addr))
            {
                variables["$RESULT"] = Var.Create("");
                return true;
            }
            return false;
        }

        private bool DoGFO(Expression[] args)
        {
            throw new NotImplementedException();
#if LATER
ulong addr;
	if(args.Length == 1 && GetUlong(args[0],  out addr))
	{
		variables["$RESULT"] = Var.Create(0);

		List<MODULEENTRY32> Modules;
		if(Host.TE_GetModules(Modules))
		{
			for(int i = 0; i < Modules.Count; i++)
			{
				if(addr >= (ulong)Modules[i].modBaseAddr && addr < ((ulong)Modules[i].modBaseAddr + Modules[i].modBaseSize))
				{
					const Librarian::LIBRARY_ITEM_DATA* lib = Librarian.GetLibraryInfoEx(Modules[i].modBaseAddr);
					if(lib is not null && lib.hFileMappingView is not null)
					{
						ULONG_PTR filebase = Dumper.GetPE32DataFromMappedFile((ULONG_PTR)lib.hFileMappingView, null, UE_IMAGEBASE);
						variables["$RESULT"] = Dumper.ConvertVAtoFileOffset((ULONG_PTR)lib.hFileMappingView, addr-(ULONG_PTR)Modules[i].modBaseAddr+filebase, false);
					}
					break;
				}
			}
		}
		return true;
	}
	return false;
#endif
        }

        // Olly only
        private bool DoGLBL(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong addr))
            {
                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        private bool DoGMA(Expression[] args)
        {
            if (args.Length == 2 && GetString(args[0], out string? mod))
            {
                if (mod.Length > 8)
                    mod = mod.Remove(8);
                mod = mod.ToLowerInvariant();

                var modules = new List<MODULEENTRY32>();
                if (Host.TE_GetModules(modules))
                {
                    for (int i = 0; i < modules.Count; i++)
                    {
                        string cur = modules[i].szModule!;
                        if (cur.Length > 8)
                            cur = cur.Remove(8);
                        if (cur.ToLowerInvariant() == mod)
                        {
                            return DoGMI(modules[i].modBaseAddr!, args[1]);
                        }
                    }
                }
                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        private bool DoGMEMI(Expression[] args)
        {
            if (args.Length == 2 && GetAddress(args[0], out Address addr) && args[1] is Identifier cmd)
            {
                variables["$RESULT"] = Var.Create(0);

                if (Host.TE_GetMemoryInfo(addr, out MEMORY_BASIC_INFORMATION? MemInfo))
                {
                    var val = cmd.Name.ToUpperInvariant();

                    if (val == "MEMORYBASE") variables["$RESULT"] = Var.Create(MemInfo.BaseAddress!.Value);
                    else if (val == "MEMORYSIZE") variables["$RESULT"] = Var.Create(MemInfo.RegionSize);
                    else if (val == "MEMORYOWNER") variables["$RESULT"] = Var.Create((ulong) MemInfo.AllocationBase);
                    else
                    {
                        errorstr = "Second operand bad";
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoNAMES(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong addr))
            {
                return true;
            }
            return false;
        }

        private bool DoGMEXP(Expression[] args)
        {
            ulong num = 0;
            if (args.Length >= 2 && args.Length <= 3 && GetUlong(args[0], out ulong addr))
            {
                if (args.Length == 3 && !GetUlong(args[2], out num))
                    return false;

                errorstr = "Unsupported command!";
                return false;

                /*
                variables["$RESULT"] = Var.Create(0);

                str = args[1].ToUpperInvariant();

                ulong args.Length = 0;
                bool cache = false, cached = false;

                t_module mod = Findmodule(addr);
                if(!mod)
                {
                    return true;
                }

                t_export exp = {0};

                if(str == "COUNT")
                {
                    cache = true;
                    tExportsCache.clear();
                    exportsCacheAddr = addr;
                }
                else
                {
                    if(exportsCacheAddr == addr && num < tExportsCache.Count)
                    {
                        exp = tExportsCache[num];
                        args.Length = tExportsCache.Count;
                        cached = true;
                    }
                }

                if(!cached)
                {
                    for(ulong i = 0; i < mod.codesize ; i++)
                    {
                        if(Findname(mod.codebase + i, NM_EXPORT, exp.label))
                        {
                            args.Length++;
                            exp.addr = mod.codebase + i;
                            if(args.Length == num && !cache) break;
                            if(cache)
                                tExportsCache.Add(exp);
                        }
                    }
                }

                if(num > args.Length) //no more
                {
                    return true;
                }

                     if(str == "COUNT")   variables["$RESULT"] = args.Length;
                else if(str == "ADDRESS") variables["$RESULT"] = exp.addr;
                else if(str == "LABEL")   variables["$RESULT"] = exp.label;
                else
                {
                    errorstr = "Second operand bad";
                    return false;
                }
                return true;
                */
            }
            return false;
        }

        private bool DoGMI(params Expression[] args)
        {
            throw new NotImplementedException();
#if LATER
ulong addr;
string str;

	if(args.Length == 2 && GetUlong(args[0],  out addr))
	{
		List<MODULEENTRY32> Modules;
		MODULEENTRY32 Module;
		ulong ModBase = 0;

		variables["$RESULT"] = Var.Create(0);

		if(Host.TE_GetModules(Modules))
		{
			for(int i = 0; i < Modules.Count; i++)
			{
				Module = Modules[i];
				if(addr >= (ulong)Module.modBaseAddr && addr < ((ulong)Module.modBaseAddr + Module.modBaseSize))
				{
					ModBase = (ulong)Module.modBaseAddr;
					break;
				}
			}
		}

		Dumper.PEStruct PEInfo;

		if(!ModBase || !GetPE32DataEx(Module.szExePath, &PEInfo))
		{
			return true;
		}

		str = Helper.toupper(args[1]);

		if(str == "MODULEBASE")
		{ 
			variables["$RESULT"] = ModBase;
		}
		else if(str == "MODULESIZE")
		{
			variables["$RESULT"] = round_up(Module.modBaseSize, PAGE_SIZE);
		}
		else if(str == "CODEBASE") // workaround: section of EP
		{
			HANDLE hFile, hMap;
			DWORD Size;
			ULONG_PTR VA;
			if(Static.FileLoad(Module.szExePath, Ue.UE_ACCESS_READ, false, &hFile, &Size, &hMap, &VA))
			{
				DWORD Section = Dumper.GetPE32SectionNumberFromVA(VA, PEInfo.ImageBase + PEInfo.OriginalEntryPoint);
				if(Section != UE_VANOTFOUND)
				{
					variables["$RESULT"] = ModBase + Dumper.GetPE32DataFromMappedFile(VA, Section, UE_SECTIONVIRTUALOFFSET);
				}
				Static.FileUnload(Module.szExePath, false, hFile, Size, hMap, VA);
			}
		}
		else if(str == "CODESIZE")
		{
			HANDLE hFile, hMap;
			DWORD Size;
			ULONG_PTR VA;
			if(Static.FileLoad(Module.szExePath, UE_ACCESS_READ, false, &hFile, &Size, &hMap, &VA))
			{
				DWORD Section = Dumpler.GetPE32SectionNumberFromVA(VA, PEInfo.ImageBase + PEInfo.OriginalEntryPoint);
				if(Section != UE_VANOTFOUND)
				{
					variables["$RESULT"] = round_up(Dumpler.GetPE32DataFromMappedFile(VA, Section, UE_SECTIONVIRTUALSIZE), PAGE_SIZE);
				}
				Static.FileUnload(Module.szExePath, false, hFile, Size, hMap, VA);
			}
		}
		else if(str == "ENTRY")
		{
			variables["$RESULT"] = ModBase + PEInfo.OriginalEntryPoint;
		}
		else if(str == "NSECT")
		{
			variables["$RESULT"] = PEInfo.SectionNumber;
		}
		else if(str == "DATABASE")
		{
			//variables["$RESULT"] = Var.Create(0);
		}
		else if(str == "EDATATABLE")
		{
			if(PEInfo.ExportTableSize)
				variables["$RESULT"] = ModBase + PEInfo.ExportTableAddress;
		}
		else if(str == "EDATASIZE")
		{
			variables["$RESULT"] = PEInfo.ExportTableSize;
		}
		else if(str == "IDATABASE")
		{
			if(PEInfo.ImportTableSize)
			{
				HANDLE hFile, hMap;
				DWORD Size;
				ULONG_PTR VA;
				if(Static.FileLoad(Module.szExePath, UE_ACCESS_READ, false, &hFile, &Size, &hMap, &VA))
				{
					DWORD Section = Dumpler.GetPE32SectionNumberFromVA(VA, PEInfo.ImageBase + PEInfo.ImportTableAddress);
					if(Section != UE_VANOTFOUND)
					{
						variables["$RESULT"] = ModBase + Dumpler.GetPE32DataFromMappedFile(VA, Section, UE_SECTIONVIRTUALOFFSET);
					}
					Static.FileUnload(Module.szExePath, false, hFile, Size, hMap, VA);
				}
			}
		}
		else if(str == "IDATATABLE")
		{
			if(PEInfo.ImportTableSize)
				variables["$RESULT"] = ModBase + PEInfo.ImportTableAddress;
		}
		else if(str == "IDATASIZE")
		{
			variables["$RESULT"] = PEInfo.ImportTableSize;
		}
		else if(str == "RESBASE")
		{
			if(PEInfo.ResourceTableSize)
			{
				HANDLE hFile, hMap;
				DWORD Size;
				ULONG_PTR VA;
				if(Static.FileLoad(Module.szExePath, UE_ACCESS_READ, false, &hFile, &Size, &hMap, &VA))
				{
					DWORD Section = Dumpler.GetPE32SectionNumberFromVA(VA, PEInfo.ImageBase + PEInfo.ResourceTableAddress);
					if(Section != UE_VANOTFOUND)
					{
						variables["$RESULT"] = ModBase + Dumpler.GetPE32DataFromMappedFile(VA, Section, UE_SECTIONVIRTUALOFFSET);
					}
					Static.FileUnload(Module.szExePath, false, hFile, Size, hMap, VA);
				}
			}
			//if(PEInfo.ResourceTableSize)
			//	variables["$RESULT"] = ModBase + PEInfo.ResourceTableAddress;
		}
		else if(str == "RESSIZE")
		{
			variables["$RESULT"] = PEInfo.ResourceTableSize;
		}
		else if(str == "RELOCTABLE")
		{
			if(PEInfo.RelocationTableSize)
				variables["$RESULT"] = ModBase + PEInfo.RelocationTableAddress;
		}
		else if(str == "RELOCSIZE")
		{
			variables["$RESULT"] = PEInfo.RelocationTableSize;
		}
		else if(str == "NAME") // max 8 chars, no extension
		{
			string name = Module.szModule;

			int offset;
			if((offset = name.LastIndexOf('.')) >= 0)
				name.resize(offset);
			if(name.Length > 8)
				name = name.remove(8);
			variables["$RESULT"] = name;
		}
		else if(str == "PATH")
		{
			variables["$RESULT"] = Module.szExePath;
		}
		else if(str == "VERSION")
		{
			string version;
			if(GetAppVersionString(Module.szExePath, "FileVersion", version))
				variables["$RESULT"] = version;
			else
				variables["$RESULT"] = "";
		}
		else
		{
			errorstr = "Second operand bad";
			return false;
		}
		return true;
	}
	errorstr = "Bad operand";
	return false;
#endif
        }

        private bool DoGMIMP(Expression[] args)
        {
            ulong num = 0;
            string str;

            if (args.Length >= 2 && args.Length <= 3 && GetUlong(args[0], out ulong addr) && args[1] is Identifier cmd)
            {
                if (args.Length == 3 && !GetUlong(args[2], out num))
                    return false;

                errorstr = "Unsupported command!";

                variables["$RESULT"] = Var.Create(0);

                str = cmd.Name.ToUpperInvariant();
                return false;

                /*
                ulong i, args.Length=0;
                string str;
                bool cache = false, cached=false;


                t_module * mod = Findmodule(addr);
                if(!mod)
                {
                    return true;
                }

                t_export exp={0};

                if(str == "COUNT")
                {
                    cache = true;
                    tImportsCache.clear();
                    importsCacheAddr = addr;
                }
                else
                {
                    if(importsCacheAddr == addr && num < tImportsCache.Count)
                    {
                        exp = tImportsCache[num];
                        args.Length = tImportsCache.Count;
                        cached = true;
                    }
                }

                if(!cached)
                {
                    for(i = 0; i < mod.codesize ; i++)
                    {
                        if (Findname(mod.codebase + i, NM_IMPORT, exp.label))
                        {
                            args.Length++;
                            exp.addr=mod.codebase + i;
                            if(args.Length == num && !cache) break;
                            if(cache)
                                tImportsCache.Add(exp);
                        }
                    }
                }

                if(num > args.Length) //no more
                {
                    return true;
                }

                if(str == "COUNT")
                {
                    variables["$RESULT"] = args.Length;
                }
                else if(str == "ADDRESS")
                {
                    variables["$RESULT"] = exp.addr;
                }
                else if(str == "LABEL")
                {
                    variables["$RESULT"] = exp.label;
                }
                else if(str == "NAME")
                {
                    string s = exp.label;
                    if(s.find('.') >= 0)
                        variables["$RESULT"] = s.substr(s.find('.')+1);
                    else 
                        variables["$RESULT"] = exp.label;
                }
                else if(str == "MODULE")
                {
                    string s = exp.label;
                    if(s.find('.') >= 0)
                    {
                        variables["$RESULT"] = s.substr(0, s.find('.'));
                    }
                    else 
                        variables["$RESULT"] = "";
                }
                else
                {
                    errorstr = "Second operand bad";
                    return false;
                }
                return true;
                */
            }
            return false;
        }

        private bool DoGN(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong addr))
            {
                string pAPI = Importer.GetAPIName(addr);
                string pDLL = Importer.GetDLLName(addr);
                if (pAPI is not null && pDLL is not null && pAPI[0] != 0 && pDLL[0] != 0)
                {
                    string API = pAPI, DLL = pDLL;

                    int offset;
                    if ((offset = DLL.LastIndexOf('.')) >= 0) // remove extension
                        DLL = DLL.Remove(offset);

                    variables["$RESULT"] = Var.Create(API);
                    variables["$RESULT_1"] = Var.Create(DLL);
                    variables["$RESULT_2"] = Var.Create(API);
                }
                else
                {
                    variables["$RESULT"] = variables["$RESULT_1"] = variables["$RESULT_2"] = Var.Create(0);
                }
                return true;
            }
            return false;
        }

        private bool DoGO(Expression[] args)
        {
            if (args.Length == 1 && GetAddress(args[0], out Address addr))
            {
                Debugger.SetBPX(addr, Ue.UE_SINGLESHOOT, SoftwareCallback);
                bInternalBP = true;
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoGOPI(Expression[] args)
        {
            string param;

            if (args.Length == 3 &&
                GetAddress(args[0], out Address _) &&
                GetUlong(args[1], out ulong index) &&
                args[2] is Identifier cmd)
            {
                if (index < 1 || index > 3)
                {
                    errorstr = "Bad operand index (1-3)";
                    return false;
                }

                index--;

                param = cmd.Name.ToUpperInvariant();

                errorstr = "Unsupported command!";
                return false;

                /*
		
                ulong size;

                BYTE buffer[MAXCMDSIZE]={0};
        //		size=Readmemory(buffer, addr, MAXCMDSIZE, MM_SILENT);
                size=Readcommand(addr,(char *) buffer);

                if (size>0) 
                {
                    t_disasm disasm;
                    size = Disasm(buffer, size, addr, null, &disasm, DISASM_ALL, Host.TE_GetCurrentThreadId());

                    if(size <= 0)
                        return false;
                    else if (param == "TYPE")
                    {
                        variables["$RESULT"] = disasm.optype[index]; // Type of operand (extended set DEC_xxx)
                        return true;
                    }
                    else if (param == "SIZE") 
                    {
                        variables["$RESULT"] = disasm.opsize[index]; // Size of operand, bytes
                        return true;
                    }
                    else if (param == "GOOD") 
                    {
                        variables["$RESULT"] = disasm.opgood[index]; // Whether address and data valid
                        return true;
                    }
                    else if (param == "ADDR") 
                    {
                        variables["$RESULT"] = disasm.opaddr[index]; // Address if memory, index if register
                        return true;
                    }
                    else if (param == "DATA") 
                    {
                        variables["$RESULT"] = disasm.opdata[index]; // Actual value (only integer operands)
                        return true;
                    }
                }
                */
            }
            return false;
        }

        private bool DoGPA(Expression[] args)
        {
            if (args.Length >= 2 && args.Length <= 3 && GetString(args[0], out string? proc) && GetString(args[1], out string? lib))
            {
                if (args.Length == 3 && !GetBool(args[2], out bool dofree))
                    return false;

                ulong addr = Importer.GetRemoteAPIAddressEx(lib, proc);
                if (addr != 0)
                {
                    variables["$RESULT"] = Var.Create(addr);
                    variables["$RESULT_1"] = Var.Create(lib);
                    variables["$RESULT_2"] = Var.Create(proc);
                }
                else
                {
                    variables["$RESULT"] = variables["$RESULT_1"] = variables["$RESULT_2"] = Var.Create(0);
                }
                return true;
            }
            return false;
        }

        private bool DoGPI(Expression[] args)
        {
            if (args.Length != 1)
                return false;
            if (!(args[0] is Identifier cmd))
                return false;
            var str = cmd.Name.ToUpperInvariant();

            if (str == "HPROCESS") // Handle of debugged process 
            {
                variables["$RESULT"] = Var.Create((ulong)Host.TE_GetProcessHandle());
            }
            else if (str == "PROCESSID") // Process ID of debugged process 
            {
                variables["$RESULT"] = Var.Create(Host.TE_GetProcessId());
            }
            else if (str == "HMAINTHREAD") // Handle of main thread of debugged process 
            {
                variables["$RESULT"] = Var.Create((ulong)Host.TE_GetMainThreadHandle());
            }
            else if (str == "MAINTHREADID") // Thread ID of main thread of debugged process 
            {
                variables["$RESULT"] = Var.Create(Host.TE_GetMainThreadId());
            }
            else if (str == "MAINBASE") // Base of main module in the debugged process (NOT DLL . loader base)
            {
                variables["$RESULT"] = Var.Create(Debugger.GetDebuggedFileBaseAddress());
            }
            else if (str == "PROCESSNAME") // File name of the debugged process/dll (no extension)
            {
                string name = Path.GetFileNameWithoutExtension(Host.TE_GetTargetPath())!;
                variables["$RESULT"] = Var.Create(name);
            }
            else if (str == "EXEFILENAME") // Full path of the debugged file/dll
            {
                variables["$RESULT"] = Var.Create(Host.TE_GetTargetPath() ?? "");
            }
            else if (str == "CURRENTDIR") // Current directory for debugged process (with trailing '\')
            {
                variables["$RESULT"] = Var.Create(Host.TE_GetTargetDirectory() ?? "");
            }
            else if (str == "SYSTEMDIR") // Windows system directory (with trailing '\')
            {
                string SysDir = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
                variables["$RESULT"] = Var.Create(Helper.pathfixup(SysDir, true));
            }
            else
            {
                errorstr = "Bad operand";
                return false;
            }
            return true;
        }

        //in dev... i try to find API parameters number and types
        private bool DoGPP(Expression[] args)
        {
            /*
            if (!DoGPA(args))
                return false;

            ulong addr = variables["$RESULT"].dw;
            if (addr==0)
                return false;

            string sAddr = _itoa(addr,buffer,16);
            if (!DoREF(sAddr))
                return false;

            int t=Plugingetvalue(VAL_REFERENCES);
            if (t<=0)
                return false;

            int size;
            t_table* tref=(t_table*) t;
            for (int n=0;n<tref.data.n;n++) {
	
                t_ref* ref= (t_ref*) Getsortedbyselection(&tref.data,n);
			
                if (ref.addr == addr)
                    continue; 

                    //Disasm origin to get comments
                BYTE buffer[MAXCMDSIZE];
                size=Readmemory(buffer, ref.addr, MAXCMDSIZE, MM_SILENT);					
                if (size>0) {

                    t_disasm disasm;
                    t_module* mod = Findmodule(ref.addr);
                    Analysecode(mod);

                    size=Disasm(buffer,size,ref.addr,null,&disasm,DISASM_ALL,null);
                    DbgMsg(disasm.nregstack,disasm.comment);

                    if (size>0) {
                        variables["$RESULT"] = ref.addr;
                        variables["$RESULT_1"] = disasm.result; //command text
                        variables["$RESULT_2"] = disasm.comment;
                        return true; 
                    }
                }
            }
            */
            return true;
        }

        // Olly only
        private bool DoGREF(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 1 && !GetUlong(args[0], out ulong line))
                    return false;

                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoGRO(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong addr))
            {
                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoGSL(Expression[] args)
        {
            throw new NotImplementedException();
#if LATER
string str = "";

	if(args.Length >= 0 && args.Length <= 1)
	{
		if(args.Length == 1)
			str = args[0];

		string[] valid_commands = {"", "CPUDASM", "CPUDUMP", "CPUSTACK"};

		variables["$RESULT"] = Var.Create(0);
		variables["$RESULT_1"] = Var.Create(0);
		variables["$RESULT_2"] = Var.Create(0);

		return (valid_commands+_countof(valid_commands) != find(valid_commands, valid_commands+_countof(valid_commands), Helper.toupper(str)));
	}
	return false;
#endif
        }

        private bool DoGSTR(Expression[] args)
        {
            ulong size = 2;
            if (args.Length >= 1 && args.Length <= 2 && GetAddress(args[0], out Address addr))
            {
                if (args.Length == 2 && !GetUlong(args[1], out size))
                    return false;

                variables["$RESULT"] = variables["$RESULT_1"] = Var.Create(0);

                if (Host.TE_GetMemoryInfo(addr, out MEMORY_BASIC_INFORMATION? MemInfo))
                {
                    ulong memsize = (ulong) (MemInfo.BaseAddress! - addr) + MemInfo.RegionSize;

                    byte[] buffer;

                    buffer = new byte[memsize + 1];

                    buffer[0] = buffer[memsize] = 0;

                    if (Host.TryReadBytes(addr, (int)memsize, buffer))
                    {
                        ulong strsize = (uint) buffer.Length;
                        if (strsize != 0 && strsize >= size && strsize < memsize)
                        {
                            variables["$RESULT"] = Var.Create(buffer.ToString()!);       //$BUGBUG! stringize buffer first.
                            variables["$RESULT_1"] = Var.Create(strsize);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private bool DoHANDLE(Expression[] args)
        {
            if (args.Length == 3 && GetUlong(args[0], out ulong x) && GetUlong(args[1], out ulong y) && GetString(args[2], out string? sClassName))
            {
                variables["$RESULT"] = Var.Create((ulong) Host.FindHandle(Host.TE_GetMainThreadId(), sClassName, x, y));
                return true;
            }
            return false;
        }

        private bool DoINC(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (GetUlong(args[0], out ulong dw))
                {
                    dw++;
                    return SetULong(args[0], dw);
                }
                else if (GetFloat(args[0], out double flt))
                {
                    flt++;
                    return SetFloat(args[0], flt);
                }
            }
            return false;
        }

        // Olly only
        private bool DoHISTORY(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong dw))
            {
                return true;
            }
            return false;
        }

        private bool DoITOA(Expression[] args)
        {
            ulong @base = 16;
            if (args.Length >= 1 && args.Length <= 2 && GetUlong(args[0], out ulong dw))
            {
                if (args.Length == 2 && !GetUlong(args[1], out @base))
                    return false;

                if (@base >= 2 && @base <= 32)
                {
                    variables["$RESULT"] = Var.Create(Helper.rul2str((uint)dw, (uint)@base));
                    return true;
                }
                errorstr = "Invalid base";
            }
            return false;
        }

        private bool DoJA(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (!zf && !cf)
                    return DoJMP(args);
                else
                    return true;
            }
            return false;
        }

        private bool DoJAE(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (!cf)
                    return DoJMP(args);
                else
                    return true;
            }
            return false;
        }

        private bool DoJB(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (cf)
                    return DoJMP(args);
                else
                    return true;
            }
            return false;
        }

        private bool DoJBE(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (zf || cf)
                    return DoJMP(args);
                else
                    return true;
            }
            return false;
        }

        private bool DoJE(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (zf)
                    return DoJMP(args);
                else
                    return true;
            }
            return false;
        }

        private bool DoJMP(Expression[] args)
        {
            if (args.Length == 1 && Script.TryGetLabel(args[0], out int lineNumber))
            {
                script_pos_next = lineNumber;
                return true;
            }
            return false;
        }

        private bool DoJNE(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (!zf)
                    return DoJMP(args);
                else
                    return true;
            }
            return false;
        }

        // TE?
        private bool DoKEY(Expression[] args)
        {
            bool shift = false, ctrl = false;
            if (args.Length >= 1 && args.Length <= 3 && GetUlong(args[0], out ulong key))
            {
                switch (args.Length)
                {
                case 3:
                    if (!GetBool(args[2], out ctrl)) return false;
                    goto case 2;
                case 2:
                    if (!GetBool(args[1], out shift)) return false;
                    break;
                }
                //Sendshortcut(PM_MAIN, 0, WM_KEYDOWN, ctrl, shift, key); 
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoLBL(Expression[] args)
        {
            if (args.Length == 2 && GetUlong(args[0], out ulong addr) && GetString(args[1], out string? lbl))
            {
                return true;
            }
            return false;
        }

        private bool DoLM(Expression[] args)
        {
            throw new NotImplementedException();
#if LATER
ulong addr, size;
string filename;

    if(args.Length == 3 && GetUlong(args[0],  out addr) && GetUlong(args[1],  out size) && GetString(args[2],  out filename))
    {
		if(!Helper.isfullpath(filename))
			filename = Host.TE_GetTargetDirectory() + filename;	

		HANDLE hFile;
		hFile = CreateFile(filename, GENERIC_READ, FILE_SHARE_READ, null, OPEN_EXISTING, FILE_ATTRIBUHost.TE_NORMAL, null);
		if(hFile != INVALID_HANDLE_VALUE)
		{
			byte [] membuf= new byte[PAGE_SIZE];
			DWORD bytes;

			bool success = true;

			if(!size)
				size = GetFileSize(hFile, null);

			variables["$RESULT"] = size;

			while(size >= sizeof(membuf) && success)
			{
				success = false;

				if(ReadFile(hFile, membuf, sizeof(membuf), &bytes, null) && (bytes == sizeof(membuf)))
				{
					if(Host.TE_WriteMemory(addr, sizeof(membuf), membuf))
					{
						success = true;
					}
				}

				addr += sizeof(membuf);
				size -= sizeof(membuf);
			}

			if(success && size)
			{
				success = false;
				if(ReadFile(hFile, membuf, size, &bytes, null) && (bytes == size))
				{
					if(Host.TE_WriteMemory(addr, size, membuf))
					{
						success = true;
					}
				}
			}

			CloseHandle(hFile);
			return success;
		}
        else errorstr = "Couldn't open file!";
    }
    return false;
#endif
        }

        // Olly only
        private bool DoLC(Expression[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoLCLR(Expression[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoLEN(Expression[] args)
        {
            if (args.Length == 1 && GetString(args[0], out string? str))
            {
                variables["$RESULT"] = Var.Create(str.Length);
                return true;
            }
            return false;
        }

        private bool DoLOADLIB(Expression[] args)
        {
            if (args.Length == 1 && GetString(args[0], out string? str))
            {
                variables["$RESULT"] = Var.Create(0);

                errorstr = "Unsupported command!";
                return false;

                /*

                    // $RESULT EAX!!!
                    resumeDebuggee = true;
                    return true;
                }

                ulong fnload;

                SaveRegisters(true);
                ulong ip = GetContextData(UE_CIP);
                variables["$RESULT"] = Var.Create(0);

                DoGPA("\"LoadLibraryA\",\"kernel32.dll\"");
                fnload = variables["$RESULT"].dw;

                //alloc memory bloc to store DLL name
                ULONG_PTR hMem = Host.TE_AllocMemory(0x1000); //VirtualAllocEx(Host.TE_GetProcessHandle(), null, 0x1000, MEM_RESERVE | MEM_COMMIT, PAGE_EXECUHost.TE_READWRITE);

                char bfdlladdr[10]={0};
                sprintf(bfdlladdr, "%09lX", hMem);

                TE_WriteMemory(hMem, str.Length, str);
                //Writememory((void*)str, (ulong) hMem, str.Length, MM_DELANAL|MM_SILENT);

                if(DoPUSH(bfdlladdr))
                {
                    char bffnloadlib[10] = {0};
                    sprintf(bffnloadlib, "%09X", fnload);
                    string libPtrToLoad = bffnloadlib;

                    //ExecuteASM("call " + libPtrToLoad);	

                    variables["$RESULT"] = Var.Create(0);

                    // result returned after process
                    // variables["$RESULT"] = pt.reg.r[REG_EAX];
                    t_dbgmemblock block={0};
                    block.hmem = (void*)hMem;
                    block.size = 0x1000;
                    block.script_pos = script_pos;
                    block.free_at_ip = ip;
                    block.result_register = true;
                    block.reg_to_return = UE_EAX; // !!!
                    block.restore_registers = true;
                    block.listmemory = true;
                    block.autoclean = true;

                    // Free memory block after next ollyloop
                    regBlockToFree(block);
                    //require_addonaction = true;
                    //back_to_debugloop = true;
                    return true;
                }
                 */
            }
            return false;
        }

        private bool DoLOG(Expression[] args)
        {
            string? prefix = "";

            if (args.Length >= 1 && args.Length <= 2)
            {
                if (args.Length == 2 && !GetString(args[1], out prefix))
                    return false;

                bool noprefix = (args.Length == 1);
                if (noprefix && IsWriteable(args[0]))
                    prefix = args[0] + ": ";

                string @out;

                if (GetUlong(args[0], out ulong dw))
                {
                    @out = Helper.rul2hexstr(dw, sizeof(ulong) * 2).ToUpperInvariant();
                }
                else if (GetFloat(args[0], out double flt))
                {
                    @out = Helper.dbl2str(flt);
                }
                else if (GetString(args[0], out string? str))
                {
                    @out = Helper.IsHexLiteral(str) ? str : Helper.CleanString(str);
                }
                else return false;

                Host.TE_Log((prefix + @out));
                return true;
            }
            return false;
        }

        private bool DoLOGBUF(Expression[] args)
        {
            string? sep = " ";
            ulong dw = 0;

            if (args.Length >= 1 && args.Length <= 3 && args[0] is Identifier id && IsVariable(id.Name))
            {
                switch (args.Length)
                {
                case 3:
                    if (!GetString(args[2], out sep)) return false;
                    goto case 2;
                case 2:
                    if (!GetUlong(args[1], out dw)) return false;
                    break;
                }

                if (0 == dw)
                    dw = 16;

                Var v = variables[id.Name];

                string line = "";
                string data = v.ToHexString();

                for (int i = 0; i < v.size; i++)
                {
                    line += data.Substring(i * 2, 2) + sep;
                    if (i > 0 && 0 == ((i + 1) % (int)dw))
                    {
                        Host.TE_Log(line);
                        line = "";
                    }
                }

                if (!string.IsNullOrEmpty(line))
                    Host.TE_Log(line);

                return true;
            }
            return false;
        }

        private bool DoMEMCPY(Expression[] args)
        {
            if (args.Length == 3)
            {
                return DoMOV(new MemoryAccess(args[0], unk), new MemoryAccess(args[1], unk), args[2]);
            }
            return false;
        }

        /// <summary>
        /// Implements MOV [dst],[src](,[maxsize])
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool DoMOV(params Expression[] args)
        {
            if (args.Length >= 2 && args.Length <= 3)
            {
                ulong maxsize = 0;
                if (args.Length == 3 && !GetUlong(args[2], out maxsize))
                    return false;

                ulong dw;
                string? str;
                double flt;

                if (!IsWriteable(args[0]) && !DoVAR(args[0]))
                {
                    return false;
                }

                // Check destination
                switch (args[0])
                {
                case Identifier id:
                if (IsVariable(id.Name))
                {
                    Var v;
                    if (maxsize > sizeof(ulong) && args[1] is MemoryAccess mem) // byte string
                    {
                        Expression tmp = mem.EffectiveAddress;
                        if (GetAddress(tmp, out Address src))
                        {
                            byte[] bytes = new byte[maxsize];
                            if (!Host.TryReadBytes(src, (int) maxsize, bytes))
                                return false;
                            v = Var.Create("#" + Helper.bytes2hexstr(bytes, (int) maxsize) + '#');
                        }
                        else
                            v = Var.Create(0);
                    }
                    else if (maxsize <= sizeof(ulong) && GetUlong(args[1], out dw)) // ulong
                    {
                        // DW to DW/FLT var
                        if (maxsize == 0)
                            maxsize = 8;
                        dw = Helper.resize(dw, (int) maxsize);
                        v = Var.Create(dw);
                        v.size = (int) maxsize;
                    }
                    else if (GetAddress(args[1], out Address addrSrc))
                    {
                        v = Var.Create(addrSrc);
                    }
                    else if (GetString(args[1], (int) maxsize, out str)) // string
                    {
                        v = Var.Create(str!);
                    }
                    else if (GetFloat(args[1], out flt)) // float
                    {
                        v = Var.Create(flt);
                    }
                    else
                        return false;

                    variables[id.Name] = v;
                    return true;
                }
                //$REFACTOR: another Identifier.
                else if (arch.TryGetRegister(id.Name, out RegisterStorage? reg))
                {
                    // Dest is register
                    if (GetUlong(args[1], out dw))
                    {
                        if (maxsize == 0)
                            maxsize = (uint)reg.DataType.Size;
                        dw = Helper.resize(dw, Math.Min((int) maxsize, (int)reg.DataType.Size));
                        return Debugger.SetContextData(reg, dw);
                    }
                }
                //$BUG: hard-wired to x86
                else if (IsFlag(id.Name))
                {
                    if (GetBool(args[1], out bool flagval))
                    {
                        eflags_t flags = new eflags_t
                        {
                            dw = Debugger.GetContextData(eContextData.UE_EFLAGS)
                        };
                        switch (id.Name[1])
                        {
                        case 'a': flags.bits.AF = flagval; break;
                        case 'c': flags.bits.CF = flagval; break;
                        case 'd': flags.bits.DF = flagval; break;
                        case 'o': flags.bits.OF = flagval; break;
                        case 'p': flags.bits.PF = flagval; break;
                        case 's': flags.bits.SF = flagval; break;
                        case 'z': flags.bits.ZF = flagval; break;
                        }

                        return Debugger.SetContextData(eContextData.UE_EFLAGS, flags.dw);
                    }
                }
                else if (IsFloatRegister(id.Name))
                {
                    if (GetFloat(args[1], out flt))
                    {
                        throw new NotImplementedException();
#if LATER
				int index = args[0][3] - '0';
				double* preg;
#if _WIN64
					XMM_SAVE_AREA32 fltctx;
					preg = (double*)&fltctx.FloatRegisters + index;
#else
					FLOATING_SAVE_AREA fltctx;
					preg = (double*)&fltctx.RegisterArea[0] + index;
#endif
				if(Debugger.GetContextFPUDataEx(Host.TE_GetCurrentThreadHandle(),  fltctx))
				{
					*preg = flt;
					return Debugger.SetContextFPUDataEx(Host.TE_GetCurrentThreadHandle(),  fltctx);
				}
#endif
                    }
                }
                break;
                case MemoryAccess memDst:
                    if (GetAddress(memDst.EffectiveAddress, out Address target))
                    {
                        if (maxsize > sizeof(ulong) && args[1] is MemoryAccess memSrc)
                        {
                            if (GetAddress(memSrc.EffectiveAddress, out Address src))
                            {
                                byte[] copybuffer;

                                copybuffer = new byte[maxsize];
                                errorstr = "Out of memory!";

                                if (!Host.TryReadBytes(src, (int) maxsize, copybuffer) || !Host.WriteMemory(target, (int) maxsize, copybuffer))
                                {
                                    return false;
                                }
                                return true;
                            }
                        }
                        else if (maxsize <= sizeof(ulong) && GetUlong(args[1], out dw))
                        {
                            if (maxsize == 0)
                                maxsize = sizeof(ulong);
                            return Host.WriteMemory(target, dw);
                        }
                        else if (GetString(args[1], (int) maxsize, out str))
                        {
                            Var v = Var.Create(str!);
                            if (maxsize == 0)
                                maxsize = (ulong) v.size;
                            maxsize = Math.Min(maxsize, (ulong) v.size);
                            return Host.WriteMemory(target, (int) maxsize, Encoding.UTF8.GetBytes(v.to_string()));
                        }
                        else if (GetFloat(args[1], out flt))
                        {
                            return Host.WriteMemory(target, flt);
                        }
                    }
                    break;
                }
            }
            return false;
        }

        private bool DoMSG(Expression[] args)
        {
            if (args.Length == 1 && GetAnyValue(args[0], out string? msg))
            {
                Debug.Print("OllyLang: {0}", msg);
                if (Host.DialogMSG(msg!, out int input))
                {
                    if (input == -1) // IDCANCEL)
                        return Pause();
                    return true;
                }
            }
            return false;
        }

        private bool DoMSGYN(Expression[] args)
        {
            if (args.Length == 1 && GetString(args[0], out string? msg))
            {
                if (Host.DialogMSGYN(msg, out int input))
                {
                    switch (input)
                    {
                    case 0:
                        variables["$RESULT"] = Var.Create(2);
                        return Pause();
                    case 1:
                        variables["$RESULT"] = Var.Create(1);
                        break;
                    default:
                        variables["$RESULT"] = Var.Create(0);
                        break;
                    }
                    return true;
                }
            }
            return false;
        }

        private bool DoOLLY(Expression[] args)
        {
            throw new NotImplementedException();
#if LATER
string param;

	if(args.Length == 1)
	{
		param = Helper.toupper(args[0]);

		if(param == "PID")
		{
			variables["$RESULT"] = GetCurrentProcessId();
			return true;
		}
		else if(param == "HWND")
		{
			variables["$RESULT"] = null;
			return true;
		}
	}
	return false;
#endif
        }

        private bool DoOR(Expression[] args)
        {
            if (args.Length == 2 && GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
            {
                return SetULong(args[0], dw1 | dw2);
            }
            return false;
        }

        private bool DoMUL(Expression[] args)
        {
            if (args.Length == 2)
            {
                if (GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
                {
                    return SetULong(args[0], dw1 * dw2);
                }
                else if (GetFloat(args[0], out double flt1) && GetFloat(args[1], out double flt2))
                {
                    return SetFloat(args[0], flt1 * flt2);
                }
                else if (GetFloat(args[0], out  flt1) && GetUlong(args[1], out dw2))
                {
                    return SetFloat(args[0], flt1 * dw2);
                }
                else if (GetUlong(args[0], out dw1) && GetFloat(args[1], out flt2))
                {
                    return SetFloat(args[0], dw1 * flt2);
                }
            }
            return false;
        }

        private bool DoNEG(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (GetUlong(args[0], out ulong dw))
                {
                    return SetULong(args[0], (ulong) (-(long) dw));
                }
                else if (GetFloat(args[0], out double flt))
                {
                    return SetFloat(args[0], -flt);
                }
            }
            return false;
        }

        private bool DoNOT(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong dw))
            {
                return SetULong(args[0], ~dw);
            }
            return false;
        }

        //see also GCI
        private bool DoOPCODE(Expression[] args)
        {
            if (args.Length == 1 && GetAddress(args[0], out Address addr))
            {
                byte[] buffer = new byte[MAX_INSTR_SIZE];

                variables["$RESULT"] = variables["$RESULT_1"] = variables["$RESULT_2"] = Var.Create(0);

                if (Host.TryReadBytes(addr, buffer.Length, buffer))
                {
                    string opstring = Host.Disassemble(buffer, addr, out int opsize);
                    if (opsize != 0)
                    {
                        variables["$RESULT"] = Var.Create(Helper.bytes2hexstr(buffer, opsize));
                        variables["$RESULT_1"] = Var.Create(opstring);
                        variables["$RESULT_2"] = Var.Create(opsize);
                    }
                }
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoOPENDUMP(Expression[] args)
        {
            if (args.Length >= 1 && args.Length <= 3 && GetUlong(args[0], out ulong addr))
            {
                switch (args.Length)
                {
                case 3:
                    if (!GetUlong(args[2], out ulong size)) return false;
                    goto case 2;
                case 2:
                    if (!GetUlong(args[1], out ulong @base)) return false;
                    break;
                }

                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoOPENTRACE(Expression[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoPAUSE(Expression[] args)
        {
            if (args.Length == 0)
            {
                return Pause();
            }
            return false;
        }

        private bool DoPOP(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 1 && !GetUlong(args[0], out ulong dw))
                    return false;

                ulong CSP = Debugger.GetContextData(eContextData.UE_CSP);
                Debugger.SetContextData(eContextData.UE_CSP, CSP + sizeof(ulong));
                if (args.Length == 1)
                {
                    var ea = Address.Ptr32((uint) CSP);
                    if (!Host.SegmentMap.TryFindSegment(ea, out ImageSegment? segment))
                        throw new AccessViolationException();
                    if (!segment.MemoryArea.TryReadLeUInt32(ea, out var value))
                        throw new AccessViolationException();
                    dw = value;
                    return SetULong(args[0], dw);
                }
                return true;
            }
            return false;
        }

        private bool DoPOPA(Expression[] args)
        {
            if (args.Length == 0)
            {
                return RestoreRegisters(true);
            }
            return false;
        }

        private bool DoPREOP(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                Address addr = default;
                if (args.Length == 1 && !GetAddress(args[0], out addr))
                    return false;
                else if (args.Length == 0)
                    addr = Debugger.InstructionPointer;

                variables["$RESULT"] = Var.Create(0);

                int prevsize = Host.LengthDisassembleBackEx(addr);
                if (prevsize != 0)
                {
                    variables["$RESULT"] = Var.Create(addr! - prevsize);
                }
                return true;
            }
            return false;
        }

        private bool DoPUSH(Expression[] args)
        {
            if (args.Length == 1 && GetUlong(args[0], out ulong dw))
            {
                ulong CSP = Debugger.GetContextData(eContextData.UE_CSP) - sizeof(ulong);
                Debugger.SetContextData(eContextData.UE_CSP, CSP);
                return Host.WriteMemory(Address.Ptr32((uint)CSP), dw); ;
            }
            return false;
        }

        private bool DoPUSHA(Expression[] args)
        {
            if (args.Length == 0)
            {
                return SaveRegisters(true);
            }
            return false;
        }

        private bool DoREADSTR(Expression[] args)
        {
            if (args.Length == 2 && 
                GetUlong(args[1], out ulong maxsize) &&
                GetString(args[0], (int) maxsize, out string? str))
            {
                variables["$RESULT"] = Var.Create(str!);
                return true;
            }
            return false;
        }

        // Restore Break Points
        // restores all hardware and software breakpoints
        // (if arg1 == 'STRICT', all soft bp set by script will be deleted and only those have been set before it runs
        // will be restored
        // if no argument set, previous soft bp will be appended to those set by script)

        // rbp [arg1]
        // arg1 = may be STRICT or nothing

        // return in:
        // - $RESULT number of restored swbp
        // - $RESULT_1 number of restored hwbp

        // ex     : rbp
        //        : rbp STRICT
        private bool DoRBP(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 1 && args[0] is Identifier cmd)
                {
                    if (cmd.Name.ToUpperInvariant() != "STRICT")
                        return false;
                }

                return true;
            }
            /*
            t_table* bpt = 0;
            t_bpoint* bpoint = 0;
            uint n,i=0;
            string ops[1];

            CreateOperands ( args, ops, 1 );
	
            variables["$RESULT"] = Var.Create(0);
            if (saved_bp != 0)
            */
            {
                /*
                bpt = ( t_table * ) Plugingetvalue ( VAL_BREAKPOINTS );
                if ( bpt is not null )
                {
                    bpoint = ( t_bpoint * ) bpt.data.data;

                    if ( ops[0] == "STRICT" )
                    {
                        int dummy;
                        dummy = bpt.data.n;
                        for ( n = 0; n < dummy; n++ )
                        {
                            Deletebreakpoints ( bpoint.addr, ( bpoint.addr ) + 1, 1 );
                        }
                    }

                    for ( n=0; n < sortedsoftbp_t.n; n++ )
                        Setbreakpoint ( softbp_t[n].addr, ( softbp_t[n].type | TY_KEEPCODE ) ^ TY_KEEPCODE, 0 );

                    variables["$RESULT"] = ( DWORD ) sortedsoftbp_t.n;

                    Broadcast ( WM_USER_CHALL, 0, 0 );
                }
                */
            }

            //Hardware Bps
            /*
            for ( n=0; n < 4; n++ ) {
                if (hwbp_t[n].addr) {
                    Sethardwarebreakpoint ( hwbp_t[n].addr, hwbp_t[n].size, hwbp_t[n].type );
                    i++;
                }
            }
            variables["$RESULT_1"] = ( DWORD ) i;
            */
            return false;
        }

        // Olly only
        private bool DoREF(Expression[] args)
        {
            string str = "MEMORY";

            if (args.Length >= 1 && args.Length <= 2 && GetUlong(args[0], out ulong addr))
            {
                if (args.Length == 2 && args[1] is Identifier domain)
                    str = domain.Name.ToUpperInvariant();

                string[] valid_commands = { "MEMORY", "CODE", "MODULE" };

                variables["$RESULT"] = Var.Create(0);
                variables["$RESULT_1"] = Var.Create(0);
                variables["$RESULT_2"] = Var.Create(0);

                return valid_commands.Contains(str, StringComparer.InvariantCultureIgnoreCase);
            }
            return false;
        }

        // Olly only
        private bool DoREFRESH(Expression[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoREPL(Expression[] args)
        {
            ulong len = 0;

            if (args.Length >= 3 && args.Length <= 4 &&
                GetAddress(args[0], out Address addr) && 
                Helper.TryGetHexLiteral(args[1], out string? v1) && 
                Helper.TryGetHexLiteral(args[2], out string? v2))
            {
                if (args.Length == 4 && !GetUlong(args[3], out len))
                    return false;
                if (args.Length == 3)
                {
                    if (!Host.TE_GetMemoryInfo(addr, out MEMORY_BASIC_INFORMATION? MemInfo))
                        return true;

                    len = (ulong) (MemInfo.BaseAddress! - addr) + MemInfo.RegionSize;
                }
                int oplen = v1.Length;
                if (oplen != v2.Length)
                {
                    errorstr = "Hex strings must have the same size!";
                    return false;
                }

                byte[]? membuf = null, mask1 = null, mask2 = null, bytes1 = null, bytes2;
                membuf = new byte[len];
                if (Host.TryReadBytes(addr, (int)len, membuf))
                {
                    bool replaced = false;

                    int bytecount = oplen / 2;

                    mask1 = new byte[bytecount];
                    mask2 = new byte[bytecount];
                    bytes1 = new byte[bytecount];
                    bytes2 = new byte[bytecount];

                    Helper.hexstr2bytemask(v1, mask1, bytecount);
                    Helper.hexstr2bytemask(v2, mask2, bytecount);
                    Helper.hexstr2bytes(v1, bytes1, bytecount);
                    Helper.hexstr2bytes(v2, bytes2, bytecount);

                    for (int i = 0; (i + bytecount) <= (int)len; )
                    {
                        if (Helper.MaskedCompare(membuf, i, bytes1, mask1, bytecount))
                        {
                            Helper.memcpy_mask(membuf, i, bytes2, mask2, bytecount);
                            i += bytecount;
                            replaced = true;
                        }
                        else i++;
                    }

                    if (replaced)
                        Host.WriteMemory(addr, (int)len, membuf);
                }

                return true;
            }
            return false;
        }

        // TE?
        private bool DoRESET(Expression[] args)
        {
            if (args.Length == 0)
            {
                //Sendshortcut(PM_MAIN, 0, WM_KEYDOWN, 1, 0, VK_F2); 
                return true;
            }
            return false;
        }

        private bool DoRET(Expression[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0)
                {
                    if (calls.Count > 0)
                    {
                        script_pos_next = calls[calls.Count - 1];
                        calls.RemoveAt(calls.Count - 1);

                        if (callbacks.Count != 0 && callbacks.Last().call == calls.Count)
                        {
                            if (callbacks.Last().returns_value)
                            {
                                errorstr = "Callback needs to return a value!";
                                return false;
                            }
                            callbacks.RemoveAt(callbacks.Count - 1);
                        }
                    }
                    else
                    {
                        Reset();
                    }
                    Debugger.StopDebug();
                    return true;
                }
                else if (args.Length == 1)
                {
                    if (calls.Count > 0)
                    {
                        script_pos_next = calls.Last();
                        calls.RemoveAt(calls.Count - 1);

                        if (callbacks.Count != 0 && callbacks.Last().call == calls.Count)
                        {
                            if (callbacks.Last().returns_value)
                            {
                                variables["$TEMP"] = Var.Create(0);
                                var _temp = new Identifier("$TEMP", unk, MemoryStorage.Instance);   //$REFACTOR: make this a method
                                if (DoMOV(_temp, args[0]))
                                {
                                    callback_return = variables["$TEMP"];
                                    variables.Remove("$TEMP");

                                    if (callback_return.type == Var.EType.EMP || callback_return.type == callbacks.Last().return_type)
                                    {
                                        callbacks.RemoveAt(callbacks.Count - 1);
                                        return true;
                                    }
                                }
                                else errorstr = "Invalid callback return type";
                            }
                            else errorstr = "Callback shouldn't return a value!";
                        }
                        else errorstr = "Callback shouldn't return a value!";
                    }
                    else errorstr = "Returning value outside of a callback!";
                }
            }
            return false;
        }

        private bool DoREV(Expression[] args)
        {
            if (args.Length == 1)
            {
                if (GetUlong(args[0], out ulong dw))
                {
                    variables["$RESULT"] = Var.Create(Helper.ToHostEndianness(dw, arch.Endianness));
                    return true;
                }
                else if (GetString(args[0], out string? str))
                {
                    Var tmp = Var.Create(str);
                    variables["$RESULT"] = tmp.reverse();
                    return true;
                }
            }
            return false;
        }

        private bool DoROL(Expression[] args)
        {
            throw new NotImplementedException();
#if LATER
ulong dw1, dw2;

	if(args.Length == 2 && GetUlong(args[0],  out dw1) && GetUlong(args[1],  out dw2))
	{
		return SetRulong(args[0],  rol(dw1, out dw2));
	}
	return false;
#endif
        }

        private bool DoROR(Expression[] args)
        {
            throw new NotImplementedException();
#if LATER
    ulong dw1, dw2;

	if(args.Length == 2 && GetUlong(args[0],  out dw1) && GetUlong(args[1],  out dw2))
	{
		return SetRulong(args[0],  ror(dw1, out dw2));
	}
	return false;
#endif
        }

        private bool DoRTR(Expression[] args)
        {
            if (args.Length == 0)
            {
                run_till_return = true;
                stepcount = -1;
                StepOverCallback();
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoRTU(Expression[] args)
        {
            if (args.Length == 0)
            {
                return_to_usercode = true;
                stepcount = -1;
                StepOverCallback();
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoRUN(Expression[] args)
        {
            if (args.Length == 0)
            {
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        // Store Break Points
        // stores all hardware and software breakpoints

        // return in:
        // - $RESULT number of restored swbp
        // - $RESULT_1 number of restored hwbp

        // ex 	: sbp
        // 		: no argument

        private bool DoSBP(Expression[] args)
        {
            if (args.Length == 0)
            {
                /*
                uint n = 0, i;
                bool success;
                t_table *bpt;
                t_bpoint *bpoint;

                variables["$RESULT"] = Var.Create(0);
                variables["$RESULT_1"] = 0;

                bpt = ( t_table * ) Plugingetvalue ( VAL_BREAKPOINTS );
                if ( bpt is not null )
                {
                    bpoint = ( t_bpoint * ) ( bpt.data.data );
                    if ( bpoint is not null )
                    {
                        n = bpt.data.n;

                        if ( n > alloc_bp )
                        {
                            //FreeBpMem();
                            success = AllocSwbpMem ( n );
                        }

                        if ( n > alloc_bp && !success ) {
                            errorstr = "Can't allocate enough memory to copy all breakpoints";
                            return false;
                        }
                        else if (n > 0)
                        {
                            memcpy ( ( void* ) softbp_t, bpt.data.data, n*sizeof ( t_bpoint ) );
                            memcpy ( ( void* ) &sortedsoftbp_t, ( void* ) &bpt.data, sizeof ( t_sorted ) );
					
                        } 

                        saved_bp = n;
                        variables["$RESULT"] =  ( DWORD ) n;
                    }
                }

                memcpy ( ( void* ) &hwbp_t, ( void* ) ( Plugingetvalue ( VAL_HINST ) +0xD8D70 ), 4 * sizeof ( t_hardbpoint ) );

                n = i = 0;
                while ( n < 4 )
                {
                    if ( hwbp_t[n].addr )
                        i++;
                    n++;
                }
                variables["$RESULT_1"] =  ( DWORD ) i;
                */
                return true;
            }
            return false;
        }

        private bool DoSCMP(params Expression[] args)
        {
            return DoSCMP(StringComparer.InvariantCulture, args);
        }

        private bool DoSCMP(StringComparer cmp, Expression [] args)
        {
            if (args.Length >= 2 && args.Length <= 3)
            {
                ulong size = 0;
                if (args.Length == 3 && !GetUlong(args[2], out size))
                    return false;

                if (GetString(args[0], (int) size, out string? s1) && GetString(args[1], (int) size, out string? s2))
                {
                    SetCMPFlags(cmp.Compare(s1, s2));
                    return true;
                }
            }
            return false;
        }

        private bool DoSCMPI(Expression[] args)
        {
            return DoSCMP(StringComparer.InvariantCultureIgnoreCase, args);
        }

        // Olly only
        private bool DoSETOPTION(Expression[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoSHL(Expression[] args)
        {
            if (args.Length == 2 && GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
            {
                return SetULong(args[0], dw1 << (int)dw2);
            }
            return false;
        }

        private bool DoSHR(Expression[] args)
        {
            if (args.Length == 2 && GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
            {
                return SetULong(args[0], dw1 >> (int)dw2);
            }
            return false;
        }

        private bool DoSTI(params Expression[] args)
        {
            if (args.Length == 0)
            {
                stepcount = 1;
                StepIntoCallback();
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Step over
        /// </summary>
        private bool DoSTO(Expression[] args)
        {
            if (args.Length == 0)
            {
                stepcount = 1;
                StepOverCallback();
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoSTR(Expression[] args)
        {
            if (args.Length == 1 && args[0] is Identifier id && IsVariable(id.Name))
            {
                Var v = variables[id.Name];
                switch (v.type)
                {
                case Var.EType.DW: // empty buf + dw ->  buf
                    v = Var.Create("##") + v.ToUInt64();
                    goto case Var.EType.STR;
                case Var.EType.STR:
                    if (v.IsBuf)
                        v = Var.Create(v.to_string());
                    variables[id.Name] = v;
                    return true;
                }
            }
            return false;
        }

        private bool DoSUB(Expression[] args)
        {
            if (args.Length == 2 && GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
            {
                if (GetUlong(args[0], out dw1) && GetUlong(args[1], out dw2))
                {
                    return SetULong(args[0], dw1 - dw2);
                }
                else if (GetFloat(args[0], out double flt1) && GetFloat(args[1], out double flt2))
                {
                    return SetFloat(args[0], flt1 - flt2);
                }
            }
            return false;
        }

        // Olly only
        private bool DoTC(Expression[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoTEST(Expression[] args)
        {
            if (args.Length == 2)
            {
                if (GetAddress(args[0], out Address addr1) && GetAddress(args[1], out Address addr2))
                {
                    zf = (addr1.ToLinear() & addr2.ToLinear()) == 0;
                    return true;
                }
                if (GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
                {
                    zf = ((dw1 & dw2) == 0);
                    return true;
                }
            }
            return false;
        }

        private bool DoTI(Expression[] args)
        {
            if (args.Length == 0)
            {
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoTICK(Expression[] args)
        {
            ulong timeref = 0;

            if (args.Length >= 0 && args.Length <= 2)
            {
                if (args.Length == 2 && !GetUlong(args[1], out timeref))
                    return false;

                ulong tickcount = Helper.MyTickCount() - tickcount_startup;

                if (args.Length == 0)
                {
                    variables["$RESULT"] = Var.Create(Helper.rul2decstr(tickcount / 1000) + " ms");
                    return true;
                }
                else if (args[0] is Identifier id && (IsVariable(id.Name) || DoVAR(id)))
                {
                    variables[id.Name] = Var.Create((uint)tickcount);
                    if (args.Length == 2)
                        variables["$RESULT"] = Var.Create((uint)(tickcount - timeref));
                    return true;
                }
            }
            return false;
        }

        // TE?
        private bool DoTICND(Expression[] args)
        {
            if (args.Length == 1 && GetString(args[0], out string? condition))
            {
                errorstr = "Unsupported command!";
                return false;

                /*
                char* buffer = new char[condition.Length + 1];
                strcpy(buffer, condition);
                if(Runtracesize() == 0) 
                {
                    ulong threadid = Getcputhreadid();
                    if(threadid == 0)
                        threadid = Plugingetvalue(VAL_MAINTHREADID);
                    t_thread* pthr = Findthread(threadid);
                    if(pthr is not null)
                        Startruntrace(&(pthr.reg)); 
                }
                Settracecondition(buffer, 0, 0, 0, 0, 0);
                Sendshortcut(PM_MAIN, 0, WM_KEYDOWN, 1, 0, VK_F11); 
                back_to_debugloop = true;
                return true;
                */

            }
            return false;
        }

        private bool DoTO(Expression[] args)
        {
            if (args.Length == 0)
            {
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        // TE?
        private bool DoTOCND(Expression[] args)
        {
            if (args.Length == 1 && GetString(args[0], out string? condition))
            {
                errorstr = "Unsupported command!";
                return false;

                /*
                char* buffer = new char[condition.Length + 1];
                strcpy(buffer, condition);
                if(Runtracesize() == 0) 
                {
                    ulong threadid = Getcputhreadid();
                    if(threadid == 0)
                        threadid = Plugingetvalue(VAL_MAINTHREADID);
                    t_thread* pthr = Findthread(threadid);
                    if(pthr is not null)
                        Startruntrace(&(pthr.reg));
                }
                Settracecondition(buffer, 0, 0, 0, 0, 0);
                Sendshortcut(PM_MAIN, 0, WM_KEYDOWN, 1, 0, VK_F12); 
                //back_to_debugloop = true;
                return true;
                */
            }
            return false;
        }

        // Unused
        private bool DoUNICODE(Expression[] args)
        {
            if (args.Length == 1 && GetBool(args[0], out bool enable))
            {
                return true;
            }
            return false;
        }

        private bool DoVAR(params Expression[] args)
        {
            if (args.Length == 1 && args[0] is Identifier id)
            {
                if (IsValidVariableName(id.Name))
                {
                    variables[id.Name] = Var.Create(0);
                    return true;
                }
                errorstr = "Bad variable name: " + args[0];
            }
            return false;
        }

        private bool DoXCHG(Expression[] args)
        {
            if (args.Length == 2)
            {
                if (GetUlong(args[0], out ulong dw1) && GetUlong(args[1], out ulong dw2))
                {
                    return SetULong(args[0], dw2) && SetULong(args[1], dw1);
                }
                else if (GetFloat(args[0], out double flt1) && GetFloat(args[1], out double flt2))
                {
                    return SetFloat(args[0], flt2) && SetFloat(args[1], flt1);
                }
                else if (args[0] is Identifier id0 && IsVariable(id0.Name) && 
                         args[1] is Identifier id1 && IsVariable(id1.Name))
                {
                    Var tmp = variables[id0.Name];
                    variables[id0.Name] = variables[id1.Name];
                    variables[id1.Name] = tmp;
                    return true;
                }
            }
            return false;
        }


        private bool DoXOR(Expression[] args)
        {
            if (args.Length == 2 &&
                GetUlong(args[0], out ulong dw1) &&
                GetUlong(args[1], out ulong dw2))
            {
                return SetULong(args[0], dw1 ^ dw2);
            }
            return false;
        }

        private bool DoWRT(Expression[] args)
        {
            throw new NotImplementedException();
#if LATER
string filename, data;

	if(args.Length == 2 && GetString(args[0],  out filename) && GetAnyValue(args[1],  out data))
	{
		if(!Helper.isfullpath(filename))
			filename = Host.TE_GetTargetDirectory() + filename;

		HANDLE hFile = CreateFile(filename, GENERIC_WRITE, null, null, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, null);
		if(hFile != INVALID_HANDLE_VALUE)
		{
			DWORD written;
			WriteFile(hFile, data, data.Length, &written, null);
			CloseHandle(hFile);
		}
		return true;
	}
	return false;
#endif
        }

        private bool DoWRTA(Expression[] args)
        {
            string? @out = "\r\n";
            if (args.Length >= 2 && args.Length <= 3 && GetString(args[0], out string? filename) && GetAnyValue(args[1], out string? data))
            {
                if (args.Length == 3 && !GetString(args[2], out @out))
                    return false;

                if (!Path.IsPathRooted(filename))
                    filename = Host.TE_GetTargetDirectory() + filename;

                var fsSvc = services.RequireService<IFileSystemService>();
                using (var hFile = fsSvc.CreateFileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    @out += data;
                    hFile.Seek(0, SeekOrigin.End);
                    byte[] b = Encoding.UTF8.GetBytes(@out);
                    hFile.Write(b, 0, b.Length);
                }
                return true;
            }
            return false;
        }

        private bool RekoAddSegmentReference(Expression [] args)
        {
            if (args.Length == 2)
            {
                if (GetAddress(args[0], out Address addr) &&
                    GetUlong(args[1], out ulong seg))
                {
                    Host.AddSegmentReference(addr, (ushort) seg);
                    return true;
                }
            }
            else if (args.Length == 1)
            {
                if (GetUlong(args[0], out ulong uSeg))
                {
                    var addrSeg = Address.SegPtr((ushort) uSeg, 0);
                    if (this.Host.SegmentMap.TryFindSegment(addrSeg, out var seg))
                    {
                        var mem = seg.MemoryArea;
                        this.Host.SegmentMap.AddOverlappingSegment($"seg{uSeg:X4}", mem, addrSeg, AccessMode.ReadWriteExecute);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool RekoDumpBytes(Expression[] args)
        {
            if (args.Length < 1)
                return false;
            ulong count = 0x80;
            if (args.Length == 2 && !GetUlong(args[1], out count))
                return false;
            if (!GetAddress(args[0], out Address addr))
                return false;
            if (!this.Host.SegmentMap.TryFindSegment(addr, out var segment))
                return false;
            var mem = segment.MemoryArea;
            long offset = addr - mem.BaseAddress;
            if (offset < 0)
                return false;
            var rdr = arch.CreateImageReader(mem, addr, (int)count);
            var memfmt = mem.Formatter;
            var sw = new StringWriter();
            var stm = new TextFormatter(sw);
            var output = new TextMemoryFormatterOutput(stm);

            memfmt.RenderMemory(rdr, System.Text.Encoding.ASCII, output);

            Host.TE_Log(sw.ToString());
            return true;
        }

        private bool RekoDisassemble(Expression[] args)
        {
            if (args.Length < 1)
                return false;
            if (!GetAddress(args[0], out Address addr))
                return false;
            ulong count = 10;
            if (args.Length == 2)
            {
                if (!GetUlong(args[1], out count))
                    return false;
            }
            if (!this.Host.SegmentMap.TryCreateImageReader(addr, arch, out var rdr))
                return false;
            var sw = new StringWriter();
            var dasm = arch.CreateDisassembler(rdr).GetEnumerator();
            for (uint i = 0; i < count && dasm.MoveNext(); ++i)
            {
                var instr = dasm.Current;
                sw.WriteLine("{0}: {1}", instr.Address, instr);
            }

            Host.TE_Log(sw.ToString());
            return true;
        }

        private bool RekoArch(Expression[] args)
        {
            if (args.Length != 1)
                return false;
            if (!GetString(args[0], out string? sArch))
                return false;
            var cfgSvc = services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture(sArch);
            if (arch is null)
            {
                Host.MsgError($"Unknown architecture '{sArch}'.");
                return false;
            }
            this.Host.SetArchitecture(arch);
            this.arch = arch;
            return true;
        }

        private bool RekoClearEntryPoints(Expression[] args)
        {
            if (args.Length != 0)
                return false;
            this.Host.ClearEntryPoints();
            return true;
        }

        private bool RekoAddEntryPoint(Expression[] args)
        {
            if (args.Length != 1)
                return false;
            if (!GetAddress(args[0], out var addr))
                return false;
            this.Host.AddEntryPoint(arch, addr);
            return true;
        }
    }
}