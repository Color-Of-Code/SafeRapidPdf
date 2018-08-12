# SafeRapidPdf

## CI-Status

[![Travis build status](https://img.shields.io/travis/jdehaan/SafeRapidPdf/master.svg?label=master&style=flat-square)](https://travis-ci.org/jdehaan/SafeRapidPdf)


## Introduction

There is already a very good pdf parser and generator: [itextsharp] (http://itextpdf.com/).
But it doesn't focus on parsing and its licensing model makes it inappropriate for some purposes.
This designed and developped from scratch library is provided under the liberal MIT license.

The focus of the library is on reading and parsing, not on writing.

The goals followed are:

 - parsing and analysing PDF contents (virus check for example)
 - integrality of parsing (document scans from start to end gathering all objects)
 - no quirks, invalid PDFs are not parsed
 - allow extraction of text and images at a very low level

This library is not intended for following purposes:

 - rendering a PDF
 - modifiying a PDF
 - generating a PDF

## File structure
 
This library attempts to provide a quick and yet reliable parser for PDF files. It focusses
on an integral parsing of the whole PDF into its primitive objects.

* Strings
* Numeric values
* Booleans
* Streams
* Arrays
* Dictionaries
* Indirect Objects
* Indirect References
* Cross Reference sections

## Document structure

The interpretation layer allows then a decomposition into pages and images among other
high level objects.

* Cross reference table
* Root
* Pages
* Graphics
* Text
* Fonts
 
The library is not interested in rendering the PDF only the informative parts will be
extracted such as the position and size of text and graphics for example.

## Online resources

 - Wikipedia explanations on [the PDF format](http://en.wikipedia.org/wiki/Portable_Document_Format)
 - A python library with similar goals:[pdf-parser] (http://blog.didierstevens.com/programs/pdf-tools/)

It is recommended to read the specification of the PDF language 1.7 for a deeper insight.
 
## Authors

The SafeRapidPdf contributors:

 - Jaap de Haan (initiator)

## License

The MIT license (Refer to the [LICENSE.md](https://github.com/jdehaan/SafeRapidPdf/blob/master/LICENSE.md) file)
