/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyrightD
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

#pragma once

#ifndef UTIL_H
#define UTIL_H

// This routine exists for two reasons:
// 1) compiler folk say this is an implementation that the compiler can
//    convert into machine specific optimizations (like SSE2 on x86);
// 2) memcpy changed implementation in glibc 2.14. As of 20120806, when
//    linked on an Ubuntu system, the resulting .so will require that
//    version of the library. Other Linux distributions don't have this
//    new glibc yet so the .so's won't run there. Thus, our own implementation
//    to remove that library dependency.
static void* __wrap_memcpy(void* dst, void* src, size_t siz)
{
	char* cdst = (char*)dst;
	const char* csrc = (char*)src;
	size_t n = siz;
	for (; 0<n; --n) *cdst++ = *csrc++;
	return dst;
}

static void* __real_memcpy(void* dst, void* src, size_t siz)
{
	char* cdst = (char*)dst;
	const char* csrc = (char*)src;
	size_t n = siz;
	for (; 0<n; --n) *cdst++ = *csrc++;
	return dst;
}

#endif
