function(set_output_dir target outdir)
	foreach( OUTPUTCONFIG ${CMAKE_CONFIGURATION_TYPES} )
		string( TOUPPER ${OUTPUTCONFIG} OUTPUTCONFIG )
		set_target_properties(${target}
			PROPERTIES
			RUNTIME_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${outdir}
			LIBRARY_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${outdir}
			ARCHIVE_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${outdir}
		)
	endforeach( OUTPUTCONFIG CMAKE_CONFIGURATION_TYPES )
endfunction()

if(NOT QUICK_CONFIGURE)
	if(CMAKE_SIZEOF_VOID_P EQUAL 8)
		set(IS_AMD64 ON)
	else()
		set(IS_AMD64 OFF)
	endif()
endif()

if(REKO_PLATFORM STREQUAL "x86")
	set(REKO_PLATFORM_X86 ON)
	if(QUICK_CONFIGURE)
		set(IS_AMD64 OFF)
	endif()
elseif(REKO_PLATFORM STREQUAL "x64")
	set(REKO_PLATFORM_X64 ON)
	if(QUICK_CONFIGURE)
		set(IS_AMD64 ON)
	elseif(NOT IS_AMD64)
		message(FATAL_ERROR "x64 builds not supported on 32-bit hosts")
	endif()
elseif(NOT DEFINED REKO_PLATFORM)
	if(IS_AMD64)
		set(REKO_PLATFORM_X64 ON)
	else()
		set(REKO_PLATFORM_X86 ON)
	endif()
endif()

if(REKO_PLATFORM_X86 AND NOT MSVC)
	set(CMAKE_C_FLAGS -m32)
	set(CMAKE_CXX_FLAGS -m32)
endif()

if(CMAKE_BUILD_TYPE STREQUAL "")
	set(CMAKE_BUILD_TYPE Debug)
endif()