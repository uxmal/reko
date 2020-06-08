{#####################################################################}
{### FILE: ADSP.p}
{#####################################################################}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT ADSP;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingADSP}
{$SETC UsingADSP := 1}
{$I+}
{$SETC ADSPIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingAppleTalk}
{$I $$Shell(PInterfaces)AppleTalk.p}
{$ENDC}
{$SETC UsingIncludes := ADSPIncludes}
CONST
{ driver control ioResults }
errRefNum = -1280;
errAborted = -1279;
errState = -1278;
errOpening = -1277;
errAttention = -1276;
errFwdReset = -1275;
errDSPQueueSize = -1274;
errOpenDenied = -1273;

{
{
{
{
{
{
{
{

bad connection refNum }
control call was aborted }
bad connection state for this operation }
open connection request failed }
attention message too long }
read terminated by forward reset }
DSP Read/Write Queue Too small }
open connection request was denied }

{driver control csCodes}
dspInit = 255;
dspRemove = 254;
dspOpen = 253;
dspClose = 252;
dspCLInit = 251;
dspCLRemove = 250;
dspCLListen = 249;
dspCLDeny = 248;

{
{
{
{
{
{
{
{

create a new connection end }
remove a connection end }
open a connection }
close a connection }
create a connection listener }
remove a connection listener }
post a listener request }
deny an open connection request }



dspStatus = 247;
dspRead = 246;
dspWrite = 245;
dspAttention = 244;
dspOptions = 243;
dspReset = 242;
dspNewCID = 241;

{
{
{
{
{
{
{

get status of connection end }
read data from the connection }
write data on the connection }
send an attention message }
set connection end options }
forward reset the connection }
generate a cid for a connection end }

{ connection opening modes }
ocRequest = 1;
ocPassive = 2;
ocAccept = 3;
ocEstablish = 4;

{
{
{
{

request a connection with remote }
wait for a connection request from remote }
accept request as delivered by listener }
consider connection to be open }

{ connection end states }
sListening = 1;
sPassive = 2;
sOpening = 3;
sOpen = 4;
sClosing = 5;
sClosed = 6;

{
{
{
{
{
{

for connection listeners }
waiting for a connection request from remote }
requesting a connection with remote }
connection is open }
connection is being torn down }
connection end state is closed }

{ client event flags }
eClosed = $80;
eTearDown = $40;
eAttention = $20;
eFwdReset = $10;

{
{
{
{

received connection closed advice }
connection closed due to broken connection }
received attention message }
received forward reset advice }

{ miscellaneous constants }
attnBufSize = 570;
minDSPQueueSize = 100;

{ size of client attention buffer }
{ Minimum size of receive or send Queue }

TYPE
{ connection control block }
TPCCB = ^TRCCB;
TRCCB = PACKED RECORD
ccbLink: TPCCB;
refNum: INTEGER;
state: INTEGER;
userFlags: Byte;
localSocket: Byte;
remoteAddress: AddrBlock;
attnCode: INTEGER;
attnSize: INTEGER;
attnPtr: Ptr;
reserved: PACKED ARRAY [1..220] OF Byte;
END;

{
{
{
{
{
{
{
{
{
{

link to next ccb }
user reference number }
state of the connection end }
flags for unsolicited connection events }
socket number of this connection end }
internet address of remote end }
attention code received }
size of received attention data }
ptr to received attention data }
adsp internal use }

{ ADSP CntrlParam ioQElement , driver control call parameter block}
DSPPBPtr = ^DSPParamBlock;
DSPParamBlock = PACKED RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;

ioResult: OSErr;
ioNamePtr: StringPtr;
ioVRefNum: INTEGER;
ioCRefNum: INTEGER;
csCode: INTEGER;
qStatus: LONGINT;
ccbRefNum: INTEGER;
CASE INTEGER OF
dspInit,dspCLInit:
(ccbPtr: TPCCB;
userRoutine: ProcPtr;
sendQSize: INTEGER;
sendQueue: Ptr;
recvQSize: INTEGER;
recvQueue: Ptr;
attnPtr: Ptr;
localSocket: Byte;
filler1: Byte;
);
dspOpen,dspCLListen,dspCLDeny:
(localCID: INTEGER;
remoteCID: INTEGER;
remoteAddress: AddrBlock;
filterAddress: AddrBlock;
sendSeq: LONGINT;
sendWindow: INTEGER;
recvSeq: LONGINT;
attnSendSeq: LONGINT;
attnRecvSeq: LONGINT;
ocMode: Byte;
ocInterval: Byte;
ocMaximum: Byte;
filler2: Byte;
);
dspClose,dspRemove:
(abort: Byte;
filler3: Byte;
);
dspStatus:
(statusCCB: TPCCB;
sendQPending: INTEGER;
sendQFree: INTEGER;
recvQPending: INTEGER;
recvQFree: INTEGER;
);
dspRead,dspWrite:
(reqCount: INTEGER;
actCount: INTEGER;
dataPtr: Ptr;
eom: Byte;
flush: Byte;
);
dspAttention:
(attnCode: INTEGER;
attnSize: INTEGER;
attnData: Ptr;
{
{
{
{

adsp driver refNum }
adsp driver control code }
adsp internal use }
refnum of ccb }

{pointer to connection control block}
{client routine to call on event}
{size of send queue (0..64K bytes)}
{client passed send queue buffer}
{size of receive queue (0..64K bytes)}
{client passed receive queue buffer}
{client passed receive attention buffer}
{local socket number}
{filler for proper byte alignment}

{local connection id}
{remote connection id}
{address of remote end}
{address filter}
{local send sequence number}
{send window size}
{receive sequence number}
{attention send sequence number}
{attention receive sequence number}
{open connection mode}
{open connection request retry interval}
{open connection request retry maximum}
{filler for proper byte alignment}

{abort connection immediately if non-zero}
{filler for proper byte alignment}

{pointer to ccb}
{pending bytes in
{available buffer
{pending bytes in
{available buffer

send queue}
space in send queue}
receive queue}
space in receive queue}

{requested number of bytes}
{actual number of bytes}
{pointer to data buffer}
{indicates logical end of message}
{send data now}

{client attention code}
{size of attention data}
{pointer to attention data}


attnInterval: Byte;
filler4: Byte;
);
dspOptions:
(sendBlocking: INTEGER;
sendTimer: Byte;
rtmtTimer: Byte;
badSeqMax: Byte;
useCheckSum: Byte;
);
dspNewCID:
(newCID: INTEGER;
);

END;

{$ENDC} { UsingADSP }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE ADSP.p}

{retransmit timer in 10-tick intervals}
{filler for proper byte alignment}

{quantum for data packets}
{send timer in 10-tick intervals}
{retransmit timer in 10-tick intervals}
{threshold for sending retransmit advice}
{use ddp packet checksum}

{new connection id returned}



{#####################################################################}
{### FILE: AIFF.p}
{#####################################################################}

{
Created: Monday, December 2, 1991 at 5:01 PM
AIFF.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1990-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT AIFF;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingAIFF}
{$SETC UsingAIFF := 1}
{$I+}
{$SETC AIFFIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := AIFFIncludes}
CONST
AIFFID = 'AIFF';
AIFCID = 'AIFC';
FormatVersionID = 'FVER';
CommonID = 'COMM';
FORMID = 'FORM';
SoundDataID = 'SSND';
MarkerID = 'MARK';
InstrumentID = 'INST';
MIDIDataID = 'MIDI';
AudioRecordingID = 'AESD';
ApplicationSpecificID = 'APPL';
CommentID = 'COMT';
NameID = 'NAME';
AuthorID = 'AUTH';
CopyrightID = '(c) ';
AnnotationID = 'ANNO';
NoLooping = 0;
ForwardLooping = 1;
ForwardBackwardLooping = 2;



{ AIFF-C Versions }
AIFCVersion1 = $A2805140;
{ Compression Names }
NoneName = 'not compressed';
ACE2to1Name = 'ACE 2-to-1';
ACE8to3Name = 'ACE 8-to-3';
MACE3to1Name = 'MACE 3-to-1';
MACE6to1Name ='MACE 6-to-1';
{ Compression Types }
NoneType = 'NONE';
ACE2Type = 'ACE2';
ACE8Type = 'ACE8';
MACE3Type = 'MAC3';
MACE6Type = 'MAC6';
TYPE
ID = LONGINT;
MarkerIdType = INTEGER;
ChunkHeader = RECORD
	ckID: ID;
	ckSize: LONGINT;
END;
ContainerChunk = RECORD
ckID: ID;
ckSize: LONGINT;
formType: ID;
END;
FormatVersionChunkPtr = ^FormatVersionChunk;
FormatVersionChunk = RECORD
ckID: ID;
ckSize: LONGINT;
timestamp: LONGINT;
END;
CommonChunkPtr = ^CommonChunk;
CommonChunk = RECORD
ckID: ID;
ckSize: LONGINT;
numChannels: INTEGER;
numSampleFrames: LONGINT;
sampleSize: INTEGER;
sampleRate: Extended80;
END;
ExtCommonChunkPtr = ^ExtCommonChunk;
ExtCommonChunk = RECORD
ckID: ID;
ckSize: LONGINT;
numChannels: INTEGER;
numSampleFrames: LONGINT;



sampleSize: INTEGER;
sampleRate: Extended80;
compressionType: ID;
compressionName: PACKED ARRAY [0..0] OF Byte;
END;
SoundDataChunkPtr = ^SoundDataChunk;
SoundDataChunk = RECORD
ckID: ID;
ckSize: LONGINT;
offset: LONGINT;
blockSize: LONGINT;
END;
Marker = RECORD
id: MarkerIdType;
position: LONGINT;
markerName: Str255;
END;
MarkerChunkPtr = ^MarkerChunk;
MarkerChunk = RECORD
ckID: ID;
ckSize: LONGINT;
numMarkers: INTEGER;
Markers: ARRAY [0..0] OF Marker;
END;
AIFFLoop = RECORD
playMode: INTEGER;
beginLoop: MarkerIdType;
endLoop: MarkerIdType;
END;
InstrumentChunkPtr = ^InstrumentChunk;
InstrumentChunk = PACKED RECORD
ckID: ID;
ckSize: LONGINT;
baseFrequency: Byte;
detune: Byte;
lowFrequency: Byte;
highFrequency: Byte;
lowVelocity: Byte;
highVelocity: Byte;
gain: INTEGER;
sustainLoop: AIFFLoop;
releaseLoop: AIFFLoop;
END;
MIDIDataChunkPtr = ^MIDIDataChunk;
MIDIDataChunk = RECORD
ckID: ID;
ckSize: LONGINT;
MIDIdata: PACKED ARRAY [0..0] OF SignedByte;
END;


AudioRecordingChunkPtr = ^AudioRecordingChunk;
AudioRecordingChunk = RECORD
ckID: ID;
ckSize: LONGINT;
AESChannelStatus: PACKED ARRAY [0..23] OF SignedByte;
END;
ApplicationSpecificChunkPtr = ^ApplicationSpecificChunk;
ApplicationSpecificChunk = RECORD
ckID: ID;
ckSize: LONGINT;
applicationSignature: OSType;
data: PACKED ARRAY [0..0] OF Byte;
END;
Comment = RECORD
timeStamp: LONGINT;
marker: MarkerIdType;
count: INTEGER;
text: PACKED ARRAY [0..0] OF Byte;
END;
CommentsChunkPtr = ^CommentsChunk;
CommentsChunk = RECORD
ckID: ID;
ckSize: LONGINT;
numComments: INTEGER;
comments: ARRAY [0..0] OF Comment;
END;
TextChunkPtr = ^TextChunk;
TextChunk = RECORD
ckID: ID;
ckSize: LONGINT;
text: PACKED ARRAY [0..0] OF Byte;
END;

{$ENDC} { UsingAIFF }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE AIFF.p}



{#####################################################################}
{### FILE: Aliases.p}
{#####################################################################}
{
Created: Monday, January 28, 1991 at 1:26 PM
Aliases.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1989-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Aliases;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingAliases}
{$SETC UsingAliases := 1}
{$I+}
{$SETC AliasesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$IFC UNDEFINED UsingAppleTalk}
{$I $$Shell(PInterfaces)AppleTalk.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$SETC UsingIncludes := AliasesIncludes}
CONST
rAliasType = 'alis';

{ Aliases are stored as resources of this type }

{ define alias resolution action rules mask }
kARMMountVol = $00000001;
{ mount the volume automatically }
kARMNoUI = $00000002;
{ no user interface allowed during resolution }
kARMMultVols = $00000008;
{ search on multiple volumes }
kARMSearch = $00000100;
{ search quickly }
kARMSearchMore = $00000200;
{ search further }
kARMSearchRelFirst = $00000400;
{ search target on a relative path first }
{ define alias record information types }

asiZoneName = -3;
asiServerName = -2;
asiVolumeName = -1;
asiAliasName = 0;
asiParentName = 1;


{
{
{
{
{

get
get
get
get
get

zone name }
server name }
volume name }
aliased file/folder/volume name }
parent folder name }

TYPE
{ define the alias record that will be the blackbox for the caller }
AliasPtr = ^AliasRecord;
AliasHandle = ^AliasPtr;
AliasRecord = RECORD
userType: OSType;
{ appl stored type like creator type }
aliasSize: INTEGER;
{ alias record size in bytes, for appl usage }
END;

AliasInfoType = INTEGER;
AliasFilterProcPtr = ProcPtr;

{ alias record information type }

{ create a new alias between fromFile-target and return alias record handle
FUNCTION NewAlias(fromFile: FSSpecPtr;
target: FSSpec;
VAR alias: AliasHandle): OSErr;
INLINE $7002,$A823;

}

{ create a minimal new alias for a target and return alias record handle }
FUNCTION NewAliasMinimal(target: FSSpec;
VAR alias: AliasHandle): OSErr;
INLINE $7008,$A823;
{ create a minimal new alias from a target fullpath (optional zone and server name) and return alias record handle
FUNCTION NewAliasMinimalFromFullPath(fullPathLength: INTEGER;
fullPath: Ptr;
zoneName: Str32;
serverName: Str31;
VAR alias: AliasHandle): OSErr;
INLINE $7009,$A823;

}

{ given an alias handle and fromFile, resolve the alias, update the alias record and return aliased filename and wasChanged flag. }
FUNCTION ResolveAlias(fromFile: FSSpecPtr;
alias: AliasHandle;
VAR target: FSSpec;
VAR wasChanged: BOOLEAN): OSErr;
INLINE $7003,$A823;
{ given an alias handle and an index specifying requested alias information type, return the information from alias record as a string. }
FUNCTION GetAliasInfo(alias: AliasHandle;
index: AliasInfoType;
VAR theString: Str63): OSErr;
INLINE $7007,$A823;
{

given a file spec, return target file spec if input file spec is an alias.
It resolves the entire alias chain or one step of the chain. It returns
info about whether the target is a folder or file; and whether the input




file spec was an alias or not. }
FUNCTION ResolveAliasFile(VAR theSpec: FSSpec;
resolveAliasChains: BOOLEAN;
VAR targetIsFolder: BOOLEAN;
VAR wasAliased: BOOLEAN): OSErr;
INLINE $700C,$A823;
{
Low Level Routines
given an alias handle and fromFile, match the alias and return aliased filename(s) and needsUpdate flag }
FUNCTION MatchAlias(fromFile: FSSpecPtr;
rulesMask: LONGINT;
alias: AliasHandle;
VAR aliasCount: INTEGER;
aliasList: FSSpecArrayPtr;
VAR needsUpdate: BOOLEAN;
aliasFilter: AliasFilterProcPtr;
yourDataPtr: UNIV Ptr): OSErr;
INLINE $7005,$A823;
{ given a fromFile-target pair and an alias handle, update the lias record pointed to by alias handle to represent target as the new alias. }
FUNCTION UpdateAlias(fromFile: FSSpecPtr;
target: FSSpec;
alias: AliasHandle;
VAR wasChanged: BOOLEAN): OSErr;
INLINE $7006,$A823;

{$ENDC}

{ UsingAliases }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Aliases.p}




{#####################################################################}
{### FILE: AppleEvents.p}
{#####################################################################}

{
Created: Monday, September 16, 1991 at 2:41 PM
AppleEvents.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1989-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT AppleEvents;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingAppleEvents}
{$SETC UsingAppleEvents := 1}
{$I+}
{$SETC AppleEventsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingMemory}
{$I $$Shell(PInterfaces)Memory.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$IFC UNDEFINED UsingEvents}
{$I $$Shell(PInterfaces)Events.p}
{$ENDC}
{$IFC UNDEFINED UsingEPPC}
{$I $$Shell(PInterfaces)EPPC.p}
{$ENDC}
{$IFC UNDEFINED UsingNotification}
{$I $$Shell(PInterfaces)Notification.p}
{$ENDC}
{$SETC UsingIncludes := AppleEventsIncludes}
CONST
typeBoolean = 'bool';
typeChar = 'TEXT';
typeSMInt = 'shor';



typeInteger = 'long';
typeSMFloat = 'sing';
typeFloat = 'doub';
typeLongInteger = 'long';
typeShortInteger = 'shor';
typeLongFloat = 'doub';
typeShortFloat = 'sing';
typeExtended = 'exte';
typeComp = 'comp';
typeMagnitude = 'magn';
typeAEList = 'list';
typeAERecord = 'reco';
typeTrue = 'true';
typeFalse = 'fals';
typeAlias = 'alis';
typeEnumerated = 'enum';
typeType = 'type';
typeAppParameters = 'appa';
typeProperty = 'prop';
typeFSS = 'fss ';
typeKeyword = 'keyw';
typeSectionH = 'sect';
typeWildCard = '****';
typeApplSignature = 'sign';
typeSessionID = 'ssid';
typeTargetID = 'targ';
typeProcessSerialNumber = 'psn ';
typeNull = 'null';

{the type of null/nonexistent data}

kCoreEventClass = 'aevt';
kAEOpenApplication = 'oapp';
kAEOpenDocuments = 'odoc';
kAEPrintDocuments = 'pdoc';
kAEQuitApplication = 'quit';
kAECreatorType = 'crea';
kAEQuitAll = 'quia';
kAEShutDown = 'shut';
kAERestart = 'rest';
kAEApplicationDied = 'obit';
keyProcessSerialNumber = 'psn ';
keyErrorNumber = 'errn';
keyErrorString = 'errs';
kAEAnswer = 'ansr';
keyDirectObject = '----';

{ keyword used in install special handler }
keyPreDispatch = 'phac';
{ PreHandler Accessor Call }
keySelectProc = 'selh';
{ More selector Call }

{ keywords used in attributes }
keyTransactionIDAttr = 'tran';
keyReturnIDAttr = 'rtid';
keyEventClassAttr = 'evcl';
keyEventIDAttr = 'evid';
keyAddressAttr = 'addr';
keyOptionalKeywordAttr = 'optk';
keyTimeoutAttr = 'timo';
keyInteractLevelAttr = 'inte';
keyEventSourceAttr = 'esrc';
keyMissedKeywordAttr = 'miss';
{this attribute is read only will be set in AESend}
{ this attribute is read only }
{ this attribute is read only }

{ constants for use in AESendMode }
kAENoReply = $00000001;
{ Sender doesn't want a reply to event }
kAEQueueReply = $00000002;
{ Sender wants a reply but won't wait }
kAEWaitReply = $00000003;
{ Sender wants a reply and will be waiting }
kAENeverInteract = $00000010;
kAECanInteract = $00000020;
kAEAlwaysInteract = $00000030;

{ Server should not interact with user }
{ Server may try to interact with user }
{ Server should always interact with user where appropriate }

kAECanSwitchLayer = $00000040;

{ Interaction may switch layer }

kAEDontReconnect = $00000080;

{ don't reconnect if there is a sessClosedErr from PPCToolbox }

kAEWantReceipt = nReturnReceipt;

{ Send wants a receipt of message }

{ constants to be used in AESendPriority }
kAENormalPriority = $00000000;
{ Post message at the end of event queue }
kAEHighPriority = nAttnMsg;
{ Post message at the front of the event queue }
{ special constants in generating events }
kAnyTransactionID = 0;
{ no transaction is in use }
kAutoGenerateReturnID = -1;
{ AECreateAppleEvent will generate a session-unique ID }
{ constant for use AESend }
kAEDefaultTimeout = -1;
kNoTimeOut = -2;

{ timeout value determined by AEM }
{ wait until reply comes back, however long it takes }

{ dispatch parameter to AEResumeTheCurrentEvent takes a pointer to a dispatch
table, or one of these two constants }
kAENoDispatch = 0;
kAEUseStandardDispatch = -1;
{ Error messages in response to reading and writing event contents }
errAECoercionFail = -1700;
errAEDescNotFound = -1701;
errAECorruptData = -1702;
errAEWrongDataType = -1703;
errAENotAEDesc = -1704;
errAEBadListItem = -1705;
{ Specified list item does not exist }
errAENewerVersion = -1706;
{ Need newer version of AppleEvent Manager }
errAENotAppleEvent = -1707;
{ The event is not in AppleEvent format }
{ Error messages in response to sending/receiving a message }
errAEEventNotHandled = -1708;
{ The AppleEvent was not handled by any handler }



errAEReplyNotValid = -1709;
errAEUnknownSendMode = -1710;
errAEWaitCanceled = -1711;
errAETimeout = -1712;

{
{
{
{

AEResetTimer was passed an invalid reply parameter }
Mode wasn't NoReply, WaitReply, or QueueReply; or Interaction level is unknown }
In AESend, User cancelled out of wait loop for reply or receipt }
AppleEvent timed out }

errAENoUserInteraction = -1713;
errAENotASpecialFunction = -1714;
errAEParamMissed = -1715;

{ no user interaction allowed }
{ there is no special function with this keyword }
{ a required parameter was not accessed }

errAEUnknownAddressType = -1716;
{ The target address type is not known }
errAEHandlerNotFound = -1717;
{ No handler in the dispatch tables fits the parameters to
AEGetEventHandler or AEGetCoercionHandler }
errAEReplyNotArrived = -1718;
errAEIllegalIndex = -1719;

{ the contents of the reply you are accessing have not arrived yet }
{ Index is out of range in a put operation }

TYPE
AEKeyword
= PACKED ARRAY [1..4] OF CHAR;
AEEventClass = PACKED ARRAY [1..4] OF CHAR;
AEEventID
= PACKED ARRAY [1..4] OF CHAR;
DescType = ResType;
{ tagged data, the standard AppleEvent data type }
AEDesc = RECORD
descriptorType: DescType;
dataHandle: Handle;
END;

AEAddressDesc = AEDesc;
AEDescList = AEDesc;
AERecord = AEDescList;
AppleEvent = AERecord;
AESendMode = LONGINT;
AESendPriority = INTEGER;

{
{
{
{
{
{

an AEDesc which contains addressing data }
a list of AEDesc is a special kind of AEDesc }
AERecord is a list of keyworded AEDesc }
an AERecord that contains an AppleEvent }
Type of parameter to AESend }
Type of priority param of AESend }

{ type of param to AEGetInteractionAllowed and AESetInteractionAllowed }
AEInteractAllowed = (kAEInteractWithSelf,kAEInteractWithLocal,kAEInteractWithAll);
{ Return param to AEGetTheCurrentEvent, and kAEEventSource attribute }
AEEventSource = (kAEUnknownSource,kAEDirectCall,kAESameProcess,kAELocalProcess,
kAERemoteProcess);

{ types for AppleEvent Array support
Basic data type of attibutes & parameters}
AEKeyDesc = RECORD
descKey: AEKeyword;
descContent: AEDesc;
END;

AEArrayType = (kAEDataArray,kAEPackedArray,kAEHandleArray,kAEDescArray,
kAEKeyDescArray);



{ Array routines support these different types of elements}
AEArrayData = RECORD
case AEArrayType OF
kAEDataArray:
(AEDataArray:
Array[0..0] OF Integer);
kAEPackedArray:
(AEPackedArray: Packed Array[0..0] OF Char);
kAEHandleArray:
(AEHandleArray: Array[0..0] OF Handle);
kAEDescArray:
(AEDescArray:
Array[0..0] OF AEDesc);
kAEKeyDescArray:
(AEKeyDescArray: Array[0..0] OF AEKeyDesc);
END;
AEArrayDataPointer = ^AEArrayData;

EventHandlerProcPtr = ProcPtr;
IdleProcPtr = ProcPtr;
EventFilterProcPtr = ProcPtr;

{
*********************************************************************
The following calls apply to any AEDesc. Every result descriptor is created for you,
so you will be responsible for memory management of the descriptors so created.
Purgeable descriptor data is not supported: the AEM does not call LoadResource.
}
FUNCTION AECreateDesc(typeCode: DescType; dataPtr: Ptr; dataSize: Size; VAR result: AEDesc): OSErr;
INLINE $303C, $0825, $A816;
FUNCTION AECoercePtr(typeCode: DescType;
dataPtr: Ptr;
dataSize: Size;
toType: DescType;
VAR result: AEDesc): OSErr;
INLINE $303C, $0A02, $A816;
FUNCTION AECoerceDesc(theAEDesc: AEDesc;
toType: DescType;
VAR result: AEDesc): OSErr;
INLINE $303C, $0603, $A816;
FUNCTION AEDisposeDesc(VAR theAEDesc: AEDesc): OSErr;
INLINE $303C, $0204, $A816;
FUNCTION AEDuplicateDesc(theAEDesc: AEDesc;
VAR result: AEDesc): OSErr;
INLINE $303C, $0405, $A816;
{ *********************************************************************
The following calls apply to AEDescList.
Since AEDescList is a subtype of AEDesc, the calls in the previous
section can also be used for AEDescList. All list and array indices are 1-based.
If the data was greater than maximumSize in the routines below, then actualSize will
be greater than maximumSize, but only maximumSize bytes will actually be retrieved. }



FUNCTION AECreateList(factoringPtr: Ptr;
factoredSize: Size;
isRecord: BOOLEAN;
VAR resultList: AEDescList): OSErr;
INLINE $303C, $0706, $A816;
FUNCTION AECountItems(theAEDescList: AEDescList;
VAR theCount: LONGINT): OSErr;
INLINE $303C, $0407, $A816;
FUNCTION AEPutPtr(theAEDescList: AEDescList;
index: LONGINT;
typeCode: DescType;
dataPtr: Ptr;
dataSize: Size): OSErr;
INLINE $303C, $0A08, $A816;
FUNCTION AEPutDesc(theAEDescList: AEDescList;
index: LONGINT;
theAEDesc: AEDesc): OSErr;
INLINE $303C, $0609, $A816;
FUNCTION AEGetNthPtr(theAEDescList: AEDescList;
index: LONGINT;
desiredType: DescType;
VAR theAEKeyword: AEKeyword;
VAR typeCode: DescType;
dataPtr: Ptr;
maximumSize: Size;
VAR actualSize: Size): OSErr;
INLINE $303C, $100A, $A816;
FUNCTION AEGetNthDesc(theAEDescList: AEDescList;
index: LONGINT;
desiredType: DescType;
VAR theAEKeyword: AEKeyword;
VAR result: AEDesc): OSErr;
INLINE $303C, $0A0B, $A816;
FUNCTION AESizeOfNthItem(theAEDescList: AEDescList;
index: LONGINT;
VAR typeCode: DescType;
VAR dataSize: Size): OSErr;
INLINE $303C, $082A, $A816;
FUNCTION AEGetArray(theAEDescList: AEDescList;
arrayType: AEArrayType;
arrayPtr: AEArrayDataPointer;
maximumSize: Size;
VAR itemType: DescType;
VAR itemSize: Size;
VAR itemCount: LONGINT): OSErr;
INLINE $303C, $0D0C, $A816;
FUNCTION AEPutArray(theAEDescList: AEDescList;
arrayType: AEArrayType;
arrayPtr: AEArrayDataPointer;
itemType: DescType;
itemSize: Size;
itemCount: LONGINT): OSErr;
INLINE $303C, $0B0D, $A816;
FUNCTION AEDeleteItem(theAEDescList: AEDescList;
index: LONGINT): OSErr;
INLINE $303C, $040E, $A816;



{ *********************************************************************
The following calls apply to AERecord.
Since AERecord is a subtype of AEDescList, the calls in the previous
sections can also be used for AERecord
an AERecord can be created by using AECreateList with isRecord set to true
FUNCTION AEPutKeyPtr(theAERecord: AERecord;
theAEKeyword: AEKeyword;
typeCode: DescType;
dataPtr: Ptr;
dataSize: Size): OSErr;
INLINE $303C, $0A0F, $A816;
FUNCTION AEPutKeyDesc(theAERecord: AERecord;
theAEKeyword: AEKeyword;
theAEDesc: AEDesc): OSErr;
INLINE $303C, $0610, $A816;
FUNCTION AEGetKeyPtr(theAERecord: AERecord;
theAEKeyword: AEKeyword;
desiredType: DescType;
VAR typeCode: DescType;
dataPtr: Ptr;
maximumSize: Size;
VAR actualSize: Size): OSErr;
INLINE $303C, $0E11, $A816;
FUNCTION AEGetKeyDesc(theAERecord: AERecord;
theAEKeyword: AEKeyword;
desiredType: DescType;
VAR result: AEDesc): OSErr;
INLINE $303C, $0812, $A816;
FUNCTION AESizeOfKeyDesc(theAERecord: AERecord;
theAEKeyword: AEKeyword;
VAR typeCode: DescType;
VAR dataSize: Size): OSErr;
INLINE $303C, $0829, $A816;
FUNCTION AEDeleteKeyDesc(theAERecord: AERecord;
theAEKeyword: AEKeyword): OSErr;
INLINE $303C, $0413, $A816;

}

{
*********************************************************************
The following calls are used to pack and unpack parameters from records of
type AppleEvent. Since AppleEvent is a subtype of AERecord, the calls in the previous
sections can also be used for variables of type AppleEvent. The next six calls
are in fact identical to the six calls for AERecord.
}
FUNCTION AEPutParamPtr(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword;
typeCode: DescType;
dataPtr: Ptr;
dataSize: Size): OSErr;
INLINE $303C,$0A0F,$A816;
FUNCTION AEPutParamDesc(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword;
theAEDesc: AEDesc): OSErr;
INLINE $303C,$0610,$A816;
FUNCTION AEGetParamPtr(theAppleEvent: AppleEvent;



theAEKeyword: AEKeyword;
desiredType: DescType;
VAR typeCode: DescType;
dataPtr: Ptr;
maximumSize: Size;
VAR actualSize: Size): OSErr;
INLINE $303C,$0E11,$A816;
FUNCTION AEGetParamDesc(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword;
desiredType: DescType;
VAR result: AEDesc): OSErr;
INLINE $303C,$0812,$A816;
FUNCTION AESizeOfParam(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword;
VAR typeCode: DescType;
VAR dataSize: Size): OSErr;
INLINE $303C,$0829,$A816;
FUNCTION AEDeleteParam(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword): OSErr;
INLINE $303C,$0413,$A816;
{ *********************************************************************
The following calls also apply to type AppleEvent. Message attributes are far more restricted, and
can only be accessed through the following 5 calls. The various list and record routines cannot be used
to access the attributes of an event. }
FUNCTION AEGetAttributePtr(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword;
desiredType: DescType;
VAR typeCode: DescType;
dataPtr: Ptr;
maximumSize: Size;
VAR actualSize: Size): OSErr;
INLINE $303C,$0E15,$A816;
FUNCTION AEGetAttributeDesc(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword;
desiredType: DescType;
VAR result: AEDesc): OSErr;
INLINE $303C,$0826,$A816;
FUNCTION AESizeOfAttribute(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword;
VAR typeCode: DescType;
VAR dataSize: Size): OSErr;
INLINE $303C,$0828,$A816;
FUNCTION AEPutAttributePtr(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword;
typeCode: DescType;
dataPtr: Ptr;
dataSize: Size): OSErr;
INLINE $303C,$0A16,$A816;
FUNCTION AEPutAttributeDesc(theAppleEvent: AppleEvent;
theAEKeyword: AEKeyword;
theAEDesc: AEDesc): OSErr;
INLINE $303C,$0627,$A816;
{ *********************************************************************
The next four calls are basic routines used to create, send, and process AppleEvents.

}



FUNCTION AECreateAppleEvent(theAEEventClass: AEEventClass;
theAEEventID: AEEventID;
target: AEAddressDesc;
returnID: INTEGER;
transactionID: LONGINT;
VAR result: AppleEvent): OSErr;
INLINE $303C,$0B14,$A816;
FUNCTION AESend(theAppleEvent: AppleEvent;
VAR reply: AppleEvent;
sendMode: AESendMode;
sendPriority: AESendPriority;
timeOutInTicks: LONGINT;
idleProc: IdleProcPtr;
filterProc: EventFilterProcPtr): OSErr;
INLINE $303C,$0D17,$A816;
FUNCTION AEProcessAppleEvent(theEventRecord: EventRecord): OSErr;
INLINE $303C,$021B,$A816;
{ During event processing, an event handler may realize that it is likely
to exceed the client's timeout limit. Passing the reply to this
routine causes a wait event to be generated to ask the client for more time.
FUNCTION AEResetTimer(reply: AppleEvent): OSErr;
INLINE $303C,$0219,$A816;

}

{
*********************************************************************
The following four calls are available for applications which need more sophisticated control
over when and how events are processed. Applications which implement multi-session servers or
which implement their own internal event queueing will probably be the major clients of these
routines.
Can be called from within a handler to prevent the AEM from disposing of
the AppleEvent when the handler returns. Can be used to asynchronously process the
event (as in MacApp). }
FUNCTION AESuspendTheCurrentEvent(theAppleEvent: AppleEvent): OSErr;
INLINE $303C,$022B,$A816;
{
Tells the AppleEvent manager that processing is either about to resume or has
been completed on a previously suspended event. The procPtr passed in as the
dispatcher parameter will be called to attempt to redispatch the event. Several
constants for the dispatcher parameter allow special behavior. They are:
- kAEUseStandardDispatch means redispatch as if the event was just received, using the
standard AppleEvent Dispatcher.
- kAENoDispatch means ignore the parameter.
Use this in the case where no redispatch is needed, and the event has been handled.
- non nil means call the routine which dispatcher points to.
}
FUNCTION AEResumeTheCurrentEvent(theAppleEvent: AppleEvent;
reply: AppleEvent;
dispatcher: EventHandlerProcPtr;
handlerRefcon: LONGINT): OSErr;
INLINE $303C,$0818,$A816;
{ Allows application to examine the currently executing event }
FUNCTION AEGetTheCurrentEvent(VAR theAppleEvent: AppleEvent): OSErr;
INLINE $303C,$021A,$A816;



{ Set the current event to the parameter }
FUNCTION AESetTheCurrentEvent(theAppleEvent: AppleEvent): OSErr;
INLINE $303C,$022C,$A816;
{
*********************************************************************
The following three calls are used to allow applications to behave courteously
when a user interaction such as a dialog box is needed.
}
FUNCTION AEGetInteractionAllowed(VAR level: AEInteractAllowed): OSErr;
INLINE $303C,$021D,$A816;
FUNCTION AESetInteractionAllowed(level: AEInteractAllowed): OSErr;
INLINE $303C,$011E,$A816;
FUNCTION AEInteractWithUser(timeOutInTicks: LONGINT;
nmReqPtr: NMRecPtr;
idleProc: IdleProcPtr): OSErr;
INLINE $303C,$061C,$A816;
{

*********************************************************************
These calls are used to set up and modify the event dispatch table }

{ Add an AppleEvent Handler }
FUNCTION AEInstallEventHandler(theAEEventClass: AEEventClass;
theAEEventID: AEEventID;
handler: EventHandlerProcPtr;
handlerRefcon: LONGINT;
isSysHandler: BOOLEAN): OSErr;
INLINE $303C,$091F,$A816;
{ Remove an AppleEvent Handler }
FUNCTION AERemoveEventHandler(theAEEventClass: AEEventClass;
theAEEventID: AEEventID;
handler: EventHandlerProcPtr;
isSysHandler: BOOLEAN): OSErr;
INLINE $303C,$0720,$A816;
{ Get the corresponding AppleEvent Handler }
FUNCTION AEGetEventHandler(theAEEventClass: AEEventClass;
theAEEventID: AEEventID;
VAR handler: EventHandlerProcPtr;
VAR handlerRefcon: LONGINT;
isSysHandler: BOOLEAN): OSErr;
INLINE $303C,$0921,$A816;
{

*********************************************************************
These calls are used to set up and modify the coercion dispatch table
}
FUNCTION AEInstallCoercionHandler(fromType: DescType;
toType: DescType;
handler: ProcPtr;
handlerRefcon: LONGINT;
fromTypeIsDesc: BOOLEAN;
isSysHandler: BOOLEAN): OSErr;
INLINE $303C, $0A22, $A816;
{

Remove a Coercion Handler

}

FUNCTION AERemoveCoercionHandler(fromType: DescType;
toType: DescType;
handler: ProcPtr;
isSysHandler: BOOLEAN): OSErr;
INLINE $303C, $0723, $A816;
{ Get the corresponding Coercion Handler }
FUNCTION AEGetCoercionHandler(fromType: DescType;
toType: DescType;
VAR handler: ProcPtr;
VAR handlerRefcon: LONGINT;
VAR fromTypeIsDesc: BOOLEAN;
isSysHandler: BOOLEAN): OSErr;
INLINE $303C, $0B24, $A816;
{
*********************************************************************
These calls are used to set up and modify special hooks into the AppleEvent Manager.
Install the special handler named by the Keyword
}
FUNCTION AEInstallSpecialHandler(functionClass: AEKeyword;
handler: ProcPtr;
isSysHandler: BOOLEAN): OSErr;
INLINE $303C, $0500, $A816;
{ Remove the special handler named by the Keyword }
FUNCTION AERemoveSpecialHandler(functionClass: AEKeyword;
handler: ProcPtr;
isSysHandler: BOOLEAN): OSErr;
INLINE $303C, $0501, $A816;
{ Get the special handler named by the Keyword }
FUNCTION AEGetSpecialHandler(functionClass: AEKeyword;
VAR handler: ProcPtr;
isSysHandler: BOOLEAN): OSErr;
INLINE $303C, $052D, $A816;

{$ENDC} { UsingAppleEvents }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE AppleEvents.p}



{#####################################################################}
{### FILE: AppleTalk.p}
{#####################################################################}

{
Created: Sunday, September 15, 1991 at 9:47 PM
AppleTalk.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT AppleTalk;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingAppleTalk}
{$SETC UsingAppleTalk := 1}
{$I+}
{$SETC AppleTalkIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$SETC UsingIncludes := AppleTalkIncludes}
CONST
{ Driver unit and reference numbers (ADSP is dynamic) }
mppUnitNum = 9;
atpUnitNum = 10;
xppUnitNum = 40;
mppRefNum = -10;
atpRefNum = -11;
xppRefNum = -41;

{
{
{
{
{
{

MPP
ATP
XPP
MPP
ATP
XPP

unit number }
unit number }
unit number }
reference number }
reference number }
reference number }

{
{
{
{

This command queued to ourself }
Write out LAP packet }
Detach LAP protocol handler }
Attach LAP protocol handler }

{ .MPP csCodes }
lookupReply = 242;
writeLAP = 243;
detachPH = 244;
attachPH = 245;



writeDDP = 246;
closeSkt = 247;
openSkt = 248;
loadNBP = 249;
lastResident = 249;
confirmName = 250;
lookupName = 251;
removeName = 252;
registerName = 253;
killNBP = 254;
unloadNBP = 255;
setSelfSend = 256;
SetMyZone = 257;
GetATalkInfo = 258;
ATalkClosePrep = 259;

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

Write out DDP packet }
Close DDP socket }
Open DDP socket }
Load NBP command-executing code }
Last resident command }
Confirm name }
Look up name on internet }
Remove name from Names Table }
Register name in Names Table }
Kill outstanding NBP request }
Unload NBP command code }
MPP: Set to allow writes to self }
Set my zone name }
get AppleTalk information }
AppleTalk close query }

{
{
{
{
{
{
{
{
{
{
{
{

NSendRequest code }
Release RspCB }
Close ATP socket }
Add response code | Require open skt }
Send response code }
Get request code }
Open ATP socket }
Send request code }
Release TCB }
Kill GetRequest }
Kill SendRequest }
Kill all getRequests for a skt }

openSess = 255;
closeSess = 254;
userCommand = 253;
userWrite = 252;
getStatus = 251;
afpCall = 250;
getParms = 249;
abortOS = 248;
closeAll = 247;
xCall = 246;

{
{
{
{
{
{
{
{
{
{

Open session }
Close session }
User command }
User write }
Get status }
AFP command (buffer has command code) }
Get parameters }
Abort open session request }
Close all open sessions }
.XPP extended calls }

{ Transition Queue transition types }
ATTransOpen = 0;
ATTransClose = 2;
ATTransClosePrep = 3;
ATTransCancelClose = 4;

{AppleTalk has opened}
{AppleTalk is about to close}
{Is it OK to close AppleTalk ?}
{Cancel the ClosePrep transition}

afpByteRangeLock = 1;
afpVolClose = 2;
afpDirClose = 3;
afpForkClose = 4;
afpCopyFile = 5;
afpDirCreate = 6;

{AFPCall
{AFPCall
{AFPCall
{AFPCall
{AFPCall
{AFPCall

{ .ATP csCodes }
nSendRequest = 248;
relRspCB = 249;
closeATPSkt = 250;
addResponse = 251;
sendResponse = 252;
getRequest = 253;
openATPSkt = 254;
sendRequest = 255;
relTCB = 256;
killGetReq = 257;
killSendReq = 258;
killAllGetReq = 259;
{ .XPP csCodes }

command
command
command
command
command
command

codes}
codes}
codes}
codes}
codes}
codes}



afpFileCreate = 7;
afpDelete = 8;
afpEnumerate = 9;
afpFlush = 10;
afpForkFlush = 11;
afpGetDirParms = 12;
afpGetFileParms = 13;
afpGetForkParms = 14;
afpGetSInfo = 15;
afpGetSParms = 16;
afpGetVolParms = 17;
afpLogin = 18;
afpContLogin = 19;
afpLogout = 20;
afpMapID = 21;
afpMapName = 22;
afpMove = 23;
afpOpenVol = 24;
afpOpenDir = 25;
afpOpenFork = 26;
afpRead = 27;
afpRename = 28;
afpSetDirParms = 29;
afpSetFileParms = 30;
afpSetForkParms = 31;
afpSetVolParms = 32;
afpWrite = 33;
afpGetFlDrParms = 34;
afpSetFlDrParms = 35;
afpDTOpen = 48;
afpDTClose = 49;
afpGetIcon = 51;
afpGtIcnInfo = 52;
afpAddAPPL = 53;
afpRmvAPPL = 54;
afpGetAPPL = 55;
afpAddCmt = 56;
afpRmvCmt = 57;
afpGetCmt = 58;
afpAddIcon = 192;

atpXOvalue = 32;
atpEOMvalue = 16;
atpSTSvalue = 8;
atpTIDValidvalue = 2;
atpSendChkvalue = 1;

zipGetLocalZones = 5;
zipGetZoneList = 6;
zipGetMyZone = 7;
LAPMgrPtr = $B18;
LAPMgrCall = 2;
LAddAEQ = 23;
LRmvAEQ = 24;

{Entry point for LAP Manager}
{Offset to LAP routines}
{LAPAddATQ routine selector}
{LAPRmvATQ routine selector}

TYPE
ABCallType = (tLAPRead,tLAPWrite,tDDPRead,tDDPWrite,tNBPLookup,tNBPConfirm,
tNBPRegister,tATPSndRequest,tATPGetRequest,tATPSdRsp,tATPAddRsp,tATPRequest,
tATPResponse);
ABProtoType = (lapProto,ddpProto,nbpProto,atpProto);

ABByte = 1..127;

LAPAdrBlock = PACKED RECORD
dstNodeID: Byte;
srcNodeID: Byte;
lapProtType: ABByte;
END;
ATQEntryPtr = ^ATQEntry;
ATQEntry = RECORD
qLink: ATQEntryPtr;
qType: INTEGER;
CallAddr: ProcPtr;
END;

{next queue entry}
{queue type}
{pointer to your routine}

AddrBlock = PACKED RECORD
aNet: INTEGER;
aNode: Byte;
aSocket: Byte;
END;
{ Real definition of EntityName is 3 PACKED strings of any length (32 is just an example). No
offests for Asm since each String address must be calculated by adding length byte to last string ptr.
In Pascal, String(32) will be 34 bytes long since fields never start on an odd byte unless they are
only a byte long. So this will generate correct looking interfaces for Pascal and C, but they will not
be the same, which is OK since they are not used. }
EntityPtr = ^EntityName;
EntityName = RECORD
objStr: Str32;
typeStr: Str32;
zoneStr: Str32;
END;
RetransType = PACKED RECORD
retransInterval: Byte;
retransCount: Byte;
END;



BDSElement = RECORD
buffSize: INTEGER;
buffPtr: Ptr;
dataSize: INTEGER;
userBytes: LONGINT;
END;

BDSType = ARRAY [0..7] OF BDSElement;
BDSPtr = ^BDSType;
BitMapType = PACKED ARRAY [0..7] OF BOOLEAN;
ABRecPtr = ^ABusRecord;
ABRecHandle = ^ABRecPtr;
ABusRecord = RECORD
abOpcode: ABCallType;
abResult: INTEGER;
abUserReference: LONGINT;
CASE ABProtoType OF
lapProto:
(lapAddress: LAPAdrBlock;
lapReqCount: INTEGER;
lapActCount: INTEGER;
lapDataPtr: Ptr);
ddpProto:
(ddpType: Byte;
ddpSocket: Byte;
ddpAddress: AddrBlock;
ddpReqCount: INTEGER;
ddpActCount: INTEGER;
ddpDataPtr: Ptr;
ddpNodeID: Byte);
nbpProto:
(nbpEntityPtr: EntityPtr;
nbpBufPtr: Ptr;
nbpBufSize: INTEGER;
nbpDataField: INTEGER;
nbpAddress: AddrBlock;
nbpRetransmitInfo: RetransType);
atpProto:
(atpSocket: Byte;
atpAddress: AddrBlock;
atpReqCount: INTEGER;
atpDataPtr: Ptr;
atpRspBDSPtr: BDSPtr;
atpBitmap: BitMapType;
atpTransID: INTEGER;
atpActCount: INTEGER;
atpUserData: LONGINT;
atpXO: BOOLEAN;
atpEOM: BOOLEAN;
atpTimeOut: Byte;
atpRetries: Byte;
atpNumBufs: Byte;
atpNumRsp: Byte;



atpBDSSize: Byte;
atpRspUData: LONGINT;
atpRspBuf: Ptr;
atpRspSize: INTEGER);
END;
AFPCommandBlock = PACKED RECORD
cmdByte: Byte;
startEndFlag: Byte;
forkRefNum: INTEGER;
rwOffset: LONGINT;
reqCount: LONGINT;
newLineFlag: Byte;
newLineChar: CHAR;
END;
WDSElement = RECORD
entryLength: INTEGER;
entryPtr: Ptr;
END;
NamesTableEntry = RECORD
qLink: QElemPtr;
nteAddress: AddrBlock;
nteData: PACKED ARRAY [1..100] OF CHAR;
END;
MPPParmType = (LAPWriteParm,AttachPHParm,DetachPHParm,OpenSktParm,CloseSktParm,
WriteDDPParm,OpenATPSktParm,CloseATPSktParm,SendRequestParm,GetRequestParm,
SendResponseParm,AddResponseParm,RelTCBParm,RelRspCBParm,RegisterNameParm,
LookupNameParm,ConfirmNameParm,RemoveNameParm,SetSelfSendParm,NSendRequestParm,
KillSendReqParm,KillGetReqParm,KillNBPParm,GetAppleTalkInfoParm,KillAllGetReqParm,
ATalkClosePrepParm,CancelATalkClosePrepParm);
MPPPBPtr = ^MPPParamBlock;
MPPParamBlock = PACKED RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
ioNamePtr: StringPtr;
ioVRefNum: INTEGER;
ioRefNum: INTEGER;
csCode: INTEGER;
CASE MPPParmType OF
LAPWriteParm:
(filler0: INTEGER;
wdsPointer: Ptr);
AttachPHParm,DetachPHParm:
(protType: Byte;
filler1: Byte;
handler: Ptr);
OpenSktParm,CloseSktParm,WriteDDPParm:
(socket: Byte;

{next queue entry}
{queue type}
{routine trap}
{routine address}
{completion routine}
{result code}
{->filename}
{volume reference or drive number}
{driver reference number}
{call command code AUTOMATICALLY set}

{->Write Data Structure}
{ALAP Protocol Type}
{->protocol handler routine}
{socket number}



checksumFlag: Byte;
{checksum flag}
listener: Ptr);
{->socket listener routine}
RegisterNameParm,LookupNameParm,ConfirmNameParm,RemoveNameParm:
(interval: Byte;
{retry interval}
count: Byte;
{retry count}
entityPtr: Ptr;
{->names table element or ->entity name}
CASE MPPParmType OF
RegisterNameParm:
(verifyFlag: Byte;
{set if verify needed}
filler3: Byte);
LookupNameParm:
(retBuffPtr: Ptr;
{->return buffer}
retBuffSize: INTEGER;
{return buffer size}
maxToGet: INTEGER;
{matches to get}
numGotten: INTEGER);
{matched gotten}
ConfirmNameParm:
(confirmAddr: AddrBlock;
{->entity}
newSocket: Byte;
{socket number}
filler4: Byte));
SetSelfSendParm:
(newSelfFlag: Byte;
{self-send toggle flag}
oldSelfFlag: Byte);
{previous self-send state}
KillNBPParm:
(nKillQEl: Ptr);
{ptr to Q element to cancel}
GetAppleTalkInfoParm:
(version: INTEGER;
{requested info version}
varsPtr: Ptr;
{pointer to well known MPP vars}
DCEPtr: Ptr;
{pointer to MPP DCE}
portID: INTEGER;
{port number [0..7]}
configuration: LONGINT;
{32-bit configuration word}
selfSend: INTEGER;
{non zero if SelfSend enabled}
netLo: INTEGER;
{low value of network range}
netHi: INTEGER;
{high value of network range}
ourAddr: LONGINT;
{our 24-bit AppleTalk address}
routerAddr: LONGINT;
{24-bit address of (last) router}
numOfPHs: INTEGER;
{max. number of protocol handlers}
numOfSkts: INTEGER;
{max. number of static sockets}
numNBPEs: INTEGER;
{max. concurrent NBP requests}
nTQueue: Ptr;
{pointer to registered name queue}
LAlength: INTEGER;
{length in bytes of data link addr}
linkAddr: Ptr;
{data link address returned}
zoneName: Ptr);
{zone name returned}
ATalkClosePrepParm:
(appName: Ptr);
{pointer to application name in buffer}
END;
ATPPBPtr = ^ATPParamBlock;
ATPParamBlock = PACKED RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
userData: LONGINT;
reqTID: INTEGER;

{next queue entry}
{queue type}
{routine trap}
{routine address}
{completion routine}
{result code}
{ATP user bytes}
{request transaction ID}



ioRefNum: INTEGER;
{driver reference number}
csCode: INTEGER;
{Call command code automatically set}
atpSocket: Byte;
{currBitMap or socket number}
CASE MPPParmType OF
SendRequestParm, SendResponseParm, GetRequestParm, AddResponseParm, KillSendReqParm:
(atpFlags: Byte;
{control information}
addrBlock: AddrBlock;
{source/dest. socket address}
reqLength: INTEGER;
{request/response length}
reqPointer: Ptr;
{-> request/response data}
bdsPointer: Ptr;
{-> response BDS}
CASE MPPParmType OF
SendRequestParm:
(numOfBuffs: Byte;
{number of responses expected}
timeOutVal: Byte;
{timeout interval}
numOfResps: Byte;
{number of responses actually received}
retryCount: Byte;
{number of retries}
intBuff: INTEGER;
{used internally for NSendRequest}
TRelTime: Byte);
{TRelease time for extended send request}
SendResponseParm:
(filler0: Byte;
{numOfBuffs}
bdsSize: Byte;
{number of BDS elements}
transID: INTEGER);
{transaction ID}
GetRequestParm:
(bitMap: Byte;
{bit map}
filler1: Byte);
AddResponseParm:
(rspNum: Byte;
{sequence number}
filler2: Byte);
KillSendReqParm:
(aKillQEl: Ptr));
{ptr to Q element to cancel}
END;
XPPPrmBlkType = (XPPPrmBlk,ASPSizeBlk,ASPAbortPrm,XCallParam);
XPPSubPrmType = (ASPOpenPrm,ASPSubPrm);
XPPEndPrmType = (AFPLoginPrm,ASPEndPrm);
XPPParmBlkPtr = ^XPPParamBlock;
XPPParamBlock = PACKED RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
cmdResult: LONGINT;
ioVRefNum: INTEGER;
ioRefNum: INTEGER;
csCode: INTEGER;
CASE XPPPrmBlkType OF
ASPAbortPrm:
(abortSCBPtr: Ptr);
ASPSizeBlk:
(aspMaxCmdSize: INTEGER;
aspQuantumSize: INTEGER;

{next queue entry}
{queue type}
{routine trap}
{routine address}
{completion routine}
{result code}
{command result (ATP user bytes)}
{volume reference or drive number}
{driver reference number}
{call command code}

{SCB pointer for AbortOS}
{for SPGetParms}
{for SPGetParms}



numSesss: INTEGER);
XPPPrmBlk:
(sessRefnum: INTEGER;
aspTimeout: Byte;
aspRetry: Byte;
CASE XPPSubPrmType OF
ASPOpenPrm:
(serverAddr: AddrBlock;
scbPointer: Ptr;
attnRoutine: Ptr);
ASPSubPrm:
(cbSize: INTEGER;
cbPtr: Ptr;
rbSize: INTEGER;
rbPtr: Ptr;
CASE XPPEndPrmType OF
AFPLoginPrm:
(afpAddrBlock: AddrBlock;
afpSCBPtr: Ptr;
afpAttnRoutine: Ptr);
ASPEndPrm:
(wdSize: INTEGER;
wdPtr: Ptr;
ccbStart: ARRAY [0..295] OF Byte)));
XCallParam:
(xppSubCode: INTEGER;
xppTimeout: Byte;
xppRetry: Byte;
filler1: INTEGER;
zipBuffPtr: Ptr;
zipNumZones: INTEGER;
zipLastFlag: Byte;
filler2: Byte;
zipInfoField: PACKED ARRAY [1..70] OF Byte);
END;

{for SPGetParms}
{offset to session refnum}
{timeout for ATP}
{retry count for ATP}

{server address block}
{SCB pointer}
{attention routine pointer}
{command block size}
{command block pointer}
{reply buffer size}
{reply buffer pointer}

{address block in AFP login}
{SCB pointer in AFP login}
{Attn routine pointer in AFP login}
{write data size}
{write data pointer}
{afpWrite max size = 296, else 150}

{retry interval (seconds)}
{retry count}
{word space for rent. see the super.}
{pointer to buffer (must be 578 bytes)}
{no. of zone names in this response}
{non-zero if no more zones}
{filler}
{on initial call, set first word to zero}

FUNCTION OpenXPP(VAR xppRefnum: INTEGER): OSErr;
FUNCTION ASPOpenSession(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ASPCloseSession(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ASPAbortOS(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ASPGetParms(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ASPCloseAll(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ASPUserWrite(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ASPUserCommand(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ASPGetStatus(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION AFPCommand(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION GetLocalZones(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION GetZoneList(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION GetMyZone(thePBptr: XPPParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PAttachPH(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PDetachPH(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PWriteLAP(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION POpenSkt(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PCloseSkt(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PWriteDDP(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;



FUNCTION PRegisterName(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PLookupName(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PConfirmName(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PRemoveName(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PSetSelfSend(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PKillNBP(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PGetAppleTalkInfo(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PATalkClosePrep(thePBptr: MPPPBPtr;async: BOOLEAN): OSErr;
FUNCTION POpenATPSkt(thePBptr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PCloseATPSkt(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PSendRequest(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PGetRequest(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PSendResponse(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PAddResponse(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PRelTCB(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PRelRspCB(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PNSendRequest(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PKillSendReq(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION PKillGetReq(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
FUNCTION ATPKillAllGetReq(thePBPtr: ATPPBPtr;async: BOOLEAN): OSErr;
PROCEDURE BuildLAPwds(wdsPtr: Ptr;dataPtr: Ptr;destHost: INTEGER;prototype: INTEGER;
frameLen: INTEGER);
PROCEDURE BuildDDPwds(wdsPtr: Ptr;headerPtr: Ptr;dataPtr: Ptr;netAddr: AddrBlock;
ddpType: INTEGER;dataLen: INTEGER);
PROCEDURE NBPSetEntity(buffer: Ptr;nbpObject: Str32;nbpType: Str32;nbpZone: Str32);
PROCEDURE NBPSetNTE(ntePtr: Ptr;nbpObject: Str32;nbpType: Str32;nbpZone: Str32;
socket: INTEGER);
FUNCTION GetBridgeAddress: INTEGER;
FUNCTION BuildBDS(buffPtr: Ptr;bdsPtr: Ptr;buffSize: INTEGER): INTEGER;
FUNCTION MPPOpen: OSErr;
FUNCTION MPPClose: OSErr;
FUNCTION LAPOpenProtocol(theLAPType: ABByte;protoPtr: Ptr): OSErr;
FUNCTION LAPCloseProtocol(theLAPType: ABByte): OSErr;
FUNCTION LAPWrite(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION LAPRead(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION LAPRdCancel(abRecord: ABRecHandle): OSErr;
FUNCTION LAPAddATQ(theATQEntry: ATQEntryPtr): OSErr;
FUNCTION LAPRmvATQ(theATQEntry: ATQEntryPtr): OSErr;
FUNCTION DDPOpenSocket(VAR theSocket: Byte;sktListener: Ptr): OSErr;
FUNCTION DDPCloseSocket(theSocket: Byte): OSErr;
FUNCTION DDPRead(abRecord: ABRecHandle;retCksumErrs: BOOLEAN;async: BOOLEAN): OSErr;
FUNCTION DDPWrite(abRecord: ABRecHandle;doChecksum: BOOLEAN;async: BOOLEAN): OSErr;
FUNCTION DDPRdCancel(abRecord: ABRecHandle): OSErr;
FUNCTION ATPLoad: OSErr;
FUNCTION ATPUnload: OSErr;
FUNCTION ATPOpenSocket(addrRcvd: AddrBlock;VAR atpSocket: Byte): OSErr;
FUNCTION ATPCloseSocket(atpSocket: Byte): OSErr;
FUNCTION ATPSndRequest(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION ATPRequest(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION ATPReqCancel(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION ATPGetRequest(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION ATPSndRsp(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION ATPAddRsp(abRecord: ABRecHandle): OSErr;
FUNCTION ATPResponse(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION ATPRspCancel(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION NBPRegister(abRecord: ABRecHandle;async: BOOLEAN): OSErr;

FUNCTION NBPLookup(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION NBPExtract(theBuffer: Ptr;numInBuf: INTEGER;whichOne: INTEGER;
VAR abEntity: EntityName;VAR address: AddrBlock): OSErr;
FUNCTION NBPConfirm(abRecord: ABRecHandle;async: BOOLEAN): OSErr;
FUNCTION NBPRemove(abEntity: EntityPtr): OSErr;
FUNCTION NBPLoad: OSErr;
FUNCTION NBPUnload: OSErr;
FUNCTION GetNodeAddress(VAR myNode: INTEGER;VAR myNet: INTEGER): OSErr;
FUNCTION IsMPPOpen: BOOLEAN;
FUNCTION IsATPOpen: BOOLEAN;
PROCEDURE ATEvent(event: LONGINT;infoPtr: Ptr);
FUNCTION ATPreFlightEvent(event: LONGINT;cancel: LONGINT;infoPtr: Ptr): OSErr;

{$ENDC} { UsingAppleTalk }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE AppleTalk.p}



{#####################################################################}
{### FILE: Balloons.p}
{#####################################################################}

{
Created: Sunday, September 15, 1991 at 9:56 PM
Balloons.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1990-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Balloons;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingBalloons}
{$SETC UsingBalloons := 1}
{$I+}
{$SETC BalloonsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$IFC UNDEFINED UsingMenus}
{$I $$Shell(PInterfaces)Menus.p}
{$ENDC}
{$IFC UNDEFINED UsingTextEdit}
{$I $$Shell(PInterfaces)TextEdit.p}
{$ENDC}
{$IFC UNDEFINED UsingTraps}
{$I $$Shell(PInterfaces)Traps.p}
{$ENDC}
{$SETC UsingIncludes := BalloonsIncludes}
CONST
hmBalloonHelpVersion = $0002;

{ The real version of the Help Manager }

{Help Mgr error range: -850 to -874}
hmHelpDisabled = -850;
{ Show Balloons mode was off, call to routine ignored }
hmBalloonAborted = -853;
{ Returned if mouse was moving or mouse wasn't in window port rect }
hmSameAsLastBalloon = -854;
{ Returned from HMShowMenuBalloon if menu & item is same as last time }
hmHelpManagerNotInited = -855; { Returned from HMGetHelpMenuHandle if help menu not setup }



hmSkippedBalloon = -857;
hmWrongVersion = -858;
hmUnknownHelpType = -859;
hmOperationUnsupported = -861;
hmNoBalloonUp = -862;
hmCloseViewActive = -863;

{
{
{
{
{
{

Returned
Returned
Returned
Returned
Returned
Returned

from calls if helpmsg specified a skip balloon }
if help mgr resource was the wrong version }
if help msg record contained a bad type }
from HMShowBalloon call if bad method passed to routine }
from HMRemoveBalloon if no balloon was visible when call was made }
from HMRemoveBalloon if CloseView was active }

kHMHelpMenuID = -16490;
kHMAboutHelpItem = 1;
kHMShowBalloonsItem = 3;

{ Resource ID and menu ID of help menu }
{ help menu item number of About Balloon Help… }
{ help menu item number of Show/Hide Balloons }

kHMHelpID = -5696;
kBalloonWDEFID = 126;

{ ID of various Help Mgr package resources (in Pack14 range) }
{ Resource ID of the WDEF proc used in standard balloons }

{ Dialog item template type constant }
helpItem = 1;
{ key value in DITL template that corresponds to the help item }
{ Options for Help Manager resources in 'hmnu', 'hdlg', 'hrct', 'hovr', & 'hfdr' resources }
hmDefaultOptions = 0;
{ default options for help manager resources }
hmUseSubID = 1;
{ treat resID's in resources as subID's of driver base ID (for Desk Accessories) }
hmAbsoluteCoords = 2;
{ ignore window port origin and treat rectangles as absolute coords (local to window) }
hmSaveBitsNoWindow = 4;
{ don't create a window, just blast bits on screen. No update event is generated }
hmSaveBitsWindow = 8;
{ create a window, but restore bits behind window when window goes away & generate update event }
hmMatchInTitle = 16;
{ for hwin resources, match string anywhere in window title string }
{ Constants for Help Types in 'hmnu', 'hdlg', 'hrct', 'hovr', & 'hfdr' resources }
kHMStringItem = 1;
{ pstring used in resource }
kHMPictItem = 2;
{ 'PICT' ResID used in resource }
kHMStringResItem = 3;
{ 'STR#' ResID & index used in resource }
kHMTEResItem = 6;
{ Styled Text Edit ResID used in resource ('TEXT' & 'styl') }
kHMSTRResItem = 7;
{ 'STR ' ResID used in resource }
kHMSkipItem = 256;
{ don't display a balloon }
kHMCompareItem = 512;
{ Compare pstring in menu item w/ PString in resource item ('hmnu' only) }
kHMNamedResourceItem = 1024;
{ Use pstring in menu item to get 'STR#', 'PICT', or 'STR ' resource ('hmnu' only) }
kHMTrackCntlItem = 2048;
{ Reserved }
{ Constants for hmmHelpType's when filling out HMMessageRecord }
khmmString = 1;
{ help message contains a PString }
khmmPict = 2;
{ help message contains a resource ID to a 'PICT' resource }
khmmStringRes = 3;
{ help message contains a res ID & index to a 'STR#' resource }
khmmTEHandle = 4;
{ help message contains a Text Edit handle }
khmmPictHandle = 5;
{ help message contains a Picture handle }
khmmTERes = 6;
{ help message contains a res ID to 'TEXT' & 'styl' resources }
khmmSTRRes = 7;
{ help message contains a res ID to a 'STR ' resource }
{ ResTypes for Styled TE Handles in Resources }
kHMTETextResType = 'TEXT';
{ Resource Type of text data for styled TE record w/o style info }
kHMTEStyleResType = 'styl';
{ Resource Type of style information for styled TE record }
{ Generic defines for the state parameter used when extracting 'hmnu' & 'hdlg' messages }
kHMEnabledItem = 0;
{ item is enabled, but not checked or control value = 0 }
kHMDisabledItem = 1;
{ item is disabled, grayed in menus or disabled in dialogs }
kHMCheckedItem = 2;
{ item is enabled, and checked or control value = 1 }
kHMOtherItem = 3;
{ item is enabled, and control value > 1 }
{ Resource Types for whichType parameter used when extracting 'hmnu' & 'hdlg' messages }



kHMMenuResType = 'hmnu';
kHMDialogResType = 'hdlg';
kHMWindListResType = 'hwin';
kHMRectListResType = 'hrct';
kHMOverrideResType = 'hovr';
kHMFinderApplResType = 'hfdr';

{
{
{
{
{
{

ResType
ResType
ResType
ResType
ResType
ResType

of
of
of
of
of
of

help
help
help
help
help
help

resource
resource
resource
resource
resource
resource

for
for
for
for
for
for

supporting menus }
supporting dialogs }
supporting windows }
rectangles in windows }
overriding system balloons }
custom balloon in Finder }

{ Method parameters to pass to HMShowBalloon }
kHMRegularWindow = 0;
{ Create a regular window floating above all windows }
kHMSaveBitsNoWindow = 1;
{ Just save the bits and draw (for MDEF calls) }
kHMSaveBitsWindow = 2;
{ Regular window, save bits behind, AND generate update event }
TYPE
HMStringResType = RECORD
hmmResID: INTEGER;
hmmIndex: INTEGER;
END;
HMMessageRecPtr = ^HMMessageRecord;
HMMessageRecord = RECORD
	hmmHelpType : INTEGER;
	CASE INTEGER OF
		khmmString: (hmmString: STR255);
		khmmPict: 	(hmmPict: INTEGER);
	khmmStringRes:	(hmmStringRes: HMStringResType);
	khmmTEHandle:	(hmmTEHandle: TEHandle);
	khmmPictHandle:	(hmmPictHandle: PicHandle);
	khmmTERes:		(hmmTERes: INTEGER);
	khmmSTRRes:		(hmmSTRRes: INTEGER);
END;

{ Public Interfaces }
FUNCTION HMGetHelpMenuHandle(VAR mh: MenuHandle): OSErr;
INLINE $303C,$0200,_Pack14;
FUNCTION HMShowBalloon(aHelpMsg: HMMessageRecord;
tip: Point;
alternateRect: RectPtr;
tipProc: Ptr;
theProc: INTEGER;
variant: INTEGER;
method: INTEGER): OSErr;
INLINE $303C,$0B01,_Pack14;
FUNCTION HMRemoveBalloon: OSErr;
INLINE $303C,$0002,_Pack14;
FUNCTION HMGetBalloons: BOOLEAN;
INLINE $303C,$0003,_Pack14;
FUNCTION HMSetBalloons(flag: BOOLEAN): OSErr;



INLINE $303C,$0104,_Pack14;
FUNCTION HMShowMenuBalloon(itemNum: INTEGER;
itemMenuID: INTEGER;
itemFlags: LONGINT;
itemReserved: LONGINT;
tip: Point;
alternateRect: RectPtr;
tipProc: Ptr;
theProc: INTEGER;
variant: INTEGER): OSErr;
INLINE $303C,$0E05,_Pack14;
FUNCTION HMGetIndHelpMsg(whichType: ResType;
whichResID: INTEGER;
whichMsg: INTEGER;
whichState: INTEGER;
VAR options: LONGINT;
VAR tip: Point;
VAR altRect: Rect;
VAR theProc: INTEGER;
VAR variant: INTEGER;
VAR aHelpMsg: HMMessageRecord;
VAR count: INTEGER): OSErr;
INLINE $303C,$1306,_Pack14;
FUNCTION HMIsBalloon: BOOLEAN;
INLINE $303C,$0007,_Pack14;
FUNCTION HMSetFont(font: INTEGER): OSErr;
INLINE $303C,$0108,_Pack14;
FUNCTION HMSetFontSize(fontSize: INTEGER): OSErr;
INLINE $303C,$0109,_Pack14;
FUNCTION HMGetFont(VAR font: INTEGER): OSErr;
INLINE $303C,$020A,_Pack14;
FUNCTION HMGetFontSize(VAR fontSize: INTEGER): OSErr;
INLINE $303C,$020B,_Pack14;
FUNCTION HMSetDialogResID(resID: INTEGER): OSErr;
INLINE $303C,$010C,_Pack14;
FUNCTION HMSetMenuResID(menuID: INTEGER;
resID: INTEGER): OSErr;
INLINE $303C,$020D,_Pack14;
FUNCTION HMBalloonRect(aHelpMsg: HMMessageRecord;
VAR coolRect: Rect): OSErr;
INLINE $303C,$040E,_Pack14;
FUNCTION HMBalloonPict(aHelpMsg: HMMessageRecord;
VAR coolPict: PicHandle): OSErr;
INLINE $303C,$040F,_Pack14;
FUNCTION HMScanTemplateItems(whichID: INTEGER;
whichResFile: INTEGER;
whichType: ResType): OSErr;
INLINE $303C,$0410,_Pack14;
FUNCTION HMExtractHelpMsg(whichType: ResType;whichResID: INTEGER;whichMsg: INTEGER;
whichState: INTEGER;VAR aHelpMsg: HMMessageRecord): OSErr;
INLINE $303C,$0711,_Pack14;
FUNCTION HMGetDialogResID(VAR resID: INTEGER): OSErr;
INLINE $303C,$0213,_Pack14;
FUNCTION HMGetMenuResID(menuID: INTEGER;VAR resID: INTEGER): OSErr;
INLINE $303C,$0314,_Pack14;
FUNCTION HMGetBalloonWindow(VAR window: WindowPtr): OSErr;


INLINE $303C,$0215,_Pack14;

{$ENDC} { UsingBalloons }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Balloons.p}





{#####################################################################}
{### FILE: CommResources.p}
{#####################################################################}

{
Created: Thursday, September 12, 1991 at 11:54 AM
CommResources.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT CommResources;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingCommResources}
{$SETC UsingCommResources := 1}
{$I+}
{$SETC CommResourcesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$SETC UsingIncludes := CommResourcesIncludes}
CONST
{
version of the Comm Resource Manager
curCRMVersion = 2;
{
tool
classCM =
classFT =
classTM =

classes (also the tool file types)
'cbnd';
'fbnd';
'tbnd';

}

}

{ constants general to the use of the Communications Resource Manager }
crmType = 9;
{ queue type
}
crmRecVersion = 1;
{ version of queue structure }
{
error codes }
crmGenericError = -1;
crmNoErr = 0;
TYPE
{ data structures general to the use of the Communications Resource Manager }



CRMErr = OSErr;
CRMRecPtr = ^CRMRec;
CRMRec = RECORD
qLink: QElemPtr;
qType: INTEGER;
crmVersion: INTEGER;
crmPrivate: LONGINT;
crmReserved: INTEGER;
crmDeviceType: LONGINT;
crmDeviceID: LONGINT;
crmAttributes: LONGINT;
crmStatus: LONGINT;
crmRefCon: LONGINT;
END;

{reserved}
{queue type -- ORD(crmType) = 9}
{version of queue element data structure}
{reserved}
{reserved}
{type of device, assigned by DTS}
{device ID; assigned when CRMInstall is called}
{pointer to attribute block}
{status variable - device specific}
{for device private use}

FUNCTION InitCRM: CRMErr;
FUNCTION CRMGetHeader: QHdrPtr;
PROCEDURE CRMInstall(crmReqPtr: QElemPtr);
FUNCTION CRMRemove(crmReqPtr: QElemPtr): OSErr;
FUNCTION CRMSearch(crmReqPtr: QElemPtr): QElemPtr;
FUNCTION CRMGetCRMVersion: INTEGER;
FUNCTION CRMGetResource(theType: ResType;theID: INTEGER): Handle;
FUNCTION CRMGet1Resource(theType: ResType;theID: INTEGER): Handle;
FUNCTION CRMGetIndResource(theType: ResType;index: INTEGER): Handle;
FUNCTION CRMGet1IndResource(theType: ResType;index: INTEGER): Handle;
FUNCTION CRMGetNamedResource(theType: ResType;name: Str255): Handle;
FUNCTION CRMGet1NamedResource(theType: ResType;name: Str255): Handle;
PROCEDURE CRMReleaseResource(theHandle: Handle);
FUNCTION CRMGetToolResource(procID: INTEGER;theType: ResType;theID: INTEGER): Handle;
FUNCTION CRMGetToolNamedResource(procID: INTEGER;theType: ResType;name: Str255): Handle;
PROCEDURE CRMReleaseToolResource(procID: INTEGER;theHandle: Handle);
FUNCTION CRMGetIndex(theHandle: Handle): LONGINT;
FUNCTION CRMLocalToRealID(bundleType: ResType;toolID: INTEGER;theType: ResType;
localID: INTEGER): INTEGER;
FUNCTION CRMRealToLocalID(bundleType: ResType;toolID: INTEGER;theType: ResType;
realID: INTEGER): INTEGER;
FUNCTION CRMGetIndToolName(bundleType: OSType;index: INTEGER;VAR toolName: Str255): OSErr;
FUNCTION CRMFindCommunications(VAR vRefNum: INTEGER;VAR dirID: LONGINT): OSErr;
FUNCTION CRMIsDriverOpen(driverName: Str255): BOOLEAN;
FUNCTION CRMParseCAPSResource(theHandle: Handle;selector: ResType;VAR value: LONGINT): CRMErr;
FUNCTION CRMReserveRF(refNum: INTEGER): OSErr;
{ decrements useCount by one }
FUNCTION CRMReleaseRF(refNum: INTEGER): OSErr;


{$ENDC} { UsingCommResources }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE CommResources.p}





{#####################################################################}
{### FILE: Connections.p}
{#####################################################################}

{
Created: Wednesday, September 11, 1991 at 11:34 AM
Connections.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Connections;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingConnections}
{$SETC UsingConnections := 1}
{$I+}
{$SETC ConnectionsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$IFC UNDEFINED UsingCTBUtilities}
{$I $$Shell(PInterfaces)CTBUtilities.p}
{$ENDC}
{$SETC UsingIncludes := ConnectionsIncludes}
CONST
{
current Connection Manager version
curCMVersion = 2;

}

{
current Connection Manager Environment Record version
curConnEnvRecVers = 0;
{ CMErr }
cmGenericError = -1;
cmNoErr = 0;
cmRejected = 1;
cmFailed = 2;
cmTimeOut = 3;
cmNotOpen = 4;
cmNotClosed = 5;
cmNoRequestPending = 6;

}



cmNotSupported = 7;
cmNoTools = 8;
cmUserCancel = 9;
cmUnknownError = 11;
TYPE
CMErr = OSErr;
CONST
{ Low word of CMRecFlags is same as CMChannel }
cmData = $00000001;
cmCntl = $00000002;
cmAttn = $00000004;
cmDataNoTimeout = $00000010;
cmCntlNoTimeout = $00000020;
cmAttnNoTimeout = $00000040;
cmDataClean = $00000100;
cmCntlClean = $00000200;
cmAttnClean = $00000400;
{ only for CMRecFlags (not CMChannel) below this point }
cmNoMenus = $00010000;
cmQuiet = $00020000;
cmConfigChanged = $00040000;
TYPE
CMRecFlags = LONGINT;

CMChannel = INTEGER;
CONST
{ CMStatFlags }
cmStatusOpening = $00000001;
cmStatusOpen = $00000002;
cmStatusClosing = $00000004;
cmStatusDataAvail = $00000008;
cmStatusCntlAvail = $00000010;
cmStatusAttnAvail = $00000020;
cmStatusDRPend = $00000040;
{data read pending}
cmStatusDWPend = $00000080;
{data write pending}
cmStatusCRPend = $00000100;
{cntl read pending}
cmStatusCWPend = $00000200;
{cntl write pending}
cmStatusARPend = $00000400;
{attn read pending}
cmStatusAWPend = $00000800;
{attn write pending}
cmStatusBreakPend = $00001000;
cmStatusListenPend = $00002000;
cmStatusIncomingCallPresent = $00004000;
cmStatusReserved0 = $00008000;
TYPE
CMStatFlags = LONGINT;
CMBufFields = (cmDataIn,cmDataOut,cmCntlIn,cmCntlOut,cmAttnIn,cmAttnOut,



cmRsrvIn,cmRsrvOut);

CMBuffers = ARRAY[CMBufFields] OF Ptr;
CMBufferSizes = ARRAY[CMBufFields] OF LONGINT;
CONST
{ CMSearchFlags }
cmSearchSevenBit = $0001;
TYPE
CMSearchFlags = INTEGER;

CONST
{ CMFlags }
cmFlagsEOM = $0001;
TYPE
CMFlags = INTEGER;

ConnEnvironRecPtr = ^ConnEnvironRec;
ConnEnvironRec = RECORD
version: INTEGER;
baudRate: LONGINT;
dataBits: INTEGER;
channels: CMChannel;
swFlowControl: BOOLEAN;
hwFlowControl: BOOLEAN;
flags: CMFlags;
END;
ConnPtr = ^ConnRecord;
ConnHandle = ^ConnPtr;
ConnRecord = RECORD
procID: INTEGER;
flags: CMRecFlags;
errCode: CMErr;
refCon: LONGINT;
userData: LONGINT;
defProc: ProcPtr;
config: Ptr;
oldConfig: Ptr;
asyncEOM: LONGINT;
reserved1: LONGINT;
reserved2: LONGINT;
cmPrivate: Ptr;
bufferArray: CMBuffers;
bufSizes: CMBufferSizes;
mluField: LONGINT;
asyncCount: CMBufferSizes;
END;



{ application routines type definitions }
ConnectionSearchCallBackProcPtr = ProcPtr;
ConnectionCompletionProcPtr = ProcPtr;
ConnectionChooseIdleProcPtr = ProcPtr;
CONST
{ CMIOPB constants and structure }
cmIOPBQType = 10;
cmIOPBversion = 0;
TYPE
CMIOPBPtr = ^CMIOPB;
CMIOPB = RECORD
qLink: QElemPtr;
qType: INTEGER;
{ cmIOPBQType }
hConn: ConnHandle;
theBuffer: Ptr;
count: LONGINT;
flags: CMFlags;
userCompletion: ConnectionCompletionProcPtr;
timeout: LONGINT;
errCode: CMErr;
channel: CMChannel;
asyncEOM: LONGINT;
reserved1: LONGINT;
reserved2: INTEGER;
version: INTEGER;
{ cmIOPBversion }
refCon: LONGINT;
{ for application }
toolData1: LONGINT;
{ for tool }
toolData2: LONGINT;
{ for tool }
END;

FUNCTION InitCM: CMErr;
FUNCTION CMGetVersion(hConn: ConnHandle): Handle;
FUNCTION CMGetCMVersion: INTEGER;
FUNCTION CMNew(procID: INTEGER;flags: CMRecFlags;desiredSizes: CMBufferSizes;
refCon: LONGINT;userData: LONGINT): ConnHandle;
PROCEDURE CMDispose(hConn: ConnHandle);
FUNCTION CMListen(hConn: ConnHandle;async: BOOLEAN;completor: ConnectionCompletionProcPtr;
timeout: LONGINT): CMErr;
FUNCTION CMAccept(hConn: ConnHandle;accept: BOOLEAN): CMErr;
FUNCTION CMOpen(hConn: ConnHandle;async: BOOLEAN;completor: ConnectionCompletionProcPtr;
timeout: LONGINT): CMErr;
FUNCTION CMClose(hConn: ConnHandle;async: BOOLEAN;completor: ConnectionCompletionProcPtr;
timeout: LONGINT;now: BOOLEAN): CMErr;
FUNCTION CMAbort(hConn: ConnHandle): CMErr;
FUNCTION CMStatus(hConn: ConnHandle;VAR sizes: CMBufferSizes;VAR flags: CMStatFlags): CMErr;



PROCEDURE CMIdle(hConn: ConnHandle);
PROCEDURE CMReset(hConn: ConnHandle);
PROCEDURE CMBreak(hConn: ConnHandle;duration: LONGINT;async: BOOLEAN;completor: ConnectionCompletionProcPtr);
FUNCTION CMRead(hConn: ConnHandle;theBuffer: Ptr;VAR toRead: LONGINT;theChannel: CMChannel;
async: BOOLEAN;completor: ConnectionCompletionProcPtr;timeout: LONGINT;
VAR flags: CMFlags): CMErr;
FUNCTION CMWrite(hConn: ConnHandle;theBuffer: Ptr;VAR toWrite: LONGINT;
theChannel: CMChannel;async: BOOLEAN;completor: ConnectionCompletionProcPtr;
timeout: LONGINT;flags: CMFlags): CMErr;
FUNCTION CMIOKill(hConn: ConnHandle;which: INTEGER): CMErr;
PROCEDURE CMActivate(hConn: ConnHandle;activate: BOOLEAN);
PROCEDURE CMResume(hConn: ConnHandle;resume: BOOLEAN);
FUNCTION CMMenu(hConn: ConnHandle;menuID: INTEGER;item: INTEGER): BOOLEAN;
FUNCTION CMValidate(hConn: ConnHandle): BOOLEAN;
PROCEDURE CMDefault(VAR theConfig: Ptr;procID: INTEGER;allocate: BOOLEAN);
FUNCTION CMSetupPreflight(procID: INTEGER;VAR magicCookie: LONGINT): Handle;
FUNCTION CMSetupFilter(procID: INTEGER;theConfig: Ptr;count: INTEGER;theDialog: DialogPtr;
VAR theEvent: EventRecord;VAR theItem: INTEGER;VAR magicCookie: LONGINT): BOOLEAN;
PROCEDURE CMSetupSetup(procID: INTEGER;theConfig: Ptr;count: INTEGER;theDialog: DialogPtr;
VAR magicCookie: LONGINT);
PROCEDURE CMSetupItem(procID: INTEGER;theConfig: Ptr;count: INTEGER;theDialog: DialogPtr;
VAR theItem: INTEGER;VAR magicCookie: LONGINT);
PROCEDURE CMSetupXCleanup(procID: INTEGER;theConfig: Ptr;count: INTEGER;
theDialog: DialogPtr;OKed: BOOLEAN;VAR magicCookie: LONGINT);
PROCEDURE CMSetupPostflight(procID: INTEGER);
FUNCTION CMGetConfig(hConn: ConnHandle): Ptr;
FUNCTION CMSetConfig(hConn: ConnHandle;thePtr: Ptr): INTEGER;
FUNCTION CMIntlToEnglish(hConn: ConnHandle;inputPtr: Ptr;VAR outputPtr: Ptr;
language: INTEGER): OSErr;
FUNCTION CMEnglishToIntl(hConn: ConnHandle;inputPtr: Ptr;VAR outputPtr: Ptr;
language: INTEGER): OSErr;
FUNCTION CMAddSearch(hConn: ConnHandle;theString: Str255;flags: CMSearchFlags;
callBack: ConnectionSearchCallBackProcPtr): LONGINT;
PROCEDURE CMRemoveSearch(hConn: ConnHandle;refnum: LONGINT);
PROCEDURE CMClearSearch(hConn: ConnHandle);
FUNCTION CMGetConnEnvirons(hConn: ConnHandle;VAR theEnvirons: ConnEnvironRec): CMErr;
FUNCTION CMChoose(VAR hConn: ConnHandle;where: Point;idleProc: ConnectionChooseIdleProcPtr): INTEGER;
PROCEDURE CMEvent(hConn: ConnHandle;theEvent: EventRecord);
PROCEDURE CMGetToolName(procID: INTEGER;VAR name: Str255);
FUNCTION CMGetProcID(name: Str255): INTEGER;
PROCEDURE CMSetRefCon(hConn: ConnHandle;refCon: LONGINT);

FUNCTION CMGetRefCon(hConn: ConnHandle): LONGINT;
FUNCTION CMGetUserData(hConn: ConnHandle): LONGINT;
PROCEDURE CMSetUserData(hConn: ConnHandle;userData: LONGINT);
PROCEDURE CMGetErrorString(hConn: ConnHandle;id: INTEGER;VAR errMsg: Str255);
FUNCTION CMNewIOPB(hConn: ConnHandle;VAR theIOPB: CMIOPBPtr): CMErr;
FUNCTION CMDisposeIOPB(hConn: ConnHandle;theIOPB: CMIOPBPtr): CMErr;
FUNCTION CMPBRead(hConn: ConnHandle;theIOPB: CMIOPBPtr;async: BOOLEAN): CMErr;
FUNCTION CMPBWrite(hConn: ConnHandle;theIOPB: CMIOPBPtr;async: BOOLEAN): CMErr;
FUNCTION CMPBIOKill(hConn: ConnHandle;theIOPB: CMIOPBPtr): CMErr;

{$ENDC} { UsingConnections }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Connections.p}



{#####################################################################}
{### FILE: ConnectionTools.p}
{#####################################################################}

{
Created: Wednesday, September 11, 1991 at 1:48 PM
ConnectionTools.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT ConnectionTools;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingConnectionTools}
{$SETC UsingConnectionTools := 1}
{$I+}
{$SETC ConnectionToolsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$IFC UNDEFINED UsingConnections}
{$I $$Shell(PInterfaces)Connections.p}
{$ENDC}
{$SETC UsingIncludes := ConnectionToolsIncludes}
CONST
{ messages for DefProc }
cmInitMsg = 0;
cmDisposeMsg = 1;
cmSuspendMsg = 2;
cmResumeMsg = 3;
cmMenuMsg = 4;
cmEventMsg = 5;
cmActivateMsg = 6;
cmDeactivateMsg = 7;
cmIdleMsg = 50;
cmResetMsg = 51;
cmAbortMsg = 52;
cmReadMsg = 100;



cmWriteMsg = 101;
cmStatusMsg = 102;
cmListenMsg = 103;
cmAcceptMsg = 104;
cmCloseMsg = 105;
cmOpenMsg = 106;
cmBreakMsg = 107;
cmIOKillMsg = 108;
cmEnvironsMsg = 109;
{ new connection tool messages for ctb 1.1 }
cmNewIOPBMsg = 110;
cmDisposeIOPBMsg = 111;
cmGetErrorStringMsg = 112;
cmPBReadMsg = 113;
cmPBWriteMsg = 114;
cmPBIOKillMsg = 115;
{
messages for validate DefProc
cmValidateMsg = 0;
cmDefaultMsg = 1;
{
messages for Setup DefProc
cmSpreflightMsg = 0;
cmSsetupMsg = 1;
cmSitemMsg = 2;
cmSfilterMsg = 3;
cmScleanupMsg = 4;

}

}

{
messages for scripting defProc
cmMgetMsg = 0;
cmMsetMsg = 1;
{
messages for localization defProc
cmL2English = 0;
cmL2Intl = 1;

}

}

{ private data constants }
cdefType = 'cdef'; { main connection definition procedure }
cvalType = 'cval'; { validation definition procedure }
csetType = 'cset'; { connection setup definition procedure }
clocType = 'cloc'; { connection configuration localization defProc }
cscrType = 'cscr'; { connection scripting defProc interfaces }
cbndType = 'cbnd';
cverType = 'vers';

{ bundle type for connection }

TYPE
CMDataBufferPtr = ^CMDataBuffer;
CMDataBuffer = RECORD
thePtr: Ptr;
count: LONGINT;
channel: CMChannel;
flags: CMFlags;
END;


CMCompletorPtr = ^CMCompletorRecord;
CMCompletorRecord = RECORD
async: BOOLEAN;
completionRoutine: ProcPtr;
END;
{ Private Data Structure }
CMSetupPtr = ^CMSetupStruct;
CMSetupStruct = RECORD
theDialog: DialogPtr;
count: INTEGER;
theConfig: Ptr;
procID: INTEGER;
{ procID of the tool }
END;

{$ENDC} { UsingConnectionTools }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE ConnectionTools.p}



{#####################################################################}
{### FILE: Controls.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:25 PM
Controls.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1990
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Controls;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingControls}
{$SETC UsingControls := 1}
{$I+}
{$SETC ControlsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := ControlsIncludes}
CONST
pushButProc = 0;
checkBoxProc = 1;
radioButProc = 2;
useWFont = 8;
scrollBarProc = 16;
inButton = 10;
inCheckBox = 11;
inUpButton = 20;
inDownButton = 21;
inPageUp = 22;
inPageDown = 23;
inThumb = 129;
popupMenuProc = 1008;
popupFixedWidth = $0001;
popupUseAddResMenu = $0004;
popupUseWFont = $0008;
popupTitleBold = $00000100;
popupTitleItalic = $00000200;
popupTitleUnderline = $00000400;

{ 63 * 16 }
{ popup menu CDEF variation codes }

{

Popup Title characteristics }



popupTitleOutline = $00000800;
popupTitleShadow = $00001000;
popupTitleCondense = $00002000;
popupTitleExtend = $00004000;
popupTitleNoStyle = $00008000;
popupTitleLeftJust = $00000000;
popupTitleCenterJust = $00000001;
popupTitleRightJust = $000000FF;
{
axis constraints for DragGrayRgn call}
noConstraint = 0;
hAxisOnly = 1;
vAxisOnly = 2;
{
control messages}
drawCntl = 0;
testCntl = 1;
calcCRgns = 2;
initCntl = 3;
dispCntl = 4;
posCntl = 5;
thumbCntl = 6;
dragCntl = 7;
autoTrack = 8;
calcCntlRgn = 10;
calcThumbRgn = 11;
cFrameColor = 0;
cBodyColor = 1;
cTextColor = 2;
cThumbColor = 3;
popupMenuCDEFproc = popupMenuProc;
TYPE
ControlPtr = ^ControlRecord;
ControlHandle = ^ControlPtr;
ControlRecord = PACKED RECORD
nextControl: ControlHandle;
contrlOwner: WindowPtr;
contrlRect: Rect;
contrlVis: Byte;
contrlHilite: Byte;
contrlValue: INTEGER;
contrlMin: INTEGER;
contrlMax: INTEGER;
contrlDefProc: Handle;
contrlData: Handle;
contrlAction: ProcPtr;
contrlRfCon: LONGINT;
contrlTitle: Str255;
END;
CCTabPtr = ^CtlCTab;
CCTabHandle = ^CCTabPtr;

{ synonym for compatibility }



CtlCTab = RECORD
ccSeed: LONGINT;
{reserved}
ccRider: INTEGER;
{see what you have done - reserved}
ctSize: INTEGER;
{usually 3 for controls}
ctTable: ARRAY [0..3] OF ColorSpec;
END;
AuxCtlPtr = ^AuxCtlRec;
AuxCtlHandle = ^AuxCtlPtr;
AuxCtlRec = RECORD
acNext: AuxCtlHandle;
acOwner: ControlHandle;
acCTable: CCTabHandle;
acFlags: INTEGER;
acReserved: LONGINT;
acRefCon: LONGINT;
END;

{handle to next AuxCtlRec}
{handle for aux record's control}
{color table for this control}
{misc flag byte}
{reserved for use by Apple}
{for use by application}

FUNCTION NewControl(theWindow: WindowPtr;boundsRect: Rect;title: Str255;
visible: BOOLEAN;value: INTEGER;min: INTEGER;max: INTEGER;procID: INTEGER;
refCon: LONGINT): ControlHandle;
INLINE $A954;
PROCEDURE SetCTitle(theControl: ControlHandle;title: Str255);
INLINE $A95F;
PROCEDURE GetCTitle(theControl: ControlHandle;VAR title: Str255);
INLINE $A95E;
FUNCTION GetNewControl(controlID: INTEGER;owner: WindowPtr): ControlHandle;
INLINE $A9BE;
PROCEDURE DisposeControl(theControl: ControlHandle);
INLINE $A955;
PROCEDURE KillControls(theWindow: WindowPtr);
INLINE $A956;
PROCEDURE HideControl(theControl: ControlHandle);
INLINE $A958;
PROCEDURE ShowControl(theControl: ControlHandle);
INLINE $A957;
PROCEDURE DrawControls(theWindow: WindowPtr);
INLINE $A969;
PROCEDURE Draw1Control(theControl: ControlHandle);
INLINE $A96D;
PROCEDURE HiliteControl(theControl: ControlHandle;hiliteState: INTEGER);
INLINE $A95D;
PROCEDURE UpdtControl(theWindow: WindowPtr;updateRgn: RgnHandle);
INLINE $A953;
PROCEDURE UpdateControls(theWindow: WindowPtr;updateRgn: RgnHandle);
INLINE $A953;
PROCEDURE MoveControl(theControl: ControlHandle;h: INTEGER;v: INTEGER);
INLINE $A959;
PROCEDURE SizeControl(theControl: ControlHandle;w: INTEGER;h: INTEGER);
INLINE $A95C;
PROCEDURE SetCtlValue(theControl: ControlHandle;theValue: INTEGER);
INLINE $A963;
FUNCTION GetCtlValue(theControl: ControlHandle): INTEGER;
INLINE $A960;
PROCEDURE SetCtlMin(theControl: ControlHandle;minValue: INTEGER);



INLINE $A964;
FUNCTION GetCtlMin(theControl: ControlHandle): INTEGER;
INLINE $A961;
PROCEDURE SetCtlMax(theControl: ControlHandle;maxValue: INTEGER);
INLINE $A965;
FUNCTION GetCtlMax(theControl: ControlHandle): INTEGER;
INLINE $A962;
PROCEDURE SetCRefCon(theControl: ControlHandle;data: LONGINT);
INLINE $A95B;
FUNCTION GetCRefCon(theControl: ControlHandle): LONGINT;
INLINE $A95A;
PROCEDURE SetCtlAction(theControl: ControlHandle;actionProc: ProcPtr);
INLINE $A96B;
FUNCTION GetCtlAction(theControl: ControlHandle): ProcPtr;
INLINE $A96A;
PROCEDURE DragControl(theControl: ControlHandle;startPt: Point;limitRect: Rect;
slopRect: Rect;axis: INTEGER);
INLINE $A967;
FUNCTION TestControl(theControl: ControlHandle;thePt: Point): INTEGER;
INLINE $A966;
FUNCTION TrackControl(theControl: ControlHandle;thePoint: Point;actionProc: ProcPtr): INTEGER;
INLINE $A968;
FUNCTION FindControl(thePoint: Point;theWindow: WindowPtr;VAR theControl: ControlHandle): INTEGER;
INLINE $A96C;
PROCEDURE SetCtlColor(theControl: ControlHandle;newColorTable: CCTabHandle);
INLINE $AA43;
FUNCTION GetAuxCtl(theControl: ControlHandle;VAR acHndl: AuxCtlHandle): BOOLEAN;
INLINE $AA44;
FUNCTION GetCVariant(theControl: ControlHandle): INTEGER;
INLINE $A809;

{$ENDC}

{ UsingControls }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Controls.p}



{#####################################################################}
{### FILE: CRMSerialDevices.p}
{#####################################################################}

{
Created: Wednesday, September 11, 1991 at 2:05 PM
CRMSerialDevices.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT CRMSerialDevices;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingCRMSerialDevices}
{$SETC UsingCRMSerialDevices := 1}
{$I+}
{$SETC CRMSerialDevicesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := CRMSerialDevicesIncludes}
CONST
{
for the crmDeviceType field of the CRMRec data structure }
crmSerialDevice = 1;

{ version of the CRMSerialRecord below }
curCRMSerRecVers = 1;
TYPE
	{ Maintains compatibility w/ apps & tools that expect an old style icon }
	CRMIconPtr = ^CRMIconRecord;
	CRMIconHandle = ^CRMIconPtr;
	CRMIconRecord = RECORD
	oldIcon: ARRAY [0..31] OF LONGINT;
	{ ICN#
	}
	oldMask: ARRAY [0..31] OF LONGINT;
	theSuite: Handle;
	{ Handle to an IconSuite
	}
	reserved: LONGINT;
END;




CRMSerialPtr = ^CRMSerialRecord;
CRMSerialRecord = RECORD
	version: INTEGER;
	inputDriverName: StringHandle;
	outputDriverName: StringHandle;
	name: StringHandle;
	deviceIcon: CRMIconHandle;
	ratedSpeed: LONGINT;
	maxSpeed: LONGINT;
	reserved: LONGINT;
END;

{$ENDC} { UsingCRMSerialDevices }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE CRMSerialDevices.p}





{#####################################################################}
{### FILE: CTBUtilities.p}
{#####################################################################}

{
Created: Thursday, September 12, 1991 at 2:53 PM
CTBUtilities.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT CTBUtilities;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingCTBUtilities}
{$SETC UsingCTBUtilities := 1}
{$I+}
{$SETC CTBUtilitiesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingMemory}
{$I $$Shell(PInterfaces)Memory.p}
{$ENDC}
{$IFC UNDEFINED UsingPackages}
{$I $$Shell(PInterfaces)Packages.p}
{$ENDC}
{$IFC UNDEFINED UsingAppleTalk}
{$I $$Shell(PInterfaces)AppleTalk.p}
{$ENDC}
{$SETC UsingIncludes := CTBUtilitiesIncludes}
CONST
{ version of Comm Toolbox Utilities }
curCTBUVersion = 2;
{
Error codes/types
ctbuGenericError = -1;
ctbuNoErr = 0;
TYPE
CTBUErr = OSErr;
CONST

}



{ Choose return codes}
chooseDisaster = -2;
chooseFailed = -1;
chooseAborted = 0;
chooseOKMinor = 1;
chooseOKMajor = 2;
chooseCancel = 3;
{ NuLookup return codes }
nlOk = 0;
nlCancel = 1;
nlEject = 2;
{ Name filter proc return codes }
nameInclude = 1;
nameDisable = 2;
nameReject = 3;
{ Zone filter proc return codes }
zoneInclude = 1;
zoneDisable = 2;
zoneReject = 3;
{
Values for hookProc items
hookOK = 1;
hookCancel = 2;
hookOutline = 3;
hookTitle = 4;
hookItemList = 5;
hookZoneTitle = 6;
hookZoneList = 7;
hookLine = 8;
hookVersion = 9;
hookReserved1 = 10;
hookReserved2 = 11;
hookReserved3 = 12;
hookReserved4 = 13;
{
"virtual" hookProc items
hookNull = 100;
hookItemRefresh = 101;
hookZoneRefresh = 102;
hookEject = 103;
hookPreflight = 104;
hookPostflight = 105;
hookKeyBase = 1000;

}

}

TYPE
{
NuLookup structures/constants
NLTypeEntry = RECORD
hIcon: Handle;
typeStr: Str32;
END;

NLType = ARRAY [0..3] OF NLTypeEntry;

}


NBPReply = RECORD
theEntity: EntityName;
theAddr: AddrBlock;
END;

NameFilterProcPtr = ProcPtr;
ZoneFilterProcPtr = ProcPtr;

FUNCTION InitCTBUtilities: CTBUErr;
FUNCTION CTBGetCTBVersion: INTEGER;

FUNCTION StandardNBP(where: Point;prompt: Str255;numTypes: INTEGER;typeList: NLType;
nameFilter: NameFilterProcPtr;zoneFilter: ZoneFilterProcPtr;hookProc: DlgHookProcPtr;
VAR theReply: NBPReply): INTEGER;
FUNCTION CustomNBP(where: Point;prompt: Str255;numTypes: INTEGER;typeList: NLType;
nameFilter: NameFilterProcPtr;zoneFilter: ZoneFilterProcPtr;hookProc: DlgHookProcPtr;
userData: LONGINT;dialogID: INTEGER;filterProc: ModalFilterProcPtr;VAR theReply: NBPReply): INTEGER;

{ Obsolete synonyms for above routines }
FUNCTION NuLookup(where: Point;prompt: Str255;numTypes: INTEGER;typeList: NLType;
nameFilter: NameFilterProcPtr;zoneFilter: ZoneFilterProcPtr;hookProc: DlgHookProcPtr;
VAR theReply: NBPReply): INTEGER;
FUNCTION NuPLookup(where: Point;prompt: Str255;numTypes: INTEGER;typeList: NLType;
nameFilter: NameFilterProcPtr;zoneFilter: ZoneFilterProcPtr;hookProc: DlgHookProcPtr;
userData: LONGINT;dialogID: INTEGER;filterProc: ModalFilterProcPtr;VAR theReply: NBPReply): INTEGER;

{$ENDC} { UsingCTBUtilities }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE CTBUtilities.p}



{#####################################################################}
{### FILE: CursorCtl.p}
{#####################################################################}
{
Created: Tuesday, August 2, 1988 at 12:24 PM
CursorCtl.p
Pascal Interface to the Macintosh Libraries

<<< CursorCtl - Cursor Control Interface File >>>

Copyright Apple Computer, Inc. 1984-1988
All rights reserved
This file contains:
InitCursorCtl(newCursors) - Init CursorCtl to load the 'acur' resource
RotateCursor(counter) - Sequence through cursor frames for counter mod 32
SpinCursor(increment) - Sequence mod 32 incrementing internal counter
Hide_Cursor() - Hide the current cursor
Show_Cursor(cursorKind) - Show the cursor
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT CursorCtl;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingCursorCtl}
{$SETC UsingCursorCtl := 1}

TYPE
{ Kinds of cursor supported by CursorCtl }
Cursors = (HIDDEN_CURSOR,I_BEAM_CURSOR,CROSS_CURSOR,PLUS_CURSOR,WATCH_CURSOR,
ARROW_CURSOR);
acurPtr = ^Acur;
acurHandle = ^acurPtr;
Acur = RECORD
n: INTEGER;
index: INTEGER;
frame1: INTEGER;
fill1: INTEGER;
frame2: INTEGER;
fill2: INTEGER;
frameN: INTEGER;

{Number of cursors ("frames of film")}
{ Next frame to show <for internal use>}
{'CURS' resource id for frame #1}
{<for internal use>}
{'CURS' resource id for frame #2}
{<for internal use>}
{'CURS' resource id for frame #N}

fillN: INTEGER;
END;


{<for internal use>}

PROCEDURE InitCursorCtl(newCursors: UNIV acurHandle);
{ Initialize the CursorCtl unit. This should be called once prior to calling
RotateCursor or SpinCursor. It need not be called if only Hide_Cursor or
Show_Cursor are used. If NewCursors is NULL, InitCursorCtl loads in the
'acur' resource and the 'CURS' resources specified by the 'acur' resource
ids. If any of the resources cannot be loaded, the cursor will not be
changed.
The 'acur' resource is assumed to either be in the currently running tool or
application, or the MPW Shell for a tool, or in the System file. The 'acur'
resource id must be 0 for a tool or application, 1 for the Shell, and 2 for
the System file.
If NewCursors is not NULL, it is ASSUMED to be a handle to an 'acur' formatted
resource designated by the caller and it will be used instead of doing a
GetResource on 'acur'. Note, if RotateCursor or SpinCursor are called without
calling InitCursorCtl, then RotateCursor and SpinCursor will do the call for
the user the first time it is called. However, the possible disadvantage of
using this technique is that the resource memory allocated may have
undesirable affect (fragmentation?) on the application. Using InitCursorCtl
has the advantage of causing the allocation at a specific time determined by
the user.
Caution: InitCursorCtl MODIFIES the 'acur' resource in memory. Specifically,
it changes each FrameN/fillN integer pair to a handle to the corresponding
'CURS' resource also in memory. Thus if NewCursors is not NULL when
InitCursorCtl is called, the caller must guarantee NewCursors always points to
a "fresh" copy of an 'acur' resource. This need only be of concern to a
caller who wants to repeatly use multiple 'acur' resources during execution of
their programs.
}
PROCEDURE RotateCursor(counter: LONGINT);
{ RotateCursor is called to rotate the "I am active" "beach ball" cursor, or to
animate whatever sequence of cursors set up by InitCursorCtl. The next cursor
("frame") is used when Counter % 32 = 0 (Counter is some kind of incrementing
or decrementing index maintained by the caller). A positive counter sequences
forward through the cursors (e.g., it rotates the "beach ball" cursor
clockwise), and a negative cursor sequences through the cursors backwards
(e.g., it rotates the "beach ball" cursor counterclockwise). Note,
RotateCursor just does a Mac SetCursor call for the proper cursor picture.
It is assumed the cursor is visible from a prior Show_Cursor call.
}
PROCEDURE SpinCursor(increment: INTEGER);
{ SpinCursor is similar in function to RotateCursor, except that instead of
passing a counter, an Increment is passed an added to a counter maintained
here. SpinCursor is provided for those users who do not happen to have a
convenient counter handy but still want to use the spinning "beach ball"
cursor, or any sequence of cursors set up by InitCursorCtl. A positive
increment sequences forward through the curos (rotating the "beach ball"


cursor clockwise), and a negative increment sequences backward through the
cursors (rotating the "beach ball" cursor counter-clockwise). A zero value
for the increment resets the counter to zero. Note, it is the increment, and
not the value of the counter that determines the sequencing direction of the
cursor (and hence the spin direction of the "beach ball" cursor).
}
PROCEDURE Hide_Cursor;
{ Hide the cursor if it is showing.This is this unit's call to the Mac
HideCursor routine.Thus the Mac cursor level is decremented by one when this
routine is called.
}
PROCEDURE Show_Cursor(cursorKind: Cursors);
{ Increment the cursor level, which may have been decremented by Hide_Cursor,
and display the specified cursor if the level becomes 0 (it is never
incremented beyond 0).The CursorKind is the kind of cursor to show. It is
one of the values HIDDEN_CURSOR, I_BEAM_CURSOR, CROSS_CURSOR, PLUS_CURSOR,
WATCH_CURSOR, and ARROW_CURSOR. Except for HIDDEN_CURSOR, a Mac SetCursor is
done for the specified cursor prior to doing a ShowCursor. HIDDEN_CURSOR just
causes a ShowCursor call. Note, ARROW_CURSOR will only work correctly if
there is already a grafPort set up pointed to by 0(A5).
}

{$ENDC}

{ UsingCursorCtl }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE CursorCtl.p}



{#####################################################################}
{### FILE: DatabaseAccess.p}
{#####################################################################}

{
Created: Tuesday, September 10, 1991 at 1:01 PM
DatabaseAccess.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1989-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT DatabaseAccess;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingDatabaseAccess}
{$SETC UsingDatabaseAccess := 1}
{$I+}
{$SETC DatabaseAccessIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingResources}
{$I $$Shell(PInterfaces)Resources.p}
{$ENDC}
{$SETC UsingIncludes := DatabaseAccessIncludes}
CONST
{ error and status codes }
rcDBNull = -800;
rcDBValue = -801;
rcDBError = -802;
rcDBBadType = -803;
rcDBBreak = -804;
rcDBExec = -805;
rcDBBadSessID = -806;
rcDBBadSessNum = -807;
rcDBBadDDEV = -808;
rcDBAsyncNotSupp = -809;
rcDBBadAsyncPB = -810;
rcDBNoHandler = -811;
rcDBWrongVersion = -812;
rcDBPackNotInited = -813;

{
{
{
{
{
{
{

bad session number for DBGetConnInfo }
bad ddev specified on DBInit }
ddev does not support async calls }
tried to kill a bad pb }
no app handler for specified data type }
incompatible versions }
attempt to call other routine before InitDBPack }

{ messages for status functions for DBStartQuery }
kDBUpdateWind = 0;



kDBAboutToInit = 1;
kDBInitComplete = 2;
kDBSendComplete = 3;
kDBExecComplete = 4;
kDBStartQueryComplete = 5;
{ messages for status functions for DBGetQueryResults }
kDBGetItemComplete = 6;
kDBGetQueryResultsComplete = 7;
{ data type codes }
typeNone = 'none';
typeDate = 'date';
typeTime = 'time';
typeTimeStamp = 'tims';
typeDecimal = 'deci';
typeMoney = 'mone';
typeVChar = 'vcha';
typeVBin = 'vbin';
typeLChar = 'lcha';
typeLBin = 'lbin';
typeDiscard = 'disc';
{ "dummy" types for DBResultsToText }
typeUnknown = 'unkn';
typeColBreak = 'colb';
typeRowBreak = 'rowb';
{ pass this in to DBGetItem for any data type }
typeAnyType = 0;
{ infinite timeout value for DBGetItem }
kDBWaitForever = -1;
{ flags for DBGetItem }
kDBLastColFlag = $0001;
kDBNullFlag = $0004;
TYPE
DBType = OSType;
{ structure for asynchronous parameter block }
DBAsyncParmBlkPtr = ^DBAsyncParamBlockRec;
DBAsyncParamBlockRec = RECORD
completionProc: ProcPtr;
{ pointer to completion routine }
result: OSErr;
{ result of call }
userRef: LONGINT;
{ for application's use }
ddevRef: LONGINT;
{ for ddev's use }
reserved: LONGINT;
{ for internal use }
END;
{ structure for resource list in QueryRecord }
ResListElem = RECORD
theType: ResType;
{ resource type }
id: INTEGER;
{ resource id }
END;



ResListPtr = ^ResListArray;
ResListHandle = ^ResListPtr;
ResListArray = ARRAY [0..255] OF ResListElem;
{ structure for query list in QueryRecord }
QueryListPtr = ^QueryArray;
QueryListHandle = ^QueryListPtr;
QueryArray = ARRAY [0..255] OF Handle;
QueryPtr = ^QueryRecord;
QueryHandle = ^QueryPtr;
QueryRecord = RECORD
version: INTEGER;
id: INTEGER;
queryProc: Handle;
ddevName: Str63;
host: Str255;
user: Str255;
password: Str255;
connStr: Str255;
currQuery: INTEGER;
numQueries: INTEGER;
queryList: QueryListHandle;
numRes: INTEGER;
resList: ResListHandle;
dataHandle: Handle;
refCon: LONGINT;
END;

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

version }
id of 'qrsc' this came from }
handle to query def proc }
ddev name }
host name }
user name }
password }
connection string }
index of current query }
number of queries in list }
handle to array of handles to text }
number of resources in list }
handle to array of resource list elements }
for use by query def proc }
for use by application }

{ structure of column types array in ResultsRecord }
ColTypesPtr = ^ColTypesArray;
ColTypesHandle = ^ColTypesPtr;
ColTypesArray = ARRAY [0..255] OF DBType;
{ structure for column info in ResultsRecord }
DBColInfoRecord = RECORD
len: INTEGER;
places: INTEGER;
flags: INTEGER;
END;
ColInfoPtr = ^ColInfoArray;
ColInfoHandle = ^ColInfoPtr;
ColInfoArray = ARRAY [0..255] OF DBColInfoRecord;
{ structure of results returned by DBGetResults }
ResultsRecord = RECORD
numRows: INTEGER;
numCols: INTEGER;
colTypes: ColTypesHandle;
colData: Handle;

{number of rows in result }
{number of columns per row }
{data type array }
{actual results }

colInfo: ColInfoHandle;
END;


{ DBColInfoRecord array }

FUNCTION InitDBPack: OSErr;
INLINE $3F3C,$0004,$303C,$0100,$A82F;
FUNCTION DBInit(VAR sessID: LONGINT;ddevName: Str63;host: Str255;user: Str255;
passwd: Str255;connStr: Str255;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0E02,$A82F;
FUNCTION DBEnd(sessID: LONGINT;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0403,$A82F;
FUNCTION DBGetConnInfo(sessID: LONGINT;sessNum: INTEGER;VAR returnedID: LONGINT;
VAR version: LONGINT;VAR ddevName: Str63;VAR host: Str255;VAR user: Str255;
VAR network: Str255;VAR connStr: Str255;VAR start: LONGINT;VAR state: OSErr;
asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$1704,$A82F;
FUNCTION DBGetSessionNum(sessID: LONGINT;VAR sessNum: INTEGER;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0605,$A82F;
FUNCTION DBSend(sessID: LONGINT;text: Ptr;len: INTEGER;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0706,$A82F;
FUNCTION DBSendItem(sessID: LONGINT;dataType: DBType;len: INTEGER;places: INTEGER;
flags: INTEGER;buffer: Ptr;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0B07,$A82F;
FUNCTION DBExec(sessID: LONGINT;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0408,$A82F;
FUNCTION DBState(sessID: LONGINT;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0409,$A82F;
FUNCTION DBGetErr(sessID: LONGINT;VAR err1: LONGINT;VAR err2: LONGINT;VAR item1: Str255;
VAR item2: Str255;VAR errorMsg: Str255;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0E0A,$A82F;
FUNCTION DBBreak(sessID: LONGINT;abort: BOOLEAN;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$050B,$A82F;
FUNCTION DBGetItem(sessID: LONGINT;timeout: LONGINT;VAR dataType: DBType;
VAR len: INTEGER;VAR places: INTEGER;VAR flags: INTEGER;buffer: Ptr;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$100C,$A82F;
FUNCTION DBUnGetItem(sessID: LONGINT;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$040D,$A82F;
FUNCTION DBKill(asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$020E,$A82F;
FUNCTION DBGetNewQuery(queryID: INTEGER;VAR query: QueryHandle): OSErr;
INLINE $303C,$030F,$A82F;
FUNCTION DBDisposeQuery(query: QueryHandle): OSErr;
INLINE $303C,$0210,$A82F;
FUNCTION DBStartQuery(VAR sessID: LONGINT;query: QueryHandle;statusProc: ProcPtr;
asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0811,$A82F;
FUNCTION DBGetQueryResults(sessID: LONGINT;VAR results: ResultsRecord;timeout: LONGINT;
statusProc: ProcPtr;asyncPB: DBAsyncParmBlkPtr): OSErr;
INLINE $303C,$0A12,$A82F;
FUNCTION DBResultsToText(results: ResultsRecord;VAR theText: Handle): OSErr;
INLINE $303C,$0413,$A82F;
FUNCTION DBInstallResultHandler(dataType: DBType;theHandler: ProcPtr;isSysHandler: BOOLEAN): OSErr;
INLINE $303C,$0514,$A82F;
FUNCTION DBRemoveResultHandler(dataType: DBType): OSErr;
INLINE $303C,$0215,$A82F;
FUNCTION DBGetResultHandler(dataType: DBType;VAR theHandler: ProcPtr;getSysHandler: BOOLEAN): OSErr;


INLINE $303C,$0516,$A82F;

{$ENDC} { UsingDatabaseAccess }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE DatabaseAccess.p}

{#####################################################################}
{### FILE: Desk.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:27 PM
Desk.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1989

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Desk;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingDesk}
{$SETC UsingDesk := 1}
{$I+}
{$SETC DeskIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$IFC UNDEFINED UsingEvents}
{$I $$Shell(PInterfaces)Events.p}
{$ENDC}
{$SETC UsingIncludes := DeskIncludes}
CONST
accEvent = 64;
accRun = 65;
accCursor = 66;
accMenu = 67;
accUndo = 68;
accCut = 70;
accCopy = 71;
accPaste = 72;
accClear = 73;
goodbye = -1;
{goodbye message}
FUNCTION OpenDeskAcc(deskAccName: Str255): INTEGER;
INLINE $A9B6;
PROCEDURE CloseDeskAcc(refNum: INTEGER);
INLINE $A9B7;
PROCEDURE SystemClick(theEvent: EventRecord;theWindow: WindowPtr);
INLINE $A9B3;
FUNCTION SystemEdit(editCmd: INTEGER): BOOLEAN;
INLINE $A9C2;
PROCEDURE SystemTask;
INLINE $A9B4;
FUNCTION SystemEvent(theEvent: EventRecord): BOOLEAN;
INLINE $A9B2;
PROCEDURE SystemMenu(menuResult: LONGINT);
INLINE $A9B5;

{$ENDC}

{ UsingDesk }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Desk.p}

{#####################################################################}
{### FILE: DeskBus.p}
{#####################################################################}

{
Created: Sunday, September 15, 1991 at 10:14 PM
DeskBus.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1987-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT DeskBus;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingDeskBus}
{$SETC UsingDeskBus := 1}
{$I+}
{$SETC DeskBusIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := DeskBusIncludes}
TYPE
ADBAddress = SignedByte;
ADBOpBPtr = ^ADBOpBlock;
ADBOpBlock = RECORD
dataBuffPtr: Ptr;
opServiceRtPtr: Ptr;
opDataAreaPtr: Ptr;
END;

{address of data buffer}
{service routine pointer}
{optional data area address}

ADBDBlkPtr = ^ADBDataBlock;
ADBDataBlock = PACKED RECORD
devType: SignedByte;
{device type}
origADBAddr: SignedByte;
{original ADB Address}
dbServiceRtPtr: Ptr;
{service routine pointer}
dbDataAreaAddr: Ptr;
{data area address}
END;
ADBSInfoPtr = ^ADBSetInfoBlock;
ADBSetInfoBlock = RECORD
siServiceRtPtr: Ptr;
siDataAreaAddr: Ptr;
END;

{service routine pointer}
{data area address}

PROCEDURE ADBReInit;
INLINE $A07B;
FUNCTION ADBOp(data: Ptr;compRout: ProcPtr;buffer: Ptr;commandNum: INTEGER): OSErr;
FUNCTION CountADBs: INTEGER;
INLINE $A077,$3E80;
FUNCTION GetIndADB(VAR info: ADBDataBlock;devTableIndex: INTEGER): ADBAddress;
FUNCTION GetADBInfo(VAR info: ADBDataBlock;adbAddr: ADBAddress): OSErr;
FUNCTION SetADBInfo(VAR info: ADBSetInfoBlock;adbAddr: ADBAddress): OSErr;

{$ENDC} { UsingDeskBus }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE DeskBus.p}


{#####################################################################}
{### FILE: Devices.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:28 PM
Devices.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Devices;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingDevices}
{$SETC UsingDevices := 1}
{$I+}
{$SETC DevicesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := DevicesIncludes}
CONST
newSelMsg = 12;
fillListMsg = 13;
getSelMsg = 14;
selectMsg = 15;
deselectMsg = 16;
terminateMsg = 17;
buttonMsg = 19;
chooserID = 1;
initDev = 0;
hitDev = 1;
closeDev = 2;
nulDev = 3;
updateDev = 4;
activDev = 5;

{Time for cdev to initialize itself}
{Hit on one of my items}
{Close yourself}
{Null event}
{Update event}
{Activate event}

deactivDev = 6;
keyEvtDev = 7;
macDev = 8;
undoDev = 9;
cutDev = 10;
copyDev = 11;
pasteDev = 12;
clearDev = 13;
cursorDev = 14;
cdevGenErr = -1;
cdevMemErr = 0;
cdevResErr = 1;
cdevUnset = 3;

{Deactivate event}
{Key down/auto key}
{Decide whether or not to show up}

{General error; gray cdev w/o alert}
{Memory shortfall; alert user please}
{Couldn't get a needed resource; alert}
{ cdevValue is initialized to this}

{ Monitors control panel messages }
initMsg = 1;
{initialization}
okMsg = 2;
{user clicked OK button}
cancelMsg = 3;
{user clicked Cancel button}
hitMsg = 4;
{user clicked control in Options dialog}
nulMsg = 5;
{periodic event}
updateMsg = 6;
{update event}
activateMsg = 7;
{not used}
deactivateMsg = 8; {not used}
keyEvtMsg = 9;
{keyboard event}
superMsg = 10;
{show superuser controls}
normalMsg = 11;
{show only normal controls}
startupMsg = 12;
{code has been loaded}
TYPE
DCtlPtr = ^DCtlEntry;
DCtlHandle = ^DCtlPtr;
DCtlEntry = RECORD
dCtlDriver: Ptr;
dCtlFlags: INTEGER;
dCtlQHdr: QHdr;
dCtlPosition: LONGINT;
dCtlStorage: Handle;
dCtlRefNum: INTEGER;
dCtlCurTicks: LONGINT;
dCtlWindow: WindowPtr;
dCtlDelay: INTEGER;
dCtlEMask: INTEGER;
dCtlMenu: INTEGER;
END;
AuxDCEPtr = ^AuxDCE;
AuxDCEHandle = ^AuxDCEPtr;
AuxDCE = PACKED RECORD
dCtlDriver: Ptr;
dCtlFlags: INTEGER;
dCtlQHdr: QHdr;
dCtlPosition: LONGINT;
dCtlStorage: Handle;
dCtlRefNum: INTEGER;
dCtlCurTicks: LONGINT;
dCtlWindow: GrafPtr;
dCtlDelay: INTEGER;
dCtlEMask: INTEGER;
dCtlMenu: INTEGER;
dCtlSlot: Byte;
dCtlSlotId: Byte;
dCtlDevBase: LONGINT;
dCtlOwner: Ptr;
dCtlExtDev: Byte;
fillByte: Byte;
END;

FUNCTION GetDCtlEntry(refNum: INTEGER): DCtlHandle;
FUNCTION SetChooserAlert(f: BOOLEAN): BOOLEAN;
FUNCTION OpenDriver(name: Str255;VAR drvrRefNum: INTEGER): OSErr;
FUNCTION CloseDriver(refNum: INTEGER): OSErr;
FUNCTION Control(refNum: INTEGER;csCode: INTEGER;csParamPtr: Ptr): OSErr;
FUNCTION Status(refNum: INTEGER;csCode: INTEGER;csParamPtr: Ptr): OSErr;
FUNCTION KillIO(refNum: INTEGER): OSErr;
FUNCTION PBControl(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBControlSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A004,$3E80;
FUNCTION PBControlAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A404,$3E80;
FUNCTION PBStatus(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBStatusSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A005,$3E80;
FUNCTION PBStatusAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A405,$3E80;
FUNCTION PBKillIO(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBKillIOSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A006,$3E80;
FUNCTION PBKillIOAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A406,$3E80;

{$ENDC}

{ UsingDevices }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Devices.p}

{#####################################################################}
{### FILE: Dialogs.p}
{#####################################################################}

{
Created: Thursday, September 12, 1991 at 2:47 PM
Dialogs.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1991

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Dialogs;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingDialogs}
{$SETC UsingDialogs := 1}
{$I+}
{$SETC DialogsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingWindows}
{$I $$Shell(PInterfaces)Windows.p}
{$ENDC}
{$IFC UNDEFINED UsingTextEdit}
{$I $$Shell(PInterfaces)TextEdit.p}
{$ENDC}
{$SETC UsingIncludes := DialogsIncludes}
CONST
ctrlItem = 4;
btnCtrl = 0;
chkCtrl = 1;
radCtrl = 2;
resCtrl = 3;
statText = 8;
editText = 16;
iconItem = 32;
picItem = 64;
userItem = 0;
itemDisable = 128;
ok = 1;
cancel = 2;
stopIcon = 0;
noteIcon = 1;
cautionIcon = 2;
TYPE
{ Dialog Item List Manipulation Constants }
DITLMethod = INTEGER;
CONST
overlayDITL = 0;
appendDITLRight = 1;
appendDITLBottom = 2;
TYPE
StageList = PACKED RECORD
boldItm4: 0..1;
boxDrwn4: BOOLEAN;
sound4: 0..3;
boldItm3: 0..1;
boxDrwn3: BOOLEAN;
sound3: 0..3;
boldItm2: 0..1;
boxDrwn2: BOOLEAN;
sound2: 0..3;
boldItm1: 0..1;
boxDrwn1: BOOLEAN;
sound1: 0..3;
END;

DialogPtr = WindowPtr;
ResumeProcPtr = ProcPtr;
SoundProcPtr = ProcPtr;
ModalFilterProcPtr = ProcPtr;
DialogPeek = ^DialogRecord;
DialogRecord = RECORD
window: WindowRecord;
items: Handle;
textH: TEHandle;
editField: INTEGER;
editOpen: INTEGER;
aDefItem: INTEGER;
END;
DialogTPtr = ^DialogTemplate;
DialogTHndl = ^DialogTPtr;
DialogTemplate = RECORD
boundsRect: Rect;
procID: INTEGER;
visible: BOOLEAN;
filler1: BOOLEAN;
goAwayFlag: BOOLEAN;
filler2: BOOLEAN;
refCon: LONGINT;
itemsID: INTEGER;
title: Str255;
END;

{default button item number - 1}
{true if alert box to be drawn}
{sound number}

{ PROCEDURE Resume; }
{ PROCEDURE DoSound; }
{ FUNCTION Filter(theDialog: DialogPtr; VAR theEvent: EventRecord; VAR itemHit: INTEGER): BOOLEAN; }


AlertTPtr = ^AlertTemplate;
AlertTHndl = ^AlertTPtr;
AlertTemplate = RECORD
boundsRect: Rect;
itemsID: INTEGER;
stages: StageList;
END;


PROCEDURE InitDialogs(resumeProc: ResumeProcPtr);
INLINE $A97B;
PROCEDURE ErrorSound(soundProc: SoundProcPtr);
INLINE $A98C;
FUNCTION NewDialog(wStorage: Ptr;boundsRect: Rect;title: Str255;visible: BOOLEAN;
procID: INTEGER;behind: WindowPtr;goAwayFlag: BOOLEAN;refCon: LONGINT;
itmLstHndl: Handle): DialogPtr;
INLINE $A97D;
FUNCTION GetNewDialog(dialogID: INTEGER;dStorage: Ptr;behind: WindowPtr): DialogPtr;
INLINE $A97C;
PROCEDURE CloseDialog(theDialog: DialogPtr);
INLINE $A982;
PROCEDURE DisposDialog(theDialog: DialogPtr);
INLINE $A983;
PROCEDURE DisposeDialog(theDialog: DialogPtr);
INLINE $A983;
PROCEDURE CouldDialog(dialogID: INTEGER);
INLINE $A979;
PROCEDURE FreeDialog(dialogID: INTEGER);
INLINE $A97A;
PROCEDURE ParamText(param0: Str255;param1: Str255;param2: Str255;param3: Str255);
INLINE $A98B;
PROCEDURE ModalDialog(filterProc: ModalFilterProcPtr;VAR itemHit: INTEGER);
INLINE $A991;
FUNCTION IsDialogEvent(theEvent: EventRecord): BOOLEAN;
INLINE $A97F;
FUNCTION DialogSelect(theEvent: EventRecord;VAR theDialog: DialogPtr;VAR itemHit: INTEGER): BOOLEAN;
INLINE $A980;
PROCEDURE DrawDialog(theDialog: DialogPtr);
INLINE $A981;
PROCEDURE UpdtDialog(theDialog: DialogPtr;updateRgn: RgnHandle);
INLINE $A978;
PROCEDURE UpdateDialog(theDialog: DialogPtr;updateRgn: RgnHandle);
INLINE $A978;
FUNCTION Alert(alertID: INTEGER;filterProc: ModalFilterProcPtr): INTEGER;
INLINE $A985;
FUNCTION StopAlert(alertID: INTEGER;filterProc: ModalFilterProcPtr): INTEGER;
INLINE $A986;
FUNCTION NoteAlert(alertID: INTEGER;filterProc: ModalFilterProcPtr): INTEGER;
INLINE $A987;
FUNCTION CautionAlert(alertID: INTEGER;filterProc: ModalFilterProcPtr): INTEGER;
INLINE $A988;
PROCEDURE CouldAlert(alertID: INTEGER);
INLINE $A989;
PROCEDURE FreeAlert(alertID: INTEGER);
INLINE $A98A;
PROCEDURE GetDItem(theDialog: DialogPtr;itemNo: INTEGER;VAR itemType: INTEGER;

VAR item: Handle;VAR box: Rect);
INLINE $A98D;
PROCEDURE SetDItem(theDialog: DialogPtr;itemNo: INTEGER;itemType: INTEGER;
item: Handle;box: Rect);
INLINE $A98E;
PROCEDURE HideDItem(theDialog: DialogPtr;itemNo: INTEGER);
INLINE $A827;
PROCEDURE ShowDItem(theDialog: DialogPtr;itemNo: INTEGER);
INLINE $A828;
PROCEDURE SelIText(theDialog: DialogPtr;itemNo: INTEGER;strtSel: INTEGER;
endSel: INTEGER);
INLINE $A97E;
PROCEDURE GetIText(item: Handle;VAR text: Str255);
INLINE $A990;
PROCEDURE SetIText(item: Handle;text: Str255);
INLINE $A98F;
FUNCTION FindDItem(theDialog: DialogPtr;thePt: Point): INTEGER;
INLINE $A984;
FUNCTION NewCDialog(dStorage: Ptr;boundsRect: Rect;title: Str255;visible: BOOLEAN;
procID: INTEGER;behind: WindowPtr;goAwayFlag: BOOLEAN;refCon: LONGINT;
items: Handle): DialogPtr;
INLINE $AA4B;
FUNCTION GetAlrtStage: INTEGER;
INLINE $3EB8,$0A9A;
PROCEDURE ResetAlrtStage;
INLINE $4278,$0A9A;
PROCEDURE DlgCut(theDialog: DialogPtr);
PROCEDURE DlgPaste(theDialog: DialogPtr);
PROCEDURE DlgCopy(theDialog: DialogPtr);
PROCEDURE DlgDelete(theDialog: DialogPtr);
PROCEDURE SetDAFont(fontNum: INTEGER);
INLINE $31DF,$0AFA;
PROCEDURE AppendDITL(theDialog: DialogPtr;theHandle: Handle;method: DITLMethod);
FUNCTION CountDITL(theDialog: DialogPtr): INTEGER;
PROCEDURE ShortenDITL(theDialog: DialogPtr;numberItems: INTEGER);

{$ENDC} { UsingDialogs }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Dialogs.p}

{#####################################################################}
{### FILE: DisAsmLookup.p}
{#####################################################################}
{
Created: Wednesday, November 1, 1989
DisAsmLookup.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1987-1990

}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT DisAsmLookup;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingDisAsmLookup}
{$SETC UsingDisAsmLookup := 1}
{$I+}
{$SETC DisAsmLookupIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := DisAsmLookupIncludes}

TYPE
LookupRegs

= (_A0_, _A1_, _A2_, _A3_, _A4_, _A5_, _A6_, _A7_,
_PC_, _ABS_, _TRAP_, _IMM_);

(*----------------------------------------------------------------------*)
PROCEDURE Disassembler(

DstAdjust:

LongInt;
{addr correction}
VAR BytesUsed: Integer;
{bytes used up }
FirstByte: UNIV Ptr;
{starting byte }
VAR Opcode:
UNIV Str255;
{mnemonic }
VAR Operand:
UNIV Str255;
{operand}
VAR Comment:
UNIV Str255;
{comment }
LookupProc: UNIV Ptr);
{search proc}


{
Disassembler is a Pascal routine to be called to disassemble a sequence
of bytes. All MC68xxx, MC68881, and MC68851 instructions are supported.
The sequence of bytes to be disassembled are pointed to by FirstByte.
BytesUsed bytes starting at FirstByte are consumed by the disassembly,
and the Opcode, Operand, and Comment strings returned as NULL TERMINATED
Pascal strings (for easier manipulation with C). The caller is then free
to format or use the output strings any way appropriate to the
application.
Depending on the opcode and effective address(s) (EA's) to be
disassembled, the Opcode, Operand, and Comment strings contain the
following information:
Case
Opcode
Operand
Comment
=======================================================================
Non PC-relative EA's
op.sz
EA's
; 'c...' (for immediates)
PC-relative EA's
op.sz
EA's
; address
Toolbox traps
DC.W
$AXXX
; TB XXXX
OS traps
DC.W
$AXXX
; OS XXXX
Invalid bytes
DC.W
$XXXX
; ????
=======================================================================
For valid disassembly of processor instructions the appropriate MC68XXX
opcode mnemonic is generated for the Opcode string along with a size
attribute when required. The source and destination EA's are generated
as the Operand along with a possible comment. Comments start with a ';'.
Traps use a DC.W assembler directive as the Opcode with the trap word
as the Operand and a comment indicating whether the trap is a toolbox or
OS trap and what the trap number is. As described later the caller can
generate symbolic substitutions into EA's and provide names for traps.
Invalid instructions cause the string 'DC.W' to be returned in the
Opcode string. Operand is '$XXXX' (the invalid word) with a comment of
'; ????'. BytesUsed is 2. This is similar to the trap call case except
for the comment.
Note, the Operand EA's is syntatically similar to but NOT COMPATIBLE
with the MPW assembler!
This is because the Disassembler generates
byte hex constants as "$XX" and word hex constants as "$XXXX". Negative
values (e.g., $FF or $FFFF) produced by the Disassembler are treated as
long word values by the MPW assembler. Thus it is assumed that
Disassembler output will NOT be used as MPW assembler input. If that is
the goal, then the caller must convert strings of the form $XX or $XXXX
in the Operand string to their decimal equivalent. The routine
ModifyOperand is provided in this unit to aid with the conversion
process.
Since a PC-relative comment is an address, the only address that the
Disassembler knows about is the address of the code pointed to by
FirstByte. Generally, that may be a buffer that has no relation to
"reality", i.e., the actual code loaded into the buffer. Therefore,
to allow the address comment to be mapped back to some actual address
the caller may specify an adjustment factor, specified by DstAdjust,
that is ADDED to the value that normally would be placed in the
comment.
Operand effective address strings are generated as a function of the
effective address mode and a special case is made for A-trap opcode
strings. In places where a possible symbolic reference could be
substituted for an address (or a portion of an address), the Disassembler
can call a user specified routine to do the substitution (using the
LookupProc parameter described later). The following table summarizes
the generated effective addresses and where symbolic substitutions (S)
can be made:
Mode
Generated Effective Address Effective Address with Substitution
========================================================================
0
Dn
Dn
1
An
An
2
(An)
(An)
3
(An)+
(An)+
4
-(An)
-(An)
5
∂(An)
S(An) or just S (if An=A5, ∂≥0)
6n
∂(An,Xn.Size*Scale)
S(An,Xn.Size*Scale)
6n
(BD,An,Xn.Size*Scale)
(S,An,Xn.Size*Scale)
6n
([BD,An],Xm.Size*Scale,OD)
([S,An],Xm.Size*Scale,OD)
6n
([BD,An,Xn.Size*Scale],OD)
([S,An,Xn.Size*Scale],OD)
70
∂
S
71
∂
S
72
*±∂
S
73
*±∂(Xn.Size*Scale)
S(Xn.Size*Scale)
73
(*±∂,Xn.Size*Scale)
(S,Xn.Size*Scale)
73
([*±∂],Xm.Size*Scale,OD)
([S],Xm.Size*Scale,OD)
73
([*±∂,Xn.Size*Scale],OD)
([S,Xn.Size*Scale],OD)
74
#data
S (#data made comment)
A-traps $AXXX
S (as opcode, AXXX made comment)
========================================================================
For A-traps, the substitution can be performed to substitute for the DC.W
opcode string. If the substitution is made then the Disassembler will
generate ,Sys and/or ,Immed flags as operands for Toolbox traps and
,AutoPop for OS traps when the bits in the trap word indicates these
settings.
|
Generated
|
Substituted
| Opcode Operand Comment
| Opcode Operand
Comment
========================================================================
Toolbox | DC.W
$AXXX
; TB XXXX | S
[,Sys][,Immed] ; AXXX
OS
| DC.W
$AXXX
; OS XXXX | S
[,AutoPop]
; AXXX
========================================================================
All displacements (∂, BD, OD) are hexadecimal values shown as a byte
($XX), word ($XXXX), or long ($XXXXXXXX) as appropriate. The *Scale is
suppressed if 1. The Size is W or L. Note that effective address
substitutions can only be made for "∂(An)", "BD,An", and "*±∂" cases.
For all the effective address modes 5, 6n, 7n, and for A-traps, a
coroutine (a procedure) whose address is specified by the LookupProc
parameter is called by the Disassembler (if LookupProc is not NIL) to
do the substitution (or A-trap comment) with a string returned by the
proc. It is assumed that the proc pointed to by LookupProc is a level 1
Pascal proc declared as follows:
PROCEDURE Lookup(

PC:

UNIV Ptr;

VAR S:
or in C,

{Addr of extension/trap word}
BaseReg: LookupRegs;
{Base register/lookup mode }
Opnd:
UNIV LongInt; {Trap word, PC addr, disp. }
Str255);
{Returned substitution}


pascal void LookUp(Ptr

PC

PC,

/* Addr of extension/trap word */
LookupRegs BaseReg, /* Base register/lookup mode
*/
long
Opnd,
/* Trap word, PC addr, disp.
*/
char
*S);
/* Returned substitution

= Pointer to instruction extension word or A-trap word in the
buffer pointed to by the Disassembler's FirstByte parameter.

BaseReg = This determines the meaning of the Opnd value and supplies
the base register for the "∂(An)", "BD,An", and "*±∂" cases.
BaseReg may contain any one of the following values:
_A0_
_A1_
_A2_
_A3_
_A4_
_A5_
_A6_
_A7_
_PC_
_ABS_
_TRAP_
_IMM_

= 0 ==>
= 1 ==>
= 2 ==>
= 3 ==>
= 4 ==>
= 5 ==>
= 6 ==>
= 7 ==>
= 8 ==>
= 9 ==>
= 10 ==>
= 11 ==>

A0
A1
A2
A3
A4
A5
A6
A7
PC-relative
Abs addr
Trap word
Immediate

(special
(special
(special
(special

case)
case)
case)
case)

For absolute addressing (modes 70 and 71), BaseReg contains
_ABS_. For A-traps, BaseReg would contain _TRAP_. For
immediate data (mode 74), BaseReg would contain _IMM_.
Opnd

= The contents of this LongInt is determined by the BaseReg
parameter just described.
For BaseReg = _IMM_ (immediate data):
Opnd contains the (extended) 32-bit immediate data specified
by the instruction.
For BaseReg = _TRAP_ (A-traps):
Opnd is the entire trap word. The high order 16 bits of
Opnd are zero.
For BaseReg = _ABS_ (absolute effective address):
Opnd contains the (extended) 32-bit address specifed by
the instruction's effective address. Such addresses would
generally be used to reference low memory globals on a
Macintosh.
For BaseReg = _PC_ (PC-relative effective address):
Opnd contains the 32-bit address represented by "*±∂"
adjusted by the Disassembler's DstAdjust parameter.
For BaseReg = _An_ (effective address with a base register):
Opnd contains the (sign-extended) 32-bit (base)
displacement from the instruction's effective address.
In the Macintosh environment, a BaseReg specifying A5
*/

implies either global data references or Jump Table
references. Positive Opnd values with an A5 BaseReg thus
mean Jump Table references, while a negative offset would
mean a global data reference. Base registers of A6 or A7
would usually mean local data.
S

= Pascal string returned from Lookup containing the effective
address substitution string or a trap name for A-traps. S is
set to null PRIOR to calling Lookup. If it is still null on
return, the string is not used. If not null, then for A-traps,
the returned string is used as the opcode string. In all other
cases the string is substituted as shown in the above table.

Depending on the application, the caller has three choices on how to
use the Disassembler and an associated Lookup proc:
(1). The caller can call just the Disassembler and provide his own Lookup
proc. In that case the calling conventions discussed above must be
followed.
(2). The caller can provide NIL for the LookupProc parameter, in which
case, NO Lookup proc will be called.
(3). The caller can call first InitLookup (described below, a proc
provided with this unit) and pass the address of this unit's
standard Lookup proc when Disassembler is called.
In this case all
the control logic to determine the kind of substitution to be done
is provided for the caller and all that need to be provided by the
user are routines to look up any or all of the following:
•
•
•
•
•
•

PC-relative references
Jump Table references
Absolute address references
Trap names
Immediate data names
References with offsets from base registers

}

PROCEDURE InitLookup(PCRelProc, JTOffProc, TrapProc, AbsAddrProc, IdProc, ImmDataProc: UNIV Ptr);
{Prepare for use of this unit's Lookup proc. When Disassembler is called
and the address of this unit's Lookup proc is specified, then for immediate
data, PC-relative, Jump Table references, A-traps, absolute addresses, and
offsets from a base register, the associated level 1 Pascal proc
specified here is called (if not NIL -- all six addresses are preset to
NIL). The calls assume the following declarations for these procs (see
Lookup, below for further details):
PROCEDURE PCRelProc(Address: UNIV LongInt;
VAR S:
UNIV Str255);
PROCEDURE JTOffProc(A5JTOffset: UNIV Integer;
VAR S:
UNIV Str255);


PROCEDURE TrapNameProc(TrapWord: UNIV Integer;
	VAR S: UNIV Str255);

PROCEDURE AbsAddrProc(AbsAddr: UNIV LongInt;
	VAR S:
UNIV Str255);
PROCEDURE IdProc(BaseReg: LookupRegs;
Offset:
UNIV LongInt;
VAR S:
UNIV Str255);


PROCEDURE ImmDataProc(ImmData: UNIV LongInt;
VAR S:
UNIV Str255);

Note: InitLookup contains initialized data which requires initializing
at load time (this is of concern only to users with assembler
main programs.}

PROCEDURE Lookup(
PC:
UNIV Ptr;

{Addr of extension/trap word}
BaseReg: LookupRegs;
{Base register/lookup mode }
Opnd:
UNIV LongInt; {Trap word, PC addr, disp. }
VAR S:
Str255);
{Returned substitution
}
{This is a standard Lookup proc available to the caller for calls to the
Disassembler. If the caller elects to use this proc, then InitLookup
MUST be called prior to any calls to the Disassembler. All the logic
to determine the type of lookup is done by this proc. For PC-relative,
Jump Table references, A-traps, absolute addresses, and offsets from a
base register, the associated level 1 Pascal proc specified in the
InitLookup call (if not NIL) is called.
This scheme simplifies the Lookup mechanism by allowing the caller
to deal with just the problems related to the application.}

PROCEDURE LookupTrapName(TrapWord: UNIV Integer;
VAR S:
UNIV Str255);
{This is a procedure provided to allow conversion of a trap instruction
(in TrapWord) to its corresponding trap name (in S). It is provided
primarily for use with the Disassembler and its address may be passed to
InitLookup above for use by this unit's Lookup routine. Alternatively,
there is nothing prohibiting the caller from using it directly for other
purposes or by some other Lookup proc.
Note: The tables in this proc make the size of this proc about 9500
bytes. The trap names are fully spelled out in upper and lower
case.}
PROCEDURE ModifyOperand(VAR Operand: UNIV Str255);
{Scan an operand string, i.e., the null terminated Pascal string returned
by the Disassembler (null MUST be present here) and modify negative hex
values to negated positive value. For example, $FFFF(A5) would be
modified to -$0001(A5). The operand to be processed is
passed as the
function's parameter which is edited "in place" and returned to the
caller.
This routine is essentially a pattern matcher and attempts to only
modify 2, 4, and 8 digit hex strings in the operand that "might" be
offsets from a base register. If the matching tests are passed, the
same number of original digits are output (because that indicates a
value's size -- byte, word, or long).
For a hex string to be modified, the following tests must be passed:
• There must have been exactly 2, 4, or 8 digits.
Only hex strings $XX, $XXXX, and $XXXXXXXX are possible candidates
because that is the only way the Disassembler generates offsets.
• Hex string must be delimited by a "(" or a ",".
The "(" allows offsets for $XXXX(An,...) and $XX(An,Xn) addressing
modes. The comma allows for the MC68020 addressing forms.
• The "$X..." must NOT be preceded by a "±".
This eliminates the possibility of modifying the offset of a
PC-relative addressing mode always generated in the form "*±$XXXX".
• The "$X..." must NOT be preceded by a "#".
This eliminates modifying immediate data.
• Value must be negative.
Negative values are the only values we modify.
modified to -$0001.

A value $FFFF is
}

FUNCTION validMacsBugSymbol(symStart, limit: UNIV Ptr;
symbol: StringPtr): StringPtr;
{Check that the bytes pointed to by symStart represents a valid MacsBug
symbol. The symbol must be fully contained in the bytes starting at
symStart, up to, but not including, the byte pointed to by the limit
parameter.
If a valid symbol is NOT found, then NIL is returned as the function's
result. However, if a valid symbol is found, it is copied to symbol (if
it is not NIL) as a null terminated Pascal string, and return a pointer
to where we think the FOLLOWING module begins. In the "old style" cases
(see below) this will always be 8 or 16 bytes after the input symStart.
For new style Apple Pascal and C cases this will depend on the symbol
length, existence of a pad byte, and size of the constant (literal) area.
In all cases, trailing blanks are removed from the symbol.
A valid MacsBug symbol consists of the characters '_', '%', spaces,
digits, and upper/lower case letters in a format determined by the first
two bytes of the symbol as follows:
1st byte | 2nd byte | Byte |
Range
| Range
| Length | Comments
=======================================================================
$20 - $7F | $20 - $7F |
8
| "Old style" MacsBug symbol format
$A0 - $7F | $20 - $7F |
8
| "Old style" MacsBug symbol format
----------------------------------------------------------------------$20 - $7F | $80 - $FF |
16
| "Old style" MacApp symbol ab==>b.a
$A0 - $7F | $80 - $FF |
16
| "Old style" MacApp symbol ab==>b.a


----------------------------------------------------------------------$80
| $01 - $FF |
n
| n = 2nd byte
(Apple symbol)
$81 - $9F | $00 - $FF |
m
| m = BAnd(1st byte,$7F) (Apple symbol)
=======================================================================
The formats are determined by whether bit 7 is set in the first and
second bytes. This bit will removed when we find it or'ed into the first
and/or second valid symbol characters.
The first two formats in the above table are the basic "old style" (preexisting) MacsBug formats. The first byte may or may not have bit 7 set
the second byte is a valid symbol character. The first byte (with bit 7
removed) and the next 7 bytes are assumed to comprise the symbol.
The second pair of formats are also "old style" formats, but used for
MacApp symbols. Bit 7 set in the second character indicates these
formats. The symbol is assumed to be 16 bytes with the second 8 bytes
preceding the first 8 bytes in the generated symbol. For example,
78abcdefgh represents the symbol abcdefgh.12345678.
The last pair of formats are reserved by Apple and generated by the MPW
Pascal and C compilers. In these cases the value of the first byte is
always between $80 and $9F, or with bit 7 removed, between $00 and $1F.
For $00, the second byte is the length of the symbol with that many bytes
following the second byte (thus a max length of 255). Values $01 to $1F
represent the length itself. A pad byte may follow these variable length
cases if the symbol does not end on a word boundary. Following the
symbol and the possible pad byte is a word containing the size of the
constants (literals) generated by the compiler.
Note that if symStart actually does point to a valid MacsBug symbol,
then you may use showMacsBugSymbol to convert the MacsBug symbol bytes to
a string that could be used as a DC.B operand for disassembly purposes.
This string explicitly shows the MacsBug symbol encodings.}

FUNCTION endOfModule(address, limit: UNIV Ptr; symbol: StringPtr;
VAR nextModule: UNIV Ptr): StringPtr; 
{Check to see if the specified memory address, contains a RTS, JMP (A0) or
RTD #n instruction immediately followed by a valid MacsBug symbol. These
sequences are the only ones which can determine an end of module when
MacsBug symbols are present. During the check, the instruction and its
following MacsBug symbol must be fully contained in the bytes starting at
the specified address parameter, up to, but not including, the byte
pointed to by the limit parameter.
If the end of module is NOT found, then NIL is returned as the
function's result. However, if a end of module is found, the MacsBug
symbol is returned in symbol (if it is not NIL) as a null terminated
Pascal string (with trailing blanks removed), and the functions returns
the pointer to the start of the MacsBug symbol (i.e., address+2 for RTS
or JMP (A0) and address+4 for RTD #n). This address may then be used as
in input parameter to showMacsBugSymbol to convert the MacsBug symbol to
a Disassembler operand string.
Also returned in nextModule is where we think the FOLLOWING module
begins. In the "old style" cases (see validMacsBugSymbol) this will

always be 8 or 16 bytes after the input address. For new style the
Apple Pascal and C cases this will depend on the symbol length, existence
of a pad byte, and size of the constant (literal) area. See
validMacsBugSymbol for a description of valid MacsBug symbol formats.}

FUNCTION showMacsBugSymbol(symStart, limit: UNIV Ptr; operand: StringPtr;
VAR bytesUsed: Integer): StringPtr; 

{Format a MacsBug symbol as a operand of a DC.B directive. The first one
or two bytes of the symbol are generated as $80+'c' if they have there
high high bits set. All other characters are shown as characters in a
string constant. The pad byte, if present, is one is also shown as $00.
When called, showMacsBugSymbol assumes that symStart is pointing at a
valid MacsBug symbol as validated by the validMacsBugSymbol or
endOfModule routines. As with validMacsBugSymbol, the symbol must be
fully contained in the bytes starting at symStart up to, but not
including, the byte pointed to by the limit parameter.
The string is returned in the 'operand' parameter as a null terminated
Pascal string. The function also returns a pointer to this string as its
return value (NIL is returned only if the byte pointed to by the limit
parameter is reached prior to processing the entire symbol -- which
should not happen if properly validated). The number of bytes used for
the symbol is returned in bytesUsed. Due to the way MacsBug symbols are
encoded, bytesUsed may not necessarily be the same as the length of the
operand string.
A valid MacsBug symbol consists of the characters '_', '%', spaces,
digits, and upper/lower case letters in a format determined by the first
two bytes of the symbol as described in the validMacsBugSymbol routine.}
{$ENDC}

{ UsingDisAsmLookup }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE DisAsmLookup.p}



{#####################################################################}
{### FILE: DiskInit.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:30 PM
DiskInit.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1989

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT DiskInit;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingDiskInit}
{$SETC UsingDiskInit := 1}
{$I+}
{$SETC DiskInitIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := DiskInitIncludes}
TYPE
HFSDefaults = RECORD
sigWord: PACKED ARRAY [0..1] OF Byte;
abSize: LONGINT;
clpSize: LONGINT;
nxFreeFN: LONGINT;
btClpSize: LONGINT;
rsrv1: INTEGER;
rsrv2: INTEGER;
rsrv3: INTEGER;
END;

{
{
{
{
{
{
{
{

signature word}
allocation block size in bytes}
clump size in bytes}
next free file number}
B-Tree clump size in bytes}
reserved}
reserved}
reserved}

PROCEDURE DILoad;
PROCEDURE DIUnload;
FUNCTION DIBadMount(where: Point;evtMessage: LONGINT): INTEGER;
FUNCTION DIFormat(drvNum: INTEGER): OSErr;
FUNCTION DIVerify(drvNum: INTEGER): OSErr;
FUNCTION DIZero(drvNum: INTEGER;volName: Str255): OSErr;


{$ENDC}


{ UsingDiskInit }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE DiskInit.p}



{#####################################################################}
{### FILE: Disks.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:30 PM
Disks.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1989

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Disks;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingDisks}
{$SETC UsingDisks := 1}
{$I+}
{$SETC DisksIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$SETC UsingIncludes := DisksIncludes}
TYPE
DriveKind = (sony,hard20);

DrvSts = RECORD
track: INTEGER;
writeProt: SignedByte;
diskInPlace: SignedByte;
installed: SignedByte;
sides: SignedByte;
driveQLink: QElemPtr;
driveQVers: INTEGER;
dQDrive: INTEGER;
dQRefNum: INTEGER;
dQFSID: INTEGER;
CASE DriveKind OF
sony:
(twoSideFmt: SignedByte;

{current track}
{bit 7 = 1 if volume is locked}
{disk in drive}
{drive installed}
{-1 for 2-sided, 0 for 1-sided}
{next queue entry}
{1 for HD20}
{drive number}
{driver reference number}
{file system ID}

{after 1st rd/wrt: 0=1 side, -1=2 side}



needsFlush: SignedByte;
diskErrs: INTEGER);
hard20:
(driveSize: INTEGER;
driveS1: INTEGER;
driveType: INTEGER;
driveManf: INTEGER;
driveChar: SignedByte;
driveMisc: SignedByte);
END;

{-1 for MacPlus drive}
{soft error count}
{drive block size low word}
{drive block size high word}
{1 for HD20}
{1 for Apple Computer, Inc.}
{230 ($E6) for HD20}
{0 -- reserved}

FUNCTION DiskEject(drvNum: INTEGER): OSErr;
FUNCTION SetTagBuffer(buffPtr: Ptr): OSErr;
FUNCTION DriveStatus(drvNum: INTEGER;VAR status: DrvSts): OSErr;

{$ENDC}

{ UsingDisks }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Disks.p}



{#####################################################################}
{### FILE: Editions.p}
{#####################################################################}
{
Created: Tuesday, January 29, 1991 at 6:35 PM
Editions.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1989-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Editions;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingEditions}
{$SETC UsingEditions := 1}
{$I+}
{$SETC EditionsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingMemory}
{$I $$Shell(PInterfaces)Memory.p}
{$ENDC}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$IFC UNDEFINED UsingAliases}
{$I $$Shell(PInterfaces)Aliases.p}
{$ENDC}
{$SETC UsingIncludes := EditionsIncludes}
CONST
{ resource types }
rSectionType = 'sect';

{ ResType of saved SectionRecords }

{ section types }
stSubscriber = $01;
stPublisher = $0A;
sumAutomatic = 0;
sumManual = 1;
pumOnSave = 0;

{ subscriber update mode - Automatically
{ subscriber update mode - Manually }
{ publisher update mode - OnSave

}
}



pumManual = 1;

{ publisher update mode - Manually }

kPartsNotUsed = 0;
kPartNumberUnknown = -1;

{ misc }

kPreviewWidth = 120;
kPreviewHeight = 120;
kPublisherDocAliasFormat = 'alis';
kPreviewFormat = 'prvw';
kFormatListFormat = 'fmts';
{ bits for formatsMask }
kPICTformatMask = 1;
kTEXTformatMask = 2;
ksndFormatMask = 4;
{ Finder types for edition files }
kPICTEditionFileType = 'edtp';
kTEXTEditionFileType = 'edtt';
ksndEditionFileType = 'edts';
kUnknownEditionFileType = 'edtu';
{ pseudo-item hits for dialogHooks
the first if for NewPublisher or NewSubscriber Dialogs }
emHookRedrawPreview = 150;
{ the following are for SectionOptions Dialog }
emHookCancelSection = 160;
emHookGoToPublisher = 161;
emHookGetEditionNow = 162;
emHookSendEditionNow = 162;
emHookManualUpdateMode = 163;
emHookAutoUpdateMode = 164;
{ the refcon field of the dialog record during a modalfilter
or dialoghook contains one the following }
emOptionsDialogRefCon = 'optn';
emCancelSectionDialogRefCon = 'cncl';
emGoToPubErrDialogRefCon = 'gerr';
kFormatLengthUnknown = -1;
TYPE
SectionType = SignedByte;
TimeStamp = LONGINT;
FormatType = PACKED ARRAY [1..4] OF CHAR;
EditionRefNum = Handle;
{ update modes }
UpdateMode = INTEGER;
SectionPtr = ^SectionRecord;
SectionHandle = ^SectionPtr;
SectionRecord = RECORD
version: SignedByte;
kind: SectionType;
mode: UpdateMode;

{
{
{
{

one byte, stSubscriber or stPublisher }
seconds since 1904 }
similar to ResType as used by scrap mgr }
used in Edition I/O }

{ sumAutomatic, pumSuspend, etc }

{ always 0x01 in system 7.0 }
{ stSubscriber or stPublisher }
{ auto or manual }


mdDate: TimeStamp;
sectionID: LONGINT;
refCon: LONGINT;
alias: AliasHandle;
subPart: LONGINT;
nextSection: SectionHandle;
controlBlock: Handle;
refNum: EditionRefNum;
END;

{ last change in document }
{ app. specific, unique per document }
{ application specific }
{ handle to Alias Record }
{ which part of container file }
{ for linked list of app's Sections }
{ used internally }
{ used internally }

EditionContainerSpecPtr = ^EditionContainerSpec;
EditionContainerSpec = RECORD
theFile: FSSpec;
theFileScript: ScriptCode;
thePart: LONGINT;
thePartName: Str31;
thePartScript: ScriptCode;
END;
EditionInfoRecord = RECORD
crDate: TimeStamp;
mdDate: TimeStamp;
fdCreator: OSType;
fdType: OSType;
container: EditionContainerSpec;
END;
NewPublisherReply = RECORD
canceled: BOOLEAN;
replacing : BOOLEAN;
usePart: BOOLEAN;
preview: Handle;
previewFormat: FormatType;
container: EditionContainerSpec;
END;
NewSubscriberReply = RECORD
canceled: BOOLEAN;
formatsMask: SignedByte;
container: EditionContainerSpec;
END;

{ date EditionContainer was created }
{ date of last change }
{ file creator }
{ file type }
{ the Edition }

SectionOptionsReply = RECORD
canceled: BOOLEAN;
changed: BOOLEAN;
sectionH: SectionHandle;
action: ResType;
END;
ExpModalFilterProcPtr = ProcPtr;
ExpDlgHookProcPtr = ProcPtr;

{ FUNCTION Filter(theDialog: DialogPtr; VAR theEvent: EventRecord; itemOffset: INTEGER; VAR itemHit: INTEGER; your }
{ FUNCTION Hook(itemOffset, item: INTEGER; theDialog: DialogPtr; yourDataPtr: Ptr): INTEGER; }

FormatIOVerb = (ioHasFormat,ioReadFormat,ioNewFormat,ioWriteFormat);

FormatIOParamBlock = RECORD
ioRefNum: LONGINT;
format: FormatType;
formatIndex: LONGINT;
offset: LONGINT;
buffPtr: Ptr;
buffLen: LONGINT;
END;

FormatIOProcPtr = ProcPtr;

{ FUNCTION IO(selector: FormatIOVerb; VAR PB: FormatIOParamBlock): OSErr; }

EditionOpenerVerb = (eoOpen,eoClose,eoOpenNew,eoCloseNew,eoCanSubscribe);

EditionOpenerParamBlock = RECORD
info: EditionInfoRecord;
sectionH: SectionHandle;
document: FSSpecPtr;
fdCreator: OSType;
ioRefNum: LONGINT;
ioProc: FormatIOProcPtr;
success: BOOLEAN;
formatsMask: SignedByte;
END;

EditionOpenerProcPtr = ProcPtr;

{ FUNCTION Opener(selector: EditionOpenerVerb; VAR PB: EditionOpenerParamBlock): OSErr; }

CONST
{
Section events now arrive in the message buffer using the AppleEvent format.
The direct object parameter is an aeTemporaryIDParamType ('tid '). The temporary
ID's type is rSectionType ('sect') and the 32-bit value is a SectionHandle.
The following is a sample buffer
name
---header
majorVersion
minorVersion
endOfMetaData
directObjKey
paramType
paramLength
tempIDType
tempID

offset
-----0
4
6
8
12
16
20
24
28

contents
-------'aevt'
0x01
0x01
';;;;'
'----'
'tid '
0x0008
'sect'
the SectionHandle <-- this is want you want}

sectionEventMsgClass = 'sect';
sectionReadMsgID = 'read';
sectionWriteMsgID = 'writ';
sectionScrollMsgID = 'scrl';
sectionCancelMsgID = 'cncl';


FUNCTION InitEditionPack: OSErr;
INLINE $3F3C,$0011,$303C,$0100,$A82D;
FUNCTION NewSection(container: EditionContainerSpec;
sectionDocument: FSSpecPtr;
kind: SectionType;
sectionID: LONGINT;
initalMode: UpdateMode;
VAR sectionH: SectionHandle): OSErr;
INLINE $303C,$0A02,$A82D;
FUNCTION RegisterSection(sectionDocument: FSSpec;
sectionH: SectionHandle;
VAR aliasWasUpdated: BOOLEAN): OSErr;
INLINE $303C,$0604,$A82D;
FUNCTION UnRegisterSection(sectionH: SectionHandle): OSErr;
INLINE $303C,$0206,$A82D;
FUNCTION IsRegisteredSection(sectionH: SectionHandle): OSErr;
INLINE $303C,$0208,$A82D;
FUNCTION AssociateSection(sectionH: SectionHandle;
newSectionDocument: FSSpecPtr): OSErr;
INLINE $303C,$040C,$A82D;
FUNCTION CreateEditionContainerFile(editionFile: FSSpec;
fdCreator: OSType;
editionFileNameScript: ScriptCode): OSErr;
INLINE $303C,$050E,$A82D;
FUNCTION DeleteEditionContainerFile(editionFile: FSSpec): OSErr;
INLINE $303C,$0210,$A82D;
FUNCTION OpenEdition(subscriberSectionH: SectionHandle;
VAR refNum: EditionRefNum): OSErr;
INLINE $303C,$0412,$A82D;
FUNCTION OpenNewEdition(publisherSectionH: SectionHandle;
fdCreator: OSType;
publisherSectionDocument: FSSpecPtr;
VAR refNum: EditionRefNum): OSErr;
INLINE $303C,$0814,$A82D;
FUNCTION CloseEdition(whichEdition: EditionRefNum;
successful: BOOLEAN): OSErr;
INLINE $303C,$0316,$A82D;
FUNCTION EditionHasFormat(whichEdition: EditionRefNum;
whichFormat: FormatType;
VAR formatSize: Size): OSErr;
INLINE $303C,$0618,$A82D;
FUNCTION ReadEdition(whichEdition: EditionRefNum;
whichFormat: FormatType;
buffPtr: UNIV Ptr;
VAR buffLen: Size): OSErr;
INLINE $303C,$081A,$A82D;
FUNCTION WriteEdition(whichEdition: EditionRefNum;
whichFormat: FormatType;
buffPtr: UNIV Ptr;
buffLen: Size): OSErr;
INLINE $303C,$081C,$A82D;
FUNCTION GetEditionFormatMark(whichEdition: EditionRefNum;
whichFormat: FormatType;
VAR currentMark: LONGINT): OSErr;
INLINE $303C,$061E,$A82D;
FUNCTION SetEditionFormatMark(whichEdition: EditionRefNum;


whichFormat: FormatType;
setMarkTo: LONGINT): OSErr;

INLINE $303C,$0620,$A82D;
FUNCTION GetEditionInfo(sectionH: SectionHandle;
VAR editionInfo: EditionInfoRecord): OSErr;
INLINE $303C,$0422,$A82D;
FUNCTION GoToPublisherSection(container: EditionContainerSpec): OSErr;
INLINE $303C,$0224,$A82D;
FUNCTION GetLastEditionContainerUsed(VAR container: EditionContainerSpec): OSErr;
INLINE $303C,$0226,$A82D;
FUNCTION GetStandardFormats(container: EditionContainerSpec;
VAR previewFormat: FormatType;
preview: Handle;
publisherAlias: Handle;
formats: Handle): OSErr;
INLINE $303C,$0A28,$A82D;
FUNCTION GetEditionOpenerProc(VAR opener: EditionOpenerProcPtr): OSErr;
INLINE $303C,$022A,$A82D;
FUNCTION SetEditionOpenerProc(opener: EditionOpenerProcPtr): OSErr;
INLINE $303C,$022C,$A82D;
FUNCTION CallEditionOpenerProc(selector: EditionOpenerVerb;
VAR PB: EditionOpenerParamBlock;
routine: EditionOpenerProcPtr): OSErr;
INLINE $303C,$052E,$A82D;
FUNCTION CallFormatIOProc(selector: FormatIOVerb;
VAR PB: FormatIOParamBlock;
routine: FormatIOProcPtr): OSErr;
INLINE $303C,$0530,$A82D;
FUNCTION NewSubscriberDialog(VAR reply: NewSubscriberReply): OSErr;
INLINE $303C,$0232,$A82D;
FUNCTION NewSubscriberExpDialog(VAR reply: NewSubscriberReply;
where: Point;
expansionDITLresID: INTEGER;
dlgHook: ExpDlgHookProcPtr;
filterProc: ExpModalFilterProcPtr;
yourDataPtr:UNIV Ptr): OSErr;
INLINE $303C,$0B34,$A82D;
FUNCTION NewPublisherDialog(VAR reply: NewPublisherReply): OSErr;
INLINE $303C,$0236,$A82D;
FUNCTION NewPublisherExpDialog(VAR reply: NewPublisherReply;
where: Point;
expansionDITLresID: INTEGER;
dlgHook: ExpDlgHookProcPtr;
filterProc: ExpModalFilterProcPtr;
yourDataPtr: UNIV Ptr): OSErr;
INLINE $303C,$0B38,$A82D;
FUNCTION SectionOptionsDialog(VAR reply: SectionOptionsReply): OSErr;
INLINE $303C,$023A,$A82D;
FUNCTION SectionOptionsExpDialog(VAR reply: SectionOptionsReply;
where: Point;
expansionDITLresID: INTEGER;
dlgHook: ExpDlgHookProcPtr;
filterProc: ExpModalFilterProcPtr;
yourDataPtr: UNIV Ptr): OSErr;
INLINE $303C,$0B3C,$A82D;

{$ENDC}


{ UsingEditions }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Editions.p}


{#####################################################################}
{### FILE: ENET.p}
{#####################################################################}

{
Created: Sunday, September 15, 1991 at 10:23 PM
ENET.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1990-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT ENET;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingENET}
{$SETC UsingENET := 1}
{$I+}
{$SETC ENETIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$SETC UsingIncludes := ENETIncludes}
CONST
ENetSetGeneral = 253;
ENetGetInfo = 252;
ENetRdCancel = 251;
ENetRead = 250;
ENetWrite = 249;
ENetDetachPH = 248;
ENetAttachPH = 247;
ENetAddMulti = 246;
ENetDelMulti = 245;

{Set "general" mode}
{Get info}
{Cancel read}
{Read}
{Write}
{Detach protocol handler}
{Attach protocol handler}
{Add a multicast address}
{Delete a multicast address}

eLenErr = -92;
eMultiErr = -91;

{Length error ddpLenErr}
{Multicast address error ddpSktErr}

EAddrRType = 'eadr';

{Alternate address resource type}

TYPE
EParamBlkPtr = ^EParamBlock;
EParamBlock = PACKED RECORD
qLink: QElemPtr;
{next queue entry}
qType: INTEGER;
{queue type}
ioTrap: INTEGER;
{routine trap}
ioCmdAddr: Ptr;
{routine address}
ioCompletion: ProcPtr;
{completion routine}
ioResult: OSErr;
{result code}
ioNamePtr: StringPtr;
{->filename}
ioVRefNum: INTEGER;
{volume reference or drive number}
ioRefNum: INTEGER;
{driver reference number}
csCode: INTEGER;
{call command code AUTOMATICALLY set}
CASE INTEGER OF
ENetWrite,ENetAttachPH,ENetDetachPH,ENetRead,ENetRdCancel,ENetGetInfo,ENetSetGeneral:
(eProtType: INTEGER;
{Ethernet protocol type}
ePointer: Ptr;
eBuffSize: INTEGER;
{buffer size}
eDataSize: INTEGER);
{number of bytes read}
ENetAddMulti,ENetDelMulti:
(eMultiAddr: ARRAY [0..5] of char);
{Multicast Address}
END;

FUNCTION EWrite(thePBptr: EParamBlkPtr;async: BOOLEAN): OSErr;
FUNCTION EAttachPH(thePBptr: EParamBlkPtr;async: BOOLEAN): OSErr;
FUNCTION EDetachPH(thePBptr: EParamBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ERead(thePBptr: EParamBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ERdCancel(thePBptr: EParamBlkPtr;async: BOOLEAN): OSErr;
FUNCTION EGetInfo(thePBptr: EParamBlkPtr;async: BOOLEAN): OSErr;
FUNCTION ESetGeneral(thePBptr: EParamBlkPtr;async: BOOLEAN): OSErr;
FUNCTION EAddMulti(thePBptr: EParamBlkPtr;async: BOOLEAN): OSErr;
FUNCTION EDelMulti(thePBptr: EParamBlkPtr;async: BOOLEAN): OSErr;

{$ENDC} { UsingENET }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE ENET.p}
{#####################################################################}
{### FILE: EPPC.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:32 PM
EPPC.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT EPPC;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingEPPC}
{$SETC UsingEPPC := 1}
{$I+}
{$SETC EPPCIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingPPCToolbox}
{$I $$Shell(PInterfaces)PPCToolbox.p}
{$ENDC}
{$IFC UNDEFINED UsingProcesses}
{$I $$Shell(PInterfaces)Processes.p}
{$ENDC}
{$IFC UNDEFINED UsingEvents}
{$I $$Shell(PInterfaces)Events.p}
{$ENDC}
{$SETC UsingIncludes := EPPCIncludes}
CONST
kHighLevelEvent = 23;
{ postOptions currently supported }
receiverIDMask = $0000F000;
receiverIDisPSN = $00008000;
receiverIDisSignature = $00007000;
receiverIDisSessionID = $00006000;
receiverIDisTargetID = $00005000;
systemOptionsMask = $00000F00;
nReturnReceipt = $00000200;
priorityMask = $000000FF;
nAttnMsg = $00000001;

{ error returns from Post and Accept }
bufferIsSmall = -607;
noOutstandingHLE = -608;
connectionInvalid = -609;
noUserInteractionAllowed = -610;

{ no user interaction allowed }

{ constant for return receipts }
HighLevelEventMsgClass = 'jaym';
rtrnReceiptMsgID = 'rtrn';
msgWasPartiallyAccepted = 2;
msgWasFullyAccepted = 1;
msgWasNotAccepted = 0;
TYPE
TargetIDPtr = ^TargetID;
TargetIDHdl = ^TargetIDPtr;
TargetID = RECORD
sessionID: LONGINT;
name: PPCPortRec;
location: LocationNameRec;
recvrName: PPCPortRec;
END;

SenderID = TargetID;
SenderIDPtr = ^SenderID;
HighLevelEventMsgPtr = ^HighLevelEventMsg;
HighLevelEventMsgHdl = ^HighLevelEventMsgPtr;
HighLevelEventMsg = RECORD
HighLevelEventMsgHeaderLength: INTEGER;
version: INTEGER;
reserved1: LONGINT;
theMsgEvent: EventRecord;
userRefcon: LONGINT;
postingOptions: LONGINT;
msgLength: LONGINT;
END;

FUNCTION PostHighLevelEvent(theEvent: EventRecord;
receiverID: Ptr;
msgRefcon: LONGINT;
msgBuff: Ptr;
msgLen: LONGINT;
postingOptions: LONGINT): OSErr;
INLINE $3F3C,$0034,$A88F;
FUNCTION AcceptHighLevelEvent(VAR sender: TargetID;
VAR msgRefcon: LONGINT;
msgBuff: Ptr;
VAR msgLen: LONGINT): OSErr;
INLINE $3F3C,$0033,$A88F;
FUNCTION GetProcessSerialNumberFromPortName(portName: PPCPortRec;VAR PSN: ProcessSerialNumber): OSErr;



INLINE $3F3C,$0035,$A88F;
FUNCTION GetPortNameFromProcessSerialNumber(VAR portName: PPCPortRec;PSN: ProcessSerialNumber): OSErr;
INLINE $3F3C,$0046,$A88F;
TYPE
GetSpecificFilterProcPtr = ProcPtr;

{ FUNCTION MyFilter(yourDataPtr: Ptr;
msgBuff: HighLevelEventMsgPtr;
sender: TargetID): Boolean;
}

FUNCTION GetSpecificHighLevelEvent(aFilter: GetSpecificFilterProcPtr;yourDataPtr: UNIV Ptr;
VAR err: OSErr): BOOLEAN;
INLINE $3F3C,$0045,$A88F;

{$ENDC}

{ UsingEPPC }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE EPPC.p}
{#####################################################################}
{### FILE: ErrMgr.p}
{#####################################################################}
{
Created: Tuesday, August 2, 1988 at 12:26 PM
ErrMgr.p
Pascal Interface to the Macintosh Libraries

<<< ErrMgr - Error File Manager Routines Interface File >>>

Copyright Apple Computer, Inc.
All rights reserved

1987-1988

This file contains:
InitErrMgr(toolname, sysename, Nbrs)
CloseErrMgr()
GetSysErrText(Nbr, Msg)
GetToolErrText(Nbr, Insert, Msg)
AddErrInsert(insert, msgString)

-

ErrMgr initialization
Close ErrMgr message files
Get a system error message for a number
Get a tool error message for a number
Add an insert to a message

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT ErrMgr;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingErrMgr}
{$SETC UsingErrMgr := 1}
{$I+}
{$SETC ErrMgrIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := ErrMgrIncludes}

PROCEDURE InitErrMgr(toolErrFilename: Str255;sysErrFilename: Str255;showToolErrNbrs: BOOLEAN);
{ ErrMgr initialization.This must be done before using any other ErrMgr
routine. Set showToolErrNbrs to true if you want all tool messages to contain
the error number following the message text enclosed in parentheses (e.g.,
"<msg txt> ([OS] Error <n>)"; system error messages always contain the error
number). The toolErrFileName parameter is used to specify the name of a
tool-specific error file, and should be the NULL or a null string if not used
(or if the tool's data fork is to be used as the error file, see
GetToolErrText for futher details). The sysErrFileName parameter is used to
specify the name of a system error file, and should normally be the NULL or a
null string, which causes the ErrMgr to look in the MPW Shell directory for
"SysErrs.Err" (see GetSysErrText). }
PROCEDURE CloseErrMgr; {c;}
{ Ideally a CloseErrMgr should be done at the end of execution to make sure all
files opened by the ErrMgr are closed. You can let normal program termination
do the closing. But if you are a purist...
}
PROCEDURE GetSysErrText(msgNbr: INTEGER;errMsg: StringPtr);
(* Get the error message text corresponding to system error number errNbr from
the system error message file (whose name was specified in the InitErrMgr
call). The text of the message is returned in errMsg and the function returns
a pointer to errMsg. The maximum length of the message is limited to 254
characters.
Note, if a system message filename was not specified to InitErrMgr, then the
ErrMgr assumes the message file contained in the file "SysErrs.Err". This
file is first accessed as "
{ShellDirectory}SysErrs.Err" on the assumption that
SysErrs.Err is kept in the same directory as the MPW Shell. If the file
cannot be opened, then an open is attempted on "SysErrs.Err" in the System
Folder. *)
PROCEDURE AddErrInsert(insert: Str255;msgString: StringPtr); {C;}
{ Add another insert to an error message string.This call is used when more
than one insert is to be added to a message (because it contains more than
one '^' character).
}
PROCEDURE GetToolErrText(msgNbr: INTEGER;errInsert: Str255;errMsg: StringPtr);
(* Get the error message text corresponding to tool error number errNbr from
the tool error message file (whose name was specified in the InitErrMgr
call). The text of the message is returned in errMsg and the function returns
a pointer to errMsg. The maximum length of the message is limited to 254
characters. If the message is to have an insert, then ErrInsert should be a
pointer to it. Otherwise it should be either be a null string or a NULL
pointer.
Inserts are indicated in error messages by specifying a '^' to indicate where
the insert is to be placed.
Note, if a tool message filename was not specified to InitErrMgr, then the
ErrMgr assumes the message file contained in the data fork of the tool calling
the ErrMgr. This name is contained in the Shell variable
{Command} and the
value of that variable is used to open the error message file. *)

{$ENDC}

{ UsingErrMgr }

{$IFC NOT UsingIncludes}
END.
{$ENDC}



{### END OF FILE ErrMgr.p}





{#####################################################################}
{### FILE: Errors.p}
{#####################################################################}
{
Created: Thursday, March 14, 1991 at 4:02 PM
Errors.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Errors;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingErrors}
{$SETC UsingErrors := 1}

CONST
paramErr = -50;
noHardwareErr = -200;
notEnoughHardwareErr = -201;
userCanceledErr = -128;
qErr = -1;
vTypErr = -2;
corErr = -3;
unimpErr = -4;
SlpTypeErr = -5;
seNoDB = -8;
controlErr = -17;
statusErr = -18;
readErr = -19;
writErr = -20;
badUnitErr = -21;
unitEmptyErr = -22;
openErr = -23;
closErr = -24;
dRemovErr = -25;
dInstErr = -26;
abortErr = -27;
iIOAbortErr = -27;
notOpenErr = -28;
unitTblFullErr = -29;

{error in user parameter list}
{Sound Manager Error Returns}
{Sound Manager Error Returns}
{queue element not found during deletion}
{invalid queue element}
{core routine number out of range}
{unimplemented core routine}
{invalid queue element}
{no debugger installed to handle debugger
{I/O System Errors}
{I/O System Errors}
{I/O System Errors}
{I/O System Errors}
{I/O System Errors}
{I/O System Errors}
{I/O System Errors}
{I/O System Errors}
{tried to remove an open driver}
{DrvrInstall couldn't find driver in
{IO call aborted by KillIO}
{IO abort error (Printing Manager)}
{Couldn't rd/wr/ctl/sts cause driver not
{unit table has no more entries}



dceExtErr = -30;
{dce extension error}
slotNumErr = -360;
{invalid slot # error}
gcrOnMFMErr = -400;
{gcr format on high density media error}
dirFulErr = -33;
{Directory full}
dskFulErr = -34;
{disk full}
nsvErr = -35;
{no such volume}
ioErr = -36;
{I/O error (bummers)}
bdNamErr = -37;
{there may be no bad names in the final
system!}
fnOpnErr = -38;
{File not open}
eofErr = -39;
{End of file}
posErr = -40;
{tried to position to before start of file
(r/w)}
mFulErr = -41;
{memory full (open) or file won't fit
(load)}
tmfoErr = -42;
{too many files open}
fnfErr = -43;
{File not found}
wPrErr = -44;
{diskette is write protected.}
fLckdErr = -45;
{file is locked}
vLckdErr = -46;
{volume is locked}
fBsyErr = -47;
{File is busy (delete)}
dupFNErr = -48;
{duplicate filename (rename)}
opWrErr = -49;
{file already open with with write
permission}
rfNumErr = -51;
{refnum error}
gfpErr = -52;
{get file position error}
volOffLinErr = -53;
{volume not on line error (was Ejected)}
permErr = -54;
{permissions error (on file open)}
volOnLinErr = -55;
{drive volume already on-line at MountVol}
nsDrvErr = -56;
{no such drive (tried to mount a bad drive
num)}
noMacDskErr = -57;
{not a mac diskette (sig bytes are wrong)}
extFSErr = -58;
{volume in question belongs to an external
fs}
fsRnErr = -59;
{file system internal error:during rename
the old entry was deleted but could not be restored.}
badMDBErr = -60;
{bad master directory block}
wrPermErr = -61;
{write permissions error}
dirNFErr = -120;
{Directory not found}
tmwdoErr = -121;
{No free WDCB available}
badMovErr = -122;
{Move into offspring error}
wrgVolTypErr = -123;
{Wrong volume type error [operation not
supported for MFS]}
volGoneErr = -124;
{Server volume has been disconnected.}
fidNotFound = -1300;
{no file thread exists.}
fidExists = -1301;
{file id already exists}
notAFileErr = -1302;
{directory specified}
diffVolErr = -1303;
{files on different volumes}
catChangedErr = -1304;
{the catalog has been modified}
desktopDamagedErr = -1305;
{desktop database files are corrupted}
sameFileErr = -1306;
{can't exchange a file with itself}
badFidErr = -1307;
{file id is dangling or doesn't match with
the file number}
envNotPresent = -5500;
{returned by glue.}
envBadVers = -5501;
{Version non-positive}
envVersTooBig = -5502;
{Version bigger than call can handle}

fontDecError = -64;
fontNotDeclared = -65;
fontSubErr = -66;
fontNotOutlineErr = -32615;
outlines only}
firstDskErr = -84;
lastDskErr = -64;
noDriveErr = -64;
offLinErr = -65;
noNybErr = -66;
noAdrMkErr = -67;
dataVerErr = -68;
badCksmErr = -69;
badBtSlpErr = -70;
noDtaMkErr = -71;
badDCksum = -72;
badDBtSlp = -73;
wrUnderrun = -74;
cantStepErr = -75;
tk0BadErr = -76;
initIWMErr = -77;
twoSideErr = -78;
drive}
spdAdjErr = -79;
seekErr = -80;
sectNFErr = -81;
fmt1Err = -82;
fmt2Err = -83;
verErr = -84;
clkRdErr = -85;
clkWrErr = -86;
prWrErr = -87;
prInitErr = -88;
uninitialized}
rcvrErr = -89;
breakRecd = -90;
{Power Manager Errors}
pmBusyErr = -13000;
pmReplyTOErr = -13001;
pmSendStartErr = -13002;
pmSendEndErr = -13003;
pmRecvStartErr = -13004;
pmRecvEndErr = -13005;
{configured for this connection}
{Scrap Manager errors}
noScrapErr = -100;
noTypeErr = -102;
memROZWarn = -99;
memROZError = -99;
memROZErr = -99;
memFullErr = -108;
nilHandleErr = -109;
{other}
memWZErr = -111;


{error during font declaration}
{font not declared}
{font substitution occured}
{bitmap font passed to routine that does
{I/O System Errors}
{I/O System Errors}
{drive not installed}
{r/w requested for an off-line drive}
{couldn't find 5 nybbles in 200 tries}
{couldn't find valid addr mark}
{read verify compare failed}
{addr mark checksum didn't check}
{bad addr mark bit slip nibbles}
{couldn't find a data mark header}
{bad data mark checksum}
{bad data mark bit slip nibbles}
{write underrun occurred}
{step handshake failed}
{track 0 detect doesn't change}
{unable to initialize IWM}
{tried to read 2nd side on a 1-sided
{unable to correctly adjust disk speed}
{track number wrong on address mark}
{sector number never found on a track}
{can't find sector 0 after track format}
{can't get enough sync}
{track failed to verify}
{unable to read same clock value twice}
{time written did not verify}
{parameter ram written didn't read-verify}
{InitUtil found the parameter ram
{SCC receiver error (framing; parity; OR)}
{Break received (SCC)}

{Power Mgr never ready to start handshake}
{Timed out waiting for reply}
{during send, pmgr did not start hs}
{during send, pmgr did not finish hs}
{during receive, pmgr did not start hs}
{during receive, pmgr did not finish hs

{No scrap exists error}
{No object of that type in scrap}
{soft error in ROZ}
{hard error in ROZ}
{hard error in ROZ}
{Not enough room in heap zone}
{Master Pointer was NIL in HandleZone or
WhichZone failed (applied to free block)}


}}
memPurErr = -112;
{block}
memAdrErr = -110;
memAZErr = -113;
memPCErr = -114;
memBCErr = -115;
memSCErr = -116;
memLockedErr = -117;
resNotFound = -192;
resFNotFound = -193;
addResFailed = -194;
addRefFailed = -195;
rmvResFailed = -196;
rmvRefFailed = -197;
resAttrErr = -198;
mapReadErr = -199;
CantDecompress = -186;
{decompress a compressed resource}
badExtResource = -185;
evtNotEnb = 1;
noMemForPictPlaybackErr = -145;
rgnTooBigError = -147;
pixMapTooDeepErr = -148;
nsStackErr = -149;
cMatchErr = -150;
cTempMemErr = -151;
structures}
cNoMemErr = -152;
cRangeErr = -153;
cProtectErr = -154;
cDevErr = -155;
cResErr = -156;
rgnTooBigErr = -500;
updPixMemErr = -125;
pictInfoVersionErr = -11000;

pictInfoIDErr = -11001;
PictInfoID is wrong }
pictInfoVerbErr = -11002;
cantLoadPickMethodErr = -11003;
colorsRequestedErr = -11004;
{illegal }
pictureDataErr = -11005;
{Sound Manager errors}
noHardware = noHardwareErr;
notEnoughHardware = notEnoughHardwareErr;
queueFull = -203;
resProblem = -204;
badChannel = -205;
badFormat = -206;
notEnoughBufferSpace = -207;
badFileFormat = -208;
{format,corrupt }
channelBusy = -209;
{already }

{trying to purge a locked or non-purgeable
address was odd; or out of range}
{Address in zone check failed}
{Pointer Check failed}
{Block Check failed}
{Size Check failed}
{trying to move a locked block (MoveHHi)}
{Resource not found}
{Resource file not found}
{AddResource failed}
{AddReference failed}
{RmveResource failed}
{RmveReference failed}
{attribute inconsistent with operation}
{map inconsistent with operation}
{resource bent ("the bends") - can't
{extended resource has a bad format.}
{event not enabled at PostEvent}

{Color2Index failed to find an index}
{failed to allocate memory for temporary
failed to allocate memory for structure}
{range error on colorTable request}
{colorTable entry protection violation}
{invalid type of graphics device}
{invalid resolution for MakeITable}
{insufficient memory to update a pixmap}
{ wrong version of the PictInfo structure
{ the internal consistancy check for the
 the passed verb was invalid }
{ unable to load the custom pick proc }
{ the number of colors requested was
 the picture data was invalid }

{*** obsolete spelling}
{*** obsolete spelling}
{Sound Manager Error Returns}
{Sound Manager Error Returns}
{Sound Manager Error Returns}
{Sound Manager Error Returns}
{ could not allocate enough memory }
{ was not type AIFF or was of bad
{ the Channel is being used for a PFD}

buffersTooSmall = -210;
channelNotBusy = -211;
noMoreRealTime = -212;
another task }
siNoSoundInHardware = -220;
siBadSoundInDevice = -221;
SoundInGetIndexedDevice}
siNoBufferSpecified = -222;
buffer passed}
siInvalidCompression = -223;
siHardDriveTooSlow = -224;
siInvalidSampleRate = -225;
siInvalidSampleSize = -226;
siDeviceBusyErr = -227;
siBadDeviceName = -228;
siBadRefNum = -229;
siInputDeviceErr = -230;
siUnknownInfoType = -231;
{driver)}
siUnknownQuality = -232;
{driver)}
{Notification Manager errors}
nmTypErr = -299;
siInitSDTblErr = 1;
{initialized.}
siInitVBLQsErr = 2;
{initialized.}
siInitSPTblErr = 3;
{initialized.}
sdmJTInitErr = 10;
sdmInitErr = 11;
sdmSRTInitErr = 12;
{initialized.}
sdmPRAMInitErr = 13;
sdmPriInitErr = 14;
smSDMInitErr = -290;
smSRTInitErr = -291;
{initialized.}
smPRAMInitErr = -292;
{initialized.}
smPriInitErr = -293;
smEmptySlot = -300;
smCRCFail = -301;
smFormatErr = -302;
smRevisionErr = -303;
smNoDir = -304;
smDisabledSlot = -305;
{smLWTstBad)}
smNosInfoArray = -306;
smResrvErr = -307;
{0.}
smUnExBusErr = -308;
smBLFieldBad = -309;
smFHBlockRdErr = -310;
smFHBlkDispErr = -311;


{ can not operate in the memory allowed }
{ not enough CPU cycles left to add
no Sound Input hardware}
{invalid index passed to
returned by synchronous SPBRecord if nil
invalid compression type}
{hard drive too slow to record to disk}
{invalid sample rate}
{invalid sample size}
{input device already in use}
{input device could not be opened}
{invalid input device reference number}
{input device hardware failure}
{invalid info type selector (returned by}
{invalid quality selector (returned by}

{wrong queue type}
{slot int dispatch table could not be}
{VBLqueues for all slots could not be}
{slot priority table could not be}
{SDM Jump Table could not be initialized.}
{SDM could not be initialized.}
{Slot Resource Table could not be}
{Slot PRAM could not be initialized.}
{Cards could not be initialized.}
{Error; SDM could not be initialized.}
{Error; Slot Resource Table could not be}
{Error; Slot Resource Table could not be}
{Error; Cards could not be initialized.}
{No card in slot}
{CRC check failed for declaration data}
{FHeader Format is not Apple's}
{Wrong revison level}
{Directory offset is Nil }
{This slot is disabled (-305 use to be}
{No sInfoArray. Memory Mgr error.}
{Fatal reserved error. Resreved field <>}
{Unexpected BusError}
{ByteLanes field was bad.}
{Error occured during _sGetFHeader.}
{Error occured during _sDisposePtr}

{(Dispose of FHeader block).}
smDisposePErr = -312;
smNoBoardSRsrc = -313;
smGetPRErr = -314;
{SIMStatus).}
smNoBoardId = -315;
smInitStatVErr = -316;
{primary or secondary init.}
smInitTblVErr = -317;
{initialize the Slot Resource Table.}
smNoJmpTbl = -318;
smBadBoardId = -319;
{record.}
smBusErrTO = -320;


{_DisposePointer error}
{No Board sResource.}
{Error occured during _sGetPRAMRec (See
{No Board Id.}
{The InitStatusV field was negative after
An error occured while trying to
SDM jump table could not be created.}
BoardId was wrong; re-init the PRAM
{BusError time out.}

{ The following errors are for primary or secondary init code. The errors are logged
in the
vendor status field of the sInfo record. Normally the vendor error is not Apple's
concern,
but a special error is needed to patch secondary inits.}

svTempDisable = -32768;
{Temporarily disable card but run primary
init.}
svDisabled = -32640;
{Reserve range -32640 to -32768 for Apple
temp disables.}
smBadRefId = -330;
{Reference Id not found in List}
smBadsList = -331;
{Bad sList: Id1 < Id2 < Id3 ...format is
not followed.}
smReservedErr = -332;
{Reserved field not zero}
smCodeRevErr = -333;
{Code revision is wrong}
smCPUErr = -334;
{Code revision is wrong}
smsPointerNil = -335;
{LPointer is nil From sOffsetData. If this
error occurs; check sInfo rec for more information.}
smNilsBlockErr = -336;
{Nil sBlock error (Dont allocate and try
to use a nil sBlock)}
smSlotOOBErr = -337;
{Slot out of bounds error}
smSelOOBErr = -338;
{Selector out of bounds error}
smNewPErr = -339;
{_NewPtr error}
smBlkMoveErr = -340;
{_BlockMove error}
smCkStatusErr = -341;
{Status of slot = fail.}
smGetDrvrNamErr = -342;
{Error occured during _sGetDrvrName.}
smDisDrvrNamErr = -343;
{Error occured during _sDisDrvrName.}
smNoMoresRsrcs = -344;
{No more sResources}
smsGetDrvrErr = -345;
{Error occurred during _sGetDriver.}
smBadsPtrErr = -346;
{Bad pointer was passed to sCalcsPointer}
smByteLanesErr = -347;
{NumByteLanes was determined to be zero.}
smOffsetErr = -348;
{Offset was too big (temporary error}
smNoGoodOpens = -349;
{No opens were successfull in the loop.}
smSRTOvrFlErr = -350;
{SRT over flow.}
smRecNotFnd = -351;
{Record not found in the SRT.}
editionMgrInitErr = -450;
{edition manager not inited by this app}
badSectionErr = -451;
{not a valid SectionRecord}
notRegisteredSectionErr = -452;
{not a registered SectionRecord}
badEditionFileErr = -453;
{edition file is corrupt}
badSubPartErr = -454;
{can not use sub parts in this release}

multiplePublisherWrn = -460;
{that container}
containerNotFoundWrn = -461;
{time}
containerAlreadyOpenWrn = -462;
notThePublisherWrn = -463;
{that container}
teScrapSizeErr = -501;
hwParamErr = -502;
{ Process Manager errors }
procNotFound = -600;
{descriptor }
memFragErr = -601;
{requirements }
appModeErr = -602;
{bit clean }
protocolErr = -603;
}
hardwareConfigErr = -604;
{call }
appMemFullErr = -605;
{launch }
appIsDaemon = -606;
{disallow this }
{MemoryDispatch errors}
notEnoughMemoryErr = -620;
notHeldErr = -621;
cannotMakeContiguousErr = -622;
notLockedErr = -623;
interruptsMaskedErr = -624;
cannotDeferErr = -625;
ddpSktErr = -91;
ddpLenErr = -92;
noBridgeErr = -93;
lapProtErr = -94;
excessCollsns = -95;
portInUse = -97;
portNotCf = -98;
{configured for this connection)}
nbpBuffOvr = -1024;
nbpNoConfirm = -1025;
nbpConfDiff = -1026;
nbpDuplicate = -1027;
nbpNotFound = -1028;
nbpNISErr = -1029;
aspBadVersNum = -1066;
aspBufTooSmall = -1067;
aspNoMoreSess = -1068;
aspNoServers = -1069;
aspParamErr = -1070;
aspServerBusy = -1071;
aspSessClosed = -1072;
aspSizeErr = -1073;
aspTooMany = -1074;
aspNoAck = -1075;
reqFailed = -1096;
tooManyReqs = -1097;
tooManySkts = -1098;
badATPSkt = -1099;
badBuffNum = -1100;
noRelErr = -1101;
cbNotFound = -1102;
noSendResp = -1103;
noDataArea = -1104;
reqAborted = -1105;
buf2SmallErr = -3101;
noMPPErr = -3102;
ckSumErr = -3103;
extractErr = -3104;
readQErr = -3105;
atpLenErr = -3106;
atpBadRsp = -3107;
recNotFnd = -3108;
sktClosedErr = -3109;
afpAccessDenied = -5000;
afpAuthContinue = -5001;
afpBadUAM = -5002;
afpBadVersNum = -5003;
afpBitmapErr = -5004;
afpCantMove = -5005;
afpDenyConflict = -5006;
afpDirNotEmpty = -5007;
afpDiskFull = -5008;
afpEofError = -5009;
afpFileBusy = -5010;
afpFlatVol = -5011;
afpItemNotFound = -5012;
afpLockErr = -5013;
afpMiscErr = -5014;
afpNoMoreLocks = -5015;
afpNoServer = -5016;
afpObjectExists = -5017;
afpObjectNotFound = -5018;
afpParmErr = -5019;
afpRangeNotLocked = -5020;
afpRangeOverlap = -5021;
afpSessClosed = -5022;
afpUserNotAuth = -5023;
afpCallNotSupported = -5024;
afpObjectTypeErr = -5025;
afpTooManyFilesOpen = -5026;
afpServerGoingDown = -5027;
afpCantRename = -5028;
afpDirNotFound = -5029;
afpIconTypeError = -5030;
afpVolLocked = -5031;
afpObjectLocked = -5032;
afpContainsSharedErr = -5033;
{contains a shared folder }
afpIDNotFound = -5034;





afpIDExists = -5035;
{$FFFFEC55}
afpDiffVolErr = -5036;
{$FFFFEC54}
afpCatalogChanged = -5037;
{$FFFFEC53}
afpSameObjectErr = -5038;
{$FFFFEC52}
afpBadIDErr = -5039;
{$FFFFEC51}
afpPwdSameErr = -5040;
{$FFFFEC50 someone tried to change their
password to the same password on a mantadory password change }
afpPwdTooShortErr = -5041;
{$FFFFEC4F the password being set is too
short: there is a minimum length that must be met or exceeded }
afpPwdExpiredErr = -5042;
{$FFFFEC4E the password being used is too
old: this requires the user to change the password before log-in can continue }
afpInsideSharedErr = -5043;
{$FFFFEC4D the folder being shared is
inside a shared folder OR the folder contains a shared folder and is being moved into
a shared folder OR the folder contains a shared folder and is being moved into the
{descendent of a shared folder. }
afpInsideTrashErr = -5044;
{$FFFFEC4C the folder being shared is
inside the trash folder OR the shared folder is being moved into the trash folder OR
the folder is being moved to the trash and it contains a shared folder }
{PPC errors}
notInitErr = -900;
nameTypeErr = -902;
{locationKindSelector in locationName }
noPortErr = -903;
noGlobalsErr = -904;
localOnlyErr = -905;
destPortErr = -906;
sessTableErr = -907;
noSessionErr = -908;
badReqErr = -909;
{operation }
portNameExistsErr = -910;
{app) }
noUserNameErr = -911;
}
userRejectErr = -912;

noMachineNameErr = -913;
{Network Setup Control Panel }
noToolboxNameErr = -914;
{likely }
noResponseErr = -915;
portClosedErr = -916;
sessClosedErr = -917;
badPortNameErr = -919;
noDefaultUserErr = -922;
{Network Setup Control Pannel }
notLoggedInErr = -923;
{exist }
noUserRefErr = -924;
networkErr = -925;
{too likely }
noInformErr = -926;
{not have inform pending }
authFailErr = -927;
{destination }

noUserRecErr = -928;
badServiceMethodErr = -930;
badLocNameErr = -931;
guestNotAllowedErr = -932;

swOverrunErr = 1;
parityErr = 16;
hwOverrunErr = 32;
framingErr = 64;
dsBusError = 1;
dsAddressErr = 2;
dsIllInstErr = 3;
dsZeroDivErr = 4;
dsChkErr = 5;
dsOvflowErr = 6;
dsPrivErr = 7;
dsTraceErr = 8;
dsLineAErr = 9;
dsLineFErr = 10;
dsMiscErr = 11;
dsCoreErr = 12;
dsIrqErr = 13;
dsIOCoreErr = 14;
dsLoadErr = 15;
dsFPErr = 16;
dsNoPackErr = 17;
dsNoPk1 = 18;
dsNoPk2 = 19;
dsNoPk3 = 20;
dsNoPk4 = 21;
dsNoPk5 = 22;
dsNoPk6 = 23;
dsNoPk7 = 24;
dsMemFullErr = 25;
dsBadLaunch = 26;
dsFSErr = 27;
dsStknHeap = 28;
negZcbFreeErr = 33;
dsFinderErr = 41;
dsBadSlotInt = 51;
dsBadSANEOpcode = 81;
dsBadPatchHeader = 83;
menuPrgErr = 84;
dsMBarNFnd = 85;
dsHMenuFindErr = 86;
dsWDEFNotFound = 87;
dsCDEFNotFound = 88;
dsMDEFNotFound = 89;
dsNoFPU = 90;
dsNoPatch = 98;
dsBadPatch = 99;
dsParityErr = 101;
dsOldSystem = 102;
ds32BitMode = 103;


dsNeedToWriteBootBlocks = 104;
dsNotEnoughRAMToBoot = 105;
dsBufPtrTooLow = 106;
dsReinsert = 30;
shutDownAlert = 42;
dsShutDownOrRestart = 20000;
dsSwitchOffOrRestart = 20001;
dsForcedQuit = 20002;

{need to write new boot blocks}
{must have at least 1.5MB of RAM to boot}
{bufPtr moved too far during boot}
{request user to reinsert off-line volume}
{handled like a shutdown error}
{user choice between ShutDown and Restart}
{user choice between switching off and}
{allow the user to ExitToShell, return if}

{System Errors that are used after MacsBug is loaded to put up dialogs since these
should not cause MacsBug to stop, they must be in the range (30, 42, 16384-32767)
negative numbers add to an existing dialog without putting up a whole new dialog}
dsMacsBugInstalled = -10;
{say “MacsBug Installed”}
dsDisassemblerInstalled = -11;
{say “Disassembler Installed”}
dsExtensionsDisabled = -13;
{say “Extensions Disabled”}
dsGreeting = 40;
{welcome to Macintosh greeting}
dsSysErr = 32767;
{general system error}
{old names here for compatibility’s sake}
WDEFNFnd = dsWDEFNotFound;
CDEFNFnd = dsCDEFNotFound;
dsNotThe1 = 31;
dsBadStartupDisk = 42;
dsSystemFileErr = 43;
dsHD20Installed = -12;
mBarNFnd = -126;
hMenuFindErr = -127;
userBreak = -490;
strUserBreak = -491;
exUserBreak = -492;

{not the disk I wanted}
{unable to mount boot volume (sad Mac}
{can’t find System file to open (sad Mac}
{say “HD20 Startup”}
{system error code for MBDF not found}
{could not find HMenu's parent in MenuKey}
{user debugger break}
{user debugger break; display string on}
{user debugger break; execute debugger}

{obsolete errors that are no longer used, but I don’t have the guts to remove from
this file}
selectorErr = paramErr;
{bad selector, for selector-based traps}
PROCEDURE SysError(errorCode: INTEGER);
INLINE $301F,$A9C9;

{$ENDC}

{ UsingErrors }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Errors.p}



{#####################################################################}
{### FILE: Events.p}
{#####################################################################}

{
Created: Sunday, September 15, 1991 at 10:40 PM
Events.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Events;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingEvents}
{$SETC UsingEvents := 1}
{$I+}
{$SETC EventsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := EventsIncludes}
CONST
nullEvent = 0;
mouseDown = 1;
mouseUp = 2;
keyDown = 3;
keyUp = 4;
autoKey = 5;
updateEvt = 6;
diskEvt = 7;
activateEvt = 8;
osEvt = 15;
{ event mask equates }
mDownMask = 2;
mUpMask = 4;
keyDownMask = 8;
keyUpMask = 16;



autoKeyMask = 32;
updateMask = 64;
diskMask = 128;
activMask = 256;
highLevelEventMask = 1024;
osMask = -32768;
everyEvent = -1;
{ event message equates }
charCodeMask = $000000FF;
keyCodeMask = $0000FF00;
adbAddrMask = $00FF0000;
osEvtMessageMask = $FF000000;
{ OS event messages. Event (sub)code is in the high byte of the message field. }
mouseMovedMessage = $FA;
suspendResumeMessage = $01;
resumeFlag = 1;
convertClipboardFlag = 2;

{ Bit 0 of message indicates resume vs suspend }
{ Bit 1 in resume message indicates clipboard change }

{ modifiers }
activeFlag = 1;
btnState = 128;
cmdKey = 256;
shiftKey = 512;
alphaLock = 1024;
optionKey = 2048;
controlKey = 4096;

{
{
{
{
{
{

Bit
Bit
Bit
Bit
Bit
Bit

0
7
0
1
2
3

of modifiers for activateEvt and mouseDown events }
of low byte is mouse button state }
}
}
}
of high byte }

{ obsolete equates }
networkEvt = 10;
driverEvt = 11;
app1Evt = 12;
app2Evt = 13;
app3Evt = 14;
app4Evt = 15;
networkMask = 1024;
driverMask = 2048;
app1Mask = 4096;
app2Mask = 8192;
app3Mask = 16384;
app4Mask = -32768;
TYPE
EventRecord = RECORD
what: INTEGER;
message: LONGINT;
when: LONGINT;
where: Point;
modifiers: INTEGER;
END;

KeyMap = PACKED ARRAY [0..127] OF BOOLEAN;

FUNCTION GetNextEvent(eventMask: INTEGER;VAR theEvent: EventRecord): BOOLEAN;
INLINE $A970;
FUNCTION WaitNextEvent(eventMask: INTEGER;VAR theEvent: EventRecord;sleep: LONGINT;
mouseRgn: RgnHandle): BOOLEAN;
INLINE $A860;
FUNCTION EventAvail(eventMask: INTEGER;VAR theEvent: EventRecord): BOOLEAN;
INLINE $A971;
PROCEDURE GetMouse(VAR mouseLoc: Point);
INLINE $A972;
FUNCTION Button: BOOLEAN;
INLINE $A974;
FUNCTION StillDown: BOOLEAN;
INLINE $A973;
FUNCTION WaitMouseUp: BOOLEAN;
INLINE $A977;
PROCEDURE GetKeys(VAR theKeys: KeyMap);
INLINE $A976;
FUNCTION TickCount: LONGINT;
INLINE $A975;
FUNCTION GetDblTime: LONGINT;
INLINE $2EB8,$02F0;
FUNCTION GetCaretTime: LONGINT;
INLINE $2EB8,$02F4;

{$ENDC} { UsingEvents }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Events.p}



{#####################################################################}
{### FILE: Files.p}
{#####################################################################}

{
Created: Sunday, September 15, 1991 at 10:42 PM
Files.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Files;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$SETC UsingFiles := 1}
{$I+}
{$SETC FilesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$IFC UNDEFINED UsingSegLoad}
{$I $$Shell(PInterfaces)SegLoad.p}
{$ENDC}
{$SETC UsingIncludes := FilesIncludes}
CONST
{ Finder Constants }
fsAtMark = 0;
fOnDesk = 1;
fsCurPerm = 0;
fHasBundle = 8192;
fsRdPerm = 1;
fInvisible = 16384;
fTrash = -3;
fsWrPerm = 2;
fDesktop = -2;
fsRdWrPerm = 3;
fDisk = 0;

fsRdWrShPerm = 4;
fsFromStart = 1;
fsFromLEOF = 2;
fsFromMark = 3;
rdVerify = 64;
ioDirFlg = 3;
ioDirMask = $10;
fsRtParID = 1;
fsRtDirID = 2;
{ see IM IV-125 }

{ CatSearch SearchBits Constants }
fsSBPartialName = 1;
fsSBFullName = 2;
fsSBFlAttrib = 4;
fsSBFlFndrInfo = 8;
fsSBFlLgLen = 32;
fsSBFlPyLen = 64;
fsSBFlRLgLen = 128;
fsSBFlRPyLen = 256;
fsSBFlCrDat = 512;
fsSBFlMdDat = 1024;
fsSBFlBkDat = 2048;
fsSBFlXFndrInfo = 4096;
fsSBFlParID = 8192;
fsSBNegate = 16384;
fsSBDrUsrWds = 8;
fsSBDrNmFls = 16;
fsSBDrCrDat = 512;
fsSBDrMdDat = 1024;
fsSBDrBkDat = 2048;
fsSBDrFndrInfo = 4096;
fsSBDrParID = 8192;
{ vMAttrib (GetVolParms) bit position constants }
bLimitFCBs = 31;
bLocalWList = 30;
bNoMiniFndr = 29;
bNoVNEdit = 28;
bNoLclSync = 27;
bTrshOffLine = 26;
bNoSwitchTo = 25;
bNoDeskItems = 20;
bNoBootBlks = 19;
bAccessCntl = 18;
bNoSysDir = 17;
bHasExtFSVol = 16;
bHasOpenDeny = 15;
bHasCopyFile = 14;
bHasMoveRename = 13;
bHasDesktopMgr = 12;
bHasShortName = 11;
bHasFolderLock = 10;
bHasPersonalAccessPrivileges = 9;
bHasUserGroupList = 8;
bHasCatSearch = 7;
bHasFileIDs = 6;



bHasBTreeMgr = 5;
bHasBlankAccessPrivileges = 4;
{ Desktop Database icon Constants }
kLargeIcon = 1;
kLarge4BitIcon = 2;
kLarge8BitIcon = 3;
kSmallIcon = 4;
kSmall4BitIcon = 5;
kSmall8BitIcon = 6;
kLargeIconSize = 256;
kLarge4BitIconSize = 512;
kLarge8BitIconSize = 1024;
kSmallIconSize = 64;
kSmall4BitIconSize = 128;
kSmall8BitIconSize = 256;
{ Foreign Privilege Model Identifiers }
fsUnixPriv = 1;
{ Version Release Stage Codes }
developStage = $20;
alphaStage = $40;
betaStage = $60;
finalStage = $80;
{ Authentication Constants }
kNoUserAuthentication = 1;
kPassword = 2;
kEncryptPassword = 3;
kTwoWayEncryptPassword = 6;
TYPE
CInfoType = (hFileInfo,dirInfo);

FXInfo = RECORD
fdIconID: INTEGER;
fdUnused: ARRAY [1..3] OF INTEGER;
fdScript: SignedByte;
fdXFlags: SignedByte;
fdComment: INTEGER;
fdPutAway: LONGINT;
END;

{Icon ID}
{unused but reserved 6 bytes}
{Script flag and number}
{More flag bits}
{Comment ID}
{Home Dir ID}

DInfo = RECORD
frRect: Rect;
frFlags: INTEGER;
frLocation: Point;
frView: INTEGER;
END;

{folder rect}
{Flags}
{folder location}
{folder view}

DXInfo = RECORD
frScroll: Point;
frOpenChain: LONGINT;

{scroll position}
{DirID chain of open folders}

frScript: SignedByte;
frXFlags: SignedByte;
frComment: INTEGER;
frPutAway: LONGINT;
END;
GetVolParmsInfoBuffer = RECORD
vMVersion: INTEGER;
vMAttrib: LONGINT;
vMLocalHand: Handle;
vMServerAdr: LONGINT;
vMVolumeGrade: LONGINT;
vMForeignPrivID: INTEGER;
END;
CInfoPBPtr = ^CInfoPBRec;
CInfoPBRec = RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
ioNamePtr: StringPtr;
ioVRefNum: INTEGER;
ioFRefNum: INTEGER;
ioFVersNum: SignedByte;
filler1: SignedByte;
ioFDirIndex: INTEGER;
ioFlAttrib: SignedByte;
filler2: SignedByte;
CASE CInfoType OF
hFileInfo:
(ioFlFndrInfo: FInfo;
ioDirID: LONGINT;
ioFlStBlk: INTEGER;
ioFlLgLen: LONGINT;
ioFlPyLen: LONGINT;
ioFlRStBlk: INTEGER;
ioFlRLgLen: LONGINT;
ioFlRPyLen: LONGINT;
ioFlCrDat: LONGINT;
ioFlMdDat: LONGINT;
ioFlBkDat: LONGINT;
ioFlXFndrInfo: FXInfo;
ioFlParID: LONGINT;
ioFlClpSiz: LONGINT);
dirInfo:
(ioDrUsrWds: DInfo;
ioDrDirID: LONGINT;
ioDrNmFls: INTEGER;
filler3: ARRAY [1..9] OF INTEGER;
ioDrCrDat: LONGINT;
ioDrMdDat: LONGINT;
ioDrBkDat: LONGINT;
ioDrFndrInfo: DXInfo;


{Script flag and number}
{More flag bits}
{comment}
{DirID}

{version number}
{bit vector of attributes (see vMAttrib constants)}
{handle to private data}
{AppleTalk server address or zero}
{approx. speed rating or zero if unrated}
{foreign privilege model supported or zero if none}



ioDrParID: LONGINT);
END;
{ Catalog position record }
CatPositionRec = RECORD
initialize: LONGINT;
priv: ARRAY [1..6] OF INTEGER;
END;
FSSpecPtr = ^FSSpec;
FSSpecHandle = ^FSSpecPtr;
FSSpec = RECORD
vRefNum: INTEGER;
parID: LONGINT;
name: Str63;
END;
FSSpecArrayPtr = ^FSSpecArray;
FSSpecArrayHandle = ^FSSpecArrayPtr;
FSSpecArray = ARRAY [0..0] OF FSSpec;

{ The following are structures to be filled out with the _GetVolMountInfo call
and passed back into the _VolumeMount call for external file system mounts. }
VolumeType = OSType;

{ the "signature" of the file system }

CONST
AppleShareMediaType = 'afpm';

{ the signature for AppleShare }

TYPE
VolMountInfoPtr = ^VolMountInfoHeader;
VolMountInfoHeader = RECORD
length: INTEGER;
media: VolumeType;
END;
AFPVolMountInfoPtr = ^AFPVolMountInfo;
AFPVolMountInfo = RECORD
length: INTEGER;
media: VolumeType;
flags: INTEGER;
nbpInterval: SignedByte;
nbpCount: SignedByte;
uamType: INTEGER;
zoneNameOffset: INTEGER;
serverNameOffset: INTEGER;
volNameOffset: INTEGER;
userNameOffset: INTEGER;
userPasswordOffset: INTEGER;
volPasswordOffset: INTEGER;
AFPData: PACKED ARRAY [1..144] OF CHAR;
END;
DTPBPtr = ^DTPBRec;

{ length of location data (including self) }
{ type of media. Variable length data follows }

{
{
{
{
{
{
{
{
{
{
{
{
{

length of location data (including self) }
type of media }
bits for no messages, no reconnect }
NBP Interval parameter (IM2, p.322) }
NBP Interval parameter (IM2, p.322) }
User Authentication Method }
short positive offset from start of struct to Zone Name }
offset to pascal Server Name string }
offset to pascal Volume Name string }
offset to pascal User Name string }
offset to pascal User Password string }
offset to pascal Volume Password string }
variable length data may follow }



DTPBRec = RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
ioNamePtr: StringPtr;
ioVRefNum: INTEGER;
ioDTRefNum: INTEGER;
ioIndex: INTEGER;
ioTagInfo: LONGINT;
ioDTBuffer: Ptr;
ioDTReqCount: LONGINT;
ioDTActCount: LONGINT;
filler1: SignedByte;
ioIconType: SignedByte;
filler2: INTEGER;
ioDirID: LONGINT;
ioFileCreator: OSType;
ioFileType: OSType;
ioFiller3: LONGINT;
ioDTLgLen: LONGINT;
ioDTPyLen: LONGINT;
ioFiller4: ARRAY [1..14] OF INTEGER;
ioAPPLParID: LONGINT;
END;
HParmBlkPtr = ^HParamBlockRec;
HParamBlockRec = RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
ioNamePtr: StringPtr;
ioVRefNum: INTEGER;
CASE ParamBlkType OF
IOParam:
(ioRefNum: INTEGER;
ioVersNum: SignedByte;
ioPermssn: SignedByte;
ioMisc: Ptr;
ioBuffer: Ptr;
ioReqCount: LONGINT;
ioActCount: LONGINT;
ioPosMode: INTEGER;
ioPosOffset: LONGINT);
FileParam:
(ioFRefNum: INTEGER;
ioFVersNum: SignedByte;
filler1: SignedByte;
ioFDirIndex: INTEGER;
ioFlAttrib: SignedByte;
ioFlVersNum: SignedByte;

{size of buffer area}
{length of vol parms data}



ioFlFndrInfo: FInfo;
ioDirID: LONGINT;
ioFlStBlk: INTEGER;
ioFlLgLen: LONGINT;
ioFlPyLen: LONGINT;
ioFlRStBlk: INTEGER;
ioFlRLgLen: LONGINT;
ioFlRPyLen: LONGINT;
ioFlCrDat: LONGINT;
ioFlMdDat: LONGINT);
VolumeParam:
(filler2: LONGINT;
ioVolIndex: INTEGER;
ioVCrDate: LONGINT;
ioVLsMod: LONGINT;
ioVAtrb: INTEGER;
ioVNmFls: INTEGER;
ioVBitMap: INTEGER;
ioAllocPtr: INTEGER;
ioVNmAlBlks: INTEGER;
ioVAlBlkSiz: LONGINT;
ioVClpSiz: LONGINT;
ioAlBlSt: INTEGER;
ioVNxtCNID: LONGINT;
ioVFrBlk: INTEGER;
ioVSigWord: INTEGER;
ioVDrvInfo: INTEGER;
ioVDRefNum: INTEGER;
ioVFSID: INTEGER;
ioVBkUp: LONGINT;
ioVSeqNum: INTEGER;
ioVWrCnt: LONGINT;
ioVFilCnt: LONGINT;
ioVDirCnt: LONGINT;
ioVFndrInfo: ARRAY [1..8] OF LONGINT);
AccessParam:
(filler3: INTEGER;
ioDenyModes: INTEGER;
filler4: INTEGER;
filler5: SignedByte;
ioACUser: SignedByte;
filler6: LONGINT;
ioACOwnerID: LONGINT;
ioACGroupID: LONGINT;
ioACAccess: LONGINT);
ObjParam:
(filler7: INTEGER;
ioObjType: INTEGER;
ioObjNamePtr: Ptr;
ioObjID: LONGINT);
CopyParam:
(ioDstVRefNum: INTEGER;
filler8: INTEGER;
ioNewName: Ptr;
ioCopyName: Ptr;
ioNewDirID: LONGINT);

{access rights data}

{access rights for directory only}
{owner ID}
{group ID}
{access rights}

{function code}
{ptr to returned creator/group name}
{creator/group ID}
{destination vol identifier}
{ptr to destination pathname}
{ptr to optional name}
{destination directory ID}

WDParam:
(filler9: INTEGER;
ioWDIndex: INTEGER;
ioWDProcID: LONGINT;
ioWDVRefNum: INTEGER;
filler10: INTEGER;
filler11: LONGINT;
filler12: LONGINT;
filler13: LONGINT;
ioWDDirID: LONGINT);
FIDParam:
(filler14: LONGINT;
ioDestNamePtr: StringPtr;
filler15: LONGINT;
ioDestDirID: LONGINT;
filler16: LONGINT;
filler17: LONGINT;
ioSrcDirID: LONGINT;
filler18: INTEGER;
ioFileID: LONGINT);
CSParam:
(ioMatchPtr: FSSpecArrayPtr;
ioReqMatchCount: LONGINT;
ioActMatchCount: LONGINT;
ioSearchBits: LONGINT;
ioSearchInfo1: CInfoPBPtr;
ioSearchInfo2: CInfoPBPtr;
ioSearchTime: LONGINT;
ioCatPosition: CatPositionRec;
ioOptBuffer: Ptr;
ioOptBufSize: LONGINT);
ForeignPrivParam:
(filler21: LONGINT;
filler22: LONGINT;
ioForeignPrivBuffer: Ptr;
ioForeignPrivReqCount: LONGINT;
ioForeignPrivActCount: LONGINT;
filler23: LONGINT;
ioForeignPrivDirID: LONGINT;
ioForeignPrivInfo1: LONGINT;
ioForeignPrivInfo2: LONGINT;
ioForeignPrivInfo3: LONGINT;
ioForeignPrivInfo4: LONGINT);
END;
CMovePBPtr = ^CMovePBRec;
CMovePBRec = RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
ioNamePtr: StringPtr;
ioVRefNum: INTEGER;
filler1: LONGINT;
{match array}
{maximum allowable matches}
{actual match count}
{search criteria selector}
{search values and range lower bounds}
{search values and range upper bounds}
{length of time to run the search}
{current position in the catalog}
{optional performance enhancement buffer}
{length of buffer pointed to by ioOptBuffer}



ioNewName: StringPtr;
filler2: LONGINT;
ioNewDirID: LONGINT;
filler3: ARRAY [1..2] OF LONGINT;
ioDirID: LONGINT;
END;
WDPBPtr = ^WDPBRec;
WDPBRec = RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
ioNamePtr: StringPtr;
ioVRefNum: INTEGER;
filler1: INTEGER;
ioWDIndex: INTEGER;
ioWDProcID: LONGINT;
ioWDVRefNum: INTEGER;
filler2: ARRAY [1..7] OF INTEGER;
ioWDDirID: LONGINT;
END;
FCBPBPtr = ^FCBPBRec;
FCBPBRec = RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
ioNamePtr: StringPtr;
ioVRefNum: INTEGER;
ioRefNum: INTEGER;
filler: INTEGER;
ioFCBIndx: INTEGER;
filler1: INTEGER;
ioFCBFlNm: LONGINT;
ioFCBFlags: INTEGER;
ioFCBStBlk: INTEGER;
ioFCBEOF: LONGINT;
ioFCBPLen: LONGINT;
ioFCBCrPs: LONGINT;
ioFCBVRefNum: INTEGER;
ioFCBClpSiz: LONGINT;
ioFCBParID: LONGINT;
END;
{ Numeric version part of 'vers' resource }
NumVersion = PACKED RECORD
CASE INTEGER OF
0:
(majorRev: SignedByte;
{1st part of version number in BCD}
minorRev: 0..9;
{2nd part is 1 nibble in BCD}

bugFixRev: 0..9;
stage: SignedByte;
nonRelRev: SignedByte);
1:
(version: LONGINT);
END;
{ 'vers' resource format }
VersRecPtr = ^VersRec;
VersRecHndl = ^VersRecPtr;
VersRec = RECORD
numericVersion: NumVersion;
countryCode: INTEGER;
shortVersion: Str255;
reserved: Str255;
END;


{3rd part is 1 nibble in BCD}
{stage code: dev, alpha, beta, final}
{revision level of non-released version}
{to use all 4 fields at one time}

{encoded version number}
{country code from intl utilities}
{version number string - worst case}
{longMessage string packed after shortVersion}

FUNCTION PBOpen(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBOpenSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A000,$3E80;
FUNCTION PBOpenAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A400,$3E80;
FUNCTION PBClose(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBCloseSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A001,$3E80;
FUNCTION PBCloseAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A401,$3E80;
FUNCTION PBRead(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBReadSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A002,$3E80;
FUNCTION PBReadAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A402,$3E80;
FUNCTION PBWrite(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBWriteSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A003,$3E80;
FUNCTION PBWriteAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A403,$3E80;
FUNCTION PBGetVInfo(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBGetVInfoSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A007,$3E80;
FUNCTION PBGetVInfoAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A407,$3E80;
FUNCTION PBGetVol(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBGetVolSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A014,$3E80;
FUNCTION PBGetVolAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A414,$3E80;
FUNCTION PBSetVol(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBSetVolSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A015,$3E80;
FUNCTION PBSetVolAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A415,$3E80;
FUNCTION PBFlushVol(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBFlushVolSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A013,$3E80;



FUNCTION PBFlushVolAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A413,$3E80;
FUNCTION PBCreate(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBCreateSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A008,$3E80;
FUNCTION PBCreateAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A408,$3E80;
FUNCTION PBDelete(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBDeleteSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A009,$3E80;
FUNCTION PBDeleteAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A409,$3E80;
FUNCTION PBOpenDF(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBOpenDFSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$701A,$A060,$3E80;
FUNCTION PBOpenDFAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$701A,$A460,$3E80;
FUNCTION PBOpenRF(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBOpenRFSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A00A,$3E80;
FUNCTION PBOpenRFAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A40A,$3E80;
FUNCTION PBRename(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBRenameSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A00B,$3E80;
FUNCTION PBRenameAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A40B,$3E80;
FUNCTION PBGetFInfo(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBGetFInfoSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A00C,$3E80;
FUNCTION PBGetFInfoAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A40C,$3E80;
FUNCTION PBSetFInfo(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBSetFInfoSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A00D,$3E80;
FUNCTION PBSetFInfoAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A40D,$3E80;
FUNCTION PBSetFLock(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBSetFLockSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A041,$3E80;
FUNCTION PBSetFLockAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A441,$3E80;
FUNCTION PBRstFLock(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBRstFLockSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A042,$3E80;
FUNCTION PBRstFLockAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A442,$3E80;
FUNCTION PBSetFVers(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBSetFVersSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A043,$3E80;
FUNCTION PBSetFVersAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A443,$3E80;
FUNCTION PBAllocate(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBAllocateSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A010,$3E80;
FUNCTION PBAllocateAsync(paramBlock: ParmBlkPtr): OSErr;



INLINE $205F,$A410,$3E80;
FUNCTION PBGetEOF(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBGetEOFSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A011,$3E80;
FUNCTION PBGetEOFAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A411,$3E80;
FUNCTION PBSetEOF(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBSetEOFSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A012,$3E80;
FUNCTION PBSetEOFAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A412,$3E80;
FUNCTION PBGetFPos(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBGetFPosSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A018,$3E80;
FUNCTION PBGetFPosAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A418,$3E80;
FUNCTION PBSetFPos(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBSetFPosSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A044,$3E80;
FUNCTION PBSetFPosAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A444,$3E80;
FUNCTION PBFlushFile(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBFlushFileSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A045,$3E80;
FUNCTION PBFlushFileAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A445,$3E80;
FUNCTION PBMountVol(paramBlock: ParmBlkPtr): OSErr;
	inline $A00F;		{ added by @uxmal; needs verification }
FUNCTION PBUnmountVol(paramBlock: ParmBlkPtr): OSErr;
FUNCTION PBEject(paramBlock: ParmBlkPtr): OSErr;
FUNCTION PBOffLine(paramBlock: ParmBlkPtr): OSErr;
FUNCTION PBCatSearch(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBCatSearchSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7018,$A260,$3E80;
FUNCTION PBCatSearchAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7018,$A660,$3E80;
PROCEDURE AddDrive(drvrRefNum: INTEGER;drvNum: INTEGER;qEl: DrvQElPtr);
FUNCTION FSOpen(fileName: Str255;vRefNum: INTEGER;VAR refNum: INTEGER): OSErr;
FUNCTION OpenDF(fileName: Str255;vRefNum: INTEGER;VAR refNum: INTEGER): OSErr;
FUNCTION FSClose(refNum: INTEGER): OSErr;
FUNCTION FSRead(refNum: INTEGER;VAR count: LONGINT;buffPtr: Ptr): OSErr;
FUNCTION FSWrite(refNum: INTEGER;VAR count: LONGINT;buffPtr: Ptr): OSErr;
FUNCTION GetVInfo(drvNum: INTEGER;volName: StringPtr;VAR vRefNum: INTEGER; VAR freeBytes: LONGINT): OSErr;
FUNCTION GetFInfo(fileName: Str255;vRefNum: INTEGER;VAR fndrInfo: FInfo): OSErr;
FUNCTION GetVol(volName: StringPtr;VAR vRefNum: INTEGER): OSErr;
FUNCTION SetVol(volName: StringPtr;vRefNum: INTEGER): OSErr;
FUNCTION UnmountVol(volName: StringPtr;vRefNum: INTEGER): OSErr;
FUNCTION Eject(volName: StringPtr;vRefNum: INTEGER): OSErr;
FUNCTION FlushVol(volName: StringPtr;vRefNum: INTEGER): OSErr;
FUNCTION Create(fileName: Str255;vRefNum: INTEGER;creator: OSType;fileType: OSType): OSErr;
FUNCTION FSDelete(fileName: Str255;vRefNum: INTEGER): OSErr;
FUNCTION OpenRF(fileName: Str255;vRefNum: INTEGER;VAR refNum: INTEGER): OSErr;
FUNCTION Rename(oldName: Str255;vRefNum: INTEGER;newName: Str255): OSErr;
FUNCTION SetFInfo(fileName: Str255;vRefNum: INTEGER;fndrInfo: FInfo): OSErr;

FUNCTION SetFLock(fileName: Str255;vRefNum: INTEGER): OSErr;
FUNCTION RstFLock(fileName: Str255;vRefNum: INTEGER): OSErr;
FUNCTION Allocate(refNum: INTEGER;VAR count: LONGINT): OSErr;
FUNCTION GetEOF(refNum: INTEGER;VAR logEOF: LONGINT): OSErr;
FUNCTION SetEOF(refNum: INTEGER;logEOF: LONGINT): OSErr;
FUNCTION GetFPos(refNum: INTEGER;VAR filePos: LONGINT): OSErr;
FUNCTION SetFPos(refNum: INTEGER;posMode: INTEGER;posOff: LONGINT): OSErr;
FUNCTION GetVRefNum(fileRefNum: INTEGER;VAR vRefNum: INTEGER): OSErr;

FUNCTION PBOpenWD(paramBlock: WDPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBOpenWDSync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$7001,$A260,$3E80;
FUNCTION PBOpenWDAsync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$7001,$A660,$3E80;
FUNCTION PBCloseWD(paramBlock: WDPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBCloseWDSync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$7002,$A260,$3E80;
FUNCTION PBCloseWDAsync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$7002,$A660,$3E80;
FUNCTION PBHSetVol(paramBlock: WDPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBHSetVolSync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$A215,$3E80;
FUNCTION PBHSetVolAsync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$A615,$3E80;
FUNCTION PBHGetVol(paramBlock: WDPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBHGetVolSync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$A214,$3E80;
FUNCTION PBHGetVolAsync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$A614,$3E80;
FUNCTION PBCatMove(paramBlock: CMovePBPtr;async: BOOLEAN): OSErr;
FUNCTION PBCatMoveSync(paramBlock: CMovePBPtr): OSErr;
INLINE $205F,$7005,$A260,$3E80;
FUNCTION PBCatMoveAsync(paramBlock: CMovePBPtr): OSErr;
INLINE $205F,$7005,$A660,$3E80;
FUNCTION PBDirCreate(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBDirCreateSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7006,$A260,$3E80;
FUNCTION PBDirCreateAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7006,$A660,$3E80;
FUNCTION PBGetWDInfo(paramBlock: WDPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBGetWDInfoSync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$7007,$A260,$3E80;
FUNCTION PBGetWDInfoAsync(paramBlock: WDPBPtr): OSErr;
INLINE $205F,$7007,$A660,$3E80;
FUNCTION PBGetFCBInfo(paramBlock: FCBPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBGetFCBInfoSync(paramBlock: FCBPBPtr): OSErr;
INLINE $205F,$7008,$A260,$3E80;
FUNCTION PBGetFCBInfoAsync(paramBlock: FCBPBPtr): OSErr;
INLINE $205F,$7008,$A660,$3E80;
FUNCTION PBGetCatInfo(paramBlock: CInfoPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBGetCatInfoSync(paramBlock: CInfoPBPtr): OSErr;
INLINE $205F,$7009,$A260,$3E80;
FUNCTION PBGetCatInfoAsync(paramBlock: CInfoPBPtr): OSErr;
INLINE $205F,$7009,$A660,$3E80;
FUNCTION PBSetCatInfo(paramBlock: CInfoPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBSetCatInfoSync(paramBlock: CInfoPBPtr): OSErr;
INLINE $205F,$700A,$A260,$3E80;
FUNCTION PBSetCatInfoAsync(paramBlock: CInfoPBPtr): OSErr;
INLINE $205F,$700A,$A660,$3E80;
FUNCTION PBAllocContig(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBAllocContigSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A210,$3E80;
FUNCTION PBAllocContigAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A610,$3E80;
FUNCTION PBLockRange(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBLockRangeSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$7010,$A260,$3E80;
FUNCTION PBLockRangeAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$7010,$A660,$3E80;
FUNCTION PBUnlockRange(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBUnlockRangeSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$7011,$A260,$3E80;
FUNCTION PBUnlockRangeAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$7011,$A660,$3E80;
FUNCTION PBSetVInfo(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBSetVInfoSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$700B,$A260,$3E80;
FUNCTION PBSetVInfoAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$700B,$A660,$3E80;
FUNCTION PBHGetVInfo(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHGetVInfoSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A207,$3E80;
FUNCTION PBHGetVInfoAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A607,$3E80;
FUNCTION PBHOpen(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHOpenSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A200,$3E80;
FUNCTION PBHOpenAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A600,$3E80;
FUNCTION PBHOpenRF(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHOpenRFSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A20A,$3E80;
FUNCTION PBHOpenRFAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A60A,$3E80;
FUNCTION PBHOpenDF(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHOpenDFSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$701A,$A260,$3E80;
FUNCTION PBHOpenDFAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$701A,$A660,$3E80;
FUNCTION PBHCreate(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHCreateSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A208,$3E80;
FUNCTION PBHCreateAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A608,$3E80;
FUNCTION PBHDelete(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHDeleteSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A209,$3E80;
FUNCTION PBHDeleteAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A609,$3E80;
FUNCTION PBHRename(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;



FUNCTION PBHRenameSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A20B,$3E80;
FUNCTION PBHRenameAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A60B,$3E80;
FUNCTION PBHRstFLock(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHRstFLockSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A242,$3E80;
FUNCTION PBHRstFLockAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A642,$3E80;
FUNCTION PBHSetFLock(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHSetFLockSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A241,$3E80;
FUNCTION PBHSetFLockAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A641,$3E80;
FUNCTION PBHGetFInfo(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHGetFInfoSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A20C,$3E80;
FUNCTION PBHGetFInfoAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A60C,$3E80;
FUNCTION PBHSetFInfo(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHSetFInfoSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A20D,$3E80;
FUNCTION PBHSetFInfoAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$A60D,$3E80;

FUNCTION PBMakeFSSpec(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBMakeFSSpecSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$701B,$A260,$3E80;
FUNCTION PBMakeFSSpecAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$701B,$A660,$3E80;
PROCEDURE FInitQueue;
INLINE $A016;
FUNCTION GetFSQHdr: QHdrPtr;
INLINE $2EBC,$0000,$0360;
FUNCTION GetDrvQHdr: QHdrPtr;
INLINE $2EBC,$0000,$0308;
FUNCTION GetVCBQHdr: QHdrPtr;
INLINE $2EBC,$0000,$0356;
FUNCTION HGetVol(volName: StringPtr;VAR vRefNum: INTEGER;VAR dirID: LONGINT): OSErr;
FUNCTION HSetVol(volName: StringPtr;vRefNum: INTEGER;dirID: LONGINT): OSErr;
FUNCTION HOpen(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255;permission: SignedByte;
VAR refNum: INTEGER): OSErr;
FUNCTION HOpenDF(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255;permission: SignedByte;
VAR refNum: INTEGER): OSErr;
FUNCTION HOpenRF(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255;permission: SignedByte;
VAR refNum: INTEGER): OSErr;
FUNCTION AllocContig(refNum: INTEGER;VAR count: LONGINT): OSErr;
FUNCTION HCreate(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255;creator: OSType;
fileType: OSType): OSErr;
FUNCTION DirCreate(vRefNum: INTEGER;parentDirID: LONGINT;directoryName: Str255;
VAR createdDirID: LONGINT): OSErr;
FUNCTION HDelete(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255): OSErr;
FUNCTION HGetFInfo(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255;VAR fndrInfo: FInfo): OSErr;
FUNCTION HSetFInfo(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255;fndrInfo: FInfo): OSErr;
FUNCTION HSetFLock(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255): OSErr;



FUNCTION HRstFLock(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255): OSErr;
FUNCTION HRename(vRefNum: INTEGER;dirID: LONGINT;oldName: Str255;newName: Str255): OSErr;
FUNCTION CatMove(vRefNum: INTEGER;dirID: LONGINT;oldName: Str255;newDirID: LONGINT;
newName: Str255): OSErr;
FUNCTION OpenWD(vRefNum: INTEGER;dirID: LONGINT;procID: LONGINT;VAR wdRefNum: INTEGER): OSErr;
FUNCTION CloseWD(wdRefNum: INTEGER): OSErr;
FUNCTION GetWDInfo(wdRefNum: INTEGER;VAR vRefNum: INTEGER;VAR dirID: LONGINT;
VAR procID: LONGINT): OSErr;
{ shared environment }
FUNCTION PBHGetVolParms(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHGetVolParmsSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7030,$A260,$3E80;
FUNCTION PBHGetVolParmsAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7030,$A660,$3E80;
FUNCTION PBHGetLogInInfo(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHGetLogInInfoSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7031,$A260,$3E80;
FUNCTION PBHGetLogInInfoAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7031,$A660,$3E80;
FUNCTION PBHGetDirAccess(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHGetDirAccessSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7032,$A260,$3E80;
FUNCTION PBHGetDirAccessAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7032,$A660,$3E80;
FUNCTION PBHSetDirAccess(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHSetDirAccessSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7033,$A260,$3E80;
FUNCTION PBHSetDirAccessAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7033,$A660,$3E80;
FUNCTION PBHMapID(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHMapIDSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7034,$A260,$3E80;
FUNCTION PBHMapIDAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7034,$A660,$3E80;
FUNCTION PBHMapName(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHMapNameSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7035,$A260,$3E80;
FUNCTION PBHMapNameAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7035,$A660,$3E80;
FUNCTION PBHCopyFile(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHCopyFileSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7036,$A260,$3E80;
FUNCTION PBHCopyFileAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7036,$A660,$3E80;
FUNCTION PBHMoveRename(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHMoveRenameSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7037,$A260,$3E80;
FUNCTION PBHMoveRenameAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7037,$A660,$3E80;
FUNCTION PBHOpenDeny(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBHOpenDenySync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7038,$A260,$3E80;
FUNCTION PBHOpenDenyAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7038,$A660,$3E80;
FUNCTION PBHOpenRFDeny(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;



FUNCTION PBHOpenRFDenySync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7039,$A260,$3E80;
FUNCTION PBHOpenRFDenyAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7039,$A660,$3E80;
FUNCTION PBExchangeFiles(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBExchangeFilesSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7017,$A260,$3E80;
FUNCTION PBExchangeFilesAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7017,$A660,$3E80;
FUNCTION PBCreateFileIDRef(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBCreateFileIDRefSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7014,$A260,$3E80;
FUNCTION PBCreateFileIDRefAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7014,$A660,$3E80;
FUNCTION PBResolveFileIDRef(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBResolveFileIDRefSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7016,$A260,$3E80;
FUNCTION PBResolveFileIDRefAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7016,$A660,$3E80;
FUNCTION PBDeleteFileIDRef(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBDeleteFileIDRefSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7015,$A260,$3E80;
FUNCTION PBDeleteFileIDRefAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7015,$A660,$3E80;
FUNCTION PBGetForeignPrivs(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBGetForeignPrivsSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7060,$A260,$3E80;
FUNCTION PBGetForeignPrivsAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7060,$A660,$3E80;
FUNCTION PBSetForeignPrivs(paramBlock: HParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION PBSetForeignPrivsSync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7061,$A260,$3E80;
FUNCTION PBSetForeignPrivsAsync(paramBlock: HParmBlkPtr): OSErr;
INLINE $205F,$7061,$A660,$3E80;
{ Desktop Manager }
FUNCTION PBDTGetPath(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7020,$A260,$3E80;
FUNCTION PBDTCloseDown(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7021,$A260,$3E80;
FUNCTION PBDTAddIcon(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTAddIconSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7022,$A260,$3E80;
FUNCTION PBDTAddIconAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7022,$A660,$3E80;
FUNCTION PBDTGetIcon(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTGetIconSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7023,$A260,$3E80;
FUNCTION PBDTGetIconAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7023,$A660,$3E80;
FUNCTION PBDTGetIconInfo(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTGetIconInfoSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7024,$A260,$3E80;
FUNCTION PBDTGetIconInfoAsync(paramBlock: DTPBPtr): OSErr;



INLINE $205F,$7024,$A660,$3E80;
FUNCTION PBDTAddAPPL(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTAddAPPLSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7025,$A260,$3E80;
FUNCTION PBDTAddAPPLAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7025,$A660,$3E80;
FUNCTION PBDTRemoveAPPL(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTRemoveAPPLSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7026,$A260,$3E80;
FUNCTION PBDTRemoveAPPLAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7026,$A660,$3E80;
FUNCTION PBDTGetAPPL(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTGetAPPLSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7027,$A260,$3E80;
FUNCTION PBDTGetAPPLAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7027,$A660,$3E80;
FUNCTION PBDTSetComment(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTSetCommentSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7028,$A260,$3E80;
FUNCTION PBDTSetCommentAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7028,$A660,$3E80;
FUNCTION PBDTRemoveComment(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTRemoveCommentSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7029,$A260,$3E80;
FUNCTION PBDTRemoveCommentAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$7029,$A660,$3E80;
FUNCTION PBDTGetComment(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTGetCommentSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702A,$A260,$3E80;
FUNCTION PBDTGetCommentAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702A,$A660,$3E80;
FUNCTION PBDTFlush(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTFlushSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702B,$A260,$3E80;
FUNCTION PBDTFlushAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702B,$A660,$3E80;
FUNCTION PBDTReset(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTResetSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702C,$A260,$3E80;
FUNCTION PBDTResetAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702C,$A660,$3E80;
FUNCTION PBDTGetInfo(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTGetInfoSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702D,$A260,$3E80;
FUNCTION PBDTGetInfoAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702D,$A660,$3E80;
FUNCTION PBDTOpenInform(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702E,$A060,$3E80;
FUNCTION PBDTDelete(paramBlock: DTPBPtr;async: BOOLEAN): OSErr;
FUNCTION PBDTDeleteSync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702F,$A060,$3E80;
FUNCTION PBDTDeleteAsync(paramBlock: DTPBPtr): OSErr;
INLINE $205F,$702F,$A460,$3E80;
{ VolumeMount traps }
FUNCTION PBGetVolMountInfoSize(paramBlock: ParmBlkPtr): OSErr;

INLINE $205F,$703F,$A260,$3E80;
FUNCTION PBGetVolMountInfo(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$7040,$A260,$3E80;
FUNCTION PBVolumeMount(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$7041,$A260,$3E80;
{ FSp traps }
FUNCTION FSMakeFSSpec(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255;
VAR spec: FSSpec): OSErr;
INLINE $303C, $0001, $AA52;
FUNCTION FSpOpenDF(spec: FSSpec;permission: SignedByte;VAR refNum: INTEGER): OSErr;
INLINE $303C, $0002, $AA52;
FUNCTION FSpOpenRF(spec: FSSpec ;permission: SignedByte;VAR refNum: INTEGER): OSErr;
INLINE $303C, $0003, $AA52;
FUNCTION FSpCreate(spec: FSSpec ;creator: OSType;fileType: OSType;scriptTag: ScriptCode): OSErr;
INLINE $303C, $0004, $AA52;
FUNCTION FSpDirCreate(spec: FSSpec;scriptTag: ScriptCode;VAR createdDirID: LONGINT): OSErr;
INLINE $303C, $0005, $AA52;
FUNCTION FSpDelete(spec: FSSpec): OSErr;
INLINE $303C, $0006, $AA52;
FUNCTION FSpGetFInfo(spec: FSSpec;VAR fndrInfo: FInfo): OSErr;
INLINE $303C, $0007, $AA52;
FUNCTION FSpSetFInfo(spec: FSSpec;fndrInfo: FInfo): OSErr;
INLINE $303C, $0008, $AA52;
FUNCTION FSpSetFLock(spec: FSSpec): OSErr;
INLINE $303C, $0009, $AA52;
FUNCTION FSpRstFLock(spec: FSSpec): OSErr;
INLINE $303C, $000A, $AA52;
FUNCTION FSpRename(spec: FSSpec;newName: Str255): OSErr;
INLINE $303C, $000B, $AA52;
FUNCTION FSpCatMove(source: FSSpec;dest: FSSpec): OSErr;
INLINE $303C, $000C, $AA52;
FUNCTION FSpExchangeFiles(source: FSSpec;dest: FSSpec): OSErr;
INLINE $303C, $000F, $AA52;

{$ENDC} { UsingFiles }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Files.p}



{#####################################################################}
{### FILE: FileTransfers.p}
{#####################################################################}

{
Created: Wednesday, September 11, 1991 at 6:08 PM
FileTransfers.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT FileTransfers;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingFileTransfers}
{$SETC UsingFileTransfers := 1}
{$I+}
{$SETC FileTransfersIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingPackages}
{$I $$Shell(PInterfaces)Packages.p}
{$ENDC}
{$IFC UNDEFINED UsingCTBUtilities}
{$I $$Shell(PInterfaces)CTBUtilities.p}
{$ENDC}
{$IFC UNDEFINED UsingConnections}
{$I $$Shell(PInterfaces)Connections.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$SETC UsingIncludes := FileTransfersIncludes}
CONST
{ current file transfer manager version }
curFTVersion = 2;
{ FTErr
}
ftGenericError = -1;
ftNoErr = 0;
ftRejected = 1;
ftFailed = 2;
ftTimeOut = 3;

ftTooManyRetry = 4;
ftNotEnoughDSpace = 5;
ftRemoteCancel = 6;
ftWrongFormat = 7;
ftNoTools = 8;
ftUserCancel = 9;
ftNotSupported = 10;
TYPE
FTErr = OSErr;
CONST
{ FTFlags }
ftIsFTMode = $00000001;
ftNoMenus = $00000002;
ftQuiet = $00000004;
ftConfigChanged = $00000010;
ftSucc = $00000080;
TYPE
FTFlags = LONGINT;

CONST
{ FTAttributes }
ftSameCircuit = $0001;
ftSendDisable = $0002;
ftReceiveDisable = $0004;
ftTextOnly = $0008;
ftNoStdFile = $0010;
ftMultipleFileSend = $0020;
TYPE
FTAttributes = INTEGER;

CONST
{ FTDirection }
ftReceiving = 0;
ftTransmitting = 1;
TYPE
FTDirection = INTEGER;

FTPtr = ^FTRecord;
FTHandle = ^FTPtr;
FTRecord = PACKED RECORD
procID: INTEGER;
flags: FTFlags;
errCode: FTErr;
refCon: LONGINT;
userData: LONGINT;





defProc: ProcPtr;
config: Ptr;
oldConfig: Ptr;
environsProc: ProcPtr;
reserved1: LONGINT;
reserved2: LONGINT;
ftPrivate: Ptr;
sendProc: ProcPtr;
recvProc: ProcPtr;
writeProc: ProcPtr;
readProc: ProcPtr;
owner: WindowPtr;
direction: FTDirection;
theReply: SFReply;
writePtr: LONGINT;
readPtr: LONGINT;
theBuf: ^char;
bufSize: LONGINT;
autoRec: Str255;
attributes: FTAttributes;
END;

CONST
{ FTReadProc messages }
ftReadOpenFile = 0;
{ count = forkFlags, buffer = pblock from PBGetFInfo }
ftReadDataFork = 1;
ftReadRsrcFork = 2;
ftReadAbort = 3;
ftReadComplete = 4;
ftReadSetFPos = 6;
{ count = forkFlags, buffer = pBlock same as PBSetFPos }
ftReadGetFPos = 7;
{ count = forkFlags, buffer = pBlock same as PBGetFPos }
{ FTWriteProc messages }
ftWriteOpenFile = 0;
{ count = forkFlags, buffer = pblock from PBGetFInfo }
ftWriteDataFork = 1;
ftWriteRsrcFork = 2;
ftWriteAbort = 3;
ftWriteComplete = 4;
ftWriteFileInfo = 5;
ftWriteSetFPos = 6;
{ count = forkFlags, buffer = pBlock same as PBSetFPos }
ftWriteGetFPos = 7;
{ count = forkFlags, buffer = pBlock same as PBGetFPos }
{ fork flags }
ftOpenDataFork = 1;
ftOpenRsrcFork = 2;

TYPE
{ application routines type definitions }
FileTransferReadProcPtr = ProcPtr;
FileTransferWriteProcPtr = ProcPtr;
FileTransferSendProcPtr = ProcPtr;
FileTransferReceiveProcPtr = ProcPtr;



FileTransferEnvironsProcPtr = ProcPtr;
FileTransferNotificationProcPtr = ProcPtr;
FileTransferChooseIdleProcPtr = ProcPtr;
FUNCTION InitFT: FTErr;
FUNCTION FTGetVersion(hFT: FTHandle): Handle;
FUNCTION FTGetFTVersion: INTEGER;
FUNCTION FTNew(procID: INTEGER;flags: FTFlags;sendProc: FileTransferSendProcPtr;
recvProc: FileTransferReceiveProcPtr;readProc: FileTransferReadProcPtr;
writeProc: FileTransferWriteProcPtr;environsProc: FileTransferEnvironsProcPtr;
owner: WindowPtr;refCon: LONGINT;userData: LONGINT): FTHandle;
PROCEDURE FTDispose(hFT: FTHandle);
FUNCTION FTStart(hFT: FTHandle;direction: FTDirection;fileInfo: SFReply): FTErr;
FUNCTION FTAbort(hFT: FTHandle): FTErr;
FUNCTION FTSend(hFT: FTHandle;numFiles: INTEGER;pFSSpec: FSSpecArrayPtr;
notifyProc: FileTransferNotificationProcPtr): FTErr;
FUNCTION FTReceive(hFT: FTHandle;pFSSpec: FSSpecPtr;notifyProc: FileTransferNotificationProcPtr): FTErr;
PROCEDURE FTExec(hFT: FTHandle);
PROCEDURE FTActivate(hFT: FTHandle;activate: BOOLEAN);
PROCEDURE FTResume(hFT: FTHandle;resume: BOOLEAN);
FUNCTION FTMenu(hFT: FTHandle;menuID: INTEGER;item: INTEGER): BOOLEAN;
FUNCTION FTChoose(VAR hFT: FTHandle;where: Point;idleProc: FileTransferChooseIdleProcPtr): INTEGER;
PROCEDURE FTEvent(hFT: FTHandle;theEvent: EventRecord);
FUNCTION FTValidate(hFT: FTHandle): BOOLEAN;
PROCEDURE FTDefault(VAR theConfig: Ptr;procID: INTEGER;allocate: BOOLEAN);
FUNCTION FTSetupPreflight(procID: INTEGER;VAR magicCookie: LONGINT): Handle;
PROCEDURE FTSetupSetup(procID: INTEGER;theConfig: Ptr;count: INTEGER;theDialog: DialogPtr;
VAR magicCookie: LONGINT);
FUNCTION FTSetupFilter(procID: INTEGER;theConfig: Ptr;count: INTEGER;theDialog: DialogPtr;
VAR theEvent: EventRecord;VAR theItem: INTEGER;VAR magicCookie: LONGINT): BOOLEAN;
PROCEDURE FTSetupItem(procID: INTEGER;theConfig: Ptr;count: INTEGER;theDialog: DialogPtr;
VAR theItem: INTEGER;VAR magicCookie: LONGINT);
PROCEDURE FTSetupXCleanup(procID: INTEGER;theConfig: Ptr;count: INTEGER;
theDialog: DialogPtr;OKed: BOOLEAN;VAR magicCookie: LONGINT);
PROCEDURE FTSetupPostflight(procID: INTEGER);
FUNCTION FTGetConfig(hFT: FTHandle): Ptr;
FUNCTION FTSetConfig(hFT: FTHandle;thePtr: Ptr): INTEGER;
FUNCTION FTIntlToEnglish(hFT: FTHandle;inputPtr: Ptr;VAR outputPtr: Ptr;
language: INTEGER): OSErr;
FUNCTION FTEnglishToIntl(hFT: FTHandle;inputPtr: Ptr;VAR outputPtr: Ptr;
language: INTEGER): OSErr;




PROCEDURE FTGetToolName(procID: INTEGER;VAR name: Str255);
FUNCTION FTGetProcID(name: Str255): INTEGER;
PROCEDURE FTSetRefCon(hFT: FTHandle;refCon: LONGINT);
FUNCTION FTGetRefCon(hFT: FTHandle): LONGINT;
PROCEDURE FTSetUserData(hFT: FTHandle;userData: LONGINT);
FUNCTION FTGetUserData(hFT: FTHandle): LONGINT;
PROCEDURE FTGetErrorString(hFT: FTHandle;id: INTEGER;VAR errMsg: Str255);

{$ENDC} { UsingFileTransfers }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE FileTransfers.p}



{#####################################################################}
{### FILE: FileTransferTools.p}
{#####################################################################}

{
Created: Thursday, September 12, 1991 at 9:51 AM
FileTransferTools.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT FileTransferTools;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingFileTransferTools}
{$SETC UsingFileTransferTools := 1}
{$I+}
{$SETC FileTransferToolsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$IFC UNDEFINED UsingFileTransfers}
{$I $$Shell(PInterfaces)FileTransfers.p}
{$ENDC}
{$SETC UsingIncludes := FileTransferToolsIncludes}
CONST
{ Control }
ftInitMsg = 0;
ftDisposeMsg = 1;
ftSuspendMsg = 2;
ftResumeMsg = 3;
ftMenuMsg = 4;
ftEventMsg = 5;
ftActivateMsg = 6;
ftDeactivateMsg = 7;
ftGetErrorStringMsg = 8;
ftAbortMsg = 52;
ftStartMsg = 100;
ftExecMsg = 102;


ftSendMsg = 103;
ftReceiveMsg = 104;
{Setup }
ftSpreflightMsg = 0;
ftSsetupMsg = 1;
ftSitemMsg = 2;
ftSfilterMsg = 3;
ftScleanupMsg = 4;
{ validate }
ftValidateMsg = 0;
ftDefaultMsg = 1;
{ scripting }
ftMgetMsg = 0;
ftMsetMsg = 1;
{ localization }
ftL2English = 0;
ftL2Intl = 1;
{ DEFs }
fdefType  = 'fdef';
fsetType  =	'fset';
fvalType  =	'fval';
flocType  =	'floc';
fscrType  =	'fscr';
fbndType = 'fbnd';
fverType = 'vers';
TYPE
FTSetupPtr = ^FTSetupStruct;
FTSetupStruct = PACKED RECORD
theDialog: DialogPtr; { the dialog form the application }
count: INTEGER;
{ first appended item }
theConfig: Ptr;
{ the config record to setup }
procID: INTEGER;
{ procID of the tool }
END;

{$ENDC} { UsingFileTransferTools }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE FileTransferTools.p}



{#####################################################################}
{### FILE: Finder.p}
{#####################################################################}

{
Created: Tuesday, November 26, 1991 at 11:43 AM
Finder.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1990-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Finder;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingFinder}
{$SETC UsingFinder := 1}

CONST
{ make only the following consts avaiable to resource files that include this file }
kCustomIconResource = -16455;
{ Custom icon family resource ID }
kContainerFolderAliasType = 'fdrp';
kContainerTrashAliasType = 'trsh';
kContainerHardDiskAliasType = 'hdsk';
kContainerFloppyAliasType = 'flpy';
kContainerServerAliasType = 'srvr';
kApplicationAliasType = 'adrp';
kContainerAliasType = 'drop';

{
{
{
{
{
{
{

{ type for Special folder aliases }
kSystemFolderAliasType = 'fasy';
kAppleMenuFolderAliasType = 'faam';
kStartupFolderAliasType = 'fast';
kPrintMonitorDocsFolderAliasType = 'fapn';
kPreferencesFolderAliasType = 'fapf';
kControlPanelFolderAliasType = 'fact';
kExtensionFolderAliasType = 'faex';

type
type
type
type
type
type
type

{ type for AppleShare folder aliases }
kExportedFolderAliasType = 'faet';
kDropFolderAliasType = 'fadr';
kSharedFolderAliasType = 'fash';
kMountedFolderAliasType = 'famn';

for
for
for
for
for
for
for

folder aliases }
trash folder aliases }
hard disk aliases }
floppy aliases }
server aliases }
application aliases }
all other containers }

{Finder Flags}
kIsOnDesk = $1;
kColor = $E;
{kColorReserved = $10
kRequiresSwitchLaunch = $20}
kIsShared = $40;
{kHasNoINITs = $80}
kHasBeenInited = $100;
{kReserved = $200}
kHasCustomIcon = $400;
kIsStationary = $800;
kNameLocked = $1000;
kHasBundle = $2000;
kIsInvisible = $4000;
kIsAlias = $8000;

{$ENDC} { UsingFinder }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Finder.p}





{#####################################################################}
{### FILE: FixMath.p}
{#####################################################################}
{
Created: Thursday, March 15, 1990 at 8:43 AM
FixMath.p
Pascal Interface to Fixed Point Math
Copyright Apple Computer, Inc.
All rights reserved
CHANGE LOG:
23 Oct 90

JPO

1985-1990

Changed functions Frac2X, Fix2X, X2Fix, and X2Frac
to in-line trap calls for non-mc68881 mode

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT FixMath;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingFixMath}
{$SETC UsingFixMath := 1}
{$I+}
{$SETC FixMathIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := FixMathIncludes}

FUNCTION Fix2Frac(x:Fixed): Fract;
INLINE $A841;
FUNCTION Fix2Long(x:Fixed): LONGINT;
INLINE $A840;
FUNCTION FixATan2(x:LONGINT;y: LONGINT): Fixed;
INLINE $A818;
FUNCTION Long2Fix(x:LONGINT): Fixed;
INLINE $A83F;
FUNCTION Frac2Fix(x:Fract): Fixed;
INLINE $A842;

{$IFC OPTION(MC68881)}
FUNCTION Frac2X(x: Fract): Extended;
FUNCTION Fix2X(x: Fixed): Extended;
FUNCTION X2Fix(x: Extended): Fixed;

FUNCTION X2Frac(x: Extended): Fract;
{$ELSEC}
FUNCTION Frac2X(x: Fract): Extended;
INLINE $A845;
FUNCTION Fix2X(x: Fixed): Extended;
INLINE $A843;
FUNCTION X2Fix(x: Extended): Fixed;
INLINE $A844;
FUNCTION X2Frac(x: Extended): Fract;
INLINE $A846;
{$ENDC}
FUNCTION FracMul(x: Fract;y: Fract): Fract;
INLINE $A84A;
FUNCTION FixDiv(x: Fixed;y: Fixed): Fixed;
INLINE $A84D;
FUNCTION FracDiv(x: Fract;y: Fract): Fract;
INLINE $A84B;
FUNCTION FracSqrt(x: Fract): Fract;
INLINE $A849;
FUNCTION FracSin(x: Fixed): Fract;
INLINE $A848;
FUNCTION FracCos(x: Fixed): Fract;
INLINE $A847;
{$ENDC}

{ UsingFixMath }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE FixMath.p}



{#####################################################################}
{### FILE: Folders.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:43 PM
Folders.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1989-90

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Folders;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingFolders}
{$SETC UsingFolders := 1}
{$I+}
{$SETC FoldersIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$SETC UsingIncludes := FoldersIncludes}
CONST
kOnSystemDisk = $8000;
kCreateFolder = TRUE;
kDontCreateFolder = FALSE;
kSystemFolderType = 'macs';
kDesktopFolderType = 'desk';
kTrashFolderType = 'trsh';
kWhereToEmptyTrashFolderType = 'empt';

{the
{the
{the
{the

system folder}
desktop folder; objects in this folder show on the desk top.}
trash folder; objects in this folder show up in the trash}
"empty trash" folder; Finder starts empty from here down}

kPrintMonitorDocsFolderType = 'prnt';

{ Print Monitor documents }

kStartupFolderType = 'strt';
kAppleMenuFolderType = 'amnu';
kControlPanelFolderType = 'ctrl';
kExtensionFolderType = 'extn';

{Finder objects (applications, documents, DAs, aliases, to...) to open at startup go here}
{Finder objects to put into the Apple menu go here}
{Control Panels go here (may contain INITs)}
{Finder extensions go here}


kPreferencesFolderType = 'pref';
kTemporaryFolderType = 'temp';

{preferences for applications go here}
{temporary files go here (deleted periodically, but don't rely on it.)}

FUNCTION FindFolder(vRefNum: INTEGER;folderType: OSType;createFolder: BOOLEAN;
VAR foundVRefNum: INTEGER;VAR foundDirID: LONGINT): OSErr;
{$IFC SystemSevenOrLater }
INLINE $7000,$A823;
{$ENDC}

{$ENDC}

{ UsingFolders }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Folders.p}


{#####################################################################}
{### FILE: Fonts.p}
{#####################################################################}
{
Created: Monday, January 28, 1991 at 4:48 PM
Fonts.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Fonts;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingFonts}
{$SETC UsingFonts := 1}
{$I+}
{$SETC FontsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := FontsIncludes}
CONST
systemFont = 0;
applFont = 1;
newYork = 2;
geneva = 3;
monaco = 4;
venice = 5;
london = 6;
athens = 7;
sanFran = 8;
toronto = 9;
cairo = 11;
losAngeles = 12;
times = 20;
helvetica = 21;
courier = 22;
symbol = 23;
mobile = 24;

commandMark = 17;
checkMark = 18;
diamondMark = 19;
appleMark = 20;
propFont = 36864;
prpFntH = 36865;
prpFntW = 36866;
prpFntHW = 36867;
fixedFont = 45056;
fxdFntH = 45057;
fxdFntW = 45058;
fxdFntHW = 45059;
fontWid = 44208;
TYPE
FMInput = PACKED RECORD
family: INTEGER;
size: INTEGER;
face: Style;
needBits: BOOLEAN;
device: INTEGER;
numer: Point;
denom: Point;
END;
FMOutPtr = ^FMOutput;
FMOutput = PACKED RECORD
errNum: INTEGER;
fontHandle: Handle;
bold: Byte;
italic: Byte;
ulOffset: Byte;
ulShadow: Byte;
ulThick: Byte;
shadow: Byte;
extra: SignedByte;
ascent: Byte;
descent: Byte;
widMax: Byte;
leading: SignedByte;
unused: Byte;
numer: Point;
denom: Point;
END;
FontRec = RECORD
fontType: INTEGER;
firstChar: INTEGER;
lastChar: INTEGER;
widMax: INTEGER;
kernMax: INTEGER;
nDescent: INTEGER;
fRectWidth: INTEGER;
fRectHeight: INTEGER;
owTLoc: INTEGER;
ascent: INTEGER;

{font type}
{ASCII code of first character}
{ASCII code of last character}
{maximum character width}
{negative of maximum character kern}
{negative of descent}
{width of font rectangle}
{height of font rectangle}
{offset to offset/width table}
{ascent}


descent: INTEGER;
leading: INTEGER;
rowWords: INTEGER;
END;

{descent}
{leading}
{row width of bit image / 2 }

FMetricRec = RECORD
ascent: Fixed;
descent: Fixed;
leading: Fixed;
widMax: Fixed;
wTabHandle: Handle;
END;

{base line to top}
{base line to bottom}
{leading between lines}
{maximum character width}
{handle to font width table}

WidEntry = RECORD
widStyle: INTEGER;
END;

{style entry applies to}

WidTable = RECORD
numWidths: INTEGER;
END;

{number of entries - 1}

AsscEntry = RECORD
fontSize: INTEGER;
fontStyle: INTEGER;
fontID: INTEGER;
END;
FontAssoc = RECORD
numAssoc: INTEGER;
END;

{font resource ID}

{number of entries - 1}

StyleTable = RECORD
fontClass: INTEGER;
offset: LONGINT;
reserved: LONGINT;
indexes: ARRAY [0..47] OF SignedByte;
END;
NameTable = RECORD
stringCount: INTEGER;
baseFontName: Str255;
END;
KernPair = RECORD
kernFirst: CHAR;
kernSecond: CHAR;
kernWidth: INTEGER;
END;

{1st character of kerned pair}
{2nd character of kerned pair}
{kerning in 1pt fixed format}

KernEntry = RECORD
kernLength: INTEGER;
kernStyle: INTEGER;
END;

{length of this entry}
{style the entry applies to}

KernTable = RECORD
numKerns: INTEGER;

{number of kerning entries}



END;
WidthTable = PACKED RECORD
tabData: ARRAY [1..256] OF Fixed;
tabFont: Handle;
sExtra: LONGINT;
style: LONGINT;
fID: INTEGER;
fSize: INTEGER;
face: INTEGER;
device: INTEGER;
inNumer: Point;
inDenom: Point;
aFID: INTEGER;
fHand: Handle;
usedFam: BOOLEAN;
aFace: Byte;
vOutput: INTEGER;
hOutput: INTEGER;
vFactor: INTEGER;
hFactor: INTEGER;
aSize: INTEGER;
tabSize: INTEGER;
END;

{character widths}
{font record used to build table}
{space extra used for table}
{extra due to style}
{font family ID}
{font size request}
{style (face) request}
{device requested}
{scale factors requested}
{scale factors requested}
{actual font family ID for table}
{family record used to build up table}
{used fixed point family widths}
{actual face produced}
{vertical scale output value}
{horizontal scale output value}
{vertical scale output value}
{horizontal scale output value}
{actual size of actual font used}
{total size of table}

FamRec = RECORD
ffFlags: INTEGER;
ffFamID: INTEGER;
ffFirstChar: INTEGER;
ffLastChar: INTEGER;
ffAscent: INTEGER;
ffDescent: INTEGER;
ffLeading: INTEGER;
ffWidMax: INTEGER;
ffWTabOff: LONGINT;
ffKernOff: LONGINT;
ffStylOff: LONGINT;
ffProperty: ARRAY [1..9] OF INTEGER;
ffIntl: ARRAY [1..2] OF INTEGER;
ffVersion: INTEGER;
END;

{flags for family}
{family ID number}
{ASCII code of 1st character}
{ASCII code of last character}
{maximum ascent for 1pt font}
{maximum descent for 1pt font}
{maximum leading for 1pt font}
{maximum widMax for 1pt font}
{offset to width table}
{offset to kerning table}
{offset to style mapping table}
{style property info}
{for international use}
{version number}

PROCEDURE InitFonts;
INLINE $A8FE;
PROCEDURE GetFontName(familyID: INTEGER;VAR name: Str255);
INLINE $A8FF;
PROCEDURE GetFNum(name: Str255;VAR familyID: INTEGER);
INLINE $A900;
FUNCTION RealFont(fontNum: INTEGER;size: INTEGER): BOOLEAN;
INLINE $A902;
PROCEDURE SetFontLock(lockFlag: BOOLEAN);
INLINE $A903;
FUNCTION FMSwapFont(inRec: FMInput): FMOutPtr;
INLINE $A901;
PROCEDURE SetFScaleDisable(fscaleDisable: BOOLEAN);



INLINE $A834;
PROCEDURE FontMetrics(theMetrics: FMetricRec);
INLINE $A835;
PROCEDURE SetFractEnable(fractEnable: BOOLEAN);
FUNCTION IsOutline(numer: Point;denom: Point): BOOLEAN;
INLINE $7000,$A854;
PROCEDURE SetOutlinePreferred(outlinePreferred: BOOLEAN);
INLINE $7001,$A854;
FUNCTION GetOutlinePreferred: BOOLEAN;
INLINE $7009,$A854;
FUNCTION OutlineMetrics(byteCount: INTEGER;textPtr: UNIV Ptr;numer: Point;
denom: Point;VAR yMax: INTEGER;VAR yMin: INTEGER;awArray: FixedPtr;lsbArray: FixedPtr;
boundsArray: RectPtr): OSErr;
INLINE $7008,$A854;
PROCEDURE SetPreserveGlyph(preserveGlyph: BOOLEAN);
INLINE $700A,$A854;
FUNCTION GetPreserveGlyph: BOOLEAN;
INLINE $700B,$A854;
FUNCTION FlushFonts: OSErr;
INLINE $700C,$A854;

{$ENDC}

{ UsingFonts }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Fonts.p}



{#####################################################################}
{### FILE: GestaltEqu.p}
{#####################################################################}

{
Created: Wednesday, December 4, 1991 at 12:31 PM
GestaltEqu.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT GestaltEqu;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingGestaltEqu}
{$SETC UsingGestaltEqu := 1}
{$I+}
{$SETC GestaltEquIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := GestaltEquIncludes}
CONST
{***********************
*
Gestalt error codes
***********************}
gestaltUnknownErr = -5550;
gestaltUndefSelectorErr = -5551;
gestaltDupSelectorErr = -5552;
gestaltLocationErr = -5553;

{
{
{
{

value returned if Gestalt doesn't know the answer }
undefined selector was passed to Gestalt }
tried to add an entry that already existed }
gestalt function ptr wasn't in sysheap }

{*************************
*
Environment Selectors
*************************}
gestaltVersion = 'vers';
gestaltAddressingModeAttr = 'addr';
gestalt32BitAddressing = 0;
gestalt32BitSysZone = 1;
gestalt32BitCapable = 2;
gestaltAliasMgrAttr = 'alis';
gestaltAliasMgrPresent = 0;

{
{
{
{
{
{
{

gestalt version }
addressing mode attributes }
using 32-bit addressing mode }
32-bit compatible system zone }
Machine is 32-bit capable }
Alias Mgr Attributes }
True if the Alias Mgr is present }



gestaltAliasMgrSupportsRemoteAppletalk = 1;
gestaltAppleTalkVersion = 'atlk';
gestaltAUXVersion = 'a/ux';
gestaltConnMgrAttr = 'conn';
gestaltConnMgrPresent = 0;
gestaltConnMgrCMSearchFix = 1;
gestaltConnMgrErrorString = 2;
gestaltConnMgrMultiAsyncIO = 3;
gestaltCRMAttr = 'crm ';
gestaltCRMPresent = 0;
gestaltCRMPersistentFix = 1;
gestaltCRMToolRsrcCalls = 2;
gestaltCTBVersion = 'ctbv';
gestaltDBAccessMgrAttr = 'dbac';
gestaltDBAccessMgrPresent = 0;
gestaltDITLExtAttr = 'ditl';
gestaltDITLExtPresent = 0;
gestaltEasyAccessAttr = 'easy';
gestaltEasyAccessOff = 0;
gestaltEasyAccessOn = 1;
gestaltEasyAccessSticky = 2;
gestaltEasyAccessLocked = 3;
gestaltEditionMgrAttr = 'edtn';
gestaltEditionMgrPresent = 0;
gestaltAppleEventsAttr = 'evnt';
gestaltAppleEventsPresent = 0;
gestaltFindFolderAttr = 'fold';
gestaltFindFolderPresent = 0;
gestaltFontMgrAttr = 'font';
gestaltOutlineFonts = 0;
gestaltFPUType = 'fpu ';
gestaltNoFPU = 0;
gestalt68881 = 1;
gestalt68882 = 2;
gestalt68040FPU = 3;
gestaltFSAttr = 'fs ';
gestaltFullExtFSDispatching = 0;
gestaltHasFSSpecCalls = 1;
gestaltHasFileSystemManager = 2;
gestaltFXfrMgrAttr = 'fxfr';
gestaltFXfrMgrPresent = 0;
gestaltFXfrMgrMultiFile = 1;
gestaltFXfrMgrErrorString = 2;
gestaltHardwareAttr = 'hdwr';
gestaltHasVIA1 = 0;
gestaltHasVIA2 = 1;
gestaltHasASC = 3;
gestaltHasSCC = 4;
gestaltHasSCSI = 7;
gestaltHasSoftPowerOff = 19;
gestaltHasSCSI961 = 21;
gestaltHasSCSI962 = 22;
gestaltHasUniversalROM = 24;
gestaltHelpMgrAttr = 'help';
gestaltHelpMgrPresent = 0;
gestaltKeyboardType = 'kbd ';

{ True if the Alias Mgr knows about Remote Appletalk }
{ appletalk version }
{a/ux version, if present }
{ connection mgr attributes }
{
{
{
{

Fix to CMAddSearch? }
has CMGetErrorString() }
CMNewIOPB, CMDisposeIOPB, CMPBRead, CMPBWrite, CMPBIOKill }
comm resource mgr attributes }

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

fix for persistent tools }
has CRMGetToolResource/ReleaseToolResource }
CommToolbox version }
Database Access Mgr attributes }
True if Database Access Mgr present }
AppenDITL, etc. calls from CTB }
True if calls are present }
Easy Access attributes }
if Easy Access present, but off (no icon) }
if Easy Access "On" }
if Easy Access "Sticky" }
if Easy Access "Locked" }
Edition Mgr attributes }
True if Edition Mgr present }
Apple Events attributes }
True if Apple Events present }
Folder Mgr attributes }
True if Folder Mgr present }
Font Mgr attributes }
True if Outline Fonts supported }
fpu type }
no FPU }
68881 FPU }
68882 FPU }
68040 built-in FPU }
file system attributes }
has really cool new HFSDispatch dispatcher }
has FSSpec calls }
has a file system manager }
file transfer manager attributes }

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

supports FTSend and FTReceive }
supports FTGetErrorString }
hardware attributes }
VIA1 exists }
VIA2 exists }
Apple Sound Chip exists }
SCC exists }
SCSI exists }
Capable of software power off }
53C96 SCSI controller on internal bus }
53C96 SCSI controller on external bus }
Do we have a Universal ROM?}
Help Mgr Attributes }
true if help mgr is present }
keyboard type }

gestaltMacKbd = 1;
gestaltMacAndPad = 2;
gestaltMacPlusKbd = 3;
gestaltExtADBKbd = 4;
gestaltStdADBKbd = 5;
gestaltPrtblADBKbd = 6;
gestaltPrtblISOKbd = 7;
gestaltStdISOADBKbd = 8;
gestaltExtISOADBKbd = 9;
gestaltADBKbdII = 10;
gestaltADBISOKbdII = 11;
gestaltPwrBookADBKbd = 12;
gestaltPwrBookISOADBKbd = 13;
gestaltLowMemorySize = 'lmem';
gestaltLogicalRAMSize = 'lram';
gestaltMiscAttr = 'misc';
gestaltScrollingThrottle = 0;
gestaltSquareMenuBar = 2;
gestaltMMUType = 'mmu ';
gestaltNoMMU = 0;
gestaltAMU = 1;
gestalt68851 = 2;
gestalt68030MMU = 3;
gestalt68040MMU = 4;
gestaltStdNBPAttr = 'nlup';
gestaltStdNBPPresent = 0;
gestaltNotificationMgrAttr = 'nmgr';
gestaltNotificationPresent = 0;
gestaltNuBusConnectors = 'sltc';
gestaltOSAttr = 'os ';
gestaltSysZoneGrowable = 0;
gestaltLaunchCanReturn = 1;
gestaltLaunchFullFileSpec = 2;
gestaltLaunchControl = 3;
gestaltTempMemSupport = 4;
gestaltRealTempMemory = 5;
gestaltTempMemTracked = 6;
gestaltIPCSupport = 7;
gestaltSysDebuggerSupport = 8;
gestaltOSTable = 'ostt';
gestaltToolboxTable = 'tbtt';
gestaltExtToolboxTable = 'xttt';
gestaltLogicalPageSize = 'pgsz';
gestaltPowerMgrAttr = 'powr';
gestaltPMgrExists = 0;
gestaltPMgrCPUIdle = 1;
gestaltPMgrSCC = 2;
gestaltPMgrSound = 3;
gestaltPPCToolboxAttr = 'ppc ';
{
{
{
{
{
{
{
{
{
{
{
{

size of low memory area }
logical ram size }
miscellaneous attributes }
true if scrolling throttle on }
true if menu bar is square }
mmu type }
no MMU }
address management unit }
68851 PMMU }
68030 built-in MMU }
68040 built-in MMU }
standard nbp attributes }

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

notification manager attributes }
notification manager exists }
bitmap of NuBus connectors}
o/s attributes }
system heap is growable }
can return from launch }
can launch from full file spec }
launch control support available }
temp memory support }
temp memory handles are real }
temporary memory handles are tracked }
IPC support is present }
system debugger support is present }
OS trap table base }
OS trap table base }
Extended Toolbox trap table base }
logical page size }
power manager attributes }

{ PPC toolbox attributes }

{
*
*
*
*
*

PPC will return the combination of following bit fields.
e.g. gestaltPPCSupportsRealTime +gestaltPPCSupportsIncoming + gestaltPPCSupportsOutGoing
indicates PPC is cuurently is only supports real time delivery
and both incoming and outgoing network sessions are allowed.
By default local real time delivery is supported as long as PPCInit has been called.}

gestaltPPCToolboxPresent = $0000;
gestaltPPCSupportsRealTime = $1000;
gestaltPPCSupportsIncoming = $0001;
gestaltPPCSupportsOutGoing = $0002;
gestaltProcessorType = 'proc';
gestalt68000 = 1;
gestalt68010 = 2;
gestalt68020 = 3;
gestalt68030 = 4;
gestalt68040 = 5;
gestaltParityAttr = 'prty';
gestaltHasParityCapability = 0;
gestaltParityEnabled = 1;
gestaltQuickdrawVersion = 'qd ';
gestaltOriginalQD = $000;
gestalt8BitQD = $100;
gestalt32BitQD = $200;
gestalt32BitQD11 = $210;
gestalt32BitQD12 = $220;
gestalt32BitQD13 = $230;
gestaltQuickdrawFeatures = 'qdrw';
gestaltHasColor = 0;
gestaltHasDeepGWorlds = 1;
gestaltHasDirectPixMaps = 2;
gestaltHasGrayishTextOr = 3;
gestaltPhysicalRAMSize = 'ram ';
gestaltPopupAttr = 'pop!';
gestaltPopupPresent = 0;
gestaltResourceMgrAttr = 'rsrc';
gestaltPartialRsrcs = 0;
gestaltScriptMgrVersion = 'scri';
gestaltScriptCount = 'scr#';
gestaltSerialAttr = 'ser ';
gestaltHasGPIaToDCDa = 0;
gestaltHasGPIaToRTxCa = 1;
gestaltHasGPIbToDCDb = 2;
gestaltSoundAttr = 'snd ';
gestaltStereoCapability = 0;
gestaltStereoMixing = 1;
gestaltSoundIOMgrPresent = 3;
gestaltBuiltInSoundInput = 4;
gestaltHasSoundInputDevice = 5;
gestaltStandardFileAttr = 'stdf';
gestaltStandardFile58 = 0;
gestaltTextEditVersion = 'te ';
gestaltTE1 = 1;
gestaltTE2 = 2;
gestaltTE3 = 3;
gestaltTE4 = 4;
gestaltTE5 = 5;
gestaltTermMgrAttr = 'term';
gestaltTermMgrPresent = 0;
gestaltTermMgrErrorString = 2;
gestaltTimeMgrVersion = 'tmgr';
gestaltStandardTimeMgr = 1;


{
{
{
{
{

PPC Toolbox is present Requires PPCInit to be called }
PPC Supports real-time delivery }
PPC will deny incoming network requests }
PPC will deny outgoing network requests }
processor type }

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

parity attributes }
has ability to check parity }
parity checking enabled }
quickdraw version }
original 1-bit QD }
8-bit color QD }
32-bit color QD }
32-bit color QDv1.1 }
32-bit color QDv1.2 }
32-bit color QDv1.3 }
quickdraw features }
color quickdraw present }
GWorlds can be deeper than 1-bit }
PixMaps can be direct (16 or 32 bit) }
supports text mode grayishTextOr }
physical RAM size }
popup cdef attributes }

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

Resource Mgr attributes }
True if partial resources exist }
Script Manager version number
<08/05/89 pke> }
number of active script systems
<08/05/89 pke> }
Serial attributes }
GPIa connected to DCDa}
GPIa connected to RTxCa clock input}
GPIb connected to DCDb }
sound attributes }
sound hardware has stereo capability }
stereo mixing on external speaker }
The Sound I/O Manager is present }
built-in Sound Input hardware is present }
Sound Input device available }
Standard File attributes }
True if selectors 5-8 (StandardPutFile-CustomGetFile) are supported }
TextEdit version number
<08/05/89 pke> }
TextEdit in MacIIci ROM <8Aug89smb> }
TextEdit with 6.0.4 Script Systems on MacIIci (Script bug fixes for MacIIci) <8Aug89smb> }
TextEdit with 6.0.4 Script Systems all but MacIIci <8Aug89smb> }
TextEdit in System 7.0 }
TextWidthHook available in TextEdit }
terminal mgr attributes }

{ time mgr version }
{ standard time mgr is present }



gestaltRevisedTimeMgr = 2;
gestaltExtendedTimeMgr = 3;
gestaltVMAttr = 'vm ';
gestaltVMPresent = 0;
{************************
*
Info-only selectors
***********************}
gestaltMachineType = 'mach';
kMachineNameStrID = -16395;
gestaltClassic = 1;
gestaltMacXL = 2;
gestaltMac512KE = 3;
gestaltMacPlus = 4;
gestaltMacSE = 5;
gestaltMacII = 6;
gestaltMacIIx = 7;
gestaltMacIIcx = 8;
gestaltMacSE030 = 9;
gestaltPortable = 10;
gestaltMacIIci = 11;
gestaltMacIIfx = 13;
gestaltMacClassic = 17;
gestaltMacIIsi = 18;
gestaltMacLC = 19;
gestaltQuadra900 = 20;
gestaltPowerBook170 = 21;
gestaltQuadra700 = 22;
gestaltClassicII = 23;
gestaltPowerBook100 = 24;
gestaltPowerBook140 = 25;
gestaltMachineIcon = 'micn';
gestaltROMSize = 'rom ';
gestaltROMVersion = 'romv';
gestaltSystemVersion = 'sysv';


{
{
{
{

revised time mgr is present }
extended time mgr is present }
virtual memory attributes }
true if virtual memory is present }

{ machine type }

{
{
{
{

machine icon }
rom size }
rom version }
system version}

FUNCTION Gestalt(selector: OSType;VAR response: LONGINT): OSErr;
FUNCTION NewGestalt(selector: OSType;gestaltFunction: ProcPtr): OSErr;
FUNCTION ReplaceGestalt(selector: OSType;gestaltFunction: ProcPtr;VAR oldGestaltFunction: ProcPtr): OSErr;

{$ENDC} { UsingGestaltEqu }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE GestaltEqu.p}



{#####################################################################}
{### FILE: Graf3D.p}
{#####################################################################}
{
Created: Monday, January 7, 1991 at 6:00 AM
Graf3D.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Graf3D;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingGraf3D}
{$SETC UsingGraf3D := 1}
{$I+}
{$SETC Graf3DIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := Graf3DIncludes}
CONST
radConst = 3754936;
TYPE
Point3D = RECORD
x: Fixed;
y: Fixed;
z: Fixed;
END;
Point2D = RECORD
x: Fixed;
y: Fixed;
END;
XfMatrix = ARRAY [0..3, 0..3] OF Fixed;
Port3DPtr = ^Port3D;
Port3DHandle = ^Port3DPtr;
Port3D = RECORD
grPort: GrafPtr;



viewRect: Rect;
xLeft: Fixed;
yTop: Fixed;
xRight: Fixed;
yBottom: Fixed;
pen: Point3D;
penPrime: Point3D;
eye: Point3D;
hSize: Fixed;
vSize: Fixed;
hCenter: Fixed;
vCenter: Fixed;
xCotan: Fixed;
yCotan: Fixed;
ident: BOOLEAN;
xForm: XfMatrix;
END;

PROCEDURE InitGrf3d(port: Port3DHandle);
PROCEDURE Open3DPort(port: Port3DPtr);
PROCEDURE SetPort3D(port: Port3DPtr);
PROCEDURE GetPort3D(VAR port: Port3DPtr);
PROCEDURE MoveTo2D(x: Fixed;y: Fixed);
PROCEDURE MoveTo3D(x: Fixed;y: Fixed;z: Fixed);
PROCEDURE LineTo2D(x: Fixed;y: Fixed);
PROCEDURE Move2D(dx: Fixed;dy: Fixed);
PROCEDURE Move3D(dx: Fixed;dy: Fixed;dz: Fixed);
PROCEDURE Line2D(dx: Fixed;dy: Fixed);
PROCEDURE Line3D(dx: Fixed;dy: Fixed;dz: Fixed);
PROCEDURE ViewPort(r: Rect);
PROCEDURE LookAt(left: Fixed;top: Fixed;right: Fixed;bottom: Fixed);
PROCEDURE ViewAngle(angle: Fixed);
PROCEDURE Identity;
PROCEDURE Scale(xFactor: Fixed;yFactor: Fixed;zFactor: Fixed);
PROCEDURE Translate(dx: Fixed;dy: Fixed;dz: Fixed);
PROCEDURE Pitch(xAngle: Fixed);
PROCEDURE Yaw(yAngle: Fixed);
PROCEDURE Roll(zAngle: Fixed);
PROCEDURE Skew(zAngle: Fixed);
PROCEDURE Transform(src: Point3D;VAR dst: Point3D);
FUNCTION Clip3D(src1: Point3D;src2: Point3D;VAR dst1: Point;VAR dst2: Point): INTEGER;
PROCEDURE SetPt3D(VAR pt3D: Point3D;x: Fixed;y: Fixed;z: Fixed);
PROCEDURE SetPt2D(VAR pt2D: Point2D;x: Fixed;y: Fixed);
PROCEDURE LineTo3D(x: Fixed;y: Fixed;z: Fixed);

{$ENDC}

{ UsingGraf3D }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Graf3D.p}



{#####################################################################}
{### FILE: HyperXCmd.p}
{#####################################################################}
{
HyperXCmd.p
Definition file for HyperCard XCMDs and XFCNs in Pascal.
Copyright Apple Computer, Inc.
All rights reserved

1987-1991

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT HyperXCmd;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingHyperXCmd}
{$SETC UsingHyperXCmd := 1}
{$I+}
{$SETC HyperXCmdIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingEvents}
{$I $$Shell(PInterfaces)Events.p}
{$ENDC}
{$IFC UNDEFINED UsingTextEdit}
{$I $$Shell(PInterfaces)TextEdit.p}
{$ENDC}
{$IFC UNDEFINED UsingMenus}
{$I $$Shell(PInterfaces)Menus.p}
{$ENDC}
{$IFC UNDEFINED UsingStandardFile}
{$I $$Shell(PInterfaces)StandardFile.p}
{$ENDC}
{$SETC UsingIncludes := HyperXCmdIncludes}
CONST
{ result codes }
xresSucc = 0;
xresFail = 1;
xresNotImp = 2;

{ XCMDBlock constants for event.what... }

xOpenEvt = 1000;       { the first event after you are created }
xCloseEvt = 1001;	   { your window is being forced close (Quit?) }
xGiveUpEditEvt = 1002; { you are losing Edit... }                    
xGiveUpSoundEvt= 1003;     { someone else is requesting HyperCard's sound channel }        
xHidePalettesEvt = 1004;   { someone called HideHCPalettes }
xShowPalettesEvt = 1005;   { someone called ShowHCPalettes }
xEditUndo = 1100;		   { Edit——Undo }
xEditCut = 1102;		   { Edit——Cut }
xEditCopy = 1103;		   { Edit——Copy }
xEditPaste = 1104;		   { Edit——Paste }
xEditClear = 1105;		   { Edit——Clear }
xSendEvt = 1200;		   { script has sent you a message (text) }
xSetPropEvt = 1201;		   { set a window property }
xGetPropEvt = 1202;		   { get a window property }
xCursorWithin = 1300;	   { cursor is within the window }
xMenuEvt = 1400;		   { user has selected an item in your menu }
xMBarClickedEvt = 1401;	   { a menu is about to be shown--update if needed }              
xShowWatchInfoEvt	=  1501;
xScriptErrorEvt		=  1502;
xDebugErrorEvt		=  1503;
xDebugStepEvt		=  1504;
xDebugTraceEvt		=  1505;
xDebugFinishedEvt	=  1506;

paletteProc		   = 2048;
palNoGrowProc	   = 2052;
palZoomProc		   = 2056;
palZoomNoGrow	   = 2060;
hasZoom			   = 8;
hasTallTBar = 2;
toggleHilite = 1;

maxCachedChecks = 16;  { maximum number of checkpoints in a script }

{ paramCount is set to these constants when first calling special XThings }

xMessageWatcherID = -2;
xVariableWatcherID = -3;
xScriptEditorID = -4;
xDebuggerID = -5;

{ XTalkObjectPtr^.objectKind values }
stackObj = 1;
bkgndObj = 2;
cardObj = 3;
fieldObj = 4;
buttonObj = 5;
{ selectors for ShowHCAlert's dialogs (shown as buttonID:buttonText) }
errorDlgID
= 1;
{ 1:OK (default) }
confirmDlgID
= 2;
{ 1:OK (default) and 2:Cancel }
confirmDelDlgID
= 3;
{ 1:Cancel (default) and 2:Delete }
yesNoCancelDlgID = 4;
{ 1:Yes (default), 2:Cancel, and 3:No }

TYPE
XCmdPtr
= ^XCmdBlock;
XCmdBlock = RECORD
paramCount: INTEGER; { If = -1 then new use for XWindoids }


params: ARRAY[1..16] OF Handle;
returnValue: Handle;
passFlag: BOOLEAN;
entryPoint: ProcPtr; { to call back to HyperCard }
request:	INTEGER;
result:		INTEGER;
inArgs:		ARRAY[1..8] OF LongInt;
outArgs:	ARRAY[1..4] OF LongInt;
END;

XWEventInfoPtr = ^XWEventInfo;
XWEventInfo
= RECORD
event:
EventRecord;
eventWindow: WindowPtr;
eventParams: ARRAY[1..9] OF LongInt;
eventResult: Handle;
END;
XTalkObjectPtr = ^XTalkObject;
XTalkObject = RECORD
objectKind:
INTEGER;
{ stack, bkgnd, card, field, or button }
stackNum: LongInt;
{ reference number of the source stack }
bkgndID: LongInt;
cardID:
LongInt;
buttonID: LongInt;
fieldID: LongInt;
END;
CheckPtHandle = ^CheckPtPtr;
CheckPtPtr
= ^CheckPts;
CheckPts = RECORD
checks: ARRAY[1..maxCachedChecks] OF INTEGER;
END;

(**** HyperTalk Utilities ****)
FUNCTION EvalExpr(paramPtr: XCmdPtr; expr: Str255): Handle;
PROCEDURE SendCardMessage(paramPtr: XCmdPtr; msg: Str255);
PROCEDURE SendHCMessage(paramPtr: XCmdPtr; msg: Str255);
PROCEDURE RunHandler(paramPtr: XCmdPtr; handler: Handle);
(**** Memory Utilities ****)
FUNCTION GetGlobal(paramPtr: XCmdPtr; globName: Str255): Handle;
PROCEDURE SetGlobal(paramPtr: XCmdPtr; globName: Str255; globValue: Handle);
PROCEDURE ZeroBytes(paramPtr: XCmdPtr; dstPtr: Ptr;longCount: LongInt);
(**** String Utilities ****)
PROCEDURE ScanToReturn(paramPtr: XCmdPtr; VAR scanPtr: Ptr);
PROCEDURE ScanToZero(paramPtr: XCmdPtr; VAR scanPtr: Ptr);
FUNCTION StringEqual(paramPtr: XCmdPtr; str1,str2: Str255): BOOLEAN;
FUNCTION StringLength(paramPtr: XCmdPtr; strPtr: Ptr): LongInt;
FUNCTION StringMatch(paramPtr: XCmdPtr; pattern: Str255; target: Ptr): Ptr;
PROCEDURE ZeroTermHandle(paramPtr: XCmdPtr; hndl: Handle);
(****

String Conversions

****)

PROCEDURE BoolToStr(paramPtr: XCmdPtr; bool: BOOLEAN; VAR str: Str255);
PROCEDURE ExtToStr(paramPtr: XCmdPtr; num: Extended80; VAR str: Str255);
PROCEDURE LongToStr(paramPtr: XCmdPtr; posNum: LongInt; VAR str: Str255);
PROCEDURE NumToHex(paramPtr: XCmdPtr; num: LongInt; nDigits: INTEGER; VAR str: Str255);
PROCEDURE NumToStr(paramPtr: XCmdPtr; num: LongInt; VAR str: Str255);
FUNCTION PasToZero(paramPtr: XCmdPtr; str: Str255): Handle;
PROCEDURE PointToStr(paramPtr: XCmdPtr; pt: Point; VAR str: Str255);
PROCEDURE RectToStr(paramPtr: XCmdPtr; rct: Rect; VAR str: Str255);
PROCEDURE ReturnToPas(paramPtr: XCmdPtr; zeroStr: Ptr; VAR pasStr: Str255);
FUNCTION StrToBool(paramPtr: XCmdPtr; str: Str255): BOOLEAN;
FUNCTION StrToExt(paramPtr: XCmdPtr; str: Str255): Extended80;
FUNCTION StrToLong(paramPtr: XCmdPtr; str: Str255): LongInt;
FUNCTION StrToNum(paramPtr: XCmdPtr; str: Str255): LongInt;
PROCEDURE StrToPoint(paramPtr: XCmdPtr; str: Str255; VAR pt: Point);
PROCEDURE StrToRect(paramPtr: XCmdPtr; str: Str255; VAR rct: Rect);
PROCEDURE  ZeroToPas(paramPtr: XCmdPtr; zeroStr: Ptr; VAR pasStr: Str255);

(**** Field Utilities ****)
FUNCTION GetFieldByID(paramPtr: XCmdPtr; cardFieldFlag: BOOLEAN; fieldID: INTEGER): Handle;
FUNCTION GetFieldByName(paramPtr: XCmdPtr; cardFieldFlag: BOOLEAN; fieldName: Str255): Handle;
FUNCTION GetFieldByNum(paramPtr: XCmdPtr; cardFieldFlag: BOOLEAN; fieldNum: INTEGER): Handle;
PROCEDURE SetFieldByID(paramPtr: XCmdPtr; cardFieldFlag: BOOLEAN; fieldID: INTEGER; fieldVal: Handle);
PROCEDURE SetFieldByName(paramPtr: XCmdPtr; cardFieldFlag: BOOLEAN; fieldName: Str255; fieldVal: Handle);
PROCEDURE SetFieldByNum(paramPtr: XCmdPtr; cardFieldFlag: BOOLEAN; fieldNum: INTEGER; fieldVal: Handle);
FUNCTION GetFieldTE(paramPtr: XCmdPtr; cardFieldFlag: BOOLEAN; fieldID,fieldNum: INTEGER;
fieldNamePtr: StringPtr): TEHandle;
PROCEDURE SetFieldTE(paramPtr: XCmdPtr; cardFieldFlag: BOOLEAN; fieldID,fieldNum: INTEGER;
fieldNamePtr: StringPtr; fieldTE: TEHandle);
(**** Miscellaneous Utilities ****)
PROCEDURE BeginXSound(paramPtr: XCmdPtr; window: WindowPtr);
PROCEDURE EndXSound(paramPtr: XCmdPtr);
FUNCTION GetFilePath(paramPtr: XCmdPtr; fileName: Str255; numTypes: INTEGER; typeList: SFTypeList;
askUser: BOOLEAN; VAR fileType: OSType; VAR fullName: Str255): BOOLEAN;
PROCEDURE GetXResInfo(paramPtr: XCmdPtr; VAR resFile: INTEGER; VAR resID: INTEGER;
VAR rType: ResType; VAR name: Str255);
PROCEDURE Notify(paramPtr: XCmdPtr);
PROCEDURE SendHCEvent(paramPtr: XCmdPtr; event: EventRecord);
PROCEDURE SendWindowMessage(paramPtr: XCmdPtr; windPtr: WindowPtr;
windowName: Str255; msg: Str255);
FUNCTION FrontDocWindow(paramPtr: XCmdPtr): WindowPtr;
FUNCTION StackNameToNum(paramPtr: XCmdPtr; stackName: Str255): LongInt;
FUNCTION ShowHCAlert(paramPtr: XCMDPtr; dlgID: INTEGER; promptStr: Str255): INTEGER;
(**** Creating and Disposing XWindoids ****)
FUNCTION NewXWindow(paramPtr: XCmdPtr; boundsRect: Rect; title: Str255; visible: BOOLEAN;
procID: INTEGER; color: BOOLEAN; floating: BOOLEAN): WindowPtr;
FUNCTION GetNewXWindow(paramPtr: XCmdPtr; templateType: ResType; templateID: INTEGER;
color: BOOLEAN; floating: BOOLEAN): WindowPtr;
PROCEDURE CloseXWindow(paramPtr: XCmdPtr; window: WindowPtr);
(**** XWindoid Utilities ****)
PROCEDURE HideHCPalettes(paramPtr: XCmdPtr);
PROCEDURE ShowHCPalettes(paramPtr: XCmdPtr);
PROCEDURE RegisterXWMenu(paramPtr: XCmdPtr; window: WindowPtr; menu: MenuHandle; registering: BOOLEAN);
PROCEDURE SetXWIdleTime(paramPtr: XCmdPtr; window: WindowPtr; interval: LongInt);



PROCEDURE XWHasInterruptCode(paramPtr: XCmdPtr; window: WindowPtr; haveCode: BOOLEAN);
PROCEDURE XWAlwaysMoveHigh(paramPtr: XCmdPtr; window: WindowPtr; moveHigh: BOOLEAN);
PROCEDURE XWAllowReEntrancy(paramPtr: XCmdPtr; window: WindowPtr; allowSysEvts: BOOLEAN; allowHCEvts: BOOLEAN);
(**** Text Editing Utilities ****)
PROCEDURE BeginXWEdit(paramPtr: XCmdPtr; window: WindowPtr);
PROCEDURE EndXWEdit(paramPtr: XCmdPtr; window: WindowPtr);
FUNCTION HCWordBreakProc(paramPtr: XCmdPtr): ProcPtr;
PROCEDURE PrintTEHandle(paramPtr: XCmdPtr; hTE: TEHandle; header: StringPtr);
(**** Script Editor support ****)
FUNCTION GetCheckPoints(paramPtr: XCmdPtr): CheckPtHandle;
PROCEDURE SetCheckPoints(paramPtr: XCmdPtr; checkLines: CheckPtHandle);
PROCEDURE FormatScript(paramPtr: XCmdPtr; scriptHndl: Handle;
VAR insertionPoint: LongInt; quickFormat: BOOLEAN);
PROCEDURE SaveXWScript(paramPtr: XCmdPtr; scriptHndl: Handle);
PROCEDURE GetObjectName(paramPtr: XCmdPtr; object: XTalkObjectPtr; VAR objName: Str255);
PROCEDURE GetObjectScript(paramPtr: XCmdPtr; object: XTalkObjectPtr; VAR scriptHndl: Handle);
PROCEDURE SetObjectScript(paramPtr: XCmdPtr; object: XTalkObjectPtr; scriptHndl: Handle);
(**** Debugging Tools support ****)
PROCEDURE AbortScript(paramPtr: XCmdPtr);
PROCEDURE GoScript(paramPtr: XCmdPtr);
PROCEDURE StepScript(paramPtr: XCmdPtr; stepInto: BOOLEAN);
PROCEDURE CountHandlers(paramPtr: XCmdPtr; VAR handlerCount: INTEGER);
PROCEDURE GetHandlerInfo(paramPtr: XCmdPtr; handlerNum: INTEGER; VAR handlerName: Str255;
VAR objectName: Str255; VAR varCount: INTEGER);
PROCEDURE GetVarInfo(paramPtr: XCmdPtr; handlerNum: INTEGER; varNum: INTEGER;
VAR varName: Str255; VAR isGlobal: BOOLEAN; VAR varValue: Str255;
varHndl: Handle);
PROCEDURE SetVarValue(paramPtr: XCmdPtr; handlerNum: INTEGER; varNum: INTEGER;
varHndl: Handle);
FUNCTION GetStackCrawl(paramPtr: XCmdPtr): Handle;
PROCEDURE TraceScript(paramPtr: XCmdPtr; traceInto: BOOLEAN);

{$ENDC}

{ UsingHyperXCmd }

{$IFC NOT UsingIncludes}
END.
{$ENDC}
{### END OF FILE HyperXCmd.p}



{#####################################################################}
{### FILE: Icons.p}
{#####################################################################}

{
Created: Tuesday, September 10, 1991 at 2:03 PM
Icons.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1990-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Icons;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingIcons}
{$SETC UsingIcons := 1}

CONST
{ The following are icons for which there are both icon suites and SICNs. }
genericDocumentIconResource = -4000;
genericStationeryIconResource = -3985;
genericEditionFileIconResource = -3989;
genericApplicationIconResource = -3996;
genericDeskAccessoryIconResource = -3991;
genericFolderIconResource = -3999;
privateFolderIconResource = -3994;
floppyIconResource = -3998;
trashIconResource = -3993;
{ The following are icons for which there are SICNs only. }
desktopIconResource = -3992;
openFolderIconResource = -3997;
genericHardDiskIconResource = -3995;
genericFileServerIconResource = -3972;
genericSuitcaseIconResource = -3970;
genericMoverObjectIconResource = -3969;
{ The following are icons for which there are icon suites only. }
genericPreferencesIconResource = -3971;
genericQueryDocumentIconResource = -16506;
genericExtensionIconResource = -16415;



systemFolderIconResource = -3983;
appleMenuFolderIconResource = -3982;
startupFolderIconResource = -3981;
ownedFolderIconResource = -3980;
dropFolderIconResource = -3979;
sharedFolderIconResource = -3978;
mountedFolderIconResource = -3977;
controlPanelFolderIconResource = -3976;
printMonitorFolderIconResource = -3975;
preferencesFolderIconResource = -3974;
extensionsFolderIconResource = -3973;
fullTrashIconResource = -3984;
large1BitMask = 'ICN#';
large4BitData = 'icl4';
large8BitData = 'icl8';
small1BitMask = 'ics#';
small4BitData = 'ics4';
small8BitData = 'ics8';
mini1BitMask = 'icm#';
mini4BitData = 'icm4';
mini8BitData = 'icm8';

{$ENDC} { UsingIcons }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Icons.p}



{#####################################################################}
{### FILE: IntEnv.p}
{#####################################################################}
{
Created: Wednesday, June 27, 1990 at 6:42 PM
IntEnv.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1989-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT IntEnv;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingIntEnv}
{$SETC UsingIntEnv := 1}
{$I+}
{$SETC IntEnvIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingPasLibIntf}
{$I $$Shell(PInterfaces)PasLibIntf.p}
{$ENDC}
{$SETC UsingIncludes := IntEnvIncludes}
CONST
{ CMD words for IEfaccess(), from <fcntl.h> }
F_OPEN = $6400; { (('d'<<8)|00), d => "directory" ops }
F_DELETE = $6401;
F_RENAME = $6402;
F_GTABINFO = $6500; { (('e'<<8)|00), e => "editor" ops }
F_STABINFO = $6501;
F_GFONTINFO = $6502;
F_SFONTINFO = $6503;
F_GPRINTREC = $6504;
F_SPRINTREC = $6505;
F_GSELINFO = $6506;
F_SSELINFO = $6507;
F_GWININFO = $6508;
F_SWININFO = $6509;
F_GSCROLLINFO = $6510;
F_SSCROLLINFO = $6511;


{ Open modes for IEopen(), from <fcntl.h> }
O_RDONLY = $0000;
O_WRONLY = $0001;
O_RDWR = $0002;
O_APPEND = $0008;
O_RSRC = $0010;
O_CREAT = $0100;
O_TRUNC = $0200;
O_EXCL = $0400;
{ IOCtl parameters }
FIOINTERACTIVE = $6602; { (('f'<<8)|02), f => "open file" ops }
FIOBUFSIZE = $6603;
FIOFNAME = $6604;
FIOREFNUM = $6605;
FIOSETEOF = $6606;

TYPE
IEString = STRING;
IEStringPtr = ^IEString;
IEStringVec = ARRAY [0..8191] OF IEStringPtr;
IEStringVecPtr = ^IEStringVec;
{$PUSH}
{$J+} { EXPORTed unit globals }
VAR
ArgC: LONGINT;
ArgV: IEStringVecPtr;
_EnvP: IEStringVecPtr;
Diagnostic: TEXT;
{$POP}
FUNCTION IEStandAlone: BOOLEAN;
FUNCTION IEgetenv(envName: STRING; VAR envValue: UNIV IEString): BOOLEAN;
FUNCTION IEfaccess(fName: STRING; opCode: LONGINT;
arg: UNIV LONGINT): LONGINT;
PROCEDURE IEopen(VAR fvar: UNIV PASCALFILE; fName: STRING; mode: LONGINT);
FUNCTION IEioctl(VAR fvar: UNIV PASCALFILE; request: LONGINT;
arg: UNIV LONGINT): LONGINT;
FUNCTION IElseek(VAR fvar: UNIV PASCALFILE; offset: LONGINT;
whence: LONGINT): LONGINT;
PROCEDURE IEatexit(exitProc: UNIV LONGINT);
{C;}
PROCEDURE IEexit(status: LONGINT);
{C;}



PROCEDURE IE_exit(status: LONGINT);
{c;}
{$ENDC}

{ UsingIntEnv }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE IntEnv.p}



{#####################################################################}
{### FILE: Language.p}
{#####################################################################}

{
Created: Sunday, September 15, 1991 at 11:20 PM
Language.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1986-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Language;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingLanguage}
{$SETC UsingLanguage := 1}

CONST
{ Language Codes }
langEnglish = 0;
langFrench = 1;
langGerman = 2;
langItalian = 3;
langDutch = 4;
langSwedish = 5;
langSpanish = 6;
langDanish = 7;
langPortuguese = 8;
langNorwegian = 9;
langHebrew = 10;
langJapanese = 11;
langArabic = 12;
langFinnish = 13;
langGreek = 14;
langIcelandic = 15;
langMaltese = 16;
langTurkish = 17;
langCroatian = 18;
langTradChinese = 19;
langUrdu = 20;
langHindi = 21;
langThai = 22;
langKorean = 23;

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smHebrew script }
smJapanese script }
smArabic script }
smRoman script }
smGreek script }
extended Roman script }
extended Roman script }
extended Roman script }
Serbo-Croatian in extended Roman script }
Chinese in traditional characters }
smArabic script }
smDevanagari script }
smThai script }
smKorean script }

langLithuanian = 24;
langPolish = 25;
langHungarian = 26;
langEstonian = 27;
langLettish = 28;
langLatvian = 28;
langLapponian = 29;
langLappish = 29;
langFaeroese = 30;
langFarsi = 31;
langPersian = 31;
langRussian = 32;
langSimpChinese = 33;
langFlemish = 34;
langIrish = 35;
langAlbanian = 36;
langRomanian = 37;
langCzech = 38;
langSlovak = 39;
langSlovenian = 40;
langYiddish = 41;
langSerbian = 42;
langMacedonian = 43;
langBulgarian = 44;
langUkrainian = 45;
langByelorussian = 46;
langUzbek = 47;
langKazakh = 48;
langAzerbaijani = 49;
langAzerbaijanAr = 50;
langArmenian = 51;
langGeorgian = 52;
langMoldavian = 53;
langKirghiz = 54;
langTajiki = 55;
langTurkmen = 56;
langMongolian = 57;
langMongolianCyr = 58;
langPashto = 59;
langKurdish = 60;
langKashmiri = 61;
langSindhi = 62;
langTibetan = 63;
langNepali = 64;
langSanskrit = 65;
langMarathi = 66;
langBengali = 67;
langAssamese = 68;
langGujarati = 69;
langPunjabi = 70;
langOriya = 71;
langMalayalam = 72;
langKannada = 73;
langTamil = 74;
langTelugu = 75;
langSinhalese = 76;


{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

smEastEurRoman script }
smEastEurRoman script }
smEastEurRoman script }
smEastEurRoman script }
smEastEurRoman script }
Synonym for langLettish }
extended Roman script }
Synonym for langLapponian }
smRoman script }
smArabic script }
Synonym for langFarsi }
smCyrillic script }
Chinese in simplified characters }
smRoman script }
smRoman script }
smRoman script }
smEastEurRoman script }
smEastEurRoman script }
smEastEurRoman script }
smEastEurRoman script }
smHebrew script }
Serbo-Croatian in smCyrillic script }
smCyrillic script }
smCyrillic script }
smCyrillic script }
smCyrillic script }
smCyrillic script }
smCyrillic script }
Azerbaijani in smCyrillic script (USSR) }
Azerbaijani in smArabic script (Iran) }
smArmenian script }
smGeorgian script }
smCyrillic script }
smCyrillic script }
smCyrillic script }
smCyrillic script }
Mongolian in smMongolian script }
Mongolian in smCyrillic script }
smArabic script }
smArabic script }
smArabic script }
smExtArabic script }
smTibetan script }
smDevanagari script }
smDevanagari script }
smDevanagari script }
smBengali script }
smBengali script }
smGujarati script }
smGurmukhi script }
smOriya script }
smMalayalam script }
smKannada script }
smTamil script }
smTelugu script }
smSinhalese script }


langBurmese = 77;
langKhmer = 78;
langLao = 79;
langVietnamese = 80;
langIndonesian = 81;
langTagalog = 82;
langMalayRoman = 83;
langMalayArabic = 84;
langAmharic = 85;
langTigrinya = 86;
langGalla = 87;
langOromo = 87;
langSomali = 88;
langSwahili = 89;
langRuanda = 90;
langRundi = 91;
langChewa = 92;
langMalagasy = 93;
langEsperanto = 94;
langWelsh = 128;
langBasque = 129;
langCatalan = 130;
langLatin = 131;
langQuechua = 132;
langGuarani = 133;
langAymara = 134;
langTatar = 135;
langUighur = 136;
langDzongkha = 137;
langJavaneseRom = 138;
langSundaneseRom = 139;


{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

smBurmese script }
smKhmer script }
smLaotian script }
smVietnamese script }
smRoman script }
smRoman script }
Malay in smRoman script }
Malay in smArabic script }
smEthiopic script }
smEthiopic script }
smEthiopic script }
Synonym for langGalla }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
extended Roman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smRoman script }
smCyrillic script }
smArabic script }
(lang of Bhutan) smTibetan script }
Javanese in smRoman script }
Sundanese in smRoman script }

{ Obsolete names, kept for backward compatibility }
langPortugese = 8;
{ old misspelled version, kept for compatibility }
langMalta = 16;
{ old misspelled version, kept for compatibility }
langYugoslavian = 18;
{ (use langCroatian, langSerbian, etc.) }
langChinese = 19;
{ (use langTradChinese or langSimpChinese) }

{$ENDC} { UsingLanguage }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Language.p}



{#####################################################################}
{### FILE: Lists.p}
{#####################################################################}
{
Created: Monday, January 7, 1991 at 5:54 AM
Lists.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Lists;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingLists}
{$SETC UsingLists := 1}
{$I+}
{$SETC ListsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingControls}
{$I $$Shell(PInterfaces)Controls.p}
{$ENDC}
{$IFC UNDEFINED UsingMemory}
{$I $$Shell(PInterfaces)Memory.p}
{$ENDC}
{$SETC UsingIncludes := ListsIncludes}
CONST
lDoVAutoscroll = 2;
lDoHAutoscroll = 1;
lOnlyOne = -128;
lExtendDrag = 64;
lNoDisjoint = 32;
lNoExtend = 16;
lNoRect = 8;
lUseSense = 4;
lNoNilHilite = 2;
lInitMsg = 0;
lDrawMsg = 1;
lHiliteMsg = 2;
lCloseMsg = 3;




TYPE
Cell = Point;
DataPtr = ^DataArray;
DataHandle = ^DataPtr;
DataArray = PACKED ARRAY [0..32000] OF CHAR;
ListPtr = ^ListRec;
ListHandle = ^ListPtr;
ListRec = RECORD
rView: Rect;
port: GrafPtr;
indent: Point;
cellSize: Point;
visible: Rect;
vScroll: ControlHandle;
hScroll: ControlHandle;
selFlags: SignedByte;
lActive: BOOLEAN;
lReserved: SignedByte;
listFlags: SignedByte;
clikTime: LONGINT;
clikLoc: Point;
mouseLoc: Point;
lClikLoop: ProcPtr;
lastClick: Cell;
refCon: LONGINT;
listDefProc: Handle;
userHandle: Handle;
dataBounds: Rect;
cells: DataHandle;
maxIndex: INTEGER;
cellArray: ARRAY [1..1] OF INTEGER;
END;

FUNCTION LNew(rView: Rect;dataBounds: Rect;cSize: Point;theProc: INTEGER;
theWindow: WindowPtr;drawIt: BOOLEAN;hasGrow: BOOLEAN;scrollHoriz: BOOLEAN;
scrollVert: BOOLEAN): ListHandle;
INLINE $3F3C,$0044,$A9E7;
PROCEDURE LDispose(lHandle: ListHandle);
INLINE $3F3C,$0028,$A9E7;
FUNCTION LAddColumn(count: INTEGER;colNum: INTEGER;lHandle: ListHandle): INTEGER;
INLINE $3F3C,$0004,$A9E7;
FUNCTION LAddRow(count: INTEGER;rowNum: INTEGER;lHandle: ListHandle): INTEGER;
INLINE $3F3C,$0008,$A9E7;
PROCEDURE LDelColumn(count: INTEGER;colNum: INTEGER;lHandle: ListHandle);
INLINE $3F3C,$0020,$A9E7;
PROCEDURE LDelRow(count: INTEGER;rowNum: INTEGER;lHandle: ListHandle);
INLINE $3F3C,$0024,$A9E7;
FUNCTION LGetSelect(next: BOOLEAN;VAR theCell: Cell;lHandle: ListHandle): BOOLEAN;
INLINE $3F3C,$003C,$A9E7;
FUNCTION LLastClick(lHandle: ListHandle): Cell;
INLINE $3F3C,$0040,$A9E7;
FUNCTION LNextCell(hNext: BOOLEAN;vNext: BOOLEAN;VAR theCell: Cell;lHandle: ListHandle): BOOLEAN;

INLINE $3F3C,$0048,$A9E7;
FUNCTION LSearch(dataPtr: Ptr;dataLen: INTEGER;searchProc: ProcPtr;VAR theCell: Cell;
lHandle: ListHandle): BOOLEAN;
INLINE $3F3C,$0054,$A9E7;
PROCEDURE LSize(listWidth: INTEGER;listHeight: INTEGER;lHandle: ListHandle);
INLINE $3F3C,$0060,$A9E7;
PROCEDURE LDoDraw(drawIt: BOOLEAN;lHandle: ListHandle);
INLINE $3F3C,$002C,$A9E7;
PROCEDURE LScroll(dCols: INTEGER;dRows: INTEGER;lHandle: ListHandle);
INLINE $3F3C,$0050,$A9E7;
PROCEDURE LAutoScroll(lHandle: ListHandle);
INLINE $3F3C,$0010,$A9E7;
PROCEDURE LUpdate(theRgn: RgnHandle;lHandle: ListHandle);
INLINE $3F3C,$0064,$A9E7;
PROCEDURE LActivate(act: BOOLEAN;lHandle: ListHandle);
INLINE $4267,$A9E7;
PROCEDURE LCellSize(cSize: Point;lHandle: ListHandle);
INLINE $3F3C,$0014,$A9E7;
FUNCTION LClick(pt: Point;modifiers: INTEGER;lHandle: ListHandle): BOOLEAN;
INLINE $3F3C,$0018,$A9E7;
PROCEDURE LAddToCell(dataPtr: Ptr;dataLen: INTEGER;theCell: Cell;lHandle: ListHandle);
INLINE $3F3C,$000C,$A9E7;
PROCEDURE LClrCell(theCell: Cell;lHandle: ListHandle);
INLINE $3F3C,$001C,$A9E7;
PROCEDURE LGetCell(dataPtr: Ptr;VAR dataLen: INTEGER;theCell: Cell;lHandle: ListHandle);
INLINE $3F3C,$0038,$A9E7;
PROCEDURE LFind(VAR offset: INTEGER;VAR len: INTEGER;theCell: Cell;lHandle: ListHandle);
INLINE $3F3C,$0034,$A9E7;
PROCEDURE LRect(VAR cellRect: Rect;theCell: Cell;lHandle: ListHandle);
INLINE $3F3C,$004C,$A9E7;
PROCEDURE LSetCell(dataPtr: Ptr;dataLen: INTEGER;theCell: Cell;lHandle: ListHandle);
INLINE $3F3C,$0058,$A9E7;
PROCEDURE LSetSelect(setIt: BOOLEAN;theCell: Cell;lHandle: ListHandle);
INLINE $3F3C,$005C,$A9E7;
PROCEDURE LDraw(theCell: Cell;lHandle: ListHandle);
INLINE $3F3C,$0030,$A9E7;

{$ENDC}

{ UsingLists }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Lists.p}

{#####################################################################}
{### FILE: MacPrint.p}
{#####################################################################}
{
File: MacPrint.p
As of MPW 3.0, interface files were reorganized to more closely
match "Inside Macintosh" reference books and be more consistant
from language to language.
Interfaces for the non-ROM based Print Manager are now found in Printing.p.
This file, which includes Printing.p, is provided for compatibility
with old sources.
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988
All Rights Reserved
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT MacPrint;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingMacPrint}
{$SETC UsingMacPrint := 1}
{$I+}
{$SETC MacPrintIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingPrinting}
{$I $$Shell(PInterfaces)Printing.p}
{$ENDC}
{$SETC UsingIncludes := MacPrintIncludes}
{$ENDC}

{ UsingMacPrint }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE MacPrint.p}



{#####################################################################}
{### FILE: Memory.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:47 PM
Memory.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Memory;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingMemory}
{$SETC UsingMemory := 1}
{$I+}
{$SETC MemoryIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := MemoryIncludes}
CONST
maxSize = $800000; {Max data block size is 8 megabytes}
defaultPhysicalEntryCount = 8;
{ values returned from the GetPageState function }
kPageInMemory = 0;
kPageOnDisk = 1;
kNotPaged = 2;
TYPE
Size = LONGINT;

{ size of a block in bytes }

THz = ^Zone;
Zone = RECORD
bkLim: Ptr;
purgePtr: Ptr;
hFstFree: Ptr;
zcbFree: LONGINT;
gzProc: ProcPtr;
moreMast: INTEGER;
flags: INTEGER;



cntRel: INTEGER;
maxRel: INTEGER;
cntNRel: INTEGER;
maxNRel: INTEGER;
cntEmpty: INTEGER;
cntHandles: INTEGER;
minCBFree: LONGINT;
purgeProc: ProcPtr;
sparePtr: Ptr;
allocPtr: Ptr;
heapData: INTEGER;
END;
MemoryBlock = RECORD
address: Ptr;
count: LONGINT;
END;
LogicalToPhysicalTable = RECORD
logical: MemoryBlock;
physical: ARRAY [0..defaultPhysicalEntryCount - 1] OF MemoryBlock;
END;

PageState = INTEGER;
StatusRegisterContents = INTEGER;
FUNCTION GetApplLimit: Ptr;
INLINE $2EB8,$0130;
FUNCTION GetZone: THz;
INLINE $A11A,$2E88;
FUNCTION SystemZone: THz;
INLINE $2EB8,$02A6;
FUNCTION ApplicZone: THz;
INLINE $2EB8,$02AA;
FUNCTION ApplicationZone: THz;
INLINE $2EB8,$02AA;
FUNCTION NewHandle(byteCount: Size): Handle;
  	INLINE $201F, $A122, $2E88;		{added by @uxmal; needs verification}
FUNCTION NewHandleSys(byteCount: Size): Handle;
FUNCTION NewHandleClear(byteCount: Size): Handle;
FUNCTION NewHandleSysClear(byteCount: Size): Handle;
FUNCTION HandleZone(h: Handle): THz;
FUNCTION RecoverHandle(p: Ptr): Handle;
FUNCTION NewPtr(byteCount: Size): Ptr;
  	INLINE $201F, $A11E, $3E80;		{added by @uxmal; needs verification}
FUNCTION NewPtrSys(byteCount: Size): Ptr;
FUNCTION NewPtrClear(byteCount: Size): Ptr;
FUNCTION NewPtrSysClear(byteCount: Size): Ptr;
FUNCTION PtrZone(p: Ptr): THz;
FUNCTION GZSaveHnd: Handle;
INLINE $2EB8,$0328;
FUNCTION TopMem: Ptr;
INLINE $2EB8,$0108;
FUNCTION MaxBlock: LONGINT;
  	INLINE $A061, $3E80;			{added by @uxmal; needs verification}
FUNCTION MaxBlockSys: LONGINT;
FUNCTION StackSpace: LONGINT;
FUNCTION NewEmptyHandle: Handle;
FUNCTION NewEmptyHandleSys: Handle;
PROCEDURE HLock(h: Handle);
INLINE $205F,$A029;
PROCEDURE HUnlock(h: Handle);
INLINE $205F,$A02A;
PROCEDURE HPurge(h: Handle);
INLINE $205F,$A049;
PROCEDURE HNoPurge(h: Handle);
INLINE $205F,$A04A;
{
// The two nested A-trap calls cause trouble. 
PROCEDURE HLockHi(h: Handle);
INLINE $205F,$A064,$A029;
}
FUNCTION StripAddress(theAddress: UNIV Ptr): Ptr;
{$IFC SystemSixOrLater }
INLINE $201F,$A055,$2E80;
{$ENDC}
FUNCTION Translate24To32(addr24: UNIV Ptr): Ptr;
INLINE $201F,$A091,$2E80;
FUNCTION TempNewHandle(logicalSize: Size;VAR resultCode: OSErr): Handle;
INLINE $3F3C,$001D,$A88F;
FUNCTION TempMaxMem(VAR grow: Size): Size;
INLINE $3F3C,$0015,$A88F;
FUNCTION TempFreeMem: LONGINT;
INLINE $3F3C,$0018,$A88F;
{ Temporary Memory routines renamed, but obsolete, in System 7.0 and later.
PROCEDURE TempHLock(h: Handle;VAR resultCode: OSErr);
INLINE $3F3C,$001E,$A88F;
PROCEDURE TempHUnlock(h: Handle;VAR resultCode: OSErr);
INLINE $3F3C,$001F,$A88F;
PROCEDURE TempDisposeHandle(h: Handle;VAR resultCode: OSErr);
INLINE $3F3C,$0020,$A88F;
FUNCTION TempTopMem: Ptr;
INLINE $3F3C,$0016,$A88F;

}

{ Temporary Memory routines as they were known before System 7.0. }
FUNCTION MFMaxMem(VAR grow: Size): Size;
	INLINE $3F3C,$0015,$A88F;
FUNCTION MFFreeMem: LONGINT;
	INLINE $3F3C,$0018,$A88F;
FUNCTION MFTempNewHandle(logicalSize: Size;VAR resultCode: OSErr): Handle;
	INLINE $3F3C,$001D,$A88F;
PROCEDURE MFTempHLock(h: Handle;VAR resultCode: OSErr);
	INLINE $3F3C,$001E,$A88F;
PROCEDURE MFTempHUnlock(h: Handle;VAR resultCode: OSErr);
	INLINE $3F3C,$001F,$A88F;
PROCEDURE MFTempDisposHandle(h: Handle;VAR resultCode: OSErr);
	INLINE $3F3C,$0020,$A88F;
FUNCTION MFTopMem: Ptr;
INLINE $3F3C,$0016,$A88F;
PROCEDURE InitApplZone;
INLINE $A02C;
PROCEDURE InitZone(pgrowZone: ProcPtr;cmoreMasters: INTEGER;limitPtr: UNIV Ptr;
startPtr: UNIV Ptr);
PROCEDURE SetZone(hz: THz);
INLINE $205F,$A01B;
FUNCTION CompactMem(cbNeeded: Size): Size;



FUNCTION CompactMemSys(cbNeeded: Size): Size;
PROCEDURE PurgeMem(cbNeeded: Size);
INLINE $201F,$A04D;
PROCEDURE PurgeMemSys(cbNeeded: Size);
INLINE $201F,$A44D;
FUNCTION FreeMem: LONGINT;
INLINE $A01C,$2E80;
FUNCTION FreeMemSys: LONGINT;
INLINE $A41C,$2E80;
PROCEDURE ResrvMem(cbNeeded: Size);
INLINE $201F,$A040;
PROCEDURE ReserveMem(cbNeeded: Size);
INLINE $201F,$A040;
PROCEDURE ReserveMemSys(cbNeeded: Size);
INLINE $201F,$A440;
FUNCTION MaxMem(VAR grow: Size): Size;
  	INLINE $A11D, $3E80, $2E88;  {out arg}	{added by @uxmal; needs verification}
FUNCTION MaxMemSys(VAR grow: Size): Size;
PROCEDURE SetGrowZone(growZone: ProcPtr);
INLINE $205F,$A04B;
PROCEDURE SetApplLimit(zoneLimit: UNIV Ptr);
INLINE $205F,$A02D;
PROCEDURE MoveHHi(h: Handle);
INLINE $205F,$A064;
PROCEDURE DisposPtr(p: Ptr);
INLINE $205F,$A01F;
PROCEDURE DisposePtr(p: Ptr);
INLINE $205F,$A01F;
FUNCTION GetPtrSize(p: Ptr): Size;
  	INLINE $205F, $A021, $3E80;				{ added by @uxmal; needs verification}
PROCEDURE SetPtrSize(p: Ptr;newSize: Size);
	INLINE $205F, $201F, $A020;				{ added by @uxmal; needs verification}
PROCEDURE DisposHandle(h: Handle);
INLINE $205F,$A023;
PROCEDURE DisposeHandle(h: Handle);
INLINE $205F,$A023;
FUNCTION GetHandleSize(h: Handle): Size;
  	INLINE $205F, $A025, $3E80;				{ added by @uxmal; needs verification}
	
PROCEDURE SetHandleSize(h: Handle;newSize: Size);
	inline $A024;							{ added by @uxmal; needs verification}
PROCEDURE EmptyHandle(h: Handle);
INLINE $205F,$A02B;
PROCEDURE ReallocHandle(h: Handle;byteCount: Size);
PROCEDURE ReallocateHandle(h: Handle;byteCount: Size);
PROCEDURE HSetRBit(h: Handle);
INLINE $205F,$A067;
PROCEDURE HClrRBit(h: Handle);
INLINE $205F,$A068;
PROCEDURE MoreMasters;
INLINE $A036;
PROCEDURE BlockMove(srcPtr: UNIV Ptr;destPtr: UNIV Ptr;byteCount: Size);
  	INLINE $205F, $225F, $201F, $A02E;		{ added by @uxmal; needs verification}

{ All CmpString... functions added by @uxmal; need verification}
FUNCTION CmpString(aStr,bStr: Str255; sizeSize : LONGINT) : BOOLEAN;
  	INLINE $205F, $225F, $201F, $A03C, $3E80;
FUNCTION CmpStringMarks(aStr,bStr: Str255; sizeSize : LONGINT) : BOOLEAN;
  	INLINE $205F, $225F, $201F, $A23C, $3E80;
FUNCTION CmpStringCase(aStr,bStr: Str255; sizeSize : LONGINT) : BOOLEAN;
  	INLINE $205F, $225F, $201F, $A43C, $3E80;
FUNCTION CmpStringMarksCase(aStr,bStr: Str255; sizeSize : LONGINT) : BOOLEAN;
  	INLINE $205F, $225F, $201F, $A63C, $3E80;
	
FUNCTION MemError: OSErr;
INLINE $3EB8,$0220;
PROCEDURE PurgeSpace(VAR total: LONGINT;VAR contig: LONGINT);
FUNCTION HGetState(h: Handle): SignedByte;
PROCEDURE HSetState(h: Handle;flags: SignedByte);
PROCEDURE SetApplBase(startPtr: UNIV Ptr);
INLINE $205F,$A057;
PROCEDURE MaxApplZone;
INLINE $A063;
FUNCTION HoldMemory(address: UNIV Ptr;count: LONGINT): OSErr;

FUNCTION UnholdMemory(address: UNIV Ptr;count: LONGINT): OSErr;
FUNCTION LockMemory(address: UNIV Ptr;count: LONGINT): OSErr;
FUNCTION LockMemoryContiguous(address: UNIV Ptr;count: LONGINT): OSErr;
FUNCTION UnlockMemory(address: UNIV Ptr;count: LONGINT): OSErr;
FUNCTION GetPhysical(VAR addresses: LogicalToPhysicalTable;VAR physicalEntryCount: LONGINT): OSErr;
FUNCTION DeferUserFn(userFunction: ProcPtr;argument: UNIV Ptr): OSErr;
FUNCTION DebuggerGetMax: LONGINT;
INLINE $7000,$A08D,$2E80;
PROCEDURE DebuggerEnter;
INLINE $7001,$A08D;
PROCEDURE DebuggerExit;
INLINE $7002,$A08D;
PROCEDURE DebuggerPoll;
INLINE $7003,$A08D;
FUNCTION GetPageState(address: UNIV Ptr): PageState;
INLINE $205F,$7004,$A08D,$3E80;
FUNCTION PageFaultFatal: BOOLEAN;
INLINE $7005,$A08D,$1E80;
FUNCTION DebuggerLockMemory(address: UNIV Ptr;count: LONGINT): OSErr;
FUNCTION DebuggerUnlockMemory(address: UNIV Ptr;count: LONGINT): OSErr;
FUNCTION EnterSupervisorMode: StatusRegisterContents;
INLINE $7008,$A08D,$3E80;

{$ENDC}

{ UsingMemory }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Memory.p}

{#####################################################################}
{### FILE: MemTypes.p}
{#####################################################################}
{
File: MemTypes.p
As of MPW 3.0, interface files were reorganized to more closely
match "Inside Macintosh" reference books and be more consistant
from language to language.
Interfaces for the basic type definitions are now found in Types.p.
This file, which includes Types.p, is provided for compatibility
with old sources.
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988
All Rights Reserved
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT MemTypes;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingMemTypes}
{$SETC UsingMemTypes := 1}
{$I+}
{$SETC MemTypesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := MemTypesIncludes}
{$ENDC}

{ UsingMemTypes }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE MemTypes.p}



{#####################################################################}
{### FILE: Menus.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:48 PM
Menus.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Menus;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingMenus}
{$SETC UsingMenus := 1}
{$I+}
{$SETC MenusIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := MenusIncludes}
CONST
noMark = 0;

{mark symbol for MarkItem}

{ menu defProc messages }
mDrawMsg = 0;
mChooseMsg = 1;
mSizeMsg = 2;
mDrawItemMsg = 4;
mCalcItemMsg = 5;
textMenuProc = 0;
hMenuCmd = 27;
hierMenu = -1;
mPopUpMsg = 3;
mctAllItems = -98;
mctLastIDIndic = -99;

{itemCmd == 0x001B ==> hierarchical menu}
{a hierarchical menu - for InsertMenu call}
{menu defProc messages - place yourself}
{search for all Items for the given ID}
{last color table entry has this in ID field}

TYPE
MenuPtr = ^MenuInfo;
MenuHandle = ^MenuPtr;
MenuInfo = RECORD
menuID: INTEGER;



menuWidth: INTEGER;
menuHeight: INTEGER;
menuProc: Handle;
enableFlags: LONGINT;
menuData: Str255;
END;
MCEntryPtr = ^MCEntry;
MCEntry = RECORD
mctID: INTEGER;
mctItem: INTEGER;
mctRGB1: RGBColor;
mctRGB2: RGBColor;
mctRGB3: RGBColor;
mctRGB4: RGBColor;
mctReserved: INTEGER;
END;

{menu ID. ID = 0 is the menu bar}
{menu Item. Item = 0 is a title}
{usage depends on ID and Item}
{usage depends on ID and Item}
{usage depends on ID and Item}
{usage depends on ID and Item}
{reserved for internal use}

{}
MCTablePtr = ^MCTable;
MCTableHandle = ^MCTablePtr;
MCTable = ARRAY [0..0] OF MCEntry;
MenuCRsrcPtr = ^MenuCRsrc;
MenuCRsrcHandle = ^MenuCRsrcPtr;
MenuCRsrc = RECORD
numEntries: INTEGER;
mcEntryRecs: MCTable;
END;

{ the entries themselves }

{number of entries}
{ARRAY [1..numEntries] of MCEntry}

PROCEDURE InitMenus;
INLINE $A930;
FUNCTION NewMenu(menuID: INTEGER;menuTitle: Str255): MenuHandle;
INLINE $A931;
FUNCTION GetMenu(resourceID: INTEGER): MenuHandle;
INLINE $A9BF;
PROCEDURE DisposeMenu(theMenu: MenuHandle);
INLINE $A932;
PROCEDURE AppendMenu(menu: MenuHandle;data: Str255);
INLINE $A933;
PROCEDURE AddResMenu(theMenu: MenuHandle;theType: ResType);
INLINE $A94D;
PROCEDURE InsertResMenu(theMenu: MenuHandle;theType: ResType;afterItem: INTEGER);
INLINE $A951;
PROCEDURE InsertMenu(theMenu: MenuHandle;beforeID: INTEGER);
INLINE $A935;
PROCEDURE DrawMenuBar;
INLINE $A937;
PROCEDURE InvalMenuBar;
INLINE $A81D;
PROCEDURE DeleteMenu(menuID: INTEGER);
INLINE $A936;



PROCEDURE ClearMenuBar;
INLINE $A934;
FUNCTION GetNewMBar(menuBarID: INTEGER): Handle;
INLINE $A9C0;
FUNCTION GetMenuBar: Handle;
INLINE $A93B;
PROCEDURE SetMenuBar(menuList: Handle);
INLINE $A93C;
PROCEDURE InsMenuItem(theMenu: MenuHandle;itemString: Str255;afterItem: INTEGER);
INLINE $A826;
PROCEDURE DelMenuItem(theMenu: MenuHandle;item: INTEGER);
INLINE $A952;
FUNCTION MenuKey(ch: CHAR): LONGINT;
INLINE $A93E;
PROCEDURE HiliteMenu(menuID: INTEGER);
INLINE $A938;
PROCEDURE SetItem(theMenu: MenuHandle;item: INTEGER;itemString: Str255);
INLINE $A947;
PROCEDURE GetItem(theMenu: MenuHandle;item: INTEGER;VAR itemString: Str255);
INLINE $A946;
PROCEDURE DisableItem(theMenu: MenuHandle;item: INTEGER);
INLINE $A93A;
PROCEDURE EnableItem(theMenu: MenuHandle;item: INTEGER);
INLINE $A939;
PROCEDURE CheckItem(theMenu: MenuHandle;item: INTEGER;checked: BOOLEAN);
INLINE $A945;
PROCEDURE SetItemMark(theMenu: MenuHandle;item: INTEGER;markChar: CHAR);
INLINE $A944;
PROCEDURE GetItemMark(theMenu: MenuHandle;item: INTEGER;VAR markChar: CHAR);
INLINE $A943;
PROCEDURE SetItemIcon(theMenu: MenuHandle;item: INTEGER;iconIndex: Byte);
INLINE $A940;
PROCEDURE GetItemIcon(theMenu: MenuHandle;item: INTEGER;VAR iconIndex: Byte);
INLINE $A93F;
PROCEDURE SetItemStyle(theMenu: MenuHandle;item: INTEGER;chStyle: Style);
INLINE $A942;
PROCEDURE GetItemStyle(theMenu: MenuHandle;item: INTEGER;VAR chStyle: Style);
PROCEDURE CalcMenuSize(theMenu: MenuHandle);
INLINE $A948;
FUNCTION CountMItems(theMenu: MenuHandle): INTEGER;
INLINE $A950;
FUNCTION GetMHandle(menuID: INTEGER): MenuHandle;
INLINE $A949;
PROCEDURE FlashMenuBar(menuID: INTEGER);
INLINE $A94C;
PROCEDURE SetMenuFlash(count: INTEGER);
INLINE $A94A;
FUNCTION MenuSelect(startPt: Point): LONGINT;
INLINE $A93D;
PROCEDURE InitProcMenu(resID: INTEGER);
INLINE $A808;
PROCEDURE GetItemCmd(theMenu: MenuHandle;item: INTEGER;VAR cmdChar: CHAR);
INLINE $A84E;
PROCEDURE SetItemCmd(theMenu: MenuHandle;item: INTEGER;cmdChar: CHAR);
INLINE $A84F;
FUNCTION PopUpMenuSelect(menu: MenuHandle;top: INTEGER;left: INTEGER;popUpItem: INTEGER): LONGINT;



INLINE $A80B;
FUNCTION MenuChoice: LONGINT;
INLINE $AA66;
PROCEDURE DelMCEntries(menuID: INTEGER;menuItem: INTEGER);
INLINE $AA60;
FUNCTION GetMCInfo: MCTableHandle;
INLINE $AA61;
PROCEDURE SetMCInfo(menuCTbl: MCTableHandle);
INLINE $AA62;
PROCEDURE DispMCInfo(menuCTbl: MCTableHandle);
INLINE $AA63;
FUNCTION GetMCEntry(menuID: INTEGER;menuItem: INTEGER): MCEntryPtr;
INLINE $AA64;
PROCEDURE SetMCEntries(numEntries: INTEGER;menuCEntries: MCTablePtr);
INLINE $AA65;

{$ENDC}

{ UsingMenus }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Menus.p}



{#####################################################################}
{### FILE: MIDI.p}
{#####################################################################}

{
Created: Tuesday, November 26, 1991 at 3:28 PM
MIDI.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT MIDI;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingMIDI}
{$SETC UsingMIDI := 1}
{$I+}
{$SETC MIDIIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := MIDIIncludes}
CONST
midiToolNum = 4;
midiMaxNameLen = 31;

{tool number of MIDI Manager for SndDispVersion call}
{maximum number of characters in port and client names}

{ Time formats }
midiFormatMSec = 0;
midiFormatBeats = 1;
midiFormat24fpsBit = 2;
midiFormat25fpsBit = 3;
midiFormat30fpsDBit = 4;
midiFormat30fpsBit = 5;
midiFormat24fpsQF = 6;
midiFormat25fpsQF = 7;
midiFormat30fpsDQF = 8;
midiFormat30fpsQF = 9;
midiInternalSync = 0;
midiExternalSync = 1;

{milliseconds}
{beats}
{24 frames/sec.}
{25 frames/sec.}
{30 frames/sec. drop-frame}
{30 frames/sec.}
{24 frames/sec. longInt format }
{25 frames/sec. longInt format }
{30 frames/sec. drop-frame longInt format }
{30 frames/sec. longInt format }
{internal sync}
{external sync}

{ Port types}
midiPortTypeTime = 0;

{time port}



midiPortTypeInput = 1;
midiPortTypeOutput = 2;
midiPortTypeTimeInv = 3;

{input port}
{output port}
{invisible time port}

{ OffsetTimes }
midiGetEverything = $7FFFFFFF;
midiGetNothing = $80000000;
midiGetCurrent = $00000000;

{get all packets, regardless of time stamps}
{get no packets, regardless of time stamps}
{get current packets only}

{

MIDI data and messages are passed in MIDIPacket records (see below).
The first byte of every MIDIPacket contains a set of flags
bits 0-1

bits 2-3
bits 4-6

00 = new MIDIPacket, not continued
01 = begining of continued MIDIPacket
10 = end of continued MIDIPacket
11 = continuation
reserved
000 = packet contains MIDI data
001 = packet contains MIDI Manager message

bit 7

0 = MIDIPacket has valid stamp
1 = stamp with current clock }

midiContMask = $03;
midiNoCont = $00;
midiStartCont = $01;
midiMidCont = $03;
midiEndCont = $02;
midiTypeMask = $70;
midiMsgType = $00;
midiMgrType = $10;
midiTimeStampMask = $80;
midiTimeStampCurrent = $80;
midiTimeStampValid = $00;
{

MIDI Manager MIDIPacket command words (the first word in the data field
for midiMgrType messages) }
midiOverflowErr = $0001;
midiSCCErr = $0002;
midiPacketErr = $0003;
midiMaxErr = $00FF;
{all command words less than this value are error indicators}
{ Valid results to be returned by readHooks }
midiKeepPacket = 0;
midiMorePacket = 1;
midiNoMorePacket = 2;
{ Errors: }
midiNoClientErr = -250;
midiNoPortErr = -251;
midiTooManyPortsErr = -252;
midiTooManyConsErr = -253;
midiVConnectErr = -254;
midiVConnectMade = -255;
midiVConnectRmvd = -256;

{no client with that ID found}
{no port with that ID found}
{too many ports already installed in the system}
{too many connections made}
{pending virtual connection created}
{pending virtual connection resolved}
{pending virtual connection removed}

midiNoConErr = -257;
midiWriteErr = -258;
midiNameLenErr = -259;
midiDupIDErr = -260;
midiInvalidCmdErr = -261;


{no connection exists between specified ports}
{MIDIWritePacket couldn't write to all connected ports}
{name supplied is longer than 31 characters}
{duplicate client ID}
{command not supported for port type}

{
Driver calls: }
midiOpenDriver = 1;
midiCloseDriver = 2;
TYPE
MIDIPacketPtr = ^MIDIPacket;
MIDIPacket = PACKED RECORD
flags: Byte;
len: Byte;
tStamp: LONGINT;
data: PACKED ARRAY [0..248] OF Byte;
END;
MIDIClkInfo = RECORD
sync: INTEGER;
curTime: LONGINT;
format: INTEGER;
END;

{synchronization external/internal}
{current value of port's clock}
{time code format}

MIDIIDRec = RECORD
clientID: OSType;
portID: OSType;
END;
MIDIPortInfoPtr = ^MIDIPortInfo;
MIDIPortInfoHdl = ^MIDIPortInfoPtr;
MIDIPortInfo = RECORD
portType: INTEGER;
timeBase: MIDIIDRec;
numConnects: INTEGER;
cList: ARRAY [1..1] OF MIDIIDRec;
END;

{type of port}
{MIDIIDRec for time base}
{number of connections}
{-r or $R- permits access to [1..numConnects] of MIDIIDRec}

MIDIPortParamsPtr = ^MIDIPortParams;
MIDIPortParams = RECORD
portID: OSType;
{ID of port, unique within client}
portType: INTEGER;
{Type of port - input, output, time, etc.}
timeBase: INTEGER;
{refnum of time base, 0 if none}
offsetTime: LONGINT;
{offset for current time stamps}
readHook: Ptr;
{routine to call when input data is valid}
refCon: LONGINT;
{refcon for port (for client use)}
initClock: MIDIClkInfo;
{initial settings for a time base}
name: Str255;
{name of the port, This is a real live string, not a ptr.}
END;
MIDIIDListPtr = ^MIDIIDList;
MIDIIDListHdl = ^MIDIIDListPtr;
MIDIIDList = RECORD
numIDs: INTEGER;
list: ARRAY [1..1] OF OSType;

{ -r or $R- permits access to [1..numIDs] of OSType }



END;

{
Prototype Declarations for readHook and timeProc
FUNCTION myReadHook(myPacket: MIDIPacketPtr; myRefCon: LONGINT) : INTEGER;
PROCEDURE myTimeProc(curTime: LONGINT; myRefCon: LONGINT);
}
FUNCTION SndDispVersion(toolnum: INTEGER): LONGINT;
FUNCTION MIDISignIn(clientID: OSType;refCon: LONGINT;icon: Handle;name: Str255): OSErr;
INLINE $203C,$0004,midiToolNum,$A800;
PROCEDURE MIDISignOut(clientID: OSType);
INLINE $203C,$0008,midiToolNum,$A800;
FUNCTION MIDIGetClients: MIDIIDListHdl;
INLINE $203C,$000C,midiToolNum,$A800;
PROCEDURE MIDIGetClientName(clientID: OSType;VAR name: Str255);
INLINE $203C,$0010,midiToolNum,$A800;
PROCEDURE MIDISetClientName(clientID: OSType;name: Str255);
INLINE $203C,$0014,midiToolNum,$A800;
FUNCTION MIDIGetPorts(clientID: OSType): MIDIIDListHdl;
INLINE $203C,$0018,midiToolNum,$A800;
FUNCTION MIDIAddPort(clientID: OSType;BufSize: INTEGER;VAR refnum: INTEGER;
init: MIDIPortParamsPtr): OSErr;
INLINE $203C,$001C,midiToolNum,$A800;
FUNCTION MIDIGetPortInfo(clientID: OSType;portID: OSType): MIDIPortInfoHdl;
INLINE $203C,$0020,midiToolNum,$A800;
FUNCTION MIDIConnectData(srcClID: OSType;srcPortID: OSType;dstClID: OSType;
dstPortID: OSType): OSErr;
INLINE $203C,$0024,midiToolNum,$A800;
FUNCTION MIDIUnConnectData(srcClID: OSType;srcPortID: OSType;dstClID: OSType;
dstPortID: OSType): OSErr;
INLINE $203C,$0028,midiToolNum,$A800;
FUNCTION MIDIConnectTime(srcClID: OSType;srcPortID: OSType;dstClID: OSType;
dstPortID: OSType): OSErr;
INLINE $203C,$002C,midiToolNum,$A800;
FUNCTION MIDIUnConnectTime(srcClID: OSType;srcPortID: OSType;dstClID: OSType;
dstPortID: OSType): OSErr;
INLINE $203C,$0030,midiToolNum,$A800;
PROCEDURE MIDIFlush(refnum: INTEGER);
INLINE $203C,$0034,midiToolNum,$A800;
FUNCTION MIDIGetReadHook(refnum: INTEGER): ProcPtr;
INLINE $203C,$0038,midiToolNum,$A800;
PROCEDURE MIDISetReadHook(refnum: INTEGER;hook: ProcPtr);
INLINE $203C,$003C,midiToolNum,$A800;
PROCEDURE MIDIGetPortName(clientID: OSType;portID: OSType;VAR name: Str255);
INLINE $203C,$0040,midiToolNum,$A800;
PROCEDURE MIDISetPortName(clientID: OSType;portID: OSType;name: Str255);
INLINE $203C,$0044,midiToolNum,$A800;
PROCEDURE MIDIWakeUp(refnum: INTEGER;time: LONGINT;period: LONGINT;timeProc: ProcPtr);
INLINE $203C,$0048,midiToolNum,$A800;
PROCEDURE MIDIRemovePort(refnum: INTEGER);
INLINE $203C,$004C,midiToolNum,$A800;
FUNCTION MIDIGetSync(refnum: INTEGER): INTEGER;
INLINE $203C,$0050,midiToolNum,$A800;



PROCEDURE MIDISetSync(refnum: INTEGER;sync: INTEGER);
INLINE $203C,$0054,midiToolNum,$A800;
FUNCTION MIDIGetCurTime(refnum: INTEGER): LONGINT;
INLINE $203C,$0058,midiToolNum,$A800;
PROCEDURE MIDISetCurTime(refnum: INTEGER;time: LONGINT);
INLINE $203C,$005C,midiToolNum,$A800;
PROCEDURE MIDIStartTime(refnum: INTEGER);
INLINE $203C,$0060,midiToolNum,$A800;
PROCEDURE MIDIStopTime(refnum: INTEGER);
INLINE $203C,$0064,midiToolNum,$A800;
PROCEDURE MIDIPoll(refnum: INTEGER;offsetTime: LONGINT);
INLINE $203C,$0068,midiToolNum,$A800;
FUNCTION MIDIWritePacket(refnum: INTEGER;packet: MIDIPacketPtr): OSErr;
INLINE $203C,$006C,midiToolNum,$A800;
FUNCTION MIDIWorldChanged(clientID: OSType): BOOLEAN;
INLINE $203C,$0070,midiToolNum,$A800;
FUNCTION MIDIGetOffsetTime(refnum: INTEGER): LONGINT;
INLINE $203C,$0074,midiToolNum,$A800;
PROCEDURE MIDISetOffsetTime(refnum: INTEGER;offsetTime: LONGINT);
INLINE $203C,$0078,midiToolNum,$A800;
FUNCTION MIDIConvertTime(srcFormat: INTEGER;dstFormat: INTEGER;time: LONGINT): LONGINT;
INLINE $203C,$007C,midiToolNum,$A800;
FUNCTION MIDIGetRefCon(refnum: INTEGER): LONGINT;
INLINE $203C,$0080,midiToolNum,$A800;
PROCEDURE MIDISetRefCon(refnum: INTEGER;refCon: LONGINT);
INLINE $203C,$0084,midiToolNum,$A800;
FUNCTION MIDIGetClRefCon(clientID: OSType): LONGINT;
INLINE $203C,$0088,midiToolNum,$A800;
PROCEDURE MIDISetClRefCon(clientID: OSType;refCon: LONGINT);
INLINE $203C,$008C,midiToolNum,$A800;
FUNCTION MIDIGetTCFormat(refnum: INTEGER): INTEGER;
INLINE $203C,$0090,midiToolNum,$A800;
PROCEDURE MIDISetTCFormat(refnum: INTEGER;format: INTEGER);
INLINE $203C,$0094,midiToolNum,$A800;
PROCEDURE MIDISetRunRate(refnum: INTEGER;rate: INTEGER;time: LONGINT);
INLINE $203C,$0098,midiToolNum,$A800;
FUNCTION MIDIGetClientIcon(clientID: OSType): Handle;
INLINE $203C,$009C,midiToolNum,$A800;

{$ENDC} { UsingMIDI }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE MIDI.p}



{#####################################################################}
{### FILE: Notification.p}
{#####################################################################}

{
Created: Tuesday, September 10, 1991 at 2:10 PM
Notification.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1989-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Notification;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingNotification}
{$SETC UsingNotification := 1}
{$I+}
{$SETC NotificationIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$SETC UsingIncludes := NotificationIncludes}
CONST
nmType = 8;
TYPE
NMProcPtr = ProcPtr;
NMRecPtr = ^NMRec;
NMRec = RECORD
qLink: QElemPtr;
qType: INTEGER;
nmFlags: INTEGER;
nmPrivate: LONGINT;
nmReserved: INTEGER;
nmMark: INTEGER;
nmIcon: Handle;
nmSound: Handle;
nmStr: StringPtr;

{next queue entry}
{queue type -- ORD(nmType) = 8}
{reserved}
{reserved}
{reserved}
{item to mark in Apple menu}
{handle to small icon}
{handle to sound record}
{string to appear in alert}

nmResp: NMProcPtr;
nmRefCon: LONGINT;
END;


{pointer to response routine}
{for application use}

FUNCTION NMInstall(nmReqPtr: NMRecPtr): OSErr;
INLINE $205F,$A05E,$3E80;
FUNCTION NMRemove(nmReqPtr: NMRecPtr): OSErr;
INLINE $205F,$A05F,$3E80;

{$ENDC} { UsingNotification }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Notification.p}


{#####################################################################}
{### FILE: ObjIntf.p}
{#####################################################################}
{
File: ObjIntf.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1986 - 1988
All rights reserved.
}
UNIT ObjIntf;
INTERFACE
TYPE
TObject = OBJECT;
FUNCTION ShallowClone: TObject;
{Lowest level method for copying an object; should not be overridden
except in very unusual cases. Simply calls HandToHand to copy
the object data.}
FUNCTION Clone: TObject;
{Defaults to calling ShallowClone; can be overridden to copy objects
refered to by fields.}
PROCEDURE ShallowFree;
{Lowest level method for freeing an object; should not be overridden
except in very unusual cases. Simply calls DisposHandle to
free the object data.}
PROCEDURE Free;
{Defaults to calling ShallowFree; can be overridden to free objects
refered to by fields.}
END.
{### END OF FILE ObjIntf.p}

{#####################################################################}
{### FILE: OSEvents.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:51 PM
OSEvents.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT OSEvents;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingOSEvents}
{$SETC UsingOSEvents := 1}
{$I+}
{$SETC OSEventsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingEvents}
{$I $$Shell(PInterfaces)Events.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$SETC UsingIncludes := OSEventsIncludes}
FUNCTION PostEvent(eventNum: INTEGER;eventMsg: LONGINT): OSErr;
FUNCTION PPostEvent(eventCode: INTEGER;eventMsg: LONGINT;VAR qEl: EvQElPtr): OSErr;
FUNCTION OSEventAvail(mask: INTEGER;VAR theEvent: EventRecord): BOOLEAN;
FUNCTION GetOSEvent(mask: INTEGER;VAR theEvent: EventRecord): BOOLEAN;
PROCEDURE FlushEvents(whichMask: INTEGER;stopMask: INTEGER);
INLINE $201F,$A032;
PROCEDURE SetEventMask(theMask: INTEGER);
INLINE $31DF,$0144;
FUNCTION GetEvQHdr: QHdrPtr;
INLINE $2EBC,$0000,$014A;

{$ENDC}

{ UsingOSEvents }

{$IFC NOT UsingIncludes}


END.
{$ENDC}

{### END OF FILE OSEvents.p}





{#####################################################################}
{### FILE: OSIntf.p}
{#####################################################################}
{
File: OSIntf.p
As of MPW 3.0, interface files were reorganized to more closely
match "Inside Macintosh" reference books and be more consistant
from language to language.
Interfaces for the Operating System calls are now found in the
files included below. This file is provided for compatibility
with old sources.
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988
All Rights Reserved
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT OSIntf;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingOSIntf}
{$SETC UsingOSIntf := 1}
{$I+}
{$SETC OSIntfIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingEvents}
{$I $$Shell(PInterfaces)Events.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$IFC UNDEFINED UsingDevices}
{$I $$Shell(PInterfaces)Devices.p}
{$ENDC}
{$IFC UNDEFINED UsingDeskBus}
{$I $$Shell(PInterfaces)DeskBus.p}
{$ENDC}
{$IFC UNDEFINED UsingDiskInit}
{$I $$Shell(PInterfaces)DiskInit.p}
{$ENDC}
{$IFC UNDEFINED UsingDisks}
{$I $$Shell(PInterfaces)Disks.p}

{$ENDC}
{$IFC UNDEFINED UsingErrors}
{$I $$Shell(PInterfaces)Errors.p}
{$ENDC}
{$IFC UNDEFINED UsingMemory}
{$I $$Shell(PInterfaces)Memory.p}
{$ENDC}
{$IFC UNDEFINED UsingOSEvents}
{$I $$Shell(PInterfaces)OSEvents.p}
{$ENDC}
{$IFC UNDEFINED UsingRetrace}
{$I $$Shell(PInterfaces)Retrace.p}
{$ENDC}
{$IFC UNDEFINED UsingSegLoad}
{$I $$Shell(PInterfaces)SegLoad.p}
{$ENDC}
{$IFC UNDEFINED UsingSerial}
{$I $$Shell(PInterfaces)Serial.p}
{$ENDC}
{$IFC UNDEFINED UsingShutDown}
{$I $$Shell(PInterfaces)ShutDown.p}
{$ENDC}
{$IFC UNDEFINED UsingSlots}
{$I $$Shell(PInterfaces)Slots.p}
{$ENDC}
{$IFC UNDEFINED UsingSound}
{$I $$Shell(PInterfaces)Sound.p}
{$ENDC}
{$IFC UNDEFINED UsingStart}
{$I $$Shell(PInterfaces)Start.p}
{$ENDC}
{$IFC UNDEFINED UsingTimer}
{$I $$Shell(PInterfaces)Timer.p}
{$ENDC}
{$SETC UsingIncludes := OSIntfIncludes}
{$ENDC}

{ UsingOSIntf }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE OSIntf.p}



{#####################################################################}
{### FILE: OSUtils.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:51 PM
OSUtils.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT OSUtils;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$SETC UsingOSUtils := 1}
{$I+}
{$SETC OSUtilsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := OSUtilsIncludes}
CONST
useFree = 0;
useATalk = 1;
useAsync = 2;
useExtClk = 3;
useMIDI = 4;
{*** Environs Equates ***}
curSysEnvVers = 2;
{ Machine Types }
envMac = -1;
envXL = -2;
envMachUnknown = 0;
env512KE = 1;
envMacPlus = 2;
envSE = 3;
envMacII = 4;
envMacIIx = 5;
envMacIIcx = 6;
envSE30 = 7;

{Externally clocked}

{Updated to equal latest SysEnvirons version}



envPortable = 8;
envMacIIci = 9;
envMacIIfx = 11;
{ CPU types }
envCPUUnknown = 0;
env68000 = 1;
env68010 = 2;
env68020 = 3;
env68030 = 4;
env68040 = 5;
{ Keyboard types }
envUnknownKbd = 0;
envMacKbd = 1;
envMacAndPad = 2;
envMacPlusKbd = 3;
envAExtendKbd = 4;
envStandADBKbd = 5;
envPrtblADBKbd = 6;
envPrtblISOKbd = 7;
envStdISOADBKbd = 8;
envExtISOADBKbd = 9;
false32b = 0;
true32b = 1;

{24 bit addressing error}
{32 bit addressing error}

{ result types for RelString Call }
sortsBefore = -1;
{first string < second string}
sortsEqual = 0;
{first string = second string}
sortsAfter = 1;
{first string > second string}
TYPE
QTypes = (dummyType,vType,ioQType,drvQType,evType,fsQType,sIQType,dtQType);
TrapType = (OSTrap,ToolTrap);
ParamBlkType = (IOParam,FileParam,VolumeParam,CntrlParam,SlotDevParam,MultiDevParam,
AccessParam,ObjParam,CopyParam,WDParam,FIDParam,CSParam,ForeignPrivParam);

SysPPtr = ^SysParmType;
SysParmType = PACKED RECORD
valid: Byte;
aTalkA: Byte;
aTalkB: Byte;
config: Byte;
portA: INTEGER;
portB: INTEGER;
alarm: LONGINT;
font: INTEGER;
kbdPrint: INTEGER;
volClik: INTEGER;
misc: INTEGER;
END;
{QElemPtr = ^QElem;}



QElemPtr = ^QElem;
FInfo = RECORD
fdType: OSType;
fdCreator: OSType;
fdFlags: INTEGER;
fdLocation: Point;
fdFldr: INTEGER;
END;

{the type of the file}
{file's creator}
{flags ex. hasbundle,invisible,locked, etc.}
{file's location in folder}
{folder containing file}

VCB = RECORD
qLink: QElemPtr;
qType: INTEGER;
vcbFlags: INTEGER;
vcbSigWord: INTEGER;
vcbCrDate: LONGINT;
vcbLsMod: LONGINT;
vcbAtrb: INTEGER;
vcbNmFls: INTEGER;
vcbVBMSt: INTEGER;
vcbAllocPtr: INTEGER;
vcbNmAlBlks: INTEGER;
vcbAlBlkSiz: LONGINT;
vcbClpSiz: LONGINT;
vcbAlBlSt: INTEGER;
vcbNxtCNID: LONGINT;
vcbFreeBks: INTEGER;
vcbVN: Str27;
vcbDrvNum: INTEGER;
vcbDRefNum: INTEGER;
vcbFSID: INTEGER;
vcbVRefNum: INTEGER;
vcbMAdr: Ptr;
vcbBufAdr: Ptr;
vcbMLen: INTEGER;
vcbDirIndex: INTEGER;
vcbDirBlk: INTEGER;
vcbVolBkUp: LONGINT;
vcbVSeqNum: INTEGER;
vcbWrCnt: LONGINT;
vcbXTClpSiz: LONGINT;
vcbCTClpSiz: LONGINT;
vcbNmRtDirs: INTEGER;
vcbFilCnt: LONGINT;
vcbDirCnt: LONGINT;
vcbFndrInfo: ARRAY [1..8] OF LONGINT;
vcbVCSize: INTEGER;
vcbVBMCSiz: INTEGER;
vcbCtlCSiz: INTEGER;
vcbXTAlBlks: INTEGER;
vcbCTAlBlks: INTEGER;
vcbXTRef: INTEGER;
vcbCTRef: INTEGER;
vcbCtlBuf: Ptr;
vcbDirIDM: LONGINT;

vcbOffsM: INTEGER;
END;
DrvQElPtr = ^DrvQEl;
DrvQEl = RECORD
qLink: QElemPtr;
qType: INTEGER;
dQDrive: INTEGER;
dQRefNum: INTEGER;
dQFSID: INTEGER;
dQDrvSz: INTEGER;
dQDrvSz2: INTEGER;
END;
ParmBlkPtr = ^ParamBlockRec;
ParamBlockRec = RECORD
qLink: QElemPtr;
qType: INTEGER;
ioTrap: INTEGER;
ioCmdAddr: Ptr;
ioCompletion: ProcPtr;
ioResult: OSErr;
ioNamePtr: StringPtr;
ioVRefNum: INTEGER;
CASE ParamBlkType OF
IOParam:
(ioRefNum: INTEGER;
ioVersNum: SignedByte;
ioPermssn: SignedByte;
ioMisc: Ptr;
ioBuffer: Ptr;
ioReqCount: LONGINT;
ioActCount: LONGINT;
ioPosMode: INTEGER;
ioPosOffset: LONGINT);
FileParam:
(ioFRefNum: INTEGER;
ioFVersNum: SignedByte;
filler1: SignedByte;
ioFDirIndex: INTEGER;
ioFlAttrib: SignedByte;
ioFlVersNum: SignedByte;
ioFlFndrInfo: FInfo;
ioFlNum: LONGINT;
ioFlStBlk: INTEGER;
ioFlLgLen: LONGINT;
ioFlPyLen: LONGINT;
ioFlRStBlk: INTEGER;
ioFlRLgLen: LONGINT;
ioFlRPyLen: LONGINT;
ioFlCrDat: LONGINT;
ioFlMdDat: LONGINT);
VolumeParam:
(filler2: LONGINT;
ioVolIndex: INTEGER;
ioVCrDate: LONGINT;





ioVLsBkUp: LONGINT;
ioVAtrb: INTEGER;
ioVNmFls: INTEGER;
ioVDirSt: INTEGER;
ioVBlLn: INTEGER;
ioVNmAlBlks: INTEGER;
ioVAlBlkSiz: LONGINT;
ioVClpSiz: LONGINT;
ioAlBlSt: INTEGER;
ioVNxtFNum: LONGINT;
ioVFrBlk: INTEGER);
CntrlParam:
(ioCRefNum: INTEGER;
csCode: INTEGER;
csParam: ARRAY [0..10] OF INTEGER);
SlotDevParam:
(filler3: LONGINT;
ioMix: Ptr;
ioFlags: INTEGER;
ioSlot: SignedByte;
ioID: SignedByte);
MultiDevParam:
(filler4: LONGINT;
ioMMix: Ptr;
ioMFlags: INTEGER;
ioSEBlkPtr: Ptr);
END;
EvQElPtr = ^EvQEl;
EvQEl = RECORD
qLink: QElemPtr;
qType: INTEGER;
evtQWhat: INTEGER;
evtQMessage: LONGINT;
evtQWhen: LONGINT;
evtQWhere: Point;
evtQModifiers: INTEGER;
END;

{this part is identical to the EventRecord as...}
{defined in ToolIntf}

VBLTask = RECORD
qLink: QElemPtr;
qType: INTEGER;
vblAddr: ProcPtr;
vblCount: INTEGER;
vblPhase: INTEGER;
END;
DeferredTask = RECORD
qLink: QElemPtr;
qType: INTEGER;
dtFlags: INTEGER;
dtAddr: ProcPtr;
dtParm: LONGINT;
dtReserved: LONGINT;
END;

{next queue entry}
{queue type}
{reserved}
{pointer to task}
{optional parameter}
{reserved--should be 0}

QElem = RECORD
CASE QTypes OF
dtQType:
(dtQElem: DeferredTask);
vType:
(vblQElem: VBLTask);
ioQType:
(ioQElem: ParamBlockRec);
drvQType:
(drvQElem: DrvQEl);
evType:
(evQElem: EvQEl);
fsQType:
(vcbQElem: VCB);
END;
{deferred}
{vertical blanking}
{I/O parameter block}
{drive}
{event}
{volume control block}

QHdrPtr = ^QHdr;
QHdr = RECORD
qFlags: INTEGER;
qHead: QElemPtr;
qTail: QElemPtr;
END;
DateTimeRec = RECORD
year: INTEGER;
month: INTEGER;
day: INTEGER;
hour: INTEGER;
minute: INTEGER;
second: INTEGER;
dayOfWeek: INTEGER;
END;
SysEnvRec = RECORD
environsVersion: INTEGER;
machineType: INTEGER;
systemVersion: INTEGER;
processor: INTEGER;
hasFPU: BOOLEAN;
hasColorQD: BOOLEAN;
keyBoardType: INTEGER;
atDrvrVersNum: INTEGER;
sysVRefNum: INTEGER;
END;

FUNCTION GetSysPPtr: SysPPtr;
INLINE $2EBC,$0000,$01F8;
PROCEDURE SysBeep(duration: INTEGER);
INLINE $A9C8;
FUNCTION KeyTrans(transData: Ptr;keycode: INTEGER;VAR state: LONGINT): LONGINT;
INLINE $A9C3;
FUNCTION DTInstall(dtTaskPtr: QElemPtr): OSErr;
FUNCTION GetMMUMode: SignedByte;
PROCEDURE SwapMMUMode(VAR mode: SignedByte);
FUNCTION SysEnvirons(versionRequested: INTEGER;VAR theWorld: SysEnvRec): OSErr;

FUNCTION ReadDateTime(VAR time: LONGINT): OSErr;
PROCEDURE GetDateTime(VAR secs: LONGINT);
FUNCTION SetDateTime(time: LONGINT): OSErr;
PROCEDURE SetTime(d: DateTimeRec);
PROCEDURE GetTime(VAR d: DateTimeRec);
PROCEDURE Date2Secs(d: DateTimeRec;VAR secs: LONGINT);
PROCEDURE Secs2Date(secs: LONGINT;VAR d: DateTimeRec);
  	INLINE $201F, $225F, $A9C6;			{added by @uxmal; needs verification}
PROCEDURE Delay(numTicks: LONGINT;VAR finalTicks: LONGINT);
	inline $A03B;						{added by @uxmal; needs verification}
FUNCTION GetTrapAddress(trapNum: INTEGER): LONGINT;
  	INLINE $201F, $A146, $2E88;			{added by @uxmal; needs verification}
PROCEDURE SetTrapAddress(trapAddr: LONGINT;trapNum: INTEGER);
FUNCTION NGetTrapAddress(trapNum: INTEGER;tTyp: TrapType): LONGINT;
PROCEDURE NSetTrapAddress(trapAddr: LONGINT;trapNum: INTEGER;tTyp: TrapType);
FUNCTION GetOSTrapAddress(trapNum: INTEGER): LONGINT;
  	INLINE $201F, $A346, $2E88;			{added by @uxmal; needs verification}
PROCEDURE SetOSTrapAddress(trapAddr: LONGINT;trapNum: INTEGER);
FUNCTION GetToolTrapAddress(trapNum: INTEGER): LONGINT;
PROCEDURE SetToolTrapAddress(trapAddr: LONGINT;trapNum: INTEGER);
FUNCTION GetToolboxTrapAddress(trapNum: INTEGER): LONGINT;
  	INLINE $201F, $A746, $2E88;			{added by @uxmal; needs verification}
PROCEDURE SetToolboxTrapAddress(trapAddr: LONGINT;trapNum: INTEGER);
FUNCTION WriteParam: OSErr;
FUNCTION EqualString(str1: Str255;str2: Str255;caseSens: BOOLEAN;diacSens: BOOLEAN): BOOLEAN;
PROCEDURE UprString(VAR theString: Str255;diacSens: BOOLEAN);
PROCEDURE Enqueue(qElement: QElemPtr;qHeader: QHdrPtr);
FUNCTION Dequeue(qElement: QElemPtr;qHeader: QHdrPtr): OSErr;
FUNCTION SetCurrentA5: LONGINT;
INLINE $2E8D,$2A78,$0904;
FUNCTION SetA5(newA5: LONGINT): LONGINT;
INLINE $2F4D,$0004,$2A5F;
PROCEDURE Environs(VAR rom: INTEGER;VAR machine: INTEGER);
FUNCTION RelString(str1: Str255;str2: Str255;caseSens: BOOLEAN;diacSens: BOOLEAN): INTEGER;
FUNCTION HandToHand(VAR theHndl: Handle): OSErr;
FUNCTION PtrToXHand(srcPtr: Ptr;dstHndl: Handle;size: LONGINT): OSErr;
FUNCTION PtrToHand(srcPtr: Ptr;VAR dstHndl: Handle;size: LONGINT): OSErr;
FUNCTION HandAndHand(hand1: Handle;hand2: Handle): OSErr;
FUNCTION PtrAndHand(ptr1: Ptr;hand2: Handle;size: LONGINT): OSErr;
FUNCTION InitUtil: OSErr;
INLINE $A03F,$3E80;
FUNCTION SwapInstructionCache(cacheEnable: BOOLEAN): BOOLEAN;
PROCEDURE FlushInstructionCache;
FUNCTION SwapDataCache(cacheEnable: BOOLEAN): BOOLEAN;
PROCEDURE FlushDataCache;

{$ENDC}

{ UsingOSUtils }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE OSUtils.p}



{#####################################################################}
{### FILE: Packages.p}
{#####################################################################}

{
Created: Sunday, September 15, 1991 at 11:56 PM
Packages.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Packages;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPackages}
{$SETC UsingPackages := 1}
{$I+}
{$SETC PackagesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingStandardFile}
{$I $$Shell(PInterfaces)StandardFile.p}
{$ENDC}
{$IFC UNDEFINED UsingScript}
{$I $$Shell(PInterfaces)Script.p}
{$ENDC}
{$SETC UsingIncludes := PackagesIncludes}
CONST
listMgr = 0;
dskInit = 2;
stdFile = 3;
flPoint = 4;
trFunc = 5;
intUtil = 6;
bdConv = 7;
editionMgr = 11;
currSymLead = 16;
currNegSym = 32;
currTrailingZ = 64;
currLeadingZ = 128;
zeroCycle = 1;

{list manager}
{Disk Initializaton}
{Standard File}
{Floating-Point Arithmetic}
{Transcendental Functions}
{International Utilities}
{Binary/Decimal Conversion}
{Edition Manager}

{0:00 AM/PM format}

longDay = 0;
longWeek = 1;
longMonth = 2;
longYear = 3;
supDay = 1;
supWeek = 2;
supMonth = 4;
supYear = 8;
dayLdingZ = 32;
mntLdingZ = 64;
century = 128;
secLeadingZ = 32;
minLeadingZ = 64;
hrLeadingZ = 128;


{day of the month}
{day of the week}
{month of the year}
{year}
{suppress day of month}
{suppress day of week}
{suppress month}
{suppress year}

{ Date Orders }
mdy = 0;
dmy = 1;
ymd = 2;
myd = 3;
dym = 4;
ydm = 5;

{ Regional version codes }
verUS = 0;
verFrance = 1;
verBritain = 2;
verGermany = 3;
verItaly = 4;
verNetherlands = 5;
verFrBelgiumLux = 6;
verSweden = 7;
verSpain = 8;
verDenmark = 9;
verPortugal = 10;
verFrCanada = 11;
verNorway = 12;
verIsrael = 13;
verJapan = 14;
verAustralia = 15;
verArabic = 16;
verFinland = 17;
verFrSwiss = 18;
verGrSwiss = 19;
verGreece = 20;
verIceland = 21;
verMalta = 22;
verCyprus = 23;
verTurkey = 24;
verYugoCroatian = 25;
verIndiaHindi = 33;
verPakistan = 34;
verLithuania = 41;
verPoland = 42;
verHungary = 43;

{ French for Belgium & Luxembourg }

{ synonym for verArabia }
{ French Swiss }
{ German Swiss }

{ Croatian system for Yugoslavia }
{ Hindi system for India }

verEstonia = 44;
verLatvia = 45;
verLapland = 46;
verFaeroeIsl = 47;
verIran = 48;
verRussia = 49;
verIreland = 50;
verKorea = 51;
verChina = 52;
verTaiwan = 53;
verThailand = 54;
minCountry = verUS;
maxCountry = verThailand;
{ English-language version for Ireland }

{Obsolete region code names, kept for backward compatibility}
verBelgiumLux = 6;
{ (use verFrBelgiumLux instead, less ambiguous) }
verArabia = 16;
verYugoslavia = 25;
{ (use verYugoCroatian instead, less ambiguous) }
verIndia = 33;
{ (use verIndiaHindi instead, less ambiguous) }
{ Special script code values for International Utilities }
iuSystemScript = -1;
{ system script }
iuCurrentScript = -2;
{ current script (for font of grafPort) }
{ Special language code values for International Utilities }
iuSystemCurLang = -2;
{ current (itlbLang) lang for system script }
iuSystemDefLang = -3;
{ default (table) lang for system script }
iuCurrentCurLang = -4;
{ current (itlbLang) lang for current script }
iuCurrentDefLang = -5;
{ default lang for current script }
iuScriptCurLang = -6;
{ current (itlbLang) lang for specified script }
iuScriptDefLang = -7;
{ default language for a specified script }
{ Table selectors for GetItlTable }
iuWordSelectTable = 0;
iuWordWrapTable = 1;
iuNumberPartsTable = 2;
iuUnTokenTable = 3;
iuWhiteSpaceList = 4;

{
{
{
{
{

get
get
get
get
get

word select break table from 'itl2' }
word wrap break table from 'itl2' }
default number parts table from 'itl4' }
unToken table from 'itl4' }
white space list from 'itl4' }

TYPE
DateForm = (shortDate,longDate,abbrevDate);

Intl0Ptr = ^Intl0Rec;
Intl0Hndl = ^Intl0Ptr;
Intl0Rec = PACKED RECORD
decimalPt: CHAR;
thousSep: CHAR;
listSep: CHAR;
currSym1: CHAR;
currSym2: CHAR;
currSym3: CHAR;
currFmt: Byte;
dateOrder: Byte;
shrtDateFmt: Byte;
dateSep: CHAR;

{decimal point character}
{thousands separator character}
{list separator character}
{currency symbol}

{currency format flags}
{order of short date elements: mdy, dmy, etc.}
{format flags for each short date element}
{date separator character}



timeCycle: Byte;
timeFmt: Byte;
mornStr: PACKED ARRAY [1..4] OF CHAR;
eveStr: PACKED ARRAY [1..4] OF CHAR;
timeSep: CHAR;
time1Suff: CHAR;
time2Suff: CHAR;
time3Suff: CHAR;
time4Suff: CHAR;
time5Suff: CHAR;
time6Suff: CHAR;
time7Suff: CHAR;
time8Suff: CHAR;
metricSys: Byte;
intl0Vers: INTEGER;
END;
Intl1Ptr = ^Intl1Rec;
Intl1Hndl = ^Intl1Ptr;
Intl1Rec = PACKED RECORD
days: ARRAY [1..7] OF Str15;
months: ARRAY [1..12] OF Str15;
suppressDay: Byte;
lngDateFmt: Byte;
dayLeading0: Byte;
abbrLen: Byte;
st0: PACKED ARRAY [1..4] OF CHAR;
st1: PACKED ARRAY [1..4] OF CHAR;
st2: PACKED ARRAY [1..4] OF CHAR;
st3: PACKED ARRAY [1..4] OF CHAR;
st4: PACKED ARRAY [1..4] OF CHAR;
intl1Vers: INTEGER;
localRtn: ARRAY [0..0] OF INTEGER;
END;

{specifies time cycle: 0..23, 1..12, or 0..11}
{format flags for each time element}
{trailing string for AM if 12-hour cycle}
{trailing string for PM if 12-hour cycle}
{time separator character}
{trailing string for AM if 24-hour cycle}

{trailing string for PM if 24-hour cycle}

{255 if metric, 0 if inches etc.}
{region code (hi byte) and version (lo byte)}

{day names}
{month names}
{255 for no day, or flags to suppress any element}
{order of long date elements}
{255 for leading 0 in day number}
{length for abbreviating names}
{separator strings for long date format}

{region code (hi byte) and version (lo byte)}
{now a flag for opt extension}

PROCEDURE InitPack(packID: INTEGER);
INLINE $A9E5;
PROCEDURE InitAllPacks;
INLINE $A9E6;
FUNCTION IUGetIntl(theID: INTEGER): Handle;
INLINE $3F3C,$0006,$A9ED;
PROCEDURE IUSetIntl(refNum: INTEGER;theID: INTEGER;intlHandle: Handle);
INLINE $3F3C,$0008,$A9ED;
PROCEDURE IUDateString(dateTime: LONGINT;longFlag: DateForm;VAR result: Str255);
INLINE $4267,$A9ED;
PROCEDURE IUDatePString(dateTime: LONGINT;longFlag: DateForm;VAR result: Str255;
intlHandle: Handle);
INLINE $3F3C,$000E,$A9ED;
PROCEDURE IUTimeString(dateTime: LONGINT;wantSeconds: BOOLEAN;VAR result: Str255);
INLINE $3F3C,$0002,$A9ED;
PROCEDURE IUTimePString(dateTime: LONGINT;wantSeconds: BOOLEAN;VAR result: Str255;
intlHandle: Handle);
INLINE $3F3C,$0010,$A9ED;
FUNCTION IUMetric: BOOLEAN;

INLINE $3F3C,$0004,$A9ED;
FUNCTION IUMagString(aPtr: Ptr;bPtr: Ptr;aLen: INTEGER;bLen: INTEGER): INTEGER;
INLINE $3F3C,$000A,$A9ED;
FUNCTION IUMagIDString(aPtr: Ptr;bPtr: Ptr;aLen: INTEGER;bLen: INTEGER): INTEGER;
INLINE $3F3C,$000C,$A9ED;
FUNCTION IUCompString(aStr: Str255;bStr: Str255): INTEGER;
FUNCTION IUEqualString(aStr: Str255;bStr: Str255): INTEGER;
PROCEDURE StringToNum(theString: Str255;VAR theNum: LONGINT);
PROCEDURE NumToString(theNum: LONGINT;VAR theString: Str255);
PROCEDURE IULDateString(VAR dateTime: LongDateTime;longFlag: DateForm;VAR result: Str255;
intlHandle: Handle);
INLINE $3F3C,$0014,$A9ED;
PROCEDURE IULTimeString(VAR dateTime: LongDateTime;wantSeconds: BOOLEAN;
VAR result: Str255;intlHandle: Handle);
INLINE $3F3C,$0016,$A9ED;
PROCEDURE IUClearCache;
INLINE $3F3C,$0018,$A9ED;
FUNCTION IUMagPString(aPtr: Ptr;bPtr: Ptr;aLen: INTEGER;bLen: INTEGER;itl2Handle: Handle): INTEGER;
INLINE $3F3C,$001A,$A9ED;
FUNCTION IUMagIDPString(aPtr: Ptr;bPtr: Ptr;aLen: INTEGER;bLen: INTEGER;
itl2Handle: Handle): INTEGER;
INLINE $3F3C,$001C,$A9ED;
FUNCTION IUCompPString(aStr: Str255;bStr: Str255;itl2Handle: Handle): INTEGER;
FUNCTION IUEqualPString(aStr: Str255;bStr: Str255;itl2Handle: Handle): INTEGER;
FUNCTION IUScriptOrder(script1: ScriptCode;script2: ScriptCode): INTEGER;
INLINE $3F3C,$001E,$A9ED;
FUNCTION IULangOrder(language1: LangCode;language2: LangCode): INTEGER;
INLINE $3F3C,$0020,$A9ED;
FUNCTION IUTextOrder(aPtr: Ptr;bPtr: Ptr;aLen: INTEGER;bLen: INTEGER;aScript: ScriptCode;
bScript: ScriptCode;aLang: LangCode;bLang: LangCode): INTEGER;
INLINE $3F3C,$0022,$A9ED;
FUNCTION IUStringOrder(aStr: Str255;bStr: Str255;aScript: ScriptCode;bScript: ScriptCode;
aLang: LangCode;bLang: LangCode): INTEGER;
PROCEDURE IUGetItlTable(script: ScriptCode;tableCode: INTEGER;VAR itlHandle: Handle;
VAR offset: LONGINT;VAR length: LONGINT);
INLINE $3F3C,$0024,$A9ED;

{$ENDC} { UsingPackages }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Packages.p}

{#####################################################################}
{### FILE: PackIntf.p}
{#####################################################################}
{
File: PackIntf.p
As of MPW 3.0, interface files were reorganized to more closely
match "Inside Macintosh" reference books and be more consistant
from language to language.
Interfaces for the Package Manager are now found in Packages.p.
This file, which includes Packages.p, is provided for compatibility
with old sources.
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988
All Rights Reserved
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT PackIntf;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPackIntf}
{$SETC UsingPackIntf := 1}
{$I+}
{$SETC PackIntfIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingPackages}
{$I $$Shell(PInterfaces)Packages.p}
{$ENDC}
{$SETC UsingIncludes := PackIntfIncludes}
{$ENDC}

{ UsingPackIntf }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE PackIntf.p}
{#####################################################################}
{### FILE: PaletteMgr.p}
{#####################################################################}
{
File: PaletteMgr.p
As of MPW 3.0, interface files were reorganized to more closely
match "Inside Macintosh" reference books and be more consistant
from language to language.
Interfaces for the Palette Manager are now found in Palettes.p.
This file, which includes Palettes.p, is provided for compatibility
with old sources.
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988
All Rights Reserved
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT PaletteMgr;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPaletteMgr}
{$SETC UsingPaletteMgr := 1}
{$I+}
{$SETC PaletteMgrIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingPalettes}
{$I $$Shell(PInterfaces)Palettes.p}
{$ENDC}
{$SETC UsingIncludes := PaletteMgrIncludes}
{$ENDC}

{ UsingPaletteMgr }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE PaletteMgr.p}



{#####################################################################}
{### FILE: Palettes.p}
{#####################################################################}

{
Created: Monday, September 16, 1991 at 12:00 AM
Palettes.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1987-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Palettes;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPalettes}
{$SETC UsingPalettes := 1}
{$I+}
{$SETC PalettesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$IFC UNDEFINED UsingWindows}
{$I $$Shell(PInterfaces)Windows.p}
{$ENDC}
{$SETC UsingIncludes := PalettesIncludes}
CONST
pmCourteous = 0;
pmTolerant = $0002;
pmAnimated = $0004;
pmExplicit = $0008;
pmWhite = $0010;
pmBlack = $0020;
pmInhibitG2 = $0100;
pmInhibitC2 = $0200;
pmInhibitG4	= $0400;
pmInhibitC4	= $0800;
pmInhibitG8	= $1000;
pmInhibitC8	= $2000;

{Record use of color on each device touched.}
{render ciRGB if ciTolerance is exceeded by best match.}
{reserve an index on each device touched and render ciRGB.}
{no reserve, no render, no record; stuff index into port.}

{ NSetPalette Update Constants }
pmNoUpdates = $8000;
pmBkUpdates = $A000;
pmFgUpdates = $C000;
pmAllUpdates = $E000;
TYPE
ColorInfo = RECORD
ciRGB: RGBColor;
ciUsage: INTEGER;
ciTolerance: INTEGER;
ciDataFields: ARRAY [0..2] OF INTEGER;
END;
PalettePtr = ^Palette;
PaletteHandle = ^PalettePtr;
Palette = RECORD
pmEntries: INTEGER;
pmDataFields: ARRAY [0..6] OF INTEGER;
pmInfo: ARRAY [0..0] OF ColorInfo;
END;

{no updates}
{background updates only}
{foreground updates only}
{all updates}

{true RGB values}
{color usage}
{tolerance value}
{private fields}

{entries in pmTable}
{private fields}

PROCEDURE InitPalettes;
INLINE $AA90;
FUNCTION NewPalette(entries: INTEGER;srcColors: CTabHandle;srcUsage: INTEGER;
srcTolerance: INTEGER): PaletteHandle;
INLINE $AA91;
FUNCTION GetNewPalette(PaletteID: INTEGER): PaletteHandle;
INLINE $AA92;
PROCEDURE DisposePalette(srcPalette: PaletteHandle);
INLINE $AA93;
PROCEDURE ActivatePalette(srcWindow: WindowPtr);
INLINE $AA94;
PROCEDURE SetPalette(dstWindow: WindowPtr;srcPalette: PaletteHandle;cUpdates: BOOLEAN);
INLINE $AA95;
PROCEDURE NSetPalette(dstWindow: WindowPtr;srcPalette: PaletteHandle;nCUpdates: INTEGER);
INLINE $AA95;
FUNCTION GetPalette(srcWindow: WindowPtr): PaletteHandle;
INLINE $AA96;
PROCEDURE CopyPalette(srcPalette: PaletteHandle;dstPalette: PaletteHandle;
srcEntry: INTEGER;dstEntry: INTEGER;dstLength: INTEGER);
INLINE $AAA1;
PROCEDURE PmForeColor(dstEntry: INTEGER);
INLINE $AA97;
PROCEDURE PmBackColor(dstEntry: INTEGER);
INLINE $AA98;
PROCEDURE AnimateEntry(dstWindow: WindowPtr;dstEntry: INTEGER;srcRGB: RGBColor);
INLINE $AA99;
PROCEDURE AnimatePalette(dstWindow: WindowPtr;srcCTab: CTabHandle;srcIndex: INTEGER;
dstEntry: INTEGER;dstLength: INTEGER);
INLINE $AA9A;
PROCEDURE GetEntryColor(srcPalette: PaletteHandle;srcEntry: INTEGER;VAR dstRGB: RGBColor);
INLINE $AA9B;
PROCEDURE SetEntryColor(dstPalette: PaletteHandle;dstEntry: INTEGER;srcRGB: RGBColor);
INLINE $AA9C;


PROCEDURE GetEntryUsage(srcPalette: PaletteHandle;srcEntry: INTEGER;VAR dstUsage: INTEGER;
VAR dstTolerance: INTEGER);
INLINE $AA9D;
PROCEDURE SetEntryUsage(dstPalette: PaletteHandle;dstEntry: INTEGER;srcUsage: INTEGER;
srcTolerance: INTEGER);
INLINE $AA9E;
PROCEDURE CTab2Palette(srcCTab: CTabHandle;dstPalette: PaletteHandle;srcUsage: INTEGER;
srcTolerance: INTEGER);
INLINE $AA9F;
PROCEDURE Palette2CTab(srcPalette: PaletteHandle;dstCTab: CTabHandle);
INLINE $AAA0;
FUNCTION Entry2Index(entry: INTEGER): LONGINT;
INLINE $7000,$AAA2;
PROCEDURE RestoreDeviceClut(gd: GDHandle);
INLINE $7002,$AAA2;
PROCEDURE ResizePalette(p: PaletteHandle;size: INTEGER);
INLINE $7003,$AAA2;
PROCEDURE SaveFore(VAR c: ColorSpec);
INLINE $303C,$040D,$AAA2;
PROCEDURE SaveBack(VAR c: ColorSpec);
INLINE $303C,$040E,$AAA2;
PROCEDURE RestoreFore(c: ColorSpec);
INLINE $303C,$040F,$AAA2;
PROCEDURE RestoreBack(c: ColorSpec);
INLINE $303C,$0410,$AAA2;
FUNCTION SetDepth(gd: GDHandle;depth: INTEGER;whichFlags: INTEGER;flags: INTEGER): OSErr;
INLINE $303C,$0A13,$AAA2;
FUNCTION HasDepth(gd: GDHandle;depth: INTEGER;whichFlags: INTEGER;flags: INTEGER): INTEGER;
INLINE $303C,$0A14,$AAA2;
FUNCTION PMgrVersion: INTEGER;
INLINE $7015,$AAA2;
PROCEDURE SetPaletteUpdates(p: PaletteHandle;updates: INTEGER);
INLINE $303C,$0616,$AAA2;
FUNCTION GetPaletteUpdates(p: PaletteHandle): INTEGER;
INLINE $303C,$0417,$AAA2;
FUNCTION GetGray(device: GDHandle;backGround: RGBColor;VAR foreGround: RGBColor): BOOLEAN;
INLINE $303C,$0C19,$AAA2;

{$ENDC} { UsingPalettes }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Palettes.p}



{#####################################################################}
{### FILE: PasLibIntf.p}
{#####################################################################}

{
Created: Monday, January 22, 1990 at 9:18 PM
PasLibIntf.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1986-1991

Interface to the Pascal I/O and Memory Manager Library.
Built-in procedure and function declarations are marked with
the ( *  comment characters * )
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT PASLIBIntf;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPASLIBINTF}
{$SETC UsingPASLIBINTF := 1}
{$I+}
{$SETC PASLIBINTFIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := PASLIBINTFIncludes}
TYPE
PASCALPOINTER = ^INTEGER; { Universal POINTER type }
PASCALFILE = FILE; { Universal FILE type }
(*
*
*
*)

PASCALBLOCK =
{ Universal block of chars }
PACKED ARRAY [0..511] OF CHAR;

CONST
{ <StdIO.h> PLSetVBuf styles }
_IOFBF = $00; { File buffering }
_IOLBF = $40; { Line buffering }
_IONBF = $04; { No buffering }



{
Mac Pascal heap management
}
PROCEDURE PLHeapInit(sizepheap: LONGINT; heapDelta: LONGINT;
memerrProc: UNIV PASCALPOINTER; allowNonCont: BOOLEAN;
forDispose: BOOLEAN);
{
The following procedure is obsolete, use PLHeapInit
}
PROCEDURE PLInitHeap(sizepheap: LONGINT; memerrProc: UNIV PASCALPOINTER;
allowNonCont: BOOLEAN; allowDispose: BOOLEAN);
PROCEDURE PLSetNonCont(allowNonCont: BOOLEAN);
PROCEDURE PLSetMErrProc(memerrProc: UNIV PASCALPOINTER);
PROCEDURE PLSetHeapType(forDispose: BOOLEAN);
PROCEDURE PLSetHeapCheck(DoIt: BOOLEAN);
{
File I/O
}
(*
*
*
*
*
*
*
*
*
*
*
*
*)

PROCEDURE RESET(VAR fvar: UNIV PASCALFILE; OPT fname: STRING);
BUILTIN;
PROCEDURE REWRITE(VAR fvar: UNIV PASCALFILE; OPT fname: STRING);
BUILTIN;
PROCEDURE OPEN(VAR fvar: UNIV PASCALFILE; fname: STRING);
BUILTIN;
PROCEDURE PLSetVBuf(VAR fvar: TEXT; buffer: UNIV PASCALPOINTER;
style: INTEGER; bufsize: INTEGER);
(*
*
*
*
*
*
*
*
*
*
*
*
*
*)

FUNCTION
BLOCKREAD(
VAR fvar: FILE;
VAR buffer: UNIV PASCALBLOCK;
nBlocks: INTEGER;
OPT stBlock:INTEGER
):
INTEGER;
BUILTIN;
FUNCTION
BLOCKWRITE(
VAR fvar: FILE;
VAR buffer: UNIV PASCALBLOCK;
nBlocks: INTEGER;
OPT stBlock:INTEGER
):
INTEGER;
BUILTIN;
FUNCTION
BYTEREAD(
VAR fvar: FILE;
VAR buffer: UNIV PASCALBLOCK;
nBytes: LONGINT;
OPT stByte: LONGINT
):
LONGINT;
BUILTIN;
FUNCTION
BYTEWRITE(
VAR fvar: FILE;
VAR buffer: UNIV PASCALBLOCK;
nBytes: LONGINT;
OPT stByte: LONGINT
):
LONGINT;
BUILTIN;
FUNCTION
EOF(OPT VAR fvar: UNIV PASCALFILE):
BOOLEAN;
BUILTIN;
FUNCTION
EOLN(OPT VAR fvar: TEXT):
BOOLEAN;
BUILTIN;
PROCEDURE
READ(VAR fvar: TEXT; OPT EXPR_LIST);
BUILTIN;
PROCEDURE
READLN(OPT VAR fvar: TEXT; OPT EXPR_LIST);
BUILTIN;
PROCEDURE
WRITE(VAR fvar: TEXT; OPT EXPR_LIST);
BUILTIN;
PROCEDURE
WRITELN(OPT VAR fvar: TEXT; OPT EXPR_LIST);
BUILTIN;
PROCEDURE
GET(VAR fvar: UNIV PASCALFILE);
BUILTIN;

PROCEDURE
PUT(VAR fvar: UNIV PASCALFILE);
BUILTIN;
PROCEDURE
SEEK(VAR fvar: UNIV PASCALFILE; recno: LONGINT);
BUILTIN;

FUNCTION PLFilePos(VAR fvar: UNIV PASCALFILE): LONGINT;
PROCEDURE PLFlush(VAR fvar: TEXT);
PROCEDURE PLCrunch(VAR fvar: UNIV PASCALFILE);
{
Directory operations.
}
PROCEDURE PLPurge(fname: STRING);
PROCEDURE PLRename(oldFname, newFname: STRING);
{
C string functions for Pascal strings
}
FUNCTION PLStrCmp(string1, string2: STR255): INTEGER;
FUNCTION PLStrnCmp(string1, string2: STR255; n: INTEGER): INTEGER;
FUNCTION PLStrCpy(VAR string1: STR255; string2: STR255): STRINGPTR;
FUNCTION PLStrnCpy(VAR string1: STR255; string2: STR255; n: INTEGER): STRINGPTR;
FUNCTION PLStrCat(VAR string1: STR255; string2: STR255): STRINGPTR;
FUNCTION PLStrnCat(VAR string1: STR255; string2: STR255; n: INTEGER): STRINGPTR;
FUNCTION PLStrChr(string1: STR255; c: CHAR): PTR;
FUNCTION PLStrrChr(string1: STR255; c: CHAR): PTR;
FUNCTION PLStrPBrk(string1, string2: STR255): PTR;
FUNCTION PLStrSpn(string1, string2: STR255): INTEGER;
FUNCTION PLStrStr(string1, string2: STR255): PTR;
FUNCTION PLStrLen(string1: STR255): INTEGER;
FUNCTION PLPos(STRING1: STR255; STRING2: STR255): INTEGER;
{$ENDC}

{ UsingPASLIBINTF }

{$IFC NOT UsingIncludes}
END.



{$ENDC}
{### END OF FILE PasLibIntf.p}





{#####################################################################}
{### FILE: Perf.p}
{#####################################################################}
{
Created: Monday, January 22, 1990 at 9:18 PM
Perf.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1986-1989

DESCRIPTION
Provides for PC-sampling of User code resources, ROM code, and RAM (misses).
Produces output text file suitable for input to PerformReport.
Design objectives:
Language independent, i.e. works with Pascal, C, and Assembly.
Covers user resources as well as ROM code.
Memory model independent, i.e. works for Desk Accessories and drivers.
Uses TimeManager on new ROMs, Vertical Blanking interrupt on 64 K ROMs.
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Perf;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPerf}
{$SETC UsingPerf := 1}
{$I+}
{$SETC PerfIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := PerfIncludes}
TYPE
PLongs = ^ALongs;
ALongs = ARRAY [1..8000] OF LONGINT;
PInts = ^AInts;
HInts = ^PInts;
AInts = ARRAY [1..8000] OF INTEGER;



{ PerfGlobals are declared as a record, so main program can allocate
as globals, desk accessory can add to globals allocated via pointer,
print driver can allocate via low memory, etc. }

TP2PerfGlobals = ^TPerfGlobals;
TPerfGlobals = RECORD
startROM: LONGINT;
{ROM Base}
romHits: LONGINT;
{used if MeasureROM is false}
misses: LONGINT;
{count of PC values outside measured memory}
segArray: PLongs;
{array of segment handles}
sizeArray: PLongs;
{array of segment sizes}
idArray: HInts;
{array of segment rsrc IDs}
baseArray: PLongs;
{array of offsets to counters for each segment}
samples: PLongs;
{samples buffer}
buffSize: LONGINT;
{size of samples buffer in bytes}
timeInterval: INTEGER;
{number of clock intervals between interrupts}
bucketSize: INTEGER;
{size of buckets power of 2}
log2buckSize: INTEGER;
{used in CvtPC}
pcOffset: INTEGER;
{offset to the user PC at interrupt time.}
numMeasure: INTEGER;
{# Code segments (w/o jump table)- ROM etc.}
firstCode: INTEGER;
{index of first Code segment}
takingSamples: BOOLEAN;
{true if sampling is enabled}
measureROM: BOOLEAN;
measureCode: BOOLEAN;
ramSeg: INTEGER;
{index of "segment" record to cover RAM > 0 if RAM (misses) are to be bucketed.}
ramBase: LONGINT;
{beginning of RAM being measured.}
measureRAMbucketSize: INTEGER;
measureRAMlog2buckSize: INTEGER;
romVersion: INTEGER;
vRefNum: INTEGER;
{Volume where the report file is to be created}
volumeSelected: BOOLEAN;
{True if user selects the report file name}
rptFileName: Str255;
{Report file name}
rptFileCreator: Str255;
{Report File Creator}
rptFileType: Str255;
{Report File type}
getResType: ResType;
{Resource type}
END;

FUNCTION InitPerf(VAR thePerfGlobals: TP2PerfGlobals;timerCount: INTEGER;
codeAndROMBucketSize: INTEGER;doROM: BOOLEAN;doAppCode: BOOLEAN;appCodeType: Str255;
romID: INTEGER;romName: Str255;doRAM: BOOLEAN;ramLow: LONGINT;ramHigh: LONGINT;
ramBucketSize: INTEGER): BOOLEAN;
{ called once to setup Performance monitoring
}
PROCEDURE TermPerf(thePerfGlobals: TP2PerfGlobals);
{ if InitPerf succeeds then TermPerf must be called before terminating program.
}
FUNCTION PerfControl(thePerfGlobals: TP2PerfGlobals;turnOn: BOOLEAN): BOOLEAN;
{ Call this to turn off/on measuring.
Returns previous state.
}

FUNCTION PerfDump(thePerfGlobals: TP2PerfGlobals;reportFile: Str255;doHistogram: BOOLEAN;
rptFileColumns: INTEGER): INTEGER;
{ Call this to dump the statistics into a file. }

{$ENDC}

{ UsingPerf }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Perf.p}



{#####################################################################}
{### FILE: Picker.p}
{#####################################################################}

{
Created: Monday, September 16, 1991 at 12:02 AM
Picker.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1987-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Picker;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPicker}
{$SETC UsingPicker := 1}
{$I+}
{$SETC PickerIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := PickerIncludes}
CONST
MaxSmallFract = $0000FFFF;

{Maximum small fract value, as long}

TYPE
{ A SmallFract value is just the fractional part of a Fixed number,
which is the low order word. SmallFracts are used to save room,
and to be compatible with Quickdraw's RGBColor. They can be
assigned directly to and from INTEGERs. }
SmallFract = INTEGER;

{ Unsigned fraction between 0 and 1 }

{ For developmental simplicity in switching between the HLS and HSV
models, HLS is reordered into HSL. Thus both models start with
hue and saturation values; value/lightness/brightness is last. }
HSVColor = RECORD
hue: SmallFract;
saturation: SmallFract;
value: SmallFract;
END;

HSLColor = RECORD
hue: SmallFract;         {Fraction of circle, red at 0}
saturation: SmallFract;  {0-1, 0 for gray, 1 for pure color}
lightness: SmallFract;   {0-1, 0 for black, 1 for max intensity}
END;

{Fraction of circle, red at 0}
{0-1, 0 for gray, 1 for pure color}
{0-1, 0 for black, 1 for white}

CMYColor = RECORD
cyan: SmallFract;
magenta: SmallFract;
yellow: SmallFract;
END;

FUNCTION Fix2SmallFract(f: Fixed): SmallFract;
INLINE $3F3C,$0001,$A82E;
FUNCTION SmallFract2Fix(s: SmallFract): Fixed;
INLINE $3F3C,$0002,$A82E;
PROCEDURE CMY2RGB(cColor: CMYColor;VAR rColor: RGBColor);
INLINE $3F3C,$0003,$A82E;
PROCEDURE RGB2CMY(rColor: RGBColor;VAR cColor: CMYColor);
INLINE $3F3C,$0004,$A82E;
PROCEDURE HSL2RGB(hColor: HSLColor;VAR rColor: RGBColor);
INLINE $3F3C,$0005,$A82E;
PROCEDURE RGB2HSL(rColor: RGBColor;VAR hColor: HSLColor);
INLINE $3F3C,$0006,$A82E;
PROCEDURE HSV2RGB(hColor: HSVColor;VAR rColor: RGBColor);
INLINE $3F3C,$0007,$A82E;
PROCEDURE RGB2HSV(rColor: RGBColor;VAR hColor: HSVColor);
INLINE $3F3C,$0008,$A82E;
FUNCTION GetColor(where: Point;prompt: Str255;inColor: RGBColor;VAR outColor: RGBColor): BOOLEAN;
INLINE $3F3C,$0009,$A82E;

{$ENDC} { UsingPicker }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Picker.p}

{#####################################################################}
{### FILE: PickerIntf.p}
{#####################################################################}
{
File: PickerIntf.p
As of MPW 3.0, interface files were reorganized to more closely
match "Inside Macintosh" reference books and be more consistant
from language to language.
Interfaces for the Color Picker are now found in Picker.p.
This file, which includes Picker.p, is provided for compatibility
with old sources.
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988
All Rights Reserved
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT PickerIntf;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPickerIntf}
{$SETC UsingPickerIntf := 1}
{$I+}
{$SETC PickerIntfIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingPicker}
{$I $$Shell(PInterfaces)Picker.p}
{$ENDC}
{$SETC UsingIncludes := PickerIntfIncludes}
{$ENDC}

{ UsingPickerIntf }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE PickerIntf.p}



{#####################################################################}
{### FILE: PictUtil.p}
{#####################################################################}

{
Created: Monday, September 16, 1991 at 12:03 AM
PictUtil.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1990-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT PictUtil;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPictUtil}
{$SETC UsingPictUtil := 1}
{$I+}
{$SETC PictUtilIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingPalettes}
{$I $$Shell(PInterfaces)Palettes.p}
{$ENDC}
{$SETC UsingIncludes := PictUtilIncludes}
CONST
{ verbs for the GetPictInfo, GetPixMapInfo, and NewPictInfo calls }
returnColorTable = 1;
returnPalette = 2;
recordComments = 4;
recordFontInfo = 8;
suppressBlackAndWhite = 16;
{ color pick methods }
systemMethod = 0;
popularMethod = 1;
medianMethod = 2;
{ color bank types }
ColorBankIsCustom = -1;
ColorBankIsExactAnd555 = 0;

{system color pick method}
{method that chooses the most popular set of colors}
{method that chooses a good average mix of colors}



ColorBankIs555 = 1;
TYPE
PictInfoID = LONGINT;
CommentSpecPtr = ^CommentSpec;
CommentSpecHandle = ^CommentSpecPtr;
CommentSpec = RECORD
count: INTEGER;
{ number of occurrances of this comment ID }
ID: INTEGER;
{ ID for the comment in the picture }
END;
FontSpecPtr = ^FontSpec;
FontSpecHandle = ^FontSpecPtr;
FontSpec = RECORD
pictFontID: INTEGER;
sysFontID: INTEGER;
size: ARRAY [0..3] OF LONGINT;
style: INTEGER;
nameOffset: LONGINT;
END;
PictInfoPtr = ^PictInfo;
PictInfoHandle = ^PictInfoPtr;
PictInfo = RECORD
version: INTEGER;
uniqueColors: LONGINT;
thePalette: PaletteHandle;
theColorTable: CTabHandle;
hRes: Fixed;
vRes: Fixed;
depth: INTEGER;
sourceRect: Rect;
textCount: LONGINT;
lineCount: LONGINT;
rectCount: LONGINT;
rRectCount: LONGINT;
ovalCount: LONGINT;
arcCount: LONGINT;
polyCount: LONGINT;
regionCount: LONGINT;
bitMapCount: LONGINT;
pixMapCount: LONGINT;
commentCount: LONGINT;
uniqueComments: LONGINT;
commentHandle: CommentSpecHandle;
uniqueFonts: LONGINT;
fontHandle: FontSpecHandle;
fontNamesHandle: Handle;
reserved1: LONGINT;
reserved2: LONGINT;
END;

{
{
{
{
{

ID of the font in the picture }
ID of the same font in the current system file }
bit array of all the sizes found (1..127) (bit 0 means > 127) }
combined style of all occurrances of the font }
offset into the fontNamesHdl handle for the font’s name }

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

this is always zero, for now }
the number of actual colors in the picture(s)/pixmap(s) }
handle to the palette information }
handle to the color table }
maximum horizontal resolution for all the pixmaps }
maximum vertical resolution for all the pixmaps }
maximum depth for all the pixmaps (in the picture) }
the picture frame rectangle (this contains the entire picture) }
total number of text strings in the picture }
total number of lines in the picture }
total number of rectangles in the picture }
total number of round rectangles in the picture }
total number of ovals in the picture }
total number of arcs in the picture }
total number of polygons in the picture }
total number of regions in the picture }
total number of bitmaps in the picture }
total number of pixmaps in the picture }
total number of comments in the picture }
the number of unique comments in the picture }
handle to all the comment information }
the number of unique fonts in the picture }
handle to the FontSpec information }
handle to the font names }

FUNCTION GetPictInfo(thePictHandle: PicHandle;
VAR thePictInfo: PictInfo;



verb: INTEGER;
colorsRequested: INTEGER;
colorPickMethod: INTEGER;
version: INTEGER): OSErr;
INLINE $303C,$0800,$A831;
FUNCTION GetPixMapInfo(thePixMapHandle: PixMapHandle;
VAR thePictInfo: PictInfo;
verb: INTEGER;
colorsRequested: INTEGER;
colorPickMethod: INTEGER;
version: INTEGER): OSErr;
INLINE $303C,$0801,$A831;
FUNCTION NewPictInfo(VAR thePictInfoID: PictInfoID;
verb: INTEGER;
colorsRequested: INTEGER;
colorPickMethod: INTEGER;
version: INTEGER): OSErr;
INLINE $303C,$0602,$A831;
FUNCTION RecordPictInfo(thePictInfoID: PictInfoID;
thePictHandle: PicHandle): OSErr;
INLINE $303C,$0403,$A831;
FUNCTION RecordPixMapInfo(thePictInfoID: PictInfoID;
thePixMapHandle: PixMapHandle): OSErr;
INLINE $303C,$0404,$A831;
FUNCTION RetrievePictInfo(thePictInfoID: PictInfoID;
VAR thePictInfo: PictInfo;
colorsRequested: INTEGER): OSErr;
INLINE $303C,$0505,$A831;
FUNCTION DisposPictInfo(thePictInfoID: PictInfoID): OSErr;
INLINE $303C,$0206,$A831;

{$ENDC} { UsingPictUtil }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE PictUtil.p}



{#####################################################################}
{### FILE: Power.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:54 PM
Power.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Power;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPower}
{$SETC UsingPower := 1}
{$I+}
{$SETC PowerIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := PowerIncludes}
CONST
{ Bit positions for ModemByte }
modemOnBit = 0;
ringWakeUpBit = 2;
modemInstalledBit = 3;
ringDetectBit = 4;
modemOnHookBit = 5;
{ masks for ModemByte }
modemOnMask = $1;
ringWakeUpMask = $4;
modemInstalledMask = $8;
ringDetectMask = $10;
modemOnHookMask = $20;
{ bit positions for BatteryByte }
chargerConnBit = 0;
hiChargeBit = 1;
chargeOverFlowBit = 2;
batteryDeadBit = 3;

batteryLowBit = 4;
connChangedBit = 5;
{ masks for BatteryByte }
chargerConnMask = $1;
hiChargeMask = $2;
chargeOverFlowMask = $4;
batteryDeadMask = $8;
batteryLowMask = $10;
connChangedMask = $20;
{ commands to SleepQRec sleepQProc }
sleepRequest = 1;
sleepDemand = 2;
sleepWakeUp = 3;
sleepRevoke = 4;
{ SleepQRec.sleepQFlags }
noCalls = 1;
noRequest = 2;
slpQType = 16;
sleepQType = 16;
TYPE
ModemByte = Byte;
BatteryByte = Byte;
PMResultCode = LONGINT;
SleepQRecPtr = ^SleepQRec;
SleepQRec = RECORD
sleepQLink: SleepQRecPtr;
sleepQType: INTEGER;
{type = 16}
sleepQProc: ProcPtr;
{Pointer to sleep routine}
sleepQFlags: INTEGER;
END;

FUNCTION DisableWUTime: OSErr;
FUNCTION GetWUTime(VAR WUTime: LONGINT;VAR WUFlag: Byte): OSErr;
FUNCTION SetWUTime(WUTime: LONGINT): OSErr;
FUNCTION BatteryStatus(VAR Status: Byte;VAR Power: Byte): OSErr;
FUNCTION ModemStatus(VAR Status: Byte): OSErr;
FUNCTION IdleUpdate: LONGINT;
INLINE $A285,$2E80;
FUNCTION GetCPUSpeed: LONGINT;
INLINE $70FF,$A485,$2E80;
PROCEDURE EnableIdle;
INLINE $7000,$A485;
PROCEDURE DisableIdle;
INLINE $7001,$A485;
PROCEDURE SleepQInstall(qRecPtr: SleepQRecPtr);
INLINE $205F,$A28A;
PROCEDURE SleepQRemove(qRecPtr: SleepQRecPtr);
INLINE $205F,$A48A;




PROCEDURE AOn;
INLINE $7004,$A685;
PROCEDURE AOnIgnoreModem;
INLINE $7005,$A685;
PROCEDURE BOn;
INLINE $7000,$A685;
PROCEDURE AOff;
INLINE $7084,$A685;
PROCEDURE BOff;
INLINE $7080,$A685;

{$ENDC}

{ UsingPower }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Power.p}



{#####################################################################}
{### FILE: PPCToolBox.p}
{#####################################################################}

{
Created: Thursday, September 5, 1991 at 5:57 PM
PPCToolBox.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1989-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT PPCToolBox;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPPCToolBox}
{$SETC UsingPPCToolBox := 1}
{$I+}
{$SETC PPCToolBoxIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingAppleTalk}
{$I $$Shell(PInterfaces)AppleTalk.p}
{$ENDC}
{$IFC UNDEFINED UsingMemory}
{$I $$Shell(PInterfaces)Memory.p}
{$ENDC}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := PPCToolBoxIncludes}
CONST
{The following is temporarily placed here, later it will be moved to GestaltEqu}
gestaltPPCSupportsStoreAndForward = $2000;
gestaltPPCVersionAttr = 'ppcv';
TYPE
PPCServiceType = SignedByte;
CONST
ppcServiceRealTime = 1;
ppcServiceStoreAndForward = 2;
TYPE



PPCLocationKind = INTEGER;
CONST
ppcNoLocation = 0;
ppcNBPLocation = 1;
ppcNBPTypeLocation = 2;

{ There is no PPCLocName }
{ Use AppleTalk NBP
}
{ Used for specifying a location name type during PPCOpen only }

TYPE
PPCPortKinds = INTEGER;
CONST
ppcByCreatorAndType = 1;
ppcByString = 2;

{ Port type is specified as colloquial Mac creator and type }
{ Port type is in pascal string format }

TYPE
PPCSessionOrigin = SignedByte;

{ Values returned for request field in PPCInform call }

CONST
{ Values returned for requestType field in PPCInform call }
ppcLocalOrigin = 1;
{ session originated from this machine }
ppcRemoteOrigin = 2;
{ session originated from remote machine }
TYPE
PPCPortRefNum = INTEGER;
PPCSessRefNum = LONGINT;
PPCPortPtr = ^PPCPortRec;
PPCPortRec = RECORD
nameScript: ScriptCode;
name: Str32;
portKindSelector: PPCPortKinds;
CASE PPCPortKinds OF
ppcByString:
(portTypeStr: Str32);
ppcByCreatorAndType:
(portCreator: OSType;
portType: OSType);
END;
LocationNamePtr = ^LocationNameRec;
LocationNameRec = RECORD
locationKindSelector: PPCLocationKind;
CASE PPCLocationKind OF
ppcNBPLocation:
(nbpEntity: EntityName);
ppcNBPTypeLocation:
(nbpType: Str32);
END;
PortInfoPtr = ^PortInfoRec;
PortInfoRec = RECORD
filler1: SignedByte;
authRequired: BOOLEAN;
name: PPCPortRec;
END;

{ script of name }
{ name of port as seen in browser }
{ which variant }

{ which variant }

{ NBP name entity }
{ just the NBP type string, for PPCOpen }



PortInfoArrayPtr = ^PortInfoArray;
PortInfoArray = ARRAY [0..0] OF PortInfoRec;
{ Procedures you will need to write }
PPCFilterProcPtr = ProcPtr;
{ FUNCTION MyPortFilter(locationName: LocationNameRec; thePortInfo: PortInfoRec): BOOLEAN; }
PPCCompProcPtr = ProcPtr;
{ PROCEDURE MyCompletionRoutine(pb: PPCParamBlockPtr); }
PPCOpenPBPtr = ^PPCOpenPBRec;
PPCOpenPBRec = RECORD
qLink:
Ptr;
csCode:
INTEGER;
intUse:
INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;
portRefNum: PPCPortRefNum;
filler1: LONGINT;
serviceType: PPCServiceType;
resFlag: SignedByte;
portName: PPCPortPtr;
locationName: LocationNamePtr;
networkVisible: BOOLEAN;
nbpRegistered: BOOLEAN;
END;
PPCInformPBPtr = ^PPCInformPBRec;
PPCInformPBRec = RECORD
qLink: Ptr;
csCode: INTEGER;
intUse: INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;
portRefNum: PPCPortRefNum;
sessRefNum: PPCSessRefNum;
serviceType: PPCServiceType;
autoAccept: BOOLEAN;
portName: PPCPortPtr;
locationName: LocationNamePtr;
userName: StringPtr;
userData: LONGINT;
requestType: PPCSessionOrigin;
END;
PPCStartPBPtr = ^PPCStartPBRec;
PPCStartPBRec = RECORD
qLink: Ptr;
csCode: INTEGER;
intUse: INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;

{
{
{
{

reserved
reserved
reserved
reserved

}
}
}
}

{ reserved }
{ 38 <-Port Reference
-->
-->
-->
-->
-->
<--

}

{
{
{
{
{
{

44
45
46
50
54
55

Bit field describing the requested port service }
Must be set to 0
}
PortName for PPC
}
If NBP Registration is required
}
make this network visible on network
}
The given location name was registered on the network }

{
{
{
{

reserved
reserved
reserved
reserved

}
}
}
}

{
{
{
{
{
{
{
{
{
{

reserved
38 -->
40 <-44 <-45 -->
46 -->
50 -->
54 -->
58 <-62 <--

}
Port Identifier }
Session Reference }
Status Flags for type of session, local, remote }
if true session will be accepted automatically }
Buffer for Source PPCPortRec }
Buffer for Source LocationNameRec }
Buffer for Soure user's name trying to link. }
value included in PPCStart's userData }
Local or Network }

{
{
{
{

reserved
reserved
reserved
reserved

}
}
}
}

{ reserved }

portRefNum: PPCPortRefNum;
sessRefNum: PPCSessRefNum;
serviceType: PPCServiceType;
resFlag: SignedByte;
portName: PPCPortPtr;
locationName: LocationNamePtr;
rejectInfo: LONGINT;
userData: LONGINT;
userRefNum: LONGINT;
END;
PPCAcceptPBPtr = ^PPCAcceptPBRec;
PPCAcceptPBRec = RECORD
qLink: Ptr;
csCode: INTEGER;
intUse: INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;
filler1: INTEGER;
sessRefNum: PPCSessRefNum;
END;
PPCRejectPBPtr = ^PPCRejectPBRec;
PPCRejectPBRec = RECORD
qLink: Ptr;
csCode: INTEGER;
intUse: INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;
filler1: INTEGER;
sessRefNum: PPCSessRefNum;
filler2: INTEGER;
filler3: LONGINT;
filler4: LONGINT;
rejectInfo: LONGINT;
END;
PPCWritePBPtr = ^PPCWritePBRec;
PPCWritePBRec = RECORD
qLink: Ptr;
csCode: INTEGER;
intUse: INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;
filler1: INTEGER;
sessRefNum: PPCSessRefNum;
bufferLength: Size;
actualLength: Size;
bufferPtr: Ptr;
more: BOOLEAN;
filler2: SignedByte;
userData: LONGINT;
blockCreator: OSType;
blockType: OSType;
END;
PPCReadPBPtr = ^PPCReadPBRec;
PPCReadPBRec = RECORD
qLink: Ptr;
csCode: INTEGER;
intUse: INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;
filler1: INTEGER;
sessRefNum: PPCSessRefNum;
bufferLength: Size;
actualLength: Size;
bufferPtr: Ptr;
more: BOOLEAN;
filler2: SignedByte;
userData: LONGINT;
blockCreator: OSType;
blockType: OSType;
END;
PPCEndPBPtr = ^PPCEndPBRec;
PPCEndPBRec = RECORD
qLink: Ptr;
csCode: INTEGER;
intUse: INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;
filler1: INTEGER;
sessRefNum: PPCSessRefNum;
END;
PPCClosePBPtr = ^PPCClosePBRec;
PPCClosePBRec = RECORD
qLink: Ptr;
csCode: INTEGER;
intUse: INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;
portRefNum: PPCPortRefNum;
END;
IPCListPortsPBPtr = ^IPCListPortsPBRec;
IPCListPortsPBRec = RECORD
qLink: Ptr;
csCode: INTEGER;


intUse: INTEGER;
intUsePtr: Ptr;
ioCompletion: PPCCompProcPtr;
ioResult: OSErr;
reserved: ARRAY [1..5] OF LONGINT;
filler1: INTEGER;
startIndex: INTEGER;
requestCount: INTEGER;
actualCount: INTEGER;
portName: PPCPortPtr;
locationName: LocationNamePtr;
bufferPtr: PortInfoArrayPtr;
END;


PPCParamBlockPtr = ^PPCParamBlockRec;
PPCParamBlockRec = RECORD
CASE Integer OF
0: (openParam:
PPCOpenPBRec);
1: (informParam: PPCInformPBRec);
2: (startParam: PPCStartPBRec);
3: (acceptParam: PPCAcceptPBRec);
4: (rejectParam: PPCRejectPBRec);
5: (writeParam: PPCWritePBRec);
6: (readParam:
PPCReadPBRec);
7: (endParam: PPCEndPBRec);
8: (closeParam: PPCClosePBRec);
9: (listPortsParam: IPCListPortsPBRec);
END;

{ PPC Calling Conventions }
FUNCTION PPCInit: OSErr;
INLINE $7000,$A0DD,$3E80;
FUNCTION PPCOpen(pb: PPCOpenPBPtr;async: BOOLEAN): OSErr;
FUNCTION PPCOpenSync(pb: PPCOpenPBPtr): OSErr;
INLINE $205F,$7001,$A0DD,$3E80;
FUNCTION PPCOpenAsync(pb: PPCOpenPBPtr): OSErr;
INLINE $205F,$7001,$A4DD,$3E80;
FUNCTION PPCInform(pb: PPCInformPBPtr;async: BOOLEAN): OSErr;
FUNCTION PPCInformSync(pb: PPCInformPBPtr): OSErr;
INLINE $205F,$7003,$A0DD,$3E80;
FUNCTION PPCInformAsync(pb: PPCInformPBPtr): OSErr;
INLINE $205F,$7003,$A4DD,$3E80;
FUNCTION PPCStart(pb: PPCStartPBPtr;async: BOOLEAN): OSErr;
FUNCTION PPCStartSync(pb: PPCStartPBPtr): OSErr;
INLINE $205F,$7002,$A0DD,$3E80;
FUNCTION PPCStartAsync(pb: PPCStartPBPtr): OSErr;
INLINE $205F,$7002,$A4DD,$3E80;
FUNCTION PPCAccept(pb: PPCAcceptPBPtr;async: BOOLEAN): OSErr;
FUNCTION PPCAcceptSync(pb: PPCAcceptPBPtr): OSErr;
INLINE $205F,$7004,$A0DD,$3E80;
FUNCTION PPCAcceptAsync(pb: PPCAcceptPBPtr): OSErr;
INLINE $205F,$7004,$A4DD,$3E80;
FUNCTION PPCReject(pb: PPCRejectPBPtr;async: BOOLEAN): OSErr;
FUNCTION PPCRejectSync(pb: PPCRejectPBPtr): OSErr;


INLINE $205F,$7005,$A0DD,$3E80;
FUNCTION PPCRejectAsync(pb: PPCRejectPBPtr): OSErr;
INLINE $205F,$7005,$A4DD,$3E80;
FUNCTION PPCWrite(pb: PPCWritePBPtr;async: BOOLEAN): OSErr;
FUNCTION PPCWriteSync(pb: PPCWritePBPtr): OSErr;
INLINE $205F,$7006,$A0DD,$3E80;
FUNCTION PPCWriteAsync(pb: PPCWritePBPtr): OSErr;
INLINE $205F,$7006,$A4DD,$3E80;
FUNCTION PPCRead(pb: PPCReadPBPtr;async: BOOLEAN): OSErr;
FUNCTION PPCReadSync(pb: PPCReadPBPtr): OSErr;
INLINE $205F,$7007,$A0DD,$3E80;
FUNCTION PPCReadAsync(pb: PPCReadPBPtr): OSErr;
INLINE $205F,$7007,$A4DD,$3E80;
FUNCTION PPCEnd(pb: PPCEndPBPtr;async: BOOLEAN): OSErr;
FUNCTION PPCEndSync(pb: PPCEndPBPtr): OSErr;
INLINE $205F,$7008,$A0DD,$3E80;
FUNCTION PPCEndAsync(pb: PPCEndPBPtr): OSErr;
INLINE $205F,$7008,$A4DD,$3E80;
FUNCTION PPCClose(pb: PPCClosePBPtr;async: BOOLEAN): OSErr;
FUNCTION PPCCloseSync(pb: PPCClosePBPtr): OSErr;
INLINE $205F,$7009,$A0DD,$3E80;
FUNCTION PPCCloseAsync(pb: PPCClosePBPtr): OSErr;
INLINE $205F,$7009,$A4DD,$3E80;
FUNCTION IPCListPorts(pb: IPCListPortsPBPtr;async: BOOLEAN): OSErr;
FUNCTION IPCListPortsSync(pb: IPCListPortsPBPtr): OSErr;
INLINE $205F,$700A,$A0DD,$3E80;
FUNCTION IPCListPortsAsync(pb: IPCListPortsPBPtr): OSErr;
INLINE $205F,$700A,$A4DD,$3E80;
FUNCTION PPCKill(pb: PPCParamBlockPtr): OSErr;
INLINE $205F,$700B,$A0DD,$3E80;
FUNCTION DeleteUserIdentity(userRef: LONGINT): OSErr;
FUNCTION GetDefaultUser(VAR userRef: LONGINT;
VAR userName: Str32): OSErr;
FUNCTION StartSecureSession(pb: PPCStartPBPtr;
VAR userName: Str32;
useDefault: BOOLEAN;
allowGuest: BOOLEAN;
VAR guestSelected: BOOLEAN;
prompt: Str255): OSErr;
FUNCTION PPCBrowser(prompt: Str255;
applListLabel: Str255;
defaultSpecified: BOOLEAN;
VAR theLocation: LocationNameRec;
VAR thePortInfo: PortInfoRec;
portFilter: PPCFilterProcPtr;
theLocNBPType: Str32): OSErr;
INLINE $303C,$0D00,$A82B;

{$ENDC} { UsingPPCToolBox }
{$IFC NOT UsingIncludes}
END.
{$ENDC}



{### END OF FILE PPCToolBox.p}





{#####################################################################}
{### FILE: Printing.p}
{#####################################################################}

{
Created: Saturday, February 15, 1992 at 11:19 AM
Printing.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Printing;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPrinting}
{$SETC UsingPrinting := 1}
{$I+}
{$SETC PrintingIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$SETC UsingIncludes := PrintingIncludes}
CONST
iPFMaxPgs = 128;
iPrPgFract = 120;
iPrPgFst = 1;
iPrPgMax = 9999;
iPrRelease = 3;
iPrSavPFil = -1;
iPrAbort = $0080;
iPrDevCtl = 7;
lPrReset = $00010000;
lPrLineFeed = $00030000;
lPrLFStd = $0003FFFF;
lPrLFSixth = $0003FFFF;
lPrPageEnd = $00020000;
lPrDocOpen = $00010000;
lPrPageOpen = $00040000;
lPrPageClose = $00020000;

{Page scale factor. ptPgSize (below) is in units of 1/iPrPgFract}
{Page range constants}
{Current version number of the code.}

{The PrDevCtl Proc's ctl number}
{The PrDevCtl Proc's CParam for reset}
{The PrDevCtl Proc's CParam for std paper advance}
{The PrDevCtl Proc's CParam for end page}



lPrDocClose = $00050000;
iFMgrCtl = 8;
iMscCtl = 9;
iPvtCtl = 10;
iMemFullErr = -108;
iIOAbort = -27;
pPrGlobals = $00000944;
bDraftLoop = 0;
bSpoolLoop = 1;
bUser1Loop = 2;
bUser2Loop = 3;
fNewRunBit = 2;
fHiResOK = 3;
fWeOpenedRF = 4;
{Driver constants }
iPrBitsCtl = 4;
lScreenBits = 0;
lPaintBits = 1;
lHiScreenBits = $00000002;
lHiPaintBits = $00000003;
iPrIOCtl = 5;
iPrEvtCtl = 6;
lPrEvtAll = $0002FFFD;
lPrEvtTop = $0001FFFD;
iPrDrvrRef = -3;
getRslDataOp = 4;
setRslOp = 5;
draftBitsOp = 6;
noDraftBitsOp = 7;
getRotnOp = 8;
NoSuchRsl = 1;
OpNotImpl = 2;
RgType1 = 1;

{The FMgr's Tail-hook Proc's ctl number}
{The FMgr's Tail-hook Proc's ctl number}
{The FMgr's Tail-hook Proc's ctl number}

{The PrVars lo mem area:}

{The Bitmap Print Proc's Screen Bitmap param}
{The Bitmap Print Proc's Paint [sq pix] param}
{The PrEvent Proc's ctl number}
{The PrEvent Proc's CParam for the entire screen}
{The PrEvent Proc's CParam for the top folder}

{the driver doesn't support this opcode}

TYPE
TFeed = (feedCut,feedFanfold,feedMechCut,feedOther);
TScan = (scanTB,scanBT,scanLR,scanRL);

TPRect = ^Rect;
PrIdleProcPtr = ProcPtr;
PItemProcPtr = ProcPtr;
TPPrPort = ^TPrPort;
TPrPort = RECORD
gPort: GrafPort;
gProcs: QDProcs;
lGParam1: LONGINT;
lGParam2: LONGINT;
lGParam3: LONGINT;
lGParam4: LONGINT;
fOurPtr: BOOLEAN;
fOurBits: BOOLEAN;
END;

{ A Rect Ptr }

{The Printer's graf port.}
{..and its procs}
{16 bytes for private parameter storage.}

{Whether the PrPort allocation was done by us.}
{Whether the BitMap allocation was done by us.}



{ Printing Graf Port. All printer imaging, whether spooling, banding, etc, happens "thru" a GrafPort.
This is the "PrPeek" record. }
TPPrInfo = ^TPrInfo;
TPrInfo = RECORD
iDev: INTEGER;
{Font mgr/QuickDraw device code}
iVRes: INTEGER;
{Resolution of device, in device coordinates}
iHRes: INTEGER;
{..note: V before H => compatable with Point.}
rPage: Rect;
{The page (printable) rectangle in device coordinates.}
END;
{ Print Info Record: The parameters needed for page composition. }
TPPrStl = ^TPrStl;
TPrStl = RECORD
wDev: INTEGER;
iPageV: INTEGER;
iPageH: INTEGER;
bPort: SignedByte;
feed: TFeed;
END;
TPPrXInfo = ^TPrXInfo;
TPrXInfo = RECORD
iRowBytes: INTEGER;
iBandV: INTEGER;
iBandH: INTEGER;
iDevBytes: INTEGER;
iBands: INTEGER;
bPatScale: SignedByte;
bUlThick: SignedByte;
bUlOffset: SignedByte;
bUlShadow: SignedByte;
scan: TScan;
bXInfoX: SignedByte;
END;
TPPrJob = ^TPrJob;
TPrJob = RECORD
iFstPage: INTEGER;
iLstPage: INTEGER;
iCopies: INTEGER;
bJDocLoop: SignedByte;
fFromUsr: BOOLEAN;
pIdleProc: PrIdleProcPtr;
pFileName: StringPtr;
iFileVol: INTEGER;
bFileVers: SignedByte;
bJobX: SignedByte;
END;
TPrFlag1 = PACKED RECORD
f15: BOOLEAN;
f14: BOOLEAN;
f13: BOOLEAN;
f12: BOOLEAN;
f11: BOOLEAN;

{Page Range.}
{No. copies.}
{The Doc style: Draft, Spool, .., and ..}
{Printing from an User's App (not PrApp) flag}
{The Proc called while waiting on IO etc.}
{Spool File Name: NIL for default.}
{Spool File vol, set to 0 initially}
{Spool File version, set to 0 initially}
{An eXtra byte.}



f10: BOOLEAN;
f9: BOOLEAN;
f8: BOOLEAN;
f7: BOOLEAN;
f6: BOOLEAN;
f5: BOOLEAN;
f4: BOOLEAN;
f3: BOOLEAN;
f2: BOOLEAN;
fLstPgFst: BOOLEAN;
fUserScale: BOOLEAN;
END;
{ Print Job: Print "form" for a single print request. }
TPPrint = ^TPrint;
THPrint = ^TPPrint;
TPrint = RECORD
iPrVersion: INTEGER;
{(2) Printing software version}
prInfo: TPrInfo;
{(14) the PrInfo data associated with the current style.}
rPaper: Rect;
{(8) The paper rectangle [offset from rPage]}
prStl: TPrStl;
{(8) This print request's style.}
prInfoPT: TPrInfo;
{(14) Print Time Imaging metrics}
prXInfo: TPrXInfo;
{(16) Print-time (expanded) Print info record.}
prJob: TPrJob;
{(20) The Print Job request (82) Total of the above; 120-82 = 38 bytes needed to fill 120}
CASE INTEGER OF
0:
(printX: ARRAY [1..19] OF INTEGER);
1:
(prFlag1: TPrFlag1;
{a word of flags}
iZoomMin: INTEGER;
iZoomMax: INTEGER;
hDocName: StringHandle);
{current doc's name, nil = front window}
END;
{ The universal 120 byte printing record }
TPPrStatus = ^TPrStatus;
TPrStatus = RECORD
iTotPages: INTEGER;
{Total pages in Print File.}
iCurPage: INTEGER;
{Current page number}
iTotCopies: INTEGER;
{Total copies requested}
iCurCopy: INTEGER;
{Current copy number}
iTotBands: INTEGER;
{Total bands per page.}
iCurBand: INTEGER;
{Current band number}
fPgDirty: BOOLEAN;
{True if current page has been written to.}
fImaging: BOOLEAN;
{Set while in band's DrawPic call.}
hPrint: THPrint;
{Handle to the active Printer record}
pPrPort: TPPrPort;
{Ptr to the active PrPort}
hPic: PicHandle;
{Handle to the active Picture}
END;
{ Print Status: Print information during printing. }
TPPfPgDir = ^TPfPgDir;
THPfPgDir = ^TPPfPgDir;
TPfPgDir = RECORD
iPages: INTEGER;
iPgPos: ARRAY [0..128] OF LONGINT;
{ARRAY [0..iPfMaxPgs] OF LONGINT}



END;
{ PicFile = a TPfHeader followed by n QuickDraw Pics (whose PicSize is invalid!) }
TPPrDlg = ^TPrDlg;
TPrDlg = RECORD
Dlg: DialogRecord;
{The Dialog window}
pFltrProc: ModalFilterProcPtr;
{The Filter Proc.}
pItemProc: PItemProcPtr;
{The Item evaluating proc.}
hPrintUsr: THPrint;
{The user's print record.}
fDoIt: BOOLEAN;
fDone: BOOLEAN;
lUser1: LONGINT;
{Four longs for user's to hang global data.}
lUser2: LONGINT;
{...Plus more stuff needed by the particular printing dialog.}
lUser3: LONGINT;
lUser4: LONGINT;
END;

PDlgInitProcPtr = ProcPtr;
TGnlData = RECORD
iOpCode: INTEGER;
iError: INTEGER;
lReserved: LONGINT;
END;
TRslRg = RECORD
iMin: INTEGER;
iMax: INTEGER;
END;
TRslRec = RECORD
iXRsl: INTEGER;
iYRsl: INTEGER;
END;
TGetRslBlk = RECORD
iOpCode: INTEGER;
iError: INTEGER;
lReserved: LONGINT;
iRgType: INTEGER;
xRslRg: TRslRg;
yRslRg: TRslRg;
iRslRecCnt: INTEGER;
rgRslRec: ARRAY [1..27] OF TRslRec;
END;
TSetRslBlk = RECORD
iOpCode: INTEGER;
iError: INTEGER;
lReserved: LONGINT;
hPrint: THPrint;
iXRsl: INTEGER;
iYRsl: INTEGER;
END;

{more fields here depending on call}



TDftBitsBlk = RECORD
iOpCode: INTEGER;
iError: INTEGER;
lReserved: LONGINT;
hPrint: THPrint;
END;
TGetRotnBlk = RECORD
iOpCode: INTEGER;
iError: INTEGER;
lReserved: LONGINT;
hPrint: THPrint;
fLandscape: BOOLEAN;
bXtra: SignedByte;
END;
TPBitMap = ^BitMap;

{ A BitMap Ptr }

TN = 0..15;

{ a Nibble }

TPWord = ^TWord;
THWord = ^TPWord;
TWord = PACKED RECORD
CASE INTEGER OF
0:
(c1,c0: CHAR);
1:
(b1,b0: SignedByte);
2:
(usb1,usb0: Byte);
3:
(n3,n2,n1,n0: TN);
4:
(f15,f14,f13,f12,f11,f10,f9,f8,f7,f6,f5,f4,f3,f2,f1,f0: BOOLEAN);
5:
(i0: INTEGER);
END;
TPLong = ^TLong;
THLong = ^TPLong;
TLong = RECORD
CASE INTEGER OF
0:
(w1,w0: TWord);
1:
(b1,b0: LONGINT);
2:
(p0: Ptr);
3:
(h0: Handle);
4:
(pt: Point);
END;

PROCEDURE PrPurge;
INLINE $2F3C,$A800,$0000,$A8FD;
PROCEDURE PrNoPurge;
INLINE $2F3C,$B000,$0000,$A8FD;
PROCEDURE PrOpen;
INLINE $2F3C,$C800,$0000,$A8FD;
PROCEDURE PrClose;
INLINE $2F3C,$D000,$0000,$A8FD;
PROCEDURE PrintDefault(hPrint: THPrint);
INLINE $2F3C,$2004,$0480,$A8FD;
FUNCTION PrValidate(hPrint: THPrint): BOOLEAN;
INLINE $2F3C,$5204,$0498,$A8FD;
FUNCTION PrStlDialog(hPrint: THPrint): BOOLEAN;
INLINE $2F3C,$2A04,$0484,$A8FD;
FUNCTION PrJobDialog(hPrint: THPrint): BOOLEAN;
INLINE $2F3C,$3204,$0488,$A8FD;
FUNCTION PrStlInit(hPrint: THPrint): TPPrDlg;
INLINE $2F3C,$3C04,$040C,$A8FD;
FUNCTION PrJobInit(hPrint: THPrint): TPPrDlg;
INLINE $2F3C,$4404,$0410,$A8FD;
PROCEDURE PrJobMerge(hPrintSrc: THPrint;hPrintDst: THPrint);
INLINE $2F3C,$5804,$089C,$A8FD;
FUNCTION PrDlgMain(hPrint: THPrint;pDlgInit: PDlgInitProcPtr): BOOLEAN;
INLINE $2F3C,$4A04,$0894,$A8FD;
FUNCTION PrOpenDoc(hPrint: THPrint;pPrPort: TPPrPort;pIOBuf: Ptr): TPPrPort;
INLINE $2F3C,$0400,$0C00,$A8FD;
PROCEDURE PrCloseDoc(pPrPort: TPPrPort);
INLINE $2F3C,$0800,$0484,$A8FD;
PROCEDURE PrOpenPage(pPrPort: TPPrPort;pPageFrame: TPRect);
INLINE $2F3C,$1000,$0808,$A8FD;
PROCEDURE PrClosePage(pPrPort: TPPrPort);
INLINE $2F3C,$1800,$040C,$A8FD;
PROCEDURE PrPicFile(hPrint: THPrint;pPrPort: TPPrPort;pIOBuf: Ptr;pDevBuf: Ptr;
VAR prStatus: TPrStatus);
INLINE $2F3C,$6005,$1480,$A8FD;
FUNCTION PrError: INTEGER;
INLINE $2F3C,$BA00,$0000,$A8FD;
PROCEDURE PrSetError(iErr: INTEGER);
INLINE $2F3C,$C000,$0200,$A8FD;
PROCEDURE PrGeneral(pData: Ptr);
INLINE $2F3C,$7007,$0480,$A8FD;
PROCEDURE PrDrvrOpen;
INLINE $2F3C,$8000,$0000,$A8FD;
PROCEDURE PrDrvrClose;
INLINE $2F3C,$8800,$0000,$A8FD;
PROCEDURE PrCtlCall(iWhichCtl: INTEGER;lParam1: LONGINT;lParam2: LONGINT;
lParam3: LONGINT);
INLINE $2F3C,$A000,$0E00,$A8FD;
FUNCTION PrDrvrDCE: Handle;
INLINE $2F3C,$9400,$0000,$A8FD;
FUNCTION PrDrvrVers: INTEGER;
INLINE $2F3C,$9A00,$0000,$A8FD;

{$ENDC} { UsingPrinting }
{$IFC NOT UsingIncludes}


END.
{$ENDC}

{### END OF FILE Printing.p}



{#####################################################################}
{### FILE: PrintTraps.p}
{#####################################################################}
{
Created: Monday, Sempember 9, 1991 at 1:06 PM
PrintTraps.p
Copyright Apple Computer, Inc.
All rights reserved

1985-1988

}
{This file is provided to support existing references to it. The up to date interface is
defined in Printing.p
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT PrintTraps;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingPrintTraps}
{$SETC UsingPrintTraps := 1}
{$I+}
{$SETC PrintTrapsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingPrinting}
{$I $$Shell(PInterfaces)Printing.p}
{$ENDC}
{$SETC UsingIncludes := PrintTrapsIncludes}

{$ENDC}

{ UsingPrintTraps }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE PrintTraps.p}



{#####################################################################}
{### FILE: Processes.p}
{#####################################################################}

{
Created: Monday, September 16, 1991 at 12:12 AM
Processes.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1989-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Processes;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingProcesses}
{$SETC UsingProcesses := 1}
{$I+}
{$SETC ProcessesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingEvents}
{$I $$Shell(PInterfaces)Events.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$SETC UsingIncludes := ProcessesIncludes}
TYPE
{ type for unique process identifier }
ProcessSerialNumberPtr = ^ProcessSerialNumber;
ProcessSerialNumber = RECORD
highLongOfPSN: LONGINT;
lowLongOfPSN: LONGINT;
END;

CONST
{************************************************************************
*
Process identifier.
************************************************************************


Various reserved process serial numbers. }
kNoProcess = 0;
kSystemProcess = 1;
kCurrentProcess = 2;
TYPE
{*********************************************************************************************************************************************
*
Definition of the parameter block passed to _Launch.
*************************************************************************
* Typedef and flags for launchControlFlags field }
LaunchFlags = INTEGER;
CONST
{************************************************************************
*
Definition of the parameter block passed to _Launch.
************************************************************************}
launchContinue = $4000;
launchNoFileFlags = $0800;
launchUseMinimum = $0400;
launchDontSwitch = $0200;
launchAllow24Bit = $0100;
launchInhibitDaemon = $0080;
TYPE
{ Format for first AppleEvent to pass to new process. The size of the overall
* buffer variable: the message body immediately follows the messageLength.
}
AppParametersPtr = ^AppParameters;
AppParameters = RECORD
theMsgEvent: EventRecord;
eventRefCon: LONGINT;
messageLength: LONGINT;
messageBuffer: ARRAY [0..0] OF SignedByte;
END;
{ Parameter block to _Launch }
LaunchPBPtr = ^LaunchParamBlockRec;
LaunchParamBlockRec = RECORD
reserved1: LONGINT;
reserved2: INTEGER;
launchBlockID: INTEGER;
launchEPBLength: LONGINT;
launchFileFlags: INTEGER;
launchControlFlags: LaunchFlags;
launchAppSpec: FSSpecPtr;
launchProcessSN: ProcessSerialNumber;
launchPreferredSize: LONGINT;
launchMinimumSize: LONGINT;
launchAvailableSize: LONGINT;
launchAppParameters: AppParametersPtr;
END;




CONST
{ Set launchBlockID to extendedBlock to specify that extensions exist.
* Set launchEPBLength to extendedBlockLen for compatibility.}

extendedBlock = $4C43; { 'LC' }
{ extendedBlockLen = (sizeof(LaunchParamBlockRec) - 12); //$TODO: pascal parser can't do sizeof }
{************************************************************************
* Definition of the information block returned by GetProcessInformation
************************************************************************
Bits in the processMode field }
modeDeskAccessory = $00020000;
modeMultiLaunch = $00010000;
modeNeedSuspendResume = $00004000;
modeCanBackground = $00001000;
modeDoesActivateOnFGSwitch = $00000800;
modeOnlyBackground = $00000400;
modeGetFrontClicks = $00000200;
modeGetAppDiedMsg = $00000100;
mode32BitCompatible = $00000080;
modeHighLevelEventAware = $00000040;
modeLocalAndRemoteHLEvents = $00000020;
modeStationeryAware = $00000010;
modeUseTextEditServices = $00000008;
TYPE
{ Record returned by GetProcessInformation }
ProcessInfoRecPtr = ^ProcessInfoRec;
ProcessInfoRec = RECORD
processInfoLength: LONGINT;
processName: StringPtr;
processNumber: ProcessSerialNumber;
processType: LONGINT;
processSignature: OSType;
processMode: LONGINT;
processLocation: Ptr;
processSize: LONGINT;
processFreeMem: LONGINT;
processLauncher: ProcessSerialNumber;
processLaunchDate: LONGINT;
processActiveTime: LONGINT;
processAppSpec: FSSpecPtr;
END;

FUNCTION LaunchApplication(LaunchParams:LaunchPBPtr):OSErr;
INLINE $205F,$A9F2,$3E80;
FUNCTION LaunchDeskAccessory(pFileSpec: FSSpecPtr;pDAName: StringPtr): OSErr;
INLINE $3F3C,$0036,$A88F;
FUNCTION GetCurrentProcess(VAR PSN: ProcessSerialNumber): OSErr;
INLINE $3F3C,$0037,$A88F;
FUNCTION GetFrontProcess(VAR PSN: ProcessSerialNumber): OSErr;
INLINE $70FF,$2F00,$3F3C,$0039,$A88F;
FUNCTION GetNextProcess(VAR PSN: ProcessSerialNumber): OSErr;
INLINE $3F3C,$0038,$A88F;
FUNCTION GetProcessInformation(PSN: ProcessSerialNumber;VAR info: ProcessInfoRec): OSErr;
INLINE $3F3C,$003A,$A88F;
FUNCTION SetFrontProcess(PSN: ProcessSerialNumber): OSErr;
INLINE $3F3C,$003B,$A88F;
FUNCTION WakeUpProcess(PSN: ProcessSerialNumber): OSErr;
INLINE $3F3C,$003C,$A88F;
FUNCTION SameProcess(PSN1: ProcessSerialNumber;PSN2: ProcessSerialNumber;
VAR result: BOOLEAN): OSErr;
INLINE $3F3C,$003D,$A88F;

{$ENDC} { UsingProcesses }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Processes.p}



{#####################################################################}
{### FILE: QDOffscreen.p}
{#####################################################################}

{
Created: Monday, September 16, 1991 at 12:14 AM
QDOffscreen.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT QDOffscreen;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingQDOffscreen}
{$SETC UsingQDOffscreen := 1}
{$I+}
{$SETC QDOffscreenIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := QDOffscreenIncludes}
CONST
{ New error codes }
cDepthErr = -157;
{invalid pixel depth}
pixPurgeBit = 0;
noNewDeviceBit = 1;
useTempMemBit = 2;
keepLocalBit = 3;
pixelsPurgeableBit = 6;
pixelsLockedBit = 7;
mapPixBit = 16;
newDepthBit = 17;
alignPixBit = 18;
newRowBytesBit = 19;
reallocPixBit = 20;
clipPixBit = 28;
stretchPixBit = 29;
ditherPixBit = 30;
gwFlagErrBit = 31;


TYPE
GWorldFlags = SET OF (pixPurge,noNewDevice,useTempMem,keepLocal,GWorldFlags4,
GWorldFlags5,pixelsPurgeable,pixelsLocked,GWorldFlags8,GWorldFlags9,GWorldFlags10,
GWorldFlags11,GWorldFlags12,GWorldFlags13,GWorldFlags14,GWorldFlags15,
mapPix,newDepth,alignPix,newRowBytes,reallocPix,GWorldFlags21,GWorldFlags22,
GWorldFlags23,GWorldFlags24,GWorldFlags25,GWorldFlags26,GWorldFlags27,
clipPix,stretchPix,ditherPix,gwFlagErr);

{ Type definition of a GWorldPtr }
GWorldPtr = CGrafPtr;
FUNCTION NewGWorld(VAR offscreenGWorld: GWorldPtr;PixelDepth: INTEGER;boundsRect: Rect;
cTable: CTabHandle;aGDevice: GDHandle;flags: GWorldFlags): QDErr;
INLINE $203C, $0016, $0000,$AB1D;
FUNCTION LockPixels(pm: PixMapHandle): BOOLEAN;
INLINE $203C, $0004, $0001, $AB1D;
PROCEDURE UnlockPixels(pm: PixMapHandle);
INLINE $203C, $0004, $0002, $AB1D;
FUNCTION UpdateGWorld(VAR offscreenGWorld: GWorldPtr;pixelDepth: INTEGER;
boundsRect: Rect;cTable: CTabHandle;aGDevice: GDHandle;flags: GWorldFlags): GWorldFlags;
INLINE $203C, $0016, $0003, $AB1D;
PROCEDURE DisposeGWorld(offscreenGWorld: GWorldPtr);
INLINE $203C, $0004, $0004, $AB1D;
PROCEDURE GetGWorld(VAR port: CGrafPtr;VAR gdh: GDHandle);
INLINE $203C, $0008, $0005, $AB1D;
PROCEDURE SetGWorld(port: CGrafPtr;gdh: GDHandle);
INLINE $203C, $0008, $0006, $AB1D;
PROCEDURE CTabChanged(ctab: CTabHandle);
INLINE $203C, $0004, $0007, $AB1D;
PROCEDURE PixPatChanged(ppat: PixPatHandle);
INLINE $203C, $0004, $0008, $AB1D;
PROCEDURE PortChanged(port: GrafPtr);
INLINE $203C, $0004, $0009, $AB1D;
PROCEDURE GDeviceChanged(gdh: GDHandle);
INLINE $203C, $0004, $000A, $AB1D;
PROCEDURE AllowPurgePixels(pm: PixMapHandle);
INLINE $203C, $0004, $000B, $AB1D;
PROCEDURE NoPurgePixels(pm: PixMapHandle);
INLINE $203C, $0004, $000C, $AB1D;
FUNCTION GetPixelsState(pm: PixMapHandle): GWorldFlags;
INLINE $203C, $0004, $000D, $AB1D;
PROCEDURE SetPixelsState(pm: PixMapHandle;state: GWorldFlags);
INLINE $203C, $0008, $000E, $AB1D;
FUNCTION GetPixBaseAddr(pm: PixMapHandle): Ptr;
INLINE $203C, $0004, $000F, $AB1D;
FUNCTION NewScreenBuffer(globalRect: Rect;purgeable: BOOLEAN;VAR gdh: GDHandle;
VAR offscreenPixMap: PixMapHandle): QDErr;
INLINE $203C, $000E, $0010, $AB1D;
PROCEDURE DisposeScreenBuffer(offscreenPixMap: PixMapHandle);
INLINE $203C, $0004, $0011, $AB1D;
FUNCTION GetGWorldDevice(offscreenGWorld: GWorldPtr): GDHandle;
INLINE $203C, $0004, $0012, $AB1D;
FUNCTION QDDone(port: GrafPtr): BOOLEAN;
INLINE $203C, $0004, $0013, $AB1D;
FUNCTION OffscreenVersion: LONGINT;


INLINE $7014, $AB1D;
FUNCTION NewTempScreenBuffer(globalRect: Rect;purgeable: BOOLEAN;VAR gdh: GDHandle;
VAR offscreenPixMap: PixMapHandle): QDErr;
INLINE $203C, $000E, $0015, $AB1D;
FUNCTION PixMap32Bit(pmHandle: PixMapHandle): BOOLEAN;
INLINE $203C, $0004, $0016, $AB1D;
FUNCTION GetGWorldPixMap(offscreenGWorld: GWorldPtr): PixMapHandle;
INLINE $203C, $0004, $0017, $AB1D;

{$ENDC} { UsingQDOffscreen }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE QDOffscreen.p}



{#####################################################################}
{### FILE: Quickdraw.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 10:59 PM
Quickdraw.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Quickdraw;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingQuickdraw}
{$SETC UsingQuickdraw := 1}
{$I+}
{$SETC QuickdrawIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := QuickdrawIncludes}
CONST
invalColReq = -1;
{ transfer modes }
srcCopy = 0;
srcOr = 1;
srcXor = 2;
srcBic = 3;
notSrcCopy = 4;
notSrcOr = 5;
notSrcXor = 6;
notSrcBic = 7;
patCopy = 8;
patOr = 9;
patXor = 10;
patBic = 11;
notPatCopy = 12;
notPatOr = 13;
notPatXor = 14;
notPatBic = 15;

{invalid color table request}

{the 16 transfer modes}

{ Special Text Transfer Mode
grayishTextOr = 49;


}

{ Arithmetic transfer modes }
blend = 32;
addPin = 33;
addOver = 34;
subPin = 35;
addMax = 37;
adMax = 37;
subOver = 38;
adMin = 39;
ditherCopy = 64;
{ Transparent mode constant }
transparent = 36;
{ QuickDraw color separation constants }
normalBit = 0;
inverseBit = 1;
redBit = 4;
greenBit = 3;
blueBit = 2;
cyanBit = 8;
magentaBit = 7;
yellowBit = 6;
blackBit = 5;
blackColor = 33;
whiteColor = 30;
redColor = 205;
greenColor = 341;
blueColor = 409;
cyanColor = 273;
magentaColor = 137;
yellowColor = 69;

{normal screen mapping}
{inverse screen mapping}
{RGB additive mapping}

{CMYBk subtractive mapping}

{colors expressed in these mappings}

picLParen = 0;
picRParen = 1;

{standard picture comments}

clutType = 0;
fixedType = 1;
directType = 2;

{0 if lookup table}
{1 if fixed table}
{2 if direct values}

gdDevType = 0;
burstDevice = 7;
ext32Device = 8;
ramInit = 10;
mainScreen = 11;
allInit = 12;
screenDevice = 13;
noDriver = 14;
screenActive = 15;

{0 = monochrome 1 = color}

hiliteBit = 7;
pHiliteBit = 0;

{flag bit in HiliteMode (lowMem flag)}
{flag bit in HiliteMode used with BitClr procedure}

{1 if initialized from 'scrn' resource}
{ 1 if main screen }
{ 1 if all devices initialized }
{1 if screen device [not used]}
{ 1 if no driver for this GDevice }
{1 if in use}



defQDColors = 127;

{resource ID of clut for default QDColors}

{ pixel type }
RGBDirect = 16;

{ 16 & 32 bits/pixel pixelType value }

{ pmVersion values }
baseAddr32 = 4;

{pixmap base address is 32-bit address}

rgnOverflowErr = -147;
insufficientStackErr = -149;

{ Region accumulation failed. Resulting region may be currupt }
{ QuickDraw could not complete the operation }

TYPE
GrafVerb = (frame,paint,erase,invert,fill);
PixelType = (chunky,chunkyPlanar,planar);

PatPtr = ^Pattern;
PatHandle = ^PatPtr;
Pattern = PACKED ARRAY [0..7] OF 0..255;

QDByte = SignedByte;
QDPtr = Ptr;

{ blind pointer }

QDHandle = Handle;

{ blind handle }

QDErr = INTEGER;
Bits16 = ARRAY [0..15] OF INTEGER;

StyleItem = (bold,italic,underline,outline,shadow,condense,extend);

Style = SET OF StyleItem;

DeviceLoopFlags = SET OF (singleDevices,dontMatchSeeds,allDevices,DeviceLoopFlags3,
DeviceLoopFlags4,DeviceLoopFlags5,DeviceLoopFlags6,DeviceLoopFlags7,DeviceLoopFlags8,
DeviceLoopFlags9,DeviceLoopFlags10,DeviceLoopFlags11,DeviceLoopFlags12,
DeviceLoopFlags13,DeviceLoopFlags14,DeviceLoopFlags15,DeviceLoopFlags16,
DeviceLoopFlags17,DeviceLoopFlags18,DeviceLoopFlags19,DeviceLoopFlags20,
DeviceLoopFlags21,DeviceLoopFlags22,DeviceLoopFlags23,DeviceLoopFlags24,
DeviceLoopFlags25,DeviceLoopFlags26,DeviceLoopFlags27,DeviceLoopFlags28,
DeviceLoopFlags29,DeviceLoopFlags30,DeviceLoopFlags31);

FontInfo = RECORD



ascent: INTEGER;
descent: INTEGER;
widMax: INTEGER;
leading: INTEGER;
END;
BitMapPtr = ^BitMap;
BitMapHandle = ^BitMapPtr;
BitMap = RECORD
baseAddr: Ptr;
rowBytes: INTEGER;
bounds: Rect;
END;
CursPtr = ^Cursor;
CursHandle = ^CursPtr;
Cursor = RECORD
data: Bits16;
mask: Bits16;
hotSpot: Point;
END;
PenState = RECORD
pnLoc: Point;
pnSize: Point;
pnMode: INTEGER;
pnPat: Pattern;
END;
RgnPtr = ^Region;
RgnHandle = ^RgnPtr;
Region = RECORD
rgnSize: INTEGER;
rgnBBox: Rect;
END;
PicPtr = ^Picture;
PicHandle = ^PicPtr;
Picture = RECORD
picSize: INTEGER;
picFrame: Rect;
END;
PolyPtr = ^Polygon;
PolyHandle = ^PolyPtr;
Polygon = RECORD
polySize: INTEGER;
polyBBox: Rect;
polyPoints: ARRAY [0..0] OF Point;
END;
QDProcsPtr = ^QDProcs;
QDProcs = RECORD
textProc: Ptr;
lineProc: Ptr;
rectProc: Ptr;

{size in bytes}
{enclosing rectangle}



rRectProc: Ptr;
ovalProc: Ptr;
arcProc: Ptr;
polyProc: Ptr;
rgnProc: Ptr;
bitsProc: Ptr;
commentProc: Ptr;
txMeasProc: Ptr;
getPicProc: Ptr;
putPicProc: Ptr;
END;
GrafPtr = ^GrafPort;
GrafPort = RECORD
device: INTEGER;
portBits: BitMap;
portRect: Rect;
visRgn: RgnHandle;
clipRgn: RgnHandle;
bkPat: Pattern;
fillPat: Pattern;
pnLoc: Point;
pnSize: Point;
pnMode: INTEGER;
pnPat: Pattern;
pnVis: INTEGER;
txFont: INTEGER;
txFace: Style;
txMode: INTEGER;
txSize: INTEGER;
spExtra: Fixed;
fgColor: LONGINT;
bkColor: LONGINT;
colrBit: INTEGER;
patStretch: INTEGER;
picSave: Handle;
rgnSave: Handle;
polySave: Handle;
grafProcs: QDProcsPtr;
END;

{txFace is unpacked byte but push as short}

WindowPtr = GrafPtr;
{typedef pascal Boolean (*ColorSearchProcPtr)(RGBColor *rgb, long *position);
typedef pascal Boolean (*ColorComplementProcPtr)(RGBColor *rgb);}
RGBColor = RECORD
red: INTEGER;
green: INTEGER;
blue: INTEGER;
END;
ColorSpecPtr = ^ColorSpec;
ColorSpec = RECORD
value: INTEGER;

{magnitude of red component}
{magnitude of green component}
{magnitude of blue component}

{index or other value}



rgb: RGBColor;
END;

{true color}

CSpecArray = ARRAY [0..0] OF ColorSpec;
CTabPtr = ^ColorTable;
CTabHandle = ^CTabPtr;
ColorTable = RECORD
ctSeed: LONGINT;
ctFlags: INTEGER;
ctSize: INTEGER;
ctTable: CSpecArray;
END;

{unique identifier for table}
{high bit: 0 = PixMap; 1 = device}
{number of entries in CTTable}
{array [0..0] of ColorSpec}

MatchRec = RECORD
red: INTEGER;
green: INTEGER;
blue: INTEGER;
matchData: LONGINT;
END;
PixMapPtr = ^PixMap;
PixMapHandle = ^PixMapPtr;
PixMap = RECORD
baseAddr: Ptr;
rowBytes: INTEGER;
bounds: Rect;
pmVersion: INTEGER;
packType: INTEGER;
packSize: LONGINT;
hRes: Fixed;
vRes: Fixed;
pixelType: INTEGER;
pixelSize: INTEGER;
cmpCount: INTEGER;
cmpSize: INTEGER;
planeBytes: LONGINT;
pmTable: CTabHandle;
pmReserved: LONGINT;
END;
PixPatPtr = ^PixPat;
PixPatHandle = ^PixPatPtr;
PixPat = RECORD
patType: INTEGER;
patMap: PixMapHandle;
patData: Handle;
patXData: Handle;
patXValid: INTEGER;
patXMap: Handle;
pat1Data: Pattern;
END;
CCrsrPtr = ^CCrsr;
CCrsrHandle = ^CCrsrPtr;

{pointer to pixels}
{offset to next line}
{encloses bitmap}
{pixMap version number}
{defines packing format}
{length of pixel data}
{horiz. resolution (ppi)}
{vert. resolution (ppi)}
{defines pixel type}
{# bits in pixel}
{# components in pixel}
{# bits per component}
{offset to next plane}
{color map for this pixMap}
{for future use. MUST BE 0}

{type of pattern}
{the pattern's pixMap}
{pixmap's data}
{expanded Pattern data}
{flags whether expanded Pattern valid}
{Handle to expanded Pattern data}
{old-Style pattern/RGB color}



CCrsr = RECORD
crsrType: INTEGER;
crsrMap: PixMapHandle;
crsrData: Handle;
crsrXData: Handle;
crsrXValid: INTEGER;
crsrXHandle: Handle;
crsr1Data: Bits16;
crsrMask: Bits16;
crsrHotSpot: Point;
crsrXTable: LONGINT;
crsrID: LONGINT;
END;
CIconPtr = ^CIcon;
CIconHandle = ^CIconPtr;
CIcon = RECORD
iconPMap: PixMap;
iconMask: BitMap;
iconBMap: BitMap;
iconData: Handle;
iconMaskData: ARRAY [0..0] OF INTEGER;
END;
GammaTblPtr = ^GammaTbl;
GammaTblHandle = ^GammaTblPtr;
GammaTbl = RECORD
gVersion: INTEGER;
gType: INTEGER;
gFormulaSize: INTEGER;
gChanCnt: INTEGER;
gDataCnt: INTEGER;
gDataWidth: INTEGER;
gFormulaData: ARRAY [0..0] OF INTEGER;
END;
ITabPtr = ^ITab;
ITabHandle = ^ITabPtr;
ITab = RECORD
iTabSeed: LONGINT;
iTabRes: INTEGER;
iTTable: ARRAY [0..0] OF SignedByte;
END;
SProcPtr = ^SProcRec;
SProcHndl = ^SProcPtr;
SProcRec = RECORD
nxtSrch: Handle;
srchProc: ProcPtr;
END;
CProcPtr = ^CProcRec;
CProcHndl = ^CProcPtr;
CProcRec = RECORD
nxtComp: CProcHndl;
compProc: ProcPtr;

{type of cursor}
{the cursor's pixmap}
{cursor's data}
{expanded cursor data}
{depth of expanded data (0 if none)}
{future use}
{one-bit cursor}
{cursor's mask}
{cursor's hotspot}
{private}
{private}

{the icon's pixMap}
{the icon's mask}
{the icon's bitMap}
{the icon's data}
{icon's mask and BitMap data}

{gamma version number}
{gamma data type}
{Formula data size}
{number of channels of data}
{number of values/channel}
{bits/corrected value (data packed to next larger byte size)}
{data for formulas followed by gamma values}

{copy of CTSeed from source CTable}
{bits/channel resolution of iTable}
{byte colortable index values}

{Handle to next SProcRec}
{pointer to search procedure}

{CProcHndl Handle to next CProcRec}
{pointer to complement procedure}



END;
GDPtr = ^GDevice;
GDHandle = ^GDPtr;
GDevice = RECORD
gdRefNum: INTEGER;
gdID: INTEGER;
gdType: INTEGER;
gdITable: ITabHandle;
gdResPref: INTEGER;
gdSearchProc: SProcHndl;
gdCompProc: CProcHndl;
gdFlags: INTEGER;
gdPMap: PixMapHandle;
gdRefCon: LONGINT;
gdNextGD: GDHandle;
gdRect: Rect;
gdMode: LONGINT;
gdCCBytes: INTEGER;
gdCCDepth: INTEGER;
gdCCXData: Handle;
gdCCXMask: Handle;
gdReserved: LONGINT;
END;
GVarPtr = ^GrafVars;
GVarHandle = ^GVarPtr;
GrafVars = RECORD
rgbOpColor: RGBColor;
rgbHiliteColor: RGBColor;
pmFgColor: Handle;
pmFgIndex: INTEGER;
pmBkColor: Handle;
pmBkIndex: INTEGER;
pmFlags: INTEGER;
END;
CQDProcsPtr = ^CQDProcs;
CQDProcs = RECORD
textProc: Ptr;
lineProc: Ptr;
rectProc: Ptr;
rRectProc: Ptr;
ovalProc: Ptr;
arcProc: Ptr;
polyProc: Ptr;
rgnProc: Ptr;
bitsProc: Ptr;
commentProc: Ptr;
txMeasProc: Ptr;
getPicProc: Ptr;
putPicProc: Ptr;
opcodeProc: Ptr;
newProc1: Ptr;
newProc2: Ptr;
newProc3: Ptr;

{driver's unit number}
{client ID for search procs}
{fixed/CLUT/direct}
{Handle to inverse lookup table}
{preferred resolution of GDITable}
{search proc list head}
{complement proc list}
{grafDevice flags word}
{describing pixMap}
{reference value}
{GDHandle Handle of next gDevice}
{ device's bounds in global coordinates}
{device's current mode}
{depth of expanded cursor data}
{depth of expanded cursor data}
{Handle to cursor's expanded data}
{Handle to cursor's expanded mask}
{future use. MUST BE 0}

{color for addPin subPin and average}
{color for hiliting}
{palette Handle for foreground color}
{index value for foreground}
{palette Handle for background color}
{index value for background}
{flags for Palette Manager}

{fields added to QDProcs}



newProc4: Ptr;
newProc5: Ptr;
newProc6: Ptr;
END;
CGrafPtr = ^CGrafPort;
CGrafPort = RECORD
device: INTEGER;
portPixMap: PixMapHandle;
portVersion: INTEGER;
grafVars: Handle;
chExtra: INTEGER;
pnLocHFrac: INTEGER;
portRect: Rect;
visRgn: RgnHandle;
clipRgn: RgnHandle;
bkPixPat: PixPatHandle;
rgbFgColor: RGBColor;
rgbBkColor: RGBColor;
pnLoc: Point;
pnSize: Point;
pnMode: INTEGER;
pnPixPat: PixPatHandle;
fillPixPat: PixPatHandle;
pnVis: INTEGER;
txFont: INTEGER;
txFace: Style;
txMode: INTEGER;
txSize: INTEGER;
spExtra: Fixed;
fgColor: LONGINT;
bkColor: LONGINT;
colrBit: INTEGER;
patStretch: INTEGER;
picSave: Handle;
rgnSave: Handle;
polySave: Handle;
grafProcs: CQDProcsPtr;
END;

{port's pixel map}
{high 2 bits always set}
{Handle to more fields}
{character extra}
{pen fraction}

{background pattern}
{RGB components of fg}
{RGB components of bk}

{pen's pattern}
{fill pattern}

{txFace is unpacked byte

CWindowPtr = CGrafPtr;
ReqListRec = RECORD
reqLSize: INTEGER;
reqLData: ARRAY [0..0] OF INTEGER;
END;
OpenCPicParams = RECORD
srcRect: Rect;
hRes: Fixed;
vRes: Fixed;
version: INTEGER;
reserved1: INTEGER;
reserved2: LONGINT;
END;

{request list size}
{request list data}

push as short}



DeviceLoopDrawingProcPtr = ProcPtr;

VAR
{$PUSH}
{$J+}
thePort: GrafPtr;
white: Pattern;
black: Pattern;
gray: Pattern;
ltGray: Pattern;
dkGray: Pattern;
arrow: Cursor;
screenBits: BitMap;
randSeed: LONGINT;
{$POP}

PROCEDURE InitGraf(globalPtr: Ptr);
INLINE $A86E;
PROCEDURE OpenPort(port: GrafPtr);
INLINE $A86F;
PROCEDURE InitPort(port: GrafPtr);
INLINE $A86D;
PROCEDURE ClosePort(port: GrafPtr);
INLINE $A87D;
PROCEDURE SetPort(port: GrafPtr);
INLINE $A873;
PROCEDURE GetPort(VAR port: GrafPtr);
INLINE $A874;
PROCEDURE GrafDevice(device: INTEGER);
INLINE $A872;
PROCEDURE SetPortBits(bm: BitMap);
INLINE $A875;
PROCEDURE PortSize(width: INTEGER;height: INTEGER);
INLINE $A876;
PROCEDURE MovePortTo(leftGlobal: INTEGER;topGlobal: INTEGER);
INLINE $A877;
PROCEDURE SetOrigin(h: INTEGER;v: INTEGER);
INLINE $A878;
PROCEDURE SetClip(rgn: RgnHandle);
INLINE $A879;
PROCEDURE GetClip(rgn: RgnHandle);
INLINE $A87A;
PROCEDURE ClipRect(r: Rect);
INLINE $A87B;
PROCEDURE BackPat(pat: Pattern);
INLINE $A87C;
PROCEDURE InitCursor;
INLINE $A850;
PROCEDURE SetCursor(crsr: Cursor);
INLINE $A851;
PROCEDURE HideCursor;
INLINE $A852;
PROCEDURE ShowCursor;
INLINE $A853;
PROCEDURE ObscureCursor;
INLINE $A856;
PROCEDURE HidePen;
INLINE $A896;
PROCEDURE ShowPen;
INLINE $A897;
PROCEDURE GetPen(VAR pt: Point);
INLINE $A89A;
PROCEDURE GetPenState(VAR pnState: PenState);
INLINE $A898;
PROCEDURE SetPenState(pnState: PenState);
INLINE $A899;
PROCEDURE PenSize(width: INTEGER;height: INTEGER);
INLINE $A89B;
PROCEDURE PenMode(mode: INTEGER);
INLINE $A89C;
PROCEDURE PenPat(pat: Pattern);
INLINE $A89D;
PROCEDURE PenNormal;
INLINE $A89E;
PROCEDURE MoveTo(h: INTEGER;v: INTEGER);
INLINE $A893;
PROCEDURE Move(dh: INTEGER;dv: INTEGER);
INLINE $A894;
PROCEDURE LineTo(h: INTEGER;v: INTEGER);
INLINE $A891;
PROCEDURE Line(dh: INTEGER;dv: INTEGER);
INLINE $A892;
PROCEDURE TextFont(font: INTEGER);
INLINE $A887;
PROCEDURE TextFace(face: Style);
INLINE $A888;
PROCEDURE TextMode(mode: INTEGER);
INLINE $A889;
PROCEDURE TextSize(size: INTEGER);
INLINE $A88A;
PROCEDURE SpaceExtra(extra: Fixed);
INLINE $A88E;
PROCEDURE DrawChar(ch: CHAR);
INLINE $A883;
PROCEDURE DrawString(s: Str255);
INLINE $A884;
PROCEDURE DrawText(textBuf: Ptr;firstByte: INTEGER;byteCount: INTEGER);
INLINE $A885;
FUNCTION CharWidth(ch: CHAR): INTEGER;
INLINE $A88D;
FUNCTION StringWidth(s: Str255): INTEGER;
INLINE $A88C;
FUNCTION TextWidth(textBuf: Ptr;firstByte: INTEGER;byteCount: INTEGER): INTEGER;
INLINE $A886;
PROCEDURE MeasureText(count: INTEGER;textAddr: Ptr;charLocs: Ptr);
INLINE $A837;
PROCEDURE GetFontInfo(VAR info: FontInfo);
INLINE $A88B;



PROCEDURE ForeColor(color: LONGINT);
INLINE $A862;
PROCEDURE BackColor(color: LONGINT);
INLINE $A863;
PROCEDURE ColorBit(whichBit: INTEGER);
INLINE $A864;
PROCEDURE SetRect(VAR r: Rect;left: INTEGER;top: INTEGER;right: INTEGER;
bottom: INTEGER);
INLINE $A8A7;
PROCEDURE OffsetRect(VAR r: Rect;dh: INTEGER;dv: INTEGER);
INLINE $A8A8;
PROCEDURE InsetRect(VAR r: Rect;dh: INTEGER;dv: INTEGER);
INLINE $A8A9;
FUNCTION SectRect(src1: Rect;src2: Rect;VAR dstRect: Rect): BOOLEAN;
INLINE $A8AA;
PROCEDURE UnionRect(src1: Rect;src2: Rect;VAR dstRect: Rect);
INLINE $A8AB;
FUNCTION EqualRect(rect1: Rect;rect2: Rect): BOOLEAN;
INLINE $A8A6;
FUNCTION EmptyRect(r: Rect): BOOLEAN;
INLINE $A8AE;
PROCEDURE FrameRect(r: Rect);
INLINE $A8A1;
PROCEDURE PaintRect(r: Rect);
INLINE $A8A2;
PROCEDURE EraseRect(r: Rect);
INLINE $A8A3;
PROCEDURE InvertRect(r: Rect);
INLINE $A8A4;
PROCEDURE FillRect(r: Rect;pat: Pattern);
INLINE $A8A5;
PROCEDURE FrameOval(r: Rect);
INLINE $A8B7;
PROCEDURE PaintOval(r: Rect);
INLINE $A8B8;
PROCEDURE EraseOval(r: Rect);
INLINE $A8B9;
PROCEDURE InvertOval(r: Rect);
INLINE $A8BA;
PROCEDURE FillOval(r: Rect;pat: Pattern);
INLINE $A8BB;
PROCEDURE FrameRoundRect(r: Rect;ovalWidth: INTEGER;ovalHeight: INTEGER);
INLINE $A8B0;
PROCEDURE PaintRoundRect(r: Rect;ovalWidth: INTEGER;ovalHeight: INTEGER);
INLINE $A8B1;
PROCEDURE EraseRoundRect(r: Rect;ovalWidth: INTEGER;ovalHeight: INTEGER);
INLINE $A8B2;
PROCEDURE InvertRoundRect(r: Rect;ovalWidth: INTEGER;ovalHeight: INTEGER);
INLINE $A8B3;
PROCEDURE FillRoundRect(r: Rect;ovalWidth: INTEGER;ovalHeight: INTEGER;
pat: Pattern);
INLINE $A8B4;
PROCEDURE FrameArc(r: Rect;startAngle: INTEGER;arcAngle: INTEGER);
INLINE $A8BE;
PROCEDURE PaintArc(r: Rect;startAngle: INTEGER;arcAngle: INTEGER);
INLINE $A8BF;



PROCEDURE EraseArc(r: Rect;startAngle: INTEGER;arcAngle: INTEGER);
INLINE $A8C0;
PROCEDURE InvertArc(r: Rect;startAngle: INTEGER;arcAngle: INTEGER);
INLINE $A8C1;
PROCEDURE FillArc(r: Rect;startAngle: INTEGER;arcAngle: INTEGER;pat: Pattern);
INLINE $A8C2;
FUNCTION NewRgn: RgnHandle;
INLINE $A8D8;
PROCEDURE OpenRgn;
INLINE $A8DA;
PROCEDURE CloseRgn(dstRgn: RgnHandle);
INLINE $A8DB;
FUNCTION BitMapToRegionGlue(region: RgnHandle;bMap: BitMap): OSErr;
FUNCTION BitMapToRegion(region: RgnHandle;bMap: BitMap): OSErr;
INLINE $A8D7;
PROCEDURE DisposeRgn(rgn: RgnHandle);
INLINE $A8D9;
PROCEDURE CopyRgn(srcRgn: RgnHandle;dstRgn: RgnHandle);
INLINE $A8DC;
PROCEDURE SetEmptyRgn(rgn: RgnHandle);
INLINE $A8DD;
PROCEDURE SetRectRgn(rgn: RgnHandle;left: INTEGER;top: INTEGER;right: INTEGER;
bottom: INTEGER);
INLINE $A8DE;
PROCEDURE RectRgn(rgn: RgnHandle;r: Rect);
INLINE $A8DF;
PROCEDURE OffsetRgn(rgn: RgnHandle;dh: INTEGER;dv: INTEGER);
INLINE $A8E0;
PROCEDURE InsetRgn(rgn: RgnHandle;dh: INTEGER;dv: INTEGER);
INLINE $A8E1;
PROCEDURE SectRgn(srcRgnA: RgnHandle;srcRgnB: RgnHandle;dstRgn: RgnHandle);
INLINE $A8E4;
PROCEDURE UnionRgn(srcRgnA: RgnHandle;srcRgnB: RgnHandle;dstRgn: RgnHandle);
INLINE $A8E5;
PROCEDURE DiffRgn(srcRgnA: RgnHandle;srcRgnB: RgnHandle;dstRgn: RgnHandle);
INLINE $A8E6;
PROCEDURE XorRgn(srcRgnA: RgnHandle;srcRgnB: RgnHandle;dstRgn: RgnHandle);
INLINE $A8E7;
FUNCTION RectInRgn(r: Rect;rgn: RgnHandle): BOOLEAN;
INLINE $A8E9;
FUNCTION EqualRgn(rgnA: RgnHandle;rgnB: RgnHandle): BOOLEAN;
INLINE $A8E3;
FUNCTION EmptyRgn(rgn: RgnHandle): BOOLEAN;
INLINE $A8E2;
PROCEDURE FrameRgn(rgn: RgnHandle);
INLINE $A8D2;
PROCEDURE PaintRgn(rgn: RgnHandle);
INLINE $A8D3;
PROCEDURE EraseRgn(rgn: RgnHandle);
INLINE $A8D4;
PROCEDURE InvertRgn(rgn: RgnHandle);
INLINE $A8D5;
PROCEDURE FillRgn(rgn: RgnHandle;pat: Pattern);
INLINE $A8D6;
PROCEDURE ScrollRect(r: Rect;dh: INTEGER;dv: INTEGER;updateRgn: RgnHandle);
INLINE $A8EF;



PROCEDURE CopyBits(srcBits: BitMap;dstBits: BitMap;srcRect: Rect;dstRect: Rect;
mode: INTEGER;maskRgn: RgnHandle);
INLINE $A8EC;
PROCEDURE SeedFill(srcPtr: Ptr;dstPtr: Ptr;srcRow: INTEGER;dstRow: INTEGER;
height: INTEGER;words: INTEGER;seedH: INTEGER;seedV: INTEGER);
INLINE $A839;
PROCEDURE CalcMask(srcPtr: Ptr;dstPtr: Ptr;srcRow: INTEGER;dstRow: INTEGER;
height: INTEGER;words: INTEGER);
INLINE $A838;
PROCEDURE CopyMask(srcBits: BitMap;maskBits: BitMap;dstBits: BitMap;srcRect: Rect;
maskRect: Rect;dstRect: Rect);
INLINE $A817;
FUNCTION OpenPicture(picFrame: Rect): PicHandle;
INLINE $A8F3;
PROCEDURE PicComment(kind: INTEGER;dataSize: INTEGER;dataHandle: Handle);
INLINE $A8F2;
PROCEDURE ClosePicture;
INLINE $A8F4;
PROCEDURE DrawPicture(myPicture: PicHandle;dstRect: Rect);
INLINE $A8F6;
PROCEDURE KillPicture(myPicture: PicHandle);
INLINE $A8F5;
FUNCTION OpenPoly: PolyHandle;
INLINE $A8CB;
PROCEDURE ClosePoly;
INLINE $A8CC;
PROCEDURE KillPoly(poly: PolyHandle);
INLINE $A8CD;
PROCEDURE OffsetPoly(poly: PolyHandle;dh: INTEGER;dv: INTEGER);
INLINE $A8CE;
PROCEDURE FramePoly(poly: PolyHandle);
INLINE $A8C6;
PROCEDURE PaintPoly(poly: PolyHandle);
INLINE $A8C7;
PROCEDURE ErasePoly(poly: PolyHandle);
INLINE $A8C8;
PROCEDURE InvertPoly(poly: PolyHandle);
INLINE $A8C9;
PROCEDURE FillPoly(poly: PolyHandle;pat: Pattern);
INLINE $A8CA;
PROCEDURE SetPt(VAR pt: Point;h: INTEGER;v: INTEGER);
INLINE $A880;
PROCEDURE LocalToGlobal(VAR pt: Point);
INLINE $A870;
PROCEDURE GlobalToLocal(VAR pt: Point);
INLINE $A871;
FUNCTION Random: INTEGER;
INLINE $A861;
PROCEDURE StuffHex(thingPtr: Ptr;s: Str255);
INLINE $A866;
FUNCTION GetPixel(h: INTEGER;v: INTEGER): BOOLEAN;
INLINE $A865;
PROCEDURE ScalePt(VAR pt: Point;srcRect: Rect;dstRect: Rect);
INLINE $A8F8;
PROCEDURE MapPt(VAR pt: Point;srcRect: Rect;dstRect: Rect);
INLINE $A8F9;
PROCEDURE MapRect(VAR r: Rect;srcRect: Rect;dstRect: Rect);
INLINE $A8FA;
PROCEDURE MapRgn(rgn: RgnHandle;srcRect: Rect;dstRect: Rect);
INLINE $A8FB;
PROCEDURE MapPoly(poly: PolyHandle;srcRect: Rect;dstRect: Rect);
INLINE $A8FC;
PROCEDURE SetStdProcs(VAR procs: QDProcs);
INLINE $A8EA;
PROCEDURE StdRect(verb: GrafVerb;r: Rect);
INLINE $A8A0;
PROCEDURE StdRRect(verb: GrafVerb;r: Rect;ovalWidth: INTEGER;ovalHeight: INTEGER);
INLINE $A8AF;
PROCEDURE StdOval(verb: GrafVerb;r: Rect);
INLINE $A8B6;
PROCEDURE StdArc(verb: GrafVerb;r: Rect;startAngle: INTEGER;arcAngle: INTEGER);
INLINE $A8BD;
PROCEDURE StdPoly(verb: GrafVerb;poly: PolyHandle);
INLINE $A8C5;
PROCEDURE StdRgn(verb: GrafVerb;rgn: RgnHandle);
INLINE $A8D1;
PROCEDURE StdBits(VAR srcBits: BitMap;VAR srcRect: Rect;dstRect: Rect;mode: INTEGER;
maskRgn: RgnHandle);
INLINE $A8EB;
PROCEDURE StdComment(kind: INTEGER;dataSize: INTEGER;dataHandle: Handle);
INLINE $A8F1;
FUNCTION StdTxMeas(byteCount: INTEGER;textAddr: Ptr;VAR numer: Point;VAR denom: Point;
VAR info: FontInfo): INTEGER;
INLINE $A8ED;
PROCEDURE StdGetPic(dataPtr: Ptr;byteCount: INTEGER);
INLINE $A8EE;
PROCEDURE StdPutPic(dataPtr: Ptr;byteCount: INTEGER);
INLINE $A8F0;
PROCEDURE AddPt(src: Point;VAR dst: Point);
INLINE $A87E;
FUNCTION EqualPt(pt1: Point;pt2: Point): BOOLEAN;
INLINE $A881;
FUNCTION PtInRect(pt: Point;r: Rect): BOOLEAN;
INLINE $A8AD;
PROCEDURE Pt2Rect(pt1: Point;pt2: Point;VAR dstRect: Rect);
INLINE $A8AC;
PROCEDURE PtToAngle(r: Rect;pt: Point;VAR angle: INTEGER);
INLINE $A8C3;
FUNCTION PtInRgn(pt: Point;rgn: RgnHandle): BOOLEAN;
INLINE $A8E8;
PROCEDURE StdText(count: INTEGER;textAddr: Ptr;numer: Point;denom: Point);
INLINE $A882;
PROCEDURE StdLine(newPt: Point);
INLINE $A890;
PROCEDURE OpenCPort(port: CGrafPtr);
INLINE $AA00;
PROCEDURE InitCPort(port: CGrafPtr);
INLINE $AA01;
PROCEDURE CloseCPort(port: CGrafPtr);
INLINE $A87D;
FUNCTION NewPixMap: PixMapHandle;
INLINE $AA03;



PROCEDURE DisposPixMap(pm: PixMapHandle);
INLINE $AA04;
PROCEDURE DisposePixMap(pm: PixMapHandle);
INLINE $AA04;
PROCEDURE CopyPixMap(srcPM: PixMapHandle;dstPM: PixMapHandle);
INLINE $AA05;
FUNCTION NewPixPat: PixPatHandle;
INLINE $AA07;
PROCEDURE DisposPixPat(pp: PixPatHandle);
INLINE $AA08;
PROCEDURE DisposePixPat(pp: PixPatHandle);
INLINE $AA08;
PROCEDURE CopyPixPat(srcPP: PixPatHandle;dstPP: PixPatHandle);
INLINE $AA09;
PROCEDURE PenPixPat(pp: PixPatHandle);
INLINE $AA0A;
PROCEDURE BackPixPat(pp: PixPatHandle);
INLINE $AA0B;
FUNCTION GetPixPat(patID: INTEGER): PixPatHandle;
INLINE $AA0C;
PROCEDURE MakeRGBPat(pp: PixPatHandle;myColor: RGBColor);
INLINE $AA0D;
PROCEDURE FillCRect(r: Rect;pp: PixPatHandle);
INLINE $AA0E;
PROCEDURE FillCOval(r: Rect;pp: PixPatHandle);
INLINE $AA0F;
PROCEDURE FillCRoundRect(r: Rect;ovalWidth: INTEGER;ovalHeight: INTEGER;
pp: PixPatHandle);
INLINE $AA10;
PROCEDURE FillCArc(r: Rect;startAngle: INTEGER;arcAngle: INTEGER;pp: PixPatHandle);
INLINE $AA11;
PROCEDURE FillCRgn(rgn: RgnHandle;pp: PixPatHandle);
INLINE $AA12;
PROCEDURE FillCPoly(poly: PolyHandle;pp: PixPatHandle);
INLINE $AA13;
PROCEDURE RGBForeColor(color: RGBColor);
INLINE $AA14;
PROCEDURE RGBBackColor(color: RGBColor);
INLINE $AA15;
PROCEDURE SetCPixel(h: INTEGER;v: INTEGER;cPix: RGBColor);
INLINE $AA16;
PROCEDURE SetPortPix(pm: PixMapHandle);
INLINE $AA06;
PROCEDURE GetCPixel(h: INTEGER;v: INTEGER;VAR cPix: RGBColor);
INLINE $AA17;
PROCEDURE GetForeColor(VAR color: RGBColor);
INLINE $AA19;
PROCEDURE GetBackColor(VAR color: RGBColor);
INLINE $AA1A;
PROCEDURE SeedCFill(srcBits: BitMap;dstBits: BitMap;srcRect: Rect;dstRect: Rect;
seedH: INTEGER;seedV: INTEGER;matchProc: ProcPtr;matchData: LONGINT);
INLINE $AA50;
PROCEDURE CalcCMask(srcBits: BitMap;dstBits: BitMap;srcRect: Rect;dstRect: Rect;
seedRGB: RGBColor;matchProc: ProcPtr;matchData: LONGINT);
INLINE $AA4F;
FUNCTION OpenCPicture(newHeader: OpenCPicParams): PicHandle;



INLINE $AA20;
PROCEDURE OpColor(color: RGBColor);
INLINE $AA21;
PROCEDURE HiliteColor(color: RGBColor);
INLINE $AA22;
PROCEDURE DisposCTable(cTable: CTabHandle);
INLINE $AA24;
PROCEDURE DisposeCTable(cTable: CTabHandle);
INLINE $AA24;
FUNCTION GetCTable(ctID: INTEGER): CTabHandle;
INLINE $AA18;
FUNCTION GetCCursor(crsrID: INTEGER): CCrsrHandle;
INLINE $AA1B;
PROCEDURE SetCCursor(cCrsr: CCrsrHandle);
INLINE $AA1C;
PROCEDURE AllocCursor;
INLINE $AA1D;
PROCEDURE DisposCCursor(cCrsr: CCrsrHandle);
INLINE $AA26;
PROCEDURE DisposeCCursor(cCrsr: CCrsrHandle);
INLINE $AA26;
FUNCTION GetCIcon(iconID: INTEGER): CIconHandle;
INLINE $AA1E;
PROCEDURE PlotCIcon(theRect: Rect;theIcon: CIconHandle);
INLINE $AA1F;
PROCEDURE DisposCIcon(theIcon: CIconHandle);
INLINE $AA25;
PROCEDURE DisposeCIcon(theIcon: CIconHandle);
INLINE $AA25;
PROCEDURE SetStdCProcs(VAR procs: CQDProcs);
INLINE $AA4E;
PROCEDURE CharExtra(extra: Fixed);
INLINE $AA23;
FUNCTION GetMaxDevice(globalRect: Rect): GDHandle;
INLINE $AA27;
FUNCTION GetCTSeed: LONGINT;
INLINE $AA28;
FUNCTION GetDeviceList: GDHandle;
INLINE $AA29;
FUNCTION GetMainDevice: GDHandle;
INLINE $AA2A;
FUNCTION GetNextDevice(curDevice: GDHandle): GDHandle;
INLINE $AA2B;
FUNCTION TestDeviceAttribute(gdh: GDHandle;attribute: INTEGER): BOOLEAN;
INLINE $AA2C;
PROCEDURE SetDeviceAttribute(gdh: GDHandle;attribute: INTEGER;value: BOOLEAN);
INLINE $AA2D;
PROCEDURE InitGDevice(qdRefNum: INTEGER;mode: LONGINT;gdh: GDHandle);
INLINE $AA2E;
FUNCTION NewGDevice(refNum: INTEGER;mode: LONGINT): GDHandle;
INLINE $AA2F;
PROCEDURE DisposGDevice(gdh: GDHandle);
INLINE $AA30;
PROCEDURE DisposeGDevice(gdh: GDHandle);
INLINE $AA30;
PROCEDURE SetGDevice(gd: GDHandle);



INLINE $AA31;
FUNCTION GetGDevice: GDHandle;
INLINE $AA32;
FUNCTION Color2Index(myColor: RGBColor): LONGINT;
INLINE $AA33;
PROCEDURE Index2Color(index: LONGINT;VAR aColor: RGBColor);
INLINE $AA34;
PROCEDURE InvertColor(VAR myColor: RGBColor);
INLINE $AA35;
FUNCTION RealColor(color: RGBColor): BOOLEAN;
INLINE $AA36;
PROCEDURE GetSubTable(myColors: CTabHandle;iTabRes: INTEGER;targetTbl: CTabHandle);
INLINE $AA37;
PROCEDURE MakeITable(cTabH: CTabHandle;iTabH: ITabHandle;res: INTEGER);
INLINE $AA39;
PROCEDURE AddSearch(searchProc: ProcPtr);
INLINE $AA3A;
PROCEDURE AddComp(compProc: ProcPtr);
INLINE $AA3B;
PROCEDURE DelSearch(searchProc: ProcPtr);
INLINE $AA4C;
PROCEDURE DelComp(compProc: ProcPtr);
INLINE $AA4D;
PROCEDURE SubPt(src: Point;VAR dst: Point);
INLINE $A87F;
PROCEDURE SetClientID(id: INTEGER);
INLINE $AA3C;
PROCEDURE ProtectEntry(index: INTEGER;protect: BOOLEAN);
INLINE $AA3D;
PROCEDURE ReserveEntry(index: INTEGER;reserve: BOOLEAN);
INLINE $AA3E;
PROCEDURE SetEntries(start: INTEGER;count: INTEGER;aTable: CSpecArray);
INLINE $AA3F;
PROCEDURE SaveEntries(srcTable: CTabHandle;resultTable: CTabHandle;VAR selection: ReqListRec);
INLINE $AA49;
PROCEDURE RestoreEntries(srcTable: CTabHandle;dstTable: CTabHandle;VAR selection: ReqListRec);
INLINE $AA4A;
FUNCTION QDError: INTEGER;
INLINE $AA40;
PROCEDURE CopyDeepMask(srcBits: BitMap;maskBits: BitMap;dstBits: BitMap;
srcRect: Rect;maskRect: Rect;dstRect: Rect;mode: INTEGER;maskRgn: RgnHandle);
INLINE $AA51;
PROCEDURE DeviceLoop(drawingRgn: RgnHandle;drawingProc: DeviceLoopDrawingProcPtr;
userData: LONGINT;flags: DeviceLoopFlags);
INLINE $ABCA;
FUNCTION GetMaskTable: Ptr;
INLINE $A836,$2E88;

{$ENDC}

{ UsingQuickdraw }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Quickdraw.p}





{#####################################################################}
{### FILE: Resources.p}
{#####################################################################}

{
Created: Thursday, September 12, 1991 at 2:50 PM
Resources.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Resources;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingResources}
{$SETC UsingResources := 1}
{$I+}
{$SETC ResourcesIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$SETC UsingIncludes := ResourcesIncludes}
CONST
resSysHeap = 64;
resPurgeable = 32;
resLocked = 16;
resProtected = 8;
resPreload = 4;
resChanged = 2;
mapReadOnly = 128;
mapCompact = 64;
mapChanged = 32;

{System or application heap?}
{Purgeable resource?}
{Load it in locked?}
{Protected?}
{Load in on OpenResFile?}
{Resource changed?}
{Resource file read-only}
{Compact resource file}
{Write map out at updat}

{ Values for setting RomMapInsert and TmpResLoad }
mapTrue = $FFFF;
{insert ROM map w/ TmpResLoad = TRUE.}
mapFalse = $FF00;
{insert ROM map w/ TmpResLoad = FALSE.}
FUNCTION InitResources: INTEGER;
INLINE $A995;



PROCEDURE RsrcZoneInit;
INLINE $A996;
PROCEDURE CloseResFile(refNum: INTEGER);
INLINE $A99A;
FUNCTION ResError: INTEGER;
INLINE $A9AF;
FUNCTION CurResFile: INTEGER;
INLINE $A994;
FUNCTION HomeResFile(theResource: Handle): INTEGER;
INLINE $A9A4;
PROCEDURE CreateResFile(fileName: Str255);
INLINE $A9B1;
FUNCTION OpenResFile(fileName: Str255): INTEGER;
INLINE $A997;
PROCEDURE UseResFile(refNum: INTEGER);
INLINE $A998;
FUNCTION CountTypes: INTEGER;
INLINE $A99E;
FUNCTION Count1Types: INTEGER;
INLINE $A81C;
PROCEDURE GetIndType(VAR theType: ResType;index: INTEGER);
INLINE $A99F;
PROCEDURE Get1IndType(VAR theType: ResType;index: INTEGER);
INLINE $A80F;
PROCEDURE SetResLoad(load: BOOLEAN);
INLINE $A99B;
FUNCTION CountResources(theType: ResType): INTEGER;
INLINE $A99C;
FUNCTION Count1Resources(theType: ResType): INTEGER;
INLINE $A80D;
FUNCTION GetIndResource(theType: ResType;index: INTEGER): Handle;
INLINE $A99D;
FUNCTION Get1IndResource(theType: ResType;index: INTEGER): Handle;
INLINE $A80E;
FUNCTION GetResource(theType: ResType;theID: INTEGER): Handle;
INLINE $A9A0;
FUNCTION Get1Resource(theType: ResType;theID: INTEGER): Handle;
INLINE $A81F;
FUNCTION GetNamedResource(theType: ResType;name: Str255): Handle;
INLINE $A9A1;
FUNCTION Get1NamedResource(theType: ResType;name: Str255): Handle;
INLINE $A820;
PROCEDURE LoadResource(theResource: Handle);
INLINE $A9A2;
PROCEDURE ReleaseResource(theResource: Handle);
INLINE $A9A3;
PROCEDURE DetachResource(theResource: Handle);
INLINE $A992;
FUNCTION UniqueID(theType: ResType): INTEGER;
INLINE $A9C1;
FUNCTION Unique1ID(theType: ResType): INTEGER;
INLINE $A810;
FUNCTION GetResAttrs(theResource: Handle): INTEGER;
INLINE $A9A6;
PROCEDURE GetResInfo(theResource: Handle;VAR theID: INTEGER;VAR theType: ResType;
VAR name: Str255);



INLINE $A9A8;
PROCEDURE SetResInfo(theResource: Handle;theID: INTEGER;name: Str255);
INLINE $A9A9;
PROCEDURE AddResource(theResource: Handle;theType: ResType;theID: INTEGER;
name: Str255);
INLINE $A9AB;
FUNCTION SizeResource(theResource: Handle): LONGINT;
INLINE $A9A5;
FUNCTION MaxSizeRsrc(theResource: Handle): LONGINT;
INLINE $A821;
FUNCTION RsrcMapEntry(theResource: Handle): LONGINT;
INLINE $A9C5;
PROCEDURE SetResAttrs(theResource: Handle;attrs: INTEGER);
INLINE $A9A7;
PROCEDURE ChangedResource(theResource: Handle);
INLINE $A9AA;
PROCEDURE RmveResource(theResource: Handle);
INLINE $A9AD;
PROCEDURE UpdateResFile(refNum: INTEGER);
INLINE $A999;
PROCEDURE WriteResource(theResource: Handle);
INLINE $A9B0;
PROCEDURE SetResPurge(install: BOOLEAN);
INLINE $A993;
FUNCTION GetResFileAttrs(refNum: INTEGER): INTEGER;
INLINE $A9F6;
PROCEDURE SetResFileAttrs(refNum: INTEGER;attrs: INTEGER);
INLINE $A9F7;
FUNCTION OpenRFPerm(fileName: Str255;vRefNum: INTEGER;permission: SignedByte): INTEGER;
INLINE $A9C4;
FUNCTION RGetResource(theType: ResType;theID: INTEGER): Handle;
INLINE $A80C;
FUNCTION HOpenResFile(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255;
permission: SignedByte): INTEGER;
{$IFC SystemSevenOrLater }
INLINE $A81A;
{$ENDC}
PROCEDURE HCreateResFile(vRefNum: INTEGER;dirID: LONGINT;fileName: Str255);
{$IFC SystemSevenOrLater }
INLINE $A81B;
{$ENDC}
FUNCTION FSpOpenResFile(spec: FSSpec;permission: SignedByte): INTEGER;
INLINE $303C,$000D,$AA52;
PROCEDURE FSpCreateResFile(spec: FSSpec;creator: OSType;fileType: OSType;
scriptTag: ScriptCode);
INLINE $303C,$000E,$AA52;
{ partial resource calls }
PROCEDURE ReadPartialResource(theResource: Handle;offset: LONGINT;buffer: UNIV Ptr;
count: LONGINT);
INLINE $7001,$A822;
PROCEDURE WritePartialResource(theResource: Handle;offset: LONGINT;buffer: UNIV Ptr;
count: LONGINT);
INLINE $7002,$A822;
PROCEDURE SetResourceSize(theResource: Handle;newSize: LONGINT);

INLINE $7003,$A822;

{$ENDC} { UsingResources }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Resources.p}





{#####################################################################}
{### FILE: Retrace.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:06 PM
Retrace.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Retrace;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingRetrace}
{$SETC UsingRetrace := 1}
{$I+}
{$SETC RetraceIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$SETC UsingIncludes := RetraceIncludes}
FUNCTION GetVBLQHdr: QHdrPtr;
INLINE $2EBC,$0000,$0160;
FUNCTION SlotVInstall(vblBlockPtr: QElemPtr;theSlot: INTEGER): OSErr;
INLINE $301F,$205F,$A06F,$3E80;
FUNCTION SlotVRemove(vblBlockPtr: QElemPtr;theSlot: INTEGER): OSErr;
INLINE $301F,$205F,$A070,$3E80;
FUNCTION AttachVBL(theSlot: INTEGER): OSErr;
INLINE $301F,$A071,$3E80;
FUNCTION DoVBLTask(theSlot: INTEGER): OSErr;
INLINE $301F,$A072,$3E80;
FUNCTION VInstall(vblTaskPtr: QElemPtr): OSErr;
INLINE $205F,$A033,$3E80;
FUNCTION VRemove(vblTaskPtr: QElemPtr): OSErr;
INLINE $205F,$A034,$3E80;

{$ENDC}

{ UsingRetrace }



{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Retrace.p}





{#####################################################################}
{### FILE: ROMDefs.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:06 PM
ROMDefs.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1986-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT ROMDefs;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingROMDefs}
{$SETC UsingROMDefs := 1}

CONST
appleFormat = 1;
romRevision = 1;
testPattern = 1519594439;

{Format of Declaration Data (IEEE will assign real value)}
{Revision of Declaration Data Format}
{FHeader long word test pattern}

sCodeRev = 2;
sCPU68000 = 1;
sCPU68020 = 2;
sCPU68030 = 3;
sCPU68040 = 4;
sMacOS68000 = 1;
sMacOS68020 = 2;
sMacOS68030 = 3;
sMacOS68040 = 4;


board = 0;
displayVideoAppleTFB = 16843009;
displayVideoAppleGM = 16843010;
networkEtherNetApple3Com = 33620225;
testSimpleAppleAny = -2147417856;
endOfList = 255;
defaultTO = 100;

{Board sResource - Required on all boards}
{Video with Apple parameters for TFB card.}
{Video with Apple parameters for GM card.}
{Ethernet with apple parameters for 3-Comm card.}
{A simple test sResource.}
{End of list}
{100 retries.}

sRsrcType = 1;
sRsrcName = 2;
sRsrcIcon = 3;
sRsrcDrvrDir = 4;
sRsrcLoadDir = 5;

{Type of sResource}
{Name of sResource}
{Icon}
{Driver directory}
{Load directory}



sRsrcBootRec = 6;
sRsrcFlags = 7;
sRsrcHWDevId = 8;
minorBaseOS = 10;
minorLength = 11;
majorBaseOS = 12;
majorLength = 13;
sRsrccicn = 15;
sRsrcicl8 = 16;
sRsrcicl4 = 17;
sGammaDir = 64;
sDRVRDir = 16;

{sBoot record}
{sResource Flags}
{Hardware Device Id}
{Offset to base of sResource in minor space.}
{Length of sResource’s address space in standard slot space.}
{Offset to base of sResource in Major space.}
{Length of sResource in super slot space.}
{Color icon}
{8-bit (indexed) icon}
{4-bit (indexed) icon}
{sGamma directory}
{sDriver directory}

drSwApple = 1;
drHwTFB = 1;
drHw3Com = 1;
drHwBSC = 3;
catBoard = 1;
catTest = 2;
catDisplay = 3;
catNetwork = 4;

{To ask for or define an Apple-compatible SW device.}
{HW ID for the TFB (original Mac II) video card.}
{HW ID for the Apple EtherTalk card.}

boardId = 32;
pRAMInitData = 33;
primaryInit = 34;
timeOutConst = 35;
vendorInfo = 36;
boardFlags = 37;
secondaryInit = 38;
sRsrcVidNames = 65;

{Board Id}
{sPRAM init data}
{Primary init record}
{Time out constant}
{Vendor information List. See Vendor List, below}
{Board Flags}
{Secondary init record/code}
{Video mode name directory}

vendorId = 1;
serialNum = 2;
revLevel = 3;
partNum = 4;
date = 5;

{Vendor Id}
{Serial number}
{Revision level}
{Part number}
{Last revision date of the card}

typeBoard = 0;
typeApple = 1;
typeVideo = 1;
typeEtherNet = 1;
testByte = 32;
testWord = 33;
testLong = 34;
testString = 35;

{Type for board types.}

{$ENDC}

{ UsingROMDefs }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE ROMDefs.p}

{Category
{Category
{Category
{Category

for
for
for
for

board types.}
test types -- not used much.}
display (video) cards.}
Networking cards.}

{Type for video types.}
{Type for ethernet types.}
{Test byte.}
{0021}
{Test Long.}
{Test String.}



{#####################################################################}
{### FILE: RTLib.p}
{#####################################################################}

{
Created: Friday, August 2, 1991 at 11:20 PM
RTLib.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1990-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT RTLib;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingRTLib}
{$SETC UsingRTLib := 1}
{$I+}
{$SETC RTLibIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := RTLibIncludes}

CONST
{
Error Codes
}
eRTNoErr
= 0;
eRTBadVersion
= 2;
eRTInvalidOp
= 4;
eRTInvalidJTPtr = 6;
{
Action Codes
}
kRTSysErr
= 0;
kRTRetry
= 1;
kRTContinue = 2;
{
Runtime Operations
}

	kRTGetVersion         = 10;
	kRTGetVersionA5 = 11;
	kRTGetJTAddress = 12;
	kRTGetJTAddressA5 = 13;
	kRTSetPreLoad = 14;
	kRTSetPreLoadA5 = 15;
	kRTSetSegLoadErr = 16;
	kRTSetSegLoadErrA5    = 17;
	kRTSetPostLoad=18;
	kRTSetPostLoadA5 = 19;
	kRTSetPreUnload = 20;
	kRTSetPreUnloadA5 = 21;
	kRTPreLaunch = 22;
	kRTPostLaunch = 23;

{
Version Definitions
}
kVERSION32BIT
= $FFFF;
kVERSION16BIT
= $0000;

TYPE
{
RTState Definition
}
RTState = RECORD
fVersion:INTEGER;
fSP:Ptr;
fJTAddr:Ptr;
fRegisters: Array[0..14] of LONGINT;
fSegNo:      INTEGER;
fSegType:    ResType;
fSegSize:    LONGINT;
fSegInCore:  BOOLEAN;
fReserved1:  BOOLEAN;
fOSErr:      OSErr;
fReserved2:  LONGINT;
{ run-time version }
{ SP: &-of user return address }
{ PC: &-of called jump table entry }
{ registers D0-D7 and A0-A6 when }
{ _LoadSeg was called }
{segment number }
{ segment type (normally 'CODE') }
{ segment size }
{ true if segment is in memory }
{ (reserved for future use) }
{ error number }
{ (reserved for future use) }

END;
RTStatePtr = ^RTState;
{
Runtime Parameter Block
}
RTParam = (RTGetVersionParam, RTGetJTAddrParam, RTSetSegLoadParam);
RTPB = RECORD
fOperation:
INTEGER;
fA5:
Ptr;
CASE RTParam OF
RTGetVersionParam:
(fVersion: INTEGER);

{ operation }
{ A5-world }

{ run-time version (returned) }



RTGetJTAddrParam:
(fJTAddr:
Ptr;
fCodeAddr: Ptr);

{ ptr to jt entry }

{ code address w/i jt entry (returned) }

RTSetSegLoadParam:
(fUserHdlr:		Ptr;	{ ptr to user handler }
 fOldUserHdlr:	Ptr		{ ptr to old user handler (returned) }
);

END;
RTPBPtr = ^RTPB;

FUNCTION Runtime (runtime_parms: RTPBPtr): OSErr;

{$ENDC}

{ UsingRTLib }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE RTLib.p}



{#####################################################################}
{### FILE: SANE.p}
{#####################################################################}
{
Created: Friday, September 15, 1989 at 5:01 PM
SANE.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1991

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT SANE;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSANE}
{$SETC UsingSANE := 1}
{$I+}
{$SETC SANEIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := SANEIncludes}
{ Elems881 mode set by -d Elems881=true on Pascal command line }
{$IFC UNDEFINED Elems881}
{$SETC Elems881 = FALSE}
{$ENDC}

CONST
{$IFC OPTION(MC68881)}
{*======================================================================*
* The interface specific to the MC68881 SANE library *
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
* The interface specific to the software SANE library
*
*======================================================================*}
Invalid = 1;
Underflow = 2;
Overflow = 4;
DivByZero = 8;
Inexact = 16;
IEEEDefaultEnv = 0;

{IEEE-default floating-point environment constant}

{$ENDC}
{*======================================================================*
* The common interface for the SANE library
*
*======================================================================*}
DecStrLen = 255;
SigDigLen = 20;

{for 68K; use 28 in 6502 SANE}

TYPE
RelOp = (GreaterThan,LessThan,EqualTo,Unordered);
NumClass = (SNaN,QNaN,Infinite,ZeroNum,NormalNum,DenormalNum);
RoundDir = (ToNearest,Upward,Downward,TowardZero);
RoundPre = (ExtPrecision,DblPrecision,RealPrecision);
DecimalKind = (FloatDecimal,FixedDecimal);
{$IFC OPTION(MC68881)}
{*======================================================================*
* The interface specific to the MC68881 SANE library *
*======================================================================*}
Exception = LONGINT;
Environment = RECORD
FPCR: LONGINT;
FPSR: LONGINT;
END;



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
* The interface specific to the software SANE library
*
*======================================================================*}
Exception = INTEGER;
Environment = INTEGER;
Extended96 = ARRAY [0..5] OF INTEGER;
MiscHaltInfo = RECORD
HaltExceptions: INTEGER;
PendingCCR: INTEGER;
PendingD0: LONGINT;
END;
{$ENDC}
{*======================================================================*
* The common interface for the SANE library
*
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



PROCEDURE GetTrapVector(VAR Traps: trapvector);
FUNCTION X96toX80(x: Extended): extended80;
FUNCTION X80toX96(x: extended80): Extended;
{$IFC Elems881 = false}
{ sine }FUNCTION Sin(x: Extended): Extended;
FUNCTION Cos(x: Extended): Extended;
FUNCTION ArcTan(x: Extended): Extended;
FUNCTION Exp(x: Extended): Extended;
FUNCTION Ln(x: Extended): Extended;
FUNCTION Log2(x: Extended): Extended;
FUNCTION Ln1(x: Extended): Extended;
FUNCTION Exp2(x: Extended): Extended;
FUNCTION Exp1(x: Extended): Extended;
FUNCTION Tan(x: Extended): Extended;
{$ENDC}
{$ELSEC}
{ return halt vector }FUNCTION GetHaltVector: LONGINT;
PROCEDURE SetHaltVector(v: LONGINT);
FUNCTION X96toX80(x: Extended96): Extended;
FUNCTION X80toX96(x: Extended): Extended96;
FUNCTION Log2(x: Extended): Extended;
FUNCTION Ln1(x: Extended): Extended;
FUNCTION Exp2(x: Extended): Extended;
FUNCTION Exp1(x: Extended): Extended;
FUNCTION Tan(x: Extended): Extended;
{$ENDC}

{*======================================================================*
* The common interface for the SANE library
*
*======================================================================*}
{--------------------------------------------------* Conversions between numeric binary types.
---------------------------------------------------}
FUNCTION Num2Integer(x: Extended): INTEGER;
FUNCTION Num2Longint(x: Extended): LONGINT;
FUNCTION Num2Real(x: Extended): real;
FUNCTION Num2Double(x: Extended): DOUBLE;
FUNCTION Num2Extended(x: Extended): Extended;
FUNCTION Num2Comp(x: Extended): Comp;
PROCEDURE Num2Dec(f: decform;x: Extended;VAR d: decimal);
FUNCTION Dec2Num(d: decimal): Extended;
PROCEDURE Num2Str(f: decform;x: Extended;VAR s: DecStr);
FUNCTION Str2Num(s: DecStr): Extended;
PROCEDURE Str2Dec(s: DecStr;VAR Index: INTEGER;VAR d: decimal;VAR ValidPrefix: BOOLEAN);
PROCEDURE CStr2Dec(s: CStrPtr;VAR Index: INTEGER;VAR d: decimal;VAR ValidPrefix: BOOLEAN);
PROCEDURE Dec2Str(f: decform;d: decimal;VAR s: DecStr);
FUNCTION Remainder(x: Extended;y: Extended;VAR quo: INTEGER): Extended;
FUNCTION Rint(x: Extended): Extended;
FUNCTION Scalb(n: INTEGER;x: Extended): Extended;
FUNCTION Logb(x: Extended): Extended;



FUNCTION CopySign(x: Extended;y: Extended): Extended;
FUNCTION NextReal(x: real;y: real): real;
FUNCTION NextDouble(x: DOUBLE;y: DOUBLE): DOUBLE;
FUNCTION NextExtended(x: Extended;y: Extended): Extended;
FUNCTION XpwrI(x: Extended;i: INTEGER): Extended;
FUNCTION XpwrY(x: Extended;y: Extended): Extended;
FUNCTION Compound(r: Extended;n: Extended): Extended;
FUNCTION Annuity(r: Extended;n: Extended): Extended;
FUNCTION RandomX(VAR x: Extended): Extended;
FUNCTION ClassReal(x: real): NumClass;
FUNCTION ClassDouble(x: DOUBLE): NumClass;
FUNCTION ClassComp(x: Comp): NumClass;
FUNCTION ClassExtended(x: Extended): NumClass;
FUNCTION SignNum(x: Extended): INTEGER;
FUNCTION NAN(i: INTEGER): Extended;
PROCEDURE SetException(e: Exception;b: BOOLEAN);
FUNCTION TestException(e: Exception): BOOLEAN;
PROCEDURE SetHalt(e: Exception;b: BOOLEAN);
FUNCTION TestHalt(e: Exception): BOOLEAN;
PROCEDURE SetRound(r: RoundDir);
FUNCTION GetRound: RoundDir;
PROCEDURE SetPrecision(p: RoundPre);
FUNCTION GetPrecision: RoundPre;
PROCEDURE SetEnvironment(e: environment);
PROCEDURE GetEnvironment(VAR e: environment);
PROCEDURE ProcEntry(VAR e: environment);
PROCEDURE ProcExit(e: environment);
FUNCTION Relation(x: Extended;y: Extended): RelOp;
{$ENDC}

{ UsingSANE }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE SANE.p}



{#####################################################################}
{### FILE: Scrap.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:07 PM
Scrap.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1989

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Scrap;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingScrap}
{$SETC UsingScrap := 1}
{$I+}
{$SETC ScrapIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := ScrapIncludes}
TYPE
PScrapStuff = ^ScrapStuff;
ScrapStuff = RECORD
scrapSize: LONGINT;
scrapHandle: Handle;
scrapCount: INTEGER;
scrapState: INTEGER;
scrapName: StringPtr;
END;

FUNCTION InfoScrap: PScrapStuff;
INLINE $A9F9;
FUNCTION UnloadScrap: LONGINT;
INLINE $A9FA;
FUNCTION LoadScrap: LONGINT;
INLINE $A9FB;
FUNCTION GetScrap(hDest: Handle;theType: ResType;VAR offset: LONGINT): LONGINT;
INLINE $A9FD;
FUNCTION ZeroScrap: LONGINT;
INLINE $A9FC;



FUNCTION PutScrap(length: LONGINT;theType: ResType;source: Ptr): LONGINT;
INLINE $A9FE;

{$ENDC}

{ UsingScrap }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Scrap.p}



{#####################################################################}
{### FILE: Script.p}
{#####################################################################}

{
Created: Tuesday, January 15, 1991 at 8:56 AM
Script.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1986-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Script;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingScript}
{$SETC UsingScript := 1}
{$I+}
{$SETC ScriptIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := ScriptIncludes}
CONST
{ Script System constants }
smSystemScript = -1;
smCurrentScript = -2;
smAllScripts = -3;
smRoman = 0;
smJapanese = 1;
smTradChinese = 2;
smKorean = 3;
smArabic = 4;
smHebrew = 5;
smGreek = 6;
smCyrillic = 7;

{designates system script.}
{designates current font script.}
{designates any script.}
{Roman}
{Japanese}
{Traditional Chinese}
{Korean}
{Arabic}
{Hebrew}
{Greek}
{Cyrillic}

smRSymbol = 8;
smDevanagari = 9;
smGurmukhi = 10;
smGujarati = 11;
smOriya = 12;
smBengali = 13;
smTamil = 14;
smTelugu = 15;
smKannada = 16;
smMalayalam = 17;
smSinhalese = 18;
smBurmese = 19;
smKhmer = 20;
smThai = 21;
smLaotian = 22;
smGeorgian = 23;
smArmenian = 24;
smSimpChinese = 25;
smTibetan = 26;
smMongolian = 27;
smGeez = 28;
smEthiopic = 28;
smEastEurRoman = 29;
smVietnamese = 30;
smExtArabic = 31;
smUninterp = 32;


{Right-left symbol}
{Devanagari}
{Gurmukhi}
{Gujarati}
{Oriya}
{Bengali}
{Tamil}
{Telugu}
{Kannada/Kanarese}
{Malayalam}
{Sinhalese}
{Burmese}
{Khmer/Cambodian}
{Thai}
{Laotian}
{Georgian}
{Armenian}
{Simplified Chinese}
{Tibetan}
{Mongolian}
{Geez/Ethiopic}
{Synonym for smGeez}
{Synonym for smSlavic}
{Vietnamese}
{extended Arabic}
{uninterpreted symbols, e.g. palette symbols}

{Obsolete names for script systems (kept for backward compatibility)}
smChinese = 2;
{(use smTradChinese or smSimpChinese)}
smRussian = 7;
{(old name for smCyrillic)}
{ smMaldivian = 25;
smAmharic = 28;
smSlavic = 29;
smSindhi = 31;
{ Calendar Codes }
calGregorian = 0;
calArabicCivil = 1;
calArabicLunar = 2;
calJapanese = 3;
calJewish = 4;
calCoptic = 5;
calPersian = 6;
{ Integer Format Codes }
intWestern = 0;
intArabic = 1;
intRoman = 2;
intJapanese = 3;
intEuropean = 4;
intOutputMask = $8000;
{ CharByte byte types }
smSingleByte = 0;
smFirstByte = -1;

(no more smMaldivian!)}
{(old name for smGeez)}
{(old name for smEastEurRoman)}
{(old name for smExtArabic)}



smLastByte = 1;
smMiddleByte = 2;
{ CharType field masks }
smcTypeMask = $000F;
smcReserved = $00F0;
smcClassMask = $0F00;
smcOrientationMask = $1000;
smcRightMask = $2000;
smcUpperMask = $4000;
smcDoubleMask = $8000;
{ Basic CharType character types }
smCharPunct = $0000;
smCharAscii = $0001;
smCharEuro = $0007;
smCharExtAscii = $0007;

{two-byte script glyph orientation}

{ More correct synonym for smCharEuro }

{ Additional CharType character types for script systems }
smCharKatakana = $0002;
{Japanese Katakana}
smCharHiragana = $0003;
{Japanese Hiragana}
smCharIdeographic = $0004;
{Hanzi, Kanji, Hanja}
smCharTwoByteGreek = $0005;
{2-byte Greek in Far East systems}
smCharTwoByteRussian = $0006;
{2-byte Cyrillic in Far East systems}
smCharBidirect = $0008;
{Arabic/Hebrew}
smCharHangul = $000C;
{Korean Hangul}
smCharJamo = $000D;
{Korean Jamo}
{ old names for some of above, for backward compatibility }
smCharFISKana = $0002;
{Katakana}
smCharFISGana = $0003;
{Hiragana}
smCharFISIdeo = $0004;
{Hanzi, Kanji, Hanja}
smCharFISGreek = $0005;
{2-byte Greek in Far East systems}
smCharFISRussian = $0006;
{2-byte Cyrillic in Far East systems}
{ CharType classes for punctuation (smCharPunct) }
smPunctNormal = $0000;
smPunctNumber = $0100;
smPunctSymbol = $0200;
smPunctBlank = $0300;
{ Additional CharType classes for punctuation in two-byte systems }
smPunctRepeat = $0400;
{ repeat marker }
smPunctGraphic = $0500;
{ line graphics }
{ CharType Katakana and Hiragana classes for two-byte systems }
smKanaSmall = $0100;
{small kana character}
smKanaHardOK = $0200;
{can have dakuten}
smKanaSoftOK = $0300;
{can have dakuten or han-dakuten}
{ CharType Ideographic classes for two-byte systems }
smIdeographicLevel1 = $0000;
{level 1 char}
smIdeographicLevel2 = $0100;
{level 2 char}
smIdeographicUser = $0200;
{user char}
{ old names for above, for backward compatibility }



smFISClassLvl1 = $0000;
smFISClassLvl2 = $0100;
smFISClassUser = $0200;

{level 1 char}
{level 2 char}
{user char}

{ CharType Jamo classes for Korean systems }
smJamoJaeum = $0000;
smJamoBogJaeum = $0100;
smJamoMoeum = $0200;
smJamoBogMoeum = $0300;

{simple consonant char}
{complex consonant char}
{simple vowel char}
{complex vowel char}

{ CharType glyph orientation for two-byte systems }
smCharHorizontal = $0000;
{ horizontal character form, or for both }
smCharVertical = $1000;
{ vertical character form }
{ CharType directions }
smCharLeft = $0000;
smCharRight = $2000;
{ CharType case modifers }
smCharLower = $0000;
smCharUpper = $4000;
{ CharType character size modifiers (1 or multiple bytes). }
smChar1byte = $0000;
smChar2byte = $8000;
{ Char2Pixel directions }
smLeftCaret = 0;
smRightCaret = -1;
smHilite = 1;

{Place caret for left block}
{Place caret for right block}
{Direction is TESysJust}

{ Transliterate target types for Roman }
smTransAscii = 0;
smTransNative = 1;
smTransCase = $FE;
smTransSystem = $FF;

{convert
{convert
{convert
{convert

{ Transliterate target types for two-byte scripts }
smTransAscii1 = 2;
{1-byte
smTransAscii2 = 3;
{2-byte
smTransKana1 = 4;
{1-byte
smTransKana2 = 5;
{2-byte
smTransGana2 = 7;
{2-byte
smTransHangul2 = 8;
{2-byte
smTransJamo2 = 9;
{2-byte
smTransBopomofo2 = 10;
{2-byte

to ASCII}
to font script}
case for all text}
to system script}

Roman}
Roman}
Japanese Katakana}
Japanese Katakana}
Japanese Hiragana (no 1-byte Hiragana)}
Korean Hangul}
Korean Jamo}
Chinese Bopomofo}

{ Transliterate target modifiers }
smTransLower = $4000;
smTransUpper = $8000;

{target becomes lowercase}
{target becomes uppercase}

{ Transliterate source mask - general }
smMaskAll = $FFFFFFFF;

{Convert all text}

{ Transliterate source masks }
smMaskAscii = $00000001;

{2^smTransAscii}



smMaskNative = $00000002;

{2^smTransNative}

{ Transliterate source masks for two-byte scripts }
smMaskAscii1 = $00000004;
{2^smTransAscii1}
smMaskAscii2 = $00000008;
{2^smTransAscii2}
smMaskKana1 = $00000010;
{2^smTransKana1}
smMaskKana2 = $00000020;
{2^smTransKana2}
smMaskGana2 = $00000080;
{2^smTransGana2}
smMaskHangul2 = $00000100;
{2^smTransHangul2}
smMaskJamo2 = $00000200;
{2^smTransJamo2}
smMaskBopomofo2 = $00000400;
{2^smTransBopomofo2}
{ Result values from GetEnvirons, SetEnvirons, GetScript and SetScript calls. }
smNotInstalled = 0;
{routine not available in script}
smBadVerb = -1;
{Bad verb passed to a routine}
smBadScript = -2;
{Bad script code passed to a routine}
{ Values for
script redraw flag. }
smRedrawChar = 0;
smRedrawWord = 1;
smRedrawLine = -1;

{Redraw character only}
{Redraw entire word (2-byte systems)}
{Redraw entire line (bidirectional systems)}

{ GetEnvirons and SetEnvirons verbs }
smVersion = 0;
smMunged = 2;
smEnabled = 4;
smBidirect = 6;
smFontForce = 8;
smIntlForce = 10;
smForced = 12;
smDefault = 14;
smPrint = 16;
smSysScript = 18;
smLastScript = 20;
smKeyScript = 22;
smSysRef = 24;
smKeyCache = 26;
smKeySwap = 28;
smGenFlags = 30;
smOverride = 32;
smCharPortion = 34;

{Script Manager version number}
{Globals change count}
{Count of enabled scripts, incl Roman}
{At least one bidirectional script}
{Force font flag}
{Force intl flag}
{Script was forced to system script}
{Script was defaulted to Roman script}
{Printer action routine}
{System script}
{Last keyboard script}
{Keyboard script}
{System folder refNum}
{obsolete}
{Swapping table handle}
{General flags long}
{Script override flags}
{Ch vs SpExtra proportion}

{ New for System 7.0: }
smDoubleByte = 36;
smKCHRCache = 38;
smRegionCode = 40;

{Flag for double-byte script installed}
{Returns pointer to KCHR cache}
{Returns current region code (verXxx)}

{ GetScript and SetScript verbs.
Note: Verbs private to script systems are negative, while
those general across script systems are non-negative. }
smScriptVersion = 0;
{Script software version}
smScriptMunged = 2;
{Script entry changed count}
smScriptEnabled = 4;
{Script enabled flag}
smScriptRight = 6;
{Right to left flag}
smScriptJust = 8;
{Justification flag}
smScriptRedraw = 10;
{Word redraw flag}

smScriptSysFond = 12;
smScriptAppFond = 14;
smScriptBundle = 16;
smScriptNumber = 16;
smScriptDate = 18;
smScriptSort = 20;
smScriptFlags = 22;
smScriptToken = 24;
smScriptEncoding = 26;
smScriptLang = 28;
smScriptNumDate = 30;
smScriptKeys = 32;
smScriptIcon = 34;
smScriptPrint = 36;
smScriptTrap = 38;
smScriptCreator = 40;
smScriptFile = 42;
smScriptName = 44;


{Preferred system font}
{Preferred Application font}
{Beginning of itlb verbs}
{Script itl0 id}
{Script itl1 id}
{Script itl2 id}
{flags word}
{Script itl4 id}
{id of optional itl5, if present}
{Current language for script}
{Script Number/Date formats.}
{Script KCHR id}
{ID # of SICN or kcs#/kcs4/kcs8 suite}
{Script printer action routine}
{Trap entry pointer}
{Script file creator}
{Script file name}
{Script name}

{ There is a hole here for old Kanji private verbs 46-76
New for System 7.0: }
smScriptMonoFondSize = 78;
smScriptPrefFondSize = 80;
smScriptSmallFondSize = 82;
smScriptSysFondSize = 84;
smScriptAppFondSize = 86;
smScriptHelpFondSize = 88;
smScriptValidStyles = 90;
smScriptAliasStyle = 92;

{default monospace FOND (hi) & size (lo)}
{preferred FOND (hi) & size (lo)}
{default small FOND (hi) & size (lo)}
{default system FOND (hi) & size (lo)}
{default app FOND (hi) & size (lo)}
{default Help Mgr FOND (hi) & size (lo)}
{mask of valid styles for script}
{style (set) to use for aliases}

{ Negative verbs for KeyScript }
smKeyNextScript = -1;
smKeySysScript = -2;
smKeySwapScript = -3;

{ Switch to next available script }
{ Switch to the system script }
{ Switch to previously-used script }

{ New for System 7.0: }
smKeyNextKybd = -4;
smKeySwapKybd = -5;

{ Switch to next keyboard in current keyscript }
{ Switch to previously-used keyboard in current keyscript }

smKeyDisableKybds = -6;
smKeyEnableKybds = -7;
smKeyToggleInline = -8;
smKeyToggleDirection = -9;
smKeyNextInputMethod = -10;
smKeySwapInputMethod = -11;

{
{
{
{
{
{

smKeyDisableKybdSwitch = -12;

{ Disable switching from the current keyboard }

{ Bits in the smScriptFlags word
(bits above 7 are non-static) }
smsfIntellCP = 0;
smsfSingByte = 1;
smsfNatCase = 2;
smsfContext = 3;

{Script
{Script
{Native
{Script

Disable keyboards not in system or Roman script }
Re-enable keyboards for all enabled scripts }
Toggle inline input for current keyscript }
Toggle default line direction (TESysJust) }
Switch to next input method in current keyscript }
Switch to last-used input method in current keyscript }

has intelligent cut & paste}
has only single bytes}
chars have upper & lower case}
is contextual}

smsfNoForceFont = 4;
smsfB0Digits = 5;
smsfAutoInit = 6;
smsfForms = 13;
smsfLigatures = 14;
smsfReverse = 15;


{Script will not force characters}
{Script has alternate digits at B0-B9}
{Auto initialize the script}
{Uses contextual forms for letters}
{Uses contextual ligatures}
{Reverses native text, right-left}

{ Bits in the smGenFlags long.
First (high-order) byte is set from itlc flags byte. }
smfShowIcon = 31;
{Show icon even if only one script}
smfDualCaret = 30;
{Use dual caret for mixed direction text}
smfNameTagEnab = 29;
{Reserved for internal use}
{ Roman script constants
The following are here for backward compatibility, but should not be used.
This information should be obtained using GetScript. }
romanSysFond = $3FFF;
{system font id number}
romanAppFond = 3;
{application font id number}
romanFlags = $0007;
{roman settings}
{ Script Manager font equates. }
smFondStart = $4000;
smFondEnd = $C000;

{start from 16K}
{past end of range at 48K}

{ Miscellaneous font equates. }
smUprHalfCharSet = $80;

{first char code in top half of std char set}

{ Character Set Extensions }
diaeresisUprY = $D9;
fraction = $DA;
intlCurrency = $DB;
leftSingGuillemet = $DC;
rightSingGuillemet = $DD;
fiLigature = $DE;
flLigature = $DF;
dblDagger = $E0;
centeredDot = $E1;
baseSingQuote = $E2;
baseDblQuote = $E3;
perThousand = $E4;
circumflexUprA = $E5;
circumflexUprE = $E6;
acuteUprA = $E7;
diaeresisUprE = $E8;
graveUprE = $E9;
acuteUprI = $EA;
circumflexUprI = $EB;
diaeresisUprI = $EC;
graveUprI = $ED;
acuteUprO = $EE;
circumflexUprO = $EF;
appleLogo = $F0;
graveUprO = $F1;
acuteUprU = $F2;
circumflexUprU = $F3;



graveUprU = $F4;
dotlessLwrI = $F5;
circumflex = $F6;
tilde = $F7;
macron = $F8;
breveMark = $F9;
overDot = $FA;
ringMark = $FB;
cedilla = $FC;
doubleAcute = $FD;
ogonek = $FE;
hachek = $FF;
{ String2Date status values }
fatalDateTime = $8000;
longDateFound = 1;
leftOverChars = 2;
sepNotIntlSep = 4;
fieldOrderNotIntl = 8;
extraneousStrings = 16;
tooManySeps = 32;
sepNotConsistent = 64;
tokenErr = $8100;
cantReadUtilities = $8200;
dateTimeNotFound = $8400;
dateTimeInvalid = $8800;
{ TokenType values }
tokenIntl = 4;
tokenEmpty = -1;
tokenUnknown = 0;
tokenWhite = 1;
tokenLeftLit = 2;
tokenRightLit = 3;
tokenAlpha = 4;
tokenNumeric = 5;
tokenNewLine = 6;
tokenLeftComment = 7;
tokenRightComment = 8;
tokenLiteral = 9;
tokenEscape = 10;
tokenAltNum = 11;
tokenRealNum = 12;
tokenAltReal = 13;
tokenReserve1 = 14;
tokenReserve2 = 15;
tokenLeftParen = 16;
tokenRightParen = 17;
tokenLeftBracket = 18;
tokenRightBracket = 19;
tokenLeftCurly = 20;
tokenRightCurly = 21;
tokenLeftEnclose = 22;
tokenRightEnclose = 23;
tokenPlus = 24;
tokenMinus = 25;

{String2Date
{String2Date
{String2Date
{String2Date
{String2Date
{String2Date
{String2Date
{String2Date
{String2Date

and String2Time mask to a fatal error}
mask to long date found}
& Time mask to warn of left over characters}
& Time mask to warn of non-standard separators}
& Time mask to warn of non-standard field order}
& Time mask to warn of unparsable strings in text}
& Time mask to warn of too many separators}
& Time mask to warn of inconsistent separators}
& Time mask for 'tokenizer err encountered'}

{the itl resource number of the tokenizer}
{used internally as an empty flag}
{chars that do not match a defined token type}
{white space}
{literal begin}
{literal end}
{alphabetic}
{numeric}
{new line}
{open comment}
{close comment}
{literal}
{character escape (e.g. '\' in "\n", "\t")}
{alternate number (e.g. $B0-B9 in Arabic,Hebrew)}
{real number}
{alternate real number}
{reserved}
{reserved}
{open parenthesis}
{close parenthesis}
{open square bracket}
{close square bracket}
{open curly bracket}
{close curly bracket}
{open guillemet}
{close guillemet}

tokenAsterisk = 26;
tokenDivide = 27;
tokenPlusMinus = 28;
tokenSlash = 29;
tokenBackSlash = 30;
tokenLess = 31;
tokenGreat = 32;
tokenEqual = 33;
tokenLessEqual2 = 34;
tokenLessEqual1 = 35;
tokenGreatEqual2 = 36;
tokenGreatEqual1 = 37;
token2Equal = 38;
tokenColonEqual = 39;
tokenNotEqual = 40;
tokenLessGreat = 41;
tokenExclamEqual = 42;
tokenExclam = 43;
tokenTilde = 44;
tokenComma = 45;
tokenPeriod = 46;
tokenLeft2Quote = 47;
tokenRight2Quote = 48;
tokenLeft1Quote = 49;
tokenRight1Quote = 50;
token2Quote = 51;
token1Quote = 52;
tokenSemicolon = 53;
tokenPercent = 54;
tokenCaret = 55;
tokenUnderline = 56;
tokenAmpersand = 57;
tokenAtSign = 58;
tokenBar = 59;
tokenQuestion = 60;
tokenPi = 61;
tokenRoot = 62;
tokenSigma = 63;
tokenIntegral = 64;
tokenMicro = 65;
tokenCapPi = 66;
tokenInfinity = 67;
tokenColon = 68;
tokenHash = 69;
tokenDollar = 70;
tokenNoBreakSpace = 71;
tokenFraction = 72;
tokenIntlCurrency = 73;
tokenLeftSingGuillemet = 74;
tokenRightSingGuillemet = 75;
tokenPerThousand = 76;
tokenEllipsis = 77;
tokenCenterDot = 78;
tokenNil = 127;
delimPad = -2;


{times/multiply}
{plus or minus symbol}

{less than symbol}
{greater than symbol}
{less than or equal, 2 characters (e.g. <=)}
{less than or equal, 1 character}
{greater than or equal, 2 characters (e.g. >=)}
{greater than or equal, 1 character}
{double equal (e.g. ==)}
{colon equal}
{not equal, 1 character}
{less/greater, Pascal not equal (e.g. <>)}
{exclamation equal, C not equal (e.g. !=)}
{exclamation point}
{centered tilde}

{open double quote}
{close double quote}
{open single quote}
{close single quote}
{double quote}
{single quote}

{vertical bar}
{lower-case pi}
{square root symbol}
{capital sigma}
{integral sign}
{capital pi}

{e.g. #}
{non-breaking space}



{ obsolete, misspelled token names kept for backward compatibility }
tokenTilda = 44;
tokenCarat = 55;
{ the NumberParts indices }
tokLeftQuote = 1;
tokRightQuote = 2;
tokLeadPlacer = 3;
tokLeader = 4;
tokNonLeader = 5;
tokZeroLead = 6;
tokPercent = 7;
tokPlusSign = 8;
tokMinusSign = 9;
tokThousands = 10;
tokSeparator = 12;
tokEscape = 13;
tokDecPoint = 14;
tokEPlus = 15;
tokEMinus = 16;
tokMaxSymbols = 31;
curNumberPartsVersion = 1;
fVNumber = 0;
{ Date equates }
smallDateBit = 31;
togChar12HourBit = 30;
togCharZCycleBit = 29;
togDelta12HourBit = 28;
genCdevRangeBit = 27;
validDateFields = -1;
maxDateField = 10;

{11 is a reserved field}

{current version of NumberParts record}
{first version of NumFormatString}

{Restrict valid date/time to range of Time global}
{If toggling hour by char, accept hours 1..12 only}
{Modifier for togChar12HourBit: accept hours 0..11 only}
{If toggling hour up/down, restrict to 12-hour range (am/pm)}
{Restrict date/time to range used by genl CDEV}

eraMask = $0001;
yearMask = $0002;
monthMask = $0004;
dayMask = $0008;
hourMask = $0010;
minuteMask = $0020;
secondMask = $0040;
dayOfWeekMask = $0080;
dayOfYearMask = $0100;
weekOfYearMask = $0200;
pmMask = $0400;
dateStdMask = $007F;

{default for ValidDate flags and ToggleDate TogglePB.togFlags}

{ Toggle results }
toggleUndefined = 0;
toggleOK = 1;
toggleBadField = 2;
toggleBadDelta = 3;
toggleBadChar = 4;
toggleUnknown = 5;
toggleBadNum = 6;
toggleOutOfRange = 7;

{synonym for toggleErr3}



toggleErr3 = 7;
toggleErr4 = 8;
toggleErr5 = 9;
{ New constants for System 7.0:
Constants for truncWhere argument in TruncString and TruncText }
smTruncEnd = 0;
{ Truncate at end }
smTruncMiddle = $4000;
{ Truncate in middle }
{ Constants for TruncString and TruncText results }
smNotTruncated = 0;
{ No truncation was necessary }
smTruncated = 1;
{ Truncation performed }
smTruncErr = -1;
{ General error }
{Constants for styleRunPosition argument in NPortionText, NDrawJust,
NMeasureJust, NChar2Pixel, and NPixel2Char.}
smOnlyStyleRun = 0;
{ This is the only style run on the line }
smLeftStyleRun = 1;
{ This is leftmost of multiple style runs on the line }
smRightStyleRun = 2;
{ This is rightmost of multiple style runs on the line }
smMiddleStyleRun = 3;
{ There are multiple style runs on the line and this
is neither the leftmost nor the rightmost. }
TYPE
TokenResults = (tokenOK,tokenOverflow,stringOverflow,badDelim,badEnding,
crash);
LongDateField = (eraField,yearField,monthField,dayField,hourField,minuteField,
secondField,dayOfWeekField,dayOfYearField,weekOfYearField,pmField,res1Field,
res2Field,res3Field);
StyledLineBreakCode = (smBreakWord,smBreakChar,smBreakOverflow);
FormatClass = (fPositive,fNegative,fZero);
FormatResultType = (fFormatOK,fBestGuess,fOutOfSynch,fSpuriousChars,fMissingDelimiter,
fExtraDecimal,fMissingLiteral,fExtraExp,fFormatOverflow,fFormStrIsNAN,
fBadPartsTable,fExtraPercent,fExtraSeparator,fEmptyFormatString);

CharByteTable = PACKED ARRAY [0..255] OF SignedByte;
ToggleResults = INTEGER;
BreakTablePtr = ^BreakTable;
BreakTable = RECORD
charTypes: ARRAY [0..255] OF SignedByte;
tripleLength: INTEGER;
triples: ARRAY [0..0] OF INTEGER;
END;
{ New NBreakTable for System 7.0: }
NBreakTablePtr = ^NBreakTable;
NBreakTable = RECORD
flags1: SignedByte;
flags2: SignedByte;
version: INTEGER;



classTableOff: INTEGER;
auxCTableOff: INTEGER;
backwdTableOff: INTEGER;
forwdTableOff: INTEGER;
doBackup: INTEGER;
reserved: INTEGER;
charTypes: ARRAY [0..255] OF SignedByte;
tables: ARRAY [0..0] OF INTEGER;
END;
OffPair = RECORD
offFirst: INTEGER;
offSecond: INTEGER;
END;

OffsetTable = ARRAY [0..2] OF OffPair;
ItlcRecord = RECORD
itlcSystem: INTEGER;
itlcReserved: INTEGER;
itlcFontForce: SignedByte;
itlcIntlForce: SignedByte;
itlcOldKybd: SignedByte;
itlcFlags: SignedByte;
itlcIconOffset: INTEGER;
itlcIconSide: SignedByte;
itlcIconRsvd: SignedByte;
itlcRegionCode: INTEGER;
itlcReserved3: ARRAY [0..33] OF SignedByte;
END;

{default system script}
{reserved}
{default font force flag}
{default intl force flag}
{MacPlus intl keybd flag}
{general flags}
{keyboard icon offset; not used in 7.0}
{keyboard icon side; not used in 7.0}
{rsvd for other icon info}
{preferred verXxx code}
{for future use}

ItlbRecord = RECORD
itlbNumber: INTEGER;
itlbDate: INTEGER;
itlbSort: INTEGER;
itlbFlags: INTEGER;
itlbToken: INTEGER;
itlbEncoding: INTEGER;
itlbLang: INTEGER;
itlbNumRep: SignedByte;
itlbDateRep: SignedByte;
itlbKeys: INTEGER;
itlbIcon: INTEGER;
END;

{itl0 id number}
{itl1 id number}
{itl2 id number}
{Script flags}
{itl4 id number}
{itl5 ID # (optional; char encoding)}
{current language for script }
{number representation code}
{date representation code }
{KCHR id number}
{ID # of SICN or kcs#/kcs4/kcs8 suite.}

{ New ItlbExtRecord structure for System 7.0 }
ItlbExtRecord = RECORD
base: ItlbRecord;
itlbLocalSize: LONGINT;
itlbMonoFond: INTEGER;
itlbMonoSize: INTEGER;
itlbPrefFond: INTEGER;
itlbPrefSize: INTEGER;
itlbSmallFond: INTEGER;
itlbSmallSize: INTEGER;

{un-extended ItlbRecord}
{size of script's local record}
{default monospace FOND ID}
{default monospace font size}
{preferred FOND ID}
{preferred font size}
{default small FOND ID}
{default small font size}



itlbSysFond: INTEGER;
itlbSysSize: INTEGER;
itlbAppFond: INTEGER;
itlbAppSize: INTEGER;
itlbHelpFond: INTEGER;
itlbHelpSize: INTEGER;
itlbValidStyles: Style;
itlbAliasStyle: Style;
END;
MachineLocation = RECORD
latitude: Fract;
longitude: Fract;
CASE INTEGER OF
0:
(dlsDelta: SignedByte);
1:
(gmtDelta: LONGINT);
END;

{default system FOND ID}
{default system font size}
{default application FOND ID}
{default application font size}
{default Help Mgr FOND ID}
{default Help Mgr font size}
{set of valid styles for script}
{style (set) to mark aliases}

{signed byte; daylight savings delta}
{must mask - see documentation}

String2DateStatus = INTEGER;
TokenType = INTEGER;
DelimType = ARRAY [0..1] OF TokenType;
CommentType = ARRAY [0..3] OF TokenType;
TokenRecPtr = ^TokenRec;
TokenRec = RECORD
theToken: TokenType;
position: Ptr;
length: LONGINT;
stringPosition: StringPtr;
END;
TokenBlockPtr = ^TokenBlock;
TokenBlock = RECORD
source: Ptr;
sourceLength: LONGINT;
tokenList: Ptr;
tokenLength: LONGINT;
tokenCount: LONGINT;
stringList: Ptr;
stringLength: LONGINT;
stringCount: LONGINT;
doString: BOOLEAN;
doAppend: BOOLEAN;
doAlphanumeric: BOOLEAN;
doNest: BOOLEAN;
leftDelims: ARRAY [0..1] OF TokenType;
rightDelims: ARRAY [0..1] OF TokenType;
leftComment: ARRAY [0..3] OF TokenType;
rightComment: ARRAY [0..3] OF TokenType;
escapeCode: TokenType;
decimalCode: TokenType;
itlResource: Handle;
reserved: ARRAY [0..7] OF LONGINT;

{pointer into original source}
{length of text in original source}
{Pascal/C string copy of identifier}

{pointer to stream of characters}
{length of source stream}
{pointer to array of tokens}
{maximum length of TokenList}
{number tokens generated by tokenizer}
{pointer to stream of identifiers}
{length of string list}
{number of bytes currently used}
{make strings & put into StringList}
{append to TokenList rather than replace}
{identifiers may include numeric}
{do comments nest?}

{escape symbol code}
{handle to itl4 resource of current script}
{must be zero!}



END;
UntokenTablePtr = ^UntokenTable;
UntokenTableHandle = ^UntokenTablePtr;
UntokenTable = RECORD
len: INTEGER;
lastToken: INTEGER;
index: ARRAY [0..255] OF INTEGER;
END;
DateCachePtr = ^DateCacheRecord;
DateCacheRecord = PACKED RECORD
hidden: ARRAY [0..255] OF INTEGER;
END;

{index table; last = lastToken}

{only for temporary use}

LongDateTime = comp;
LongDateCvt = RECORD
CASE INTEGER OF
0:
(c: Comp);
1:
(lHigh: LONGINT;
lLow: LONGINT);
END;
LongDateRec = RECORD
CASE INTEGER OF
0:
(era: INTEGER;
year: INTEGER;
month: INTEGER;
day: INTEGER;
hour: INTEGER;
minute: INTEGER;
second: INTEGER;
dayOfWeek: INTEGER;
dayOfYear: INTEGER;
weekOfYear: INTEGER;
pm: INTEGER;
res1: INTEGER;
res2: INTEGER;
res3: INTEGER);
1:
(list: ARRAY [0..13] OF INTEGER);
2:
(eraAlt: INTEGER;
oldDate: DateTimeRec);
END;

{Index by LongDateField!}

DateDelta = SignedByte;
TogglePB = RECORD
togFlags: LONGINT;

{caller normally sets low word to dateStdMask=$7F}



amChars: ResType;
pmChars: ResType;
reserved: ARRAY [0..3] OF LONGINT;
END;

{from 'itl0', but uppercased}
{from 'itl0', but uppercased}

FormatOrder = ARRAY [0..0] OF INTEGER;
FormatOrderPtr = ^FormatOrder;
FormatStatus = INTEGER;
WideChar = RECORD
CASE BOOLEAN OF
TRUE:
(a: PACKED ARRAY [0..1] OF CHAR);
FALSE:
(b: INTEGER);
END;

{0 is the high order character}

WideCharArr = RECORD
size: INTEGER;
data: PACKED ARRAY [0..9] OF WideChar;
END;
NumFormatString = PACKED RECORD
fLength: Byte;
fVersion: Byte;
data: PACKED ARRAY [0..253] OF SignedByte;
END;
Itl4Ptr = ^Itl4Rec;
Itl4Handle = ^Itl4Ptr;
Itl4Rec = RECORD
flags: INTEGER;
resourceType: LONGINT;
resourceNum: INTEGER;
version: INTEGER;
resHeader1: LONGINT;
resHeader2: LONGINT;
numTables: INTEGER;
mapOffset: LONGINT;
strOffset: LONGINT;
fetchOffset: LONGINT;
unTokenOffset: LONGINT;
defPartsOffset: LONGINT;
resOffset6: LONGINT;
resOffset7: LONGINT;
resOffset8: LONGINT;
END;
{ New NItl4Rec for System 7.0: }
NItl4Ptr = ^NItl4Rec;
NItl4Handle = ^NItl4Ptr;
NItl4Rec = RECORD
flags: INTEGER;
resourceType: LONGINT;
resourceNum: INTEGER;

{private data}

{reserved}
{contains 'itl4'}
{resource ID}
{version number}
{reserved}
{reserved}
{number of tables, one-based}
{offset to table that maps byte to token}
{offset to routine that copies canonical string}
{offset to routine that gets next byte of character}
{offset to table that maps token to canonical string}
{offset to default number parts table}
{reserved}
{reserved}
{reserved}

{reserved}
{contains 'itl4'}
{resource ID}



version: INTEGER;
format: INTEGER;
resHeader: INTEGER;
resHeader2: LONGINT;
numTables: INTEGER;
mapOffset: LONGINT;
strOffset: LONGINT;
fetchOffset: LONGINT;
unTokenOffset: LONGINT;
defPartsOffset: LONGINT;
whtSpListOffset: LONGINT;
resOffset7: LONGINT;
resOffset8: LONGINT;
resLength1: INTEGER;
resLength2: INTEGER;
resLength3: INTEGER;
unTokenLength: INTEGER;
defPartsLength: INTEGER;
whtSpListLength: INTEGER;
resLength7: INTEGER;
resLength8: INTEGER;
END;
NumberPartsPtr = ^NumberParts;
NumberParts = RECORD
version: INTEGER;
data: ARRAY [1..31] OF WideChar;
pePlus: WideCharArr;
peMinus: WideCharArr;
peMinusPlus: WideCharArr;
altNumTable: WideCharArr;
reserved: PACKED ARRAY [0..19] OF CHAR;
END;

{version number}
{format code}
{reserved}
{reserved}
{number of tables, one-based}
{offset to table that maps byte to token}
{offset to routine that copies canonical string}
{offset to routine that gets next byte of character}
{offset to table that maps token to canonical string}
{offset to default number parts table}
{offset to white space code list}
{reserved}
{reserved}
{reserved}
{reserved}
{reserved}
{length of untoken table}
{length of default number parts table}
{length of white space code list}
{reserved}
{reserved}

{index by [tokLeftQuote..tokMaxSymbols]}

FVector = RECORD
start: INTEGER;
length: INTEGER;
END;

TripleInt = ARRAY [0..2] OF FVector;

{ index by [fPositive..fZero] }

ScriptRunStatus = RECORD
script: SignedByte;
variant: SignedByte;
END;

{ New types for System 7.0:
type for truncWhere parameter in new TruncString, TruncText }
TruncCode = INTEGER;
{ type for styleRunPosition parameter in NPixel2Char etc. }
JustStyleCode = INTEGER;



FUNCTION FontScript: INTEGER;
INLINE $2F3C,$8200,$0000,$A8B5;
FUNCTION IntlScript: INTEGER;
INLINE $2F3C,$8200,$0002,$A8B5;
PROCEDURE KeyScript(code: INTEGER);
INLINE $2F3C,$8002,$0004,$A8B5;
FUNCTION Font2Script(fontNumber: INTEGER): INTEGER;
INLINE $2F3C,$8202,$0006,$A8B5;
FUNCTION GetEnvirons(verb: INTEGER): LONGINT;
INLINE $2F3C,$8402,$0008,$A8B5;
FUNCTION SetEnvirons(verb: INTEGER;param: LONGINT): OSErr;
INLINE $2F3C,$8206,$000A,$A8B5;
FUNCTION GetScript(script: INTEGER;verb: INTEGER): LONGINT;
INLINE $2F3C,$8404,$000C,$A8B5;
FUNCTION SetScript(script: INTEGER;verb: INTEGER;param: LONGINT): OSErr;
INLINE $2F3C,$8208,$000E,$A8B5;
FUNCTION CharByte(textBuf: Ptr;textOffset: INTEGER): INTEGER;
INLINE $2F3C,$8206,$0010,$A8B5;
FUNCTION CharType(textBuf: Ptr;textOffset: INTEGER): INTEGER;
INLINE $2F3C,$8206,$0012,$A8B5;
FUNCTION Pixel2Char(textBuf: Ptr;textLen: INTEGER;slop: INTEGER;pixelWidth: INTEGER;
VAR leadingEdge: BOOLEAN): INTEGER;
INLINE $2F3C,$820E,$0014,$A8B5;
FUNCTION Char2Pixel(textBuf: Ptr;textLen: INTEGER;slop: INTEGER;offset: INTEGER;
direction: INTEGER): INTEGER;
INLINE $2F3C,$820C,$0016,$A8B5;
FUNCTION Transliterate(srcHandle: Handle;dstHandle: Handle;target: INTEGER;
srcMask: LONGINT): OSErr;
INLINE $2F3C,$820E,$0018,$A8B5;
PROCEDURE FindWord(textPtr: Ptr;textLength: INTEGER;offset: INTEGER;leadingEdge: BOOLEAN;
breaks: BreakTablePtr;VAR offsets: OffsetTable);
INLINE $2F3C,$8012,$001A,$A8B5;
PROCEDURE HiliteText(textPtr: Ptr;textLength: INTEGER;firstOffset: INTEGER;
secondOffset: INTEGER;VAR offsets: OffsetTable);
INLINE $2F3C,$800E,$001C,$A8B5;
PROCEDURE DrawJust(textPtr: Ptr;textLength: INTEGER;slop: INTEGER);
INLINE $2F3C,$8008,$001E,$A8B5;
PROCEDURE MeasureJust(textPtr: Ptr;textLength: INTEGER;slop: INTEGER;charLocs: Ptr);
INLINE $2F3C,$800C,$0020,$A8B5;
FUNCTION ParseTable(VAR table: CharByteTable): BOOLEAN;
INLINE $2F3C,$8204,$0022,$A8B5;
FUNCTION GetDefFontSize: INTEGER;
INLINE $3EB8,$0BA8,$6604,$3EBC,$000C;
FUNCTION GetSysFont: INTEGER;
INLINE $3EB8,$0BA6;
FUNCTION GetAppFont: INTEGER;
INLINE $3EB8,$0984;
FUNCTION GetMBarHeight: INTEGER;
INLINE $3EB8,$0BAA;
FUNCTION GetSysJust: INTEGER;
INLINE $3EB8,$0BAC;
PROCEDURE SetSysJust(newJust: INTEGER);
INLINE $31DF,$0BAC;
PROCEDURE ReadLocation(VAR loc: MachineLocation);
INLINE $205F,$203C,$000C,$00E4,$A051;
PROCEDURE WriteLocation(loc: MachineLocation);



INLINE $205F,$203C,$000C,$00E4,$A052;
PROCEDURE UprText(textPtr: Ptr;len: INTEGER);
INLINE $301F,$205F,$A054;
PROCEDURE LwrText(textPtr: Ptr;len: INTEGER);
INLINE $301F,$205F,$A056;

{ New for 7.0 }
PROCEDURE LowerText(textPtr: Ptr;len: INTEGER);
INLINE $301F,$205F,$A056;
PROCEDURE StripText(textPtr: Ptr;len: INTEGER);
INLINE $301F,$205F,$A256;
PROCEDURE UpperText(textPtr: Ptr;len: INTEGER);
INLINE $301F,$205F,$A456;
PROCEDURE StripUpperText(textPtr: Ptr;len: INTEGER);
INLINE $301F,$205F,$A656;
FUNCTION StyledLineBreak(textPtr: Ptr;textLen: LONGINT;textStart: LONGINT;
textEnd: LONGINT;flags: LONGINT;VAR textWidth: Fixed;VAR textOffset: LONGINT): StyledLineBreakCode;
INLINE $2F3C,$821C,$FFFE,$A8B5;
PROCEDURE GetFormatOrder(ordering: FormatOrderPtr;firstFormat: INTEGER;
lastFormat: INTEGER;lineRight: BOOLEAN;rlDirProc: Ptr;dirParam: Ptr);
INLINE $2F3C,$8012,$FFFC,$A8B5;
FUNCTION IntlTokenize(tokenParam: TokenBlockPtr): TokenResults;
INLINE $2F3C,$8204,$FFFA,$A8B5;
FUNCTION InitDateCache(theCache: DateCachePtr): OSErr;
INLINE $2F3C,$8204,$FFF8,$A8B5;
FUNCTION String2Date(textPtr: Ptr;textLen: LONGINT;theCache: DateCachePtr;
VAR lengthUsed: LONGINT;VAR dateTime: LongDateRec): String2DateStatus;
INLINE $2F3C,$8214,$FFF6,$A8B5;
FUNCTION String2Time(textPtr: Ptr;textLen: LONGINT;theCache: DateCachePtr;
VAR lengthUsed: LONGINT;VAR dateTime: LongDateRec): String2DateStatus;
INLINE $2F3C,$8214,$FFF4,$A8B5;
PROCEDURE LongDate2Secs(lDate: LongDateRec;VAR lSecs: LongDateTime);
INLINE $2F3C,$8008,$FFF2,$A8B5;
PROCEDURE LongSecs2Date(VAR lSecs: LongDateTime;VAR lDate: LongDateRec);
INLINE $2F3C,$8008,$FFF0,$A8B5;
FUNCTION ToggleDate(VAR lSecs: LongDateTime;field: LongDateField;delta: DateDelta;
ch: INTEGER;params: TogglePB): ToggleResults;
INLINE $2F3C,$820E,$FFEE,$A8B5;
FUNCTION Str2Format(inString: Str255;partsTable: NumberParts;VAR outString: NumFormatString): FormatStatus;
INLINE $2F3C,$820C,$FFEC,$A8B5;
FUNCTION Format2Str(myCanonical: NumFormatString;partsTable: NumberParts;
VAR outString: Str255;VAR positions: TripleInt): FormatStatus;
INLINE $2F3C,$8210,$FFEA,$A8B5;
FUNCTION FormatX2Str(x: Extended80;myCanonical: NumFormatString;partsTable: NumberParts;
VAR outString: Str255): FormatStatus;
INLINE $2F3C,$8210,$FFE8,$A8B5;
FUNCTION FormatStr2X(source: Str255;myCanonical: NumFormatString;partsTable: NumberParts;
VAR x: Extended80): FormatStatus;
INLINE $2F3C,$8210,$FFE6,$A8B5;
FUNCTION PortionText(textPtr: Ptr;textLen: LONGINT): Fixed;
INLINE $2F3C,$8408,$0024,$A8B5;
FUNCTION FindScriptRun(textPtr: Ptr;textLen: LONGINT;VAR lenUsed: LONGINT): ScriptRunStatus;
INLINE $2F3C,$820C,$0026,$A8B5;
FUNCTION VisibleLength(textPtr: Ptr;textLen: LONGINT): LONGINT;

INLINE $2F3C,$8408,$0028,$A8B5;
FUNCTION ValidDate(vDate: LongDateRec;flags: LONGINT;VAR newSecs: LongDateTime): INTEGER;
INLINE $2F3C,$820C,$FFE4,$A8B5;

{ New for 7.0 }
PROCEDURE NFindWord(textPtr: Ptr;textLength: INTEGER;offset: INTEGER;leadingEdge: BOOLEAN;
nbreaks: NBreakTablePtr;VAR offsets: OffsetTable);
INLINE $2F3C,$8012,$FFE2,$A8B5;
FUNCTION TruncString(width: INTEGER;VAR theString: Str255;truncWhere: TruncCode): INTEGER;
INLINE $2F3C,$8208,$FFE0,$A8B5;
FUNCTION TruncText(width: INTEGER;textPtr: Ptr;VAR length: INTEGER;truncWhere: TruncCode): INTEGER;
INLINE $2F3C,$820C,$FFDE,$A8B5;
FUNCTION ReplaceText(baseText: Handle;substitutionText: Handle;key: Str15): INTEGER;
INLINE $2F3C,$820C,$FFDC,$A8B5;
FUNCTION NPixel2Char(textBuf: Ptr;textLen: LONGINT;slop: Fixed;pixelWidth: Fixed;
VAR leadingEdge: BOOLEAN;VAR widthRemaining: Fixed;styleRunPosition: JustStyleCode;
numer: Point;denom: Point): INTEGER;
INLINE $2F3C,$8222,$002E,$A8B5;
FUNCTION NChar2Pixel(textBuf: Ptr;textLen: LONGINT;slop: Fixed;offset: LONGINT;
direction: INTEGER;styleRunPosition: JustStyleCode;numer: Point;denom: Point): INTEGER;
INLINE $2F3C,$821C,$0030,$A8B5;
PROCEDURE NDrawJust(textPtr: Ptr;textLength: LONGINT;slop: Fixed;styleRunPosition: JustStyleCode;
numer: Point;denom: Point);
INLINE $2F3C,$8016,$0032,$A8B5;
PROCEDURE NMeasureJust(textPtr: Ptr;textLength: LONGINT;slop: Fixed;charLocs: Ptr;
styleRunPosition: JustStyleCode;numer: Point;denom: Point);
INLINE $2F3C,$801A,$0034,$A8B5;
FUNCTION NPortionText(textPtr: Ptr;textLen: LONGINT;styleRunPosition: JustStyleCode;
numer: Point;denom: Point): Fixed;
INLINE $2F3C,$8412,$0036,$A8B5;

{$ENDC} { UsingScript }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Script.p}



{#####################################################################}
{### FILE: SCSI.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:14 PM
SCSI.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1986-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT SCSI;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSCSI}
{$SETC UsingSCSI := 1}
{$I+}
{$SETC SCSIIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := SCSIIncludes}
CONST
scInc = 1;
scNoInc = 2;
scAdd = 3;
scMove = 4;
scLoop = 5;
scNop = 6;
scStop = 7;
scComp = 8;
scCommErr = 2;
scArbNBErr = 3;
scBadParmsErr = 4;
scPhaseErr = 5;
scCompareErr = 6;
scMgrBusyErr = 7;
scSequenceErr = 8;
scBusTOErr = 9;
scComplPhaseErr = 10;
sbSIGWord = $4552;
pMapSIG = $504D;

{communications error, operation timeout}
{arbitration timeout waiting for not BSY}
{bad parameter or TIB opcode}
{SCSI bus not in correct phase for attempted operation}
{data compare error}
{SCSI Manager busy }
{attempted operation is out of sequence}
{CPU bus timeout}
{SCSI bus wasn't in Status phase}



TYPE
Block0 = PACKED RECORD
sbSig: INTEGER;
sbBlkSize: INTEGER;
sbBlkCount: LONGINT;
sbDevType: INTEGER;
sbDevId: INTEGER;
sbData: LONGINT;
sbDrvrCount: INTEGER;
ddBlock: LONGINT;
ddSize: INTEGER;
ddType: INTEGER;
ddPad: ARRAY [0..242] OF INTEGER;
END;
Partition = PACKED RECORD
pmSig: INTEGER;
pmSigPad: INTEGER;
pmMapBlkCnt: LONGINT;
pmPyPartStart: LONGINT;
pmPartBlkCnt: LONGINT;
pmPartName: PACKED ARRAY [0..31] OF CHAR;
pmParType: PACKED ARRAY [0..31] OF CHAR;
pmLgDataStart: LONGINT;
pmDataCnt: LONGINT;
pmPartStatus: LONGINT;
pmLgBootStart: LONGINT;
pmBootSize: LONGINT;
pmBootAddr: LONGINT;
pmBootAddr2: LONGINT;
pmBootEntry: LONGINT;
pmBootEntry2: LONGINT;
pmBootCksum: LONGINT;
pmProcessor: PACKED ARRAY [0..15] OF CHAR;
pmPad: ARRAY [0..187] OF INTEGER;
END;

{unique value for SCSI block 0}
{block size of device}
{number of blocks on device}
{device type}
{device id}
{not used}
{driver descriptor count}
{1st driver's starting block}
{size of 1st driver (512-byte blks)}
{system type (1 for Mac+)}
{ARRAY[0..242] OF INTEGER; not used}

{unique value for map entry blk}
{currently unused}
{# of blks in partition map}
{physical start blk of partition}
{# of blks in this partition}
{ASCII partition name}
{ASCII partition type}
{log. # of partition's 1st data blk}
{# of blks in partition's data area}
{bit field for partition status}
{log. blk of partition's boot code}
{number of bytes in boot code}
{memory load address of boot code}
{currently unused}
{entry point of boot code}
{currently unused}
{checksum of boot code}
{ASCII for the processor type}
{512 bytes long currently unused}

SCSIInstr = RECORD
scOpcode: INTEGER;
scParam1: LONGINT;
scParam2: LONGINT;
END;

FUNCTION SCSIReset: OSErr;
INLINE $4267,$A815;
FUNCTION SCSIGet: OSErr;
INLINE $3F3C,$0001,$A815;
FUNCTION SCSISelect(targetID: INTEGER): OSErr;
INLINE $3F3C,$0002,$A815;
FUNCTION SCSICmd(buffer: Ptr;count: INTEGER): OSErr;
INLINE $3F3C,$0003,$A815;
FUNCTION SCSIRead(tibPtr: Ptr): OSErr;
INLINE $3F3C,$0005,$A815;
FUNCTION SCSIRBlind(tibPtr: Ptr): OSErr;
INLINE $3F3C,$0008,$A815;


FUNCTION SCSIWrite(tibPtr: Ptr): OSErr;
INLINE $3F3C,$0006,$A815;
FUNCTION SCSIWBlind(tibPtr: Ptr): OSErr;
INLINE $3F3C,$0009,$A815;
FUNCTION SCSIComplete(VAR stat: INTEGER;VAR message: INTEGER;wait: LONGINT): OSErr;
INLINE $3F3C,$0004,$A815;
FUNCTION SCSIStat: INTEGER;
INLINE $3F3C,$000A,$A815;
FUNCTION SCSISelAtn(targetID: INTEGER): OSErr;
INLINE $3F3C,$000B,$A815;
FUNCTION SCSIMsgIn(VAR message: INTEGER): OSErr;
INLINE $3F3C,$000C,$A815;
FUNCTION SCSIMsgOut(message: INTEGER): OSErr;
INLINE $3F3C,$000D,$A815;

{$ENDC}

{ UsingSCSI }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE SCSI.p}

{#####################################################################}
{### FILE: SCSIIntf.p}
{#####################################################################}
{
File: SCSIIntf.p
As of MPW 3.0, interface files were reorganized to more closely
match "Inside Macintosh" reference books and be more consistant
from language to language.
Interfaces for the SCSI Manager are now found in SCSI.p.
This file, which includes SCSI.p, is provided for compatibility
with old sources.
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988
All Rights Reserved
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT SCSIIntf;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSCSIIntf}
{$SETC UsingSCSIIntf := 1}
{$I+}
{$SETC SCSIIntfIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingSCSI}
{$I $$Shell(PInterfaces)SCSI.p}
{$ENDC}
{$SETC UsingIncludes := SCSIIntfIncludes}
{$ENDC}

{ UsingSCSIIntf }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE SCSIIntf.p}



{#####################################################################}
{### FILE: SegLoad.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:15 PM
SegLoad.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT SegLoad;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSegLoad}
{$SETC UsingSegLoad := 1}
{$I+}
{$SETC SegLoadIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := SegLoadIncludes}
CONST
appOpen = 0;
appPrint = 1;
TYPE
AppFile = RECORD
vRefNum: INTEGER;
fType: OSType;
versNum: INTEGER;
fName: Str255;
END;

{Open the Document (s)}
{Print the Document (s)}

{versNum in high byte}

PROCEDURE UnloadSeg(routineAddr: Ptr);
INLINE $A9F1;
PROCEDURE ExitToShell;
INLINE $A9F4;
PROCEDURE GetAppParms(VAR apName: Str255;VAR apRefNum: INTEGER;VAR apParam: Handle);
INLINE $A9F5;
PROCEDURE CountAppFiles(VAR message: INTEGER;VAR count: INTEGER);
PROCEDURE GetAppFiles(index: INTEGER;VAR theFile: AppFile);


PROCEDURE ClrAppFiles(index: INTEGER);

{$ENDC}

{ UsingSegLoad }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE SegLoad.p}



{#####################################################################}
{### FILE: Serial.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:15 PM
Serial.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Serial;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSerial}
{$SETC UsingSerial := 1}
{$I+}
{$SETC SerialIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := SerialIncludes}
CONST
baud300 = 380;
baud600 = 189;
baud1200 = 94;
baud1800 = 62;
baud2400 = 46;
baud3600 = 30;
baud4800 = 22;
baud7200 = 14;
baud9600 = 10;
baud19200 = 4;
baud57600 = 0;
stop10 = 16384;
stop15 = -32768;
stop20 = -16384;
noParity = 0;
oddParity = 4096;
evenParity = 12288;
data5 = 0;
data6 = 2048;
data7 = 1024;
data8 = 3072;
ctsEvent = 32;
breakEvent = 128;
xOffWasSent = 128;
dtrNegated = 64;
ainRefNum = -6;
aoutRefNum = -7;
binRefNum = -8;
boutRefNum = -9;

TYPE
SPortSel = (sPortA,sPortB);

SerShk = PACKED RECORD
fXOn: Byte;
{XOn flow control enabled flag}
fCTS: Byte;
{CTS flow control enabled flag}
xOn: CHAR;
{XOn character}
xOff: CHAR;
{XOff character}
errs: Byte;
{errors mask bits}
evts: Byte;
{event enable mask bits}
fInX: Byte;
{Input flow control enabled flag}
fDTR: Byte;
{DTR input flow control flag}
END;
SerStaRec = PACKED RECORD
cumErrs: Byte;
xOffSent: Byte;
rdPend: Byte;
wrPend: Byte;
ctsHold: Byte;
xOffHold: Byte;
END;

{$ENDC}

FUNCTION SerReset(refNum: INTEGER;serConfig: INTEGER): OSErr;
FUNCTION SerSetBuf(refNum: INTEGER;serBPtr: Ptr;serBLen: INTEGER): OSErr;
FUNCTION SerHShake(refNum: INTEGER;flags: SerShk): OSErr;
FUNCTION SerSetBrk(refNum: INTEGER): OSErr;
FUNCTION SerClrBrk(refNum: INTEGER): OSErr;
FUNCTION SerGetBuf(refNum: INTEGER;VAR count: LONGINT): OSErr;
FUNCTION SerStatus(refNum: INTEGER;VAR serSta: SerStaRec): OSErr;

{ UsingSerial }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Serial.p}



{#####################################################################}
{### FILE: ShutDown.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:15 PM
ShutDown.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1987-1989

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT ShutDown;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingShutDown}
{$SETC UsingShutDown := 1}
{$I+}
{$SETC ShutDownIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := ShutDownIncludes}
CONST
sdOnPowerOff = 1;
sdOnRestart = 2;
sdOnUnmount = 4;
sdOnDrivers = 8;
sdRestartOrPower = 3;

{call
{call
{call
{call
{call

procedure before power off.}
procedure before restart.}
procedure before unmounting.}
procedure before closing drivers.}
before either power off or restart.}

PROCEDURE ShutDwnPower;
INLINE $3F3C,$0001,$A895;
PROCEDURE ShutDwnStart;
INLINE $3F3C,$0002,$A895;
PROCEDURE ShutDwnInstall(shutDownProc: ProcPtr;flags: INTEGER);
INLINE $3F3C,$0003,$A895;
PROCEDURE ShutDwnRemove(shutDownProc: ProcPtr);
INLINE $3F3C,$0004,$A895;

{$ENDC}

{ UsingShutDown }

{$IFC NOT UsingIncludes}
END.




{$ENDC}

{### END OF FILE ShutDown.p}





{#####################################################################}
{### FILE: Signal.p}
{#####################################################################}

{
Created: Friday, August 2, 1991 at 11:40 PM
Signal.p
Pascal Interface to the Macintosh Libraries
Signal Handling interface.
This must be compatible with C's <signal.h>
Copyright Apple Computer, Inc. 1986, 1987, 1988, 1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Signal;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSignal}
{$SETC UsingSignal := 1}

TYPE
SignalMap =
INTEGER;
SignalHandler = ^LONGINT;
CONST
SIG_ERR =      -1;
SIG_IGN =	   0;
SIG_DFL =	   1;
SIG_HOLD =	   3;
SIG_RELEASE =  5;
SIGABRT =	   $0001;
SIGINT =	   $0002;
SIGFPE =	   $0004;
SIGILL =	   $0008;
SIGSEGV =	   $0010;
SIGTERM =	   $0020;

{ Pointer to function }

{ Returned by IEsignal on error }

{ Currently only SIGINT implemented }

{ Signal Handling Functions }
FUNCTION
IEsignal(sigNum: LONGINT; sigHdlr: UNIV SignalHandler):
SignalHandler; {C;}

FUNCTION
IEraise(sigNum: LONGINT):
LONGINT; {c;}

{$ENDC} { UsingSignal }
{$IFC NOT UsingIncludes}
END.
{$ENDC}
{### END OF FILE Signal.p}





{#####################################################################}
{### FILE: Slots.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:16 PM
Slots.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1986-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Slots;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSlots}
{$SETC UsingSlots := 1}
{$I+}
{$SETC SlotsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingOSEvents}
{$I $$Shell(PInterfaces)OSEvents.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$SETC UsingIncludes := SlotsIncludes}
CONST
fCardIsChanged = 1;
fCkForSame = 0;
fCkForNext = 1;
fWarmStart = 2;

{Card is Changed field in StatusFlags field of sInfoArray}
{For SearchSRT. Flag to check for SAME sResource in the table. }
{For SearchSRT. Flag to check for NEXT sResource in the table. }
{If this bit is set then warm start else cold start.}

stateNil = 0;
stateSDMInit = 1;
statePRAMInit = 2;
statePInit = 3;
stateSInit = 4;

{State}
{:Slot declaration manager Init}
{:sPRAM record init}
{:Primary init}
{:Secondary init}
{ flags for spParamData }
fall = 0;
foneslot = 1;
fnext = 2;
TYPE
SQElemPtr = ^SlotIntQElement;
SlotIntQElement = RECORD
sqLink: Ptr;
sqType: INTEGER;
sqPrio: INTEGER;
sqAddr: ProcPtr;
sqParm: LONGINT;
END;
SpBlockPtr = ^SpBlock;
SpBlock = PACKED RECORD
spResult: LONGINT;
spsPointer: Ptr;
spSize: LONGINT;
spOffsetData: LONGINT;
spIOFileName: Ptr;
spsExecPBlk: Ptr;
spParamData: LONGINT;
spMisc: LONGINT;
spReserved: LONGINT;
spIOReserved: INTEGER;
spRefNum: INTEGER;
spCategory: INTEGER;
spCType: INTEGER;
spDrvrSW: INTEGER;
spDrvrHW: INTEGER;
spTBMask: SignedByte;
spSlot: SignedByte;
spID: SignedByte;
spExtDev: SignedByte;
spHwDev: SignedByte;
spByteLanes: SignedByte;
spFlags: SignedByte;
spKey: SignedByte;
END;
SInfoRecPtr = ^SInfoRecord;
SInfoRecord = PACKED RECORD
siDirPtr: Ptr;
siInitStatusA: INTEGER;
siInitStatusV: INTEGER;
siState: SignedByte;
siCPUByteLanes: SignedByte;
siTopOfROM: SignedByte;
siStatusFlags: SignedByte;
siTOConst: INTEGER;
siReserved: PACKED ARRAY [0..1] OF SignedByte;
siROMAddr: Ptr;
siSlot: CHAR;
siPadding: PACKED ARRAY [0..2] OF SignedByte;

{ bit 0: set=search enabled/disabled sRsrc's }
{
1: set=search sRsrc's in given slot only }
{
2: set=search for next sRsrc }

{ptr to next element}
{queue type ID for validity}
{priority}
{interrupt service routine}
{optional A1 parameter}

{FUNCTION Result}
{structure pointer}
{size of structure}
{offset/data field used by sOffsetData}
{ptr to IOFile name for sDisDrvrName}
{pointer to sExec parameter block.}
{misc parameter data (formerly spStackPtr).}
{misc field for SDM.}
{reserved for future expansion}
{Reserved field of Slot Resource Table}
{RefNum}
{sType: Category}
{Type}
{DrvrSW}
{DrvrHW}
{type bit mask bits 0..3 mask words 0..3}
{slot number}
{structure ID}
{ID of the external device}
{Id of the hardware device.}
{bytelanes from card ROM format block}
{standard flags}
{Internal use only}

{Pointer to directory}
{initialization E}
{status returned by vendor init code}
{initialization state}
{0=[d0..d7] 1=[d8..d15]}
{Top of ROM= $FssFFFFx: x is TopOfROM}
{bit 0 - card is changed}
{Time Out C for BusErr}
{reserved}
{ addr of top of ROM }
{ slot number }
{ reserved }



END;
SDMRecord = PACKED RECORD
sdBEVSave: ProcPtr;
sdBusErrProc: ProcPtr;
sdErrorEntry: ProcPtr;
sdReserved: LONGINT;
END;
FHeaderRecPtr = ^FHeaderRec;
FHeaderRec = PACKED RECORD
fhDirOffset: LONGINT;
fhLength: LONGINT;
fhCRC: LONGINT;
fhROMRev: SignedByte;
fhFormat: SignedByte;
fhTstPat: LONGINT;
fhReserved: SignedByte;
fhByteLanes: SignedByte;
END;
SEBlock = PACKED RECORD
seSlot: SignedByte;
sesRsrcId: SignedByte;
seStatus: INTEGER;
seFlags: SignedByte;
seFiller0: SignedByte;
seFiller1: SignedByte;
seFiller2: SignedByte;
seResult: LONGINT;
seIOFileName: LONGINT;
seDevice: SignedByte;
sePartition: SignedByte;
seOSType: SignedByte;
seReserved: SignedByte;
seRefNum: SignedByte;
seNumDevices: SignedByte;
seBootState: SignedByte;
END;

{ Principle }
FUNCTION SReadByte(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7000,$A06E,$3E80;
FUNCTION SReadWord(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7001,$A06E,$3E80;
FUNCTION SReadLong(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7002,$A06E,$3E80;
FUNCTION SGetCString(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7003,$A06E,$3E80;
FUNCTION SGetBlock(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7005,$A06E,$3E80;
FUNCTION SFindStruct(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7006,$A06E,$3E80;
FUNCTION SReadStruct(spBlkPtr: SpBlockPtr): OSErr;

{Save old BusErr vector}
{Go here to determine if it is a BusErr}
{Go here if BusErrProc finds real BusErr}
{Reserved}

{offset to directory}
{length of ROM}
{CRC}
{revision of ROM}
{format - 2}
{test pattern}
{reserved}
{ByteLanes}

{Slot number.}
{sResource Id.}
{Status of code executed by sExec.}
{Flags}
{Filler, must be SignedByte to align on odd boundry}
{Filler}
{Filler}
{Result of sLoad.}
{Pointer to IOFile name.}
{Which device to read from.}
{The partition.}
{Type of OS.}
{Reserved field.}
{RefNum of the driver.}
{ Number of devices to load.}
{State of StartBoot code.}



INLINE $205F,$7007,$A06E,$3E80;

{ Special }
FUNCTION SReadInfo(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7010,$A06E,$3E80;
FUNCTION SReadPRAMRec(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7011,$A06E,$3E80;
FUNCTION SPutPRAMRec(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7012,$A06E,$3E80;
FUNCTION SReadFHeader(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7013,$A06E,$3E80;
FUNCTION SNextSRsrc(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7014,$A06E,$3E80;
FUNCTION SNextTypeSRsrc(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7015,$A06E,$3E80;
FUNCTION SRsrcInfo(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7016,$A06E,$3E80;
FUNCTION SCkCardStat(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7018,$A06E,$3E80;
FUNCTION SReadDrvrName(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7019,$A06E,$3E80;
FUNCTION SFindDevBase(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$701B,$A06E,$3E80;
FUNCTION SFindBigDevBase(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$701C,$A06E,$3E80;

{ Advanced }
FUNCTION InitSDeclMgr(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7020,$A06E,$3E80;
FUNCTION SPrimaryInit(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7021,$A06E,$3E80;
FUNCTION SCardChanged(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7022,$A06E,$3E80;
FUNCTION SExec(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7023,$A06E,$3E80;
FUNCTION SOffsetData(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7024,$A06E,$3E80;
FUNCTION SInitPRAMRecs(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7025,$A06E,$3E80;
FUNCTION SReadPBSize(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7026,$A06E,$3E80;
FUNCTION SCalcStep(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7028,$A06E,$3E80;
FUNCTION SInitSRsrcTable(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7029,$A06E,$3E80;
FUNCTION SSearchSRT(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$702A,$A06E,$3E80;
FUNCTION SUpdateSRT(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$702B,$A06E,$3E80;
FUNCTION SCalcSPointer(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$702C,$A06E,$3E80;
FUNCTION SGetDriver(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$702D,$A06E,$3E80;
FUNCTION SPtrToSlot(spBlkPtr: SpBlockPtr): OSErr;



INLINE $205F,$702E,$A06E,$3E80;
FUNCTION SFindSInfoRecPtr(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$702F,$A06E,$3E80;
FUNCTION SFindSRsrcPtr(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7030,$A06E,$3E80;
FUNCTION SDeleteSRTRec(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7031,$A06E,$3E80;
FUNCTION OpenSlot(paramBlock: ParmBlkPtr;async: BOOLEAN): OSErr;
FUNCTION OpenSlotSync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A200,$3E80;
FUNCTION OpenSlotAsync(paramBlock: ParmBlkPtr): OSErr;
INLINE $205F,$A600,$3E80;

{ Device Manager Slot Support }
FUNCTION SIntInstall(sIntQElemPtr: SQElemPtr;theSlot: INTEGER): OSErr;
INLINE $301F,$205F,$A075,$3E80;
FUNCTION SIntRemove(sIntQElemPtr: SQElemPtr;theSlot: INTEGER): OSErr;
INLINE $301F,$205F,$A076,$3E80;
FUNCTION SVersion(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7008,$A06E,$3E80;
FUNCTION SetSRsrcState(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$7009,$A06E,$3E80;
FUNCTION InsertSRTRec(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$700A,$A06E,$3E80;
FUNCTION SGetSRsrc(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$700B,$A06E,$3E80;
FUNCTION SGetTypeSRsrc(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$700C,$A06E,$3E80;
FUNCTION SGetSRsrcPtr(spBlkPtr: SpBlockPtr): OSErr;
INLINE $205F,$701D,$A06E,$3E80;

{$ENDC}

{ UsingSlots }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Slots.p}



{#####################################################################}
{### FILE: Sound.p}
{#####################################################################}

{
Created: Monday, December 2, 1991 at 5:09 PM
Sound.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1986-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Sound;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSound}
{$SETC UsingSound := 1}
{$I+}
{$SETC SoundIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$SETC UsingIncludes := SoundIncludes}
CONST
swMode = -1;
ftMode = 1;
ffMode = 0;
synthCodeRsrc = 'snth';
soundListRsrc = 'snd ';

{ Sound Driver modes }

{ Resource types used by Sound Manager }

twelfthRootTwo = 1.05946309434;
rate22khz = $56EE8BA3;
rate11khz = $2B7745D1;

{ 22254.54545 in fixed-point }
{ 11127.27273 in fixed-point }

{ synthesizer numbers for SndNewChannel }
squareWaveSynth = 1;
waveTableSynth = 3;
sampledSynth = 5;

{square wave synthesizer}
{wave table synthesizer}
{sampled sound synthesizer}



{ old Sound Manager MACE synthesizer numbers }
MACE3snthID = 11;
MACE6snthID = 13;
{ command numbers for SndDoCommand and SndDoImmediate }
nullCmd = 0;
initCmd = 1;
freeCmd = 2;
quietCmd = 3;
flushCmd = 4;
reInitCmd = 5;
waitCmd = 10;
pauseCmd = 11;
resumeCmd = 12;
callBackCmd = 13;
syncCmd = 14;
emptyCmd = 15;
tickleCmd = 20;
requestNextCmd = 21;
howOftenCmd = 22;
wakeUpCmd = 23;
availableCmd = 24;
versionCmd = 25;
totalLoadCmd = 26;
loadCmd = 27;
scaleCmd = 30;
tempoCmd = 31;
freqDurationCmd = 40;
restCmd = 41;
freqCmd = 42;
ampCmd = 43;
timbreCmd = 44;
getAmpCmd = 45;
waveTableCmd = 60;
phaseCmd = 61;
soundCmd = 80;
bufferCmd = 81;
rateCmd = 82;
continueCmd = 83;
doubleBufferCmd = 84;
getRateCmd = 85;
sizeCmd = 90;
convertCmd = 91;
stdQLength = 128;
dataOffsetFlag = $8000;
waveInitChannelMask = $07;
waveInitChannel0 = $04;



waveInitChannel1 = $05;
waveInitChannel2 = $06;
waveInitChannel3 = $07;
{ channel initialization parameters }
initPanMask = $0003;
initSRateMask = $0030;
initStereoMask = $00C0;
initCompMask = $FF00;

{
{
{
{

mask
mask
mask
mask

initChanLeft = $0002;
initChanRight = $0003;
initNoInterp = $0004;
initNoDrop = $0008;
initMono = $0080;
initStereo = $00C0;
initMACE3 = $0300;
initMACE6 = $0400;

{
{
{
{
{
{
{
{

left stereo channel }
right stereo channel }
no linear interpolation }
no drop-sample conversion }
monophonic channel }
stereo channel }
MACE 3:1 }
MACE 6:1 }

initChan0
initChan1
initChan2
initChan3

{
{
{
{

channel
channel
channel
channel

=
=
=
=

$0004;
$0005;
$0006;
$0007;

for
for
for
for

right/left pan values }
sample rate values }
mono/stereo values }
compression IDs }

0
1
2
3

-

wave
wave
wave
wave

table
table
table
table

only
only
only
only

}
}
}
}

stdSH = $00;
extSH = $FF;
cmpSH = $FE;

{ Standard sound header encode value }
{ Extended sound header encode value }
{ Compressed sound header encode value }

notCompressed = 0;
twoToOne = 1;
eightToThree = 2;
threeToOne = 3;
sixToOne = 4;

{ compression ID's }

outsideCmpSH = 0;
insideCmpSH = 1;
aceSuccess = 0;
aceMemFull = 1;
aceNilBlock = 2;
aceBadComp = 3;
aceBadEncode = 4;
aceBadDest = 5;
aceBadCmd = 6;
sixToOnePacketSize = 8;
threeToOnePacketSize = 16;
stateBlockSize = 64;
leftOverBlockSize = 32;

{ MACE constants }

firstSoundFormat = $0001;
secondSoundFormat = $0002;

{ general sound format }
{ special sampled sound format (HyperCard) }

dbBufferReady = $00000001;
dbLastBuffer = $00000004;

{ double buffer is filled }
{ last double buffer to play }

sysBeepDisable = $0000;
sysBeepEnable = $0001;

{ SysBeep() enable flags }



unitTypeNoSelection = $FFFF;
unitTypeSeconds = $0000;
TYPE
{
Structures for Sound Driver

{ unitTypes for AudioSelection.unitType }

}

FreeWave = PACKED ARRAY [0..30000] OF Byte;
FFSynthPtr = ^FFSynthRec;
FFSynthRec = RECORD
mode: INTEGER;
count: Fixed;
waveBytes: FreeWave;
END;
Tone = RECORD
count: INTEGER;
amplitude: INTEGER;
duration: INTEGER;
END;

Tones = ARRAY [0..5000] OF Tone;
SWSynthPtr = ^SWSynthRec;
SWSynthRec = RECORD
mode: INTEGER;
triplets: Tones;
END;

Wave = PACKED ARRAY [0..255] OF Byte;
WavePtr = ^Wave;
FTSndRecPtr = ^FTSoundRec;
FTSoundRec = RECORD
duration: INTEGER;
sound1Rate: Fixed;
sound1Phase: LONGINT;
sound2Rate: Fixed;
sound2Phase: LONGINT;
sound3Rate: Fixed;
sound3Phase: LONGINT;
sound4Rate: Fixed;
sound4Phase: LONGINT;
sound1Wave: WavePtr;
sound2Wave: WavePtr;
sound3Wave: WavePtr;
sound4Wave: WavePtr;
END;
FTSynthPtr = ^FTSynthRec;
FTSynthRec = RECORD
mode: INTEGER;



sndRec: FTSndRecPtr;
END;
{

Structures for Sound Manager

}

SndCommand = PACKED RECORD
cmd: INTEGER;
param1: INTEGER;
param2: LONGINT;
END;

Time = LONGINT;

{ in half milliseconds }

SndChannelPtr = ^SndChannel;
SndChannel = PACKED RECORD
nextChan: SndChannelPtr;
firstMod: Ptr;
{ reserved for the Sound Manager }
callBack: ProcPtr;
userInfo: LONGINT;
wait: Time;
{ The following is for internal Sound Manager use only.}
cmdInProgress: SndCommand;
flags: INTEGER;
qLength: INTEGER;
qHead: INTEGER;
{ next spot to read or -1 if empty }
qTail: INTEGER;
{ next spot to write = qHead if full }
queue: ARRAY [0..stdQLength - 1] OF SndCommand;
END;
{ MACE structures }
StateBlockPtr = ^StateBlock;
StateBlock = RECORD
stateVar: ARRAY [0..stateBlockSize - 1] OF INTEGER;
END;
LeftOverBlockPtr = ^LeftOverBlock;
LeftOverBlock = RECORD
count: LONGINT;
sampleArea: PACKED ARRAY [0..leftOverBlockSize - 1] OF Byte;
END;
ModRef = RECORD
modNumber: INTEGER;
modInit: LONGINT;
END;
SndListPtr = ^SndListResource;
SndListResource = RECORD
format: INTEGER;
numModifiers: INTEGER;
modifierPart: ARRAY [0..0] OF ModRef;
numCommands: INTEGER;
commandPart: ARRAY [0..0] OF SndCommand;
dataPart: PACKED ARRAY [0..0] OF Byte;

{This is a variable length array}
{This is a variable length array}
{This is a variable length array}



END;
SoundHeaderPtr = ^SoundHeader;
SoundHeader = PACKED RECORD
samplePtr: Ptr;
length: LONGINT;
sampleRate: Fixed;
loopStart: LONGINT;
loopEnd: LONGINT;
encode: Byte;
baseFrequency: Byte;
sampleArea: PACKED ARRAY [0..0] OF Byte;
END;
CmpSoundHeaderPtr = ^CmpSoundHeader;
CmpSoundHeader = PACKED RECORD
samplePtr: Ptr;
numChannels: LONGINT;
sampleRate: Fixed;
loopStart: LONGINT;
loopEnd: LONGINT;
encode: Byte;
baseFrequency: Byte;
numFrames: LONGINT;
AIFFSampleRate: Extended80;
markerChunk: Ptr;
futureUse1: Ptr;
futureUse2: Ptr;
stateVars: StateBlockPtr;
leftOverSamples: LeftOverBlockPtr;
compressionID: INTEGER;
packetSize: INTEGER;
snthID: INTEGER;
sampleSize: INTEGER;
sampleArea: PACKED ARRAY [0..0] OF Byte;
END;
ExtSoundHeaderPtr = ^ExtSoundHeader;
ExtSoundHeader = PACKED RECORD
samplePtr: Ptr;
numChannels: LONGINT;
sampleRate: Fixed;
loopStart: LONGINT;
loopEnd: LONGINT;
encode: Byte;
baseFrequency: Byte;
numFrames: LONGINT;
AIFFSampleRate: Extended80;
markerChunk: Ptr;
instrumentChunks: Ptr;
AESRecording: Ptr;
sampleSize: INTEGER;
futureUse1: INTEGER;
futureUse2: LONGINT;
futureUse3: LONGINT;
futureUse4: LONGINT;

{
{
{
{
{
{
{

if NIL then samples are in sampleArea }
length of sound in bytes }
sample rate for this sound }
start of looping portion }
end of looping portion }
header encoding }
baseFrequency value }

{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

if nil then samples are in sample area }
number of channels i.e. mono = 1 }
sample rate in Apples Fixed point representation }
loopStart of sound before compression }
loopEnd of sound before compression }
data structure used , stdSH, extSH, or cmpSH }
same meaning as regular SoundHeader }
length in frames ( packetFrames or sampleFrames ) }
IEEE sample rate }
sync track }
reserved by Apple }
reserved by Apple }
pointer to State Block }
used to save truncated samples between compression calls }
0 means no compression, non zero means compressionID }
number of bits in compressed sample packet }
resource ID of Sound Manager snth that contains NRT C/E }
number of bits in non-compressed sample }
space for when samples follow directly }

{
{
{
{
{
{
{
{
{
{
{

if nil then samples are in sample area }
number of channels, ie mono = 1 }
sample rate in Apples Fixed point representation }
same meaning as regular SoundHeader }
same meaning as regular SoundHeader }
data structure used , stdSH, extSH, or cmpSH }
same meaning as regular SoundHeader }
length in total number of frames }
IEEE sample rate }
sync track }
AIFF instrument chunks }

{
{
{
{
{

number of bits in
reserved by Apple
reserved by Apple
reserved by Apple
reserved by Apple

sample }
}
}
}
}



sampleArea: PACKED ARRAY [0..0] OF Byte;
END;

{ space for when samples follow directly }

ConversionBlockPtr = ^ConversionBlock;
ConversionBlock = RECORD
destination: INTEGER;
unused: INTEGER;
inputPtr: CmpSoundHeaderPtr;
outputPtr: CmpSoundHeaderPtr;
END;
SMStatusPtr = ^SMStatus;
SMStatus = PACKED RECORD
smMaxCPULoad: INTEGER;
smNumChannels: INTEGER;
smCurCPULoad: INTEGER;
END;
SCStatusPtr = ^SCStatus;
SCStatus = RECORD
scStartTime: Fixed;
scEndTime: Fixed;
scCurrentTime: Fixed;
scChannelBusy: BOOLEAN;
scChannelDisposed: BOOLEAN;
scChannelPaused: BOOLEAN;
scUnused: BOOLEAN;
scChannelAttributes: LONGINT;
scCPULoad: LONGINT;
END;
AudioSelectionPtr = ^AudioSelection;
AudioSelection = PACKED RECORD
unitType: LONGINT;
selStart: Fixed;
selEnd: Fixed;
END;
SndDoubleBufferPtr = ^SndDoubleBuffer;
SndDoubleBuffer = PACKED RECORD
dbNumFrames: LONGINT;
dbFlags: LONGINT;
dbUserInfo: ARRAY [0..1] OF LONGINT;
dbSoundData: PACKED ARRAY [0..0] OF Byte;
END;
SndDoubleBufferHeaderPtr = ^SndDoubleBufferHeader;
SndDoubleBufferHeader = PACKED RECORD
dbhNumChannels: INTEGER;
dbhSampleSize: INTEGER;
dbhCompressionID: INTEGER;
dbhPacketSize: INTEGER;
dbhSampleRate: Fixed;
dbhBufferPtr: ARRAY [0..1] OF SndDoubleBufferPtr;
dbhDoubleBack: ProcPtr;
END;



FUNCTION SndDoCommand(chan: SndChannelPtr;cmd: SndCommand;noWait: BOOLEAN): OSErr;
INLINE $A803;
FUNCTION SndDoImmediate(chan: SndChannelPtr;cmd: SndCommand): OSErr;
INLINE $A804;
FUNCTION SndNewChannel(VAR chan: SndChannelPtr;synth: INTEGER;init: LONGINT;
userRoutine: ProcPtr): OSErr;
INLINE $A807;
FUNCTION SndDisposeChannel(chan: SndChannelPtr;quietNow: BOOLEAN): OSErr;
INLINE $A801;
FUNCTION SndPlay(chan: SndChannelPtr;sndHdl: Handle;async: BOOLEAN): OSErr;
INLINE $A805;
FUNCTION SndAddModifier(chan: SndChannelPtr;modifier: ProcPtr;id: INTEGER;
init: LONGINT): OSErr;
INLINE $A802;
FUNCTION SndControl(id: INTEGER;VAR cmd: SndCommand): OSErr;
INLINE $A806;
PROCEDURE SetSoundVol(level: INTEGER);
PROCEDURE GetSoundVol(VAR level: INTEGER);
PROCEDURE StartSound(synthRec: Ptr;numBytes: LONGINT;completionRtn: ProcPtr);
PROCEDURE StopSound;
FUNCTION SoundDone: BOOLEAN;
FUNCTION SndSoundManagerVersion: NumVersion;
INLINE $203C,$000C,$0008,$A800;
FUNCTION SndStartFilePlay(chan: SndChannelPtr;fRefNum: INTEGER;resNum: INTEGER;
bufferSize: LONGINT;theBuffer: Ptr;theSelection: AudioSelectionPtr;theCompletion: ProcPtr;
async: BOOLEAN): OSErr;
INLINE $203C,$0D00,$0008,$A800;
FUNCTION SndPauseFilePlay(chan: SndChannelPtr): OSErr;
INLINE $203C,$0204,$0008,$A800;
FUNCTION SndStopFilePlay(chan: SndChannelPtr;async: BOOLEAN): OSErr;
INLINE $203C,$0308,$0008,$A800;
FUNCTION SndChannelStatus(chan: SndChannelPtr;theLength: INTEGER;theStatus: SCStatusPtr): OSErr;
INLINE $203C,$0010,$0008,$A800;
FUNCTION SndManagerStatus(theLength: INTEGER;theStatus: SMStatusPtr): OSErr;
INLINE $203C,$0014,$0008,$A800;
PROCEDURE SndGetSysBeepState(VAR sysBeepState: INTEGER);
INLINE $203C,$0018,$0008,$A800;
FUNCTION SndSetSysBeepState(sysBeepState: INTEGER): OSErr;
INLINE $203C,$001C,$0008,$A800;
FUNCTION SndPlayDoubleBuffer(chan: SndChannelPtr;theParams: SndDoubleBufferHeaderPtr): OSErr;
INLINE $203C,$0020,$0008,$A800;
FUNCTION MACEVersion: NumVersion;
INLINE $203C,$0000,$0010,$A800;
PROCEDURE Comp3to1(inBuffer: Ptr;outBuffer: Ptr;cnt: LONGINT;inState: Ptr;
outState: Ptr;numChannels: LONGINT;whichChannel: LONGINT);
INLINE $203C,$0004,$0010,$A800;
PROCEDURE Exp1to3(inBuffer: Ptr;outBuffer: Ptr;cnt: LONGINT;inState: Ptr;
outState: Ptr;numChannels: LONGINT;whichChannel: LONGINT);
INLINE $203C,$0008,$0010,$A800;
PROCEDURE Comp6to1(inBuffer: Ptr;outBuffer: Ptr;cnt: LONGINT;inState: Ptr;
outState: Ptr;numChannels: LONGINT;whichChannel: LONGINT);
INLINE $203C,$000C,$0010,$A800;
PROCEDURE Exp1to6(inBuffer: Ptr;outBuffer: Ptr;cnt: LONGINT;inState: Ptr;
outState: Ptr;numChannels: LONGINT;whichChannel: LONGINT);
INLINE $203C,$0010,$0010,$A800;

{$ENDC} { UsingSound }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Sound.p}

{#####################################################################}
{### FILE: SoundInput.p}
{#####################################################################}

{
Created: Tuesday, September 10, 1991 at 2:15 PM
SoundInput.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1990-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT SoundInput;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSoundInput}
{$SETC UsingSoundInput := 1}
{$I+}
{$SETC SoundInputIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$SETC UsingIncludes := SoundInputIncludes}
CONST
siDeviceIsConnected = 1;
siDeviceNotConnected = 0;
siDontKnowIfConnected = -1;
siReadPermission = 0;
siWritePermission = 1;

{
{
{
{
{

input device is connected and ready for input }
input device is not connected }
can't tell if input device is connected }
permission passed to SPBOpenDevice }
permission passed to SPBOpenDevice }

{ Info Selectors for Sound Input Drivers }
siDeviceConnected = 'dcon';
{ input device connection status }
siAGCOnOff = 'agc ';
{ automatic gain control state }
siPlayThruOnOff = 'plth';
{ playthrough state }
siTwosComplementOnOff = 'twos';
{ two's complement state }
siLevelMeterOnOff = 'lmet';
{ level meter state }
siRecordingQuality = 'qual';
{ recording quality }

siVoxRecordInfo = 'voxr';
siVoxStopInfo = 'voxs';
siNumberChannels = 'chan';
siSampleSize = 'ssiz';
siSampleRate = 'srat';
siCompressionType = 'comp';
siCompressionFactor = 'cmfa';
siCompressionHeader = 'cmhd';
siDeviceName = 'name';
siDeviceIcon = 'icon';
siDeviceBufferInfo = 'dbin';
siSampleSizeAvailable = 'ssav';
siSampleRateAvailable = 'srav';
siCompressionAvailable = 'cmav';
siChannelAvailable = 'chav';
siAsync = 'asyn';
siOptionsDialog = 'optd';
siContinuous = 'cont';
siActiveChannels = 'chac';
siActiveLevels = 'lmac';
siInputSource = 'sour';
siInitializeDriver = 'init';
siCloseDriver = 'clos';
siPauseRecording = 'paus';
siUserInterruptProc = 'user';


{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{
{

VOX record parameters }
VOX stop parameters }
current number of channels }
current sample size }
current sample rate }
current compression type }
current compression factor }
return compression header }
input device name }
input device icon }
size of interrupt buffer }
sample sizes available }
sample rates available }
compression types available }
number of channels available }
asynchronous capability }
display options dialog }
continous recording }
active channels }
active meter levels }
input source selector }
reserved for internal use only
reserved for internal use only
reserved for internal use only
reserved for internal use only

{
{
{
{
{
{
{
{
{
{

reference number of sound input device }
number of bytes to record }
number of milliseconds to record }
length of buffer in bytes }
buffer to store sound data in }
completion routine }
interrupt routine }
user-defined field }
error }
reserved - must be zero }

}
}
}
}

{ Qualities }
siBestQuality = 'best';
siBetterQuality = 'betr';
siGoodQuality = 'good';
TYPE
{ Sound Input Parameter Block }
SPBPtr = ^SPB;
SPB = RECORD
inRefNum: LONGINT;
count: LONGINT;
milliseconds: LONGINT;
bufferLength: LONGINT;
bufferPtr: Ptr;
completionRoutine: ProcPtr;
interruptRoutine: ProcPtr;
userLong: LONGINT;
error: OSErr;
unused1: LONGINT;
END;

FUNCTION SPBVersion: NumVersion;
INLINE $203C,$0000,$0014,$A800;
FUNCTION SndRecord(filterProc: ModalFilterProcPtr;corner: Point;quality: OSType;
VAR sndHandle: Handle): OSErr;
INLINE $203C,$0804,$0014,$A800;
FUNCTION SndRecordToFile(filterProc: ModalFilterProcPtr;corner: Point;quality: OSType;
fRefNum: INTEGER): OSErr;
INLINE $203C,$0708,$0014,$A800;
FUNCTION SPBSignInDevice(deviceRefNum: INTEGER;deviceName: Str255): OSErr;
INLINE $203C,$030C,$0014,$A800;
FUNCTION SPBSignOutDevice(deviceRefNum: INTEGER): OSErr;
INLINE $203C,$0110,$0014,$A800;
FUNCTION SPBGetIndexedDevice(count: INTEGER;VAR deviceName: Str255;VAR deviceIconHandle: Handle): OSErr;
INLINE $203C,$0514,$0014,$A800;
FUNCTION SPBOpenDevice(deviceName: Str255;permission: INTEGER;VAR inRefNum: LONGINT): OSErr;
INLINE $203C,$0518,$0014,$A800;
FUNCTION SPBCloseDevice(inRefNum: LONGINT): OSErr;
INLINE $203C,$021C,$0014,$A800;
FUNCTION SPBRecord(inParamPtr: SPBPtr;asynchFlag: BOOLEAN): OSErr;
INLINE $203C,$0320,$0014,$A800;
FUNCTION SPBRecordToFile(fRefNum: INTEGER;inParamPtr: SPBPtr;asynchFlag: BOOLEAN): OSErr;
INLINE $203C,$0424,$0014,$A800;
FUNCTION SPBPauseRecording(inRefNum: LONGINT): OSErr;
INLINE $203C,$0228,$0014,$A800;
FUNCTION SPBResumeRecording(inRefNum: LONGINT): OSErr;
INLINE $203C,$022C,$0014,$A800;
FUNCTION SPBStopRecording(inRefNum: LONGINT): OSErr;
INLINE $203C,$0230,$0014,$A800;
FUNCTION SPBGetRecordingStatus(inRefNum: LONGINT;VAR recordingStatus: INTEGER;
VAR meterLevel: INTEGER;VAR totalSamplesToRecord: LONGINT;VAR numberOfSamplesRecorded: LONGINT;
VAR totalMsecsToRecord: LONGINT;VAR numberOfMsecsRecorded: LONGINT): OSErr;
INLINE $203C,$0E34,$0014,$A800;
FUNCTION SPBGetDeviceInfo(inRefNum: LONGINT;infoType: OSType;infoData: Ptr): OSErr;
INLINE $203C,$0638,$0014,$A800;
FUNCTION SPBSetDeviceInfo(inRefNum: LONGINT;infoType: OSType;infoData: Ptr): OSErr;
INLINE $203C,$063C,$0014,$A800;
FUNCTION SPBMillisecondsToBytes(inRefNum: LONGINT;VAR milliseconds: LONGINT): OSErr;
INLINE $203C,$0440,$0014,$A800;
FUNCTION SPBBytesToMilliseconds(inRefNum: LONGINT;VAR byteCount: LONGINT): OSErr;
INLINE $203C,$0444,$0014,$A800;
FUNCTION SetupSndHeader(sndHandle: Handle;numChannels: INTEGER;sampleRate: Fixed;
sampleSize: INTEGER;compressionType: OSType;baseNote: INTEGER;numBytes: LONGINT;
VAR headerLen: INTEGER): OSErr;
INLINE $203C,$0D48,$0014,$A800;
FUNCTION SetupAIFFHeader(fRefNum: INTEGER;numChannels: INTEGER;sampleRate: Fixed;
sampleSize: INTEGER;compressionType: OSType;numBytes: LONGINT;numFrames: LONGINT): OSErr;
INLINE $203C,$0B4C,$0014,$A800;

{$ENDC} { UsingSoundInput }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE SoundInput.p}



{#####################################################################}
{### FILE: StandardFile.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:19 PM
StandardFile.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT StandardFile;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingStandardFile}
{$SETC UsingStandardFile := 1}
{$I+}
{$SETC StandardFileIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$IFC UNDEFINED UsingFiles}
{$I $$Shell(PInterfaces)Files.p}
{$ENDC}
{$SETC UsingIncludes := StandardFileIncludes}
CONST
{ resource IDs and item offsets of pre-7.0 dialogs }
putDlgID = -3999;
putSave = 1;
putCancel = 2;
putEject = 5;
putDrive = 6;
putName = 7;
getDlgID = -4000;
getOpen = 1;
getCancel = 3;
getEject = 5;



getDrive = 6;
getNmList = 7;
getScroll = 8;
{ resource IDs and item offsets of 7.0 dialogs }
sfPutDialogID = -6043;
sfGetDialogID = -6042;
sfItemOpenButton = 1;
sfItemCancelButton = 2;
sfItemBalloonHelp = 3;
sfItemVolumeUser = 4;
sfItemEjectButton = 5;
sfItemDesktopButton = 6;
sfItemFileListUser = 7;
sfItemPopUpMenuUser = 8;
sfItemDividerLinePict = 9;
sfItemFileNameTextEdit = 10;
sfItemPromptStaticText = 11;
sfItemNewFolderUser = 12;

{ pseudo-item hits for use in DlgHook }
sfHookFirstCall = -1;
sfHookCharOffset = $1000;
sfHookNullEvent = 100;
sfHookRebuildList = 101;
sfHookFolderPopUp = 102;
sfHookOpenFolder = 103;

{ the following are only in system 7.0+ }
sfHookOpenAlias = 104;
sfHookGoToDesktop = 105;
sfHookGoToAliasTarget = 106;
sfHookGoToParent = 107;
sfHookGoToNextDrive = 108;
sfHookGoToPrevDrive = 109;
sfHookChangeSelection = 110;
sfHookSetActiveOffset = 200;
sfHookLastCall = -2;
{ the refcon field of the dialog record during a
modalfilter or dialoghook contains one of the following }
sfMainDialogRefCon = 'stdf';
sfNewFolderDialogRefCon = 'nfdr';
sfReplaceDialogRefCon = 'rplc';
sfStatWarnDialogRefCon = 'stat';
sfLockWarnDialogRefCon = 'lock';
sfErrorDialogRefCon = 'err ';
TYPE
SFReply = RECORD
good: BOOLEAN;
copy: BOOLEAN;
fType: OSType;
vRefNum: INTEGER;


version: INTEGER;
fName: Str63;
END;
StandardFileReply = RECORD
sfGood: BOOLEAN;
sfReplacing: BOOLEAN;
sfType: OSType;
sfFile: FSSpec;
sfScript: ScriptCode;
sfFlags: INTEGER;
sfIsFolder: BOOLEAN;
sfIsVolume: BOOLEAN;
sfReserved1: LONGINT;
sfReserved2: INTEGER;
END;

DlgHookProcPtr = ProcPtr;
FileFilterProcPtr = ProcPtr;

{ FUNCTION Hook(item: INTEGER; theDialog: DialogPtr): INTEGER; }
{ FUNCTION FileFilter(PB: CInfoPBPtr): BOOLEAN; }

{ the following also include an extra parameter
DlgHookYDProcPtr = ProcPtr;
{ FUNCTION
ModalFilterYDProcPtr = ProcPtr;
{ FUNCTION
FileFilterYDProcPtr = ProcPtr;
{ FUNCTION
ActivateYDProcPtr = ProcPtr;
{ PROCEDURE

of "your data pointer" }
Hook(item: INTEGER; theDialog: DialogPtr; yourDataPtr: Ptr): INTEGER; }
Filter(theDialog: DialogPtr; VAR theEvent: EventRecord; VAR itemHit: INTEGER; yourDataPtr: Ptr): BOOLEAN; }
FileFilter(PB: CInfoPBPtr; yourDataPtr: Ptr): BOOLEAN; }
Activate(theDialog; DialogPtr; itemNo: INTEGER; activating: BOOLEAN; yourDataPtr: Ptr); }

SFTypeList = ARRAY [0..3] OF OSType;
PROCEDURE SFPutFile(where: Point;
prompt: Str255;
origName: Str255;
dlgHook: DlgHookProcPtr;
VAR reply: SFReply);
INLINE $3F3C,$0001,$A9EA;
PROCEDURE SFGetFile(where: Point;
prompt: Str255;
fileFilter: FileFilterProcPtr;
numTypes: INTEGER;
typeList: SFTypeList;
dlgHook: DlgHookProcPtr;
VAR reply: SFReply);
INLINE $3F3C,$0002,$A9EA;
PROCEDURE SFPPutFile(where: Point;
prompt: Str255;
origName: Str255;
dlgHook: DlgHookProcPtr;
VAR reply: SFReply;
dlgID: INTEGER;
filterProc: ModalFilterProcPtr);
INLINE $3F3C,$0003,$A9EA;
PROCEDURE SFPGetFile(where: Point;
prompt: Str255;




fileFilter: FileFilterProcPtr;
numTypes: INTEGER;
typeList: SFTypeList;
dlgHook: DlgHookProcPtr;
VAR reply: SFReply;
dlgID: INTEGER;
filterProc: ModalFilterProcPtr);
INLINE $3F3C,$0004,$A9EA;
PROCEDURE StandardPutFile(prompt: Str255;
defaultName: Str255;
VAR reply: StandardFileReply);
INLINE $3F3C,$0005,$A9EA;
PROCEDURE StandardGetFile(fileFilter: FileFilterProcPtr;
numTypes: INTEGER;
typeList: SFTypeList;
VAR reply: StandardFileReply);
INLINE $3F3C,$0006,$A9EA;
PROCEDURE CustomPutFile(prompt: Str255;
defaultName: Str255;
VAR reply: StandardFileReply;
dlgID: INTEGER;
where: Point;
dlgHook: DlgHookYDProcPtr;
filterProc: ModalFilterYDProcPtr;
activeList: Ptr;
activateProc: ActivateYDProcPtr;
yourDataPtr: UNIV Ptr);
INLINE $3F3C,$0007,$A9EA;
PROCEDURE CustomGetFile(fileFilter: FileFilterYDProcPtr;
numTypes: INTEGER;
typeList: SFTypeList;
VAR reply: StandardFileReply;
dlgID: INTEGER;
where: Point;
dlgHook: DlgHookYDProcPtr;
filterProc: ModalFilterYDProcPtr;
activeList: Ptr;
activateProc: ActivateYDProcPtr;
yourDataPtr: UNIV Ptr);
INLINE $3F3C,$0008,$A9EA;

{
New StandardFile routine comments:
activeList is pointer to array of integer (16-bits).
first integer is length of list.
following integers are possible activatable DITL items, in
the order that the tab key will cycle through. The first
in the list is the item made active when dialog is first shown.
activateProc is a pointer to a procedure like:

PROCEDURE MyActivateProc(theDialog:
itemNo:
activating:
yourDataPtr:

DialogPtr;
INTEGER;
BOOLEAN;
Ptr);

The activateProc is called with activating=FALSE on the itemNo
about to deactivate then with activating=TRUE on the itemNo
about to become the active item. (like activate event)
yourDataPtr is a nice little extra that makes life easier without
globals. CustomGetFile & CustomPPutFile when calling any of their
call back procedures, pushes the extra parameter of yourDataPtr on
the stack.
In addition the filterProc in CustomGetFile & CustomPPutFile is called
before before SF does any mapping, instead of after.
}

{$ENDC}

{ UsingStandardFile }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE StandardFile.p}



{#####################################################################}
{### FILE: Start.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:20 PM
Start.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Start;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingStart}
{$SETC UsingStart := 1}
{$I+}
{$SETC StartIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := StartIncludes}
TYPE
DefStartType = (slotDev,scsiDev);

DefStartPtr = ^DefStartRec;
DefStartRec = RECORD
CASE DefStartType OF
slotDev:
(sdExtDevID: SignedByte;
sdPartition: SignedByte;
sdSlotNum: SignedByte;
sdSRsrcID: SignedByte);
scsiDev:
(sdReserved1: SignedByte;
sdReserved2: SignedByte;
sdRefNum: INTEGER);
END;
DefVideoPtr = ^DefVideoRec;
DefVideoRec = RECORD
sdSlot: SignedByte;

sdsResource: SignedByte;
END;
DefOSPtr = ^DefOSRec;
DefOSRec = RECORD
sdReserved: SignedByte;
sdOSType: SignedByte;
END;

PROCEDURE GetDefaultStartup(paramBlock: DefStartPtr);
INLINE $205F,$A07D;
PROCEDURE SetDefaultStartup(paramBlock: DefStartPtr);
INLINE $205F,$A07E;
PROCEDURE GetVideoDefault(paramBlock: DefVideoPtr);
INLINE $205F,$A080;
PROCEDURE SetVideoDefault(paramBlock: DefVideoPtr);
INLINE $205F,$A081;
PROCEDURE GetOSDefault(paramBlock: DefOSPtr);
INLINE $205F,$A084;
PROCEDURE SetOSDefault(paramBlock: DefOSPtr);
INLINE $205F,$A083;
PROCEDURE SetTimeout(count: INTEGER);
PROCEDURE GetTimeout(VAR count: INTEGER);

{$ENDC}

{ UsingStart }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Start.p}

{#####################################################################}
{### FILE: Strings.p}
{#####################################################################}
{
Created: Friday, October 20, 1989 at 3:26 PM
Strings.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1989

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Strings;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingStrings}
{$SETC UsingStrings := 1}
{$I+}
{$SETC StringsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$SETC UsingIncludes := StringsIncludes}

FUNCTION C2PStr(cString: UNIV Ptr): StringPtr;
PROCEDURE C2PStrProc(cString: UNIV Ptr);
FUNCTION P2CStr(pString: StringPtr): Ptr;
PROCEDURE P2CStrProc(pString: StringPtr);
{$ENDC}

{ UsingStrings }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Strings.p}



{#####################################################################}
{### FILE: SysEqu.p}
{#####################################################################}

{
Created: Friday, November 15, 1991 at 9:35 AM
SysEqu.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT SysEqu;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingSysEqu}
{$SETC UsingSysEqu := 1}

CONST
PCDeskPat = $20B;
HiKeyLast = $216;
KbdLast = $218;
ExpandMem = $2B6;
SCSIBase = $0C00;
SCSIDMA = $0C04;
SCSIHsk = $0C08;
SCSIGlobals = $0C0C;
RGBBlack = $0C10;
RGBWhite = $0C16;
RowBits = $0C20;
ColLines = $0C22;
ScreenBytes = $0C24;
NMIFlag = $0C2C;
VidType = $0C2D;
VidMode = $0C2E;
SCSIPoll = $0C2F;
SEVarBase = $0C30;
MMUFlags = $0CB0;
MMUType = $0CB1;
MMU32bit = $0CB2;
MMUFluff = $0CB3;
MMUTbl = $0CB4;
MMUTblSize = $0CB8;
SInfoPtr = $0CBC;
ASCBase = $0CC0;

{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL

VAR] desktop pat, top bit only! others are in use}
VAR] Same as KbdVars}
VAR] Same as KbdVars+2}
VAR] pointer to expanded memory block}
VAR] (long) base address for SCSI chip read}
VAR] (long) base address for SCSI DMA}
VAR] (long) base address for SCSI handshake}
VAR] (long) ptr for SCSI mgr locals}
VAR] (6 bytes) the black field for color}
VAR] (6 bytes) the white field for color}
VAR] (word) screen horizontal pixels}
VAR] (word) screen vertical pixels}
VAR] (long) total screen bytes}
VAR] (byte) flag for NMI debounce}
VAR] (byte) video board type ID}
VAR] (byte) video mode (4=4bit color)}
VAR] (byte) poll for device zero only once.}
VAR] }
VAR] (byte) cleared to zero (reserved for future use)}
VAR] (byte) kind of MMU present}
VAR] (byte) boolean reflecting current machine MMU mode}
VAR] (byte) fluff byte forced by reducing MMUMode to MMU32bit.}
VAR] (long) pointer to MMU Mapping table}
VAR] (long) size of the MMU mapping table}
VAR] (long) pointer to Slot manager information}
VAR] (long) pointer to Sound Chip}

SMGlobals = $0CC4;
TheGDevice = $0CC8;
CQDGlobals = $0CCC;
ADBBase = $0CF8;
WarmStart = $0CFC;
TimeDBRA = $0D00;
TimeSCCDB = $0D02;
SlotQDT = $0D04;
SlotPrTbl = $0D08;
SlotVBLQ = $0D0C;
ScrnVBLPtr = $0D10;
SlotTICKS = $0D14;
TableSeed = $0D20;
SRsrcTblPtr = $0D24;
JVBLTask = $0D28;
WMgrCPort = $0D2C;
VertRRate = $0D30;
ChunkyDepth = $0D60;
CrsrPtr = $0D62;
PortList = $0D66;
MickeyBytes = $0D6A;
QDErrLM = $0D6E;
VIA2DT = $0D70;
SInitFlags = $0D90;
DTQueue = $0D92;
DTQFlags = $0D92;
DTskQHdr = $0D94;
DTskQTail = $0D98;
JDTInstall = $0D9C;
HiliteRGB = $0DA0;
TimeSCSIDB = $0B24;
DSCtrAdj = $0DA8;
IconTLAddr = $0DAC;
VideoInfoOK = $0DB0;
EndSRTPtr = $0DB4;
SDMJmpTblPtr = $0DB8;
JSwapMMU = $0DBC;
SdmBusErr = $0DC0;
LastTxGDevice = $0DC4;
NewCrsrJTbl = $88C;
JAllocCrsr = $88C;
JSetCCrsr = $890;
JOpcodeProc = $894;
CrsrBase = $898;
CrsrDevice = $89C;
SrcDevice = $8A0;
MainDevice = $8A4;
DeviceList = $8A8;
CrsrRow = $8AC;
QDColors = $8B0;
HiliteMode = $938;
BusErrVct = $08;
RestProc = $A8C;
ROM85 = $28E;
ROMMapHndl = $B06;
ScrVRes = $102;


{ (long)
{[GLOBAL
{ (long)
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL
{[GLOBAL

pointer to Sound Manager Globals}
VAR] (long) the current graphics device}
quickDraw global extensions}
VAR] (long) pointer to Front Desk Buss Variables}
VAR] (long) flag to indicate it is a warm start}
VAR] (word) number of iterations of DBRA per millisecond}
VAR] (word) number of iter's of SCC access & DBRA.}
VAR] ptr to slot queue table}
VAR] ptr to slot priority table}
VAR] ptr to slot VBL queue table}
VAR] save for ptr to main screen VBL queue}
VAR] ptr to slot tickcount table}
VAR] (long) seed value for color table ID's}
VAR] (long) pointer to slot resource table.}
VAR] vector to slot VBL task interrupt handler}
VAR] window manager color port }
VAR] (word) Vertical refresh rate for start manager. }
VAR] depth of the pixels}
VAR] pointer to cursor save area}
VAR] list of grafports}
VAR] long pointer to cursor stuff}
VAR] QDErr has name conflict w/ type. QuickDraw error code [word]}
VAR] 32 bytes for VIA2 dispatch table for NuMac}
VAR] StartInit.a flags [word]}
VAR] (10 bytes) deferred task queue header}
VAR] flag word for DTQueue}
VAR] ptr to head of queue}
VAR] ptr to tail of queue}
VAR] (long) ptr to deferred task install routine}
VAR] 6 bytes: rgb of hilite color}
VAR] (word) number of iter's of SCSI access & DBRA}
VAR] (long) Center adjust for DS rect.}
VAR] (long) pointer to where start icons are to be put.}
VAR] (long) Signals to CritErr that the Video card is ok}
VAR] (long) Pointer to the end of the Slot Resource Table (Not the SRT buffer).}
VAR] (long) Pointer to the SDM jump table}
VAR] (long) jump vector to SwapMMU routine}
VAR] (long) Pointer to the SDM busErr handler}
VAR] (long) copy of TheGDevice set up for fast text measure}
VAR] location of new crsr jump vectors}
VAR] (long) vector to routine that allocates cursor}
VAR] (long) vector to routine that sets color cursor}
VAR] (long) vector to process new picture opcodes}
VAR] (long) scrnBase for cursor}
VAR] (long) current cursor device}
VAR] (LONG) Src device for Stretchbits}
VAR] (long) the main screen device}
VAR] (long) list of display devices}
VAR] (word) rowbytes for current cursor screen}
VAR] (long) handle to default colors}
VAR] used for color highlighting}
VAR] bus error vector}
VAR] Resume procedure f InitDialogs [pointer]}
VAR] (word) actually high bit - 0 for ROM vers $75 (sic) and later}
VAR] (long) handle of ROM resource map}
VAR] Pixels per inch vertically (word)
screen vertical dots/inch [word]}
ScrHRes = $104;
{[GLOBAL VAR] Pixels per inch horizontally (word)
screen horizontal dots/inch [word]}
ScrnBase = $824;
{[GLOBAL VAR] Address of main screen buffer
Screen Base [pointer]}
ScreenRow = $106;
{[GLOBAL VAR] rowBytes of screen [word]}
MBTicks = $16E;
{[GLOBAL VAR] tick count @ last mouse button [long]}
JKybdTask = $21A;
{[GLOBAL VAR] keyboard VBL task hook [pointer]}
KeyLast = $184;
{[GLOBAL VAR] ASCII for last valid keycode [word]}
KeyTime = $186;
{[GLOBAL VAR] tickcount when KEYLAST was rec'd [long]}
KeyRepTime = $18A;
{[GLOBAL VAR] tickcount when key was last repeated [long]}
SPConfig = $1FB;
{[GLOBAL VAR] Use types for serial ports (byte)
config bits: 4-7 A, 0-3 B (see use type below)}
SPPortA = $1FC;
{[GLOBAL VAR] Modem port configuration (word)
SCC port A configuration [word]}
SPPortB = $1FE;
{[GLOBAL VAR] Printer port configuration (word)
SCC port B configuration [word]}
SCCRd = $1D8;
{[GLOBAL VAR] SCC read base address
SCC base read address [pointer]}
SCCWr = $1DC;
{[GLOBAL VAR] SCC write base address
SCC base write address [pointer]}
DoubleTime = $2F0;
{[GLOBAL VAR] Double-click interval in ticks (long)
double click ticks [long]}
CaretTime = $2F4;
{[GLOBAL VAR] Caret-blink interval in ticks (long)
caret blink ticks [long]}
KeyThresh = $18E;
{[GLOBAL VAR] Auto-key threshold (word)
threshold for key repeat [word]}
KeyRepThresh = $190;
{[GLOBAL VAR] Auto-key rate (word)
key repeat speed [word]}
SdVolume = $260;
{[GLOBAL VAR] Current speaker volume (byte: low-order three bits only)
Global volume(sound) control [byte]}
Ticks = $16A;
{[GLOBAL VAR] Current number of ticks since system startup (long)
Tick count, time since boot [unsigned long]}
TimeLM = $20C;
{[GLOBAL VAR] Time has name conflict w/ type. Clock time (extrapolated) [long]}
MonkeyLives = $100;
{[GLOBAL VAR] monkey lives if >= 0 [word]}
SEvtEnb = $15C;
{[GLOBAL VAR] 0 if SystemEvent should return FALSE (byte)
enable SysEvent calls from GNE [byte]}
JournalFlag = $8DE;
{[GLOBAL VAR] Journaling mode (word)
journaling state [word]}
JournalRef = $8E8;
{[GLOBAL VAR] Reference number of journaling device driver (word)
Journalling driver's refnum [word]}
BufPtr = $10C;
{[GLOBAL VAR] Address of end of jump table
top of application memory [pointer]}
StkLowPt = $110;
{[GLOBAL VAR] Lowest stack as measured in VBL task [pointer]}
TheZone = $118;
{[GLOBAL VAR] Address of current heap zone
current heap zone [pointer]}
ApplLimit = $130;
{[GLOBAL VAR] Application heap limit
application limit [pointer]}
SysZone = $2A6;
{[GLOBAL VAR] Address of system heap zone
system heap zone [pointer]}
ApplZone = $2AA;
{[GLOBAL VAR] Address of application heap zone
application heap zone [pointer]}
HeapEnd = $114;
{[GLOBAL VAR] Address of end of application heap zone
end of heap [pointer]}
HiHeapMark = $BAE;
{[GLOBAL VAR] (long) highest address used by a zone below sp<01Nov85 JTC>}
MemErr = $220;
{[GLOBAL VAR] last memory manager error [word]}



UTableBase = $11C;
{[GLOBAL VAR] Base address of unit table
unit I/O table [pointer]}
UnitNtryCnt = $1D2;
{[GLOBAL VAR] count of entries in unit table [word]}
JFetch = $8F4;
{[GLOBAL VAR] Jump vector for Fetch function
fetch a byte routine for drivers [pointer]}
JStash = $8F8;
{[GLOBAL VAR] Jump vector for Stash function
stash a byte routine for drivers [pointer]}
JIODone = $8FC;
{[GLOBAL VAR] Jump vector for IODone function
IODone entry location [pointer]}
DrvQHdr = $308;
{[GLOBAL VAR] Drive queue header (10 bytes)
queue header of drives in system [10 bytes]}
BootDrive = $210;
{[GLOBAL VAR] drive number of boot drive [word]}
EjectNotify = $338;
{[GLOBAL VAR] eject notify procedure [pointer]}
IAZNotify = $33C;
{[GLOBAL VAR] world swaps notify procedure [pointer]}
SFSaveDisk = $214;
{[GLOBAL VAR] Negative of volume reference number used by Standard File Package (word)
last vRefNum seen by standard file [word]}
CurDirStore = $398;
{[GLOBAL VAR] save dir across calls to Standard File [long]}
OneOne = $A02;
{[GLOBAL VAR] $00010001
constant $00010001 [long]}
MinusOne = $A06;
{[GLOBAL VAR] $FFFFFFFF
constant $FFFFFFFF [long]}
Lo3Bytes = $31A;
{[GLOBAL VAR] $00FFFFFF
constant $00FFFFFF [long]}
ROMBase = $2AE;
{[GLOBAL VAR] Base address of ROM
ROM base address [pointer]}
RAMBase = $2B2;
{[GLOBAL VAR] Trap dispatch table's base address for routines in RAM
RAM base address [pointer]}
SysVersion = $15A;
{[GLOBAL VAR] version # of RAM-based system [word]}
RndSeed = $156;
{[GLOBAL VAR] Random number seed (long)
random seed/number [long]}
Scratch20 = $1E4;
{[GLOBAL VAR] 20-byte scratch area
scratch [20 bytes]}
Scratch8 = $9FA;
{[GLOBAL VAR] 8-byte scratch area
scratch [8 bytes]}
ToolScratch = $9CE;
{[GLOBAL VAR] 8-byte scratch area
scratch [8 bytes]}
ApplScratch = $A78;
{[GLOBAL VAR] 12-byte application scratch area
scratch [12 bytes]}
ScrapSize = $960;
{[GLOBAL VAR] Size in bytes of desk scrap (long)
scrap length [long]}
ScrapHandle = $964;
{[GLOBAL VAR] Handle to desk scrap in memory
memory scrap [handle]}
ScrapCount = $968;
{[GLOBAL VAR] Count changed by ZeroScrap (word)
validation byte [word]}
ScrapState = $96A;
{[GLOBAL VAR] Tells where desk scrap is (word)
scrap state [word]}
ScrapName = $96C;
{[GLOBAL VAR] Pointer to scrap file name (preceded by length byte)
pointer to scrap name [pointer]}
IntlSpec = $BA0;
{[GLOBAL VAR] (long) - ptr to extra Intl data }
SwitcherTPtr = $286;
{[GLOBAL VAR] Switcher's switch table }
CPUFlag = $12F;
{[GLOBAL VAR] $00=68000, $01=68010, $02=68020 (old ROM inits to $00)}
VIA = $1D4;
{[GLOBAL VAR] VIA base address
VIA base address [pointer]}
IWM = $1E0;
{[GLOBAL VAR] IWM base address [pointer]}
Lvl1DT = $192;
{[GLOBAL VAR] Level-1 secondary interrupt vector table (32 bytes)
Interrupt level 1 dispatch table [32 bytes]}



Lvl2DT = $1B2;
{[GLOBAL VAR] Level-2 secondary interrupt vector table (32 bytes)
Interrupt level 2 dispatch table [32 bytes]}
ExtStsDT = $2BE;
{[GLOBAL VAR] External/status interrupt vector table (16 bytes)
SCC ext/sts secondary dispatch table [16 bytes]}
SPValid = $1F8;
{[GLOBAL VAR] Validity status (byte)
validation field ($A7) [byte]}
SPATalkA = $1F9;
{[GLOBAL VAR] AppleTalk node ID hint for modem port (byte)
AppleTalk node number hint for port A}
SPATalkB = $1FA;
{[GLOBAL VAR] AppleTalk node ID hint for printer port (byte)
AppleTalk node number hint for port B}
SPAlarm = $200;
{[GLOBAL VAR] Alarm setting (long)
alarm time [long]}
SPFont = $204;
{[GLOBAL VAR] Application font number minus 1 (word)
default application font number minus 1 [word]}
SPKbd = $206;
{[GLOBAL VAR] Auto-key threshold and rate (byte)
kbd repeat thresh in 4/60ths [2 4-bit]}
SPPrint = $207;
{[GLOBAL VAR] Printer connection (byte)
print stuff [byte]}
SPVolCtl = $208;
{[GLOBAL VAR] Speaker volume setting in parameter RAM (byte)
volume control [byte]}
SPClikCaret = $209;
{[GLOBAL VAR] Double-click and caret-blink times (byte)
double click/caret time in 4/60ths[2 4-bit]}
SPMisc1 = $20A;
{[GLOBAL VAR] miscellaneous [1 byte]}
SPMisc2 = $20B;
{[GLOBAL VAR] Mouse scaling, system startup disk, menu blink (byte)
miscellaneous [1 byte]}
GetParam = $1E4;
{[GLOBAL VAR] system parameter scratch [20 bytes]}
SysParam = $1F8;
{[GLOBAL VAR] Low-memory copy of parameter RAM (20 bytes)
system parameter memory [20 bytes]}
CrsrThresh = $8EC;
{[GLOBAL VAR] Mouse-scaling threshold (word)
delta threshold for mouse scaling [word]}
JCrsrTask = $8EE;
{[GLOBAL VAR] address of CrsrVBLTask [long]}
MTemp = $828;
{[GLOBAL VAR] Low-level interrupt mouse location [long]}
RawMouse = $82C;
{[GLOBAL VAR] un-jerked mouse coordinates [long]}
CrsrRect = $83C;
{[GLOBAL VAR] Cursor hit rectangle [8 bytes]}
TheCrsr = $844;
{[GLOBAL VAR] Cursor data, mask & hotspot [68 bytes]}
CrsrAddr = $888;
{[GLOBAL VAR] Address of data under cursor [long]}
CrsrSave = $88C;
{[GLOBAL VAR] data under the cursor [64 bytes]}
CrsrVis = $8CC;
{[GLOBAL VAR] Cursor visible? [byte]}
CrsrBusy = $8CD;
{[GLOBAL VAR] Cursor locked out? [byte]}
CrsrNew = $8CE;
{[GLOBAL VAR] Cursor changed? [byte]}
CrsrState = $8D0;
{[GLOBAL VAR] Cursor nesting level [word]}
CrsrObscure = $8D2;
{[GLOBAL VAR] Cursor obscure semaphore [byte]}
KbdVars = $216;
{[GLOBAL VAR] Keyboard manager variables [4 bytes]}
KbdType = $21E;
{[GLOBAL VAR] keyboard model number [byte]}
MBState = $172;
{[GLOBAL VAR] current mouse button state [byte]}
KeyMapLM = $174;
{[GLOBAL VAR] KeyMap has name conflict w/ type. Bitmap of the keyboard [4 longs]}
KeypadMap = $17C;
{[GLOBAL VAR] bitmap for numeric pad-18bits [long]}
Key1Trans = $29E;
{[GLOBAL VAR] keyboard translator procedure [pointer]}
Key2Trans = $2A2;
{[GLOBAL VAR] numeric keypad translator procedure [pointer]}
JGNEFilter = $29A;
{[GLOBAL VAR] GetNextEvent filter proc [pointer]}
KeyMVars = $B04;
{[GLOBAL VAR] (word) for ROM KEYM proc state}
Mouse = $830;
{[GLOBAL VAR] processed mouse coordinate [long]}
CrsrPin = $834;
{[GLOBAL VAR] cursor pinning rectangle [8 bytes]}
CrsrCouple = $8CF;
{[GLOBAL VAR] cursor coupled to mouse? [byte]}
CrsrScale = $8D3;
{[GLOBAL VAR] cursor scaled? [byte]}
MouseMask = $8D6;
{[GLOBAL VAR] V-H mask for ANDing with mouse [long]}



MouseOffset = $8DA;
{[GLOBAL VAR] V-H offset for adding after ANDing [long]}
AlarmState = $21F;
{[GLOBAL VAR] Bit7=parity, Bit6=beeped, Bit0=enable [byte]}
VBLQueue = $160;
{[GLOBAL VAR] Vertical retrace queue header (10 bytes)
VBL queue header [10 bytes]}
SysEvtMask = $144;
{[GLOBAL VAR] System event mask (word)
system event mask [word]}
SysEvtBuf = $146;
{[GLOBAL VAR] system event queue element buffer [pointer]}
EventQueue = $14A;
{[GLOBAL VAR] Event queue header (10 bytes)
event queue header [10 bytes]}
EvtBufCnt = $154;
{[GLOBAL VAR] max number of events in SysEvtBuf - 1 [word]}
GZRootHnd = $328;
{[GLOBAL VAR] Handle to relocatable block not to be moved by grow zone function
root handle for GrowZone [handle]}
GZRootPtr = $32C;
{[GLOBAL VAR] root pointer for GrowZone [pointer]}
GZMoveHnd = $330;
{[GLOBAL VAR] moving handle for GrowZone [handle]}
MemTop = $108;
{[GLOBAL VAR] Address of end of RAM (on Macintosh XL, end of RAM available to applications)
top of memory [pointer]}
MmInOK = $12E;
{[GLOBAL VAR] initial memory mgr checks ok? [byte]}
HpChk = $316;
{[GLOBAL VAR] heap check RAM code [pointer]}
MaskBC = $31A;
{[GLOBAL VAR] Memory Manager Byte Count Mask [long]}
MaskHandle = $31A;
{[GLOBAL VAR] Memory Manager Handle Mask [long]}
MaskPtr = $31A;
{[GLOBAL VAR] Memory Manager Pointer Mask [long]}
MinStack = $31E;
{[GLOBAL VAR] Minimum space allotment for stack (long)
min stack size used in InitApplZone [long]}
DefltStack = $322;
{[GLOBAL VAR] Default space allotment for stack (long)
default size of stack [long]}
MMDefFlags = $326;
{[GLOBAL VAR] default zone flags [word]}
DSAlertTab = $2BA;
{[GLOBAL VAR] Pointer to system error alert table in use
system error alerts [pointer]}
DSAlertRect = $3F8;
{[GLOBAL VAR] Rectangle enclosing system error alert (8 bytes)
rectangle for disk-switch alert [8 bytes]}
DSDrawProc = $334;
{[GLOBAL VAR] alternate syserror draw procedure [pointer]}
DSWndUpdate = $15D;
{[GLOBAL VAR] GNE not to paintBehind DS AlertRect? [byte]}
WWExist = $8F2;
{[GLOBAL VAR] window manager initialized? [byte]}
QDExist = $8F3;
{[GLOBAL VAR] quickdraw is initialized [byte]}
ResumeProc = $A8C;
{[GLOBAL VAR] Address of resume procedure
Resume procedure from InitDialogs [pointer]}
DSErrCode = $AF0;
{[GLOBAL VAR] Current system error ID (word)
last system error alert ID}
IntFlag = $15F;
{[GLOBAL VAR] reduce interrupt disable time when bit 7 = 0}
SerialVars = $2D0;
{[GLOBAL VAR] async driver variables [16 bytes]}
ABusVars = $2D8;
{[GLOBAL VAR] Pointer to AppleTalk variables
;Pointer to AppleTalk local variables}
ABusDCE = $2DC;
{[GLOBAL VAR] ;Pointer to AppleTalk DCE}
PortAUse = $290;
{[GLOBAL VAR] bit 7: 1 = not in use, 0 = in use}
PortBUse = $291;
{[GLOBAL VAR] Current availability of serial port B (byte)
port B use, same format as PortAUse}
SCCASts = $2CE;
{[GLOBAL VAR] SCC read reg 0 last ext/sts rupt - A [byte]}
SCCBSts = $2CF;
{[GLOBAL VAR] SCC read reg 0 last ext/sts rupt - B [byte]}
DskErr = $142;
{[GLOBAL VAR] disk routine result code [word]}
PWMBuf2 = $312;
{[GLOBAL VAR] PWM buffer 1 (or 2 if sound) [pointer]}
SoundPtr = $262;
{[GLOBAL VAR] Pointer to four-tone record
4VE sound definition table [pointer]}
SoundBase = $266;
{[GLOBAL VAR] Pointer to free-form synthesizer buffer
sound bitMap [pointer]}
SoundVBL = $26A;
{[GLOBAL VAR] vertical retrace control element [16 bytes]}
SoundDCE = $27A;
{[GLOBAL VAR] sound driver DCE [pointer]}



SoundActive = $27E;
{[GLOBAL VAR]
SoundLevel = $27F;
{[GLOBAL VAR]
current level in buffer [byte]}
CurPitch = $280;
{[GLOBAL VAR]
current pitch value [word]}
DskVerify = $12C;
{[GLOBAL VAR]
TagData = $2FA;
{[GLOBAL VAR]
BufTgFNum = $2FC;
{[GLOBAL VAR]
file number [long]}
BufTgFFlg = $300;
{[GLOBAL VAR]
flags [word]}
BufTgFBkNum = $302;
{[GLOBAL VAR]
logical block number [word]}
BufTgDate = $304;
{[GLOBAL VAR]
time stamp [word]}
ScrDmpEnb = $2F8;
{[GLOBAL VAR]
screen dump enabled? [byte]}
ScrDmpType = $2F9;
{[GLOBAL VAR]
ScrapVars = $960;
{[GLOBAL VAR]
ScrapInfo = $960;
{[GLOBAL VAR]
ScrapEnd = $980;
{[GLOBAL VAR]
ScrapTag = $970;
{[GLOBAL VAR]
LaunchFlag = $902;
{[GLOBAL VAR]
SaveSegHandle = $930;
{[GLOBAL VAR]
CurJTOffset = $934;
{[GLOBAL VAR]
current jump table offset [word]}
CurPageOption = $936;
{[GLOBAL VAR]
current page 2 configuration [word]}
LoaderPBlock = $93A;
{[GLOBAL VAR]
CurApRefNum = $900;
{[GLOBAL VAR]
refNum of application's resFile [word]}
CurrentA5 = $904;
{[GLOBAL VAR]
current value of A5 [pointer]}
CurStackBase = $908;
{[GLOBAL VAR]
current stack base [pointer]}
CurApName = $910;
{[GLOBAL VAR]
name of application [STRING[31]]}
LoadTrap = $12D;
{[GLOBAL VAR]
SegHiEnable = $BB2;
{[GLOBAL VAR]
sound is active? [byte]}
Amplitude in 740-byte buffer (byte)
Value of count in square-wave synthesizer buffer (word)
used by 3.5 disk driver for read/verify [byte]}
sector tag info for disk drivers [14 bytes]}
File tags buffer: file number (long)
File tags buffer:

flags (word:

bit 1=1 if resource fork)

File tags buffer:

logical block number (word)

File tags buffer:

date and time of last modification (long)

0 if GetNextEvent shouldn't process Command-Shift-number combinations (byte)
FF dumps screen, FE dumps front window [byte]}
scrap manager variables [32 bytes]}
scrap length [long]}
end of scrap vars}
scrap file name [STRING[15]]}
from launch or chain [byte]}
seg 0 handle [handle]}
Offset to jump table from location pointed to by A5 (word)
Sound/screen buffer configuration passed to Chain or Launch (word)
param block for ExitToShell [10 bytes]}
Reference number of current application's resource file (word)
Address of boundary between application globals and application parameters
Address of base of stack; start of application globals
Name of current application (length byte followed by up to 31 characters)
trap before launch? [byte]}
(byte) 0 to disable MoveHHi in LoadSeg}

{ Window Manager Globals }
WindowList = $9D6;
{[GLOBAL VAR] Pointer to first window in window list; 0 if using events but not windows
Z-ordered linked list of windows [pointer]}
PaintWhite = $9DC;
{[GLOBAL VAR] Flag for whether to paint window white before update event (word)
erase newly drawn windows? [word]}
WMgrPort = $9DE;
{[GLOBAL VAR] Pointer to Window Manager port
window manager's grafport [pointer]}
GrayRgn = $9EE;
{[GLOBAL VAR] Handle to region drawn as desktop
rounded gray desk region [handle]}
CurActivate = $A64;
{[GLOBAL VAR] Pointer to window to receive activate event
window slated for activate event [pointer]}
CurDeactive = $A68;
{[GLOBAL VAR] Pointer to window to receive deactivate event
window slated for deactivate event [pointer]}
DragHook = $9F6;
{[GLOBAL VAR] Address of procedure to execute during TrackGoAway, DragWindow, GrowWindow, DragGrayRgn, TrackControl, and DragC
user hook during dragging [pointer]}
DeskPattern = $A3C;
{[GLOBAL VAR] Pattern with which desktop is painted (8 bytes)




desk pattern [8 bytes]}
DeskHook = $A6C;
{[GLOBAL VAR] Address of procedure for painting desktop or responding to clicks on desktop
hook for painting the desk [pointer]}
GhostWindow = $A84;
{[GLOBAL VAR] Pointer to window never to be considered frontmost
window hidden from FrontWindow [pointer]}
{ Text Edit Globals }
TEDoText = $A70;
{[GLOBAL VAR]
textEdit doText proc hook [pointer]}
TERecal = $A74;
{[GLOBAL VAR]
textEdit recalText proc hook [pointer]}
TEScrpLength = $AB0;
{[GLOBAL VAR]
textEdit Scrap Length [word]}
TEScrpHandle = $AB4;
{[GLOBAL VAR]
textEdit Scrap [handle]}
TEWdBreak = $AF6;
{[GLOBAL VAR]
WordRedraw = $BA5;
{[GLOBAL VAR]
TESysJust = $BAC;
{[GLOBAL VAR]
{ Resource Manager Globals }
TopMapHndl = $A50;
{[GLOBAL VAR]
topmost map in list [handle]}
SysMapHndl = $A54;
{[GLOBAL VAR]
system map [handle]}
SysMap = $A58;
{[GLOBAL VAR]
reference number of system map [word]}
CurMap = $A5A;
{[GLOBAL VAR]
reference number of current map [word]}
ResReadOnly = $A5C;
{[GLOBAL VAR]
ResLoad = $A5E;
{[GLOBAL VAR]
Auto-load feature [word]}
ResErr = $A60;
{[GLOBAL VAR]
Resource error code [word]}
ResErrProc = $AF2;
{[GLOBAL VAR]
Resource error procedure [pointer]}
SysResName = $AD8;
{[GLOBAL VAR]
Name of system resource file [STRING[19]]}
RomMapInsert = $B9E;
{[GLOBAL VAR]
TmpResLoad = $B9F;
{[GLOBAL VAR]

Address of TextEdit multi-purpose routine
Address of routine to recalculate line starts for TextEdit
Size in bytes of TextEdit scrap (long)
Handle to TextEdit scrap
default word break routine [pointer]}
(byte) - used by TextEdit RecalDraw}
(word) system justification (intl. textEdit)}

Handle to resource map of most recently opened resource file
Handle to map of system resource file
Reference number of system resource file (word)
Reference number of current resource file (word)
Read only flag [word]}
Current SetResLoad state (word)
Current value of ResError (word)
Address of resource error procedure
Name of system resource file (length byte followed by up to 19 characters)
(byte) determines if we should link in map}
second byte is temporary ResLoad value.}

{ Menu Mgr globals }
MBarHeight = $BAA;

{[GLOBAL VAR] height of the menu bar}

{ CommToolbox Global }
CommToolboxGlobals = $0BB4;

{[GLOBAL VAR] pointer to CommToolbox globals }

{$ENDC} { UsingSysEqu }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE SysEqu.p}



{#####################################################################}
{### FILE: Terminals.p}
{#####################################################################}

{
Created: Thursday, September 12, 1991 at 10:33 AM
Terminals.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Terminals;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingTerminals}
{$SETC UsingTerminals := 1}
{$I+}
{$SETC TerminalsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$IFC UNDEFINED UsingCTBUtilities}
{$I $$Shell(PInterfaces)CTBUtilities.p}
{$ENDC}
{$IFC UNDEFINED UsingConnections}
{$I $$Shell(PInterfaces)Connections.p}
{$ENDC}
{$SETC UsingIncludes := TerminalsIncludes}
CONST
{ current Terminal Manager version }
curTMVersion = 2;
{ current Terminal Manager Environment Record version }
curTermEnvRecVers = 0;
{ error codes
}
tmGenericError = -1;
tmNoErr = 0;
tmNotSent = 1;
tmEnvironsChanged = 2;
tmNotSupported = 7;



tmNoTools = 8;
TYPE
TMErr = OSErr;
CONST
{ TMFlags }
tmInvisible = $00000001;
tmSaveBeforeClear = $00000002;
tmNoMenus = $00000004;
tmAutoScroll = $00000008;
tmConfigChanged = $00000010;
TYPE
{ TMFlags
}
TMFlags = LONGINT;

CONST
{ TMSelTypes & TMSearchTypes }
selTextNormal = $0001;
selTextBoxed = $0002;
selGraphicsMarquee = $0004;
selGraphicsLasso = $0008;
tmSearchNoDiacrit = $0100; {These are only for TMSearchTypes}
tmSearchNoCase = $0200;
{These are only for TMSearchTypes}
TYPE
{ TMSelTypes & TMSearchTypes }
TMSearchTypes = INTEGER;

TMSelTypes = INTEGER;
CONST
{ TMCursorTypes }
cursorText = 1;
cursorGraphics = 2;
TYPE
TMCursorTypes = INTEGER;

CONST { TMTermTypes }
tmTextTerminal = $0001;
tmGraphicsTerminal = $0002;
TYPE { TMTermTypes }
	TMTermTypes = INTEGER;

TermDataBlockPtr = ^TermDataBlock;
TermDataBlockH = ^TermDataBlockPtr;
TermDataBlock = RECORD
flags: TMTermTypes;
theData: Handle;
auxData: Handle;
reserved: LONGINT;
END;
TermEnvironPtr = ^TermEnvironRec;
TermEnvironRec = RECORD
version: INTEGER;
termType: TMTermTypes;
textRows: INTEGER;
textCols: INTEGER;
cellSize: Point;
graphicSize: Rect;
slop: Point;
auxSpace: Rect;
END;
TMSelection = RECORD
CASE INTEGER OF
1: (selRect: Rect);
2: (selRgnHandle: RgnHandle;
filler: LONGINT);
END;
{
TMTermTypes
}
TermPtr = ^TermRecord;
TermHandle = ^TermPtr;
TermRecord = RECORD
procID: INTEGER;
flags: TMFlags;
errCode: TMErr;
refCon: LONGINT;
userData: LONGINT;
defProc: ProcPtr;
config: Ptr;
oldConfig: Ptr;
environsProc: ProcPtr;
reserved1: LONGINT;
reserved2: LONGINT;
tmPrivate: Ptr;
sendProc: ProcPtr;
breakProc: ProcPtr;
cacheProc: ProcPtr;
clikLoop: ProcPtr;
owner: WindowPtr;
termRect: Rect;
viewRect: Rect;
visRect: Rect;
lastIdle: LONGINT;
selection: TMSelection;
selType: TMSelTypes;
mluField: LONGINT;
END;

{ application routines type definitions }
TerminalSendProcPtr = ProcPtr;
TerminalBreakProcPtr = ProcPtr;
TerminalCacheProcPtr = ProcPtr;
TerminalSearchCallBackProcPtr = ProcPtr;
TerminalClikLoopProcPtr = ProcPtr;
TerminalEnvironsProcPtr = ProcPtr;
TerminalChooseIdleProcPtr = ProcPtr;
FUNCTION InitTM: TMErr;
FUNCTION TMGetVersion(hTerm: TermHandle): Handle;
FUNCTION TMGetTMVersion: INTEGER;
FUNCTION TMNew(termRect: Rect;viewRect: Rect;flags: TMFlags;procID: INTEGER;
owner: WindowPtr;sendProc: TerminalSendProcPtr;cacheProc: TerminalCacheProcPtr;
breakProc: TerminalBreakProcPtr;clikLoop: TerminalClikLoopProcPtr;environsProc: TerminalEnvironsProcPtr;
refCon: LONGINT;userData: LONGINT): TermHandle;
PROCEDURE TMDispose(hTerm: TermHandle);
PROCEDURE TMKey(hTerm: TermHandle;theEvent: EventRecord);
PROCEDURE TMUpdate(hTerm: TermHandle;visRgn: RgnHandle);
PROCEDURE TMPaint(hTerm: TermHandle;theTermData: TermDataBlock;theRect: Rect);
PROCEDURE TMActivate(hTerm: TermHandle;activate: BOOLEAN);
PROCEDURE TMResume(hTerm: TermHandle;resume: BOOLEAN);
PROCEDURE TMClick(hTerm: TermHandle;theEvent: EventRecord);
PROCEDURE TMIdle(hTerm: TermHandle); 

FUNCTION TMStream(hTerm: TermHandle;theBuffer: Ptr;theLength: LONGINT;flags: CMFlags): LONGINT;
FUNCTION TMMenu(hTerm: TermHandle;menuID: INTEGER;item: INTEGER): BOOLEAN;
PROCEDURE TMReset(hTerm: TermHandle);
PROCEDURE TMClear(hTerm: TermHandle);
PROCEDURE TMResize(hTerm: TermHandle;newViewRect: Rect);
FUNCTION TMGetSelect(hTerm: TermHandle;theData: Handle;VAR theType: ResType): LONGINT;
PROCEDURE TMGetLine(hTerm: TermHandle;lineNo: INTEGER;VAR theTermData: TermDataBlock);
PROCEDURE TMSetSelection(hTerm: TermHandle;theSelection: TMSelection;selType: TMSelTypes);
PROCEDURE TMScroll(hTerm: TermHandle;dh: INTEGER;dv: INTEGER);
FUNCTION TMValidate(hTerm: TermHandle): BOOLEAN;
PROCEDURE TMDefault(VAR theConfig: Ptr;procID: INTEGER;allocate: BOOLEAN);
FUNCTION TMSetupPreflight(procID: INTEGER;VAR magicCookie: LONGINT): Handle;
PROCEDURE TMSetupSetup(procID: INTEGER;theConfig: Ptr;count: INTEGER;theDialog: DialogPtr;
VAR magicCookie: LONGINT);
FUNCTION TMSetupFilter(procID: INTEGER;theConfig: Ptr;count: INTEGER;theDialog: DialogPtr;
VAR theEvent: EventRecord;VAR theItem: INTEGER;VAR magicCookie: LONGINT): BOOLEAN;
PROCEDURE TMSetupItem(procID: INTEGER;theConfig: Ptr;count: INTEGER;theDialog: DialogPtr;
VAR theItem: INTEGER;VAR magicCookie: LONGINT);

PROCEDURE TMSetupXCleanup(procID: INTEGER;theConfig: Ptr;count: INTEGER;
theDialog: DialogPtr;OKed: BOOLEAN;VAR magicCookie: LONGINT);
PROCEDURE TMSetupPostflight(procID: INTEGER);
FUNCTION TMGetConfig(hTerm: TermHandle): Ptr;
FUNCTION TMSetConfig(hTerm: TermHandle;thePtr: Ptr): INTEGER;
FUNCTION TMIntlToEnglish(hTerm: TermHandle;inputPtr: Ptr;VAR outputPtr: Ptr;
language: INTEGER): OSErr;
FUNCTION TMEnglishToIntl(hTerm: TermHandle;inputPtr: Ptr;VAR outputPtr: Ptr;
language: INTEGER): OSErr;
PROCEDURE TMGetToolName(id: INTEGER;VAR name: Str255);
FUNCTION TMGetProcID(name: Str255): INTEGER;
PROCEDURE TMSetRefCon(hTerm: TermHandle;refCon: LONGINT);
FUNCTION TMGetRefCon(hTerm: TermHandle): LONGINT;
PROCEDURE TMSetUserData(hTerm: TermHandle;userData: LONGINT);
FUNCTION TMGetUserData(hTerm: TermHandle): LONGINT;
FUNCTION TMAddSearch(hTerm: TermHandle;theString: Str255;where: Rect;searchType: TMSearchTypes;
callBack: TerminalSearchCallBackProcPtr): INTEGER;
PROCEDURE TMRemoveSearch(hTerm: TermHandle;refnum: INTEGER);
PROCEDURE TMClearSearch(hTerm: TermHandle);
FUNCTION TMGetCursor(hTerm: TermHandle;cursType: TMCursorTypes): Point;
FUNCTION TMGetTermEnvirons(hTerm: TermHandle;VAR theEnvirons: TermEnvironRec): TMErr;
FUNCTION TMChoose(VAR hTerm: TermHandle;where: Point;idleProc: TerminalChooseIdleProcPtr): INTEGER;
PROCEDURE TMEvent(hTerm: TermHandle;theEvent: EventRecord);
FUNCTION TMDoTermKey(hTerm: TermHandle;theKey: Str255): BOOLEAN;
FUNCTION TMCountTermKeys(hTerm: TermHandle): INTEGER;
PROCEDURE TMGetIndTermKey(hTerm: TermHandle;id: INTEGER;VAR theKey: Str255);
PROCEDURE TMGetErrorString(hTerm: TermHandle;id: INTEGER;VAR errMsg: Str255);

{$ENDC} { UsingTerminals }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Terminals.p}



{#####################################################################}
{### FILE: TerminalTools.p}
{#####################################################################}

{
Created: Thursday, September 12, 1991 at 11:07 AM
TerminalTools.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT TerminalTools;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingTerminalTools}
{$SETC UsingTerminalTools := 1}
{$I+}
{$SETC TerminalToolsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$IFC UNDEFINED UsingTerminals}
{$I $$Shell(PInterfaces)Terminals.p}
{$ENDC}
{$SETC UsingIncludes := TerminalToolsIncludes}
CONST
tdefType = 'tdef';
tvalType = 'tval';
tsetType = 'tset';
tlocType = 'tloc';
tscrType = 'tscr';
tbndType = 'tbnd';
tverType = 'vers';

{ messages }
tmInitMsg = 0;
tmDisposeMsg = 1;
tmSuspendMsg = 2;
tmResumeMsg = 3;
tmMenuMsg = 4;
tmEventMsg = 5;
tmActivateMsg = 6;
tmDeactivateMsg = 7;
tmGetErrorStringMsg = 8;
tmIdleMsg = 50;
tmResetMsg = 51;
tmKeyMsg = 100;
tmStreamMsg = 101;
tmResizeMsg = 102;
tmUpdateMsg = 103;
tmClickMsg = 104;
tmGetSelectionMsg = 105;
tmSetSelectionMsg = 106;
tmScrollMsg = 107;
tmClearMsg = 108;
tmGetLineMsg = 109;
tmPaintMsg = 110;
tmCursorMsg = 111;
tmGetEnvironsMsg = 112;
tmDoTermKeyMsg = 113;
tmCountTermKeysMsg = 114;
tmGetIndTermKeyMsg = 115;
{ messages for validate DefProc }
tmValidateMsg = 0;
tmDefaultMsg = 1;
{ messages for Setup DefProc }
tmSpreflightMsg = 0;
tmSsetupMsg = 1;
tmSitemMsg = 2;
tmSfilterMsg = 3;
tmScleanupMsg = 4;

{ messages for scripting defProc }
tmMgetMsg = 0;
tmMsetMsg = 1;
{ messages for localization defProc }
tmL2English = 0;
tmL2Intl = 1;
TYPE
TMSearchBlockPtr = ^TMSearchBlock;
TMSearchBlock = RECORD
theString: StringHandle;
where: Rect;
searchType: TMSearchTypes;
callBack: ProcPtr;
refnum: INTEGER;
next: TMSearchBlockPtr;
END;
TMSetupPtr = ^TMSetupStruct;
TMSetupStruct = RECORD
theDialog: DialogPtr;

count: INTEGER;
theConfig: Ptr;
procID: INTEGER;
END;
{ procID of the tool }

{$ENDC} { UsingTerminalTools }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE TerminalTools.p}

{#####################################################################}
{### FILE: TextEdit.p}
{#####################################################################}

{
Created: Thursday, September 12, 1991 at 12:34 PM
TextEdit.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT TextEdit;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingTextEdit}
{$SETC UsingTextEdit := 1}
{$I+}
{$SETC TextEditIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := TextEditIncludes}
CONST
{ Justification styles }
teJustLeft = 0;
teJustCenter = 1;
teJustRight = -1;
teForceLeft = -2;
{ new names for the Justification styles }
teFlushDefault = 0;
{flush according to the line direction }
teCenter = 1;
{center justify }
teFlushRight = -1;
{flush right for all scripts }
teFlushLeft = -2;
{flush left for all scripts }
{ Set/Replace style modes }
fontBit = 0;
faceBit = 1;
sizeBit = 2;
clrBit = 3;
addSizeBit = 4;

{set
{set
{set
{set
{add

font}
face}
size}
color}
size mode}



toglBit = 5;

{set faces in toggle mode}

{ TESetStyle/TEContinuousStyle modes }
doFont = 1;
doFace = 2;
doSize = 4;
doColor = 8;
doAll = 15;
addSize = 16;
doToggle = 32;

{ set font (family) number}
{set character style}
{set type size}
{set color}
{set all attributes}
{adjust type size}
{toggle mode for TESetStyle & TEContinuousStyle}

{ offsets into TEDispatchRec }
EOLHook = 0;
DRAWHook = 4;
WIDTHHook = 8;
HITTESTHook = 12;
nWIDTHHook = 24;
TextWidthHook = 28;

{[ProcPtr]
{[ProcPtr]
{[ProcPtr]
{[ProcPtr]
{[ProcPtr]
{[ProcPtr]

TEEOLHook}
TEWidthHook}
TEDrawHook}
TEHitTestHook}
nTEWidthHook}
TETextWidthHook}

{ selectors for TECustomHook }
intEOLHook = 0;
intDrawHook = 1;
intWidthHook = 2;
intHitTestHook = 3;
intNWidthHook = 6;
intTextWidthHook = 7;

{TEIntHook
{TEIntHook
{TEIntHook
{TEIntHook
{TEIntHook
{TEIntHook

value}
value}
value}
value}
value for new version of WidthHook}
value for new TextWidthHook}

{ feature or bit definitions for TEFeatureFlag }
teFAutoScr = 0;
{00000001b}
teFTextBuffering = 1;
{00000010b}
teFOutlineHilite = 2;
{00000100b}
teFInlineInput = 3;
{00001000b}
teFUseTextServices = 4;
{00010000b}
{ action for the new "bit (un)set" interface, TEFeatureFlag }
TEBitClear = 0;
TEBitSet = 1;
{set the selector bit}
TEBitTest = -1;
{no change; just return the current setting}
{constants for identifying the routine that called FindWord }
teWordSelect = 4;
{clickExpand to select word}
teWordDrag = 8;
{clickExpand to drag new word}
teFromFind = 12;
{FindLine called it ($0C)}
teFromRecal = 16;
{RecalLines called it ($10)}
TYPE
TEPtr = ^TERec;
TEHandle = ^TEPtr;
TERec = RECORD
destRect: Rect;
viewRect: Rect;
selRect: Rect;
lineHeight: INTEGER;
fontAscent: INTEGER;
selPoint: Point;
selStart: INTEGER;



selEnd: INTEGER;
active: INTEGER;
wordBreak: ProcPtr;
clikLoop: ProcPtr;
clickTime: LONGINT;
clickLoc: INTEGER;
caretTime: LONGINT;
caretState: INTEGER;
just: INTEGER;
teLength: INTEGER;
hText: Handle;
recalBack: INTEGER;
recalLines: INTEGER;
clikStuff: INTEGER;
crOnly: INTEGER;
txFont: INTEGER;
txFace: Style;
{txFace is unpacked byte}
txMode: INTEGER;
txSize: INTEGER;
inPort: GrafPtr;
highHook: ProcPtr;
caretHook: ProcPtr;
nLines: INTEGER;
lineStarts: ARRAY [0..16000] OF INTEGER;
END;
CharsPtr = ^Chars;
CharsHandle = ^CharsPtr;
Chars = PACKED ARRAY [0..32000] OF CHAR;
StyleRun = RECORD
startChar: INTEGER;
styleIndex: INTEGER;
END;

{starting character position}
{index in style table}

STElement = RECORD
stCount: INTEGER;
stHeight: INTEGER;
stAscent: INTEGER;
stFont: INTEGER;
stFace: Style;
stSize: INTEGER;
stColor: RGBColor;
END;

{number of runs in this style}
{line height}
{font ascent}
{font (family) number}
{character Style}
{size in points}
{absolute (RGB) color}

STPtr = ^TEStyleTable;
STHandle = ^STPtr;
TEStyleTable = ARRAY [0..1776] OF STElement;
LHElement = RECORD
lhHeight: INTEGER;
lhAscent: INTEGER;
END;

{maximum height in line}
{maximum ascent in line}



LHPtr = ^LHTable;
LHHandle = ^LHPtr;
LHTable = ARRAY [0..8000] OF LHElement;
ScrpSTElement = RECORD
scrpStartChar: LONGINT;
scrpHeight: INTEGER;
scrpAscent: INTEGER;
scrpFont: INTEGER;
scrpFace: Style;
scrpSize: INTEGER;
scrpColor: RGBColor;
END;

{starting character position}
{starting character position}

{unpacked byte}

ScrpSTTable = ARRAY[0..1600] OF ScrpSTElement;
StScrpPtr = ^StScrpRec;
StScrpHandle = ^StScrpPtr;
StScrpRec = RECORD
scrpNStyles: INTEGER;
scrpStyleTab: ScrpSTTable;
END;
NullStPtr = ^NullStRec;
NullStHandle = ^NullStPtr;
NullStRec = RECORD
teReserved: LONGINT;
nullScrap: StScrpHandle;
END;
TEStylePtr = ^TEStyleRec;
TEStyleHandle = ^TEStylePtr;
TEStyleRec = RECORD
nRuns: INTEGER;
nStyles: INTEGER;
styleTab: STHandle;
lhTab: LHHandle;
teRefCon: LONGINT;
nullStyle: NullStHandle;
runs: ARRAY [0..8000] OF StyleRun;
END;
TextStylePtr = ^TextStyle;
TextStyleHandle = ^TextStylePtr;
TextStyle = RECORD
tsFont: INTEGER;
tsFace: Style;
tsSize: INTEGER;
tsColor: RGBColor;
END;

TEIntHook = INTEGER;
PROCEDURE TEInit;

{number of styles in scrap}
{table of styles for scrap}

{reserved for future expansion}
{handle to scrap style table}

{number of style runs}
{size of style table}
{handle to style table}
{handle to line-height table}
{reserved for application use}
{Handle to style set at null selection}
{ARRAY [0..8000] OF StyleRun}

{font (family) number}
{character Style}
{size in point}
{absolute (RGB) color}



INLINE $A9CC;
FUNCTION TENew(destRect: Rect;viewRect: Rect): TEHandle;
INLINE $A9D2;
PROCEDURE TEDispose(hTE: TEHandle);
INLINE $A9CD;
PROCEDURE TESetText(text: Ptr;length: LONGINT;hTE: TEHandle);
INLINE $A9CF;
FUNCTION TEGetText(hTE: TEHandle): CharsHandle;
INLINE $A9CB;
PROCEDURE TEIdle(hTE: TEHandle);
INLINE $A9DA;
PROCEDURE TESetSelect(selStart: LONGINT;selEnd: LONGINT;hTE: TEHandle);
INLINE $A9D1;
PROCEDURE TEActivate(hTE: TEHandle);
INLINE $A9D8;
PROCEDURE TEDeactivate(hTE: TEHandle);
INLINE $A9D9;
PROCEDURE TEKey(key: CHAR;hTE: TEHandle);
INLINE $A9DC;
PROCEDURE TECut(hTE: TEHandle);
INLINE $A9D6;
PROCEDURE TECopy(hTE: TEHandle);
INLINE $A9D5;
PROCEDURE TEPaste(hTE: TEHandle);
INLINE $A9DB;
PROCEDURE TEDelete(hTE: TEHandle);
INLINE $A9D7;
PROCEDURE TEInsert(text: Ptr;length: LONGINT;hTE: TEHandle);
INLINE $A9DE;
PROCEDURE TESetJust(just: INTEGER;hTE: TEHandle);
INLINE $A9DF;
PROCEDURE TEUpdate(rUpdate: Rect;hTE: TEHandle);
INLINE $A9D3;
PROCEDURE TextBox(text: Ptr;length: LONGINT;box: Rect;just: INTEGER);
INLINE $A9CE;
PROCEDURE TEScroll(dh: INTEGER;dv: INTEGER;hTE: TEHandle);
INLINE $A9DD;
PROCEDURE TESelView(hTE: TEHandle);
INLINE $A811;
PROCEDURE TEPinScroll(dh: INTEGER;dv: INTEGER;hTE: TEHandle);
INLINE $A812;
PROCEDURE TEAutoView(fAuto: BOOLEAN;hTE: TEHandle);
INLINE $A813;
FUNCTION TEScrapHandle: Handle;
INLINE $2EB8,$0AB4;
PROCEDURE TECalText(hTE: TEHandle);
INLINE $A9D0;
FUNCTION TEGetOffset(pt: Point;hTE: TEHandle): INTEGER;
INLINE $A83C;
FUNCTION TEGetPoint(offset: INTEGER;hTE: TEHandle): Point;
INLINE $3F3C,$0008,$A83D;
PROCEDURE TEClick(pt: Point;fExtend: BOOLEAN;h: TEHandle);
INLINE $A9D4;
FUNCTION TEStylNew(destRect: Rect;viewRect: Rect): TEHandle;
INLINE $A83E;
FUNCTION TEStyleNew(destRect: Rect;viewRect: Rect): TEHandle;



INLINE $A83E;
PROCEDURE SetStylHandle(theHandle: TEStyleHandle;hTE: TEHandle);
INLINE $3F3C,$0005,$A83D;
PROCEDURE SetStyleHandle(theHandle: TEStyleHandle;hTE: TEHandle);
INLINE $3F3C,$0005,$A83D;
FUNCTION GetStylHandle(hTE: TEHandle): TEStyleHandle;
INLINE $3F3C,$0004,$A83D;
FUNCTION GetStyleHandle(hTE: TEHandle): TEStyleHandle;
INLINE $3F3C,$0004,$A83D;
PROCEDURE TEGetStyle(offset: INTEGER;VAR theStyle: TextStyle;VAR lineHeight: INTEGER;
VAR fontAscent: INTEGER;hTE: TEHandle);
INLINE $3F3C,$0003,$A83D;
PROCEDURE TEStylPaste(hTE: TEHandle);
INLINE $3F3C,$0000,$A83D;
PROCEDURE TEStylePaste(hTE: TEHandle);
INLINE $3F3C,$0000,$A83D;
PROCEDURE TESetStyle(mode: INTEGER;newStyle: TextStyle;redraw: BOOLEAN;
hTE: TEHandle);
INLINE $3F3C,$0001,$A83D;
PROCEDURE TEReplaceStyle(mode: INTEGER;oldStyle: TextStyle;newStyle: TextStyle;
redraw: BOOLEAN;hTE: TEHandle);
INLINE $3F3C,$0002,$A83D;
FUNCTION GetStylScrap(hTE: TEHandle): StScrpHandle;
INLINE $3F3C,$0006,$A83D;
FUNCTION GetStyleScrap(hTE: TEHandle): StScrpHandle;
INLINE $3F3C,$0006,$A83D;
PROCEDURE TEStylInsert(text: Ptr;length: LONGINT;hST: StScrpHandle;hTE: TEHandle);
INLINE $3F3C,$0007,$A83D;
PROCEDURE TEStyleInsert(text: Ptr;length: LONGINT;hST: StScrpHandle;hTE: TEHandle);
INLINE $3F3C,$0007,$A83D;
FUNCTION TEGetHeight(endLine: LONGINT;startLine: LONGINT;hTE: TEHandle): LONGINT;
INLINE $3F3C,$0009,$A83D;
FUNCTION TEContinuousStyle(VAR mode: INTEGER;VAR aStyle: TextStyle;hTE: TEHandle): BOOLEAN;
INLINE $3F3C,$000A,$A83D;
PROCEDURE SetStylScrap(rangeStart: LONGINT;rangeEnd: LONGINT;newStyles: StScrpHandle;
redraw: BOOLEAN;hTE: TEHandle);
INLINE $3F3C,$000B,$A83D;
PROCEDURE SetStyleScrap(rangeStart: LONGINT;rangeEnd: LONGINT;newStyles: StScrpHandle;
redraw: BOOLEAN;hTE: TEHandle);
INLINE $3F3C,$000B,$A83D;
PROCEDURE TECustomHook(which: TEIntHook;VAR addr: ProcPtr;hTE: TEHandle);
INLINE $3F3C,$000C,$A83D;
FUNCTION TENumStyles(rangeStart: LONGINT;rangeEnd: LONGINT;hTE: TEHandle): LONGINT;
INLINE $3F3C,$000D,$A83D;
FUNCTION TEFeatureFlag(feature: INTEGER;action: INTEGER;hTE: TEHandle): INTEGER;
INLINE $3F3C,$000E,$A83D;
FUNCTION TEGetScrapLen: LONGINT;
PROCEDURE TESetScrapLen(length: LONGINT);
FUNCTION TEFromScrap: OSErr;
FUNCTION TEToScrap: OSErr;
PROCEDURE SetClikLoop(clikProc: ProcPtr;hTE: TEHandle);
PROCEDURE SetWordBreak(wBrkProc: ProcPtr;hTE: TEHandle);

{$ENDC} { UsingTextEdit }



{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE TextEdit.p}





{#####################################################################}
{### FILE: Timer.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:25 PM
Timer.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Timer;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingTimer}
{$SETC UsingTimer := 1}
{$I+}
{$SETC TimerIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingTypes}
{$I $$Shell(PInterfaces)Types.p}
{$ENDC}
{$IFC UNDEFINED UsingOSUtils}
{$I $$Shell(PInterfaces)OSUtils.p}
{$ENDC}
{$SETC UsingIncludes := TimerIncludes}
TYPE
TMTaskPtr = ^TMTask;
TMTask = RECORD
qLink: QElemPtr;
qType: INTEGER;
tmAddr: ProcPtr;
tmCount: LONGINT;
tmWakeUp: LONGINT;
tmReserved: LONGINT;
END;

PROCEDURE InsTime(tmTaskPtr: QElemPtr);
INLINE $205F,$A058;
PROCEDURE InsXTime(tmTaskPtr: QElemPtr);
INLINE $205F,$A458;
PROCEDURE PrimeTime(tmTaskPtr: QElemPtr;count: LONGINT);
INLINE $201F,$205F,$A05A;



PROCEDURE RmvTime(tmTaskPtr: QElemPtr);
INLINE $205F,$A059;

{$ENDC}

{ UsingTimer }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Timer.p}



{#####################################################################}
{### FILE: ToolIntf.p}
{#####################################################################}
{
File: ToolIntf.p
As of MPW 3.0, interface files were reorganized to more closely
match "Inside Macintosh" reference books and be more consistant
from language to language.
Interfaces for the Macintosh toolbox calls are now found in the
files included below. This file is provided for compatibility
with old sources.
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988
All Rights Reserved
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT ToolIntf;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingToolIntf}
{$SETC UsingToolIntf := 1}
{$I+}
{$SETC ToolIntfIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingEvents}
{$I $$Shell(PInterfaces)Events.p}
{$ENDC}
{$IFC UNDEFINED UsingControls}
{$I $$Shell(PInterfaces)Controls.p}
{$ENDC}
{$IFC UNDEFINED UsingDesk}
{$I $$Shell(PInterfaces)Desk.p}
{$ENDC}
{$IFC UNDEFINED UsingWindows}
{$I $$Shell(PInterfaces)Windows.p}
{$ENDC}
{$IFC UNDEFINED UsingTextEdit}
{$I $$Shell(PInterfaces)TextEdit.p}
{$ENDC}
{$IFC UNDEFINED UsingDialogs}
{$I $$Shell(PInterfaces)Dialogs.p}
{$ENDC}
{$IFC UNDEFINED UsingFonts}
{$I $$Shell(PInterfaces)Fonts.p}

{$ENDC}
{$IFC UNDEFINED UsingLists}
{$I $$Shell(PInterfaces)Lists.p}
{$ENDC}
{$IFC UNDEFINED UsingMenus}
{$I $$Shell(PInterfaces)Menus.p}
{$ENDC}
{$IFC UNDEFINED UsingResources}
{$I $$Shell(PInterfaces)Resources.p}
{$ENDC}
{$IFC UNDEFINED UsingScrap}
{$I $$Shell(PInterfaces)Scrap.p}
{$ENDC}
{$IFC UNDEFINED UsingToolUtils}
{$I $$Shell(PInterfaces)ToolUtils.p}
{$ENDC}
{$SETC UsingIncludes := ToolIntfIncludes}
{$ENDC}

{ UsingToolIntf }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE ToolIntf.p}



{#####################################################################}
{### FILE: ToolUtils.p}
{#####################################################################}
{
Created: Sunday, January 6, 1991 at 11:25 PM
ToolUtils.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved

1985-1990

}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT ToolUtils;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingToolUtils}
{$SETC UsingToolUtils := 1}
{$I+}
{$SETC ToolUtilsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := ToolUtilsIncludes}
CONST
sysPatListID = 0;
iBeamCursor = 1;
crossCursor = 2;
plusCursor = 3;
watchCursor = 4;
TYPE
Int64Bit = RECORD
hiLong: LONGINT;
loLong: LONGINT;
END;

FUNCTION FixRatio(numer: INTEGER;denom: INTEGER): Fixed;
INLINE $A869;
FUNCTION FixMul(a: Fixed;b: Fixed): Fixed;
INLINE $A868;
FUNCTION FixRound(x: Fixed): INTEGER;
INLINE $A86C;
FUNCTION GetString(stringID: INTEGER): StringHandle;



INLINE $A9BA;
FUNCTION Munger(h: Handle;offset: LONGINT;ptr1: Ptr;len1: LONGINT;ptr2: Ptr;
len2: LONGINT): LONGINT;
INLINE $A9E0;
PROCEDURE PackBits(VAR srcPtr: Ptr;VAR dstPtr: Ptr;srcBytes: INTEGER);
INLINE $A8CF;
PROCEDURE UnpackBits(VAR srcPtr: Ptr;VAR dstPtr: Ptr;dstBytes: INTEGER);
INLINE $A8D0;
FUNCTION BitTst(bytePtr: Ptr;bitNum: LONGINT): BOOLEAN;
INLINE $A85D;
PROCEDURE BitSet(bytePtr: Ptr;bitNum: LONGINT);
INLINE $A85E;
PROCEDURE BitClr(bytePtr: Ptr;bitNum: LONGINT);
INLINE $A85F;
FUNCTION BitAnd(value1: LONGINT;value2: LONGINT): LONGINT;
INLINE $A858;
FUNCTION BitOr(value1: LONGINT;value2: LONGINT): LONGINT;
INLINE $A85B;
FUNCTION BitXor(value1: LONGINT;value2: LONGINT): LONGINT;
INLINE $A859;
FUNCTION BitNot(value: LONGINT): LONGINT;
INLINE $A85A;
FUNCTION BitShift(value: LONGINT;count: INTEGER): LONGINT;
INLINE $A85C;
FUNCTION HiWord(x: LONGINT): INTEGER;
INLINE $A86A;
FUNCTION LoWord(x: LONGINT): INTEGER;
INLINE $A86B;
PROCEDURE LongMul(a: LONGINT;b: LONGINT;VAR result: Int64Bit);
INLINE $A867;
FUNCTION GetIcon(iconID: INTEGER): Handle;
INLINE $A9BB;
PROCEDURE PlotIcon(theRect: Rect;theIcon: Handle);
INLINE $A94B;
FUNCTION GetPattern(patternID: INTEGER): PatHandle;
INLINE $A9B8;
FUNCTION GetCursor(cursorID: INTEGER): CursHandle;
INLINE $A9B9;
FUNCTION GetPicture(pictureID: INTEGER): PicHandle;
INLINE $A9BC;
FUNCTION SlopeFromAngle(angle: INTEGER): Fixed;
INLINE $A8BC;
FUNCTION AngleFromSlope(slope: Fixed): INTEGER;
INLINE $A8C4;
PROCEDURE SetString(theString: StringHandle;strNew: Str255);
INLINE $A907;
FUNCTION DeltaPoint(ptA: Point;ptB: Point): LONGINT;
INLINE $A94F;
FUNCTION NewString(theString: Str255): StringHandle;
INLINE $A906;
PROCEDURE ShieldCursor(shieldRect: Rect;offsetPt: Point);
INLINE $A855;
PROCEDURE GetIndString(VAR theString: Str255;strListID: INTEGER;index: INTEGER);
PROCEDURE ScreenRes(VAR scrnHRes: INTEGER;VAR scrnVRes: INTEGER);
INLINE $225F,$32B8,$0102,$225F,$32B8,$0104;
PROCEDURE GetIndPattern(VAR thePat: Pattern;patternListID: INTEGER;index: INTEGER);


{$ENDC}
{ UsingToolUtils }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE ToolUtils.p}


{ Added by uxmal: Apple SANE numeric library bindings. Derived from:
  http://www.mac.linux-m68k.org/devel/macalmanac.php

_PACK 4  -  $A9EB       FLOATING-POINT MATH             Stack-based

NOTE: The Standard Apple Numerics Environment (SANE) declares a second name for
      this trap: _FP68K. SANE also requires the data type of routine parameters;
      this is accomplished by setting certain high bits of the routine number
      (and altering the 16-bit value). The numbers listed below, with all high
      bits clear, will work for 80-bit extended-precision floating-point param-
      eters. See the Apple Numerics Manual for more information.

Name                 Sel     Name                Sel     Name                Sel
-----------------  -----    -----------------  -----    -----------------  -----
FOABS                 15    FOLOGB                26    FOGETENV               3
FOADD                  0    FOMUL                  4    FOSETHV                5
FOB2D                 11    FONEG                 13    FOSETXCP              21
FOCLASS               28    FONEXT                19    FOSQRT                18
FOCMP                  8    FOPROCENTRY           23    FOSUB                  2
FOCPX                 10    FOPROCEXIT            25    FOTESTXCP             27
FOCPYSGN              17    FOREM                 12    FOTTI                 22
FOD2B                  9    FORTI                 20    FOX2Z                 16
FODIV                  6    FOSCALB               24    FOZ2X                 14
FOGETHV                7    FOSETENV               1
}

UNIT SANE;
INTERFACE 

PROCEDURE FOB2D(arg0: real; arg1: real);
INLINE $3F3C,$000B,$A9EB;

END.


{#####################################################################}
{### FILE: Traps.p}
{#####################################################################}

{
Created: Saturday, December 7, 1991 at 12:50 PM
Traps.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1986-1991
All rights reserved
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Traps;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingTraps}
{$SETC UsingTraps := 1}
CONST
{
; QuickDraw
}
_CopyMask = $A817;
_MeasureText = $A837;
_GetMaskTable = $A836;
_CalcMask = $A838;
_SeedFill = $A839;
_InitCursor = $A850;
_SetCursor = $A851;
_HideCursor = $A852;
_ShowCursor = $A853;
_ShieldCursor = $A855;
_ObscureCursor = $A856;
_BitAnd = $A858;
_BitXOr = $A859;
_BitNot = $A85A;
_BitOr = $A85B;
_BitShift = $A85C;
_BitTst = $A85D;
_BitSet = $A85E;
_BitClr = $A85F;
_Random = $A861;
_ForeColor = $A862;
_BackColor = $A863;
_ColorBit = $A864;
_GetPixel = $A865;
_StuffHex = $A866;
_LongMul = $A867;
_FixMul = $A868;
_FixRatio = $A869;
_HiWord = $A86A;
_LoWord = $A86B;
_FixRound = $A86C;
_InitPort = $A86D;
_InitGraf = $A86E;
_OpenPort = $A86F;
_LocalToGlobal = $A870;
_GlobalToLocal = $A871;
_GrafDevice = $A872;
_SetPort = $A873;
_GetPort = $A874;
_SetPBits = $A875;
_PortSize = $A876;
_MovePortTo = $A877;
_SetOrigin = $A878;
_SetClip = $A879;
_GetClip = $A87A;
_ClipRect = $A87B;
_BackPat = $A87C;
_ClosePort = $A87D;
_AddPt = $A87E;
_SubPt = $A87F;
_SetPt = $A880;
_EqualPt = $A881;
_StdText = $A882;
_DrawChar = $A883;
_DrawString = $A884;
_DrawText = $A885;
_TextWidth = $A886;
_TextFont = $A887;
_TextFace = $A888;
_TextMode = $A889;
_TextSize = $A88A;
_GetFontInfo = $A88B;
_StringWidth = $A88C;
_CharWidth = $A88D;
_SpaceExtra = $A88E;
_StdLine = $A890;
_LineTo = $A891;
_Line = $A892;
_MoveTo = $A893;
_Move = $A894;
_ShutDown = $A895;
_HidePen = $A896;
_ShowPen = $A897;
_GetPenState = $A898;
_SetPenState = $A899;
_GetPen = $A89A;
_PenSize = $A89B;
_PenMode = $A89C;
_PenPat = $A89D;
_PenNormal = $A89E;
_Unimplemented = $A89F;
_StdRect = $A8A0;
_FrameRect = $A8A1;
_PaintRect = $A8A2;
_EraseRect = $A8A3;
_InverRect = $A8A4;
_FillRect = $A8A5;
_EqualRect = $A8A6;
_SetRect = $A8A7;
_OffsetRect = $A8A8;
_InsetRect = $A8A9;
_SectRect = $A8AA;
_UnionRect = $A8AB;
_Pt2Rect = $A8AC;
_PtInRect = $A8AD;
_EmptyRect = $A8AE;
_StdRRect = $A8AF;
_FrameRoundRect = $A8B0;
_PaintRoundRect = $A8B1;
_EraseRoundRect = $A8B2;
_InverRoundRect = $A8B3;
_FillRoundRect = $A8B4;
_StdOval = $A8B6;
_FrameOval = $A8B7;
_PaintOval = $A8B8;
_EraseOval = $A8B9;
_InvertOval = $A8BA;
_FillOval = $A8BB;
_SlopeFromAngle = $A8BC;
_StdArc = $A8BD;
_FrameArc = $A8BE;
_PaintArc = $A8BF;
_EraseArc = $A8C0;
_InvertArc = $A8C1;
_FillArc = $A8C2;
_PtToAngle = $A8C3;
_AngleFromSlope = $A8C4;
_StdPoly = $A8C5;
_FramePoly = $A8C6;
_PaintPoly = $A8C7;
_ErasePoly = $A8C8;
_InvertPoly = $A8C9;
_FillPoly = $A8CA;
_OpenPoly = $A8CB;
_ClosePgon = $A8CC;
_ClosePoly = $A8CC;
_KillPoly = $A8CD;
_OffsetPoly = $A8CE;
_PackBits = $A8CF;
_UnpackBits = $A8D0;
_StdRgn = $A8D1;
_FrameRgn = $A8D2;
_PaintRgn = $A8D3;
_EraseRgn = $A8D4;
_InverRgn = $A8D5;
_FillRgn = $A8D6;
_BitMapRgn = $A8D7;
_BitMapToRegion = $A8D7;
_NewRgn = $A8D8;
_DisposRgn = $A8D9;
_DisposeRgn = $A8D9;
_OpenRgn = $A8DA;
_CloseRgn = $A8DB;
_CopyRgn = $A8DC;
_SetEmptyRgn = $A8DD;
_SetRecRgn = $A8DE;
_RectRgn = $A8DF;
_OfsetRgn = $A8E0;
_OffsetRgn = $A8E0;
_InsetRgn = $A8E1;
_EmptyRgn = $A8E2;
_EqualRgn = $A8E3;
_SectRgn = $A8E4;
_UnionRgn = $A8E5;
_DiffRgn = $A8E6;
_XOrRgn = $A8E7;
_PtInRgn = $A8E8;
_RectInRgn = $A8E9;
_SetStdProcs = $A8EA;
_StdBits = $A8EB;
_CopyBits = $A8EC;
_StdTxMeas = $A8ED;
_StdGetPic = $A8EE;
_ScrollRect = $A8EF;
_StdPutPic = $A8F0;
_StdComment = $A8F1;
_PicComment = $A8F2;
_OpenPicture = $A8F3;
_ClosePicture = $A8F4;
_KillPicture = $A8F5;
_DrawPicture = $A8F6;
_Layout = $A8F7;
_ScalePt = $A8F8;
_MapPt = $A8F9;
_MapRect = $A8FA;
_MapRgn = $A8FB;
_MapPoly = $A8FC;
{
; Toolbox
}
_Count1Resources = $A80D;
_Get1IxResource = $A80E;
_Get1IxType = $A80F;
_Unique1ID = $A810;
_TESelView = $A811;
_TEPinScroll = $A812;
_TEAutoView = $A813;
_Pack8 = $A816;
_FixATan2 = $A818;
_XMunger = $A819;
_HOpenResFile = $A81A;
_HCreateResFile = $A81B;
_Count1Types = $A81C;
_Get1Resource = $A81F;
_Get1NamedResource = $A820;
_MaxSizeRsrc = $A821;
_InsMenuItem = $A826;
_HideDItem = $A827;
_ShowDItem = $A828;
_LayerDispatch = $A829;
_Pack9 = $A82B;
_Pack10 = $A82C;
_Pack11 = $A82D;
_Pack12 = $A82E;
_Pack13 = $A82F;
_Pack14 = $A830;
_Pack15 = $A831;
_ScrnBitMap = $A833;
_SetFScaleDisable = $A834;
_FontMetrics = $A835;
_ZoomWindow = $A83A;
_TrackBox = $A83B;
_PrGlue = $A8FD;
_InitFonts = $A8FE;
_GetFName = $A8FF;
_GetFNum = $A900;
_FMSwapFont = $A901;
_RealFont = $A902;
_SetFontLock = $A903;
_DrawGrowIcon = $A904;
_DragGrayRgn = $A905;
_NewString = $A906;
_SetString = $A907;
_ShowHide = $A908;
_CalcVis = $A909;
_CalcVBehind = $A90A;
_ClipAbove = $A90B;
_PaintOne = $A90C;
_PaintBehind = $A90D;
_SaveOld = $A90E;
_DrawNew = $A90F;
_GetWMgrPort = $A910;
_CheckUpDate = $A911;
_InitWindows = $A912;
_NewWindow = $A913;
_DisposWindow = $A914;
_DisposeWindow = $A914;
_ShowWindow = $A915;
_HideWindow = $A916;
_GetWRefCon = $A917;
_SetWRefCon = $A918;
_GetWTitle = $A919;
_SetWTitle = $A91A;
_MoveWindow = $A91B;
_HiliteWindow = $A91C;
_SizeWindow = $A91D;
_TrackGoAway = $A91E;
_SelectWindow = $A91F;
_BringToFront = $A920;
_SendBehind = $A921;
_BeginUpDate = $A922;
_EndUpDate = $A923;
_FrontWindow = $A924;
_DragWindow = $A925;
_DragTheRgn = $A926;
_InvalRgn = $A927;
_InvalRect = $A928;
_ValidRgn = $A929;
_ValidRect = $A92A;
_GrowWindow = $A92B;
_FindWindow = $A92C;
_CloseWindow = $A92D;
_SetWindowPic = $A92E;
_GetWindowPic = $A92F;
_InitMenus = $A930;
_NewMenu = $A931;
_DisposMenu = $A932;
_DisposeMenu = $A932;
_AppendMenu = $A933;
_ClearMenuBar = $A934;
_InsertMenu = $A935;
_DeleteMenu = $A936;
_DrawMenuBar = $A937;
_InvalMenuBar = $A81D;
_HiliteMenu = $A938;
_EnableItem = $A939;
_DisableItem = $A93A;
_GetMenuBar = $A93B;
_SetMenuBar = $A93C;
_MenuSelect = $A93D;
_MenuKey = $A93E;
_GetItmIcon = $A93F;
_SetItmIcon = $A940;
_GetItmStyle = $A941;
_SetItmStyle = $A942;
_GetItmMark = $A943;
_SetItmMark = $A944;
_CheckItem = $A945;
_GetItem = $A946;
_SetItem = $A947;
_CalcMenuSize = $A948;
_GetMHandle = $A949;
_SetMFlash = $A94A;
_PlotIcon = $A94B;
_FlashMenuBar = $A94C;
_AddResMenu = $A94D;
_PinRect = $A94E;
_DeltaPoint = $A94F;
_CountMItems = $A950;
_InsertResMenu = $A951;
_DelMenuItem = $A952;
_UpdtControl = $A953;
_NewControl = $A954;
_DisposControl = $A955;
_DisposeControl = $A955;
_KillControls = $A956;
_ShowControl = $A957;
_HideControl = $A958;
_MoveControl = $A959;
_GetCRefCon = $A95A;
_SetCRefCon = $A95B;
_SizeControl = $A95C;
_HiliteControl = $A95D;
_GetCTitle = $A95E;
_SetCTitle = $A95F;
_GetCtlValue = $A960;
_GetMinCtl = $A961;
_GetMaxCtl = $A962;
_SetCtlValue = $A963;
_SetMinCtl = $A964;
_SetMaxCtl = $A965;
_TestControl = $A966;
_DragControl = $A967;
_TrackControl = $A968;
_DrawControls = $A969;
_GetCtlAction = $A96A;
_SetCtlAction = $A96B;
_FindControl = $A96C;
_Draw1Control = $A96D;
_Dequeue = $A96E;
_Enqueue = $A96F;
_WaitNextEvent = $A860;
_GetNextEvent = $A970;
_EventAvail = $A971;
_GetMouse = $A972;
_StillDown = $A973;
_Button = $A974;
_TickCount = $A975;
_GetKeys = $A976;
_WaitMouseUp = $A977;
_UpdtDialog = $A978;
_CouldDialog = $A979;
_FreeDialog = $A97A;
_InitDialogs = $A97B;
_GetNewDialog = $A97C;
_NewDialog = $A97D;
_SelIText = $A97E;
_IsDialogEvent = $A97F;
_DialogSelect = $A980;
_DrawDialog = $A981;
_CloseDialog = $A982;
_DisposDialog = $A983;
_DisposeDialog = $A983;
_FindDItem = $A984;
_Alert = $A985;
_StopAlert = $A986;
_NoteAlert = $A987;
_CautionAlert = $A988;
_CouldAlert = $A989;
_FreeAlert = $A98A;
_ParamText = $A98B;
_ErrorSound = $A98C;
_GetDItem = $A98D;
_SetDItem = $A98E;
_SetIText = $A98F;
_GetIText = $A990;
_ModalDialog = $A991;
_DetachResource = $A992;
_SetResPurge = $A993;
_CurResFile = $A994;
_InitResources = $A995;
_RsrcZoneInit = $A996;
_OpenResFile = $A997;
_UseResFile = $A998;
_UpdateResFile = $A999;
_CloseResFile = $A99A;
_SetResLoad = $A99B;
_CountResources = $A99C;
_GetIndResource = $A99D;
_CountTypes = $A99E;
_GetIndType = $A99F;
_GetResource = $A9A0;
_GetNamedResource = $A9A1;
_LoadResource = $A9A2;
_ReleaseResource = $A9A3;
_HomeResFile = $A9A4;
_SizeRsrc = $A9A5;
_GetResAttrs = $A9A6;
_SetResAttrs = $A9A7;
_GetResInfo = $A9A8;
_SetResInfo = $A9A9;
_ChangedResource = $A9AA;
_AddResource = $A9AB;
_AddReference = $A9AC;
_RmveResource = $A9AD;
_RmveReference = $A9AE;
_ResError = $A9AF;
_WriteResource = $A9B0;
_CreateResFile = $A9B1;
_SystemEvent = $A9B2;
_SystemClick = $A9B3;
_SystemTask = $A9B4;
_SystemMenu = $A9B5;
_OpenDeskAcc = $A9B6;
_CloseDeskAcc = $A9B7;
_GetPattern = $A9B8;
_GetCursor = $A9B9;
_GetString = $A9BA;
_GetIcon = $A9BB;
_GetPicture = $A9BC;
_GetNewWindow = $A9BD;
_GetNewControl = $A9BE;


_GetRMenu = $A9BF;
_GetNewMBar = $A9C0;
_UniqueID = $A9C1;
_SysEdit = $A9C2;
_OpenRFPerm = $A9C4;
_RsrcMapEntry = $A9C5;
_Secs2Date = $A9C6;
_Date2Secs = $A9C7;
_SysBeep = $A9C8;
_SysError = $A9C9;
_PutIcon = $A9CA;
_Munger = $A9E0;
_HandToHand = $A9E1;
_PtrToXHand = $A9E2;
_PtrToHand = $A9E3;
_HandAndHand = $A9E4;
_InitPack = $A9E5;
_InitAllPacks = $A9E6;
_Pack0 = $A9E7;
_Pack1 = $A9E8;
_Pack2 = $A9E9;
_Pack3 = $A9EA;
_FP68K = $A9EB;
_Pack4 = $A9EB;



_Elems68K = $A9EC;
_Pack5 = $A9EC;
_Pack6 = $A9ED;
_DECSTR68K = $A9EE;
_Pack7 = $A9EE;
_PtrAndHand = $A9EF;
_LoadSeg = $A9F0;
_UnLoadSeg = $A9F1;
_Launch = $A9F2;
_Chain = $A9F3;
_ExitToShell = $A9F4;
_GetAppParms = $A9F5;
_GetResFileAttrs = $A9F6;
_SetResFileAttrs = $A9F7;
_MethodDispatch = $A9F8;
_InfoScrap = $A9F9;
_UnlodeScrap = $A9FA;
_UnloadScrap = $A9FA;
_LodeScrap = $A9FB;
_LoadScrap = $A9FB;
_ZeroScrap = $A9FC;
_GetScrap = $A9FD;
_PutScrap = $A9FE;
_Debugger = $A9FF;
_IconDispatch = $ABC9;
_DebugStr = $ABFF;
{
; Resource Manager
}
_ResourceDispatch = $A822;





{
; PPCToolbox
}
_PPC = $A0DD;
{
; Alias Manager
}
_AliasDispatch = $A823;
{
; Component Manager
}
_ComponentDispatch = $A82A;
{
; Device Manager (some shared by the File Manager)
}
_Open = $A000;
_Close = $A001;
_Read = $A002;
_Write = $A003;
_Control = $A004;
_Status = $A005;
_KillIO = $A006;
{
; File Manager
}
_GetVolInfo = $A007;
_Create = $A008;
_Delete = $A009;
_OpenRF = $A00A;
_Rename = $A00B;
_GetFileInfo = $A00C;
_SetFileInfo = $A00D;
_UnmountVol = $A00E;
_HUnmountVol = $A20E;
_MountVol = $A00F;
_Allocate = $A010;
_GetEOF = $A011;
_SetEOF = $A012;
_FlushVol = $A013;
_GetVol = $A014;
_SetVol = $A015;
_FInitQueue = $A016;
_Eject = $A017;
_GetFPos = $A018;
_SetFilLock = $A041;
_RstFilLock = $A042;



_SetFilType = $A043;
_SetFPos = $A044;
_FlushFile = $A045;
_HOpen = $A200;
_HGetVInfo = $A207;
_HCreate = $A208;
_HDelete = $A209;
_HOpenRF = $A20A;
_HRename = $A20B;
_HGetFileInfo = $A20C;
_HSetFileInfo = $A20D;
_AllocContig = $A210;
_HSetVol = $A215;
_HGetVol = $A214;
_HSetFLock = $A241;
_HRstFLock = $A242;
{
; dispatch trap for remaining File Manager (and Desktop Manager) calls
}
_FSDispatch = $A060;
_HFSDispatch = $A260;
{
; High level FSSpec calls
}
_HighLevelFSDispatch = $AA52;
{
; Memory Manager
}
_InitZone = $A019;
_GetZone = $A11A;
_SetZone = $A01B;
_FreeMem = $A01C;
_MaxMem = $A11D;
_NewPtr = $A11E;
_NewPtrSys = $A51E;
_NewPtrClear = $A31E;
_NewPtrSysClear = $A71E;
_DisposPtr = $A01F;
_DisposePtr = $A01F;
_SetPtrSize = $A020;
_GetPtrSize = $A021;
_NewHandle = $A122;
_NewHandleClear = $A322;
_DisposHandle = $A023;
_DisposeHandle = $A023;
_SetHandleSize = $A024;
_GetHandleSize = $A025;
_HandleZone = $A126;
_ReallocHandle = $A027;
_RecoverHandle = $A128;



_HLock = $A029;
_HUnlock = $A02A;
_EmptyHandle = $A02B;
_InitApplZone = $A02C;
_SetApplLimit = $A02D;
_BlockMove = $A02E;
_MemoryDispatch = $A05C;
_MemoryDispatchA0Result = $A15C;
_DeferUserFn = $A08F;
_DebugUtil = $A08D;
{
; Event Manager
}
_PostEvent = $A02F;
_PPostEvent = $A12F;
_OSEventAvail = $A030;
_GetOSEvent = $A031;
_FlushEvents = $A032;
_VInstall = $A033;
_VRemove = $A034;
_OffLine = $A035;
_MoreMasters = $A036;
_WriteParam = $A038;
_ReadDateTime = $A039;
_SetDateTime = $A03A;
_Delay = $A03B;
_CmpString = $A03C;
_DrvrInstall = $A03D;
_DrvrRemove = $A03E;
_InitUtil = $A03F;
_ResrvMem = $A040;
_GetTrapAddress = $A146;
_SetTrapAddress = $A047;
_GetOSTrapAddress = $A346;
_SetOSTrapAddress = $A247;
_GetToolTrapAddress = $A746;
_SetToolTrapAddress = $A647;
_GetToolBoxTrapAddress = $A746;
_SetToolBoxTrapAddress = $A647;
_PtrZone = $A148;
_HPurge = $A049;
_HNoPurge = $A04A;
_SetGrowZone = $A04B;
_CompactMem = $A04C;
_PurgeMem = $A04D;
_AddDrive = $A04E;
_RDrvrInstall = $A04F;
_LwrString = $A056;
_UprString = $A054;
_SetApplBase = $A057;
_HWPriv = $A198;
{
; New names for (mostly) new flavors of old LwrString trap (redone <13>)

}
_LowerText = $A056;
_StripText = $A256;
_UpperText = $A456;
_StripUpperText = $A656;
{
; Temporary Memory routines
}
_OSDispatch = $A88F;
_RelString = $A050;
_ReadXPRam = $A051;
_WriteXPRam = $A052;
_InsTime = $A058;
_InsXTime = $A458;
_RmvTime = $A059;
_PrimeTime = $A05A;
_PowerOff = $A05B;
_MaxBlock = $A061;
_PurgeSpace = $A162;
_MaxApplZone = $A063;
_MoveHHi = $A064;
_StackSpace = $A065;
_NewEmptyHandle = $A166;
_HSetRBit = $A067;
_HClrRBit = $A068;
_HGetState = $A069;
_HSetState = $A06A;
_InitFS = $A06C;
_InitEvents = $A06D;
_StripAddress = $A055;
_Translate24To32 = $A091;
_SetAppBase = $A057;
_SwapMMUMode = $A05D;
_SlotVInstall = $A06F;
_SlotVRemove = $A070;
_AttachVBL = $A071;
_DoVBLTask = $A072;
_SIntInstall = $A075;
_SIntRemove = $A076;
_CountADBs = $A077;
_GetIndADB = $A078;
_GetADBInfo = $A079;
_SetADBInfo = $A07A;
_ADBReInit = $A07B;
_ADBOp = $A07C;
_GetDefaultStartup = $A07D;
_SetDefaultStartup = $A07E;
_InternalWait = $A07F;
_RGetResource = $A80C;
_GetVideoDefault = $A080;
_SetVideoDefault = $A081;
_DTInstall = $A082;
_SetOSDefault = $A083;
_GetOSDefault = $A084;
_IOPInfoAccess = $A086;
_IOPMsgRequest = $A087;
_IOPMoveData = $A088;
{
; Power Manager
}
_PMgrOp = $A085;
_IdleUpdate = $A285;
_IdleState = $A485;
_SerialPower = $A685;
_Sleep = $A08A;
_SleepQInstall = $A28A;
_SlpQInstall = $A28A;
_SleepQRemove = $A48A;
_SlpQRemove = $A48A;
{
; Comm. Toolbox
}
_CommToolboxDispatch = $A08B;
_SysEnvirons = $A090;
{
; Egret Manager
}
_EgretDispatch = $A092;
_Gestalt = $A1AD;
_NewGestalt = $A3AD;
_ReplaceGestalt = $A5AD;
_GetGestaltProcPtr = $A7AD;
_InitProcMenu = $A808;
_GetItemCmd = $A84E;
_SetItemCmd = $A84F;
_PopUpMenuSelect = $A80B;
_KeyTrans = $A9C3;
{
; TextEdit
}
_TEGetText = $A9CB;
_TEInit = $A9CC;
_TEDispose = $A9CD;
_TextBox = $A9CE;
_TESetText = $A9CF;
_TECalText = $A9D0;
_TESetSelect = $A9D1;
_TENew = $A9D2;
_TEUpdate = $A9D3;
_TEClick = $A9D4;
_TECopy = $A9D5;
_TECut = $A9D6;



_TEDelete = $A9D7;
_TEActivate = $A9D8;
_TEDeactivate = $A9D9;
_TEIdle = $A9DA;
_TEPaste = $A9DB;
_TEKey = $A9DC;
_TEScroll = $A9DD;
_TEInsert = $A9DE;
_TESetJust = $A9DF;
_TEGetOffset = $A83C;
_TEDispatch = $A83D;
_TEStyleNew = $A83E;
{
; Color Quickdraw
}
_OpenCPort = $AA00;
_InitCPort = $AA01;
_CloseCPort = $A87D;
_NewPixMap = $AA03;
_DisposPixMap = $AA04;
_DisposePixMap = $AA04;
_CopyPixMap = $AA05;
_SetPortPix = $AA06;
_NewPixPat = $AA07;
_DisposPixPat = $AA08;
_DisposePixPat = $AA08;
_CopyPixPat = $AA09;
_PenPixPat = $AA0A;
_BackPixPat = $AA0B;
_GetPixPat = $AA0C;
_MakeRGBPat = $AA0D;
_FillCRect = $AA0E;
_FillCOval = $AA0F;
_FillCRoundRect = $AA10;
_FillCArc = $AA11;
_FillCRgn = $AA12;
_FillCPoly = $AA13;
_RGBForeColor = $AA14;
_RGBBackColor = $AA15;
_SetCPixel = $AA16;
_GetCPixel = $AA17;
_GetCTable = $AA18;
_GetForeColor = $AA19;
_GetBackColor = $AA1A;
_GetCCursor = $AA1B;
_SetCCursor = $AA1C;
_AllocCursor = $AA1D;
_GetCIcon = $AA1E;
_PlotCIcon = $AA1F;
_OpenCPicture = $AA20;
_OpColor = $AA21;
_HiliteColor = $AA22;
_CharExtra = $AA23;
_DisposCTable = $AA24;



_DisposeCTable = $AA24;
_DisposCIcon = $AA25;
_DisposeCIcon = $AA25;
_DisposCCursor = $AA26;
_DisposeCCursor = $AA26;
_SeedCFill = $AA50;
_CalcCMask = $AA4F;
_CopyDeepMask = $AA51;
{
; Routines for video devices
}
_GetMaxDevice = $AA27;
_GetCTSeed = $AA28;
_GetDeviceList = $AA29;
_GetMainDevice = $AA2A;
_GetNextDevice = $AA2B;
_TestDeviceAttribute = $AA2C;
_SetDeviceAttribute = $AA2D;
_InitGDevice = $AA2E;
_NewGDevice = $AA2F;
_DisposGDevice = $AA30;
_DisposeGDevice = $AA30;
_SetGDevice = $AA31;
_GetGDevice = $AA32;
_DeviceLoop = $ABCA;
{
; Color Manager
}
_Color2Index = $AA33;
_Index2Color = $AA34;
_InvertColor = $AA35;
_RealColor = $AA36;
_GetSubTable = $AA37;
_UpdatePixMap = $AA38;
{
; Dialog Manager
}
_NewCDialog = $AA4B;
_MakeITable = $AA39;
_AddSearch = $AA3A;
_AddComp = $AA3B;
_SetClientID = $AA3C;
_ProtectEntry = $AA3D;
_ReserveEntry = $AA3E;
_SetEntries = $AA3F;
_QDError = $AA40;
_SaveEntries = $AA49;
_RestoreEntries = $AA4A;
_DelSearch = $AA4C;
_DelComp = $AA4D;



_SetStdCProcs = $AA4E;
_StdOpcodeProc = $ABF8;
{
; added to Toolbox for color
}
_SetWinColor = $AA41;
_GetAuxWin = $AA42;
_SetCtlColor = $AA43;
_GetAuxCtl = $AA44;
_NewCWindow = $AA45;
_GetNewCWindow = $AA46;
_SetDeskCPat = $AA47;
_GetCWMgrPort = $AA48;
_GetCVariant = $A809;
_GetWVariant = $A80A;
{
; added to Menu Manager for color
}
_DelMCEntries = $AA60;
_GetMCInfo = $AA61;
_SetMCInfo = $AA62;
_DispMCInfo = $AA63;
_GetMCEntry = $AA64;
_SetMCEntries = $AA65;
{
; Menu Manager
}
_MenuChoice = $AA66;
{
; Dialog Manager?
}
_ModalDialogMenuSetup = $AA67;
_DialogDispatch = $AA68;
{
; Font Manager
}
_SetFractEnable = $A814;
_FontDispatch = $A854;
{
; Palette Manager
}
_InitPalettes = $AA90;
_NewPalette = $AA91;
_GetNewPalette = $AA92;





_DisposePalette = $AA93;
_ActivatePalette = $AA94;
_SetPalette = $AA95;
_NSetPalette = $AA95;
_GetPalette = $AA96;
_PmForeColor = $AA97;
_PmBackColor = $AA98;
_AnimateEntry = $AA99;
_AnimatePalette = $AA9A;
_GetEntryColor = $AA9B;
_SetEntryColor = $AA9C;
_GetEntryUsage = $AA9D;
_SetEntryUsage = $AA9E;
_CTab2Palette = $AA9F;
_Palette2CTab = $AAA0;
_CopyPalette = $AAA1;
_PaletteDispatch = $AAA2;
{
; Sound Manager
}
_SoundDispatch = $A800;
_SndDisposeChannel = $A801;
_SndAddModifier = $A802;
_SndDoCommand = $A803;
_SndDoImmediate = $A804;
_SndPlay = $A805;
_SndControl = $A806;
_SndNewChannel = $A807;
_SlotManager = $A06E;
_ScriptUtil = $A8B5;
_SCSIDispatch = $A815;
_Long2Fix = $A83F;
_Fix2Long = $A840;
_Fix2Frac = $A841;
_Frac2Fix = $A842;
_Fix2X = $A843;
_X2Fix = $A844;
_Frac2X = $A845;
_X2Frac = $A846;
_FracCos = $A847;
_FracSin = $A848;
_FracSqrt = $A849;
_FracMul = $A84A;
_FracDiv = $A84B;
_FixDiv = $A84D;
_NMInstall = $A05E;
_NMRemove = $A05F;
{
; All QDOffscreen Routines go through one trap with a selector
}
_QDExtensions = $AB1D;


{
; UserDelay
}
_UserDelay = $A84C;
_InitDogCow = $A89F;
_EnableDogCow = $A89F;
_DisableDogCow = $A89F;
_Moof = $A89F;
_HFSPinaforeDispatch = $AA52;

{$ENDC} { UsingTraps }
{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Traps.p}





{#####################################################################}
{### FILE: Types.p}
{#####################################################################}

{
Created: Saturday, January 5, 1991 at 9:27 AM
Types.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1985-1991
All rights reserved.
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Types;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingTypes}
{$SETC UsingTypes := 1}
{$IFC UNDEFINED SystemSevenOrLater}
{$SETC SystemSevenOrLater := FALSE}
{$ENDC}
{$IFC UNDEFINED SystemSixOrLater}
{$SETC SystemSixOrLater := SystemSevenOrLater}
{$ENDC}

CONST
noErr = 0;

TYPE
Byte = 0..255;
SignedByte = - 128..127;
Ptr = ^SignedByte;
Handle = ^Ptr;

{All is well}

{ unsigned byte for fontmgr }
{ any byte in memory }
{

pointer to a master pointer }

{$IFC UNDEFINED qMacApp}
IntegerPtr = ^INTEGER;
LongIntPtr = ^LONGINT;
{$ENDC}
Fixed = LONGINT;

{ fixed point arithmetic type }


FixedPtr = ^Fixed;
Fract = LONGINT;
FractPtr = ^Fract;
{$IFC OPTION(MC68881)}
Extended80 = ARRAY [0..4] OF INTEGER;
{$ELSEC}
Extended80 = EXTENDED;
{$ENDC}
VHSelect = (v,h);

ProcPtr = Ptr;

{ pointer to a procedure }

StringPtr = ^Str255;
StringHandle = ^StringPtr;
Str255 = String[255];

{ maximum string size }

Str63 = String[63];
Str32 = String[32];
Str31 = String[31];
Str27 = String[27];
Str15 = String[15];

OSErr = INTEGER;
{ error code }
OSType = PACKED ARRAY [1..4] OF CHAR;
OSTypePtr = ^OSType;
ResType = PACKED ARRAY [1..4] OF CHAR;
ResTypePtr = ^ResType;
ScriptCode = INTEGER;
LangCode = INTEGER;

PointPtr = ^Point;
Point = RECORD
CASE INTEGER OF
1:
(v: INTEGER;
{vertical coordinate}
h: INTEGER);
{horizontal coordinate}
2:
(vh: ARRAY[VHSelect] OF INTEGER);
END;
RectPtr = ^Rect;
Rect = RECORD
    CASE INTEGER OF
    1:
        (top: INTEGER;
         left: INTEGER;
         bottom: INTEGER;
         right: INTEGER);
    2:
       (topLeft: Point;
        botRight: Point);
END;

PROCEDURE Debugger;
INLINE $A9FF;
PROCEDURE DebugStr(aStr: Str255);
INLINE $ABFF;
PROCEDURE SysBreak;
INLINE $303C,$FE16,$A9C9;
PROCEDURE SysBreakStr(debugStr: Str255);
INLINE $303C,$FE15,$A9C9;
PROCEDURE SysBreakFunc(debugFunc: Str255);
INLINE $303C,$FE14,$A9C9;

{$ENDC} { UsingTypes }
{$IFC NOT UsingIncludes}
END.
{$ENDC}


{#####################################################################}
{### FILE: Video.p}
{#####################################################################}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Video;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingVideo}
{$SETC UsingVideo := 1}
{$I+}
{$SETC VideoIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$SETC UsingIncludes := VideoIncludes}
CONST
mBaseOffset = 1;
mRowBytes = 2;
mBounds = 3;
mVersion = 4;
mHRes = 5;
mVRes = 6;
mPixelType = 7;
mPixelSize = 8;
mCmpCount = 9;
mCmpSize = 10;
mPlaneBytes = 11;
mVertRefRate = 14;
mVidParams = 1;
mTable = 2;
mPageCnt = 3;
mDevType = 4;

{Id of mBaseOffset.}
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video sResource parameter Id's
{Video parameter block id.}
{Offset to the table.}
{Number of pages}
{Device Type}

oneBitMode = 128;
twoBitMode = 129;
fourBitMode = 130;

{Id of OneBitMode Parameter list.}
{Id of TwoBitMode Parameter list.}
{Id of FourBitMode Parameter list.}

}
}
}
}
}
}
}
}
}
}
}



eightBitMode = 131;
sixteenBitMode = 132;
thirtyTwoBitMode = 133;

{Id of EightBitMode Parameter list.}
{Id of SixteenBitMode Parameter list.}
{Id of ThirtyTwoBitMode Parameter list.}

firstVidMode = 128;
secondVidMode = 129;
thirdVidMode = 130;
fourthVidMode = 131;
fifthVidMode = 132;
sixthVidMode = 133;

{The new, better way to do the above.
{
QuickDraw only supports six video
{
at this time.

}
}
}

spGammaDir = 64;
spVidNamesDir = 65;
{ Control Codes }
cscReset = 0;
cscKillIO = 1;
cscSetMode = 2;
cscSetEntries = 3;
cscSetGamma = 4;
cscGrayPage = 5;
cscGrayScreen = 5;
cscSetGray = 6;
cscSetInterrupt = 7;
cscDirectSetEntries = 8;
cscSetDefaultMode = 9;
{ Status Codes }
cscGetMode = 2;
cscGetEntries = 3;
cscGetPageCnt = 4;
cscGetPages = 4;
cscGetPageBase = 5;
cscGetBaseAddr = 5;
cscGetGray = 6;
cscGetInterrupt = 7;
cscGetGamma = 8;
cscGetDefaultMode = 9;
TYPE
VPBlockPtr = ^VPBlock;
VPBlock = RECORD
vpBaseOffset: LONGINT;
vpRowBytes: INTEGER;
vpBounds: Rect;
vpVersion: INTEGER;
vpPackType: INTEGER;
vpPackSize: LONGINT;
vpHRes: LONGINT;
vpVRes: LONGINT;
vpPixelType: INTEGER;
vpPixelSize: INTEGER;
vpCmpCount: INTEGER;
vpCmpSize: INTEGER;
vpPlaneBytes: LONGINT;
END;

{ This is what C&D 2 calls it. }
{ This is what C&D 2 calls it. }

{Offset to page zero of video RAM (From minorBaseOS).}
{Width of each row of video memory.}
{BoundsRect for the video display (gives dimensions).}
{PixelMap version number.}

{Horizontal resolution of the device (pixels per inch).}
{Vertical resolution of the device (pixels per inch).}
{Defines the pixel type.}
{Number of bits in pixel.}
{Number of components in pixel.}
{Number of bits per component}
{Offset from one plane to the next.}



VDEntRecPtr = ^VDEntryRecord;
VDEntryRecord = RECORD
csTable: Ptr;
{(long) pointer to color table entry=value, r,g,b:INTEGER}
END;
{ Parm block for SetGray control call }
VDGrayPtr = ^VDGrayRecord;
VDGrayRecord = RECORD
csMode: BOOLEAN;
{Same as GDDevType value (0=mono, 1=color)}
END;
{ Parm block for SetEntries control call }
VDSetEntryPtr = ^VDSetEntryRecord;
VDSetEntryRecord = RECORD
csTable: ^ColorSpec;
{Pointer to an array of color specs}
csStart: INTEGER;
{Which spec in array to start with, or -1}
csCount: INTEGER;
{Number of color spec entries to set}
END;
{ Parm block for SetGamma control call }
VDGamRecPtr = ^VDGammaRecord;
VDGammaRecord = RECORD
csGTable: Ptr;
{pointer to gamma table}
END;
VDPgInfoPtr = ^VDPageInfo;
VDPageInfo = RECORD
csMode: INTEGER;
csData: LONGINT;
csPage: INTEGER;
csBaseAddr: Ptr;
END;
VDSzInfoPtr = ^VDSizeInfo;
VDSizeInfo = RECORD
csHSize: INTEGER;
csHPos: INTEGER;
csVSize: INTEGER;
csVPos: INTEGER;
END;


VDSettingsPtr = ^VDSettings;
VDSettings = RECORD
csParamCnt: INTEGER;
csBrightMax: INTEGER;
csBrightDef: INTEGER;
csBrightVal: INTEGER;
csCntrstMax: INTEGER;
csCntrstDef: INTEGER;
csCntrstVal: INTEGER;
csTintMax: INTEGER;
csTintDef: INTEGER;
csTintVal: INTEGER;
csHueMax: INTEGER;
csHueDef: INTEGER;

csHueVal: INTEGER;
csHorizDef: INTEGER;
csHorizVal: INTEGER;
csHorizMax: INTEGER;
csVertDef: INTEGER;
csVertVal: INTEGER;
csVertMax: INTEGER;
END;

{$ENDC}

{ UsingVideo }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE Video.p}


{#####################################################################}
{### FILE: VideoIntf.p}
{#####################################################################}
{
File: VideoIntf.p
As of MPW 3.0, interface files were reorganized to more closely
match "Inside Macintosh" reference books and be more consistant
from language to language.
Interfaces for the VideoIntf are now found in Video.p.
This file, which includes Video.p, is provided for compatibility
with old sources.
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc. 1988
All Rights Reserved
}
{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT VideoIntf;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingVideoIntf}
{$SETC UsingVideoIntf := 1}
{$I+}
{$SETC VideoIntfIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingVideo}
{$I $$Shell(PInterfaces)Video.p}
{$ENDC}
{$SETC UsingIncludes := VideoIntfIncludes}
{$ENDC}

{ UsingVideoIntf }

{$IFC NOT UsingIncludes}
END.
{$ENDC}

{### END OF FILE VideoIntf.p}


{#####################################################################}
{### FILE: Windows.p}
{#####################################################################}
{
Created: Saturday, December 15, 1990 at 8:08 PM
Windows.p
Pascal Interface to the Macintosh Libraries
Copyright Apple Computer, Inc.
All rights reserved.
}

{$IFC UNDEFINED UsingIncludes}
{$SETC UsingIncludes := 0}
{$ENDC}
{$IFC NOT UsingIncludes}
UNIT Windows;
INTERFACE
{$ENDC}
{$IFC UNDEFINED UsingWindows}
{$SETC UsingWindows := 1}
{$I+}
{$SETC WindowsIncludes := UsingIncludes}
{$SETC UsingIncludes := 1}
{$IFC UNDEFINED UsingQuickdraw}
{$I $$Shell(PInterfaces)Quickdraw.p}
{$ENDC}
{$IFC UNDEFINED UsingEvents}
{$I $$Shell(PInterfaces)Events.p}
{$ENDC}
{$IFC UNDEFINED UsingControls}
{$I $$Shell(PInterfaces)Controls.p}
{$ENDC}
{$SETC UsingIncludes := WindowsIncludes}
CONST
documentProc = 0;
dBoxProc = 1;
plainDBox = 2;
altDBoxProc = 3;
noGrowDocProc = 4;
movableDBoxProc = 5;
zoomDocProc = 8;
zoomNoGrow = 12;
rDocProc = 16;
dialogKind = 2;
userKind = 8;
{FindWindow Result Codes}
inDesk = 0;


inMenuBar = 1;
inSysWindow = 2;
inContent = 3;
inDrag = 4;
inGrow = 5;
inGoAway = 6;
inZoomIn = 7;
inZoomOut = 8;
{window messages}
wDraw = 0;
wHit = 1;
wCalcRgns = 2;
wNew = 3;
wDispose = 4;
wGrow = 5;
wDrawGIcon = 6;
{defProc hit test codes}
wNoHit = 0;
wInContent = 1;
wInDrag = 2;
wInGrow = 3;
wInGoAway = 4;
wInZoomIn = 5;
wInZoomOut = 6;
deskPatID = 16;
{Window Part Identifiers which correlate color table entries with window elements}
wContentColor = 0;
wFrameColor = 1;
wTextColor = 2;
wHiliteColor = 3;
wTitleBarColor = 4;
TYPE
WindowPeek = ^WindowRecord;
WindowRecord = RECORD
port: GrafPort;
windowKind: INTEGER;
visible: BOOLEAN;
hilited: BOOLEAN;
goAwayFlag: BOOLEAN;
spareFlag: BOOLEAN;
strucRgn: RgnHandle;
contRgn: RgnHandle;
updateRgn: RgnHandle;
windowDefProc: Handle;
dataHandle: Handle;
titleHandle: StringHandle;
titleWidth: INTEGER;
controlList: ControlHandle;
nextWindow: WindowPeek;
windowPic: PicHandle;
refCon: LONGINT;
END;



CWindowPeek = ^CWindowRecord;
CWindowRecord = RECORD
port: CGrafPort;
windowKind: INTEGER;
visible: BOOLEAN;
hilited: BOOLEAN;
goAwayFlag: BOOLEAN;
spareFlag: BOOLEAN;
strucRgn: RgnHandle;
contRgn: RgnHandle;
updateRgn: RgnHandle;
windowDefProc: Handle;
dataHandle: Handle;
titleHandle: StringHandle;
titleWidth: INTEGER;
controlList: ControlHandle;
nextWindow: CWindowPeek;
windowPic: PicHandle;
refCon: LONGINT;
END;
WStateDataPtr = ^WStateData;
WStateDataHandle = ^WStateDataPtr;
WStateData = RECORD
userState: Rect;
{user state}
stdState: Rect;
{standard state}
END;
AuxWinPtr = ^AuxWinRec;
AuxWinHandle = ^AuxWinPtr;
AuxWinRec = RECORD
awNext: AuxWinHandle;
awOwner: WindowPtr;
awCTable: CTabHandle;
dialogCItem: Handle;
awFlags: LONGINT;
awReserved: CTabHandle;
awRefCon: LONGINT;
END;

{handle to next AuxWinRec}
{ptr to window }
{color table for this window}
{handle to dialog manager structures}
{reserved for expansion}
{reserved for expansion}
{user Constant}

WCTabPtr = ^WinCTab;
WCTabHandle = ^WCTabPtr;
WinCTab = RECORD
wCSeed: LONGINT;
{reserved}
wCReserved: INTEGER;
{reserved}
ctSize: INTEGER;
{usually 4 for windows}
ctTable: ARRAY [0..4] OF ColorSpec;
END;

PROCEDURE InitWindows;
INLINE $A912;
PROCEDURE GetWMgrPort(VAR wPort: GrafPtr);
INLINE $A910;
FUNCTION NewWindow(wStorage: Ptr;boundsRect: Rect;title: Str255;visible: BOOLEAN;
theProc: INTEGER;behind: WindowPtr;goAwayFlag: BOOLEAN;refCon: LONGINT): WindowPtr;
INLINE $A913;
FUNCTION GetNewWindow(windowID: INTEGER;wStorage: Ptr;behind: WindowPtr): WindowPtr;
INLINE $A9BD;
PROCEDURE CloseWindow(theWindow: WindowPtr);
INLINE $A92D;
PROCEDURE DisposeWindow(theWindow: WindowPtr);
INLINE $A914;
PROCEDURE GetWTitle(theWindow: WindowPtr;VAR title: Str255);
INLINE $A919;
PROCEDURE SelectWindow(theWindow: WindowPtr);
INLINE $A91F;
PROCEDURE HideWindow(theWindow: WindowPtr);
INLINE $A916;
PROCEDURE ShowWindow(theWindow: WindowPtr);
INLINE $A915;
PROCEDURE ShowHide(theWindow: WindowPtr;showFlag: BOOLEAN);
INLINE $A908;
PROCEDURE HiliteWindow(theWindow: WindowPtr;fHilite: BOOLEAN);
INLINE $A91C;
PROCEDURE BringToFront(theWindow: WindowPtr);
INLINE $A920;
PROCEDURE SendBehind(theWindow: WindowPtr;behindWindow: WindowPtr);
INLINE $A921;
FUNCTION FrontWindow: WindowPtr;
INLINE $A924;
PROCEDURE DrawGrowIcon(theWindow: WindowPtr);
INLINE $A904;
PROCEDURE MoveWindow(theWindow: WindowPtr;hGlobal: INTEGER;vGlobal: INTEGER;
front: BOOLEAN);
INLINE $A91B;
PROCEDURE SizeWindow(theWindow: WindowPtr;w: INTEGER;h: INTEGER;fUpdate: BOOLEAN);
INLINE $A91D;
PROCEDURE ZoomWindow(theWindow: WindowPtr;partCode: INTEGER;front: BOOLEAN);
INLINE $A83A;
PROCEDURE InvalRect(badRect: Rect);
INLINE $A928;
PROCEDURE InvalRgn(badRgn: RgnHandle);
INLINE $A927;
PROCEDURE ValidRect(goodRect: Rect);
INLINE $A92A;
PROCEDURE ValidRgn(goodRgn: RgnHandle);
INLINE $A929;
PROCEDURE BeginUpdate(theWindow: WindowPtr);
INLINE $A922;
PROCEDURE EndUpdate(theWindow: WindowPtr);
INLINE $A923;
PROCEDURE SetWRefCon(theWindow: WindowPtr;data: LONGINT);
INLINE $A918;
FUNCTION GetWRefCon(theWindow: WindowPtr): LONGINT;
INLINE $A917;
PROCEDURE SetWindowPic(theWindow: WindowPtr;pic: PicHandle);
INLINE $A92E;
FUNCTION GetWindowPic(theWindow: WindowPtr): PicHandle;
INLINE $A92F;
FUNCTION CheckUpdate(VAR theEvent: EventRecord): BOOLEAN;



INLINE $A911;
PROCEDURE ClipAbove(window: WindowPeek);
INLINE $A90B;
PROCEDURE SaveOld(window: WindowPeek);
INLINE $A90E;
PROCEDURE DrawNew(window: WindowPeek;update: BOOLEAN);
INLINE $A90F;
PROCEDURE PaintOne(window: WindowPeek;clobberedRgn: RgnHandle);
INLINE $A90C;
PROCEDURE PaintBehind(startWindow: WindowPeek;clobberedRgn: RgnHandle);
INLINE $A90D;
PROCEDURE CalcVis(window: WindowPeek);
INLINE $A909;
PROCEDURE CalcVisBehind(startWindow: WindowPeek;clobberedRgn: RgnHandle);
INLINE $A90A;
FUNCTION GrowWindow(theWindow: WindowPtr;startPt: Point;bBox: Rect): LONGINT;
INLINE $A92B;
FUNCTION FindWindow(thePoint: Point;VAR theWindow: WindowPtr): INTEGER;
INLINE $A92C;
FUNCTION PinRect(theRect: Rect;thePt: Point): LONGINT;
INLINE $A94E;
FUNCTION DragGrayRgn(theRgn: RgnHandle;startPt: Point;boundsRect: Rect;
slopRect: Rect;axis: INTEGER;actionProc: ProcPtr): LONGINT;
INLINE $A905;
FUNCTION TrackBox(theWindow: WindowPtr;thePt: Point;partCode: INTEGER): BOOLEAN;
INLINE $A83B;
PROCEDURE GetCWMgrPort(VAR wMgrCPort: CGrafPtr);
INLINE $AA48;
PROCEDURE SetWinColor(theWindow: WindowPtr;newColorTable: WCTabHandle);
INLINE $AA41;
FUNCTION GetAuxWin(theWindow: WindowPtr;VAR awHndl: AuxWinHandle): BOOLEAN;
INLINE $AA42;
PROCEDURE SetDeskCPat(deskPixPat: PixPatHandle);
INLINE $AA47;
FUNCTION NewCWindow(wStorage: Ptr;boundsRect: Rect;title: Str255;visible: BOOLEAN;
procID: INTEGER;behind: WindowPtr;goAwayFlag: BOOLEAN;refCon: LONGINT): WindowPtr;
INLINE $AA45;
FUNCTION GetNewCWindow(windowID: INTEGER;wStorage: Ptr;behind: WindowPtr): WindowPtr;
INLINE $AA46;
FUNCTION GetWVariant(theWindow: WindowPtr): INTEGER;
INLINE $A80A;
FUNCTION GetGrayRgn: RgnHandle;
INLINE $2EB8,$09EE;
PROCEDURE SetWTitle(theWindow: WindowPtr;title: Str255);
INLINE $A91A;
FUNCTION TrackGoAway(theWindow: WindowPtr;thePt: Point): BOOLEAN;
INLINE $A91E;
PROCEDURE DragWindow(theWindow: WindowPtr;startPt: Point;boundsRect: Rect);
INLINE $A925;

{$ENDC}

{ UsingWindows }

{$IFC NOT UsingIncludes}
END.
{$ENDC}


{### END OF FILE Windows.p}
