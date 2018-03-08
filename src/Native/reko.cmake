function(check_msys output)
	set(SHELL "$ENV{SHELL}")

	# If there's no SHELL it's not msys
	if(NOT SHELL)
		set(${output} FALSE PARENT_SCOPE)
		return()
	endif()

	execute_process(
		COMMAND ${SHELL} -c 'echo -ne $OSTYPE'
		OUTPUT_VARIABLE ostype
	)

	if(${ostype} STREQUAL "msys")
		set(${output} TRUE PARENT_SCOPE)
		return()
	endif()

	set(${output} FALSE PARENT_SCOPE)
endfunction()

function(clear_cache build_dir)
	if(EXISTS ${build_dir}/CMakeCache.txt)
		message(STATUS "Removing ${build_dir}/CMakeCache.txt")
		file(REMOVE ${build_dir}/CMakeCache.txt)
	endif()
endfunction()

function(process_project name path)
	set(BUILD_DIR ${path}/build/${REKO_PLATFORM}/${CMAKE_BUILD_TYPE})

	if(ACTION STREQUAL "clean")
		clean_project(${name} ${path} ${BUILD_DIR})
	else()
		invoke_cmake(${name} ${path} ${BUILD_DIR})
	endif()
endfunction()

function(clean_project name path build_dir)
	if(EXISTS ${build_dir})
		message(STATUS "Removing ${build_dir}")
		file(REMOVE_RECURSE ${build_dir})
	endif()
endfunction()

function(invoke_cmake name path build_dir)
	if(NOT EXISTS ${build_dir})
		file(MAKE_DIRECTORY ${build_dir})
	endif()

	check_msys(IS_MSYS)

	if(WIN32)
		message(STATUS "IS_MSYS: ${IS_MSYS}")
		if(IS_MSYS)
			set(GENERATOR "MSYS Makefiles")
		elseif(MINGW)
			set(GENERATOR "MinGW Makefiles")
		else()
			set(GENERATOR ${REKO_COMPILER})
		endif()
	endif()
	
	set(CMAKE_ARGS ${path})
	if(GENERATOR)
		list(APPEND CMAKE_ARGS -G ${GENERATOR})
	endif()

	if(REKO_PATH)
		list(APPEND CMAKE_ARGS "-DREKO_PATH=${REKO_PATH}")
	endif()

	if(REKO_PLATFORM)
		list(APPEND CMAKE_ARGS "-DREKO_PLATFORM=${REKO_PLATFORM}")
	endif()

	if(CMAKE_BUILD_TYPE)
		list(APPEND CMAKE_ARGS "-DCMAKE_BUILD_TYPE=${CMAKE_BUILD_TYPE}")
	endif()

	execute_process(
		COMMAND ${CMAKE_COMMAND} ${CMAKE_ARGS}
		WORKING_DIRECTORY ${build_dir}
		RESULT_VARIABLE retcode
	)

	if(NOT ${retcode} EQUAL 0)
		message(FATAL_ERROR "[CMake] ${name} configuration failed for: ${path}")
	endif()

	set(build_args "")

	if(IS_MSYS OR MINGW OR UNIX)
		list(APPEND build_args -- -j ${NUM_THREADS})
	elseif(WIN32 AND MSVC)
		list(APPEND build_args -- /m:${NUM_THREADS})
	endif()

	execute_process(
		COMMAND ${CMAKE_COMMAND} --build . ${build_args}
		WORKING_DIRECTORY ${build_dir}
		RESULT_VARIABLE retcode
	)

	if(NOT ${retcode} EQUAL 0)
		message(FATAL_ERROR "[CMake] ${name} build failed: ${path}")
	endif()
endfunction()

message("== Configuration ==")
message("=> Build Type: ${CMAKE_BUILD_TYPE}")
message("=> Generator : ${REKO_COMPILER}")
message("")

include(ProcessorCount)
ProcessorCount(NUM_THREADS)

message(STATUS "Building native libraries")
process_project(native ${CMAKE_CURRENT_SOURCE_DIR})