# C/C++ samples

This CMake project provides the following C/C++ samples: 
* **id3DocumentSampleCLI**

## Build requirements

* CMake >= 2.8.12
* [Linux] gcc
* [Windows] Visual Studio >= 15

## Before to build

There are some source code modifications to make before to build.

### License path

To run the sample you will need a license file. To retrieve this file you can use the CLI tool to get your hardware ID and then use either the windows activation tool to retrieve the file or contact id3 with it.

id3 Document SDK needs to check this license before any other operation. You need to fill in the path to your license in the source files:

    std::string license_path = "your_license_path_here";

### Models directory

id3 Document SDK is based on machine learning methods and requires model files to perform image processing operations. Please read the upper README and the SDK documentation (developer guide) to download models.

For this sample you need:
- document_detector_v1b.id3nn
- mrz_reader_v2a.id3nn

You can put the models wherever you want on your PC and then fill in the path to your models in the source files:
```c++    
std::string models_dir = "../../../sdk/models/";
```

### Documents reference files

id3 Document SDK need specific data to process each identity document, which must be loaded based on your needs. Please read the upper README and the SDK documentation (developer guide) to download document reference files.

For this sample you need:
- alb_id_detection_1b_1.0.0.0.id3dr

You can put the models wherever you want on your PC and then fill in the path to your models in the source files:
```c++    
std::string document_references_dir = "../../../sdk/document_references/";
```


## Linux build steps

### id3DocumentSampleCLI

To build and run **id3DocumentSampleCLI**, use the following command lines:

```bash
mkdir build
cd build
cmake -DLINUX_BUILD=ON ..
make
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:../../../../bin/linux/x64
./id3DocumentSampleCLI
```

## Windows build steps

### id3DocumentSampleCLI

To build and run **id3DocumentSampleCLI**, use the following command lines:
```bat
mkdir build
cd build
cmake -G "Visual Studio 15 2017 Win64" -DWINDOWS_BUILD=ON ..
cmake --build .
cd Debug
.\id3DocumentSampleCLI.exe
```