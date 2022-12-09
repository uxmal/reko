get_filename_component(dest_dir ${COPY_SOURCE_PATH} DIRECTORY)

if(NOT EXISTS ${dest_dir})
	execute_process(
		COMMAND ${CMAKE_COMMAND} -E make_directory ${dest_dir}
	)
endif()

if(EXISTS ${COPY_SOURCE_PATH})
	execute_process(
		COMMAND ${CMAKE_COMMAND} -E copy ${COPY_SOURCE_PATH} ${COPY_DEST_PATH}
	)
elseif(EXISTS ${COPY_FALLBACK_PATH})
	execute_process(
		COMMAND ${CMAKE_COMMAND} -E copy ${COPY_FALLBACK_PATH} ${COPY_DEST_PATH}
	)
else()
	set(msg "Cannot copy '${COPY_SOURCE_PATH}' -> ${COPY_DEST_PATH}'")
	if(COPY_FALLBACK_PATH)
		string(APPEND msg ". Attempted fallback '${COPY_FALLBACK_PATH}'")
	endif()
	message(FATAL_ERROR "${msg}")
endif()