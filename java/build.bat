rem Build the sample with the jar from the sdk
javac -cp ".;../sdk/java/eu.id3.document.jar" DocumentCLI.java

rem Copy native library to current directory
copy ..\\sdk\\bin\\windows\\x64\\id3Document.dll id3Document.dll
rem Run the samples
java -cp ".;../sdk/java/eu.id3.document.jar" DocumentCLI