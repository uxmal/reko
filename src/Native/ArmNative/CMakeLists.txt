file(GLOB SOURCES *.cpp)

add_library(ArmNative SHARED ${SOURCES})

target_compile_definitions(ArmNative PUBLIC UNICODE)
target_link_libraries(ArmNative ${CAPSTONE_LIBRARIES})
