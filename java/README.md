# Document SDK Java samples

This repository contains Java samples of **id3 Technologies** Document SDK.

## Requirements

First of all you must follow the upper README steps to get a license file and install the SDK.

A Java environnment must be available on the system with java and javac tools in path.

Once everything is setup you can proceed to the following steps.

## Building the solution

### Filling the license path

Before to build any of the JAVA samples, you need to fill in the path to your license in the source code. Look for the following line in **DocumentCLI.java** and replace expression 'your_license_path_here' with your correct path.

### Ensuring models and templates are present

Following models are required to be in the sdk/models/ directory:

    - document_detector_v2b.id3nn
    - mrz_reader_v2a.id3nn

Following document templates are required to be in the sdk/document_templates/ directory:

    - ALB_BO_01001_detector_2B_2.2.0.0.id3dr

### Build and run

Samples build will directly use the java and javac tools.
For conveniency a .bat windows script and a .sh linux script are provided.
