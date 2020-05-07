{
Created: Saturday, October 15, 1988 at 8:46 AM
    SANE.p
    Pascal interface for Standard Apple Numeric Environment

	Copyright © Symantec Corporation 1991
    Copyright Apple Computer, Inc.  1985-1988
    All rights reserved
}
{ Found at https://raw.githubusercontent.com/puk/watchmaker.js/master/original/Think%20Pascal%204%20%C6%92/THINK%20Pascal%204.0%20Folder/Interfaces/SANE.p }

UNIT SANE;
INTERFACE


{ Elems881 mode set by -d Elems881=true on Pascal command line }

{$IFC UNDEFINED Elems881}
{$SETC Elems881 = FALSE}
{$ENDC}


CONST
{$IFC OPTION(MC68881)}

{*======================================================================*
 *  The interface specific to the MC68881 SANE library  *
 *======================================================================*}

Inexact = 8;
DivByZero = 16;
Underflow = 32;
Overflow = 64;
Invalid = 128;
CurInex1 = 256;
CurInex2 = 512;
CurDivByZero = 1024;
CurUnderflow = 2048;
CurOverflow = 4096;
CurOpError = 8192;
CurSigNaN = 16384;
CurBSonUnor = 32768;


{$ELSEC}

{*======================================================================*
 *  The interface specific to the software SANE library    *
 *======================================================================*}

Invalid = 1;
Underflow = 2;
Overflow = 4;
DivByZero = 8;
Inexact = 16;
IEEEDefaultEnv = 0;     {IEEE-default floating-point environment constant}


{$ENDC}

{*======================================================================*
 *  The common interface for the SANE library    *
 *======================================================================*}
 
DecStrLen = 255;
SigDigLen = 20;         {for 68K; use 28 in 6502 SANE}


TYPE

RelOp = (GreaterThan,LessThan,EqualTo,Unordered);

NumClass = (SNaN,QNaN,Infinite,ZeroNum,NormalNum,DenormalNum);

RoundDir = (ToNearest,Upward,Downward,TowardZero);

RoundPre = (ExtPrecision,DblPrecision,RealPrecision);

DecimalKind = (FloatDecimal,FixedDecimal);

{$IFC OPTION(MC68881)}

{*======================================================================*
 *  The interface specific to the MC68881 SANE library  *
 *======================================================================*}
Exception = LONGINT;

Environment = RECORD
    FPCR: LONGINT;
    FPSR: LONGINT;
    END;

Extended80 = ARRAY [0..4] OF INTEGER;

TrapVector = RECORD
    Unordered: LONGINT;
    Inexact: LONGINT;
    DivByZero: LONGINT;
    Underflow: LONGINT;
    OpError: LONGINT;
    Overflow: LONGINT;
    SigNaN: LONGINT;
    END;

{$ELSEC}

{*======================================================================*
*  The interface specific to the software SANE library    *
*======================================================================*}

Exception = INTEGER;

Environment = INTEGER;

Extended96 = ARRAY [0..5] OF INTEGER;

{$ENDC}

{*======================================================================*
*  The common interface for the SANE library    *
*======================================================================*}

DecStr = STRING[DecStrLen];

DecForm = RECORD
    style: DecimalKind;
    digits: INTEGER;
    END;

Decimal = RECORD
    sgn: 0..1;
    exp: INTEGER;
    sig: STRING[SigDigLen];
    END;

CStrPtr = ^CHAR;



{$IFC OPTION(MC68881)}

{ return IEEE default environment }
FUNCTION IEEEDefaultEnv: environment;
PROCEDURE SetTrapVector(Traps: trapvector);
{ FPCP trap vectors <-- Traps }

{-------------------------------------------------------------------
  * FUNCTIONs for converting between SANE Extended formats
  -------------------------------------------------------------------}


PROCEDURE GetTrapVector(VAR Traps: trapvector);
{ Traps <-- FPCP trap vectors }

FUNCTION X96toX80(x: Extended): extended80;
{ X96toX80 <-- 96 bit x in 80 bit extended format }

FUNCTION X80toX96(x: extended80): Extended;
{ X80toX96 <-- 80 bit x in 96 bit extended format }

{$IFC Elems881 = false}

FUNCTION Sin(x: Extended): Extended;
{ sine }

FUNCTION Cos(x: Extended): Extended;
{ cosine }

FUNCTION ArcTan(x: Extended): Extended;
{ inverse tangent }

FUNCTION Exp(x: Extended): Extended;
{ base-e exponential }

FUNCTION Ln(x: Extended): Extended;
{ base-e log }

FUNCTION Log2(x: Extended): Extended;
{ base-2 log }

FUNCTION Ln1(x: Extended): Extended;
{ ln(1+x) }

FUNCTION Exp2(x: Extended): Extended;
{ base-2 exponential }

FUNCTION Exp1(x: Extended): Extended;
{ exp(x) - 1 }

FUNCTION Tan(x: Extended): Extended;
{ tangent }

{$ENDC}

{$ELSEC}

{ return halt vector }FUNCTION GetHaltVector: LONGINT;
PROCEDURE SetHaltVector(v: LONGINT);
{ halt vector <-- v }


FUNCTION X96toX80(x: Extended96): Extended;
{ 96 bit x in 80 bit extended format }

FUNCTION X80toX96(x: Extended): Extended96;
{ 80 bit x in 96 bit extended format }
{-------------------------------------------------------------------
* SANE library functions
 -------------------------------------------------------------------}

FUNCTION Log2(x: Extended): Extended;
{ base-2 log }

FUNCTION Ln1(x: Extended): Extended;
{ ln(1+x) }

FUNCTION Exp2(x: Extended): Extended;
{ base-2 exponential }

FUNCTION Exp1(x: Extended): Extended;
{ exp(x) - 1 }

FUNCTION Tan(x: Extended): Extended;
{ tangent }

{$ENDC}


{*======================================================================*
*  The common interface for the SANE library    *
*======================================================================*}

{---------------------------------------------------
* Conversions between numeric binary types.
---------------------------------------------------}

FUNCTION Num2Integer(x: Extended): INTEGER;
FUNCTION Num2Longint(x: Extended): LONGINT;
FUNCTION Num2Real(x: Extended): real;
FUNCTION Num2Double(x: Extended): DOUBLE;
FUNCTION Num2Extended(x: Extended): Extended;
FUNCTION Num2Comp(x: Extended): Comp;
{ ---------------------------------------------------
* Conversions between binary and decimal.
--------------------------------------------------- }

PROCEDURE Num2Dec(f: decform;x: Extended;VAR d: decimal);
{ d <-- x according to format f }

FUNCTION Dec2Num(d: decimal): Extended;
{ Dec2Num <-- d }

PROCEDURE Num2Str(f: decform;x: Extended;VAR s: DecStr);
{ s <-- x according to format f }

FUNCTION Str2Num(s: DecStr): Extended;
{ Str2Num <-- s }

{---------------------------------------------------
* Conversions between decimal formats.
---------------------------------------------------}

PROCEDURE Str2Dec(s: DecStr;VAR Index: INTEGER;VAR d: decimal;VAR ValidPrefix: BOOLEAN);
{ On input Index is starting index into s, on output Index is
one greater than index of last character of longest numeric
substring;
d <-- Decimal rep of longest numeric substring;
ValidPrefix <-- "s, beginning at Index, contains valid numeric
string or valid prefix of some numeric string" }

PROCEDURE CStr2Dec(s: CStrPtr;VAR Index: INTEGER;VAR d: decimal;VAR ValidPrefix: BOOLEAN);
{ Str2Dec for character buffers or C strings instead of Pascal
strings: the first argument is the the address of a character
buffer and ValidPrefix <-- "scanning ended with a null byte" }

PROCEDURE Dec2Str(f: decform;d: decimal;VAR s: DecStr);
{ s <-- d according to format f }

{---------------------------------------------------
* Arithmetic, auxiliary, and elementary functions.
---------------------------------------------------}

FUNCTION Remainder(x: Extended;y: Extended;VAR quo: INTEGER): Extended;
{ Remainder <-- x rem y; quo <-- low-order seven bits of integer
quotient x/y so that -127 < quo < 127 }

FUNCTION Rint(x: Extended): Extended;
{ round to integral value }

FUNCTION Scalb(n: INTEGER;x: Extended): Extended;
{ scale binary;  Scalb <-- x * 2^n }

FUNCTION Logb(x: Extended): Extended;
{ Logb <-- unbiased exponent of x }

FUNCTION CopySign(x: Extended;y: Extended): Extended;
{ CopySign <-- y with sign of x }

(* FUNCTION NextReal(x: real;y: real): real; *)
FUNCTION NextReal(x: Extended;y: Extended): real;
(* FUNCTION NextDouble(x: DOUBLE;y: DOUBLE): DOUBLE; *)
FUNCTION NextDouble(x: Extended;y: Extended): DOUBLE;
FUNCTION NextExtended(x: Extended;y: Extended): Extended;
{ return next representable value from x toward y }

FUNCTION XpwrI(x: Extended;i: INTEGER): Extended;
{ XpwrI <-- x^i }

FUNCTION XpwrY(x: Extended;y: Extended): Extended;
{ XpwrY <-- x^y }

FUNCTION Compound(r: Extended;n: Extended): Extended;
{ Compound <-- (1+r)^n }

FUNCTION Annuity(r: Extended;n: Extended): Extended;
{ Annuity <-- (1 - (1+r)^(-n)) / r }

FUNCTION RandomX(VAR x: Extended): Extended;
{ returns next random number and updates argument;
x integral, 1 <= x <= (2^31)-2 }

{---------------------------------------------------
* Inquiry routines.
---------------------------------------------------}

(* FUNCTION ClassReal(x: real): NumClass; *)
FUNCTION ClassReal(x: Extended): NumClass;

(* FUNCTION ClassDouble(x: DOUBLE): NumClass; *)
FUNCTION ClassDouble(x: Extended): NumClass;

(* FUNCTION ClassComp(x: Comp): NumClass; *)
FUNCTION ClassComp(x: Extended): NumClass;

FUNCTION ClassExtended(x: Extended): NumClass;
{ return class of x }


FUNCTION SignNum(x: Extended): INTEGER;
{ 0 if sign bit clear, 1 if sign bit set }

{---------------------------------------------------
* NaN function.
---------------------------------------------------}

FUNCTION NAN(i: INTEGER): Extended;
{ returns NaN with code i }

{---------------------------------------------------
* Environment access routines.
---------------------------------------------------}

PROCEDURE SetException(e: Exception;b: BOOLEAN);
{ set e flags if b is true, clear e flags otherwise; may cause halt }

FUNCTION TestException(e: Exception): BOOLEAN;
{ return true if any e flag is set, return false otherwise }

PROCEDURE SetHalt(e: Exception;b: BOOLEAN);
{ set e halt enables if b is true, clear e halt enables otherwise }

FUNCTION TestHalt(e: Exception): BOOLEAN;
{ return true if any e halt is enabled, return false otherwise }

PROCEDURE SetRound(r: RoundDir);
{ set rounding direction to r }

FUNCTION GetRound: RoundDir;
{ return rounding direction }

PROCEDURE SetPrecision(p: RoundPre);
{ set rounding precision to p }

FUNCTION GetPrecision: RoundPre;
{ return rounding precision }


PROCEDURE SetEnvironment(e: environment);
{ set environment to e }

PROCEDURE GetEnvironment(VAR e: environment);
{ e <-- environment }

PROCEDURE ProcEntry(VAR e: environment);
{ e <-- environment;  environment <-- IEEE default env }

PROCEDURE ProcExit(e: environment);
{ temp <-- exceptions; environment <-- e;
signal exceptions in temp }

{---------------------------------------------------
* Comparison routine.
---------------------------------------------------}

FUNCTION Relation(x: Extended;y: Extended): RelOp;
{ return Relation such that "x Relation y" is true }


IMPLEMENTATION
END.