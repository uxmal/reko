function(check_msys output)
	execute_process(
		COMMAND uname -o
		OUTPUT_VARIABLE ostype
		RESULT_VARIABLE result
	)
	if(NOT result EQUAL 0)
		set(${output} FALSE PARENT_SCOPE)
		return()
	endif()

	string(TOLOWER "${ostype}" ostype)
	string(STRIP "${ostype}" ostype)

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
		if(NOT IS_MSYS AND NOT MINGW)
			if(REKO_PLATFORM STREQUAL "x86")
				set(REKO_ARCH "Win32")
			else()
				set(REKO_ARCH ${REKO_PLATFORM})
			endif()
			list(APPEND CMAKE_ARGS -A ${REKO_ARCH})
		endif()
	endif()

	if(CMAKE_BUILD_TYPE)
		list(APPEND CMAKE_ARGS "-DCMAKE_BUILD_TYPE=${CMAKE_BUILD_TYPE}")
	endif()

	if(QUICK_CONFIGURE)
		message(STATUS "Skipping compiler sanity checks")
		list(APPEND CMAKE_ARGS
			"-DQUICK_CONFIGURE=ON"
			"-DCMAKE_C_COMPILER_FORCED=ON"
			"-DCMAKE_CXX_COMPILER_FORCED=ON"
		)		
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

	include(ProcessorCount)
	ProcessorCount(NUM_THREADS)

	if(IS_MSYS OR MINGW OR UNIX)
		list(APPEND build_args -- -j ${NUM_THREADS})
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
message("=> Platform  : ${REKO_PLATFORM}")
message("")

if(NOT DEFINED IS_MSYS)
	check_msys(IS_MSYS)
endif()


message(STATUS "Building native libraries")
process_project(native ${CMAKE_CURRENT_SOURCE_DIR})
