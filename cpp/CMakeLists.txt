cmake_minimum_required(VERSION 2.8.12)

project(id3DocumentSdkCppSamples)

macro(postBuildCopyDirectory target dst_dir)
    set(dirs_list ${ARGN})
    foreach (src_dir ${dirs_list})
        add_custom_command(
                TARGET ${target} POST_BUILD
                COMMAND ${CMAKE_COMMAND} -E copy_directory
                ${src_dir}
                ${dst_dir})
    endforeach ()
endmacro()

set(ID3Document_SDK_ROOT_DIR ${CMAKE_CURRENT_LIST_DIR}/../sdk/)

include_directories(${ID3Document_SDK_ROOT_DIR}include)

if (LINUX_BUILD)
    set(LINK_DIR ${ID3Document_SDK_ROOT_DIR}bin/linux/x64)
elseif (WINDOWS_BUILD)
    set(LINK_DIR ${ID3Document_SDK_ROOT_DIR}bin/windows/x64)
endif ()

link_directories(${LINK_DIR})

add_executable(id3DocumentSampleCLI id3DocumentSampleCLI.cpp)
target_link_libraries(id3DocumentSampleCLI id3Document)

postBuildCopyDirectory(id3DocumentSampleCLI $<TARGET_FILE_DIR:id3DocumentSampleCLI> ${LINK_DIR})
postBuildCopyDirectory(id3DocumentSampleCLI $<TARGET_FILE_DIR:id3DocumentSampleCLI>/data ${CMAKE_CURRENT_LIST_DIR}/../data)
postBuildCopyDirectory(id3DocumentSampleCLI $<TARGET_FILE_DIR:id3DocumentSampleCLI>/models ${CMAKE_CURRENT_LIST_DIR}/../sdk/models)
postBuildCopyDirectory(id3DocumentSampleCLI $<TARGET_FILE_DIR:id3DocumentSampleCLI>/document_references ${CMAKE_CURRENT_LIST_DIR}/../sdk/document_references)