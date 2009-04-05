default: all

BULLET_SDK_INCLUDE_PATH=../
BULLET_SDK_LIB=../\ ../../lib
BULLET_LIB_NAME=bulletnet
BULLET_LIB_NAME_DEBUG=bulletnetd

DEBUG_PATH=../../lib
RELEASE_PATH=../../lib

COMPILER=$(CC)
CFLAGS = -g -O3
FLAGS = $(CFLAGS) -I $(BULLET_SDK_INCLUDE_PATH)
LDFLAGS=libbulletdynamics.a libbulletcollision.a libbulletmath.a -lstdc++

BULLET_SRC= \
	btAxisSweep3_net.cpp \
	btBoxShape_net.cpp \
	btCapsuleShape_net.cpp \
	btCollisionDispatcher_net.cpp \
	btCollisionObject_net.cpp \
	btCollisionShape_net.cpp \
	btCollisionWorld_net.cpp \
	btDefaultCollisionConfiguration_net.cpp \
	btDefaultMotionState_net.cpp \
	btDiscreteDynamicsWorld_net.cpp \
	btGeneric6DofConstraint_net.cpp \
	btGhostObject_net.cpp \
	btGImpactMeshShape_net.cpp \
	btHeightfieldTerrainShape_net.cpp \
	btMatrix3x3_net.cpp \
	btMotionState_net.cpp \
	btQuaternion_net.cpp \
	btRigidBody_net.cpp \
	btRigidBodyConstructionInfo_net.cpp \
	btSequentialImpulseConstraintSolver_net.cpp \
	btSphereShape_net.cpp \
	btTransform_net.cpp \
	btTriangleIndexVertexArray_net.cpp \
	btTriangleMesh_net.cpp \
	btTypedConstraint_net.cpp \
	btvector3_net.cpp \
	bulletnet_net.cpp \
	ClosestNotMeRaycastResultCallback_net.cpp \
	conversion.cpp

BULLET_SRC_OBJECTS=$(BULLET_SRC:.cpp=.o)

.cpp.o: 
	$(COMPILER) -Os -c $(FLAGS) -o $@ $<

.PHONY:	all clean

all: $(BULLET_LIB_NAME)


$(BULLET_LIB_NAME): $(BULLET_SRC_OBJECTS)
	$(COMPILER) $(FLAGS) -fPIC -shared $^ -o lib$@.so $(LDFLAGS)
	cp lib$(BULLET_LIB_NAME).so $(DEBUG_PATH)/lib$(BULLET_LIB_NAME_DEBUG).so
	cp lib$(BULLET_LIB_NAME).so $(RELEASE_PATH)/lib$(BULLET_LIB_NAME).so
	strip $(RELEASE_PATH)/lib$(BULLET_LIB_NAME).so

clean:
	rm *.o
	rm *.so
#	rm $(RELEASE_PATH)/lib$(BULLET_LIB_NAME).so
#	rm $(DEBUG_PATH)/lib$(BULLET_LIB_NAME).so
