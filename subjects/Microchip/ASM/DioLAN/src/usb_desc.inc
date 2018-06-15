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
;-----------------------------------------------------------------------------
#define USE_PROD_STRING		0	; 1 - Support for Product string descriptor
;-----------------------------------------------------------------------------
#ifndef BOOTLOADER_VID
#define	BOOTLOADER_VID	0x0000  	; MUST BE VALID Vendor ID
#endif
#ifndef BOOTLOADER_PID
#define	BOOTLOADER_PID	0x0000		; MUST BE VALID Product ID
#endif

#define DSC_DEV			0x01
#define DSC_CFG			0x02
#define DSC_STR			0x03
#define DSC_INTF		0x04
#define DSC_EP			0x05

#define _DEFAULT		0x01 << 7		; Default Value (Bit 7 is set)
#define _SELF			0x01 << 6		; Self-powered (Supports if set)
#define _RWU			0x01 << 5		; Remote Wakeup (Supports if set)

#define HID_INTF		0x03
#define NO_SUBCLASS		0x00
#define BOOT_INTF_SUBCLASS	0x01

#define HID_PROTOCOL_NONE	0x00
#define HID_PROTOCOL_KEYBOAD	0x01
#define HID_PROTOCOL_MOUSE	0x02

#define _EP01_OUT		0x01
#define _EP01_IN		0x81

#define _CTRL			0x00
#define _ISO			0x01
#define _BULK			0x02
#define _INT			0x03

#define DSC_HID			0x21
#define DSC_RPT			0x22
#define DSC_PHY			0x23

#define HID_NUM_OF_DSC		0x01

#define HID_RPT01_SIZE		.23

#define CFG_ATTRIBUTES		_DEFAULT
;-----------------------------------------------------------------------------
; Descriptor sizes
#define USB_DEV_DESC_SIZE	.18
#define USB_CFG_DESC_SIZE	.9
#define USB_IF_DESC_SIZE	.9
#define USB_HID_DESC_SIZE	.9
#define USB_EP_DESC_SIZE	.7
#define USB_CFG_TOTAL_SIZE	(USB_CFG_DESC_SIZE + USB_IF_DESC_SIZE + USB_HID_DESC_SIZE + USB_EP_DESC_SIZE)
#define USB_LANG_DESC_SIZE	.4
#define USB_MFG_DESC_SIZE	.22
#if USE_PROD_STRING
#define USB_PROD_DESC_SIZE	.22
#endif
#define USB_HID_RPT_SIZE	.23
;-----------------------------------------------------------------------------
