#! /usr/bin/env python

import os
import angr
from angrutils import plot_cfg


sample = "./test"

if __name__ == "__main__":
    proj = angr.Project(sample, load_options={'auto_load_libs':False})
    main_sym = proj.loader.main_bin.get_symbol("main")

    if True:
	cfg = proj.analyses.CFGAccurate(fail_fast=False, 
	    starts=[main_sym.addr], 
	    context_sensitivity_level=3,
	    enable_function_hints=False,
	    keep_state=True, 
	    enable_advanced_backward_slicing=False, 
	    enable_symbolic_back_traversal=False,
	    normalize=True)
	#import IPython ; IPython.embed()

	plot_cfg(cfg, "cfgacc", format="raw", asminst=True, vexinst=False, debug_info=False, remove_imports=False, remove_path_terminator=True)
	os.system("xdot cfgacc.raw")

    if True:
	cfg = proj.analyses.CFGFast(fail_fast=False, 
	    start=main_sym.addr, 
	    end=main_sym.addr+main_sym.size, 
	    symbols=False, 
	    function_prologues=False, 
	    pickle_intermediate_results=False,
	    collect_data_references=False,
	    resolve_indirect_jumps=True,
	    normalize=True)
	plot_cfg(cfg, "cfgfst", format="raw", asminst=True, vexinst=False, debug_info=False, remove_imports=False, remove_path_terminator=True)
	os.system("xdot cfgfst.raw")

