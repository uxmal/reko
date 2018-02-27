namespace Reko.Arch.Microchip.PIC18
{

    /// <summary>
    /// Interface for PIC18 Operands' visitors.
    /// </summary>
    public interface IPIC18Operand
    {
        void Accept(IPIC18OperandVisitor visitor);
        T Accept<T>(IPIC18OperandVisitor<T> visitor);
        T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context);

    }

    /// <summary>
    /// Interface defining the permitted visitor methods on PIC18 Operands.
    /// </summary>
    public interface IPIC18OperandVisitor
    {
        void VisitImm4(PIC18Immed4Operand imm4);
        void VisitImm6(PIC18Immed6Operand imm6);
        void VisitImm8(PIC18Immed8Operand imm8);
        void VisitImm12(PIC18Immed12Operand imm12);
        void VisitImm14(PIC18Immed14Operand imm14);
        void VisitFSR2Idx(PIC18FSR2IdxOperand idx8);
        void VisitDataAbs12(PIC18Data12bitAbsAddrOperand addr12);
        void VisitDataAbs14(PIC18Data14bitAbsAddrOperand addr14);
        void VisitDataBit(PIC18DataBitAccessOperand bitno);
        void VisitDataBanked(PIC18BankedAccessOperand mem);
        void VisitDataByteWDest(PIC18DataByteAccessWithDestOperand mem);
        void VisitProgRel8(PIC18ProgRel8AddrOperand roff8);
        void VisitProgRel11(PIC18ProgRel11AddrOperand roff11);
        void VisitProgAbs(PIC18ProgAbsAddrOperand addr20);
        void VisitFSRNum(PIC18FSROperand fsrnum);
        void VisitShadow(PIC18ShadowOperand shad);
        void VisitTblRW(PIC18TableReadWriteOperand tblmode);
        void VisitEEPROM(PIC18DataEEPROMOperand eeprom);
        void VisitASCII(PIC18DataASCIIOperand eeprom);
        void VisitDB(PIC18DataByteOperand bytes);
        void VisitDW(PIC18DataWordOperand words);
        void VisitIDLocs(PIC18IDLocsOperand idlocs);
        void VisitConfig(PIC18ConfigOperand config);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions on PIC18 Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    public interface IPIC18OperandVisitor<T>
    {
        T VisitImm4(PIC18Immed4Operand imm4);
        T VisitImm6(PIC18Immed6Operand imm6);
        T VisitImm8(PIC18Immed8Operand imm8);
        T VisitImm12(PIC18Immed12Operand imm12);
        T VisitImm14(PIC18Immed14Operand imm14);
        T VisitFSR2Idx(PIC18FSR2IdxOperand idx8);
        T VisitDataAbs12(PIC18Data12bitAbsAddrOperand addr12);
        T VisitDataAbs14(PIC18Data14bitAbsAddrOperand addr14);
        T VisitDataBit(PIC18DataBitAccessOperand bitno);
        T VisitDataBanked(PIC18BankedAccessOperand mem);
        T VisitDataByteWDest(PIC18DataByteAccessWithDestOperand mem);
        T VisitProgRel8(PIC18ProgRel8AddrOperand roff8);
        T VisitProgRel11(PIC18ProgRel11AddrOperand roff11);
        T VisitProgAbs(PIC18ProgAbsAddrOperand addr20);
        T VisitFSRNum(PIC18FSROperand fsrnum);
        T VisitShadow(PIC18ShadowOperand shad);
        T VisitTblRW(PIC18TableReadWriteOperand tblmode);
        T VisitEEPROM(PIC18DataEEPROMOperand eeprom);
        T VisitASCII(PIC18DataASCIIOperand eeprom);
        T VisitDB(PIC18DataByteOperand bytes);
        T VisitDW(PIC18DataWordOperand words);
        T VisitIDLocs(PIC18IDLocsOperand idlocs);
        T VisitConfig(PIC18ConfigOperand config);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions with context on PIC18 Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    /// <typeparam name="C">Generic type parameter of the context.</typeparam>
    public interface IPIC18OperandVisitor<T, C>
    {
        T VisitImm4(PIC18Immed4Operand imm4, C context);
        T VisitImm6(PIC18Immed6Operand imm6, C context);
        T VisitImm8(PIC18Immed8Operand imm8, C context);
        T VisitImm12(PIC18Immed12Operand imm12, C context);
        T VisitImm14(PIC18Immed14Operand imm14, C context);
        T VisitFSR2Idx(PIC18FSR2IdxOperand idx8, C context);
        T VisitDataAbs12(PIC18Data12bitAbsAddrOperand addr12, C context);
        T VisitDataAbs14(PIC18Data14bitAbsAddrOperand addr14, C context);
        T VisitDataBit(PIC18DataBitAccessOperand bitno, C context);
        T VisitDataBanked(PIC18BankedAccessOperand mem, C context);
        T VisitDataByteWDest(PIC18DataByteAccessWithDestOperand mem, C context);
        T VisitProgRel8(PIC18ProgRel8AddrOperand roff8, C context);
        T VisitProgRel11(PIC18ProgRel11AddrOperand roff11, C context);
        T VisitProgAbs(PIC18ProgAbsAddrOperand addr20, C context);
        T VisitFSRNum(PIC18FSROperand fsrnum, C context);
        T VisitShadow(PIC18ShadowOperand shad, C context);
        T VisitTblRW(PIC18TableReadWriteOperand tblmode, C context);
        T VisitEEPROM(PIC18DataEEPROMOperand eeprom, C context);
        T VisitASCII(PIC18DataASCIIOperand eeprom, C context);
        T VisitDB(PIC18DataByteOperand bytes, C context);
        T VisitDW(PIC18DataWordOperand words, C context);
        T VisitIDLocs(PIC18IDLocsOperand idlocs, C context);
        T VisitConfig(PIC18ConfigOperand config, C context);
    }

}
