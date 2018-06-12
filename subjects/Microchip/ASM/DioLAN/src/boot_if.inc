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
; UEPn Initialization Parameters
;-----------------------------------------------------------------------------
#define BOOT_CMD_SIZE	.64
#ifdef HID_OUT_EP_SIZE
#if HID_OUT_EP_SIZE < BOOT_CMD_SIZE
	error	"HID_OUT and CMD size missmatch: HID_OUT_EP_SIZE < BOOT_CMD_SIZE"
#endif
#endif

#define BOOT_REP_SIZE	.64
#ifdef HID_IN_EP_SIZE
#if HID_IN_EP_SIZE < BOOT_CMD_SIZE
	error	"HID_IN and CMD size missmatch: HID_IN_EP_SIZE<BOOT_REP_SIZE"
#endif
#endif
;-----------------------------------------------------------------------------
; Command
#define cmd	0
; To change command you have to fix 
; hid_process_cmd function in boot_asm.asm
#define BOOT_READ_FLASH 	0x01
#define BOOT_WRITE_FLASH	0x02
#define BOOT_ERASE_FLASH	0x03
#define BOOT_GET_FW_VER		0x04
#define BOOT_RESET     		0x05
#define	BOOT_READ_ID		0x06
#define	BOOT_WRITE_ID		0x07
#define	BOOT_READ_EEPROM	0x08
#define	BOOT_WRITE_EEPROM	0x09
#define	BOOT_CMD_UNKNOWN	0xFF

#define BOOT_ID_LEN		8

; ID is used to link between command and reply 
; Reply must contain the same id as in related command
#define id	1
#define cmdata	2
;-----------------------------------------------------------------------------
