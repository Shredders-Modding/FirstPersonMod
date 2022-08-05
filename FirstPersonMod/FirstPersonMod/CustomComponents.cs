using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;

namespace FirstPersonMod
{
    [RegisterTypeInIl2Cpp]
    public class PositionTargeter : MonoBehaviour
    {
        public Transform m_Xtarget;
        public Transform m_Ytarget;
        public Transform m_Ztarget;
        public Vector3 m_posOffset;

        private float m_newX;
        private float m_newY;
        private float m_newZ;

        public PositionTargeter(IntPtr ptr) : base(ptr) { }

        void Start()
        {
            m_newX = 0;
            m_newY = 0;
            m_newZ = 0;
        }

        void LateUpdate()
        {
            if (m_Xtarget)
            {
                m_newX = m_Xtarget.position.x + m_posOffset.x;
            }

            if (m_Ytarget)
            {
                m_newY = m_Ytarget.position.y + m_posOffset.y;
            }

            if (m_Ztarget)
            {
                m_newZ = m_Ztarget.position.z + m_posOffset.z;
            }

            transform.position = new Vector3(m_newX, m_newY, m_newZ);
        }
    }

    [RegisterTypeInIl2Cpp]
    public class RotationTargeter : MonoBehaviour
    {
        public Rigidbody m_rigidbody;
        public Transform m_Xtarget;
        private Quaternion m_xangle;
        public Transform m_Ytarget;
        private Quaternion m_yangle;
        public Transform m_Ztarget;
        private Quaternion m_zangle;

        public Vector3 m_angleOffset;
        public bool m_useRigidbodyRotation;
        public float m_smoothFactor;

        public Quaternion m_finalRotation;

        public RotationTargeter(IntPtr ptr) : base(ptr) { }

        void Start()
        {
            m_smoothFactor = 10f;
        }

        public Quaternion GetRotation()
        {
            if (m_useRigidbodyRotation && m_rigidbody)
            {
                /*
                Vector3 facing = m_rigidbody.rotation * Vector3.forward;
                Vector3 velocity = m_rigidbody.velocity;

                // returns the angle difference relative to a third axis (e.g. straight up)
                float relativeAngleDifference = Vector3.SignedAngle(facing, velocity, Vector3.up);
                */

                return Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_rigidbody.velocity) * Quaternion.Euler(m_angleOffset), Time.deltaTime * m_smoothFactor);
            }
            else
            {
                //Quaternion quaternion = new Quaternion();
                //quaternion.SetEulerAngles(0, 0, 0);
                if (m_Xtarget)
                {
                    return Quaternion.Slerp(transform.rotation, m_Xtarget.rotation * Quaternion.Euler(m_angleOffset), Time.deltaTime * m_smoothFactor);
                }
                return new Quaternion(0,0,0,0);
            }
        }
    }
}
