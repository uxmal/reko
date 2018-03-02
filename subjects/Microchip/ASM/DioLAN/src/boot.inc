;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;  BootLoader.                                                             ;;
;;  Copyright (C) 2007 Diolan ( http://www.diolan.com )                     ;;
;;                                                                          ;;
;;  This program is free software: you can redistribute it and/or modify    ;;
;;  it under the terms of the GNU General Public License as published by    ;;
;;  the Free Software Foundation, either version 3 of the License, or       ;;
;;  (at your option) any later version.                                     ;;
;;                                                                          ;;
;;  This program is distributed in the hope that it will be useful,         ;;
;;  but WITHOUT ANY WARRANTY; without even the implied warranty of          ;;
;;  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the           ;;
;;  GNU General Public License for more details.                            ;;
;;                                                                          ;;
;;  You should have received a copy of the GNU General Public License       ;;
;;  along with this program.  If not, see <http://www.gnu.org/licenses/>    ;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Main definitions
;-----------------------------------------------------------------------------
; Bootloader Version
#define FW_VER_MAJOR		0
#define FW_VER_MINOR		2
#define FW_VER_SUB_MINOR	2
#define FW_VER_CODE		((FW_VER_MAJOR << 8) | (FW_VER_MINOR << 4) | (FW_VER_SUB_MINOR))
;-----------------------------------------------------------------------------
; Application FW RESET and INTERRUPT VECTORS
#define	APP_RESET_VECTOR       		0x000800
#define	APP_HIGH_INTERRUPT_VECTOR	0x000808
#define	APP_LOW_INTERRUPT_VECTOR	0x000818
;-----------------------------------------------------------------------------
; CPU Power & Speed
; If CPU_5V_HS is 1 then CPU run from 48 MHz PLL clock. REQUIRE 5V Power Supply
; If CPU_5V_HS is 0 then CPU run from 16 MHz OSC directly. Possible 3.3V Power Supply
#define CPU_5V_HS		1
;-----------------------------------------------------------------------------
; EEPROM Mark to run bootloader
#define USE_EEPROM_MARK		1	; Enable EEPROM MARK. To disable = 0
#define EEPROM_MARK_ADDR	0x00	; EEPROM Address for MARK
#define EEPROM_MARK		0x5A	; MARK value
;-----------------------------------------------------------------------------
; EEPROM and ID encoding by XTEA support
#define ENCODE_EEPROM		0	; Disable EEPROM encoding. To enable = 1
#define ENCODE_ID		0	; Disable ID encoding. To enable = 1
;-----------------------------------------------------------------------------
; Debug messages over UART
#define UART_DEBUG	0
#if UART_DEBUG
UD_INIT	macro
	clrf	TXSTA
	clrf	RCSTA
	clrf	BAUDCON
	bsf	TXSTA, BRGH
	movlw	((.48000000 / .16 / .115200) - 1)	; .25 = 115200
	movwf	SPBRG
	bsf	RCSTA, SPEN
	bsf	TXSTA, TXEN
	endm
UD_TX	macro	BYTE
	btfss	PIR1, TXIF
	bra	$ - 2
	movlw	BYTE
	movwf	TXREG
	bcf	PIR1, TXIF
	endm
UD_TXB	macro	BYTE
	btfss	PIR1, TXIF
	bra	$ - 2
	movf	BYTE, W
	addlw	0x30
	movwf	TXREG
	bcf	PIR1, TXIF
	endm
#else
UD_INIT	macro
	endm
UD_TX	macro	BYTE
	endm
UD_TXB	macro	BYTE
	endm
#endif ; UART_DEBUG
;-----------------------------------------------------------------------------
