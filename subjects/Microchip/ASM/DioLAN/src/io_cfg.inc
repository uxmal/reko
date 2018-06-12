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
; I/O configuration
;-----------------------------------------------------------------------------
; Constants
;-----------------------------------------------------------------------------
; LED
#define USE_LED			1	; Undef/comment if you not want to use FW LED
#ifdef	USE_LED
#define LED			LATE
#define LED_TRIS		TRISE
#define LED_PIN			0
#endif
;-----------------------------------------------------------------------------
; Jumpers
#define USE_JP_BOOTLOADER_EN	1	; Undef/comment if you not want to use FW Jumper
#ifdef	USE_JP_BOOTLOADER_EN
#define JP_BOOTLOADER_TRIS	TRISE
#define JP_BOOTLOADER_PORT	PORTE
#define JP_BOOTLOADER_PIN	0	
#endif
;-----------------------------------------------------------------------------
