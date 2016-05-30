#pragma once

#include <collection.h>
#include <ppltasks.h>
#ifndef TRACE
#ifdef _DEBUG
#include <stdio.h>
inline void TRACE(WCHAR const * const format, ...)
{
	va_list args;
	va_start(args, format);
	WCHAR output[512];
	vswprintf_s(output, format, args);
	OutputDebugStringW(output);
	va_end(args);
}
#else
#define TRACE __noop
#endif
#endif

#ifndef ASSERT
#define ASSERT(cond) _ASSERTE(cond)
#if WINAPI_FAMILY_PHONE_APP == WINAPI_FAMILY
#ifdef _DEBUG
#undef ASSERT
#define ASSERT(expression) { if (!(expression)) throw Platform::Exception::CreateException(E_FAIL); }
#endif
#endif
#endif