##
## Copyright(C) 2018 Stefano Moioli <smxdev4@gmail.com>
## This file is part of Reko Decompiler
##

message(STATUS "Merging log files from: ${LOG_FILES}")

file(READ ${LOG_FILES} log_files)

set(SUBJECTS_DIR "${CMAKE_CURRENT_BINARY_DIR}")
set(FINAL_LOG "${SUBJECTS_DIR}/regressions_cmake.log")

if(WIN32)
	set(NEWLINE "\r\n")
else
	set(NEWLINE "\n")
endif()

foreach(log_file ${log_files})
	message(STATUS "Merging: ${log_file}")
	
	file(RELATIVE_PATH log_relative ${SUBJECTS_DIR} ${log_file})

	file(READ ${log_file} log_data)
	file(APPEND ${FINAL_LOG} "== ${log_relative}${NEWLINE}")
	file(APPEND ${FINAL_LOG} ${log_data})
endforeach()
