#include "conversion.h"
#include "LinearMath/btVector3.h"
#include "LinearMath/btMatrix3x3.h"
#include "LinearMath/btQuaternion.h"
#include <iostream>

void freeUMMemory(IntPtr pointer, bool arrayType)
{
	if (arrayType)
		delete[] pointer;
	else
		delete pointer;
}

void Pointer_SafeRelease(IntPtr pointer)
{
	
}





#ifdef _MSC_VER
bool fixmarshal(bool val)
{
	__asm mov eax, 100;
	return val;
}
#endif