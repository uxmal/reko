VID		?= 0x0000
PID		?= 0x0000
XTEA_KEY	?= "ABCDEFGHIJKLMNOP"

PROJECT		= bootloader
PROCESSOR	= 18f4455

SRCS		= boot.asm boot_asm.asm usb.asm usb_desc.asm vectors.asm xtea.asm
INCS		= boot_if.inc boot.inc io_cfg.inc usb_defs.inc usb_desc.inc usb.inc P18F4455.INC
OBJS		= $(foreach f, $(SRCS), $(basename $(f)).o )

COF		= $(PROJECT).cof
HEX		= $(PROJECT).hex
MAP		= $(PROJECT).map

OBJ_DIR		= _output

MPLAB_DIR	= "C:\Program Files\Microchip\MPASM Suite\"
ASM		= mpasmwin.exe
LINK		= mplink.exe
MP2HEX		= mp2hex.exe
LKR		= $(PROJECT).lkr

ASMFLAGS	=

ifneq ($(VID), "")
ASMFLAGS += /dBOOTLOADER_VID=$(VID)
endif

ifneq ($(PID), "")
ASMFLAGS += /dBOOTLOADER_PID=$(PID)
endif

ifneq ($(XTEA_KEY), "")
ASMFLAGS += /dXTEA_KEY=$(XTEA_KEY)
endif

all: $(HEX)

$(HEX): $(COF)
	$(MP2HEX) $(COF)

$(COF): $(OBJS)
	$(LINK) $(LKR) $(OBJS) /m$(MAP) /o$@

%.o: %.asm $(INCS)
	$(ASM) /q /p$(PROCESSOR) $< /e /l /y /o$@ $(ASMFLAGS)

clean:
	rm -f *.o *.ERR *.LST *.map *.cod *.cof *.hex
	rm -rf $(OBJ_DIR)

print:
	@echo "SRCS = $(SRCS)"
	@echo "INCS = $(INCS)"
	@echo "OBJS = $(OBJS)"

.PHONY: all clean print
