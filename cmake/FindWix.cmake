# Redistribution and use is allowed under the OSI-approved 3-clause BSD license.
# Copyright (c) 2018 Apriorit Inc. All rights reserved.

if(CMAKE_SIZEOF_VOID_P EQUAL 8)
    set(WIX_ARCH "x64")
else()
    set(WIX_ARCH "x86")
endif()

file(GLOB WIX_FILES
    "C:/Program Files*/WiX Toolset v*/bin"
)

if(WIX_FILES)
    list(GET WIX_FILES -1 WIX_LATEST_FILE)
endif()

unset(${WIX_FILES})

include(FindPackageHandleStandardArgs)
find_package_handle_standard_args(WIX REQUIRED_VARS WIX_LATEST_FILE)

if(NOT WIX_LATEST_FILE)
    return()
endif()

get_filename_component(WIX_ROOT ${WIX_LATEST_FILE} DIRECTORY)
message(STATUS "WIX_ROOT: " ${WIX_ROOT})

unset(${WIX_LATEST_FILE})

function(wix_add_project _target)
    cmake_parse_arguments(WIX "" "OUTPUT_NAME" "EXTENSIONS;DEPENDS" ${ARGN})

    if("${WIX_OUTPUT_NAME}" STREQUAL "")
        set(WIX_OUTPUT_NAME "${_target}.msi")
    endif()

    if(NOT ("${CMAKE_RUNTIME_OUTPUT_DIRECTORY}" STREQUAL ""))
        set(WIX_OUTPUT_NAME ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/${WIX_OUTPUT_NAME})
    endif()

    foreach(currentFILE ${WIX_UNPARSED_ARGUMENTS})
        GET_FILENAME_COMPONENT(SOURCE_FILE_NAME ${currentFILE} NAME_WE)
        list(APPEND WIXOBJ_LIST "${SOURCE_FILE_NAME}.wixobj")
        list(APPEND WIX_SOURCES_LIST ${CMAKE_CURRENT_LIST_DIR}/${currentFILE})
    endforeach()

    foreach(current_WIX_EXTENSION ${WIX_EXTENSIONS})
        list(APPEND EXTENSION_LIST "-ext")
        list(APPEND EXTENSION_LIST ${current_WIX_EXTENSION})
    endforeach()

    # Call WiX compiler
    add_custom_command(
        OUTPUT ${CMAKE_CURRENT_BINARY_DIR}/${WIXOBJ_LIST}
        COMMAND "${WIX_ROOT}/bin/candle.exe" -arch ${WIX_ARCH} ${WIX_COMPILE_FLAGS} -o "${CMAKE_CURRENT_BINARY_DIR}/" ${WIX_SOURCES_LIST} -I"${CMAKE_CURRENT_BINARY_DIR}" -I"${CMAKE_CURRENT_BINARY_DIR}/wxi/$<CONFIG>"
        DEPENDS ${WIX_SOURCES_LIST}
        COMMENT "Compiling to wixobj file(s)"
        )

    # Link MSI file
    add_custom_command(
        OUTPUT ${WIX_OUTPUT_NAME}
        COMMAND "${WIX_ROOT}/bin/light.exe" ${WIX_LINK_FLAGS} -o ${WIX_OUTPUT_NAME} ${WIXOBJ_LIST} ${EXTENSION_LIST}
        DEPENDS ${WIXOBJ_LIST} ${WIX_DEPENDS}
        COMMENT "Linking to ${WIX_OUTPUT_NAME} file"
        )

    add_custom_target(${_target} ALL
        DEPENDS ${WIX_OUTPUT_NAME}
        SOURCES ${WIX_UNPARSED_ARGUMENTS}
        )

    get_cmake_property(WIX_variableNames VARIABLES)
    list(REMOVE_DUPLICATES WIX_variableNames)
    list(REMOVE_ITEM WIX_variableNames "CMAKE_BUILD_TYPE")
    string(CONCAT VARS_FILE "<?xml version='1.0' encoding='UTF-8'?>\n\n<Include>\n")
    # handle CMAKE_BUILD_TYPE in a special way to support multiconfiguration generators
    string(CONCAT VARS_FILE ${VARS_FILE} "\t<?define CMAKE_BUILD_TYPE='$<CONFIG>' ?>\n")
    foreach(WIX_variableName ${WIX_variableNames})
        string(REPLACE "$" "$$" OUT "${WIX_variableName}='${${WIX_variableName}}'")
        string(CONCAT VARS_FILE ${VARS_FILE} "\t<?define ${OUT} ?>\n")
    endforeach()
    string(CONCAT VARS_FILE ${VARS_FILE} "</Include>")
    file(GENERATE OUTPUT "${CMAKE_CURRENT_BINARY_DIR}/wxi/$<CONFIG>/vars.wxi" CONTENT "${VARS_FILE}")

    string(CONCAT DEPENDS_FILE "<?xml version='1.0' encoding='UTF-8'?>\n\n<Include>\n")
    foreach(current_depends ${WIX_DEPENDS})
        string(CONCAT DEPENDS_FILE ${DEPENDS_FILE} "\t<?define TARGET_FILE:${current_depends}='$<TARGET_FILE:${current_depends}>' ?>\n")
        # skip pdb for csharp projects as it says: "TARGET_PDB_FILE is not supported by the target linker"
        string(CONCAT DEPENDS_FILE ${DEPENDS_FILE} "\t<?define TARGET_PDB_FILE:${current_depends}='$<$<STREQUAL:$<TARGET_PROPERTY:${current_depends},VS_DOTNET_REFERENCES>,>:$<TARGET_PDB_FILE:${current_depends}>>' ?>\n")
    endforeach()
    string(CONCAT DEPENDS_FILE ${DEPENDS_FILE} "</Include>")
    file(GENERATE OUTPUT "${CMAKE_CURRENT_BINARY_DIR}/wxi/$<CONFIG>/depends.wxi" CONTENT "${DEPENDS_FILE}")
endfunction()
