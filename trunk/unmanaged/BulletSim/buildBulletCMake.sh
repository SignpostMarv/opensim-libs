#! /bin/bash
# Script to build Bullet on a target system.

STARTDIR=$(pwd)

UNAME=$(uname)
MACH=$(uname -m)
BULLET=bullet-2

cd $BULLET
mkdir -p bullet-build
cd bullet-build

if [[ "$UNAME" == "Darwin" ]] ; then
    cmake .. -G "Unix Makefiles" \
            -DBUILD_BULLET2_DEMOS=off \
            -DBUILD_BULLET3=on \
            -DBUILD_CLSOCKET=off \
            -DBUILD_CPU_DEMOS=off \
            -DBUILD_ENET=off \
            -DBUILD_EXTRAS=on \
            -DBUILD_DEMOS=off \
            -DBUILD_OPENGL_DEMOS=off \
            -DBUILD_PYBULLET=off \
            -DBUILD_SHARED_LIBS=off \
            -DBUILD_UNIT_TESTS=off \
            -DINSTALL_EXTRA_LIBS=on \
            -DINSTALL_LIBS=on \
            -DCMAKE_OSX_ARCHITECTURES="i386; x86_64" \
            -DCMAKE_CXX_FLAGS="-arch i386 -arch x86_64 -mmacosx-version-min=10.11" \
            -DCMAKE_C_FLAGS="-arch i386 -arch x86_64 -mmacosx-version-min=10.11" \
            -DCMAKE_EXE_LINKER_FLAGS="-arch i386 -arch x86_64 -mmacosx-version-min=10.11" \
            -DCMAKE_VERBOSE_MAKEFILE="on" \
            -DCMAKE_BUILD_TYPE=Release
else
    if [[ "$MACH" == "x86_64" ]] ; then
        cmake .. -G "Unix Makefiles" \
                -DBUILD_BULLET2_DEMOS=off \
                -DBUILD_BULLET3=on \
                -DBUILD_CLSOCKET=off \
                -DBUILD_CPU_DEMOS=off \
                -DBUILD_ENET=off \
                -DBUILD_EXTRAS=on \
                -DBUILD_DEMOS=off \
                -DBUILD_OPENGL_DEMOS=off \
                -DBUILD_PYBULLET=off \
                -DBUILD_SHARED_LIBS=off \
                -DBUILD_UNIT_TESTS=off \
                -DINSTALL_EXTRA_LIBS=on \
                -DINSTALL_LIBS=on \
                -DCMAKE_CXX_FLAGS="-fPIC" \
                -DCMAKE_BUILD_TYPE=Release
    else
        cmake .. -G "Unix Makefiles" \
                -DBUILD_BULLET2_DEMOS=off \
                -DBUILD_BULLET3=on \
                -DBUILD_CLSOCKET=off \
                -DBUILD_CPU_DEMOS=off \
                -DBUILD_ENET=off \
                -DBUILD_EXTRAS=on \
                -DBUILD_DEMOS=off \
                -DBUILD_OPENGL_DEMOS=off \
                -DBUILD_PYBULLET=off \
                -DBUILD_SHARED_LIBS=off \
                -DBUILD_UNIT_TESTS=off \
                -DINSTALL_EXTRA_LIBS=on \
                -DINSTALL_LIBS=on \
                -DCMAKE_BUILD_TYPE=Release
    fi
fi

make -j4

# make install

# As an alternative to installation, move the .a files in to a local directory
#    Good as it doesn't require admin privilages
cd "$STARTDIR"
mkdir -p lib
cd "${BULLET}"
cp */*/*.a */*/*/*.a ../lib

cd "$STARTDIR"
mkdir -p include
cd "${BULLET}/src"
cp *.h ../../include
for file in $(find . -name \*.h) ; do
    xxxx="../../include/$(dirname $file)" 
    mkdir -p "$xxxx"
    cp "$file" "$xxxx"
done

cd ../Extras
for file in $(find . -name \*.h) ; do
    xxxx="../../include/$(dirname $file)"
    mkdir -p "$xxxx"
    cp "$file" "$xxxx"
done
for file in $(find . -name \*.inl) ; do
    xxxx="../../include/$(dirname $file)"
    mkdir -p "$xxxx"
    cp "$file" "$xxxx"
done
