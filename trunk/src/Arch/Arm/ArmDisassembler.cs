using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class OpGroup
        {
            public BitRange [] Selectors;
            public OpRec [] oprecs;
        }

        public struct BitRange {
            public uint bitPos;
            public uint mask;
        }

    public class OpRec
    {
        public Opcode op;
        public string format;
        public int mask;
        public OpGroup Group;
    }

    class ArmDisassembler
    {
        public void Decode(uint instr)
        {
            uint cond = instr>>28;
            if (cond == 0xF)
            {
                //Unconditional();
            }
            else
            {
                uint sel = ((instr >> 23) & 0x0E) | ((instr >> 4) & 1);
                var oprec = firstLevel.Selectors[sel];
            }
        }

 
        
        public static OpGroup firstLevel = new OpGroup
        {
            Selectors = new BitRange[]{
                new BitRange { bitPos=28, mask=0xF }
            },

            oprecs = new OpRec[]{ 
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = predicated },
                new OpRec { Group = nonpredicated },

            }
        };

        public static OpGroup predicated = new OpGroup
        {
            Selectors = {},
            oprecs = new OpRec[]{

            new OpRec { Group = DataMisc },
            new OpRec { Group=  DataMisc },
            new OpRec { Group=  DataMisc },
            new OpRec { Group = DataMisc },

            new OpRec { Group = LdStWord },
            new OpRec { Group = LdStWord },
            new OpRec { Group = LdStWord },
            new OpRec { Group = Media },

            new OpRec { Group = BranchBlockTransfer },
            new OpRec { Group = BranchBlockTransfer},
            new OpRec { Group = BranchBlockTransfer},
            new OpRec { Group = BranchBlockTransfer},

            new OpRec { Group = SysCoproc },
            new OpRec { Group = SysCoproc },
            new OpRec { Group = SysCoproc },
            new OpRec { Group = SysCoproc },
            }
        };

        public static OpGroup DataMisc = new OpGroup
        {
            
        };

                public static OpGroup LdStWord = new OpGroup
        {
        };
                public static OpGroup Media = new OpGroup
        {
        };

                public static OpGroup BranchBlockTransfer = new OpGroup
        {
        };

                public static OpGroup SysCoproc = new OpGroup
        {
        };

        public static OpGroup nonpredicated = new OpGroup
        {
            Selectors = new BitRange[] { new BitRange{ bitPos = 27, mask = 1 },},
                oprecs = new OpRec[]
                {
                    new OpRec{ Group = NonpMisc },
                    new OpRec { Group = nonp2 } 
                }
        };

        public static OpGroup nonp2 = new OpGroup
        {
            Selectors = new BitRange[] { new BitRange { bitPos = 24, mask =0x7}, },
            oprecs = new OpRec[]
            {
                new OpRec { op= Opcode.srs, },
                new OpRec { op= Opcode.srs, }
            }
        };

        public static OpGroup NonpMisc = new OpGroup
        {
        };



        public static OpRec[] ops = new OpRec[] {
            // 00-0F
                new OpRec{ op = Opcode.and, format="lli"},
                new OpRec{ op = Opcode.and, format="llr"},
                new OpRec{ op = Opcode.and, format="lri"},
                new OpRec{ op = Opcode.and, format="lrr"},
                new OpRec{ op = Opcode.and, format="ari"},
                new OpRec{ op = Opcode.and, format="arr"},
                new OpRec{ op = Opcode.and, format="rri"},
                new OpRec{ op = Opcode.and , format="rrr"},

                new OpRec{ op = Opcode.and , format="lli"},
                new OpRec { op= Opcode.mul, format="RdRnRsRm" },
                new OpRec{ op = Opcode.and , format="lri"},
                new OpRec(),
                new OpRec(),
                new OpRec(),
                new OpRec(),
                new OpRec(),
            
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // 10-1F
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // 20-2F
            new OpRec{op=Opcode.and,  format="ndRi"  },
            new OpRec{op=Opcode.ands, format="ndRi" },
            new OpRec{op=Opcode.eor,  format="ndRi"  },
            new OpRec{op=Opcode.eors, format="ndRi" },
            new OpRec{op=Opcode.sub,  format="ndRi"  },
            new OpRec{op=Opcode.subs, format="ndRi" },
            new OpRec{op=Opcode.rsb,  format="ndRi"  },
            new OpRec{op=Opcode.rsbs, format="ndRi" },

            new OpRec{op=Opcode.add,  format="ndRi"  },
            new OpRec{op=Opcode.adds, format="ndRi" },
            new OpRec{op=Opcode.adc,  format="ndRi"  },
            new OpRec{op=Opcode.adcs, format="ndRi" },
            new OpRec{op=Opcode.sbc,  format="ndRi"  },
            new OpRec{op=Opcode.sbcs, format="ndRi" },
            new OpRec{op=Opcode.rsc,  format="ndRi"  },
            new OpRec{op=Opcode.rscs, format="ndRi" },

            // 30-3F
            new OpRec{op=Opcode.tst,  format="ndRi"  },
            new OpRec{op=Opcode.tsts, format="ndRi" },
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // 40-4F
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // 50-5F
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // 60-6F
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // 70-7F
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // 80-8F
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // 90-9F
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // A0-AF
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},

            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},
            new OpRec{ op = Opcode.b, format="O"},


            // B0-BF
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},

            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},
            new OpRec{ op = Opcode.bl, format="O"},



            // C0-CF
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // D0-DF
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // E0-EF
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            // F0-FF
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),
            new OpRec(),

        };


    // 00-01 - cruft
    // 20-5F - rows
    // 60-7F - striped rows
    // 80-9F - rows
    // A0-AF - B
    // B0-BF - BL
    // C0-DF - rows
    // E0-EF - crucft
    // F0-FF - SWI
}
}
/*
http://imrannazar.com/ARM-Opcode-Map
 * <!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
 <head>
  <title>Imran Nazar: ARM Opcode Map</title>
  <meta http-equiv="description" content="There are many places in which you can look if you want an explanation of the instructions available on the ARM series of processor cores. However, there is no overview of the instruction set in the form of a table or map; not even the official ARM instruction reference provides this anywhere in its 811 pages. I produced an opcode map in 2006, in an attempt to rectify this problem: it provides a mapping of all the instructions and addressing modes for cores up to ARM version 4, with version 5 extensions highlighted in blue and the DSP extensions in green.">
  <link href="/css/index.css" type="text/css" rel="stylesheet">
  <link href="/css/content.css" type="text/css" rel="stylesheet">
  <link rel='stylesheet' type='text/css' href='/content/css/opcode-map.css'>
  <link rel="alternate" type="application/rss+xml"  href="/rss.xml" title="ImranNazar.com">
  <style type="text/css">
#head h1 { filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=crop,src='http://imrannazar.com/content/titles/opcode-map.png'); }
#head > h1 { background: url(http://imrannazar.com/content/titles/opcode-map.png) no-repeat top left; }
  </style>
 </head>
 <body>
  <div id="wrapper">
   <div id="head">
    <h1><a href="http://imrannazar.com/">Imran Nazar</a>: ARM Opcode Map</h1>
    <a id="navlink" href="#nav">Skip to navigation</a>
   </div>
   <div id="container">
    <div id="content">
  <p>The following is a full opcode map of instructions for the ARM7 and ARM9 series of CPU cores. Instructions added for ARM9 are highlighted in blue, and instructions specific to the M-extension are shown in green. The Thumb instruction set is also included, in Table 2.</p>
  <div id="opcodemaps">
  <table>
   <caption>Table 1. ARM Opcode Map.</caption>
   <thead>
    <tr>
     <th rowspan='2'>Bits<br>27-20</th>
     <th colspan='16'>Bits 7-4</th>
    </tr>
    <tr>
     <th>0</th><th>1</th><th>2</th><th>3</th>
     <th>4</th><th>5</th><th>6</th><th>7</th>
     <th>8</th><th>9</th><th>A</th><th>B</th>
     <th>C</th><th>D</th><th>E</th><th>F</th>
    </tr>
   </thead>
   <tbody>
<tr>
 <td class='bit'>00</td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical And'>AND</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical And'>AND</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical And'>AND</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical And'>AND</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Multiply registers'>MUL</span></td>
 <td><span title='Logical And'>AND</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical And'>AND</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical And'>AND</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, post-decrement'>ptrm</span></td>
</tr>
<tr>
 <td class='bit'>01</td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Multiply registers, setting flags'>MULS</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical And, setting flags'>ANDS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
</tr>
<tr>
 <td class='bit'>02</td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Multiply and accumulate registers'>MLA</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, post-decrement'>ptrm</span></td>
</tr>
<tr>
 <td class='bit'>03</td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Multiply and accumulate registers, setting flags'>MLAS</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, post-decrement'>ptrm</span></td>
 <td><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, post-decrement'>ptrm</span></td>
</tr>
<tr>
 <td class='bit'>04</td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>05</td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract, setting flags'>SUBS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>06</td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Subtract register from value'>RSB</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>07</td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
 <td><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>08</td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Unsigned long multiply (32x32 to 64)'>UMULL</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register'>ADD</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, post-increment'>ptrp</span></td>
</tr>
<tr>
 <td class='bit'>09</td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Unsigned long multiply, setting flags'>UMULLS</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register, setting flags'>ADDS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, post-increment'>ptrp</span></td>
</tr>
<tr>
 <td class='bit'>0A</td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Unsigned long multiply and accumulate'>UMLAL</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register with carry'>ADC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, post-increment'>ptrp</span></td>
</tr>
<tr>
 <td class='bit'>0B</td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Unsigned long multiply and accumulate, setting flags'>UMLALS</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, post-increment'>ptrp</span></td>
 <td><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, post-increment'>ptrp</span></td>
</tr>
<tr>
 <td class='bit'>0C</td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Signed long multiply (32x32 to 64)'>SMULL</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract from register with borrow'>SBC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>0D</td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Signed long multiply, setting flags'>SMULLS</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>0E</td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Signed long multiply and accumulate'>SMLAL</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract register from value with borrow'>RSC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>0F</td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Signed long multiply and accumulate, setting flags'>SMLALS</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, post-increment'>ptip</span></td>
 <td><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>10</td>
 <td><span title='Move status word to register'>MRS</span> <span title='Register, CPSR'>rc</span></td>
 <td class='und' colspan='4'></td>
 <td class='edsp'><span title='Saturated add'>QADD</span></td>
 <td class='und' colspan='2'></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with bottom-half of second, and accumulate'>SMLABB</span></td>
 <td><span title='Swap registers with memory word'>SWP</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with bottom-half of second, and accumulate'>SMLATB</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Negative register offset'>ofrm</span></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with top-half of second, and accumulate'>SMLABT</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Negative register offset'>ofrm</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with top-half of second, and accumulate'>SMLATT</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Negative register offset'>ofrm</span></td>
</tr>
<tr>
 <td class='bit'>11</td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Negative register offset'>ofrm</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Negative register offset'>ofrm</span></td>
 <td><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Negative register offset'>ofrm</span></td>
</tr>
<tr>
 <td class='bit'>12</td>
 <td><span title='Move value to status word'>MSR</span> <span title='Register, CPSR'>rc</span></td>
 <td><span title='Branch and switch execution modes'>BX</span></td>
 <td class='und'></td>
 <td class='arm9'><span title='Branch, link and switch execution modes '>BLX</span> <span title='Register offset'>reg</span></td>
 <td class='und'></td>
 <td class='edsp'><span title='Saturated subtract'>QSUB</span></td>
 <td class='und'></td>
 <td class='arm9'><span title='Software breakpoint'>BKPT</span></td>
 <td class='edsp'><span title='Signed multiply first operand with bottom-half of second operand, keeping top 32 bits, and accumulate'>SMLAWB</span></td>
 <td class='und'></td>
 <td class='edsp'><span title='Signed multiply first operand with bottom-half of second operand, keeping top 32 bits'>SMULWB</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, pre-decrement'>prrm</span></td>
 <td class='edsp'><span title='Signed multiply first operand with top-half of second operand, keeping top 32 bits, and accumulate'>SMLAWT</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, pre-decrement'>prrm</span></td>
 <td class='edsp'><span title='Signed multiply first operand with top-half of second operand, keeping top 32 bits'>SMULWT</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, pre-decrement'>prrm</span></td>
</tr>
<tr>
 <td class='bit'>13</td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, pre-decrement'>prrm</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, pre-decrement'>prrm</span></td>
 <td><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, pre-decrement'>prrm</span></td>
</tr>
<tr>
 <td class='bit'>14</td>
 <td><span title='Move status word to register'>MRS</span> <span title='Register, SPSR'>rs</span></td>
 <td class='und' colspan='4'></td>
 <td class='edsp'><span title='Saturated add with doubling of second operand'>QDADD</span></td>
 <td class='und' colspan='2'></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with bottom-half of second, and 64-bit accumulate'>SMLALBB</span></td>
 <td><span title='Swap registers with memory byte'>SWPB</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with bottom-half of second, and 64-bit accumulate'>SMLALTB</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Negative immediate offset'>ofim</span></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with top-half of second, and 64-bit accumulate'>SMLALBT</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Negative immediate offset'>ofim</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with top-half of second, and 64-bit accumulate'>SMLALTT</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>15</td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Negative immediate offset'>ofim</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Negative immediate offset'>ofim</span></td>
 <td><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>16</td>
 <td><span title='Move value to status word'>MSR</span> <span title='Register, SPSR'>rs</span></td>
 <td class='arm9'><span title='Count leading zeros in register'>CLZ</span></td>
 <td class='und' colspan='3'></td>
 <td class='edsp'><span title='Saturated subtract with doubling of second operand'>QDSUB</span></td>
 <td class='und' colspan='2'></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with bottom-half of second'>SMULBB</span></td>
 <td class='und'></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with bottom-half of second'>SMULTB</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
 <td class='edsp'><span title='Signed multiply bottom-half of first operand with top-half of second'>SMULBT</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
 <td class='edsp'><span title='Signed multiply top-half of first operand with top-half of second'>SMULTT</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>17</td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
 <td><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>18</td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Logical Or'>ORR</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Positive register offset'>ofrp</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Positive register offset'>ofrp</span></td>
 <td><span title='Logical Or'>ORR</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Positive register offset'>ofrp</span></td>
</tr>
<tr>
 <td class='bit'>19</td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Positive register offset'>ofrp</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Positive register offset'>ofrp</span></td>
 <td><span title='Logical Or, setting flags'>ORRS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Positive register offset'>ofrp</span></td>
</tr>
<tr>
 <td class='bit'>1A</td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Register offset, pre-increment'>prrp</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Register offset, pre-increment'>prrp</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Register offset, pre-increment'>prrp</span></td>
</tr>
<tr>
 <td class='bit'>1B</td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Register offset, pre-increment'>prrp</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Register offset, pre-increment'>prrp</span></td>
 <td><span title='Move value to a register, setting flags'>MOVS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Register offset, pre-increment'>prrp</span></td>
</tr>
<tr>
 <td class='bit'>1C</td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Positive immediate offset'>ofip</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Positive immediate offset'>ofip</span></td>
 <td><span title='Clear bits in register (NAND)'>BIC</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>1D</td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Positive immediate offset'>ofip</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Positive immediate offset'>ofip</span></td>
 <td><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>1E</td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Store halfword'>STRH</span> <span title='Immediate offset, pre-increment'>prip</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td class='edsp'><span title='Load doubleword'>LDRD</span> <span title='Immediate offset, pre-increment'>prip</span></td>
 <td><span title='Move negation of value to a register'>MVN</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td class='edsp'><span title='Store doubleword'>STRD</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>1F</td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-left by register'>llr</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-right by register'>lrr</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Arithmetic shift-right by register'>arr</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Rotate right by register'>rrr</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-left by immediate'>lli</span></td>
 <td class='und'></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Logical shift-right by immediate'>lri</span></td>
 <td><span title='Load halfword'>LDRH</span> <span title='Immediate offset, pre-increment'>prip</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Arithmetic shift-right by immediate'>ari</span></td>
 <td><span title='Load signed byte'>LDRSB</span> <span title='Immediate offset, pre-increment'>prip</span></td>
 <td><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Rotate right by immediate, or rotate right with extend (RRX)'>rri</span></td>
 <td><span title='Load signed halfword'>LDRSH</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>20</td>
 <td colspan='16'><span title='Logical And'>AND</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>21</td>
 <td colspan='16'><span title='Logical And, setting flags'>ANDS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>22</td>
 <td colspan='16'><span title='Logical Exclusive-or'>EOR</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>23</td>
 <td colspan='16'><span title='Logical Exclusive-or, setting flags'>EORS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>24</td>
 <td colspan='16'><span title='Subtract from register'>SUB</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>25</td>
 <td colspan='16'><span title='Subtract, setting flags'>SUBS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>26</td>
 <td colspan='16'><span title='Subtract register from value'>RSB</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>27</td>
 <td colspan='16'><span title='Reverse Subtract, setting flags'>RSBS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>28</td>
 <td colspan='16'><span title='Add to register'>ADD</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>29</td>
 <td colspan='16'><span title='Add to register, setting flags'>ADDS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2A</td>
 <td colspan='16'><span title='Add to register with carry'>ADC</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2B</td>
 <td colspan='16'><span title='Add to register with carry, setting flags'>ADCS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2C</td>
 <td colspan='16'><span title='Subtract from register with borrow'>SBC</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2D</td>
 <td colspan='16'><span title='Subtract from register with borrow, setting flags'>SBCS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2E</td>
 <td colspan='16'><span title='Subtract register from value with borrow'>RSC</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>2F</td>
 <td colspan='16'><span title='Subtract register from value with borrow, setting flags'>RSCS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>30</td>
 <td class='und' colspan='16'></td>
</tr>
<tr>
 <td class='bit'>31</td>
 <td colspan='16'><span title='Test bits in register (Logical And), setting flags'>TSTS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>32</td>
 <td colspan='16'><span title='Move value to status word'>MSR</span> <span title='Immediate, CPSR'>ic</span></td>
</tr>
<tr>
 <td class='bit'>33</td>
 <td colspan='16'><span title='Test equivalence of bits in register (Logical Exclusive-or), setting flags'>TEQS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>34</td>
 <td class='und' colspan='16'></td>
</tr>
<tr>
 <td class='bit'>35</td>
 <td colspan='16'><span title='Compare register to value (Subtract), setting flags'>CMPS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>36</td>
 <td colspan='16'><span title='Move value to status word'>MSR</span> <span title='Immediate, SPSR'>is</span></td>
</tr>
<tr>
 <td class='bit'>37</td>
 <td colspan='16'><span title='Compare register to negation of value (Add), setting flags'>CMNS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>38</td>
 <td colspan='16'><span title='Logical Or'>ORR</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>39</td>
 <td colspan='16'><span title='Logical Or, setting flags'>ORRS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3A</td>
 <td colspan='16'><span title='Move value to a register'>MOV</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3B</td>
 <td colspan='16'><span title='Move value to a register, setting flags'>MOVS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3C</td>
 <td colspan='16'><span title='Clear bits in register (NAND)'>BIC</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3D</td>
 <td colspan='16'><span title='Clear bits in register (NAND), setting flags'>BICS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3E</td>
 <td colspan='16'><span title='Move negation of value to a register'>MVN</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>3F</td>
 <td colspan='16'><span title='Move negation of value to a register, setting flags'>MVNS</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>40</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>41</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>42</td>
 <td colspan='16'><span title='Store word from user-mode register'>STRT</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>43</td>
 <td colspan='16'><span title='Load word into user-mode register'>LDRT</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>44</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>45</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>46</td>
 <td colspan='16'><span title='Store byte from user-mode register'>STRBT</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>47</td>
 <td colspan='16'><span title='Load byte into user-mode register'>LDRBT</span> <span title='Immediate offset, post-decrement'>ptim</span></td>
</tr>
<tr>
 <td class='bit'>48</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>49</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4A</td>
 <td colspan='16'><span title='Store word from user-mode register'>STRT</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4B</td>
 <td colspan='16'><span title='Load word into user-mode register'>LDRT</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4C</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4D</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4E</td>
 <td colspan='16'><span title='Store byte from user-mode register'>STRBT</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>4F</td>
 <td colspan='16'><span title='Load byte into user-mode register'>LDRBT</span> <span title='Immediate offset, post-increment'>ptip</span></td>
</tr>
<tr>
 <td class='bit'>50</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>51</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>52</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>53</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>54</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>55</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Negative immediate offset'>ofim</span></td>
</tr>
<tr>
 <td class='bit'>56</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>57</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Immediate offset, pre-decrement'>prim</span></td>
</tr>
<tr>
 <td class='bit'>58</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>59</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>5A</td>
 <td colspan='16'><span title='Store word'>STR</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>5B</td>
 <td colspan='16'><span title='Load word'>LDR</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>5C</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>5D</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Positive immediate offset'>ofip</span></td>
</tr>
<tr>
 <td class='bit'>5E</td>
 <td colspan='16'><span title='Store byte'>STRB</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>5F</td>
 <td colspan='16'><span title='Load byte'>LDRB</span> <span title='Immediate offset, pre-increment'>prip</span></td>
</tr>
<tr>
 <td class='bit'>60</td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>61</td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>62</td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>63</td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>64</td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>65</td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>66</td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>67</td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Left-shifted register offset, post-decrement'>ptrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-shifted register offset, post-decrement'>ptrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Arithmetic-right-shifted register offset, post-decrement'>ptrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-rotated register offset, post-decrement'>ptrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>68</td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>69</td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6A</td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word from user-mode register'>STRT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6B</td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word into user-mode register'>LDRT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6C</td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6D</td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6E</td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte from user-mode register'>STRBT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>6F</td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Left-shifted register offset, post-increment'>ptrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-shifted register offset, post-increment'>ptrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Arithmetic-right-shifted register offset, post-increment'>ptrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte into user-mode register'>LDRBT</span> <span title='Right-rotated register offset, post-increment'>ptrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>70</td>
 <td><span title='Store word'>STR</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>71</td>
 <td><span title='Load word'>LDR</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>72</td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>73</td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>74</td>
 <td><span title='Store byte'>STRB</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>75</td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative left-shifted register offset'>ofrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative right-shifted register offset'>ofrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative arithmetic-right-shifted register offset'>ofrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Negative right-rotated register offset'>ofrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>76</td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>77</td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, pre-decrement'>prrmll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, pre-decrement'>prrmlr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, pre-decrement'>prrmar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, pre-decrement'>prrmrr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>78</td>
 <td><span title='Store word'>STR</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>79</td>
 <td><span title='Load word'>LDR</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7A</td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Store word'>STR</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7B</td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Load word'>LDR</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7C</td>
 <td><span title='Store byte'>STRB</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7D</td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive left-shifted register offset'>ofrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive right-shifted register offset'>ofrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive arithmetic-right-shifted register offset'>ofrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Positive right-rotated register offset'>ofrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7E</td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Store byte'>STRB</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>7F</td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Left-shifted register offset, pre-increment'>prrpll</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-shifted register offset, pre-increment'>prrplr</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Arithmetic-right-shifted register offset, pre-increment'>prrpar</span></td>
 <td class='und'></td>
 <td><span title='Load byte'>LDRB</span> <span title='Right-rotated register offset, pre-increment'>prrprr</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>80</td>
 <td colspan='16'><span title='Store multiple words, decrement after'>STMDA</span></td>
</tr>
<tr>
 <td class='bit'>81</td>
 <td colspan='16'><span title='Load multiple words, decrement after'>LDMDA</span></td>
</tr>
<tr>
 <td class='bit'>82</td>
 <td colspan='16'><span title='Store multiple words, decrement after'>STMDA</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>83</td>
 <td colspan='16'><span title='Load multiple words, decrement after'>LDMDA</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>84</td>
 <td colspan='16'><span title='Store multiple words, decrement after'>STMDA</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>85</td>
 <td colspan='16'><span title='Load multiple words, decrement after'>LDMDA</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>86</td>
 <td colspan='16'><span title='Store multiple words, decrement after'>STMDA</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>87</td>
 <td colspan='16'><span title='Load multiple words, decrement after'>LDMDA</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>88</td>
 <td colspan='16'><span title='Store multiple words, increment after'>STMIA</span></td>
</tr>
<tr>
 <td class='bit'>89</td>
 <td colspan='16'><span title='Load multiple words, increment after'>LDMIA</span></td>
</tr>
<tr>
 <td class='bit'>8A</td>
 <td colspan='16'><span title='Store multiple words, increment after'>STMIA</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>8B</td>
 <td colspan='16'><span title='Load multiple words, increment after'>LDMIA</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>8C</td>
 <td colspan='16'><span title='Store multiple words, increment after'>STMIA</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>8D</td>
 <td colspan='16'><span title='Load multiple words, increment after'>LDMIA</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>8E</td>
 <td colspan='16'><span title='Store multiple words, increment after'>STMIA</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>8F</td>
 <td colspan='16'><span title='Load multiple words, increment after'>LDMIA</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>90</td>
 <td colspan='16'><span title='Store multiple words, decrement before'>STMDB</span></td>
</tr>
<tr>
 <td class='bit'>91</td>
 <td colspan='16'><span title='Load multiple words, decrement before'>LDMDB</span></td>
</tr>
<tr>
 <td class='bit'>92</td>
 <td colspan='16'><span title='Store multiple words, decrement before'>STMDB</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>93</td>
 <td colspan='16'><span title='Load multiple words, decrement before'>LDMDB</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>94</td>
 <td colspan='16'><span title='Store multiple words, decrement before'>STMDB</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>95</td>
 <td colspan='16'><span title='Load multiple words, decrement before'>LDMDB</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>96</td>
 <td colspan='16'><span title='Store multiple words, decrement before'>STMDB</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>97</td>
 <td colspan='16'><span title='Load multiple words, decrement before'>LDMDB</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>98</td>
 <td colspan='16'><span title='Store multiple words, increment before'>STMIB</span></td>
</tr>
<tr>
 <td class='bit'>99</td>
 <td colspan='16'><span title='Load multiple words, increment before'>LDMIB</span></td>
</tr>
<tr>
 <td class='bit'>9A</td>
 <td colspan='16'><span title='Store multiple words, increment before'>STMIB</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>9B</td>
 <td colspan='16'><span title='Load multiple words, increment before'>LDMIB</span> <span title='Write back'>w</span></td>
</tr>
<tr>
 <td class='bit'>9C</td>
 <td colspan='16'><span title='Store multiple words, increment before'>STMIB</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>9D</td>
 <td colspan='16'><span title='Load multiple words, increment before'>LDMIB</span> <span title='Use user-mode registers'>u</span></td>
</tr>
<tr>
 <td class='bit'>9E</td>
 <td colspan='16'><span title='Store multiple words, increment before'>STMIB</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>9F</td>
 <td colspan='16'><span title='Load multiple words, increment before'>LDMIB</span> <span title='Use user-mode registers, with write back'>uw</span></td>
</tr>
<tr>
 <td class='bit'>A0</td>
 <td rowspan='16' colspan='16'><span title='Branch'>B</span></td>
</tr>
<tr>
 <td class='bit'>A1</td>
</tr>
<tr>
 <td class='bit'>A2</td>
</tr>
<tr>
 <td class='bit'>A3</td>
</tr>
<tr>
 <td class='bit'>A4</td>
</tr>
<tr>
 <td class='bit'>A5</td>
</tr>
<tr>
 <td class='bit'>A6</td>
</tr>
<tr>
 <td class='bit'>A7</td>
</tr>
<tr>
 <td class='bit'>A8</td>
</tr>
<tr>
 <td class='bit'>A9</td>
</tr>
<tr>
 <td class='bit'>AA</td>
</tr>
<tr>
 <td class='bit'>AB</td>
</tr>
<tr>
 <td class='bit'>AC</td>
</tr>
<tr>
 <td class='bit'>AD</td>
</tr>
<tr>
 <td class='bit'>AE</td>
</tr>
<tr>
 <td class='bit'>AF</td>
</tr>
<tr>
 <td class='bit'>B0</td>
 <td rowspan='16' colspan='16'><span title='Branch and link'>BL</span></td>
</tr>
<tr>
 <td class='bit'>B1</td>
</tr>
<tr>
 <td class='bit'>B2</td>
</tr>
<tr>
 <td class='bit'>B3</td>
</tr>
<tr>
 <td class='bit'>B4</td>
</tr>
<tr>
 <td class='bit'>B5</td>
</tr>
<tr>
 <td class='bit'>B6</td>
</tr>
<tr>
 <td class='bit'>B7</td>
</tr>
<tr>
 <td class='bit'>B8</td>
</tr>
<tr>
 <td class='bit'>B9</td>
</tr>
<tr>
 <td class='bit'>BA</td>
</tr>
<tr>
 <td class='bit'>BB</td>
</tr>
<tr>
 <td class='bit'>BC</td>
</tr>
<tr>
 <td class='bit'>BD</td>
</tr>
<tr>
 <td class='bit'>BE</td>
</tr>
<tr>
 <td class='bit'>BF</td>
</tr>
<tr>
 <td class='bit'>C0</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Negative offset'>ofm</span></td>
</tr>
<tr>
 <td class='bit'>C1</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Negative offset'>ofm</span></td>
</tr>
<tr>
 <td class='bit'>C2</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Pre-decrement'>prm</span></td>
</tr>
<tr>
 <td class='bit'>C3</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Pre-decrement'>prm</span></td>
</tr>
<tr>
 <td class='bit'>C4</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Negative offset'>ofm</span></td>
</tr>
<tr>
 <td class='bit'>C5</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Negative offset'>ofm</span></td>
</tr>
<tr>
 <td class='bit'>C6</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Pre-decrement'>prm</span></td>
</tr>
<tr>
 <td class='bit'>C7</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Pre-decrement'>prm</span></td>
</tr>
<tr>
 <td class='bit'>C8</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Positive offset'>ofp</span></td>
</tr>
<tr>
 <td class='bit'>C9</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Positive offset'>ofp</span></td>
</tr>
<tr>
 <td class='bit'>CA</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Pre-increment'>prp</span></td>
</tr>
<tr>
 <td class='bit'>CB</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Pre-increment'>prp</span></td>
</tr>
<tr>
 <td class='bit'>CC</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Positive offset'>ofp</span></td>
</tr>
<tr>
 <td class='bit'>CD</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Positive offset'>ofp</span></td>
</tr>
<tr>
 <td class='bit'>CE</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Pre-increment'>prp</span></td>
</tr>
<tr>
 <td class='bit'>CF</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Pre-increment'>prp</span></td>
</tr>
<tr>
 <td class='bit'>D0</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Unindexed, bits 7-0 available for copro use'>unm</span></td>
</tr>
<tr>
 <td class='bit'>D1</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Unindexed, bits 7-0 available for copro use'>unm</span></td>
</tr>
<tr>
 <td class='bit'>D2</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Post-decrement'>ptm</span></td>
</tr>
<tr>
 <td class='bit'>D3</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Post-decrement'>ptm</span></td>
</tr>
<tr>
 <td class='bit'>D4</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Unindexed, bits 7-0 available for copro use'>unm</span></td>
</tr>
<tr>
 <td class='bit'>D5</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Unindexed, bits 7-0 available for copro use'>unm</span></td>
</tr>
<tr>
 <td class='bit'>D6</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Post-decrement'>ptm</span></td>
</tr>
<tr>
 <td class='bit'>D7</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Post-decrement'>ptm</span></td>
</tr>
<tr>
 <td class='bit'>D8</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Unindexed, bits 7-0 available for copro use'>unp</span></td>
</tr>
<tr>
 <td class='bit'>D9</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Unindexed, bits 7-0 available for copro use'>unp</span></td>
</tr>
<tr>
 <td class='bit'>DA</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Post-increment'>ptp</span></td>
</tr>
<tr>
 <td class='bit'>DB</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Post-increment'>ptp</span></td>
</tr>
<tr>
 <td class='bit'>DC</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Unindexed, bits 7-0 available for copro use'>unp</span></td>
</tr>
<tr>
 <td class='bit'>DD</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Unindexed, bits 7-0 available for copro use'>unp</span></td>
</tr>
<tr>
 <td class='bit'>DE</td>
 <td colspan='16'><span title='Store coprocessor data to memory'>STC</span> <span title='Post-increment'>ptp</span></td>
</tr>
<tr>
 <td class='bit'>DF</td>
 <td colspan='16'><span title='Load coprocessor data from memory'>LDC</span> <span title='Post-increment'>ptp</span></td>
</tr>
<tr>
 <td class='bit'>E0</td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 <td rowspan='16'><span title='Perform coprocessor data operation'>CDP</span></td>
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E1</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>E2</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E3</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>E4</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E5</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>E6</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E7</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>E8</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>E9</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>EA</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>EB</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>EC</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>ED</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>EE</td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
 
 <td><span title='Write coprocessor register from ARM register'>MCR</span></td>
</tr>
<tr>
 <td class='bit'>EF</td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
 
 <td><span title='Read coprocessor register to ARM register'>MRC</span></td>
</tr>
<tr>
 <td class='bit'>F0</td>
 <td rowspan='16' colspan='16'><span title='Software interrupt (enter supervisor mode)'>SWI</span></td>
</tr>
<tr>
 <td class='bit'>F1</td>
</tr>
<tr>
 <td class='bit'>F2</td>
</tr>
<tr>
 <td class='bit'>F3</td>
</tr>
<tr>
 <td class='bit'>F4</td>
</tr>
<tr>
 <td class='bit'>F5</td>
</tr>
<tr>
 <td class='bit'>F6</td>
</tr>
<tr>
 <td class='bit'>F7</td>
</tr>
<tr>
 <td class='bit'>F8</td>
</tr>
<tr>
 <td class='bit'>F9</td>
</tr>
<tr>
 <td class='bit'>FA</td>
</tr>
<tr>
 <td class='bit'>FB</td>
</tr>
<tr>
 <td class='bit'>FC</td>
</tr>
<tr>
 <td class='bit'>FD</td>
</tr>
<tr>
 <td class='bit'>FE</td>
</tr>
<tr>
 <td class='bit'>FF</td>
</tr>
   </tbody>
  </table>
  <table>
   <caption>Table 2. Thumb Opcode Map.</caption>
   <thead>
    <tr>
     <th rowspan='2'>Bits<br>15-12</th>
     <th colspan='16'>Bits 11-8</th>
    </tr>
    <tr>
     <th>0</th><th>1</th><th>2</th><th>3</th>
     <th>4</th><th>5</th><th>6</th><th>7</th>
     <th>8</th><th>9</th><th>A</th><th>B</th>
     <th>C</th><th>D</th><th>E</th><th>F</th>
    </tr>
   </thead>
   <tbody>
<tr>
 <td class='bit'>0</td>
 <td colspan='8'><span title='Logical shift-left register'>LSL</span> <span title='Immediate value'>imm</span></td>
 <td colspan='8'><span title='Logical shift-right register'>LSR</span> <span title='Immediate value'>imm</span></td>
</tr>
<tr>
 <td class='bit'>1</td>
 <td colspan='8'><span title='Arithmetic shift-right register'>ASR</span> <span title='Immediate value'>imm</span></td>
 <td colspan='2'><span title='Add to register'>ADD</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Subtract from register'>SUB</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Add to register'>ADD</span> <span title='3-bit immediate offset'>imm3</span></td>
 <td colspan='2'><span title='Subtract from register'>SUB</span> <span title='3-bit immediate offset'>imm3</span></td>
</tr>
<tr>
 <td class='bit'>2</td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r0'>i8r0</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r1'>i8r1</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r2'>i8r2</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r3'>i8r3</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r4'>i8r4</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r5'>i8r5</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r6'>i8r6</span></td>
 <td><span title='Move value to a register'>MOV</span> <span title='8-bit immediate offset, using r7'>i8r7</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r0'>i8r0</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r1'>i8r1</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r2'>i8r2</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r3'>i8r3</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r4'>i8r4</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r5'>i8r5</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r6'>i8r6</span></td>
 <td><span title='Compare register to value (Subtract)'>CMP</span> <span title='8-bit immediate offset, using r7'>i8r7</span></td>
</tr>
<tr>
 <td class='bit'>3</td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r0'>i8r0</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r1'>i8r1</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r2'>i8r2</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r3'>i8r3</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r4'>i8r4</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r5'>i8r5</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r6'>i8r6</span></td>
 <td><span title='Add to register'>ADD</span> <span title='8-bit immediate offset, using r7'>i8r7</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r0'>i8r0</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r1'>i8r1</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r2'>i8r2</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r3'>i8r3</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r4'>i8r4</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r5'>i8r5</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r6'>i8r6</span></td>
 <td><span title='Subtract from register'>SUB</span> <span title='8-bit immediate offset, using r7'>i8r7</span></td>
</tr>
<tr>
 <td class='bit'>4</td>
 <td><span title='Thumb data processing'>DP</span> <span title='Instruction group 1'>g1</span></td>
 <td><span title='Thumb data processing'>DP</span> <span title='Instruction group 2'>g2</span></td>
 <td><span title='Thumb data processing'>DP</span> <span title='Instruction group 3'>g3</span></td>
 <td><span title='Thumb data processing'>DP</span> <span title='Instruction group 4'>g4</span></td>
 <td><span title='Add registers, select from all 16'>ADDH</span></td>
 <td><span title='Compare registers, select from all 16'>CMPH</span></td>
 <td><span title='Move to a register, select from all 16'>MOVH</span></td>
 <td><span title='Branch and switch execution modes'>BX</span> <span title='Register offset'>reg</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r0'>r0</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r1'>r1</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r2'>r2</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r3'>r3</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r4'>r4</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r5'>r5</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r6'>r6</span></td>
 <td><span title='r15-relative load word'>LDRPC</span> <span title='Using r7'>r7</span></td>
</tr>
<tr>
 <td class='bit'>5</td>
 <td colspan='2'><span title='Store word'>STR</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Store halfword'>STRH</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Store byte'>STRB</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load signed byte'>LDRSB</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load word'>LDR</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load halfword'>LDRH</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load byte'>LDRB</span> <span title='Register offset'>reg</span></td>
 <td colspan='2'><span title='Load signed halfword'>LDRSH</span> <span title='Register offset'>reg</span></td>
</tr>
<tr>
 <td class='bit'>6</td>
 <td colspan='8'><span title='Store word'>STR</span> <span title='5-bit immediate offset'>imm5</span></td>
 <td colspan='8'><span title='Load word'>LDR</span> <span title='5-bit immediate offset'>imm5</span></td>
</tr>
<tr>
 <td class='bit'>7</td>
 <td colspan='8'><span title='Store byte'>STRB</span> <span title='5-bit immediate offset'>imm5</span></td>
 <td colspan='8'><span title='Load byte'>LDRB</span> <span title='5-bit immediate offset'>imm5</span></td>
</tr>
<tr>
 <td class='bit'>8</td>
 <td colspan='8'><span title='Store halfword'>STRH</span> <span title='5-bit immediate offset'>imm5</span></td>
 <td colspan='8'><span title='Load halfword'>LDRH</span> <span title='5-bit immediate offset'>imm5</span></td>
</tr>
<tr>
 <td class='bit'>9</td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r0'>r0</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r1'>r1</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r2'>r2</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r3'>r3</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r4'>r4</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r5'>r5</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r6'>r6</span></td>
 <td><span title='r13-relative store word'>STRSP</span> <span title='Using r7'>r7</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r0'>r0</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r1'>r1</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r2'>r2</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r3'>r3</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r4'>r4</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r5'>r5</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r6'>r6</span></td>
 <td><span title='r13-relative load word'>LDRSP</span> <span title='Using r7'>r7</span></td>
</tr>
<tr>
 <td class='bit'>A</td>
 <td><span title=''>ADDPC</span> <span title='Using r0'>r0</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r1'>r1</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r2'>r2</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r3'>r3</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r4'>r4</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r5'>r5</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r6'>r6</span></td>
 <td><span title=''>ADDPC</span> <span title='Using r7'>r7</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r0'>r0</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r1'>r1</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r2'>r2</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r3'>r3</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r4'>r4</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r5'>r5</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r6'>r6</span></td>
 <td><span title=''>ADDSP</span> <span title='Using r7'>r7</span></td>
</tr>
<tr>
 <td class='bit'>B</td>
 <td><span title=''>ADDSP</span> <span title='7-bit immediate offset'>imm7</span></td>
 <td class='und' colspan='3'></td>
 <td><span title='Store multiple words to memory (STMDB equivalent)'>PUSH</span></td>
 <td><span title='Store multiple words to memory (STMDB equivalent)'>PUSH</span> <span title='Include r14'>lr</span></td>
 <td class='und' colspan='6'></td>
 <td><span title='Load multiple words from memory (LDMIA equivalent)'>POP</span></td>
 <td><span title='Load multiple words from memory (LDMIA equivalent)'>POP</span> <span title='Include r15'>pc</span></td>
 <td class='arm9'><span title='Software breakpoint'>BKPT</span></td>
 <td class='und'></td>
</tr>
<tr>
 <td class='bit'>C</td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r0'>r0</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r1'>r1</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r2'>r2</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r3'>r3</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r4'>r4</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r5'>r5</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r6'>r6</span></td>
 <td><span title='Store multiple words, increment after'>STMIA</span> <span title='Using r7'>r7</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r0'>r0</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r1'>r1</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r2'>r2</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r3'>r3</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r4'>r4</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r5'>r5</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r6'>r6</span></td>
 <td><span title='Load multiple words, increment after'>LDMIA</span> <span title='Using r7'>r7</span></td>
</tr>
<tr>
 <td class='bit'>D</td>
 <td><span title='Branch if zero flag set'>BEQ</span></td>
 <td><span title='Branch if zero flag clear'>BNE</span></td>
 <td><span title='Branch if carry flag set'>BCS</span></td>
 <td><span title='Branch if carry flag clear'>BCC</span></td>
 <td><span title='Branch if negative flag set'>BMI</span></td>
 <td><span title='Branch if negative flag clear'>BPL</span></td>
 <td><span title='Branch if overflow flag set'>BVS</span></td>
 <td><span title='Branch if overflow flag clear'>BVC</span></td>
 <td><span title='Branch if higher (unsigned)'>BHI</span></td>
 <td><span title='Branch if lower or the same (unsigned)'>BLS</span></td>
 <td><span title='Branch if greater than or equal to'>BGE</span></td>
 <td><span title='Branch if less than'>BLT</span></td>
 <td><span title='Branch if greater than'>BGT</span></td>
 <td><span title='Branch if less than or equal to'>BLE</span></td>
 <td class='und'></td>
 <td><span title='Software interrupt (enter supervisor mode)'>SWI</span></td>
</tr>
<tr>
 <td class='bit'>E</td>
 <td colspan='8'><span title='Branch'>B</span></td>
 <td class='arm9' colspan='8'><span title='Branch, link and switch execution modes '>BLX</span> <span title='Two-instruction branch, low 11 bits of offset'>off</span></td>
</tr>
<tr>
 <td class='bit'>F</td>
 <td colspan='8'><span title='Branch and link'>BL</span> <span title='Two-instruction branch, high 11 bits of offset'>setup</span></td>
 <td colspan='8'><span title='Branch and link'>BL</span> <span title='Two-instruction branch, low 11 bits of offset'>off</span></td>
</tr>
   </tbody>
  </table>
  <table>
   <caption>Table 2A. Thumb Opcode Map - Register/Register Data Processing.</caption>
   <thead>
    <tr>
     <th rowspan='2'>Bits<br>9-8</th>
     <th colspan='4'>Bits 7-6</th>
    </tr>
    <tr>
     <th>0</th><th>1</th><th>2</th><th>3</th>
    </tr>
   </thead>
   <tbody>
<tr>
 <td class='bit'>0</td>
 <td><span title='Logical And'>AND</span></td>
 <td><span title='Logical Exclusive-or'>EOR</span></td>
 <td><span title='Logical Left-shift'>LSL</span></td>
 <td><span title='Logical Right-shift'>LSR</span></td>
</tr>
<tr>
 <td class='bit'>1</td>
 <td><span title='Arithmetic Right-shift'>ASR</span></td>
 <td><span title='Add'>ADD</span></td>
 <td><span title='Subtract'>SUB</span></td>
 <td><span title='Rotate right'>ROR</span></td>
</tr>
<tr>
 <td class='bit'>2</td>
 <td><span title='Test Bits (Logical And)'>TST</span></td>
 <td><span title='Negate (Subtract from zero)'>NEG</span></td>
 <td><span title='Compare (Subtract)'>CMP</span></td>
 <td><span title='Compare negative (Add)'>CMN</span></td>
</tr>
<tr>
 <td class='bit'>3</td>
 <td><span title='Logical Or'>ORR</span></td>
 <td><span title='Multiply'>MUL</span></td>
 <td><span title='Bit Clear (NAND)'>BIC</span></td>
 <td><span title='Move negative (NOT)'>MVN</span></td>
</tr>
   </tbody>
  </table>
  </div>
<p><em>Article dated: 4th Oct 2007</em></p>    
    </div>
   </div>
   <div id="foot">
    <div class="inner">
     Operated by Imran Nazar Ltd, registered in the UK (#07698370). Content copyright Imran Nazar, 2005-2011.<br>
     Design and imagery copyright Imran Nazar, 2008-2011; "Parchment" used by license from <a href="http://sxc.hu">sxc</a>.
    </div>
   </div>
   <ul id="nav">
    <li  id="nav_Articles"><a href="/Articles">Articles</a></li>
    <li class="hilite" id="nav_Programming"><a href="/Programming">Programming</a></li>
    <li  id="nav_Fiction"><a href="/Fiction">Fiction</a></li>
    <li  id="nav_Contact"><a href="/Contact">Get in touch</a></li>
   </ul>
   <a href="/rss.xml" id="rss">Get the RSS feed</a>
   <div id="ads">
<script type="text/javascript"><!--


*/
