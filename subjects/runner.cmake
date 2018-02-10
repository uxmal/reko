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