using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstPersonMod
{
    class MenuBuilder : MonoBehaviour
    {
        public MenuBuilder(IntPtr ptr) : base(ptr) { }

        private GameObject fovParent;
        private Slider fovSlider;
        private TMP_InputField fovInputFieldTMPro;
        private Button fovResetButton;

        private GameObject angleOffsetParent;
        private Slider angleOffsetSlider;
        private TMP_InputField angleOffsetInputFieldTMPro;
        private Button angleOffsetResetButton;

        private GameObject activateParent;
        private Button activateButton;
        private TextMeshProUGUI activateButtonTMP;


        private void Start()
        {
            //FOV
            fovParent = gameObject.transform.Find("Fov_Parent").gameObject;
            fovSlider = fovParent.transform.Find("Slider").GetComponent<Slider>();
            fovInputFieldTMPro = fovParent.transform.Find("InputField").GetComponent<TMP_InputField>();
            fovResetButton = fovParent.transform.Find("ResetButton").gameObject.GetComponent<Button>();

            fovSlider.value = ModManager.m_fov;
            fovInputFieldTMPro.text = ModManager.m_fov.ToString("F2");

            fovSlider.onValueChanged.AddListener(new Action<float>(OnFovChange));
            fovInputFieldTMPro.onSubmit.AddListener(new Action<string>(OnFovSubmit));
            fovResetButton.onClick.AddListener(new Action(OnFovReset));

            //Angle Offset
            angleOffsetParent = gameObject.transform.Find("AngleOffset_Parent").gameObject;
            angleOffsetSlider = angleOffsetParent.transform.Find("Slider").GetComponent<Slider>();
            angleOffsetInputFieldTMPro = angleOffsetParent.transform.Find("InputField").GetComponent<TMP_InputField>();
            angleOffsetResetButton = angleOffsetParent.transform.Find("ResetButton").gameObject.GetComponent<Button>();

            angleOffsetSlider.value = ModManager.m_angleOffset;
            angleOffsetInputFieldTMPro.text = ModManager.m_angleOffset.ToString("F2");

            angleOffsetSlider.onValueChanged.AddListener(new Action<float>(OnAngleOffsetChange));
            angleOffsetInputFieldTMPro.onSubmit.AddListener(new Action<string>(OnAngleOffsetSubmit));
            angleOffsetResetButton.onClick.AddListener(new Action(OnAngleOffsetReset));

            //Activate button
            activateParent = gameObject.transform.Find("Activate_Parent").gameObject;
            activateButton = activateParent.transform.Find("Button").gameObject.GetComponent<Button>();
            activateButtonTMP = activateButton.GetComponentInChildren<TextMeshProUGUI>();
            activateButtonTMP.text = "ACTIVATE";

            activateButton.onClick.AddListener(new Action(OnActivateClick));

            ModLogger.Log("Menu builder initalized");
        }

        //FOV
        private void OnFovReset()
        {
            fovSlider.value = ModManager.m_fovPref.DefaultValue;
            fovInputFieldTMPro.text = ModManager.m_fovPref.DefaultValue.ToString();
            ModManager.SetFov(ModManager.m_fovPref.DefaultValue);
        }

        private void OnFovSubmit(string text)
        {
            fovSlider.value = float.Parse(text);
            ModManager.SetFov(fovSlider.value);
        }

        private void OnFovChange(float value)
        {
            fovInputFieldTMPro.text = value.ToString("F2");
            ModManager.SetFov(value);
        }

        //ANGLE OFFSET
        private void OnAngleOffsetReset()
        {
            angleOffsetSlider.value = ModManager.m_angleOffsetPref.DefaultValue;
            angleOffsetInputFieldTMPro.text = ModManager.m_angleOffsetPref.DefaultValue.ToString();
            ModManager.SetAngleOffset(ModManager.m_angleOffsetPref.DefaultValue);
        }

        private void OnAngleOffsetSubmit(string text)
        {
            angleOffsetSlider.value = float.Parse(text);
            ModManager.SetAngleOffset(angleOffsetSlider.value);
        }

        private void OnAngleOffsetChange(float value)
        {
            angleOffsetInputFieldTMPro.text = value.ToString("F2");
            ModManager.SetAngleOffset(value);
        }

        //ACTIVATE BUTTON
        private void OnActivateClick()
        {
            if (ModManager.m_modActivated)
            {
                ModManager.ActivateMod(false);
                activateButtonTMP.text = "ACTIVATE";
            }
            else
            {
                ModManager.ActivateMod(true);
                activateButtonTMP.text = "DEACTIVATE";
            }
        }
    }
}
