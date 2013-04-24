# Build BulletSim

IDIR = /usr/local/include/bullet
LDIR = /usr/local/lib

# the Bullet libraries are linked statically so we don't have to also distribute the shared binaries
BULLETLIBS = $(LDIR)/libBulletDynamics.a $(LDIR)/libBulletCollision.a $(LDIR)/libLinearMath.a $(LDIR)/libHACD.a
 
#CC = gcc
CC = g++
LD = g++
UNAME := $(shell uname)
UNAMEPROCESSOR := $(shell uname --processor)

# Kludge for building libBulletSim.so with different library dependencies
#    As of 20130424, 64bit Ubuntu needs to wrap memcpy so it doesn't pull in glibc 2.14.
#    The wrap is not needed on Ubuntu 32bit and, in fact, causes crashes.
ifeq ($(UNAMEPROCESSOR), x86_64)
WRAPMEMCPY = -Wl,--wrap=memcpy
else
WRAPMEMCPY =
endif

ifeq ($(UNAME), Linux)
TARGET = libBulletSim.so
CFLAGS = -I$(IDIR) -fPIC -g
LFLAGS = $(WRAPMEMCPY) -shared -Wl,-soname,$(TARGET) -o $(TARGET)
endif
ifeq ($(UNAME), Darwin)
TARGET = libBulletSim.dylib
CFLAGS = -arch i386 -I$(IDIR) -fPIC -g
LFLAGS = -arch i386 -dynamiclib -Wl -o $(TARGET)
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
