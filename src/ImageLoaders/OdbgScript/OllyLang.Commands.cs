#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.ImageLoaders.OdbgScript
{
    using Core.Services;
    using rulong = System.UInt64;

    public partial class OllyLang
    {
        const int MAX_INSTR_SIZE = 16;      // x86
        const int PAGE_SIZE = 4096;         // x86 sometimes.

        bool DoADD(string[] args)
        {
            rulong dw1, dw2;
            double flt1, flt2;
            string str1, str2;

            if (args.Length == 2)
            {
                if (GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
                {
                    return SetRulong(args[0], dw1 + dw2);
                }
                else if (GetFloat(args[0], out flt1) && GetFloat(args[1], out flt2))
                {
                    return SetFloat(args[0], flt1 + flt2);
                }
                else if (GetFloat(args[0], out flt1) && GetRulong(args[1], out dw2))
                {
                    return SetFloat(args[0], flt1 + dw2);
                }
                else if (GetString(args[0], out str1) && GetRulong(args[1], out dw2))
                {
                    Var v1 = Var.Create(str1), v2 = Var.Create(dw2);
                    Var v3 = v1 + v2;
                    return SetString(args[0], v3.str);
                }
                else if ((GetString(args[0], out str1) && GetAnyValue(args[1], out str2)) || (GetAnyValue(args[0], out str1) && GetAnyValue(args[1], out str2)))
                {
                    Var v1 = Var.Create(str1), v2 = Var.Create(str2);
                    Var v3 = v1 + v2;
                    return SetString(args[0], v3.str);
                }
            }
            return false;
        }

        private bool DoAI(string[] args)
        {
            if (args.Length == 0)
            {
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoALLOC(string[] args)
        {
            rulong size;

            if (args.Length == 1 && GetRulong(args[0], out size))
            {
                rulong addr = Host.TE_AllocMemory(size);
                variables["$RESULT"] = Var.Create(addr);
                if (addr != 0)
                    regBlockToFree(addr, size, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Analyze the module containing the address <paramref name='addr' />
        /// </summary>
        /// <remarks>
        /// Only valid for ODBC, we ignore the command here.
        /// </remarks>
        private bool DoAN(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
            {
                return true;
            }
            return false;
        }

        private bool DoAND(string[] args)
        {
            rulong dw1, dw2;

            if (args.Length == 2 && GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
            {
                return SetRulong(args[0], dw1 & dw2);
            }
            return false;
        }

        private bool DoAO(string[] args)
        {
            if (args.Length == 0)
            {
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoASK(string[] args)
        {
            string title;
            string returned;

            if (args.Length == 1 && GetString(args[0], out title))
            {
                variables["$RESULT"] = variables["$RESULT_1"] = Var.Create(0);

                if (Host.DialogASK(title, out returned))
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

        private bool DoASM(string[] args)
        {
            string cmd;
            rulong addr, attempt;

            if (args.Length >= 2 && args.Length <= 3 && GetRulong(args[0], out addr) && GetString(args[1], out cmd))
            {
                if (args.Length == 3 && !GetRulong(args[2], out attempt))
                    return false;

                int len = Host.AssembleEx(FormatAsmDwords(cmd), addr);
                if (len == 0)
                {
                    errorstr = "Invalid command: " + cmd;
                    return false;
                }
                variables["$RESULT"] = Var.Create((rulong)len);
                return true;
            }
            return false;
        }

        private bool DoASMTXT(string[] args)
        {
            rulong addr;
            string asmfile;

            if (args.Length == 2 && GetRulong(args[0], out addr) && GetString(args[1], out asmfile))
            {
                int totallen = 0;

                List<string> lines = Host.getlines_file(asmfile);

                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (line.Length != 0)
                    {
                        int len = Host.AssembleEx(FormatAsmDwords(line), (rulong)(addr + (uint)totallen));
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

        private bool DoATOI(string[] args)
        {
            string str;
            rulong @base = 16;

            if (args.Length >= 1 && args.Length <= 2 && GetString(args[0], out str))
            {
                if (args.Length == 2 && !GetRulong(args[1], out @base))
                    return false;

                variables["$RESULT"] = Var.Create(Helper.str2rul(str, (uint)@base));
                return true;
            }
            return false;
        }

        private bool DoBC(string[] args)
        {
            rulong addr;

            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0)
                {
                    return DoBCA();
                }
                else if (GetRulong(args[0], out addr))
                {
                    Debugger.DeleteBPX(addr);
                    return true;
                }
            }
            return false;
        }

        private bool DoBCA(params string[] args)
        {
            if (args.Length == 0)
            {
                Debugger.RemoveAllBreakPoints(Ue.UE_OPTION_REMOVEALLDISABLED);
                Debugger.RemoveAllBreakPoints(Ue.UE_OPTION_REMOVEALLENABLED);
                return true;
            }
            return false;
        }

        private bool DoBD(string[] args)
        {
            rulong addr;

            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0)
                {
                    return DoBDA();
                }
                else if (GetRulong(args[0], out addr))
                {
                    Debugger.DisableBPX(addr);
                    return true;
                }
            }
            return false;
        }

        private bool DoBDA(params string[] args)
        {
            if (args.Length == 0)
            {
                Debugger.RemoveAllBreakPoints(Ue.UE_OPTION_DISABLEALL); // int3 only?
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoBEGINSEARCH(string[] args)
        {
            rulong start = 0;

            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 1 && !GetRulong(args[0], out start))
                    return false;

                return true;
            }
            return false;
        }

        // Olly only
        private bool DoENDSEARCH(params string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoBP(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
            {
                Debugger.SetBPX(addr, Ue.UE_BREAKPOINT, SoftwareCallback);
                return true;
            }
            return false;
        }

        // TE?
        private bool DoBPCND(string[] args)
        {
            rulong addr = 0;
            string condition;

            if (args.Length == 2 && GetRulong(args[0], out addr) && GetString(args[1], out condition))
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

        private bool DoBPD(string[] args)
        {
            if (args.Length == 1)
            {
                return callCommand(DoBPX, args[0], "1");
            }
            return false;
        }

        private bool DoBPGOTO(string[] args)
        {
            rulong addr;

            if (args.Length == 2 && GetRulong(args[0], out addr) && script.IsLabel(args[1]))
            {
                bpjumps[addr] = script.Labels[args[1]];
                return true;
            }
            return false;
        }

        private bool DoBPHWCA(string[] args)
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

        private bool DoBPHWC(string[] args)
        {
            rulong addr;

            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0)
                {
                    return DoBPHWCA(new string[0]);
                }
                else if (GetRulong(args[0], out addr))
                {
                    rulong[] DRX = new rulong[4];

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

        private bool DoBPHWS(string[] args)
        {
            rulong addr;
            string typestr = "x";

            if (args.Length >= 1 && args.Length <= 2 && GetRulong(args[0], out addr))
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

        // TE?
        private bool DoBPL(string[] args)
        {
            rulong addr;
            string expression;

            if (args.Length == 2 && GetRulong(args[0], out addr) && GetString(args[1], out expression))
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

        // TE?
        private bool DoBPLCND(string[] args)
        {
            rulong addr;
            string expression, condition;

            if (args.Length == 3 && GetRulong(args[0], out addr) && GetString(args[1], out expression) && GetString(args[2], out condition))
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

        private bool DoBPMC(params string[] args)
        {
            if (args.Length == 0)
            {
                if (membpaddr != 0 && membpsize != 0)
                {
                    Debugger.RemoveMemoryBPX(membpaddr, membpsize);
                    membpaddr = membpsize = 0;
                }
                return true;
            }
            return false;
        }

        private bool DoBPRM(string[] args)
        {
            rulong addr, size;

            if (args.Length == 2 && GetRulong(args[0], out addr) && GetRulong(args[1], out size))
            {
                if (membpaddr != 0 && membpsize != 0)
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

        private bool DoBPWM(string[] args)
        {
            rulong addr, size;

            if (args.Length == 2 && GetRulong(args[0], out addr) && GetRulong(args[1], out size))
            {
                if (membpaddr != 0 && membpsize != 0)
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

        // TE?
        private bool DoBPX(string[] args)
        {
            string callname;
            rulong del = 0;

            if (args.Length >= 1 && args.Length <= 2 && GetString(args[0], out callname))
            {
                if (args.Length == 2 && !GetRulong(args[1], out del))
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

                if(reftable==null || reftable.data.n==0)
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

        private bool DoBUF(string[] args)
        {
            if (args.Length == 1 && IsVariable(args[0]))
            {
                Var v = variables[args[0]];

                switch (v.type)
                {
                case Var.etype.STR: // empty buf + str . buf
                    if (!v.IsBuf)
                        v = Var.Create("##") + v.str;
                    break;
                case Var.etype.DW: // empty buf + dw . buf
                    v = Var.Create("##") + v.ToUInt64();
                    break;
                }
                return true;
            }
            return false;
        }

        private bool DoCALL(params string[] args)
        {
            if (args.Length == 1 && script.IsLabel(args[0]))
            {
                calls.Add(script_pos + 1);
                return DoJMP(args);
            }
            return false;
        }

        static string[] valid_commands = { "SCRIPT", "SCRIPTLOG", "MODULES", "MEMORY", "THREADS", "BREAKPOINTS", "REFERENCES", "SOURCELIST", "WATCHES", "PATCHES", "CPU", "RUNTRACE", "WINDOWS", "CALLSTACK", "LOG", "TEXT", "FILE", "HANDLES", "SEH", "SOURCE" };

        private bool DoCLOSE(string[] args)
        {
            throw new NotImplementedException();
#if LATER
rulong hwnd;

	if(args.Length == 1)
	{
		if(valid_commands+_countof(valid_commands) != find(valid_commands, valid_commands+_countof(valid_commands), Helper.toupper(args[0])))
		{
			return true;
		}
		else if(GetRulong(args[0],  out hwnd) && hwnd)
		{
			DestroyWindow((HWND)hwnd);
			return true;
		}
		errorstr = "Bad operand";
	}
	return false;
#endif
        }

        private bool DoCMP(string[] args)
        {
            rulong dw1, dw2;
            string s1, s2;
            double flt1, flt2;
            rulong size = 0;

            if (args.Length >= 2 && args.Length <= 3)
            {
                if (args.Length == 3 && !GetRulong(args[2], out size))
                    return false;

                Var v1, v2;
                if (GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
                {
                    v1 = Var.Create(dw1);
                    v2 = Var.Create(dw2);
                }
                else if (GetFloat(args[0], out flt1) && GetFloat(args[1], out flt2))
                {
                    v1 = Var.Create(flt1);
                    v2 = Var.Create(flt2);
                }
                else if (GetAnyValue(args[0], out s1, true) && GetAnyValue(args[1], out s2, true))
                {
                    // see also SCMP command, code is not finished here...
                    v1 = Var.Create(s1);
                    v2 = Var.Create(s2);
                }
                else
                    return false;

                if (size > 0)
                {
                    v1.resize((int)size);
                    v2.resize((int)size);
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
        private bool DoCMT(string[] args)
        {
            rulong addr;
            string cmt;

            if (args.Length == 2 && GetRulong(args[0], out addr) && GetString(args[1], out cmt))
            {
                return true;
            }
            return false;
        }

        private bool DoCOB(string[] args)
        {
            if (args.Length == 0)
            {
                EOB_row = -1;
                return true;
            }
            return false;
        }

        private bool DoCOE(string[] args)
        {
            if (args.Length == 0)
            {
                EOE_row = -1;
                return true;
            }
            return false;
        }

        private bool DoDBH(string[] args)
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

        private bool DoDBS(string[] args)
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

        private bool DoDEC(string[] args)
        {
            rulong dw;
            double flt;

            if (args.Length == 1)
            {
                if (GetRulong(args[0], out dw))
                {
                    dw--;
                    return SetRulong(args[0], dw);
                }
                else if (GetFloat(args[0], out flt))
                {
                    flt--;
                    return SetFloat(args[0], flt);
                }
            }
            return false;
        }

        private bool DoDIV(string[] args)
        {
            rulong dw1, dw2;
            double flt1, flt2;

            if (args.Length == 2)
            {
                if (GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2) && dw2 != 0)
                {
                    return SetRulong(args[0], dw1 / dw2);
                }
                else if (GetFloat(args[0], out flt1) && GetFloat(args[1], out flt2) && flt2 != 0)
                {
                    return SetFloat(args[0], flt1 / flt2);
                }
                else if (GetFloat(args[0], out flt1) && GetRulong(args[1], out dw2) && dw2 != 0)
                {
                    return SetFloat(args[0], flt1 / dw2);
                }
                else if (GetRulong(args[0], out dw1) && GetFloat(args[1], out flt2) && flt2 != 0)
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

        private bool DoDM(string[] args)
        {
            rulong addr, size;
            string filename;

            if (args.Length == 3 && GetRulong(args[0], out addr) && GetRulong(args[1], out size) && GetString(args[2], out filename))
            {
                if (!Path.IsPathRooted(filename))
                    filename = Host.TE_GetTargetDirectory() + filename;

                // Truncate existing file
                var fsSvc = services.RequireService<IFileSystemService>();
                using (Stream hfile =  fsSvc.CreateFileStream(filename, FileMode.Create, FileAccess.Write))
                {
                }
                return DoDMA(args);
            }
            return false;
        }

        private bool DoDMA(string[] args)
        {
            throw new NotImplementedException();
#if LATER
rulong addr, size;
string filename;

	if(args.Length == 3 && GetRulong(args[0],  out addr) && GetRulong(args[1],  out size) && GetString(args[2],  out filename))
	{
		bool success = true;

        if(!Helper.isfullpath(filename))
			filename = Host.TE_GetTargetDirectory() + filename;

		variables["$RESULT"] = Var.Create(size);

		HANDLE hFile = CreateFile(filename, GENERIC_WRITE, null, null, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, null);
		if(hFile != INVALID_HANDLE_VALUE)
		{
			byte[] membuf = new byte[PAGE_SIZE];
			rulong sum = 0;
			DWORD bytes;

			SetFilePointer(hFile, 0, null, FILE_END);

			while(size >= membuf.Length && success)
			{
				success = false;
				if(!Host.TE_ReadMemory(addr, sizeof(membuf), membuf))
				{
					memset(membuf, 0, sizeof(membuf));
				}

				success = hFile.Write(WriteFile(hFile, membuf, sizeof(membuf), &bytes, null) && (bytes == sizeof(membuf)));

				addr += sizeof(membuf);
				size -= sizeof(membuf);
			}

			if(success && size)
			{
				success = false;
				if(!TE_ReadMemory(addr, size, membuf))
				{
					memset(membuf, 0, size);
				}
				success = (WriteFile(hFile, membuf, size, &bytes, null) && (bytes == size));
			}

			CloseHandle(hFile);
			return success;
		}
		else errorstr = "Couldn't create file";
	}
	return false;
#endif
        }

        private bool DoDPE(string[] args)
        {
            string filename;
            rulong ep;

            if (args.Length == 2 && GetString(args[0], out filename) && GetRulong(args[1], out ep))
            {
                // We're cheating here and not actually dumping to disk. We don't need to 
                // because the image is unpacked in memory, and we are now ready to start
                // partying on it. All we have to do now is tell the caller how the value
                // of the original entry point, which is conveniently passed in the other
                // argument.
                Host.SetOriginalEntryPoint(ep);
                return true;
            }
            return false;
        }

        private bool DoENDE(string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoERUN(string[] args)
        {
            if (args.Length == 0)
            {
                ignore_exceptions = true;
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoESTI(string[] args)
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

        private bool DoESTEP(string[] args)
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

        private bool DoEOB(string[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0) // Go interactive
                {
                    EOB_row = -1;
                    return true;
                }
                else if (script.IsLabel(args[0])) // Set label to go to
                {
                    EOB_row = (int)script.Labels[args[0]];
                    return true;
                }
            }
            return false;
        }

        private bool DoEOE(string[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 0) // Go interactive
                {
                    EOE_row = -1;
                    return true;
                }
                else if (script.IsLabel(args[0])) // Set label to go to
                {
                    EOE_row = (int)script.Labels[args[0]];
                    return true;
                }
            }
            return false;
        }

        private bool DoEVAL(string[] args)
        {
            string to_eval;

            if (args.Length == 1 && GetString(args[0], out to_eval))
            {
                variables["$RESULT"] = Var.Create(ResolveVarsForExec(to_eval, false));
                return true;
            }
            return false;
        }

        private bool DoEXEC(string[] args)
        {
            if (args.Length == 0)
            {
                uint first = script_pos + 1;
                uint ende = (uint)script.NextCommandIndex((int)first);

                if (ende > script.Lines.Count)
                {
                    errorstr = "EXEC needs ENDE command!";
                    return false;
                }

                // max size for all commands + jmp eip
                int memsize = (int)(ende - first + 1) * MAX_INSTR_SIZE;

                pmemforexec = Host.TE_AllocMemory((uint)memsize);
                if (pmemforexec != 0)
                {
                    int len, totallen = 0;

                    for (uint i = first; i < ende; i++)
                    {
                        string line = ResolveVarsForExec(script.Lines[(int)i].RawLine, true);
                        if ((len = Host.AssembleEx(line, (pmemforexec + (uint)totallen))) == 0)
                        {
                            Host.TE_FreeMemory(pmemforexec);
                            errorstr = "Invalid command: " + line;
                            return false;
                        }
                        totallen += len;
                    }

                    //return at ENDE
                    script_pos_next = ende;

                    rulong eip = Debugger.GetContextData(eContextData.UE_CIP);

                    // Add jump to original EIP
                    string jmpstr = "jmp " + Helper.rul2hexstr(eip);
                    rulong jmpaddr = pmemforexec + (uint)totallen;

                    len = Host.AssembleEx(jmpstr, pmemforexec + (uint)totallen);
                    totallen += len;

                    // Set new eip and run to the original one
                    Debugger.SetContextData(eContextData.UE_CIP, pmemforexec);

                    // ignore next breakpoint
                    bInternalBP = true;

                    debuggee_running = false; // :s

                    //Debugger.SetBPX(jmpaddr, UE_BREAKPOINT, &EXECJMPCallback);
                    Debugger.SetBPX(eip, Ue.UE_BREAKPOINT, SoftwareCallback);

                    t_dbgmemblock block = new t_dbgmemblock();

                    block.address = pmemforexec;
                    block.size = (uint)memsize;
                    block.script_pos = script_pos_next;
                    block.free_at_ip = eip;
                    block.autoclean = true;

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
        private bool DoFILL(string[] args)
        {
            rulong addr, len;
            byte val = 0;

            if (args.Length == 3 && GetRulong(args[0], out addr) && GetRulong(args[1], out len) && GetByte(args[2], ref val))
            {
                byte[] membuf = new byte[PAGE_SIZE];
                int cb = Math.Min((int)len, membuf.Length);
                for (int i = 0; i < cb; ++i)
                {
                    membuf[i] = val;
                }
                
                // First write full pages, then partial page.
                while (len >= (rulong)membuf.Length)
                {
                    if (!Host.WriteMemory(addr, membuf.Length, membuf))
                        return false;

                    addr += (rulong)membuf.Length;
                    len -= (rulong)membuf.Length;
                }
                if (len > 0 && !Host.WriteMemory(addr, (int)len, membuf))
                    return false;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Searches 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool DoFIND(string[] args)
        {
            rulong addr;
            string finddata;
            rulong maxsize = 0;

            if (args.Length < 2 || args.Length > 3 || !GetRulong(args[0], out addr))
                return false;

            if (args.Length == 3 && !GetRulong(args[2], out maxsize))
                return false;

            rulong dw;
            if (GetRulong(args[1], out dw))
            {
                finddata = Helper.rul2hexstr(Helper.reverse(dw));
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
                finddata = v.to_bytes();
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
            MEMORY_BASIC_INFORMATION MemInfo;
            if (Host.TE_GetMemoryInfo(addr, out MemInfo))
            {
                int memlen = (int)(MemInfo.BaseAddress + MemInfo.RegionSize - addr);
                if (maxsize != 0 && (int)maxsize < memlen)
                    memlen = (int)maxsize;

                var ea = Address.Ptr32((uint)addr);
                ImageSegment segment;
                if (!Host.SegmentMap.TryFindSegment(ea, out segment))
                    throw new AccessViolationException();
                byte[] membuf = new byte[memlen];
                if (segment.MemoryArea.TryReadBytes(ea, memlen, membuf))
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
                            variables["$RESULT"] = Var.Create(addr + (ulong)i);
                            break;
                        }
                    }
                }
            }
            return true;
        }

        // TE?
        private bool DoFINDCALLS(string[] args)
        {
            //bool bRefVisible = false, bResetDisam = false;
            rulong addr; //, @base, size, disamsel = 0;

            if (args.Length >= 1 && args.Length <= 2 && GetRulong(args[0], out addr))
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
            if (tt!=null)
                bRefVisible=(tt.hw!=0);

            t_dump* td;
            td=(t_dump*) Plugingetvalue(VAL_CPUDASM);
            if (td==null)
                return false;

            if(GetRulong(ops[0], addr))
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
			
                    if(tt==null)
                        tt=(t_table*) Plugingetvalue(VAL_REFERENCES);

                    if(tt!=null) 
                    {
                        t_ref* tr;

                        if(tt.data.n > 1) 
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
                                    if(tr != null && tr.dest != 0)
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
                            if (tr!=null)
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
        private bool DoFINDCMD(string[] args)
        {
            //bool bRefVisible = false, bResetDisam = false;
            //string cmd, cmds;
            //int len;
            //int pos;
            //rulong addr, @base,size, attempt, opsize = 3, disamsel = 0;
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
            if (tt!=null)
                bRefVisible=(tt.hw!=0);

            t_dump* td;
            td=(t_dump*) Plugingetvalue(VAL_CPUDASM);
            if (td==null)
                return false;

            ulong tmpaddr = TE_AllocMemory(0x100);

            if (GetRulong(ops[0], addr) 
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
			
                    if (tt==null)
                        tt=(t_table*) Plugingetvalue(VAL_REFERENCES);

                    if (tt!=null) 
                    {
                        t_ref* tr;
                        if (tt.data.n > 1)
                        {
                            tr=(t_ref*) Getsortedbyselection(&tt.data, 1); //0 is CPU initial
                            if (tr!=null)
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

            if (GetRulong(ops[0], addr) 
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
        private bool DoFINDOP(string[] args)
        {
            rulong addr;
            string finddata;
            rulong maxsize = 0;

            if (args.Length >= 2 && args.Length <= 3 && GetRulong(args[0], out addr))
            {
                if (args.Length == 3 && !GetRulong(args[2], out maxsize))
                    return false;

                rulong dw;
                if (GetRulong(args[1], out dw))
                {
                    finddata = Helper.rul2hexstr(Helper.reverse(dw));
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
                    finddata = v.to_bytes();
                    if (!v.IsBuf)
                        Helper.ReplaceString(ref finddata, "3f", "??"); // 0x3F = '?' . wildcard like "mov ?ax, ?bx"
                }
                else
                    return false;

                if (Helper.IsHexWild(finddata))
                {
                    variables["$RESULT"] = Var.Create(0);

                    // search in current mem block
                    MEMORY_BASIC_INFORMATION MemInfo;
                    if (Host.TE_GetMemoryInfo(addr, out MemInfo))
                    {
                        rulong memlen = (rulong)MemInfo.BaseAddress + MemInfo.RegionSize - addr;
                        if (maxsize != 0 && maxsize < memlen)
                            memlen = maxsize;

                        byte[] membuf = null, mask = null, bytes = null;

                        membuf = new byte[memlen];
                        if (Host.TryReadBytes(addr, memlen, membuf))
                        {
                            int bytecount = finddata.Length / 2;

                            mask = new byte[bytecount];
                            bytes = new byte[bytecount + MAX_INSTR_SIZE];

                            Helper.hexstr2bytemask(finddata, mask, bytecount);
                            Helper.hexstr2bytes(finddata, bytes, bytecount);

                            for (int i = 0; (i + bytecount) <= (uint)memlen; )
                            {
                                int len = Host.LengthDisassemble(membuf, i);
                                if (len == 0)
                                    break;

                                if (len >= bytecount && Helper.MaskedCompare(membuf, i, bytes, mask, bytecount))
                                {
                                    variables["$RESULT"] = Var.Create(addr + (uint)i);
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

        private bool DoFINDMEM(string[] args)
        {
            rulong addr = 0;
            if (args.Length >= 1 && args.Length <= 2)
            {
                if (args.Length == 2 && !GetRulong(args[1], out addr))
                    return false;

                variables["$RESULT"] = Var.Create(0);

                MEMORY_BASIC_INFORMATION MemInfo;
                while (Host.TE_GetMemoryInfo(addr, out MemInfo) && variables["$RESULT"].ToUInt64() == 0)
                {
                    if (!callCommand(DoFIND, Helper.rul2hexstr(addr), args[0]))
                        return false;
                    addr = (rulong)MemInfo.BaseAddress + MemInfo.RegionSize;
                }
                return true;
            }
            return false;
        }

        private bool DoFREE(string[] args)
        {
            rulong addr, size = 0;

            if (args.Length >= 1 && args.Length <= 2 && GetRulong(args[0], out addr))
            {
                if (args.Length == 2 && !GetRulong(args[1], out size))
                    return false;

                variables["$RESULT"] = Var.Create(0);

                if ((size == 0 && Host.TE_FreeMemory(addr)) || (size != 0 && Host.TE_FreeMemory(addr, size)))
                {
                    variables["$RESULT"] = Var.Create(1);
                    unregMemBlock(addr);
                }
                return true;
            }
            return false;
        }

        private bool DoGAPI(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
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

        private bool DoGBPM(string[] args)
        {
            if (args.Length == 0)
            {
                variables["$RESULT"] = Var.Create(break_memaddr);
                return true;
            }
            return false;
        }

        private bool DoGBPR(string[] args)
        {
            if (args.Length == 0)
            {
                variables["$RESULT"] = Var.Create(break_reason);
                return true;
            }
            return false;
        }

        private bool DoGCI(string[] args)
        {
            rulong addr;
            if (args.Length == 2 && GetRulong(args[0], out addr))
            {
                var param = args[1].ToUpperInvariant();
                if (param == "COMMAND")
                {
                    int size = 0;
                    var instr = Host.DisassembleEx(Address.Ptr32((uint)addr));
                    if (size != 0)
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
        private bool DoGCMT(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
            {
                variables["$RESULT"] = Var.Create("");
                return true;
            }
            return false;
        }

        private bool DoGFO(string[] args)
        {
            throw new NotImplementedException();
#if LATER
rulong addr;
	if(args.Length == 1 && GetRulong(args[0],  out addr))
	{
		variables["$RESULT"] = Var.Create(0);

		List<MODULEENTRY32> Modules;
		if(Host.TE_GetModules(Modules))
		{
			for(int i = 0; i < Modules.Count; i++)
			{
				if(addr >= (rulong)Modules[i].modBaseAddr && addr < ((rulong)Modules[i].modBaseAddr + Modules[i].modBaseSize))
				{
					const Librarian::LIBRARY_ITEM_DATA* lib = Librarian.GetLibraryInfoEx(Modules[i].modBaseAddr);
					if(lib!=null && lib.hFileMappingView!=null)
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
        private bool DoGLBL(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
            {
                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        private bool DoGMA(string[] args)
        {
            string mod;

            if (args.Length == 2 && GetString(args[0], out mod))
            {
                if (mod.Length > 8)
                    mod = mod.Remove(8);
                mod = mod.ToLowerInvariant();

                var modules = new List<MODULEENTRY32>();
                if (Host.TE_GetModules(modules))
                {
                    for (int i = 0; i < modules.Count; i++)
                    {
                        string cur = modules[i].szModule;
                        if (cur.Length > 8)
                            cur = cur.Remove(8);
                        if (cur.ToLowerInvariant() == mod)
                        {
                            return callCommand(DoGMI, Helper.rul2hexstr((rulong)modules[i].modBaseAddr), args[1]);
                        }
                    }
                }
                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        private bool DoGMEMI(string[] args)
        {
            rulong addr;
            string val;

            if (args.Length == 2 && GetRulong(args[0], out addr))
            {
                variables["$RESULT"] = Var.Create(0);

                MEMORY_BASIC_INFORMATION MemInfo;
                if (Host.TE_GetMemoryInfo(addr, out MemInfo))
                {
                    val = args[1].ToUpperInvariant();

                    if (val == "MEMORYBASE") variables["$RESULT"] = Var.Create((rulong)MemInfo.BaseAddress);
                    else if (val == "MEMORYSIZE") variables["$RESULT"] = Var.Create(MemInfo.RegionSize);
                    else if (val == "MEMORYOWNER") variables["$RESULT"] = Var.Create((rulong)MemInfo.AllocationBase);
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
        private bool DoNAMES(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
            {
                return true;
            }
            return false;
        }

        private bool DoGMEXP(string[] args)
        {
            rulong addr, num = 0;
            if (args.Length >= 2 && args.Length <= 3 && GetRulong(args[0], out addr))
            {
                if (args.Length == 3 && !GetRulong(args[2], out num))
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

        private bool DoGMI(string[] args)
        {
            throw new NotImplementedException();
#if LATER
rulong addr;
string str;

	if(args.Length == 2 && GetRulong(args[0],  out addr))
	{
		List<MODULEENTRY32> Modules;
		MODULEENTRY32 Module;
		rulong ModBase = 0;

		variables["$RESULT"] = Var.Create(0);

		if(Host.TE_GetModules(Modules))
		{
			for(int i = 0; i < Modules.Count; i++)
			{
				Module = Modules[i];
				if(addr >= (rulong)Module.modBaseAddr && addr < ((rulong)Module.modBaseAddr + Module.modBaseSize))
				{
					ModBase = (rulong)Module.modBaseAddr;
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

        private bool DoGMIMP(string[] args)
        {
            rulong addr, num = 0;
            string str;

            if (args.Length >= 2 && args.Length <= 3 && GetRulong(args[0], out addr))
            {
                if (args.Length == 3 && !GetRulong(args[2], out num))
                    return false;

                errorstr = "Unsupported command!";

                variables["$RESULT"] = Var.Create(0);

                str = (args[1]).ToUpperInvariant();
                return false;

                /*
                rulong i, args.Length=0;
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

        private bool DoGN(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
            {
                string pAPI = Importer.GetAPIName(addr);
                string pDLL = Importer.GetDLLName(addr);
                if (pAPI != null && pDLL != null && pAPI[0] != 0 && pDLL[0] != 0)
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

        private bool DoGO(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
            {
                Debugger.SetBPX(addr, Ue.UE_SINGLESHOOT, SoftwareCallback);
                bInternalBP = true;
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoGOPI(string[] args)
        {
            rulong addr, index;
            string param;

            if (args.Length == 3 && GetRulong(args[0], out addr) && GetRulong(args[1], out index))
            {
                if (index < 1 || index > 3)
                {
                    errorstr = "Bad operand index (1-3)";
                    return false;
                }

                index--;

                param = Helper.toupper(args[2]);

                errorstr = "Unsupported command!";
                return false;

                /*
		
                rulong size;

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

        private bool DoGPA(string[] args)
        {
            string proc, lib;
            bool dofree;

            if (args.Length >= 2 && args.Length <= 3 && GetString(args[0], out proc) && GetString(args[1], out lib))
            {
                if (args.Length == 3 && !GetBool(args[2], out dofree))
                    return false;

                rulong addr = Importer.GetRemoteAPIAddressEx(lib, proc);
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

        private bool DoGPI(string[] args)
        {
            if (args.Length != 1)
                return false;
            var str = args[0].ToUpperInvariant();

            if (str == "HPROCESS") // Handle of debugged process 
            {
                variables["$RESULT"] = Var.Create((rulong)Host.TE_GetProcessHandle());
            }
            else if (str == "PROCESSID") // Process ID of debugged process 
            {
                variables["$RESULT"] = Var.Create(Host.TE_GetProcessId());
            }
            else if (str == "HMAINTHREAD") // Handle of main thread of debugged process 
            {
                variables["$RESULT"] = Var.Create((rulong)Host.TE_GetMainThreadHandle());
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
                string name = Path.GetFileNameWithoutExtension(Host.TE_GetTargetPath());
                variables["$RESULT"] = Var.Create(name);
            }
            else if (str == "EXEFILENAME") // Full path of the debugged file/dll
            {
                variables["$RESULT"] = Var.Create(Host.TE_GetTargetPath());
            }
            else if (str == "CURRENTDIR") // Current directory for debugged process (with trailing '\')
            {
                variables["$RESULT"] = Var.Create(Host.TE_GetTargetDirectory());
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
        private bool DoGPP(string[] args)
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
        private bool DoGREF(string[] args)
        {
            rulong line;

            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 1 && !GetRulong(args[0], out line))
                    return false;

                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoGRO(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
            {
                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoGSL(string[] args)
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

        private bool DoGSTR(string[] args)
        {
            rulong addr, size = 2;

            if (args.Length >= 1 && args.Length <= 2 && GetRulong(args[0], out addr))
            {
                if (args.Length == 2 && !GetRulong(args[1], out size))
                    return false;

                variables["$RESULT"] = variables["$RESULT_1"] = Var.Create(0);

                MEMORY_BASIC_INFORMATION MemInfo;
                if (Host.TE_GetMemoryInfo(addr, out MemInfo))
                {
                    rulong memsize = (rulong)MemInfo.BaseAddress + MemInfo.RegionSize - addr;

                    byte[] buffer;

                    buffer = new byte[memsize + 1];

                    buffer[0] = buffer[memsize] = 0;

                    if (Host.TryReadBytes(addr, memsize, buffer))
                    {
                        rulong strsize = (uint)buffer.Length;
                        if (strsize != 0 && strsize >= size && strsize < memsize)
                        {
                            variables["$RESULT"] = Var.Create(buffer.ToString());       //$BUGBUG! stringize buffer first.
                            variables["$RESULT_1"] = Var.Create(strsize);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private bool DoHANDLE(string[] args)
        {
            rulong x, y;
            string sClassName;

            if (args.Length == 3 && GetRulong(args[0], out x) && GetRulong(args[1], out y) && GetString(args[2], out sClassName))
            {
                variables["$RESULT"] = Var.Create((rulong)Host.FindHandle(Host.TE_GetMainThreadId(), sClassName, x, y));
                return true;
            }
            return false;
        }

        private bool DoINC(string[] args)
        {
            rulong dw;
            double flt;

            if (args.Length == 1)
            {
                if (GetRulong(args[0], out dw))
                {
                    dw++;
                    return SetRulong(args[0], dw);
                }
                else if (GetFloat(args[0], out flt))
                {
                    flt++;
                    return SetFloat(args[0], flt);
                }
            }
            return false;
        }

        // Olly only
        private bool DoHISTORY(string[] args)
        {
            rulong dw;

            if (args.Length == 1 && GetRulong(args[0], out dw))
            {
                return true;
            }
            return false;
        }

        private bool DoITOA(string[] args)
        {
            rulong dw;
            rulong @base = 16;

            if (args.Length >= 1 && args.Length <= 2 && GetRulong(args[0], out dw))
            {
                if (args.Length == 2 && !GetRulong(args[1], out @base))
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

        private bool DoJA(string[] args)
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

        private bool DoJAE(string[] args)
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

        private bool DoJB(string[] args)
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

        private bool DoJBE(string[] args)
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

        private bool DoJE(string[] args)
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

        private bool DoJMP(string[] args)
        {
            if (args.Length == 1 && script.IsLabel(args[0]))
            {
                script_pos_next = script.Labels[args[0]];
                return true;
            }
            return false;
        }

        private bool DoJNE(string[] args)
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
        private bool DoKEY(string[] args)
        {
            rulong key;
            bool shift = false, ctrl = false;

            if (args.Length >= 1 && args.Length <= 3 && GetRulong(args[0], out key))
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
        private bool DoLBL(string[] args)
        {
            rulong addr;
            string lbl;

            if (args.Length == 2 && GetRulong(args[0], out addr) && GetString(args[1], out lbl))
            {
                return true;
            }
            return false;
        }

        private bool DoLM(string[] args)
        {
            throw new NotImplementedException();
#if LATER
rulong addr, size;
string filename;

    if(args.Length == 3 && GetRulong(args[0],  out addr) && GetRulong(args[1],  out size) && GetString(args[2],  out filename))
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
        private bool DoLC(string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoLCLR(string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoLEN(string[] args)
        {
            string str;

            if (args.Length == 1 && GetString(args[0], out str))
            {
                variables["$RESULT"] = Var.Create(str.Length);
                return true;
            }
            return false;
        }

        private bool DoLOADLIB(string[] args)
        {
            string str;

            if (args.Length == 1 && GetString(args[0], out str))
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
                rulong ip = GetContextData(UE_CIP);
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
                 */
                {
                    /*
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
                 */
                    //require_addonaction = true;
                    //back_to_debugloop = true;
                    return true;
                }

            }
            return false;
        }

        private bool DoLOG(string[] args)
        {
            string prefix = "";

            if (args.Length >= 1 && args.Length <= 2)
            {
                if (args.Length == 2 && !GetString(args[1], out prefix))
                    return false;

                bool noprefix = (args.Length == 1);

                rulong dw;
                string str;
                double flt;

                if (noprefix && is_writable(args[0]))
                    prefix = args[0] + ": ";

                string @out;

                if (GetRulong(args[0], out dw))
                {
                    @out = Helper.toupper(Helper.rul2hexstr(dw, sizeof(rulong) * 2));
                }
                else if (GetFloat(args[0], out flt))
                {
                    @out = Helper.dbl2str(flt);
                }
                else if (GetString(args[0], out str))
                {
                    @out = Helper.IsHexLiteral(str) ? str : Helper.CleanString(str);
                }
                else return false;

                Host.TE_Log((prefix + @out));
                return true;
            }
            return false;
        }

        private bool DoLOGBUF(string[] args)
        {
            string sep = " ";
            rulong dw = 0;

            if (args.Length >= 1 && args.Length <= 3 && IsVariable(args[0]))
            {
                switch (args.Length)
                {
                case 3:
                    if (!GetString(args[2], out sep)) return false;
                    goto case 2;
                case 2:
                    if (!GetRulong(args[1], out dw)) return false;
                    break;
                }

                if (0 == dw)
                    dw = 16;

                Var v = variables[args[0]];

                string line = "";
                string data = v.to_bytes();

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

        private bool DoMEMCPY(string[] args)
        {
            if (args.Length == 3)
            {
                return callCommand(DoMOV, ('[' + args[0] + ']'), ('[' + args[1] + ']'), args[2]);
            }
            return false;
        }

        /// <summary>
        /// Implements MOV [dst],[src](,[maxsize])
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool DoMOV(string[] args)
        {
            rulong maxsize = 0;

            if (args.Length >= 2 && args.Length <= 3)
            {
                if (args.Length == 3 && !GetRulong(args[2], out maxsize))
                    return false;

                rulong dw;
                string str;
                double flt;

                if (!is_writable(args[0]) && !DoVAR(args[0]))
                {
                    return false;
                }

                register_t reg;

                // Check destination
                if (IsVariable(args[0]))
                {
                    Var v;
                    if (maxsize > sizeof(rulong) && Helper.IsMemoryAccess(args[1])) // byte string
                    {
                        string tmp = Helper.UnquoteString(args[1], '[', ']');
                        rulong src;
                        if (GetRulong(tmp, out src))
                        {
                            Debug.Assert(src != 0);
                            byte[] bytes = new byte[maxsize];
                            if (!Host.TryReadBytes(src, maxsize, bytes))
                                return false;
                            v = Var.Create("#" + Helper.bytes2hexstr(bytes, (int)maxsize) + '#');
                        }
                        else
                            v = Var.Create(0);
                    }
                    else if (maxsize <= sizeof(rulong) && GetRulong(args[1], out dw)) // rulong
                    {
                        // DW to DW/FLT var
                        if (maxsize == 0)
                            maxsize = sizeof(rulong);
                        // dw = Helper.resize(dw, (int)maxsize);
                        v = Var.Create(dw);
                        v.size = (int)maxsize;
                    }
                    else if (GetString(args[1], (int)maxsize, out str)) // string
                    {
                        v = Var.Create(str);
                    }
                    else if (GetFloat(args[1], out flt)) // float
                    {
                        v = Var.Create(flt);
                    }
                    else
                        return false;

                    variables[args[0]] = v;
                    return true;
                }
                else if ((reg = find_register(args[0])) != null)
                {
                    // Dest is register
                    if (GetRulong(args[1], out dw))
                    {
                        if (maxsize == 0)
                            maxsize = reg.size;
                        dw = Helper.resize(dw, Math.Min((int)maxsize, reg.size));
                        if (reg.size < sizeof(rulong))
                        {
                            //rulong oldval, newval;
                            rulong oldval = Debugger.GetContextData(reg.id);
                            throw new NotImplementedException("oldval &= ~(((1 << (reg.size * 8)) - 1) << (reg.offset * 8));");
                            //newval = resize(dw, reg.size) << (reg.offset * 8);
                            //dw = oldval | newval;
                        }
                        return Debugger.SetContextData(reg.id, dw);
                    }
                }
                else if (is_flag(args[0]))
                {
                    bool flagval;

                    if (GetBool(args[1], out flagval))
                    {
                        eflags_t flags = new eflags_t();
                        flags.dw = Debugger.GetContextData(eContextData.UE_EFLAGS);

                        switch (args[0][1])
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
                else if (is_floatreg(args[0]))
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
                else if (Helper.IsMemoryAccess(args[0]))
                {
                    string tmp = Helper.UnquoteString(args[0], '[', ']');

                    rulong target;
                    if (GetRulong(tmp, out target))
                    {
                        Debug.Assert(target != 0);

                        if (maxsize > sizeof(rulong) && Helper.IsMemoryAccess(args[1]))
                        {
                            tmp = Helper.UnquoteString(args[1], '[', ']');

                            rulong src;
                            if (GetRulong(tmp, out src))
                            {
                                Debug.Assert(src != 0);

                                byte[] copybuffer;

                                copybuffer = new byte[maxsize];
                                errorstr = "Out of memory!";

                                if (!Host.TryReadBytes(src, maxsize, copybuffer) || !Host.WriteMemory(target, (int)maxsize, copybuffer))
                                {
                                    return false;
                                }
                                return true;
                            }
                        }
                        else if (maxsize <= sizeof(rulong) && GetRulong(args[1], out dw))
                        {
                            if (maxsize == 0)
                                maxsize = sizeof(rulong);
                            return Host.WriteMemory(target, dw);
                        }
                        else if (GetString(args[1], (int)maxsize, out str))
                        {
                            Var v = Var.Create(str);
                            if (maxsize == 0)
                                maxsize = (rulong)v.size;
                            maxsize = Math.Min(maxsize, (rulong)v.size);
                            return Host.WriteMemory(target, (int)maxsize, Encoding.UTF8.GetBytes(v.to_string()));
                        }
                        else if (GetFloat(args[1], out flt))
                        {
                            return Host.WriteMemory(target, flt);
                        }
                    }
                }
            }
            return false;
        }

        private bool DoMSG(string[] args)
        {
            string msg;
            if (args.Length == 1 && GetAnyValue(args[0], out msg))
            {
                int input;
                if (Host.DialogMSG(msg, out input))
                {
                    if (input == -1) // IDCANCEL)
                        return Pause();
                    return true;
                }
            }
            return false;
        }

        private bool DoMSGYN(string[] args)
        {
            string msg;

            if (args.Length == 1 && GetString(args[0], out msg))
            {
                DialogResult input;
                if (Host.DialogMSGYN(msg, out input))
                {
                    switch (input)
                    {
                    case DialogResult.Cancel:
                        variables["$RESULT"] = Var.Create(2);
                        return Pause();
                    case DialogResult.OK:
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

        private bool DoOLLY(string[] args)
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

        private bool DoOR(string[] args)
        {
            rulong dw1, dw2;

            if (args.Length == 2 && GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
            {
                return SetRulong(args[0], dw1 | dw2);
            }
            return false;
        }

        private bool DoMUL(string[] args)
        {
            rulong dw1, dw2;
            double flt1, flt2;

            if (args.Length == 2)
            {
                if (GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
                {
                    return SetRulong(args[0], dw1 * dw2);
                }
                else if (GetFloat(args[0], out flt1) && GetFloat(args[1], out flt2))
                {
                    return SetFloat(args[0], flt1 * flt2);
                }
                else if (GetFloat(args[0], out  flt1) && GetRulong(args[1], out dw2))
                {
                    return SetFloat(args[0], flt1 * dw2);
                }
                else if (GetRulong(args[0], out dw1) && GetFloat(args[1], out flt2))
                {
                    return SetFloat(args[0], dw1 * flt2);
                }
            }
            return false;
        }

        private bool DoNEG(string[] args)
        {
            rulong dw;
            double flt;

            if (args.Length == 1)
            {
                if (GetRulong(args[0], out dw))
                {
                    return SetRulong(args[0], (rulong)(-(long)dw));
                }
                else if (GetFloat(args[0], out flt))
                {
                    return SetFloat(args[0], -flt);
                }
            }
            return false;
        }

        private bool DoNOT(string[] args)
        {
            rulong dw;

            if (args.Length == 1 && GetRulong(args[0], out  dw))
            {
                return SetRulong(args[0], ~dw);
            }
            return false;
        }

        //see also GCI
        private bool DoOPCODE(string[] args)
        {
            rulong addr;

            if (args.Length == 1 && GetRulong(args[0], out addr))
            {
                byte[] buffer = new byte[MAX_INSTR_SIZE];

                variables["$RESULT"] = variables["$RESULT_1"] = variables["$RESULT_2"] = Var.Create(0);

                if (Host.TryReadBytes(addr, (rulong)buffer.Length, buffer))
                {
                    int opsize;
                    string opstring = Host.Disassemble(buffer, addr, out opsize);
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
        private bool DoOPENDUMP(string[] args)
        {
            rulong addr, @base, size;

            if (args.Length >= 1 && args.Length <= 3 && GetRulong(args[0], out addr))
            {
                switch (args.Length)
                {
                case 3:
                    if (!GetRulong(args[2], out size)) return false;
                    goto case 2;
                case 2:
                    if (!GetRulong(args[1], out @base)) return false;
                    break;
                }

                variables["$RESULT"] = Var.Create(0);
                return true;
            }
            return false;
        }

        // Olly only
        private bool DoOPENTRACE(string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoPAUSE(string[] args)
        {
            if (args.Length == 0)
            {
                return Pause();
            }
            return false;
        }

        private bool DoPOP(string[] args)
        {
            rulong dw;

            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 1 && !GetRulong(args[0], out dw))
                    return false;

                rulong CSP = Debugger.GetContextData(eContextData.UE_CSP);
                Debugger.SetContextData(eContextData.UE_CSP, CSP + sizeof(rulong));
                if (args.Length == 1)
                {
                    var ea = Address.Ptr32((uint)CSP);
                    ImageSegment segment;
                    if (!Host.SegmentMap.TryFindSegment(ea, out segment))
                        throw new AccessViolationException();
                    dw = segment.MemoryArea.ReadLeUInt32(ea);
                    return SetRulong(args[0], dw);
                }
                return true;
            }
            return false;
        }

        private bool DoPOPA(string[] args)
        {
            if (args.Length == 0)
            {
                return RestoreRegisters(true);
            }
            return false;
        }

        private bool DoPREOP(string[] args)
        {
            rulong addr = 0;

            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 1 && !GetRulong(args[0], out addr))
                    return false;
                else if (args.Length == 0)
                    addr = Debugger.GetContextData(eContextData.UE_CIP);

                variables["$RESULT"] = Var.Create(0);

                int prevsize = Host.LengthDisassembleBackEx(addr);
                if (prevsize != 0)
                {
                    variables["$RESULT"] = Var.Create(addr - (rulong)prevsize);
                }
                return true;
            }
            return false;
        }

        private bool DoPUSH(string[] args)
        {
            rulong dw;

            if (args.Length == 1 && GetRulong(args[0], out dw))
            {
                rulong CSP = Debugger.GetContextData(eContextData.UE_CSP) - sizeof(rulong);
                Debugger.SetContextData(eContextData.UE_CSP, CSP);
                return Host.WriteMemory(CSP, dw); ;
            }
            return false;
        }

        private bool DoPUSHA(string[] args)
        {
            if (args.Length == 0)
            {
                return SaveRegisters(true);
            }
            return false;
        }

        private bool DoREADSTR(string[] args)
        {
            rulong maxsize;
            string str;

            if (args.Length == 2 && GetRulong(args[1], out maxsize) && GetString(args[0], (int)maxsize, out str))
            {
                variables["$RESULT"] = Var.Create(str);
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
        private bool DoRBP(string[] args)
        {
            if (args.Length >= 0 && args.Length <= 1)
            {
                if (args.Length == 1)
                {
                    if (args[0].ToUpperInvariant() != "STRICT")
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
            */
            if ( saved_bp != 0)
            {
                /*
                bpt = ( t_table * ) Plugingetvalue ( VAL_BREAKPOINTS );
                if ( bpt != null )
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
        private bool DoREF(string[] args)
        {
            rulong addr;
            string str = "MEMORY";

            if (args.Length >= 1 && args.Length <= 2 && GetRulong(args[0], out addr))
            {
                if (args.Length == 2)
                    str = args[1];

                string[] valid_commands = { "MEMORY", "CODE", "MODULE" };

                variables["$RESULT"] = Var.Create(0);
                variables["$RESULT_1"] = Var.Create(0);
                variables["$RESULT_2"] = Var.Create(0);

                return valid_commands.Contains(str, StringComparer.InvariantCultureIgnoreCase);

            }
            return false;
        }

        // Olly only
        private bool DoREFRESH(string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoREPL(string[] args)
        {
            rulong addr;
            string v1, v2;
            rulong len = 0;

            if (args.Length >= 3 && args.Length <= 4 && GetRulong(args[0], out addr) && Helper.IsHexLiteral(args[1]) && Helper.IsHexLiteral(args[2]))
            {
                if (args.Length == 4 && !GetRulong(args[3], out len))
                    return false;
                else if (args.Length == 3)
                {
                    MEMORY_BASIC_INFORMATION MemInfo;
                    if (!Host.TE_GetMemoryInfo(addr, out MemInfo))
                        return true;

                    len = (rulong)MemInfo.BaseAddress + MemInfo.RegionSize - addr;
                }

                v1 = Helper.UnquoteString(args[1], '#');
                v2 = Helper.UnquoteString(args[2], '#');

                int oplen = v1.Length;

                if (oplen != v2.Length)
                {
                    errorstr = "Hex strings must have the same size!";
                    return false;
                }

                byte[] membuf = null, mask1 = null, mask2 = null, bytes1 = null, bytes2;

                membuf = new byte[len];
                if (Host.TryReadBytes(addr, len, membuf))
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
        private bool DoRESET(string[] args)
        {
            if (args.Length == 0)
            {
                //Sendshortcut(PM_MAIN, 0, WM_KEYDOWN, 1, 0, VK_F2); 
                return true;
            }
            return false;
        }

        private bool DoRET(string[] args)
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
                                if (callCommand(DoMOV, "$TEMP", args[0]))
                                {
                                    callback_return = variables["$TEMP"];
                                    variables.Remove("$TEMP");

                                    if (callback_return.type == Var.etype.EMP || callback_return.type == callbacks.Last().return_type)
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

        private bool DoREV(string[] args)
        {
            rulong dw;
            string str;

            if (args.Length == 1)
            {
                if (GetRulong(args[0], out dw))
                {
                    variables["$RESULT"] = Var.Create(Helper.reverse(dw));
                    return true;
                }
                else if (GetString(args[0], out str))
                {
                    Var tmp = Var.Create(str);
                    variables["$RESULT"] = tmp.reverse();
                    return true;
                }
            }
            return false;
        }

        private bool DoROL(string[] args)
        {
            throw new NotImplementedException();
#if LATER
rulong dw1, dw2;

	if(args.Length == 2 && GetRulong(args[0],  out dw1) && GetRulong(args[1],  out dw2))
	{
		return SetRulong(args[0],  rol(dw1, out dw2));
	}
	return false;
#endif
        }

        private bool DoROR(string[] args)
        {
            throw new NotImplementedException();
#if LATER
    rulong dw1, dw2;

	if(args.Length == 2 && GetRulong(args[0],  out dw1) && GetRulong(args[1],  out dw2))
	{
		return SetRulong(args[0],  ror(dw1, out dw2));
	}
	return false;
#endif
        }

        private bool DoRTR(string[] args)
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

        private bool DoRTU(string[] args)
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

        private bool DoRUN(string[] args)
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

        private bool DoSBP(string[] args)
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
                if ( bpt != null )
                {
                    bpoint = ( t_bpoint * ) ( bpt.data.data );
                    if ( bpoint != null )
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

        private bool DoSCMP(string[] args)
        {
            string s1, s2;
            rulong size = 0;

            if (args.Length >= 2 && args.Length <= 3)
            {
                if (args.Length == 3 && !GetRulong(args[2], out size))
                    return false;

                if (GetString(args[0], (int)size, out s1) && GetString(args[1], (int)size, out s2))
                {
                    SetCMPFlags(s1.CompareTo(s2));
                    return true;
                }
            }
            return false;
        }

        private bool DoSCMPI(string[] args)
        {
            if (args.Length >= 2 && args.Length <= 3)
            {
                switch (args.Length)
                {
                case 2:
                    return callCommand(DoSCMP, (args[0]).ToLowerInvariant(), (args[1]).ToLowerInvariant());
                case 3:
                    return callCommand(DoSCMP, (args[0]).ToLowerInvariant(), (args[1]).ToLowerInvariant(), args[2]);
                }
            }
            return false;
        }

        // Olly only
        private bool DoSETOPTION(string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoSHL(string[] args)
        {
            rulong dw1, dw2;

            if (args.Length == 2 && GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
            {
                return SetRulong(args[0], dw1 << (int)dw2);
            }
            return false;
        }

        private bool DoSHR(string[] args)
        {
            rulong dw1, dw2;

            if (args.Length == 2 && GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
            {
                return SetRulong(args[0], dw1 >> (int)dw2);
            }
            return false;
        }

        private bool DoSTI(params string[] args)
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

        private bool DoSTO(string[] args)
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

        private bool DoSTR(string[] args)
        {
            if (args.Length == 1 && IsVariable(args[0]))
            {
                Var v = variables[args[0]];
                switch (v.type)
                {
                case Var.etype.DW: // empty buf + dw ->  buf
                    v = Var.Create("##") + v.ToUInt64();
                    goto case Var.etype.STR;
                case Var.etype.STR:
                    if (v.IsBuf)
                        v = Var.Create(v.to_string());
                    return true;
                }
            }
            return false;
        }

        private bool DoSUB(string[] args)
        {
            rulong dw1, dw2;
            double flt1, flt2;

            if (args.Length == 2 && GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
            {
                if (GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
                {
                    return SetRulong(args[0], dw1 - dw2);
                }
                else if (GetFloat(args[0], out flt1) && GetFloat(args[1], out flt2))
                {
                    return SetFloat(args[0], flt1 - flt2);
                }
            }
            return false;
        }

        // Olly only
        private bool DoTC(string[] args)
        {
            if (args.Length == 0)
            {
                return true;
            }
            return false;
        }

        private bool DoTEST(string[] args)
        {
            rulong dw1, dw2;

            if (args.Length == 2 && GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
            {
                zf = ((dw1 & dw2) == 0);
                return true;
            }
            return false;
        }

        private bool DoTI(string[] args)
        {
            if (args.Length == 0)
            {
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        private bool DoTICK(string[] args)
        {
            rulong timeref = 0;

            if (args.Length >= 0 && args.Length <= 2)
            {
                if (args.Length == 2 && !GetRulong(args[1], out timeref))
                    return false;

                rulong tickcount = Helper.MyTickCount() - tickcount_startup;

                if (args.Length == 0)
                {
                    variables["$RESULT"] = Var.Create(Helper.rul2decstr(tickcount / 1000) + " ms");
                    return true;
                }
                else if (IsVariable(args[0]) || DoVAR(args[0]))
                {
                    variables[args[0]] = Var.Create((uint)tickcount);
                    if (args.Length == 2)
                        variables["$RESULT"] = Var.Create((uint)(tickcount - timeref));
                    return true;
                }
            }
            return false;
        }

        // TE?
        private bool DoTICND(string[] args)
        {
            string condition;

            if (args.Length == 1 && GetString(args[0], out condition))
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
                    if(pthr != null)
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

        private bool DoTO(string[] args)
        {
            if (args.Length == 0)
            {
                resumeDebuggee = true;
                return true;
            }
            return false;
        }

        // TE?
        private bool DoTOCND(string[] args)
        {
            string condition;

            if (args.Length == 1 && GetString(args[0], out condition))
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
                    if(pthr != null)
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
        private bool DoUNICODE(string[] args)
        {
            bool enable;

            if (args.Length == 1 && GetBool(args[0], out enable))
            {
                return true;
            }
            return false;
        }

        private bool DoVAR(params string[] args)
        {
            if (args.Length == 1)
            {
                if (is_valid_variable_name(args[0]))
                {
                    variables[args[0]] = Var.Create(0);
                    return true;
                }
                errorstr = "Bad variable name: " + args[0];
            }
            return false;
        }

        private bool DoXCHG(string[] args)
        {
            rulong dw1, dw2;
            double flt1, flt2;

            if (args.Length == 2)
            {
                if (GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
                {
                    return SetRulong(args[0], dw2) && SetRulong(args[1], dw1);
                }
                else if (GetFloat(args[0], out flt1) && GetFloat(args[1], out flt2))
                {
                    return SetFloat(args[0], flt2) && SetFloat(args[1], flt1);
                }
                else if (IsVariable(args[0]) && IsVariable(args[1]))
                {
                    Var tmp = variables[args[0]];
                    variables[args[0]] = variables[args[1]];
                    variables[args[1]] = tmp;
                    return true;
                }
            }
            return false;
        }


        private bool DoXOR(string[] args)
        {
            rulong dw1, dw2;

            if (args.Length == 2 && GetRulong(args[0], out dw1) && GetRulong(args[1], out dw2))
            {
                return SetRulong(args[0], dw1 ^ dw2);
            }
            return false;
        }

        private bool DoWRT(string[] args)
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

        private bool DoWRTA(string[] args)
        {
            string filename, data, @out = "\r\n";

            if (args.Length >= 2 && args.Length <= 3 && GetString(args[0], out filename) && GetAnyValue(args[1], out data))
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
    }
}