using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using Il2CppLirp;
using Il2CppCinemachine;
using Il2CppInterop.Runtime.Injection;

namespace FirstPersonMod
{
    public enum CameraType
    {
        Ride,
        Tricks,
        Idle,
        None,
    }

    public class ModManager : MelonMod
    {
        public static ModManager instance;
        private AssetManager m_assetManager;
        public static CameraManager m_cameraManager;
        public static bool m_isDebugActive;
        public static bool m_isInit;
        public static bool m_modActivated;

        public static UserSession m_userSession;
        public static SnowboardController m_snowboardController;
        public static Transform m_headNubT;
        public static List<GameObject> m_headObjects;
        public static Rigidbody m_snowboardRb;
        public static bool m_isInAir;

        public static CinemachineVirtualCamera m_firstPersonCamera;
        public static float m_fov;
        public static float m_angleOffset;

        public static MelonPreferences_Category m_firstPersonPrefCategory;
        public static MelonPreferences_Entry<string> m_fovPref;
        public static MelonPreferences_Entry<string> m_angleOffsetPref;

        public override void OnInitializeMelon()
        {
            ClassInjector.RegisterTypeInIl2Cpp<PositionTargeter>();
            ClassInjector.RegisterTypeInIl2Cpp<RotationTargeter>();
            ClassInjector.RegisterTypeInIl2Cpp<CameraManager>();
            ClassInjector.RegisterTypeInIl2Cpp<MenuBuilder>();

            m_isDebugActive = false;
            m_isInit = false;

            m_headObjects = new List<GameObject>();

            m_firstPersonPrefCategory = MelonPreferences.CreateCategory("firstPersonPrefCategory");
            m_fovPref = m_firstPersonPrefCategory.CreateEntry("fovPref", 90f.ToString("F2"));
            m_angleOffsetPref = m_firstPersonPrefCategory.CreateEntry("angleOffsetPref", 20f.ToString("F2"));

            m_fov = float.Parse(m_fovPref.Value);
            m_angleOffset = float.Parse(m_angleOffsetPref.Value);

            m_assetManager = new AssetManager();
    }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Loader")
            {
                m_assetManager.CreateMenu();
            }

            if (sceneName == "GameBase")
            {
                m_userSession = GameObject.Find("UserSession").GetComponent<UserSession>();
            }
        }

        public override void OnLateUpdate()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                if (m_assetManager.instantiatedMenu.active)
                    m_assetManager.instantiatedMenu.SetActive(false);
                else
                    m_assetManager.instantiatedMenu.SetActive(true);
            }
        }

        static void ComputeSpinning()
        {
            if (m_snowboardRb.angularVelocity.magnitude > 0.01f)
            {
               // ModLogger.Log($"Spinning!");
            }
        }

        public static void InitializedCameras()
        {
            GameObject parent = new GameObject("FirstPersonModParent");
            
            //RIDE CAMERA ROTATION AND POSITION
            GameObject firstPersonCam = new GameObject("FirstPersonCamera");
            firstPersonCam.transform.SetParent(parent.transform);

            PositionTargeter rideCamPosTargeter = firstPersonCam.AddComponent<PositionTargeter>();
            rideCamPosTargeter.m_Xtarget = m_headNubT;
            rideCamPosTargeter.m_Ytarget = m_headNubT;
            rideCamPosTargeter.m_Ztarget = m_headNubT;
            rideCamPosTargeter.m_posOffset = new Vector3(0, 0, 0);

            RotationTargeter rideCamRotTargeter = firstPersonCam.AddComponent<RotationTargeter>();
            m_snowboardRb = m_snowboardController.gameObject.GetComponent<Rigidbody>();
            rideCamRotTargeter.m_rigidbody = m_snowboardRb;
            rideCamRotTargeter.m_smoothFactor = 10;
            rideCamRotTargeter.m_useRigidbodyRotation = true;
            rideCamRotTargeter.m_angleOffset = new Vector3(m_angleOffset, 0, 0);

            //TRICKS CAMERA ROATION AND POSITION
            RotationTargeter tricksCamRotTargeter = firstPersonCam.AddComponent<RotationTargeter>();
            tricksCamRotTargeter.m_Xtarget = m_headNubT;
            tricksCamRotTargeter.m_Ytarget = m_headNubT;
            tricksCamRotTargeter.m_Ztarget = m_headNubT;
            tricksCamRotTargeter.m_smoothFactor = 40;
            tricksCamRotTargeter.m_useRigidbodyRotation = false;
            tricksCamRotTargeter.m_angleOffset = new Vector3(270f, 0, 90f);

            firstPersonCam.SetActive(false);
            m_firstPersonCamera = firstPersonCam.AddComponent<CinemachineVirtualCamera>();
            LensSettings lensSettings = new LensSettings();
            lensSettings.FieldOfView = m_fov;
            lensSettings.OrthographicSize = 10;
            lensSettings.NearClipPlane = 0.01f;
            lensSettings.FarClipPlane = 10000;
            m_firstPersonCamera.m_Lens = lensSettings;
            m_firstPersonCamera.m_Priority = 90;

            //CAMERA SETUP
            m_cameraManager = firstPersonCam.AddComponent<CameraManager>();
            m_cameraManager.m_camera = m_firstPersonCamera;
            m_cameraManager.m_snowboardRb = m_snowboardRb;
            m_cameraManager.m_posTargeter = rideCamPosTargeter;
            m_cameraManager.m_rideRotTargeter = rideCamRotTargeter;
            m_cameraManager.m_tricksRotTargeter = tricksCamRotTargeter;
            m_cameraManager.m_computeCamera = false;
            m_isInit = true;
            ModLogger.Log("First Person Mod initialized");
        }

        public static void ActivateMod(bool _activate)
        {
            if (m_isInit)
            {
                m_firstPersonCamera.gameObject.SetActive(_activate);
                ShowHead(!_activate);
                m_cameraManager.SetComputeCamera(_activate);
                m_modActivated = _activate;
            }
        }

        public static void ShowHead(bool _show)
        {
            if (m_headObjects.Count > 0)
            {
                foreach (GameObject gameObject in m_headObjects)
                {
                    gameObject.SetActive(_show);
                }
            }
        }

        public static void SetIsInAir(bool _isInAir)
        {
            m_isInAir = _isInAir;
            /*
            if (!m_isInAir)
                SetActiveCamera(CameraType.Ride);*/
        }

        public static void SetFov(float _fov)
        {
            m_fov = _fov;
            m_fovPref.Value = _fov.ToString("F2");
            if (m_isInit)
            {
                m_cameraManager.SetFov(_fov);
            }
        }

        public static void SetAngleOffset(float _offset)
        {
            m_angleOffset = _offset;
            m_angleOffsetPref.Value = _offset.ToString("F2");
            if (m_isInit)
            {
                m_cameraManager.SetAngleOffset(_offset);
            }
        }
    }
}
