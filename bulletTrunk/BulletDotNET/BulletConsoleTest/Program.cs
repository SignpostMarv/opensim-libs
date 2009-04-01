using System;
using BulletDotNET;


namespace BulletConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            btVector3 testvec = new btVector3(-2, 1, 0);
            Console.WriteLine(String.Format("Original: {0}", testvec.testStr()));
            btVector3 testvec2 = testvec.absolute();
            Console.WriteLine(String.Format("absolute: {0}", testvec2.testStr()));
            Console.WriteLine(String.Format("angle:{0}", testvec.angle(testvec2)));
            Console.WriteLine(String.Format("closestAxis(orig):{0}", testvec.closestAxis()));
            btVector3 testvec3 = testvec.cross(testvec2);
            Console.WriteLine(String.Format("cross: {0}", testvec3.testStr()));
            Console.WriteLine(String.Format("distance: {0}", testvec.distance(testvec2)));
            Console.WriteLine(String.Format("distance2: {0}", testvec.distance2(testvec2)));
            Console.WriteLine(String.Format("dot: {0}",  testvec.dot(testvec2)));
            Console.WriteLine(String.Format("furthestAxis(orig): {0}", testvec.furthestAxis()));
            btVector3 testvec4 = testvec.normalized();
            Console.WriteLine(String.Format("normalized: {0}", testvec4.testStr()));
            testvec4.setInterpolate3(testvec, testvec2, 0.5f);
            Console.WriteLine(String.Format("interpolate3: {0}", testvec4.testStr()));
            testvec4.setValue(7f, -0.09f, 2.5f);
            Console.WriteLine(String.Format("setvec: {0}", testvec4.testStr()));
            testvec4.setX(5.0f);
            testvec4.setY(-0.25f);
            testvec4.setZ(90f);
            testvec.setValue(0, 0, -1024);
            testvec2.setValue(256, 256, 1024);
            Console.WriteLine(String.Format("setvecIndividual: {0}", testvec4.testStr()));
            btAxisSweep3 testbtAxisSweep3 = new btAxisSweep3(testvec, testvec2, 50);
            btDefaultCollisionConfiguration colconfig = new btDefaultCollisionConfiguration();
            btCollisionDispatcher coldisp = new btCollisionDispatcher(colconfig);
            btSequentialImpulseConstraintSolver seqimpconssol = new btSequentialImpulseConstraintSolver();
            btDiscreteDynamicsWorld dynamicsWorld = new btDiscreteDynamicsWorld(coldisp, testbtAxisSweep3, seqimpconssol,
                                                                                colconfig);
            dynamicsWorld.setGravity(new btVector3(0, 0, -9.87f));
            Console.WriteLine(String.Format("stepWorld: {0}", dynamicsWorld.stepSimulation((6f / 60), 5, (1f / 60))));
            Console.WriteLine(String.Format("stepWorld: {0}", dynamicsWorld.stepSimulation((6f / 60), 5, (1f / 60))));
            Console.WriteLine(String.Format("stepWorld: {0}", dynamicsWorld.stepSimulation((6f / 60), 5, (1f / 60))));
            Console.WriteLine(String.Format("stepWorld: {0}", dynamicsWorld.stepSimulation((6f / 60), 5, (1f / 60))));
            btQuaternion testquat = new btQuaternion(50, 0, 0, 1);
            btQuaternion testquatnorm = testquat.normalized();
            Console.WriteLine(String.Format("testquat: {0}", testquat.testStr()));
            Console.WriteLine(String.Format("testquatnormalize: {0}", testquatnorm.testStr()));
            Console.WriteLine(String.Format("testquatLength: {0}", testquat.length()));
            Console.WriteLine(String.Format("testquatnormalizeLength: {0}", testquatnorm.length()));

            float[] heightdata = new float[256*256];
            for (int j=0;j<256*256;j++)
            {
               if (j%2==0)
                    heightdata[j] = 21f;
               else
                   heightdata[j] = 28f;
                
            }

            btHeightfieldTerrainShape obj = new btHeightfieldTerrainShape(256, 256, heightdata, 1.0f, 0, 256,
                                                                          (int)btHeightfieldTerrainShape.UPAxis.Z,
                                                                          (int)btHeightfieldTerrainShape.PHY_ScalarType.
                                                                              PHY_FLOAT, false);

            btCapsuleShape cap = new btCapsuleShape(0.23f, 3);

            btTriangleMesh testMesh = new btTriangleMesh(true, false);
            testMesh.addTriangle(new btVector3(1, 0, 1), new btVector3(1, 0, -1), new btVector3(-1, 0, -1), false);
            testMesh.addTriangle(new btVector3(1, -1, 1), new btVector3(1, -1, -1), new btVector3(-1, -1, -1), false);
            testMesh.addTriangle(new btVector3(1, -1, 1), new btVector3(1, 0, 1), new btVector3(-1, -1, -1), false);
            testMesh.addTriangle(new btVector3(1, 0, 1), new btVector3(1, -1, -1), new btVector3(-1, 0, -1), false);
            testMesh.addTriangle(new btVector3(1, -1, -1), new btVector3(-1, 0, -1), new btVector3(-1, -1, -1), false);
            testMesh.addTriangle(new btVector3(1, -1, -1), new btVector3(1, 0, -1), new btVector3(-1, 0, -1), false);
            testMesh.addTriangle(new btVector3(1, 0, 1), new btVector3(1, -1, -1), new btVector3(1, 0, -1), false);
            testMesh.addTriangle(new btVector3(1, -1, 1), new btVector3(1, -1, -1), new btVector3(1, 0, 1), false);
            btGImpactMeshShape meshtest = new btGImpactMeshShape(testMesh);
            meshtest.updateBound();

            btRigidBody groundbody = new btRigidBody(0,
                                                     new btDefaultMotionState(
                                                         new btTransform(new btQuaternion(0, 0, 0, 1),
                                                                         new btVector3(128, 128, 256f / 2f))), obj,
                                                     new btVector3(0, 0, 0));

            btRigidBody capbody = new btRigidBody(200,
                                                     new btDefaultMotionState(
                                                         new btTransform(new btQuaternion(0, 0, 0, 1),
                                                                         new btVector3(128, 128, 25))), cap,
                                                     new btVector3(0, 0, 0));

            btRigidBody meshbody = new btRigidBody(200,
                                         new btDefaultMotionState(
                                             new btTransform(new btQuaternion(0, 0, 0, 1),
                                                             new btVector3(128, 128, 29))), meshtest,
                                         new btVector3(0, 0, 0));


            btRigidBodyConstructionInfo constructioninfotest = new btRigidBodyConstructionInfo();
            constructioninfotest.m_collisionShape = new btBoxShape(new btVector3(0.5f,0.5f,0.5f));
            constructioninfotest.m_localInertia = new btVector3(0, 0, 0);
            constructioninfotest.m_motionState = new btDefaultMotionState(new btTransform(new btQuaternion(0.3f, -0.4f, 0.8f, 0.1f), new btVector3(128.5f, 128, 25)),
                                                                          new btTransform(new btQuaternion(0,0,0,1),new btVector3(0,0.25f,0)));
            constructioninfotest.m_startWorldTransform = new btTransform(new btQuaternion(0,0,0,1),new btVector3(0,0,0));
            constructioninfotest.m_mass = 2000000;
            constructioninfotest.m_linearDamping = 0;
            constructioninfotest.m_angularDamping = 0;
            constructioninfotest.m_friction = 0.1f;
            constructioninfotest.m_restitution = 0;
            constructioninfotest.m_linearSleepingThreshold = 0.8f;
            constructioninfotest.m_angularSleepingThreshold = 1;
            constructioninfotest.m_additionalDamping = false;
            constructioninfotest.m_additionalDampingFactor = 0.005f;
            constructioninfotest.m_additionalLinearDampingThresholdSqr = 0.01f;
            constructioninfotest.m_additionalAngularDampingThresholdSqr = 0.01f;
            constructioninfotest.m_additionalAngularDampingFactor = 0.01f;
            constructioninfotest.commit();
            btGImpactCollisionAlgorithm.registerAlgorithm(coldisp);
            btRigidBody cubetest = new btRigidBody(constructioninfotest);

            dynamicsWorld.addRigidBody(groundbody);
            dynamicsWorld.addRigidBody(cubetest);
            dynamicsWorld.addRigidBody(capbody);
            dynamicsWorld.addRigidBody(meshbody);

            int frame = 0;
            for (int i = 0; i < 26; i++ )
            {
                int frames = dynamicsWorld.stepSimulation(((i%60) / 60f), 10, (1f / 60));
                frame += frames;
                Console.WriteLine(String.Format("Cube: frame {0} frames: {1} POS:{2}, quat:{3}", frame, frames, cubetest.getInterpolationWorldTransform().getOrigin().testStr(), cubetest.getWorldTransform().getRotation().testStr()));
                Console.WriteLine(String.Format("Cap: frame {0} frames: {1} POS:{2}, quat:{3}", frame, frames, capbody.getInterpolationWorldTransform().getOrigin().testStr(), capbody.getWorldTransform().getRotation().testStr()));
                Console.WriteLine(String.Format("Mesh: frame {0} frames: {1} POS:{2}, quat:{3}", frame, frames, meshbody.getInterpolationWorldTransform().getOrigin().testStr(), meshbody.getWorldTransform().getRotation().testStr()));
            
            }

            dynamicsWorld.removeRigidBody(meshbody);
            dynamicsWorld.removeRigidBody(capbody);
            dynamicsWorld.removeRigidBody(cubetest);
            dynamicsWorld.removeRigidBody(groundbody);
            cubetest.Dispose();
            groundbody.Dispose();
            capbody.Dispose();
            cap.Dispose();
            obj.Dispose();
            testbtAxisSweep3.Dispose();
            dynamicsWorld.Dispose();
            coldisp.Dispose();
            colconfig.Dispose();
            seqimpconssol.Dispose();


            testvec.Dispose();
            testvec2.Dispose();
            testvec3.Dispose();
            testvec4.Dispose();
            
        }
    }
}
