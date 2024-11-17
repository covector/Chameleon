 using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static Utils;

public class PlayerOptions : MonoBehaviour
{
    public static PlayerOptions instance;
    private void Awake()
    {
        FetchPlayerPrefs();
        instance = this;
    }

    public bool canEdit { get; private set; }
    public void Open()
    {
        GetComponent<Canvas>().enabled = true;
        canEdit = true;
    }
    public void Close()
    {
        GetComponent<Canvas>().enabled = false;
        canEdit = false;
    }

    public float Sensitivity { get; private set; }
    public float Volume { get; private set; }
    public float Brightness { get; private set; }
    public Dictionary<string, KeyCode> KeyBinds { get; private set; }

    private KeyCode FetchKeyCodePref(string key, KeyCode fallback)
    {
        return TryParseKey(PlayerPrefs.GetString(key, ""), fallback);
    }
    private void SetKeyCodePrefSafe(string key, KeyCode fallback)
    {
        PlayerPrefs.SetString(key, TryParseKey(key, fallback).ToString());
    }
    private float FetchFloatPref(string key, float min, float max, float fallback)
    {
        return Mathf.Clamp(PlayerPrefs.GetFloat(key, fallback), min, max);
    }

    public Slider sliderSensitivity;
    public Slider sliderVolume;
    public Slider sliderBrightness;
    public KeybindOption keybindPickUp;
    public KeybindOption keybindScanner;

    private void FetchPlayerPrefs()
    {
        Sensitivity = FetchFloatPref("Sensitivity", 0f, 2f, 1f);
        sliderSensitivity.value = MapValues(Sensitivity, 0f, 2f, 0f, 1f);
        Volume = FetchFloatPref("Volume", 0f, 2f, 1f);
        sliderVolume.value = MapValues(Volume, 0f, 2f, 0f, 1f);
        UpdateVolume();
        Brightness = FetchFloatPref("Brightness", -0.5f, 1f, 0f);
        sliderBrightness.value = MapValues(Brightness, -0.5f, 1f, 0f, 1f);
        UpdateBrightness();
        KeyBinds = new Dictionary<string, KeyCode>();
        KeyBinds["PickUp"] = FetchKeyCodePref("PickUp", KeyCode.E);
        keybindPickUp.SetUp((KeyCode key) => {
            if (canEdit) {
                KeyBinds["PickUp"] = key;
                PlayerPrefs.SetString("PickUp", key.ToString());
            }
            Debug.Log("Modified PickUp");
        }, KeyBinds["PickUp"]);
        KeyBinds["Scanner"] = FetchKeyCodePref("Scanner", KeyCode.Q);
        keybindScanner.SetUp((KeyCode key) => {
            if (canEdit)
            {
                KeyBinds["Scanner"] = key;
                PlayerPrefs.SetString("Scanner", key.ToString());
            }
            Debug.Log("Modified Scanner");
        }, KeyBinds["Scanner"]);
    }

    public void SetSensitivity(float sliderValue)
    {
        if (canEdit)
        {
            float value = MapValues(sliderValue, 0f, 1f, 0f, 2f);
            PlayerPrefs.SetFloat("Sensitivity", value);
            Sensitivity = value;
        }
    }

    public void SetVolume(float sliderValue)
    {
        if (canEdit)
        {
            float value = MapValues(sliderValue, 0f, 1f, 0f, 2f);
            PlayerPrefs.SetFloat("Volume", value);
            Volume = value;
            UpdateVolume();
        }
    }

    public void SetBrightness(float sliderValue)
    {
        if (canEdit)
        {
            float value = MapValues(sliderValue, 0f, 1f, -0.5f, 1f);
            PlayerPrefs.SetFloat("Brightness", value);
            Brightness = value;
            UpdateBrightness();
        }
    }

    public ConfirmBox confirm;
    public void PromptResetAll()
    {
        confirm.Show("Reset?", (bool yes) => {
            if (yes)
            {
                PlayerPrefs.SetFloat("Sensitivity", 1f);
                PlayerPrefs.SetFloat("Volume", 1f);
                PlayerPrefs.SetFloat("Brightness", 0f);
                PlayerPrefs.SetString("PickUp", "E");
                PlayerPrefs.SetString("Scanner", "Q");
                FetchPlayerPrefs();
            }
        });
    }

    public void UpdateVolume()
    {
        AudioListener.volume = Volume;
    }

    public void UpdateBrightness()
    {
        LiftGammaGain liftGammaGain;
        FindFirstObjectByType<Volume>().profile.TryGet(out liftGammaGain);
        if (liftGammaGain != null)
        {
            liftGammaGain.gamma.Override(Vector4.one * Brightness);
        }
    }
}
