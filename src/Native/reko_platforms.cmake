# Platform-specific CMake settings go here.

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
		set(IS_64BIT ON)
	else()
		set(IS_64BIT OFF)
	endif()
endif()

if(REKO_PLATFORM STREQUAL "x86")
	set(REKO_PLATFORM_X86 ON)
	if(QUICK_CONFIGURE)
		set(IS_64BIT OFF)
	endif()
elseif(REKO_PLATFORM STREQUAL "x64")
	set(REKO_PLATFORM_X64 ON)
	if(QUICK_CONFIGURE)
		set(IS_64BIT ON)
	elseif(NOT IS_64BIT)
		message(FATAL_ERROR "x64 builds not supported on 32-bit hosts")
	endif()
elseif(NOT DEFINED REKO_PLATFORM OR REKO_PLATFORM STREQUAL "AnyCPU")
	if(IS_64BIT)
		set(REKO_PLATFORM_X64 ON)
	else()
		set(REKO_PLATFORM_X86 ON)
	endif()
endif()

if(REKO_PLATFORM_X86)
	if (MSVC)
		set(CMAKE_C_FLAGS /Zi)
		set(CMAKE_CXX_FLAGS /Zi)
	else()
		set(CMAKE_C_FLAGS -m32)
		set(CMAKE_CXX_FLAGS -m32)
	endif()
endif()

# this is a temp variable
unset(IS_64BIT)

if(CMAKE_BUILD_TYPE STREQUAL "")
	set(CMAKE_BUILD_TYPE Debug)
endif()