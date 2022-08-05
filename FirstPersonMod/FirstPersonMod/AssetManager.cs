using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FirstPersonMod
{
    class AssetManager
    {
        private GameObject _menuGameObject;
        public GameObject instantiatedMenu;
        public MenuBuilder menuBuilder;

        public AssetManager()
        {
            AssetBundle modDataBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetFullPath("."), "UserData/firstpersonmod.bundle"));
            if (modDataBundle)
            {
                //MelonLogger.Msg("Asset bundle loaded.");
                var menuAsset = modDataBundle.LoadAsset("FirstPersonMod_Canvas");
                _menuGameObject = menuAsset.Cast<GameObject>();
                ModLogger.Log("MenuObject loaded.");
            }
        }

        public void CreateMenu()
        {
            instantiatedMenu = GameObject.Instantiate(_menuGameObject);
            menuBuilder = instantiatedMenu.AddComponent<MenuBuilder>();
            UnityEngine.Object.DontDestroyOnLoad(instantiatedMenu);
            instantiatedMenu.SetActive(false);
        }
    }
}
