# BMP# (bmp-sharp)

Simple .NetStandard C# library for handling BMP files

## Why this library

I'm working on a library that will help me parse CIFAR 10 dataset and I wanted to simply display binary image data. Problem was that I couldn't find any .NET library that will take byte[] and with few parameters display it as image (without conversion into float or use Windows.System.Forms/WPF) so I wrote this simple library to help me out.

## Plan

Write platform independent (.NET Standard) library without any additional dependencies (if possible) that will help with BMP files.

## Out of scope

- Image filtering

## Wishfull thinking

- Implement all BMP compression algorithms (maybe at the end)

## Not planning to implement by myself

- Support all BMP formats (different byte ordering etc)
- Color profiles
- Alpha channes
- ...
