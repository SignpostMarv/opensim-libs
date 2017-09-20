OPENSIM README
this is version ODE-OpenSim.0.13.3

The ODE code in this repository correspondes to ODE release 0.13.1 r1902 with selected adictions from more recent vrsions and modifications by opensim 

= BUILD INSTRUCTIONS =

== WINDOWS ==
- open a comand prompt and change dir to ..\build and run:
for windows 32bits target:
	premake4 --only-single --platform=x32 --no-threading-intf  vs2008	
for windows 64bits target:
	premake4 --only-single --platform=x64 --no-threading-intf vs2008	
this will create a solution ode.sln for visual studio 2008 in build/vs2008

- open the ode.sln solution in visual studio 2008
- select the ReleaseDLL configuration, and platform (win32 or x64) acording to the target
- do a (menu)Build/Rebuild Solution
the ode.dll should be present in lib/ReleaseDLL
copy it to opensim bin/lib32 or bin/lib64 acording to platform

warning: current solution makes no distintion on platform and so compiles both to same locations. The ode.dll present at lib/ 
will be for the last platform compiled.

optionally you can produce a Debug version, selecting DebugDLL. Result will be at lib/DebugDLL. copy ode.dll and ode.pdb 
to bin/lib32 or bin/lib64 opensim folder acording to platform.
C++ debug does have a large impact on performance. You should only use it for testing. 

== On Linux ==
if you dont see the file ./configure you need to do
./bootstrap
to create it. Check it so see its dependencies on several linux tools.
you may need to do chmod +x bootstrap before since git keeps losing it

(could not test following adapted from justin instructions bellow)

== On Linux 32-bit ==
./configure --disable-asserts --enable-shared --disable-threading-intf 
make
cp ode/src/.libs/libode.so.1.1.1 $OPENSIM/bin/lib32/libode.so	 (possible name is not ..so.1.1.1 )

== On Linux 64-bit ==
./configure --disable-asserts --enable-shared --disable-threading-intf 
make
cp ode/src/.libs/libode.so.1.1.1 $OPENSIM/bin/lib64/libode-x86_64.so (possible name is not ..so.1.1.1 )

== On Linux 64-bit to cross-compile to 32-bit ==
CFLAGS=-m32 CPPFLAGS=-m32 LDFLAGS=-m32 ./configure --build=i686-pc-linux-gnu --disable-asserts --enable-shared --disable-threading-intf
make
cp ode/src/.libs/libode.so.1.1.1 $OPENSIM/bin/lib32/libode.so

you can run strip to remove debug information and reduce file size

you may need to ajdust files bin/Ode.NET.dll.config  and bin/OpenSim.Region.PhysicsModule.ubOde.dll.config


== On Mac OS X Intel 64-bit to compile to a 32-bit, 64-bit Intel and PowerPC universal binary ==
CFLAGS="-g -O2 -isysroot /Developer/SDKs/MacOSX10.6.sdk -arch i386 -arch x86_64 -arch ppc" CXXFLAGS="-g -O2 -isysroot /Developer/SDKs/MacOSX10.6.sdk -arch i386 -arch x86_64 -arch ppc" LDFLAGS="-arch i386 -arch x86_64 -arch ppc" ./configure --disable-asserts --enable-shared --disable-dependency-tracking --disable-threading-intf
make
cp ode/src/.libs/libode.dylib $OPENSIM/bin/lib32/libode.dylib (32bits or )
cp ode/src/.libs/libode.dylib $OPENSIM/bin/lib64/libode.dylib (64bits)


engine ubOde shows ode.dll configuration in console and OpenSim.log similar to:
[ubODE] ode library configuration: ODE_single_precision ODE_OPENSIM OS0.13.3


==old coments: ==

The ODE code in this repository has already had the necessary patches applied to get it to work with OpenSimulator.

These instructions are to rebuild the libraries if necessary.

= BUILD INSTRUCTIONS =

== On Linux 32-bit ==
./configure --with-trimesh=gimpact --disable-asserts --enable-shared --disable-demos --without-x
make
cp ode/src/.libs/libode.so.1.1.1 $OPENSIM/bin/libode.so

== On Linux 64-bit ==
./configure --with-trimesh=gimpact --disable-asserts --enable-shared --disable-demos --without-x
make
cp ode/src/.libs/libode.so.1.1.1 $OPENSIM/bin/libode-x86_64.so

== On Linux 64-bit to cross-compile to 32-bit ==
CFLAGS=-m32 CPPFLAGS=-m32 LDFLAGS=-m32 ./configure --build=i686-pc-linux-gnu --with-trimesh=gimpact --disable-asserts --enable-shared --disable-demos --without-x
make
cp ode/src/.libs/libode.so.1.1.1 $OPENSIM/bin/libode.so

== On Windows 64-bit to compile to 32-bit ==
premake4 --with-gimpact --only-single --platform=x32 --dotnet=msnet vs2008
open visual studio 2008
switch to ReleaseDLL configuration
compile
cp build/vs2008/ode.dll $OPENSIM/bin/ode.dll

This is necessary because on Windows we can currently only run OpenSimulator in 32-bit mode

== On Mac OS X Intel 64-bit to compile to a 32-bit, 64-bit Intel and PowerPC universal binary ==
CFLAGS="-g -O2 -isysroot /Developer/SDKs/MacOSX10.6.sdk -arch i386 -arch x86_64 -arch ppc" CXXFLAGS="-g -O2 -isysroot /Developer/SDKs/MacOSX10.6.sdk -arch i386 -arch x86_64 -arch ppc" LDFLAGS="-arch i386 -arch x86_64 -arch ppc" ./configure --with-trimesh=gimpact --disable-asserts --enable-shared --disable-dependency-tracking --disable-demos --without-x
make
cp ode/src/.libs/libode.dylib $OPENSIM/bin/libode.dylib

= OLD BUILD INSTRUCTIONS =

These were the old instructions before OpenSim switched to compiling with the GIMPACT collider in commit 4eef6725f4116aa70de729b71d60636a7d0a68f5 (17 Jan 2012) rather than OPCODE.

== On Linux x86 (32 bit) ==

When building on a 32-bit machine, use the following commands

chmod a+x ou/bootstrap
sh autogen.sh
./configure --enable-old-trimesh --disable-asserts --enable-shared
make
cp ode/src/.libs/libode.so $OPENSIM/bin

You can use the same commands to build on a 64 bit machine, but in this case the final command is

cp ode/src/.libs/libode.so $OPENSIM/bin/libode-x86_64.so

== On Windows x86 (32 bit) ==

premake4 --old-trimesh --only-single --platform=x32 --dotnet=msnet vs2008
compile with vs2008

== On Mac OS X Intel (64 bit) ==

CFLAGS="-g -O2 -isysroot /Developer/SDKs/MacOSX10.6.sdk -arch i386 -arch x86_64 -arch ppc" CXXFLAGS="-g -O2 -isysroot /Developer/SDKs/MacOSX10.6.sdk -arch i386 -arch x86_64 -arch ppc" LDFLAGS="-arch i386 -arch x86_64 -arch ppc" ./configure --enable-old-trimesh --disable-asserts --enable-shared --disable-dependency-tracking --disable-demos --without-x
make
    
--disable-demos --without-x is required to build ODE on Mac OS X
CFLAGS, CXXFLAGS and --disable-dependency-tracking are necessary to build a universal dylib (some compilation lines use CFLAGS instead of CXXFLAGS)
The other settings are tweaks for using ODE with OpenSim
