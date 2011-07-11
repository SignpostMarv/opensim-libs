# Scratch makefile for linux

IDIR = /usr/local/include/bullet
LDIR = /usr/local/lib

BULLETLIBS = -L$(LDIR) -lBulletDynamics -lBulletCollision -lLinearMath -lc 
 
#CC = gcc
CC = g++
CFLAGS = -I$(IDIR) -fPIC -g

all: libBulletSim.so

libBulletSim.so : BulletSim.o API.o
	$(CC) -shared -Wl,-soname,libBulletSim.so -o libBulletSim.so BulletSim.o API.o $(BULLETLIBS)

BulletSim.o : BulletSim.h BulletSim.cpp
	$(CC) $(CFLAGS) -c BulletSim.cpp

API.o : API.cpp
	$(CC) $(CFLAGS) -c API.cpp 

clean:
	rm -f *.o *.so

