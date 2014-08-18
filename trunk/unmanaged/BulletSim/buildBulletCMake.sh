#! /bin/bash
# Script to build Bullet on a target system.

MACH=$(uname -m)
BULLET=bullet-2.82

cd $BULLET
mkdir -p bullet-build
cd bullet-build

if [[ "$MACH" = "x86_64" ]] ; then
    cmake .. -G "Unix Makefiles" \
            -DBUILD_EXTRAS=on \
            -DBUILD_DEMOS=off \
            -DBUILD_SHARED_LIBS=off \
            -DINSTALL_LIBS=on \
            -DINSTALL_EXTRA_LIBS=on \
            -DCMAKE_CXX_FLAGS="-fPIC" \
            -DCMAKE_BUILD_TYPE=Release
else
    cmake .. -G "Unix Makefiles" \
            -DBUILD_EXTRAS=on \
            -DBUILD_DEMOS=off \
            -DBUILD_SHARED_LIBS=off \
            -DINSTALL_LIBS=on \
            -DINSTALL_EXTRA_LIBS=on \
            -DCMAKE_BUILD_TYPE=Release
fi

make -j4

make install
