##
## Copyright(C) 2018 Stefano Moioli <smxdev4@gmail.com>
## This file is part of Reko Decompiler
##

set(OUTPUT_EXTENSIONS "asm" "c" "dis" "h")

function(rm_by_ext directory ext)
	file(GLOB files "${directory}/*.${ext}")
	foreach(file ${files})
		message(STATUS "=> Removing ${file}")
		file(REMOVE ${file})
	endforeach()
endfunction()

# Unmarshal lists
string(REPLACE "*" ";" COMMAND ${COMMAND})

message(STATUS "> ${COMMAND}")

foreach(ext ${OUTPUT_EXTENSIONS})
	rm_by_ext(${TEST_DIRECTORY} ${ext})
endforeach()

execute_process(
	COMMAND ${COMMAND}
	WORKING_DIRECTORY ${TEST_DIRECTORY}
	OUTPUT_VARIABLE test_result
)

set(LOG_FILE ${TEST_DIRECTORY}/${TEST_NAME}.log)
file(WRITE ${LOG_FILE} ${test_result})