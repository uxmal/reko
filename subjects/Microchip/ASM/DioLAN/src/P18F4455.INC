        LIST

;==========================================================================
;  $Id: P18F4455.INC,v 1.9 2006/10/25 17:47:56 nairnj Exp $
;  MPASM PIC18F4455 processor include
; 
;  (c) Copyright 1999-2006 Microchip Technology, All rights reserved
;==========================================================================

        NOLIST

;==========================================================================
;  This header file defines configurations, registers, and other useful
;  bits of information for the PIC18F4455 microcontroller.  These names
;  are taken to match the data sheets as closely as possible.
;
;  Note that the processor must be selected before this file is included.
;  The processor may be selected the following ways:
;
;       1. Command line switch:
;               C:\MPASM MYFILE.ASM /PIC18F4455
;       2. LIST directive in the source file
;               LIST   P=PIC18F4455
;       3. Processor Type entry in the MPASM full-screen interface
;       4. Setting the processor in the MPLAB Project Dialog
;==========================================================================

;==========================================================================
;
;       Verify Processor
;
;==========================================================================
        IFNDEF __18F4455
           MESSG "Processor-header file mismatch.  Verify selected processor."
        ENDIF

;==========================================================================
;       18xxxx Family        EQUates
;==========================================================================
FSR0            EQU 0
FSR1            EQU 1
FSR2            EQU 2

FAST            EQU 1

W               EQU 0
A               EQU 0
ACCESS          EQU 0
BANKED          EQU 1
;==========================================================================

;==========================================================================
;       16Cxxx/17Cxxx Substitutions
;==========================================================================
  #define DDRA  TRISA      ; PIC17Cxxx SFR substitution
  #define DDRB  TRISB      ; PIC17Cxxx SFR substitution
  #define DDRC  TRISC      ; PIC17Cxxx SFR substitution
  #define DDRD  TRISD      ; PIC17Cxxx SFR substitution
  #define DDRE  TRISE      ; PIC17Cxxx SFR substitution

;==========================================================================
;
;       Register Definitions
;
;==========================================================================

;----- Register Files -----------------------------------------------------
SPPDATA          EQU  H'0F62'
SPPCFG           EQU  H'0F63'
SPPEPS           EQU  H'0F64'
SPPCON           EQU  H'0F65'
UFRM             EQU  H'0F66'
UFRML            EQU  H'0F66'
UFRMH            EQU  H'0F67'
UIR              EQU  H'0F68'
UIE              EQU  H'0F69'
UEIR             EQU  H'0F6A'
UEIE             EQU  H'0F6B'
USTAT            EQU  H'0F6C'
UCON             EQU  H'0F6D'
UADDR            EQU  H'0F6E'
UCFG             EQU  H'0F6F'
UEP0             EQU  H'0F70'
UEP1             EQU  H'0F71'
UEP2             EQU  H'0F72'
UEP3             EQU  H'0F73'
UEP4             EQU  H'0F74'
UEP5             EQU  H'0F75'
UEP6             EQU  H'0F76'
UEP7             EQU  H'0F77'
UEP8             EQU  H'0F78'
UEP9             EQU  H'0F79'
UEP10            EQU  H'0F7A'
UEP11            EQU  H'0F7B'
UEP12            EQU  H'0F7C'
UEP13            EQU  H'0F7D'
UEP14            EQU  H'0F7E'
UEP15            EQU  H'0F7F'
PORTA            EQU  H'0F80'
PORTB            EQU  H'0F81'
PORTC            EQU  H'0F82'
PORTD            EQU  H'0F83'
PORTE            EQU  H'0F84'
LATA             EQU  H'0F89'
LATB             EQU  H'0F8A'
LATC             EQU  H'0F8B'
LATD             EQU  H'0F8C'
LATE             EQU  H'0F8D'
DDRA             EQU  H'0F92'
TRISA            EQU  H'0F92'
DDRB             EQU  H'0F93'
TRISB            EQU  H'0F93'
DDRC             EQU  H'0F94'
TRISC            EQU  H'0F94'
DDRD             EQU  H'0F95'
TRISD            EQU  H'0F95'
DDRE             EQU  H'0F96'
TRISE            EQU  H'0F96'
OSCTUNE          EQU  H'0F9B'
PIE1             EQU  H'0F9D'
PIR1             EQU  H'0F9E'
IPR1             EQU  H'0F9F'
PIE2             EQU  H'0FA0'
PIR2             EQU  H'0FA1'
IPR2             EQU  H'0FA2'
EECON1           EQU  H'0FA6'
EECON2           EQU  H'0FA7'
EEDATA           EQU  H'0FA8'
EEADR            EQU  H'0FA9'
RCSTA            EQU  H'0FAB'
TXSTA            EQU  H'0FAC'
TXREG            EQU  H'0FAD'
RCREG            EQU  H'0FAE'
SPBRG            EQU  H'0FAF'
SPBRGH           EQU  H'0FB0'
T3CON            EQU  H'0FB1'
TMR3L            EQU  H'0FB2'
TMR3H            EQU  H'0FB3'
CMCON            EQU  H'0FB4'
CVRCON           EQU  H'0FB5'
CCP1AS           EQU  H'0FB6'
ECCP1AS          EQU  H'0FB6'
CCP1DEL          EQU  H'0FB7'
ECCP1DEL         EQU  H'0FB7'
BAUDCON          EQU  H'0FB8'
CCP2CON          EQU  H'0FBA'
CCPR2            EQU  H'0FBB'
CCPR2L           EQU  H'0FBB'
CCPR2H           EQU  H'0FBC'
CCP1CON          EQU  H'0FBD'
ECCP1CON         EQU  H'0FBD'
CCPR1            EQU  H'0FBE'
CCPR1L           EQU  H'0FBE'
CCPR1H           EQU  H'0FBF'
ADCON2           EQU  H'0FC0'
ADCON1           EQU  H'0FC1'
ADCON0           EQU  H'0FC2'
ADRES            EQU  H'0FC3'
ADRESL           EQU  H'0FC3'
ADRESH           EQU  H'0FC4'
SSPCON2          EQU  H'0FC5'
SSPCON1          EQU  H'0FC6'
SSPSTAT          EQU  H'0FC7'
SSPADD           EQU  H'0FC8'
SSPBUF           EQU  H'0FC9'
T2CON            EQU  H'0FCA'
PR2              EQU  H'0FCB'
TMR2             EQU  H'0FCC'
T1CON            EQU  H'0FCD'
TMR1L            EQU  H'0FCE'
TMR1H            EQU  H'0FCF'
RCON             EQU  H'0FD0'
WDTCON           EQU  H'0FD1'
HLVDCON          EQU  H'0FD2'
LVDCON           EQU  H'0FD2'
OSCCON           EQU  H'0FD3'
T0CON            EQU  H'0FD5'
TMR0L            EQU  H'0FD6'
TMR0H            EQU  H'0FD7'
STATUS           EQU  H'0FD8'
FSR2L            EQU  H'0FD9'
FSR2H            EQU  H'0FDA'
PLUSW2           EQU  H'0FDB'
PREINC2          EQU  H'0FDC'
POSTDEC2         EQU  H'0FDD'
POSTINC2         EQU  H'0FDE'
INDF2            EQU  H'0FDF'
BSR              EQU  H'0FE0'
FSR1L            EQU  H'0FE1'
FSR1H            EQU  H'0FE2'
PLUSW1           EQU  H'0FE3'
PREINC1          EQU  H'0FE4'
POSTDEC1         EQU  H'0FE5'
POSTINC1         EQU  H'0FE6'
INDF1            EQU  H'0FE7'
WREG             EQU  H'0FE8'
FSR0L            EQU  H'0FE9'
FSR0H            EQU  H'0FEA'
PLUSW0           EQU  H'0FEB'
PREINC0          EQU  H'0FEC'
POSTDEC0         EQU  H'0FED'
POSTINC0         EQU  H'0FEE'
INDF0            EQU  H'0FEF'
INTCON3          EQU  H'0FF0'
INTCON2          EQU  H'0FF1'
INTCON           EQU  H'0FF2'
PROD             EQU  H'0FF3'
PRODL            EQU  H'0FF3'
PRODH            EQU  H'0FF4'
TABLAT           EQU  H'0FF5'
TBLPTR           EQU  H'0FF6'
TBLPTRL          EQU  H'0FF6'
TBLPTRH          EQU  H'0FF7'
TBLPTRU          EQU  H'0FF8'
PC               EQU  H'0FF9'
PCL              EQU  H'0FF9'
PCLATH           EQU  H'0FFA'
PCLATU           EQU  H'0FFB'
STKPTR           EQU  H'0FFC'
TOS              EQU  H'0FFD'
TOSL             EQU  H'0FFD'
TOSH             EQU  H'0FFE'
TOSU             EQU  H'0FFF'

;----- SPPCFG Bits -----------------------------------------------------
WS0              EQU  H'0000'
WS1              EQU  H'0001'
WS2              EQU  H'0002'
WS3              EQU  H'0003'
CLK1EN           EQU  H'0004'
CSEN             EQU  H'0005'
CLKCFG0          EQU  H'0006'
CLKCFG1          EQU  H'0007'


;----- SPPEPS Bits -----------------------------------------------------
ADDR0            EQU  H'0000'
ADDR1            EQU  H'0001'
ADDR2            EQU  H'0002'
ADDR3            EQU  H'0003'
SPPBUSY          EQU  H'0004'
WRSPP            EQU  H'0006'
RDSPP            EQU  H'0007'


;----- SPPCON Bits -----------------------------------------------------
SPPEN            EQU  H'0000'
SPPOWN           EQU  H'0001'


;----- UFRML Bits -----------------------------------------------------
FRM0             EQU  H'0000'
FRM1             EQU  H'0001'
FRM2             EQU  H'0002'
FRM3             EQU  H'0003'
FRM4             EQU  H'0004'
FRM5             EQU  H'0005'
FRM6             EQU  H'0006'
FRM7             EQU  H'0007'


;----- UFRMH Bits -----------------------------------------------------
FRM8             EQU  H'0000'
FRM9             EQU  H'0001'
FRM10            EQU  H'0002'


;----- UIR Bits -----------------------------------------------------
URSTIF           EQU  H'0000'
UERRIF           EQU  H'0001'
ACTVIF           EQU  H'0002'
TRNIF            EQU  H'0003'
IDLEIF           EQU  H'0004'
STALLIF          EQU  H'0005'
SOFIF            EQU  H'0006'


;----- UIE Bits -----------------------------------------------------
URSTIE           EQU  H'0000'
UERRIE           EQU  H'0001'
ACTVIE           EQU  H'0002'
TRNIE            EQU  H'0003'
IDLEIE           EQU  H'0004'
STALLIE          EQU  H'0005'
SOFIE            EQU  H'0006'


;----- UEIR Bits -----------------------------------------------------
PIDEF            EQU  H'0000'
CRC5EF           EQU  H'0001'
CRC16EF          EQU  H'0002'
DFN8EF           EQU  H'0003'
BTOEF            EQU  H'0004'
BTSEF            EQU  H'0007'


;----- UEIE Bits -----------------------------------------------------
PIDEE            EQU  H'0000'
CRC5EE           EQU  H'0001'
CRC16EE          EQU  H'0002'
DFN8EE           EQU  H'0003'
BTOEE            EQU  H'0004'
BTSEE            EQU  H'0007'


;----- USTAT Bits -----------------------------------------------------
PPBI             EQU  H'0001'
DIR              EQU  H'0002'
ENDP0            EQU  H'0003'
ENDP1            EQU  H'0004'
ENDP2            EQU  H'0005'
ENDP3            EQU  H'0006'


;----- UCON Bits -----------------------------------------------------
SUSPND           EQU  H'0001'
RESUME           EQU  H'0002'
USBEN            EQU  H'0003'
PKTDIS           EQU  H'0004'
SE0              EQU  H'0005'
PPBRST           EQU  H'0006'


;----- UADDR Bits -----------------------------------------------------
ADDR0            EQU  H'0000'
ADDR1            EQU  H'0001'
ADDR2            EQU  H'0002'
ADDR3            EQU  H'0003'
ADDR4            EQU  H'0004'
ADDR5            EQU  H'0005'
ADDR6            EQU  H'0006'


;----- UCFG Bits -----------------------------------------------------
PPB0             EQU  H'0000'
PPB1             EQU  H'0001'
FSEN             EQU  H'0002'
UTRDIS           EQU  H'0003'
UPUEN            EQU  H'0004'
UOEMON           EQU  H'0006'
UTEYE            EQU  H'0007'


;----- UEP0 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP1 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP2 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP3 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP4 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP5 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP6 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP7 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP8 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP9 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP10 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP11 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP12 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP13 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP14 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- UEP15 Bits -----------------------------------------------------
EPSTALL          EQU  H'0000'
EPINEN           EQU  H'0001'
EPOUTEN          EQU  H'0002'
EPCONDIS         EQU  H'0003'
EPHSHK           EQU  H'0004'


;----- PORTA Bits -----------------------------------------------------
RA0              EQU  H'0000'
RA1              EQU  H'0001'
RA2              EQU  H'0002'
RA3              EQU  H'0003'
RA4              EQU  H'0004'
RA5              EQU  H'0005'
RA6              EQU  H'0006'

AN0              EQU  H'0000'
AN1              EQU  H'0001'
AN2              EQU  H'0002'
AN3              EQU  H'0003'
T0CKI            EQU  H'0004'
AN4              EQU  H'0005'
OSC2             EQU  H'0006'

VREFM            EQU  H'0002'
VREFP            EQU  H'0003'
LVDIN            EQU  H'0005'

HLVDIN           EQU  H'0005'


;----- PORTB Bits -----------------------------------------------------
RB0              EQU  H'0000'
RB1              EQU  H'0001'
RB2              EQU  H'0002'
RB3              EQU  H'0003'
RB4              EQU  H'0004'
RB5              EQU  H'0005'
RB6              EQU  H'0006'
RB7              EQU  H'0007'

INT0             EQU  H'0000'
INT1             EQU  H'0001'
INT2             EQU  H'0002'

PGM              EQU  H'0005'
PGC              EQU  H'0006'
PGD              EQU  H'0007'


;----- PORTC Bits -----------------------------------------------------
RC0              EQU  H'0000'
RC1              EQU  H'0001'
RC2              EQU  H'0002'
RC4              EQU  H'0004'
RC5              EQU  H'0005'
RC6              EQU  H'0006'
RC7              EQU  H'0007'

T1OSO            EQU  H'0000'
T1OSI            EQU  H'0001'
CCP1             EQU  H'0002'
TX               EQU  H'0006'
RX               EQU  H'0007'

T13CKI           EQU  H'0000'
P1A              EQU  H'0002'
CK               EQU  H'0006'
; DT is a reserved word
; DT               EQU  H'0007'


;----- PORTD Bits -----------------------------------------------------
RD0              EQU  H'0000'
RD1              EQU  H'0001'
RD2              EQU  H'0002'
RD3              EQU  H'0003'
RD4              EQU  H'0004'
RD5              EQU  H'0005'
RD6              EQU  H'0006'
RD7              EQU  H'0007'

SPP0             EQU  H'0000'
SPP1             EQU  H'0001'
SPP2             EQU  H'0002'
SPP3             EQU  H'0003'
SPP4             EQU  H'0004'
SPP5             EQU  H'0005'
SPP6             EQU  H'0006'
SPP7             EQU  H'0007'


;----- PORTE Bits -----------------------------------------------------
RE0              EQU  H'0000'
RE1              EQU  H'0001'
RE2              EQU  H'0002'
RE3              EQU  H'0003'
RDPU             EQU  H'0007'

CK1SPP           EQU  H'0000'
CK2SPP           EQU  H'0001'
OESPP            EQU  H'0002'


;----- LATA Bits -----------------------------------------------------
LATA0            EQU  H'0000'
LATA1            EQU  H'0001'
LATA2            EQU  H'0002'
LATA3            EQU  H'0003'
LATA4            EQU  H'0004'
LATA5            EQU  H'0005'
LATA6            EQU  H'0006'


;----- LATB Bits -----------------------------------------------------
LATB0            EQU  H'0000'
LATB1            EQU  H'0001'
LATB2            EQU  H'0002'
LATB3            EQU  H'0003'
LATB4            EQU  H'0004'
LATB5            EQU  H'0005'
LATB6            EQU  H'0006'
LATB7            EQU  H'0007'


;----- LATC Bits -----------------------------------------------------
LATC0            EQU  H'0000'
LATC1            EQU  H'0001'
LATC2            EQU  H'0002'
LATC6            EQU  H'0006'
LATC7            EQU  H'0007'


;----- LATD Bits -----------------------------------------------------
LATD0            EQU  H'0000'
LATD1            EQU  H'0001'
LATD2            EQU  H'0002'
LATD3            EQU  H'0003'
LATD4            EQU  H'0004'
LATD5            EQU  H'0005'
LATD6            EQU  H'0006'
LATD7            EQU  H'0007'


;----- LATE Bits -----------------------------------------------------
LATE0            EQU  H'0000'
LATE1            EQU  H'0001'
LATE2            EQU  H'0002'


;----- DDRA Bits -----------------------------------------------------
RA0              EQU  H'0000'
RA1              EQU  H'0001'
RA2              EQU  H'0002'
RA3              EQU  H'0003'
RA4              EQU  H'0004'
RA5              EQU  H'0005'
RA6              EQU  H'0006'


;----- TRISA Bits -----------------------------------------------------
TRISA0           EQU  H'0000'
TRISA1           EQU  H'0001'
TRISA2           EQU  H'0002'
TRISA3           EQU  H'0003'
TRISA4           EQU  H'0004'
TRISA5           EQU  H'0005'
TRISA6           EQU  H'0006'


;----- DDRB Bits -----------------------------------------------------
RB0              EQU  H'0000'
RB1              EQU  H'0001'
RB2              EQU  H'0002'
RB3              EQU  H'0003'
RB4              EQU  H'0004'
RB5              EQU  H'0005'
RB6              EQU  H'0006'
RB7              EQU  H'0007'


;----- TRISB Bits -----------------------------------------------------
TRISB0           EQU  H'0000'
TRISB1           EQU  H'0001'
TRISB2           EQU  H'0002'
TRISB3           EQU  H'0003'
TRISB4           EQU  H'0004'
TRISB5           EQU  H'0005'
TRISB6           EQU  H'0006'
TRISB7           EQU  H'0007'


;----- DDRC Bits -----------------------------------------------------
RC0              EQU  H'0000'
RC1              EQU  H'0001'
RC2              EQU  H'0002'
RC6              EQU  H'0006'
RC7              EQU  H'0007'


;----- TRISC Bits -----------------------------------------------------
TRISC0           EQU  H'0000'
TRISC1           EQU  H'0001'
TRISC2           EQU  H'0002'
TRISC6           EQU  H'0006'
TRISC7           EQU  H'0007'


;----- DDRD Bits -----------------------------------------------------
RD0              EQU  H'0000'
RD1              EQU  H'0001'
RD2              EQU  H'0002'
RD3              EQU  H'0003'
RD4              EQU  H'0004'
RD5              EQU  H'0005'
RD6              EQU  H'0006'
RD7              EQU  H'0007'


;----- TRISD Bits -----------------------------------------------------
TRISD0           EQU  H'0000'
TRISD1           EQU  H'0001'
TRISD2           EQU  H'0002'
TRISD3           EQU  H'0003'
TRISD4           EQU  H'0004'
TRISD5           EQU  H'0005'
TRISD6           EQU  H'0006'
TRISD7           EQU  H'0007'


;----- DDRE Bits -----------------------------------------------------
RE0              EQU  H'0000'
RE1              EQU  H'0001'
RE2              EQU  H'0002'


;----- TRISE Bits -----------------------------------------------------
TRISE0           EQU  H'0000'
TRISE1           EQU  H'0001'
TRISE2           EQU  H'0002'


;----- OSCTUNE Bits -----------------------------------------------------
TUN0             EQU  H'0000'
TUN1             EQU  H'0001'
TUN2             EQU  H'0002'
TUN3             EQU  H'0003'
TUN4             EQU  H'0004'
INTSRC           EQU  H'0007'


;----- PIE1 Bits -----------------------------------------------------
TMR1IE           EQU  H'0000'
TMR2IE           EQU  H'0001'
CCP1IE           EQU  H'0002'
SSPIE            EQU  H'0003'
TXIE             EQU  H'0004'
RCIE             EQU  H'0005'
ADIE             EQU  H'0006'
SPPIE            EQU  H'0007'


;----- PIR1 Bits -----------------------------------------------------
TMR1IF           EQU  H'0000'
TMR2IF           EQU  H'0001'
CCP1IF           EQU  H'0002'
SSPIF            EQU  H'0003'
TXIF             EQU  H'0004'
RCIF             EQU  H'0005'
ADIF             EQU  H'0006'
SPPIF            EQU  H'0007'


;----- IPR1 Bits -----------------------------------------------------
TMR1IP           EQU  H'0000'
TMR2IP           EQU  H'0001'
CCP1IP           EQU  H'0002'
SSPIP            EQU  H'0003'
TXIP             EQU  H'0004'
RCIP             EQU  H'0005'
ADIP             EQU  H'0006'
SPPIP            EQU  H'0007'


;----- PIE2 Bits -----------------------------------------------------
CCP2IE           EQU  H'0000'
TMR3IE           EQU  H'0001'
LVDIE            EQU  H'0002'
BCLIE            EQU  H'0003'
EEIE             EQU  H'0004'
USBIE            EQU  H'0005'
CMIE             EQU  H'0006'
OSCFIE           EQU  H'0007'

HLVDIE           EQU  H'0002'


;----- PIR2 Bits -----------------------------------------------------
CCP2IF           EQU  H'0000'
TMR3IF           EQU  H'0001'
LVDIF            EQU  H'0002'
BCLIF            EQU  H'0003'
EEIF             EQU  H'0004'
USBIF            EQU  H'0005'
CMIF             EQU  H'0006'
OSCFIF           EQU  H'0007'

HLVDIF           EQU  H'0002'


;----- IPR2 Bits -----------------------------------------------------
CCP2IP           EQU  H'0000'
TMR3IP           EQU  H'0001'
LVDIP            EQU  H'0002'
BCLIP            EQU  H'0003'
EEIP             EQU  H'0004'
USBIP            EQU  H'0005'
CMIP             EQU  H'0006'
OSCFIP           EQU  H'0007'

HLVDIP           EQU  H'0002'


;----- EECON1 Bits -----------------------------------------------------
RD               EQU  H'0000'
WR               EQU  H'0001'
WREN             EQU  H'0002'
WRERR            EQU  H'0003'
FREE             EQU  H'0004'
CFGS             EQU  H'0006'
EEPGD            EQU  H'0007'


;----- RCSTA Bits -----------------------------------------------------
RX9D             EQU  H'0000'
OERR             EQU  H'0001'
FERR             EQU  H'0002'
ADDEN            EQU  H'0003'
CREN             EQU  H'0004'
SREN             EQU  H'0005'
RX9              EQU  H'0006'
SPEN             EQU  H'0007'

ADEN             EQU  H'0003'


;----- TXSTA Bits -----------------------------------------------------
TX9D             EQU  H'0000'
TRMT             EQU  H'0001'
BRGH             EQU  H'0002'
SENDB            EQU  H'0003'
SYNC             EQU  H'0004'
TXEN             EQU  H'0005'
TX9              EQU  H'0006'
CSRC             EQU  H'0007'


;----- T3CON Bits -----------------------------------------------------
TMR3ON           EQU  H'0000'
TMR3CS           EQU  H'0001'
T3SYNC           EQU  H'0002'
T3CCP1           EQU  H'0003'
T3CKPS0          EQU  H'0004'
T3CKPS1          EQU  H'0005'
T3CCP2           EQU  H'0006'
RD16             EQU  H'0007'

T3NSYNC          EQU  H'0002'

NOT_T3SYNC       EQU  H'0002'


;----- CMCON Bits -----------------------------------------------------
CM0              EQU  H'0000'
CM1              EQU  H'0001'
CM2              EQU  H'0002'
CIS              EQU  H'0003'
C1INV            EQU  H'0004'
C2INV            EQU  H'0005'
C1OUT            EQU  H'0006'
C2OUT            EQU  H'0007'


;----- CVRCON Bits -----------------------------------------------------
CVR0             EQU  H'0000'
CVR1             EQU  H'0001'
CVR2             EQU  H'0002'
CVR3             EQU  H'0003'
CVREF            EQU  H'0004'
CVRR             EQU  H'0005'
CVROE            EQU  H'0006'
CVREN            EQU  H'0007'

CVRSS            EQU  H'0004'


;----- CCP1AS Bits -----------------------------------------------------
PSSBD0           EQU  H'0000'
PSSBD1           EQU  H'0001'
PSSAC0           EQU  H'0002'
PSSAC1           EQU  H'0003'
ECCPAS0          EQU  H'0004'
ECCPAS1          EQU  H'0005'
ECCPAS2          EQU  H'0006'
ECCPASE          EQU  H'0007'


;----- ECCP1AS Bits -----------------------------------------------------
PSSBD0           EQU  H'0000'
PSSBD1           EQU  H'0001'
PSSAC0           EQU  H'0002'
PSSAC1           EQU  H'0003'
ECCPAS0          EQU  H'0004'
ECCPAS1          EQU  H'0005'
ECCPAS2          EQU  H'0006'
ECCPASE          EQU  H'0007'


;----- CCP1DEL Bits -----------------------------------------------------
PDC0             EQU  H'0000'
PDC1             EQU  H'0001'
PDC2             EQU  H'0002'
PDC3             EQU  H'0003'
PDC4             EQU  H'0004'
PDC5             EQU  H'0005'
PDC6             EQU  H'0006'
PRSEN            EQU  H'0007'


;----- ECCP1DEL Bits -----------------------------------------------------
PDC0             EQU  H'0000'
PDC1             EQU  H'0001'
PDC2             EQU  H'0002'
PDC3             EQU  H'0003'
PDC4             EQU  H'0004'
PDC5             EQU  H'0005'
PDC6             EQU  H'0006'
PRSEN            EQU  H'0007'


;----- BAUDCON Bits -----------------------------------------------------
ABDEN            EQU  H'0000'
WUE              EQU  H'0001'
BRG16            EQU  H'0003'
SCKP             EQU  H'0004'
RCIDL            EQU  H'0006'
ABDOVF           EQU  H'0007'

RCMT             EQU  H'0006'


;----- CCP2CON Bits -----------------------------------------------------
CCP2M0           EQU  H'0000'
CCP2M1           EQU  H'0001'
CCP2M2           EQU  H'0002'
CCP2M3           EQU  H'0003'
DC2B0            EQU  H'0004'
DC2B1            EQU  H'0005'


;----- CCP1CON Bits -----------------------------------------------------
CCP1M0           EQU  H'0000'
CCP1M1           EQU  H'0001'
CCP1M2           EQU  H'0002'
CCP1M3           EQU  H'0003'
DC1B0            EQU  H'0004'
DC1B1            EQU  H'0005'
P1M0             EQU  H'0006'
P1M1             EQU  H'0007'


;----- ECCP1CON Bits -----------------------------------------------------
CCP1M0           EQU  H'0000'
CCP1M1           EQU  H'0001'
CCP1M2           EQU  H'0002'
CCP1M3           EQU  H'0003'
DC1B0            EQU  H'0004'
DC1B1            EQU  H'0005'
P1M0             EQU  H'0006'
P1M1             EQU  H'0007'


;----- ADCON2 Bits -----------------------------------------------------
ADCS0            EQU  H'0000'
ADCS1            EQU  H'0001'
ADCS2            EQU  H'0002'
ACQT0            EQU  H'0003'
ACQT1            EQU  H'0004'
ACQT2            EQU  H'0005'
ADFM             EQU  H'0007'


;----- ADCON1 Bits -----------------------------------------------------
PCFG0            EQU  H'0000'
PCFG1            EQU  H'0001'
PCFG2            EQU  H'0002'
PCFG3            EQU  H'0003'
VCFG0            EQU  H'0004'
VCFG1            EQU  H'0005'


;----- ADCON0 Bits -----------------------------------------------------
ADON             EQU  H'0000'
GO_DONE          EQU  H'0001'
CHS0             EQU  H'0002'
CHS1             EQU  H'0003'
CHS2             EQU  H'0004'
CHS3             EQU  H'0005'

DONE             EQU  H'0001'

GO               EQU  H'0001'

NOT_DONE         EQU  H'0001'


;----- SSPCON2 Bits -----------------------------------------------------
SEN              EQU  H'0000'
RSEN             EQU  H'0001'
PEN              EQU  H'0002'
RCEN             EQU  H'0003'
ACKEN            EQU  H'0004'
ACKDT            EQU  H'0005'
ACKSTAT          EQU  H'0006'
GCEN             EQU  H'0007'


;----- SSPCON1 Bits -----------------------------------------------------
SSPM0            EQU  H'0000'
SSPM1            EQU  H'0001'
SSPM2            EQU  H'0002'
SSPM3            EQU  H'0003'
CKP              EQU  H'0004'
SSPEN            EQU  H'0005'
SSPOV            EQU  H'0006'
WCOL             EQU  H'0007'


;----- SSPSTAT Bits -----------------------------------------------------
BF               EQU  H'0000'
UA               EQU  H'0001'
R_W              EQU  H'0002'
S                EQU  H'0003'
P                EQU  H'0004'
D_A              EQU  H'0005'
CKE              EQU  H'0006'
SMP              EQU  H'0007'

I2C_READ         EQU  H'0002'
I2C_START        EQU  H'0003'
I2C_STOP         EQU  H'0004'
I2C_DAT          EQU  H'0005'

NOT_W            EQU  H'0002'
NOT_A            EQU  H'0005'

NOT_WRITE        EQU  H'0002'
NOT_ADDRESS      EQU  H'0005'

READ_WRITE       EQU  H'0002'
DATA_ADDRESS     EQU  H'0005'

R                EQU  H'0002'
D                EQU  H'0005'


;----- T2CON Bits -----------------------------------------------------
T2CKPS0          EQU  H'0000'
T2CKPS1          EQU  H'0001'
TMR2ON           EQU  H'0002'
T2OUTPS0         EQU  H'0003'
T2OUTPS1         EQU  H'0004'
T2OUTPS2         EQU  H'0005'
T2OUTPS3         EQU  H'0006'


;----- T1CON Bits -----------------------------------------------------
TMR1ON           EQU  H'0000'
TMR1CS           EQU  H'0001'
T1SYNC           EQU  H'0002'
T1OSCEN          EQU  H'0003'
T1CKPS0          EQU  H'0004'
T1CKPS1          EQU  H'0005'
T1RUN            EQU  H'0006'
RD16             EQU  H'0007'

NOT_T1SYNC       EQU  H'0002'


;----- RCON Bits -----------------------------------------------------
NOT_BOR          EQU  H'0000'
NOT_POR          EQU  H'0001'
NOT_PD           EQU  H'0002'
NOT_TO           EQU  H'0003'
NOT_RI           EQU  H'0004'
SBOREN           EQU  H'0006'
NOT_IPEN         EQU  H'0007'

BOR              EQU  H'0000'
POR              EQU  H'0001'
PD               EQU  H'0002'
TO               EQU  H'0003'
RI               EQU  H'0004'
IPEN             EQU  H'0007'


;----- WDTCON Bits -----------------------------------------------------
SWDTEN           EQU  H'0000'

SWDTE            EQU  H'0000'


;----- HLVDCON Bits -----------------------------------------------------
LVDL0            EQU  H'0000'
LVDL1            EQU  H'0001'
LVDL2            EQU  H'0002'
LVDL3            EQU  H'0003'
LVDEN            EQU  H'0004'
IRVST            EQU  H'0005'

LVV0             EQU  H'0000'
LVV1             EQU  H'0001'
LVV2             EQU  H'0002'
LVV3             EQU  H'0003'
BGST             EQU  H'0005'

HLVDL0           EQU  H'0000'
HLVDL1           EQU  H'0001'
HLVDL2           EQU  H'0002'
HLVDL3           EQU  H'0003'
HLVDEN           EQU  H'0004'
VDIRMAG          EQU  H'0007'

IVRST            EQU  H'0005'


;----- LVDCON Bits -----------------------------------------------------
LVDL0            EQU  H'0000'
LVDL1            EQU  H'0001'
LVDL2            EQU  H'0002'
LVDL3            EQU  H'0003'
LVDEN            EQU  H'0004'
IRVST            EQU  H'0005'

LVV0             EQU  H'0000'
LVV1             EQU  H'0001'
LVV2             EQU  H'0002'
LVV3             EQU  H'0003'
BGST             EQU  H'0005'

HLVDL0           EQU  H'0000'
HLVDL1           EQU  H'0001'
HLVDL2           EQU  H'0002'
HLVDL3           EQU  H'0003'
HLVDEN           EQU  H'0004'
VDIRMAG          EQU  H'0007'

IVRST            EQU  H'0005'


;----- OSCCON Bits -----------------------------------------------------
SCS0             EQU  H'0000'
SCS1             EQU  H'0001'
IOFS             EQU  H'0002'
OSTS             EQU  H'0003'
IRCF0            EQU  H'0004'
IRCF1            EQU  H'0005'
IRCF2            EQU  H'0006'
IDLEN            EQU  H'0007'

FLTS             EQU  H'0002'


;----- T0CON Bits -----------------------------------------------------
T0PS0            EQU  H'0000'
T0PS1            EQU  H'0001'
T0PS2            EQU  H'0002'
PSA              EQU  H'0003'
T0SE             EQU  H'0004'
T0CS             EQU  H'0005'
T08BIT           EQU  H'0006'
TMR0ON           EQU  H'0007'


;----- STATUS Bits -----------------------------------------------------
C                EQU  H'0000'
DC               EQU  H'0001'
Z                EQU  H'0002'
OV               EQU  H'0003'
N                EQU  H'0004'


;----- INTCON3 Bits -----------------------------------------------------
INT1IF           EQU  H'0000'
INT2IF           EQU  H'0001'
INT1IE           EQU  H'0003'
INT2IE           EQU  H'0004'
INT1IP           EQU  H'0006'
INT2IP           EQU  H'0007'

INT1F            EQU  H'0000'
INT2F            EQU  H'0001'
INT1E            EQU  H'0003'
INT2E            EQU  H'0004'
INT1P            EQU  H'0006'
INT2P            EQU  H'0007'


;----- INTCON2 Bits -----------------------------------------------------
RBIP             EQU  H'0000'
TMR0IP           EQU  H'0002'
INTEDG2          EQU  H'0004'
INTEDG1          EQU  H'0005'
INTEDG0          EQU  H'0006'
NOT_RBPU         EQU  H'0007'

T0IP             EQU  H'0002'
RBPU             EQU  H'0007'


;----- INTCON Bits -----------------------------------------------------
RBIF             EQU  H'0000'
INT0IF           EQU  H'0001'
TMR0IF           EQU  H'0002'
RBIE             EQU  H'0003'
INT0IE           EQU  H'0004'
TMR0IE           EQU  H'0005'
PEIE             EQU  H'0006'
GIE              EQU  H'0007'

INT0F            EQU  H'0001'
T0IF             EQU  H'0002'
INT0E            EQU  H'0004'
T0IE             EQU  H'0005'
GIEL             EQU  H'0006'
GIEH             EQU  H'0007'


;----- STKPTR Bits -----------------------------------------------------
STKPTR0          EQU  H'0000'
STKPTR1          EQU  H'0001'
STKPTR2          EQU  H'0002'
STKPTR3          EQU  H'0003'
STKPTR4          EQU  H'0004'
STKUNF           EQU  H'0006'
STKFUL           EQU  H'0007'



;==========================================================================
;
;       RAM Definitions
;
;==========================================================================
       __MAXRAM  H'0FFF'
       __BADRAM  H'0800'-H'0F5F'
       __BADRAM  H'0F85'-H'0F88'
       __BADRAM  H'0F8E'-H'0F91'
       __BADRAM  H'0F97'-H'0F9A'
       __BADRAM  H'0F9C'
       __BADRAM  H'0FA3'-H'0FA5'
       __BADRAM  H'0FAA'
       __BADRAM  H'0FB9'
       __BADRAM  H'0FD4'

;==========================================================================
;
;   IMPORTANT: For the PIC18 devices, the __CONFIG directive has been
;              superseded by the CONFIG directive.  The following settings
;              are available for this device.
;
;   PLL Prescaler Selection bits:
;     PLLDIV = 1           No prescale (4 MHz oscillator input drives PLL directly)
;     PLLDIV = 2           Divide by 2 (8 MHz oscillator input)
;     PLLDIV = 3           Divide by 3 (12 MHz oscillator input)
;     PLLDIV = 4           Divide by 4 (16 MHz oscillator input)
;     PLLDIV = 5           Divide by 5 (20 MHz oscillator input)
;     PLLDIV = 6           Divide by 6 (24 MHz oscillator input)
;     PLLDIV = 10          Divide by 10 (40 MHz oscillator input)
;     PLLDIV = 12          Divide by 12 (48 MHz oscillator input)
;
;   CPU System Clock Postscaler:
;     CPUDIV = OSC1_PLL2   [OSC1/OSC2 Src: /1][96 MHz PLL Src: /2]
;     CPUDIV = OSC2_PLL3   [OSC1/OSC2 Src: /2][96 MHz PLL Src: /3]
;     CPUDIV = OSC3_PLL4   [OSC1/OSC2 Src: /3][96 MHz PLL Src: /4]
;     CPUDIV = OSC4_PLL6   [OSC1/OSC2 Src: /4][96 MHz PLL Src: /6]
;
;   USB Clock Selection bit (used in Full Speed USB mode only; UCFG:FSEN = 1):
;     USBDIV = 1           USB clock source comes directly from the primary oscillator block with no postscale
;     USBDIV = 2           USB clock source comes from the 96 MHz PLL divided by 2
;
;   Oscillator Selection bits:
;     FOSC = XT_XT         XT oscillator, XT used by USB
;     FOSC = XTPLL_XT      XT oscillator, PLL enabled, XT used by USB
;     FOSC = ECIO_EC       External clock, port function on RA6, EC used by USB
;     FOSC = EC_EC         External clock, CLKOUT on RA6, EC used by USB
;     FOSC = ECPLLIO_EC    External clock, PLL enabled, port function on RA6, EC used by USB
;     FOSC = ECPLL_EC      External clock, PLL enabled, CLKOUT on RA6, EC used by USB
;     FOSC = INTOSCIO_EC   Internal oscillator, port function on RA6, EC used by USB
;     FOSC = INTOSC_EC     Internal oscillator, CLKOUT on RA6, EC used by USB
;     FOSC = INTOSC_XT     Internal oscillator, XT used by USB
;     FOSC = INTOSC_HS     Internal oscillator, HS used by USB
;     FOSC = HS            HS oscillator, HS used by USB
;     FOSC = HSPLL_HS      HS oscillator, PLL enabled, HS used by USB
;
;   Fail-Safe Clock Monitor Enable bit:
;     FCMEN = OFF          Fail-Safe Clock Monitor disabled
;     FCMEN = ON           Fail-Safe Clock Monitor enabled
;
;   Internal/External Oscillator Switchover bit:
;     IESO = OFF           Oscillator Switchover mode disabled
;     IESO = ON            Oscillator Switchover mode enabled
;
;   Power-up Timer Enable bit:
;     PWRT = ON            PWRT enabled
;     PWRT = OFF           PWRT disabled
;
;   Brown-out Reset Enable bits:
;     BOR = OFF            Brown-out Reset disabled in hardware and software
;     BOR = SOFT           Brown-out Reset enabled and controlled by software (SBOREN is enabled)
;     BOR = ON_ACTIVE      Brown-out Reset enabled in hardware only and disabled in Sleep mode (SBOREN is disabled)
;     BOR = ON             Brown-out Reset enabled in hardware only (SBOREN is disabled)
;
;   Brown-out Voltage bits:
;     BORV = 0             Maximum setting
;     BORV = 1             
;     BORV = 2             
;     BORV = 3             Minimum setting
;
;   USB Voltage Regulator Enable bit:
;     VREGEN = OFF         USB voltage regulator disabled
;     VREGEN = ON          USB voltage regulator enabled
;
;   Watchdog Timer Enable bit:
;     WDT = OFF            HW Disabled - SW Controlled
;     WDT = ON             HW Enabled - SW Disabled
;
;   Watchdog Timer Postscale Select bits:
;     WDTPS = 1            1:1
;     WDTPS = 2            1:2
;     WDTPS = 4            1:4
;     WDTPS = 8            1:8
;     WDTPS = 16           1:16
;     WDTPS = 32           1:32
;     WDTPS = 64           1:64
;     WDTPS = 128          1:128
;     WDTPS = 256          1:256
;     WDTPS = 512          1:512
;     WDTPS = 1024         1:1024
;     WDTPS = 2048         1:2048
;     WDTPS = 4096         1:4096
;     WDTPS = 8192         1:8192
;     WDTPS = 16384        1:16384
;     WDTPS = 32768        1:32768
;
;   MCLR Pin Enable bit:
;     MCLRE = OFF          RE3 input pin enabled; MCLR disabled
;     MCLRE = ON           MCLR pin enabled; RE3 input pin disabled
;
;   Low-Power Timer 1 Oscillator Enable bit:
;     LPT1OSC = OFF        Timer1 configured for higher power operation
;     LPT1OSC = ON         Timer1 configured for low-power operation
;
;   PORTB A/D Enable bit:
;     PBADEN = OFF         PORTB<4:0> pins are configured as digital I/O on Reset
;     PBADEN = ON          PORTB<4:0> pins are configured as analog input channels on Reset
;
;   CCP2 MUX bit:
;     CCP2MX = OFF         CCP2 input/output is multiplexed with RB3
;     CCP2MX = ON          CCP2 input/output is multiplexed with RC1
;
;   Stack Full/Underflow Reset Enable bit:
;     STVREN = OFF         Stack full/underflow will not cause Reset
;     STVREN = ON          Stack full/underflow will cause Reset
;
;   Single-Supply ICSP Enable bit:
;     LVP = OFF            Single-Supply ICSP disabled
;     LVP = ON             Single-Supply ICSP enabled
;
;   Dedicated In-Circuit Debug/Programming Port (ICPORT) Enable bit:
;     ICPRT = OFF          ICPORT disabled
;     ICPRT = ON           ICPORT enabled
;
;   Extended Instruction Set Enable bit:
;     XINST = OFF          Instruction set extension and Indexed Addressing mode disabled (Legacy mode)
;     XINST = ON           Instruction set extension and Indexed Addressing mode enabled
;
;   Background Debugger Enable bit:
;     DEBUG = ON           Background debugger enabled, RB6 and RB7 are dedicated to In-Circuit Debug
;     DEBUG = OFF          Background debugger disabled, RB6 and RB7 configured as general purpose I/O pins
;
;   Code Protection bit Block 0:
;     CP0 = ON             Block 0 (000800-001FFFh) code-protected
;     CP0 = OFF            Block 0 (000800-001FFFh) not code-protected
;
;   Code Protection bit Block 1:
;     CP1 = ON             Block 1 (002000-003FFFh) code-protected
;     CP1 = OFF            Block 1 (002000-003FFFh) not code-protected
;
;   Code Protection bit Block 2:
;     CP2 = ON             Block 2 (004000-005FFFh) code-protected
;     CP2 = OFF            Block 2 (004000-005FFFh) not code-protected
;
;   Boot Block Code Protection bit:
;     CPB = ON             Boot block (000000-0007FFh) code-protected
;     CPB = OFF            Boot block (000000-0007FFh) not code-protected
;
;   Data EEPROM Code Protection bit:
;     CPD = ON             Data EEPROM code-protected
;     CPD = OFF            Data EEPROM not code-protected
;
;   Write Protection bit Block 0:
;     WRT0 = ON            Block 0 (000800-001FFFh) write-protected
;     WRT0 = OFF           Block 0 (000800-001FFFh) not write-protected
;
;   Write Protection bit Block 1:
;     WRT1 = ON            Block 1 (002000-003FFFh) write-protected
;     WRT1 = OFF           Block 1 (002000-003FFFh) not write-protected
;
;   Write Protection bit Block 2:
;     WRT2 = ON            Block 2 (004000-005FFFh) write-protected
;     WRT2 = OFF           Block 2 (004000-005FFFh) not write-protected
;
;   Boot Block Write Protection bit:
;     WRTB = ON            Boot block (000000-0007FFh) write-protected
;     WRTB = OFF           Boot block (000000-0007FFh) not write-protected
;
;   Configuration Register Write Protection bit:
;     WRTC = ON            Configuration registers (300000-3000FFh) write-protected
;     WRTC = OFF           Configuration registers (300000-3000FFh) not write-protected
;
;   Data EEPROM Write Protection bit:
;     WRTD = ON            Data EEPROM write-protected
;     WRTD = OFF           Data EEPROM not write-protected
;
;   Table Read Protection bit Block 0:
;     EBTR0 = ON           Block 0 (000800-001FFFh) protected from table reads executed in other blocks
;     EBTR0 = OFF          Block 0 (000800-001FFFh) not protected from table reads executed in other blocks
;
;   Table Read Protection bit Block 1:
;     EBTR1 = ON           Block 1 (002000-003FFFh) protected from table reads executed in other blocks
;     EBTR1 = OFF          Block 1 (002000-003FFFh) not protected from table reads executed in other blocks
;
;   Table Read Protection bit Block 2:
;     EBTR2 = ON           Block 2 (004000-005FFFh) protected from table reads executed in other blocks
;     EBTR2 = OFF          Block 2 (004000-005FFFh) not protected from table reads executed in other blocks
;
;   Boot Block Table Read Protection:
;     EBTRB = ON           Boot block (000000-0007FFh) protected from table reads executed in other blocks
;     EBTRB = OFF          Boot block (000000-0007FFh) not protected from table reads executed in other blocks
;
;==========================================================================
;==========================================================================
;
;       Configuration Bits
;
;   NAME            Address
;   CONFIG1L        300000h
;   CONFIG1H        300001h
;   CONFIG2L        300002h
;   CONFIG2H        300003h
;   CONFIG3H        300005h
;   CONFIG4L        300006h
;   CONFIG5L        300008h
;   CONFIG5H        300009h
;   CONFIG6L        30000Ah
;   CONFIG6H        30000Bh
;   CONFIG7L        30000Ch
;   CONFIG7H        30000Dh
;
;==========================================================================

; The following is an assignment of address values for all of the
; configuration registers for the purpose of table reads
_CONFIG1L        EQU  H'300000'
_CONFIG1H        EQU  H'300001'
_CONFIG2L        EQU  H'300002'
_CONFIG2H        EQU  H'300003'
_CONFIG3H        EQU  H'300005'
_CONFIG4L        EQU  H'300006'
_CONFIG5L        EQU  H'300008'
_CONFIG5H        EQU  H'300009'
_CONFIG6L        EQU  H'30000A'
_CONFIG6H        EQU  H'30000B'
_CONFIG7L        EQU  H'30000C'
_CONFIG7H        EQU  H'30000D'

;----- CONFIG1L Options --------------------------------------------------
_PLLDIV_1_1L         EQU  H'F8'    ; No prescale (4 MHz oscillator input drives PLL directly)
_PLLDIV_2_1L         EQU  H'F9'    ; Divide by 2 (8 MHz oscillator input)
_PLLDIV_3_1L         EQU  H'FA'    ; Divide by 3 (12 MHz oscillator input)
_PLLDIV_4_1L         EQU  H'FB'    ; Divide by 4 (16 MHz oscillator input)
_PLLDIV_5_1L         EQU  H'FC'    ; Divide by 5 (20 MHz oscillator input)
_PLLDIV_6_1L         EQU  H'FD'    ; Divide by 6 (24 MHz oscillator input)
_PLLDIV_10_1L        EQU  H'FE'    ; Divide by 10 (40 MHz oscillator input)
_PLLDIV_12_1L        EQU  H'FF'    ; Divide by 12 (48 MHz oscillator input)

_CPUDIV_OSC1_PLL2_1L EQU  H'E7'    ; [OSC1/OSC2 Src: /1][96 MHz PLL Src: /2]
_CPUDIV_OSC2_PLL3_1L EQU  H'EF'    ; [OSC1/OSC2 Src: /2][96 MHz PLL Src: /3]
_CPUDIV_OSC3_PLL4_1L EQU  H'F7'    ; [OSC1/OSC2 Src: /3][96 MHz PLL Src: /4]
_CPUDIV_OSC4_PLL6_1L EQU  H'FF'    ; [OSC1/OSC2 Src: /4][96 MHz PLL Src: /6]

_USBDIV_1_1L         EQU  H'DF'    ; USB clock source comes directly from the primary oscillator block with no postscale
_USBDIV_2_1L         EQU  H'FF'    ; USB clock source comes from the 96 MHz PLL divided by 2

;----- CONFIG1H Options --------------------------------------------------
_FOSC_XT_XT_1H       EQU  H'F0'    ; XT oscillator, XT used by USB
_FOSC_XTPLL_XT_1H    EQU  H'F2'    ; XT oscillator, PLL enabled, XT used by USB
_FOSC_ECIO_EC_1H     EQU  H'F4'    ; External clock, port function on RA6, EC used by USB
_FOSC_EC_EC_1H       EQU  H'F5'    ; External clock, CLKOUT on RA6, EC used by USB
_FOSC_ECPLLIO_EC_1H  EQU  H'F6'    ; External clock, PLL enabled, port function on RA6, EC used by USB
_FOSC_ECPLL_EC_1H    EQU  H'F7'    ; External clock, PLL enabled, CLKOUT on RA6, EC used by USB
_FOSC_INTOSCIO_EC_1H EQU  H'F8'    ; Internal oscillator, port function on RA6, EC used by USB
_FOSC_INTOSC_EC_1H   EQU  H'F9'    ; Internal oscillator, CLKOUT on RA6, EC used by USB
_FOSC_INTOSC_XT_1H   EQU  H'FA'    ; Internal oscillator, XT used by USB
_FOSC_INTOSC_HS_1H   EQU  H'FB'    ; Internal oscillator, HS used by USB
_FOSC_HS_1H          EQU  H'FC'    ; HS oscillator, HS used by USB
_FOSC_HSPLL_HS_1H    EQU  H'FE'    ; HS oscillator, PLL enabled, HS used by USB

_FCMEN_OFF_1H        EQU  H'BF'    ; Fail-Safe Clock Monitor disabled
_FCMEN_ON_1H         EQU  H'FF'    ; Fail-Safe Clock Monitor enabled

_IESO_OFF_1H         EQU  H'7F'    ; Oscillator Switchover mode disabled
_IESO_ON_1H          EQU  H'FF'    ; Oscillator Switchover mode enabled

;----- CONFIG2L Options --------------------------------------------------
_PWRT_ON_2L          EQU  H'FE'    ; PWRT enabled
_PWRT_OFF_2L         EQU  H'FF'    ; PWRT disabled

_BOR_OFF_2L          EQU  H'F9'    ; Brown-out Reset disabled in hardware and software
_BOR_SOFT_2L         EQU  H'FB'    ; Brown-out Reset enabled and controlled by software (SBOREN is enabled)
_BOR_ON_ACTIVE_2L    EQU  H'FD'    ; Brown-out Reset enabled in hardware only and disabled in Sleep mode (SBOREN is disabled)
_BOR_ON_2L           EQU  H'FF'    ; Brown-out Reset enabled in hardware only (SBOREN is disabled)

_BORV_0_2L           EQU  H'E7'    ; Maximum setting
_BORV_1_2L           EQU  H'EF'    ; 
_BORV_2_2L           EQU  H'F7'    ; 
_BORV_3_2L           EQU  H'FF'    ; Minimum setting

_VREGEN_OFF_2L       EQU  H'DF'    ; USB voltage regulator disabled
_VREGEN_ON_2L        EQU  H'FF'    ; USB voltage regulator enabled

;----- CONFIG2H Options --------------------------------------------------
_WDT_OFF_2H          EQU  H'FE'    ; HW Disabled - SW Controlled
_WDT_ON_2H           EQU  H'FF'    ; HW Enabled - SW Disabled

_WDTPS_1_2H          EQU  H'E1'    ; 1:1
_WDTPS_2_2H          EQU  H'E3'    ; 1:2
_WDTPS_4_2H          EQU  H'E5'    ; 1:4
_WDTPS_8_2H          EQU  H'E7'    ; 1:8
_WDTPS_16_2H         EQU  H'E9'    ; 1:16
_WDTPS_32_2H         EQU  H'EB'    ; 1:32
_WDTPS_64_2H         EQU  H'ED'    ; 1:64
_WDTPS_128_2H        EQU  H'EF'    ; 1:128
_WDTPS_256_2H        EQU  H'F1'    ; 1:256
_WDTPS_512_2H        EQU  H'F3'    ; 1:512
_WDTPS_1024_2H       EQU  H'F5'    ; 1:1024
_WDTPS_2048_2H       EQU  H'F7'    ; 1:2048
_WDTPS_4096_2H       EQU  H'F9'    ; 1:4096
_WDTPS_8192_2H       EQU  H'FB'    ; 1:8192
_WDTPS_16384_2H      EQU  H'FD'    ; 1:16384
_WDTPS_32768_2H      EQU  H'FF'    ; 1:32768

;----- CONFIG3H Options --------------------------------------------------
_MCLRE_OFF_3H        EQU  H'7F'    ; RE3 input pin enabled; MCLR disabled
_MCLRE_ON_3H         EQU  H'FF'    ; MCLR pin enabled; RE3 input pin disabled

_LPT1OSC_OFF_3H      EQU  H'FB'    ; Timer1 configured for higher power operation
_LPT1OSC_ON_3H       EQU  H'FF'    ; Timer1 configured for low-power operation

_PBADEN_OFF_3H       EQU  H'FD'    ; PORTB<4:0> pins are configured as digital I/O on Reset
_PBADEN_ON_3H        EQU  H'FF'    ; PORTB<4:0> pins are configured as analog input channels on Reset

_CCP2MX_OFF_3H       EQU  H'FE'    ; CCP2 input/output is multiplexed with RB3
_CCP2MX_ON_3H        EQU  H'FF'    ; CCP2 input/output is multiplexed with RC1

;----- CONFIG4L Options --------------------------------------------------
_STVREN_OFF_4L       EQU  H'FE'    ; Stack full/underflow will not cause Reset
_STVREN_ON_4L        EQU  H'FF'    ; Stack full/underflow will cause Reset

_LVP_OFF_4L          EQU  H'FB'    ; Single-Supply ICSP disabled
_LVP_ON_4L           EQU  H'FF'    ; Single-Supply ICSP enabled

_ICPRT_OFF_4L        EQU  H'DF'    ; ICPORT disabled
_ICPRT_ON_4L         EQU  H'FF'    ; ICPORT enabled

_XINST_OFF_4L        EQU  H'BF'    ; Instruction set extension and Indexed Addressing mode disabled (Legacy mode)
_XINST_ON_4L         EQU  H'FF'    ; Instruction set extension and Indexed Addressing mode enabled

_DEBUG_ON_4L         EQU  H'7F'    ; Background debugger enabled, RB6 and RB7 are dedicated to In-Circuit Debug
_DEBUG_OFF_4L        EQU  H'FF'    ; Background debugger disabled, RB6 and RB7 configured as general purpose I/O pins

;----- CONFIG5L Options --------------------------------------------------
_CP0_ON_5L           EQU  H'FE'    ; Block 0 (000800-001FFFh) code-protected
_CP0_OFF_5L          EQU  H'FF'    ; Block 0 (000800-001FFFh) not code-protected

_CP1_ON_5L           EQU  H'FD'    ; Block 1 (002000-003FFFh) code-protected
_CP1_OFF_5L          EQU  H'FF'    ; Block 1 (002000-003FFFh) not code-protected

_CP2_ON_5L           EQU  H'FB'    ; Block 2 (004000-005FFFh) code-protected
_CP2_OFF_5L          EQU  H'FF'    ; Block 2 (004000-005FFFh) not code-protected

;----- CONFIG5H Options --------------------------------------------------
_CPB_ON_5H           EQU  H'BF'    ; Boot block (000000-0007FFh) code-protected
_CPB_OFF_5H          EQU  H'FF'    ; Boot block (000000-0007FFh) not code-protected

_CPD_ON_5H           EQU  H'7F'    ; Data EEPROM code-protected
_CPD_OFF_5H          EQU  H'FF'    ; Data EEPROM not code-protected

;----- CONFIG6L Options --------------------------------------------------
_WRT0_ON_6L          EQU  H'FE'    ; Block 0 (000800-001FFFh) write-protected
_WRT0_OFF_6L         EQU  H'FF'    ; Block 0 (000800-001FFFh) not write-protected

_WRT1_ON_6L          EQU  H'FD'    ; Block 1 (002000-003FFFh) write-protected
_WRT1_OFF_6L         EQU  H'FF'    ; Block 1 (002000-003FFFh) not write-protected

_WRT2_ON_6L          EQU  H'FB'    ; Block 2 (004000-005FFFh) write-protected
_WRT2_OFF_6L         EQU  H'FF'    ; Block 2 (004000-005FFFh) not write-protected

;----- CONFIG6H Options --------------------------------------------------
_WRTB_ON_6H          EQU  H'BF'    ; Boot block (000000-0007FFh) write-protected
_WRTB_OFF_6H         EQU  H'FF'    ; Boot block (000000-0007FFh) not write-protected

_WRTC_ON_6H          EQU  H'DF'    ; Configuration registers (300000-3000FFh) write-protected
_WRTC_OFF_6H         EQU  H'FF'    ; Configuration registers (300000-3000FFh) not write-protected

_WRTD_ON_6H          EQU  H'7F'    ; Data EEPROM write-protected
_WRTD_OFF_6H         EQU  H'FF'    ; Data EEPROM not write-protected

;----- CONFIG7L Options --------------------------------------------------
_EBTR0_ON_7L         EQU  H'FE'    ; Block 0 (000800-001FFFh) protected from table reads executed in other blocks
_EBTR0_OFF_7L        EQU  H'FF'    ; Block 0 (000800-001FFFh) not protected from table reads executed in other blocks

_EBTR1_ON_7L         EQU  H'FD'    ; Block 1 (002000-003FFFh) protected from table reads executed in other blocks
_EBTR1_OFF_7L        EQU  H'FF'    ; Block 1 (002000-003FFFh) not protected from table reads executed in other blocks

_EBTR2_ON_7L         EQU  H'FB'    ; Block 2 (004000-005FFFh) protected from table reads executed in other blocks
_EBTR2_OFF_7L        EQU  H'FF'    ; Block 2 (004000-005FFFh) not protected from table reads executed in other blocks

;----- CONFIG7H Options --------------------------------------------------
_EBTRB_ON_7H         EQU  H'BF'    ; Boot block (000000-0007FFh) protected from table reads executed in other blocks
_EBTRB_OFF_7H        EQU  H'FF'    ; Boot block (000000-0007FFh) not protected from table reads executed in other blocks


_DEVID1          EQU  H'3FFFFE'
_DEVID2          EQU  H'3FFFFF'

_IDLOC0          EQU  H'200000'
_IDLOC1          EQU  H'200001'
_IDLOC2          EQU  H'200002'
_IDLOC3          EQU  H'200003'
_IDLOC4          EQU  H'200004'
_IDLOC5          EQU  H'200005'
_IDLOC6          EQU  H'200006'
_IDLOC7          EQU  H'200007'

        LIST
