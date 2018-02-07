function(check_msys output)
	set(${output} ("$ENV{OSTYPE}" STREQUAL "msys"))
endfunction()

function(clear_cache build_dir)
	if(EXISTS ${build_dir}/CMakeCache.txt)
		message(STATUS "Removing ${build_dir}/CMakeCache.txt")
		file(REMOVE ${build_dir}/CMakeCache.txt)
	endif()
endfunction()

function(invoke_cmake name path)
	set(BUILD_DIR ${path}/build)

	if(NOT EXISTS ${BUILD_DIR})
		file(MAKE_DIRECTORY ${BUILD_DIR})
	endif()

	if(WIN32)
		check_msys(IS_MSYS)
		if(IS_MSYS)
			set(GENERATOR "MSYS Makefiles")
		elseif(MINGW)
			set(GENERATOR "MinGW Makefiles")
		else()
			set(GENERATOR ${REKO_COMPILER})
		endif()
	endif()

	# TODO: do this when cleaning/changing build type only
	clear_cache(${BUILD_DIR})
	message(STATUS "CMake command line: ${CMAKE_COMMAND} .. ${GENERATOR}.REKO_COMPILER= ${REKO_COMPILER} ")
	
	set(CMAKE_ARGS "..")
	if(GENERATOR)
		list(APPEND CMAKE_ARGS -G ${GENERATOR})
	endif()

	#the problem is each argument here is passed as such, so "-G FOO" is one argument,. this should fail
	execute_process(
		COMMAND ${CMAKE_COMMAND} ${CMAKE_ARGS}
		WORKING_DIRECTORY ${BUILD_DIR}
		RESULT_VARIABLE retcode
	)

	if(NOT ${retcode} EQUAL 0)
		message(FATAL_ERROR "[CMake] ${name} configuration failed for: ${path}")
	endif()

	execute_process(
		COMMAND ${CMAKE_COMMAND} --build .
		WORKING_DIRECTORY ${BUILD_DIR}
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

set(ADDONS_PATH ${CMAKE_CURRENT_SOURCE_DIR}/addons)
message(STATUS "Building addons")
invoke_cmake(addons ${ADDONS_PATH})