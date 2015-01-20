// ODbgScript.cpp : Defines the entry point for the DLL application.
//
//#define STRICT

#define _CHAR_UNSIGNED 1 // huhu

#include "StdAfx.h"

//static HINSTANCE hinst;                // DLL instance
//static HWND      hwmain;               // Handle of main OllyDbg window

// Temp storage
char buff[65535] = {0};

// Script state
int script_state = SS_NONE;

#ifndef __AFX_H__
// Entry point into a plugin DLL. Many system calls require DLL instance
// which is passed to DllEntryPoint() as one of parameters. Remember it.
BOOL APIENTRY DllMain(HINSTANCE hi,DWORD reason,LPVOID reserved)
{
    switch (reason)
	{
		case DLL_PROCESS_ATTACH:
			//hinst = hi;        // Mark plugin instance
			//no more used (bug with global vars)
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
		case DLL_PROCESS_DETACH:
			break;
    }

	#ifdef _DEBUG
	//RedirectIOToConsole();
	#endif

    return TRUE;
}
#endif

// Report plugin name and return version of plugin interface.
extc int _export cdecl ODBG_Plugindata(char shortname[32]) 
{
	strcpy(shortname, "ODbgScript");
	return PLUGIN_VERSION;
}

// OllyDbg calls this obligatory function once during startup. I place all
// one-time initializations here. Parameter features is reserved for future
// extentions, do not use it.
extc int _export cdecl ODBG_Plugininit(int ollydbgversion, HWND hw, ulong *features) 
{
	HINSTANCE hinst;

	if(ollydbgversion < PLUGIN_VERSION) {
		MessageBox(hwndOllyDbg(), TEXT("Incompatible Ollydbg Version !"), TEXT("ODbgScript"), MB_OK | MB_ICONERROR | MB_TOPMOST);
		return -1;
	}

	// Report plugin in the log window.
	Addtolist(0, 0, "ODbgScript v%i.%i.%i " VERSIONCOMPILED ,VERSIONHI,VERSIONLO,VERSIONST);
	Addtolist(0, -1, "  http://odbgscript.sf.net");
	ollylang = new OllyLang();

	if (Createsorteddata(&ollylang->wndProg.data,"ODbgScript Data", 
		sizeof(t_wndprog_data),50, (SORTFUNC *)wndprog_sort_function,NULL)!=0)
			return -1; 

	if (Createsorteddata(&ollylang->wndLog.data,"ODbgScript Log", 
	sizeof(t_wndlog_data),20, (SORTFUNC *)wndlog_sort_function,NULL)!=0)
		return -1;

	hinst = hinstModule();

	if (Registerpluginclass(wndprogclass,NULL,hinst,wndprog_winproc)<0) {
		return -1;
	}
	if (Registerpluginclass(wndlogclass,NULL,hinst,wndlog_winproc)<0) {
		return -1;
	}
	if (Plugingetvalue(VAL_RESTOREWINDOWPOS)!=0 && Pluginreadintfromini(hinst,"Restore Script Log",0)!=0)
		initLogWindow();

	if (Plugingetvalue(VAL_RESTOREWINDOWPOS)!=0 && Pluginreadintfromini(hinst,"Restore Script window",0)!=0)
		initProgTable();

	return 0;
}

// This function is called each time OllyDbg passes main Windows loop. When
// debugged application stops, b ring command line window in foreground.
extc void _export cdecl ODBG_Pluginmainloop(DEBUG_EVENT *debugevent) 
{	
	t_status status; 
	status = Getstatus();
	script_state = ollylang->script_state;
	
    // module load event. kept for future use. http://www.openrce.org/articles/full_view/25
    /*if (debugevent && debugevent->dwDebugEventCode == LOAD_DLL_DEBUG_EVENT) {
		string filename;
		if (str_filename_from_handle(debugevent->u.LoadDll.hFile, filename)) {
			MsgBox(filename,""); 
		}
	}
	*/

	if (debugevent && debugevent->dwDebugEventCode == OUTPUT_DEBUG_STRING_EVENT && debugevent->u.DebugString.nDebugStringLength>0
		&& !IsBadCodePtr((FARPROC)debugevent->u.DebugString.lpDebugStringData))
		MsgBox(debugevent->u.DebugString.lpDebugStringData,"");

	// Check for breakpoint jumps
	if(script_state == SS_RUNNING && debugevent && debugevent->dwDebugEventCode == EXCEPTION_DEBUG_EVENT)
	{

		EXCEPTION_DEBUG_INFO edi = debugevent->u.Exception;
		if(edi.ExceptionRecord.ExceptionCode != EXCEPTION_SINGLE_STEP)
			ollylang->OnException(edi.ExceptionRecord.ExceptionCode);
		else if(edi.ExceptionRecord.ExceptionCode == EXCEPTION_BREAKPOINT)
			ollylang->OnBreakpoint(PP_EXCEPTION,EXCEPTION_DEBUG_EVENT);
/*		else	
			if(script_state == SS_RUNNING)
			{
				t_thread* t;
				t = Findthread(Getcputhreadid());
				CONTEXT context;
				context.ContextFlags = CONTEXT_DEBUG_REGISTERS;
				GetThreadContext(t->thread, &context);

				//Hardware Breakpoints...
				if(t->reg.ip == context.Dr0 || t->reg.ip == context.Dr1 || t->reg.ip == context.Dr2 || t->reg.ip == context.Dr3) {
					ollylang->OnBreakpoint(PP_HWBREAK,t->reg.ip);
				}

			}
*/
	}

	if(status == STAT_STOPPED && (script_state == SS_RUNNING || script_state == SS_LOADED || script_state == SS_PAUSED))
	{

		if (ollylang->require_addonaction) {
			try
			{
				ollylang->ProcessAddonAction();
			}
			catch( ... )
			{
				MessageBox(hwndOllyDbg(), TEXT("An error occured in the plugin!\nPlease contact Epsylon3."), TEXT("ODbgScript"), MB_OK | MB_ICONERROR | MB_TOPMOST);
			}
		}

	}


	if(status == STAT_STOPPED && (script_state == SS_RUNNING || script_state == SS_LOADED))
	{

		try
		{
			ollylang->Step(0);
			script_state = ollylang->script_state;
		}
		catch( ... )
		{
			MessageBox(hwndOllyDbg(), TEXT("An error occured in the plugin!\nPlease contact Epsylon3."), TEXT("ODbgScript"), MB_OK | MB_ICONERROR | MB_TOPMOST);
			delete ollylang;
		}

	}

	//Refocus script windows (ex: when using "Step")
	if (    ollylang->wndProg.hw 
		&& (status == STAT_STOPPED || status == STAT_EVENT)
		&& (script_state != SS_RUNNING)
		) 
	{
		if (focusonstop>0) { 
//			InvalidateProgWindow();
			SetForegroundWindow(ollylang->wndProg.hw);
			SetFocus(ollylang->wndProg.hw);
			focusonstop--;
		}
	}	

}

extc int _export cdecl ODBG_Pausedex(int reasonex, int dummy, t_reg* reg, DEBUG_EVENT* debugevent)
{
	EXCEPTION_DEBUG_INFO edi;
	if(debugevent)
		edi = debugevent->u.Exception;

	script_state = ollylang->script_state;

//cout << hex << reasonex << endl;

	// Handle events
	if(script_state == SS_RUNNING || script_state == SS_PAUSED) 
	//PAUSED also TO PROCESS "BPGOTO" BREAKPOINTS
	{
		switch(reasonex) 
		{
		case PP_INT3BREAK:
		case PP_HWBREAK:
		case PP_MEMBREAK:
			ollylang->OnBreakpoint(reasonex,dummy);
			break;
		case PP_EXCEPTION:
		case PP_ACCESS:
		case PP_GUARDED:
		case PP_SINGLESTEP | PP_BYPROGRAM:
			ollylang->OnException(edi.ExceptionRecord.ExceptionCode);
			break;
		}
		if (ollylang->wndProg.hw) {
			Selectandscroll(&ollylang->wndProg,ollylang->pgr_scriptpos,2);
			InvalidateProgWindow();
		}
	}

	return 0;
}

// Function adds items to main OllyDbg menu (origin=PM_MAIN).
extc int _export cdecl ODBG_Pluginmenu(int origin, char data[4096], void *item) 
{
    t_dump *pd;

	switch (origin) {
		
	case PM_MAIN:
	//MRU Menu content, actions are 21..25
		ZeroMemory(buff, sizeof(buff));
		//strcpy(buff, "Run &Script{0 Open...|");
		strcpy(buff, "0 Run &Script...|");
		//mruGetMenu(&buff[strlen(buff)]);
		strcpy(&buff[strlen(buff)],
		//"}|"
/*		"1 Abort"
		",2 Pause"
		",3 Resume"
		",4 Step"
		"|"
*/		"30 Script &Window..."
		",31 &Log Window..."
		"|"
		"20 &Help"
		"|"
		"10 &About"
//		",11 TEST"
//		",12 TEST2"
		);

		strcpy(data,buff);
		return 1;

	case PM_DISASM:
	  pd=(t_dump *)item;
      if (pd==NULL || pd->size==0)
          return 0; 

		ZeroMemory(buff, sizeof(buff));
		strcpy(buff, "# Run Scri&pt{0 Open...|");
		mruGetCurrentMenu(&buff[strlen(buff)]);
 		strcpy(&buff[strlen(buff)],
			"}"
			"Script &Functions...{"
			"30  Script &Window\t"
			",31  Script &Log\t"
			"|"
			",4 Step\t"
			",2 Pause\tPAUSE"
			",3 Resume\t"
			",1 Abort\t"
			"|"
			",32 Edit Script..."
			"}"

		);
		
		strcpy(data,buff);
		return 1;

	}
	return 0; // No pop-up menus in OllyDbg's windows
}

extc int _export cdecl ExecuteScript(const char* const filename)
{
	ollylang->LoadScript((LPSTR)filename);
	ollylang->Step(0);
	return 0;
}

// Needed by OllyscriptEditor
extc HWND _export cdecl DebugScript(const char* const filename)
{
	if (filename!="")
	{
		ollylang->LoadScript((LPSTR)filename);
		ollylang->Pause();
		script_state = ollylang->script_state;
		initProgTable();
		SetForegroundWindow(ollylang->wndProg.hw);
		SetFocus(ollylang->wndProg.hw);     
	}
	return ollylang->wndProg.hw;
}

// Receives commands from windows menus.
extc void _export cdecl ODBG_Pluginaction(int origin, int action, void *item) 
{

  switch (origin)
  {
	case PM_MAIN:
	case PM_DISASM:
		break;
	default:
		//Other windows ignored
		return;
  }

  char s[256];
  HINSTANCE hinst  = hinstModule();
  HWND      hwmain = hwndOllyDbg();
  OPENFILENAME ofn={0};
  switch (action) 
  {
	case 0: // Run script
		       // common dialog box structure
		char szFile[260];       // buffer for file name
		
		// Initialize OPENFILENAME
		//ZeroMemory(&ofn, sizeof(ofn));
		ofn.lStructSize = sizeof(ofn);
		ofn.hwndOwner = hwmain;
		ofn.lpstrFile = szFile; 
		//
		// Set lpstrFile[0] to '\0' so that GetOpenFileName does not 
		// use the contents of szFile to initialize itself.
		//
		ofn.lpstrFile[0] = '\0';
		ofn.nMaxFile = sizeof(szFile);
		ofn.lpstrFilter = "Olly Scripts\0*.osc;*.txt\0All\0*.*\0";
		ofn.nFilterIndex = 1;
		ofn.lpstrFileTitle = NULL;
		ofn.nMaxFileTitle = 0;
		Pluginreadstringfromini(hinst, "ScriptDir", buff, 0);
		ofn.lpstrInitialDir = buff;
		ofn.lpstrTitle = "Select Script";
		ofn.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;
		

		// Display the Open dialog box. 
		if (GetOpenFileName(&ofn)==TRUE) //Comdlg32.lib
		{
			// Load script
			ollylang->LoadScript(ofn.lpstrFile);
			if (ollylang->wndProg.hw) {
				SetForegroundWindow(ollylang->wndProg.hw);
				SetFocus(ollylang->wndProg.hw);
			}
			// Start script
			ollylang->Resume();
		}
		break;

	case 1: // Abort
		MessageBox(hwmain, TEXT("Script aborted!"), TEXT("ODbgScript"), MB_OK|MB_ICONEXCLAMATION );
		ollylang->Reset(); 
		ollylang->Pause();
		break;

	case 2: // Pause
		ollylang->Pause();
		break;

	case 3: // Resume
		ollylang->Resume();
		break;

	case 4: // Step
		ollylang->Step(1);
		script_state = ollylang->script_state;
		break;

	case 5: // Force Pause (like Pause Key)
		focusonstop=4;
		ollylang->Pause();
		script_state = ollylang->script_state;
    	break;

	case 10:
		sprintf(s,"ODbgScript plugin v%i.%i.%i\n"
			      "by Epsylon3@gmail.com\n\n"
				  "From OllyScript written by SHaG\n"
				  "PE dumper by R@dier\n"
				  "Byte replacement algo by Hex\n\n"
				  "http://odbgscript.sf.net/ \n\n"
				  "Compiled %s %s \n"
			       VERSIONCOMPILED "\n",
			VERSIONHI,VERSIONLO,VERSIONST, __DATE__, __TIME__);
		MessageBox(hwmain, s, TEXT("ODbgScript"), MB_OK|MB_ICONINFORMATION );
		break;
	case 20: 
		{
			string directory, helpfile;
			getPluginDirectory(directory);
			helpfile = directory + "\\ODbgScript.txt";			
			ShellExecute(hwndOllyDbg(),"open",helpfile.c_str(),NULL,directory.c_str(),SW_SHOWDEFAULT);
		}
		break;
	case 21: // MRU List in CPU Window
	case 22:
	case 23:
	case 24:
	case 25:
	case 26:
	case 27:
	case 28:
	case 29:
		{
			action-=20; 
			char key[5]="NRU ";
			key[3]=action+0x30;
						
			ZeroMemory(&buff, sizeof(buff));
			Pluginreadstringfromini(hinst,key,buff,0);

			// Load script
			ollylang->LoadScript(buff);

			mruAddFile(buff);
 
			// Save script directory
			char* buf2;
			GetFullPathName(buff,sizeof(buff),buff,&buf2); *buf2=0;			
			Pluginwritestringtoini(hinst, "ScriptDir", buff);

			ollylang->Resume();
			if (ollylang->wndProg.hw) {
				SetForegroundWindow(ollylang->wndProg.hw);
				SetFocus(ollylang->wndProg.hw);
			}

			break;
		}
	case 30:
		{
			initProgTable();
			break;
		}
	case 31:
		{
			initLogWindow();
			break;
		}
	case 32: // Edit Script
		{
			ShellExecute(hwndOllyDbg(),"open",ollylang->scriptpath.c_str(),NULL,ollylang->currentdir.c_str(),SW_SHOWDEFAULT);
			break;
		}
	case 11:
		{
//			string x = "Hej";
//			string y = ToLower(x);
//			__asm nop;
		}
	case 12:
		{
//			Broadcast(WM_USER_CHALL, 0, 0);
		}
//			t_thread* thr = Findthread(Getcputhreadid());
//			byte buffer[4];
//			ulong fs = thr->reg.limit[2]; // BUG IN ODBG!!!
//			fs += 0x30;
//			Readmemory(buffer, fs, 4, MM_RESTORE);
//			fs = *((ulong*)buffer);
//			fs += 2;
//			buffer[0] = 0;
//			Writememory(buffer, fs, 1, MM_RESTORE);
//			cout << endl;
		
//			ulong addr = t->reg.s[SEG_FS];
//			Readmemory(buffer, addr, 4, MM_RESTORE);
//			cout << hex << &buffer;

			/*
			HMODULE hMod = GetModuleHandle("OllyScript.dll");
			if(hMod) // Check that the other plugin is present and loaded
			{
				// Get address of exported function
				int (*pFunc)(char*) = (int (*)(char*)) GetProcAddress(hMod, "ExecuteScript");
				if(pFunc) // Check that the other plugin exports the correct function
					pFunc("xxx"); // Execute exported function
			}

			cout << hex << hMod << endl;*/
			//403008 401035
			/*DWORD pid = Plugingetvalue(VAL_PROCESSID);
			DebugSetProcessKillOnExit(FALSE);
			DebugActiveProcessStop(pid);
			break;*/
			//t_module* mod = Findmodule(0x401000);
			//cout << hex << mod->codebase;
			
			//cout << hex << mod->codebase;
		
		break;

    default: 
		break;
  }
}

extc int ODBG_Pluginshortcut(int origin,int ctrl,int alt,int shift,int key,void *item) {

	switch (origin) {
	case PM_MAIN:
		if (key==VK_PAUSE) {
			//will pause when running on give focus to script window
			focusonstop=4;
			ollylang->Pause();
			script_state = ollylang->script_state;
		//	SetForegroundWindow(ollylang->wndProg.hw);
		//	SetFocus(ollylang->wndProg.hw);
		}
		break; //This function is usually called twice
	case PM_DISASM:

		break; 
/*
PM_MAIN	item is always NULL	Main window
PM_DUMP	(t_dump *)	Any Dump window
PM_MODULES	(t_module *)	Modules window
PM_MEMORY	(t_memory *)	Memory window
PM_THREADS	(t_thread *)	Threads window
PM_BREAKPOINTS	(t_bpoint *)	Breakpoints window
PM_REFERENCES	(t_ref *)	References window
PM_RTRACE	(int *)	Run trace window
PM_WATCHES	(1-based index)	Watches window
PM_WINDOWS	(t_window *)	Windows window
PM_DISASM	(t_dump *)	CPU Disassembler
PM_CPUDUMP	(t_dump *)	CPU Dump
PM_CPUSTACK	(t_dump *)	CPU Stack
PM_CPUREGS	(t_reg *)	CPU Registers
*/
	case PM_DUMP:
	{
		if (key==VK_F5) {
			//Used to retrieve t_dump after OPENDUMP
			t_dump * pd;
			pd=(t_dump *)item;
			if (pd && pd->table.hw != 0) {
				ollylang->dumpWindows[pd->table.hw] = pd;
			}
			return 1;
		}
	}
	default:
			//if (key==VK_F8 && shift==0 && ctrl==0) {
#ifdef _DEBUG
			char* data = new char[256];
			sprintf(data,"ODBG_Pluginshortcut %d %d",origin,key);
			Addtolist(0, -1, data );
			delete[] data;
			return 0;
#endif
;
	}
	return 0;

}

// OllyDbg calls this optional function when user restarts debugged app
extc void _export cdecl ODBG_Pluginreset()
{	
	//we keep the script state on restart (paused or not)
	if (ollylang->script_state == SS_PAUSED) {
		ollylang->Reset();
		ollylang->Pause();
	} 
	else 
		ollylang->Reset();
}

int ODBG_Plugincmd(int reason,t_reg *reg,char *cmd)
/*
OllyDbg will call it each time the debugged application pauses on conditional logging breakpoint that specifies commands to be passed to plugins

reason - reason why program was paused, currently always PP_EVENT;
reg - pointer to registers of thread that caused application to pause, may be NULL;
cmd - null-terminated command to plugin.
*/
{
	if (reason!=PP_EVENT)
		return 0;

	int p;
	string scmd,args;
	scmd.assign(cmd);
	if ((p=scmd.find_first_of(" \t\r\n"))!=string::npos) {
		args=trim(scmd.substr(p+1));
		scmd=trim(scmd.substr(0,p));
	}
	if (ollylang->isCommand(scmd)) {
		ollylang->callCommand(scmd,args);
		return 1;
	}

	return 0; //dont stop to other plugins
}

// OllyDbg calls this optional function when user wants to terminate OllyDbg.
extc int _export cdecl ODBG_Pluginclose() 
{

	if (ollylang->hwndinput != 0) {
		EndDialog(ollylang->hwndinput, 0);
		ollylang->hwndinput=0;
	}

	ollylang->SaveBreakPoints((char*)ollylang->scriptpath.c_str());

	Pluginwriteinttoini(hinstModule(),"Restore Script window",(ollylang->wndProg.hw!=NULL));
	Pluginwriteinttoini(hinstModule(),"Restore Script Log",(ollylang->wndLog.hw!=NULL));
	return 0;
}

// OllyDbg calls this optional function once on exit. At this moment, all
// windows created by plugin are already destroyed (and received WM_DESTROY
// messages). Function must free all internally allocated resources, like
// window classes, files, memory and so on.
extc void _export cdecl ODBG_Plugindestroy()
{
	delete ollylang;
	Unregisterpluginclass(wndprogclass);
	Unregisterpluginclass(wndlogclass);
}


#include "Dialogs.cpp"
#include "Progress.cpp"
#include "LogWindows.cpp"
#include "Search.cpp"
