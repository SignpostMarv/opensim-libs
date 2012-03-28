# Build BulletSim

IDIR = /usr/local/include/bullet
LDIR = /usr/local/lib

BULLETLIBS = -L$(LDIR) -lBulletDynamics -lBulletCollision -lLinearMath -lc 
 
#CC = gcc
CC = g++
CFLAGS = -I$(IDIR) -fPIC -g

BASEFILES = API.cpp BulletSim.cpp

OBJECTFILES = IPhysObject.cpp AvatarObject.cpp PrimObject.cpp GroundPlaneObject.cpp TerrainObject.cpp

THINGFILES = Constraint.cpp

COLLECTIONFILES = ConstraintCollection.cpp ObjectCollection.cpp

SRC = $(BASEFILES) $(OBJECTFILES) $(THINGFILE) $(COLLECTIONFILES)
# SRC = $(wildcard *.cpp)

BIN = $(patsubst %.cpp, %.o, $(SRC))


all: libBulletSim.so

libBulletSim.so : $(BIN)
	$(CC) -shared -Wl,-soname,libBulletSim.so -o libBulletSim.so $(BIN) $(BULLETLIBS)

%.o: %.cpp
	$(CC) $(CFLAGS) -c $?

BulletSim.cpp : BulletSim.h GroundPlaneObject.h TerrainObject.h Constraint.h

BulletSim.h: ArchStuff.h APIData.h IPhysObject.h TerrainObject.h ObjectCollection.h ConstraintCollection.h WorldData.h

API.cpp : BulletSim.h

IPhysObject.cpp: IPhysObject.h AvatarObject.h PrimObject.h

IPhysObject.h: APIData.h WorldData.h

AvatarObject.cpp:  AvatarObject.h BulletSim.h

AvatarObject.h: IPhysObject.h APIData.h WorldData.h

PrimObject.cpp: PrimObject.h

PrimObject.h: IPhysObject.h APIData.h WorldData.h

GroundPlaneObject.cpp: GroundPlaneObject.h 

GroundPlaneObject.h: IPhysObject.h APIData.h WorldData.h

TerrainObject.cpp: TerrainObject.h

TerrainObject.h: IPhysObject.h APIData.h WorldData.h

Constraint.cpp: Constraint.h IPhysObject.h ObjectCollection.h

Constraint.h:  WorldData.h ArchStuff.h

ConstraintCollection.cpp: ConstraintCollection.h 

ConstraintCollection.h: ArchStuff.h WorldData.h Constraint.h

ObjectCollection.cpp: ObjectCollection.h

ObjectCollection.h: IPhysObject.h ArchStuff.h

clean:
	rm -f *.o *.so
