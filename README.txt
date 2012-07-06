this is a code repository containing (hopefully) up-to-date versions of various 3rd party library packages used by opensim.

OpenSim checks out with most of these libs precompiled in the /bin folder.  In the case of managed code, these will be .dll assemblies that run on any .net/mono platform.  In the case of unmanaged (native) code, ie C/C++ libs with .NET wrappers (ODE, SQlite, openjpeg), there will typically be a *.net.dll wrapper, which invokes either a native .dll (on windows), or a native shared library (.so, on unix).  The binaries shipping in opensim are compiled on windows Vista and Ubuntu 7.04, in a 32-bit x86 environment.  Other operating systems and hardware setups may require you to recompile the libraries and manually copy them to your ./bin folder.  Alternatively, you may want to get certain packages from other sources (such as a distribution management system), install them in places like SYSTEM32 (win32) or /usr/local/lib.

Another reason to use this code is to update, patch, debug, and potentially add features to the native code.  Note that anything other than minor build patches would constitute a fork from the project in question; such a decision should be made in communion with the community (unless you're doing this for yourself and never plan to contribute it back, in which case blah! to you)

-danx0r, 10/19/07
   
