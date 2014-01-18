using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Hunk
{
    class HunkShow
    {/*import Hunk
import HunkDisassembler
from amitools.util.HexDump import *

class HunkShow:
  
  def __init__(self, hunk_file, show_relocs=False, show_debug=False, \
                     disassemble=False, disassemble_start=0, hexdump=False, brief=False, \
                     use_objdump=False, cpu='68000'):
    self.hunk_file = hunk_file
    
    # clone file refs
    self.header = hunk_file.header
    self.segments = hunk_file.segments
    self.overlay = hunk_file.overlay
    self.overlay_headers = hunk_file.overlay_headers
    self.overlay_segments = hunk_file.overlay_segments
    self.libs = hunk_file.libs
    self.units = hunk_file.units
    
    self.show_relocs=show_relocs
    self.show_debug=show_debug
    self.disassemble=disassemble
    self.disassemble_start=disassemble_start
    self.use_objdump=use_objdump
    self.cpu = cpu
    self.hexdump=hexdump
    self.brief=brief
  
  def show_segments(self):
    hunk_type = self.hunk_file.type
    if hunk_type == Hunk.TYPE_LOADSEG:
      self.show_loadseg_segments()
    elif hunk_type == Hunk.TYPE_UNIT:
      self.show_unit_segments()
    elif hunk_type == Hunk.TYPE_LIB:
      self.show_lib_segments()
      
  def show_lib_segments(self):
    for lib in self.libs:
      print "Library #%d" % lib['lib_no']
      for unit in lib['units']:
        self.print_unit(unit['unit_no'], unit['name'])
        for segment in unit['segments']:
          self.show_segment(segment, unit['segments'])
      
  def show_unit_segments(self):
    for unit in self.units:
      self.print_unit(unit['unit_no'], unit['name'])
      for segment in unit['segments']:
        self.show_segment(segment, unit['segments'])
  
  def show_loadseg_segments(self):
    # header + segments
    if not self.brief:
      self.print_header(self.header)
    for segment in self.segments:
      self.show_segment(segment, self.segments)
    
    # overlay
    if self.overlay != None:
      print "Overlay"
      num_ov = len(self.overlay_headers)
      for o in xrange(num_ov):
        if not self.brief:
          self.print_header(self.overlay_headers[o])
        for segment in self.overlay_segments[o]:
          self.show_segment(segment, self.overlay_segments[o])
    
  def show_segment(self, hunk, seg_list):
    main = hunk[0]

    # unit hunks are named
    name = ""
    if hunk[0].has_key('name'):
      name = "'%s'" % main['name']

    type_name = main['type_name'].replace("HUNK_","")
    size = main['size']
    hunk_no = main['hunk_no']
    if main.has_key('data_file_offset'):
      data_file_offset = main['data_file_offset']
    else:
      data_file_offset = None
    hunk_file_offset = main['hunk_file_offset']
    if main.has_key('alloc_size'):
      alloc_size = main['alloc_size']
    else:
      alloc_size = None
    
    self.print_segment_header(hunk_no, type_name, size, name, data_file_offset, hunk_file_offset, alloc_size)
    if self.hexdump:
      print_hex(main['data'],indent=8)

    for extra in hunk[1:]:
      self.show_extra_hunk(extra)

    # index hunk info is embedded if its in a lib
    if main.has_key('index_hunk'):
      self.show_index_info(main['index_hunk'])

    if main['type'] == Hunk.HUNK_CODE and self.disassemble and len(main['data'])>0:
      disas = HunkDisassembler.HunkDisassembler(use_objdump = self.use_objdump, cpu = self.cpu)
      print
      disas.show_disassembly(hunk, seg_list, self.disassemble_start)
      print

  def show_index_info(self, info):
    # references from index
    if info.has_key('refs'):
      self.print_extra("refs","#%d" % len(info['refs']))
      if not self.brief:
        for ref in info['refs']:
          self.print_symbol(-1,ref['name'],"(%d bits)" % ref['bits'])
    # defines from index
    if info.has_key('defs'):
      self.print_extra("defs","#%d" % len(info['defs']))
      if not self.brief:
        for d in info['defs']:
          self.print_symbol(d['value'],d['name'],"(type %d)" % d['type'])
 
  def show_extra_hunk(self, hunk):
    hunk_type = hunk['type']
    if hunk_type in Hunk.reloc_hunks:
      type_name = hunk['type_name'].replace("HUNK_","").lower()
      self.print_extra("reloc","%s #%d" % (type_name, len(hunk['reloc'])))
      if not self.brief:
        self.show_reloc_hunk(hunk)
        
    elif hunk_type == Hunk.HUNK_DEBUG:
      self.print_extra("debug","%s  offset=%08x" % (hunk['debug_type'], hunk['debug_offset']))
      if not self.brief:
        self.show_debug_hunk(hunk)
    
    elif hunk_type == Hunk.HUNK_SYMBOL:
      self.print_extra("symbol","#%d" % (len(hunk['symbols'])))
      if not self.brief:
        self.show_symbol_hunk(hunk)
    
    elif hunk_type == Hunk.HUNK_EXT:
      self.print_extra("ext","def #%d  ref #%d  common #%d" % (len(hunk['ext_def']),len(hunk['ext_ref']),len(hunk['ext_common'])))
      if not self.brief:
        self.show_ext_hunk(hunk)
    
    else:
      self.print_extra("extra","%s" % hunk['type_name'])

  def show_reloc_hunk(self, hunk):
    reloc = hunk['reloc']
    for hunk_num in reloc:
      offsets = reloc[hunk_num]
      if self.show_relocs:
        for offset in offsets:
          self.print_symbol(offset,"Segment #%d" % hunk_num,"")
      else:
        self.print_extra_sub("To Segment #%d: %4d entries" % (hunk_num, len(offsets)))

  def show_debug_hunk(self, hunk):
    debug_type = hunk['debug_type']
    if debug_type == 'LINE':
      self.print_extra_sub("line for '%s'" % hunk['src_file'])
      if self.show_debug:
        for src_off in hunk['src_map']:
          addr = src_off[1]
          line = src_off[0]
          self.print_symbol(addr,"line %d" % line,"")
    else:
      if self.show_debug:
        print_hex(hunk['data'],indent=8)
    
  def show_symbol_hunk(self, hunk):
    for symbol in hunk['symbols']:
      self.print_symbol(symbol[1],symbol[0],"")
      
  def show_ext_hunk(self, hunk):
    # definition
    for ext in hunk['ext_def']:
      tname = ext['type_name'].replace("EXT_","").lower()
      self.print_symbol(ext['def'],ext['name'],tname)
    # references
    for ext in hunk['ext_ref']:
      refs = ext['refs']
      tname = ext['type_name'].replace("EXT_","").lower()
      for ref in refs:
        self.print_symbol(ref,ext['name'],tname)

    # common_base
    for ext in hunk['ext_common']:
      tname = ext['type_name'].replace("EXT_","").lower()
      self.print_symbol(ext['common_size'],ext['name'],tname)

  # ----- printing -----

  def print_header(self, hdr):
    print "\t      header (segments: first=%d, last=%d, table size=%d)" % (hdr['first_hunk'], hdr['last_hunk'], hdr['table_size'])

  def print_extra(self, type_name, info):
    print "\t\t%8s  %s" % (type_name, info)
    
  def print_extra_sub(self, text):
    print "\t\t\t%s" % text

  def print_segment_header(self, hunk_no, type_name, size, name, data_file_offset, hunk_file_offset, alloc_size):
    extra = ""
    if alloc_size != None:
      extra += "alloc size %08x  " % alloc_size
    extra += "file header @%08x" % hunk_file_offset
    if data_file_offset != None:
      extra += "  data @%08x" % data_file_offset
    print "\t#%03d  %-5s  size %08x  %s  %s" % (hunk_no, type_name, size, extra, name)

  def print_symbol(self,addr,name,extra):
    if addr == -1:
      a = "xxxxxxxx"
    else:
      a = "%08x" % addr
    print "\t\t\t%s  %-32s  %s" % (a,name,extra)

  def print_unit(self, no, name):
    print "  #%03d  UNIT  %s" % (no, name)
        



      */
    }
}
