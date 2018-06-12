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
; Device Reset Vectors
;-----------------------------------------------------------------------------
	#include "P18F4455.INC"
	#include "boot.inc"
	#include "io_cfg.inc"
;-----------------------------------------------------------------------------
; Externals
;-----------------------------------------------------------------------------
	extern	main  
	extern	bootloader_soft_reset
;-----------------------------------------------------------------------------
; START
;-----------------------------------------------------------------------------
VECTORS		CODE	0x0000
	;--- RESET Vector
	org	0x0000
	clrf	TBLPTRU
	clrf	TBLPTRH
	bra		pre_main
;-----------------------------------------------------------------------------
;--- HIGH Interrupt Vector
	org	0x0008
	goto	APP_HIGH_INTERRUPT_VECTOR
;-----------------------------------------------------------------------------
pre_main
	; All I/O to Digital mode
	movlw	0x0F
	movwf	ADCON1
	bra		main
;-----------------------------------------------------------------------------
; Here is 4 bytes (2 program words) free
; in address range from 0x012 to 0x015 inclusive.
; Can be used for something short for size optimization.
;-----------------------------------------------------------------------------
;--- BOOTLOADER External Entry Point                         
	org	0x0016
 if USE_EEPROM_MARK 
	bra	bootloader_soft_reset
 endif        
        
        ;--- HIGH Interrupt Vector
	org	0x0018
	goto	APP_LOW_INTERRUPT_VECTOR
;-----------------------------------------------------------------------------
; APPLICATION STUB
;-----------------------------------------------------------------------------
APPSTRT CODE APP_RESET_VECTOR
	bra	$
;-----------------------------------------------------------------------------
	END
