# AshLib
<img src="res/icon.png" width="200"/>
Library that comes with multiple utilities

## Documentation
**ALL DOCUMENTATION IS HAND MADE**

For a more detailed explanation, you can visit the [Usage Documentation](./documentation/AshLibUsageDocumentation.pdf)  
For a more practical explanation in the form of examples, you can visit the [Examples](./documentation/examples)  
For a reference on everything, you can visit the [Reference Documentation](./documentation/AshLibReferenceDocumentation.pdf)  

## AshFile
An AshFile is a data structure composed of camps. Each camp has a name and a value. This value can be of different types(numbers, text...)
It also is a file format, with the extension **.ash**. It is very easy to save and load an AshFile from a file, making it a reliable and easy way to use it in programming projects and then store it
You can also use Models to ensure the camps are in a correct format

## DeltaHelper
This little utility will make it easy to calculate fps and deltaTime. You just need to call a method once a frame

## TimeTool
Utility that helps you debug time related stuff(for example, the percentage operations are taking each frame)

## TreeLog
Utility that will help you produce a Collapsible and idented log for making debugging easier. There is a notepad++ custom language in [here](./n++). Note that this custom language is thought for the dark theme

## FormatString
A formatted string, for displaying colored(and more options!) text in the console

## Dates
This is a struct that represents a time, down to seconds, between the years 1488 and 2511.
Its main attraction is you can convert this Date int just 6 base64 printable chars. This is called s CPTF(Compressed printable date format)

## Trees
Trees are graphs made of nodes, that have a value of any type and child nodes. They are useful for a multitude of reasons. There is support for a string format

## Dependencies
This utility helps you handle files in a project, using a folder similar to the .minecraft folder

## Colors and Vectors
This library comes with structs for RGB colors and 2,3 and 4 dimension vectors