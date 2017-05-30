;*
;* NGPC Register Defines
;*
;* by Jeff Frohwein
;*
;* v1.0 - Original release, 00-Apr-17
;* v1.1 - Converted from $xxxx to 0xxxxh format, 00-May-25
;* v1.2 - Added more regs, 00-May-27
;*

rWDCR   equ     6fh     ; Watch Dog Control Register
WD_CLR  equ     4eh     ;  Value required to be written to rWDT

rBV     equ     6F80h   ; Battery Voltage (Read Only word) (0-3ffh)
rSL     equ     6F82h   ; Sys Lever (Controller Input, read byte)
rUSERB  equ     6F84h   ; User Boot (Read only byte)
rUSERS  equ     6F85h   ; User Shutdown (Read only byte)
rUSERA  equ     6F86h   ; User Answer (Read/write byte)
rLANG   equ     6F87h   ; Language (Read Only byte: 0=Jap, 1=Eng)
rOSV    equ     6F91h   ; OS Version (Read only byte)

; User Program Interrupt Vectors

rSWI3   equ     6FB8h   ; Software Interrupt (SWI 3)
rSWI4   equ     6FBCh   ; Software Interrupt (SWI 4)
rSWI5   equ     6FC0h   ; Software Interrupt (SWI 5)
rSWI6   equ     6FC4h   ; Software Interrupt (SWI 6)
rRTCI   equ     6FC8h   ; RTC Alarm Interrupt
rVBI    equ     6FCCh   ; Vertical Blanking Interrupt
rZ80I   equ     6FD0h   ; Interrupt from Z80
rTI0    equ     6FD4h   ; Timer Interrupt (8 bit timer 0)
rTI1    equ     6FD8h   ; Timer Interrupt (8 bit timer 1)
rTI2    equ     6FDCh   ; Timer Interrupt (8 bit timer 2)
rTI3    equ     6FE0h   ; Timer Interrupt (8 bit timer 3)
rSTI    equ     6FE4h   ; Serial Transmission Interrupt
rSRI    equ     6FE8h   ; Serial Reception Interrupt
rEMDMA0 equ     6FF0h   ; End Micro DMA Int (MicroDMA 0)
rEMDMA1 equ     6FF4h   ; End Micro DMA Int (MicroDMA 1)
rEMDMA2 equ     6FF8h   ; End Micro DMA Int (MicroDMA 2)
rEMDMA3 equ     6FFCh   ; End Micro DMA Int (MicroDMA 3)

; Vint/Hint Enable/Disable

rICR    equ     8000h   ; Interrupt Control Register

; Window Registers

rWBAX   equ     8002h   ; WBA.H = Window Horizontal Origin
rWBAY   equ     8003h   ; WBA.V = Window Vertical Origin
rWSIX   equ     8004h   ; WSI.H = Window X Size
rWSIY   equ     8005h   ; WSI.V = Window Y Size

; Frame Rate Register

rREF    equ     8006h   ; REF = Frame Rate Register

; Raster Position Registers

rRASH   equ     8008h   ; RAS.H = Raster Position Horizontal
rRASV   equ     8009h   ; RAS.V = Raster Position Vertical

; 2D Status Register

r2DSR   equ     8010h   ; Character Over / Vblank Status

; 2D Control Register

r2DCR   equ     8012h   ; NEG / OOWC Setting

; Sprite Position Offset

rPOX    equ     8020h   ; PO.H = Position Offset Horizontal
rPOY    equ     8021h   ; PO.V = Position Offset Vertical

; Scroll Plane Priority Function

rPF     equ     8030h   ; Scroll plane 1 in front/back

; Scroll Offset Registers

rS1SOX  equ     8032h   ; S1SO.H = Scroll Plane 1 X Scroll Offset
rS1SOY  equ     8033h   ; S1SO.V = Scroll Plane 1 Y Scroll Offset
rS2SOX  equ     8034h   ; S2SO.H = Scroll Plane 2 X Scroll Offset
rS2SOY  equ     8035h   ; S2SO.V = Scroll Plane 2 Y Scroll Offset

; Background Colo(u)r Register

rBCR    equ     8118h   ; BGON / BGC Settings

; Colo(u)r Palette RAM

_SPRPAL  equ    8200h   ; Sprite palettes (K2GE)
_SCR1PAL equ    8280h   ; Scroll 1 palettes (K2GE)
_SCR2PAL equ    8300h   ; Scroll 2 palettes (K2GE)
_BGCPAL  equ    83E0h   ; Background Colo(u)r Palettes
_WCPAL   equ    83F0h   ; Window Colo(u)r Palettes

rLCR     equ    8400h   ; LED Control Register
rLFC     equ    8402h   ; LED Flash Cycle

r2DRES      equ 87E0h   ; 2D software reset (WRITE)
RESET2D_VAL equ 52h     ;  2D circuit initialization with "52" write

; Mode Selection Register

rMSR    equ     87E2h   ; Mode Selection Register (K1GE/K2GE Select)

rIPR    equ     87FEh   ; Input Port Register

; RAM Equates

_MAINRAM equ    4000h   ; Main work RAM Start Address
_SPRRAM  equ    8800h   ; Sprite VRAM Start Address
_SCR1RAM equ    9000h   ; Scroll 1 VRAM Start Address
_SCR2RAM equ    9800h   ; Scroll 2 VRAM Start Address
_TILERAM equ    0A000h  ; Sprite/Scroll Tileset Start Address

_INTROM equ     0ff0000h ; Internal ROM Start Address

; Controller Defines

PADF_D          equ     80h
PADF_OPTION     equ     40h
PADF_B          equ     20h
PADF_A          equ     10h
PADF_RIGHT      equ     08h
PADF_LEFT       equ     04h
PADF_DOWN       equ     02h
PADF_UP         equ     01h

PADB_D          equ     7
PADB_OPTION     equ     6
PADB_B          equ     5
PADB_A          equ     4
PADB_RIGHT      equ     3
PADB_LEFT       equ     2
PADB_DOWN       equ     1
PADB_UP         equ     0

; Screen Defines

SCRN_X     equ  160 ; Width of screen in pixels
SCRN_Y     equ  152 ; Height of screen in pixels
SCRN_X_B   equ  20  ; Width of screen in bytes
SCRN_Y_B   equ  19  ; Height of screen in bytes

SCRN_VX    equ  256 ; Virtual width of screen in pixels
SCRN_VY    equ  256 ; Virtual height of screen in pixels
SCRN_VX_B  equ  64  ; Virtual width of screen in bytes
SCRN_VY_B  equ  64  ; Virtual height of screen in bytes

; This macro accept RGB values from 0-255 in R,G,B Format

RGBSet: MACRO parR,parG,parB
        dw      ((parB >> 4) << 8) + ((parG >> 4) << 4) + (parR >> 4)
        ENDM

; Main RAM Definition Macros

RAMDB:  MACRO para
para    equ   START_OF_RAM
START_OF_RAM eval START_OF_RAM+1
        ENDM

RAMDW:  MACRO para
para    equ   START_OF_RAM
START_OF_RAM eval START_OF_RAM+2
        ENDM

RAMDD:  MACRO para
para    equ   START_OF_RAM
START_OF_RAM eval START_OF_RAM+4
        ENDM

