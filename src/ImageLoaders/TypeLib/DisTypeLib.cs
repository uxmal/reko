//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤
//	"DisTypeLib.bas"

//This code is based on code from the ReactOS
//project at: http://www.reactos.org/en/index.html
//The primary source file used was:
//	...reactos\dll\win32\oleaut32\typelib.c

//So far, it only disassembles data with the "MSFT"
//magic value (not the "SLTG" type yet).

//It is intended to be integrated into bep (Bin
//Edit Plus) and perhaps other sub-projects

//This should have a TypeLib "diff" feature
//to help learn more about its format.

//This file is GPL 2008, by TheirCorp
//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤

#if VISUALBASIC
namespace Decompiler.TypeLib
{
    public class DisTypeLib
    {


//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤

//%Debug		= 1 //enable/disable debugging code
const int Study		= 1 ; // enable/disable code to help study the format
const int Priority	= 0 ; // set minimum priority level for logging and alerts

#if DEBUG
const int ProfileOn		= 1;

public static int dbg;		
public static string  dbs;	


#else

#endif

//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤

public const string Caption = "TheirCorp's Type Library Dumper";

public short GoBtn   = 1001;
public short PathTxt = 1101;
public short LogTxt  = 1102;
public short FileLbl = 1103;
public short MsgLbl  = 1104;

public short LogLines = 8;
//%fo  	  = 10 // output file// s number
Macro fo = 10 // output file's number
%IsOpen		= 0

Global ghDlg		As Dword
Global LocalPath	As String   // local path

//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤

Declare Function GetDroppedFile(ByVal hDrop As Long, fs As String) As Long
Declare Function ProcessFile(fs As String) As Long
Declare CallBack Function ShowDlgProc()
Declare Sub UpdateLog(ps As String)
Declare Function DisSltg(cs As String) As Dword

//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤


//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤

Declare Function GetResource( _
	cs As String,       _
	ByVal ird As IMAGE_RESOURCE_DIRECTORY Ptr, _
	ByVal de As Dword,  _   // value to subtract from ".link" offsets's
	ByVal ss As Dword,   _   // section size
	ByVal Offset As Dword _   // offset to resource section
	) As Long
Declare Function GetTypeLibData(cs As String, fs As String) As Long

Declare Function Locale(lcid As Long) As String
Declare Function VarType(ByVal pn As Long) As String
Declare Function tlName(cs As String, SegDir As MSFT_SegDir, ByVal offs As Long) As String
Declare Function tlString(cs As String, SegDir As MSFT_SegDir, ByVal offs As Long) As String
Declare Function DisFunction(cs As String, SegDir As MSFT_SegDir, ByVal pBase As Dword, ByVal nFunc As Long, ByVal nProp As Long) As Long
Declare Function DisTypeLib(cs As String) As Dword

//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤

Sub UpdateLog(ps As String)
Static ct   As Long
Static ls   As String
Static ts   As String

	If Len(Command$) Then
		// MsgBox ps

	Else
		Incr ct

		If Len(ps) Then
			// Control Get Text ghDlg, %LogTxt To ls
			// Dialog DoEvents
			ps = ps & $CrLf
		Else
			ct = 1
			ls = ""
			ts = ""
		End If

		If ct > %LogLines Then ts = Remain$(ts, $CrLf) & ps
		ls = ls & ps

		If Left$(ps, 5) = "Ready" Then
			Control Set Text ghDlg, %LogTxt, LTrim$(Left$(ls, 32000), Any $CrLf)
			Dialog DoEvents
			Control Send ghDlg, %LogTxt, %EM_SETSEL, 65536, 65536
			Dialog DoEvents
			Control Send ghDlg, %LogTxt, %EM_SCROLLCARET, 0, 0
			Dialog DoEvents
		Else
			Control Set Text ghDlg, %LogTxt, ts
		End If

	End If

End Sub // UpdateLog

//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤
//returns %true, if any files were received
Function GetDroppedFile(ByVal hDrop As Long, fs As String) As Long
Local ct  As Dword
Local az  As Asciiz * %MAX_PATH

	ct = DragQueryFile(hDrop, &HFFFFFFFF&, "", ByVal 0&)

	If ct > 0 Then
		az = Space$(%MAX_PATH)
		ct = DragQueryFile(hDrop, 0, az, Len(az) - 1)
		fs = Left$(az, ct)
		Function = %True
	End If

End Function

//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤
//expects "fs" to contain the path and name
//of either a PE or TLB format file
//It determines which type it is automatically
int ProcessFile(string fs)
{
int n		;
byte [] cs	;
string ls	;

	If Len(Dir$(fs)) Then

        n = GetTypeLibData(cs, fs);

		If n Then

			UpdateLog "Processing..."

			// --------------------------------------
			// open an output file for the disassembly
			Try
				$If %Def(%Study)
					CurFile = fs
					Note "" // reset message counter
				$EndIf
				ls = Mid$(fs, InStr(-1, fs, "\") + 1)
				ls = MCase$(Extract$(ls, ".")) & ".txt"
				Open LocalPath & "\" & ls For Output As fo
				UpdateLog "Output file: " & ls
			Catch
				UpdateLog "Error opening output file: " & ls
				Exit Function
			End Try

			If n = 1 Then DisTypeLib(cs)

		End If

	Else
		UpdateLog "Couldn't find: " & fs
	End If

	If FileAttr(fo, %IsOpen) Then Close# fo

End Function // ProcessFile

//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤

CallBack Function ShowDlgProc()
Local  fs   As String

$If %Def(%ProfileOn)
	Profile "Profile.txt"
$EndIf

	Select Case As Long CbMsg

		// Case %WM_INITDIALOG

		Case %WM_NCACTIVATE
			Static hWndSaveFocus  As Dword
			If IsFalse CbWParam Then
				hWndSaveFocus = GetFocus()
			ElseIf hWndSaveFocus Then
				SetFocus(hWndSaveFocus)
				hWndSaveFocus = 0
			End If

		Case %WM_DROPFILES
			If (GetDroppedFile(CbWParam, fs)) Then
				Control Set Text ghDlg, %PathTxt, fs
			End If
			DragFinish CbWParam

		Case %WM_COMMAND
			Select Case As Long CbCtl
				Case %GoBtn
					If CbCtlMsg = %BN_CLICKED Or CbCtlMsg = 1 Then
						Control Get Text ghDlg, %PathTxt To fs
						UpdateLog ""
						ProcessFile(fs)
						UpdateLog "Ready..."
					End If
			End Select

	End Select

End Function

//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤

Function PBMain() As Long
Local lRslt As Long

$If %Def(%ProfileOn)
	Profile "Profile.txt"
$EndIf

	// get and save the local path without a trailing backslash
	LocalPath = String$(%MAX_PATH, $Nul)
	GetModuleFileName(ByVal 0, ByVal StrPtr(LocalPath), %MAX_PATH)
	LocalPath = Left$(LocalPath, InStr(-1, LocalPath, "\") - 1)


	If Len(Command$) Then

		ProcessFile(Command$)
		MsgBox "Done"

	Else

		Dialog New %HWND_DESKTOP, $Caption, , , 256, 138, %WS_POPUP Or %WS_BORDER _
			Or %WS_DLGFRAME Or %WS_CAPTION Or %WS_SYSMENU Or %WS_MINIMIZEBOX Or _
			%WS_VISIBLE Or %DS_MODALFRAME Or %DS_SETFOREGROUND Or %DS_3DLOOK Or _
			%DS_NOFAILCREATE Or %DS_SETFONT, %WS_EX_WINDOWEDGE Or _
			%WS_EX_ACCEPTFILES Or %WS_EX_CONTROLPARENT Or %WS_EX_LEFT Or _
			%WS_EX_LTRREADING Or %WS_EX_RIGHTSCROLLBAR, To ghDlg

		Control Add Label,   ghDlg, %FileLbl, "&File:", 0, 4, 19, 10, %WS_CHILD Or _
			%WS_VISIBLE Or %WS_TABSTOP Or %SS_RIGHT, %WS_EX_LEFT Or %WS_EX_LTRREADING

		Control Add TextBox, ghDlg, %PathTxt, "", 20, 2, 233, 12

		Control Add Button,  ghDlg, %GoBtn, "&Go", 220, 17, 32, 14, %WS_CHILD Or _
			%WS_VISIBLE Or %WS_BORDER Or %WS_TABSTOP Or %BS_TEXT Or _
			%BS_DEFPUSHBUTTON Or %BS_PUSHBUTTON Or %BS_CENTER Or %BS_VCENTER, _
			%WS_EX_LEFT Or %WS_EX_LTRREADING

		Control Add Label, ghDlg, %MsgLbl, "&Messages:", 2, 22, 39, 10

		Control Add TextBox, ghDlg, %LogTxt, "", 2, 34, 252, 100, %WS_CHILD Or _
			%WS_VISIBLE Or %WS_TABSTOP Or %WS_HSCROLL Or %WS_VSCROLL Or %ES_LEFT _
			Or %ES_MULTILINE Or %ES_AUTOHSCROLL Or %ES_AUTOVSCROLL Or %ES_WANTRETURN, _
			%WS_EX_CLIENTEDGE Or %WS_EX_LEFT Or %WS_EX_LTRREADING Or %WS_EX_RIGHTSCROLLBAR

		Dialog Send ghDlg, %DM_SETDEFID, %GoBtn, 0
		Control Set Focus ghDlg, %PathTxt

		DragAcceptFiles ghDlg, %True // Register window to accept dropped files.

		Dialog Show Modal ghDlg, Call ShowDlgProc To lRslt

		Function = lRslt

	End If

End Function

//¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤

#endif