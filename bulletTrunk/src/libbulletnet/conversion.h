#ifndef CONVERSION_H
#define CONVERSION_H

#include <stdlib.h>

#include <stdio.h>
#include <iostream>
#include <wchar.h>
#include <string>

#ifdef WIN32
#define STDCALL __stdcall*
#define uint unsigned int
#else
#define STDCALL *
#endif

//This type's name is taken from dotNET's match
//Of course in C++ it is just a basic pointer but it is quite more in C#
typedef void* IntPtr;

//Special header for Microsoft's Visual Studio
//Needed for DllImport
//Of course for a Linux compilation, nothing is needed that's why we almost undefine the EXPORT macro
#ifdef _MSC_VER
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif

//Please, DO NOT THINK I AM MAD, this is needed for a pointless bug
//In Visual Studio 8
#ifdef _MSC_VER
bool fixmarshal(bool val);
#define _FIX_BOOL_MARSHAL_BUG(val) return fixmarshal(val);
#else
#define _FIX_BOOL_MARSHAL_BUG(val) return val;
#endif

extern "C" { EXPORT void Pointer_SafeRelease(IntPtr pointer); }


extern "C"
{
	EXPORT void freeUMMemory(IntPtr pointer, bool arrayType);
}


#endif