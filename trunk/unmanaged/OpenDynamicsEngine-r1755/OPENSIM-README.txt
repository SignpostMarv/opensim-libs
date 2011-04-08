OPENSIM README

On Linux x86 (32 bit) this version of ODE as used in OpenSim was consumed with the following commands

chmod a+x ou/bootstrap
sh autogen.sh
./configure --enable-old-trimesh --disable-asserts --enable-shared
make
cp ./ode/src/.libs/libode.so ~/ODESVN
cp ~/ODESVN/libode.so ~/opensim/bin


On Windows x86 (32 bit)

premake4 --old-trimesh --only-single --platform=x32 --dotnet=msnet vs2008
compile with vs2008


On Mac OS X Intel (64 bit)

CFLAGS="-g -O2 -isysroot /Developer/SDKs/MacOSX10.6.sdk -arch i386 -arch x86_64 -arch ppc" CXXFLAGS="-g -O2 -isysroot /Developer/SDKs/MacOSX10.6.sdk -arch i386 -arch x86_64 -arch ppc" LDFLAGS="-arch i386 -arch x86_64 -arch ppc" ./configure --enable-old-trimesh --disable-asserts --enable-shared --disable-dependency-tracking --disable-demos --without-x
make
    
--disable-demos --without-x is required to build ODE on Mac OS X
CFLAGS, CXXFLAGS and --disable-dependency-tracking are necessary to build a universal dylib (some compilation lines use CFLAGS instead of CXXFLAGS)
The other settings are tweaks for using ODE with OpenSim
