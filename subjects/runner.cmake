##
## Copyright(C) 2018 Stefano Moioli <smxdev4@gmail.com>
## This file is part of Reko Decompiler
##

# Unmarshal lists
string(REPLACE "*" ";" CLEAR_FILES ${CLEAR_FILES})
string(REPLACE "*" ";" COMMAND ${COMMAND})

message(STATUS "> ${COMMAND}")

if(CLEAR_FILES)
	foreach(file ${CLEAR_FILES})
		message(STATUS "=> Removing ${file}")
		file(REMOVE ${file})
	endforeach()
endif()

execute_process(
	COMMAND ${COMMAND}
	WORKING_DIRECTORY ${TEST_DIRECTORY}
)

if(OUT_DASM AND NOT EXISTS "${OUT_DASM}")
	message(FATAL_ERROR "${OUT_DASM} hasn't been generated")
endif()

if(OUT_DIS AND NOT EXISTS "${OUT_DIS}")
	message(FATAL_ERROR "${OUT_DIS} hasn't been generated")
endif()

if(OUT_SRC AND NOT EXISTS "${OUT_SRC}")
	message(FATAL_ERROR "${OUT_SRC} hasn't been generated")
endif()

if(OUT_HDR AND NOT EXISTS "${OUT_HDR}")
	message(FATAL_ERROR "${OUT_HDR} hasn't been generated")
endif()

if(OUT_GLOBALS AND NOT EXISTS "${OUT_GLOBALS}")
	message(FATAL_ERROR "${OUT_GLOBALS} hasn't been generated")
endif()