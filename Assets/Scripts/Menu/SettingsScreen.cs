using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
  [SerializeField] private GameObject startScreen;

  [SerializeField] private AudioMixer mixer;

  private Slider GlobalVolumeSlider;
  private Slider BGMVolumeSlider;
  private Slider SFXVolumeSlider;
  private TMP_Dropdown LanguageDropdown;
  private TMP_Dropdown FontDropdown;
  private Toggle FlashingScreenToggle;

  private void Awake()
  {
    transform.Find("ButtonCloseSettings").GetComponent<Button>().Select();
    GlobalVolumeSlider = transform.Find("Panel/Container/GlobalVolume/Slider").GetComponent<Slider>();
    BGMVolumeSlider = transform.Find("Panel/Container/BGMVolume/Slider").GetComponent<Slider>();
    SFXVolumeSlider = transform.Find("Panel/Container/SFXVolume/Slider").GetComponent<Slider>();
    LanguageDropdown = transform.Find("Panel/Container/LanguageSelect/Dropdown").GetComponent<TMP_Dropdown>();
    FontDropdown = transform.Find("Panel/Container/FontSelect/Dropdown").GetComponent<TMP_Dropdown>();
    FlashingScreenToggle = transform.Find("Panel/Container/FlashScreenEnabled/Toggle").GetComponent<Toggle>();
  }

  void Start()
  {
    GlobalVolumeSlider.value = PlayerPrefs.HasKey("GlobalVolume") ? PlayerPrefs.GetFloat("GlobalVolume") : PlayerPrefsApplier.defaultGlobalVolume;
    BGMVolumeSlider.value = PlayerPrefs.HasKey("BGMVolume") ? PlayerPrefs.GetFloat("BGMVolume") : PlayerPrefsApplier.defaultBGMVolume;
    SFXVolumeSlider.value = PlayerPrefs.HasKey("SFXVolume") ? PlayerPrefs.GetFloat("SFXVolume") : PlayerPrefsApplier.defaultSFXVolume;
    LanguageDropdown.value = PlayerPrefs.HasKey("Language") ? PlayerPrefs.GetInt("Language") : PlayerPrefsApplier.defaultLanguage;
    FontDropdown.value = PlayerPrefs.HasKey("Font") ? PlayerPrefs.GetInt("Font") : PlayerPrefsApplier.defaultFont;
    FlashingScreenToggle.isOn = PlayerPrefs.HasKey("FlashingEnabled") ? (PlayerPrefs.GetInt("FlashingEnabled") != 0) : PlayerPrefsApplier.defaultFlashingScreenEnabled != 0;
  }

  public void OnGlobalVolumeChanged()
  {
    PlayerPrefs.SetFloat("GlobalVolume", GlobalVolumeSlider.value);
    mixer.SetFloat("MasterVolume", Mathf.Log10(GlobalVolumeSlider.value) * 20);
  }


  public void OnBGMVolumeChanged()
  {
    PlayerPrefs.SetFloat("BGMVolume", BGMVolumeSlider.value);
    mixer.SetFloat("BGMVolume", Mathf.Log10(BGMVolumeSlider.value) * 20);
  }


  public void OnSFXVolumeChanged()
  {
    PlayerPrefs.SetFloat("SFXVolume", SFXVolumeSlider.value);
    mixer.SetFloat("SFXVolume", Mathf.Log10(SFXVolumeSlider.value) * 20);
  }

  public void OnLanguageDropdownValueChanged()
  {
    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LanguageDropdown.value];
    PlayerPrefs.SetInt("Language", LanguageDropdown.value);
  }

  public void OnFontDropdownValueChanged()
  {
    FontManager.instance.SetFontById(FontDropdown.value);
    PlayerPrefs.SetInt("Font", FontDropdown.value);

  }

  public void OnFlashScreenToggleChange()
  {
    PlayerPrefs.SetInt("FlashingEnabled", FlashingScreenToggle.isOn ? 1 : 0);
  }

  public void onClickCloseSettings()
  {
    startScreen.SetActive(true);
    gameObject.SetActive(false);
  }

  private void OnEnable()
  {
    transform.Find("ButtonCloseSettings").GetComponent<Button>().Select();
  }
}
