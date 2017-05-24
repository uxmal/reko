;*
;*
;* System Call Functions
;*
;* by Jeff Frohwein, 00-Apr-16
;*
;* Version 1.0 - Original release, 00-Apr-16
;* Version 1.1 - Converted from $xxx to 0xxh format, 00-May-25
;* Version 1.2 - Added more system call info, 00-May-27
;*

SYS_SUCCESS            equ     0       ; success
SYS_FAILURE            equ     0ffh    ; failure

VECT_SHUTDOWN          equ     0       ; Shutdown (Power OFF)
VECT_CLOCKGEARSET      equ     1       ; CPU operation clock setting
VECT_INTLVSET          equ     4       ; Interrupt level setting
VECT_RTCGET            equ     2       ; Real time clock - obtain time
VECT_ALARMSET          equ     9       ; Real time clock - alarm setting during game operation
VECT_ALARMDOWNSET      equ     0bh     ; Real time clock - unit start up alarm setting
VECT_SYSFONTSET        equ     5       ; System font setting
VECT_FLASHWRITE        equ     6       ; Flash memory - data write
VECT_FLASHALLERS       equ     7       ; Flash memory - erase all blocks
VECT_FLASHERS          equ     8       ; Flash memory - erase specified blocks
VECT_FLASHPROTECT      equ     0dh     ; Flash memory - protect specified blocks
VECT_GEMODESET         equ     0eh     ; Color LCD color mode setting (color version only)

INT_CLR_BIT            equ     7       ; Interrupt request clear bit

;User Boot defines

BOOTB_ALARM            equ     7       ; 0:no alarm startup   1:alarm startup
BOOTB_POWER            equ     6       ; 0:no power startup   1:power startup
BOOTB_RESUME           equ     5       ; 0:no resume startup  1:resume startup

BOOTF_ALARM            equ     80h     ; 0:no alarm startup   1:alarm startup
BOOTF_POWER            equ     40h     ; 0:no power startup   1:power startup
BOOTF_RESUME           equ     20h     ; 0:no resume startup  1:resume startup

;User Shutdown defines

SHUTDOWNB_MP           equ     7       ; power off shutdown request (0= no request, 1 = requested)
SHUTDOWNB_TIME         equ     6       ; long time no use shutdown request (0= no request, 1 = requested)
SHUTDOWNB_BAT          equ     5       ; main power use shutdown (0= no request, 1 = requested)

SHUTDOWNF_MP           equ     80h     ; power off shutdown request (0= no request, 1 = requested)
SHUTDOWNF_TIME         equ     40h     ; long time no use shutdown request (0= no request, 1 = requested)
SHUTDOWNF_BAT          equ     20h     ; main power use shutdown (0= no request, 1 = requested)

;User Answer defines

ANSWERB_RESUME         equ     7       ; resume operation (0=none, 1=exists)
ANSWERB_TIME           equ     6       ; long time no use shutdown operation (0=none, 1=exists)

ANSWERF_RESUME         equ     80h     ; resume operation (0=none, 1=exists)
ANSWERF_TIME           equ     40h     ; long time no use shutdown operation (0=none, 1=exists)

; *** Shutdown (Power OFF) ***
; Entry:
;    rw3 = VECT_SHUTDOWN
; Exit:
;    NGP is powered off.
;


; *** CPU operation clock setting ***
; Entry:
;    rw3 = VECT_CLOCKGEARSET
;    rb3 = CPU Speed Setting (0=6.144MHz, 1=3.072MHz, 2=1.536MHz, 3=768KHz, 4 = 384KHz)
;    rc3 = Auto Clock Speed Regeneration with button presses (0=no, >0=yes)
; Exit:
;    CPU speed is changed.
;


; *** Sets the interrupt level ***
; Entry:
;    rw3 = VECT_INTLVSET
;    rb3 = Interrupt Level (0-5)
;    rc3 = Interrupt # to be set (0-9)
;


; *** Real time clock - obtain time ***
; Entry:
;    rw3 = VECT_RTCGET
;    xhl3 = Pointer to RAM to return time.
; Exit:
;    Values returned in Binary Coded Decimal (BCD).
;    (Ptr+0) = Year  (00-90=2000~2090, 91-99=1991~1999)
;    (Ptr+1) = Month (1-12)
;    (Ptr+2) = Day   (1-31)
;    (Ptr+3) = Hours (0-23)
;    (Ptr+4) = Mins  (0-59)
;    (Ptr+5) = Secs  (0-59)
;    (Ptr+6) = Upper 4 bits: leap year (0-3) (0=leap year, 1 = 1 year after, etc)
;              Lower 4 bits: DayOfWeek (0-6) (0=Sun, 1=Mon, 2=Tue, 3=Wed, etc)
;


; *** Real time clock - Set Alarm ***
; Entry:
;    rw3 = VECT_ALARMSET
;    qc3 = Day    (Error if other than 1-31 or 0ffh)
;    rb3 = Hour   (Error if other than 0-23 or 0ffh)
;    rc3 = Minute (Error if other than 0-59 or 0ffh)
; Exit:
;    ra3 = SYS_SUCCESS or SYS_FAILURE
;


; *** Real time clock - Set Power ON Alarm ***
; Entry:
;    rw3 = VECT_ALARMDOWNSET
;    qc3 = Day    (Error if other than 1-31 or 0ffh)
;    rb3 = Hour   (Error if other than 0-23 or 0ffh)
;    rc3 = Minute (Error if other than 0-59 or 0ffh)
; Exit:
;    ra3 = SYS_SUCCESS or SYS_FAILURE
;


; *** System font setting ***
; Entry:
;    rw3 = VECT_SYSFONTSET
;    ra3 = Upper 4 bits: Specify palette code for transparency (0-3)
;          Lower 4 bits: Specify palette for font (0-3)
; Exit:
;    Copy 256 system font characters to Character RAM from $A000-$AFFF.
;


; *** Write data to flash memory ***
; Entry:
;    rw3 = VECT_FLASHWRITE
;    ra3 = Flash Memory Address (0=$200000, 1=$800000)
;    rbc3 = Transfer byte number (transfer in multiples of 256: 1=256, 2=512, etc)
;    xhl3 = Transfer from Address
;    xde3 = Transfer to Address
; Exit:
;    ra3 = SYS_SUCCESS if ok, other values are an error.
;


; *** Erase all blocks of flash memory (except protected areas) ***
; Entry:
;    rw3 = VECT_FLASHALLERS
;    ra3 = Flash Memory Address (0=$200000, 1=$800000)
; Exit:
;    ra3 = SYS_SUCCESS if ok, other values are an error.
;


; *** Erase block of flash memory (except protected areas) ***
; Entry:
;    rw3 = VECT_FLASHERS
;    ra3 = Flash Memory Address (0=$200000, 1=$800000)
;    rb3 = Block Number
; Exit:
;    ra3 = SYS_SUCCESS if ok, other values are an error.
;


; *** Protect blocks of flash memory ***
; Entry:
;    rw3 = VECT_FLASHPROTECT
;    ra3 = 1
;    rb3 = Start block number for protect
;    rc3 = Flash memory type (0=4MBit, 1=8MBit, 2=16MBit)
;    rd3 = Number of blocks to protect
; Exit:
;    ra3 = SYS_SUCCESS if ok, other values are an error.
;


; *** Set the display mode ***
; Entry:
;    rw3 = VECT_GEMODESET
;    ra3 = Mode setting (0=K1GE, 10h=K2GE)
; Exit:
;    Display mode is set.
;


SystemCallCode: macro
        push    sr
        ldf     3
        push    xix
        add     w,w
        add     w,w
        ld      xix,0fffe00h
        ld      xix,(xix+w)
        call    (xix)
        pop     xix
        pop     sr
        ret

        endm

; Set NGP or NGPC mode

OsVersionCode: macro
        cp      (rOSV),0
        jr      nz,OSVExit

        res     3,(6f83h)
        ld      (6da0h),0
OSVExit:
        ret

        endm
