set(REKO_SRC ${CMAKE_CURRENT_LIST_DIR})
list(INSERT CMAKE_MODULE_PATH 0 "${REKO_SRC}/../cmake")
include(FindDotNet)

macro(rmrf path)
	message("=> rm -r ${path}")
	file(REMOVE_RECURSE "${path}")
endmacro()

execute_process(
	COMMAND ${DOTNET_EXE} clean
	COMMAND_ERROR_IS_FATAL ANY
	WORKING_DIRECTORY ${CMAKE_CURRENT_LIST_DIR}
)

file(GLOB_RECURSE CANDIDATES
	LIST_DIRECTORIES TRUE
		${REKO_SRC}/**/bin
		${REKO_SRC}/**/obj
)

foreach(path IN LISTS CANDIDATES)
	if(NOT IS_DIRECTORY "${path}"
	OR NOT path MATCHES "/(bin|obj)$")
		continue()
	endif()	
	rmrf("${path}")
endforeach()

set(REKO_CMAKE_BUILD_TARGETS ${REKO_SRC}/BuildTargets/build)

if(IS_DIRECTORY ${REKO_CMAKE_BUILD_TARGETS})
	rmrf(${REKO_CMAKE_BUILD_TARGETS})
endif()