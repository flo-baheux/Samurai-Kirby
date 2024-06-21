using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;

public class PlayerPrefsApplier : MonoBehaviour
{
  [SerializeField] private AudioMixer mixer;

  public static float defaultGlobalVolume = 1;
  public static float defaultBGMVolume = 1;
  public static float defaultSFXVolume = 1;
  public static int defaultLanguage = 1;
  public static int defaultFont = 1;
  public static int defaultFlashingScreenEnabled = 1;

  void Start()
  {
    if (PlayerPrefs.HasKey("GlobalVolume"))
      mixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("GlobalVolume")) * 20);
    else
      mixer.SetFloat("MasterVolume", Mathf.Log10(defaultGlobalVolume) * 20);

    if (PlayerPrefs.HasKey("BGMVolume"))
      mixer.SetFloat("BGMVolume", Mathf.Log10(PlayerPrefs.GetFloat("BGMVolume")) * 20);
    else
      mixer.SetFloat("BGMVolume", Mathf.Log10(defaultBGMVolume) * 20);


    if (PlayerPrefs.HasKey("SFXVolume"))
      mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume")) * 20);
    else
      mixer.SetFloat("SFXVolume", Mathf.Log10(defaultSFXVolume) * 20);
  }
}
