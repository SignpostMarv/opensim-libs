OPENSIM README

On Linux x86 (32 bit) this version of ODE as used in OpenSim was consumed with the following commands

cd opende/trunk
chmod a+x ou/bootstrap
sh autogen.sh
./configure --enable-old-trimesh --disable-asserts --enable-shared
make
cp ./ode/src/.libs/libode.so ~/ODESVN
cp ~/ODESVN/libode.so ~/opensim/bin


On Windows x86 (32 bit)

premake4 --old-trimesh --only-single --platform=x32 --dotnet=msnet vs2008
compile with vs2008
