function(CheckGitWrite cache_file git_hash)
    file(WRITE ${cache_file} ${git_hash})
endfunction()

function(CheckGitRead cache_file git_hash)
    if (EXISTS ${cache_file})
        file(STRINGS ${cache_file} CONTENT)
        LIST(GET CONTENT 0 var)

        set(${git_hash} ${var} PARENT_SCOPE)
    endif()
endfunction()


function(CheckGitVersion)
    cmake_parse_arguments(git "SCRIPT_MODE" "SCRIPT_FILE;CACHE_FILE;REPO_DIR;TEMPLATE_FILE;OUTPUT_FILE" "" ${ARGN})

    if(NOT git_SCRIPT_MODE)
        add_custom_target(AlwaysCheckGit
        COMMAND ${CMAKE_COMMAND}
            -DGIT_SCRIPT=1
            -DGIT_CACHE_FILE=${git_CACHE_FILE}
            -DGIT_TEMPLATE_FILE=${git_TEMPLATE_FILE}
            -DGIT_OUTPUT_FILE=${git_OUTPUT_FILE}
            -DGIT_HASH_CACHE=${GIT_HASH_CACHE}
            # this is the path to this running script
            # it could be obtained with CMAKE_CURRENT_FUNCTION_LIST_DIR, but requires CMake 3.17
            -P ${git_SCRIPT_FILE}
            BYPRODUCTS ${git_OUTPUT_FILE}
        )
        return()
    endif()

    find_program(GIT_PROGRAM git REQUIRED)

    if(NOT git_REPO_DIR)
        set(git_REPO_DIR ${CMAKE_CURRENT_LIST_DIR})
    endif()

    if(NOT git_CACHE_FILE)
        set(git_CACHE_FILE ${CMAKE_CURRENT_BINARY_DIR}/git-state.txt)
    endif()

    message(STATUS "Checking git hash cache: ${git_CACHE_FILE}")

    # Get the latest abbreviated commit hash of the working branch
    execute_process(
        COMMAND ${GIT_PROGRAM} rev-parse --short HEAD
        WORKING_DIRECTORY ${git_REPO_DIR}
        OUTPUT_VARIABLE GIT_HASH
        OUTPUT_STRIP_TRAILING_WHITESPACE
    )

    message(STATUS "git hash: ${GIT_HASH}")

    CheckGitRead(${git_CACHE_FILE} GIT_HASH_CACHE)

    get_filename_component(output_dir "${git_OUTPUT_FILE}" DIRECTORY)
    if (NOT EXISTS ${output_dir})
        file(MAKE_DIRECTORY ${output_dir})
    endif ()

    # Only update the output file if the hash has changed. This will
    # prevent us from rebuilding the project more than we need to.
    if (NOT "${GIT_HASH}" STREQUAL "${GIT_HASH_CACHE}" OR NOT EXISTS ${git_OUTPUT_FILE})
        # Set che GIT_HASH_CACHE variable the next build won't have
        # to regenerate the source file.
        CheckGitWrite(${git_CACHE_FILE} ${GIT_HASH})

        message(STATUS "Generating ${git_OUTPUT_FILE}")
        configure_file(${git_TEMPLATE_FILE} ${git_OUTPUT_FILE} @ONLY)
    else()
        message(STATUS "Skipping generation of ${git_OUTPUT_FILE} - up to date")
    endif()
endfunction()

# This is used to run this function from an external cmake process.
if (GIT_SCRIPT)
    CheckGitVersion(
        SCRIPT_MODE
        CACHE_FILE ${GIT_CACHE_FILE}
        TEMPLATE_FILE ${GIT_TEMPLATE_FILE}
        REPO_DIR ${GIT_REPO_DIR}
        OUTPUT_FILE ${GIT_OUTPUT_FILE}
    )
endif()