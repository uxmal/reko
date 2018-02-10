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

if(NOT NUM_JOBS)
	ProcessorCount(NUM_JOBS)
endif()

message(STATUS "> We can use ${NUM_JOBS} CPU Threads")
set(CTEST_PARALLEL_LEVEL ${NUM_JOBS})


function(add_log_file log_file)
	get_property(LOG_FILES GLOBAL PROPERTY LOG_FILES)
	list(APPEND LOG_FILES ${log_file})
	set_property(GLOBAL PROPERTY LOG_FILES "${LOG_FILES}")
endfunction()

function(save_logs_list logs_list_file)
	get_property(LOG_FILES GLOBAL PROPERTY LOG_FILES)
	file(WRITE ${logs_list_file} "${LOG_FILES}")
endfunction()

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
	
	set(item_idx 0)
	list(LENGTH list n_items)

	foreach(item ${list})
		if(item)
			set(result "${result}${item}")
		endif()

		math(EXPR item_idx "${item_idx}+1")
		if(${item_idx} LESS ${n_items})
			set(result "${result}${separator}")
		endif()
	endforeach()

	set(${out_list} "${result}" PARENT_SCOPE)
endfunction()

function(get_test_runner_cmdline)
	# switches, single value, list value
	cmake_parse_arguments(ARGS "" "TEST_NAME;TEST_DIRECTORY;CMDLINE_VAR" "EXTRA_ARGS" ${ARGN})

	set(final_cmd ${CMAKE_COMMAND})

	set(REKO_CMD ${CLR_RUNTIME} ${REKO_CMDLINE_DIR}/${REKO_CMDLINE_EXE})

	if(ARGS_EXTRA_ARGS)
		list(APPEND REKO_CMD ${ARGS_EXTRA_ARGS})
	endif()

	marshal_list("*" "${REKO_CMD}" REKO_CMD)

	list(APPEND final_cmd "-DTEST_NAME=${ARGS_TEST_NAME}")
	list(APPEND final_cmd "-DTEST_DIRECTORY=${ARGS_TEST_DIRECTORY}")
	list(APPEND final_cmd "-DCOMMAND=${REKO_CMD}")
	list(APPEND final_cmd -P ${TEST_RUNNER})

	set(${ARGS_CMDLINE_VAR} ${final_cmd} PARENT_SCOPE)
endfunction()

function(define_regression_test_dcproj test_dir project_file out_target)
	get_test_name(${test_dir} test_name)

	get_filename_component(proj_name ${project_file} NAME)

	set(test_name ${test_name}_${proj_name})
	message(STATUS "> Add_dcproject ${test_name} => ${project_file}")

	get_test_runner_cmdline(
		TEST_NAME
			${test_name}
		TEST_DIRECTORY
			${test_dir}
		EXTRA_ARGS
			${project_file}
		CMDLINE_VAR runner_cmd
	)

	add_log_file("${test_dir}/${test_name}.log")

	# run .dcproject
	add_test(
		NAME  ${test_name}
		COMMAND ${runner_cmd}
		WORKING_DIRECTORY ${TOPLEVEL}
	)

	set(${out_target} ${test_name} PARENT_SCOPE)
endfunction()

function(define_regression_test_script test_dir script_file out_targets)
	# Get list of lines
	file(STRINGS ${script_file} script)

	set(tasks "")
	foreach(line ${script})
		if(line MATCHES "^#" OR NOT line)
			continue()
		endif()

		set(cmdline "")

		## Split by space (string to list), and collect non empty args
		string(STRIP ${line} _cmdline)
		string(REPLACE " " ";" _cmdline ${_cmdline})

		foreach(piece ${_cmdline})
			if(NOT piece)
				continue()
			endif()

			list(APPEND cmdline ${piece})
		endforeach()

		marshal_list("*" "${cmdline}" cmdline)
		list(APPEND tasks "${cmdline}")
	endforeach()

	## If there is no task (only comments) skip this directory
	if(NOT tasks)
		return()
	endif()

	set(test_names "")

	foreach(cmdline ${tasks})
		string(REPLACE "*" ";" cmdline ${cmdline})

		# XXX: Assuming the executable name is the last argument
		list(GET cmdline -1 exe_name)

		get_test_name(${test_dir} test_name)
		set(test_name ${test_name}_${exe_name})

		message(STATUS "> Add_script ${test_name} => ${cmdline}")

		# Remove "decompiler.exe"
		list(REMOVE_AT cmdline 0)

		get_test_runner_cmdline(
			TEST_NAME
				${test_name}
			TEST_DIRECTORY
				${test_dir}
			EXTRA_ARGS
				${cmdline}
			CMDLINE_VAR runner_cmd
		)

		add_log_file("${test_dir}/${test_name}.log")

		# run script
		add_test(
			NAME ${test_name}
			COMMAND ${runner_cmd}
			WORKING_DIRECTORY ${TOPLEVEL}
		)

		list(APPEND test_names ${test_name})
	endforeach()

	set(${out_targets} ${test_names} PARENT_SCOPE)
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
			${test_dir} ${script_file} script_targets
		)

		if(script_targets)
			foreach(script_target ${script_targets})
				configure_test(${script_target})
			endforeach()
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
	set(CTEST_PARALLEL_LEVEL "-j ${NUM_JOBS}")
endif()

set(logs_list_file ${CMAKE_CURRENT_BINARY_DIR}/logs.txt)
save_logs_list(${logs_list_file})

add_custom_target(RunTests ALL
	COMMAND ctest -C $<CONFIGURATION>
		--output-on-failure
		${CTEST_PARALLEL_LEVEL}
		${CTEST_VERBOSE}
)

add_custom_target(CollectLogs ALL
	DEPENDS RunTests
	WORKING_DIRECTORY ${TOPLEVEL}
	COMMAND
		${CMAKE_COMMAND}
		"-DLOG_FILES=${logs_list_file}"
		-P ${TOPLEVEL}/collect.cmake
)