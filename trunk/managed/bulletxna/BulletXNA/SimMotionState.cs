using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;
using BulletXNA.BulletDynamics;
namespace BulletXNA
{
    public struct EntityProperties
    {
        

        public uint ID;
        public IndexedVector3 Position;
        public IndexedQuaternion Rotation;
        public IndexedVector3 Velocity;
        public IndexedVector3 Acceleration;
        public IndexedVector3 AngularVelocity;

        public EntityProperties FromTransform(uint id, IndexedMatrix startTransform)
        {
            EntityProperties ret = new EntityProperties();
            ID = id;
            Position = startTransform._origin;
            Rotation = startTransform.GetRotation();
            return ret;
        }
        public static bool operator ==(EntityProperties value1, EntityProperties value2)
        {
            if (value1.ID == value2.ID && value1.Position == value2.Position && value1.Rotation == value2.Rotation && value1.Velocity == value2.Velocity && value1.Acceleration == value2.Acceleration)
                return value1.AngularVelocity == value2.AngularVelocity;
            else
                return false;
        }

        public static bool operator !=(EntityProperties value1, EntityProperties value2)
        {
            if (value1.ID == value2.ID && value1.Position == value2.Position && value1.Rotation == value2.Rotation && value1.Velocity == value2.Velocity && value1.Acceleration == value2.Acceleration)
                return value1.AngularVelocity != value2.AngularVelocity;
            else
                return true;
        }


        public bool Equals(EntityProperties other)
        {
            if (this.ID == other.ID && this.Position == other.Position && this.Rotation == other.Rotation && this.Velocity == other.Velocity && this.Acceleration == other.Acceleration)
                return this.AngularVelocity == other.AngularVelocity;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is EntityProperties)
                flag = this.Equals((EntityProperties)obj);
            return flag;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CollisionDesc
    {
        public uint aID;
        public uint bID;
        public IndexedVector3 point;
        public IndexedVector3 normal;
    }

    public class SimMotionState : DefaultMotionState
    {
        public RigidBody Rigidbody;
        public IndexedVector3 ZeroVect;

        private IndexedMatrix m_xform;

        private EntityProperties m_properties;
        private EntityProperties m_lastProperties;
        private CollisionWorld  m_world;

        const float POSITION_TOLERANCE = 0.05f;
        const float VELOCITY_TOLERANCE = 0.001f;
        const float ROTATION_TOLERANCE = 0.01f;
        const float ANGULARVELOCITY_TOLERANCE = 0.01f;

        public SimMotionState(CollisionWorld pWorld, uint id, IndexedMatrix starTransform, object frameUpdates)
        {
            m_properties = new EntityProperties()
                               {
                                   ID = id,
                                   Position = starTransform._origin,
                                   Rotation = starTransform.GetRotation()
                               };
            m_lastProperties = new EntityProperties()
            {
                ID = id,
                Position = starTransform._origin,
                Rotation = starTransform.GetRotation()
            };
            m_world = pWorld;
            m_xform = starTransform;
        }

        public override void GetWorldTransform(out IndexedMatrix worldTrans)
        {
            worldTrans = m_xform;
        }

        public override void SetWorldTransform(IndexedMatrix worldTrans)
        {
            SetWorldTransform(ref worldTrans);
        }

        public override void SetWorldTransform(ref IndexedMatrix worldTrans)
        {
            SetWorldTransform(ref worldTrans, false);
        }
        public void SetWorldTransform(ref IndexedMatrix worldTrans, bool force)
        {
            m_xform = worldTrans;
            // Put the new transform into m_properties
            m_properties.Position = m_xform._origin;
            m_properties.Rotation = m_xform.GetRotation();
            // A problem with stock Bullet is that we don't get an event when an object is deactivated.
            // This means that the last non-zero values for linear and angular velocity
            // are left in the viewer who does dead reconning and the objects look like
            // they float off.
            // BulletSim ships with a patch to Bullet which creates such an event.
            m_properties.Velocity = Rigidbody.GetLinearVelocity();
            m_properties.AngularVelocity = Rigidbody.GetAngularVelocity();

            if (force

                || !AlmostEqual(ref m_lastProperties.Position, ref m_properties.Position, POSITION_TOLERANCE)
                || !AlmostEqual(ref m_properties.Rotation, ref m_lastProperties.Rotation, ROTATION_TOLERANCE)
                // If the Velocity and AngularVelocity are zero, most likely the object has
                //    been deactivated. If they both are zero and they have become zero recently,
                //    make sure a property update is sent so the zeros make it to the viewer.
                || ((m_properties.Velocity == ZeroVect && m_properties.AngularVelocity == ZeroVect)
                    &&
                    (m_properties.Velocity != m_lastProperties.Velocity ||
                     m_properties.AngularVelocity != m_lastProperties.AngularVelocity))
                //	If Velocity and AngularVelocity are non-zero but have changed, send an update.
                || !AlmostEqual(ref m_properties.Velocity, ref m_lastProperties.Velocity, VELOCITY_TOLERANCE)
                ||
                !AlmostEqual(ref m_properties.AngularVelocity, ref m_lastProperties.AngularVelocity,
                             ANGULARVELOCITY_TOLERANCE)
                )


            {
                // Add this update to the list of updates for this frame.
                m_lastProperties = m_properties;
                if (m_world.LastEntityProperty < m_world.UpdatedObjects.Length)
                    m_world.UpdatedObjects[m_world.LastEntityProperty++]=(m_properties);
                
                //(*m_updatesThisFrame)[m_properties.ID] = &m_properties;
            }
                
                
                

        }
        internal static bool AlmostEqual(ref IndexedVector3 v1, ref IndexedVector3 v2, float nEpsilon)
        {
            return
            (((v1.X - nEpsilon) < v2.X) && (v2.X < (v1.X + nEpsilon))) &&
            (((v1.Y - nEpsilon) < v2.Y) && (v2.Y < (v1.Y + nEpsilon))) &&
            (((v1.Z - nEpsilon) < v2.Z) && (v2.Z < (v1.Z + nEpsilon)));
        }

        internal static bool AlmostEqual(ref IndexedQuaternion v1, ref IndexedQuaternion v2, float nEpsilon)
        {
            return
            (((v1.X - nEpsilon) < v2.X) && (v2.X < (v1.X + nEpsilon))) &&
            (((v1.Y - nEpsilon) < v2.Y) && (v2.Y < (v1.Y + nEpsilon))) &&
            (((v1.Z - nEpsilon) < v2.Z) && (v2.Z < (v1.Z + nEpsilon))) &&
            (((v1.W - nEpsilon) < v2.W) && (v2.W < (v1.W + nEpsilon)));
        }
        
    }
}
