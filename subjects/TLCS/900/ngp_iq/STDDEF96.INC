                save
                listing off     ; kein Listing Åber diesen File

;****************************************************************************
;*                                                                          *
;*   AS 1.39 - Datei STDDEF96.INC                                           *
;*                                                                          *
;*   Sinn : enthÑlt Register- und Speicheradressen fÅr TLCS-900-Prozessoren *
;*          Port- bzw. Registernamen nach Toshiba-Konvention                *
;*          aus dem Handbuch zur Prozessorfamilie TLCS-900                  *
;*                                                                          *
;*   Original von Ernst Ahlers, Oktober 1993                                *
;*   fÅr AS angepa·t von Alfred Arnold                                      *
;*                                                                          *
;*   letzte énderungen :  30.10.1993                                        *
;*                                                                          *
;****************************************************************************

                ifndef  stddef96inc     ; verhindert Mehrfacheinbindung

stddef96inc     equ     1

                if      (MOMCPU<>9879873)&&(MOMCPU<>9683265)
		 fatal  "Falscher Prozessortyp eingestellt: nur 96C141 oder 93C141 erlaubt"
                endif

                if      MOMPASS=1
                 message "TLCS-900(L)-Adre·definitionen"
                 message "(C) 1993 Ernst Ahlers, Alfred Arnold"
                endif

;----------------------------------------------------------------------------
; hier geht's los...

P0              equ     0000H   ; Port 0 Register
P0CR            equ     0002H   ; Port 0 Control

P1              equ     0001H   ; Port 1 Register
P1CR            equ     0004H   ; Port 1 Control
P1FC            equ     0005H   ; Port 1 Function

P2              equ     0006H   ; Port 2 Register
P2CR            equ     0008H   ; Port 2 Control
P2FC            equ     0009H   ; Port 2 Function

P3              equ     0007H   ; Port 3 Register
P3CR            equ     000AH   ; Port 3 Control
P3FC            equ     000BH   ; Port 3 Function

P4              equ     000CH   ; Port 4 Register
P4CR            equ     000EH   ; Port 4 Control
P4FC            equ     0010H   ; Port 4 Function

P5              equ     000DH   ; Port 5 Register

P6              equ     0012H   ; Port 6 Register
P6CR            equ     0014H   ; Port 6 Control
P6FC            equ     0016H   ; Port 6 Function

P7              equ     0013H   ; Port 7 Register
P7CR            equ     0015H   ; Port 7 Control
P7FC            equ     0017H   ; Port 7 Function

P8              equ     0018H   ; Port 8 Register
P8CR            equ     001AH   ; Port 8 Control
P8FC            equ     001CH   ; Port 8 Function

P9              equ     0019H   ; Port 9 Register
P9CR            equ     001BH   ; Port 9 Control
P9FC            equ     001DH   ; Port 9 Function

TRUN            equ     0020H   ; Timer Operation Control
TREG0           equ     0022H   ; Timer Register 0
TREG1           equ     0023H   ; Timer Register 1
TMOD            equ     0024H   ; Timer Mode Control
TFFCF           equ     0025H   ; Timer Flip-Flop Control
TREG2           equ     0026H   ; Timer Register 2
TREG3           equ     0027H   ; Timer Register 3
P0MOD           equ     0028H   ; 8-Bit PWM 0 Mode Control
P1MOD           equ     0029H   ; 8-Bit PWM 1 Mode Control
PFFCR           equ     002AH   ; 8-Bit PWM Flip-Flop Control

TREG4L          equ     0030H   ; Timer 4 Low Byte
TREG4H          equ     0031H   ; Timer 4 High Byte
TREG5L          equ     0032H   ; Timer 5 Low Byte
TREG5H          equ     0033H   ; Timer 5 High Byte
CAP1L           equ     0034H   ; Capture 1 Low Byte
CAP1H           equ     0035H   ; Capture 1 High Byte
CAP2L           equ     0036H   ; Capture 2 Low Byte
CAP2H           equ     0037H   ; Capture 2 High Byte
T4MOD           equ     0038H   ; 16-Bit Timer 4 Mode Control
T4FFCR          equ     0039H   ; 16-Bit Timer 4 Flip-Flop Control
T45CR           equ     003AH   ; 16-Bit Timer 4/5 Control

TREG6L          equ     0040H   ; Timer 6 Low Byte
TREG6H          equ     0041H   ; Timer 6 High Byte
TREG7L          equ     0042H   ; Timer 7 Low Byte
TREG7H          equ     0043H   ; Timer 7 High Byte
CAP3L           equ     0044H   ; Capture 3 Low Byte
CAP3H           equ     0045H   ; Capture 3 High Byte
CAP4L           equ     0046H   ; Capture 4 Low Byte
CAP4H           equ     0047H   ; Capture 4 High Byte
T5MOD           equ     0048H   ; 16-Bit Timer 4 Mode Control
T5FFCR          equ     0049H   ; 16-Bit Timer 4 Flip-Flop Control

PG0REG          equ     004CH   ; Pattern Generator 0
PG1REG          equ     004DH   ; Pattern Generator 1
PG01CR          equ     004EH   ; Pattern Generator 0/1 Control

SC0BUF          equ     0050H   ; Serial Channel 0 Buffer Data
SC0CR           equ     0051H   ; Serial Channel 0 Control
SC0MOD          equ     0052H   ; Serial Channel 0 Mode Control
BR0CR           equ     0053H   ; Bit Rate Ser. Channel 0 Control
SC1BUF          equ     0054H   ; Serial Channel 1 Buffer Data
SC1CR           equ     0055H   ; Serial Channel 1 Control
SC1MOD          equ     0056H   ; Serial Channel 1 Mode Control
BR1CR           equ     0057H   ; Bit Rate Ser. Channel 1 Control

ODE             equ     0058H   ; Port 9 Open Drain Enable
WDMOD           equ     005CH   ; Watchdog Timer Mode
WDCR            equ     005DH   ; Watchdog Timer Control

ADMOD           equ     005EH   ; A/D Control
ADREG0L         equ     0060H   ; A/D Result Channel 0 Bit 7..6 = AD1..0
ADREG0H         equ     0061H   ; A/D Result Channel 0 Bit 7..0 = AD9..2
ADREG1L         equ     0062H   ; A/D Result Channel 1 Bit 7..6 = AD1..0
ADREG1H         equ     0063H   ; A/D Result Channel 1 Bit 7..0 = AD9..2
ADREG2L         equ     0064H   ; A/D Result Channel 2 Bit 7..6 = AD1..0
ADREG2H         equ     0065H   ; A/D Result Channel 2 Bit 7..0 = AD9..2
ADREG3L         equ     0066H   ; A/D Result Channel 3 Bit 7..6 = AD1..0
ADREG3H         equ     0067H   ; A/D Result Channel 3 Bit 7..0 = AD9..2

B0CS            equ     0068H   ; Block 0 CS/WAIT Control
B1CS            equ     0069H   ; Block 1 CS/WAIT Control
B2CS            equ     006AH   ; Block 2 CS/WAIT Control

INTE0AD         equ     0070H   ; Interrupt Enable/Level AD/Timer 0
INTE45          equ     0071H   ; Interrupt Enable/Level IRQ 5/4
INTE67          equ     0072H   ; Interrupt Enable/Level IRQ 7/6
INTET10         equ     0073H   ; Interrupt Enable/Level Timer 1/0
INTEPW10        equ     0074H   ; Interrupt Enable/Level Timer 3/2 (PWM 1/0)
INTET54         equ     0075H   ; Interrupt Enable/Level TREG 5/4
INTET76         equ     0076H   ; Interrupt Enable/Level TREG 7/6
INTES0          equ     0077H   ; Interrupt Enable/Level Serial Channel 0
INTES1          equ     0078H   ; Interrupt Enable/Level Serial Channel 1
IIMC            equ     007BH   ; Interrupt Input Mode Control

DMA0V           equ     007CH   ; fDMA 0 Start Vector
DMA1V           equ     007DH   ; fDMA 1 Start Vector
DMA2V           equ     007EH   ; fDMA 2 Start Vector
DMA3V           equ     007FH   ; fDMA 3 Start Vector

; feste Speicherbereiche

IRAM            equ     000080H ; Internal RAM 1K...
IRAMEND         equ     00047FH ; ...up to 00047FH
IEAREA          equ     008000H ; Interrupt Entry Area...
IEAREAEND       equ     0081FFH ; ...up to 0081FFH
IROM            equ     008200H ; Internal (P)ROM 31.5K...
IROMEND         equ     00FFFFH ; ...up to 00FFFFH
EXTMEM          equ     010000H ; External Memory
MEMEND          equ     0FFFFFFH ; ...up to FFFFFFH

;---------------------------------------------------------------------------
; das war's...

                endif

                restore

