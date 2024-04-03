using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontManager : MonoBehaviour
{
  [SerializeField] private TMP_FontAsset fontKirby;
  [SerializeField] private TMP_FontAsset fontLiberation;

  private Dictionary<int, TMP_FontAsset> fontById;
  private TMP_FontAsset currentFont;

  public event Action FontChanged;

  internal static FontManager instance;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else if (instance != this)
    {
      Destroy(this);
    }
    DontDestroyOnLoad(gameObject);

    fontById = new Dictionary<int, TMP_FontAsset> {
      { 0, fontKirby },
      { 1, fontLiberation }
    };
  }

  void Start()
  {
    SetFontById(PlayerPrefs.HasKey("Font") ? PlayerPrefs.GetInt("Font") : 0);
  }

  public void SetFontById(int fontId)
  {
    currentFont = fontById.ContainsKey(fontId) ? fontById[fontId] : fontKirby;
    FontChanged?.Invoke();
  }

  public TMP_FontAsset getCurrentFont()
  {
    return currentFont;
  }
}
