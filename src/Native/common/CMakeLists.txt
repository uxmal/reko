set(CMAKE_CXX_STANDARD 14)

add_library(commom
	compact.c guids.cpp
)
set_property(TARGET common PROPERTY POSITION_INDEPENDENT_CODE ON)
