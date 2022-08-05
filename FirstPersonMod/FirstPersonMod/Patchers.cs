using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace FirstPersonMod
{
    [HarmonyPatch(typeof(Lirp.SnowboardController), "Show")]
    class SnowboardControllerPatcher_Show
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.SnowboardController __instance)
        {
            try
            {
                if (__instance == ModManager.m_userSession.sc)
                {
                    if (!ModManager.m_snowboardController)
                    {
                        ModManager.m_snowboardController = __instance;
                        ModManager.m_headNubT = __instance.gameObject.transform.Find("Root/characterbase(Clone)/Root/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Neck/Bip01 Head/Bip01 HeadNub");
                        ModLogger.Log("Head nub found");
                        ModManager.InitializedCameras();

                        List<string> headObjectsNameList = new List<string>() { "Goggles", "Hat", "Head", "Scarf" };
                        foreach (string name in headObjectsNameList)
                        {
                            GameObject objectToAdd = __instance.gameObject.transform.Find("Root/characterbase(Clone)/" + name).gameObject;
                            ModManager.m_headObjects.Add(objectToAdd);
                            ModLogger.Log($"GameObject found for : {objectToAdd.name}");
                        }

                        if (ModManager.m_modActivated)
                        {
                            ModManager.SetIsInAir(false);
                            ModManager.m_cameraManager.BlendToRide(2f);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
        }
    }

    [HarmonyPatch(typeof(Lirp.SnowboardController), "StartNewRide")]
    class StartRidePatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.SnowboardController __instance)
        {
            try
            {
                if (__instance == ModManager.m_snowboardController)
                {
                    if (ModManager.m_modActivated)
                    {
                        ModLogger.Log("StartingRide!");
                        ModManager.SetIsInAir(false);
                        ModManager.m_cameraManager.BlendToRide(0.2f);
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
        }
    }

    [HarmonyPatch(typeof(Lirp.ActionInAir), "OnStart")]
    class ActionInAirPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.ActionInAir __instance)
        {
            try
            {
                if (__instance.sc == ModManager.m_snowboardController)
                {
                    ModLogger.Log("Jumping!");
                    ModManager.SetIsInAir(true);
                    if (ModManager.m_modActivated)
                        ModManager.m_cameraManager.BlendToTricks();
                }
            }
            catch (System.Exception ex)
            {

            }
        }
    }

    [HarmonyPatch(typeof(Lirp.ActionTakeoff), "OnStart")]
    class ActionTakeoffPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.ActionTakeoff __instance)
        {
            try
            {
                if (__instance.physics.sc == ModManager.m_snowboardController)
                {
                    ModLogger.Log("Jumping!");
                    ModManager.SetIsInAir(true);
                    if (ModManager.m_modActivated)
                        ModManager.m_cameraManager.BlendToTricks();
                }
            }
            catch (System.Exception ex)
            {

            }
        }
    }

    [HarmonyPatch(typeof(Lirp.LandDetector), "StartLanding")]
    class OnLandPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.LandDetector __instance)
        {
            try
            {
                if (__instance.sc == ModManager.m_snowboardController)
                {
                    ModLogger.Log("Landing!");
                    ModManager.SetIsInAir(false);
                    if (ModManager.m_modActivated)
                        ModManager.m_cameraManager.BlendToRide(1.8f, 0.4f);
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }

    [HarmonyPatch(typeof(Lirp.ActionGrab), "OnStart")]
    class OnGrabPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.ActionGrab __instance)
        {
            try
            {
                if (__instance.sc == ModManager.m_snowboardController)
                {
                    ModLogger.Log("Grabing!");
                    ModManager.SetIsInAir(true);
                    if (ModManager.m_modActivated)
                        ModManager.m_cameraManager.BlendToTricks();
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }

    [HarmonyPatch(typeof(Lirp.ActionButter), "OnStart")]
    class OnButterPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.ActionButter __instance)
        {
            try
            {
                if (__instance.sc == ModManager.m_snowboardController)
                {
                    ModLogger.Log("Buttering!");
                    ModManager.SetIsInAir(true);
                    if (ModManager.m_modActivated)
                        ModManager.m_cameraManager.BlendToTricks(0.5f);
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }

    [HarmonyPatch(typeof(Lirp.ActionButter), "OnEnd")]
    class OnButterEndPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.ActionButter __instance)
        {
            try
            {
                if (__instance.sc == ModManager.m_snowboardController)
                {
                    ModLogger.Log("End Buttering!");
                    if (ModManager.m_modActivated && ModManager.m_isInAir)
                        ModManager.m_cameraManager.BlendToRide(1.8f, 0.4f);
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }

    [HarmonyPatch(typeof(Lirp.ActionRail), "OnStart")]
    class OnRailPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.ActionRail __instance)
        {
            try
            {
                if (__instance.rideHandler.sc == ModManager.m_snowboardController)
                {
                    ModLogger.Log("Railing!");
                    ModManager.SetIsInAir(true);
                    if (ModManager.m_modActivated)
                        ModManager.m_cameraManager.BlendToTricks();
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }

    [HarmonyPatch(typeof(Lirp.ActionCrash), "OnStart")]
    class OnCrashPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.ActionCrash __instance)
        {
            try
            {
                if (__instance.sc == ModManager.m_snowboardController)
                {
                    ModLogger.Log("Crashing!");
                    ModManager.SetIsInAir(true);
                    if (ModManager.m_modActivated)
                        ModManager.m_cameraManager.BlendToTricks(0.2f);
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }

    [HarmonyPatch(typeof(MenuViewStateSetter), "OnShowView")]
    class MenuViewOnShowPatcher
    {
        [HarmonyPrefix]
        public static void Prefix(System.Reflection.MethodBase __originalMethod, MenuViewStateSetter __instance)
        {
            try
            {
                if (ModManager.m_modActivated)
                {
                    ModManager.m_firstPersonCamera.gameObject.SetActive(false);
                    ModManager.m_cameraManager.SetComputeCamera(false);
                    ModLogger.Log("Is in menu");
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }

    [HarmonyPatch(typeof(MenuViewStateSetter), "OnHideView")]
    class MenuViewOnHidePatcher
    {
        [HarmonyPrefix]
        public static void Prefix(System.Reflection.MethodBase __originalMethod, MenuViewStateSetter __instance)
        {
            try
            {

                if (ModManager.m_modActivated)
                {
                    ModManager.m_firstPersonCamera.gameObject.SetActive(true);
                    ModManager.m_cameraManager.SetComputeCamera(true);
                    ModLogger.Log("Out of menu");
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }
}
