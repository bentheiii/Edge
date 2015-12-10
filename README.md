#Edge
Edge- the C# Companion Library

Hello!
Edge is a general-purpose library for c# made to improve usability in the language. It is currently a WIP and meant for private use, but can be used by anyone who finds it useful. The many modules of Edge are:

###Array Extentions
This static class (`Edge.Arrays.arrayExtensions`) offers many extension methods for arrays, dictionaries and enumerables that makes processing them easier. 

---
###Rotator Arrays
A Rotator Array is a special array-like list that can shift its base left or right and access indexes outside of its range (using modulo, so `array[-1]` will refer to the last element).

---
###Sparse Array
A Sparse Array is an array suited for arrays which are mostly the same value, it supports multiple Ranks.

---
###Array2D
An extension class for multi-Rank Arrays

---
###Symmetrical Matrix
A special kind of two-dimensional array with the special property that `a[x,y] == a[y,x]`. saves you half the space if you're only gonna use it. Can be either reflexive or not reflexive (whether element `a[x,x]as exists or not)

---
###Assert Extensions
A static class that's meant to help with asserting by utilizing Fields (see below)

---
###Color Extensions
Yet another Extension class (get used to them) useful for manipulating colors

---
###Color Collections
A class of read-only-lists that store often-used colors.

---
###Comparison
A collection of useful comparers and equality comparers

---
###Complex
A complex number, complete with complicated math functions, two difference comparers, and parsing.

---
###Control Mouse Dragger
allows to easily move a winforms control with a mouse drag.

---
###Control Extensions
Extensions to make common winforms controls easier.

---
###Window
This namespace includes a lot of utility objects for windows features for winforms (taskbar progress bar, flashing window, and so on).

---
###Credentials
A module designed for access control, includes multiple credential classes and credential validator classes.

---
###XPath Marksman
A module to handle XML loading and searching with quick namespace assignment.

---
###Entity Manager
A module to help with LiteDB updating

---
###Keyboard
A module to detect key presses.

---
###DotNet Framework
A module to detect what .Net frameworks are installed.

---
###Platform
A module to detect the platform of the current machine.

----
###Disk
A module to see disk space usages on the current machine.

---
###Screen Lens
A module to capture screens and windows to image.

---
###Guard
A guard is a mutable wrapper for immutable types

---
###Event Guard
An Event Guard is a Guard wrapped with events for being set, get, and both.

---
###ICreator
A general interface for factory patterns.

---
###Fields
Fields are a powerful tool for general arithmatic, capable of treating operators for generic variables. Fields are already predefined for most common types and all types that have operators are supported through dynamic, general-purpose fields, but you can define your own for your own for your classes.

---
###Field Wrapper
Can wrap any object to support Field-dependent operations.

---
###Formulas
A generic formula structure that handles complicated operations. The formula module uses Fields to supports any type.

---
###Funnel
A chain-of-responsibility-like structure that's more powerful and (arguably) prettier than switches.

---
###Conditional Funnel/ Qualifier Funnel/ Prefix Funnel/ Type Funnel
Wrappers for the Funnel to make it more friendly for specific usages.

---
###Graphs
A structure for handling trees and graphs, directed and undirected. Including multiple search and path-finding functions

---
###Image Extensions
Extensions for image handling.

---
###Lock Bitmap
A data structure to ease locking bitmaps. courtesy of [this codeproject](http://www.codeproject.com/Tips/240428/Work-with-bitmap-faster-with-Csharp).

---
###Loops
A very powerful LINQ-style extension class that makes it very easy to work with enumerables.

---
###Matrices
A Field-supporting matrix module that supports lazily-initialized and constant-size matrices

---
###Modulary
A structure that supports modulo arithmatic

---
###Web Guard
A static class that assists with minor web-based operations

---
###Number Magic
A static class that can handle complicated numberical operations.

---
###Params
A powerful engine for delivering multiple strings through a single string.

---
###Load Files
A static class to load data from files and streams.

---
###File Thumbnails
A static class to get the thumbnail of a file.

---
###File Path
A static class to process files and paths.

---
###Perma Object
A powerful object type that stores the data in a file for it to be used across processes and instances.

---
###Ports
Wrapper classes for UDP and TCP ports that can send and receive any object, not just bytes.

---
###Receiver Threads and Listener Threads
A thread designed to handle connections

---
###MultiSocket
A socket that multiple processes can hook up to.

---
###Processor Monitor
A static class that handles other processes and console windows.

---
###Routine
A static class that has the Timeout method, that can stop code if it takes too long.

---
###New Processes
A static class that helps with opening new processes.

---
###Random Generator
Structures that support generating random numbers.

---
###Serialization
A static class that helps out with object serialization.

---
###Hashing
A static class with consistent string hashing.

---
###Series
A structure to represent a mathematical series.

---
###Point Extensions
An extension class for points and pointf.

---
###Line/ Line Segment/ Ray
A structure to represent an infinite or finite straight line.

---
###Polygons
A structure to represents a polygon on a 2D plane.

---
###Roller Num
A structure that stores a number that wraps between two bounds. Uses fields.

---
###Bound Num
A structure that stores a number that maxes and mins between two bounds. Uses fields.

---
###Inflater Num
A structure that stores it own ever highest and lowest values.  Uses fields.

---
###Discrete Statistic
A structure that can store a statistic and return the expected value and variance.

---
###Smart Discrete Statistic
A structure like a regular statistic that makes so that repeating values are less likely.

---
###Dice
A structure used to represent dice, get a random roll, maximum/minimum values and the probabilities for a result.

---
###Split Stream
An object to store and write to multiple writers at the same time.

---
###Stream Extensions
An extension class for textreader

---
###Symbols
A structure to represent real numbers (and able to approximate them to a BigRational)

---
###System Extensions
An extension class to make class methods (especially mathematical) more accessible.

---
###Cloning
An extension class to help with mutable objects

---
###ObjId Extensions/Id Distributor
Classes to generalize Object ID Generation.

---
###Theater
Classes to make small-scale graphics and animation to an image.

---
###Generic Thread/ Return Thread
Wrappers to make threading more bearable (will probably delete this one)

---
###Idle Timer
An object to act as a stopwatch and keep execution time since it was reset or initiated.

---
###Active Timer
A thread that performs an action at a set interval.

---
###Units
Structures to hold on to and convert many units and measurements.

---
###Word Play
An extension class to help with common string operations.

---
###Encryption
A wrapper to help with Triple-DES encryption.

---
###Parser
A class to help with parsing through regex

---
I know this is a lot, splitting this project is on the to-do list, as well as better documentation.
