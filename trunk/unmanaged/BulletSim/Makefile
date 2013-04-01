# Build BulletSim

IDIR = /usr/local/include/bullet
LDIR = /usr/local/lib

# the Bullet libraries are linked statically so we don't have to also distribute the shared binaries
BULLETLIBS = $(LDIR)/libBulletDynamics.a $(LDIR)/libBulletCollision.a $(LDIR)/libLinearMath.a $(LDIR)/libHACD.a
 
#CC = gcc
CC = g++
CFLAGS = -I$(IDIR) -fPIC -g

BASEFILES = API2.cpp BulletSim.cpp

SRC = $(BASEFILES)
# SRC = $(wildcard *.cpp)

BIN = $(patsubst %.cpp, %.o, $(SRC))


all: libBulletSim.so

libBulletSim.so : $(BIN)
	$(CC) -shared -Wl,-soname,libBulletSim.so -o libBulletSim.so $(BIN) $(BULLETLIBS)

%.o: %.cpp
	$(CC) $(CFLAGS) -c $?

BulletSim.cpp : BulletSim.h Util.h

BulletSim.h: ArchStuff.h APIData.h WorldData.h

API2.cpp : BulletSim.h

clean:
	rm -f *.o *.so
