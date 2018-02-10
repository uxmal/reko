##
## Copyright(C) 2018 Stefano Moioli <smxdev4@gmail.com>
## This file is part of Reko Decompiler
##

message(STATUS "Merging log files from: ${LOG_FILES}")

file(READ ${LOG_FILES} log_files)

set(FINAL_LOG "testfoobar.log")

foreach(log_file ${log_files})
	message(STATUS "Merging: ${log_file}")
	file(READ ${log_file} log_data)
	file(APPEND ${FINAL_LOG} ${log_data})
endforeach()