# BMP# (bmp-sharp)

Simple native .NetStandard C# library for handling Bitmap (BMP) files.

## Description

This is simple Bitmap library that helps you wrap binary data (pixels) into BMP header for saving into file and vice versa.

It supports only 24 BGR and 32 bits ABGR byte arrays.

## Why this library

I'm working on a library that will help me parse CIFAR 10 dataset and I wanted to simply display binary image data. Problem was that I couldn't find any .NET library that will take byte[] and with few parameters display it as image (without conversion into float or use Windows.System.Forms/WPF) so I wrote this simple library to help me out.

## Out of scope

- All different headers formats
- Image filtering
- Compression support
- 1/4/8 bits per pixel support (i'm open for pull requests)
- Alpha channels

## History

### 0.2.0 Initial release

- Some bug fixes
- [WiP] Adding RGBA support (32 bits)

### 0.1.0 Initial release

- Added support of saving byte[] to BMP
- Should support reading/saving of little/big endian platforms (x86 / ARM)
