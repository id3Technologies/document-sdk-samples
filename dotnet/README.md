# Document SDK .NET samples

This repository contains Microsoft .NET samples of **id3 Technologies** Document SDK.

## Requirements

First of all you must follow the upper README steps to get a license file and install the SDK.

.NET samples require **Microsoft Visual Studio 2022** (or more recent) to be installed on your PC.

Once everything is setup you can proceed to the following steps.

## Building the solution

Open the solution file **id3.Document.Samples.sln** with **Microsoft Visual Studio**.

### Setting references

The reference to id3.Document package should already be set up in each project.

Also, for the Windows Form projects, references to **OpenCvSharp** and **OpenCvSharp.Extensions** should be set up as well to handle a webcam. The IDE should offer you to download the Nuget packages if necessary. Follow its instructions.

### Filling the license path

Before to build any of the .NET samples, you need to fill in the path to your license in the source code. Look for the following line in **Program.cs** or **Form1.cs** and replace expression 'your_license_path_here' with your correct path.

    DocumentLicense.CheckLicense(@"your_license_path_here");

Once everything is ready, you can now build the samples and launch them.
