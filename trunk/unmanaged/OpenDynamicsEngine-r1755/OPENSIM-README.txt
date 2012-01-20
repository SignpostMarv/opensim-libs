OPENSIM README

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
