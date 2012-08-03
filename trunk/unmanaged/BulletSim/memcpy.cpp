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

/*
 * As of 20120803, Ubuntu ships with GLIBC version 2.14 while other common Linux's
 * (CentOS, Redhat, ...) are distributed with older versions (2.12, 2.11.3, ...).
 * In GLIBC v2.14, the memcpy routine was modified in a way that broke the
 * undocumented "left to right" memory operation of previous memcpy's.
 *
 * Linking a library on a system with GLIBC v2,14 causes that library to have
 * one symbol dependency on v2.14. All other linkages are to older versions of
 * GLIBC.
 *
 * What this extra file and extra parameters in the Makefile accomplish is to
 * map memcpy to an older version of GLIBC. This makes the resulting library
 * able to run on Linux's that do not have the newer GLIBC version.
 *
 * Someday, the Linux's will catch up with each other and this file and the
 * parameters in the Makefile can be removed.
 *
 * Solution found at:
 *     http://stackoverflow.com/questions/8823267/linking-against-older-symbol-version-in-a-so-file
 * Specificly, the answer http://stackoverflow.com/a/8862631 
 *     by 'anight' (http://stackoverflow.com/users/1149316/anight)
 * Licensed Creative Commons Attribution Share Alike (http://creativecommons.org/licenses/by-sa/2.5/)
 *     as per Stack Overflow TOS (http://stackexchange.com/legal/terms-of-service) as of 20120803
 *   
 */
#include <string.h>

asm (".symver memcpy, memcpy@GLIBC_2.2.5");

void* __wrap_memcpy(void *dest, const void *src, size_t n)
{
	return memcpy(dest, src, n);
}
