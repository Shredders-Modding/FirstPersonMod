using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinemachine;
using MelonLoader;
using UnityEngine;

namespace FirstPersonMod
{
    [RegisterTypeInIl2Cpp]
    public class CameraManager : MonoBehaviour
    {
        public CinemachineVirtualCamera m_camera;

        public Rigidbody m_snowboardRb;

        public Vector3 m_angleOffset = new Vector3(0,0,0); 
        public float m_fov = 100f; 

        public float m_tricksBlend = 0f;
        public float m_tricksBlendEnd = 0f;
        public float m_tricksBlendStart = 1f;
        private float m_blendTimeTarget = 1f;
        private float m_currentBlendTime = 100f;

        public PositionTargeter m_posTargeter;
        public RotationTargeter m_rideRotTargeter;
        public RotationTargeter m_tricksRotTargeter;

        public bool m_computeCamera;
        public bool m_computeLerpTime = false;

        private float m_currentTimerValue = 0;
        private float m_timerValue = 0.2f;
        private float m_previousSmoothValue = 0;
        private float m_nextSmoothValue = 0;
        private float m_currentSmoothValue = 0;

        private int m_bufferCounter = 0;
        private int m_bufferCounterTarget = 20;
        private float[] m_rotationFactorBuffer = new float[20];

        private bool m_isIdle = false;

        public CameraManager(IntPtr ptr) : base(ptr) { }

        void LateUpdate()
        {
            if (m_computeCamera)
            {
                //ModLogger.Log($"Forward vs direction = {Mathf.Abs(Vector3.Dot(m_snowboardRb.transform.forward, m_snowboardRb.velocity.normalized))}"); // < 0.9 pas dans le même sens
                //ModLogger.Log($"Up direction = {Vector3.Dot(m_snowboardRb.transform.up, Vector3.down)}"); // 1 = à l'envers
                //ModLogger.Log($"m_snowboardRb.angularVelocity.magnitude = {m_snowboardRb.angularVelocity.magnitude}");
                //ModLogger.Log($"m_smoothFactor = {m_tricksRotTargeter.m_smoothFactor}");
                UpdateCameraRotation();
                UpdateCameraBlending();

                if (m_snowboardRb.velocity.magnitude < 1.3f && !m_isIdle)
                {
                    m_isIdle = true;
                    BlendToTricks(0.3f);
                    ModLogger.Log("Idling");
                }
                else if (m_snowboardRb.velocity.magnitude >= 1.3f && m_isIdle)
                {
                    m_isIdle = false;
                    BlendToRide(1.5f);
                    ModLogger.Log("Not idling");
                }
            }
        }

        private void UpdateCameraRotation()
        {
            if (m_bufferCounter < m_bufferCounterTarget)
            {
                m_rotationFactorBuffer[m_bufferCounter] = (Mathf.Clamp(m_snowboardRb.angularVelocity.magnitude, 1, 10) - 1) / 9;
                ++m_bufferCounter;
            }
            else
                m_bufferCounter = 0;

            float totalRotationFactorValue = 0;
            for (int i = 0; i < m_bufferCounterTarget; i++)
            {
                totalRotationFactorValue += m_rotationFactorBuffer[i];
            }
            float meanRotationFactor = totalRotationFactorValue / m_bufferCounterTarget;
            m_currentSmoothValue = (meanRotationFactor * 34.5f) + 1.5f;
            m_tricksRotTargeter.m_smoothFactor = m_currentSmoothValue;

            if (m_computeLerpTime)
            {
                m_blendTimeTarget = Mathf.Lerp(2f, 0.2f, Mathf.Clamp(m_currentSmoothValue / 2.5f, 0, 1f));
                //ModLogger.Log($"Computed blendTimeTarget = {m_blendTimeTarget}");
            }

            Quaternion rideRot = m_rideRotTargeter.GetRotation();
            Quaternion tricksRot = m_tricksRotTargeter.GetRotation();
            transform.rotation = Quaternion.Slerp(rideRot, tricksRot, m_tricksBlend);
        }

        private void UpdateCameraBlending()
        {
            if (m_currentBlendTime < m_blendTimeTarget)
            {
                if (m_currentBlendTime >= 0)
                {
                    m_tricksBlend = Mathf.Lerp(m_tricksBlendStart, m_tricksBlendEnd, m_currentBlendTime / m_blendTimeTarget);
                }
                m_currentBlendTime += Time.deltaTime;
                //ModLogger.Log($"m_tricksBlend = {m_tricksBlend}");
            }
            else
            {
                m_computeLerpTime = false;
                m_tricksBlend = m_tricksBlendEnd;
                //ModLogger.Log($"m_tricksBlend = {m_tricksBlend}");
            }
        }

        public void SetComputeCamera(bool _activate)
        {
            m_computeCamera = _activate;
        }

        public void BlendToRide(float _lerpTime)
        {
            m_tricksBlendEnd = 0;
            m_tricksBlendStart = m_tricksBlend;

            m_currentBlendTime = 0;
            m_blendTimeTarget = _lerpTime;
            //ModLogger.Log($"Blend start with endValue = {m_tricksBlendEnd}");
        }

        public void BlendToRide(float _lerpTime, float _offest)
        {
            m_tricksBlendEnd = 0;
            m_tricksBlendStart = m_tricksBlend;

            m_currentBlendTime = 0 - _offest;
            m_blendTimeTarget = _lerpTime;
            //ModLogger.Log($"Blend start with endValue = {m_tricksBlendEnd}");
        }

        public void BlendToTricks(float _lerpTime)
        {
            m_tricksBlendEnd = 1f;
            m_tricksBlendStart = m_tricksBlend;

            m_currentBlendTime = 0;
            m_blendTimeTarget = _lerpTime;
            //ModLogger.Log($"Blend start with endValue = {m_tricksBlendEnd}");
        }

        public void BlendToTricks()
        {

            if (!m_computeLerpTime)
            {
                m_tricksBlendEnd = 1f;
                m_tricksBlendStart = m_tricksBlend;

                m_currentBlendTime = 0f;
                m_computeLerpTime = true;
            }
            //ModLogger.Log($"Blend start with endValue = {m_tricksBlendEnd}");
        }

        public void SetFov(float _fov)
        {
            m_fov = _fov;
            LensSettings lensSettings = m_camera.m_Lens;
            lensSettings.FieldOfView = m_fov;
            m_camera.m_Lens = lensSettings;
        }

        public void SetAngleOffset(float _offset)
        {
            m_rideRotTargeter.m_angleOffset = new Vector3(_offset, 0, 0);
        }
    }
}
