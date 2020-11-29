if(NOT CMAKE_BUILD_TYPE)
	set(CMAKE_BUILD_TYPE "Debug")
endif()

set(REKO_SRC ${CMAKE_CURRENT_LIST_DIR})


function(csproj_output_path)
	cmake_parse_arguments(asm "LIB;EXE" "PROJECT_SUBDIR;PROJECT_NAME;FRAMEWORK;OUTPUT_PREFIX;OUTPUT_SUFFIX;RESULT" "" ${ARGN})

	set(asm_basedir ${REKO_SRC})
	if(asm_PROJECT_SUBDIR)
		set(asm_basedir ${asm_basedir}/${asm_PROJECT_SUBDIR})
	endif()

	if(NOT asm_FRAMEWORK)
		if(asm_LIB)
			set(asm_FRAMEWORK ${REKO_LIB_FRAMEWORK})
		elseif(asm_EXE)
			set(asm_FRAMEWORK ${REKO_EXE_FRAMEWORK})
		else()
			message(FATAL_ERROR "Neither FRAMEWORK nor LIB/EXE specified")
		endif()
	endif()

	if(NOT asm_OUTPUT_SUFFIX)
		if(asm_LIB)
			set(asm_OUTPUT_SUFFIX ".dll")
		elseif(asm_EXE)
			if(WIN32)
				set(asm_OUTPUT_SUFFIX ".exe")
			else()
				set(asm_OUTPUT_SUFFIX "")
			endif()
		endif()
	endif()

	set(asm_filename ${asm_OUTPUT_PREFIX}${asm_PROJECT_NAME}${asm_OUTPUT_SUFFIX})
	set(asm_path ${asm_basedir}/${asm_PROJECT_NAME}/bin/${REKO_PLATFORM}/${CMAKE_BUILD_TYPE}/${asm_FRAMEWORK})

	set(${asm_RESULT} ${asm_path}/${asm_filename} PARENT_SCOPE)
endfunction()

function(targetname_from_path path output)
	string(REPLACE "${REKO_SRC}" "" path "${path}")
	string(REPLACE "/" "_" path "${path}")
	set(${output} ${path} PARENT_SCOPE)
endfunction()

function(add_copy_target)
	cmake_parse_arguments(copy "" "SRC_PATH;DEST_PATH;OUT_TARGET_NAME" "" ${ARGN})

	targetname_from_path(${copy_SRC_PATH} copy_TARGET_NAME)
	set(${copy_OUT_TARGET_NAME} ${copy_TARGET_NAME} PARENT_SCOPE)

	if(TARGET ${copy_TARGET_NAME})
		return()
	endif()
	
	get_filename_component(dest_dir ${src_path} DIRECTORY)

	# create a command that will copy this file
	add_custom_command(
		OUTPUT ${copy_DEST_PATH}
		COMMENT "Copying ${copy_SRC_PATH} -> ${copy_DEST_PATH}"
		COMMAND ${CMAKE_COMMAND} -E make_directory ${dest_dir}
		COMMAND ${CMAKE_COMMAND} -E copy ${copy_SRC_PATH} ${copy_DEST_PATH}
	)

	# create a target that will run the copy command if the destination file is out of date
	add_custom_target(
		${copy_TARGET_NAME}
		DEPENDS ${copy_DEST_PATH}
	)
endfunction()

function(add_c2xml_target)
	cmake_parse_arguments(c2xml "" "REKO_ARCH;REKO_ENV;DIALECT;OUTPUT;TARGET_NAME;HEADER" "REQUIRED_ARCHITECTURES;REQUIRED_ENVIRONMENTS" ${ARGN})

	#csproj_output_path(
	#	EXE
	#	PROJECT_SUBDIR "tools"
	#	PROJECT_NAME "c2xml"
	#	RESULT c2xml_exe
	#)
	#get_filename_component(c2xml_dir ${c2xml_exe} DIRECTORY)

	set(c2xml_dir ${CMAKE_BINARY_DIR}/C2Xml/${REKO_EXE_FRAMEWORK})

	RUN_DOTNET(${REKO_SRC}/tools/c2xml/C2Xml.csproj
		NO_RESTORE_PRE
		NO_CLEAN
		CONFIG ${CMAKE_BUILD_TYPE}
		PLATFORM ${REKO_PLATFORM}
		ARGUMENTS
			-a "${c2xml_REKO_ARCH}"
			-e "${c2xml_REKO_ENV}"
			-d "${c2xml_DIALECT}"
			${c2xml_HEADER}
			${c2xml_OUTPUT}
		SOURCES ${c2xml_HEADER}
		OUTPUT ${c2xml_OUTPUT}
		TARGET_NAME ${c2xml_TARGET_NAME}
	)

	## start off with reko.config
	set(file_list "${REKO_SRC}/Drivers/reko.config")

	## collect arch dlls
	foreach(arch ${c2xml_REQUIRED_ARCHITECTURES})
		csproj_output_path(
			LIB
			PROJECT_SUBDIR "Arch"
			PROJECT_NAME "${arch}"
			OUTPUT_PREFIX "Reko.Arch."
			RESULT arch_dll
		)
		message("[arch] ${arch} => ${arch_dll}")
		list(APPEND file_list ${arch_dll})
	endforeach()

	## collect env dlls
	foreach(env ${c2xml_REQUIRED_ENVIRONMENTS})
		csproj_output_path(
			LIB
			PROJECT_SUBDIR "Environments"
			PROJECT_NAME "${env}"
			OUTPUT_PREFIX "Reko.Environments."
			RESULT env_dll
		)
		message("[env] ${env} => ${env_dll}")
		list(APPEND file_list ${env_dll})
	endforeach()

	foreach(src_path ${file_list})
		# use the filename without extension as the copy target name
		get_filename_component(filename_we "${src_path}" NAME_WE)
		get_filename_component(filename "${src_path}" NAME)

		set(dest_path ${c2xml_dir}/${filename})

		add_copy_target(
			SRC_PATH "${src_path}"
			DEST_PATH "${dest_path}"
			OUT_TARGET_NAME copy_target_name
		)

		# we need to copy the assemblies before we can run c2xml
		add_dependencies(${c2xml_TARGET_NAME} ${copy_target_name})
	endforeach()
endfunction()

set(GUI_BASEDIR ${REKO_SRC}/Gui)
RUN_DOTNET(${REKO_SRC}/tools/xslt/xslt.csproj
	NO_RESTORE_PRE
	NO_CLEAN
	CONFIG ${CMAKE_BUILD_TYPE}
	PLATFORM ${REKO_PLATFORM}
	ARGUMENTS
		${GUI_BASEDIR}/cmdids.xslt
		${GUI_BASEDIR}/decompiler-menus.xml
		${GUI_BASEDIR}/CmdIds.cs
	SOURCES
		${GUI_BASEDIR}/cmdids.xslt
		${GUI_BASEDIR}/decompiler-menus.xml
	OUTPUT
		${GUI_BASEDIR}/CmdIds.cs
	TARGET_NAME "gen_decompiler_menus"
)

csproj_output_path(
	LIB
	PROJECT_NAME "Core"
	OUTPUT_PREFIX "Reko."
	RESULT reko_core_dll
)

RUN_DOTNET(${REKO_SRC}/tools/hdrgen/hdrgen.csproj
	NO_RESTORE_PRE
	NO_CLEAN
	CONFIG ${CMAKE_BUILD_TYPE}
	PLATFORM ${REKO_PLATFORM}
	ARGUMENTS
		${reko_core_dll}
		${REKO_SRC}/Native/include/reko.h
	SOURCES
		${reko_core_dll}
	OUTPUT
		${REKO_SRC}/Native/include/reko.h
	TARGET_NAME "run_hdrgen"
)

function(add_c2xml_target_windows main_target header)
	string(REPLACE "/" "_" target ${header})
	set(target_name "gen_c2xml_windows_${target}")

	add_c2xml_target(
		REKO_ARCH "x86-protected-32"
		REKO_ENV "win32"
		REQUIRED_ARCHITECTURES "X86"
		REQUIRED_ENVIRONMENTS "Windows"
		DIALECT "msvc"
		TARGET_NAME "${target_name}"
		HEADER ${REKO_SRC}/Environments/Windows/${header}.inc
		OUTPUT ${REKO_SRC}/Environments/Windows/${header}.xml
	)
	add_dependencies(${main_target} ${target_name})
endfunction()

function(add_c2xml_target_os2 main_target header)
	string(REPLACE "/" "_" target ${header})
	set(target_name "gen_c2xml_windows_${target}")

	add_c2xml_target(
		REKO_ARCH "x86-protected-32"
		REKO_ENV "os2-16"
		REQUIRED_ARCHITECTURES "X86"
		REQUIRED_ENVIRONMENTS "OS2"
		DIALECT "msvc"
		TARGET_NAME "${target_name}"
		HEADER ${REKO_SRC}/Environments/OS2/${header}.inc
		OUTPUT ${REKO_SRC}/Environments/OS2/${header}.xml
	)
	add_dependencies(${main_target} ${target_name})
endfunction()

function(add_c2xml_target_sysv main_target header)
	string(REPLACE "/" "_" target ${header})
	set(target_name "gen_c2xml_sysv_${target}")

	add_c2xml_target(
		REKO_ARCH "x86-protected-32"
		REKO_ENV "elf-neutral"
		REQUIRED_ARCHITECTURES "X86"
		REQUIRED_ENVIRONMENTS "SysV"
		DIALECT "gcc"
		TARGET_NAME "${target_name}"
		HEADER ${REKO_SRC}/Environments/SysV/${header}.inc
		OUTPUT ${REKO_SRC}/Environments/SysV/${header}.xml
	)
	add_dependencies(${main_target} ${target_name})
endfunction()

add_custom_target(gen_c2xml_windows)
add_c2xml_target_windows(gen_c2xml_windows msvcrt)
add_c2xml_target_windows(gen_c2xml_windows ntoskrnl)

add_custom_target(gen_c2xml_os2)
add_c2xml_target_os2(gen_c2xml_os2 os2-16)
add_c2xml_target_os2(gen_c2xml_os2 16/fsu)
add_c2xml_target_os2(gen_c2xml_os2 16/fsh)
add_c2xml_target_os2(gen_c2xml_os2 16/fsd)

add_custom_target(gen_c2xml_sysv)
add_c2xml_target_sysv(gen_c2xml_sysv lp32)
add_c2xml_target_sysv(gen_c2xml_sysv libc.so)
add_c2xml_target_sysv(gen_c2xml_sysv Xlib)
add_c2xml_target_sysv(gen_c2xml_sysv opengl)

add_custom_target(nativeproxy_clean
	COMMENT "Cleaning NativeProxy..."
	WORKING_DIRECTORY ${REKO_SRC}/Native
	COMMAND ${CMAKE_COMMAND}
		-DREKO_PATH=${REKO_SRC}/../
		-DREKO_PLATFORM=${REKO_PLATFORM}
		-DCMAKE_BUILD_TYPE=${CMAKE_BUILD_TYPE}
		-DACTION=clean
		-P ${REKO_SRC}/Native/reko.cmake
)

add_custom_target(nativeproxy_build
	COMMENT "Building NativeProxy..."
	WORKING_DIRECTORY ${REKO_SRC}/Native
	COMMAND ${CMAKE_COMMAND}
		-DREKO_PATH=${REKO_SRC}/../
		-DREKO_PLATFORM=${REKO_PLATFORM}
		-DCMAKE_BUILD_TYPE=${CMAKE_BUILD_TYPE}
		-P ${REKO_SRC}/Native/reko.cmake
)