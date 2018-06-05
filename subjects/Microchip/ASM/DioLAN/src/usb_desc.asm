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
; USB Descriptors
;-----------------------------------------------------------------------------
	#include "P18F4455.INC"
	#include "boot.inc"
	#include "usb_defs.inc"
	#include "usb_desc.inc"
;-----------------------------------------------------------------------------
; Packed code segment for USB descriptor
; We need to avoid zero padding for descriptors.
; And put descriptors after XTEA_KEY_SECTION.
USB_DESCRIPTORS	CODE_PACK	0x01C + 0x1C
;-----------------------------------------------------------------------------
	global	USB_DEV_DESC
	global	USB_DEV_DESC_end
	global	USB_CFG_DESC
	global	USB_CFG_DESC_end
	global	USB_IF_DESC
	global	USB_IF_DESC_end
	global	USB_HID_DESC
	global	USB_HID_DESC_end
	global	USB_EP_DESC
	global	USB_EP_DESC_end
	global	USB_HID_RPT
	global	USB_HID_RPT_end
	global	USB_LANG_DESC
	global	USB_LANG_DESC_end
	global	USB_MFG_DESC
	global	USB_MFG_DESC_end
#if USE_PROD_STRING
	global	USB_PROD_DESC
	global	USB_PROD_DESC_end
#endif
;-----------------------------------------------------------------------------
; USB Device Descriptor
USB_DEV_DESC
USB_DEV_DESC_bLength		db	(USB_DEV_DESC_end - USB_DEV_DESC)	; Size
USB_DEV_DESC_bDscType		db	DSC_DEV	; Descriptor type = device
USB_DEV_DESC_bcdUSB		dw	0x0200	; USB 2.0 version
USB_DEV_DESC_bDevCls		db	0x00	; Class. Specified at Interface level
USB_DEV_DESC_bDevSubCls		db	0x00	; SubClass. Specified at Interface level
USB_DEV_DESC_bDevProtocol	db	0x00	; Protocol. Specified at Interface level
USB_DEV_DESC_bMaxPktSize0	db	EP0_BUFF_SIZE
USB_DEV_DESC_idVendor		dw	BOOTLOADER_VID
USB_DEV_DESC_idProduct		dw	BOOTLOADER_PID
USB_DEV_DESC_bcdDevice		dw	FW_VER_CODE ; Device release number
USB_DEV_DESC_iMfg		db	0x01	; Manufacturer string index
USB_DEV_DESC_iProduct		db	0x02	; Product string index
USB_DEV_DESC_iSerialNum		db	0x00	; No serial number
USB_DEV_DESC_bNumCfg		db	0x01	; One configuration supported
USB_DEV_DESC_end
;-----------------------------------------------------------------------------
; Configuration Descriptor
; This descriptor includes Interface and endpoints' descriptors inplace
; Configuration 1 Descriptor
USB_CFG_DESC
USB_CFG_DESC_bLength		db	(USB_CFG_DESC_end - USB_CFG_DESC) ; Size
USB_CFG_DESC_bDscType		db	DSC_CFG		; Descriptor type = Configuration
USB_CFG_DESC_wTotalLength	dw	(USB_CFG_DESC_Total_end - USB_CFG_DESC) ; Total length of data for this configuration
USB_CFG_DESC_bNumIntf		db	1		; Number of interfaces
USB_CFG_DESC_bCfgValue		db	1		; Index value of this configuration
USB_CFG_DESC_iCfg		db	0		; Configuration string index
USB_CFG_DESC_bmAttributes	db	CFG_ATTRIBUTES	; Attributes
USB_CFG_DESC_bMaxPower		db	.50		; Max power consumption (2X mA)
USB_CFG_DESC_end
; Interface Descriptor included in configuration
USB_IF_DESC
USB_IF_DESC_bLength		db	(USB_IF_DESC_end - USB_IF_DESC)	; Size
USB_IF_DESC_bDscType		db	DSC_INTF	; Descriptor type = Interface
USB_IF_DESC_bIntfNum		db	0		; Interface Number
USB_IF_DESC_bAltSetting		db	0		; Alternate Setting Number
USB_IF_DESC_bNumEPs		db	1		; Number of endpoints
USB_IF_DESC_bIntfCls		db	HID_INTF	; HID Interface Class Code
USB_IF_DESC_bIntfSubCls		db	NO_SUBCLASS	; HID Interface Class SubClass Codes
USB_IF_DESC_bIntfProtocol	db	HID_PROTOCOL_NONE	; HID Interface Class Protocol Codes
USB_IF_DESC_iIntf		db	0		; Interface string index
USB_IF_DESC_end
; HID Class-Specific Descriptor included in configuration
USB_HID_DESC
USB_HID_DESC_bLength		db	(USB_HID_DESC_end - USB_HID_DESC)	; Size
USB_HID_DESC_bDscType		db	DSC_HID	; HID descriptor type
USB_HID_DESC_bcdHID		dw	0x0101	; HID Spec Release Number in BCD format
USB_HID_DESC_bCountryCode	db	0x00	; Country Code (0x00 for Not supported)
USB_HID_DESC_bNumDsc		db	HID_NUM_OF_DSC	; Number of class descriptors
USB_HID_DESC_HDR_bDscType	db	DSC_RPT	; Report descriptor type
USB_HID_DESC_wDscLength		dw	(USB_HID_RPT_end - USB_HID_RPT)	; Size of the report descriptor
USB_HID_DESC_end
; Endpoint Descriptors included in configuration
USB_EP_DESC
USB_EP_DESC_bLength	db	(USB_EP_DESC_end - USB_EP_DESC)	; Size
USB_EP_DESC_bDscType	db	DSC_EP			; Descriptor type = Endpoint
USB_EP_DESC_bEPAdr	db	_EP01_IN		; Endpoint Address
USB_EP_DESC_bmAttributes	db	_INT			; Endpoint type
USB_EP_DESC_wMaxPktSize	dw	HID_IN_EP_SIZE		; Endpoint size
USB_EP_DESC_bInterval	db	0x01			; Pool interval
USB_EP_DESC_end
USB_CFG_DESC_Total_end
;-----------------------------------------------------------------------------
; Report descriptor
USB_HID_RPT
	db	0x05, 0x0C		; Usage Page (Consumer devices)
	db	0x09, 0x00		; Usage (unassigned)
	db	0xA1, 0x02		; Collection (datalink)
	db	0x09, 0x00		; Usage (unassigned)
	db	0x95, HID_IN_EP_SIZE	; report count 8
	db	0x75, 0x08		; report size 8
	db	0x81, 0x00		; input
	db	0x09, 0x00		; Usage (unassigned)
	db	0x95, HID_OUT_EP_SIZE	; report count 8
	db	0x75, 0x08		; report size 8
	db	0x91, 0x00		; output
	db	0xC0			; End Collection
USB_HID_RPT_end
;-----------------------------------------------------------------------------
; String descriptors language ID Descriptor
USB_LANG_DESC
USB_LANG_DESC_bLength	db	(USB_LANG_DESC_end - USB_LANG_DESC)	; Size
USB_LANG_DESC_bDscType	db	DSC_STR	; Descriptor type = string
USB_LANG_DESC_string	dw	0x0409	; Language ID = EN_US
USB_LANG_DESC_end
;-----------------------------------------------------------------------------
; Manufacturer string Descriptor
USB_MFG_DESC
USB_MFG_DESC_bLength	db	(USB_MFG_DESC_end - USB_MFG_DESC)	; Size
USB_MFG_DESC_bDscType	db	DSC_STR	; Descriptor type = string
			dw	'D','i','o','l','a','n';,'.','c','o','m'
USB_MFG_DESC_end
;-----------------------------------------------------------------------------
; Product string Descriptor
#if USE_PROD_STRING
USB_PROD_DESC
USB_PROD_DESC_bLength	db	(USB_PROD_DESC_end - USB_PROD_DESC)	; Size
USB_PROD_DESC_bDscType	db	DSC_STR	; Descriptor type = string
			dw	'B','o','o','t','L','o','a','d','e','r'
USB_PROD_DESC_end
#endif
;-----------------------------------------------------------------------------
USB_DESC_END_SECTION
;-----------------------------------------------------------------------------
	END
