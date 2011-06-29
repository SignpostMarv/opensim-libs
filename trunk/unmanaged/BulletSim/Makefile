# Scratch makefile for linux

BulletSim.o :
	gcc -I bullet-2.78/src -fPIC -g -c BulletSim.cpp

API.o :
	gcc -I bullet-2.78/src -fPIC -g -c API.cpp 

BulletSim.so : BulletSim.o API.o
	gcc -sharead -Wl,-soname,BulletSim.so -o BulletSim.so BulletSim.o API.o -lc
