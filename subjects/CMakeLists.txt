##
## Copyright(C) 2018 Stefano Moioli <smxdev4@gmail.com>
## This file is part of Reko Decompiler
##

cmake_minimum_required(VERSION 3.0)

project(reko-regression-tests)
enable_testing()

include(ProcessorCount)

# The maximum time a regression test can take
if(NOT CTEST_TESTING_TIMEOUT)
	set(CTEST_TESTING_TIMEOUT 60)
endif()

ProcessorCount(NumThreads)

message(STATUS "> We can use ${NumThreads} CPU Threads")
set(CTEST_PARALLEL_LEVEL ${NumThreads})

# Retrives the .NET runtime executable (for unix)
macro(get_clr_runtime out_runner)
	if(UNIX AND NOT WIN32)
		set(${out_runner} "mono")
	else()
		set(${out_runner} "")
	endif()
endmacro()

# Path where regression test subdirectories are placed
set(TOPLEVEL ${CMAKE_CURRENT_LIST_DIR})

## REKO_PLATFORM, CMAKE_BUILD_TYPE
### Used to build the path to decompile.exe
if(NOT REKO_PLATFORM)
	set(REKO_PLATFORM "x86")
endif()

if(NOT CMAKE_BUILD_TYPE)
	set(CMAKE_BUILD_TYPE "Debug")
endif()

get_filename_component(
	REKO_CMDLINE_DIR
	"${TOPLEVEL}/../src/Drivers/CmdLine"
	ABSOLUTE
)

get_clr_runtime(CLR_RUNTIME)
set(TEST_RUNNER ${TOPLEVEL}/runner.cmake)
set(REKO_CMDLINE_DIR "${REKO_CMDLINE_DIR}/bin/${REKO_PLATFORM}/${CMAKE_BUILD_TYPE}")
set(REKO_CMDLINE_EXE "decompile.exe")

set(OUTPUT_EXTENSIONS "asm" "c" "dis" "h")

# Returns a list of files that should be removed (given a list of files)
function(filter_clear_dir_list in_files out_list)
	set(result "")
	foreach(file ${in_files})
		foreach(ext ${OUTPUT_EXTENSIONS})
			if(file MATCHES "\\.${ext}$")
				list(APPEND result ${file})
			endif()
		endforeach()
	endforeach()

	set(${out_list} ${result} PARENT_SCOPE)
endfunction()

# Returns a list of files that should be removed (given a path to look at)
function(get_clean_dir_list test_dir out_files)
	file(GLOB in_files "${test_dir}/*")
	filter_clear_dir_list("${in_files}" result)

	set(${out_files} ${result} PARENT_SCOPE)
endfunction()

function(get_test_name test_dir out_name)
	file(RELATIVE_PATH test_name ${TOPLEVEL} ${test_dir})
	string(REPLACE "/" "_" test_name ${test_name})
	string(REPLACE "\\" "_" test_name ${test_name})
	
	set(${out_name} ${test_name} PARENT_SCOPE)
endfunction()

# swaps the default ";" list separator for a custom one
# we need to do this to pass a list inside a list (list of args inside list of args)
# we additionally remove empty elements in the list while doing so
function(marshal_list separator list out_list)
	set(result "")
	foreach(item ${list})
		if(item)
			set(result "${result}${item}${separator}")
		endif()
	endforeach()

	set(${out_list} "${result}" PARENT_SCOPE)
endfunction()

function(get_test_runner_cmdline)
	# switches, single value, list value
	cmake_parse_arguments(ARGS "" "IN_FILENAME;OUT_DASM;OUT_DIS;OUT_SRC;OUT_HDR;OUT_GLOBALS;TEST_DIRECTORY;CMDLINE_VAR" "EXTRA_ARGS" ${ARGN})

	set(final_cmd ${CMAKE_COMMAND})

	# Get the list of files that should be cleaned
	get_clean_dir_list(${ARGS_TEST_DIRECTORY} clean_args)

	if(clean_args)
		marshal_list("*" "${clean_args}" clean_args)
		list(APPEND final_cmd "-DCLEAR_FILES=${clean_args}")
	endif()
	

	## Enable checks if the caller wants to make sure
	## that the dasm/dis/src/header/globals files were specified
	## also convert the relative path to absolute while forwarding
	if(ARGS_OUT_DASM)
		list(APPEND final_cmd "-DOUT_DASM=${ARGS_TEST_DIRECTORY}/${ARGS_OUT_DASM}")
	endif()

	if(ARGS_OUT_DIS)
		list(APPEND final_cmd "-DOUT_DIS=${ARGS_TEST_DIRECTORY}/${ARGS_OUT_DIS}")
	endif()

	if(ARGS_OUT_SRC)
		list(APPEND final_cmd "-DOUT_SRC=${ARGS_TEST_DIRECTORY}/${ARGS_OUT_SRC}")
	endif()

	if(ARGS_OUT_HDR)
		list(APPEND final_cmd "-DOUT_HDR=${ARGS_TEST_DIRECTORY}/${ARGS_OUT_HDR}")
	endif()

	if(ARGS_OUT_GLOBALS)
		list(APPEND final_cmd "-DOUT_GLOBALS=${ARGS_TEST_DIRECTORY}/${ARGS_OUT_GLOBALS}")
	endif()

	set(REKO_CMD ${CLR_RUNTIME} ${REKO_CMDLINE_DIR}/${REKO_CMDLINE_EXE})

	if(ARGS_EXTRA_ARGS)
		list(APPEND REKO_CMD ${ARGS_EXTRA_ARGS})
	endif()

	marshal_list("*" "${REKO_CMD}" REKO_CMD)

	list(APPEND final_cmd "-DTEST_DIRECTORY=${ARGS_TEST_DIRECTORY}")
	list(APPEND final_cmd "-DCOMMAND=${REKO_CMD}")
	list(APPEND final_cmd -P ${TEST_RUNNER})

	set(${ARGS_CMDLINE_VAR} ${final_cmd} PARENT_SCOPE)
endfunction()

function(define_regression_test_dcproj test_dir project_file out_target)
	# read .dcproject and scan it to find the output names
	file(STRINGS ${project_file} prj_lines)

	foreach(line ${prj_lines})
		if(line MATCHES "<filename>(.*)</filename>")
			set(arg_in_filename "${CMAKE_MATCH_1}")
		elseif(line MATCHES "<disassembly>(.*)</disassembly>")
			set(arg_dasm_out "${CMAKE_MATCH_1}")
		elseif(line MATCHES "<intermediate-code>(.*)</intermediate-code>")
			set(arg_dis_out "${CMAKE_MATCH_1}")
		elseif(line MATCHES "<output>(.*)</output>")
			set(arg_c_src_out "${CMAKE_MATCH_1}")
		elseif(line MATCHES "<types-file>(.*)</types-file>")
			set(arg_c_hdr_out "${CMAKE_MATCH_1}")
		elseif(line MATCHES "<global-vars>(.*)</global-vars>")
			set(arg_c_globals_out "${CMAKE_MATCH_1}")
		endif()
	endforeach()	

	if(NOT arg_in_filename)
		message(FATAL_ERROR "${project_file} doesn't contain <filename>")
	endif()

	## Prepare the filename without extension, to fill any missing directive as
	## <name_we>.extension
	get_filename_component(in_filename_we ${arg_in_filename} NAME_WE)

	if(NOT arg_dasm_out)
		set(arg_dasm_out "${in_filename_we}.asm")
	endif()

	if(NOT arg_dis_out)
		set(arg_dis_out "${in_filename_we}.dis")
	endif()

	if(NOT arg_c_src_out)
		set(arg_c_src_out "${in_filename_we}.c")
	endif()

	if(NOT arg_c_hdr_out)
		set(arg_c_hdr_out "${in_filename_we}.h")
	endif()

	if(NOT arg_c_globals_out)
		set(arg_c_globals_out "${in_filename_we}.globals.c")
	endif()
	
	get_test_name(${test_dir} test_name)
	set(test_name ${test_name}_dcproject)

	message(STATUS "> Add_dcproject ${test_name} => ${project_file}")

	get_test_runner_cmdline(
		TEST_DIRECTORY
			${test_dir}
		EXTRA_ARGS
			${project_file}
		CMDLINE_VAR runner_cmd
		
		OUT_DASM ${arg_dasm_out}
		OUT_DIS	${arg_dis_out}
		OUT_SRC	${arg_c_src_out}
		OUT_HDR ${arg_c_hdr_out}
		OUT_GLOBALS ${arg_c_globals_out}
	)

	# run .dcproject
	add_test(
		NAME  ${test_name}
		COMMAND ${runner_cmd}
		WORKING_DIRECTORY ${TOPLEVEL}
	)

	set(${out_target} ${test_name} PARENT_SCOPE)
endfunction()

function(define_regression_test_script test_dir script_file out_target)
	# Get list of lines
	file(STRINGS ${script_file} script)

	set(cmdline "")

	foreach(line ${script})
		if(line MATCHES "^#" OR NOT line)
			continue()
		endif()

		## Split by space (string to list), and collect non empty args
		string(STRIP ${line} _cmdline)
		string(REPLACE " " ";" _cmdline ${_cmdline})

		foreach(piece ${_cmdline})
			if(NOT piece)
				continue()
			endif()

			list(APPEND cmdline ${piece})
		endforeach()
	endforeach()

	## If there is no cmdline (only comments) this test is disabled, skip it
	if(NOT cmdline)
		return()
	endif()

	## TODO: extract the binary name from cmdline for output checks

	get_test_name(${test_dir} test_name)
	set(test_name ${test_name}_script)

	message(STATUS "> Add_script ${test_name} => ${cmdline}")

	# Remove the executable name
	list(REMOVE_AT cmdline 0)

	get_test_runner_cmdline(
		TEST_DIRECTORY
			${test_dir}
		EXTRA_ARGS
			${cmdline}
		CMDLINE_VAR runner_cmd
	)

	# run script
	add_test(
		NAME ${test_name}
		COMMAND ${runner_cmd}
		WORKING_DIRECTORY ${TOPLEVEL}
	)

	set(${out_target} ${test_name} PARENT_SCOPE)
endfunction()

macro(configure_test test_name)
	set_tests_properties(${test_name} PROPERTIES
		TIMEOUT ${CTEST_TESTING_TIMEOUT}
	)
endmacro()

# Processes a directory, looking for .dcproject or subject.cmd
function(process_directory test_dir)
	file(GLOB files "${test_dir}/*")

	foreach(file ${files})
		if(file MATCHES "\\.dcproject$")
			# Define a .dcproject test
			define_regression_test_dcproj(
				${test_dir} ${file} dcproj_target
			)

			if(dcproj_target)
				configure_test(${dcproj_target})
			endif()
		endif()
	endforeach()

	set(script_file ${test_dir}/subject.cmd)
	if(EXISTS ${script_file})
		# Define a subject.cmd test
		define_regression_test_script(
			${test_dir} ${script_file} script_target
		)

		if(script_target)
			configure_test(${script_target})
		endif()
	endif()
endfunction()

## Get a list of directories in a given path
function(get_dirs path out_dirs)
	file(GLOB dirs "${path}/*")
	set(result "")
	
	foreach(entry ${dirs})
		if(IS_DIRECTORY ${entry})
			list(APPEND result ${entry})
		endif()
	endforeach()
	
	set(${out_dirs} ${result} PARENT_SCOPE)
endfunction()

# Recursively discover regression tests
function(discover path)
	get_dirs(${path} dirs)
	foreach(test_dir ${dirs})
		process_directory(${test_dir})
		discover(${test_dir})
	endforeach()
endfunction()

discover(${TOPLEVEL})


## Prepare ctest arguments
if(VERBOSE)
	set(CTEST_VERBOSE "--verbose")
endif()

if(NO_PARALLEL)
	set(CTEST_PARALLEL_LEVEL OFF)
else()
	set(CTEST_PARALLEL_LEVEL "-j ${NumThreads}")
endif()

add_custom_target(RunTests ALL
	COMMAND ctest -C $<CONFIGURATION>
		--output-on-failure
		${CTEST_PARALLEL_LEVEL}
		${CTEST_VERBOSE}
)