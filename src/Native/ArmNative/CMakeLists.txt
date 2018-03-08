file(GLOB SOURCES *.cpp)

add_library(ArmNative SHARED ${SOURCES})

target_compile_definitions(ArmNative PUBLIC UNICODE)
target_link_libraries(ArmNative common ${CAPSTONE_LIBRARIES})

set_output_dir(ArmNative ${CMAKE_CURRENT_BINARY_DIR})

