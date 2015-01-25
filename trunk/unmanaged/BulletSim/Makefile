# Build BulletSim

# IDIR = /usr/local/include/bullet
# LDIR = /usr/local/lib
IDIR = ./include
LDIR = ./lib

# the Bullet libraries are linked statically so we don't have to also distribute the shared binaries
BULLETLIBS = $(LDIR)/libBulletDynamics.a $(LDIR)/libBulletCollision.a $(LDIR)/libLinearMath.a $(LDIR)/libHACD.a
 
#CC = gcc
CC = g++
LD = g++
UNAME := $(shell uname)
UNAMEPROCESSOR := $(shell uname -m)

# Kludge for building libBulletSim.so with different library dependencies
#    As of 20130424, 64bit Ubuntu needs to wrap memcpy so it doesn't pull in glibc 2.14.
#    The wrap is not needed on Ubuntu 32bit and, in fact, causes crashes.
ifeq ($(UNAMEPROCESSOR), x86_64)
WRAPMEMCPY = -Wl,--wrap=memcpy
else
WRAPMEMCPY =
endif

# Linux build.
ifeq ($(UNAME), Linux)
TARGET = libBulletSim.so
CFLAGS = -I$(IDIR) -fPIC -g
LFLAGS = $(WRAPMEMCPY) -shared -Wl,-soname,$(TARGET) -o $(TARGET)
endif

# OSX build. Builds 32bit dylib on 64bit system. (Need 32bit because Mono is 32bit only).
ifeq ($(UNAME), Darwin)
TARGET = libBulletSim.dylib
# CC = clang
# LD = clang
#CFLAGS = -m32 -I$(IDIR) -fPIC -g
#LFLAGS = -m32 -dynamiclib -Wl -o $(TARGET)
#CFLAGS = -m32 -arch i386 -stdlib=libstdc++ -mmacosx-version-min=10.6 -I$(IDIR) -g 
#LFLAGS = -v -m32 -arch i386 -std=c++11 -stdlib=libstdc++ -mmacosx-version-min=10.6 -dynamic -o $(TARGET)
CFLAGS = -m32 -arch i386 -I$(IDIR) -g 
LFLAGS = -v -dynamiclib -m32 -arch i386 -o $(TARGET)
endif

BASEFILES = API2.cpp BulletSim.cpp

SRC = $(BASEFILES)
# SRC = $(wildcard *.cpp)

BIN = $(patsubst %.cpp, %.o, $(SRC))


all: $(TARGET)

$(TARGET) : $(BIN)
	$(LD) $(LFLAGS) $(BIN) $(BULLETLIBS)

%.o: %.cpp
	$(CC) $(CFLAGS) -c $?

BulletSim.cpp : BulletSim.h Util.h

BulletSim.h: ArchStuff.h APIData.h WorldData.h

API2.cpp : BulletSim.h

clean:
	rm -f *.o $(TARGET)
