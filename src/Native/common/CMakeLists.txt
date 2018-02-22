set(SOURCES compat.c)
if(UNIX)
	list(APPEND SOURCES guids.cpp)
endif()

set(CMAKE_CXX_STANDARD 14)

add_library(common ${SOURCES})
set_property(TARGET common PROPERTY POSITION_INDEPENDENT_CODE ON)
